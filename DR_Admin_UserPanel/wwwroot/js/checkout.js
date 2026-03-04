"use strict";
function getMarkerOrderIds(marker) {
    const ids = Array.isArray(marker.orderIds) && marker.orderIds.length > 0
        ? marker.orderIds
        : [marker.orderId];
    return ids.filter((id) => Number.isInteger(id) && id > 0);
}
function getMarkerPrimaryOrderId(marker) {
    const ids = getMarkerOrderIds(marker);
    return ids.length > 0 ? ids[0] : marker.orderId;
}
function getRecurringModeIntervalDays(mode) {
    const normalized = (mode ?? '').trim().toLowerCase();
    const monthMatch = normalized.match(/(\d+)\s*(month|months|mnd|m|mo)/);
    if (monthMatch && monthMatch[1]) {
        return Math.max(1, Number.parseInt(monthMatch[1], 10)) * 30;
    }
    const yearMatch = normalized.match(/(\d+)\s*(year|years|yr|y|annual|annually)/);
    if (yearMatch && yearMatch[1]) {
        return Math.max(1, Number.parseInt(yearMatch[1], 10)) * 365;
    }
    if (normalized.includes('year') || normalized.includes('annual') || normalized.includes('yearly')) {
        return 365;
    }
    if (normalized.includes('quarter')) {
        return 90;
    }
    if (normalized.includes('week')) {
        return 7;
    }
    return 30;
}
const checkoutOrderMarkerStorageKey = 'up_checkout_last_added_order';
const checkoutDomainSearchPath = '/shop/domain-search';
let stripeInstance = null;
let stripeCardElement = null;
let stripeClientSecret = null;
let stripePaymentIntentId = null;
function initializeCheckout() {
    const page = document.getElementById('checkout-page');
    if (!page || page.dataset.bound === 'true') {
        return;
    }
    page.dataset.bound = 'true';
    initializePaymentInstrumentPanel();
    renderSummary();
    void loadLoggedInCustomer();
    restoreOrderMarkerForCurrentCart();
    renderPaymentStatusFromQuery();
    const form = document.getElementById('checkout-form');
    form?.addEventListener('submit', async (event) => {
        event.preventDefault();
        await submitCheckout();
    });
    bindDeleteOrderActions();
    updateCheckoutDeleteOrderVisibility();
}
function bindDeleteOrderActions() {
    const confirmButton = document.getElementById('checkout-delete-order-confirm');
    if (!confirmButton || confirmButton.dataset.bound === 'true') {
        return;
    }
    confirmButton.dataset.bound = 'true';
    confirmButton.addEventListener('click', () => {
        void deletePendingOrder();
    });
}
function updateCheckoutDeleteOrderVisibility() {
    const button = document.getElementById('checkout-delete-order-open');
    if (!button) {
        return;
    }
    const marker = getStoredOrderMarker();
    const typedWindow = window;
    const state = typedWindow.UserPanelCart?.getState();
    const hasCartLines = !!state && (state.domain !== null ||
        state.hosting.length > 0 ||
        state.services.length > 0);
    const canDelete = (marker?.orderId ?? 0) > 0 || hasCartLines;
    button.classList.toggle('d-none', !canDelete);
}
function initializePaymentInstrumentPanel() {
    const instrumentSelect = document.getElementById('checkout-payment-instrument');
    instrumentSelect?.addEventListener('change', () => {
        renderPaymentInstrumentFields();
        updateCheckoutContinueButtonVisibility();
        if (isCreditCardInstrumentCode(instrumentSelect.value)) {
            void continueToPayment();
            return;
        }
        hideStripePaymentForm();
    });
    const continueButton = document.getElementById('checkout-payment-instrument-submit');
    continueButton?.addEventListener('click', () => {
        void continueToPayment();
    });
    const addButton = document.getElementById('checkout-payment-methods-open-create');
    addButton?.addEventListener('click', () => {
        void prepareCheckoutPaymentMethodModal();
    });
    const addForm = document.getElementById('checkout-payment-methods-create-form');
    addForm?.addEventListener('submit', (event) => {
        event.preventDefault();
        void saveCheckoutPaymentMethod();
    });
}
function isCreditCardInstrumentCode(code) {
    const selected = (code ?? '').trim().toLowerCase();
    return selected === 'creditcard' || selected === 'credit card' || selected === 'card';
}
function updateCheckoutContinueButtonVisibility() {
    const select = document.getElementById('checkout-payment-instrument');
    const continueButton = document.getElementById('checkout-payment-instrument-submit');
    if (!select || !continueButton) {
        return;
    }
    continueButton.classList.toggle('d-none', isCreditCardInstrumentCode(select.value));
}
async function prepareCheckoutPaymentMethodModal() {
    const typedWindow = window;
    const select = document.getElementById('checkout-payment-methods-instrument');
    if (!select) {
        return;
    }
    const response = await typedWindow.UserPanelApi?.request('/PaymentInstruments/active', {
        method: 'GET'
    }, true);
    const options = (response?.success && response.data ? response.data : [])
        .filter((item) => item.isActive)
        .sort((a, b) => (a.displayOrder - b.displayOrder) || a.name.localeCompare(b.name));
    if (options.length === 0) {
        select.innerHTML = '<option value="">No active instruments</option>';
        return;
    }
    select.innerHTML = options
        .map((item) => `<option value="${escapeHtmlCheckout(item.code)}">${escapeHtmlCheckout(item.name)}</option>`)
        .join('');
}
function mapInstrumentCodeToPaymentMethodType(code) {
    const normalized = code.trim().toLowerCase();
    if (normalized === 'creditcard' || normalized === 'credit card' || normalized === 'card') {
        return 0;
    }
    if (normalized === 'bankaccount' || normalized === 'bank account') {
        return 1;
    }
    if (normalized === 'paypal') {
        return 2;
    }
    if (normalized === 'cash') {
        return 3;
    }
    if (normalized === 'invoice') {
        return 4;
    }
    return 99;
}
async function saveCheckoutPaymentMethod() {
    const typedWindow = window;
    const instrument = document.getElementById('checkout-payment-methods-instrument')?.value.trim() ?? '';
    const isDefault = document.getElementById('checkout-payment-methods-default')?.checked ?? true;
    if (!instrument) {
        typedWindow.UserPanelAlerts?.showError('checkout-alert-error', 'Payment instrument is required.');
        return;
    }
    const response = await typedWindow.UserPanelApi?.request('/CustomerPaymentMethods/mine', {
        method: 'POST',
        body: JSON.stringify({
            paymentInstrument: instrument,
            type: mapInstrumentCodeToPaymentMethodType(instrument),
            paymentMethodToken: '',
            isDefault
        })
    }, true);
    if (!response || !response.success) {
        typedWindow.UserPanelAlerts?.showError('checkout-alert-error', response?.message ?? 'Could not add payment instrument.');
        return;
    }
    const modalElement = document.getElementById('checkout-payment-methods-modal');
    if (modalElement) {
        typedWindow.bootstrap?.Modal?.getInstance(modalElement)?.hide();
    }
    typedWindow.UserPanelAlerts?.showSuccess('checkout-alert-success', 'Payment instrument added.');
    await loadPaymentInstruments();
}
function closeCheckoutDeleteModal() {
    const typedWindow = window;
    const modalElement = document.getElementById('checkout-delete-order-modal');
    if (!modalElement) {
        return;
    }
    const instance = typedWindow.bootstrap?.Modal?.getInstance(modalElement);
    instance?.hide();
}
function clearCheckoutSessionState() {
    const typedWindow = window;
    typedWindow.UserPanelCart?.clear();
    sessionStorage.removeItem(checkoutOrderMarkerStorageKey);
    const form = document.getElementById('checkout-form');
    if (form) {
        form.dataset.orderAdded = 'false';
    }
    const submitButton = document.getElementById('checkout-submit');
    if (submitButton) {
        submitButton.dataset.submitting = 'false';
    }
    setCheckoutSubmitDisabled(false);
    const paymentCard = document.getElementById('checkout-payment-instrument-card');
    paymentCard?.classList.add('d-none');
    const orderNumberEl = document.getElementById('checkout-added-order-number');
    if (orderNumberEl) {
        orderNumberEl.textContent = '-';
    }
    const ordersList = document.getElementById('checkout-added-orders-list');
    if (ordersList) {
        ordersList.innerHTML = '';
    }
    const paymentStatus = document.getElementById('checkout-payment-status');
    if (paymentStatus) {
        paymentStatus.textContent = 'Order removed from checkout.';
    }
    hideStripePaymentForm();
    renderSummary();
}
function setCheckoutSubmitDisabled(disabled) {
    const submitButton = document.getElementById('checkout-submit');
    if (submitButton) {
        submitButton.disabled = disabled;
    }
    const confirmInput = document.getElementById('checkout-confirm');
    if (confirmInput) {
        confirmInput.disabled = disabled;
    }
}
function redirectToDomainSearch(delayMs = 1200) {
    window.setTimeout(() => {
        window.location.href = checkoutDomainSearchPath;
    }, delayMs);
}
async function deletePendingOrder() {
    const typedWindow = window;
    typedWindow.UserPanelAlerts?.hide('checkout-alert-success');
    typedWindow.UserPanelAlerts?.hide('checkout-alert-error');
    const marker = getStoredOrderMarker();
    if (!marker || marker.orderId <= 0) {
        clearCheckoutSessionState();
        closeCheckoutDeleteModal();
        typedWindow.UserPanelAlerts?.showSuccess('checkout-alert-success', 'Local checkout data removed.');
        redirectToDomainSearch(1000);
        return;
    }
    const orderIds = getMarkerOrderIds(marker);
    for (const orderId of orderIds) {
        const response = await typedWindow.UserPanelApi?.request(`/Orders/checkout/${orderId}/cancel`, {
            method: 'POST'
        }, true);
        if (!response || !response.success) {
            if (response?.statusCode === 409) {
                typedWindow.UserPanelAlerts?.showError('checkout-alert-error', response.message ?? 'Order is already paid and cannot be deleted.');
                closeCheckoutDeleteModal();
                return;
            }
            typedWindow.UserPanelAlerts?.showError('checkout-alert-error', response?.message ?? 'Could not delete order.');
            closeCheckoutDeleteModal();
            return;
        }
    }
    clearCheckoutSessionState();
    closeCheckoutDeleteModal();
    typedWindow.UserPanelAlerts?.showSuccess('checkout-alert-success', 'Order cancelled and removed from your session.');
    redirectToDomainSearch(1200);
}
function renderSummary() {
    const typedWindow = window;
    const state = typedWindow.UserPanelCart?.getState();
    const container = document.getElementById('checkout-summary-lines');
    if (!container || !state) {
        return;
    }
    let oneTimeTotal = 0;
    let recurringTotal = 0;
    const lines = [];
    if (state.domain) {
        const domainIsRecurring = state.domain.isRecurring === true;
        if (domainIsRecurring) {
            recurringTotal += state.domain.premiumPrice;
        }
        else {
            oneTimeTotal += state.domain.premiumPrice;
        }
        const periodLabel = typeof state.domain.periodYears === 'number' && state.domain.periodYears > 0
            ? ` (${state.domain.periodYears} year${state.domain.periodYears > 1 ? 's' : ''})`
            : '';
        const modeLabel = domainIsRecurring ? 'recurring' : 'one-time';
        lines.push(`<div class="d-flex justify-content-between"><span>Domain: ${escapeHtmlCheckout(state.domain.domainName)}${periodLabel} (${modeLabel})</span><span>${state.domain.premiumPrice.toFixed(2)}</span></div>`);
        if (state.domain.includePrivacy) {
            const privacyAmount = typeof state.domain.privacyPriceTotal === 'number' ? state.domain.privacyPriceTotal : 0;
            if (domainIsRecurring) {
                recurringTotal += privacyAmount;
            }
            else {
                oneTimeTotal += privacyAmount;
            }
            lines.push(`<div class="d-flex justify-content-between"><span>WHOIS Privacy${domainIsRecurring ? ' (recurring)' : ''}</span><span>${privacyAmount.toFixed(2)}</span></div>`);
        }
    }
    state.hosting.forEach((item) => {
        const amount = item.billingCycle === 'yearly' ? item.yearlyPrice : item.monthlyPrice;
        recurringTotal += amount;
        const hostingName = item.name?.trim() ? item.name : `Hosting #${item.id}`;
        lines.push(`<div class="d-flex justify-content-between"><span>Hosting: ${escapeHtmlCheckout(hostingName)} (${item.billingCycle})</span><span>${amount.toFixed(2)}</span></div>`);
    });
    state.services.forEach((item) => {
        recurringTotal += item.price;
        const serviceName = item.name?.trim() ? item.name : `Service #${item.id}`;
        lines.push(`<div class="d-flex justify-content-between"><span>Service: ${escapeHtmlCheckout(serviceName)}</span><span>${item.price.toFixed(2)}</span></div>`);
    });
    lines.push('<hr/>');
    lines.push(`<div class="d-flex justify-content-between"><span>Discount</span><span>- ${state.discount.toFixed(2)}</span></div>`);
    lines.push(`<div class="d-flex justify-content-between fw-semibold"><span>Total now</span><span>${Math.max(0, oneTimeTotal + recurringTotal - state.discount).toFixed(2)}</span></div>`);
    container.innerHTML = lines.join('');
}
async function loadLoggedInCustomer() {
    const typedWindow = window;
    const customerIdEl = document.getElementById('checkout-customer-id');
    const customerIdDisplay = document.getElementById('checkout-customer-id-display');
    const customerName = document.getElementById('checkout-customer-name');
    const customerEmail = document.getElementById('checkout-customer-email');
    if (!customerIdEl || !customerIdDisplay || !customerName || !customerEmail) {
        return;
    }
    const response = await typedWindow.UserPanelApi?.request('/MyAccount/me', {
        method: 'GET'
    }, true);
    if (!response || !response.success || !response.data) {
        customerIdEl.value = '';
        customerIdDisplay.textContent = '-';
        customerName.textContent = '-';
        customerEmail.textContent = '-';
        return;
    }
    const customerId = response.data.customer?.id ?? 0;
    const customerReference = response.data.customer?.referenceNumber;
    customerIdEl.value = customerId > 0 ? customerId.toString() : '';
    customerIdDisplay.textContent = typeof customerReference === 'number' && customerReference > 0
        ? `REF${customerReference}`
        : '-';
    customerName.textContent = response.data.customer?.name ?? response.data.username;
    customerEmail.textContent = response.data.customer?.email ?? response.data.email;
}
function escapeHtmlCheckout(value) {
    return value
        .replace(/&/g, '&amp;')
        .replace(/</g, '&lt;')
        .replace(/>/g, '&gt;')
        .replace(/"/g, '&quot;')
        .replace(/'/g, '&#39;');
}
function getCartSignature(state) {
    return JSON.stringify(state);
}
function getStoredOrderMarker() {
    try {
        const raw = sessionStorage.getItem(checkoutOrderMarkerStorageKey);
        if (!raw) {
            return null;
        }
        return JSON.parse(raw);
    }
    catch {
        return null;
    }
}
function saveOrderMarker(marker) {
    sessionStorage.setItem(checkoutOrderMarkerStorageKey, JSON.stringify(marker));
}
function restoreOrderMarkerForCurrentCart() {
    const typedWindow = window;
    const state = typedWindow.UserPanelCart?.getState();
    if (!state) {
        return;
    }
    const marker = getStoredOrderMarker();
    if (!marker || marker.cartSignature !== getCartSignature(state)) {
        return;
    }
    const markerIds = getMarkerOrderIds(marker);
    const markerNumbers = (Array.isArray(marker.orderNumbers) && marker.orderNumbers.length > 0)
        ? marker.orderNumbers
        : [marker.orderNumber];
    const restoredOrders = markerIds.map((id, index) => ({
        id,
        orderNumber: markerNumbers[index] ?? marker.orderNumber,
        recurringMode: 'restored',
        isRecurring: false
    }));
    markOrderAsAdded(restoredOrders, false);
}
function markOrderAsAdded(orders, persist) {
    const typedWindow = window;
    const state = typedWindow.UserPanelCart?.getState();
    if (!state || orders.length === 0) {
        return;
    }
    const sortedOrders = [...orders].sort((a, b) => {
        if (a.isRecurring !== b.isRecurring) {
            return a.isRecurring ? 1 : -1;
        }
        const dayDiff = getRecurringModeIntervalDays(a.recurringMode) - getRecurringModeIntervalDays(b.recurringMode);
        if (dayDiff !== 0) {
            return dayDiff;
        }
        return a.orderNumber.localeCompare(b.orderNumber);
    });
    const primary = sortedOrders[0];
    const orderIds = sortedOrders.map((item) => item.id);
    const orderNumbers = sortedOrders.map((item) => item.orderNumber);
    if (persist) {
        saveOrderMarker({
            cartSignature: getCartSignature(state),
            orderId: primary.id,
            orderNumber: primary.orderNumber,
            orderIds,
            orderNumbers,
            createdAt: new Date().toISOString()
        });
    }
    const submitButton = document.getElementById('checkout-submit');
    if (submitButton) {
        submitButton.dataset.submitting = 'false';
    }
    setCheckoutSubmitDisabled(true);
    const form = document.getElementById('checkout-form');
    if (form) {
        form.dataset.orderAdded = 'true';
    }
    const orderNumberEl = document.getElementById('checkout-added-order-number');
    if (orderNumberEl) {
        orderNumberEl.textContent = orderNumbers.join(', ');
    }
    const ordersList = document.getElementById('checkout-added-orders-list');
    if (ordersList) {
        ordersList.innerHTML = sortedOrders
            .map((item) => {
            const typeLabel = item.isRecurring
                ? `Recurring (${item.recurringMode})`
                : 'One-time';
            return `<div>${escapeHtmlCheckout(item.orderNumber)} - ${escapeHtmlCheckout(typeLabel)}</div>`;
        })
            .join('');
    }
    const paymentCard = document.getElementById('checkout-payment-instrument-card');
    if (paymentCard) {
        paymentCard.classList.remove('d-none');
    }
    void loadPaymentInstruments();
    updateCheckoutDeleteOrderVisibility();
}
async function loadPaymentInstruments() {
    const typedWindow = window;
    const select = document.getElementById('checkout-payment-instrument');
    if (!select) {
        return;
    }
    const methodsResponse = await typedWindow.UserPanelApi?.request('/CustomerPaymentMethods/mine', {
        method: 'GET'
    }, true);
    const allowedInstrumentCodes = new Set();
    if (methodsResponse?.success && methodsResponse.data) {
        methodsResponse.data
            .filter((item) => item.isActive)
            .forEach((item) => {
            const code = mapPaymentMethodTypeToInstrumentCode(item.type);
            if (code) {
                allowedInstrumentCodes.add(code.toLowerCase());
            }
        });
    }
    if (allowedInstrumentCodes.size === 0) {
        select.innerHTML = '<option value="">No active payment methods available</option>';
        renderPaymentInstrumentFields();
        await prepareCheckoutPaymentMethodModal();
        const modalElement = document.getElementById('checkout-payment-methods-modal');
        if (modalElement) {
            typedWindow.bootstrap?.Modal?.getOrCreateInstance(modalElement)?.show();
        }
        return;
    }
    const response = await typedWindow.UserPanelApi?.request('/PaymentInstruments/active', {
        method: 'GET'
    }, true);
    if (!response || !response.success || !response.data || response.data.length === 0) {
        select.innerHTML = '<option value="">No active instruments</option>';
        renderPaymentInstrumentFields();
        return;
    }
    const items = response.data
        .filter((item) => item.isActive)
        .filter((item) => allowedInstrumentCodes.has(item.code.toLowerCase()))
        .sort((a, b) => (a.displayOrder - b.displayOrder) || a.name.localeCompare(b.name));
    if (items.length === 0) {
        select.innerHTML = '<option value="">No matching active instruments</option>';
        renderPaymentInstrumentFields();
        updateCheckoutContinueButtonVisibility();
        return;
    }
    select.innerHTML = items
        .map((item) => `<option value="${escapeHtmlCheckout(item.code)}">${escapeHtmlCheckout(item.name)}</option>`)
        .join('');
    renderPaymentInstrumentFields();
    updateCheckoutContinueButtonVisibility();
    if (isCreditCardInstrumentCode(select.value)) {
        void continueToPayment();
    }
}
async function resolveCheckoutCustomerId() {
    const customerIdInput = document.getElementById('checkout-customer-id');
    const parsed = Number.parseInt(customerIdInput?.value ?? '', 10);
    if (!Number.isNaN(parsed) && parsed > 0) {
        return parsed;
    }
    const typedWindow = window;
    const response = await typedWindow.UserPanelApi?.request('/MyAccount/me', {
        method: 'GET'
    }, true);
    const customerId = response?.data?.customer?.id ?? 0;
    if (customerIdInput && customerId > 0) {
        customerIdInput.value = customerId.toString();
    }
    return customerId > 0 ? customerId : null;
}
function mapPaymentMethodTypeToInstrumentCode(type) {
    if (type === 0 || type === '0') {
        return 'CreditCard';
    }
    if (type === 1 || type === '1') {
        return 'BankAccount';
    }
    if (type === 2 || type === '2') {
        return 'PayPal';
    }
    if (type === 3 || type === '3') {
        return 'Cash';
    }
    if (type === 4 || type === '4') {
        return 'Invoice';
    }
    return 'Other';
}
function getInstrumentFieldDefinitions(instrumentCode) {
    const normalized = instrumentCode.trim().toLowerCase();
    if (normalized === 'creditcard' || normalized === 'credit card' || normalized === 'card') {
        return [];
    }
    if (normalized === 'bankaccount' || normalized === 'bank account') {
        return [
            { name: 'accountHolderName', label: 'Account holder name', type: 'text', required: true },
            { name: 'bankName', label: 'Bank name', type: 'text', required: true },
            { name: 'accountNumber', label: 'Account number / IBAN', type: 'text', required: true },
            { name: 'routingNumber', label: 'Routing / SWIFT', type: 'text', required: true }
        ];
    }
    if (normalized === 'paypal') {
        return [
            { name: 'paypalEmail', label: 'PayPal email', type: 'email', required: true }
        ];
    }
    if (normalized === 'cash') {
        return [
            { name: 'payerName', label: 'Payer name', type: 'text', required: true }
        ];
    }
    return [
        { name: 'paymentReference', label: 'Payment reference', type: 'text', required: true },
        { name: 'details', label: 'Details', type: 'text', required: false }
    ];
}
function renderPaymentInstrumentFields() {
    const select = document.getElementById('checkout-payment-instrument');
    const fieldsContainer = document.getElementById('checkout-payment-instrument-fields');
    if (!select || !fieldsContainer) {
        return;
    }
    const selectedCode = select.value;
    const fields = getInstrumentFieldDefinitions(selectedCode);
    if (fields.length === 0) {
        fieldsContainer.innerHTML = '';
        return;
    }
    fieldsContainer.innerHTML = fields.map((field) => `
        <div class="col-12 col-md-6">
            <label for="checkout-pi-${escapeHtmlCheckout(field.name)}" class="form-label">${escapeHtmlCheckout(field.label)}</label>
            <input id="checkout-pi-${escapeHtmlCheckout(field.name)}" data-payment-field="${escapeHtmlCheckout(field.name)}" class="form-control" type="${field.type}" ${field.required ? 'required' : ''} />
        </div>
    `).join('');
}
async function continueToPayment() {
    const typedWindow = window;
    const select = document.getElementById('checkout-payment-instrument');
    const fieldsContainer = document.getElementById('checkout-payment-instrument-fields');
    if (!select || !fieldsContainer) {
        return;
    }
    const requiredFields = Array.from(fieldsContainer.querySelectorAll('input[required]'));
    const missing = requiredFields.some((input) => !input.value.trim());
    if (missing) {
        typedWindow.UserPanelAlerts?.showError('checkout-alert-error', 'Please fill all required payment instrument fields.');
        return;
    }
    const status = document.getElementById('checkout-payment-status');
    if (status) {
        status.textContent = `Order added. Payment instrument selected: ${select.options[select.selectedIndex]?.text ?? select.value}.`;
    }
    const marker = getStoredOrderMarker();
    if (!marker || marker.orderId <= 0) {
        typedWindow.UserPanelAlerts?.showError('checkout-alert-error', 'Order marker could not be resolved. Please place order again.');
        return;
    }
    const primaryOrderId = getMarkerPrimaryOrderId(marker);
    const markerOrderLabel = (Array.isArray(marker.orderNumbers) && marker.orderNumbers.length > 0)
        ? marker.orderNumbers.join(', ')
        : marker.orderNumber;
    const state = typedWindow.UserPanelCart?.getState();
    if (!state) {
        typedWindow.UserPanelAlerts?.showError('checkout-alert-error', 'Cart state could not be resolved.');
        return;
    }
    const totalAmount = calculateCheckoutTotal(state);
    const baseUrl = window.location.origin;
    const selectedInstrument = select.value.trim();
    const response = await typedWindow.UserPanelApi?.request('/PaymentIntents', {
        method: 'POST',
        body: JSON.stringify({
            orderId: primaryOrderId,
            amount: totalAmount,
            currency: 'EUR',
            paymentGatewayId: 0,
            paymentInstrument: selectedInstrument,
            returnUrl: `${baseUrl}/shop/checkout?paymentStatus=success`,
            cancelUrl: `${baseUrl}/shop/checkout?paymentStatus=failed`,
            description: `Checkout payment for order ${markerOrderLabel}`
        })
    }, true);
    if (!response || !response.success || !response.data) {
        typedWindow.UserPanelAlerts?.showError('checkout-alert-error', response?.message ?? 'Could not initialize payment intent.');
        return;
    }
    if (status) {
        status.textContent = `Payment initialized. Intent #${response.data.id} (${response.data.status}).`;
    }
    stripePaymentIntentId = response.data.id;
    const provider = (response.data.paymentGatewayProviderCode ?? '').trim().toLowerCase();
    if (provider === 'stripe') {
        const prepared = prepareStripePaymentForm(response.data);
        if (!prepared) {
            typedWindow.UserPanelAlerts?.showError('checkout-alert-error', 'Stripe payment form could not be initialized.');
            return;
        }
        typedWindow.UserPanelAlerts?.showSuccess('checkout-alert-success', 'Stripe form is ready. Complete card payment below.');
        return;
    }
    hideStripePaymentForm();
    typedWindow.UserPanelAlerts?.showSuccess('checkout-alert-success', 'Payment intent created. Continue with provider confirmation flow.');
}
function prepareStripePaymentForm(intent) {
    const typedWindow = window;
    const publishableKey = (intent.paymentGatewayPublicKey ?? '').trim();
    const clientSecret = (intent.clientSecret ?? '').trim();
    if (!publishableKey || !clientSecret || !typedWindow.Stripe) {
        return false;
    }
    const cardContainer = document.getElementById('checkout-stripe-card-element');
    const cardPanel = document.getElementById('checkout-stripe-form-card');
    const payButton = document.getElementById('checkout-stripe-pay-button');
    if (!cardContainer || !cardPanel || !payButton) {
        return false;
    }
    cardContainer.innerHTML = '';
    const stripeFactory = typedWindow.Stripe;
    stripeInstance = stripeFactory(publishableKey, { locale: 'nb' });
    const stripeElements = stripeInstance.elements();
    stripeCardElement = stripeElements.create('card', { hidePostalCode: true });
    stripeCardElement.mount('#checkout-stripe-card-element');
    stripeClientSecret = clientSecret;
    if (payButton.dataset.bound !== 'true') {
        payButton.dataset.bound = 'true';
        payButton.addEventListener('click', () => {
            void confirmStripePayment();
        });
    }
    clearStripeError();
    cardPanel.classList.remove('d-none');
    return true;
}
function hideStripePaymentForm() {
    const cardPanel = document.getElementById('checkout-stripe-form-card');
    cardPanel?.classList.add('d-none');
    clearStripeError();
    stripePaymentIntentId = null;
}
function showStripeError(message) {
    const errorContainer = document.getElementById('checkout-stripe-error');
    if (!errorContainer) {
        return;
    }
    errorContainer.textContent = message;
    errorContainer.classList.remove('d-none');
}
function clearStripeError() {
    const errorContainer = document.getElementById('checkout-stripe-error');
    if (!errorContainer) {
        return;
    }
    errorContainer.textContent = '';
    errorContainer.classList.add('d-none');
}
async function confirmStripePayment() {
    const typedWindow = window;
    if (!stripeInstance || !stripeCardElement || !stripeClientSecret) {
        showStripeError('Stripe session is not initialized.');
        return;
    }
    clearStripeError();
    const result = await stripeInstance.confirmCardPayment(stripeClientSecret, {
        payment_method: { card: stripeCardElement },
        return_url: `${window.location.origin}/shop/checkout?paymentStatus=success`
    });
    if (result.error) {
        showStripeError(result.error.message ?? 'Payment failed. Please check card details.');
        return;
    }
    if (!stripePaymentIntentId || stripePaymentIntentId <= 0) {
        typedWindow.UserPanelAlerts?.showError('checkout-alert-error', 'Payment intent ID could not be resolved for API confirmation.');
        return;
    }
    const confirmResponse = await typedWindow.UserPanelApi?.request(`/PaymentIntents/${stripePaymentIntentId}/confirm`, {
        method: 'POST',
        body: JSON.stringify('')
    }, true);
    if (!confirmResponse || !confirmResponse.success) {
        typedWindow.UserPanelAlerts?.showError('checkout-alert-error', confirmResponse?.message ?? 'Payment was processed, but API confirmation failed.');
        return;
    }
    typedWindow.UserPanelAlerts?.showSuccess('checkout-alert-success', 'Payment confirmed. Recurring subscriptions are being synchronized.');
}
function calculateCheckoutTotal(state) {
    let total = 0;
    if (state.domain) {
        total += state.domain.premiumPrice;
        if (state.domain.includePrivacy) {
            total += typeof state.domain.privacyPriceTotal === 'number' ? state.domain.privacyPriceTotal : 0;
        }
    }
    total += state.hosting.reduce((sum, item) => sum + (item.billingCycle === 'yearly' ? item.yearlyPrice : item.monthlyPrice), 0);
    total += state.services.reduce((sum, item) => sum + item.price, 0);
    total -= state.discount;
    return Math.max(0, total);
}
async function submitCheckout() {
    const typedWindow = window;
    typedWindow.UserPanelAlerts?.hide('checkout-alert-success');
    typedWindow.UserPanelAlerts?.hide('checkout-alert-error');
    const form = document.getElementById('checkout-form');
    if (form?.dataset.orderAdded === 'true') {
        typedWindow.UserPanelAlerts?.showSuccess('checkout-alert-success', 'Order is already added. Continue with payment instrument below.');
        return;
    }
    const submitButton = document.getElementById('checkout-submit');
    if (submitButton?.dataset.submitting === 'true') {
        return;
    }
    if (submitButton) {
        submitButton.dataset.submitting = 'true';
    }
    setCheckoutSubmitDisabled(true);
    const customerIdValue = document.getElementById('checkout-customer-id')?.value ?? '';
    const customerId = Number.parseInt(customerIdValue, 10);
    const confirmed = document.getElementById('checkout-confirm')?.checked ?? false;
    if (Number.isNaN(customerId) || customerId <= 0) {
        typedWindow.UserPanelAlerts?.showError('checkout-alert-error', 'Customer ID is required.');
        if (submitButton) {
            submitButton.dataset.submitting = 'false';
        }
        setCheckoutSubmitDisabled(false);
        return;
    }
    if (!confirmed) {
        typedWindow.UserPanelAlerts?.showError('checkout-alert-error', 'You must confirm the order details before placing the order.');
        if (submitButton) {
            submitButton.dataset.submitting = 'false';
        }
        setCheckoutSubmitDisabled(false);
        return;
    }
    const state = typedWindow.UserPanelCart?.getState();
    if (!state) {
        typedWindow.UserPanelAlerts?.showError('checkout-alert-error', 'Cart could not be loaded.');
        if (submitButton) {
            submitButton.dataset.submitting = 'false';
        }
        setCheckoutSubmitDisabled(false);
        return;
    }
    const oneTimeLines = [];
    const recurringGroups = new Map();
    const addRecurringLine = (mode, line) => {
        const normalizedMode = mode.trim().toLowerCase();
        const existing = recurringGroups.get(normalizedMode);
        if (existing) {
            existing.push(line);
            return;
        }
        recurringGroups.set(normalizedMode, [line]);
    };
    const resolveRecurringMode = (rawMode, fallback) => {
        const value = (rawMode ?? '').trim();
        return value.length > 0 ? value : fallback;
    };
    const computeLinesTotal = (lines) => lines
        .reduce((sum, line) => sum + ((line.quantity > 0 ? line.quantity : 1) * line.unitPrice), 0);
    if (state.domain) {
        const domainIsRecurring = state.domain.isRecurring === true;
        const domainLine = {
            serviceId: null,
            description: `Domain: ${state.domain.domainName}${typeof state.domain.periodYears === 'number' ? ` (${state.domain.periodYears} year${state.domain.periodYears > 1 ? 's' : ''})` : ''}${domainIsRecurring ? ' recurring' : ''}`,
            quantity: 1,
            unitPrice: state.domain.premiumPrice,
            isRecurring: domainIsRecurring,
            notes: ''
        };
        if (domainIsRecurring) {
            const domainMode = resolveRecurringMode(typeof state.domain.periodYears === 'number' && state.domain.periodYears > 0
                ? `${state.domain.periodYears}year`
                : 'yearly', 'yearly');
            addRecurringLine(domainMode, domainLine);
        }
        else {
            oneTimeLines.push(domainLine);
        }
        if (state.domain.includePrivacy) {
            const privacyLine = {
                serviceId: null,
                description: 'WHOIS Privacy',
                quantity: 1,
                unitPrice: typeof state.domain.privacyPriceTotal === 'number' ? state.domain.privacyPriceTotal : 0,
                isRecurring: domainIsRecurring,
                notes: ''
            };
            if (domainIsRecurring) {
                const privacyMode = resolveRecurringMode(typeof state.domain.periodYears === 'number' && state.domain.periodYears > 0
                    ? `${state.domain.periodYears}year`
                    : 'yearly', 'yearly');
                addRecurringLine(privacyMode, privacyLine);
            }
            else {
                oneTimeLines.push(privacyLine);
            }
        }
    }
    state.hosting.forEach((item) => {
        const hostingLine = {
            serviceId: null,
            description: `Hosting: ${item.name?.trim() ? item.name : `Package #${item.id}`} (${item.billingCycle})`,
            quantity: 1,
            unitPrice: item.billingCycle === 'yearly' ? item.yearlyPrice : item.monthlyPrice,
            isRecurring: true,
            notes: ''
        };
        addRecurringLine(resolveRecurringMode(item.billingCycle, 'monthly'), hostingLine);
    });
    state.services.forEach((item) => {
        const maybeBillingCycle = item.billingCycle;
        const serviceMode = resolveRecurringMode(maybeBillingCycle, 'monthly');
        addRecurringLine(serviceMode, {
            serviceId: item.id,
            description: `Service: ${item.name?.trim() ? item.name : `Service #${item.id}`}`,
            quantity: 1,
            unitPrice: item.price,
            isRecurring: true,
            notes: ''
        });
    });
    if (oneTimeLines.length === 0 && recurringGroups.size === 0) {
        typedWindow.UserPanelAlerts?.showError('checkout-alert-error', 'No order lines were added. Returning to domain search...');
        if (submitButton) {
            submitButton.dataset.submitting = 'false';
        }
        setCheckoutSubmitDisabled(false);
        redirectToDomainSearch(3000);
        return;
    }
    const createdOrders = [];
    const now = Date.now();
    const startDateIso = new Date(now).toISOString();
    const endDateIso = new Date(now + (365 * 24 * 60 * 60 * 1000)).toISOString();
    if (oneTimeLines.length > 0) {
        const oneTimePayload = {
            customerId,
            serviceId: null,
            quoteId: null,
            orderType: 0,
            startDate: startDateIso,
            endDate: endDateIso,
            nextBillingDate: new Date(now + (30 * 24 * 60 * 60 * 1000)).toISOString(),
            setupFee: Math.max(0, computeLinesTotal(oneTimeLines) - state.discount),
            recurringAmount: 0,
            couponCode: null,
            autoRenew: false,
            orderLines: oneTimeLines
        };
        const oneTimeResponse = await typedWindow.UserPanelApi?.request('/Orders/checkout', {
            method: 'POST',
            body: JSON.stringify(oneTimePayload)
        }, true);
        if (!oneTimeResponse || !oneTimeResponse.success || !oneTimeResponse.data) {
            typedWindow.UserPanelAlerts?.showError('checkout-alert-error', oneTimeResponse?.message ?? 'Could not place one-time order.');
            if (submitButton) {
                submitButton.dataset.submitting = 'false';
            }
            setCheckoutSubmitDisabled(false);
            return;
        }
        createdOrders.push({
            id: oneTimeResponse.data.id,
            orderNumber: oneTimeResponse.data.orderNumber,
            recurringMode: 'one-time',
            isRecurring: false
        });
    }
    const recurringSegments = Array.from(recurringGroups.entries())
        .sort((a, b) => {
        const dayDiff = getRecurringModeIntervalDays(a[0]) - getRecurringModeIntervalDays(b[0]);
        if (dayDiff !== 0) {
            return dayDiff;
        }
        return a[0].localeCompare(b[0]);
    });
    for (const [mode, lines] of recurringSegments) {
        const recurringPayload = {
            customerId,
            serviceId: null,
            quoteId: null,
            orderType: 0,
            startDate: startDateIso,
            endDate: endDateIso,
            nextBillingDate: new Date(now + (getRecurringModeIntervalDays(mode) * 24 * 60 * 60 * 1000)).toISOString(),
            setupFee: 0,
            recurringAmount: Math.max(0, computeLinesTotal(lines)),
            couponCode: null,
            autoRenew: true,
            orderLines: lines
        };
        const recurringResponse = await typedWindow.UserPanelApi?.request('/Orders/checkout', {
            method: 'POST',
            body: JSON.stringify(recurringPayload)
        }, true);
        if (!recurringResponse || !recurringResponse.success || !recurringResponse.data) {
            typedWindow.UserPanelAlerts?.showError('checkout-alert-error', recurringResponse?.message ?? `Could not place recurring order (${mode}).`);
            if (submitButton) {
                submitButton.dataset.submitting = 'false';
            }
            setCheckoutSubmitDisabled(false);
            return;
        }
        createdOrders.push({
            id: recurringResponse.data.id,
            orderNumber: recurringResponse.data.orderNumber,
            recurringMode: mode,
            isRecurring: true
        });
    }
    typedWindow.UserPanelAlerts?.showSuccess('checkout-alert-success', `Orders created: ${createdOrders.map((item) => item.orderNumber).join(', ')}`);
    const status = document.getElementById('checkout-payment-status');
    if (status) {
        status.textContent = `Orders created (${createdOrders.length}). Payment initialization is pending API integration.`;
    }
    markOrderAsAdded(createdOrders, true);
}
function renderPaymentStatusFromQuery() {
    const status = new URLSearchParams(window.location.search).get('paymentStatus');
    const container = document.getElementById('checkout-payment-status');
    if (!container || !status) {
        return;
    }
    const normalized = status.toLowerCase();
    if (normalized === 'success') {
        container.textContent = 'Payment completed successfully.';
    }
    else if (normalized === 'failed') {
        container.textContent = 'Payment failed. Please retry.';
    }
    else {
        container.textContent = 'Payment is pending confirmation.';
    }
}
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initializeCheckout);
}
else {
    initializeCheckout();
}
function registerCheckoutEnhancedLoadListener() {
    const typedWindow = window;
    if (typedWindow.Blazor?.addEventListener) {
        typedWindow.Blazor.addEventListener('enhancedload', initializeCheckout);
        return;
    }
    window.setTimeout(registerCheckoutEnhancedLoadListener, 100);
}
registerCheckoutEnhancedLoadListener();
//# sourceMappingURL=checkout.js.map
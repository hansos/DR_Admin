interface CheckoutCartState {
    domain: {
        domainName: string;
        premiumPrice: number;
        periodYears?: number;
        isRecurring?: boolean;
        includePrivacy?: boolean;
        privacyPriceTotal?: number;
    } | null;
    hosting: Array<{
        id: number;
        name?: string;
        billingCycle: 'monthly' | 'yearly';
        monthlyPrice: number;
        yearlyPrice: number;
    }>;
    services: Array<{
        id: number;
        name?: string;
        price: number;
    }>;
    discount: number;
}

function getMarkerOrderIds(marker: CheckoutOrderMarker): number[] {
    const ids = Array.isArray(marker.orderIds) && marker.orderIds.length > 0
        ? marker.orderIds
        : [marker.orderId];

    return ids.filter((id) => Number.isInteger(id) && id > 0);
}

function getMarkerPrimaryOrderId(marker: CheckoutOrderMarker): number {
    const ids = getMarkerOrderIds(marker);
    return ids.length > 0 ? ids[0] : marker.orderId;
}

function getRecurringModeIntervalDays(mode: string): number {
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

interface CheckoutCustomerPaymentMethodDto {
    id: number;
    type: number | string;
    isActive: boolean;
}

interface CheckoutAccountDto {
    id: number;
    username: string;
    email: string;
    customer?: {
        id: number;
        referenceNumber?: number;
        name: string;
        email: string;
    };
}

interface CreateOrderDto {
    customerId: number;
    serviceId: number | null;
    quoteId: number | null;
    orderType: number;
    startDate: string;
    endDate: string;
    nextBillingDate: string;
    setupFee: number;
    recurringAmount: number;
    couponCode: string | null;
    autoRenew: boolean;
    orderLines: CreateOrderLineDto[];
}

interface CreateOrderLineDto {
    serviceId: number | null;
    description: string;
    quantity: number;
    unitPrice: number;
    isRecurring: boolean;
    notes: string;
}

interface OrderDto {
    id: number;
    orderNumber: string;
    status: string;
}

interface PaymentIntentDto {
    id: number;
    status: string | number;
    gatewayIntentId: string;
    clientSecret: string;
    paymentGatewayProviderCode?: string;
    paymentGatewayPublicKey?: string;
}

interface StripeCardElement {
    mount: (selector: string) => void;
}

interface StripeElements {
    create: (type: 'card', options?: { hidePostalCode?: boolean }) => StripeCardElement;
}

interface StripeInstance {
    elements: () => StripeElements;
    confirmCardPayment: (clientSecret: string, data: {
        payment_method: { card: StripeCardElement };
        return_url?: string;
    }) => Promise<{ error?: { message?: string } }>;
}

interface PaymentInstrumentDto {
    id: number;
    code: string;
    name: string;
    isActive: boolean;
    displayOrder: number;
}

interface CheckoutOrderMarker {
    cartSignature: string;
    orderId: number;
    orderNumber: string;
    orderIds?: number[];
    orderNumbers?: string[];
    createdAt: string;
}

interface CheckoutCreatedOrder {
    id: number;
    orderNumber: string;
    recurringMode: string;
    isRecurring: boolean;
}

interface InstrumentFieldDefinition {
    name: string;
    label: string;
    type: 'text' | 'email' | 'number' | 'tel';
    required: boolean;
}

interface CheckoutWindow extends Window {
    UserPanelApi?: {
        request: <T>(path: string, options?: RequestInit, requiresAuth?: boolean) => Promise<{ success: boolean; data?: T; message?: string; statusCode?: number }>;
    };
    UserPanelAlerts?: {
        showSuccess: (id: string, message: string) => void;
        showError: (id: string, message: string) => void;
        hide: (id: string) => void;
    };
    UserPanelCart?: {
        getState: () => CheckoutCartState;
        clear: () => void;
    };
    bootstrap?: {
        Modal?: {
            getInstance: (element: Element) => { hide: () => void } | null;
            getOrCreateInstance: (element: Element) => { show: () => void; hide: () => void };
        };
    };
    Blazor?: {
        addEventListener?: (eventName: string, callback: () => void) => void;
    };
    Stripe?: (publishableKey: string, options?: { locale?: string }) => StripeInstance;
}

const checkoutOrderMarkerStorageKey = 'up_checkout_last_added_order';
const checkoutDomainSearchPath = '/shop/domain-search';
const checkoutSuccessPath = '/shop/checkout/success';
let stripeInstance: StripeInstance | null = null;
let stripeCardElement: StripeCardElement | null = null;
let stripeClientSecret: string | null = null;
let stripePaymentIntentId: number | null = null;

function buildCheckoutSuccessUrl(paymentStatus: 'success' | 'failed' | 'pending'): string {
    const params = new URLSearchParams();
    params.set('paymentStatus', paymentStatus);

    const marker = getStoredOrderMarker();
    if (marker && marker.orderId > 0) {
        params.set('orderId', marker.orderId.toString());
        const orderNumbers = (Array.isArray(marker.orderNumbers) && marker.orderNumbers.length > 0)
            ? marker.orderNumbers
            : [marker.orderNumber];
        params.set('orderNumbers', orderNumbers.join(','));
    }

    return `${checkoutSuccessPath}?${params.toString()}`;
}

function initializeCheckout(): void {
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

    const form = document.getElementById('checkout-form') as HTMLFormElement | null;
    form?.addEventListener('submit', async (event: Event) => {
        event.preventDefault();
        await submitCheckout();
    });

    bindDeleteOrderActions();
    updateCheckoutDeleteOrderVisibility();
}

function bindDeleteOrderActions(): void {
    const confirmButton = document.getElementById('checkout-delete-order-confirm') as HTMLButtonElement | null;
    if (!confirmButton || confirmButton.dataset.bound === 'true') {
        return;
    }

    confirmButton.dataset.bound = 'true';
    confirmButton.addEventListener('click', () => {
        void deletePendingOrder();
    });
}

function updateCheckoutDeleteOrderVisibility(): void {
    const button = document.getElementById('checkout-delete-order-open') as HTMLButtonElement | null;
    if (!button) {
        return;
    }

    const marker = getStoredOrderMarker();
    const typedWindow = window as CheckoutWindow;
    const state = typedWindow.UserPanelCart?.getState();

    const hasCartLines = !!state && (
        state.domain !== null ||
        state.hosting.length > 0 ||
        state.services.length > 0
    );

    const canDelete = (marker?.orderId ?? 0) > 0 || hasCartLines;
    button.classList.toggle('d-none', !canDelete);
}

function initializePaymentInstrumentPanel(): void {
    const instrumentSelect = document.getElementById('checkout-payment-instrument') as HTMLSelectElement | null;
    instrumentSelect?.addEventListener('change', () => {
        renderPaymentInstrumentFields();
        updateCheckoutContinueButtonVisibility();

        if (isCreditCardInstrumentCode(instrumentSelect.value)) {
            void continueToPayment();
            return;
        }

        hideStripePaymentForm();
    });

    const continueButton = document.getElementById('checkout-payment-instrument-submit') as HTMLButtonElement | null;
    continueButton?.addEventListener('click', () => {
        void continueToPayment();
    });

    const addButton = document.getElementById('checkout-payment-methods-open-create') as HTMLButtonElement | null;
    addButton?.addEventListener('click', () => {
        void prepareCheckoutPaymentMethodModal();
    });

    const addForm = document.getElementById('checkout-payment-methods-create-form') as HTMLFormElement | null;
    addForm?.addEventListener('submit', (event: Event) => {
        event.preventDefault();
        void saveCheckoutPaymentMethod();
    });
}

function isCreditCardInstrumentCode(code: string): boolean {
    const selected = (code ?? '').trim().toLowerCase();
    return selected === 'creditcard' || selected === 'credit card' || selected === 'card';
}

function updateCheckoutContinueButtonVisibility(): void {
    const select = document.getElementById('checkout-payment-instrument') as HTMLSelectElement | null;
    const continueButton = document.getElementById('checkout-payment-instrument-submit') as HTMLButtonElement | null;
    if (!select || !continueButton) {
        return;
    }

    continueButton.classList.toggle('d-none', isCreditCardInstrumentCode(select.value));
}

async function prepareCheckoutPaymentMethodModal(): Promise<void> {
    const typedWindow = window as CheckoutWindow;
    const select = document.getElementById('checkout-payment-methods-instrument') as HTMLSelectElement | null;
    if (!select) {
        return;
    }

    const response = await typedWindow.UserPanelApi?.request<PaymentInstrumentDto[]>('/PaymentInstruments/active', {
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

function mapInstrumentCodeToPaymentMethodType(code: string): number {
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

async function saveCheckoutPaymentMethod(): Promise<void> {
    const typedWindow = window as CheckoutWindow;
    const instrument = (document.getElementById('checkout-payment-methods-instrument') as HTMLSelectElement | null)?.value.trim() ?? '';
    const isDefault = (document.getElementById('checkout-payment-methods-default') as HTMLInputElement | null)?.checked ?? true;

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

function closeCheckoutDeleteModal(): void {
    const typedWindow = window as CheckoutWindow;
    const modalElement = document.getElementById('checkout-delete-order-modal');
    if (!modalElement) {
        return;
    }

    const instance = typedWindow.bootstrap?.Modal?.getInstance(modalElement);
    instance?.hide();
}

function clearCheckoutSessionState(): void {
    const typedWindow = window as CheckoutWindow;
    typedWindow.UserPanelCart?.clear();
    sessionStorage.removeItem(checkoutOrderMarkerStorageKey);

    const form = document.getElementById('checkout-form') as HTMLFormElement | null;
    if (form) {
        form.dataset.orderAdded = 'false';
    }

    const submitButton = document.getElementById('checkout-submit') as HTMLButtonElement | null;
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

function setCheckoutSubmitDisabled(disabled: boolean): void {
    const submitButton = document.getElementById('checkout-submit') as HTMLButtonElement | null;
    if (submitButton) {
        submitButton.disabled = disabled;
    }

    const confirmInput = document.getElementById('checkout-confirm') as HTMLInputElement | null;
    if (confirmInput) {
        confirmInput.disabled = disabled;
    }
}

function redirectToDomainSearch(delayMs: number = 1200): void {
    window.setTimeout(() => {
        window.location.href = checkoutDomainSearchPath;
    }, delayMs);
}

async function deletePendingOrder(): Promise<void> {
    const typedWindow = window as CheckoutWindow;
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
        const response = await typedWindow.UserPanelApi?.request<OrderDto>(`/Orders/checkout/${orderId}/cancel`, {
            method: 'POST'
        }, true);

        if (!response || !response.success) {
            if (response?.statusCode === 409) {
                clearCheckoutSessionState();
                typedWindow.UserPanelAlerts?.showError('checkout-alert-error', response.message ?? 'Order is already paid and cannot be deleted. Local checkout session was cleared.');
                closeCheckoutDeleteModal();
                redirectToDomainSearch(1800);
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

function renderSummary(): void {
    const typedWindow = window as CheckoutWindow;
    const state = typedWindow.UserPanelCart?.getState();
    const container = document.getElementById('checkout-summary-lines');

    if (!container || !state) {
        return;
    }

    let oneTimeTotal = 0;
    let recurringTotal = 0;
    const lines: string[] = [];

    if (state.domain) {
        const domainIsRecurring = state.domain.isRecurring === true;
        if (domainIsRecurring) {
            recurringTotal += state.domain.premiumPrice;
        } else {
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
            } else {
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

async function loadLoggedInCustomer(): Promise<void> {
    const typedWindow = window as CheckoutWindow;
    const customerIdEl = document.getElementById('checkout-customer-id') as HTMLInputElement | null;
    const customerIdDisplay = document.getElementById('checkout-customer-id-display');
    const customerName = document.getElementById('checkout-customer-name');
    const customerEmail = document.getElementById('checkout-customer-email');

    if (!customerIdEl || !customerIdDisplay || !customerName || !customerEmail) {
        return;
    }

    const response = await typedWindow.UserPanelApi?.request<CheckoutAccountDto>('/MyAccount/me', {
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

function escapeHtmlCheckout(value: string): string {
    return value
        .replace(/&/g, '&amp;')
        .replace(/</g, '&lt;')
        .replace(/>/g, '&gt;')
        .replace(/"/g, '&quot;')
        .replace(/'/g, '&#39;');
}

function getCartSignature(state: CheckoutCartState): string {
    return JSON.stringify(state);
}

function getStoredOrderMarker(): CheckoutOrderMarker | null {
    try {
        const raw = sessionStorage.getItem(checkoutOrderMarkerStorageKey);
        if (!raw) {
            return null;
        }

        return JSON.parse(raw) as CheckoutOrderMarker;
    } catch {
        return null;
    }
}

function saveOrderMarker(marker: CheckoutOrderMarker): void {
    sessionStorage.setItem(checkoutOrderMarkerStorageKey, JSON.stringify(marker));
}

function restoreOrderMarkerForCurrentCart(): void {
    const typedWindow = window as CheckoutWindow;
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

    const restoredOrders: CheckoutCreatedOrder[] = markerIds.map((id, index) => ({
        id,
        orderNumber: markerNumbers[index] ?? marker.orderNumber,
        recurringMode: 'restored',
        isRecurring: false
    }));

    markOrderAsAdded(restoredOrders, false);
}

function markOrderAsAdded(orders: CheckoutCreatedOrder[], persist: boolean): void {
    const typedWindow = window as CheckoutWindow;
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

    const submitButton = document.getElementById('checkout-submit') as HTMLButtonElement | null;
    if (submitButton) {
        submitButton.dataset.submitting = 'false';
    }
    setCheckoutSubmitDisabled(true);

    const form = document.getElementById('checkout-form') as HTMLFormElement | null;
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

async function loadPaymentInstruments(): Promise<void> {
    const typedWindow = window as CheckoutWindow;
    const select = document.getElementById('checkout-payment-instrument') as HTMLSelectElement | null;
    if (!select) {
        return;
    }

    const methodsResponse = await typedWindow.UserPanelApi?.request<CheckoutCustomerPaymentMethodDto[]>('/CustomerPaymentMethods/mine', {
        method: 'GET'
    }, true);

    const allowedInstrumentCodes = new Set<string>();
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

    const response = await typedWindow.UserPanelApi?.request<PaymentInstrumentDto[]>('/PaymentInstruments/active', {
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

async function resolveCheckoutCustomerId(): Promise<number | null> {
    const customerIdInput = document.getElementById('checkout-customer-id') as HTMLInputElement | null;
    const parsed = Number.parseInt(customerIdInput?.value ?? '', 10);
    if (!Number.isNaN(parsed) && parsed > 0) {
        return parsed;
    }

    const typedWindow = window as CheckoutWindow;
    const response = await typedWindow.UserPanelApi?.request<CheckoutAccountDto>('/MyAccount/me', {
        method: 'GET'
    }, true);

    const customerId = response?.data?.customer?.id ?? 0;
    if (customerIdInput && customerId > 0) {
        customerIdInput.value = customerId.toString();
    }

    return customerId > 0 ? customerId : null;
}

function mapPaymentMethodTypeToInstrumentCode(type: number | string): string {
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

function getInstrumentFieldDefinitions(instrumentCode: string): InstrumentFieldDefinition[] {
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

function renderPaymentInstrumentFields(): void {
    const select = document.getElementById('checkout-payment-instrument') as HTMLSelectElement | null;
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

async function continueToPayment(): Promise<void> {
    const typedWindow = window as CheckoutWindow;
    const select = document.getElementById('checkout-payment-instrument') as HTMLSelectElement | null;
    const fieldsContainer = document.getElementById('checkout-payment-instrument-fields');
    if (!select || !fieldsContainer) {
        return;
    }

    const requiredFields = Array.from(fieldsContainer.querySelectorAll<HTMLInputElement>('input[required]'));
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

    const response = await typedWindow.UserPanelApi?.request<PaymentIntentDto>('/PaymentIntents', {
        method: 'POST',
        body: JSON.stringify({
            orderId: primaryOrderId,
            amount: totalAmount,
            currency: 'EUR',
            paymentGatewayId: 0,
            paymentInstrument: selectedInstrument,
            returnUrl: `${baseUrl}${buildCheckoutSuccessUrl('success')}`,
            cancelUrl: `${baseUrl}${buildCheckoutSuccessUrl('failed')}`,
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

function prepareStripePaymentForm(intent: PaymentIntentDto): boolean {
    const typedWindow = window as CheckoutWindow;
    const publishableKey = (intent.paymentGatewayPublicKey ?? '').trim();
    const clientSecret = (intent.clientSecret ?? '').trim();
    if (!publishableKey || !clientSecret || !typedWindow.Stripe) {
        return false;
    }

    const cardContainer = document.getElementById('checkout-stripe-card-element');
    const cardPanel = document.getElementById('checkout-stripe-form-card');
    const payButton = document.getElementById('checkout-stripe-pay-button') as HTMLButtonElement | null;
    if (!cardContainer || !cardPanel || !payButton) {
        return false;
    }

    cardContainer.innerHTML = '';
    const stripeFactory = typedWindow.Stripe as unknown as (publishableKey: string, options?: { locale?: string }) => StripeInstance;
    stripeInstance = stripeFactory(publishableKey, { locale: 'nb' });

    const stripeElements = stripeInstance.elements() as unknown as {
        create: (type: 'card', options?: { hidePostalCode?: boolean }) => StripeCardElement;
    };
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

function hideStripePaymentForm(): void {
    const cardPanel = document.getElementById('checkout-stripe-form-card');
    cardPanel?.classList.add('d-none');
    clearStripeError();
    stripePaymentIntentId = null;
}

function showStripeError(message: string): void {
    const errorContainer = document.getElementById('checkout-stripe-error');
    if (!errorContainer) {
        return;
    }

    errorContainer.textContent = message;
    errorContainer.classList.remove('d-none');
}

function clearStripeError(): void {
    const errorContainer = document.getElementById('checkout-stripe-error');
    if (!errorContainer) {
        return;
    }

    errorContainer.textContent = '';
    errorContainer.classList.add('d-none');
}

function setStripePaymentProcessing(isProcessing: boolean): void {
    const payButton = document.getElementById('checkout-stripe-pay-button') as HTMLButtonElement | null;
    if (payButton) {
        if (!payButton.dataset.defaultLabel) {
            payButton.dataset.defaultLabel = payButton.innerHTML;
        }

        payButton.disabled = isProcessing;
        payButton.innerHTML = isProcessing
            ? '<span class="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>Processing payment...'
            : (payButton.dataset.defaultLabel ?? 'Pay now');
    }

    const status = document.getElementById('checkout-payment-status');
    if (status && isProcessing) {
        status.textContent = 'Processing card payment. Please wait...';
    }
}

async function confirmStripePayment(): Promise<void> {
    const typedWindow = window as CheckoutWindow;
    if (!stripeInstance || !stripeCardElement || !stripeClientSecret) {
        showStripeError('Stripe session is not initialized.');
        return;
    }

    setStripePaymentProcessing(true);
    clearStripeError();
    const result = await stripeInstance.confirmCardPayment(stripeClientSecret, {
        payment_method: { card: stripeCardElement },
        return_url: `${window.location.origin}${buildCheckoutSuccessUrl('success')}`
    });

    if (result.error) {
        setStripePaymentProcessing(false);
        showStripeError(result.error.message ?? 'Payment failed. Please check card details.');
        return;
    }

    if (!stripePaymentIntentId || stripePaymentIntentId <= 0) {
        setStripePaymentProcessing(false);
        typedWindow.UserPanelAlerts?.showError('checkout-alert-error', 'Payment intent ID could not be resolved for API confirmation.');
        return;
    }

    const confirmResponse = await typedWindow.UserPanelApi?.request(`/PaymentIntents/${stripePaymentIntentId}/confirm`, {
        method: 'POST',
        body: JSON.stringify('')
    }, true);

    if (!confirmResponse || !confirmResponse.success) {
        setStripePaymentProcessing(false);
        typedWindow.UserPanelAlerts?.showError('checkout-alert-error', confirmResponse?.message ?? 'Payment was processed, but API confirmation failed.');
        return;
    }

    typedWindow.UserPanelAlerts?.showSuccess('checkout-alert-success', 'Payment confirmed. Recurring subscriptions are being synchronized.');
    window.location.href = buildCheckoutSuccessUrl('success');
}

function calculateCheckoutTotal(state: CheckoutCartState): number {
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

async function submitCheckout(): Promise<void> {
    const typedWindow = window as CheckoutWindow;
    typedWindow.UserPanelAlerts?.hide('checkout-alert-success');
    typedWindow.UserPanelAlerts?.hide('checkout-alert-error');

    const form = document.getElementById('checkout-form') as HTMLFormElement | null;
    if (form?.dataset.orderAdded === 'true') {
        typedWindow.UserPanelAlerts?.showSuccess('checkout-alert-success', 'Order is already added. Continue with payment instrument below.');
        return;
    }

    const submitButton = document.getElementById('checkout-submit') as HTMLButtonElement | null;
    if (submitButton?.dataset.submitting === 'true') {
        return;
    }

    if (submitButton) {
        submitButton.dataset.submitting = 'true';
    }
    setCheckoutSubmitDisabled(true);

    const customerIdValue = (document.getElementById('checkout-customer-id') as HTMLInputElement | null)?.value ?? '';
    const customerId = Number.parseInt(customerIdValue, 10);
    const confirmed = (document.getElementById('checkout-confirm') as HTMLInputElement | null)?.checked ?? false;

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

    const oneTimeLines: CreateOrderLineDto[] = [];
    const recurringGroups = new Map<string, CreateOrderLineDto[]>();

    const addRecurringLine = (mode: string, line: CreateOrderLineDto): void => {
        const normalizedMode = mode.trim().toLowerCase();
        const existing = recurringGroups.get(normalizedMode);
        if (existing) {
            existing.push(line);
            return;
        }

        recurringGroups.set(normalizedMode, [line]);
    };

    const resolveRecurringMode = (rawMode: string | undefined | null, fallback: string): string => {
        const value = (rawMode ?? '').trim();
        return value.length > 0 ? value : fallback;
    };

    const computeLinesTotal = (lines: CreateOrderLineDto[]): number => lines
        .reduce((sum, line) => sum + ((line.quantity > 0 ? line.quantity : 1) * line.unitPrice), 0);

    if (state.domain) {
        const domainIsRecurring = state.domain.isRecurring === true;
        const domainLine: CreateOrderLineDto = {
            serviceId: null,
            description: `Domain: ${state.domain.domainName}${typeof state.domain.periodYears === 'number' ? ` (${state.domain.periodYears} year${state.domain.periodYears > 1 ? 's' : ''})` : ''}${domainIsRecurring ? ' recurring' : ''}`,
            quantity: 1,
            unitPrice: state.domain.premiumPrice,
            isRecurring: domainIsRecurring,
            notes: ''
        };
        if (domainIsRecurring) {
            const domainMode = resolveRecurringMode(
                typeof state.domain.periodYears === 'number' && state.domain.periodYears > 0
                    ? `${state.domain.periodYears}year`
                    : 'yearly',
                'yearly');
            addRecurringLine(domainMode, domainLine);
        } else {
            oneTimeLines.push(domainLine);
        }

        if (state.domain.includePrivacy) {
            const privacyLine: CreateOrderLineDto = {
                serviceId: null,
                description: 'WHOIS Privacy',
                quantity: 1,
                unitPrice: typeof state.domain.privacyPriceTotal === 'number' ? state.domain.privacyPriceTotal : 0,
                isRecurring: domainIsRecurring,
                notes: ''
            };
            if (domainIsRecurring) {
                const privacyMode = resolveRecurringMode(
                    typeof state.domain.periodYears === 'number' && state.domain.periodYears > 0
                        ? `${state.domain.periodYears}year`
                        : 'yearly',
                    'yearly');
                addRecurringLine(privacyMode, privacyLine);
            } else {
                oneTimeLines.push(privacyLine);
            }
        }
    }

    state.hosting.forEach((item) => {
        const hostingLine: CreateOrderLineDto = {
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
        const maybeBillingCycle = (item as unknown as { billingCycle?: string }).billingCycle;
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

    const createdOrders: CheckoutCreatedOrder[] = [];
    const now = Date.now();
    const startDateIso = new Date(now).toISOString();
    const endDateIso = new Date(now + (365 * 24 * 60 * 60 * 1000)).toISOString();

    if (oneTimeLines.length > 0) {
        const oneTimePayload: CreateOrderDto = {
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

        const oneTimeResponse = await typedWindow.UserPanelApi?.request<OrderDto>('/Orders/checkout', {
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
        const recurringPayload: CreateOrderDto = {
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

        const recurringResponse = await typedWindow.UserPanelApi?.request<OrderDto>('/Orders/checkout', {
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

function renderPaymentStatusFromQuery(): void {
    const status = new URLSearchParams(window.location.search).get('paymentStatus');
    const container = document.getElementById('checkout-payment-status');
    if (!container || !status) {
        return;
    }

    const normalized = status.toLowerCase();
    if (normalized === 'success') {
        container.textContent = 'Payment completed successfully. Redirecting...';
        window.location.href = buildCheckoutSuccessUrl('success');
    } else if (normalized === 'failed') {
        container.textContent = 'Payment failed. Redirecting...';
        window.location.href = buildCheckoutSuccessUrl('failed');
    } else {
        container.textContent = 'Payment is pending confirmation. Redirecting...';
        window.location.href = buildCheckoutSuccessUrl('pending');
    }
}

if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initializeCheckout);
} else {
    initializeCheckout();
}

function registerCheckoutEnhancedLoadListener(): void {
    const typedWindow = window as CheckoutWindow;
    if (typedWindow.Blazor?.addEventListener) {
        typedWindow.Blazor.addEventListener('enhancedload', initializeCheckout);
        return;
    }

    window.setTimeout(registerCheckoutEnhancedLoadListener, 100);
}

registerCheckoutEnhancedLoadListener();

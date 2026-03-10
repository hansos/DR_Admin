"use strict";
let latestResult = null;
let defaultRegistrarId = null;
let defaultRegistrarCode = null;
let defaultRegistrarCodeRequest = null;
let serviceCatalog = null;
let serviceCatalogRequest = null;
let hostingCatalog = null;
let hostingCatalogRequest = null;
let latestCalculatedPrice = null;
let latestCalculatedCurrency = 'USD';
let latestTldId = null;
let latestPrivacyPrice = null;
let isDomainSelectionLocked = false;
let hasAttemptedDomainSearch = false;
const basketPositionStorageKey = 'up_domain_search_basket_position';
const domainSearchCheckoutOrderMarkerStorageKey = 'up_checkout_last_added_order';
const domainSearchGuestAttemptsStorageKey = 'up_domain_search_guest_attempts';
const domainSearchGuestAttemptsLimit = 10;
const domainSearchGuestAttemptsWindowMs = 60 * 60 * 1000;
function initializeDomainSearch() {
    const form = document.getElementById('domain-search-form');
    if (!form || form.dataset.bound === 'true') {
        return;
    }
    form.dataset.bound = 'true';
    initializeBasketDragging();
    renderFloatingBasket();
    bindDomainSearchDeleteOrderActions();
    updateDomainSearchDeleteOrderVisibility();
    updateFlowProgressIndicator();
    const addAndBundleButton = document.getElementById('domain-search-add-and-bundle');
    addAndBundleButton?.addEventListener('click', () => {
        if (!addResultToCart(false)) {
            return;
        }
        if (!isDomainSearchUserLoggedIn()) {
            redirectToLoginFromDomainSearch(true);
            return;
        }
        function updateDomainSearchDeleteOrderVisibility() {
            const button = document.getElementById('domain-search-delete-order-open');
            if (!button) {
                return;
            }
            const marker = getStoredCheckoutOrderMarker();
            const typedWindow = window;
            const state = typedWindow.UserPanelCart?.getState();
            const hasCartLines = !!state && (state.domain !== null ||
                state.hosting.length > 0 ||
                state.services.length > 0);
            const canDelete = (marker?.orderId ?? 0) > 0 || hasCartLines;
            button.classList.toggle('d-none', !canDelete);
        }
        isDomainSelectionLocked = true;
        setDomainFormLocked(true);
        addAndBundleButton.disabled = true;
        renderResult(latestResult);
        updateFlowProgressIndicator();
        setUpsellVisibility(true);
        void renderUpsellOptions();
        const upsellCard = document.getElementById('domain-search-upsell-card');
        upsellCard?.scrollIntoView({ behavior: 'smooth', block: 'start' });
    });
    const backToSearchButton = document.getElementById('domain-search-back-to-search');
    backToSearchButton?.addEventListener('click', () => {
        switchToDomainSearchMode();
    });
    const recurringSelect = document.getElementById('domain-search-recurring');
    recurringSelect?.addEventListener('change', () => {
        if (!latestResult || !latestResult.isAvailable) {
            return;
        }
        if (isDomainSelectionLocked) {
            addResultToCart(false);
        }
        renderResult(latestResult);
        renderFloatingBasket();
    });
    const checkoutButton = document.getElementById('domain-search-checkout');
    checkoutButton?.addEventListener('click', () => {
        const typedWindow = window;
        if (!addResultToCart(false)) {
            typedWindow.UserPanelAlerts?.showError('domain-search-alert-error', 'Search and select an available domain before checkout.');
            return;
        }
        window.location.href = '/shop/checkout';
    });
    const transferButton = document.getElementById('domain-search-transfer');
    transferButton?.addEventListener('click', () => {
        showTransferNotImplementedModal();
    });
    const alternativesButton = document.getElementById('domain-search-alternatives');
    alternativesButton?.addEventListener('click', () => {
        const domainName = latestResult?.domainName ?? getInputValue('domain-search-input');
        if (!domainName) {
            return;
        }
        void renderAlternativeDomains(domainName);
    });
    const alternativesList = document.getElementById('domain-search-alternatives-list');
    alternativesList?.addEventListener('click', (event) => {
        const target = event.target;
        const item = target.closest('[data-domain-alternative]');
        if (!item) {
            return;
        }
        if (item.dataset.availability !== 'available' || item.disabled) {
            return;
        }
        const alternativeDomain = item.dataset.domainAlternative ?? '';
        const input = document.getElementById('domain-search-input');
        if (!input || !alternativeDomain) {
            return;
        }
        input.value = alternativeDomain;
        form.requestSubmit();
    });
    const serviceList = document.getElementById('domain-search-upsell-services-list');
    serviceList?.addEventListener('click', (event) => {
        const target = event.target;
        const button = target.closest('[data-service-id]');
        if (!button) {
            return;
        }
        const id = Number.parseInt(button.dataset.serviceId ?? '', 10);
        if (Number.isNaN(id) || id <= 0) {
            return;
        }
        toggleServiceSelection(id);
    });
    const hostingList = document.getElementById('domain-search-upsell-hosting-list');
    hostingList?.addEventListener('click', (event) => {
        const target = event.target;
        const button = target.closest('[data-hosting-id]');
        if (!button) {
            return;
        }
        const id = Number.parseInt(button.dataset.hostingId ?? '', 10);
        if (Number.isNaN(id) || id <= 0) {
            return;
        }
        const container = button.closest('[data-hosting-row]');
        const cycleSelect = container?.querySelector('[data-hosting-cycle]');
        const billingCycle = cycleSelect?.value === 'yearly' ? 'yearly' : 'monthly';
        toggleHostingSelection(id, billingCycle);
    });
    hostingList?.addEventListener('change', (event) => {
        const target = event.target;
        const cycleSelect = target.closest('[data-hosting-cycle]');
        if (!cycleSelect) {
            return;
        }
        const id = Number.parseInt(cycleSelect.dataset.hostingCycle ?? '', 10);
        if (Number.isNaN(id) || id <= 0) {
            return;
        }
        const billingCycle = cycleSelect.value === 'yearly' ? 'yearly' : 'monthly';
        updateHostingCycle(id, billingCycle);
    });
    const periodSelect = document.getElementById('domain-search-period');
    periodSelect?.addEventListener('change', async () => {
        if (!latestResult || !latestResult.isAvailable) {
            return;
        }
        latestCalculatedPrice = await getDomainRegistrationPrice(latestResult.domainName, getSelectedPeriodYears());
        setPrivacyPriceLabel();
        renderResult(latestResult);
        if (isDomainSelectionLocked) {
            addResultToCart(false);
        }
    });
    const privacyToggle = document.getElementById('domain-search-privacy');
    privacyToggle?.addEventListener('change', () => {
        if (isDomainSelectionLocked) {
            addResultToCart(false);
        }
    });
    form.addEventListener('submit', async (event) => {
        event.preventDefault();
        const typedWindow = window;
        typedWindow.UserPanelAlerts?.hide('domain-search-alert-success');
        typedWindow.UserPanelAlerts?.hide('domain-search-alert-error');
        hasAttemptedDomainSearch = true;
        updateDomainSearchDeleteOrderVisibility();
        isDomainSelectionLocked = false;
        setDomainFormLocked(false);
        const domainName = getInputValue('domain-search-input');
        if (!domainName) {
            typedWindow.UserPanelAlerts?.showError('domain-search-alert-error', 'Domain name is required.');
            return;
        }
        const registrar = await getDefaultRegistrarSelection();
        if (!registrar) {
            typedWindow.UserPanelAlerts?.showError('domain-search-alert-error', 'Default registrar is not configured.');
            renderResult(null);
            return;
        }
        const encodedDomain = encodeURIComponent(domainName);
        const encodedRegistrarId = encodeURIComponent(registrar.id.toString());
        if (!isDomainSearchUserLoggedIn() && registerGuestDomainSearchAttemptAndShouldForceLogin()) {
            redirectToLoginFromDomainSearch(false);
            return;
        }
        const response = await typedWindow.UserPanelApi?.request(`/Registrars/${encodedRegistrarId}/isavailable/${encodedDomain}`, {
            method: 'GET'
        }, false);
        if (!response || !response.success || !response.data) {
            typedWindow.UserPanelAlerts?.showError('domain-search-alert-error', response?.message ?? 'Could not check domain availability.');
            renderResult(null);
            return;
        }
        latestResult = response.data;
        await applyDomainSettings(response.data.domainName);
        latestCalculatedPrice = await getDomainRegistrationPrice(response.data.domainName, getSelectedPeriodYears());
        renderResult(response.data);
    });
    applyDomainSearchQueryPrefillAndSubmit(form);
    void restoreDomainSearchFromCart();
}
function applyDomainSearchQueryPrefillAndSubmit(form) {
    const query = new URLSearchParams(window.location.search);
    const domain = (query.get('domain') ?? '').trim();
    if (!domain) {
        return;
    }
    const input = document.getElementById('domain-search-input');
    if (!input) {
        return;
    }
    input.value = domain;
    if (form.dataset.autoSearchDomain === domain.toLowerCase()) {
        return;
    }
    form.dataset.autoSearchDomain = domain.toLowerCase();
    window.setTimeout(() => {
        form.requestSubmit();
    }, 0);
}
function showTransferNotImplementedModal() {
    const typedWindow = window;
    const modalElement = document.getElementById('domain-search-transfer-not-implemented-modal');
    if (!modalElement) {
        return;
    }
    const instance = typedWindow.bootstrap?.Modal?.getOrCreateInstance(modalElement);
    instance?.show();
}
function bindDomainSearchDeleteOrderActions() {
    const confirmButton = document.getElementById('domain-search-delete-order-confirm');
    if (!confirmButton || confirmButton.dataset.bound === 'true') {
        return;
    }
    confirmButton.dataset.bound = 'true';
    confirmButton.addEventListener('click', () => {
        void deleteOrderFromDomainSearch();
    });
}
function updateDomainSearchDeleteOrderVisibility() {
    const button = document.getElementById('domain-search-delete-order-open');
    if (!button) {
        return;
    }
    const marker = getStoredCheckoutOrderMarker();
    const hasCartLines = hasDomainSearchCartLines();
    const canDelete = hasAttemptedDomainSearch && ((marker?.orderId ?? 0) > 0 || hasCartLines);
    button.classList.toggle('d-none', !canDelete);
}
function hasDomainSearchCartLines() {
    const typedWindow = window;
    const state = typedWindow.UserPanelCart?.getState();
    return !!state && (state.domain !== null ||
        state.hosting.length > 0 ||
        state.services.length > 0);
}
function getStoredCheckoutOrderMarker() {
    try {
        const raw = sessionStorage.getItem(domainSearchCheckoutOrderMarkerStorageKey);
        if (!raw) {
            return null;
        }
        return JSON.parse(raw);
    }
    catch {
        return null;
    }
}
function closeDomainSearchDeleteOrderModal() {
    const typedWindow = window;
    const modalElement = document.getElementById('domain-search-delete-order-modal');
    if (!modalElement) {
        return;
    }
    const instance = typedWindow.bootstrap?.Modal?.getInstance(modalElement);
    instance?.hide();
}
function clearDomainSearchOrderState() {
    const typedWindow = window;
    typedWindow.UserPanelCart?.clear();
    sessionStorage.removeItem(domainSearchCheckoutOrderMarkerStorageKey);
    defaultRegistrarId = null;
    defaultRegistrarCode = null;
    defaultRegistrarCodeRequest = null;
    const domainInput = document.getElementById('domain-search-input');
    if (domainInput) {
        domainInput.value = '';
    }
    latestResult = null;
    latestCalculatedPrice = null;
    latestPrivacyPrice = null;
    isDomainSelectionLocked = false;
    setDomainFormLocked(false);
    renderResult(null);
    renderFloatingBasket();
    updateDomainSearchDeleteOrderVisibility();
}
async function deleteOrderFromDomainSearch() {
    const typedWindow = window;
    typedWindow.UserPanelAlerts?.hide('domain-search-alert-success');
    typedWindow.UserPanelAlerts?.hide('domain-search-alert-error');
    const marker = getStoredCheckoutOrderMarker();
    if (!marker || marker.orderId <= 0) {
        clearDomainSearchOrderState();
        closeDomainSearchDeleteOrderModal();
        typedWindow.UserPanelAlerts?.showSuccess('domain-search-alert-success', 'Local order session data removed.');
        return;
    }
    const response = await typedWindow.UserPanelApi?.request(`/Orders/checkout/${marker.orderId}/cancel`, {
        method: 'POST'
    }, true);
    if (!response || !response.success) {
        if (response?.statusCode === 409) {
            clearDomainSearchOrderState();
            typedWindow.UserPanelAlerts?.showSuccess('domain-search-alert-success', response.message ?? 'Order cannot be cancelled anymore. Local order session data was removed.');
            closeDomainSearchDeleteOrderModal();
            return;
        }
        typedWindow.UserPanelAlerts?.showError('domain-search-alert-error', response?.message ?? 'Could not delete order.');
        closeDomainSearchDeleteOrderModal();
        return;
    }
    clearDomainSearchOrderState();
    closeDomainSearchDeleteOrderModal();
    typedWindow.UserPanelAlerts?.showSuccess('domain-search-alert-success', 'Order cancelled and removed from your session.');
}
async function restoreDomainSearchFromCart() {
    if (!isDomainSearchUserLoggedIn()) {
        return;
    }
    const typedWindow = window;
    const state = typedWindow.UserPanelCart?.getState();
    if (!state?.domain) {
        return;
    }
    const domain = state.domain;
    const domainInput = document.getElementById('domain-search-input');
    if (domainInput) {
        domainInput.value = domain.domainName;
    }
    if (domain.registrarCode) {
        defaultRegistrarCode = domain.registrarCode;
    }
    latestResult = {
        success: true,
        domainName: domain.domainName,
        isAvailable: true,
        isTldSupported: true,
        message: '',
        premiumPrice: domain.premiumPrice
    };
    await applyDomainSettings(domain.domainName);
    const periodSelect = document.getElementById('domain-search-period');
    if (periodSelect) {
        const hasStoredPeriod = Array.from(periodSelect.options).some((option) => option.value === domain.periodYears.toString());
        if (hasStoredPeriod) {
            periodSelect.value = domain.periodYears.toString();
        }
    }
    const privacyInput = document.getElementById('domain-search-privacy');
    if (privacyInput && !privacyInput.disabled) {
        privacyInput.checked = domain.includePrivacy;
    }
    latestCalculatedPrice = typeof domain.premiumPrice === 'number' && domain.premiumPrice > 0
        ? domain.premiumPrice
        : await getDomainRegistrationPrice(domain.domainName, domain.periodYears);
    isDomainSelectionLocked = true;
    setDomainFormLocked(true);
    renderResult(latestResult);
    setUpsellVisibility(true);
    await renderUpsellOptions();
    renderFloatingBasket();
    const query = new URLSearchParams(window.location.search);
    if (query.get('resumeBundle') === '1') {
        const upsellCard = document.getElementById('domain-search-upsell-card');
        upsellCard?.scrollIntoView({ behavior: 'smooth', block: 'start' });
        query.delete('resumeBundle');
        const updatedQuery = query.toString();
        const updatedUrl = updatedQuery ? `${window.location.pathname}?${updatedQuery}` : window.location.pathname;
        window.history.replaceState({}, '', updatedUrl);
    }
}
function renderResult(result) {
    const card = document.getElementById('domain-search-result-card');
    const summary = document.getElementById('domain-search-result-summary');
    const priceInfo = document.getElementById('domain-search-result-price');
    const addAndBundleButton = document.getElementById('domain-search-add-and-bundle');
    const transferButton = document.getElementById('domain-search-transfer');
    const alternativesButton = document.getElementById('domain-search-alternatives');
    const alternativesList = document.getElementById('domain-search-alternatives-list');
    if (!card || !summary || !priceInfo || !addAndBundleButton || !transferButton || !alternativesButton || !alternativesList) {
        return;
    }
    if (!result) {
        card.classList.add('d-none');
        card.classList.remove('border-success', 'bg-success-subtle', 'border-danger', 'bg-danger-subtle');
        priceInfo.classList.add('d-none');
        priceInfo.textContent = '';
        addAndBundleButton.classList.add('d-none');
        addAndBundleButton.disabled = false;
        addAndBundleButton.innerHTML = '<i class="bi bi-bag-check"></i> Add domain & choose hosting/services';
        transferButton.classList.add('d-none');
        transferButton.disabled = false;
        alternativesButton.classList.add('d-none');
        alternativesList.classList.add('d-none');
        alternativesList.innerHTML = '';
        summary.textContent = '';
        renderSelectedDomainSummary(null);
        setUpsellVisibility(false);
        updateFlowProgressIndicator();
        return;
    }
    transferButton.disabled = shouldDisableTransfer(result);
    if (result.isAvailable) {
        const shownPrice = getSelectedDomainPrice(result);
        if (isDomainSelectionLocked) {
            card.classList.add('d-none');
            renderSelectedDomainSummary({
                domainName: result.domainName,
                periodYears: getSelectedPeriodYears(),
                price: shownPrice > 0 ? `${shownPrice.toFixed(2)} ${latestCalculatedCurrency}` : 'Price unavailable'
            });
        }
        else {
            card.classList.remove('d-none');
            renderSelectedDomainSummary(null);
            card.classList.remove('border-danger', 'bg-danger-subtle');
            card.classList.add('border-success', 'bg-success-subtle');
            const years = getSelectedPeriodYears();
            summary.innerHTML = `Your new domain: <strong>${escapeHtml(result.domainName)}</strong>, Price (${years} year${years > 1 ? 's' : ''}): ${shownPrice > 0 ? `${shownPrice.toFixed(2)} ${latestCalculatedCurrency}` : 'Price unavailable'}`;
            if (shownPrice > 0) {
                priceInfo.classList.add('d-none');
                priceInfo.textContent = '';
            }
            else {
                priceInfo.classList.add('d-none');
                priceInfo.textContent = '';
            }
            addAndBundleButton.classList.remove('d-none');
            addAndBundleButton.disabled = false;
            addAndBundleButton.innerHTML = `<i class="bi bi-bag-check"></i> ${getAddAndBundleButtonLabel(result)}`;
            transferButton.classList.add('d-none');
            alternativesButton.classList.add('d-none');
            alternativesList.classList.add('d-none');
            alternativesList.innerHTML = '';
        }
        addAndBundleButton.disabled = isDomainSelectionLocked;
        setUpsellVisibility(isDomainSelectionLocked);
        if (isDomainSelectionLocked) {
            void renderUpsellOptions();
        }
    }
    else {
        card.classList.remove('d-none');
        renderSelectedDomainSummary(null);
        card.classList.remove('border-success', 'bg-success-subtle');
        card.classList.add('border-danger', 'bg-danger-subtle');
        summary.textContent = formatUnavailableDomainMessage(result);
        priceInfo.classList.add('d-none');
        priceInfo.textContent = '';
        addAndBundleButton.classList.add('d-none');
        addAndBundleButton.disabled = false;
        addAndBundleButton.innerHTML = '<i class="bi bi-bag-check"></i> Add domain & choose hosting/services';
        transferButton.classList.remove('d-none');
        alternativesButton.classList.remove('d-none');
        alternativesList.classList.add('d-none');
        alternativesList.innerHTML = '';
        setUpsellVisibility(false);
    }
    updateFlowProgressIndicator();
}
function formatUnavailableDomainMessage(result) {
    const message = result.message.trim();
    if (!message) {
        return `${result.domainName} is not available.`;
    }
    const lowerMessage = message.toLowerCase();
    const lowerDomainName = result.domainName.toLowerCase();
    if (lowerMessage.includes('registered') && !lowerMessage.includes(lowerDomainName)) {
        return `${result.domainName} is registered.`;
    }
    return message;
}
function shouldDisableTransfer(result) {
    return result.message.trim() === 'You already own this domain in your account.';
}
function addResultToCart(showMessage) {
    if (!latestResult || !latestResult.isAvailable) {
        return false;
    }
    const typedWindow = window;
    const periodValue = Number.parseInt(getInputValue('domain-search-period'), 10);
    const periodYears = Number.isNaN(periodValue) ? 1 : periodValue;
    const privacyInput = document.getElementById('domain-search-privacy');
    const includePrivacy = !!privacyInput?.checked;
    const state = typedWindow.UserPanelCart?.getState();
    if (!state) {
        return false;
    }
    if (!defaultRegistrarCode) {
        typedWindow.UserPanelAlerts?.showError('domain-search-alert-error', 'Default registrar is not configured.');
        return false;
    }
    state.domain = {
        domainName: latestResult.domainName,
        registrarCode: defaultRegistrarCode,
        periodYears,
        isRecurring: isDomainRecurring(),
        includePrivacy,
        premiumPrice: getSelectedDomainPrice(latestResult),
        privacyPriceTotal: includePrivacy && typeof latestPrivacyPrice === 'number' ? latestPrivacyPrice * periodYears : 0
    };
    typedWindow.UserPanelCart?.saveState(state);
    renderFloatingBasket();
    if (showMessage) {
        typedWindow.UserPanelAlerts?.showSuccess('domain-search-alert-success', 'Domain added to cart. Continue by selecting hosting/services or checkout.');
    }
    function isDomainRecurring() {
        const recurringSelect = document.getElementById('domain-search-recurring');
        return recurringSelect?.value === 'recurring';
    }
    return true;
}
function getSelectedDomainPrice(result) {
    if (typeof result.premiumPrice === 'number' && result.premiumPrice > 0) {
        return result.premiumPrice;
    }
    return latestCalculatedPrice ?? 0;
}
function getAddAndBundleButtonLabel(result) {
    const typedWindow = window;
    const selectedDomainName = typedWindow.UserPanelCart?.getState()?.domain?.domainName ?? '';
    if (selectedDomainName.trim().toLowerCase() === result.domainName.trim().toLowerCase()) {
        return 'Continue with domain & choose hosting/services';
    }
    return 'Add domain & choose hosting/services';
}
function renderSelectedDomainSummary(selection) {
    const card = document.getElementById('domain-search-selection-summary-card');
    const domainElement = document.getElementById('domain-search-selection-summary-domain');
    const priceElement = document.getElementById('domain-search-selection-summary-price');
    if (!card || !domainElement || !priceElement) {
        return;
    }
    if (!selection) {
        card.classList.add('d-none');
        domainElement.textContent = '';
        priceElement.textContent = '';
        return;
    }
    card.classList.remove('d-none');
    domainElement.textContent = selection.domainName;
    priceElement.textContent = `Price (${selection.periodYears} year${selection.periodYears > 1 ? 's' : ''}): ${selection.price}`;
}
function getCurrentFlowStep() {
    const marker = getStoredCheckoutOrderMarker();
    const hasCartLines = hasDomainSearchCartLines();
    if ((marker?.orderId ?? 0) > 0 && hasCartLines) {
        return 3;
    }
    if (isDomainSelectionLocked || hasCartLines) {
        return 2;
    }
    return 1;
}
function updateFlowProgressIndicator() {
    const progressBar = document.getElementById('domain-search-flow-progress-bar');
    const step1Indicator = document.getElementById('domain-search-flow-step-1-indicator');
    const step2Indicator = document.getElementById('domain-search-flow-step-2-indicator');
    const step3Indicator = document.getElementById('domain-search-flow-step-3-indicator');
    const step1Label = document.getElementById('domain-search-flow-step-1-label');
    const step2Label = document.getElementById('domain-search-flow-step-2-label');
    const step3Label = document.getElementById('domain-search-flow-step-3-label');
    if (!progressBar || !step1Indicator || !step2Indicator || !step3Indicator || !step1Label || !step2Label || !step3Label) {
        return;
    }
    const step = getCurrentFlowStep();
    const width = step === 1 ? 33 : step === 2 ? 66 : 100;
    progressBar.setAttribute('aria-valuenow', width.toString());
    progressBar.style.width = `${width}%`;
    const applyState = (indicator, label, state) => {
        indicator.classList.remove('bg-white', 'text-muted', 'border-secondary-subtle', 'bg-primary', 'text-white', 'border-primary', 'bg-success', 'border-success');
        label.classList.remove('text-muted', 'text-primary', 'text-success', 'fw-semibold');
        if (state === 'active') {
            indicator.classList.add('bg-primary', 'text-white', 'border-primary');
            label.classList.add('text-primary', 'fw-semibold');
            return;
        }
        if (state === 'completed') {
            indicator.classList.add('bg-success', 'text-white', 'border-success');
            label.classList.add('text-success', 'fw-semibold');
            return;
        }
        indicator.classList.add('bg-white', 'text-muted', 'border-secondary-subtle');
        label.classList.add('text-muted');
    };
    applyState(step1Indicator, step1Label, step > 1 ? 'completed' : step === 1 ? 'active' : 'pending');
    applyState(step2Indicator, step2Label, step > 2 ? 'completed' : step === 2 ? 'active' : 'pending');
    applyState(step3Indicator, step3Label, step === 3 ? 'active' : 'pending');
}
async function applyDomainSettings(domainName) {
    const typedWindow = window;
    const tldExtension = getTldExtension(domainName);
    if (!tldExtension) {
        latestTldId = null;
        latestPrivacyPrice = null;
        setPeriodOptions(1, 1, 1);
        setPrivacyPriceLabel();
        return;
    }
    const tldResponse = await typedWindow.UserPanelApi?.request(`/Tlds/extension/${encodeURIComponent(tldExtension)}`, {
        method: 'GET'
    }, false);
    if (!tldResponse || !tldResponse.success || !tldResponse.data) {
        latestTldId = null;
        latestPrivacyPrice = null;
        setPeriodOptions(1, 1, getSelectedPeriodYears());
        setPrivacyPriceLabel();
        return;
    }
    const tldData = tldResponse.data;
    latestTldId = tldData.id;
    const defaultYears = normalizeYears(tldData.defaultRegistrationYears, 1);
    const maxYears = normalizeYears(tldData.maxRegistrationYears, Math.max(defaultYears, 1));
    setPeriodOptions(defaultYears, maxYears, getSelectedPeriodYears());
    const privacyToggle = document.getElementById('domain-search-privacy');
    if (privacyToggle) {
        const requiresPrivacy = tldData.requiresPrivacy === true;
        privacyToggle.checked = requiresPrivacy ? true : privacyToggle.checked;
        privacyToggle.disabled = requiresPrivacy;
    }
    await loadPrivacyPrice(tldData.id);
}
function normalizeYears(value, fallback) {
    if (typeof value !== 'number' || Number.isNaN(value) || value <= 0) {
        return fallback;
    }
    return Math.max(1, Math.floor(value));
}
function setPeriodOptions(defaultYears, maxYears, selectedYears) {
    const periodSelect = document.getElementById('domain-search-period');
    if (!periodSelect) {
        return;
    }
    const normalizedMax = Math.max(1, maxYears);
    const normalizedDefault = Math.min(Math.max(1, defaultYears), normalizedMax);
    const normalizedSelected = selectedYears >= 1 && selectedYears <= normalizedMax
        ? selectedYears
        : normalizedDefault;
    const options = [];
    for (let year = 1; year <= normalizedMax; year += 1) {
        options.push(`<option value="${year}" ${year === normalizedSelected ? 'selected' : ''}>${year} year${year > 1 ? 's' : ''}</option>`);
    }
    periodSelect.innerHTML = options.join('');
}
async function loadPrivacyPrice(tldId) {
    const typedWindow = window;
    const response = await typedWindow.UserPanelApi?.request(`/tld-pricing/sales/tld/${tldId}/current`, {
        method: 'GET'
    }, false);
    if (!response || !response.success || !response.data) {
        latestPrivacyPrice = null;
        setPrivacyPriceLabel();
        return;
    }
    latestPrivacyPrice = typeof response.data.privacyPrice === 'number' ? response.data.privacyPrice : null;
    latestCalculatedCurrency = response.data.currency ?? latestCalculatedCurrency;
    setPrivacyPriceLabel();
}
function setPrivacyPriceLabel() {
    const priceLabel = document.getElementById('domain-search-privacy-price');
    if (!priceLabel) {
        return;
    }
    if (typeof latestPrivacyPrice === 'number') {
        const years = getSelectedPeriodYears();
        const totalPrivacyPrice = latestPrivacyPrice * years;
        priceLabel.textContent = `(${totalPrivacyPrice.toFixed(2)} ${latestCalculatedCurrency} for ${years} year${years > 1 ? 's' : ''})`;
        return;
    }
    priceLabel.textContent = '(price unavailable)';
}
function renderFloatingBasket() {
    const panel = document.getElementById('domain-search-basket-panel');
    const linesContainer = document.getElementById('domain-search-basket-lines');
    const totalContainer = document.getElementById('domain-search-basket-total');
    if (!panel || !linesContainer || !totalContainer) {
        return;
    }
    const typedWindow = window;
    const state = typedWindow.UserPanelCart?.getState();
    if (!state) {
        panel.classList.add('d-none');
        linesContainer.innerHTML = '';
        totalContainer.textContent = '0.00';
        updateDomainSearchDeleteOrderVisibility();
        return;
    }
    const lines = [];
    let total = 0;
    if (state.domain) {
        const domainPrice = typeof state.domain.premiumPrice === 'number' ? state.domain.premiumPrice : 0;
        total += domainPrice;
        const paymentMode = state.domain.isRecurring ? 'recurring' : 'one-time';
        lines.push(`<div class="d-flex justify-content-between"><span>Domain: ${escapeHtml(state.domain.domainName)} (${state.domain.periodYears} year${state.domain.periodYears > 1 ? 's' : ''}, ${paymentMode})</span><span>${domainPrice.toFixed(2)}</span></div>`);
        if (state.domain.includePrivacy && typeof latestPrivacyPrice === 'number') {
            const privacyPrice = latestPrivacyPrice * state.domain.periodYears;
            total += privacyPrice;
            lines.push(`<div class="d-flex justify-content-between"><span>WHOIS Privacy</span><span>${privacyPrice.toFixed(2)}</span></div>`);
        }
    }
    state.hosting.forEach((item) => {
        const amount = item.billingCycle === 'yearly' ? item.yearlyPrice : item.monthlyPrice;
        total += amount;
        lines.push(`<div class="d-flex justify-content-between"><span>Hosting: ${escapeHtml(item.name)} (${item.billingCycle})</span><span>${amount.toFixed(2)}</span></div>`);
    });
    state.services.forEach((item) => {
        total += item.price;
        lines.push(`<div class="d-flex justify-content-between"><span>Service: ${escapeHtml(item.name)}</span><span>${item.price.toFixed(2)}</span></div>`);
    });
    if (state.discount > 0) {
        total -= state.discount;
        lines.push(`<div class="d-flex justify-content-between"><span>Discount</span><span>- ${state.discount.toFixed(2)}</span></div>`);
    }
    if (lines.length === 0) {
        panel.classList.add('d-none');
        linesContainer.innerHTML = '';
        totalContainer.textContent = '0.00';
        updateDomainSearchDeleteOrderVisibility();
        return;
    }
    panel.classList.remove('d-none');
    linesContainer.innerHTML = lines.join('');
    totalContainer.textContent = Math.max(0, total).toFixed(2);
    ensureBasketVisible(panel);
    updateDomainSearchDeleteOrderVisibility();
}
function initializeBasketDragging() {
    const panel = document.getElementById('domain-search-basket-panel');
    const handle = document.getElementById('domain-search-basket-drag-handle');
    if (!panel || !handle || panel.dataset.dragBound === 'true') {
        return;
    }
    panel.dataset.dragBound = 'true';
    restoreBasketPosition(panel);
    window.addEventListener('resize', () => {
        ensureBasketVisible(panel);
        saveBasketPosition(panel);
    });
    let isDragging = false;
    let offsetX = 0;
    let offsetY = 0;
    const onMouseMove = (event) => {
        if (!isDragging) {
            return;
        }
        moveBasketTo(panel, event.clientX - offsetX, event.clientY - offsetY);
    };
    const onMouseUp = () => {
        if (!isDragging) {
            return;
        }
        isDragging = false;
        handle.classList.remove('active');
        saveBasketPosition(panel);
        document.removeEventListener('mousemove', onMouseMove);
        document.removeEventListener('mouseup', onMouseUp);
    };
    handle.addEventListener('mousedown', (event) => {
        event.preventDefault();
        const rect = panel.getBoundingClientRect();
        offsetX = event.clientX - rect.left;
        offsetY = event.clientY - rect.top;
        isDragging = true;
        handle.classList.add('active');
        panel.style.bottom = 'auto';
        document.addEventListener('mousemove', onMouseMove);
        document.addEventListener('mouseup', onMouseUp);
    });
}
function ensureBasketVisible(panel) {
    const left = Number.parseFloat(panel.style.left);
    const top = Number.parseFloat(panel.style.top);
    if (Number.isNaN(left) || Number.isNaN(top)) {
        return;
    }
    moveBasketTo(panel, left, top);
}
function moveBasketTo(panel, left, top) {
    const maxLeft = Math.max(0, window.innerWidth - panel.offsetWidth);
    const maxTop = Math.max(0, window.innerHeight - panel.offsetHeight);
    const boundedLeft = Math.min(Math.max(0, left), maxLeft);
    const boundedTop = Math.min(Math.max(0, top), maxTop);
    panel.style.left = `${boundedLeft}px`;
    panel.style.top = `${boundedTop}px`;
    panel.style.right = 'auto';
    panel.style.bottom = 'auto';
}
function saveBasketPosition(panel) {
    try {
        const left = Number.parseFloat(panel.style.left);
        const top = Number.parseFloat(panel.style.top);
        if (Number.isNaN(left) || Number.isNaN(top)) {
            return;
        }
        localStorage.setItem(basketPositionStorageKey, JSON.stringify({ left, top }));
    }
    catch {
    }
}
function restoreBasketPosition(panel) {
    try {
        const raw = localStorage.getItem(basketPositionStorageKey);
        if (!raw) {
            return;
        }
        const parsed = JSON.parse(raw);
        if (typeof parsed.left !== 'number' || typeof parsed.top !== 'number') {
            return;
        }
        moveBasketTo(panel, parsed.left, parsed.top);
    }
    catch {
    }
}
function getSelectedPeriodYears() {
    const periodValue = Number.parseInt(getInputValue('domain-search-period'), 10);
    if (Number.isNaN(periodValue) || periodValue <= 0) {
        return 1;
    }
    return periodValue;
}
function setDomainFormLocked(isLocked) {
    const formCard = document.getElementById('domain-search-form-card');
    const form = document.getElementById('domain-search-form');
    if (!form) {
        return;
    }
    const controls = Array.from(form.querySelectorAll('input, select, button'));
    controls.forEach((element) => {
        element.disabled = isLocked;
    });
    formCard?.classList.toggle('d-none', isLocked);
}
function switchToDomainSearchMode() {
    isDomainSelectionLocked = false;
    setDomainFormLocked(false);
    setUpsellVisibility(false);
    renderResult(latestResult);
    const domainInput = document.getElementById('domain-search-input');
    domainInput?.focus();
}
async function getDomainRegistrationPrice(domainName, years) {
    const tldExtension = getTldExtension(domainName);
    if (!tldExtension) {
        latestCalculatedCurrency = 'USD';
        return null;
    }
    const typedWindow = window;
    const tldResponse = await typedWindow.UserPanelApi?.request(`/Tlds/extension/${encodeURIComponent(tldExtension)}`, {
        method: 'GET'
    }, true);
    if (!tldResponse || !tldResponse.success || !tldResponse.data) {
        latestCalculatedCurrency = 'USD';
        return null;
    }
    const pricingRequest = {
        tldId: tldResponse.data.id,
        operationType: 'Registration',
        years,
        isFirstYear: true
    };
    const pricingResponse = await typedWindow.UserPanelApi?.request('/tld-pricing/calculate', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(pricingRequest)
    }, false);
    if (!pricingResponse || !pricingResponse.success || !pricingResponse.data) {
        latestCalculatedCurrency = 'USD';
        return null;
    }
    const responseData = pricingResponse.data;
    const finalPrice = typeof responseData.finalPrice === 'number'
        ? responseData.finalPrice
        : typeof responseData.FinalPrice === 'number'
            ? responseData.FinalPrice
            : null;
    const currency = responseData.currency ?? responseData.Currency ?? 'USD';
    latestCalculatedCurrency = currency;
    return finalPrice;
}
function getTldExtension(domainName) {
    const trimmedDomain = domainName.trim().toLowerCase();
    if (!trimmedDomain) {
        return null;
    }
    const labels = trimmedDomain.split('.').filter((segment) => segment.length > 0);
    if (labels.length < 2) {
        return null;
    }
    return labels[labels.length - 1] ?? null;
}
function setUpsellVisibility(isVisible) {
    const card = document.getElementById('domain-search-upsell-card');
    if (!card) {
        return;
    }
    if (isVisible) {
        card.classList.remove('d-none');
        return;
    }
    card.classList.add('d-none');
    const servicesList = document.getElementById('domain-search-upsell-services-list');
    if (servicesList) {
        servicesList.innerHTML = '';
    }
    const hostingList = document.getElementById('domain-search-upsell-hosting-list');
    if (hostingList) {
        hostingList.innerHTML = '';
    }
    const serviceCount = document.getElementById('domain-search-upsell-service-count');
    if (serviceCount) {
        serviceCount.textContent = '0';
    }
    const hostingCount = document.getElementById('domain-search-upsell-hosting-count');
    if (hostingCount) {
        hostingCount.textContent = '0';
    }
}
async function renderUpsellOptions() {
    await Promise.all([
        renderServiceUpsell(),
        renderHostingUpsell()
    ]);
}
async function renderServiceUpsell() {
    const list = document.getElementById('domain-search-upsell-services-list');
    const count = document.getElementById('domain-search-upsell-service-count');
    if (!list || !count) {
        return;
    }
    const catalog = await getServiceCatalog();
    if (!catalog || catalog.length === 0) {
        list.innerHTML = '<div class="list-group-item text-muted">No optional services are currently available.</div>';
        count.textContent = '0';
        return;
    }
    const typedWindow = window;
    const state = typedWindow.UserPanelCart?.getState();
    const selectedServices = state?.services ?? [];
    count.textContent = selectedServices.length.toString();
    list.innerHTML = catalog.map((item) => {
        const isSelected = selectedServices.some((entry) => entry.id === item.id);
        const price = typeof item.price === 'number' ? item.price : 0;
        return `<div class="list-group-item d-flex justify-content-between align-items-center gap-2">
            <div>
                <div class="fw-semibold">${escapeHtml(item.name)}</div>
                <div class="small text-muted">${price.toFixed(2)}</div>
            </div>
            <button type="button" class="btn btn-sm ${isSelected ? 'btn-outline-danger' : 'btn-outline-primary'}" data-service-id="${item.id}">${isSelected ? 'Remove' : 'Add'}</button>
        </div>`;
    }).join('');
}
async function renderHostingUpsell() {
    const list = document.getElementById('domain-search-upsell-hosting-list');
    const count = document.getElementById('domain-search-upsell-hosting-count');
    if (!list || !count) {
        return;
    }
    const catalog = await getHostingCatalog();
    if (!catalog || catalog.length === 0) {
        list.innerHTML = '<div class="list-group-item text-muted">No hosting packages are currently available.</div>';
        count.textContent = '0';
        return;
    }
    const typedWindow = window;
    const state = typedWindow.UserPanelCart?.getState();
    const selectedHosting = state?.hosting ?? [];
    count.textContent = selectedHosting.length.toString();
    list.innerHTML = catalog.map((item) => {
        const selected = selectedHosting.find((entry) => entry.id === item.id);
        const billingCycle = selected?.billingCycle ?? 'monthly';
        const isSelected = !!selected;
        return `<div class="list-group-item d-flex flex-wrap justify-content-between align-items-center gap-2" data-hosting-row="${item.id}">
            <div>
                <div class="fw-semibold">${escapeHtml(item.name)}</div>
                <div class="small text-muted">Monthly ${item.monthlyPrice.toFixed(2)} / Yearly ${item.yearlyPrice.toFixed(2)}</div>
            </div>
            <div class="d-flex gap-2 align-items-center">
                <select class="form-select form-select-sm" data-hosting-cycle="${item.id}">
                    <option value="monthly" ${billingCycle === 'monthly' ? 'selected' : ''}>Monthly</option>
                    <option value="yearly" ${billingCycle === 'yearly' ? 'selected' : ''}>Yearly</option>
                </select>
                <button type="button" class="btn btn-sm ${isSelected ? 'btn-outline-danger' : 'btn-outline-primary'}" data-hosting-id="${item.id}">${isSelected ? 'Remove' : 'Add'}</button>
            </div>
        </div>`;
    }).join('');
}
async function getServiceCatalog() {
    if (serviceCatalog) {
        return serviceCatalog;
    }
    if (serviceCatalogRequest) {
        return serviceCatalogRequest;
    }
    const typedWindow = window;
    serviceCatalogRequest = (async () => {
        const response = await typedWindow.UserPanelApi?.request('/Services/catalog', {
            method: 'GET'
        }, true);
        if (!response || !response.success || !response.data) {
            return null;
        }
        serviceCatalog = response.data;
        return serviceCatalog;
    })();
    const result = await serviceCatalogRequest;
    serviceCatalogRequest = null;
    return result;
}
function toggleServiceSelection(serviceId) {
    const typedWindow = window;
    const state = typedWindow.UserPanelCart?.getState();
    if (!state) {
        return;
    }
    const currentIndex = state.services.findIndex((item) => item.id === serviceId);
    if (currentIndex >= 0) {
        state.services.splice(currentIndex, 1);
        typedWindow.UserPanelCart?.saveState(state);
        renderFloatingBasket();
        void renderServiceUpsell();
        return;
    }
    const source = serviceCatalog?.find((item) => item.id === serviceId);
    if (!source) {
        return;
    }
    state.services.push({
        id: source.id,
        name: source.name,
        price: typeof source.price === 'number' ? source.price : 0
    });
    typedWindow.UserPanelCart?.saveState(state);
    renderFloatingBasket();
    void renderServiceUpsell();
}
async function getHostingCatalog() {
    if (hostingCatalog) {
        return hostingCatalog;
    }
    if (hostingCatalogRequest) {
        return hostingCatalogRequest;
    }
    const typedWindow = window;
    hostingCatalogRequest = (async () => {
        const response = await typedWindow.UserPanelApi?.request('/HostingPackages/catalog/active', {
            method: 'GET'
        }, true);
        if (!response || !response.success || !response.data) {
            return null;
        }
        hostingCatalog = response.data;
        return hostingCatalog;
    })();
    const result = await hostingCatalogRequest;
    hostingCatalogRequest = null;
    return result;
}
function toggleHostingSelection(hostingId, billingCycle) {
    const typedWindow = window;
    const state = typedWindow.UserPanelCart?.getState();
    if (!state) {
        return;
    }
    const existingIndex = state.hosting.findIndex((item) => item.id === hostingId);
    if (existingIndex >= 0) {
        state.hosting.splice(existingIndex, 1);
        typedWindow.UserPanelCart?.saveState(state);
        renderFloatingBasket();
        void renderHostingUpsell();
        return;
    }
    const source = hostingCatalog?.find((item) => item.id === hostingId);
    if (!source) {
        return;
    }
    state.hosting = [{
            id: source.id,
            name: source.name,
            monthlyPrice: source.monthlyPrice,
            yearlyPrice: source.yearlyPrice,
            billingCycle
        }];
    typedWindow.UserPanelCart?.saveState(state);
    renderFloatingBasket();
    void renderHostingUpsell();
}
function updateHostingCycle(hostingId, billingCycle) {
    const typedWindow = window;
    const state = typedWindow.UserPanelCart?.getState();
    if (!state) {
        return;
    }
    const existing = state.hosting.find((item) => item.id === hostingId);
    if (!existing) {
        return;
    }
    existing.billingCycle = billingCycle;
    typedWindow.UserPanelCart?.saveState(state);
    renderFloatingBasket();
}
function isDomainSearchUserLoggedIn() {
    const typedWindow = window;
    return typedWindow.UserPanelAuth?.isLoggedIn() === true;
}
function registerGuestDomainSearchAttemptAndShouldForceLogin() {
    const now = Date.now();
    const from = now - domainSearchGuestAttemptsWindowMs;
    const current = getStoredGuestDomainSearchAttempts().filter((value) => value >= from);
    current.push(now);
    localStorage.setItem(domainSearchGuestAttemptsStorageKey, JSON.stringify(current));
    return current.length > domainSearchGuestAttemptsLimit;
}
function getStoredGuestDomainSearchAttempts() {
    try {
        const raw = localStorage.getItem(domainSearchGuestAttemptsStorageKey);
        if (!raw) {
            return [];
        }
        const parsed = JSON.parse(raw);
        if (!Array.isArray(parsed)) {
            return [];
        }
        return parsed
            .filter((value) => typeof value === 'number' && Number.isFinite(value))
            .map((value) => Number(value));
    }
    catch {
        return [];
    }
}
function redirectToLoginFromDomainSearch(resumeBundle) {
    const params = new URLSearchParams(window.location.search);
    if (resumeBundle) {
        params.set('resumeBundle', '1');
    }
    else {
        params.delete('resumeBundle');
    }
    const query = params.toString();
    const returnUrl = query ? `${window.location.pathname}?${query}` : window.location.pathname;
    window.location.href = `/account/login?returnUrl=${encodeURIComponent(returnUrl)}`;
}
function getInputValue(id) {
    const input = document.getElementById(id);
    return input?.value.trim() ?? '';
}
async function getDefaultRegistrarCode() {
    const registrar = await getDefaultRegistrarSelection();
    return registrar?.code ?? null;
}
async function getDefaultRegistrarSelection() {
    if (defaultRegistrarCode && defaultRegistrarId && defaultRegistrarId > 0) {
        return { id: defaultRegistrarId, code: defaultRegistrarCode };
    }
    if (defaultRegistrarCodeRequest) {
        const code = await defaultRegistrarCodeRequest;
        return defaultRegistrarId && defaultRegistrarId > 0 && code
            ? { id: defaultRegistrarId, code }
            : null;
    }
    const typedWindow = window;
    defaultRegistrarCodeRequest = (async () => {
        const response = await typedWindow.UserPanelApi?.request('/Registrars/active', {
            method: 'GET'
        }, false);
        if (!response || !response.success || !response.data) {
            return null;
        }
        const registrar = defaultRegistrarCode
            ? response.data.find((item) => item.code === defaultRegistrarCode) ?? response.data.find((item) => item.isDefault)
            : response.data.find((item) => item.isDefault);
        if (!registrar || !registrar.code || registrar.id <= 0) {
            return null;
        }
        defaultRegistrarId = registrar.id;
        defaultRegistrarCode = registrar.code;
        return defaultRegistrarCode;
    })();
    const code = await defaultRegistrarCodeRequest;
    defaultRegistrarCodeRequest = null;
    return defaultRegistrarId && defaultRegistrarId > 0 && code
        ? { id: defaultRegistrarId, code }
        : null;
}
async function renderAlternativeDomains(domainName) {
    const list = document.getElementById('domain-search-alternatives-list');
    if (!list) {
        return;
    }
    list.classList.remove('d-none');
    list.innerHTML = '<div class="list-group-item text-muted">Loading suggestions...</div>';
    const typedWindow = window;
    const response = await typedWindow.UserPanelApi?.request(`/DomainManager/domain/name/${encodeURIComponent(domainName)}/alternatives?count=12`, {
        method: 'GET'
    }, false);
    if (!response || !response.success || !response.data) {
        list.innerHTML = '<div class="list-group-item text-muted">No alternatives found.</div>';
        return;
    }
    const suggestions = response.data.suggestions ?? response.data.Suggestions ?? [];
    if (!suggestions.length) {
        list.innerHTML = '<div class="list-group-item text-muted">No alternatives found.</div>';
        return;
    }
    list.innerHTML = suggestions.map((item) => (`<button type="button" class="list-group-item list-group-item-action d-flex justify-content-between align-items-center" data-domain-alternative="${escapeHtml(item)}" data-availability="pending" disabled>
            <span>${escapeHtml(item)}</span>
            <span class="domain-search-alternative-status"><span class="badge text-bg-warning"><span class="spinner-border spinner-border-sm me-1" role="status" aria-hidden="true"></span>Pending</span></span>
        </button>`)).join('');
    const registrarCode = await getDefaultRegistrarCode();
    if (!registrarCode) {
        list.innerHTML = '<div class="list-group-item text-muted">Default registrar is not configured.</div>';
        return;
    }
    await Promise.all(suggestions.map(async (item) => {
        const status = await checkAlternativeAvailability(item, registrarCode);
        updateAlternativeAvailability(item, status);
    }));
}
async function checkAlternativeAvailability(domainName, registrarCode) {
    const typedWindow = window;
    const response = await typedWindow.UserPanelApi?.request(`/DomainManager/registrar/${encodeURIComponent(registrarCode)}/domain/name/${encodeURIComponent(domainName)}/is-available`, {
        method: 'GET'
    }, false);
    if (!response || !response.success || !response.data) {
        return 'taken';
    }
    return response.data.isAvailable && response.data.isTldSupported !== false ? 'available' : 'taken';
}
function updateAlternativeAvailability(domainName, status) {
    const list = document.getElementById('domain-search-alternatives-list');
    if (!list) {
        return;
    }
    const candidates = Array.from(list.querySelectorAll('[data-domain-alternative]'));
    const item = candidates.find((element) => (element.dataset.domainAlternative ?? '') === domainName);
    if (!item) {
        return;
    }
    const statusElement = item.querySelector('.domain-search-alternative-status');
    item.classList.remove('list-group-item-warning', 'list-group-item-success', 'list-group-item-danger');
    if (status === 'available') {
        item.dataset.availability = 'available';
        item.disabled = false;
        item.classList.add('list-group-item-success');
        if (statusElement) {
            statusElement.innerHTML = '<span class="badge text-bg-success">Available</span>';
        }
        return;
    }
    item.dataset.availability = 'taken';
    item.disabled = true;
    item.classList.add('list-group-item-danger');
    if (statusElement) {
        statusElement.innerHTML = '<span class="badge text-bg-danger">Taken</span>';
    }
}
function escapeHtml(value) {
    return value
        .replace(/&/g, '&amp;')
        .replace(/</g, '&lt;')
        .replace(/>/g, '&gt;')
        .replace(/"/g, '&quot;')
        .replace(/'/g, '&#39;');
}
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initializeDomainSearch);
}
else {
    initializeDomainSearch();
}
window.addEventListener('popstate', initializeDomainSearch);
const domainSearchWindow = window;
function registerDomainSearchEnhancedLoadListener() {
    if (domainSearchWindow.Blazor?.addEventListener) {
        domainSearchWindow.Blazor.addEventListener('enhancedload', initializeDomainSearch);
        return;
    }
    window.setTimeout(registerDomainSearchEnhancedLoadListener, 100);
}
registerDomainSearchEnhancedLoadListener();
//# sourceMappingURL=domain-search.js.map
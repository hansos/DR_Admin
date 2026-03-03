"use strict";
let latestResult = null;
let defaultRegistrarCode = null;
let defaultRegistrarCodeRequest = null;
let serviceCatalog = null;
let serviceCatalogRequest = null;
let hostingCatalog = null;
let hostingCatalogRequest = null;
let latestCalculatedPrice = null;
let latestCalculatedCurrency = 'USD';
let isDomainSelectionLocked = false;
function initializeDomainSearch() {
    const form = document.getElementById('domain-search-form');
    if (!form || form.dataset.bound === 'true') {
        return;
    }
    form.dataset.bound = 'true';
    const addAndBundleButton = document.getElementById('domain-search-add-and-bundle');
    addAndBundleButton?.addEventListener('click', () => {
        if (!addResultToCart(false)) {
            return;
        }
        isDomainSelectionLocked = true;
        setDomainFormLocked(true);
        addAndBundleButton.disabled = true;
        setUpsellVisibility(true);
        void renderUpsellOptions();
        const upsellCard = document.getElementById('domain-search-upsell-card');
        upsellCard?.scrollIntoView({ behavior: 'smooth', block: 'start' });
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
        const domainName = latestResult?.domainName ?? getInputValue('domain-search-input');
        if (!domainName) {
            return;
        }
        window.location.href = `/shop/checkout?flow=transfer&domain=${encodeURIComponent(domainName)}`;
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
    form.addEventListener('submit', async (event) => {
        event.preventDefault();
        const typedWindow = window;
        typedWindow.UserPanelAlerts?.hide('domain-search-alert-success');
        typedWindow.UserPanelAlerts?.hide('domain-search-alert-error');
        isDomainSelectionLocked = false;
        setDomainFormLocked(false);
        const domainName = getInputValue('domain-search-input');
        if (!domainName) {
            typedWindow.UserPanelAlerts?.showError('domain-search-alert-error', 'Domain name is required.');
            return;
        }
        const registrarCode = await getDefaultRegistrarCode();
        if (!registrarCode) {
            typedWindow.UserPanelAlerts?.showError('domain-search-alert-error', 'Default registrar is not configured.');
            renderResult(null);
            return;
        }
        const encodedDomain = encodeURIComponent(domainName);
        const encodedRegistrar = encodeURIComponent(registrarCode);
        const response = await typedWindow.UserPanelApi?.request(`/DomainManager/registrar/${encodedRegistrar}/domain/name/${encodedDomain}/is-available`, {
            method: 'GET'
        }, true);
        if (!response || !response.success || !response.data) {
            typedWindow.UserPanelAlerts?.showError('domain-search-alert-error', response?.message ?? 'Could not check domain availability.');
            renderResult(null);
            return;
        }
        latestResult = response.data;
        latestCalculatedPrice = await getDomainRegistrationPrice(response.data.domainName, getSelectedPeriodYears());
        renderResult(response.data);
    });
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
        priceInfo.classList.add('d-none');
        priceInfo.textContent = '';
        addAndBundleButton.classList.add('d-none');
        addAndBundleButton.disabled = false;
        transferButton.classList.add('d-none');
        alternativesButton.classList.add('d-none');
        alternativesList.classList.add('d-none');
        alternativesList.innerHTML = '';
        summary.textContent = '';
        setUpsellVisibility(false);
        return;
    }
    card.classList.remove('d-none');
    if (result.isAvailable) {
        summary.textContent = `${result.domainName} is available. Add it now and complete your bundle.`;
        const shownPrice = getSelectedDomainPrice(result);
        if (shownPrice > 0) {
            priceInfo.classList.remove('d-none');
            priceInfo.textContent = `Price (${getSelectedPeriodYears()} year): ${shownPrice.toFixed(2)} ${latestCalculatedCurrency}`;
        }
        else {
            priceInfo.classList.add('d-none');
            priceInfo.textContent = '';
        }
        addAndBundleButton.classList.remove('d-none');
        addAndBundleButton.disabled = isDomainSelectionLocked;
        transferButton.classList.add('d-none');
        alternativesButton.classList.add('d-none');
        alternativesList.classList.add('d-none');
        alternativesList.innerHTML = '';
        setUpsellVisibility(isDomainSelectionLocked);
        if (isDomainSelectionLocked) {
            void renderUpsellOptions();
        }
    }
    else {
        summary.textContent = result.message || `${result.domainName} is not available.`;
        priceInfo.classList.add('d-none');
        priceInfo.textContent = '';
        addAndBundleButton.classList.add('d-none');
        addAndBundleButton.disabled = false;
        transferButton.classList.remove('d-none');
        alternativesButton.classList.remove('d-none');
        alternativesList.classList.add('d-none');
        alternativesList.innerHTML = '';
        setUpsellVisibility(false);
    }
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
        includePrivacy,
        premiumPrice: getSelectedDomainPrice(latestResult)
    };
    typedWindow.UserPanelCart?.saveState(state);
    if (showMessage) {
        typedWindow.UserPanelAlerts?.showSuccess('domain-search-alert-success', 'Domain added to cart. Continue by selecting hosting/services or checkout.');
    }
    return true;
}
function getSelectedDomainPrice(result) {
    if (typeof result.premiumPrice === 'number' && result.premiumPrice > 0) {
        return result.premiumPrice;
    }
    return latestCalculatedPrice ?? 0;
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
    if (formCard) {
        formCard.classList.toggle('opacity-75', isLocked);
    }
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
        const response = await typedWindow.UserPanelApi?.request('/Services', {
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
        const response = await typedWindow.UserPanelApi?.request('/HostingPackages/active', {
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
        void renderHostingUpsell();
        return;
    }
    const source = hostingCatalog?.find((item) => item.id === hostingId);
    if (!source) {
        return;
    }
    state.hosting.push({
        id: source.id,
        name: source.name,
        monthlyPrice: source.monthlyPrice,
        yearlyPrice: source.yearlyPrice,
        billingCycle
    });
    typedWindow.UserPanelCart?.saveState(state);
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
}
function getInputValue(id) {
    const input = document.getElementById(id);
    return input?.value.trim() ?? '';
}
async function getDefaultRegistrarCode() {
    if (defaultRegistrarCode) {
        return defaultRegistrarCode;
    }
    if (defaultRegistrarCodeRequest) {
        return defaultRegistrarCodeRequest;
    }
    const typedWindow = window;
    defaultRegistrarCodeRequest = (async () => {
        const response = await typedWindow.UserPanelApi?.request('/Registrars/active', {
            method: 'GET'
        }, true);
        if (!response || !response.success || !response.data) {
            return null;
        }
        const registrar = response.data.find((item) => item.isDefault);
        if (!registrar || !registrar.code) {
            return null;
        }
        defaultRegistrarCode = registrar.code;
        return defaultRegistrarCode;
    })();
    const result = await defaultRegistrarCodeRequest;
    defaultRegistrarCodeRequest = null;
    return result;
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
    }, true);
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
    }, true);
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
//# sourceMappingURL=domain-search.js.map
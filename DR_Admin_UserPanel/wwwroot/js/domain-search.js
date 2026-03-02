"use strict";
let latestResult = null;
let defaultRegistrarCode = null;
let defaultRegistrarCodeRequest = null;
let currentCustomerId;
function initializeDomainSearch() {
    const form = document.getElementById('domain-search-form');
    if (!form || form.dataset.bound === 'true') {
        return;
    }
    form.dataset.bound = 'true';
    const addButton = document.getElementById('domain-search-add-to-cart');
    addButton?.addEventListener('click', () => {
        addResultToCart();
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
    form.addEventListener('submit', async (event) => {
        event.preventDefault();
        const typedWindow = window;
        typedWindow.UserPanelAlerts?.hide('domain-search-alert-success');
        typedWindow.UserPanelAlerts?.hide('domain-search-alert-error');
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
        const internalDomain = await getInternalDomain(domainName);
        if (internalDomain) {
            const internalDomainOwnerId = internalDomain.customerId ?? internalDomain.CustomerId ?? null;
            const customerId = await getCurrentCustomerId();
            if (customerId !== null && internalDomainOwnerId === customerId) {
                typedWindow.UserPanelAlerts?.showError('domain-search-alert-error', 'You already own this domain in your account.');
                renderResult(null);
                return;
            }
            latestResult = {
                success: true,
                domainName,
                isAvailable: false,
                isTldSupported: true,
                message: `${domainName} is already registered.`
            };
            renderResult(latestResult);
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
        renderResult(response.data);
    });
}
function renderResult(result) {
    const card = document.getElementById('domain-search-result-card');
    const summary = document.getElementById('domain-search-result-summary');
    const addButton = document.getElementById('domain-search-add-to-cart');
    const transferButton = document.getElementById('domain-search-transfer');
    const alternativesButton = document.getElementById('domain-search-alternatives');
    const alternativesList = document.getElementById('domain-search-alternatives-list');
    if (!card || !summary || !addButton || !transferButton || !alternativesButton || !alternativesList) {
        return;
    }
    if (!result) {
        card.classList.add('d-none');
        addButton.classList.add('d-none');
        transferButton.classList.add('d-none');
        alternativesButton.classList.add('d-none');
        alternativesList.classList.add('d-none');
        alternativesList.innerHTML = '';
        summary.textContent = '';
        return;
    }
    card.classList.remove('d-none');
    if (result.isAvailable) {
        summary.textContent = `${result.domainName} is available${typeof result.premiumPrice === 'number' ? ` (Premium: ${result.premiumPrice.toFixed(2)})` : ''}.`;
        addButton.classList.remove('d-none');
        transferButton.classList.add('d-none');
        alternativesButton.classList.add('d-none');
        alternativesList.classList.add('d-none');
        alternativesList.innerHTML = '';
    }
    else {
        summary.textContent = result.message || `${result.domainName} is not available.`;
        addButton.classList.add('d-none');
        transferButton.classList.remove('d-none');
        alternativesButton.classList.remove('d-none');
        alternativesList.classList.add('d-none');
        alternativesList.innerHTML = '';
    }
}
function addResultToCart() {
    if (!latestResult || !latestResult.isAvailable) {
        return;
    }
    const typedWindow = window;
    const periodValue = Number.parseInt(getInputValue('domain-search-period'), 10);
    const periodYears = Number.isNaN(periodValue) ? 1 : periodValue;
    const privacyInput = document.getElementById('domain-search-privacy');
    const includePrivacy = !!privacyInput?.checked;
    const state = typedWindow.UserPanelCart?.getState();
    if (!state) {
        return;
    }
    if (!defaultRegistrarCode) {
        typedWindow.UserPanelAlerts?.showError('domain-search-alert-error', 'Default registrar is not configured.');
        return;
    }
    state.domain = {
        domainName: latestResult.domainName,
        registrarCode: defaultRegistrarCode,
        periodYears,
        includePrivacy,
        premiumPrice: typeof latestResult.premiumPrice === 'number' ? latestResult.premiumPrice : 0
    };
    typedWindow.UserPanelCart?.saveState(state);
    typedWindow.UserPanelAlerts?.showSuccess('domain-search-alert-success', 'Domain added to cart.');
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
async function getInternalDomain(domainName) {
    const typedWindow = window;
    const response = await typedWindow.UserPanelApi?.request(`/RegisteredDomains/name/${encodeURIComponent(domainName)}`, {
        method: 'GET'
    }, true);
    if (response?.success && response.data) {
        return response.data;
    }
    if (response?.statusCode === 404) {
        return null;
    }
    return null;
}
async function getCurrentCustomerId() {
    if (typeof currentCustomerId !== 'undefined') {
        return currentCustomerId;
    }
    const typedWindow = window;
    const response = await typedWindow.UserPanelApi?.request('/MyAccount/me', {
        method: 'GET'
    }, true);
    if (!response || !response.success || !response.data) {
        currentCustomerId = null;
        return currentCustomerId;
    }
    const customer = response.data.customer ?? response.data.Customer;
    const id = customer?.id ?? customer?.Id;
    currentCustomerId = typeof id === 'number' ? id : null;
    return currentCustomerId;
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
    const internalDomain = await getInternalDomain(domainName);
    if (internalDomain) {
        return 'taken';
    }
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
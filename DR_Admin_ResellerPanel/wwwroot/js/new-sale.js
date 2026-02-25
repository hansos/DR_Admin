"use strict";
// @ts-nocheck
(function () {
    function getApiBaseUrl() {
        var _a, _b;
        return (_b = (_a = window.AppSettings) === null || _a === void 0 ? void 0 : _a.apiBaseUrl) !== null && _b !== void 0 ? _b : '';
    }

    function getAuthToken() {
        const auth = window.Auth;
        if (auth === null || auth === void 0 ? void 0 : auth.getToken) {
            return auth.getToken();
        }

        return sessionStorage.getItem('rp_authToken');
    }

    async function apiRequest(endpoint, options = {}) {
        var _a, _b, _c;
        try {
            const headers = Object.assign({ 'Content-Type': 'application/json' }, options.headers);
            const authToken = getAuthToken();
            if (authToken) {
                headers['Authorization'] = `Bearer ${authToken}`;
            }

            const response = await fetch(endpoint, Object.assign(Object.assign({}, options), { headers, credentials: 'include' }));
            const contentType = (_a = response.headers.get('content-type')) !== null && _a !== void 0 ? _a : '';
            const hasJson = contentType.includes('application/json');
            const data = hasJson ? await response.json() : null;

            if (!response.ok) {
                return {
                    success: false,
                    message: (data && ((_b = data.message) !== null && _b !== void 0 ? _b : data.title)) || `Request failed with status ${response.status}`,
                };
            }

            return {
                success: (data === null || data === void 0 ? void 0 : data.success) !== false,
                data: (_c = data === null || data === void 0 ? void 0 : data.data) !== null && _c !== void 0 ? _c : data,
                message: data === null || data === void 0 ? void 0 : data.message,
            };
        }
        catch (error) {
            console.error('New sale request failed', error);
            return {
                success: false,
                message: 'Network error. Please try again.',
            };
        }
    }

    function extractItems(raw) {
        var _a, _b, _c, _d, _e;
        if (Array.isArray(raw)) {
            return { items: raw, meta: null };
        }

        const candidates = [raw, raw === null || raw === void 0 ? void 0 : raw.data, raw === null || raw === void 0 ? void 0 : raw.Data, (_a = raw === null || raw === void 0 ? void 0 : raw.data) === null || _a === void 0 ? void 0 : _a.data, (_b = raw === null || raw === void 0 ? void 0 : raw.data) === null || _b === void 0 ? void 0 : _b.Data];
        const items = (Array.isArray(raw === null || raw === void 0 ? void 0 : raw.Data) && raw.Data) ||
            (Array.isArray(raw === null || raw === void 0 ? void 0 : raw.data) && raw.data) ||
            (Array.isArray((_c = raw === null || raw === void 0 ? void 0 : raw.data) === null || _c === void 0 ? void 0 : _c.Data) && raw.data.Data) ||
            (Array.isArray((_d = raw === null || raw === void 0 ? void 0 : raw.data) === null || _d === void 0 ? void 0 : _d.data) && raw.data.data) ||
            (Array.isArray((_e = raw === null || raw === void 0 ? void 0 : raw.Data) === null || _e === void 0 ? void 0 : _e.Data) && raw.Data.Data) ||
            [];

        const meta = candidates.find((c) => c && typeof c === 'object' && (c.totalCount !== undefined || c.TotalCount !== undefined ||
            c.totalPages !== undefined || c.TotalPages !== undefined ||
            c.currentPage !== undefined || c.CurrentPage !== undefined ||
            c.pageSize !== undefined || c.PageSize !== undefined)) || null;

        return { items, meta };
    }

    function esc(text) {
        const map = { '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#039;' };
        return (text || '').toString().replace(/[&<>"']/g, (char) => map[char]);
    }

    function showSuccess(message) {
        const alert = document.getElementById('new-sale-alert-success');
        if (!alert) {
            return;
        }

        alert.textContent = message;
        alert.classList.remove('d-none');

        const errorAlert = document.getElementById('new-sale-alert-error');
        errorAlert === null || errorAlert === void 0 ? void 0 : errorAlert.classList.add('d-none');

        setTimeout(() => alert.classList.add('d-none'), 5000);
    }

    function showError(message) {
        const alert = document.getElementById('new-sale-alert-error');
        if (!alert) {
            return;
        }

        alert.textContent = message;
        alert.classList.remove('d-none');

        const successAlert = document.getElementById('new-sale-alert-success');
        successAlert === null || successAlert === void 0 ? void 0 : successAlert.classList.add('d-none');
    }

    let registrarOptions = [];
    let selectedRegistrarId = null;
    let selectedRegistrarLabel = '';

    function setRegistrarSelection(registrarId, registrarLabel) {
        selectedRegistrarId = registrarId ? String(registrarId) : null;
        selectedRegistrarLabel = registrarLabel || '';

        const display = document.getElementById('new-sale-registrar-display');
        if (display) {
            display.textContent = registrarLabel || 'Not selected';
        }
    }

    function showModal(id) {
        const element = document.getElementById(id);
        if (!element || !window.bootstrap) {
            return;
        }

        if (element.parentElement !== document.body) {
            document.body.appendChild(element);
        }

        const modal = new window.bootstrap.Modal(element);
        modal.show();
    }

    function hideModal(id) {
        const element = document.getElementById(id);
        if (!element || !window.bootstrap) {
            return;
        }

        const modal = window.bootstrap.Modal.getInstance(element);
        modal === null || modal === void 0 ? void 0 : modal.hide();
    }

    async function loadRegistrars() {
        const select = document.getElementById('new-sale-settings-registrar');
        if (!select) {
            return;
        }

        select.innerHTML = '<option value="">Loading registrars...</option>';
        const response = await apiRequest(`${getApiBaseUrl()}/Registrars/active`, { method: 'GET' });

        if (!response.success) {
            select.innerHTML = '<option value="">Select registrar</option>';
            showError(response.message || 'Failed to load registrars');
            setRegistrarSelection(null, 'Not selected');
            return;
        }

        const raw = response.data;
        const registrars = Array.isArray(raw) ? raw : Array.isArray(raw === null || raw === void 0 ? void 0 : raw.data) ? raw.data : [];

        if (!registrars.length) {
            select.innerHTML = '<option value="">No registrars available</option>';
            setRegistrarSelection(null, 'Not selected');
            return;
        }

        registrarOptions = registrars.map((registrar) => {
            const id = registrar.id ?? registrar.Id ?? 0;
            const name = registrar.name ?? registrar.Name ?? '';
            const code = registrar.code ?? registrar.Code ?? '';
            const isDefault = (registrar.isDefault ?? registrar.IsDefault) === true;
            const label = code ? `${name} (${code})` : name;
            return {
                id: String(id),
                label,
                isDefault,
            };
        });

        const options = registrarOptions.map((registrar) => {
            return `<option value="${registrar.id}">${esc(registrar.label)}</option>`;
        }).join('');

        select.innerHTML = `<option value="">Select registrar</option>${options}`;

        const defaultRegistrar = registrarOptions.find((registrar) => registrar.isDefault) || registrarOptions[0];
        if (defaultRegistrar) {
            select.value = defaultRegistrar.id;
            setRegistrarSelection(defaultRegistrar.id, defaultRegistrar.label);
        }
    }

    async function loadCustomers() {
        const select = document.getElementById('new-sale-customer-select');
        if (!select) {
            return;
        }

        select.innerHTML = '<option value="">Loading customers...</option>';
        const params = new URLSearchParams();
        params.set('pageNumber', '1');
        params.set('pageSize', '100');
        const response = await apiRequest(`${getApiBaseUrl()}/Customers?${params.toString()}`, { method: 'GET' });

        if (!response.success) {
            select.innerHTML = '<option value="">Select customer</option>';
            showError(response.message || 'Failed to load customers');
            return;
        }

        const raw = response.data;
        const extracted = extractItems(raw);
        const customers = extracted.items || [];

        if (!customers.length) {
            select.innerHTML = '<option value="">No customers available</option>';
            return;
        }

        const options = customers
            .map((customer) => {
                const id = customer.id ?? customer.Id ?? 0;
                const name = customer.name ?? customer.Name ?? '';
                const email = customer.email ?? customer.Email ?? '';
                const label = email ? `${name} (${email})` : name;
                return { id, label };
            })
            .sort((a, b) => a.label.localeCompare(b.label))
            .map((customer) => `<option value="${customer.id}">${esc(customer.label)}</option>`)
            .join('');

        select.innerHTML = `<option value="">Select customer</option>${options}`;
    }

    function toggleCustomerOption() {
        const existing = document.getElementById('new-sale-existing-customer');
        const newCustomer = document.getElementById('new-sale-new-customer');
        const existingRadio = document.getElementById('new-sale-customer-existing');

        if (!existing || !newCustomer || !existingRadio) {
            return;
        }

        const showExisting = existingRadio.checked;
        existing.classList.toggle('d-none', !showExisting);
        newCustomer.classList.toggle('d-none', showExisting);
    }

    function showCustomerSelection(domainName) {
        const card = document.getElementById('new-sale-customer-card');
        const selectedDomain = document.getElementById('new-sale-selected-domain');
        const existingRadio = document.getElementById('new-sale-customer-existing');

        if (!card || !selectedDomain || !existingRadio) {
            return;
        }

        selectedDomain.textContent = domainName;
        existingRadio.checked = true;
        toggleCustomerOption();
        card.classList.remove('d-none');
        card.scrollIntoView({ behavior: 'smooth', block: 'start' });
    }

    function renderSearchResult(html) {
        const target = document.getElementById('new-sale-search-result');
        if (target) {
            target.innerHTML = html;
        }
    }

    async function checkDomainAvailability() {
        const domainInput = document.getElementById('new-sale-domain-name');
        if (!domainInput) {
            return;
        }

        const domainName = domainInput.value.trim();
        const registrarId = selectedRegistrarId;

        if (!registrarId) {
            renderSearchResult(`
                <div class="alert alert-warning" role="alert">
                    <i class="bi bi-exclamation-triangle"></i> Please select a registrar.
                </div>
            `);
            return;
        }

        if (!domainName) {
            renderSearchResult(`
                <div class="alert alert-warning" role="alert">
                    <i class="bi bi-exclamation-triangle"></i> Please enter a domain name.
                </div>
            `);
            return;
        }

        renderSearchResult(`
            <div class="text-center py-3">
                <div class="spinner-border text-primary" role="status">
                    <span class="visually-hidden">Checking...</span>
                </div>
                <p class="mt-2 mb-0">Checking domain availability...</p>
            </div>
        `);

        const response = await apiRequest(`${getApiBaseUrl()}/Registrars/${registrarId}/isavailable/${encodeURIComponent(domainName)}`, { method: 'GET' });

        if (!response.success) {
            renderSearchResult(`
                <div class="alert alert-danger" role="alert">
                    <i class="bi bi-exclamation-triangle"></i> ${esc(response.message || 'Failed to check domain availability.')}
                </div>
            `);
            return;
        }

        const data = response.data || {};
        const isTldSupported = (data.isTldSupported ?? data.IsTldSupported);
        const isAvailable = (data.isAvailable ?? data.IsAvailable) === true;

        if (isTldSupported === false) {
            renderSearchResult(`
                <div class="alert alert-warning" role="alert">
                    <h6 class="alert-heading"><i class="bi bi-exclamation-triangle"></i> TLD not supported</h6>
                    <p class="mb-0">The selected registrar does not support this TLD. Try a different extension or registrar.</p>
                </div>
            `);
            return;
        }

        if (isAvailable) {
            renderSearchResult(`
                <div class="alert alert-success" role="alert">
                    <h6 class="alert-heading"><i class="bi bi-check-circle"></i> Domain available</h6>
                    <p class="mb-2"><strong>${esc(domainName)}</strong> is available for registration.</p>
                    <button type="button" class="btn btn-success" id="new-sale-register-btn" data-domain="${esc(domainName)}">
                        <i class="bi bi-arrow-right-circle"></i> Register domain
                    </button>
                </div>
            `);
            return;
        }

        renderSearchResult(`
            <div class="alert alert-danger" role="alert">
                <h6 class="alert-heading"><i class="bi bi-x-circle"></i> Domain not available</h6>
                <p><strong>${esc(domainName)}</strong> is already registered.</p>
                <p class="mb-3">You can initiate a domain transfer if the current registrant provides the authorization code. Transfers typically complete within 5-7 days after the current registrar approves the request.</p>
                <button type="button" class="btn btn-outline-primary" id="new-sale-transfer-btn" data-domain="${esc(domainName)}">
                    <i class="bi bi-arrow-left-right"></i> Transfer domain
                </button>
            </div>
        `);
    }

    function bindEvents() {
        const form = document.getElementById('new-sale-search-form');
        const searchResult = document.getElementById('new-sale-search-result');
        const existingRadio = document.getElementById('new-sale-customer-existing');
        const newRadio = document.getElementById('new-sale-customer-new');
        const openSettings = document.getElementById('new-sale-settings-open');
        const settingsSave = document.getElementById('new-sale-settings-save');
        const settingsSelect = document.getElementById('new-sale-settings-registrar');

        form === null || form === void 0 ? void 0 : form.addEventListener('submit', (event) => {
            event.preventDefault();
            checkDomainAvailability();
        });

        searchResult === null || searchResult === void 0 ? void 0 : searchResult.addEventListener('click', (event) => {
            const target = event.target;
            const button = target.closest('#new-sale-register-btn');
            if (!button) {
                return;
            }

            const domainName = button.dataset.domain || '';
            if (domainName) {
                showCustomerSelection(domainName);
                showSuccess(`Ready to register ${domainName}. Select a customer to continue.`);
            }
        });

        existingRadio === null || existingRadio === void 0 ? void 0 : existingRadio.addEventListener('change', toggleCustomerOption);
        newRadio === null || newRadio === void 0 ? void 0 : newRadio.addEventListener('change', toggleCustomerOption);

        openSettings === null || openSettings === void 0 ? void 0 : openSettings.addEventListener('click', () => {
            showModal('new-sale-settings-modal');
        });

        settingsSave === null || settingsSave === void 0 ? void 0 : settingsSave.addEventListener('click', () => {
            if (!settingsSelect) {
                return;
            }

            const registrarId = settingsSelect.value;
            if (!registrarId) {
                showError('Select a registrar to continue.');
                return;
            }

            const selected = registrarOptions.find((registrar) => registrar.id === registrarId);
            setRegistrarSelection(registrarId, selected ? selected.label : 'Selected registrar');
            hideModal('new-sale-settings-modal');
        });
    }

    function initializeNewSalePage() {
        const page = document.getElementById('dashboard-new-sale-page');
        if (!page || page.dataset.initialized === 'true') {
            return;
        }

        page.dataset.initialized = 'true';
        bindEvents();
        loadRegistrars();
        loadCustomers();
    }

    function setupPageObserver() {
        initializeNewSalePage();

        if (document.body) {
            const observer = new MutationObserver(() => {
                const page = document.getElementById('dashboard-new-sale-page');
                if (page && page.dataset.initialized !== 'true') {
                    initializeNewSalePage();
                }
            });
            observer.observe(document.body, { childList: true, subtree: true });
        }
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', setupPageObserver);
    }
    else {
        setupPageObserver();
    }
})();

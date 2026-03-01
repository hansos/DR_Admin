"use strict";
(() => {
    const storageKey = 'new-sale-state';
    let currentState = null;
    let serviceTypes = [];
    let services = [];
    let contactLookup = new Map();
    const categoryNames = ['Email hosting', 'SSL certificates', 'DNS zone packages', 'Custom services'];
    const getApiBaseUrl = () => {
        const settings = window.AppSettings;
        return settings?.apiBaseUrl ?? '';
    };
    const getAuthToken = () => {
        const auth = window.Auth;
        if (auth?.getToken) {
            return auth.getToken();
        }
        return sessionStorage.getItem('rp_authToken');
    };
    const esc = (text) => {
        const map = { '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#039;' };
        return (text || '').replace(/[&<>"']/g, (char) => map[char] ?? char);
    };
    const apiRequest = async (endpoint, options = {}) => {
        try {
            const headers = {
                'Content-Type': 'application/json',
                ...options.headers,
            };
            const authToken = getAuthToken();
            if (authToken) {
                headers.Authorization = `Bearer ${authToken}`;
            }
            const response = await fetch(endpoint, {
                ...options,
                headers,
                credentials: 'include',
            });
            const contentType = response.headers.get('content-type') ?? '';
            const hasJson = contentType.includes('application/json');
            const data = hasJson ? await response.json() : null;
            if (!response.ok) {
                return {
                    success: false,
                    message: (data && (data.message ?? data.title)) ||
                        `Request failed with status ${response.status}`,
                };
            }
            const parsed = data;
            return {
                success: parsed?.success !== false,
                data: parsed?.data ?? data,
                message: parsed?.message,
            };
        }
        catch (error) {
            console.error('New sale services request failed', error);
            return {
                success: false,
                message: 'Network error. Please try again.',
            };
        }
    };
    const loadState = () => {
        const raw = sessionStorage.getItem(storageKey);
        if (!raw) {
            return null;
        }
        try {
            return JSON.parse(raw);
        }
        catch {
            return null;
        }
    };
    const saveState = () => {
        if (!currentState) {
            return;
        }
        const otherServices = {
            selectedServiceIds: getSelectedServiceIds(),
            customServiceNotes: getTextValue('new-sale-services-custom-note') || undefined,
            transferAuthCode: getTextValue('new-sale-services-epp-code') || undefined,
            registrationPeriodYears: getRegisterPeriodYears(),
            autoRenew: getCheckboxValue('new-sale-services-auto-renew'),
            privacyProtection: getCheckboxValue('new-sale-services-privacy'),
            domainOperationPrice: readDomainOperationPrice(),
            currency: readDomainOperationCurrency(),
        };
        currentState.otherServices = otherServices;
        sessionStorage.setItem(storageKey, JSON.stringify(currentState));
    };
    const showSuccess = (message) => {
        const alert = document.getElementById('new-sale-services-alert-success');
        if (!alert) {
            return;
        }
        alert.textContent = message;
        alert.classList.remove('d-none');
        document.getElementById('new-sale-services-alert-error')?.classList.add('d-none');
        setTimeout(() => alert.classList.add('d-none'), 4000);
    };
    const showError = (message) => {
        const alert = document.getElementById('new-sale-services-alert-error');
        if (!alert) {
            return;
        }
        alert.textContent = message;
        alert.classList.remove('d-none');
        document.getElementById('new-sale-services-alert-success')?.classList.add('d-none');
    };
    const getTextValue = (id) => {
        const el = document.getElementById(id);
        return (el?.value ?? '').trim();
    };
    const setTextValue = (id, value) => {
        const el = document.getElementById(id);
        if (el) {
            el.value = value;
        }
    };
    const getCheckboxValue = (id) => {
        const el = document.getElementById(id);
        return !!el?.checked;
    };
    const setCheckboxValue = (id, value) => {
        const el = document.getElementById(id);
        if (el) {
            el.checked = value;
        }
    };
    const getRegisterPeriodYears = () => {
        const select = document.getElementById('new-sale-services-registration-period');
        const years = Number(select?.value ?? '1');
        if (!Number.isFinite(years) || years < 1 || years > 10) {
            return 1;
        }
        return years;
    };
    const setRegisterPeriodYears = (value) => {
        const select = document.getElementById('new-sale-services-registration-period');
        if (!select) {
            return;
        }
        if (value >= 1 && value <= 10) {
            select.value = String(value);
        }
    };
    const parseNumber = (value) => {
        const parsed = Number(value);
        return Number.isFinite(parsed) ? parsed : null;
    };
    const setContextHeader = () => {
        if (!currentState) {
            return;
        }
        const domain = document.getElementById('new-sale-services-domain');
        const flow = document.getElementById('new-sale-services-flow');
        const customer = document.getElementById('new-sale-services-customer');
        const hosting = document.getElementById('new-sale-services-hosting');
        if (domain) {
            domain.textContent = currentState.domainName || '-';
        }
        if (flow) {
            flow.textContent = currentState.flowType || '-';
        }
        if (customer) {
            customer.textContent = currentState.selectedCustomer?.name || currentState.selectedCustomer?.customerName || '-';
        }
        if (hosting) {
            hosting.textContent = currentState.hostingPackageId ? `Package #${currentState.hostingPackageId}` : 'Skipped';
        }
    };
    const renderFlowStatus = () => {
        const offer = currentState?.offer;
        const quoteId = document.getElementById('new-sale-services-flow-quote-id');
        const status = document.getElementById('new-sale-services-flow-status');
        const lastAction = document.getElementById('new-sale-services-flow-last-action');
        const lastRevision = document.getElementById('new-sale-services-flow-last-revision');
        if (quoteId) {
            quoteId.textContent = offer?.quoteId ? String(offer.quoteId) : '-';
        }
        if (status) {
            status.textContent = offer?.status || 'Draft';
        }
        if (lastAction) {
            lastAction.textContent = offer?.lastAction || '-';
        }
        if (lastRevision) {
            lastRevision.textContent = offer?.lastRevisionNumber ? String(offer.lastRevisionNumber) : '-';
        }
    };
    const parseListData = (raw) => {
        if (Array.isArray(raw)) {
            return raw;
        }
        const wrapped = raw;
        if (Array.isArray(wrapped?.data)) {
            return wrapped?.data ?? [];
        }
        if (Array.isArray(wrapped?.Data)) {
            return wrapped?.Data ?? [];
        }
        return [];
    };
    const normalizeServiceType = (item) => {
        const typed = (item ?? {});
        return {
            id: Number(typed.id ?? typed.Id ?? 0),
            name: String(typed.name ?? typed.Name ?? ''),
            description: String(typed.description ?? typed.Description ?? ''),
        };
    };
    const normalizeService = (item) => {
        const typed = (item ?? {});
        const price = Number(typed.price ?? typed.Price);
        return {
            id: Number(typed.id ?? typed.Id ?? 0),
            name: String(typed.name ?? typed.Name ?? ''),
            description: String(typed.description ?? typed.Description ?? ''),
            serviceTypeId: Number(typed.serviceTypeId ?? typed.ServiceTypeId ?? 0),
            price: Number.isFinite(price) ? price : null,
        };
    };
    const normalizeContact = (item) => {
        const typed = (item ?? {});
        return {
            id: Number(typed.id ?? typed.Id ?? 0),
            firstName: String(typed.firstName ?? typed.FirstName ?? ''),
            lastName: String(typed.lastName ?? typed.LastName ?? ''),
            email: String(typed.email ?? typed.Email ?? ''),
            phone: String(typed.phone ?? typed.Phone ?? ''),
        };
    };
    const normalizeTld = (item) => {
        const typed = (item ?? {});
        return {
            id: Number(typed.id ?? typed.Id ?? 0),
            extension: String(typed.extension ?? typed.Extension ?? ''),
        };
    };
    const getSelectedServiceIds = () => {
        const boxes = document.querySelectorAll('input[name="new-sale-services-item"]:checked');
        return Array.from(boxes)
            .map((item) => Number(item.value))
            .filter((id) => Number.isFinite(id) && id > 0);
    };
    const updateSelectedCount = () => {
        const badge = document.getElementById('new-sale-services-selected-count');
        if (!badge) {
            return;
        }
        const count = getSelectedServiceIds().length;
        badge.textContent = `${count} selected`;
    };
    const resolveCategoryName = (service) => {
        const typeName = serviceTypes.find((item) => item.id === service.serviceTypeId)?.name.toLowerCase() ?? '';
        const haystack = `${service.name} ${service.description} ${typeName}`.toLowerCase();
        if (haystack.includes('ssl') || haystack.includes('certificate') || haystack.includes('tls')) {
            return 'SSL certificates';
        }
        if (haystack.includes('dns') || haystack.includes('zone') || haystack.includes('nameserver')) {
            return 'DNS zone packages';
        }
        if (haystack.includes('mail') || haystack.includes('email') || haystack.includes('smtp') || haystack.includes('imap')) {
            return 'Email hosting';
        }
        return 'Custom services';
    };
    const formatMoney = (amount) => {
        if (amount === null) {
            return 'Price on request';
        }
        return amount.toFixed(2);
    };
    const renderServiceGroups = () => {
        const wrapper = document.getElementById('new-sale-services-groups');
        if (!wrapper) {
            return;
        }
        const selected = new Set(currentState?.otherServices?.selectedServiceIds ?? []);
        const grouped = new Map();
        categoryNames.forEach((name) => grouped.set(name, []));
        services.forEach((service) => {
            const category = resolveCategoryName(service);
            const current = grouped.get(category) ?? [];
            current.push(service);
            grouped.set(category, current);
        });
        wrapper.innerHTML = categoryNames.map((category) => {
            const items = grouped.get(category) ?? [];
            const content = items.length
                ? items.map((item) => {
                    const checked = selected.has(item.id) ? 'checked' : '';
                    return `
                        <div class="form-check mb-2">
                            <input class="form-check-input" type="checkbox" name="new-sale-services-item" id="new-sale-service-item-${item.id}" value="${item.id}" ${checked} />
                            <label class="form-check-label" for="new-sale-service-item-${item.id}">
                                <span class="fw-semibold">${esc(item.name)}</span>
                                <span class="text-muted d-block small">${esc(item.description || 'No description')}</span>
                                <span class="small text-muted">Price: ${esc(formatMoney(item.price))}</span>
                            </label>
                        </div>
                    `;
                }).join('')
                : '<div class="text-muted small">No services available.</div>';
            return `
                <div class="col-12 col-lg-6">
                    <div class="border rounded p-3 h-100">
                        <h6 class="mb-2">${esc(category)}</h6>
                        ${content}
                    </div>
                </div>
            `;
        }).join('');
        updateSelectedCount();
    };
    const loadServiceCatalog = async () => {
        const status = document.getElementById('new-sale-services-status');
        if (status) {
            status.innerHTML = '<span class="spinner-border spinner-border-sm me-1"></span>Loading additional services...';
        }
        const [serviceTypesResponse, servicesResponse] = await Promise.all([
            apiRequest(`${getApiBaseUrl()}/ServiceTypes`, { method: 'GET' }),
            apiRequest(`${getApiBaseUrl()}/Services`, { method: 'GET' }),
        ]);
        if (!serviceTypesResponse.success || !servicesResponse.success) {
            serviceTypes = [];
            services = [];
            renderServiceGroups();
            if (status) {
                status.innerHTML = '<span class="text-danger">Failed to load service catalog.</span>';
            }
            showError(serviceTypesResponse.message || servicesResponse.message || 'Failed to load service catalog.');
            return;
        }
        serviceTypes = parseListData(serviceTypesResponse.data)
            .map((item) => normalizeServiceType(item))
            .filter((item) => item.id > 0);
        services = parseListData(servicesResponse.data)
            .map((item) => normalizeService(item))
            .filter((item) => item.id > 0);
        renderServiceGroups();
        if (status) {
            status.textContent = `${services.length} service option(s) loaded.`;
        }
    };
    const getFlowType = () => (currentState?.flowType ?? '').toLowerCase();
    const setFlowSpecificVisibility = () => {
        const flowType = getFlowType();
        const transferCard = document.getElementById('new-sale-services-transfer-card');
        const registerCard = document.getElementById('new-sale-services-register-card');
        const isTransfer = flowType === 'transfer';
        const isRegister = flowType === 'register';
        transferCard?.classList.toggle('d-none', !isTransfer);
        registerCard?.classList.toggle('d-none', !isRegister);
    };
    const getDomainTld = () => {
        const domain = (currentState?.domainName ?? '').trim().toLowerCase();
        if (!domain || !domain.includes('.')) {
            return '';
        }
        const parts = domain.split('.').filter((part) => part.length > 0);
        return parts.length >= 2 ? parts[parts.length - 1] : '';
    };
    const resolveOperationType = () => {
        const flowType = getFlowType();
        if (flowType === 'transfer') {
            return 'Transfer';
        }
        if (flowType === 'renew') {
            return 'Renewal';
        }
        return 'Registration';
    };
    const setDomainPriceText = (text) => {
        const el = document.getElementById('new-sale-services-domain-price');
        if (el) {
            el.textContent = text;
        }
    };
    const readDomainOperationPrice = () => {
        const raw = document.getElementById('new-sale-services-domain-price')?.getAttribute('data-price') ?? '';
        return parseNumber(raw);
    };
    const readDomainOperationCurrency = () => {
        const raw = document.getElementById('new-sale-services-domain-price')?.getAttribute('data-currency') ?? '';
        return raw || undefined;
    };
    const setDomainOperationPriceData = (price, currency) => {
        const el = document.getElementById('new-sale-services-domain-price');
        if (!el) {
            return;
        }
        if (price === null) {
            el.removeAttribute('data-price');
        }
        else {
            el.setAttribute('data-price', String(price));
        }
        if (currency) {
            el.setAttribute('data-currency', currency);
        }
        else {
            el.removeAttribute('data-currency');
        }
    };
    const loadRegisterPricing = async () => {
        if (getFlowType() !== 'register') {
            return;
        }
        const tld = getDomainTld();
        if (!tld) {
            setDomainPriceText('-');
            setDomainOperationPriceData(null, undefined);
            return;
        }
        const tldResponse = await apiRequest(`${getApiBaseUrl()}/Tlds/extension/${encodeURIComponent(tld)}`, { method: 'GET' });
        if (!tldResponse.success) {
            setDomainPriceText('Unable to resolve TLD pricing.');
            setDomainOperationPriceData(null, undefined);
            return;
        }
        const normalizedTld = normalizeTld(tldResponse.data);
        if (normalizedTld.id <= 0) {
            setDomainPriceText('No pricing configured for selected TLD.');
            setDomainOperationPriceData(null, undefined);
            return;
        }
        const years = getRegisterPeriodYears();
        const pricingResponse = await apiRequest(`${getApiBaseUrl()}/tld-pricing/calculate`, {
            method: 'POST',
            body: JSON.stringify({
                tldId: normalizedTld.id,
                operationType: resolveOperationType(),
                years,
                isFirstYear: true,
            }),
        });
        if (!pricingResponse.success) {
            setDomainPriceText('Unable to calculate domain price.');
            setDomainOperationPriceData(null, undefined);
            return;
        }
        const raw = pricingResponse.data ?? {};
        const finalPrice = Number(raw.finalPrice ?? raw.FinalPrice);
        const currency = String(raw.currency ?? raw.Currency ?? currentState?.pricing?.currency ?? 'USD');
        if (!Number.isFinite(finalPrice)) {
            setDomainPriceText('Unable to calculate domain price.');
            setDomainOperationPriceData(null, currency);
            return;
        }
        setDomainPriceText(`${finalPrice.toFixed(2)} ${currency} / ${years} year(s)`);
        setDomainOperationPriceData(finalPrice, currency);
    };
    const loadContactsSummary = async () => {
        const summary = document.getElementById('new-sale-services-contacts-summary');
        if (!summary) {
            return;
        }
        if (!currentState?.selectedCustomer?.id) {
            summary.innerHTML = '<div class="text-muted">No customer selected.</div>';
            return;
        }
        const response = await apiRequest(`${getApiBaseUrl()}/ContactPersons/customer/${currentState.selectedCustomer.id}`, {
            method: 'GET',
        });
        if (!response.success) {
            summary.innerHTML = '<div class="text-danger">Failed to load contact details.</div>';
            return;
        }
        const contacts = parseListData(response.data)
            .map((item) => normalizeContact(item))
            .filter((item) => item.id > 0);
        contactLookup = new Map();
        contacts.forEach((contact) => {
            contactLookup.set(contact.id, contact);
        });
        const ids = currentState.domainContacts ?? {};
        const roles = [
            { role: 'Registrant', id: ids.registrantContactId },
            { role: 'Administrative', id: ids.adminContactId },
            { role: 'Technical', id: ids.techContactId },
            { role: 'Billing', id: ids.billingContactId },
        ];
        summary.innerHTML = `
            <div class="row g-3">
                ${roles.map((role) => {
            const contact = role.id ? contactLookup.get(role.id) : undefined;
            if (!contact) {
                return `
                            <div class="col-12 col-md-6">
                                <div class="text-muted small">${esc(role.role)}</div>
                                <div class="fw-semibold">-</div>
                            </div>
                        `;
            }
            const fullName = `${contact.firstName} ${contact.lastName}`.trim() || contact.email || `Contact #${contact.id}`;
            return `
                        <div class="col-12 col-md-6">
                            <div class="text-muted small">${esc(role.role)}</div>
                            <div class="fw-semibold">${esc(fullName)}</div>
                            <div class="small text-muted">${esc(contact.email || '-')} · ${esc(contact.phone || '-')}</div>
                        </div>
                    `;
        }).join('')}
            </div>
        `;
    };
    const restoreOtherServicesState = () => {
        const saved = currentState?.otherServices;
        if (!saved) {
            setFlowSpecificVisibility();
            return;
        }
        setTextValue('new-sale-services-custom-note', saved.customServiceNotes ?? '');
        setTextValue('new-sale-services-epp-code', saved.transferAuthCode ?? '');
        setCheckboxValue('new-sale-services-auto-renew', saved.autoRenew !== false);
        setCheckboxValue('new-sale-services-privacy', saved.privacyProtection === true);
        setRegisterPeriodYears(saved.registrationPeriodYears ?? 1);
        if (typeof saved.domainOperationPrice === 'number' && Number.isFinite(saved.domainOperationPrice)) {
            const currency = saved.currency ?? 'USD';
            setDomainPriceText(`${saved.domainOperationPrice.toFixed(2)} ${currency}`);
            setDomainOperationPriceData(saved.domainOperationPrice, currency);
        }
        setFlowSpecificVisibility();
    };
    const validateBeforeContinue = () => {
        const flowType = getFlowType();
        if (flowType === 'transfer') {
            const eppCode = getTextValue('new-sale-services-epp-code');
            if (eppCode.length < 6) {
                showError('A valid EPP/auth code is required for transfers.');
                return false;
            }
        }
        if (flowType === 'register') {
            const years = getRegisterPeriodYears();
            if (years < 1 || years > 10) {
                showError('Registration period must be between 1 and 10 years.');
                return false;
            }
        }
        return true;
    };
    const proceedToPage5 = () => {
        if (!validateBeforeContinue()) {
            return;
        }
        saveState();
        window.location.href = '/dashboard/new-sale/offer';
    };
    const skipAddOns = () => {
        if (!validateBeforeContinue()) {
            return;
        }
        document.querySelectorAll('input[name="new-sale-services-item"]').forEach((item) => {
            item.checked = false;
        });
        updateSelectedCount();
        saveState();
        showSuccess('Additional services skipped.');
        window.location.href = '/dashboard/new-sale/offer';
    };
    const bindEvents = () => {
        document.getElementById('new-sale-services-groups')?.addEventListener('change', (event) => {
            const target = event.target;
            if (target.name === 'new-sale-services-item') {
                updateSelectedCount();
                saveState();
            }
        });
        document.getElementById('new-sale-services-custom-note')?.addEventListener('input', saveState);
        document.getElementById('new-sale-services-epp-code')?.addEventListener('input', saveState);
        document.getElementById('new-sale-services-auto-renew')?.addEventListener('change', saveState);
        document.getElementById('new-sale-services-privacy')?.addEventListener('change', saveState);
        document.getElementById('new-sale-services-registration-period')?.addEventListener('change', () => {
            saveState();
            void loadRegisterPricing();
        });
        document.getElementById('new-sale-services-next')?.addEventListener('click', proceedToPage5);
        document.getElementById('new-sale-services-skip')?.addEventListener('click', skipAddOns);
    };
    const initializePage = async () => {
        const page = document.getElementById('dashboard-new-sale-services-page');
        if (!page || page.dataset.initialized === 'true') {
            return;
        }
        page.dataset.initialized = 'true';
        currentState = loadState();
        if (!currentState?.domainName || !currentState?.flowType || !currentState?.selectedCustomer) {
            window.location.href = '/dashboard/new-sale';
            return;
        }
        setContextHeader();
        renderFlowStatus();
        restoreOtherServicesState();
        bindEvents();
        await Promise.all([
            loadServiceCatalog(),
            loadContactsSummary(),
        ]);
        await loadRegisterPricing();
    };
    const setupObserver = () => {
        void initializePage();
        if (document.body) {
            const observer = new MutationObserver(() => {
                const page = document.getElementById('dashboard-new-sale-services-page');
                if (page && page.dataset.initialized !== 'true') {
                    void initializePage();
                }
            });
            observer.observe(document.body, { childList: true, subtree: true });
        }
    };
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', setupObserver);
    }
    else {
        setupObserver();
    }
})();
//# sourceMappingURL=new-sale-services.js.map
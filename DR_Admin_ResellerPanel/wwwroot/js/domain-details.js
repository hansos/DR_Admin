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
            console.error('Domain details request failed', error);
            return {
                success: false,
                message: 'Network error. Please try again.',
            };
        }
    }
    let domainId = null;
    let currentDomain = null;
    let currentRegistrarCode = null;
    let currentDomainName = null;
    function initializePage() {
        var _a, _b, _c, _d, _e, _f;
        const page = document.getElementById('domain-details-page');
        if (!page || page.dataset.initialized === 'true') {
            return;
        }
        page.dataset.initialized = 'true';
        const params = new URLSearchParams(window.location.search);
        const idParam = params.get('id');
        const parsed = Number(idParam);
        (_a = document.getElementById('domain-dns-sync')) === null || _a === void 0 ? void 0 : _a.addEventListener('click', openDnsSyncModal);
        (_b = document.getElementById('domain-dns-sync-confirm')) === null || _b === void 0 ? void 0 : _b.addEventListener('click', syncDnsRecords);
        (_c = document.getElementById('domain-details-select')) === null || _c === void 0 ? void 0 : _c.addEventListener('click', openSelectDomainModal);
        (_d = document.getElementById('domain-details-manual-load')) === null || _d === void 0 ? void 0 : _d.addEventListener('click', loadDomainFromManualInput);
        (_e = document.getElementById('domain-dns-open-zones')) === null || _e === void 0 ? void 0 : _e.addEventListener('click', openDnsZones);
        (_f = document.getElementById('domain-dns-open-zones')) === null || _f === void 0 ? void 0 : _f.addEventListener('click', openDnsZones);
        if (!idParam || !Number.isFinite(parsed) || parsed <= 0) {
            showManualEntry();
            return;
        }
        function openDnsZones() {
            if (!domainId) {
                showError('Select a domain before opening DNS zones.');
                return;
            }
            window.location.href = `/dns/zones?domain-id=${encodeURIComponent(String(domainId))}`;
        }
        function openDnsZones() {
            if (!domainId) {
                showError('Select a domain before opening DNS zones.');
                return;
            }
            window.location.href = `/dns/zones?domain-id=${encodeURIComponent(String(domainId))}`;
        }
        domainId = parsed;
        loadDomainDetails();
    }
    function showManualEntry() {
        var _a;
        setLoading(false);
        const manualCard = document.getElementById('domain-details-empty');
        manualCard === null || manualCard === void 0 ? void 0 : manualCard.classList.remove('d-none');
        (_a = document.getElementById('domain-details-content')) === null || _a === void 0 ? void 0 : _a.classList.add('d-none');
        setSelectedDomainLabel(null);
        openSelectDomainModal();
    }
    function openSelectDomainModal() {
        const input = document.getElementById('domain-details-manual-name');
        if (input) {
            input.value = currentDomainName !== null && currentDomainName !== void 0 ? currentDomainName : '';
        }
        showModal('domain-details-select-modal');
        input === null || input === void 0 ? void 0 : input.focus();
        input === null || input === void 0 ? void 0 : input.addEventListener('keydown', (event) => {
            if (event.key === 'Enter') {
                event.preventDefault();
                loadDomainFromManualInput();
            }
        });
    }
    async function loadDomainFromManualInput() {
        var _a, _b;
        const input = document.getElementById('domain-details-manual-name');
        const name = ((_a = input === null || input === void 0 ? void 0 : input.value) !== null && _a !== void 0 ? _a : '').trim();
        if (!name) {
            showError('Enter a domain name to load.');
            return;
        }
        clearAlerts();
        setLoading(true);
        const response = await apiRequest(`${getApiBaseUrl()}/RegisteredDomains/name/${encodeURIComponent(name)}`, { method: 'GET' });
        if (!response.success || !response.data) {
            setLoading(false);
            showError(response.message || 'Domain not found.');
            return;
        }
        currentDomain = normalizeDomain(response.data);
        domainId = currentDomain.id;
        updateDomainFields(currentDomain);
        setLoading(false);
        (_b = document.getElementById('domain-details-empty')) === null || _b === void 0 ? void 0 : _b.classList.add('d-none');
        hideModal('domain-details-select-modal');
        await loadDnsRecords();
        await loadDomainContacts();
    }
    function setLoading(isLoading) {
        const loading = document.getElementById('domain-details-loading');
        const content = document.getElementById('domain-details-content');
        if (loading) {
            loading.classList.toggle('d-none', !isLoading);
        }
        if (content) {
            content.classList.toggle('d-none', isLoading);
        }
    }
    function showSuccess(message) {
        const alert = document.getElementById('domain-details-alert-success');
        if (!alert) {
            return;
        }
        alert.textContent = message;
        alert.classList.remove('d-none');
        const errorAlert = document.getElementById('domain-details-alert-error');
        errorAlert === null || errorAlert === void 0 ? void 0 : errorAlert.classList.add('d-none');
    }
    function showError(message) {
        const alert = document.getElementById('domain-details-alert-error');
        if (!alert) {
            return;
        }
        alert.textContent = message;
        alert.classList.remove('d-none');
        const successAlert = document.getElementById('domain-details-alert-success');
        successAlert === null || successAlert === void 0 ? void 0 : successAlert.classList.add('d-none');
    }
    function clearAlerts() {
        var _a, _b;
        (_a = document.getElementById('domain-details-alert-success')) === null || _a === void 0 ? void 0 : _a.classList.add('d-none');
        (_b = document.getElementById('domain-details-alert-error')) === null || _b === void 0 ? void 0 : _b.classList.add('d-none');
    }
    function normalizeDomain(item) {
        var _a, _b, _c, _d, _e, _f, _g, _h, _j, _k, _l, _m, _o, _p, _q, _r, _s, _t, _u, _v, _w, _x, _y, _z;
        return {
            id: (_b = (_a = item.id) !== null && _a !== void 0 ? _a : item.Id) !== null && _b !== void 0 ? _b : 0,
            customerId: (_d = (_c = item.customerId) !== null && _c !== void 0 ? _c : item.CustomerId) !== null && _d !== void 0 ? _d : 0,
            serviceId: (_f = (_e = item.serviceId) !== null && _e !== void 0 ? _e : item.ServiceId) !== null && _f !== void 0 ? _f : 0,
            name: (_h = (_g = item.name) !== null && _g !== void 0 ? _g : item.Name) !== null && _h !== void 0 ? _h : '',
            providerId: (_k = (_j = item.providerId) !== null && _j !== void 0 ? _j : item.ProviderId) !== null && _k !== void 0 ? _k : 0,
            status: (_m = (_l = item.status) !== null && _l !== void 0 ? _l : item.Status) !== null && _m !== void 0 ? _m : '',
            registrationDate: (_p = (_o = item.registrationDate) !== null && _o !== void 0 ? _o : item.RegistrationDate) !== null && _p !== void 0 ? _p : '',
            expirationDate: (_r = (_q = item.expirationDate) !== null && _q !== void 0 ? _q : item.ExpirationDate) !== null && _r !== void 0 ? _r : '',
            createdAt: (_t = (_s = item.createdAt) !== null && _s !== void 0 ? _s : item.CreatedAt) !== null && _t !== void 0 ? _t : null,
            updatedAt: (_v = (_u = item.updatedAt) !== null && _u !== void 0 ? _u : item.UpdatedAt) !== null && _v !== void 0 ? _v : null,
            customer: (_x = (_w = item.customer) !== null && _w !== void 0 ? _w : item.Customer) !== null && _x !== void 0 ? _x : null,
            registrar: (_z = (_y = item.registrar) !== null && _y !== void 0 ? _y : item.Registrar) !== null && _z !== void 0 ? _z : null,
        };
    }
    async function loadDomainDetails() {
        if (!domainId) {
            return;
        }
        clearAlerts();
        setLoading(true);
        const response = await apiRequest(`${getApiBaseUrl()}/RegisteredDomains/${domainId}`, { method: 'GET' });
        if (!response.success || !response.data) {
            setLoading(false);
            showError(response.message || 'Failed to load domain details.');
            return;
        }
        currentDomain = normalizeDomain(response.data);
        updateDomainFields(currentDomain);
        setLoading(false);
        await loadDnsRecords();
        await loadDomainContacts();
    }
    function updateDomainFields(domain) {
        var _a, _b, _c, _d, _e, _f, _g, _h, _j, _k, _l, _m;
        currentDomainName = domain.name || null;
        const customerName = (_d = (_b = (_a = domain.customer) === null || _a === void 0 ? void 0 : _a.name) !== null && _b !== void 0 ? _b : (_c = domain.customer) === null || _c === void 0 ? void 0 : _c.Name) !== null && _d !== void 0 ? _d : '';
        const registrarName = (_h = (_f = (_e = domain.registrar) === null || _e === void 0 ? void 0 : _e.name) !== null && _f !== void 0 ? _f : (_g = domain.registrar) === null || _g === void 0 ? void 0 : _g.Name) !== null && _h !== void 0 ? _h : '';
        currentRegistrarCode = (_m = (_k = (_j = domain.registrar) === null || _j === void 0 ? void 0 : _j.code) !== null && _k !== void 0 ? _k : (_l = domain.registrar) === null || _l === void 0 ? void 0 : _l.Code) !== null && _m !== void 0 ? _m : null;
        const registrarDisplay = registrarName
            ? `${registrarName}${currentRegistrarCode ? ` (${currentRegistrarCode})` : ''}`
            : currentRegistrarCode
                ? currentRegistrarCode
                : domain.providerId
                    ? `#${domain.providerId}`
                    : '-';
        const customerDisplay = customerName
            ? `${customerName} (#${domain.customerId})`
            : domain.customerId
                ? `#${domain.customerId}`
                : '-';
        setText('domain-details-name', domain.name || '-');
        setText('domain-details-id', String(domain.id || '-'));
        setText('domain-details-status', domain.status || '-');
        setText('domain-details-registrar', registrarDisplay);
        setText('domain-details-customer', customerDisplay);
        setText('domain-details-service', domain.serviceId ? String(domain.serviceId) : '-');
        setText('domain-details-registered', domain.registrationDate ? formatDate(domain.registrationDate) : '-');
        setText('domain-details-expires', domain.expirationDate ? formatDate(domain.expirationDate) : '-');
        const downloadButton = document.getElementById('domain-dns-sync');
        if (downloadButton) {
            downloadButton.disabled = !currentRegistrarCode || !currentDomainName;
        }
        setSelectedDomainLabel(currentDomainName);
    }
    function setSelectedDomainLabel(domainName) {
        const selectButton = document.getElementById('domain-details-select');
        if (!selectButton) {
            return;
        }
        selectButton.innerHTML = domainName
            ? `<i class="bi bi-search"></i> ${esc(domainName)}`
            : '<i class="bi bi-search"></i> Select domain';
    }
    async function loadDnsRecords() {
        const loading = document.getElementById('domain-dns-records-loading');
        const content = document.getElementById('domain-dns-records-content');
        const empty = document.getElementById('domain-dns-records-empty');
        if (loading) {
            loading.classList.remove('d-none');
        }
        content === null || content === void 0 ? void 0 : content.classList.add('d-none');
        empty === null || empty === void 0 ? void 0 : empty.classList.add('d-none');
        if (!domainId) {
            return;
        }
        const response = await apiRequest(`${getApiBaseUrl()}/DnsRecords/domain/${domainId}`, { method: 'GET' });
        if (!response.success) {
            loading === null || loading === void 0 ? void 0 : loading.classList.add('d-none');
            empty === null || empty === void 0 ? void 0 : empty.classList.remove('d-none');
            setRecordCount(0);
            return;
        }
        const records = Array.isArray(response.data) ? response.data : [];
        renderDnsRecords(records);
    }
    async function loadDomainContacts() {
        const loading = document.getElementById('domain-contacts-loading');
        const content = document.getElementById('domain-contacts-content');
        const empty = document.getElementById('domain-contacts-empty');
        loading === null || loading === void 0 ? void 0 : loading.classList.remove('d-none');
        content === null || content === void 0 ? void 0 : content.classList.add('d-none');
        empty === null || empty === void 0 ? void 0 : empty.classList.add('d-none');
        if (!domainId) {
            loading === null || loading === void 0 ? void 0 : loading.classList.add('d-none');
            empty === null || empty === void 0 ? void 0 : empty.classList.remove('d-none');
            return;
        }
        const response = await apiRequest(`${getApiBaseUrl()}/DomainContacts/domain/${domainId}`, { method: 'GET' });
        if (!response.success) {
            loading === null || loading === void 0 ? void 0 : loading.classList.add('d-none');
            empty === null || empty === void 0 ? void 0 : empty.classList.remove('d-none');
            return;
        }
        const raw = response.data;
        const contacts = Array.isArray(raw)
            ? raw
            : Array.isArray(raw === null || raw === void 0 ? void 0 : raw.data)
                ? raw.data
                : Array.isArray(raw === null || raw === void 0 ? void 0 : raw.Data)
                    ? raw.Data
                    : [];
        renderDomainContacts(contacts);
    }
    function renderDomainContacts(contacts) {
        const loading = document.getElementById('domain-contacts-loading');
        const content = document.getElementById('domain-contacts-content');
        const empty = document.getElementById('domain-contacts-empty');
        const accordion = document.getElementById('domain-contacts-accordion');
        loading === null || loading === void 0 ? void 0 : loading.classList.add('d-none');
        if (!accordion) {
            return;
        }
        if (!contacts.length) {
            accordion.innerHTML = '';
            empty === null || empty === void 0 ? void 0 : empty.classList.remove('d-none');
            content === null || content === void 0 ? void 0 : content.classList.add('d-none');
            return;
        }
        const contactOrder = { Registrant: 1, Administrative: 2, Technical: 3, Billing: 4 };
        contacts.sort((a, b) => { var _a, _b, _c, _d; return ((_b = contactOrder[(_a = a.contactType) !== null && _a !== void 0 ? _a : '']) !== null && _b !== void 0 ? _b : 999) - ((_d = contactOrder[(_c = b.contactType) !== null && _c !== void 0 ? _c : '']) !== null && _d !== void 0 ? _d : 999); });
        accordion.innerHTML = contacts.map((contact, index) => {
            var _a, _b, _c, _d, _e, _f, _g, _h, _j, _k, _l, _m;
            const contactId = `domain-contact-${index}`;
            const isFirst = index === 0;
            const type = contact.contactType || 'Contact';
            const fullName = `${(_a = contact.firstName) !== null && _a !== void 0 ? _a : ''} ${(_b = contact.lastName) !== null && _b !== void 0 ? _b : ''}`.trim() || '-';
            const typeInfo = getContactTypeInfo(type);
            return `
        <div class="accordion-item">
            <h2 class="accordion-header" id="heading-${contactId}">
                <button class="accordion-button ${isFirst ? '' : 'collapsed'}" type="button"
                        data-bs-toggle="collapse" data-bs-target="#collapse-${contactId}"
                        aria-expanded="${isFirst}" aria-controls="collapse-${contactId}">
                    <i class="bi ${typeInfo.icon} ${typeInfo.color} me-2"></i>
                    <strong>${esc(type)}</strong> - ${esc(fullName)}
                </button>
            </h2>
            <div id="collapse-${contactId}" class="accordion-collapse collapse ${isFirst ? 'show' : ''}"
                 aria-labelledby="heading-${contactId}" data-bs-parent="#domain-contacts-accordion">
                <div class="accordion-body">
                    <div class="row">
                        <div class="col-md-6">
                            <table class="table table-sm table-borderless">
                                <tr><th width="40%">First Name:</th><td>${esc((_c = contact.firstName) !== null && _c !== void 0 ? _c : '-')}</td></tr>
                                <tr><th>Last Name:</th><td>${esc((_d = contact.lastName) !== null && _d !== void 0 ? _d : '-')}</td></tr>
                                <tr><th>Organization:</th><td>${esc((_e = contact.organization) !== null && _e !== void 0 ? _e : '-')}</td></tr>
                                <tr><th>Email:</th><td>${contact.email ? `<a href="mailto:${esc(contact.email)}">${esc(contact.email)}</a>` : '-'}</td></tr>
                                <tr><th>Phone:</th><td>${contact.phone ? `<a href="tel:${esc(contact.phone)}">${esc(contact.phone)}</a>` : '-'}</td></tr>
                                <tr><th>Fax:</th><td>${esc((_f = contact.fax) !== null && _f !== void 0 ? _f : '-')}</td></tr>
                            </table>
                        </div>
                        <div class="col-md-6">
                            <table class="table table-sm table-borderless">
                                <tr><th width="40%">Address Line 1:</th><td>${esc((_g = contact.addressLine1) !== null && _g !== void 0 ? _g : '-')}</td></tr>
                                <tr><th>Address Line 2:</th><td>${esc((_h = contact.addressLine2) !== null && _h !== void 0 ? _h : '-')}</td></tr>
                                <tr><th>City:</th><td>${esc((_j = contact.city) !== null && _j !== void 0 ? _j : '-')}</td></tr>
                                <tr><th>State/Province:</th><td>${esc((_k = contact.stateProvince) !== null && _k !== void 0 ? _k : '-')}</td></tr>
                                <tr><th>Postal Code:</th><td>${esc((_l = contact.postalCode) !== null && _l !== void 0 ? _l : '-')}</td></tr>
                                <tr><th>Country:</th><td>${esc((_m = contact.country) !== null && _m !== void 0 ? _m : '-')}</td></tr>
                            </table>
                        </div>
                    </div>
                </div>
            </div>
        </div>`;
        }).join('');
        empty === null || empty === void 0 ? void 0 : empty.classList.add('d-none');
        content === null || content === void 0 ? void 0 : content.classList.remove('d-none');
    }
    function getContactTypeInfo(contactType) {
        var _a;
        const typeMap = {
            Registrant: { icon: 'bi-person-badge', color: 'text-primary' },
            Administrative: { icon: 'bi-person-gear', color: 'text-success' },
            Technical: { icon: 'bi-person-workspace', color: 'text-info' },
            Billing: { icon: 'bi-credit-card', color: 'text-warning' },
        };
        return (_a = typeMap[contactType]) !== null && _a !== void 0 ? _a : { icon: 'bi-person', color: 'text-secondary' };
    }
    function renderDnsRecords(records) {
        const loading = document.getElementById('domain-dns-records-loading');
        const content = document.getElementById('domain-dns-records-content');
        const empty = document.getElementById('domain-dns-records-empty');
        const tableBody = document.getElementById('domain-dns-records-table-body');
        loading === null || loading === void 0 ? void 0 : loading.classList.add('d-none');
        if (!tableBody) {
            return;
        }
        if (!records.length) {
            tableBody.innerHTML = '';
            empty === null || empty === void 0 ? void 0 : empty.classList.remove('d-none');
            content === null || content === void 0 ? void 0 : content.classList.add('d-none');
            setRecordCount(0);
            return;
        }
        tableBody.innerHTML = records.map((record) => {
            var _a, _b, _c, _d;
            return `
        <tr>
            <td>${esc(record.type || '-')}</td>
            <td>${esc(record.name || '-')}</td>
            <td>${esc(record.value || '-')}</td>
            <td>${(_a = record.ttl) !== null && _a !== void 0 ? _a : '-'}</td>
            <td>${(_b = record.priority) !== null && _b !== void 0 ? _b : '-'}</td>
            <td>${(_c = record.weight) !== null && _c !== void 0 ? _c : '-'}</td>
            <td>${(_d = record.port) !== null && _d !== void 0 ? _d : '-'}</td>
        </tr>`;
        }).join('');
        setRecordCount(records.length);
        empty === null || empty === void 0 ? void 0 : empty.classList.add('d-none');
        content === null || content === void 0 ? void 0 : content.classList.remove('d-none');
    }
    function setRecordCount(count) {
        const badge = document.getElementById('domain-dns-record-count');
        if (badge) {
            badge.textContent = `${count} record${count === 1 ? '' : 's'}`;
        }
    }
    function openDnsSyncModal() {
        if (!currentRegistrarCode || !currentDomainName) {
            showError('Registrar code or domain name is missing for sync.');
            return;
        }
        setSyncSummary('');
        setSyncBusy(false);
        setText('domain-dns-sync-domain', currentDomainName || '-');
        setText('domain-dns-sync-registrar', currentRegistrarCode || '-');
        showModal('domain-dns-sync-modal');
    }
    function setSyncBusy(isBusy) {
        const progress = document.getElementById('domain-dns-sync-progress');
        const confirm = document.getElementById('domain-dns-sync-confirm');
        if (progress) {
            progress.classList.toggle('d-none', !isBusy);
        }
        if (confirm) {
            confirm.disabled = isBusy;
        }
    }
    function setSyncSummary(message) {
        const summary = document.getElementById('domain-dns-sync-summary');
        if (!summary) {
            return;
        }
        if (!message) {
            summary.textContent = '';
            summary.classList.add('d-none');
            return;
        }
        summary.textContent = message;
        summary.classList.remove('d-none');
    }
    async function syncDnsRecords() {
        if (!currentRegistrarCode || !currentDomainName) {
            showError('Registrar code or domain name is missing for sync.');
            return;
        }
        setSyncSummary('');
        setSyncBusy(true);
        const endpoint = `${getApiBaseUrl()}/DomainManager/registrar/${encodeURIComponent(currentRegistrarCode)}/domain/name/${encodeURIComponent(currentDomainName)}/dns-records/sync`;
        const response = await apiRequest(endpoint, { method: 'POST' });
        setSyncBusy(false);
        if (!response.success) {
            setSyncSummary(response.message || 'Sync failed.');
            return;
        }
        setSyncSummary(response.message || 'DNS records synced from registrar.');
        showSuccess('DNS records synced successfully.');
        await loadDnsRecords();
    }
    function setText(id, value) {
        const el = document.getElementById(id);
        if (el) {
            el.textContent = value;
        }
    }
    function esc(text) {
        const map = { '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#039;' };
        return (text || '').toString().replace(/[&<>"']/g, (char) => map[char]);
    }
    function formatDate(value) {
        const date = new Date(value);
        if (Number.isNaN(date.getTime())) {
            return value;
        }
        return date.toLocaleString();
    }
    function showModal(id) {
        const element = document.getElementById(id);
        if (!element || !window.bootstrap) {
            return;
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
    function setupPageObserver() {
        initializePage();
        if (document.body) {
            const observer = new MutationObserver(() => {
                const page = document.getElementById('domain-details-page');
                if (page && page.dataset.initialized !== 'true') {
                    initializePage();
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
//# sourceMappingURL=domain-details.js.map
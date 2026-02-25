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
        var _a, _b;
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
        if (!idParam || !Number.isFinite(parsed) || parsed <= 0) {
            showManualEntry();
            return;
        }
        domainId = parsed;
        loadDomainDetails();
    }
    function showManualEntry() {
        const manualCard = document.getElementById('domain-details-empty');
        setLoading(false);
        manualCard === null || manualCard === void 0 ? void 0 : manualCard.classList.remove('d-none');
        document.getElementById('domain-details-content') === null || document.getElementById('domain-details-content') === void 0 ? void 0 : document.getElementById('domain-details-content').classList.add('d-none');
        const loadButton = document.getElementById('domain-details-manual-load');
        loadButton === null || loadButton === void 0 ? void 0 : loadButton.addEventListener('click', loadDomainFromManualInput);
        const input = document.getElementById('domain-details-manual-name');
        input === null || input === void 0 ? void 0 : input.addEventListener('keydown', (event) => {
            if (event.key === 'Enter') {
                event.preventDefault();
                loadDomainFromManualInput();
            }
        });
        input === null || input === void 0 ? void 0 : input.focus();
    }
    async function loadDomainFromManualInput() {
        const input = document.getElementById('domain-details-manual-name');
        const name = (input === null || input === void 0 ? void 0 : input.value) ? input.value.trim() : '';
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
        document.getElementById('domain-details-empty') === null || document.getElementById('domain-details-empty') === void 0 ? void 0 : document.getElementById('domain-details-empty').classList.add('d-none');
        await loadDnsRecords();
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
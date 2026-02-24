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
            console.error('Expiring domains request failed', error);
            return {
                success: false,
                message: 'Network error. Please try again.',
            };
        }
    }
    function extractItems(raw) {
        var _a, _b, _c, _d, _e, _f;
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
        const meta = (_f = candidates.find((c) => c && typeof c === 'object' && (c.totalCount !== undefined || c.TotalCount !== undefined ||
            c.totalPages !== undefined || c.TotalPages !== undefined ||
            c.currentPage !== undefined || c.CurrentPage !== undefined ||
            c.pageSize !== undefined || c.PageSize !== undefined))) !== null && _f !== void 0 ? _f : null;
        return { items, meta };
    }
    function normalizeItem(item) {
        var _a, _b, _c, _d, _e, _f, _g, _h, _j, _k, _l, _m, _o, _p, _q, _r;
        return {
            id: (_b = (_a = item.id) !== null && _a !== void 0 ? _a : item.Id) !== null && _b !== void 0 ? _b : 0,
            customerId: (_d = (_c = item.customerId) !== null && _c !== void 0 ? _c : item.CustomerId) !== null && _d !== void 0 ? _d : 0,
            serviceId: (_f = (_e = item.serviceId) !== null && _e !== void 0 ? _e : item.ServiceId) !== null && _f !== void 0 ? _f : 0,
            name: (_h = (_g = item.name) !== null && _g !== void 0 ? _g : item.Name) !== null && _h !== void 0 ? _h : '',
            providerId: (_k = (_j = item.providerId) !== null && _j !== void 0 ? _j : item.ProviderId) !== null && _k !== void 0 ? _k : 0,
            status: (_m = (_l = item.status) !== null && _l !== void 0 ? _l : item.Status) !== null && _m !== void 0 ? _m : '',
            registrationDate: item.registrationDate ?? item.RegistrationDate ?? '',
            expirationDate: item.expirationDate ?? item.ExpirationDate ?? '',
            customer: (_p = (_o = item.customer) !== null && _o !== void 0 ? _o : item.Customer) !== null && _p !== void 0 ? _p : null,
            registrar: (_r = (_q = item.registrar) !== null && _q !== void 0 ? _q : item.Registrar) !== null && _r !== void 0 ? _r : null,
        };
    }
    async function loadAllDomains() {
        let allItems = [];
        let pageNumber = 1;
        const pageSize = 200;
        let totalPages = 1;
        while (pageNumber <= totalPages) {
            const params = new URLSearchParams();
            params.set('pageNumber', String(pageNumber));
            params.set('pageSize', String(pageSize));
            const response = await apiRequest(`${getApiBaseUrl()}/RegisteredDomains?${params.toString()}`, { method: 'GET' });
            if (!response.success) {
                throw new Error(response.message || 'Failed to load domains');
            }
            const raw = response.data;
            const extracted = extractItems(raw);
            const meta = extracted.meta ?? raw;
            const items = extracted.items.map(normalizeItem);
            allItems = allItems.concat(items);
            totalPages = (meta === null || meta === void 0 ? void 0 : meta.totalPages) ?? (meta === null || meta === void 0 ? void 0 : meta.TotalPages) ?? (raw === null || raw === void 0 ? void 0 : raw.totalPages) ?? (raw === null || raw === void 0 ? void 0 : raw.TotalPages) ?? totalPages;
            pageNumber += 1;
            if (!extracted.items.length) {
                break;
            }
        }
        return allItems;
    }
    function getCustomerName(domain) {
        var _a, _b;
        const cust = domain.customer;
        return (_b = (_a = cust === null || cust === void 0 ? void 0 : cust.name) !== null && _a !== void 0 ? _a : cust === null || cust === void 0 ? void 0 : cust.Name) !== null && _b !== void 0 ? _b : '';
    }
    function getRegistrarName(domain) {
        var _a, _b;
        const reg = domain.registrar;
        return (_b = (_a = reg === null || reg === void 0 ? void 0 : reg.name) !== null && _a !== void 0 ? _a : reg === null || reg === void 0 ? void 0 : reg.Name) !== null && _b !== void 0 ? _b : '';
    }
    function renderSection(bodyId, domains, emptyMessage) {
        const tbody = document.getElementById(bodyId);
        if (!tbody) {
            return;
        }
        if (!domains.length) {
            tbody.innerHTML = `<tr><td colspan="4" class="text-center text-muted">${emptyMessage}</td></tr>`;
            return;
        }
        tbody.innerHTML = domains.map((domain) => {
            const customerName = getCustomerName(domain);
            const registrarName = getRegistrarName(domain);
            const customerDisplay = customerName
                ? `${esc(customerName)} <span class="text-muted">(#${domain.customerId})</span>`
                : `<span class="text-muted">#${domain.customerId}</span>`;
            const registrarDisplay = registrarName
                ? `${esc(registrarName)} <span class="text-muted">(#${domain.providerId})</span>`
                : `<span class="text-muted">#${domain.providerId}</span>`;
            const expires = domain.expirationDate ? formatDate(domain.expirationDate) : '-';
            return `
        <tr>
            <td><code>${esc(domain.name)}</code></td>
            <td>${customerDisplay}</td>
            <td>${registrarDisplay}</td>
            <td>${esc(expires)}</td>
        </tr>`;
        }).join('');
    }
    function showError(message) {
        const alert = document.getElementById('expiring-domains-alert-error');
        if (!alert) {
            return;
        }
        alert.textContent = message;
        alert.classList.remove('d-none');
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
    async function loadExpiringDomains() {
        try {
            const allDomains = await loadAllDomains();
            const now = new Date();
            const sevenDays = new Date(now.getTime());
            sevenDays.setDate(sevenDays.getDate() + 7);
            const thirtyDays = new Date(now.getTime());
            thirtyDays.setDate(thirtyDays.getDate() + 30);
            const expired = allDomains.filter((domain) => {
                const exp = new Date(domain.expirationDate);
                return domain.expirationDate && !Number.isNaN(exp.getTime()) && exp < now;
            });
            const next7 = allDomains.filter((domain) => {
                const exp = new Date(domain.expirationDate);
                return domain.expirationDate && !Number.isNaN(exp.getTime()) && exp >= now && exp <= sevenDays;
            });
            const next30 = allDomains.filter((domain) => {
                const exp = new Date(domain.expirationDate);
                return domain.expirationDate && !Number.isNaN(exp.getTime()) && exp > sevenDays && exp <= thirtyDays;
            });
            renderSection('expiring-domains-7-body', next7, 'No domains expiring in the next 7 days.');
            renderSection('expiring-domains-30-body', next30, 'No domains expiring in 7-30 days.');
            renderSection('expiring-domains-expired-body', expired, 'No expired domains.');
        }
        catch (error) {
            console.error('Failed to load expiring domains', error);
            showError((error === null || error === void 0 ? void 0 : error.message) || 'Failed to load expiring domains');
        }
    }
    function initializeExpiringDomainsPage() {
        const page = document.getElementById('expiring-domains-page');
        if (!page || page.dataset.initialized === 'true') {
            return;
        }
        page.dataset.initialized = 'true';
        loadExpiringDomains();
    }
    function setupPageObserver() {
        initializeExpiringDomainsPage();
        if (document.body) {
            const observer = new MutationObserver(() => {
                const page = document.getElementById('expiring-domains-page');
                if (page && page.dataset.initialized !== 'true') {
                    initializeExpiringDomainsPage();
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

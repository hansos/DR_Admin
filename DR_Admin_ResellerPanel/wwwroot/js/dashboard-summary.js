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
            console.error('Dashboard summary request failed', error);
            return {
                success: false,
                message: 'Network error. Please try again.',
            };
        }
    }
    function initializePage() {
        const page = document.getElementById('dashboard-summary-page');
        if (!page || page.dataset.initialized === 'true') {
            return;
        }
        page.dataset.initialized = 'true';
        loadPendingSummary();
    }
    async function loadPendingSummary() {
        clearError();
        setPendingLoading(true);
        try {
            const domains = await loadAllDomains();
            const pendingResults = await Promise.all(domains.map(async (domain) => {
                const response = await apiRequest(`${getApiBaseUrl()}/DnsRecords/domain/${domain.id}/pending-sync`, { method: 'GET' });
                if (!response.success) {
                    return { domain, count: null, error: response.message };
                }
                const records = Array.isArray(response.data) ? response.data : [];
                return { domain, count: records.length, error: null };
            }));
            const pending = pendingResults
                .filter((item) => item.count !== null && item.count > 0)
                .sort((a, b) => { var _a, _b; return ((_a = b.count) !== null && _a !== void 0 ? _a : 0) - ((_b = a.count) !== null && _b !== void 0 ? _b : 0); });
            renderPendingTable(pending);
            if (!pending.length) {
                setText('dashboard-summary-pending-note', 'No domains have pending DNS records.');
            }
            else {
                setText('dashboard-summary-pending-note', `${pending.length} domain(s) require registrar sync.`);
            }
        }
        catch (error) {
            showError((error === null || error === void 0 ? void 0 : error.message) || 'Failed to load pending DNS records.');
            renderPendingTable([]);
            setText('dashboard-summary-pending-note', 'Unable to load pending DNS records.');
        }
        finally {
            setPendingLoading(false);
        }
    }
    async function loadAllDomains() {
        var _a, _b, _c, _d;
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
            const meta = (_a = extracted.meta) !== null && _a !== void 0 ? _a : raw;
            const items = extracted.items.map(normalizeDomain);
            allItems = allItems.concat(items);
            totalPages = (_d = (_c = (_b = meta === null || meta === void 0 ? void 0 : meta.totalPages) !== null && _b !== void 0 ? _b : meta === null || meta === void 0 ? void 0 : meta.TotalPages) !== null && _c !== void 0 ? _c : raw === null || raw === void 0 ? void 0 : raw.totalPages) !== null && _d !== void 0 ? _d : raw === null || raw === void 0 ? void 0 : raw.TotalPages;
            pageNumber += 1;
            if (!extracted.items.length) {
                break;
            }
        }
        return allItems;
    }
    function extractItems(raw) {
        if (Array.isArray(raw)) {
            return { items: raw, meta: null };
        }
        const candidates = [raw, raw === null || raw === void 0 ? void 0 : raw.data, raw === null || raw === void 0 ? void 0 : raw.Data, raw === null || raw === void 0 ? void 0 : raw.data?.data, raw === null || raw === void 0 ? void 0 : raw.data?.Data];
        const items =
            (Array.isArray(raw === null || raw === void 0 ? void 0 : raw.Data) && raw.Data) ||
                (Array.isArray(raw === null || raw === void 0 ? void 0 : raw.data) && raw.data) ||
                (Array.isArray(raw === null || raw === void 0 ? void 0 : raw.data?.Data) && raw.data.Data) ||
                (Array.isArray(raw === null || raw === void 0 ? void 0 : raw.data?.data) && raw.data.data) ||
                (Array.isArray(raw === null || raw === void 0 ? void 0 : raw.Data?.Data) && raw.Data.Data) ||
                [];
        const meta = candidates.find((c) => c && typeof c === 'object' && (
            c.totalCount !== undefined || c.TotalCount !== undefined ||
                c.totalPages !== undefined || c.TotalPages !== undefined ||
                c.currentPage !== undefined || c.CurrentPage !== undefined ||
                c.pageSize !== undefined || c.PageSize !== undefined));
        return { items, meta };
    }
    function normalizeDomain(raw) {
        var _a, _b, _c;
        return {
            id: (_b = (_a = raw.id) !== null && _a !== void 0 ? _a : raw.Id) !== null && _b !== void 0 ? _b : 0,
            name: (_c = raw.name) !== null && _c !== void 0 ? _c : raw.Name ?? raw.domainName ?? '',
        };
    }
    function setPendingLoading(isLoading) {
        const loading = document.getElementById('dashboard-summary-pending-loading');
        if (loading) {
            loading.classList.toggle('d-none', !isLoading);
        }
    }
    function renderPendingTable(rows) {
        const tbody = document.getElementById('dashboard-summary-pending-table');
        if (!tbody) {
            return;
        }
        if (!rows.length) {
            tbody.innerHTML = '<tr><td colspan="2" class="text-center text-muted">No pending DNS records.</td></tr>';
            return;
        }
        tbody.innerHTML = rows.map((row) => `
        <tr>
            <td><code>${esc(row.domain.name || `Domain #${row.domain.id}`)}</code></td>
            <td class="text-end"><span class="fw-semibold">${row.count ?? '-'}</span></td>
        </tr>
    `).join('');
    }
    function setText(id, value) {
        const element = document.getElementById(id);
        if (element) {
            element.textContent = value;
        }
    }
    function showError(message) {
        const alert = document.getElementById('dashboard-summary-alert-error');
        if (!alert) {
            return;
        }
        alert.textContent = message;
        alert.classList.remove('d-none');
    }
    function clearError() {
        const alert = document.getElementById('dashboard-summary-alert-error');
        if (alert) {
            alert.textContent = '';
            alert.classList.add('d-none');
        }
    }
    function esc(value) {
        return value.replace(/[&<>"]/g, (match) => {
            switch (match) {
                case '&':
                    return '&amp;';
                case '<':
                    return '&lt;';
                case '>':
                    return '&gt;';
                case '"':
                    return '&quot;';
                default:
                    return match;
            }
        });
    }
    function setupPageObserver() {
        initializePage();
        if (document.body) {
            const observer = new MutationObserver(() => {
                const page = document.getElementById('dashboard-summary-page');
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

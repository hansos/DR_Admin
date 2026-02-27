"use strict";
// @ts-nocheck
(function () {
    let hasOngoingWorkflowWarning = false;
    function getApiBaseUrl() {
        return window.AppSettings?.apiBaseUrl ?? '';
    }
    function getAuthToken() {
        const auth = window.Auth;
        if (auth?.getToken) {
            return auth.getToken();
        }
        return sessionStorage.getItem('rp_authToken');
    }
    async function apiRequest(endpoint, options = {}) {
        try {
            const headers = {
                'Content-Type': 'application/json',
                ...options.headers,
            };
            const authToken = getAuthToken();
            if (authToken) {
                headers['Authorization'] = `Bearer ${authToken}`;
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
                    message: (data && (data.message ?? data.title)) || `Request failed with status ${response.status}`,
                };
            }
            return {
                success: data?.success !== false,
                data: data?.data ?? data,
                message: data?.message,
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
        renderOngoingWorkflowPanel();
        loadPendingSummary();
    }
    function renderOngoingWorkflowPanel() {
        const card = document.getElementById('dashboard-summary-workflow-card');
        if (!card) {
            return;
        }
        const state = loadNewSaleState();
        const domainName = state?.domainName?.trim() ?? '';
        const customer = state?.selectedCustomer;
        const hasCustomer = !!customer && Number(customer.id ?? 0) > 0;
        if (!domainName || !hasCustomer) {
            hasOngoingWorkflowWarning = false;
            card.classList.add('d-none');
            return;
        }
        const customerName = customer?.name?.trim() || customer?.customerName?.trim() || `#${customer?.id}`;
        setText('dashboard-summary-workflow-domain', domainName);
        setText('dashboard-summary-workflow-customer', customerName);
        hasOngoingWorkflowWarning = true;
        card.classList.remove('d-none');
    }
    function loadNewSaleState() {
        const raw = sessionStorage.getItem('new-sale-state');
        if (!raw) {
            return null;
        }
        try {
            return JSON.parse(raw);
        }
        catch {
            return null;
        }
    }
    async function loadPendingSummary() {
        clearError();
        setAllClearVisible(false);
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
                .sort((a, b) => (b.count ?? 0) - (a.count ?? 0));
            renderPendingTable(pending);
            setPendingCardVisible(pending.length > 0);
            setAllClearVisible(pending.length === 0 && !hasOngoingWorkflowWarning);
            if (!pending.length) {
                setText('dashboard-summary-pending-note', 'No domains have pending DNS records.');
            }
            else {
                setText('dashboard-summary-pending-note', `${pending.length} domain(s) require registrar sync.`);
            }
        }
        catch (error) {
            setPendingCardVisible(true);
            setAllClearVisible(false);
            showError(error?.message || 'Failed to load pending DNS records.');
            renderPendingTable([]);
            setText('dashboard-summary-pending-note', 'Unable to load pending DNS records.');
        }
        finally {
            setPendingLoading(false);
        }
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
            const items = extracted.items.map(normalizeDomain);
            allItems = allItems.concat(items);
            totalPages = meta?.totalPages ?? meta?.TotalPages ?? raw?.totalPages ?? raw?.TotalPages ?? totalPages;
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
        const candidates = [raw, raw?.data, raw?.Data, raw?.data?.data, raw?.data?.Data];
        const items = (Array.isArray(raw?.Data) && raw.Data) ||
            (Array.isArray(raw?.data) && raw.data) ||
            (Array.isArray(raw?.data?.Data) && raw.data.Data) ||
            (Array.isArray(raw?.data?.data) && raw.data.data) ||
            (Array.isArray(raw?.Data?.Data) && raw.Data.Data) ||
            [];
        const meta = candidates.find((c) => c && typeof c === 'object' && (c.totalCount !== undefined || c.TotalCount !== undefined ||
            c.totalPages !== undefined || c.TotalPages !== undefined ||
            c.currentPage !== undefined || c.CurrentPage !== undefined ||
            c.pageSize !== undefined || c.PageSize !== undefined));
        return { items, meta };
    }
    function normalizeDomain(raw) {
        return {
            id: raw.id ?? raw.Id ?? 0,
            name: raw.name ?? raw.Name ?? raw.domainName ?? '',
        };
    }
    function setPendingLoading(isLoading) {
        const loading = document.getElementById('dashboard-summary-pending-loading');
        if (loading) {
            loading.classList.toggle('d-none', !isLoading);
        }
    }
    function setPendingCardVisible(isVisible) {
        const card = document.getElementById('dashboard-summary-pending-card');
        if (card) {
            card.classList.toggle('d-none', !isVisible);
        }
    }
    function setAllClearVisible(isVisible) {
        const card = document.getElementById('dashboard-summary-all-clear-card');
        if (card) {
            card.classList.toggle('d-none', !isVisible);
        }
    }
    function renderPendingTable(rows) {
        const tbody = document.getElementById('dashboard-summary-pending-table');
        if (!tbody) {
            return;
        }
        if (!rows.length) {
            tbody.innerHTML = '';
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
//# sourceMappingURL=dashboard-summary.js.map
"use strict";
(() => {
    const pageId = 'order-tax-snapshots-page';
    function getApiBaseUrl() {
        const settings = window.AppSettings;
        return settings?.apiBaseUrl ?? '';
    }
    function getAuthToken() {
        const auth = window.Auth;
        if (auth?.getToken) {
            return auth.getToken();
        }
        return sessionStorage.getItem('rp_authToken');
    }
    async function apiRequest(endpoint) {
        try {
            const headers = { 'Content-Type': 'application/json' };
            const token = getAuthToken();
            if (token) {
                headers.Authorization = `Bearer ${token}`;
            }
            const response = await fetch(endpoint, { method: 'GET', headers, credentials: 'include' });
            const contentType = response.headers.get('content-type') ?? '';
            const payload = contentType.includes('application/json') ? await response.json() : null;
            if (!response.ok) {
                const body = (payload ?? {});
                return { success: false, message: body.message ?? body.title ?? `Request failed with status ${response.status}` };
            }
            const wrapped = (payload ?? {});
            return { success: wrapped.success !== false, data: wrapped.data ?? payload, message: wrapped.message };
        }
        catch {
            return { success: false, message: 'Network error. Please try again.' };
        }
    }
    function extractItems(payload) {
        if (Array.isArray(payload)) {
            return payload;
        }
        const obj = (payload ?? {});
        if (Array.isArray(obj.data)) {
            return obj.data;
        }
        if (Array.isArray(obj.Data)) {
            return obj.Data;
        }
        return [];
    }
    function showSuccess(message) {
        const success = document.getElementById('order-tax-snapshots-alert-success');
        const error = document.getElementById('order-tax-snapshots-alert-error');
        if (success) {
            success.textContent = message;
            success.classList.remove('d-none');
        }
        error?.classList.add('d-none');
    }
    function showError(message) {
        const success = document.getElementById('order-tax-snapshots-alert-success');
        const error = document.getElementById('order-tax-snapshots-alert-error');
        if (error) {
            error.textContent = message;
            error.classList.remove('d-none');
        }
        success?.classList.add('d-none');
    }
    function formatMoney(value) {
        const amount = Number(value ?? 0);
        if (!Number.isFinite(amount)) {
            return '-';
        }
        return amount.toFixed(2);
    }
    function formatDate(value) {
        const text = String(value ?? '');
        if (!text) {
            return '-';
        }
        const date = new Date(text);
        return Number.isNaN(date.getTime()) ? text : date.toLocaleString();
    }
    function renderRows(items) {
        const tbody = document.getElementById('order-tax-snapshots-table-body');
        if (!tbody) {
            return;
        }
        if (items.length === 0) {
            tbody.innerHTML = '<tr><td colspan="7" class="text-center text-muted">No records found.</td></tr>';
            return;
        }
        tbody.innerHTML = items.map((item) => {
            const id = Number(item.id ?? item.Id ?? 0);
            const orderId = Number(item.orderId ?? item.OrderId ?? 0);
            const idempotencyKey = String(item.idempotencyKey ?? item.IdempotencyKey ?? '-');
            const netAmount = formatMoney(item.netAmount ?? item.NetAmount);
            const taxAmount = formatMoney(item.taxAmount ?? item.TaxAmount);
            const grossAmount = formatMoney(item.grossAmount ?? item.GrossAmount);
            const createdAt = formatDate(item.createdAt ?? item.CreatedAt);
            return `<tr>
                <td>${id}</td>
                <td>${orderId}</td>
                <td>${idempotencyKey || '-'}</td>
                <td>${netAmount}</td>
                <td>${taxAmount}</td>
                <td>${grossAmount}</td>
                <td>${createdAt}</td>
            </tr>`;
        }).join('');
    }
    async function loadData(showFeedback) {
        const response = await apiRequest(`${getApiBaseUrl()}/OrderTaxSnapshots`);
        if (!response.success) {
            showError(response.message ?? 'Failed to load order tax snapshots.');
            return;
        }
        renderRows(extractItems(response.data));
        if (showFeedback) {
            showSuccess(`Loaded at ${new Date().toLocaleString()}`);
        }
    }
    function initializePage() {
        const page = document.getElementById(pageId);
        if (!page || page.dataset.initialized === 'true') {
            return;
        }
        page.dataset.initialized = 'true';
        document.getElementById('order-tax-snapshots-refresh')?.addEventListener('click', async () => {
            await loadData(true);
        });
        void loadData(false);
    }
    function setup() {
        initializePage();
        if (!document.body) {
            return;
        }
        const observer = new MutationObserver(() => initializePage());
        observer.observe(document.body, { childList: true, subtree: true });
    }
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', setup);
    }
    else {
        setup();
    }
})();
//# sourceMappingURL=order-tax-snapshots.js.map
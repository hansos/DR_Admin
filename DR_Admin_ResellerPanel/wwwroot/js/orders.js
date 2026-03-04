"use strict";
(() => {
    const orderStatusLabels = {
        0: 'Pending',
        1: 'Active',
        2: 'Suspended',
        3: 'Cancelled',
        4: 'Expired',
        5: 'Trial',
    };
    const orderTypeLabels = {
        0: 'New',
        1: 'Renewal',
        2: 'Upgrade',
        3: 'Downgrade',
        4: 'Addon',
    };
    let allOrders = [];
    let filteredOrders = [];
    let currentPage = 1;
    let pageSize = 25;
    let totalCount = 0;
    let totalPages = 1;
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
    const parseString = (value) => typeof value === 'string' ? value : '';
    const parseNumber = (value) => {
        if (typeof value === 'number' && Number.isFinite(value)) {
            return value;
        }
        if (typeof value === 'string' && value.trim() !== '') {
            const parsed = Number(value);
            if (Number.isFinite(parsed)) {
                return parsed;
            }
        }
        return 0;
    };
    const normalizeEnum = (value, labels) => {
        if (typeof value === 'string' && value.trim() !== '') {
            return value;
        }
        const numeric = parseNumber(value);
        return labels[numeric] ?? String(numeric);
    };
    const formatDate = (value) => {
        if (!value) {
            return '-';
        }
        const date = new Date(value);
        if (Number.isNaN(date.getTime())) {
            return value;
        }
        return date.toLocaleString();
    };
    const formatMoney = (amount, currencyCode) => {
        if (!Number.isFinite(amount)) {
            return '-';
        }
        try {
            return new Intl.NumberFormat(undefined, {
                style: 'currency',
                currency: currencyCode || 'EUR',
                minimumFractionDigits: 2,
                maximumFractionDigits: 2,
            }).format(amount);
        }
        catch {
            return `${amount.toFixed(2)} ${currencyCode || 'EUR'}`;
        }
    };
    const apiRequest = async (endpoint, options = {}) => {
        try {
            const headers = {
                'Content-Type': 'application/json',
                ...options.headers,
            };
            const token = getAuthToken();
            if (token) {
                headers.Authorization = `Bearer ${token}`;
            }
            const response = await fetch(endpoint, {
                ...options,
                headers,
                credentials: 'include',
            });
            const contentType = response.headers.get('content-type') ?? '';
            const hasJson = contentType.includes('application/json');
            const body = hasJson ? await response.json() : null;
            if (!response.ok) {
                const errorBody = (body ?? {});
                return {
                    success: false,
                    message: errorBody.message ?? errorBody.title ?? `Request failed with status ${response.status}`,
                };
            }
            const envelope = (body ?? {});
            return {
                success: envelope.success !== false,
                data: envelope.data ?? body,
                message: envelope.message,
            };
        }
        catch (error) {
            console.error('Orders request failed', error);
            return {
                success: false,
                message: 'Network error. Please try again.',
            };
        }
    };
    const extractOrders = (payload) => {
        if (Array.isArray(payload)) {
            return payload;
        }
        const objectPayload = payload;
        if (Array.isArray(objectPayload?.data)) {
            return objectPayload.data;
        }
        if (Array.isArray(objectPayload?.Data)) {
            return objectPayload.Data;
        }
        const nestedData = objectPayload?.data;
        if (Array.isArray(nestedData?.data)) {
            return nestedData.data;
        }
        if (Array.isArray(nestedData?.Data)) {
            return nestedData.Data;
        }
        return [];
    };
    const normalizeOrder = (item) => {
        const row = (item ?? {});
        return {
            id: parseNumber(row.id ?? row.Id),
            orderNumber: parseString(row.orderNumber ?? row.OrderNumber),
            customerId: parseNumber(row.customerId ?? row.CustomerId),
            orderType: normalizeEnum(row.orderType ?? row.OrderType, orderTypeLabels),
            status: normalizeEnum(row.status ?? row.Status, orderStatusLabels),
            recurringAmount: parseNumber(row.recurringAmount ?? row.RecurringAmount),
            currencyCode: parseString(row.currencyCode ?? row.CurrencyCode) || 'EUR',
            createdAt: parseString(row.createdAt ?? row.CreatedAt),
            nextBillingDate: parseString(row.nextBillingDate ?? row.NextBillingDate),
        };
    };
    const showError = (message) => {
        const alert = document.getElementById('orders-alert-error');
        if (!alert) {
            return;
        }
        alert.textContent = message;
        alert.classList.remove('d-none');
        document.getElementById('orders-alert-success')?.classList.add('d-none');
    };
    const hideError = () => {
        document.getElementById('orders-alert-error')?.classList.add('d-none');
    };
    const loadPageSizeFromUi = () => {
        const select = document.getElementById('orders-page-size');
        const parsed = Number(select?.value ?? '25');
        if (Number.isFinite(parsed) && parsed > 0) {
            pageSize = parsed;
        }
    };
    const getFilterValue = (id) => {
        const element = document.getElementById(id);
        return (element?.value ?? '').trim();
    };
    const applyFilters = () => {
        const status = getFilterValue('orders-filter-status');
        const type = getFilterValue('orders-filter-type');
        const customerIdRaw = getFilterValue('orders-filter-customerid');
        const customerId = customerIdRaw ? Number(customerIdRaw) : null;
        filteredOrders = allOrders.filter((order) => {
            if (status && order.status !== status) {
                return false;
            }
            if (type && order.orderType !== type) {
                return false;
            }
            if (customerId !== null && Number.isFinite(customerId) && order.customerId !== customerId) {
                return false;
            }
            return true;
        });
        currentPage = 1;
        updateView();
    };
    const resetFilters = () => {
        const status = document.getElementById('orders-filter-status');
        const type = document.getElementById('orders-filter-type');
        const customerId = document.getElementById('orders-filter-customerid');
        if (status) {
            status.value = '';
        }
        if (type) {
            type.value = '';
        }
        if (customerId) {
            customerId.value = '';
        }
        filteredOrders = [...allOrders];
        currentPage = 1;
        updateView();
    };
    const getPagedOrders = () => {
        totalCount = filteredOrders.length;
        totalPages = Math.max(1, Math.ceil(totalCount / pageSize));
        if (currentPage > totalPages) {
            currentPage = totalPages;
        }
        const start = (currentPage - 1) * pageSize;
        return filteredOrders.slice(start, start + pageSize);
    };
    const renderTable = () => {
        const tableBody = document.getElementById('orders-table-body');
        if (!tableBody) {
            return;
        }
        const paged = getPagedOrders();
        if (!paged.length) {
            tableBody.innerHTML = '<tr><td colspan="8" class="text-center text-muted">No orders found.</td></tr>';
            return;
        }
        tableBody.innerHTML = paged.map((order) => `
            <tr>
                <td>${order.id}</td>
                <td><code>${esc(order.orderNumber || '-')}</code></td>
                <td>${order.customerId}</td>
                <td>${esc(order.orderType)}</td>
                <td><span class="badge bg-${order.status === 'Active' ? 'success' : order.status === 'Pending' || order.status === 'Trial' ? 'warning text-dark' : order.status === 'Cancelled' ? 'danger' : 'secondary'}">${esc(order.status)}</span></td>
                <td>${esc(formatMoney(order.recurringAmount, order.currencyCode))}</td>
                <td>${esc(formatDate(order.createdAt))}</td>
                <td>${esc(formatDate(order.nextBillingDate))}</td>
            </tr>
        `).join('');
    };
    const renderPagination = () => {
        const info = document.getElementById('orders-pagination-info');
        const list = document.getElementById('orders-paging-controls-list');
        if (!info || !list) {
            return;
        }
        if (!totalCount) {
            info.textContent = 'Showing 0 of 0';
            list.innerHTML = '';
            return;
        }
        const start = (currentPage - 1) * pageSize + 1;
        const end = Math.min(currentPage * pageSize, totalCount);
        info.textContent = `Showing ${start}-${end} of ${totalCount}`;
        if (totalPages <= 1) {
            list.innerHTML = '';
            return;
        }
        const makeItem = (label, page, disabled, active = false) => {
            const cls = `page-item${disabled ? ' disabled' : ''}${active ? ' active' : ''}`;
            const ariaCurrent = active ? ' aria-current="page"' : '';
            const ariaDisabled = disabled ? ' aria-disabled="true" tabindex="-1"' : '';
            const dataPage = disabled ? '' : ` data-page="${page}"`;
            return `<li class="${cls}"><a class="page-link" href="#"${dataPage}${ariaCurrent}${ariaDisabled}>${label}</a></li>`;
        };
        const pages = new Set();
        pages.add(1);
        if (totalPages >= 2) {
            pages.add(2);
            pages.add(totalPages - 1);
        }
        pages.add(totalPages);
        for (let page = currentPage - 1; page <= currentPage + 1; page += 1) {
            if (page >= 1 && page <= totalPages) {
                pages.add(page);
            }
        }
        const sortedPages = Array.from(pages)
            .filter((page) => page >= 1 && page <= totalPages)
            .sort((a, b) => a - b);
        let html = '';
        html += makeItem('Previous', currentPage - 1, currentPage <= 1);
        let lastPage = 0;
        for (const page of sortedPages) {
            if (lastPage > 0 && page - lastPage > 1) {
                html += '<li class="page-item disabled"><span class="page-link">…</span></li>';
            }
            html += makeItem(String(page), page, false, page === currentPage);
            lastPage = page;
        }
        html += makeItem('Next', currentPage + 1, currentPage >= totalPages);
        list.innerHTML = html;
    };
    const updateView = () => {
        loadPageSizeFromUi();
        renderTable();
        renderPagination();
    };
    const loadOrders = async () => {
        const tableBody = document.getElementById('orders-table-body');
        if (tableBody) {
            tableBody.innerHTML = '<tr><td colspan="8" class="text-center"><div class="spinner-border text-primary"></div></td></tr>';
        }
        hideError();
        const response = await apiRequest(`${getApiBaseUrl()}/Orders`, { method: 'GET' });
        if (!response.success) {
            showError(response.message || 'Failed to load orders.');
            if (tableBody) {
                tableBody.innerHTML = '<tr><td colspan="8" class="text-center text-danger">Failed to load data</td></tr>';
            }
            return;
        }
        const list = extractOrders(response.data);
        allOrders = list.map((item) => normalizeOrder(item));
        filteredOrders = [...allOrders];
        updateView();
    };
    const changePage = (page) => {
        if (page < 1 || page > totalPages) {
            return;
        }
        currentPage = page;
        updateView();
    };
    const bindEvents = () => {
        document.getElementById('orders-apply')?.addEventListener('click', applyFilters);
        document.getElementById('orders-reset')?.addEventListener('click', resetFilters);
        document.getElementById('orders-page-size')?.addEventListener('change', () => {
            currentPage = 1;
            updateView();
        });
        document.getElementById('orders-paging-controls')?.addEventListener('click', (event) => {
            const target = event.target;
            const link = target.closest('a[data-page]');
            if (!link) {
                return;
            }
            event.preventDefault();
            const page = Number(link.dataset.page ?? '0');
            if (!Number.isFinite(page)) {
                return;
            }
            changePage(page);
        });
    };
    const initializePage = () => {
        const page = document.getElementById('orders-page');
        if (!page || page.dataset.initialized === 'true') {
            return;
        }
        page.dataset.initialized = 'true';
        bindEvents();
        void loadOrders();
    };
    const setupObserver = () => {
        initializePage();
        if (document.body) {
            const observer = new MutationObserver(() => {
                const page = document.getElementById('orders-page');
                if (page && page.dataset.initialized !== 'true') {
                    initializePage();
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
//# sourceMappingURL=orders.js.map
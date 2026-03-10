"use strict";
(() => {
    let ordersCustomerId = null;
    let ordersPageNumber = 1;
    const ordersPageSize = 10;
    let ordersFiltered = [];
    function initializeOrdersPage() {
        const page = document.getElementById('orders-page');
        if (!page || page.dataset.initialized === 'true') {
            return;
        }
        page.dataset.initialized = 'true';
        document.getElementById('orders-prev')?.addEventListener('click', () => {
            if (ordersPageNumber > 1) {
                ordersPageNumber -= 1;
                renderOrdersPage();
            }
        });
        document.getElementById('orders-next')?.addEventListener('click', () => {
            if (ordersPageNumber * ordersPageSize < ordersFiltered.length) {
                ordersPageNumber += 1;
                renderOrdersPage();
            }
        });
        document.getElementById('orders-table-body')?.addEventListener('click', (event) => {
            const target = event.target;
            const button = target.closest('button[data-id]');
            if (!button) {
                return;
            }
            const id = Number.parseInt(button.dataset.id ?? '', 10);
            if (!Number.isNaN(id) && id > 0) {
                void loadOrderDetails(id);
            }
        });
        void loadOrders();
    }
    async function loadOrders() {
        const typedWindow = window;
        ordersCustomerId = await resolveOrdersCustomerId();
        if (!ordersCustomerId) {
            typedWindow.UserPanelAlerts?.showError('orders-alert-error', 'Could not resolve customer account.');
            ordersFiltered = [];
            renderOrdersPage();
            return;
        }
        const response = await typedWindow.UserPanelApi?.request('/Orders', { method: 'GET' }, true);
        if (!response || !response.success || !response.data) {
            typedWindow.UserPanelAlerts?.showError('orders-alert-error', response?.message ?? 'Could not load orders.');
            ordersFiltered = [];
            renderOrdersPage();
            return;
        }
        const items = normalizeOrders(response.data);
        ordersFiltered = items
            .filter((item) => item.customerId === ordersCustomerId)
            .sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime());
        ordersPageNumber = 1;
        renderOrdersPage();
    }
    function normalizeOrders(payload) {
        if (Array.isArray(payload)) {
            return payload;
        }
        if (Array.isArray(payload.items)) {
            return payload.items;
        }
        if (Array.isArray(payload.data)) {
            return payload.data;
        }
        return [];
    }
    function renderOrdersPage() {
        const start = (ordersPageNumber - 1) * ordersPageSize;
        const pageItems = ordersFiltered.slice(start, start + ordersPageSize);
        renderOrdersRows(pageItems);
        renderOrdersPageInfo(pageItems.length);
    }
    function renderOrdersRows(items) {
        const tableBody = document.getElementById('orders-table-body');
        if (!tableBody) {
            return;
        }
        if (items.length === 0) {
            tableBody.innerHTML = '<tr><td colspan="7" class="text-center text-muted">No orders found.</td></tr>';
            return;
        }
        tableBody.innerHTML = items.map((item) => `
        <tr>
            <td>${escapeOrdersText(item.orderNumber)}</td>
            <td>${escapeOrdersText(String(item.orderType))}</td>
            <td>${escapeOrdersText(String(item.status))}</td>
            <td>${formatOrdersDate(item.createdAt)}</td>
            <td>${formatOrdersDate(item.nextBillingDate)}</td>
            <td>${formatOrdersMoney(item.recurringAmount)} ${escapeOrdersText(item.currencyCode || 'EUR')}</td>
            <td><button class="btn btn-outline-primary btn-sm" type="button" data-id="${item.id}">View lines</button></td>
        </tr>
    `).join('');
    }
    function renderOrdersPageInfo(count) {
        const info = document.getElementById('orders-pagination-info');
        if (info) {
            info.textContent = `Page ${ordersPageNumber} · Showing ${count} item(s) · Total ${ordersFiltered.length}`;
        }
    }
    async function loadOrderDetails(id) {
        const typedWindow = window;
        const response = await typedWindow.UserPanelApi?.request(`/Orders/${id}`, { method: 'GET' }, true);
        if (!response || !response.success || !response.data) {
            typedWindow.UserPanelAlerts?.showError('orders-alert-error', response?.message ?? 'Could not load order details.');
            return;
        }
        if (ordersCustomerId && response.data.customerId !== ordersCustomerId) {
            typedWindow.UserPanelAlerts?.showError('orders-alert-error', 'Order does not belong to current customer.');
            return;
        }
        const card = document.getElementById('orders-details-card');
        const body = document.getElementById('orders-details-body');
        if (!card || !body) {
            return;
        }
        const lines = response.data.orderLines ?? [];
        const rows = lines.length === 0
            ? '<tr><td colspan="6" class="text-center text-muted">No order lines found.</td></tr>'
            : lines.map((line) => `
            <tr>
                <td>${line.lineNumber}</td>
                <td>${escapeOrdersText(line.description)}</td>
                <td>${line.quantity}</td>
                <td>${formatOrdersMoney(line.unitPrice)}</td>
                <td>${formatOrdersMoney(line.totalPrice)}</td>
                <td>${line.isRecurring ? 'Yes' : 'No'}</td>
            </tr>
        `).join('');
        card.classList.remove('d-none');
        body.innerHTML = `
        <div class="mb-3">
            <div><strong>${escapeOrdersText(response.data.orderNumber)}</strong></div>
            <div>Status: ${escapeOrdersText(String(response.data.status))}</div>
            <div>Type: ${escapeOrdersText(String(response.data.orderType))}</div>
            <div>Created: ${formatOrdersDate(response.data.createdAt)}</div>
        </div>
        <div class="table-responsive">
            <table class="table table-sm align-middle mb-0">
                <thead>
                    <tr>
                        <th>Line</th>
                        <th>Description</th>
                        <th>Qty</th>
                        <th>Unit</th>
                        <th>Total</th>
                        <th>Recurring</th>
                    </tr>
                </thead>
                <tbody>
                    ${rows}
                </tbody>
            </table>
        </div>
    `;
    }
    async function resolveOrdersCustomerId() {
        const typedWindow = window;
        const response = await typedWindow.UserPanelApi?.request('/MyAccount/me', { method: 'GET' }, true);
        return response?.success ? (response.data?.customer?.id ?? null) : null;
    }
    function formatOrdersDate(value) {
        const date = new Date(value);
        if (Number.isNaN(date.getTime())) {
            return '-';
        }
        return date.toLocaleDateString();
    }
    function formatOrdersMoney(value) {
        return Number.isFinite(value) ? value.toFixed(2) : '0.00';
    }
    function escapeOrdersText(value) {
        return value
            .replace(/&/g, '&amp;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;')
            .replace(/'/g, '&#039;');
    }
    function setupOrdersObserver() {
        initializeOrdersPage();
        const observer = new MutationObserver(() => {
            initializeOrdersPage();
        });
        observer.observe(document.body, { childList: true, subtree: true });
    }
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', setupOrdersObserver);
    }
    else {
        setupOrdersObserver();
    }
})();
//# sourceMappingURL=orders.js.map
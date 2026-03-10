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
        document.getElementById('orders-list')?.addEventListener('click', (event) => {
            const target = event.target;
            if (target.closest('.orders-lines-panel')) {
                return;
            }
            const listItem = target.closest('[data-order-item-id]');
            if (!listItem) {
                return;
            }
            const id = Number.parseInt(listItem.dataset.orderItemId ?? '', 10);
            if (!Number.isNaN(id) && id > 0) {
                void toggleOrderLines(id, listItem);
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
        renderOrdersItems(pageItems);
        renderOrdersPageInfo(pageItems.length);
    }
    function renderOrdersItems(items) {
        const list = document.getElementById('orders-list');
        if (!list) {
            return;
        }
        if (items.length === 0) {
            list.innerHTML = '<li class="list-group-item text-center text-muted">No orders found.</li>';
            return;
        }
        list.innerHTML = items.map((item) => `
        <li class="list-group-item" data-order-item-id="${item.id}">
            <div class="d-flex flex-column flex-xl-row justify-content-between gap-3">
                <div class="row g-2 flex-grow-1">
                    <div class="col-6 col-md-4 col-xl-2">
                        <div class="small text-muted">Order</div>
                        <div class="fw-semibold">${escapeOrdersText(item.orderNumber)}</div>
                    </div>
                    <div class="col-6 col-md-4 col-xl-2">
                        <div class="small text-muted">Type</div>
                        <div>${escapeOrdersText(String(item.orderType))}</div>
                    </div>
                    <div class="col-6 col-md-4 col-xl-2">
                        <div class="small text-muted">Status</div>
                        <div>${escapeOrdersText(String(item.status))}</div>
                    </div>
                    <div class="col-6 col-md-4 col-xl-2">
                        <div class="small text-muted">Created</div>
                        <div>${formatOrdersDate(item.createdAt)}</div>
                    </div>
                    <div class="col-6 col-md-4 col-xl-2">
                        <div class="small text-muted">Next billing</div>
                        <div>${formatOrdersDate(item.nextBillingDate)}</div>
                    </div>
                    <div class="col-6 col-md-4 col-xl-2">
                        <div class="small text-muted">Recurring</div>
                        <div>${formatOrdersMoney(item.recurringAmount)} ${escapeOrdersText(item.currencyCode || 'EUR')}</div>
                    </div>
                </div>
            </div>
            <div class="orders-lines-panel d-none border-top mt-3 pt-3" data-loaded="false"></div>
        </li>
    `).join('');
    }
    function renderOrdersPageInfo(count) {
        const info = document.getElementById('orders-pagination-info');
        if (info) {
            info.textContent = `Page ${ordersPageNumber} · Showing ${count} item(s) · Total ${ordersFiltered.length}`;
        }
    }
    async function toggleOrderLines(id, listItem) {
        const panel = listItem.querySelector('.orders-lines-panel');
        if (!panel) {
            return;
        }
        const isOpen = !panel.classList.contains('d-none');
        if (isOpen) {
            panel.classList.add('d-none');
            listItem.classList.remove('order-item-open');
            return;
        }
        closeOtherOrderPanels(id);
        if (panel.dataset.loaded !== 'true') {
            panel.innerHTML = '<div class="text-muted">Loading order lines...</div>';
            const loaded = await loadOrderDetailsIntoPanel(id, panel);
            if (!loaded) {
                panel.classList.add('d-none');
                listItem.classList.remove('order-item-open');
                return;
            }
            panel.dataset.loaded = 'true';
        }
        panel.classList.remove('d-none');
        listItem.classList.add('order-item-open');
    }
    function closeOtherOrderPanels(activeOrderId) {
        const list = document.getElementById('orders-list');
        if (!list) {
            return;
        }
        const openPanels = list.querySelectorAll('.orders-lines-panel:not(.d-none)');
        openPanels.forEach((panel) => {
            const listItem = panel.closest('[data-order-item-id]');
            const orderId = Number.parseInt(listItem?.dataset.orderItemId ?? '', 10);
            if (!Number.isNaN(orderId) && orderId !== activeOrderId) {
                panel.classList.add('d-none');
                listItem?.classList.remove('order-item-open');
            }
        });
    }
    async function loadOrderDetailsIntoPanel(id, panel) {
        const typedWindow = window;
        const response = await typedWindow.UserPanelApi?.request(`/Orders/${id}`, { method: 'GET' }, true);
        if (!response || !response.success || !response.data) {
            typedWindow.UserPanelAlerts?.showError('orders-alert-error', response?.message ?? 'Could not load order details.');
            return false;
        }
        if (ordersCustomerId && response.data.customerId !== ordersCustomerId) {
            typedWindow.UserPanelAlerts?.showError('orders-alert-error', 'Order does not belong to current customer.');
            return false;
        }
        const lines = response.data.orderLines ?? [];
        const lineItems = lines.length === 0
            ? '<li class="list-group-item text-center text-muted">No order lines found.</li>'
            : lines.map((line) => `
            <li class="list-group-item">
                <div class="row g-2">
                    <div class="col-6 col-md-2">
                        <div class="small text-muted">Line</div>
                        <div>${line.lineNumber}</div>
                    </div>
                    <div class="col-12 col-md-4">
                        <div class="small text-muted">Description</div>
                        <div>${escapeOrdersText(line.description)}</div>
                    </div>
                    <div class="col-6 col-md-2">
                        <div class="small text-muted">Qty</div>
                        <div>${line.quantity}</div>
                    </div>
                    <div class="col-6 col-md-2">
                        <div class="small text-muted">Unit</div>
                        <div>${formatOrdersMoney(line.unitPrice)}</div>
                    </div>
                    <div class="col-6 col-md-1">
                        <div class="small text-muted">Total</div>
                        <div>${formatOrdersMoney(line.totalPrice)}</div>
                    </div>
                    <div class="col-6 col-md-1">
                        <div class="small text-muted">Recurring</div>
                        <div>${line.isRecurring ? 'Yes' : 'No'}</div>
                    </div>
                </div>
            </li>
        `).join('');
        panel.innerHTML = `
        <ul class="list-group list-group-flush">
            ${lineItems}
        </ul>
    `;
        return true;
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
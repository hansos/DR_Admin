"use strict";
(() => {
    let invoicesCustomerId = null;
    let invoicesPageNumber = 1;
    const invoicesPageSize = 10;
    let invoicesLastCount = 0;
    function initializeInvoicesPage() {
        const page = document.getElementById('invoices-page');
        if (!page || page.dataset.initialized === 'true') {
            return;
        }
        page.dataset.initialized = 'true';
        document.getElementById('invoices-prev')?.addEventListener('click', () => {
            if (invoicesPageNumber > 1) {
                invoicesPageNumber -= 1;
                void loadInvoices();
            }
        });
        document.getElementById('invoices-next')?.addEventListener('click', () => {
            if (invoicesLastCount >= invoicesPageSize) {
                invoicesPageNumber += 1;
                void loadInvoices();
            }
        });
        document.getElementById('invoices-list')?.addEventListener('click', (event) => {
            const target = event.target;
            if (target.closest('.invoices-details-panel')) {
                return;
            }
            const listItem = target.closest('[data-invoice-item-id]');
            if (!listItem) {
                return;
            }
            const id = Number.parseInt(listItem.dataset.invoiceItemId ?? '', 10);
            if (!Number.isNaN(id) && id > 0) {
                void toggleInvoiceDetails(id, listItem);
            }
        });
        void loadInvoices();
    }
    async function loadInvoices() {
        const typedWindow = window;
        invoicesCustomerId = await resolveInvoicesCustomerId();
        if (!invoicesCustomerId) {
            typedWindow.UserPanelAlerts?.showError('invoices-alert-error', 'Could not resolve customer account.');
            invoicesLastCount = 0;
            renderInvoicesItems([]);
            renderInvoicesPageInfo(0);
            return;
        }
        const response = await typedWindow.UserPanelApi?.request(`/Invoices/customer/${invoicesCustomerId}?pageNumber=${invoicesPageNumber}&pageSize=${invoicesPageSize}`, { method: 'GET' }, true);
        if (!response || !response.success || !response.data) {
            typedWindow.UserPanelAlerts?.showError('invoices-alert-error', response?.message ?? 'Could not load invoices.');
            invoicesLastCount = 0;
            renderInvoicesItems([]);
            renderInvoicesPageInfo(0);
            return;
        }
        const items = normalizeInvoices(response.data);
        invoicesLastCount = items.length;
        renderInvoicesItems(items);
        renderInvoicesPageInfo(items.length);
    }
    function normalizeInvoices(payload) {
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
    function renderInvoicesItems(items) {
        const list = document.getElementById('invoices-list');
        if (!list) {
            return;
        }
        if (items.length === 0) {
            list.innerHTML = '<li class="list-group-item text-center text-muted">No invoices found.</li>';
            return;
        }
        list.innerHTML = items.map((item) => `
        <li class="list-group-item" data-invoice-item-id="${item.id}">
            <div class="d-flex flex-column flex-xl-row justify-content-between gap-3">
                <div class="row g-2 flex-grow-1">
                    <div class="col-6 col-md-4 col-xl-2">
                        <div class="small text-muted">Invoice</div>
                        <div class="fw-semibold">${escapeInvoicesText(item.invoiceNumber)}</div>
                    </div>
                    <div class="col-6 col-md-4 col-xl-2">
                        <div class="small text-muted">Status</div>
                        <div>${escapeInvoicesText(formatInvoiceStatus(item.status))}</div>
                    </div>
                    <div class="col-6 col-md-4 col-xl-2">
                        <div class="small text-muted">Issue</div>
                        <div>${formatInvoicesDate(item.issueDate)}</div>
                    </div>
                    <div class="col-6 col-md-4 col-xl-2">
                        <div class="small text-muted">Due</div>
                        <div>${formatInvoicesDate(getEffectiveInvoiceDueDate(item))}</div>
                    </div>
                    <div class="col-6 col-md-4 col-xl-2">
                        <div class="small text-muted">Total</div>
                        <div>${formatInvoicesMoney(item.totalAmount)} ${escapeInvoicesText(item.currencyCode)}</div>
                    </div>
                    <div class="col-6 col-md-4 col-xl-2">
                        <div class="small text-muted">Amount due</div>
                        <div>${formatInvoicesMoney(item.amountDue)} ${escapeInvoicesText(item.currencyCode)}</div>
                    </div>
                </div>
            </div>
            <div class="invoices-details-panel d-none border-top mt-3 pt-3" data-loaded="false"></div>
        </li>
    `).join('');
    }
    function renderInvoicesPageInfo(count) {
        const info = document.getElementById('invoices-pagination-info');
        if (info) {
            info.textContent = `Page ${invoicesPageNumber} · Showing ${count} item(s)`;
        }
    }
    async function toggleInvoiceDetails(id, listItem) {
        const panel = listItem.querySelector('.invoices-details-panel');
        if (!panel) {
            return;
        }
        const isOpen = !panel.classList.contains('d-none');
        if (isOpen) {
            panel.classList.add('d-none');
            listItem.classList.remove('invoice-item-open');
            return;
        }
        closeOtherInvoicePanels(id);
        if (panel.dataset.loaded !== 'true') {
            panel.innerHTML = '<div class="text-muted">Loading invoice details...</div>';
            const loaded = await loadInvoiceDetailsIntoPanel(id, panel);
            if (!loaded) {
                panel.classList.add('d-none');
                listItem.classList.remove('invoice-item-open');
                return;
            }
            panel.dataset.loaded = 'true';
        }
        panel.classList.remove('d-none');
        listItem.classList.add('invoice-item-open');
    }
    function closeOtherInvoicePanels(activeInvoiceId) {
        const list = document.getElementById('invoices-list');
        if (!list) {
            return;
        }
        const openPanels = list.querySelectorAll('.invoices-details-panel:not(.d-none)');
        openPanels.forEach((panel) => {
            const listItem = panel.closest('[data-invoice-item-id]');
            const invoiceId = Number.parseInt(listItem?.dataset.invoiceItemId ?? '', 10);
            if (!Number.isNaN(invoiceId) && invoiceId !== activeInvoiceId) {
                panel.classList.add('d-none');
                listItem?.classList.remove('invoice-item-open');
            }
        });
    }
    async function loadInvoiceDetailsIntoPanel(id, panel) {
        const typedWindow = window;
        const response = await typedWindow.UserPanelApi?.request(`/Invoices/${id}`, { method: 'GET' }, true);
        if (!response || !response.success || !response.data) {
            typedWindow.UserPanelAlerts?.showError('invoices-alert-error', response?.message ?? 'Could not load invoice details.');
            return false;
        }
        const payload = response.data;
        const lines = payload.invoiceLines ?? payload.InvoiceLines ?? [];
        const lineItems = lines.length === 0
            ? '<li class="list-group-item text-center text-muted">No invoice lines found.</li>'
            : lines.map((line) => `
            <li class="list-group-item">
                <div class="row g-2">
                    <div class="col-6 col-md-2">
                        <div class="small text-muted">Line</div>
                        <div>${line.lineNumber ?? line.LineNumber ?? '-'}</div>
                    </div>
                    <div class="col-12 col-md-4">
                        <div class="small text-muted">Description</div>
                        <div>${escapeInvoicesText(line.description ?? line.Description ?? '-')}</div>
                    </div>
                    <div class="col-6 col-md-2">
                        <div class="small text-muted">Qty</div>
                        <div>${line.quantity ?? line.Quantity ?? '-'}</div>
                    </div>
                    <div class="col-6 col-md-2">
                        <div class="small text-muted">Unit</div>
                        <div>${formatInvoicesMoney(line.unitPrice ?? line.UnitPrice ?? 0)}</div>
                    </div>
                    <div class="col-6 col-md-2">
                        <div class="small text-muted">Total</div>
                        <div>${formatInvoicesMoney(line.totalPrice ?? line.TotalPrice ?? 0)}</div>
                    </div>
                </div>
            </li>
        `).join('');
        panel.innerHTML = `
        <ul class="list-group list-group-flush">
            ${lineItems}
            <li class="list-group-item border-top">
                <div class="small text-muted">Payment method</div>
                <div>${escapeInvoicesText(payload.paymentMethod ?? payload.PaymentMethod ?? '-')}</div>
            </li>
            <li class="list-group-item">
                <div class="small text-muted">Notes</div>
                <div>${escapeInvoicesText(payload.notes ?? payload.Notes ?? 'No notes')}</div>
            </li>
        </ul>
    `;
        return true;
    }
    function getEffectiveInvoiceDueDate(invoice) {
        const paymentMethod = String(invoice.paymentMethod ?? '').toLowerCase();
        const isCardPayment = paymentMethod.includes('card');
        const isPrepaid = Number.isFinite(invoice.amountDue) && invoice.amountDue <= 0;
        return isCardPayment && isPrepaid ? invoice.issueDate : invoice.dueDate;
    }
    async function resolveInvoicesCustomerId() {
        const typedWindow = window;
        const response = await typedWindow.UserPanelApi?.request('/MyAccount/me', { method: 'GET' }, true);
        return response?.success ? (response.data?.customer?.id ?? null) : null;
    }
    function formatInvoicesDate(value) {
        const date = new Date(value);
        if (Number.isNaN(date.getTime())) {
            return '-';
        }
        return date.toLocaleDateString();
    }
    function formatInvoicesMoney(value) {
        return Number.isFinite(value) ? value.toFixed(2) : '0.00';
    }
    function formatInvoiceStatus(value) {
        if (typeof value === 'string' && value.trim().length > 0) {
            return value;
        }
        const numeric = Number(value);
        if (!Number.isFinite(numeric)) {
            return '-';
        }
        switch (numeric) {
            case 0:
                return 'Draft';
            case 1:
                return 'Issued';
            case 2:
                return 'Paid';
            case 3:
                return 'Overdue';
            case 4:
                return 'Cancelled';
            case 5:
                return 'Credited';
            default:
                return `Status ${numeric}`;
        }
    }
    function escapeInvoicesText(value) {
        return String(value ?? '')
            .replace(/&/g, '&amp;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;')
            .replace(/'/g, '&#039;');
    }
    function setupInvoicesObserver() {
        initializeInvoicesPage();
        const observer = new MutationObserver(() => {
            initializeInvoicesPage();
        });
        observer.observe(document.body, { childList: true, subtree: true });
    }
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', setupInvoicesObserver);
    }
    else {
        setupInvoicesObserver();
    }
})();
//# sourceMappingURL=invoices.js.map
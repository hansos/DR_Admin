"use strict";
(() => {
    const invoiceStatusLabels = {
        0: 'Draft',
        1: 'Issued',
        2: 'Paid',
        3: 'Overdue',
        4: 'Cancelled',
        5: 'Credited',
    };
    let allInvoices = [];
    let filteredInvoices = [];
    let currentPage = 1;
    let pageSize = 25;
    let totalCount = 0;
    let totalPages = 1;
    let selectedInvoiceId = null;
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
    const parseNullableNumber = (value) => {
        const parsed = parseNumber(value);
        return parsed > 0 ? parsed : null;
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
            console.error('Invoices request failed', error);
            return {
                success: false,
                message: 'Network error. Please try again.',
            };
        }
    };
    const extractInvoices = (payload) => {
        if (Array.isArray(payload)) {
            return payload;
        }
        const objectPayload = payload;
        if (Array.isArray(objectPayload?.items)) {
            return objectPayload.items;
        }
        if (Array.isArray(objectPayload?.Items)) {
            return objectPayload.Items;
        }
        if (Array.isArray(objectPayload?.data)) {
            return objectPayload.data;
        }
        if (Array.isArray(objectPayload?.Data)) {
            return objectPayload.Data;
        }
        const nestedData = objectPayload?.data;
        if (Array.isArray(nestedData?.items)) {
            return nestedData.items;
        }
        if (Array.isArray(nestedData?.Items)) {
            return nestedData.Items;
        }
        if (Array.isArray(nestedData?.data)) {
            return nestedData.data;
        }
        if (Array.isArray(nestedData?.Data)) {
            return nestedData.Data;
        }
        return [];
    };
    const normalizeInvoiceLine = (item) => {
        const row = (item ?? {});
        return {
            id: parseNumber(row.id ?? row.Id),
            lineNumber: parseNumber(row.lineNumber ?? row.LineNumber),
            description: parseString(row.description ?? row.Description),
            quantity: parseNumber(row.quantity ?? row.Quantity),
            unitPrice: parseNumber(row.unitPrice ?? row.UnitPrice),
            totalPrice: parseNumber(row.totalPrice ?? row.TotalPrice),
            taxAmount: parseNumber(row.taxAmount ?? row.TaxAmount),
            totalWithTax: parseNumber(row.totalWithTax ?? row.TotalWithTax),
            notes: parseString(row.notes ?? row.Notes),
        };
    };
    const normalizeInvoice = (item) => {
        const row = (item ?? {});
        const linesRaw = row.invoiceLines ?? row.InvoiceLines;
        const lines = Array.isArray(linesRaw)
            ? linesRaw.map((line) => normalizeInvoiceLine(line))
            : [];
        return {
            id: parseNumber(row.id ?? row.Id),
            invoiceNumber: parseString(row.invoiceNumber ?? row.InvoiceNumber),
            customerId: parseNumber(row.customerId ?? row.CustomerId),
            orderId: parseNullableNumber(row.orderId ?? row.OrderId),
            status: normalizeEnum(row.status ?? row.Status, invoiceStatusLabels),
            issueDate: parseString(row.issueDate ?? row.IssueDate),
            dueDate: parseString(row.dueDate ?? row.DueDate),
            paidAt: parseString(row.paidAt ?? row.PaidAt),
            totalAmount: parseNumber(row.totalAmount ?? row.TotalAmount),
            amountDue: parseNumber(row.amountDue ?? row.AmountDue),
            currencyCode: parseString(row.currencyCode ?? row.CurrencyCode) || 'EUR',
            paymentMethod: parseString(row.paymentMethod ?? row.PaymentMethod),
            notes: parseString(row.notes ?? row.Notes),
            invoiceLines: lines,
        };
    };
    const renderInvoiceLinesPanel = (invoice) => {
        if (!invoice.invoiceLines.length) {
            return '<div class="small text-muted py-2">No invoice lines found for this invoice.</div>';
        }
        const lines = [...invoice.invoiceLines].sort((a, b) => a.lineNumber - b.lineNumber || a.id - b.id);
        return `
            <div class="py-2">
                <div class="fw-semibold mb-2">Invoice lines</div>
                <div class="table-responsive">
                    <table class="table table-sm mb-2">
                        <thead>
                            <tr>
                                <th>Line</th>
                                <th>Description</th>
                                <th>Qty</th>
                                <th class="text-end">Unit</th>
                                <th class="text-end">Tax</th>
                                <th class="text-end">Total</th>
                            </tr>
                        </thead>
                        <tbody>
                            ${lines.map((line) => `
                                <tr>
                                    <td>${line.lineNumber || '-'}</td>
                                    <td>${esc(line.description || '-')}</td>
                                    <td>${line.quantity}</td>
                                    <td class="text-end">${esc(formatMoney(line.unitPrice, invoice.currencyCode))}</td>
                                    <td class="text-end">${esc(formatMoney(line.taxAmount, invoice.currencyCode))}</td>
                                    <td class="text-end">${esc(formatMoney(line.totalWithTax || line.totalPrice, invoice.currencyCode))}</td>
                                </tr>
                            `).join('')}
                        </tbody>
                    </table>
                </div>
                <div class="small text-muted">
                    <div><strong>Order:</strong> ${invoice.orderId ? `#${invoice.orderId}` : '-'}</div>
                    <div><strong>Payment method:</strong> ${esc(invoice.paymentMethod || '-')}</div>
                    <div><strong>Notes:</strong> ${esc(invoice.notes || '-')}</div>
                </div>
            </div>
        `;
    };
    const showError = (message) => {
        const alert = document.getElementById('invoices-alert-error');
        if (!alert) {
            return;
        }
        alert.textContent = message;
        alert.classList.remove('d-none');
        document.getElementById('invoices-alert-success')?.classList.add('d-none');
    };
    const hideError = () => {
        document.getElementById('invoices-alert-error')?.classList.add('d-none');
    };
    const loadPageSizeFromUi = () => {
        const select = document.getElementById('invoices-page-size');
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
        const status = getFilterValue('invoices-filter-status');
        const customerIdRaw = getFilterValue('invoices-filter-customerid');
        const customerId = customerIdRaw ? Number(customerIdRaw) : null;
        filteredInvoices = allInvoices.filter((invoice) => {
            if (status && invoice.status !== status) {
                return false;
            }
            if (customerId !== null && Number.isFinite(customerId) && invoice.customerId !== customerId) {
                return false;
            }
            return true;
        });
        currentPage = 1;
        updateView();
    };
    const resetFilters = () => {
        const status = document.getElementById('invoices-filter-status');
        const customerId = document.getElementById('invoices-filter-customerid');
        if (status) {
            status.value = '';
        }
        if (customerId) {
            customerId.value = '';
        }
        filteredInvoices = [...allInvoices];
        currentPage = 1;
        updateView();
    };
    const getPagedInvoices = () => {
        totalCount = filteredInvoices.length;
        totalPages = Math.max(1, Math.ceil(totalCount / pageSize));
        if (currentPage > totalPages) {
            currentPage = totalPages;
        }
        const start = (currentPage - 1) * pageSize;
        return filteredInvoices.slice(start, start + pageSize);
    };
    const renderTable = () => {
        const tableBody = document.getElementById('invoices-table-body');
        if (!tableBody) {
            return;
        }
        const paged = getPagedInvoices();
        if (!paged.length) {
            tableBody.innerHTML = '<tr><td colspan="8" class="text-center text-muted">No invoices found.</td></tr>';
            return;
        }
        tableBody.innerHTML = paged.map((invoice) => {
            const statusClass = invoice.status === 'Paid'
                ? 'success'
                : invoice.status === 'Issued'
                    ? 'primary'
                    : invoice.status === 'Overdue'
                        ? 'danger'
                        : invoice.status === 'Draft'
                            ? 'warning text-dark'
                            : 'secondary';
            const isSelected = selectedInvoiceId === invoice.id;
            const selectedClass = isSelected ? 'invoices-row-selected' : '';
            return `
                <tr data-invoice-id="${invoice.id}" class="${selectedClass}">
                    <td>${invoice.id}</td>
                    <td><code>${esc(invoice.invoiceNumber || '-')}</code></td>
                    <td>${invoice.customerId}</td>
                    <td><span class="badge bg-${statusClass}">${esc(invoice.status)}</span></td>
                    <td>${esc(formatDate(invoice.issueDate))}</td>
                    <td>${esc(formatDate(invoice.dueDate))}</td>
                    <td>${esc(formatMoney(invoice.totalAmount, invoice.currencyCode))}</td>
                    <td>${esc(formatMoney(invoice.amountDue, invoice.currencyCode))}</td>
                </tr>
                ${isSelected ? `<tr class="invoices-row-drilldown"><td colspan="8">${renderInvoiceLinesPanel(invoice)}</td></tr>` : ''}
            `;
        }).join('');
    };
    const renderPagination = () => {
        const info = document.getElementById('invoices-pagination-info');
        const list = document.getElementById('invoices-paging-controls-list');
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
    const loadInvoices = async () => {
        const tableBody = document.getElementById('invoices-table-body');
        if (tableBody) {
            tableBody.innerHTML = '<tr><td colspan="8" class="text-center"><div class="spinner-border text-primary"></div></td></tr>';
        }
        hideError();
        const response = await apiRequest(`${getApiBaseUrl()}/Invoices`, { method: 'GET' });
        if (!response.success) {
            showError(response.message || 'Failed to load invoices.');
            if (tableBody) {
                tableBody.innerHTML = '<tr><td colspan="8" class="text-center text-danger">Failed to load data</td></tr>';
            }
            return;
        }
        const list = extractInvoices(response.data);
        allInvoices = list.map((item) => normalizeInvoice(item));
        filteredInvoices = [...allInvoices];
        if (selectedInvoiceId !== null && !filteredInvoices.some((invoice) => invoice.id === selectedInvoiceId)) {
            selectedInvoiceId = null;
        }
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
        document.getElementById('invoices-apply')?.addEventListener('click', applyFilters);
        document.getElementById('invoices-reset')?.addEventListener('click', resetFilters);
        document.getElementById('invoices-page-size')?.addEventListener('change', () => {
            currentPage = 1;
            updateView();
        });
        document.getElementById('invoices-paging-controls')?.addEventListener('click', (event) => {
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
        document.getElementById('invoices-table-body')?.addEventListener('click', (event) => {
            const target = event.target;
            const row = target.closest('tr[data-invoice-id]');
            if (!row) {
                return;
            }
            const invoiceId = Number(row.dataset.invoiceId ?? '0');
            if (!Number.isFinite(invoiceId) || invoiceId <= 0) {
                return;
            }
            selectedInvoiceId = selectedInvoiceId === invoiceId ? null : invoiceId;
            updateView();
        });
    };
    const initializePage = () => {
        const page = document.getElementById('invoices-page');
        if (!page || page.dataset.initialized === 'true') {
            return;
        }
        page.dataset.initialized = 'true';
        bindEvents();
        void loadInvoices();
    };
    const setupObserver = () => {
        initializePage();
        if (document.body) {
            const observer = new MutationObserver(() => {
                const page = document.getElementById('invoices-page');
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
//# sourceMappingURL=invoices.js.map
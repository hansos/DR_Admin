"use strict";
(() => {
    let allPayments = [];
    let filteredPayments = [];
    let currentPage = 1;
    let pageSize = 25;
    let totalCount = 0;
    let totalPages = 1;
    let selectedPaymentId = null;
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
            console.error('Payments request failed', error);
            return {
                success: false,
                message: 'Network error. Please try again.',
            };
        }
    };
    const extractPayments = (payload) => {
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
        return [];
    };
    const normalizeAllocation = (item) => {
        const row = (item ?? {});
        return {
            id: parseNumber(row.id ?? row.Id),
            amountApplied: parseNumber(row.amountApplied ?? row.AmountApplied),
            currency: parseString(row.currency ?? row.Currency) || 'EUR',
            invoiceBalance: parseNumber(row.invoiceBalance ?? row.InvoiceBalance),
            invoiceTotalAmount: parseNumber(row.invoiceTotalAmount ?? row.InvoiceTotalAmount),
            isFullPayment: Boolean(row.isFullPayment ?? row.IsFullPayment ?? false),
            createdAt: parseString(row.createdAt ?? row.CreatedAt),
        };
    };
    const normalizePayment = (item) => {
        const row = (item ?? {});
        const allocationsRaw = row.allocations ?? row.Allocations;
        const allocations = Array.isArray(allocationsRaw)
            ? allocationsRaw.map((entry) => normalizeAllocation(entry))
            : [];
        return {
            id: parseNumber(row.id ?? row.Id),
            invoiceId: parseNumber(row.invoiceId ?? row.InvoiceId),
            invoiceNumber: parseString(row.invoiceNumber ?? row.InvoiceNumber),
            customerId: parseNumber(row.customerId ?? row.CustomerId),
            customerName: parseString(row.customerName ?? row.CustomerName),
            paymentMethod: parseString(row.paymentMethod ?? row.PaymentMethod),
            status: parseString(row.status ?? row.Status),
            transactionId: parseString(row.transactionId ?? row.TransactionId),
            amount: parseNumber(row.amount ?? row.Amount),
            currencyCode: parseString(row.currencyCode ?? row.CurrencyCode) || 'EUR',
            paymentGatewayId: parseNullableNumber(row.paymentGatewayId ?? row.PaymentGatewayId),
            paymentGatewayName: parseString(row.paymentGatewayName ?? row.PaymentGatewayName),
            processedAt: parseString(row.processedAt ?? row.ProcessedAt),
            refundedAmount: parseNumber(row.refundedAmount ?? row.RefundedAmount),
            createdAt: parseString(row.createdAt ?? row.CreatedAt),
            allocations,
        };
    };
    const statusBadgeClass = (status) => {
        switch (status) {
            case 'Completed':
                return 'success';
            case 'Processing':
                return 'primary';
            case 'Pending':
                return 'warning text-dark';
            case 'Failed':
                return 'danger';
            case 'Refunded':
            case 'PartiallyRefunded':
                return 'info text-dark';
            case 'Disputed':
                return 'dark';
            default:
                return 'secondary';
        }
    };
    const renderAllocationsPanel = (payment) => {
        if (!payment.allocations.length) {
            return `
                <div class="small text-muted py-2">No invoice allocations found for this payment transaction.</div>
                <div class="small text-muted">
                    <div><strong>Gateway:</strong> ${esc(payment.paymentGatewayName || '-')}</div>
                    <div><strong>Refunded:</strong> ${esc(formatMoney(payment.refundedAmount, payment.currencyCode))}</div>
                </div>
            `;
        }
        const rows = [...payment.allocations].sort((a, b) => {
            const ad = new Date(a.createdAt).getTime();
            const bd = new Date(b.createdAt).getTime();
            return bd - ad;
        });
        return `
            <div class="py-2">
                <div class="fw-semibold mb-2">Invoice allocations</div>
                <div class="table-responsive">
                    <table class="table table-sm mb-2">
                        <thead>
                            <tr>
                                <th>ID</th>
                                <th>Applied</th>
                                <th>Invoice Total</th>
                                <th>Balance</th>
                                <th>Full Payment</th>
                                <th>Created</th>
                            </tr>
                        </thead>
                        <tbody>
                            ${rows.map((allocation) => `
                                <tr>
                                    <td>${allocation.id}</td>
                                    <td>${esc(formatMoney(allocation.amountApplied, allocation.currency))}</td>
                                    <td>${esc(formatMoney(allocation.invoiceTotalAmount, allocation.currency))}</td>
                                    <td>${esc(formatMoney(allocation.invoiceBalance, allocation.currency))}</td>
                                    <td>${allocation.isFullPayment ? '<span class="badge bg-success">Yes</span>' : '<span class="badge bg-secondary">No</span>'}</td>
                                    <td>${esc(formatDate(allocation.createdAt))}</td>
                                </tr>
                            `).join('')}
                        </tbody>
                    </table>
                </div>
                <div class="small text-muted">
                    <div><strong>Gateway:</strong> ${esc(payment.paymentGatewayName || '-')}</div>
                    <div><strong>Refunded:</strong> ${esc(formatMoney(payment.refundedAmount, payment.currencyCode))}</div>
                </div>
            </div>
        `;
    };
    const showError = (message) => {
        const alert = document.getElementById('payments-alert-error');
        if (!alert) {
            return;
        }
        alert.textContent = message;
        alert.classList.remove('d-none');
        document.getElementById('payments-alert-success')?.classList.add('d-none');
    };
    const hideError = () => {
        document.getElementById('payments-alert-error')?.classList.add('d-none');
    };
    const loadPageSizeFromUi = () => {
        const select = document.getElementById('payments-page-size');
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
        const status = getFilterValue('payments-filter-status');
        const customerIdRaw = getFilterValue('payments-filter-customerid');
        const invoiceIdRaw = getFilterValue('payments-filter-invoiceid');
        const customerId = customerIdRaw ? Number(customerIdRaw) : null;
        const invoiceId = invoiceIdRaw ? Number(invoiceIdRaw) : null;
        filteredPayments = allPayments.filter((payment) => {
            if (status && payment.status !== status) {
                return false;
            }
            if (customerId !== null && Number.isFinite(customerId) && payment.customerId !== customerId) {
                return false;
            }
            if (invoiceId !== null && Number.isFinite(invoiceId) && payment.invoiceId !== invoiceId) {
                return false;
            }
            return true;
        });
        currentPage = 1;
        updateView();
    };
    const resetFilters = () => {
        const status = document.getElementById('payments-filter-status');
        const customerId = document.getElementById('payments-filter-customerid');
        const invoiceId = document.getElementById('payments-filter-invoiceid');
        if (status) {
            status.value = '';
        }
        if (customerId) {
            customerId.value = '';
        }
        if (invoiceId) {
            invoiceId.value = '';
        }
        filteredPayments = [...allPayments];
        currentPage = 1;
        updateView();
    };
    const getPagedPayments = () => {
        totalCount = filteredPayments.length;
        totalPages = Math.max(1, Math.ceil(totalCount / pageSize));
        if (currentPage > totalPages) {
            currentPage = totalPages;
        }
        const start = (currentPage - 1) * pageSize;
        return filteredPayments.slice(start, start + pageSize);
    };
    const renderTable = () => {
        const tableBody = document.getElementById('payments-table-body');
        if (!tableBody) {
            return;
        }
        const paged = getPagedPayments();
        if (!paged.length) {
            tableBody.innerHTML = '<tr><td colspan="8" class="text-center text-muted">No payment transactions found.</td></tr>';
            return;
        }
        tableBody.innerHTML = paged.map((payment) => {
            const isSelected = selectedPaymentId === payment.id;
            return `
                <tr data-payment-id="${payment.id}">
                    <td>${payment.id}</td>
                    <td><code>${esc(payment.transactionId || '-')}</code></td>
                    <td><a href="/billing/invoices?id=${encodeURIComponent(String(payment.invoiceId))}">#${payment.invoiceId}</a>${payment.invoiceNumber ? ` <span class="text-muted">(${esc(payment.invoiceNumber)})</span>` : ''}</td>
                    <td>${payment.customerId}${payment.customerName ? ` <span class="text-muted">(${esc(payment.customerName)})</span>` : ''}</td>
                    <td><span class="badge bg-${statusBadgeClass(payment.status)}">${esc(payment.status || '-')}</span></td>
                    <td>${esc(payment.paymentMethod || '-')}</td>
                    <td>${esc(formatMoney(payment.amount, payment.currencyCode))}</td>
                    <td>${esc(formatDate(payment.processedAt || payment.createdAt))}</td>
                </tr>
                ${isSelected ? `<tr class="payments-row-drilldown"><td colspan="8">${renderAllocationsPanel(payment)}</td></tr>` : ''}
            `;
        }).join('');
    };
    const renderPagination = () => {
        const info = document.getElementById('payments-pagination-info');
        const list = document.getElementById('payments-paging-controls-list');
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
    const loadPayments = async () => {
        const tableBody = document.getElementById('payments-table-body');
        if (tableBody) {
            tableBody.innerHTML = '<tr><td colspan="8" class="text-center"><div class="spinner-border text-primary"></div></td></tr>';
        }
        hideError();
        const response = await apiRequest(`${getApiBaseUrl()}/PaymentTransactions`, { method: 'GET' });
        if (!response.success) {
            showError(response.message || 'Failed to load payment transactions.');
            if (tableBody) {
                tableBody.innerHTML = '<tr><td colspan="8" class="text-center text-danger">Failed to load data</td></tr>';
            }
            return;
        }
        const list = extractPayments(response.data);
        allPayments = list.map((item) => normalizePayment(item));
        filteredPayments = [...allPayments];
        if (selectedPaymentId !== null && !filteredPayments.some((payment) => payment.id === selectedPaymentId)) {
            selectedPaymentId = null;
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
        document.getElementById('payments-apply')?.addEventListener('click', applyFilters);
        document.getElementById('payments-reset')?.addEventListener('click', resetFilters);
        document.getElementById('payments-page-size')?.addEventListener('change', () => {
            currentPage = 1;
            updateView();
        });
        document.getElementById('payments-paging-controls')?.addEventListener('click', (event) => {
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
        document.getElementById('payments-table-body')?.addEventListener('click', (event) => {
            const target = event.target;
            const row = target.closest('tr[data-payment-id]');
            if (!row) {
                return;
            }
            const paymentId = Number(row.dataset.paymentId ?? '0');
            if (!Number.isFinite(paymentId) || paymentId <= 0) {
                return;
            }
            selectedPaymentId = selectedPaymentId === paymentId ? null : paymentId;
            updateView();
        });
    };
    const initializePage = () => {
        const page = document.getElementById('payments-page');
        if (!page || page.dataset.initialized === 'true') {
            return;
        }
        page.dataset.initialized = 'true';
        bindEvents();
        void loadPayments();
    };
    const setupObserver = () => {
        initializePage();
        if (document.body) {
            const observer = new MutationObserver(() => {
                const page = document.getElementById('payments-page');
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
//# sourceMappingURL=payments.js.map
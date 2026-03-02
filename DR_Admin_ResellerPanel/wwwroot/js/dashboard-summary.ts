// @ts-nocheck
(function() {
interface Domain {
    id: number;
    name: string;
}

let hasOngoingWorkflowWarning = false;
let pendingAcceptQuoteId: number | null = null;
let pendingAcceptQuoteNumber = '';

interface PagedResult<T> {
    data?: T[];
    Data?: T[];
    currentPage?: number;
    CurrentPage?: number;
    pageSize?: number;
    PageSize?: number;
    totalCount?: number;
    TotalCount?: number;
    totalPages?: number;
    TotalPages?: number;
}

interface ApiResponse<T> {
    success: boolean;
    data?: T;
    message?: string;
}

interface NewSaleState {
    showOngoingCard?: boolean;
    isOfferListContext?: boolean;
    domainName?: string;
    selectedCustomer?: {
        id?: number;
        name?: string;
        customerName?: string;
    };
    pricing?: {
        currency?: string;
    };
    otherServices?: {
        currency?: string;
    };
    offer?: {
        quoteId?: number;
        status?: string;
        lastAction?: string;
        lastRevisionNumber?: number;
        acceptedAt?: string;
        grandTotal?: number;
    };
}

interface QuoteSummaryItem {
    id: number;
    quoteNumber: string;
    domainName: string;
    customerName: string;
    createdAt: string;
    status: string;
    totalAmount: number;
    currencyCode: string;
}

interface OrderSummaryItem {
    id: number;
    orderNumber: string;
    status: string;
    totalAmount: number;
    currencyCode: string;
    quoteId?: number;
}

interface InvoiceSummaryItem {
    id: number;
    invoiceNumber: string;
    status: string;
    totalAmount: number;
    currencyCode: string;
}

function getApiBaseUrl(): string {
    return (window as any).AppSettings?.apiBaseUrl ?? '';
}

function getBootstrap(): any | null {
    return (window as any).bootstrap ?? null;
}

function getAuthToken(): string | null {
    const auth = (window as any).Auth;
    if (auth?.getToken) {
        return auth.getToken();
    }

    return sessionStorage.getItem('rp_authToken');
}

async function apiRequest<T>(endpoint: string, options: RequestInit = {}): Promise<ApiResponse<T>> {
    try {
        const headers: Record<string, string> = {
            'Content-Type': 'application/json',
            ...(options.headers as Record<string, string>),
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
            success: (data as any)?.success !== false,
            data: (data as any)?.data ?? data,
            message: (data as any)?.message,
        };
    } catch (error) {
        console.error('Dashboard summary request failed', error);
        return {
            success: false,
            message: 'Network error. Please try again.',
        };
    }
}

function initializePage(): void {
    const page = document.getElementById('dashboard-summary-page') as HTMLElement | null;
    if (!page || (page as any).dataset.initialized === 'true') {
        return;
    }

    (page as any).dataset.initialized = 'true';

    bindQuoteActions();

    renderOngoingWorkflowPanel();
    loadPendingSummary();
    loadSalesSummary();
}

function updateSalesCardLayout(quoteCount: number, orderCount: number, invoiceCount: number): void {
    const cards = [
        { id: 'dashboard-summary-quotes-card-wrap', count: quoteCount },
        { id: 'dashboard-summary-orders-card-wrap', count: orderCount },
        { id: 'dashboard-summary-open-invoices-card-wrap', count: invoiceCount },
    ];

    const filled = cards.filter((card) => card.count > 0);
    const empty = cards.filter((card) => card.count <= 0);

    filled.forEach((card, index) => {
        const element = document.getElementById(card.id);
        if (!element) {
            return;
        }

        element.className = 'col-12';
        element.style.order = String(index + 1);
    });

    empty.forEach((card, index) => {
        const element = document.getElementById(card.id);
        if (!element) {
            return;
        }

        element.className = 'col-12 col-xl-4';
        element.style.order = String(filled.length + index + 1);
    });
}

function renderOngoingWorkflowPanel(): void {
    const card = document.getElementById('dashboard-summary-workflow-card');
    const draftLink = document.getElementById('dashboard-summary-workflow-link');
    if (!card) {
        return;
    }

    const state = loadNewSaleState();
    const domainName = state?.domainName?.trim() ?? '';
    const customer = state?.selectedCustomer;
    const showOngoingCard = state?.showOngoingCard === true;
    const hideWorkflowPanel = state?.isOfferListContext === true || !showOngoingCard;

    if (!domainName || hideWorkflowPanel) {
        hasOngoingWorkflowWarning = false;
        card.classList.add('d-none');
        return;
    }

    const customerId = Number(customer?.id ?? 0);
    const customerName = customer?.name?.trim() || customer?.customerName?.trim() || (customerId > 0 ? `#${customerId}` : '-');
    setText('dashboard-summary-workflow-domain', domainName);
    setText('dashboard-summary-workflow-customer', customerName);
    setText('dashboard-summary-workflow-status', state?.offer?.status ?? 'Draft');

    if (draftLink) {
        draftLink.classList.remove('d-none');
    }

    hasOngoingWorkflowWarning = true;
    card.classList.remove('d-none');
}

async function loadSalesSummary(): Promise<void> {
    const response = await apiRequest<any>(`${getApiBaseUrl()}/System/sales-summary`, { method: 'GET' });
    if (!response.success || !response.data) {
        renderSummaryTable('dashboard-summary-offers-body', [], 'Could not load quotes');
        renderSummaryTable('dashboard-summary-orders-body', [], 'Could not load orders');
        renderSummaryTable('dashboard-summary-open-invoices-body', [], 'Could not load open invoices');
        setText('dashboard-summary-offers-count', '0');
        setText('dashboard-summary-orders-count', '0');
        setText('dashboard-summary-open-invoices-count', '0');
        updateSalesCardLayout(0, 0, 0);
        return;
    }

    const offers = extractItems(response.data?.offers).items
        .map(normalizeQuote)
        .filter((item) => item.status.toLowerCase() !== 'converted')
        .slice(0, 8);
    const orders = extractItems(response.data?.orders).items
        .map(normalizeOrder)
        .slice(0, 8);
    const ordersWithAcceptedDraft = orders;
    const openInvoices = extractItems(response.data?.openInvoices).items
        .map(normalizeInvoice)
        .slice(0, 8);

    setText('dashboard-summary-offers-count', String(offers.length));
    setText('dashboard-summary-orders-count', String(ordersWithAcceptedDraft.length));
    setText('dashboard-summary-open-invoices-count', String(openInvoices.length));
    updateSalesCardLayout(offers.length, ordersWithAcceptedDraft.length, openInvoices.length);

    renderQuoteTable(offers);

    renderSummaryTable(
        'dashboard-summary-orders-body',
        ordersWithAcceptedDraft.map((item) => ({
            identifier: item.orderNumber || `#${item.id}`,
            status: item.status,
            amount: formatMoney(item.totalAmount, item.currencyCode),
        })),
        'No orders found'
    );

    renderSummaryTable(
        'dashboard-summary-open-invoices-body',
        openInvoices.map((item) => ({
            identifier: item.invoiceNumber || `#${item.id}`,
            status: item.status,
            amount: formatMoney(item.totalAmount, item.currencyCode),
        })),
        'No open invoices found'
    );
}

async function loadOffersSummary(): Promise<void> {
    const response = await apiRequest<any[]>(`${getApiBaseUrl()}/Quotes`, { method: 'GET' });
    if (!response.success) {
        renderSummaryTable('dashboard-summary-offers-body', [], 'Could not load quotes');
        setText('dashboard-summary-offers-count', '0');
        return;
    }

    const rawItems = extractItems(response.data).items;
    const offers = rawItems
        .map(normalizeQuote)
        .filter((item) => item.status.toLowerCase() !== 'converted')
        .sort((a, b) => b.id - a.id)
        .slice(0, 8);

    setText('dashboard-summary-offers-count', String(offers.length));
    renderSummaryTable(
        'dashboard-summary-offers-body',
        offers.map((item) => ({
            identifier: item.quoteNumber || `#${item.id}`,
            status: item.status,
            amount: formatMoney(item.totalAmount, item.currencyCode),
        })),
        'No quotes found'
    );
}

async function loadOrdersSummary(): Promise<void> {
    const response = await apiRequest<any[]>(`${getApiBaseUrl()}/Orders`, { method: 'GET' });
    if (!response.success) {
        renderSummaryTable('dashboard-summary-orders-body', [], 'Could not load orders');
        setText('dashboard-summary-orders-count', '0');
        return;
    }

    const rawItems = extractItems(response.data).items;
    const orders = rawItems
        .map(normalizeOrder)
        .sort((a, b) => b.id - a.id)
        .slice(0, 8);

    setText('dashboard-summary-orders-count', String(orders.length));
    renderSummaryTable(
        'dashboard-summary-orders-body',
        orders.map((item) => ({
            identifier: item.orderNumber || `#${item.id}`,
            status: item.status,
            amount: formatMoney(item.totalAmount, item.currencyCode),
        })),
        'No orders found'
    );
}

async function loadOpenInvoicesSummary(): Promise<void> {
    const [issuedResponse, overdueResponse, draftResponse] = await Promise.all([
        apiRequest<any[]>(`${getApiBaseUrl()}/Invoices/status/Issued`, { method: 'GET' }),
        apiRequest<any[]>(`${getApiBaseUrl()}/Invoices/status/Overdue`, { method: 'GET' }),
        apiRequest<any[]>(`${getApiBaseUrl()}/Invoices/status/Draft`, { method: 'GET' }),
    ]);

    const responses = [issuedResponse, overdueResponse, draftResponse].filter((res) => res.success);
    const failedAll = responses.length === 0;
    if (failedAll) {
        renderSummaryTable('dashboard-summary-open-invoices-body', [], 'Could not load open invoices');
        setText('dashboard-summary-open-invoices-count', '0');
        return;
    }

    const allRawInvoices = responses.flatMap((res) => extractItems(res.data).items);
    const unique = new Map<number, InvoiceSummaryItem>();
    allRawInvoices.map(normalizeInvoice).forEach((item) => {
        unique.set(item.id, item);
    });

    const openInvoices = Array.from(unique.values())
        .sort((a, b) => b.id - a.id)
        .slice(0, 8);

    setText('dashboard-summary-open-invoices-count', String(openInvoices.length));
    renderSummaryTable(
        'dashboard-summary-open-invoices-body',
        openInvoices.map((item) => ({
            identifier: item.invoiceNumber || `#${item.id}`,
            status: item.status,
            amount: formatMoney(item.totalAmount, item.currencyCode),
        })),
        'No open invoices found'
    );
}

function loadNewSaleState(): NewSaleState | null {
    const raw = sessionStorage.getItem('new-sale-state');
    if (!raw) {
        return null;
    }
    try {
        return JSON.parse(raw) as NewSaleState;
    } catch {
        return null;
    }
}

function normalizeQuote(raw: any): QuoteSummaryItem {
    return {
        id: Number(raw.id ?? raw.Id ?? 0),
        quoteNumber: String(raw.quoteNumber ?? raw.QuoteNumber ?? ''),
        domainName: String(raw.domainName ?? raw.DomainName ?? ''),
        customerName: String(raw.customerName ?? raw.CustomerName ?? ''),
        createdAt: String(raw.createdAt ?? raw.CreatedAt ?? ''),
        status: resolveQuoteStatus(raw.status ?? raw.Status),
        totalAmount: Number(raw.totalAmount ?? raw.TotalAmount ?? 0),
        currencyCode: String(raw.currencyCode ?? raw.CurrencyCode ?? 'USD'),
    };
}

function extractDomainName(value: string): string {
    const text = (value || '').trim();
    if (!text) {
        return '-';
    }

    const match = text.match(/[a-z0-9][a-z0-9\-]*\.[a-z0-9][a-z0-9\-.]*/i);
    return (match ? match[0] : text).toLowerCase();
}

function formatDate(value: string): string {
    const parsed = new Date(value);
    if (!value || Number.isNaN(parsed.getTime())) {
        return '-';
    }

    return parsed.toLocaleDateString();
}

function renderQuoteTable(rows: QuoteSummaryItem[]): void {
    const body = document.getElementById('dashboard-summary-offers-body');
    if (!body) {
        return;
    }

    if (!rows.length) {
        body.innerHTML = '<tr><td colspan="6" class="text-center text-muted">No quotes found</td></tr>';
        return;
    }

    body.innerHTML = rows.map((item) => {
        const quoteNumber = item.quoteNumber || `#${item.id}`;
        const domainName = extractDomainName(item.domainName);
        const customerName = item.customerName || '-';
        const dateText = formatDate(item.createdAt);

        return `
        <tr>
            <td><a href="/dashboard/quote/offer?quoteId=${encodeURIComponent(String(item.id))}">${esc(quoteNumber)}</a></td>
            <td>${esc(domainName)}</td>
            <td>${esc(customerName)}</td>
            <td>${esc(dateText)}</td>
            <td>${esc(item.status)}</td>
            <td class="text-end">
                <button class="btn btn-sm btn-primary" type="button" data-action="accept-quote" data-id="${item.id}" data-quote-number="${esc(quoteNumber)}">Accept</button>
            </td>
        </tr>
    `;
    }).join('');
}

function openAcceptQuoteModal(quoteId: number, quoteNumber: string): void {
    pendingAcceptQuoteId = quoteId;
    pendingAcceptQuoteNumber = quoteNumber;

    setText('dashboard-summary-accept-quote-number', quoteNumber || `#${quoteId}`);

    const modalElement = document.getElementById('dashboard-summary-accept-quote-modal');
    const bootstrap = getBootstrap();
    if (!modalElement || !bootstrap?.Modal) {
        return;
    }

    const modal = bootstrap.Modal.getInstance(modalElement) || new bootstrap.Modal(modalElement);
    modal.show();
}

function closeAcceptQuoteModal(): void {
    const modalElement = document.getElementById('dashboard-summary-accept-quote-modal');
    const bootstrap = getBootstrap();
    if (!modalElement || !bootstrap?.Modal) {
        return;
    }

    const modal = bootstrap.Modal.getInstance(modalElement);
    modal?.hide();
}

async function acceptQuoteFromDashboard(): Promise<void> {
    if (!pendingAcceptQuoteId) {
        return;
    }

    clearError();

    const response = await apiRequest<any>(`${getApiBaseUrl()}/Quotes/${pendingAcceptQuoteId}/convert`, { method: 'POST' });
    if (!response.success) {
        showError(response.message || `Could not accept quote ${pendingAcceptQuoteNumber || `#${pendingAcceptQuoteId}`}.`);
        return;
    }

    closeAcceptQuoteModal();
    pendingAcceptQuoteId = null;
    pendingAcceptQuoteNumber = '';
    await loadSalesSummary();
}

function bindQuoteActions(): void {
    const tableBody = document.getElementById('dashboard-summary-offers-body');
    tableBody?.addEventListener('click', (event) => {
        const target = event.target as HTMLElement;
        const button = target.closest('button[data-action="accept-quote"]') as HTMLButtonElement | null;
        if (!button) {
            return;
        }

        const quoteId = Number(button.dataset.id ?? 0);
        if (!quoteId) {
            return;
        }

        const quoteNumber = button.dataset.quoteNumber || `#${quoteId}`;
        openAcceptQuoteModal(quoteId, quoteNumber);
    });

    document.getElementById('dashboard-summary-accept-quote-confirm')?.addEventListener('click', () => {
        void acceptQuoteFromDashboard();
    });
}

function normalizeOrder(raw: any): OrderSummaryItem {
    return {
        id: Number(raw.id ?? raw.Id ?? 0),
        orderNumber: String(raw.orderNumber ?? raw.OrderNumber ?? ''),
        status: resolveOrderStatus(raw.status ?? raw.Status),
        totalAmount: Number(raw.totalAmount ?? raw.TotalAmount ?? 0),
        currencyCode: String(raw.currencyCode ?? raw.CurrencyCode ?? 'USD'),
        quoteId: Number(raw.quoteId ?? raw.QuoteId ?? 0) || undefined,
    };
}

function normalizeInvoice(raw: any): InvoiceSummaryItem {
    return {
        id: Number(raw.id ?? raw.Id ?? 0),
        invoiceNumber: String(raw.invoiceNumber ?? raw.InvoiceNumber ?? ''),
        status: resolveInvoiceStatus(raw.status ?? raw.Status),
        totalAmount: Number(raw.totalAmount ?? raw.TotalAmount ?? 0),
        currencyCode: String(raw.currencyCode ?? raw.CurrencyCode ?? 'USD'),
    };
}

function resolveQuoteStatus(status: any): string {
    const value = Number(status);
    switch (value) {
        case 0: return 'Draft';
        case 1: return 'Sent';
        case 2: return 'Accepted';
        case 3: return 'Rejected';
        case 4: return 'Expired';
        case 5: return 'Converted';
        default: return String(status ?? '-');
    }
}

function resolveOrderStatus(status: any): string {
    const value = Number(status);
    switch (value) {
        case 0: return 'Pending';
        case 1: return 'Active';
        case 2: return 'Suspended';
        case 3: return 'Cancelled';
        case 4: return 'Expired';
        case 5: return 'Trial';
        default: return String(status ?? '-');
    }
}

function resolveInvoiceStatus(status: any): string {
    const value = Number(status);
    switch (value) {
        case 0: return 'Draft';
        case 1: return 'Issued';
        case 2: return 'Paid';
        case 3: return 'Overdue';
        case 4: return 'Cancelled';
        case 5: return 'Credited';
        default: return String(status ?? '-');
    }
}

function formatMoney(amount: number, currency: string): string {
    const normalizedAmount = Number.isFinite(amount) ? amount : 0;
    return `${normalizedAmount.toFixed(2)} ${currency || 'USD'}`;
}

function renderSummaryTable(
    bodyId: string,
    rows: { identifier: string; status: string; amount: string; linkUrl?: string; openInNewTab?: boolean }[],
    emptyMessage: string
): void {
    const body = document.getElementById(bodyId);
    if (!body) {
        return;
    }

    if (!rows.length) {
        body.innerHTML = `<tr><td colspan="3" class="text-center text-muted">${esc(emptyMessage)}</td></tr>`;
        return;
    }

    body.innerHTML = rows.map((row) => {
        const identifierHtml = row.linkUrl
            ? `<a href="${esc(row.linkUrl)}"${row.openInNewTab ? ' target="_blank" rel="noopener noreferrer"' : ''}>${esc(row.identifier)}</a>`
            : esc(row.identifier);

        return `
        <tr>
            <td>${identifierHtml}</td>
            <td>${esc(row.status)}</td>
            <td class="text-end">${esc(row.amount)}</td>
        </tr>
    `;
    }).join('');
}

async function loadPendingSummary(): Promise<void> {
    clearError();
    setAllClearVisible(false);
    setPendingLoading(true);

    try {
        const domains = await loadAllDomains();
        const pendingResults = await Promise.all(domains.map(async (domain) => {
            const response = await apiRequest<any[]>(`${getApiBaseUrl()}/DnsRecords/domain/${domain.id}/pending-sync`, { method: 'GET' });
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
        } else {
            setText('dashboard-summary-pending-note', `${pending.length} domain(s) require registrar sync.`);
        }
    } catch (error: any) {
        setPendingCardVisible(true);
        setAllClearVisible(false);
        showError(error?.message || 'Failed to load pending DNS records.');
        renderPendingTable([]);
        setText('dashboard-summary-pending-note', 'Unable to load pending DNS records.');
    } finally {
        setPendingLoading(false);
    }
}

async function loadAllDomains(): Promise<Domain[]> {
    let allItems: Domain[] = [];
    let pageNumber = 1;
    const pageSize = 200;
    let totalPages = 1;

    while (pageNumber <= totalPages) {
        const params = new URLSearchParams();
        params.set('pageNumber', String(pageNumber));
        params.set('pageSize', String(pageSize));

        const response = await apiRequest<PagedResult<Domain> | Domain[]>(`${getApiBaseUrl()}/RegisteredDomains?${params.toString()}`, { method: 'GET' });
        if (!response.success) {
            throw new Error(response.message || 'Failed to load domains');
        }

        const raw = response.data as any;
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

function extractItems(raw: any): { items: any[]; meta: any } {
    if (Array.isArray(raw)) {
        return { items: raw, meta: null };
    }

    const candidates: any[] = [raw, raw?.data, raw?.Data, raw?.data?.data, raw?.data?.Data];

    const items =
        (Array.isArray(raw?.Data) && raw.Data) ||
        (Array.isArray(raw?.data) && raw.data) ||
        (Array.isArray(raw?.data?.Data) && raw.data.Data) ||
        (Array.isArray(raw?.data?.data) && raw.data.data) ||
        (Array.isArray(raw?.Data?.Data) && raw.Data.Data) ||
        [];

    const meta = candidates.find((c) => c && typeof c === 'object' && (
        c.totalCount !== undefined || c.TotalCount !== undefined ||
        c.totalPages !== undefined || c.TotalPages !== undefined ||
        c.currentPage !== undefined || c.CurrentPage !== undefined ||
        c.pageSize !== undefined || c.PageSize !== undefined
    ));

    return { items, meta };
}

function normalizeDomain(raw: any): Domain {
    return {
        id: raw.id ?? raw.Id ?? 0,
        name: raw.name ?? raw.Name ?? raw.domainName ?? '',
    };
}

function setPendingLoading(isLoading: boolean): void {
    const loading = document.getElementById('dashboard-summary-pending-loading');
    if (loading) {
        loading.classList.toggle('d-none', !isLoading);
    }
}

function setPendingCardVisible(isVisible: boolean): void {
    const card = document.getElementById('dashboard-summary-pending-card');
    if (card) {
        card.classList.toggle('d-none', !isVisible);
    }
}

function setAllClearVisible(isVisible: boolean): void {
    const card = document.getElementById('dashboard-summary-all-clear-card');
    if (card) {
        card.classList.toggle('d-none', !isVisible);
    }
}

function renderPendingTable(rows: { domain: Domain; count: number | null }[]): void {
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

function setText(id: string, value: string): void {
    const element = document.getElementById(id);
    if (element) {
        element.textContent = value;
    }
}

function showError(message: string): void {
    const alert = document.getElementById('dashboard-summary-alert-error');
    if (!alert) {
        return;
    }

    alert.textContent = message;
    alert.classList.remove('d-none');
}

function clearError(): void {
    const alert = document.getElementById('dashboard-summary-alert-error');
    if (alert) {
        alert.textContent = '';
        alert.classList.add('d-none');
    }
}

function esc(value: string): string {
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

function setupPageObserver(): void {
    initializePage();

    if (document.body) {
        const observer = new MutationObserver(() => {
            const page = document.getElementById('dashboard-summary-page');
            if (page && (page as any).dataset.initialized !== 'true') {
                initializePage();
            }
        });
        observer.observe(document.body, { childList: true, subtree: true });
    }
}

if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', setupPageObserver);
} else {
    setupPageObserver();
}

})();

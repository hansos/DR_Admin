((): void => {
interface InvoiceDto {
    id: number;
    invoiceNumber: string;
    status: string;
    issueDate: string;
    dueDate: string;
    totalAmount: number;
    amountDue: number;
    currencyCode: string;
    paymentMethod: string;
    notes: string;
}

interface UserAccountDto {
    customer?: { id: number } | null;
}

interface PagedResult<T> {
    items?: T[];
    data?: T[];
}

interface InvoicesWindow extends Window {
    UserPanelApi?: {
        request: <T>(path: string, options?: RequestInit, requiresAuth?: boolean) => Promise<{ success: boolean; data?: T; message?: string }>;
    };
    UserPanelAlerts?: {
        showError: (id: string, message: string) => void;
    };
}

let invoicesCustomerId: number | null = null;
let invoicesPageNumber = 1;
const invoicesPageSize = 10;
let invoicesLastCount = 0;

function initializeInvoicesPage(): void {
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

    document.getElementById('invoices-table-body')?.addEventListener('click', (event: Event) => {
        const target = event.target as HTMLElement;
        const button = target.closest('button[data-id]') as HTMLButtonElement | null;
        if (!button) {
            return;
        }

        const id = Number.parseInt(button.dataset.id ?? '', 10);
        if (!Number.isNaN(id) && id > 0) {
            void loadInvoiceDetails(id);
        }
    });

    void loadInvoices();
}

async function loadInvoices(): Promise<void> {
    const typedWindow = window as InvoicesWindow;
    invoicesCustomerId = await resolveInvoicesCustomerId();

    if (!invoicesCustomerId) {
        typedWindow.UserPanelAlerts?.showError('invoices-alert-error', 'Could not resolve customer account.');
        renderInvoicesRows([]);
        return;
    }

    const response = await typedWindow.UserPanelApi?.request<PagedResult<InvoiceDto> | InvoiceDto[]>(`/Invoices/customer/${invoicesCustomerId}?pageNumber=${invoicesPageNumber}&pageSize=${invoicesPageSize}`, { method: 'GET' }, true);

    if (!response || !response.success || !response.data) {
        typedWindow.UserPanelAlerts?.showError('invoices-alert-error', response?.message ?? 'Could not load invoices.');
        renderInvoicesRows([]);
        return;
    }

    const items = normalizeInvoices(response.data);
    invoicesLastCount = items.length;
    renderInvoicesRows(items);
    renderInvoicesPageInfo(items.length);
}

function normalizeInvoices(payload: PagedResult<InvoiceDto> | InvoiceDto[]): InvoiceDto[] {
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

function renderInvoicesRows(items: InvoiceDto[]): void {
    const tableBody = document.getElementById('invoices-table-body');
    if (!tableBody) {
        return;
    }

    if (items.length === 0) {
        tableBody.innerHTML = '<tr><td colspan="6" class="text-center text-muted">No invoices found.</td></tr>';
        return;
    }

    tableBody.innerHTML = items.map((item) => `
        <tr>
            <td>${escapeInvoicesText(item.invoiceNumber)}</td>
            <td>${escapeInvoicesText(item.status)}</td>
            <td>${formatInvoicesDate(item.issueDate)}</td>
            <td>${formatInvoicesDate(item.dueDate)}</td>
            <td>${item.totalAmount.toFixed(2)} ${escapeInvoicesText(item.currencyCode)}</td>
            <td><button class="btn btn-outline-primary btn-sm" type="button" data-id="${item.id}">View</button></td>
        </tr>
    `).join('');
}

function renderInvoicesPageInfo(count: number): void {
    const info = document.getElementById('invoices-pagination-info');
    if (info) {
        info.textContent = `Page ${invoicesPageNumber} · Showing ${count} item(s)`;
    }
}

async function loadInvoiceDetails(id: number): Promise<void> {
    const typedWindow = window as InvoicesWindow;
    const response = await typedWindow.UserPanelApi?.request<InvoiceDto>(`/Invoices/${id}`, { method: 'GET' }, true);

    if (!response || !response.success || !response.data) {
        typedWindow.UserPanelAlerts?.showError('invoices-alert-error', response?.message ?? 'Could not load invoice details.');
        return;
    }

    const card = document.getElementById('invoices-details-card');
    const body = document.getElementById('invoices-details-body');
    if (!card || !body) {
        return;
    }

    card.classList.remove('d-none');
    body.innerHTML = `
        <div><strong>${escapeInvoicesText(response.data.invoiceNumber)}</strong></div>
        <div>Status: ${escapeInvoicesText(response.data.status)}</div>
        <div>Due: ${formatInvoicesDate(response.data.dueDate)}</div>
        <div>Total: ${response.data.totalAmount.toFixed(2)} ${escapeInvoicesText(response.data.currencyCode)}</div>
        <div>Amount due: ${response.data.amountDue.toFixed(2)} ${escapeInvoicesText(response.data.currencyCode)}</div>
        <div>Payment method: ${escapeInvoicesText(response.data.paymentMethod || '-')}</div>
        <div class="mt-2 text-muted">${escapeInvoicesText(response.data.notes || 'No notes')}</div>
    `;
}

async function resolveInvoicesCustomerId(): Promise<number | null> {
    const typedWindow = window as InvoicesWindow;
    const response = await typedWindow.UserPanelApi?.request<UserAccountDto>('/MyAccount/me', { method: 'GET' }, true);
    return response?.success ? (response.data?.customer?.id ?? null) : null;
}

function formatInvoicesDate(value: string): string {
    const date = new Date(value);
    if (Number.isNaN(date.getTime())) {
        return '-';
    }

    return date.toLocaleDateString();
}

function escapeInvoicesText(value: string): string {
    return value
        .replace(/&/g, '&amp;')
        .replace(/</g, '&lt;')
        .replace(/>/g, '&gt;')
        .replace(/"/g, '&quot;')
        .replace(/'/g, '&#039;');
}

function setupInvoicesObserver(): void {
    initializeInvoicesPage();

    const observer = new MutationObserver(() => {
        initializeInvoicesPage();
    });

    observer.observe(document.body, { childList: true, subtree: true });
}

if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', setupInvoicesObserver);
} else {
    setupInvoicesObserver();
}
})();

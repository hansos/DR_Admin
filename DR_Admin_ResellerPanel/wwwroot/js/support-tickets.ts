(function() {
interface SupportTicketMessage {
    id: number;
    senderUserId: number;
    senderUsername: string;
    senderRole: string;
    message: string;
    createdAt: string;
}

interface SupportTicket {
    id: number;
    customerId: number;
    customerName: string;
    subject: string;
    status: string;
    priority: string;
    updatedAt: string;
    lastMessageAt?: string | null;
    messages: SupportTicketMessage[];
}

interface PagedSupportTickets {
    data?: SupportTicket[];
    Data?: SupportTicket[];
    totalCount?: number;
    TotalCount?: number;
    pageSize?: number;
    PageSize?: number;
    currentPage?: number;
    CurrentPage?: number;
    totalPages?: number;
    TotalPages?: number;
}

interface ApiResponse<T> {
    success: boolean;
    data?: T;
    message?: string;
}

interface AuthWindow extends Window {
    AppSettings?: { apiBaseUrl?: string };
    Auth?: { getToken?: () => string | null };
}

const typedWindow = window as AuthWindow;
let items: SupportTicket[] = [];
let currentPage = 1;
let pageSize = 25;
let totalPages = 1;
let totalCount = 0;
let selectedTicketId: number | null = null;
let autoRefreshHandle: number | null = null;
let isAutoRefreshing = false;

function getApiBaseUrl(): string {
    return typedWindow.AppSettings?.apiBaseUrl ?? '';
}

function getAuthToken(): string | null {
    if (typedWindow.Auth?.getToken) {
        return typedWindow.Auth.getToken();
    }

    return sessionStorage.getItem('rp_authToken');
}

async function apiRequest<T>(endpoint: string, options: RequestInit = {}): Promise<ApiResponse<T>> {
    try {
        const headers = new Headers(options.headers ?? {});
        headers.set('Content-Type', 'application/json');

        const authToken = getAuthToken();
        if (authToken) {
            headers.set('Authorization', `Bearer ${authToken}`);
        }

        const response = await fetch(endpoint, {
            ...options,
            headers,
            credentials: 'include'
        });

        const contentType = response.headers.get('content-type') ?? '';
        const hasJson = contentType.includes('application/json');
        const data = hasJson ? await response.json() as unknown : null;

        if (!response.ok) {
            const payload = data as { message?: string; title?: string } | null;
            return {
                success: false,
                message: payload?.message ?? payload?.title ?? `Request failed with status ${response.status}`
            };
        }

        return {
            success: true,
            data: data as T
        };
    } catch {
        return {
            success: false,
            message: 'Network error. Please try again.'
        };
    }
}

function normalizeTicket(item: SupportTicket): SupportTicket {
    const raw = item as unknown as Record<string, unknown>;
    return {
        id: Number(raw.id ?? raw.Id ?? 0),
        customerId: Number(raw.customerId ?? raw.CustomerId ?? 0),
        customerName: String(raw.customerName ?? raw.CustomerName ?? ''),
        subject: String(raw.subject ?? raw.Subject ?? ''),
        status: String(raw.status ?? raw.Status ?? ''),
        priority: String(raw.priority ?? raw.Priority ?? ''),
        updatedAt: String(raw.updatedAt ?? raw.UpdatedAt ?? ''),
        lastMessageAt: (raw.lastMessageAt ?? raw.LastMessageAt ?? null) as string | null,
        messages: normalizeMessages(raw.messages ?? raw.Messages)
    };
}

function normalizeMessages(value: unknown): SupportTicketMessage[] {
    if (!Array.isArray(value)) {
        return [];
    }

    return value.map((item) => {
        const raw = item as Record<string, unknown>;
        return {
            id: Number(raw.id ?? raw.Id ?? 0),
            senderUserId: Number(raw.senderUserId ?? raw.SenderUserId ?? 0),
            senderUsername: String(raw.senderUsername ?? raw.SenderUsername ?? ''),
            senderRole: String(raw.senderRole ?? raw.SenderRole ?? ''),
            message: String(raw.message ?? raw.Message ?? ''),
            createdAt: String(raw.createdAt ?? raw.CreatedAt ?? '')
        };
    });
}

function getStatusFilter(): string {
    const status = document.getElementById('reseller-support-status-filter') as HTMLSelectElement | null;
    return status?.value ?? '';
}

function buildTicketsUrl(): string {
    const params = new URLSearchParams();
    params.set('pageNumber', String(currentPage));
    params.set('pageSize', String(pageSize));

    const status = getStatusFilter();
    if (status) {
        params.set('status', status);
    }

    return `${getApiBaseUrl()}/SupportTickets?${params.toString()}`;
}

async function loadTickets(): Promise<void> {
    const tableBody = document.getElementById('reseller-support-table-body');
    if (!tableBody) {
        return;
    }

    tableBody.innerHTML = '<tr><td colspan="7" class="text-center"><div class="spinner-border text-primary"></div></td></tr>';
    hideAlerts();

    const response = await apiRequest<PagedSupportTickets | SupportTicket[]>(buildTicketsUrl(), { method: 'GET' });
    if (!response.success || !response.data) {
        showError(response.message ?? 'Failed to load support tickets.');
        tableBody.innerHTML = '<tr><td colspan="7" class="text-center text-danger">Failed to load tickets.</td></tr>';
        return;
    }

    const result = response.data as unknown as Record<string, unknown>;
    const listCandidate = Array.isArray(result.data)
        ? result.data
        : Array.isArray(result.Data)
            ? result.Data
            : Array.isArray(response.data)
                ? response.data
                : [];

    items = listCandidate.map((item) => normalizeTicket(item as SupportTicket));

    totalCount = Number(result.totalCount ?? result.TotalCount ?? items.length);
    currentPage = Number(result.currentPage ?? result.CurrentPage ?? currentPage);
    pageSize = Number(result.pageSize ?? result.PageSize ?? pageSize);
    totalPages = Math.max(1, Number(result.totalPages ?? result.TotalPages ?? Math.ceil(totalCount / Math.max(pageSize, 1))));

    renderTable();
    renderPagination();

    if (selectedTicketId !== null) {
        const stillExists = items.some((ticket) => ticket.id === selectedTicketId);
        if (!stillExists) {
            selectedTicketId = null;
            clearDetailPane();
        }
    }
}

function renderTable(): void {
    const tableBody = document.getElementById('reseller-support-table-body');
    if (!tableBody) {
        return;
    }

    if (items.length === 0) {
        tableBody.innerHTML = '<tr><td colspan="7" class="text-center text-muted">No tickets found.</td></tr>';
        return;
    }

    tableBody.innerHTML = items.map((ticket) => {
        const selectedClass = selectedTicketId === ticket.id ? 'table-primary' : '';
        return `<tr class="${selectedClass}">
            <td>${ticket.id}</td>
            <td>${escapeHtml(ticket.customerName)} (#${ticket.customerId})</td>
            <td>${escapeHtml(ticket.subject)}</td>
            <td>${renderStatusBadge(ticket.status)}</td>
            <td>${escapeHtml(ticket.priority)}</td>
            <td>${formatDate(ticket.lastMessageAt ?? ticket.updatedAt)}</td>
            <td><button type="button" class="btn btn-outline-primary btn-sm" data-action="open" data-id="${ticket.id}">Open</button></td>
        </tr>`;
    }).join('');
}

function renderPagination(): void {
    const info = document.getElementById('reseller-support-pagination-info');
    const list = document.getElementById('reseller-support-pagination-list');
    if (!info || !list) {
        return;
    }

    if (totalCount === 0) {
        info.textContent = 'Showing 0 of 0';
        list.innerHTML = '';
        return;
    }

    const start = (currentPage - 1) * pageSize + 1;
    const end = Math.min(totalCount, currentPage * pageSize);
    info.textContent = `Showing ${start}-${end} of ${totalCount}`;

    const previousDisabled = currentPage <= 1;
    const nextDisabled = currentPage >= totalPages;
    list.innerHTML = `
        <li class="page-item ${previousDisabled ? 'disabled' : ''}"><a class="page-link" href="#" data-page="${currentPage - 1}">Previous</a></li>
        <li class="page-item disabled"><span class="page-link">Page ${currentPage} / ${totalPages}</span></li>
        <li class="page-item ${nextDisabled ? 'disabled' : ''}"><a class="page-link" href="#" data-page="${currentPage + 1}">Next</a></li>`;
}

function renderStatusBadge(status: string): string {
    const normalized = status.toLowerCase();
    if (normalized === 'closed') {
        return '<span class="badge bg-secondary">Closed</span>';
    }

    if (normalized === 'inprogress') {
        return '<span class="badge bg-primary">In progress</span>';
    }

    if (normalized === 'waitingforcustomer') {
        return '<span class="badge bg-warning text-dark">Waiting for customer</span>';
    }

    return '<span class="badge bg-success">Open</span>';
}

async function openTicket(ticketId: number): Promise<void> {
    hideAlerts();
    const response = await apiRequest<SupportTicket>(`${getApiBaseUrl()}/SupportTickets/${ticketId}`, { method: 'GET' });
    if (!response.success || !response.data) {
        showError(response.message ?? 'Could not load ticket details.');
        return;
    }

    const ticket = normalizeTicket(response.data);
    selectedTicketId = ticket.id;
    renderTable();
    renderDetails(ticket);
}

function renderDetails(ticket: SupportTicket): void {
    const title = document.getElementById('reseller-support-details-title');
    const thread = document.getElementById('reseller-support-thread');
    const status = document.getElementById('reseller-support-status') as HTMLSelectElement | null;
    const reply = document.getElementById('reseller-support-reply') as HTMLTextAreaElement | null;
    const send = document.getElementById('reseller-support-send') as HTMLButtonElement | null;

    if (title) {
        title.textContent = `Ticket #${ticket.id}: ${ticket.subject}`;
    }

    if (status) {
        status.disabled = false;
        status.value = ticket.status;
        status.dataset.ticketId = String(ticket.id);
    }

    if (reply) {
        reply.disabled = false;
    }

    if (send) {
        send.disabled = false;
    }

    if (!thread) {
        return;
    }

    if (ticket.messages.length === 0) {
        thread.innerHTML = '<p class="text-muted mb-0">No messages yet.</p>';
        return;
    }

    thread.innerHTML = ticket.messages.map((message) => {
        const senderClass = message.senderRole.toLowerCase() === 'support' ? 'text-primary' : 'text-success';
        return `<div class="mb-3">
            <div class="small ${senderClass}"><strong>${escapeHtml(message.senderRole)}</strong> · ${escapeHtml(message.senderUsername)} · ${formatDate(message.createdAt)}</div>
            <div class="mt-1">${escapeHtml(message.message).replace(/\n/g, '<br />')}</div>
        </div>`;
    }).join('');

    thread.scrollTop = thread.scrollHeight;
}

function clearDetailPane(): void {
    const title = document.getElementById('reseller-support-details-title');
    const thread = document.getElementById('reseller-support-thread');
    const status = document.getElementById('reseller-support-status') as HTMLSelectElement | null;
    const reply = document.getElementById('reseller-support-reply') as HTMLTextAreaElement | null;
    const send = document.getElementById('reseller-support-send') as HTMLButtonElement | null;

    if (title) {
        title.textContent = 'Ticket details';
    }

    if (thread) {
        thread.innerHTML = '<p class="text-muted mb-0">Select a ticket to show conversation.</p>';
    }

    if (status) {
        status.disabled = true;
        delete status.dataset.ticketId;
    }

    if (reply) {
        reply.value = '';
        reply.disabled = true;
    }

    if (send) {
        send.disabled = true;
    }
}

async function updateStatus(newStatus: string): Promise<void> {
    const ticketId = selectedTicketId;
    if (!ticketId) {
        return;
    }

    const response = await apiRequest<SupportTicket>(`${getApiBaseUrl()}/SupportTickets/${ticketId}/status`, {
        method: 'PATCH',
        body: JSON.stringify({ status: newStatus })
    });

    if (!response.success) {
        showError(response.message ?? 'Could not update status.');
        return;
    }

    showSuccess('Ticket status updated.');
    await loadTickets();
    await openTicket(ticketId);
}

async function sendReply(event: Event): Promise<void> {
    event.preventDefault();
    const ticketId = selectedTicketId;
    const reply = document.getElementById('reseller-support-reply') as HTMLTextAreaElement | null;
    const message = reply?.value.trim() ?? '';

    if (!ticketId || !message) {
        showError('Reply message is required.');
        return;
    }

    const response = await apiRequest<SupportTicket>(`${getApiBaseUrl()}/SupportTickets/${ticketId}/messages`, {
        method: 'POST',
        body: JSON.stringify({ message })
    });

    if (!response.success) {
        showError(response.message ?? 'Could not send reply.');
        return;
    }

    if (reply) {
        reply.value = '';
    }

    showSuccess('Reply sent.');
    await loadTickets();
    await openTicket(ticketId);
}

function onPaginationClick(event: Event): void {
    const target = event.target as HTMLElement;
    const link = target.closest('a[data-page]') as HTMLAnchorElement | null;
    if (!link) {
        return;
    }

    event.preventDefault();
    const nextPage = Number(link.dataset.page ?? '0');
    if (!Number.isFinite(nextPage) || nextPage < 1 || nextPage > totalPages || nextPage === currentPage) {
        return;
    }

    currentPage = nextPage;
    void loadTickets();
}

function bindEvents(): void {
    document.getElementById('reseller-support-refresh')?.addEventListener('click', () => {
        void loadTickets();
    });

    document.getElementById('reseller-support-apply')?.addEventListener('click', () => {
        currentPage = 1;
        void loadTickets();
    });

    document.getElementById('reseller-support-page-size')?.addEventListener('change', () => {
        const input = document.getElementById('reseller-support-page-size') as HTMLSelectElement | null;
        const parsed = Number(input?.value ?? '25');
        pageSize = Number.isFinite(parsed) && parsed > 0 ? parsed : 25;
        currentPage = 1;
        void loadTickets();
    });

    document.getElementById('reseller-support-table-body')?.addEventListener('click', (event: Event) => {
        const target = event.target as HTMLElement;
        const button = target.closest('button[data-action="open"]') as HTMLButtonElement | null;
        if (!button) {
            return;
        }

        const ticketId = Number(button.dataset.id ?? '0');
        if (!Number.isFinite(ticketId) || ticketId <= 0) {
            return;
        }

        void openTicket(ticketId);
    });

    document.getElementById('reseller-support-reply-form')?.addEventListener('submit', (event) => {
        void sendReply(event);
    });

    document.getElementById('reseller-support-status')?.addEventListener('change', (event: Event) => {
        const select = event.target as HTMLSelectElement;
        void updateStatus(select.value);
    });

    document.getElementById('reseller-support-pagination-list')?.addEventListener('click', onPaginationClick);
}

function showSuccess(message: string): void {
    const success = document.getElementById('reseller-support-alert-success');
    const error = document.getElementById('reseller-support-alert-error');
    if (success) {
        success.textContent = message;
        success.classList.remove('d-none');
    }

    error?.classList.add('d-none');
}

function showError(message: string): void {
    const success = document.getElementById('reseller-support-alert-success');
    const error = document.getElementById('reseller-support-alert-error');
    if (error) {
        error.textContent = message;
        error.classList.remove('d-none');
    }

    success?.classList.add('d-none');
}

function hideAlerts(): void {
    document.getElementById('reseller-support-alert-success')?.classList.add('d-none');
    document.getElementById('reseller-support-alert-error')?.classList.add('d-none');
}

function formatDate(value: string): string {
    const date = new Date(value);
    if (Number.isNaN(date.getTime())) {
        return '-';
    }

    return date.toLocaleString();
}

function escapeHtml(value: string): string {
    return value
        .replace(/&/g, '&amp;')
        .replace(/</g, '&lt;')
        .replace(/>/g, '&gt;')
        .replace(/"/g, '&quot;')
        .replace(/'/g, '&#039;');
}

function initializePage(): void {
    const page = document.getElementById('reseller-support-tickets-page');
    if (!page || page.dataset.initialized === 'true') {
        return;
    }

    page.dataset.initialized = 'true';
    bindEvents();
    clearDetailPane();
    void loadTickets();
}

function setupObserver(): void {
    initializePage();

    const observer = new MutationObserver(() => {
        initializePage();
    });

    observer.observe(document.body, { childList: true, subtree: true });
}

function startAutoRefresh(): void {
    if (autoRefreshHandle !== null) {
        return;
    }

    autoRefreshHandle = window.setInterval(() => {
        if (document.hidden || isAutoRefreshing) {
            return;
        }

        const page = document.getElementById('reseller-support-tickets-page');
        if (!page || page.dataset.initialized !== 'true') {
            return;
        }

        isAutoRefreshing = true;
        void loadTickets()
            .then(async () => {
                if (selectedTicketId !== null) {
                    await openTicket(selectedTicketId);
                }
            })
            .finally(() => {
                isAutoRefreshing = false;
            });
    }, 30000);
}

document.addEventListener('visibilitychange', () => {
    if (!document.hidden) {
        const page = document.getElementById('reseller-support-tickets-page');
        if (!page || page.dataset.initialized !== 'true') {
            return;
        }

        void loadTickets().then(async () => {
            if (selectedTicketId !== null) {
                await openTicket(selectedTicketId);
            }
        });
    }
});

if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', () => {
        setupObserver();
        startAutoRefresh();
    });
} else {
    setupObserver();
    startAutoRefresh();
}
})();

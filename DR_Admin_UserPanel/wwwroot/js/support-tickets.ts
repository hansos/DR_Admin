((): void => {
interface SupportTicketDto {
    id: number;
    customerId: number;
    customerName: string;
    subject: string;
    status: string;
    priority: string;
    messages: SupportTicketMessageDto[];
    updatedAt?: string | null;
    createdAt?: string | null;
}

interface SupportTicketMessageDto {
    id: number;
    senderUserId: number;
    senderUsername: string;
    senderRole: string;
    message: string;
    createdAt: string;
}

interface SupportTicketsWindow extends Window {
    UserPanelApi?: {
        request: <T>(path: string, options?: RequestInit, requiresAuth?: boolean) => Promise<{ success: boolean; data?: T; message?: string }>;
    };
    UserPanelAlerts?: {
        showSuccess: (id: string, message: string) => void;
        showError: (id: string, message: string) => void;
        hide: (id: string) => void;
    };
}

let supportTickets: SupportTicketDto[] = [];
let selectedTicketId: number | null = null;
let autoRefreshHandle: number | null = null;
let isAutoRefreshing = false;

function initializeSupportTicketsPage(): void {
    const page = document.getElementById('support-tickets-page');
    if (!page || page.dataset.initialized === 'true') {
        return;
    }

    page.dataset.initialized = 'true';

    document.getElementById('support-tickets-refresh')?.addEventListener('click', () => {
        void loadSupportTickets();
    });

    document.getElementById('support-tickets-create-form')?.addEventListener('submit', (event) => {
        event.preventDefault();
        void createSupportTicket();
    });

    document.getElementById('support-tickets-table-body')?.addEventListener('click', (event: Event) => {
        const target = event.target as HTMLElement;
        const row = target.closest('tr') as HTMLTableRowElement | null;
        if (!row) {
            return;
        }

        const firstCell = row.querySelector('td');
        const id = Number(firstCell?.textContent ?? '0');
        if (!Number.isFinite(id) || id <= 0) {
            return;
        }

        void openSupportTicket(id);
    });

    document.getElementById('support-ticket-reply-form')?.addEventListener('submit', (event: Event) => {
        event.preventDefault();
        void sendSupportTicketMessage();
    });

    clearConversation();

    void loadSupportTickets();
}

async function loadSupportTickets(): Promise<void> {
    const typedWindow = window as SupportTicketsWindow;
    typedWindow.UserPanelAlerts?.hide('support-tickets-alert-success');
    typedWindow.UserPanelAlerts?.hide('support-tickets-alert-error');

    const response = await typedWindow.UserPanelApi?.request<SupportTicketDto[]>('/SupportTickets', { method: 'GET' }, true);

    if (!response || !response.success || !response.data) {
        typedWindow.UserPanelAlerts?.showError('support-tickets-alert-error', response?.message ?? 'Could not load support tickets.');
        renderSupportTickets([]);
        return;
    }

    supportTickets = response.data;
    renderSupportTickets(supportTickets);

    if (selectedTicketId !== null) {
        const found = supportTickets.find((ticket) => ticket.id === selectedTicketId);
        if (found) {
            renderConversation(found);
        } else {
            selectedTicketId = null;
            clearConversation();
        }
    }
}

function renderSupportTickets(items: SupportTicketDto[]): void {
    const tableBody = document.getElementById('support-tickets-table-body');
    if (!tableBody) {
        return;
    }

    if (items.length === 0) {
        tableBody.innerHTML = '<tr><td colspan="5" class="text-center text-muted">No tickets found.</td></tr>';
        return;
    }

    tableBody.innerHTML = items
        .sort((a, b) => Date.parse(b.updatedAt ?? b.createdAt ?? '') - Date.parse(a.updatedAt ?? a.createdAt ?? ''))
        .map((item) => `<tr class="${selectedTicketId === item.id ? 'table-primary' : ''}">
            <td>${item.id}</td>
            <td>${escapeSupportTicketsText(item.subject)}</td>
            <td>${renderSupportTicketStatus(item.status)}</td>
            <td>${escapeSupportTicketsText(item.priority)}</td>
            <td>${formatSupportTicketsDate(item.updatedAt ?? item.createdAt ?? '')}</td>
        </tr>`)
        .join('');
}

function renderSupportTicketStatus(status: string): string {
    const normalized = status.toLowerCase();
    if (normalized === 'closed') {
        return '<span class="badge rounded-pill bg-secondary">Closed</span>';
    }

    if (normalized === 'inprogress') {
        return '<span class="badge rounded-pill bg-primary">In progress</span>';
    }

    if (normalized === 'waitingforcustomer') {
        return '<span class="badge rounded-pill bg-warning text-dark">Waiting for customer</span>';
    }

    return '<span class="badge rounded-pill bg-success">Open</span>';
}

async function createSupportTicket(): Promise<void> {
    const typedWindow = window as SupportTicketsWindow;
    typedWindow.UserPanelAlerts?.hide('support-tickets-alert-success');
    typedWindow.UserPanelAlerts?.hide('support-tickets-alert-error');

    const subject = readSupportTicketsInputValue('support-tickets-subject');
    const message = readSupportTicketsInputValue('support-tickets-message');
    const priority = readSupportTicketsInputValue('support-tickets-priority');

    if (!subject || !message) {
        typedWindow.UserPanelAlerts?.showError('support-tickets-alert-error', 'Subject and message are required.');
        return;
    }

    const response = await typedWindow.UserPanelApi?.request('/SupportTickets', {
        method: 'POST',
        body: JSON.stringify({ subject, message, priority })
    }, true);

    if (!response || !response.success) {
        typedWindow.UserPanelAlerts?.showError('support-tickets-alert-error', response?.message ?? 'Could not create support ticket.');
        return;
    }

    clearSupportTicketsForm();
    typedWindow.UserPanelAlerts?.showSuccess('support-tickets-alert-success', response.message ?? 'Support ticket created.');
    await loadSupportTickets();
}

async function openSupportTicket(ticketId: number): Promise<void> {
    const typedWindow = window as SupportTicketsWindow;
    const response = await typedWindow.UserPanelApi?.request<SupportTicketDto>(`/SupportTickets/${ticketId}`, { method: 'GET' }, true);

    if (!response || !response.success || !response.data) {
        typedWindow.UserPanelAlerts?.showError('support-tickets-alert-error', response?.message ?? 'Could not load support ticket details.');
        return;
    }

    const ticket = response.data;
    selectedTicketId = ticket.id;
    const existingIndex = supportTickets.findIndex((item) => item.id === ticket.id);
    if (existingIndex >= 0) {
        supportTickets[existingIndex] = ticket;
    } else {
        supportTickets.push(ticket);
    }

    renderSupportTickets(supportTickets);
    renderConversation(ticket);
}

function renderConversation(ticket: SupportTicketDto): void {
    const title = document.getElementById('support-ticket-details-title');
    const thread = document.getElementById('support-ticket-thread');
    const reply = document.getElementById('support-ticket-reply') as HTMLTextAreaElement | null;
    const send = document.getElementById('support-ticket-send') as HTMLButtonElement | null;

    if (title) {
        title.textContent = `Conversation for #${ticket.id}: ${ticket.subject}`;
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

    if (!ticket.messages || ticket.messages.length === 0) {
        thread.innerHTML = '<p class="text-muted mb-0">No messages yet.</p>';
        return;
    }

    thread.innerHTML = ticket.messages
        .sort((a, b) => Date.parse(a.createdAt) - Date.parse(b.createdAt))
        .map((message) => {
            const senderClass = message.senderRole.toLowerCase() === 'support' ? 'text-primary' : 'text-success';
            return `<div class="mb-3">
                <div class="small ${senderClass}"><strong>${escapeSupportTicketsText(message.senderRole)}</strong> · ${escapeSupportTicketsText(message.senderUsername)} · ${formatSupportTicketsDate(message.createdAt)}</div>
                <div class="mt-1">${escapeSupportTicketsText(message.message).replace(/\n/g, '<br />')}</div>
            </div>`;
        })
        .join('');

    thread.scrollTop = thread.scrollHeight;
}

function clearConversation(): void {
    const title = document.getElementById('support-ticket-details-title');
    const thread = document.getElementById('support-ticket-thread');
    const reply = document.getElementById('support-ticket-reply') as HTMLTextAreaElement | null;
    const send = document.getElementById('support-ticket-send') as HTMLButtonElement | null;

    if (title) {
        title.textContent = 'Conversation';
    }

    if (thread) {
        thread.innerHTML = '<p class="text-muted mb-0">Select a ticket to see conversation.</p>';
    }

    if (reply) {
        reply.value = '';
        reply.disabled = true;
    }

    if (send) {
        send.disabled = true;
    }
}

async function sendSupportTicketMessage(): Promise<void> {
    const typedWindow = window as SupportTicketsWindow;
    if (selectedTicketId === null) {
        typedWindow.UserPanelAlerts?.showError('support-tickets-alert-error', 'Select a ticket before sending a message.');
        return;
    }

    const reply = document.getElementById('support-ticket-reply') as HTMLTextAreaElement | null;
    const message = reply?.value.trim() ?? '';
    if (!message) {
        typedWindow.UserPanelAlerts?.showError('support-tickets-alert-error', 'Reply message is required.');
        return;
    }

    const response = await typedWindow.UserPanelApi?.request<SupportTicketDto>(`/SupportTickets/${selectedTicketId}/messages`, {
        method: 'POST',
        body: JSON.stringify({ message })
    }, true);

    if (!response || !response.success || !response.data) {
        typedWindow.UserPanelAlerts?.showError('support-tickets-alert-error', response?.message ?? 'Could not send reply message.');
        return;
    }

    if (reply) {
        reply.value = '';
    }

    typedWindow.UserPanelAlerts?.showSuccess('support-tickets-alert-success', 'Reply sent.');
    await loadSupportTickets();
    renderConversation(response.data);
}

function clearSupportTicketsForm(): void {
    const subject = document.getElementById('support-tickets-subject') as HTMLInputElement | null;
    const message = document.getElementById('support-tickets-message') as HTMLTextAreaElement | null;
    const priority = document.getElementById('support-tickets-priority') as HTMLSelectElement | null;

    if (subject) {
        subject.value = '';
    }

    if (message) {
        message.value = '';
    }

    if (priority) {
        priority.value = 'Normal';
    }
}

function readSupportTicketsInputValue(id: string): string {
    const input = document.getElementById(id) as HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement | null;
    return input?.value.trim() ?? '';
}

function formatSupportTicketsDate(value: string): string {
    const date = new Date(value);
    if (Number.isNaN(date.getTime())) {
        return '-';
    }

    return `${date.toLocaleDateString()} ${date.toLocaleTimeString()}`;
}

function escapeSupportTicketsText(value: string): string {
    return value
        .replace(/&/g, '&amp;')
        .replace(/</g, '&lt;')
        .replace(/>/g, '&gt;')
        .replace(/"/g, '&quot;')
        .replace(/'/g, '&#039;');
}

function setupSupportTicketsObserver(): void {
    initializeSupportTicketsPage();

    const observer = new MutationObserver(() => {
        initializeSupportTicketsPage();
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

        const page = document.getElementById('support-tickets-page');
        if (!page || page.dataset.initialized !== 'true') {
            return;
        }

        isAutoRefreshing = true;
        void loadSupportTickets().finally(() => {
            isAutoRefreshing = false;
        });
    }, 30000);
}

document.addEventListener('visibilitychange', () => {
    if (!document.hidden) {
        const page = document.getElementById('support-tickets-page');
        if (page && page.dataset.initialized === 'true') {
            void loadSupportTickets();
        }
    }
});

if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', () => {
        setupSupportTicketsObserver();
        startAutoRefresh();
    });
} else {
    setupSupportTicketsObserver();
    startAutoRefresh();
}
})();

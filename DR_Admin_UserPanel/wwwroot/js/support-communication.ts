((): void => {
interface CommunicationThreadDto {
    id: number;
    subject: string;
    status: string;
    unreadCount: number;
    lastMessageAtUtc: string | null;
}

interface CommunicationThreadDetailsDto extends CommunicationThreadDto {
    participants: Array<{ emailAddress: string; role: string }>;
    messages: Array<{ id: number; direction: string; fromAddress: string; subject: string; bodyText: string | null; bodyHtml: string | null; sentAtUtc: string | null; receivedAtUtc: string | null; isRead: boolean }>;
}

interface SupportCommunicationWindow extends Window {
    Blazor?: {
        addEventListener?: (eventName: string, callback: () => void) => void;
    };
    UserPanelApi?: {
        request: <T>(path: string, options?: RequestInit, requiresAuth?: boolean) => Promise<{ success: boolean; data?: T; message?: string }>;
    };
    UserPanelAlerts?: {
        showSuccess: (id: string, message: string) => void;
        showError: (id: string, message: string) => void;
        hide: (id: string) => void;
    };
}

let selectedThreadId: number | null = null;
let threads: CommunicationThreadDto[] = [];
let threadsRequestId = 0;
let detailsRequestId = 0;

function initializeSupportCommunicationPage(): void {
    const page = document.getElementById('user-communication-page');
    if (!page || page.dataset.initialized === 'true') {
        return;
    }

    page.dataset.initialized = 'true';

    document.getElementById('user-communication-refresh')?.addEventListener('click', () => {
        void loadThreads();
    });

    document.getElementById('user-communication-table-body')?.addEventListener('click', (event: Event) => {
        const target = event.target as HTMLElement;
        const row = target.closest('tr[data-thread-id]') as HTMLTableRowElement | null;
        if (!row) {
            return;
        }

        const id = Number(row.dataset.threadId ?? '0');
        if (!id) {
            return;
        }

        void openThread(id);
    });

    document.getElementById('user-communication-reply-form')?.addEventListener('submit', (event: Event) => {
        event.preventDefault();
        if (!selectedThreadId) {
            return;
        }

        void sendReply(selectedThreadId);
    });

    document.getElementById('user-communication-thread')?.addEventListener('click', (event: Event) => {
        const target = event.target as HTMLElement;
        const button = target.closest('button[data-mark-read]') as HTMLButtonElement | null;
        if (!button) {
            return;
        }

        const messageId = Number(button.dataset.markRead ?? '0');
        if (!messageId) {
            return;
        }

        void markMessageRead(messageId);
    });

    void loadThreads();
}

async function loadThreads(): Promise<void> {
    const typedWindow = window as SupportCommunicationWindow;
    typedWindow.UserPanelAlerts?.hide('user-communication-alert-success');
    typedWindow.UserPanelAlerts?.hide('user-communication-alert-error');

    setButtonBusy('user-communication-refresh', true, 'Refreshing...');
    const requestId = ++threadsRequestId;
    const response = await typedWindow.UserPanelApi?.request<CommunicationThreadDto[]>('/CommunicationThreads?maxItems=200', { method: 'GET' }, true);
    setButtonBusy('user-communication-refresh', false, 'Refresh');
    if (requestId !== threadsRequestId) {
        return;
    }

    if (!response || !response.success || !response.data) {
        typedWindow.UserPanelAlerts?.showError('user-communication-alert-error', response?.message ?? 'Could not load communication threads.');
        renderThreads([]);
        return;
    }

    threads = response.data;
    renderThreads(threads);

    if (selectedThreadId) {
        const selected = threads.find((item) => item.id === selectedThreadId);
        if (selected) {
            await openThread(selectedThreadId, false);
        }
    }
}

function renderThreads(items: CommunicationThreadDto[]): void {
    const tableBody = document.getElementById('user-communication-table-body');
    if (!tableBody) {
        return;
    }

    if (!items.length) {
        tableBody.innerHTML = '<tr><td colspan="4" class="text-center text-muted py-4">No communication threads found.</td></tr>';
        return;
    }

    tableBody.innerHTML = items
        .sort((a, b) => Date.parse(b.lastMessageAtUtc ?? '') - Date.parse(a.lastMessageAtUtc ?? ''))
        .map((item) => `
            <tr data-thread-id="${item.id}" class="${selectedThreadId === item.id ? 'table-primary' : ''}">
                <td>${escapeSupportCommunicationText(item.subject)}</td>
                <td>${escapeSupportCommunicationText(item.status)}</td>
                <td>${item.unreadCount}</td>
                <td>${formatSupportCommunicationDate(item.lastMessageAtUtc)}</td>
            </tr>
        `)
        .join('');
}

async function openThread(threadId: number, redraw: boolean = true): Promise<void> {
    const typedWindow = window as SupportCommunicationWindow;
    const requestId = ++detailsRequestId;
    const response = await typedWindow.UserPanelApi?.request<CommunicationThreadDetailsDto>(`/CommunicationThreads/${threadId}`, { method: 'GET' }, true);
    if (requestId !== detailsRequestId) {
        return;
    }

    if (!response || !response.success || !response.data) {
        typedWindow.UserPanelAlerts?.showError('user-communication-alert-error', response?.message ?? 'Could not load communication thread details.');
        return;
    }

    const thread = response.data;
    selectedThreadId = thread.id;

    if (redraw) {
        renderThreads(threads);
    }

    const title = document.getElementById('user-communication-title');
    if (title) {
        title.textContent = `Conversation: ${thread.subject}`;
    }

    const empty = document.getElementById('user-communication-empty');
    const panel = document.getElementById('user-communication-panel');
    empty?.classList.add('d-none');
    panel?.classList.remove('d-none');

    const threadElement = document.getElementById('user-communication-thread');
    if (threadElement) {
        threadElement.innerHTML = thread.messages
            .map((message) => `
                <div class="border rounded bg-white p-2 mb-2">
                    <div class="d-flex justify-content-between align-items-center small mb-1">
                        <span><strong>${escapeSupportCommunicationText(message.direction)}</strong> - ${escapeSupportCommunicationText(message.fromAddress)}</span>
                        <span class="text-muted">${formatSupportCommunicationDate(message.sentAtUtc ?? message.receivedAtUtc)}</span>
                    </div>
                    <div class="fw-semibold mb-1">${escapeSupportCommunicationText(message.subject)}</div>
                    <div>${escapeSupportCommunicationText(message.bodyText ?? message.bodyHtml ?? '')}</div>
                    <div class="mt-2 d-flex justify-content-between align-items-center">
                        <span class="badge bg-${message.isRead ? 'secondary' : 'warning text-dark'}">${message.isRead ? 'Read' : 'Unread'}</span>
                        ${message.direction.toLowerCase() === 'inbound' && !message.isRead
                            ? `<button class="btn btn-sm btn-outline-secondary" type="button" data-mark-read="${message.id}">Mark read</button>`
                            : ''}
                    </div>
                </div>
            `)
            .join('');
    }

    const toInput = document.getElementById('user-communication-reply-to') as HTMLInputElement | null;
    if (toInput) {
        const toAddresses = thread.participants.filter((p) => p.role.toLowerCase() === 'to').map((p) => p.emailAddress);
        toInput.value = toAddresses.join(';');
    }

    const replyInput = document.getElementById('user-communication-reply-body') as HTMLTextAreaElement | null;
    if (replyInput) {
        replyInput.value = '';
    }
}

async function sendReply(threadId: number): Promise<void> {
    const typedWindow = window as SupportCommunicationWindow;

    if (!typedWindow.UserPanelApi) {
        typedWindow.UserPanelAlerts?.showError('user-communication-alert-error', 'API client is not available.');
        return;
    }

    const to = readSupportCommunicationInputValue('user-communication-reply-to');
    const bodyText = readSupportCommunicationInputValue('user-communication-reply-body');

    if (!to || !bodyText) {
        typedWindow.UserPanelAlerts?.showError('user-communication-alert-error', 'Recipient and reply message are required.');
        return;
    }

    setButtonBusy('user-communication-send', true, 'Sending...');
    const response = await typedWindow.UserPanelApi?.request(`/CommunicationThreads/${threadId}/reply`, {
        method: 'POST',
        body: JSON.stringify({
            to,
            cc: null,
            bcc: null,
            subject: null,
            bodyText,
            bodyHtml: null,
            provider: null,
            attachmentPaths: null
        })
    }, true);
    setButtonBusy('user-communication-send', false, 'Send reply');

    if (!response || !response.success) {
        typedWindow.UserPanelAlerts?.showError('user-communication-alert-error', response?.message ?? 'Could not queue reply.');
        return;
    }

    const bodyInput = document.getElementById('user-communication-reply-body') as HTMLTextAreaElement | null;
    if (bodyInput) {
        bodyInput.value = '';
    }

    typedWindow.UserPanelAlerts?.showSuccess('user-communication-alert-success', 'Reply queued successfully.');
    await loadThreads();
}

async function markMessageRead(messageId: number): Promise<void> {
    const typedWindow = window as SupportCommunicationWindow;
    if (!typedWindow.UserPanelApi) {
        typedWindow.UserPanelAlerts?.showError('user-communication-alert-error', 'API client is not available.');
        return;
    }

    setMarkReadButtonsDisabled(true);
    const response = await typedWindow.UserPanelApi?.request(`/CommunicationThreads/messages/${messageId}/read-state`, {
        method: 'PATCH',
        body: JSON.stringify({ isRead: true })
    }, true);
    setMarkReadButtonsDisabled(false);

    if (!response || !response.success) {
        typedWindow.UserPanelAlerts?.showError('user-communication-alert-error', response?.message ?? 'Could not mark message as read.');
        return;
    }
    if (selectedThreadId) {
        await openThread(selectedThreadId);
    }
}

function setButtonBusy(id: string, busy: boolean, label: string): void {
    const button = document.getElementById(id) as HTMLButtonElement | null;
    if (!button) {
        return;
    }

    button.disabled = busy;
    button.textContent = label;
}

function setMarkReadButtonsDisabled(disabled: boolean): void {
    const buttons = document.querySelectorAll('#user-communication-thread button[data-mark-read]');
    buttons.forEach((node) => {
        const button = node as HTMLButtonElement;
        button.disabled = disabled;
    });
}

function setButtonBusy(id: string, busy: boolean, label: string): void {
    const button = document.getElementById(id) as HTMLButtonElement | null;
    if (!button) {
        return;
    }

    button.disabled = busy;
    button.textContent = label;
}

function setMarkReadButtonsDisabled(disabled: boolean): void {
    const buttons = document.querySelectorAll('#user-communication-thread button[data-mark-read]');
    buttons.forEach((node) => {
        const button = node as HTMLButtonElement;
        button.disabled = disabled;
    });
}

    if (selectedThreadId) {
        await openThread(selectedThreadId);
    }
}

function readSupportCommunicationInputValue(id: string): string {
    const input = document.getElementById(id) as HTMLInputElement | HTMLTextAreaElement | null;
    return input?.value.trim() ?? '';
}

function formatSupportCommunicationDate(value: string | null): string {
    if (!value) {
        return '-';
    }

    const date = new Date(value);
    if (Number.isNaN(date.getTime())) {
        return '-';
    }

    return date.toLocaleString();
}

function escapeSupportCommunicationText(value: string): string {
    const map: Record<string, string> = {
        '&': '&amp;',
        '<': '&lt;',
        '>': '&gt;',
        '"': '&quot;',
        "'": '&#039;'
    };

    return (value ?? '').toString().replace(/[&<>"']/g, (char) => map[char]);
}

if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initializeSupportCommunicationPage);
} else {
    initializeSupportCommunicationPage();
}

window.addEventListener('popstate', initializeSupportCommunicationPage);

const typedWindow = window as SupportCommunicationWindow;
function registerSupportCommunicationEnhancedLoadListener(): void {
    if (typedWindow.Blazor?.addEventListener) {
        typedWindow.Blazor.addEventListener('enhancedload', initializeSupportCommunicationPage);
        return;
    }

    window.setTimeout(registerSupportCommunicationEnhancedLoadListener, 100);
}

registerSupportCommunicationEnhancedLoadListener();
})();

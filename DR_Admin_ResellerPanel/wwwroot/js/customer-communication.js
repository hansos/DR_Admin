"use strict";
// @ts-nocheck
(function () {
    let threads = [];
    let selectedThreadId = null;
    let threadsRequestId = 0;
    let detailsRequestId = 0;
    function getApiBaseUrl() {
        return window.AppSettings?.apiBaseUrl ?? '';
    }
    function getAuthToken() {
        const auth = window.Auth;
        if (auth?.getToken) {
            return auth.getToken();
        }
        return sessionStorage.getItem('rp_authToken');
    }
    async function apiRequest(endpoint, options = {}) {
        try {
            const headers = {
                'Content-Type': 'application/json',
                ...options.headers,
            };
            const authToken = getAuthToken();
            if (authToken) {
                headers.Authorization = `Bearer ${authToken}`;
            }
            const response = await fetch(endpoint, {
                ...options,
                headers,
                credentials: 'include',
            });
            const contentType = response.headers.get('content-type') ?? '';
            const hasJson = contentType.includes('application/json');
            const payload = hasJson ? await response.json() : null;
            if (!response.ok) {
                return {
                    success: false,
                    message: payload?.message ?? payload?.title ?? `Request failed (${response.status})`
                };
            }
            return {
                success: true,
                data: payload?.data ?? payload
            };
        }
        catch (error) {
            console.error('Customer communication request failed', error);
            return { success: false, message: 'Network error' };
        }
    }
    function initializePage() {
        const page = document.getElementById('customer-communication-page');
        if (!page || page.dataset.initialized === 'true') {
            return;
        }
        page.dataset.initialized = 'true';
        document.getElementById('customer-communication-refresh')?.addEventListener('click', () => {
            void loadThreads();
        });
        document.getElementById('customer-communication-filter-customer-id')?.addEventListener('input', () => {
            void loadThreads();
        });
        document.getElementById('customer-communication-filter-entity-type')?.addEventListener('input', () => {
            void loadThreads();
        });
        document.getElementById('customer-communication-filter-status')?.addEventListener('change', () => {
            void loadThreads();
        });
        document.getElementById('customer-communication-filter-search')?.addEventListener('input', () => {
            void loadThreads();
        });
        document.getElementById('customer-communication-table-body')?.addEventListener('click', (event) => {
            const target = event.target;
            const row = target.closest('tr[data-thread-id]');
            if (!row) {
                return;
            }
            const id = Number(row.dataset.threadId ?? '0');
            if (!id) {
                return;
            }
            void openThread(id);
        });
        document.getElementById('customer-communication-close-thread')?.addEventListener('click', () => {
            if (!selectedThreadId) {
                return;
            }
            void updateThreadStatus(selectedThreadId, 'Closed');
        });
        document.getElementById('customer-communication-reply-form')?.addEventListener('submit', (event) => {
            event.preventDefault();
            if (!selectedThreadId) {
                return;
            }
            void queueReply(selectedThreadId);
        });
        document.getElementById('customer-communication-messages')?.addEventListener('click', (event) => {
            const target = event.target;
            const button = target.closest('button[data-mark-read]');
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
    async function loadThreads() {
        const tableBody = document.getElementById('customer-communication-table-body');
        if (!tableBody) {
            return;
        }
        const apiBaseUrl = getApiBaseUrl();
        if (!apiBaseUrl) {
            showError('API base URL is missing. Check application settings.');
            tableBody.innerHTML = '<tr><td colspan="6" class="text-center text-danger py-4">Missing API configuration.</td></tr>';
            return;
        }
        const requestId = ++threadsRequestId;
        tableBody.innerHTML = '<tr><td colspan="6" class="text-center py-4"><div class="spinner-border text-primary"></div></td></tr>';
        setButtonBusy('customer-communication-refresh', true, '<i class="bi bi-arrow-clockwise"></i> Refreshing...');
        const customerId = document.getElementById('customer-communication-filter-customer-id')?.value.trim();
        const relatedEntityType = document.getElementById('customer-communication-filter-entity-type')?.value.trim();
        const status = document.getElementById('customer-communication-filter-status')?.value.trim();
        const search = document.getElementById('customer-communication-filter-search')?.value.trim();
        const params = new URLSearchParams();
        params.set('maxItems', '200');
        if (customerId)
            params.set('customerId', customerId);
        if (relatedEntityType)
            params.set('relatedEntityType', relatedEntityType);
        if (status)
            params.set('status', status);
        if (search)
            params.set('search', search);
        const response = await apiRequest(`${apiBaseUrl}/CommunicationThreads?${params.toString()}`, { method: 'GET' });
        setButtonBusy('customer-communication-refresh', false, '<i class="bi bi-arrow-clockwise"></i> Refresh');
        if (requestId !== threadsRequestId) {
            return;
        }
        if (!response.success || !Array.isArray(response.data)) {
            showError(response.message ?? 'Could not load communication threads.');
            tableBody.innerHTML = '<tr><td colspan="6" class="text-center text-danger py-4">Failed to load communication threads.</td></tr>';
            return;
        }
        hideAlerts();
        threads = response.data;
        renderThreads();
        if (selectedThreadId) {
            const selected = threads.find((item) => item.id === selectedThreadId);
            if (selected) {
                void openThread(selectedThreadId, false);
            }
        }
    }
    function renderThreads() {
        const tableBody = document.getElementById('customer-communication-table-body');
        if (!tableBody) {
            return;
        }
        if (!threads.length) {
            tableBody.innerHTML = '<tr><td colspan="6" class="text-center text-muted py-4">No communication threads found.</td></tr>';
            return;
        }
        tableBody.innerHTML = threads
            .sort((a, b) => Date.parse(b.lastMessageAtUtc ?? '') - Date.parse(a.lastMessageAtUtc ?? ''))
            .map((thread) => `
            <tr data-thread-id="${thread.id}" class="${selectedThreadId === thread.id ? 'table-primary' : ''}">
                <td>${thread.id}</td>
                <td>${esc(thread.subject)}</td>
                <td>${thread.customerId ?? '-'}</td>
                <td><span class="badge bg-${thread.status.toLowerCase() === 'open' ? 'success' : 'secondary'}">${esc(thread.status)}</span></td>
                <td>${thread.unreadCount}</td>
                <td>${formatDate(thread.lastMessageAtUtc)}</td>
            </tr>
        `)
            .join('');
    }
    async function openThread(threadId, redraw = true) {
        const apiBaseUrl = getApiBaseUrl();
        if (!apiBaseUrl) {
            showError('API base URL is missing. Check application settings.');
            return;
        }
        const requestId = ++detailsRequestId;
        const response = await apiRequest(`${apiBaseUrl}/CommunicationThreads/${threadId}`, { method: 'GET' });
        if (requestId !== detailsRequestId) {
            return;
        }
        if (!response.success || !response.data) {
            showError(response.message ?? 'Could not load thread details.');
            return;
        }
        selectedThreadId = threadId;
        const thread = response.data;
        if (redraw) {
            renderThreads();
        }
        const empty = document.getElementById('customer-communication-details-empty');
        const panel = document.getElementById('customer-communication-details-panel');
        const title = document.getElementById('customer-communication-details-title');
        const participants = document.getElementById('customer-communication-participants');
        const messages = document.getElementById('customer-communication-messages');
        const closeButton = document.getElementById('customer-communication-close-thread');
        empty?.classList.add('d-none');
        panel?.classList.remove('d-none');
        if (title) {
            title.textContent = `Thread #${thread.id}: ${thread.subject}`;
        }
        if (participants) {
            participants.innerHTML = thread.participants
                .map((participant) => `<span class="me-3"><strong>${esc(participant.role)}:</strong> ${esc(participant.emailAddress)}</span>`)
                .join('');
        }
        if (messages) {
            messages.innerHTML = thread.messages
                .map((message) => `
                <div class="border rounded bg-white p-2 mb-2">
                    <div class="d-flex justify-content-between align-items-center small mb-1">
                        <span><strong>${esc(message.direction)}</strong> - ${esc(message.fromAddress)}</span>
                        <span class="text-muted">${formatDate(message.sentAtUtc ?? message.receivedAtUtc)}</span>
                    </div>
                    <div class="fw-semibold mb-1">${esc(message.subject)}</div>
                    <div>${esc(message.bodyText ?? message.bodyHtml ?? '')}</div>
                    <div class="mt-2 d-flex justify-content-between align-items-center">
                        <span class="badge bg-${message.isRead ? 'secondary' : 'warning text-dark'}">${message.isRead ? 'Read' : 'Unread'}</span>
                        ${message.direction.toLowerCase() === 'inbound' && !message.isRead
                ? `<button class="btn btn-sm btn-outline-secondary" type="button" data-mark-read="${message.id}"><i class="bi bi-envelope-open"></i> Mark read</button>`
                : ''}
                    </div>
                </div>
            `)
                .join('');
        }
        const toInput = document.getElementById('customer-communication-reply-to');
        if (toInput) {
            const toAddresses = thread.participants.filter((p) => p.role.toLowerCase() === 'to').map((p) => p.emailAddress);
            toInput.value = toAddresses.join(';');
        }
        const subjectInput = document.getElementById('customer-communication-reply-subject');
        if (subjectInput) {
            subjectInput.value = thread.subject;
        }
        const bodyInput = document.getElementById('customer-communication-reply-body');
        if (bodyInput) {
            bodyInput.value = '';
        }
        if (closeButton) {
            closeButton.disabled = thread.status.toLowerCase() !== 'open';
        }
    }
    async function queueReply(threadId) {
        const to = document.getElementById('customer-communication-reply-to')?.value.trim() ?? '';
        const cc = document.getElementById('customer-communication-reply-cc')?.value.trim() ?? '';
        const bcc = document.getElementById('customer-communication-reply-bcc')?.value.trim() ?? '';
        const subject = document.getElementById('customer-communication-reply-subject')?.value.trim() ?? '';
        const bodyText = document.getElementById('customer-communication-reply-body')?.value.trim() ?? '';
        if (!to || !bodyText) {
            showError('To and Message are required for reply.');
            return;
        }
        const payload = {
            to,
            cc: cc || null,
            bcc: bcc || null,
            subject: subject || null,
            bodyText,
            bodyHtml: null,
            provider: null,
            attachmentPaths: null
        };
        setButtonBusy('customer-communication-reply-send', true, '<i class="bi bi-hourglass-split"></i> Queueing...');
        const response = await apiRequest(`${getApiBaseUrl()}/CommunicationThreads/${threadId}/reply`, {
            method: 'POST',
            body: JSON.stringify(payload)
        });
        setButtonBusy('customer-communication-reply-send', false, '<i class="bi bi-send"></i> Queue Reply');
        if (!response.success) {
            showError(response.message ?? 'Could not queue reply.');
            return;
        }
        showSuccess('Reply queued successfully.');
        const bodyInput = document.getElementById('customer-communication-reply-body');
        if (bodyInput) {
            bodyInput.value = '';
        }
        await loadThreads();
    }
    async function markMessageRead(messageId) {
        const response = await apiRequest(`${getApiBaseUrl()}/CommunicationThreads/messages/${messageId}/read-state`, {
            method: 'PATCH',
            body: JSON.stringify({ isRead: true })
        });
        if (!response.success) {
            showError(response.message ?? 'Could not mark message as read.');
            return;
        }
        if (selectedThreadId) {
            await openThread(selectedThreadId);
        }
    }
    async function updateThreadStatus(threadId, status) {
        setButtonBusy('customer-communication-close-thread', true, '<i class="bi bi-hourglass-split"></i> Updating...');
        const response = await apiRequest(`${getApiBaseUrl()}/CommunicationThreads/${threadId}/status`, {
            method: 'PATCH',
            body: JSON.stringify({ status })
        });
        setButtonBusy('customer-communication-close-thread', false, '<i class="bi bi-lock"></i> Close Thread');
        if (!response.success) {
            showError(response.message ?? 'Could not update thread status.');
            return;
        }
        showSuccess(`Thread updated to ${status}.`);
        await loadThreads();
    }
    function formatDate(value) {
        if (!value) {
            return '-';
        }
        const date = new Date(value);
        if (Number.isNaN(date.getTime())) {
            return '-';
        }
        return date.toLocaleString();
    }
    function esc(value) {
        const map = {
            '&': '&amp;',
            '<': '&lt;',
            '>': '&gt;',
            '"': '&quot;',
            "'": '&#039;'
        };
        return (value ?? '').toString().replace(/[&<>"']/g, (char) => map[char]);
    }
    function showSuccess(message) {
        const element = document.getElementById('customer-communication-alert-success');
        if (!element) {
            return;
        }
        element.textContent = message;
        element.classList.remove('d-none');
        document.getElementById('customer-communication-alert-error')?.classList.add('d-none');
    }
    function showError(message) {
        const element = document.getElementById('customer-communication-alert-error');
        if (!element) {
            return;
        }
        element.textContent = message;
        element.classList.remove('d-none');
        document.getElementById('customer-communication-alert-success')?.classList.add('d-none');
    }
    function hideAlerts() {
        document.getElementById('customer-communication-alert-success')?.classList.add('d-none');
        document.getElementById('customer-communication-alert-error')?.classList.add('d-none');
    }
    function setButtonBusy(id, busy, labelHtml) {
        const button = document.getElementById(id);
        if (!button) {
            return;
        }
        button.disabled = busy;
        button.innerHTML = labelHtml;
    }
    function setMarkReadButtonsDisabled(disabled) {
        const buttons = document.querySelectorAll('#customer-communication-messages button[data-mark-read]');
        buttons.forEach((node) => {
            const button = node;
            button.disabled = disabled;
        });
    }
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initializePage);
    }
    else {
        initializePage();
    }
    window.addEventListener('popstate', initializePage);
    function registerEnhancedLoadListener() {
        if (window.Blazor?.addEventListener) {
            window.Blazor.addEventListener('enhancedload', initializePage);
            return;
        }
        window.setTimeout(registerEnhancedLoadListener, 100);
    }
    registerEnhancedLoadListener();
})();
//# sourceMappingURL=customer-communication.js.map
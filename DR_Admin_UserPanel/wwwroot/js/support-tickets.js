"use strict";
(() => {
    let supportTickets = [];
    let selectedTicketId = null;
    let autoRefreshHandle = null;
    let isAutoRefreshing = false;
    function initializeSupportTicketsPage() {
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
        document.getElementById('support-tickets-table-body')?.addEventListener('click', (event) => {
            const target = event.target;
            const row = target.closest('tr');
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
        document.getElementById('support-ticket-reply-form')?.addEventListener('submit', (event) => {
            event.preventDefault();
            void sendSupportTicketMessage();
        });
        clearConversation();
        void loadSupportTickets();
    }
    async function loadSupportTickets() {
        const typedWindow = window;
        typedWindow.UserPanelAlerts?.hide('support-tickets-alert-success');
        typedWindow.UserPanelAlerts?.hide('support-tickets-alert-error');
        const response = await typedWindow.UserPanelApi?.request('/SupportTickets', { method: 'GET' }, true);
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
            }
            else {
                selectedTicketId = null;
                clearConversation();
            }
        }
    }
    function renderSupportTickets(items) {
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
    function renderSupportTicketStatus(status) {
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
    async function createSupportTicket() {
        const typedWindow = window;
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
    async function openSupportTicket(ticketId) {
        const typedWindow = window;
        const response = await typedWindow.UserPanelApi?.request(`/SupportTickets/${ticketId}`, { method: 'GET' }, true);
        if (!response || !response.success || !response.data) {
            typedWindow.UserPanelAlerts?.showError('support-tickets-alert-error', response?.message ?? 'Could not load support ticket details.');
            return;
        }
        const ticket = response.data;
        selectedTicketId = ticket.id;
        const existingIndex = supportTickets.findIndex((item) => item.id === ticket.id);
        if (existingIndex >= 0) {
            supportTickets[existingIndex] = ticket;
        }
        else {
            supportTickets.push(ticket);
        }
        renderSupportTickets(supportTickets);
        renderConversation(ticket);
    }
    function renderConversation(ticket) {
        const title = document.getElementById('support-ticket-details-title');
        const thread = document.getElementById('support-ticket-thread');
        const reply = document.getElementById('support-ticket-reply');
        const send = document.getElementById('support-ticket-send');
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
    function clearConversation() {
        const title = document.getElementById('support-ticket-details-title');
        const thread = document.getElementById('support-ticket-thread');
        const reply = document.getElementById('support-ticket-reply');
        const send = document.getElementById('support-ticket-send');
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
    async function sendSupportTicketMessage() {
        const typedWindow = window;
        if (selectedTicketId === null) {
            typedWindow.UserPanelAlerts?.showError('support-tickets-alert-error', 'Select a ticket before sending a message.');
            return;
        }
        const reply = document.getElementById('support-ticket-reply');
        const message = reply?.value.trim() ?? '';
        if (!message) {
            typedWindow.UserPanelAlerts?.showError('support-tickets-alert-error', 'Reply message is required.');
            return;
        }
        const response = await typedWindow.UserPanelApi?.request(`/SupportTickets/${selectedTicketId}/messages`, {
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
    function clearSupportTicketsForm() {
        const subject = document.getElementById('support-tickets-subject');
        const message = document.getElementById('support-tickets-message');
        const priority = document.getElementById('support-tickets-priority');
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
    function readSupportTicketsInputValue(id) {
        const input = document.getElementById(id);
        return input?.value.trim() ?? '';
    }
    function formatSupportTicketsDate(value) {
        const date = new Date(value);
        if (Number.isNaN(date.getTime())) {
            return '-';
        }
        return `${date.toLocaleDateString()} ${date.toLocaleTimeString()}`;
    }
    function escapeSupportTicketsText(value) {
        return value
            .replace(/&/g, '&amp;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;')
            .replace(/'/g, '&#039;');
    }
    function setupSupportTicketsObserver() {
        initializeSupportTicketsPage();
        const observer = new MutationObserver(() => {
            initializeSupportTicketsPage();
        });
        observer.observe(document.body, { childList: true, subtree: true });
    }
    function startAutoRefresh() {
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
    }
    else {
        setupSupportTicketsObserver();
        startAutoRefresh();
    }
})();
//# sourceMappingURL=support-tickets.js.map
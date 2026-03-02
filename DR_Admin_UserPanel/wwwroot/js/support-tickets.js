"use strict";
(() => {
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
        renderSupportTickets(response.data);
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
            .map((item) => `<tr>
            <td>${item.id}</td>
            <td>${escapeSupportTicketsText(item.subject)}</td>
            <td>${escapeSupportTicketsText(item.status)}</td>
            <td>${escapeSupportTicketsText(item.priority)}</td>
            <td>${formatSupportTicketsDate(item.updatedAt ?? item.createdAt ?? '')}</td>
        </tr>`)
            .join('');
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
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', setupSupportTicketsObserver);
    }
    else {
        setupSupportTicketsObserver();
    }
})();
//# sourceMappingURL=support-tickets.js.map
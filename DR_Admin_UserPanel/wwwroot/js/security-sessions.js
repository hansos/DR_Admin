"use strict";
(() => {
    function initializeSecuritySessionsPage() {
        const page = document.getElementById('security-sessions-page');
        if (!page || page.dataset.initialized === 'true') {
            return;
        }
        page.dataset.initialized = 'true';
        document.getElementById('security-sessions-refresh')?.addEventListener('click', () => {
            void loadSecuritySessions();
        });
        void loadSecuritySessions();
    }
    async function loadSecuritySessions() {
        const typedWindow = window;
        typedWindow.UserPanelAlerts?.hide('security-sessions-alert-error');
        const response = await typedWindow.UserPanelApi?.request('/LoginHistories?pageNumber=1&pageSize=50', { method: 'GET' }, true);
        if (!response || !response.success || !response.data) {
            typedWindow.UserPanelAlerts?.showError('security-sessions-alert-error', response?.message ?? 'Could not load session history.');
            renderSecuritySessions([]);
            return;
        }
        renderSecuritySessions(normalizeSecuritySessions(response.data));
    }
    function normalizeSecuritySessions(payload) {
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
    function renderSecuritySessions(items) {
        const tableBody = document.getElementById('security-sessions-table-body');
        if (!tableBody) {
            return;
        }
        if (items.length === 0) {
            tableBody.innerHTML = '<tr><td colspan="4" class="text-center text-muted">No sessions found.</td></tr>';
            return;
        }
        tableBody.innerHTML = items
            .sort((a, b) => Date.parse(b.attemptedAt) - Date.parse(a.attemptedAt))
            .map((item) => {
            const resultBadge = item.isSuccessful
                ? '<span class="badge bg-success">Success</span>'
                : '<span class="badge bg-danger">Failed</span>';
            const device = item.failureReason?.trim()
                ? `${escapeSecuritySessionsText(item.userAgent)}<div class="small text-danger">${escapeSecuritySessionsText(item.failureReason)}</div>`
                : escapeSecuritySessionsText(item.userAgent);
            return `<tr>
                <td>${formatSecuritySessionsDate(item.attemptedAt)}</td>
                <td>${resultBadge}</td>
                <td>${escapeSecuritySessionsText(item.ipAddress)}</td>
                <td>${device}</td>
            </tr>`;
        })
            .join('');
    }
    function formatSecuritySessionsDate(value) {
        const date = new Date(value);
        if (Number.isNaN(date.getTime())) {
            return '-';
        }
        return `${date.toLocaleDateString()} ${date.toLocaleTimeString()}`;
    }
    function escapeSecuritySessionsText(value) {
        return value
            .replace(/&/g, '&amp;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;')
            .replace(/'/g, '&#039;');
    }
    function setupSecuritySessionsObserver() {
        initializeSecuritySessionsPage();
        const observer = new MutationObserver(() => {
            initializeSecuritySessionsPage();
        });
        observer.observe(document.body, { childList: true, subtree: true });
    }
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', setupSecuritySessionsObserver);
    }
    else {
        setupSecuritySessionsObserver();
    }
})();
//# sourceMappingURL=security-sessions.js.map
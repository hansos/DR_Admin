"use strict";
(() => {
    function initializePrivacyPage() {
        const page = document.getElementById('privacy-page');
        if (!page || page.dataset.initialized === 'true') {
            return;
        }
        page.dataset.initialized = 'true';
        document.getElementById('privacy-export-request')?.addEventListener('click', () => {
            void requestPrivacyExport();
        });
        document.getElementById('privacy-delete-request')?.addEventListener('click', () => {
            void requestPrivacyDeletion();
        });
        void loadPrivacySummary();
    }
    async function loadPrivacySummary() {
        const typedWindow = window;
        typedWindow.UserPanelAlerts?.hide('privacy-alert-success');
        typedWindow.UserPanelAlerts?.hide('privacy-alert-error');
        const response = await typedWindow.UserPanelApi?.request('/MyAccount/me', { method: 'GET' }, true);
        if (!response || !response.success || !response.data) {
            typedWindow.UserPanelAlerts?.showError('privacy-alert-error', response?.message ?? 'Could not load privacy summary.');
            return;
        }
        renderPrivacySummary(response.data);
    }
    function renderPrivacySummary(account) {
        const container = document.getElementById('privacy-account-summary');
        if (!container) {
            return;
        }
        const customerText = account.customer
            ? `<div>Customer: ${escapePrivacyText(account.customer.name)} (#${account.customer.id})</div>
           <div>Email: ${escapePrivacyText(account.customer.email)}</div>
           <div>Phone: ${escapePrivacyText(account.customer.phone)}</div>`
            : '<div class="text-muted">No customer profile is linked to this user.</div>';
        const verified = account.emailConfirmed
            ? '<span class="badge bg-success">Verified</span>'
            : '<span class="badge bg-warning text-dark">Not verified</span>';
        container.innerHTML = `
        <div><strong>${escapePrivacyText(account.username)}</strong> ${verified}</div>
        <div>Email: ${escapePrivacyText(account.email)}</div>
        <hr />
        ${customerText}
    `;
    }
    async function requestPrivacyExport() {
        const typedWindow = window;
        typedWindow.UserPanelAlerts?.hide('privacy-alert-success');
        typedWindow.UserPanelAlerts?.hide('privacy-alert-error');
        const response = await typedWindow.UserPanelApi?.request('/MyAccount/privacy/export', { method: 'POST' }, true);
        if (!response || !response.success) {
            typedWindow.UserPanelAlerts?.showError('privacy-alert-error', response?.message ?? 'Could not submit export request.');
            return;
        }
        typedWindow.UserPanelAlerts?.showSuccess('privacy-alert-success', response.message ?? 'Export request submitted.');
    }
    async function requestPrivacyDeletion() {
        const typedWindow = window;
        typedWindow.UserPanelAlerts?.hide('privacy-alert-success');
        typedWindow.UserPanelAlerts?.hide('privacy-alert-error');
        const response = await typedWindow.UserPanelApi?.request('/MyAccount/privacy/delete-request', { method: 'POST' }, true);
        if (!response || !response.success) {
            typedWindow.UserPanelAlerts?.showError('privacy-alert-error', response?.message ?? 'Could not submit deletion request.');
            return;
        }
        typedWindow.UserPanelAlerts?.showSuccess('privacy-alert-success', response.message ?? 'Deletion request submitted.');
    }
    function escapePrivacyText(value) {
        return value
            .replace(/&/g, '&amp;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;')
            .replace(/'/g, '&#039;');
    }
    function setupPrivacyObserver() {
        initializePrivacyPage();
        const observer = new MutationObserver(() => {
            initializePrivacyPage();
        });
        observer.observe(document.body, { childList: true, subtree: true });
    }
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', setupPrivacyObserver);
    }
    else {
        setupPrivacyObserver();
    }
})();
//# sourceMappingURL=privacy.js.map
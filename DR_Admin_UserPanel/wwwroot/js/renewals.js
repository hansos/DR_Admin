"use strict";
function initializeRenewalsPage() {
    const page = document.getElementById('renewals-page');
    if (!page || page.dataset.bound === 'true') {
        return;
    }
    page.dataset.bound = 'true';
    void loadRenewalsSummary();
}
async function loadRenewalsSummary() {
    const typedWindow = window;
    const response = await typedWindow.UserPanelApi?.request('/Subscriptions', { method: 'GET' }, true);
    if (!response || !response.success) {
        typedWindow.UserPanelAlerts?.showError('renewals-alert-error', response?.message ?? 'Could not load renewals.');
    }
}
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initializeRenewalsPage);
}
else {
    initializeRenewalsPage();
}
//# sourceMappingURL=renewals.js.map
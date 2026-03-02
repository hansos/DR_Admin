"use strict";
function initializeNotificationsPage() {
    const page = document.getElementById('notifications-page');
    if (!page || page.dataset.bound === 'true') {
        return;
    }
    page.dataset.bound = 'true';
    void loadNotificationsSummary();
}
async function loadNotificationsSummary() {
    const typedWindow = window;
    const response = await typedWindow.UserPanelApi?.request('/Orders', { method: 'GET' }, true);
    if (!response || !response.success) {
        typedWindow.UserPanelAlerts?.showError('notifications-alert-error', response?.message ?? 'Could not load notifications.');
    }
}
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initializeNotificationsPage);
}
else {
    initializeNotificationsPage();
}
//# sourceMappingURL=notifications.js.map
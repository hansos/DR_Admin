"use strict";
function initializeDashboard() {
    const page = document.getElementById('dashboard-summary-page');
    if (!page || page.dataset.bound === 'true') {
        return;
    }
    page.dataset.bound = 'true';
    void loadDashboard();
}
async function loadDashboard() {
    const typedWindow = window;
    const response = await typedWindow.UserPanelApi?.request('/Orders', { method: 'GET' }, true);
    if (!response || !response.success || !response.data) {
        typedWindow.UserPanelAlerts?.showError('dashboard-alert-error', response?.message ?? 'Could not load dashboard data.');
        return;
    }
    const orders = response.data;
    const activeDomains = orders.filter((order) => order.serviceId === null).length;
    const activeHosting = orders.filter((order) => order.serviceId !== null).length;
    setText('dashboard-active-domains', activeDomains.toString());
    setText('dashboard-active-hosting', activeHosting.toString());
    setText('dashboard-active-services', orders.length.toString());
    const renewals = document.getElementById('dashboard-renewals');
    if (!renewals) {
        return;
    }
    const items = orders
        .filter((order) => !!order.nextBillingDate)
        .sort((a, b) => Date.parse(a.nextBillingDate) - Date.parse(b.nextBillingDate))
        .slice(0, 5);
    if (items.length === 0) {
        renewals.textContent = 'No upcoming renewals.';
        return;
    }
    renewals.innerHTML = items
        .map((order) => `<div class="d-flex justify-content-between border-bottom py-2"><span>${order.orderNumber}</span><span>${formatDate(order.nextBillingDate)}</span></div>`)
        .join('');
}
function formatDate(value) {
    const date = new Date(value);
    if (Number.isNaN(date.getTime())) {
        return '-';
    }
    return date.toLocaleDateString();
}
function setText(id, value) {
    const element = document.getElementById(id);
    if (element) {
        element.textContent = value;
    }
}
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initializeDashboard);
}
else {
    initializeDashboard();
}
//# sourceMappingURL=dashboard-summary.js.map
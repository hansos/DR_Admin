"use strict";
function showAlert(id, message, variant) {
    const element = document.getElementById(id);
    if (!element) {
        return;
    }
    element.textContent = message;
    element.classList.remove('d-none', 'alert-success', 'alert-danger');
    element.classList.add(variant === 'success' ? 'alert-success' : 'alert-danger');
}
function hide(id) {
    const element = document.getElementById(id);
    if (!element) {
        return;
    }
    element.classList.add('d-none');
    element.textContent = '';
}
const alertsWindow = window;
alertsWindow.UserPanelAlerts = {
    showSuccess: (id, message) => showAlert(id, message, 'success'),
    showError: (id, message) => showAlert(id, message, 'error'),
    hide
};
//# sourceMappingURL=alerts.js.map
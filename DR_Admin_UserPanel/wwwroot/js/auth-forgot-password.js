"use strict";
function initializeForgotPassword() {
    const form = document.getElementById('auth-forgot-password-form');
    if (!form || form.dataset.bound === 'true') {
        return;
    }
    form.dataset.bound = 'true';
    form.addEventListener('submit', async (event) => {
        event.preventDefault();
        const typedWindow = window;
        typedWindow.UserPanelAlerts?.hide('auth-forgot-password-alert-success');
        typedWindow.UserPanelAlerts?.hide('auth-forgot-password-alert-error');
        const email = readForgotValue('auth-forgot-password-email');
        if (!email) {
            typedWindow.UserPanelAlerts?.showError('auth-forgot-password-alert-error', 'Email is required.');
            return;
        }
        const response = await typedWindow.UserPanelApi?.request('/MyAccount/request-password-reset', {
            method: 'POST',
            body: JSON.stringify({ email })
        }, false);
        if (!response || !response.success) {
            typedWindow.UserPanelAlerts?.showError('auth-forgot-password-alert-error', response?.message ?? 'Could not request reset link.');
            return;
        }
        typedWindow.UserPanelAlerts?.showSuccess('auth-forgot-password-alert-success', response.message ?? 'If the account exists, a reset link has been sent.');
        form.reset();
    });
}
function readForgotValue(id) {
    const input = document.getElementById(id);
    return input?.value.trim() ?? '';
}
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initializeForgotPassword);
}
else {
    initializeForgotPassword();
}
//# sourceMappingURL=auth-forgot-password.js.map
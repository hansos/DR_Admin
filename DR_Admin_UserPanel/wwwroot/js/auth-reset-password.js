"use strict";
function initializeResetPassword() {
    const form = document.getElementById('auth-reset-password-form');
    if (!form || form.dataset.bound === 'true') {
        return;
    }
    form.dataset.bound = 'true';
    bindResetPasswordToggle('auth-reset-password-new', 'auth-reset-password-toggle-new');
    bindResetPasswordToggle('auth-reset-password-confirm', 'auth-reset-password-toggle-confirm');
    form.addEventListener('submit', async (event) => {
        event.preventDefault();
        const typedWindow = window;
        typedWindow.UserPanelAlerts?.hide('auth-reset-password-alert-success');
        typedWindow.UserPanelAlerts?.hide('auth-reset-password-alert-error');
        const token = new URLSearchParams(window.location.search).get('token') ?? '';
        const newPassword = readResetValue('auth-reset-password-new');
        const confirmPassword = readResetValue('auth-reset-password-confirm');
        if (!token) {
            typedWindow.UserPanelAlerts?.showError('auth-reset-password-alert-error', 'Missing reset token.');
            return;
        }
        if (!newPassword || !confirmPassword) {
            typedWindow.UserPanelAlerts?.showError('auth-reset-password-alert-error', 'Both password fields are required.');
            return;
        }
        if (newPassword !== confirmPassword) {
            typedWindow.UserPanelAlerts?.showError('auth-reset-password-alert-error', 'Passwords do not match.');
            return;
        }
        const payload = { token, newPassword, confirmPassword };
        const response = await typedWindow.UserPanelApi?.request('/MyAccount/reset-password', {
            method: 'POST',
            body: JSON.stringify(payload)
        }, false);
        if (!response || !response.success) {
            typedWindow.UserPanelAlerts?.showError('auth-reset-password-alert-error', response?.message ?? 'Password reset failed.');
            return;
        }
        typedWindow.UserPanelAlerts?.showSuccess('auth-reset-password-alert-success', response.message ?? 'Password reset successful. Redirecting to login.');
        setTimeout(() => {
            window.location.href = '/account/login';
        }, 1200);
    });
}
function bindResetPasswordToggle(inputId, toggleId) {
    const input = document.getElementById(inputId);
    const toggle = document.getElementById(toggleId);
    const icon = toggle?.querySelector('i');
    if (!input || !toggle || !icon || toggle.dataset.bound === 'true') {
        return;
    }
    toggle.dataset.bound = 'true';
    toggle.addEventListener('click', () => {
        const isPassword = input.type === 'password';
        input.type = isPassword ? 'text' : 'password';
        icon.className = isPassword ? 'bi bi-eye-slash' : 'bi bi-eye';
        toggle.setAttribute('aria-label', isPassword ? 'Hide password' : 'Show password');
    });
}
function readResetValue(id) {
    const input = document.getElementById(id);
    return input?.value.trim() ?? '';
}
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initializeResetPassword);
}
else {
    initializeResetPassword();
}
function registerResetEnhancedLoadListener() {
    const typedWindow = window;
    if (typedWindow.Blazor?.addEventListener) {
        typedWindow.Blazor.addEventListener('enhancedload', initializeResetPassword);
        return;
    }
    window.setTimeout(registerResetEnhancedLoadListener, 100);
}
registerResetEnhancedLoadListener();
//# sourceMappingURL=auth-reset-password.js.map
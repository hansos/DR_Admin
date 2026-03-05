"use strict";
function initializeForgotPassword() {
    const form = document.getElementById('auth-forgot-password-form');
    if (!form || form.dataset.bound === 'true') {
        return;
    }
    form.dataset.bound = 'true';
    const retryButton = document.getElementById('auth-forgot-password-retry');
    if (retryButton && retryButton.dataset.bound !== 'true') {
        retryButton.dataset.bound = 'true';
        retryButton.addEventListener('click', () => {
            void retryForgotPassword();
        });
    }
    form.addEventListener('submit', async (event) => {
        event.preventDefault();
        await submitForgotPassword();
    });
}
async function submitForgotPassword() {
    const typedWindow = window;
    typedWindow.UserPanelAlerts?.hide('auth-forgot-password-alert-success');
    typedWindow.UserPanelAlerts?.hide('auth-forgot-password-alert-error');
    const email = readForgotValue('auth-forgot-password-email');
    if (!email) {
        typedWindow.UserPanelAlerts?.showError('auth-forgot-password-alert-error', 'Email is required.');
        return;
    }
    const response = await sendForgotPasswordRequest(email);
    if (!response || !response.success) {
        typedWindow.UserPanelAlerts?.showError('auth-forgot-password-alert-error', response?.message ?? 'Could not request reset link.');
        return;
    }
    typedWindow.UserPanelAlerts?.showSuccess('auth-forgot-password-alert-success', response.message ?? 'If the account exists, a reset link has been sent.');
    setForgotPasswordPostSubmitState(true);
}
async function retryForgotPassword() {
    const typedWindow = window;
    typedWindow.UserPanelAlerts?.hide('auth-forgot-password-alert-success');
    typedWindow.UserPanelAlerts?.hide('auth-forgot-password-alert-error');
    setForgotPasswordPostSubmitState(false);
    const emailInput = document.getElementById('auth-forgot-password-email');
    emailInput?.focus();
}
async function sendForgotPasswordRequest(email) {
    const typedWindow = window;
    const siteCode = typedWindow.UserPanelSettings?.frontendSiteCode ?? 'shop';
    const payload = { email, siteCode };
    return typedWindow.UserPanelApi?.request('/MyAccount/request-password-reset', {
        method: 'POST',
        body: JSON.stringify(payload)
    }, false);
}
function setForgotPasswordPostSubmitState(enabled) {
    const emailInput = document.getElementById('auth-forgot-password-email');
    if (emailInput) {
        emailInput.disabled = enabled;
    }
    const submitContainer = document.getElementById('auth-forgot-password-submit-container');
    const postActions = document.getElementById('auth-forgot-password-post-actions');
    if (submitContainer) {
        submitContainer.classList.toggle('d-none', enabled);
    }
    if (postActions) {
        postActions.classList.toggle('d-none', !enabled);
    }
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
function registerForgotEnhancedLoadListener() {
    const typedWindow = window;
    if (typedWindow.Blazor?.addEventListener) {
        typedWindow.Blazor.addEventListener('enhancedload', initializeForgotPassword);
        return;
    }
    window.setTimeout(registerForgotEnhancedLoadListener, 100);
}
registerForgotEnhancedLoadListener();
//# sourceMappingURL=auth-forgot-password.js.map
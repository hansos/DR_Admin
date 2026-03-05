"use strict";
async function initializeConfirmEmail() {
    const page = document.getElementById('account-confirm-email-page');
    if (!page) {
        return;
    }
    const typedWindow = window;
    typedWindow.UserPanelAlerts?.hide('auth-confirm-email-alert-success');
    typedWindow.UserPanelAlerts?.hide('auth-confirm-email-alert-error');
    const query = new URLSearchParams(window.location.search);
    const token = query.get('token') ?? '';
    const email = query.get('email') ?? '';
    setConfirmEmailAddress(email || 'your account');
    const submitButton = document.getElementById('auth-confirm-email-submit');
    if (submitButton) {
        submitButton.dataset.token = token;
        submitButton.disabled = !token;
        if (submitButton.dataset.bound !== 'true') {
            submitButton.dataset.bound = 'true';
            submitButton.addEventListener('click', async () => {
                const activeToken = submitButton.dataset.token ?? '';
                await confirmEmailToken(activeToken);
            });
        }
    }
    if (!token) {
        typedWindow.UserPanelAlerts?.showError('auth-confirm-email-alert-error', 'Missing confirmation token.');
        setConfirmEmailStatus('Use the link from your email to confirm your account.');
        return;
    }
}
async function confirmEmailToken(token) {
    const typedWindow = window;
    typedWindow.UserPanelAlerts?.hide('auth-confirm-email-alert-success');
    typedWindow.UserPanelAlerts?.hide('auth-confirm-email-alert-error');
    if (!token) {
        typedWindow.UserPanelAlerts?.showError('auth-confirm-email-alert-error', 'Missing confirmation token.');
        setConfirmEmailStatus('Use the link from your email to confirm your account.');
        return;
    }
    const submitButton = document.getElementById('auth-confirm-email-submit');
    if (submitButton) {
        submitButton.disabled = true;
    }
    const payload = { confirmationToken: token };
    const response = await typedWindow.UserPanelApi?.request('/MyAccount/confirm-email', {
        method: 'POST',
        body: JSON.stringify(payload)
    }, false);
    if (!response || !response.success) {
        typedWindow.UserPanelAlerts?.showError('auth-confirm-email-alert-error', response?.message ?? 'Email confirmation failed.');
        setConfirmEmailStatus('The confirmation link is invalid or expired.');
        if (submitButton) {
            submitButton.disabled = false;
        }
        return;
    }
    typedWindow.UserPanelAlerts?.showSuccess('auth-confirm-email-alert-success', response.message ?? 'Email confirmed successfully.');
    setConfirmEmailStatus('Your email has been verified.');
}
function setConfirmEmailStatus(message) {
    const status = document.getElementById('auth-confirm-email-status');
    if (status) {
        status.textContent = message;
    }
}
function setConfirmEmailAddress(email) {
    const element = document.getElementById('auth-confirm-email-address');
    if (element) {
        element.textContent = email;
    }
}
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', () => {
        void initializeConfirmEmail();
    });
}
else {
    void initializeConfirmEmail();
}
function registerConfirmEnhancedLoadListener() {
    const typedWindow = window;
    if (typedWindow.Blazor?.addEventListener) {
        typedWindow.Blazor.addEventListener('enhancedload', () => {
            void initializeConfirmEmail();
        });
        return;
    }
    window.setTimeout(registerConfirmEnhancedLoadListener, 100);
}
registerConfirmEnhancedLoadListener();
//# sourceMappingURL=auth-confirm-email.js.map
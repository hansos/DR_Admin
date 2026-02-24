"use strict";
// @ts-nocheck
(function () {
    function getApiBaseUrl() {
        var _a;
        const baseUrl = (_a = window.AppSettings) === null || _a === void 0 ? void 0 : _a.apiBaseUrl;
        if (!baseUrl) {
            const fallback = window.location.protocol === 'https:'
                ? 'https://localhost:7201/api/v1'
                : 'http://localhost:5133/api/v1';
            return fallback;
        }
        return baseUrl;
    }
    async function apiRequest(endpoint, options = {}) {
        var _a, _b, _c;
        try {
            const headers = Object.assign({ 'Content-Type': 'application/json' }, options.headers);
            const response = await fetch(endpoint, Object.assign(Object.assign({}, options), { headers, credentials: 'include' }));
            const contentType = (_a = response.headers.get('content-type')) !== null && _a !== void 0 ? _a : '';
            const hasJson = contentType.includes('application/json');
            const data = hasJson ? await response.json() : null;
            if (!response.ok) {
                return {
                    success: false,
                    message: (data && ((_b = data.message) !== null && _b !== void 0 ? _b : data.title)) || `Request failed with status ${response.status}`,
                };
            }
            return {
                success: true,
                data: (_c = data === null || data === void 0 ? void 0 : data.data) !== null && _c !== void 0 ? _c : data,
                message: data === null || data === void 0 ? void 0 : data.message,
            };
        }
        catch (error) {
            console.error('Reset password request failed', error);
            return {
                success: false,
                message: 'Network error. Please try again.',
            };
        }
    }
    function showSuccess(message) {
        const alertEl = document.getElementById('reset-password-alert-success');
        const errorEl = document.getElementById('reset-password-alert-error');
        if (errorEl) {
            errorEl.classList.add('d-none');
            errorEl.textContent = '';
        }
        if (alertEl) {
            alertEl.textContent = message;
            alertEl.classList.remove('d-none');
        }
    }
    function showError(message) {
        const alertEl = document.getElementById('reset-password-alert-error');
        const successEl = document.getElementById('reset-password-alert-success');
        if (successEl) {
            successEl.classList.add('d-none');
            successEl.textContent = '';
        }
        if (alertEl) {
            alertEl.textContent = message;
            alertEl.classList.remove('d-none');
        }
    }
    function hideAlerts() {
        const successEl = document.getElementById('reset-password-alert-success');
        const errorEl = document.getElementById('reset-password-alert-error');
        if (successEl) {
            successEl.classList.add('d-none');
            successEl.textContent = '';
        }
        if (errorEl) {
            errorEl.classList.add('d-none');
            errorEl.textContent = '';
        }
    }
    function getQueryParameter(name) {
        const urlParams = new URLSearchParams(window.location.search);
        return urlParams.get(name);
    }
    async function resetPassword(token, newPassword, confirmPassword) {
        const submitBtn = document.getElementById('reset-password-submit');
        if (submitBtn) {
            submitBtn.disabled = true;
            submitBtn.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>Resetting...';
        }
        const response = await apiRequest(`${getApiBaseUrl()}/MyAccount/reset-password`, {
            method: 'POST',
            body: JSON.stringify({ token, newPassword, confirmPassword }),
        });
        if (submitBtn) {
            submitBtn.disabled = false;
            submitBtn.innerHTML = '<i class="bi bi-check-lg"></i> Reset Password';
        }
        if (response.success) {
            showSuccess(response.message || 'Password reset successfully. You can now sign in with your new password.');
            const form = document.getElementById('reset-password-form');
            if (form) {
                form.reset();
            }
            // Redirect to login page after 2 seconds
            setTimeout(() => {
                window.location.href = '/login';
            }, 2000);
        }
        else {
            showError(response.message || 'Failed to reset password. Please try again or request a new reset link.');
        }
    }
    function bindEvents() {
        const form = document.getElementById('reset-password-form');
        if (!form) {
            return;
        }
        // Check if token exists in URL
        const token = getQueryParameter('token');
        if (!token) {
            showError('Invalid or missing reset token. Please request a new password reset.');
            const submitBtn = document.getElementById('reset-password-submit');
            if (submitBtn) {
                submitBtn.disabled = true;
            }
            return;
        }
        form.addEventListener('submit', async (e) => {
            var _a, _b;
            e.preventDefault();
            hideAlerts();
            const newPasswordInput = document.getElementById('reset-password-new');
            const confirmPasswordInput = document.getElementById('reset-password-confirm');
            const newPassword = (_a = newPasswordInput === null || newPasswordInput === void 0 ? void 0 : newPasswordInput.value.trim()) !== null && _a !== void 0 ? _a : '';
            const confirmPassword = (_b = confirmPasswordInput === null || confirmPasswordInput === void 0 ? void 0 : confirmPasswordInput.value.trim()) !== null && _b !== void 0 ? _b : '';
            if (!newPassword) {
                showError('Please enter your new password.');
                return;
            }
            if (newPassword.length < 6) {
                showError('Password must be at least 6 characters long.');
                return;
            }
            if (!confirmPassword) {
                showError('Please confirm your new password.');
                return;
            }
            if (newPassword !== confirmPassword) {
                showError('Passwords do not match.');
                return;
            }
            await resetPassword(token, newPassword, confirmPassword);
        });
    }
    let initialized = false;
    function initializeResetPassword() {
        const form = document.getElementById('reset-password-form');
        if (!form) {
            return false;
        }
        if (initialized) {
            return true;
        }
        initialized = true;
        bindEvents();
        console.log('Reset password page initialized');
        return true;
    }
    function tryInitialize() {
        if (initializeResetPassword()) {
            return;
        }
        let attempts = 0;
        const maxAttempts = 50;
        const interval = setInterval(() => {
            attempts++;
            if (initializeResetPassword() || attempts >= maxAttempts) {
                clearInterval(interval);
            }
        }, 100);
    }
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', tryInitialize);
    }
    else {
        tryInitialize();
    }
})();
//# sourceMappingURL=reset-password.js.map
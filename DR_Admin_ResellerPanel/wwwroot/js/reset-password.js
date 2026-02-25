"use strict";
// @ts-nocheck
(function () {
    function getApiBaseUrl() {
        const baseUrl = window.AppSettings?.apiBaseUrl;
        if (!baseUrl) {
            const fallback = window.location.protocol === 'https:'
                ? 'https://localhost:7201/api/v1'
                : 'http://localhost:5133/api/v1';
            return fallback;
        }
        return baseUrl;
    }
    async function apiRequest(endpoint, options = {}) {
        try {
            const headers = {
                'Content-Type': 'application/json',
                ...options.headers,
            };
            const response = await fetch(endpoint, {
                ...options,
                headers,
                credentials: 'include',
            });
            const contentType = response.headers.get('content-type') ?? '';
            const hasJson = contentType.includes('application/json');
            const data = hasJson ? await response.json() : null;
            if (!response.ok) {
                return {
                    success: false,
                    message: (data && (data.message ?? data.title)) || `Request failed with status ${response.status}`,
                };
            }
            return {
                success: true,
                data: data?.data ?? data,
                message: data?.message,
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
        // Password toggle functionality
        setupPasswordToggle('toggle-reset-password-new', 'reset-password-new', 'toggle-reset-password-new-icon');
        setupPasswordToggle('toggle-reset-password-confirm', 'reset-password-confirm', 'toggle-reset-password-confirm-icon');
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
            e.preventDefault();
            hideAlerts();
            const newPasswordInput = document.getElementById('reset-password-new');
            const confirmPasswordInput = document.getElementById('reset-password-confirm');
            const newPassword = newPasswordInput?.value.trim() ?? '';
            const confirmPassword = confirmPasswordInput?.value.trim() ?? '';
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
    function setupPasswordToggle(buttonId, inputId, iconId) {
        const toggleBtn = document.getElementById(buttonId);
        const passwordInput = document.getElementById(inputId);
        const toggleIcon = document.getElementById(iconId);
        if (toggleBtn && passwordInput && toggleIcon) {
            toggleBtn.addEventListener('click', () => {
                if (passwordInput.type === 'password') {
                    passwordInput.type = 'text';
                    toggleIcon.className = 'bi bi-eye-slash';
                }
                else {
                    passwordInput.type = 'password';
                    toggleIcon.className = 'bi bi-eye';
                }
            });
        }
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
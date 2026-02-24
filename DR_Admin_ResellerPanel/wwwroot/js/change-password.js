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
    function getAuthToken() {
        const auth = window.Auth;
        if (auth === null || auth === void 0 ? void 0 : auth.getToken) {
            return auth.getToken();
        }
        return sessionStorage.getItem('rp_authToken');
    }
    async function apiRequest(endpoint, options = {}) {
        var _a, _b, _c;
        try {
            const headers = Object.assign({ 'Content-Type': 'application/json' }, options.headers);
            const authToken = getAuthToken();
            if (authToken) {
                headers['Authorization'] = `Bearer ${authToken}`;
            }
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
            console.error('Change password request failed', error);
            return {
                success: false,
                message: 'Network error. Please try again.',
            };
        }
    }
    function showSuccess(message) {
        const alertEl = document.getElementById('change-password-alert-success');
        const errorEl = document.getElementById('change-password-alert-error');
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
        const alertEl = document.getElementById('change-password-alert-error');
        const successEl = document.getElementById('change-password-alert-success');
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
        const successEl = document.getElementById('change-password-alert-success');
        const errorEl = document.getElementById('change-password-alert-error');
        if (successEl) {
            successEl.classList.add('d-none');
            successEl.textContent = '';
        }
        if (errorEl) {
            errorEl.classList.add('d-none');
            errorEl.textContent = '';
        }
    }
    async function changePassword(currentPassword, newPassword, confirmPassword) {
        const submitBtn = document.getElementById('change-password-submit');
        if (submitBtn) {
            submitBtn.disabled = true;
            submitBtn.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>Changing...';
        }
        const response = await apiRequest(`${getApiBaseUrl()}/MyAccount/change-password`, {
            method: 'POST',
            body: JSON.stringify({ currentPassword, newPassword, confirmPassword }),
        });
        if (submitBtn) {
            submitBtn.disabled = false;
            submitBtn.innerHTML = '<i class="bi bi-check-lg"></i> Change Password';
        }
        if (response.success) {
            showSuccess(response.message || 'Password changed successfully.');
            const form = document.getElementById('change-password-form');
            if (form) {
                form.reset();
            }
        }
        else {
            showError(response.message || 'Failed to change password. Please try again.');
        }
    }
    function bindEvents() {
        const form = document.getElementById('change-password-form');
        if (!form) {
            return;
        }
        // Password toggle functionality
        setupPasswordToggle('toggle-change-password-current', 'change-password-current', 'toggle-change-password-current-icon');
        setupPasswordToggle('toggle-change-password-new', 'change-password-new', 'toggle-change-password-new-icon');
        setupPasswordToggle('toggle-change-password-confirm', 'change-password-confirm', 'toggle-change-password-confirm-icon');
        form.addEventListener('submit', async (e) => {
            var _a, _b, _c;
            e.preventDefault();
            hideAlerts();
            const currentInput = document.getElementById('change-password-current');
            const newInput = document.getElementById('change-password-new');
            const confirmInput = document.getElementById('change-password-confirm');
            const currentPassword = (_a = currentInput === null || currentInput === void 0 ? void 0 : currentInput.value) !== null && _a !== void 0 ? _a : '';
            const newPassword = (_b = newInput === null || newInput === void 0 ? void 0 : newInput.value) !== null && _b !== void 0 ? _b : '';
            const confirmPassword = (_c = confirmInput === null || confirmInput === void 0 ? void 0 : confirmInput.value) !== null && _c !== void 0 ? _c : '';
            if (!currentPassword) {
                showError('Please enter your current password.');
                return;
            }
            if (!newPassword) {
                showError('Please enter a new password.');
                return;
            }
            if (newPassword.length < 8) {
                showError('New password must be at least 8 characters long.');
                return;
            }
            if (newPassword !== confirmPassword) {
                showError('New passwords do not match.');
                return;
            }
            await changePassword(currentPassword, newPassword, confirmPassword);
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
    function initializeChangePassword() {
        const page = document.getElementById('change-password-page');
        if (!page) {
            return false;
        }
        if (initialized) {
            return true;
        }
        initialized = true;
        bindEvents();
        console.log('Change password page initialized');
        return true;
    }
    function tryInitialize() {
        if (initializeChangePassword()) {
            return;
        }
        let attempts = 0;
        const maxAttempts = 50;
        const interval = setInterval(() => {
            attempts++;
            if (initializeChangePassword() || attempts >= maxAttempts) {
                clearInterval(interval);
            }
        }, 100);
    }
    // Initialize when DOM is ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', tryInitialize);
    }
    else {
        tryInitialize();
    }
    // Listen for Blazor's enhanced navigation
    function setupBlazorNavListener() {
        const blazor = window.Blazor;
        if (blazor === null || blazor === void 0 ? void 0 : blazor.addEventListener) {
            blazor.addEventListener('enhancedload', () => {
                initialized = false;
                tryInitialize();
            });
            console.log('ChangePassword: Blazor enhancedload listener registered');
        }
        else {
            setTimeout(setupBlazorNavListener, 100);
        }
    }
    setupBlazorNavListener();
    // Handle InteractiveServer re-render after initial page load
    window.addEventListener('load', () => {
        if (!initialized) {
            tryInitialize();
        }
    });
})();
//# sourceMappingURL=change-password.js.map
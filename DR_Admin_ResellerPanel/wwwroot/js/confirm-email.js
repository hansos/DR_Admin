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
            console.error('Confirm email request failed', error);
            return {
                success: false,
                message: 'Network error. Please try again.',
            };
        }
    }
    function showSuccess(message) {
        const alertEl = document.getElementById('confirm-email-alert-success');
        const errorEl = document.getElementById('confirm-email-alert-error');
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
        const alertEl = document.getElementById('confirm-email-alert-error');
        const successEl = document.getElementById('confirm-email-alert-success');
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
        const successEl = document.getElementById('confirm-email-alert-success');
        const errorEl = document.getElementById('confirm-email-alert-error');
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
    async function confirmEmail(token) {
        const submitBtn = document.getElementById('confirm-email-submit');
        if (submitBtn) {
            submitBtn.disabled = true;
            submitBtn.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>Confirming...';
        }
        const response = await apiRequest(`${getApiBaseUrl()}/MyAccount/confirm-email`, {
            method: 'POST',
            body: JSON.stringify({ confirmationToken: token }),
        });
        if (submitBtn) {
            submitBtn.disabled = false;
            submitBtn.innerHTML = '<i class="bi bi-check-lg"></i> Confirm Email';
        }
        if (response.success) {
            showSuccess(response.message || 'Email confirmed successfully. You can now sign in.');
            if (submitBtn) {
                submitBtn.disabled = true;
            }
            setTimeout(() => {
                window.location.href = '/login';
            }, 3000);
        }
        else {
            showError(response.message || 'Email confirmation failed. The link may be invalid or expired.');
        }
    }
    function setEmailAddress(email) {
        const element = document.getElementById('confirm-email-address');
        if (element) {
            element.textContent = email;
        }
    }
    function setStatus(message) {
        const element = document.getElementById('confirm-email-status');
        if (element) {
            element.textContent = message;
        }
    }
    function bindEvents() {
        const submitBtn = document.getElementById('confirm-email-submit');
        if (!submitBtn) {
            return;
        }
        const token = getQueryParameter('token');
        const email = getQueryParameter('email');
        if (email) {
            setEmailAddress(email);
        }
        if (!token) {
            showError('Missing confirmation token. Please use the link from your email.');
            submitBtn.disabled = true;
            return;
        }
        submitBtn.addEventListener('click', async () => {
            hideAlerts();
            await confirmEmail(token);
        });
    }
    let initialized = false;
    function initializeConfirmEmail() {
        const submitBtn = document.getElementById('confirm-email-submit');
        if (!submitBtn) {
            return false;
        }
        if (initialized) {
            return true;
        }
        initialized = true;
        bindEvents();
        console.log('Confirm email page initialized');
        return true;
    }
    function tryInitialize() {
        if (initializeConfirmEmail()) {
            return;
        }
        let attempts = 0;
        const maxAttempts = 50;
        const interval = setInterval(() => {
            attempts++;
            if (initializeConfirmEmail() || attempts >= maxAttempts) {
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
//# sourceMappingURL=confirm-email.js.map
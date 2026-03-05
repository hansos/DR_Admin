"use strict";
(() => {
    const typedWindow = window;
    function getApiBaseUrl() {
        return typedWindow.AppSettings?.apiBaseUrl ?? '';
    }
    function getAuthToken() {
        if (typedWindow.Auth?.getToken) {
            return typedWindow.Auth.getToken();
        }
        return sessionStorage.getItem('rp_authToken');
    }
    function setText(id, text) {
        const el = document.getElementById(id);
        if (!el) {
            return;
        }
        el.textContent = text;
    }
    function showSuccess(message) {
        const success = document.getElementById('help-status-alert-success');
        const error = document.getElementById('help-status-alert-error');
        if (success) {
            success.textContent = message;
            success.classList.remove('d-none');
        }
        error?.classList.add('d-none');
    }
    function showError(message) {
        const success = document.getElementById('help-status-alert-success');
        const error = document.getElementById('help-status-alert-error');
        if (error) {
            error.textContent = message;
            error.classList.remove('d-none');
        }
        success?.classList.add('d-none');
    }
    function hideAlerts() {
        document.getElementById('help-status-alert-success')?.classList.add('d-none');
        document.getElementById('help-status-alert-error')?.classList.add('d-none');
    }
    function maskToken(token) {
        if (token.length <= 16) {
            return token;
        }
        return `${token.slice(0, 8)}...${token.slice(-8)}`;
    }
    function decodeJwtPayload(token) {
        const parts = token.split('.');
        if (parts.length < 2) {
            return null;
        }
        try {
            const base64 = parts[1].replace(/-/g, '+').replace(/_/g, '/');
            const padded = base64.padEnd(Math.ceil(base64.length / 4) * 4, '=');
            const json = atob(padded);
            return JSON.parse(json);
        }
        catch {
            return null;
        }
    }
    function formatExpiry(token) {
        const payload = decodeJwtPayload(token);
        if (!payload?.exp) {
            return 'Unknown';
        }
        const date = new Date(payload.exp * 1000);
        if (Number.isNaN(date.getTime())) {
            return 'Unknown';
        }
        return date.toLocaleString();
    }
    function refreshStatus() {
        hideAlerts();
        const apiBaseUrl = getApiBaseUrl();
        const token = getAuthToken();
        setText('help-status-api-base-url', apiBaseUrl || '(not configured)');
        setText('help-status-auth-state', token ? 'Authenticated' : 'Not authenticated');
        setText('help-status-token-preview', token ? maskToken(token) : '-');
        setText('help-status-token-length', token ? String(token.length) : '0');
        setText('help-status-token-expiry', token ? formatExpiry(token) : '-');
    }
    async function copyTokenToClipboard() {
        const token = sessionStorage.getItem('rp_authToken');
        if (!token) {
            showError('No rp_authToken found in session storage.');
            return;
        }
        try {
            if (navigator.clipboard?.writeText) {
                await navigator.clipboard.writeText(token);
            }
            else {
                const textarea = document.createElement('textarea');
                textarea.value = token;
                textarea.style.position = 'fixed';
                textarea.style.left = '-9999px';
                document.body.appendChild(textarea);
                textarea.focus();
                textarea.select();
                const copied = document.execCommand('copy');
                document.body.removeChild(textarea);
                if (!copied) {
                    throw new Error('Copy command failed.');
                }
            }
            showSuccess('rp_authToken copied to clipboard.');
        }
        catch {
            showError('Unable to copy rp_authToken to clipboard.');
        }
    }
    function bindEvents() {
        document.getElementById('help-status-refresh')?.addEventListener('click', refreshStatus);
        document.getElementById('help-status-copy-token')?.addEventListener('click', () => {
            void copyTokenToClipboard();
        });
    }
    function initializePage() {
        const page = document.getElementById('help-status-page');
        if (!page || page.dataset.initialized === 'true') {
            return;
        }
        page.dataset.initialized = 'true';
        bindEvents();
        refreshStatus();
    }
    function setupObserver() {
        initializePage();
        if (!document.body) {
            return;
        }
        const observer = new MutationObserver(() => {
            initializePage();
        });
        observer.observe(document.body, { childList: true, subtree: true });
    }
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', setupObserver);
    }
    else {
        setupObserver();
    }
})();
//# sourceMappingURL=help-status.js.map
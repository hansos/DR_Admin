(() => {
    interface AuthWindow extends Window {
        AppSettings?: { apiBaseUrl?: string };
        Auth?: { getToken?: () => string | null };
    }

    interface JwtPayload {
        exp?: number;
    }

    const typedWindow = window as AuthWindow;

    function getApiBaseUrl(): string {
        return typedWindow.AppSettings?.apiBaseUrl ?? '';
    }

    function getAuthToken(): string | null {
        if (typedWindow.Auth?.getToken) {
            return typedWindow.Auth.getToken();
        }

        return sessionStorage.getItem('rp_authToken');
    }

    function setText(id: string, text: string): void {
        const el = document.getElementById(id);
        if (!el) {
            return;
        }

        el.textContent = text;
    }

    function showError(message: string): void {
        const success = document.getElementById('help-status-alert-success');
        const error = document.getElementById('help-status-alert-error');

        if (error) {
            error.textContent = message;
            error.classList.remove('d-none');
        }

        success?.classList.add('d-none');
    }

    function hideAlerts(): void {
        document.getElementById('help-status-alert-success')?.classList.add('d-none');
        document.getElementById('help-status-alert-error')?.classList.add('d-none');
    }

    function maskToken(token: string): string {
        if (token.length <= 16) {
            return token;
        }

        return `${token.slice(0, 8)}...${token.slice(-8)}`;
    }

    function decodeJwtPayload(token: string): JwtPayload | null {
        const parts = token.split('.');
        if (parts.length < 2) {
            return null;
        }

        try {
            const base64 = parts[1].replace(/-/g, '+').replace(/_/g, '/');
            const padded = base64.padEnd(Math.ceil(base64.length / 4) * 4, '=');
            const json = atob(padded);
            return JSON.parse(json) as JwtPayload;
        } catch {
            return null;
        }
    }

    function formatExpiry(token: string): string {
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

    function refreshStatus(): void {
        hideAlerts();

        const apiBaseUrl = getApiBaseUrl();
        const token = getAuthToken();

        setText('help-status-api-base-url', apiBaseUrl || '(not configured)');
        setText('help-status-auth-state', token ? 'Authenticated' : 'Not authenticated');
        setText('help-status-token-preview', token ? maskToken(token) : '-');
        setText('help-status-token-length', token ? String(token.length) : '0');
        setText('help-status-token-expiry', token ? formatExpiry(token) : '-');
    }

    function bindEvents(): void {
        document.getElementById('help-status-refresh')?.addEventListener('click', refreshStatus);
    }

    function initializePage(): void {
        const page = document.getElementById('help-status-page');
        if (!page || page.dataset.initialized === 'true') {
            return;
        }

        page.dataset.initialized = 'true';
        bindEvents();
        refreshStatus();
    }

    function setupObserver(): void {
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
    } else {
        setupObserver();
    }
})();

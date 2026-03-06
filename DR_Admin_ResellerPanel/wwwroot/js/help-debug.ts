(() => {
    interface AuthWindow extends Window {
        AppSettings?: { apiBaseUrl?: string };
        Auth?: { getToken?: () => string | null };
    }

    interface BuildModeResponse {
        mode?: string;
        Mode?: string;
        isDebug?: boolean;
        IsDebug?: boolean;
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

    function showSuccess(message: string): void {
        const success = document.getElementById('help-debug-alert-success');
        const error = document.getElementById('help-debug-alert-error');

        if (success) {
            success.textContent = message;
            success.classList.remove('d-none');
        }

        error?.classList.add('d-none');
    }

    function showError(message: string): void {
        const success = document.getElementById('help-debug-alert-success');
        const error = document.getElementById('help-debug-alert-error');

        if (error) {
            error.textContent = message;
            error.classList.remove('d-none');
        }

        success?.classList.add('d-none');
    }

    function hideAlerts(): void {
        document.getElementById('help-debug-alert-success')?.classList.add('d-none');
        document.getElementById('help-debug-alert-error')?.classList.add('d-none');
    }

    async function getBuildMode(): Promise<BuildModeResponse | null> {
        const apiBaseUrl = getApiBaseUrl();
        if (!apiBaseUrl) {
            return null;
        }

        const headers = new Headers();
        headers.set('Content-Type', 'application/json');

        const token = getAuthToken();
        if (token) {
            headers.set('Authorization', `Bearer ${token}`);
        }

        try {
            const response = await fetch(`${apiBaseUrl}/Test/build-mode`, {
                method: 'GET',
                headers,
                credentials: 'include',
            });

            if (!response.ok) {
                return null;
            }

            return (await response.json()) as BuildModeResponse;
        } catch {
            return null;
        }
    }

    async function refreshStatus(): Promise<void> {
        hideAlerts();

        const buildMode = await getBuildMode();
        if (!buildMode) {
            setText('help-debug-build-mode', 'Unavailable');
            setText('help-debug-build-flag', 'Unknown');
            showError('Unable to verify API build mode.');
            return;
        }

        const mode = buildMode.mode ?? buildMode.Mode ?? 'Unknown';
        const isDebug = buildMode.isDebug === true || buildMode.IsDebug === true;

        setText('help-debug-build-mode', mode);
        setText('help-debug-build-flag', isDebug ? 'true' : 'false');

        if (!isDebug) {
            showError('Debug page is only available when the API runs in Debug mode. Redirecting to dashboard...');
            window.setTimeout(() => {
                window.location.replace('/');
            }, 1200);
            return;
        }

        showSuccess('Debug mode verified.');
    }

    function bindEvents(): void {
        document.getElementById('help-debug-refresh')?.addEventListener('click', () => {
            void refreshStatus();
        });
    }

    function initializePage(): void {
        const page = document.getElementById('help-debug-page');
        if (!page || page.dataset.initialized === 'true') {
            return;
        }

        page.dataset.initialized = 'true';
        bindEvents();
        void refreshStatus();
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

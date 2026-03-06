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

    interface SnapshotExportResponse {
        success?: boolean;
        Success?: boolean;
        fileName?: string;
        FileName?: string;
        filePath?: string;
        FilePath?: string;
        errorMessage?: string;
        ErrorMessage?: string;
    }

    interface SnapshotImportResponse {
        success?: boolean;
        Success?: boolean;
        filePath?: string;
        FilePath?: string;
        errorMessage?: string;
        ErrorMessage?: string;
    }

    const typedWindow = window as AuthWindow;
    let isDebugMode = false;

    function getApiBaseUrl(): string {
        return typedWindow.AppSettings?.apiBaseUrl ?? '';
    }

    async function callApi<T>(path: string, method: 'GET' | 'POST', body?: object): Promise<T | null> {
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
            const response = await fetch(`${apiBaseUrl}${path}`, {
                method,
                headers,
                credentials: 'include',
                body: body ? JSON.stringify(body) : undefined,
            });

            if (!response.ok) {
                return null;
            }

            return (await response.json()) as T;
        } catch {
            return null;
        }
    }

    function getSnapshotFileName(): string {
        const input = document.getElementById('help-debug-snapshot-file') as HTMLInputElement | null;
        return input?.value.trim() ?? '';
    }

    function setLastSnapshotFile(value: string): void {
        setText('help-debug-snapshot-last-file', value || '-');
    }

    async function exportSnapshot(): Promise<void> {
        if (!isDebugMode) {
            showError('Export is only available in Debug mode.');
            return;
        }

        const fileName = getSnapshotFileName();
        const query = fileName ? `?fileName=${encodeURIComponent(fileName)}` : '';
        const result = await callApi<SnapshotExportResponse>(`/Test/admin-mycompany/export${query}`, 'POST');

        if (!result) {
            showError('Failed to export admin/MyCompany snapshot.');
            return;
        }

        const success = result.success === true || result.Success === true;
        if (!success) {
            showError(result.errorMessage ?? result.ErrorMessage ?? 'Failed to export admin/MyCompany snapshot.');
            return;
        }

        const savedFile = result.fileName ?? result.FileName ?? result.filePath ?? result.FilePath ?? '-';
        setLastSnapshotFile(savedFile);
        showSuccess('Admin/MyCompany snapshot exported successfully.');
    }

    async function importSnapshot(): Promise<void> {
        if (!isDebugMode) {
            showError('Import is only available in Debug mode.');
            return;
        }

        const fileName = getSnapshotFileName();
        if (!fileName) {
            showError('Enter snapshot file name before importing.');
            return;
        }

        const result = await callApi<SnapshotImportResponse>('/Test/admin-mycompany/import', 'POST', { fileName });
        if (!result) {
            showError('Failed to import admin/MyCompany snapshot.');
            return;
        }

        const success = result.success === true || result.Success === true;
        if (!success) {
            showError(result.errorMessage ?? result.ErrorMessage ?? 'Failed to import admin/MyCompany snapshot.');
            return;
        }

        setLastSnapshotFile(fileName);
        showSuccess('Admin/MyCompany snapshot imported successfully.');
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
        isDebugMode = isDebug;

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

        document.getElementById('help-debug-export-snapshot')?.addEventListener('click', () => {
            void exportSnapshot();
        });

        document.getElementById('help-debug-import-snapshot')?.addEventListener('click', () => {
            void importSnapshot();
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

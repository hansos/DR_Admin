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

    interface SeedTestDataResponse {
        success?: boolean;
        Success?: boolean;
        message?: string;
        Message?: string;
    }

    interface DebugRuntimeInfoResponse {
        databaseConnectionDescription?: string;
        DatabaseConnectionDescription?: string;
        simulatorRegistrarDatabasePath?: string;
        SimulatorRegistrarDatabasePath?: string;
        userJsonImportFilePath?: string;
        UserJsonImportFilePath?: string;
        adminJsonImportFilePath?: string;
        AdminJsonImportFilePath?: string;
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

    function getText(id: string): string {
        const el = document.getElementById(id);
        return el?.textContent?.trim() ?? '';
    }

    async function copyToClipboard(value: string, successMessage: string): Promise<void> {
        if (!value) {
            showError('Nothing to copy.');
            return;
        }

        try {
            await navigator.clipboard.writeText(value);
            showSuccess(successMessage);
        } catch {
            showError('Unable to copy to clipboard.');
        }
    }

    function setAuthTokenDisplay(): void {
        const token = getAuthToken();
        setText('help-debug-auth-token', token && token.length > 0 ? token : '-');
    }

    async function copyAuthToken(): Promise<void> {
        const token = getAuthToken();
        await copyToClipboard(token ?? '', 'rp_authToken copied to clipboard.');
    }

    async function copyPathValue(sourceId: string, label: string): Promise<void> {
        const value = getText(sourceId);
        await copyToClipboard(value === '-' ? '' : value, `${label} path copied to clipboard.`);
    }

    async function copyAllPaths(): Promise<void> {
        const simulatorDbPath = getText('help-debug-path-simulator-db');
        const userJsonPath = getText('help-debug-path-user-json');
        const adminJsonPath = getText('help-debug-path-admin-json');

        const lines = [
            `simulator-registrar database: ${simulatorDbPath}`,
            `user json import file: ${userJsonPath}`,
            `admin json import file: ${adminJsonPath}`,
        ].filter((line) => !line.endsWith(': -'));

        await copyToClipboard(lines.join('\n'), 'All debug paths copied to clipboard.');
    }

    async function loadDebugRuntimeInfo(): Promise<void> {
        const result = await callApi<DebugRuntimeInfoResponse>('/Test/debug-runtime-info', 'GET');
        if (!result) {
            setText('help-debug-db-connection', 'Unavailable');
            setText('help-debug-path-simulator-db', '-');
            setText('help-debug-path-user-json', '-');
            setText('help-debug-path-admin-json', '-');
            showError('Failed to load debug runtime details.');
            return;
        }

        const connectionDescription = result.databaseConnectionDescription ?? result.DatabaseConnectionDescription ?? 'Unavailable';
        const simulatorDbPath = result.simulatorRegistrarDatabasePath ?? result.SimulatorRegistrarDatabasePath ?? '-';
        const userJsonPath = result.userJsonImportFilePath ?? result.UserJsonImportFilePath ?? '-';
        const adminJsonPath = result.adminJsonImportFilePath ?? result.AdminJsonImportFilePath ?? '-';

        setText('help-debug-db-connection', connectionDescription);
        setText('help-debug-path-simulator-db', simulatorDbPath);
        setText('help-debug-path-user-json', userJsonPath);
        setText('help-debug-path-admin-json', adminJsonPath);
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

    async function seedTestData(): Promise<void> {
        if (!isDebugMode) {
            showError('Seeding test data is only available in Debug mode.');
            return;
        }

        const result = await callApi<SeedTestDataResponse>('/Test/seed-data', 'POST');
        if (!result) {
            showError('Failed to seed test data.');
            return;
        }

        const success = (result.success ?? result.Success ?? true) === true;
        if (!success) {
            showError(result.message ?? result.Message ?? 'Failed to seed test data.');
            return;
        }

        showSuccess(result.message ?? result.Message ?? 'Test data seeded successfully.');
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
        setAuthTokenDisplay();

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

        await loadDebugRuntimeInfo();

        showSuccess('Debug mode verified.');
    }

    function bindEvents(): void {
        document.getElementById('help-debug-refresh')?.addEventListener('click', () => {
            void refreshStatus();
        });

        document.getElementById('help-debug-export-snapshot')?.addEventListener('click', () => {
            void exportSnapshot();
        });

        document.getElementById('help-debug-seed-test-data')?.addEventListener('click', () => {
            void seedTestData();
        });

        document.getElementById('help-debug-copy-token')?.addEventListener('click', () => {
            void copyAuthToken();
        });

        document.getElementById('help-debug-copy-path-simulator-db')?.addEventListener('click', () => {
            void copyPathValue('help-debug-path-simulator-db', 'Simulator registrar database');
        });

        document.getElementById('help-debug-copy-path-user-json')?.addEventListener('click', () => {
            void copyPathValue('help-debug-path-user-json', 'User json import file');
        });

        document.getElementById('help-debug-copy-path-admin-json')?.addEventListener('click', () => {
            void copyPathValue('help-debug-path-admin-json', 'Admin json import file');
        });

        document.getElementById('help-debug-copy-all-paths')?.addEventListener('click', () => {
            void copyAllPaths();
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

"use strict";
(() => {
    const typedWindow = window;
    let isDebugMode = false;
    function getApiBaseUrl() {
        return typedWindow.AppSettings?.apiBaseUrl ?? '';
    }
    async function callApi(path, method, body) {
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
            return (await response.json());
        }
        catch {
            return null;
        }
    }
    function getSnapshotFileName() {
        const input = document.getElementById('help-debug-snapshot-file');
        return input?.value.trim() ?? '';
    }
    function setLastSnapshotFile(value) {
        setText('help-debug-snapshot-last-file', value || '-');
    }
    function getText(id) {
        const el = document.getElementById(id);
        return el?.textContent?.trim() ?? '';
    }
    async function copyToClipboard(value, successMessage) {
        if (!value) {
            showError('Nothing to copy.');
            return;
        }
        try {
            await navigator.clipboard.writeText(value);
            showSuccess(successMessage);
        }
        catch {
            showError('Unable to copy to clipboard.');
        }
    }
    function setAuthTokenDisplay() {
        const token = getAuthToken();
        setText('help-debug-auth-token', token && token.length > 0 ? token : '-');
    }
    async function copyAuthToken() {
        const token = getAuthToken();
        await copyToClipboard(token ?? '', 'rp_authToken copied to clipboard.');
    }
    async function copyPathValue(sourceId, label) {
        const value = getText(sourceId);
        await copyToClipboard(value === '-' ? '' : value, `${label} path copied to clipboard.`);
    }
    async function copyAllPaths() {
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
    async function loadDebugRuntimeInfo() {
        const result = await callApi('/Test/debug-runtime-info', 'GET');
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
    async function exportSnapshot() {
        if (!isDebugMode) {
            showError('Export is only available in Debug mode.');
            return;
        }
        const fileName = getSnapshotFileName();
        const query = fileName ? `?fileName=${encodeURIComponent(fileName)}` : '';
        const result = await callApi(`/Test/admin-mycompany/export${query}`, 'POST');
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
    async function seedTestData() {
        if (!isDebugMode) {
            showError('Seeding test data is only available in Debug mode.');
            return;
        }
        const result = await callApi('/Test/seed-data', 'POST');
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
        const success = document.getElementById('help-debug-alert-success');
        const error = document.getElementById('help-debug-alert-error');
        if (success) {
            success.textContent = message;
            success.classList.remove('d-none');
        }
        error?.classList.add('d-none');
    }
    function showError(message) {
        const success = document.getElementById('help-debug-alert-success');
        const error = document.getElementById('help-debug-alert-error');
        if (error) {
            error.textContent = message;
            error.classList.remove('d-none');
        }
        success?.classList.add('d-none');
    }
    function hideAlerts() {
        document.getElementById('help-debug-alert-success')?.classList.add('d-none');
        document.getElementById('help-debug-alert-error')?.classList.add('d-none');
    }
    async function getBuildMode() {
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
            return (await response.json());
        }
        catch {
            return null;
        }
    }
    async function refreshStatus() {
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
    function bindEvents() {
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
    function initializePage() {
        const page = document.getElementById('help-debug-page');
        if (!page || page.dataset.initialized === 'true') {
            return;
        }
        page.dataset.initialized = 'true';
        bindEvents();
        void refreshStatus();
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
//# sourceMappingURL=help-debug.js.map
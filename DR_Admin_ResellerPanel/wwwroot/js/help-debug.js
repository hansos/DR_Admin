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
    async function importSnapshot() {
        if (!isDebugMode) {
            showError('Import is only available in Debug mode.');
            return;
        }
        const fileName = getSnapshotFileName();
        if (!fileName) {
            showError('Enter snapshot file name before importing.');
            return;
        }
        const result = await callApi('/Test/admin-mycompany/import', 'POST', { fileName });
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
    function bindEvents() {
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
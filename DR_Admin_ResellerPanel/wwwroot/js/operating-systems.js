"use strict";
// @ts-nocheck
(function () {
    function getApiBaseUrl() {
        var _a, _b;
        return (_b = (_a = window.AppSettings) === null || _a === void 0 ? void 0 : _a.apiBaseUrl) !== null && _b !== void 0 ? _b : '';
    }
    let allOperatingSystems = [];
    let editingId = null;
    let pendingDeleteId = null;
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
                success: (data === null || data === void 0 ? void 0 : data.success) !== false,
                data: (_c = data === null || data === void 0 ? void 0 : data.data) !== null && _c !== void 0 ? _c : data,
                message: data === null || data === void 0 ? void 0 : data.message,
            };
        }
        catch (error) {
            console.error('Operating systems request failed', error);
            return {
                success: false,
                message: 'Network error. Please try again.',
            };
        }
    }
    async function loadOperatingSystems() {
        var _a;
        const tableBody = document.getElementById('operating-systems-table-body');
        if (!tableBody) {
            return;
        }
        tableBody.innerHTML = '<tr><td colspan="7" class="text-center"><div class="spinner-border text-primary"></div></td></tr>';
        const response = await apiRequest(`${getApiBaseUrl()}/OperatingSystems`, { method: 'GET' });
        if (!response.success) {
            showError(response.message || 'Failed to load operating systems');
            tableBody.innerHTML = '<tr><td colspan="7" class="text-center text-danger">Failed to load data</td></tr>';
            return;
        }
        const rawItems = Array.isArray(response.data)
            ? response.data
            : Array.isArray((_a = response.data) === null || _a === void 0 ? void 0 : _a.data)
                ? response.data.data
                : [];
        allOperatingSystems = rawItems.map((item) => {
            var _a, _b, _c, _d, _e, _f, _g, _h, _j, _k, _l, _m;
            return ({
                id: (_b = (_a = item.id) !== null && _a !== void 0 ? _a : item.Id) !== null && _b !== void 0 ? _b : 0,
                name: (_d = (_c = item.name) !== null && _c !== void 0 ? _c : item.Name) !== null && _d !== void 0 ? _d : '',
                displayName: (_f = (_e = item.displayName) !== null && _e !== void 0 ? _e : item.DisplayName) !== null && _f !== void 0 ? _f : '',
                version: (_h = (_g = item.version) !== null && _g !== void 0 ? _g : item.Version) !== null && _h !== void 0 ? _h : null,
                description: (_k = (_j = item.description) !== null && _j !== void 0 ? _j : item.Description) !== null && _k !== void 0 ? _k : null,
                isActive: (_m = (_l = item.isActive) !== null && _l !== void 0 ? _l : item.IsActive) !== null && _m !== void 0 ? _m : false,
            });
        });
        renderTable();
    }
    function renderTable() {
        const tableBody = document.getElementById('operating-systems-table-body');
        if (!tableBody) {
            return;
        }
        if (!allOperatingSystems.length) {
            tableBody.innerHTML = '<tr><td colspan="7" class="text-center text-muted">No operating systems found. Click "New Operating System" to add one.</td></tr>';
            return;
        }
        tableBody.innerHTML = allOperatingSystems.map((os) => `
        <tr>
            <td>${os.id}</td>
            <td><code>${esc(os.name)}</code></td>
            <td>${esc(os.displayName)}</td>
            <td>${esc(os.version || '-')}</td>
            <td>${esc(os.description || '-')}</td>
            <td><span class="badge bg-${os.isActive ? 'success' : 'secondary'}">${os.isActive ? 'Active' : 'Inactive'}</span></td>
            <td>
                <div class="btn-group btn-group-sm">
                    <button class="btn btn-outline-primary" type="button" data-action="edit" data-id="${os.id}" title="Edit"><i class="bi bi-pencil"></i></button>
                    <button class="btn btn-outline-danger" type="button" data-action="delete" data-id="${os.id}" data-name="${esc(os.displayName)}" title="Delete"><i class="bi bi-trash"></i></button>
                </div>
            </td>
        </tr>
    `).join('');
    }
    function openCreate() {
        editingId = null;
        const modalTitle = document.getElementById('operating-systems-modal-title');
        if (modalTitle) {
            modalTitle.textContent = 'New Operating System';
        }
        const form = document.getElementById('operating-systems-form');
        form === null || form === void 0 ? void 0 : form.reset();
        const isActiveInput = document.getElementById('operating-systems-is-active');
        if (isActiveInput) {
            isActiveInput.checked = true;
        }
        showModal('operating-systems-edit-modal');
    }
    function openEdit(id) {
        const os = allOperatingSystems.find((item) => item.id === id);
        if (!os) {
            return;
        }
        editingId = id;
        const modalTitle = document.getElementById('operating-systems-modal-title');
        if (modalTitle) {
            modalTitle.textContent = 'Edit Operating System';
        }
        const nameInput = document.getElementById('operating-systems-name');
        const displayNameInput = document.getElementById('operating-systems-display-name');
        const versionInput = document.getElementById('operating-systems-version');
        const descriptionInput = document.getElementById('operating-systems-description');
        const isActiveInput = document.getElementById('operating-systems-is-active');
        if (nameInput) {
            nameInput.value = os.name;
        }
        if (displayNameInput) {
            displayNameInput.value = os.displayName;
        }
        if (versionInput) {
            versionInput.value = os.version || '';
        }
        if (descriptionInput) {
            descriptionInput.value = os.description || '';
        }
        if (isActiveInput) {
            isActiveInput.checked = os.isActive;
        }
        showModal('operating-systems-edit-modal');
    }
    async function saveOperatingSystem() {
        var _a, _b, _c;
        const nameInput = document.getElementById('operating-systems-name');
        const displayNameInput = document.getElementById('operating-systems-display-name');
        const versionInput = document.getElementById('operating-systems-version');
        const descriptionInput = document.getElementById('operating-systems-description');
        const isActiveInput = document.getElementById('operating-systems-is-active');
        const name = (_a = nameInput === null || nameInput === void 0 ? void 0 : nameInput.value.trim()) !== null && _a !== void 0 ? _a : '';
        const displayName = (_b = displayNameInput === null || displayNameInput === void 0 ? void 0 : displayNameInput.value.trim()) !== null && _b !== void 0 ? _b : '';
        if (!name || !displayName) {
            showError('Internal Name and Display Name are required');
            return;
        }
        const payload = {
            name,
            displayName,
            version: (versionInput === null || versionInput === void 0 ? void 0 : versionInput.value.trim()) || null,
            description: (descriptionInput === null || descriptionInput === void 0 ? void 0 : descriptionInput.value.trim()) || null,
            isActive: (_c = isActiveInput === null || isActiveInput === void 0 ? void 0 : isActiveInput.checked) !== null && _c !== void 0 ? _c : false,
        };
        const response = editingId
            ? await apiRequest(`${getApiBaseUrl()}/OperatingSystems/${editingId}`, { method: 'PUT', body: JSON.stringify(payload) })
            : await apiRequest(`${getApiBaseUrl()}/OperatingSystems`, { method: 'POST', body: JSON.stringify(payload) });
        if (response.success) {
            hideModal('operating-systems-edit-modal');
            showSuccess(editingId ? 'Operating system updated successfully' : 'Operating system created successfully');
            loadOperatingSystems();
        }
        else {
            showError(response.message || 'Save failed');
        }
    }
    function openDelete(id, name) {
        pendingDeleteId = id;
        const deleteName = document.getElementById('operating-systems-delete-name');
        if (deleteName) {
            deleteName.textContent = name;
        }
        showModal('operating-systems-delete-modal');
    }
    async function doDelete() {
        if (!pendingDeleteId) {
            return;
        }
        const response = await apiRequest(`${getApiBaseUrl()}/OperatingSystems/${pendingDeleteId}`, { method: 'DELETE' });
        hideModal('operating-systems-delete-modal');
        if (response.success) {
            showSuccess('Operating system deleted successfully');
            loadOperatingSystems();
        }
        else {
            showError(response.message || 'Delete failed');
        }
        pendingDeleteId = null;
    }
    function esc(text) {
        const map = { '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#039;' };
        return (text || '').toString().replace(/[&<>"']/g, (char) => map[char]);
    }
    function showSuccess(message) {
        const alert = document.getElementById('operating-systems-alert-success');
        if (!alert) {
            return;
        }
        alert.textContent = message;
        alert.classList.remove('d-none');
        const errorAlert = document.getElementById('operating-systems-alert-error');
        errorAlert === null || errorAlert === void 0 ? void 0 : errorAlert.classList.add('d-none');
        setTimeout(() => alert.classList.add('d-none'), 5000);
    }
    function showError(message) {
        const alert = document.getElementById('operating-systems-alert-error');
        if (!alert) {
            return;
        }
        alert.textContent = message;
        alert.classList.remove('d-none');
        const successAlert = document.getElementById('operating-systems-alert-success');
        successAlert === null || successAlert === void 0 ? void 0 : successAlert.classList.add('d-none');
    }
    function showModal(id) {
        const element = document.getElementById(id);
        if (!element || !window.bootstrap) {
            return;
        }
        const modal = new window.bootstrap.Modal(element);
        modal.show();
    }
    function hideModal(id) {
        const element = document.getElementById(id);
        if (!element || !window.bootstrap) {
            return;
        }
        const modal = window.bootstrap.Modal.getInstance(element);
        modal === null || modal === void 0 ? void 0 : modal.hide();
    }
    function bindTableActions() {
        const tableBody = document.getElementById('operating-systems-table-body');
        if (!tableBody) {
            return;
        }
        tableBody.addEventListener('click', (event) => {
            var _a;
            const target = event.target;
            const button = target.closest('button[data-action]');
            if (!button) {
                return;
            }
            const id = Number(button.dataset.id);
            if (!id) {
                return;
            }
            if (button.dataset.action === 'edit') {
                openEdit(id);
                return;
            }
            if (button.dataset.action === 'delete') {
                openDelete(id, (_a = button.dataset.name) !== null && _a !== void 0 ? _a : '');
            }
        });
    }
    function initializeOperatingSystemsPage() {
        var _a, _b, _c;
        const page = document.getElementById('operating-systems-page');
        if (!page || page.dataset.initialized === 'true') {
            return;
        }
        page.dataset.initialized = 'true';
        (_a = document.getElementById('operating-systems-create')) === null || _a === void 0 ? void 0 : _a.addEventListener('click', openCreate);
        (_b = document.getElementById('operating-systems-save')) === null || _b === void 0 ? void 0 : _b.addEventListener('click', saveOperatingSystem);
        (_c = document.getElementById('operating-systems-confirm-delete')) === null || _c === void 0 ? void 0 : _c.addEventListener('click', doDelete);
        bindTableActions();
        loadOperatingSystems();
    }
    function setupPageObserver() {
        // Try immediate initialization
        initializeOperatingSystemsPage();
        // Set up MutationObserver for Blazor navigation
        if (document.body) {
            const observer = new MutationObserver(() => {
                const page = document.getElementById('operating-systems-page');
                if (page && page.dataset.initialized !== 'true') {
                    initializeOperatingSystemsPage();
                }
            });
            observer.observe(document.body, { childList: true, subtree: true });
        }
    }
    // Initialize when DOM is ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', setupPageObserver);
    }
    else {
        setupPageObserver();
    }
})();
//# sourceMappingURL=operating-systems.js.map
"use strict";
// @ts-nocheck
(function () {
    function getApiBaseUrl() {
        var _a, _b;
        return (_b = (_a = window.AppSettings) === null || _a === void 0 ? void 0 : _a.apiBaseUrl) !== null && _b !== void 0 ? _b : '';
    }
    let allServerTypes = [];
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
            console.error('Server types request failed', error);
            return {
                success: false,
                message: 'Network error. Please try again.',
            };
        }
    }
    async function loadServerTypes() {
        var _a;
        const tableBody = document.getElementById('server-types-table-body');
        if (!tableBody) {
            return;
        }
        tableBody.innerHTML = '<tr><td colspan="6" class="text-center"><div class="spinner-border text-primary"></div></td></tr>';
        const response = await apiRequest(`${getApiBaseUrl()}/ServerTypes`, { method: 'GET' });
        if (!response.success) {
            showError(response.message || 'Failed to load server types');
            tableBody.innerHTML = '<tr><td colspan="6" class="text-center text-danger">Failed to load data</td></tr>';
            return;
        }
        const rawItems = Array.isArray(response.data)
            ? response.data
            : Array.isArray((_a = response.data) === null || _a === void 0 ? void 0 : _a.data)
                ? response.data.data
                : [];
        allServerTypes = rawItems.map((item) => {
            var _a, _b, _c, _d, _e, _f, _g, _h, _j, _k;
            return ({
                id: (_b = (_a = item.id) !== null && _a !== void 0 ? _a : item.Id) !== null && _b !== void 0 ? _b : 0,
                name: (_d = (_c = item.name) !== null && _c !== void 0 ? _c : item.Name) !== null && _d !== void 0 ? _d : '',
                displayName: (_f = (_e = item.displayName) !== null && _e !== void 0 ? _e : item.DisplayName) !== null && _f !== void 0 ? _f : '',
                description: (_h = (_g = item.description) !== null && _g !== void 0 ? _g : item.Description) !== null && _h !== void 0 ? _h : null,
                isActive: (_k = (_j = item.isActive) !== null && _j !== void 0 ? _j : item.IsActive) !== null && _k !== void 0 ? _k : false,
            });
        });
        renderTable();
    }
    function renderTable() {
        const tableBody = document.getElementById('server-types-table-body');
        if (!tableBody) {
            return;
        }
        if (!allServerTypes.length) {
            tableBody.innerHTML = '<tr><td colspan="6" class="text-center text-muted">No server types found. Click "New Server Type" to add one.</td></tr>';
            return;
        }
        tableBody.innerHTML = allServerTypes.map((type) => `
        <tr>
            <td>${type.id}</td>
            <td><code>${esc(type.name)}</code></td>
            <td>${esc(type.displayName)}</td>
            <td>${esc(type.description || '-')}</td>
            <td><span class="badge bg-${type.isActive ? 'success' : 'secondary'}">${type.isActive ? 'Active' : 'Inactive'}</span></td>
            <td>
                <div class="btn-group btn-group-sm">
                    <button class="btn btn-outline-primary" type="button" data-action="edit" data-id="${type.id}" title="Edit"><i class="bi bi-pencil"></i></button>
                    <button class="btn btn-outline-danger" type="button" data-action="delete" data-id="${type.id}" data-name="${esc(type.displayName)}" title="Delete"><i class="bi bi-trash"></i></button>
                </div>
            </td>
        </tr>
    `).join('');
    }
    function openCreate() {
        editingId = null;
        const modalTitle = document.getElementById('server-types-modal-title');
        if (modalTitle) {
            modalTitle.textContent = 'New Server Type';
        }
        const form = document.getElementById('server-types-form');
        form === null || form === void 0 ? void 0 : form.reset();
        const isActiveInput = document.getElementById('server-types-is-active');
        if (isActiveInput) {
            isActiveInput.checked = true;
        }
        showModal('server-types-edit-modal');
    }
    function openEdit(id) {
        const type = allServerTypes.find((item) => item.id === id);
        if (!type) {
            return;
        }
        editingId = id;
        const modalTitle = document.getElementById('server-types-modal-title');
        if (modalTitle) {
            modalTitle.textContent = 'Edit Server Type';
        }
        const nameInput = document.getElementById('server-types-name');
        const displayNameInput = document.getElementById('server-types-display-name');
        const descriptionInput = document.getElementById('server-types-description');
        const isActiveInput = document.getElementById('server-types-is-active');
        if (nameInput) {
            nameInput.value = type.name;
        }
        if (displayNameInput) {
            displayNameInput.value = type.displayName;
        }
        if (descriptionInput) {
            descriptionInput.value = type.description || '';
        }
        if (isActiveInput) {
            isActiveInput.checked = type.isActive;
        }
        showModal('server-types-edit-modal');
    }
    async function saveServerType() {
        var _a, _b, _c;
        const nameInput = document.getElementById('server-types-name');
        const displayNameInput = document.getElementById('server-types-display-name');
        const descriptionInput = document.getElementById('server-types-description');
        const isActiveInput = document.getElementById('server-types-is-active');
        const name = (_a = nameInput === null || nameInput === void 0 ? void 0 : nameInput.value.trim()) !== null && _a !== void 0 ? _a : '';
        const displayName = (_b = displayNameInput === null || displayNameInput === void 0 ? void 0 : displayNameInput.value.trim()) !== null && _b !== void 0 ? _b : '';
        if (!name || !displayName) {
            showError('Internal Name and Display Name are required');
            return;
        }
        const payload = {
            name,
            displayName,
            description: (descriptionInput === null || descriptionInput === void 0 ? void 0 : descriptionInput.value.trim()) || null,
            isActive: (_c = isActiveInput === null || isActiveInput === void 0 ? void 0 : isActiveInput.checked) !== null && _c !== void 0 ? _c : false,
        };
        const response = editingId
            ? await apiRequest(`${getApiBaseUrl()}/ServerTypes/${editingId}`, { method: 'PUT', body: JSON.stringify(payload) })
            : await apiRequest(`${getApiBaseUrl()}/ServerTypes`, { method: 'POST', body: JSON.stringify(payload) });
        if (response.success) {
            hideModal('server-types-edit-modal');
            showSuccess(editingId ? 'Server type updated successfully' : 'Server type created successfully');
            loadServerTypes();
        }
        else {
            showError(response.message || 'Save failed');
        }
    }
    function openDelete(id, name) {
        pendingDeleteId = id;
        const deleteName = document.getElementById('server-types-delete-name');
        if (deleteName) {
            deleteName.textContent = name;
        }
        showModal('server-types-delete-modal');
    }
    async function doDelete() {
        if (!pendingDeleteId) {
            return;
        }
        const response = await apiRequest(`${getApiBaseUrl()}/ServerTypes/${pendingDeleteId}`, { method: 'DELETE' });
        hideModal('server-types-delete-modal');
        if (response.success) {
            showSuccess('Server type deleted successfully');
            loadServerTypes();
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
        const alert = document.getElementById('server-types-alert-success');
        if (!alert) {
            return;
        }
        alert.textContent = message;
        alert.classList.remove('d-none');
        const errorAlert = document.getElementById('server-types-alert-error');
        errorAlert === null || errorAlert === void 0 ? void 0 : errorAlert.classList.add('d-none');
        setTimeout(() => alert.classList.add('d-none'), 5000);
    }
    function showError(message) {
        const alert = document.getElementById('server-types-alert-error');
        if (!alert) {
            return;
        }
        alert.textContent = message;
        alert.classList.remove('d-none');
        const successAlert = document.getElementById('server-types-alert-success');
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
        const tableBody = document.getElementById('server-types-table-body');
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
    function initializeServerTypesPage() {
        var _a, _b, _c;
        const page = document.getElementById('server-types-page');
        if (!page || page.dataset.initialized === 'true') {
            return;
        }
        page.dataset.initialized = 'true';
        (_a = document.getElementById('server-types-create')) === null || _a === void 0 ? void 0 : _a.addEventListener('click', openCreate);
        (_b = document.getElementById('server-types-save')) === null || _b === void 0 ? void 0 : _b.addEventListener('click', saveServerType);
        (_c = document.getElementById('server-types-confirm-delete')) === null || _c === void 0 ? void 0 : _c.addEventListener('click', doDelete);
        bindTableActions();
        loadServerTypes();
    }
    function setupPageObserver() {
        // Try immediate initialization
        initializeServerTypesPage();
        // Set up MutationObserver for Blazor navigation
        if (document.body) {
            const observer = new MutationObserver(() => {
                const page = document.getElementById('server-types-page');
                if (page && page.dataset.initialized !== 'true') {
                    initializeServerTypesPage();
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
//# sourceMappingURL=server-types.js.map
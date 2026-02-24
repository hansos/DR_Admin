"use strict";
// @ts-nocheck
(function () {
    function getApiBaseUrl() {
        var _a;
        const baseUrl = (_a = window.AppSettings) === null || _a === void 0 ? void 0 : _a.apiBaseUrl;
        if (!baseUrl) {
            const fallback = window.location.protocol === 'https:'
                ? 'https://localhost:7201/api/v1'
                : 'http://localhost:5133/api/v1';
            return fallback;
        }
        return baseUrl;
    }
    let allRoles = [];
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
                data: ((_c = data === null || data === void 0 ? void 0 : data.data) !== null && _c !== void 0 ? _c : data),
                message: data === null || data === void 0 ? void 0 : data.message,
            };
        }
        catch (error) {
            console.error('Roles request failed', error);
            return {
                success: false,
                message: 'Network error. Please try again.',
            };
        }
    }
    async function loadRoles() {
        var _a;
        const tableBody = document.getElementById('roles-table-body');
        if (!tableBody) {
            return;
        }
        tableBody.innerHTML = '<tr><td colspan="6" class="text-center"><div class="spinner-border text-primary"></div></td></tr>';
        const response = await apiRequest(`${getApiBaseUrl()}/Roles`, { method: 'GET' });
        if (!response.success) {
            showError(response.message || 'Failed to load roles');
            tableBody.innerHTML = '<tr><td colspan="6" class="text-center text-danger">Failed to load data</td></tr>';
            return;
        }
        const rawItems = Array.isArray(response.data)
            ? response.data
            : Array.isArray((_a = response.data) === null || _a === void 0 ? void 0 : _a.data)
                ? response.data.data
                : [];
        allRoles = rawItems.map((item) => {
            var _a, _b, _c, _d, _e, _f, _g, _h, _j, _k, _l, _m;
            return ({
                id: (_b = (_a = item.id) !== null && _a !== void 0 ? _a : item.Id) !== null && _b !== void 0 ? _b : 0,
                name: (_d = (_c = item.name) !== null && _c !== void 0 ? _c : item.Name) !== null && _d !== void 0 ? _d : '',
                description: (_f = (_e = item.description) !== null && _e !== void 0 ? _e : item.Description) !== null && _f !== void 0 ? _f : '',
                code: (_h = (_g = item.code) !== null && _g !== void 0 ? _g : item.Code) !== null && _h !== void 0 ? _h : '',
                createdAt: (_k = (_j = item.createdAt) !== null && _j !== void 0 ? _j : item.CreatedAt) !== null && _k !== void 0 ? _k : null,
                updatedAt: (_m = (_l = item.updatedAt) !== null && _l !== void 0 ? _l : item.UpdatedAt) !== null && _m !== void 0 ? _m : null,
            });
        });
        renderTable();
    }
    function renderTable() {
        const tableBody = document.getElementById('roles-table-body');
        if (!tableBody) {
            return;
        }
        if (!allRoles.length) {
            tableBody.innerHTML = '<tr><td colspan="6" class="text-center text-muted">No roles found. Click "New Role" to add one.</td></tr>';
            return;
        }
        tableBody.innerHTML = allRoles.map((role) => {
            const created = role.createdAt ? formatDate(role.createdAt) : '-';
            return `
        <tr>
            <td>${role.id}</td>
            <td><code>${esc(role.code)}</code></td>
            <td>${esc(role.name)}</td>
            <td>${esc(role.description || '-')}</td>
            <td>${created}</td>
            <td>
                <div class="btn-group btn-group-sm">
                    <button class="btn btn-outline-primary" type="button" data-action="edit" data-id="${role.id}" title="Edit"><i class="bi bi-pencil"></i></button>
                    <button class="btn btn-outline-danger" type="button" data-action="delete" data-id="${role.id}" data-name="${esc(role.name)}" title="Delete"><i class="bi bi-trash"></i></button>
                </div>
            </td>
        </tr>`;
        }).join('');
    }
    function openCreate() {
        editingId = null;
        const modalTitle = document.getElementById('roles-modal-title');
        if (modalTitle) {
            modalTitle.textContent = 'New Role';
        }
        const form = document.getElementById('roles-form');
        form === null || form === void 0 ? void 0 : form.reset();
        showModal('roles-edit-modal');
    }
    function openEdit(id) {
        const role = allRoles.find((item) => item.id === id);
        if (!role) {
            return;
        }
        editingId = id;
        const modalTitle = document.getElementById('roles-modal-title');
        if (modalTitle) {
            modalTitle.textContent = 'Edit Role';
        }
        const codeInput = document.getElementById('roles-code');
        const nameInput = document.getElementById('roles-name');
        const descriptionInput = document.getElementById('roles-description');
        if (codeInput) {
            codeInput.value = role.code;
        }
        if (nameInput) {
            nameInput.value = role.name;
        }
        if (descriptionInput) {
            descriptionInput.value = role.description || '';
        }
        showModal('roles-edit-modal');
    }
    async function saveRole() {
        var _a, _b, _c;
        const codeInput = document.getElementById('roles-code');
        const nameInput = document.getElementById('roles-name');
        const descriptionInput = document.getElementById('roles-description');
        const code = (_a = codeInput === null || codeInput === void 0 ? void 0 : codeInput.value.trim()) !== null && _a !== void 0 ? _a : '';
        const name = (_b = nameInput === null || nameInput === void 0 ? void 0 : nameInput.value.trim()) !== null && _b !== void 0 ? _b : '';
        if (!code || !name) {
            showError('Code and Name are required');
            return;
        }
        const payload = {
            code,
            name,
            description: (_c = descriptionInput === null || descriptionInput === void 0 ? void 0 : descriptionInput.value.trim()) !== null && _c !== void 0 ? _c : '',
        };
        const response = editingId
            ? await apiRequest(`${getApiBaseUrl()}/Roles/${editingId}`, { method: 'PUT', body: JSON.stringify(payload) })
            : await apiRequest(`${getApiBaseUrl()}/Roles`, { method: 'POST', body: JSON.stringify(payload) });
        if (response.success) {
            hideModal('roles-edit-modal');
            showSuccess(editingId ? 'Role updated successfully' : 'Role created successfully');
            loadRoles();
        }
        else {
            showError(response.message || 'Save failed');
        }
    }
    function openDelete(id, name) {
        pendingDeleteId = id;
        const deleteName = document.getElementById('roles-delete-name');
        if (deleteName) {
            deleteName.textContent = name;
        }
        showModal('roles-delete-modal');
    }
    async function doDelete() {
        if (!pendingDeleteId) {
            return;
        }
        const response = await apiRequest(`${getApiBaseUrl()}/Roles/${pendingDeleteId}`, { method: 'DELETE' });
        hideModal('roles-delete-modal');
        if (response.success) {
            showSuccess('Role deleted successfully');
            loadRoles();
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
    function formatDate(value) {
        if (!value) {
            return '';
        }
        try {
            const date = new Date(value);
            if (isNaN(date.getTime())) {
                return value;
            }
            return date.toLocaleString();
        }
        catch (_a) {
            return value;
        }
    }
    function showSuccess(message) {
        const alert = document.getElementById('roles-alert-success');
        if (!alert) {
            return;
        }
        alert.textContent = message;
        alert.classList.remove('d-none');
        const errorAlert = document.getElementById('roles-alert-error');
        errorAlert === null || errorAlert === void 0 ? void 0 : errorAlert.classList.add('d-none');
        setTimeout(() => alert.classList.add('d-none'), 5000);
    }
    function showError(message) {
        const alert = document.getElementById('roles-alert-error');
        if (!alert) {
            return;
        }
        alert.textContent = message;
        alert.classList.remove('d-none');
        const successAlert = document.getElementById('roles-alert-success');
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
        const tableBody = document.getElementById('roles-table-body');
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
    function initializeRolesPage() {
        var _a, _b, _c;
        const page = document.getElementById('roles-page');
        if (!page || page.getAttribute('data-initialized') === 'true') {
            return;
        }
        page.setAttribute('data-initialized', 'true');
        (_a = document.getElementById('roles-create')) === null || _a === void 0 ? void 0 : _a.addEventListener('click', openCreate);
        (_b = document.getElementById('roles-save')) === null || _b === void 0 ? void 0 : _b.addEventListener('click', () => { saveRole(); });
        (_c = document.getElementById('roles-confirm-delete')) === null || _c === void 0 ? void 0 : _c.addEventListener('click', () => { doDelete(); });
        bindTableActions();
        loadRoles();
    }
    function setupPageObserver() {
        initializeRolesPage();
        if (document.body) {
            const observer = new MutationObserver(() => {
                const page = document.getElementById('roles-page');
                if (page && page.getAttribute('data-initialized') !== 'true') {
                    initializeRolesPage();
                }
            });
            observer.observe(document.body, { childList: true, subtree: true });
        }
    }
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', setupPageObserver);
    }
    else {
        setupPageObserver();
    }
})();
//# sourceMappingURL=roles.js.map
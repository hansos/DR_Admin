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
    let allUsers = [];
    let editingId = null;
    let pendingDeleteId = null;
    let availableRoles = [];
    let rolesLoaded = false;
    let rolesEditingUser = null;
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
            console.error('Users request failed', error);
            return {
                success: false,
                message: 'Network error. Please try again.',
            };
        }
    }
    async function loadUsers() {
        const tableBody = document.getElementById('users-table-body');
        if (!tableBody) {
            return;
        }
        tableBody.innerHTML = '<tr><td colspan="7" class="text-center"><div class="spinner-border text-primary"></div></td></tr>';
        const response = await apiRequest(`${getApiBaseUrl()}/Users`, { method: 'GET' });
        if (!response.success) {
            showError(response.message || 'Failed to load users');
            tableBody.innerHTML = '<tr><td colspan="7" class="text-center text-danger">Failed to load data</td></tr>';
            return;
        }
        const raw = response.data;
        const rawItems = Array.isArray(raw)
            ? raw
            : Array.isArray(raw === null || raw === void 0 ? void 0 : raw.items)
                ? raw.items
                : Array.isArray(raw === null || raw === void 0 ? void 0 : raw.data)
                    ? raw.data
                    : [];
        allUsers = rawItems.map((item) => {
            var _a, _b, _c, _d, _e, _f, _g, _h, _j, _k, _l, _m, _o, _p, _q, _r;
            return ({
                id: (_b = (_a = item.id) !== null && _a !== void 0 ? _a : item.Id) !== null && _b !== void 0 ? _b : 0,
                customerId: (_d = (_c = item.customerId) !== null && _c !== void 0 ? _c : item.CustomerId) !== null && _d !== void 0 ? _d : null,
                username: (_f = (_e = item.username) !== null && _e !== void 0 ? _e : item.Username) !== null && _f !== void 0 ? _f : '',
                email: (_h = (_g = item.email) !== null && _g !== void 0 ? _g : item.Email) !== null && _h !== void 0 ? _h : '',
                isActive: (_k = (_j = item.isActive) !== null && _j !== void 0 ? _j : item.IsActive) !== null && _k !== void 0 ? _k : false,
                roles: (_m = (_l = item.roles) !== null && _l !== void 0 ? _l : item.Roles) !== null && _m !== void 0 ? _m : [],
                createdAt: (_p = (_o = item.createdAt) !== null && _o !== void 0 ? _o : item.CreatedAt) !== null && _p !== void 0 ? _p : null,
                updatedAt: (_r = (_q = item.updatedAt) !== null && _q !== void 0 ? _q : item.UpdatedAt) !== null && _r !== void 0 ? _r : null,
            });
        });
        renderTable();
    }
    function renderTable() {
        const tableBody = document.getElementById('users-table-body');
        if (!tableBody) {
            return;
        }
        if (!allUsers.length) {
            tableBody.innerHTML = '<tr><td colspan="7" class="text-center text-muted">No users found. Click "New User" to add one.</td></tr>';
            return;
        }
        tableBody.innerHTML = allUsers.map((user) => {
            const rolesText = (user.roles && user.roles.length)
                ? user.roles.map((r) => `<span class="badge bg-secondary me-1">${esc(r)}</span>`).join('')
                : '<span class="text-muted">None</span>';
            const created = user.createdAt ? formatDate(user.createdAt) : '-';
            return `
        <tr>
            <td>${user.id}</td>
            <td>${esc(user.username)}</td>
            <td>${esc(user.email)}</td>
            <td>${rolesText}</td>
            <td><span class="badge bg-${user.isActive ? 'success' : 'secondary'}">${user.isActive ? 'Active' : 'Inactive'}</span></td>
            <td>${created}</td>
            <td>
                <div class="btn-group btn-group-sm">
                    <button class="btn btn-outline-primary" type="button" data-action="edit" data-id="${user.id}" title="Edit"><i class="bi bi-pencil"></i></button>
                    <button class="btn btn-outline-secondary" type="button" data-action="roles" data-id="${user.id}" title="Manage Roles"><i class="bi bi-shield-lock"></i></button>
                    <button class="btn btn-outline-danger" type="button" data-action="delete" data-id="${user.id}" data-name="${esc(user.username)}" title="Delete"><i class="bi bi-trash"></i></button>
                </div>
            </td>
        </tr>`;
        }).join('');
    }
    function hideRolesError() {
        const alert = document.getElementById('users-roles-alert-error');
        if (!alert) {
            return;
        }
        alert.textContent = '';
        alert.classList.add('d-none');
    }
    function showRolesError(message) {
        const alert = document.getElementById('users-roles-alert-error');
        if (!alert) {
            return;
        }
        alert.textContent = message;
        alert.classList.remove('d-none');
    }
    async function loadAvailableRoles() {
        var _a;
        if (rolesLoaded) {
            return;
        }
        const response = await apiRequest(`${getApiBaseUrl()}/Roles`, { method: 'GET' });
        if (!response.success) {
            showRolesError(response.message || 'Failed to load roles');
            rolesLoaded = true;
            availableRoles = [];
            return;
        }
        const rawItems = Array.isArray(response.data)
            ? response.data
            : Array.isArray((_a = response.data) === null || _a === void 0 ? void 0 : _a.data)
                ? response.data.data
                : [];
        availableRoles = rawItems.map((item) => {
            var _a, _b, _c, _d, _e, _f, _g, _h;
            return ({
                id: (_b = (_a = item.id) !== null && _a !== void 0 ? _a : item.Id) !== null && _b !== void 0 ? _b : 0,
                name: (_d = (_c = item.name) !== null && _c !== void 0 ? _c : item.Name) !== null && _d !== void 0 ? _d : '',
                description: (_f = (_e = item.description) !== null && _e !== void 0 ? _e : item.Description) !== null && _f !== void 0 ? _f : '',
                code: (_h = (_g = item.code) !== null && _g !== void 0 ? _g : item.Code) !== null && _h !== void 0 ? _h : '',
            });
        }).filter((r) => !!r.name);
        rolesLoaded = true;
    }
    function renderRolesList(selectedRoles) {
        const list = document.getElementById('users-roles-list');
        if (!list) {
            return;
        }
        if (!availableRoles.length) {
            list.innerHTML = '<div class="text-muted">No roles found.</div>';
            return;
        }
        const selectedSet = new Set((selectedRoles || []).map((r) => (r || '').toLowerCase()));
        list.innerHTML = availableRoles.map((role) => {
            const id = `users-roles-${role.id}`;
            const checked = selectedSet.has((role.name || '').toLowerCase()) ? 'checked' : '';
            const help = role.description ? `<div class="form-text">${esc(role.description)}</div>` : '';
            return `
            <div class="form-check">
                <input class="form-check-input" type="checkbox" id="${id}" data-role-name="${esc(role.name)}" ${checked} />
                <label class="form-check-label" for="${id}">${esc(role.name)}${role.code ? ` <span class=\"text-muted\">(${esc(role.code)})</span>` : ''}</label>
                ${help}
            </div>
        `;
        }).join('');
    }
    async function openManageRoles(id) {
        const user = allUsers.find((item) => item.id === id);
        if (!user) {
            return;
        }
        rolesEditingUser = user;
        hideRolesError();
        const title = document.getElementById('users-roles-modal-title');
        if (title) {
            title.textContent = `Manage Roles: ${user.username}`;
        }
        const list = document.getElementById('users-roles-list');
        if (list) {
            list.innerHTML = '<div class="text-center"><div class="spinner-border text-primary"></div></div>';
        }
        showModal('users-roles-modal');
        await loadAvailableRoles();
        renderRolesList(user.roles || []);
    }
    async function saveUserRoles() {
        var _a;
        if (!rolesEditingUser) {
            return;
        }
        hideRolesError();
        const inputs = Array.from(document.querySelectorAll('#users-roles-list input[type="checkbox"][data-role-name]'));
        const roles = inputs
            .filter((input) => input.checked)
            .map((input) => { var _a; return ((_a = input.getAttribute('data-role-name')) !== null && _a !== void 0 ? _a : '').trim(); })
            .filter((name) => !!name);
        const payload = {
            customerId: (_a = rolesEditingUser.customerId) !== null && _a !== void 0 ? _a : null,
            username: rolesEditingUser.username,
            email: rolesEditingUser.email,
            isActive: rolesEditingUser.isActive,
            roles,
        };
        const response = await apiRequest(`${getApiBaseUrl()}/Users/${rolesEditingUser.id}`, {
            method: 'PUT',
            body: JSON.stringify(payload),
        });
        if (response.success) {
            hideModal('users-roles-modal');
            showSuccess('User roles updated successfully');
            rolesEditingUser = null;
            loadUsers();
        }
        else {
            showRolesError(response.message || 'Failed to save roles');
        }
    }
    function openCreate() {
        editingId = null;
        const modalTitle = document.getElementById('users-modal-title');
        if (modalTitle) {
            modalTitle.textContent = 'New User';
        }
        const form = document.getElementById('users-form');
        form === null || form === void 0 ? void 0 : form.reset();
        const isActiveInput = document.getElementById('users-is-active');
        if (isActiveInput) {
            isActiveInput.checked = true;
        }
        const passwordGroup = document.getElementById('users-password-group');
        if (passwordGroup) {
            passwordGroup.classList.remove('d-none');
        }
        showModal('users-edit-modal');
    }
    function openEdit(id) {
        const user = allUsers.find((item) => item.id === id);
        if (!user) {
            return;
        }
        editingId = id;
        const modalTitle = document.getElementById('users-modal-title');
        if (modalTitle) {
            modalTitle.textContent = 'Edit User';
        }
        const usernameInput = document.getElementById('users-username');
        const emailInput = document.getElementById('users-email');
        const passwordInput = document.getElementById('users-password');
        const roleInput = document.getElementById('users-role');
        const customerInput = document.getElementById('users-customer-id');
        const isActiveInput = document.getElementById('users-is-active');
        if (usernameInput) {
            usernameInput.value = user.username;
        }
        if (emailInput) {
            emailInput.value = user.email;
        }
        if (passwordInput) {
            passwordInput.value = '';
        }
        if (roleInput) {
            roleInput.value = user.roles && user.roles.length ? user.roles[0] : '';
        }
        if (customerInput) {
            customerInput.value = user.customerId != null ? String(user.customerId) : '';
        }
        if (isActiveInput) {
            isActiveInput.checked = user.isActive;
        }
        const passwordGroup = document.getElementById('users-password-group');
        if (passwordGroup) {
            passwordGroup.classList.add('d-none');
        }
        showModal('users-edit-modal');
    }
    async function saveUser() {
        var _a, _b, _c, _d, _e, _f;
        const usernameInput = document.getElementById('users-username');
        const emailInput = document.getElementById('users-email');
        const passwordInput = document.getElementById('users-password');
        const roleInput = document.getElementById('users-role');
        const customerInput = document.getElementById('users-customer-id');
        const isActiveInput = document.getElementById('users-is-active');
        const username = (_a = usernameInput === null || usernameInput === void 0 ? void 0 : usernameInput.value.trim()) !== null && _a !== void 0 ? _a : '';
        const email = (_b = emailInput === null || emailInput === void 0 ? void 0 : emailInput.value.trim()) !== null && _b !== void 0 ? _b : '';
        const password = (_c = passwordInput === null || passwordInput === void 0 ? void 0 : passwordInput.value.trim()) !== null && _c !== void 0 ? _c : '';
        const role = (_d = roleInput === null || roleInput === void 0 ? void 0 : roleInput.value.trim()) !== null && _d !== void 0 ? _d : '';
        const customerIdValue = (_e = customerInput === null || customerInput === void 0 ? void 0 : customerInput.value.trim()) !== null && _e !== void 0 ? _e : '';
        const isActive = (_f = isActiveInput === null || isActiveInput === void 0 ? void 0 : isActiveInput.checked) !== null && _f !== void 0 ? _f : false;
        if (!username) {
            showError('Username is required');
            return;
        }
        if (!email) {
            showError('Email is required');
            return;
        }
        if (!editingId && !password) {
            showError('Password is required when creating a new user');
            return;
        }
        let customerId = null;
        if (customerIdValue) {
            const parsed = Number(customerIdValue);
            if (!isNaN(parsed) && parsed >= 0) {
                customerId = parsed;
            }
        }
        let response;
        if (editingId) {
            const payload = {
                customerId,
                username,
                email,
                isActive,
            };
            if (role) {
                payload.roles = [role];
            }
            response = await apiRequest(`${getApiBaseUrl()}/Users/${editingId}`, {
                method: 'PUT',
                body: JSON.stringify(payload),
            });
        }
        else {
            const payload = {
                customerId,
                username,
                password,
                email,
                isActive,
            };
            if (role) {
                payload.roles = [role];
            }
            response = await apiRequest(`${getApiBaseUrl()}/Users`, {
                method: 'POST',
                body: JSON.stringify(payload),
            });
        }
        if (response.success) {
            hideModal('users-edit-modal');
            showSuccess(editingId ? 'User updated successfully' : 'User created successfully');
            loadUsers();
        }
        else {
            showError(response.message || 'Save failed');
        }
    }
    function openDelete(id, name) {
        pendingDeleteId = id;
        const deleteName = document.getElementById('users-delete-name');
        if (deleteName) {
            deleteName.textContent = name;
        }
        showModal('users-delete-modal');
    }
    async function doDelete() {
        if (!pendingDeleteId) {
            return;
        }
        const response = await apiRequest(`${getApiBaseUrl()}/Users/${pendingDeleteId}`, { method: 'DELETE' });
        hideModal('users-delete-modal');
        if (response.success) {
            showSuccess('User deleted successfully');
            loadUsers();
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
        const alert = document.getElementById('users-alert-success');
        if (!alert) {
            return;
        }
        alert.textContent = message;
        alert.classList.remove('d-none');
        const errorAlert = document.getElementById('users-alert-error');
        errorAlert === null || errorAlert === void 0 ? void 0 : errorAlert.classList.add('d-none');
        setTimeout(() => alert.classList.add('d-none'), 5000);
    }
    function showError(message) {
        const alert = document.getElementById('users-alert-error');
        if (!alert) {
            return;
        }
        alert.textContent = message;
        alert.classList.remove('d-none');
        const successAlert = document.getElementById('users-alert-success');
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
        const tableBody = document.getElementById('users-table-body');
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
            if (button.dataset.action === 'roles') {
                openManageRoles(id);
                return;
            }
            if (button.dataset.action === 'delete') {
                openDelete(id, (_a = button.dataset.name) !== null && _a !== void 0 ? _a : '');
            }
        });
    }
    function initializeUsersPage() {
        var _a, _b, _c, _d;
        const page = document.getElementById('users-page');
        if (!page || page.getAttribute('data-initialized') === 'true') {
            return;
        }
        page.setAttribute('data-initialized', 'true');
        (_a = document.getElementById('users-create')) === null || _a === void 0 ? void 0 : _a.addEventListener('click', openCreate);
        (_b = document.getElementById('users-save')) === null || _b === void 0 ? void 0 : _b.addEventListener('click', () => { saveUser(); });
        (_c = document.getElementById('users-confirm-delete')) === null || _c === void 0 ? void 0 : _c.addEventListener('click', () => { doDelete(); });
        (_d = document.getElementById('users-roles-save')) === null || _d === void 0 ? void 0 : _d.addEventListener('click', () => { saveUserRoles(); });
        bindTableActions();
        loadUsers();
    }
    function setupPageObserver() {
        // Try immediate initialization
        initializeUsersPage();
        // Set up MutationObserver for Blazor navigation
        if (document.body) {
            const observer = new MutationObserver(() => {
                const page = document.getElementById('users-page');
                if (page && page.getAttribute('data-initialized') !== 'true') {
                    initializeUsersPage();
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
//# sourceMappingURL=users.js.map
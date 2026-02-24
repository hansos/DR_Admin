"use strict";
// @ts-nocheck
(function () {
    function getApiBaseUrl() {
        var _a, _b;
        return (_b = (_a = window.AppSettings) === null || _a === void 0 ? void 0 : _a.apiBaseUrl) !== null && _b !== void 0 ? _b : '';
    }
    let allServerIpAddresses = [];
    let servers = [];
    const serverNameLookup = {};
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
            console.error('Server IP addresses request failed', error);
            return {
                success: false,
                message: 'Network error. Please try again.',
            };
        }
    }
    async function loadServers() {
        var _a;
        const response = await apiRequest(`${getApiBaseUrl()}/Servers`, { method: 'GET' });
        const rawItems = Array.isArray(response.data)
            ? response.data
            : Array.isArray((_a = response.data) === null || _a === void 0 ? void 0 : _a.data)
                ? response.data.data
                : [];
        servers = rawItems.map((item) => {
            var _a, _b, _c, _d;
            return ({
                id: (_b = (_a = item.id) !== null && _a !== void 0 ? _a : item.Id) !== null && _b !== void 0 ? _b : 0,
                name: (_d = (_c = item.name) !== null && _c !== void 0 ? _c : item.Name) !== null && _d !== void 0 ? _d : '',
            });
        });
        Object.keys(serverNameLookup).forEach((key) => delete serverNameLookup[Number(key)]);
        servers.forEach((srv) => {
            if (srv.id) {
                serverNameLookup[srv.id] = srv.name;
            }
        });
        populateServerDropdown();
    }
    function populateServerDropdown() {
        const serverSelect = document.getElementById('server-ip-addresses-server-id');
        if (serverSelect) {
            const selected = serverSelect.value;
            serverSelect.innerHTML = '<option value="">Select Server...</option>' +
                servers.map(s => `<option value="${s.id}">${esc(s.name)}</option>`).join('');
            if (selected) {
                serverSelect.value = selected;
            }
        }
    }
    async function loadServerIpAddresses() {
        var _a;
        const tableBody = document.getElementById('server-ip-addresses-table-body');
        if (!tableBody) {
            return;
        }
        tableBody.innerHTML = '<tr><td colspan="8" class="text-center"><div class="spinner-border text-primary"></div></td></tr>';
        const response = await apiRequest(`${getApiBaseUrl()}/ServerIpAddresses`, { method: 'GET' });
        if (!response.success) {
            showError(response.message || 'Failed to load IP addresses');
            tableBody.innerHTML = '<tr><td colspan="8" class="text-center text-danger">Failed to load data</td></tr>';
            return;
        }
        const rawItems = Array.isArray(response.data)
            ? response.data
            : Array.isArray((_a = response.data) === null || _a === void 0 ? void 0 : _a.data)
                ? response.data.data
                : [];
        allServerIpAddresses = rawItems.map((item) => {
            var _a, _b, _c, _d, _e, _f, _g, _h, _j, _k, _l, _m, _o, _p, _q, _r, _s, _t, _u;
            const serverId = (_b = (_a = item.serverId) !== null && _a !== void 0 ? _a : item.ServerId) !== null && _b !== void 0 ? _b : 0;
            return {
                id: (_d = (_c = item.id) !== null && _c !== void 0 ? _c : item.Id) !== null && _d !== void 0 ? _d : 0,
                serverId,
                serverName: (_g = (_f = (_e = item.serverName) !== null && _e !== void 0 ? _e : item.ServerName) !== null && _f !== void 0 ? _f : serverNameLookup[serverId]) !== null && _g !== void 0 ? _g : null,
                ipAddress: (_j = (_h = item.ipAddress) !== null && _h !== void 0 ? _h : item.IpAddress) !== null && _j !== void 0 ? _j : '',
                ipVersion: (_l = (_k = item.ipVersion) !== null && _k !== void 0 ? _k : item.IpVersion) !== null && _l !== void 0 ? _l : 'IPv4',
                isPrimary: (_o = (_m = item.isPrimary) !== null && _m !== void 0 ? _m : item.IsPrimary) !== null && _o !== void 0 ? _o : false,
                status: (_q = (_p = item.status) !== null && _p !== void 0 ? _p : item.Status) !== null && _q !== void 0 ? _q : 'Active',
                assignedTo: (_s = (_r = item.assignedTo) !== null && _r !== void 0 ? _r : item.AssignedTo) !== null && _s !== void 0 ? _s : null,
                notes: (_u = (_t = item.notes) !== null && _t !== void 0 ? _t : item.Notes) !== null && _u !== void 0 ? _u : null,
            };
        });
        renderTable();
    }
    function renderTable() {
        const tableBody = document.getElementById('server-ip-addresses-table-body');
        if (!tableBody) {
            return;
        }
        if (!allServerIpAddresses.length) {
            tableBody.innerHTML = '<tr><td colspan="8" class="text-center text-muted">No IP addresses found. Click "New IP Address" to add one.</td></tr>';
            return;
        }
        tableBody.innerHTML = allServerIpAddresses.map((ip) => `
        <tr>
            <td>${ip.id}</td>
            <td><code>${esc(ip.ipAddress)}</code></td>
            <td><span class="badge bg-${ip.ipVersion === 'IPv6' ? 'info' : 'secondary'}">${esc(ip.ipVersion)}</span></td>
            <td>${esc(ip.serverName || '-')}</td>
            <td>${ip.isPrimary ? '<span class="badge bg-primary">Primary</span>' : '-'}</td>
            <td><span class="badge bg-${getStatusBadgeColor(ip.status)}">${esc(ip.status)}</span></td>
            <td>${esc(ip.assignedTo || '-')}</td>
            <td>
                <div class="btn-group btn-group-sm">
                    <button class="btn btn-outline-primary" type="button" data-action="edit" data-id="${ip.id}" title="Edit"><i class="bi bi-pencil"></i></button>
                    <button class="btn btn-outline-danger" type="button" data-action="delete" data-id="${ip.id}" data-name="${esc(ip.ipAddress)}" title="Delete"><i class="bi bi-trash"></i></button>
                </div>
            </td>
        </tr>
    `).join('');
    }
    function getStatusBadgeColor(status) {
        switch (status === null || status === void 0 ? void 0 : status.toLowerCase()) {
            case 'active': return 'success';
            case 'reserved': return 'warning';
            case 'blocked': return 'danger';
            default: return 'secondary';
        }
    }
    function openCreate() {
        editingId = null;
        const modalTitle = document.getElementById('server-ip-addresses-modal-title');
        if (modalTitle) {
            modalTitle.textContent = 'New IP Address';
        }
        const form = document.getElementById('server-ip-addresses-form');
        form === null || form === void 0 ? void 0 : form.reset();
        const ipVersionSelect = document.getElementById('server-ip-addresses-ip-version');
        if (ipVersionSelect) {
            ipVersionSelect.value = 'IPv4';
        }
        const statusSelect = document.getElementById('server-ip-addresses-status');
        if (statusSelect) {
            statusSelect.value = 'Active';
        }
        const isPrimaryInput = document.getElementById('server-ip-addresses-is-primary');
        if (isPrimaryInput) {
            isPrimaryInput.checked = false;
        }
        populateServerDropdown();
        showModal('server-ip-addresses-edit-modal');
    }
    function openEdit(id) {
        const ip = allServerIpAddresses.find((item) => item.id === id);
        if (!ip) {
            return;
        }
        editingId = id;
        const modalTitle = document.getElementById('server-ip-addresses-modal-title');
        if (modalTitle) {
            modalTitle.textContent = 'Edit IP Address';
        }
        const ipAddressInput = document.getElementById('server-ip-addresses-ip-address');
        const ipVersionInput = document.getElementById('server-ip-addresses-ip-version');
        const serverIdInput = document.getElementById('server-ip-addresses-server-id');
        const statusInput = document.getElementById('server-ip-addresses-status');
        const isPrimaryInput = document.getElementById('server-ip-addresses-is-primary');
        const assignedToInput = document.getElementById('server-ip-addresses-assigned-to');
        const notesInput = document.getElementById('server-ip-addresses-notes');
        if (ipAddressInput) {
            ipAddressInput.value = ip.ipAddress;
        }
        if (ipVersionInput) {
            ipVersionInput.value = ip.ipVersion;
        }
        populateServerDropdown();
        if (serverIdInput) {
            serverIdInput.value = String(ip.serverId);
        }
        if (statusInput) {
            statusInput.value = ip.status;
        }
        if (isPrimaryInput) {
            isPrimaryInput.checked = ip.isPrimary;
        }
        if (assignedToInput) {
            assignedToInput.value = ip.assignedTo || '';
        }
        if (notesInput) {
            notesInput.value = ip.notes || '';
        }
        showModal('server-ip-addresses-edit-modal');
    }
    async function saveServerIpAddress() {
        var _a, _b, _c, _d, _e, _f;
        const ipAddressInput = document.getElementById('server-ip-addresses-ip-address');
        const ipVersionInput = document.getElementById('server-ip-addresses-ip-version');
        const serverIdInput = document.getElementById('server-ip-addresses-server-id');
        const statusInput = document.getElementById('server-ip-addresses-status');
        const isPrimaryInput = document.getElementById('server-ip-addresses-is-primary');
        const assignedToInput = document.getElementById('server-ip-addresses-assigned-to');
        const notesInput = document.getElementById('server-ip-addresses-notes');
        const ipAddress = (_a = ipAddressInput === null || ipAddressInput === void 0 ? void 0 : ipAddressInput.value.trim()) !== null && _a !== void 0 ? _a : '';
        const ipVersion = (_c = (_b = ipVersionInput === null || ipVersionInput === void 0 ? void 0 : ipVersionInput.value) === null || _b === void 0 ? void 0 : _b.trim()) !== null && _c !== void 0 ? _c : '';
        const serverId = (serverIdInput === null || serverIdInput === void 0 ? void 0 : serverIdInput.value) ? Number(serverIdInput.value) : null;
        const status = (_e = (_d = statusInput === null || statusInput === void 0 ? void 0 : statusInput.value) === null || _d === void 0 ? void 0 : _d.trim()) !== null && _e !== void 0 ? _e : 'Active';
        if (!ipAddress || !ipVersion || !serverId || !status) {
            showError('IP Address, IP Version, Server, and Status are required');
            return;
        }
        const payload = {
            serverId,
            ipAddress,
            ipVersion,
            status,
            isPrimary: (_f = isPrimaryInput === null || isPrimaryInput === void 0 ? void 0 : isPrimaryInput.checked) !== null && _f !== void 0 ? _f : false,
            assignedTo: (assignedToInput === null || assignedToInput === void 0 ? void 0 : assignedToInput.value.trim()) || null,
            notes: (notesInput === null || notesInput === void 0 ? void 0 : notesInput.value.trim()) || null,
        };
        const response = editingId
            ? await apiRequest(`${getApiBaseUrl()}/ServerIpAddresses/${editingId}`, { method: 'PUT', body: JSON.stringify(payload) })
            : await apiRequest(`${getApiBaseUrl()}/ServerIpAddresses`, { method: 'POST', body: JSON.stringify(payload) });
        if (response.success) {
            hideModal('server-ip-addresses-edit-modal');
            showSuccess(editingId ? 'IP address updated successfully' : 'IP address created successfully');
            loadServerIpAddresses();
        }
        else {
            showError(response.message || 'Save failed');
        }
    }
    function openDelete(id, name) {
        pendingDeleteId = id;
        const deleteName = document.getElementById('server-ip-addresses-delete-name');
        if (deleteName) {
            deleteName.textContent = name;
        }
        showModal('server-ip-addresses-delete-modal');
    }
    async function doDelete() {
        if (!pendingDeleteId) {
            return;
        }
        const response = await apiRequest(`${getApiBaseUrl()}/ServerIpAddresses/${pendingDeleteId}`, { method: 'DELETE' });
        hideModal('server-ip-addresses-delete-modal');
        if (response.success) {
            showSuccess('IP address deleted successfully');
            loadServerIpAddresses();
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
        const alert = document.getElementById('server-ip-addresses-alert-success');
        if (!alert) {
            return;
        }
        alert.textContent = message;
        alert.classList.remove('d-none');
        const errorAlert = document.getElementById('server-ip-addresses-alert-error');
        errorAlert === null || errorAlert === void 0 ? void 0 : errorAlert.classList.add('d-none');
        setTimeout(() => alert.classList.add('d-none'), 5000);
    }
    function showError(message) {
        const alert = document.getElementById('server-ip-addresses-alert-error');
        if (!alert) {
            return;
        }
        alert.textContent = message;
        alert.classList.remove('d-none');
        const successAlert = document.getElementById('server-ip-addresses-alert-success');
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
        const tableBody = document.getElementById('server-ip-addresses-table-body');
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
    function initializeServerIpAddressesPage() {
        var _a, _b, _c;
        const page = document.getElementById('server-ip-addresses-page');
        if (!page || page.dataset.initialized === 'true') {
            return;
        }
        page.dataset.initialized = 'true';
        (_a = document.getElementById('server-ip-addresses-create')) === null || _a === void 0 ? void 0 : _a.addEventListener('click', openCreate);
        (_b = document.getElementById('server-ip-addresses-save')) === null || _b === void 0 ? void 0 : _b.addEventListener('click', saveServerIpAddress);
        (_c = document.getElementById('server-ip-addresses-confirm-delete')) === null || _c === void 0 ? void 0 : _c.addEventListener('click', doDelete);
        bindTableActions();
        loadServers()
            .catch((error) => {
            console.error('Failed to load servers for IP addresses', error);
            showError('Failed to load servers. Some labels may be missing.');
        })
            .finally(() => {
            loadServerIpAddresses();
        });
    }
    function setupPageObserver() {
        // Try immediate initialization
        initializeServerIpAddressesPage();
        // Set up MutationObserver for Blazor navigation
        if (document.body) {
            const observer = new MutationObserver(() => {
                const page = document.getElementById('server-ip-addresses-page');
                if (page && page.dataset.initialized !== 'true') {
                    initializeServerIpAddressesPage();
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
//# sourceMappingURL=server-ip-addresses.js.map
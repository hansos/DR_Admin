"use strict";
// @ts-nocheck
(function () {
    function getApiBaseUrl() {
        var _a, _b;
        return (_b = (_a = window.AppSettings) === null || _a === void 0 ? void 0 : _a.apiBaseUrl) !== null && _b !== void 0 ? _b : '';
    }
    let allServers = [];
    let serverTypes = [];
    let operatingSystems = [];
    let hostProviders = [];
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
            console.error('Servers request failed', error);
            return {
                success: false,
                message: 'Network error. Please try again.',
            };
        }
    }
    async function loadLookupData() {
        const [serverTypesRes, osRes, providersRes] = await Promise.all([
            apiRequest(`${getApiBaseUrl()}/ServerTypes`, { method: 'GET' }),
            apiRequest(`${getApiBaseUrl()}/OperatingSystems`, { method: 'GET' }),
            apiRequest(`${getApiBaseUrl()}/HostProviders`, { method: 'GET' }),
        ]);
        serverTypes = extractArray(serverTypesRes.data);
        operatingSystems = extractArray(osRes.data);
        hostProviders = extractArray(providersRes.data);
        populateDropdowns();
    }
    function extractArray(data) {
        const rawItems = Array.isArray(data)
            ? data
            : Array.isArray(data === null || data === void 0 ? void 0 : data.data)
                ? data.data
                : [];
        return rawItems.map((item) => {
            var _a, _b, _c, _d, _e, _f, _g, _h;
            return ({
                id: (_b = (_a = item.id) !== null && _a !== void 0 ? _a : item.Id) !== null && _b !== void 0 ? _b : 0,
                name: (_d = (_c = item.name) !== null && _c !== void 0 ? _c : item.Name) !== null && _d !== void 0 ? _d : '',
                displayName: (_h = (_g = (_f = (_e = item.displayName) !== null && _e !== void 0 ? _e : item.DisplayName) !== null && _f !== void 0 ? _f : item.name) !== null && _g !== void 0 ? _g : item.Name) !== null && _h !== void 0 ? _h : '',
            });
        });
    }
    function populateDropdowns() {
        const serverTypeSelect = document.getElementById('servers-server-type-id');
        if (serverTypeSelect) {
            const selected = serverTypeSelect.value;
            serverTypeSelect.innerHTML = '<option value="">Select Server Type...</option>' +
                serverTypes.map(t => `<option value="${t.id}">${esc(t.displayName || t.name || '')}</option>`).join('');
            if (selected) {
                serverTypeSelect.value = selected;
            }
        }
        const osSelect = document.getElementById('servers-operating-system-id');
        if (osSelect) {
            const selected = osSelect.value;
            osSelect.innerHTML = '<option value="">Select OS...</option>' +
                operatingSystems.map(os => `<option value="${os.id}">${esc(os.displayName || os.name || '')}</option>`).join('');
            if (selected) {
                osSelect.value = selected;
            }
        }
        const providerSelect = document.getElementById('servers-host-provider-id');
        if (providerSelect) {
            const selected = providerSelect.value;
            providerSelect.innerHTML = '<option value="">Select Provider...</option>' +
                hostProviders.map(p => `<option value="${p.id}">${esc(p.displayName || p.name || '')}</option>`).join('');
            if (selected) {
                providerSelect.value = selected;
            }
        }
    }
    function normalizeServerArray(data) {
        if (Array.isArray(data)) {
            return data;
        }
        if (Array.isArray(data === null || data === void 0 ? void 0 : data.data)) {
            return data.data;
        }
        return [];
    }
    async function loadServers() {
        const tableBody = document.getElementById('servers-table-body');
        if (!tableBody) {
            return;
        }
        tableBody.innerHTML = '<tr><td colspan="8" class="text-center"><div class="spinner-border text-primary"></div></td></tr>';
        const response = await apiRequest(`${getApiBaseUrl()}/Servers`, { method: 'GET' });
        if (!response.success) {
            showError(response.message || 'Failed to load servers');
            tableBody.innerHTML = '<tr><td colspan="8" class="text-center text-danger">Failed to load data</td></tr>';
            return;
        }
        // Debug: Log the raw response structure
        console.log('API Response structure:', response);
        console.log('Response.data type:', Array.isArray(response.data) ? 'Array' : typeof response.data);
        console.log('Response.data:', response.data);
        const rawItems = normalizeServerArray(response.data);
        console.log('Extracted array length:', rawItems.length);
        if (rawItems.length > 0) {
            console.log('First raw item from API:', rawItems[0]);
        }
        allServers = rawItems.map((item) => {
            var _a, _b, _c, _d, _e, _f, _g, _h, _j, _k, _l, _m, _o, _p, _q, _r, _s, _t, _u, _v, _w, _x, _y, _z, _0, _1, _2, _3, _4;
            const rawStatus = (_a = item.status) !== null && _a !== void 0 ? _a : item.Status;
            let statusValue = null;
            if (typeof rawStatus === 'boolean') {
                statusValue = rawStatus;
            }
            else if (typeof rawStatus === 'string') {
                statusValue = rawStatus.toLowerCase() === 'active' ? true : rawStatus.toLowerCase() === 'inactive' ? false : null;
            }
            const server = {
                id: (_c = (_b = item.id) !== null && _b !== void 0 ? _b : item.Id) !== null && _c !== void 0 ? _c : 0,
                name: (_e = (_d = item.name) !== null && _d !== void 0 ? _d : item.Name) !== null && _e !== void 0 ? _e : '',
                location: (_g = (_f = item.location) !== null && _f !== void 0 ? _f : item.Location) !== null && _g !== void 0 ? _g : null,
                serverTypeId: (_j = (_h = item.serverTypeId) !== null && _h !== void 0 ? _h : item.ServerTypeId) !== null && _j !== void 0 ? _j : 0,
                serverTypeName: (_l = (_k = item.serverTypeName) !== null && _k !== void 0 ? _k : item.ServerTypeName) !== null && _l !== void 0 ? _l : '',
                operatingSystemId: (_o = (_m = item.operatingSystemId) !== null && _m !== void 0 ? _m : item.OperatingSystemId) !== null && _o !== void 0 ? _o : 0,
                operatingSystemName: (_q = (_p = item.operatingSystemName) !== null && _p !== void 0 ? _p : item.OperatingSystemName) !== null && _q !== void 0 ? _q : '',
                hostProviderId: (_s = (_r = item.hostProviderId) !== null && _r !== void 0 ? _r : item.HostProviderId) !== null && _s !== void 0 ? _s : null,
                hostProviderName: (_u = (_t = item.hostProviderName) !== null && _t !== void 0 ? _t : item.HostProviderName) !== null && _u !== void 0 ? _u : null,
                status: statusValue,
                cpuCores: (_w = (_v = item.cpuCores) !== null && _v !== void 0 ? _v : item.CpuCores) !== null && _w !== void 0 ? _w : null,
                ramMB: (_z = (_y = (_x = item.ramMB) !== null && _x !== void 0 ? _x : item.RamMB) !== null && _y !== void 0 ? _y : item.ramMb) !== null && _z !== void 0 ? _z : null,
                diskSpaceGB: (_2 = (_1 = (_0 = item.diskSpaceGB) !== null && _0 !== void 0 ? _0 : item.DiskSpaceGB) !== null && _1 !== void 0 ? _1 : item.diskSpaceGb) !== null && _2 !== void 0 ? _2 : null,
                notes: (_4 = (_3 = item.notes) !== null && _3 !== void 0 ? _3 : item.Notes) !== null && _4 !== void 0 ? _4 : null,
            };
            // Debug log for first item to verify data structure
            if (rawItems.indexOf(item) === 0) {
                console.log('First server after mapping:', server);
                console.log('Raw status was:', rawStatus, 'Converted to:', statusValue);
            }
            return server;
        });
        renderTable();
    }
    function renderTable() {
        const tableBody = document.getElementById('servers-table-body');
        if (!tableBody) {
            return;
        }
        if (!allServers.length) {
            tableBody.innerHTML = '<tr><td colspan="8" class="text-center text-muted">No servers found. Click "New Server" to add one.</td></tr>';
            return;
        }
        tableBody.innerHTML = allServers.map((server) => `
        <tr>
            <td>${server.id}</td>
            <td><strong>${esc(server.name)}</strong></td>
            <td>${esc(server.location || '-')}</td>
            <td>${esc(server.serverTypeName || '-')}</td>
            <td>${esc(server.operatingSystemName || '-')}</td>
            <td>${esc(server.hostProviderName || '-')}</td>
            <td><span class="badge bg-${getStatusBadgeColor(server.status)}">${esc(getStatusText(server.status))}</span></td>
            <td>
                <div class="btn-group btn-group-sm">
                    <button class="btn btn-outline-primary" type="button" data-action="edit" data-id="${server.id}" title="Edit"><i class="bi bi-pencil"></i></button>
                    <button class="btn btn-outline-danger" type="button" data-action="delete" data-id="${server.id}" data-name="${esc(server.name)}" title="Delete"><i class="bi bi-trash"></i></button>
                </div>
            </td>
        </tr>
    `).join('');
    }
    function getStatusBadgeColor(status) {
        if (status === true) {
            return 'success';
        }
        if (status === false) {
            return 'secondary';
        }
        return 'warning';
    }
    function getStatusText(status) {
        if (status === true) {
            return 'Active';
        }
        if (status === false) {
            return 'Inactive';
        }
        return 'Unknown';
    }
    function openCreate() {
        editingId = null;
        const modalTitle = document.getElementById('servers-modal-title');
        if (modalTitle) {
            modalTitle.textContent = 'New Server';
        }
        const form = document.getElementById('servers-form');
        form === null || form === void 0 ? void 0 : form.reset();
        const statusCheckbox = document.getElementById('servers-status');
        if (statusCheckbox) {
            statusCheckbox.checked = true;
        }
        populateDropdowns();
        showModal('servers-edit-modal');
    }
    function openEdit(id) {
        const server = allServers.find((item) => item.id === id);
        if (!server) {
            console.error('Server not found with ID:', id);
            return;
        }
        console.log('Editing server:', server);
        editingId = id;
        const modalTitle = document.getElementById('servers-modal-title');
        if (modalTitle) {
            modalTitle.textContent = 'Edit Server';
        }
        const nameInput = document.getElementById('servers-name');
        const locationInput = document.getElementById('servers-location');
        const serverTypeInput = document.getElementById('servers-server-type-id');
        const osInput = document.getElementById('servers-operating-system-id');
        const providerInput = document.getElementById('servers-host-provider-id');
        const statusInput = document.getElementById('servers-status');
        const cpuCoresInput = document.getElementById('servers-cpu-cores');
        const ramMBInput = document.getElementById('servers-ram-mb');
        const diskSpaceGBInput = document.getElementById('servers-disk-space-gb');
        const notesInput = document.getElementById('servers-notes');
        // Set basic text fields
        if (nameInput) {
            nameInput.value = server.name || '';
        }
        if (locationInput) {
            locationInput.value = server.location || '';
        }
        // Populate dropdowns before setting values
        populateDropdowns();
        // Set dropdown values - use empty string if null/undefined to reset to default
        if (serverTypeInput) {
            serverTypeInput.value = server.serverTypeId ? String(server.serverTypeId) : '';
            console.log('Set serverTypeId to:', serverTypeInput.value);
        }
        if (osInput) {
            osInput.value = server.operatingSystemId ? String(server.operatingSystemId) : '';
            console.log('Set operatingSystemId to:', osInput.value);
        }
        if (providerInput) {
            providerInput.value = server.hostProviderId ? String(server.hostProviderId) : '';
            console.log('Set hostProviderId to:', providerInput.value);
        }
        if (statusInput) {
            statusInput.checked = server.status !== false;
            console.log('Set status to:', statusInput.checked);
        }
        // Set numeric fields - use empty string if null to clear the field
        if (cpuCoresInput) {
            cpuCoresInput.value = server.cpuCores !== null && server.cpuCores !== undefined ? String(server.cpuCores) : '';
            console.log('Set cpuCores to:', cpuCoresInput.value);
        }
        if (ramMBInput) {
            ramMBInput.value = server.ramMB !== null && server.ramMB !== undefined ? String(server.ramMB) : '';
            console.log('Set ramMB to:', ramMBInput.value);
        }
        if (diskSpaceGBInput) {
            diskSpaceGBInput.value = server.diskSpaceGB !== null && server.diskSpaceGB !== undefined ? String(server.diskSpaceGB) : '';
            console.log('Set diskSpaceGB to:', diskSpaceGBInput.value);
        }
        if (notesInput) {
            notesInput.value = server.notes || '';
        }
        showModal('servers-edit-modal');
    }
    async function saveServer() {
        var _a, _b;
        const nameInput = document.getElementById('servers-name');
        const locationInput = document.getElementById('servers-location');
        const serverTypeInput = document.getElementById('servers-server-type-id');
        const osInput = document.getElementById('servers-operating-system-id');
        const providerInput = document.getElementById('servers-host-provider-id');
        const statusInput = document.getElementById('servers-status');
        const cpuCoresInput = document.getElementById('servers-cpu-cores');
        const ramMBInput = document.getElementById('servers-ram-mb');
        const diskSpaceGBInput = document.getElementById('servers-disk-space-gb');
        const notesInput = document.getElementById('servers-notes');
        const name = (_a = nameInput === null || nameInput === void 0 ? void 0 : nameInput.value.trim()) !== null && _a !== void 0 ? _a : '';
        const serverTypeId = (serverTypeInput === null || serverTypeInput === void 0 ? void 0 : serverTypeInput.value) ? Number(serverTypeInput.value) : null;
        const operatingSystemId = (osInput === null || osInput === void 0 ? void 0 : osInput.value) ? Number(osInput.value) : null;
        const status = (_b = statusInput === null || statusInput === void 0 ? void 0 : statusInput.checked) !== null && _b !== void 0 ? _b : true;
        if (!name || !serverTypeId || !operatingSystemId) {
            showError('Server Name, Server Type, and Operating System are required');
            return;
        }
        const payload = {
            name,
            serverTypeId,
            operatingSystemId,
            status,
            location: (locationInput === null || locationInput === void 0 ? void 0 : locationInput.value.trim()) || null,
            hostProviderId: (providerInput === null || providerInput === void 0 ? void 0 : providerInput.value) ? Number(providerInput.value) : null,
            cpuCores: (cpuCoresInput === null || cpuCoresInput === void 0 ? void 0 : cpuCoresInput.value) ? Number(cpuCoresInput.value) : null,
            ramMB: (ramMBInput === null || ramMBInput === void 0 ? void 0 : ramMBInput.value) ? Number(ramMBInput.value) : null,
            diskSpaceGB: (diskSpaceGBInput === null || diskSpaceGBInput === void 0 ? void 0 : diskSpaceGBInput.value) ? Number(diskSpaceGBInput.value) : null,
            notes: (notesInput === null || notesInput === void 0 ? void 0 : notesInput.value.trim()) || null,
        };
        const response = editingId
            ? await apiRequest(`${getApiBaseUrl()}/Servers/${editingId}`, { method: 'PUT', body: JSON.stringify(payload) })
            : await apiRequest(`${getApiBaseUrl()}/Servers`, { method: 'POST', body: JSON.stringify(payload) });
        if (response.success) {
            hideModal('servers-edit-modal');
            showSuccess(editingId ? 'Server updated successfully' : 'Server created successfully');
            loadServers();
        }
        else {
            showError(response.message || 'Save failed');
        }
    }
    function openDelete(id, name) {
        pendingDeleteId = id;
        const deleteName = document.getElementById('servers-delete-name');
        if (deleteName) {
            deleteName.textContent = name;
        }
        showModal('servers-delete-modal');
    }
    async function doDelete() {
        if (!pendingDeleteId) {
            return;
        }
        const response = await apiRequest(`${getApiBaseUrl()}/Servers/${pendingDeleteId}`, { method: 'DELETE' });
        hideModal('servers-delete-modal');
        if (response.success) {
            showSuccess('Server deleted successfully');
            loadServers();
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
        const alert = document.getElementById('servers-alert-success');
        if (!alert) {
            return;
        }
        alert.textContent = message;
        alert.classList.remove('d-none');
        const errorAlert = document.getElementById('servers-alert-error');
        errorAlert === null || errorAlert === void 0 ? void 0 : errorAlert.classList.add('d-none');
        setTimeout(() => alert.classList.add('d-none'), 5000);
    }
    function showError(message) {
        const alert = document.getElementById('servers-alert-error');
        if (!alert) {
            return;
        }
        alert.textContent = message;
        alert.classList.remove('d-none');
        const successAlert = document.getElementById('servers-alert-success');
        successAlert === null || successAlert === void 0 ? void 0 : successAlert.classList.add('d-none');
    }
    function showModal(id) {
        const element = document.getElementById(id);
        if (!element || !window.bootstrap) {
            return;
        }
        const modal = window.bootstrap.Modal.getOrCreateInstance(element);
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
        const tableBody = document.getElementById('servers-table-body');
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
    function initializeServersPage() {
        var _a, _b, _c;
        const page = document.getElementById('servers-page');
        if (!page || page.dataset.initialized === 'true') {
            return;
        }
        page.dataset.initialized = 'true';
        (_a = document.getElementById('servers-create')) === null || _a === void 0 ? void 0 : _a.addEventListener('click', openCreate);
        (_b = document.getElementById('servers-save')) === null || _b === void 0 ? void 0 : _b.addEventListener('click', saveServer);
        (_c = document.getElementById('servers-confirm-delete')) === null || _c === void 0 ? void 0 : _c.addEventListener('click', doDelete);
        bindTableActions();
        loadLookupData();
        loadServers();
    }
    function setupPageObserver() {
        // Try immediate initialization
        initializeServersPage();
        // Set up MutationObserver for Blazor navigation
        if (document.body) {
            const observer = new MutationObserver(() => {
                const page = document.getElementById('servers-page');
                if (page && page.dataset.initialized !== 'true') {
                    initializeServersPage();
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
//# sourceMappingURL=servers.js.map
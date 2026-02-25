"use strict";
// @ts-nocheck
(function () {
    function getApiBaseUrl() {
        return window.AppSettings?.apiBaseUrl ?? '';
    }
    let allServers = [];
    let serverTypes = [];
    let operatingSystems = [];
    let hostProviders = [];
    let editingId = null;
    let pendingDeleteId = null;
    function getAuthToken() {
        const auth = window.Auth;
        if (auth?.getToken) {
            return auth.getToken();
        }
        return sessionStorage.getItem('rp_authToken');
    }
    async function apiRequest(endpoint, options = {}) {
        try {
            const headers = {
                'Content-Type': 'application/json',
                ...options.headers,
            };
            const authToken = getAuthToken();
            if (authToken) {
                headers['Authorization'] = `Bearer ${authToken}`;
            }
            const response = await fetch(endpoint, {
                ...options,
                headers,
                credentials: 'include',
            });
            const contentType = response.headers.get('content-type') ?? '';
            const hasJson = contentType.includes('application/json');
            const data = hasJson ? await response.json() : null;
            if (!response.ok) {
                return {
                    success: false,
                    message: (data && (data.message ?? data.title)) || `Request failed with status ${response.status}`,
                };
            }
            return {
                success: data?.success !== false,
                data: data?.data ?? data,
                message: data?.message,
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
            : Array.isArray(data?.data)
                ? data.data
                : [];
        return rawItems.map((item) => ({
            id: item.id ?? item.Id ?? 0,
            name: item.name ?? item.Name ?? '',
            displayName: item.displayName ?? item.DisplayName ?? item.name ?? item.Name ?? '',
        }));
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
        if (Array.isArray(data?.data)) {
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
            const rawStatus = item.status ?? item.Status;
            let statusValue = null;
            if (typeof rawStatus === 'boolean') {
                statusValue = rawStatus;
            }
            else if (typeof rawStatus === 'string') {
                statusValue = rawStatus.toLowerCase() === 'active' ? true : rawStatus.toLowerCase() === 'inactive' ? false : null;
            }
            const server = {
                id: item.id ?? item.Id ?? 0,
                name: item.name ?? item.Name ?? '',
                location: item.location ?? item.Location ?? null,
                serverTypeId: item.serverTypeId ?? item.ServerTypeId ?? 0,
                serverTypeName: item.serverTypeName ?? item.ServerTypeName ?? '',
                operatingSystemId: item.operatingSystemId ?? item.OperatingSystemId ?? 0,
                operatingSystemName: item.operatingSystemName ?? item.OperatingSystemName ?? '',
                hostProviderId: item.hostProviderId ?? item.HostProviderId ?? null,
                hostProviderName: item.hostProviderName ?? item.HostProviderName ?? null,
                status: statusValue,
                cpuCores: item.cpuCores ?? item.CpuCores ?? null,
                ramMB: item.ramMB ?? item.RamMB ?? item.ramMb ?? null,
                diskSpaceGB: item.diskSpaceGB ?? item.DiskSpaceGB ?? item.diskSpaceGb ?? null,
                notes: item.notes ?? item.Notes ?? null,
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
        form?.reset();
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
        const name = nameInput?.value.trim() ?? '';
        const serverTypeId = serverTypeInput?.value ? Number(serverTypeInput.value) : null;
        const operatingSystemId = osInput?.value ? Number(osInput.value) : null;
        const status = statusInput?.checked ?? true;
        if (!name || !serverTypeId || !operatingSystemId) {
            showError('Server Name, Server Type, and Operating System are required');
            return;
        }
        const payload = {
            name,
            serverTypeId,
            operatingSystemId,
            status,
            location: locationInput?.value.trim() || null,
            hostProviderId: providerInput?.value ? Number(providerInput.value) : null,
            cpuCores: cpuCoresInput?.value ? Number(cpuCoresInput.value) : null,
            ramMB: ramMBInput?.value ? Number(ramMBInput.value) : null,
            diskSpaceGB: diskSpaceGBInput?.value ? Number(diskSpaceGBInput.value) : null,
            notes: notesInput?.value.trim() || null,
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
        errorAlert?.classList.add('d-none');
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
        successAlert?.classList.add('d-none');
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
        modal?.hide();
    }
    function bindTableActions() {
        const tableBody = document.getElementById('servers-table-body');
        if (!tableBody) {
            return;
        }
        tableBody.addEventListener('click', (event) => {
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
                openDelete(id, button.dataset.name ?? '');
            }
        });
    }
    function initializeServersPage() {
        const page = document.getElementById('servers-page');
        if (!page || page.dataset.initialized === 'true') {
            return;
        }
        page.dataset.initialized = 'true';
        document.getElementById('servers-create')?.addEventListener('click', openCreate);
        document.getElementById('servers-save')?.addEventListener('click', saveServer);
        document.getElementById('servers-confirm-delete')?.addEventListener('click', doDelete);
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
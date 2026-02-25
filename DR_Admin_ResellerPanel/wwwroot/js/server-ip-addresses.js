"use strict";
// @ts-nocheck
(function () {
    function getApiBaseUrl() {
        return window.AppSettings?.apiBaseUrl ?? '';
    }
    let allServerIpAddresses = [];
    let servers = [];
    const serverNameLookup = {};
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
            console.error('Server IP addresses request failed', error);
            return {
                success: false,
                message: 'Network error. Please try again.',
            };
        }
    }
    async function loadServers() {
        const response = await apiRequest(`${getApiBaseUrl()}/Servers`, { method: 'GET' });
        const rawItems = Array.isArray(response.data)
            ? response.data
            : Array.isArray(response.data?.data)
                ? response.data.data
                : [];
        servers = rawItems.map((item) => ({
            id: item.id ?? item.Id ?? 0,
            name: item.name ?? item.Name ?? '',
        }));
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
            : Array.isArray(response.data?.data)
                ? response.data.data
                : [];
        allServerIpAddresses = rawItems.map((item) => {
            const serverId = item.serverId ?? item.ServerId ?? 0;
            return {
                id: item.id ?? item.Id ?? 0,
                serverId,
                serverName: item.serverName ?? item.ServerName ?? serverNameLookup[serverId] ?? null,
                ipAddress: item.ipAddress ?? item.IpAddress ?? '',
                ipVersion: item.ipVersion ?? item.IpVersion ?? 'IPv4',
                isPrimary: item.isPrimary ?? item.IsPrimary ?? false,
                status: item.status ?? item.Status ?? 'Active',
                assignedTo: item.assignedTo ?? item.AssignedTo ?? null,
                notes: item.notes ?? item.Notes ?? null,
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
        switch (status?.toLowerCase()) {
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
        form?.reset();
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
        const ipAddressInput = document.getElementById('server-ip-addresses-ip-address');
        const ipVersionInput = document.getElementById('server-ip-addresses-ip-version');
        const serverIdInput = document.getElementById('server-ip-addresses-server-id');
        const statusInput = document.getElementById('server-ip-addresses-status');
        const isPrimaryInput = document.getElementById('server-ip-addresses-is-primary');
        const assignedToInput = document.getElementById('server-ip-addresses-assigned-to');
        const notesInput = document.getElementById('server-ip-addresses-notes');
        const ipAddress = ipAddressInput?.value.trim() ?? '';
        const ipVersion = ipVersionInput?.value?.trim() ?? '';
        const serverId = serverIdInput?.value ? Number(serverIdInput.value) : null;
        const status = statusInput?.value?.trim() ?? 'Active';
        if (!ipAddress || !ipVersion || !serverId || !status) {
            showError('IP Address, IP Version, Server, and Status are required');
            return;
        }
        const payload = {
            serverId,
            ipAddress,
            ipVersion,
            status,
            isPrimary: isPrimaryInput?.checked ?? false,
            assignedTo: assignedToInput?.value.trim() || null,
            notes: notesInput?.value.trim() || null,
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
        errorAlert?.classList.add('d-none');
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
        successAlert?.classList.add('d-none');
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
        modal?.hide();
    }
    function bindTableActions() {
        const tableBody = document.getElementById('server-ip-addresses-table-body');
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
    function initializeServerIpAddressesPage() {
        const page = document.getElementById('server-ip-addresses-page');
        if (!page || page.dataset.initialized === 'true') {
            return;
        }
        page.dataset.initialized = 'true';
        document.getElementById('server-ip-addresses-create')?.addEventListener('click', openCreate);
        document.getElementById('server-ip-addresses-save')?.addEventListener('click', saveServerIpAddress);
        document.getElementById('server-ip-addresses-confirm-delete')?.addEventListener('click', doDelete);
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
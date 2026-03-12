"use strict";
// @ts-nocheck
(function () {
    let allPanels = [];
    let filteredPanels = [];
    let servers = [];
    let panelTypes = [];
    let ipOptions = [];
    let editingId = null;
    let pendingDeleteId = null;
    let selectedServerFromQuery = null;
    function getApiBaseUrl() {
        return window.AppSettings?.apiBaseUrl ?? '';
    }
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
            console.error('Server control panels request failed', error);
            return {
                success: false,
                message: 'Network error. Please try again.',
            };
        }
    }
    function extractArray(data) {
        if (Array.isArray(data)) {
            return data;
        }
        if (Array.isArray(data?.data)) {
            return data.data;
        }
        if (Array.isArray(data?.Data)) {
            return data.Data;
        }
        return [];
    }
    async function loadLookups() {
        const [serversRes, panelTypesRes] = await Promise.all([
            apiRequest(`${getApiBaseUrl()}/Servers`, { method: 'GET' }),
            apiRequest(`${getApiBaseUrl()}/ControlPanelTypes/active`, { method: 'GET' }),
        ]);
        servers = extractArray(serversRes.data).map((item) => ({
            id: item.id ?? item.Id ?? 0,
            name: item.name ?? item.Name ?? '',
            displayName: item.displayName ?? item.DisplayName ?? item.name ?? item.Name ?? '',
        }));
        panelTypes = extractArray(panelTypesRes.data).map((item) => ({
            id: item.id ?? item.Id ?? 0,
            name: item.name ?? item.Name ?? '',
            displayName: item.displayName ?? item.DisplayName ?? item.name ?? item.Name ?? '',
        }));
        renderLookupDropdowns();
    }
    function renderLookupDropdowns() {
        const serverFilter = document.getElementById('server-control-panels-filter-server');
        const serverSelect = document.getElementById('server-control-panels-server-id');
        const typeFilter = document.getElementById('server-control-panels-filter-type');
        const typeSelect = document.getElementById('server-control-panels-type-id');
        if (serverFilter) {
            const selected = serverFilter.value;
            serverFilter.innerHTML = '<option value="">All servers</option>' +
                servers.map((s) => `<option value="${s.id}">${esc(s.displayName || s.name || '')}</option>`).join('');
            if (selected) {
                serverFilter.value = selected;
            }
        }
        if (serverSelect) {
            const selected = serverSelect.value;
            serverSelect.innerHTML = '<option value="">Select server...</option>' +
                servers.map((s) => `<option value="${s.id}">${esc(s.displayName || s.name || '')}</option>`).join('');
            if (selected) {
                serverSelect.value = selected;
            }
        }
        if (typeFilter) {
            const selected = typeFilter.value;
            typeFilter.innerHTML = '<option value="">All panel types</option>' +
                panelTypes.map((t) => `<option value="${t.id}">${esc(t.displayName || t.name || '')}</option>`).join('');
            if (selected) {
                typeFilter.value = selected;
            }
        }
        if (typeSelect) {
            const selected = typeSelect.value;
            typeSelect.innerHTML = '<option value="">Select panel type...</option>' +
                panelTypes.map((t) => `<option value="${t.id}">${esc(t.displayName || t.name || '')}</option>`).join('');
            if (selected) {
                typeSelect.value = selected;
            }
        }
    }
    async function loadPanels() {
        const response = await apiRequest(`${getApiBaseUrl()}/ServerControlPanels`, { method: 'GET' });
        if (!response.success) {
            showError(response.message || 'Failed to load control panel instances.');
            return;
        }
        allPanels = extractArray(response.data).map((item) => ({
            id: item.id ?? item.Id ?? 0,
            serverId: item.serverId ?? item.ServerId ?? 0,
            controlPanelTypeId: item.controlPanelTypeId ?? item.ControlPanelTypeId ?? 0,
            apiUrl: item.apiUrl ?? item.ApiUrl ?? '',
            port: item.port ?? item.Port ?? 0,
            useHttps: item.useHttps ?? item.UseHttps ?? true,
            username: item.username ?? item.Username ?? null,
            status: item.status ?? item.Status ?? 'Active',
            ipAddressId: item.ipAddressId ?? item.IpAddressId ?? null,
            ipAddressValue: item.ipAddressValue ?? item.IpAddressValue ?? null,
            isConnectionHealthy: item.isConnectionHealthy ?? item.IsConnectionHealthy ?? null,
            lastConnectionTest: item.lastConnectionTest ?? item.LastConnectionTest ?? null,
        }));
        applyFilters();
    }
    function applyFilters() {
        const serverFilterValue = Number(document.getElementById('server-control-panels-filter-server')?.value ?? '0');
        const typeFilterValue = Number(document.getElementById('server-control-panels-filter-type')?.value ?? '0');
        const healthFilterValue = document.getElementById('server-control-panels-filter-health')?.value ?? 'all';
        filteredPanels = allPanels.filter((panel) => {
            if (serverFilterValue > 0 && panel.serverId !== serverFilterValue) {
                return false;
            }
            if (typeFilterValue > 0 && panel.controlPanelTypeId !== typeFilterValue) {
                return false;
            }
            if (healthFilterValue === 'healthy' && panel.isConnectionHealthy !== true) {
                return false;
            }
            if (healthFilterValue === 'error' && panel.isConnectionHealthy !== false) {
                return false;
            }
            if (healthFilterValue === 'unknown' && panel.isConnectionHealthy != null) {
                return false;
            }
            return true;
        });
        renderTable();
    }
    function renderTable() {
        const body = document.getElementById('server-control-panels-table-body');
        if (!body) {
            return;
        }
        setText('server-control-panels-count', String(filteredPanels.length));
        if (!filteredPanels.length) {
            body.innerHTML = '<tr><td colspan="6" class="text-center text-muted">No control panel instances found.</td></tr>';
            return;
        }
        body.innerHTML = filteredPanels.map((panel) => {
            const health = panel.isConnectionHealthy == null
                ? '<span class="badge bg-secondary">Unknown</span>'
                : panel.isConnectionHealthy
                    ? '<span class="badge bg-success">Healthy</span>'
                    : '<span class="badge bg-danger">Error</span>';
            return `
        <tr>
            <td>${esc(getServerName(panel.serverId))}</td>
            <td>${esc(getPanelTypeName(panel.controlPanelTypeId))}</td>
            <td><code>${esc(panel.apiUrl || '-')}</code><div class="small text-muted">Port ${panel.port || '-'} · ${panel.useHttps ? 'HTTPS' : 'HTTP'}</div></td>
            <td><span class="badge bg-${panel.status === 'Active' ? 'success' : panel.status === 'Inactive' ? 'secondary' : 'warning'}">${esc(panel.status || '-')}</span></td>
            <td>${health}</td>
            <td class="text-end">
                <div class="btn-group btn-group-sm">
                    <button class="btn btn-outline-secondary" type="button" data-action="test" data-id="${panel.id}" title="Test"><i class="bi bi-plug"></i></button>
                    <button class="btn btn-outline-primary" type="button" data-action="edit" data-id="${panel.id}" title="Edit"><i class="bi bi-pencil"></i></button>
                    <button class="btn btn-outline-danger" type="button" data-action="delete" data-id="${panel.id}" title="Delete"><i class="bi bi-trash"></i></button>
                </div>
            </td>
        </tr>`;
        }).join('');
    }
    function getServerName(id) {
        const match = servers.find((s) => s.id === id);
        return match?.displayName || match?.name || `Server #${id}`;
    }
    function getPanelTypeName(id) {
        const match = panelTypes.find((t) => t.id === id);
        return match?.displayName || match?.name || `Type #${id}`;
    }
    async function loadIpOptions(serverId) {
        if (!serverId) {
            ipOptions = [];
            renderIpOptions();
            return;
        }
        const response = await apiRequest(`${getApiBaseUrl()}/ServerIpAddresses/server/${serverId}`, { method: 'GET' });
        if (!response.success) {
            ipOptions = [];
            renderIpOptions();
            return;
        }
        ipOptions = extractArray(response.data).map((item) => ({
            id: item.id ?? item.Id ?? 0,
            serverId: item.serverId ?? item.ServerId ?? 0,
            ipAddress: item.ipAddress ?? item.IpAddress ?? '',
            isPrimary: item.isPrimary ?? item.IsPrimary ?? false,
        }));
        renderIpOptions();
    }
    function renderIpOptions() {
        const select = document.getElementById('server-control-panels-ip-id');
        if (!select) {
            return;
        }
        const selected = select.value;
        select.innerHTML = '<option value="">No IP binding</option>' +
            ipOptions.map((ip) => `<option value="${ip.id}">${esc(ip.ipAddress)}${ip.isPrimary ? ' (Primary)' : ''}</option>`).join('');
        if (selected) {
            select.value = selected;
        }
    }
    async function openCreate() {
        editingId = null;
        setText('server-control-panels-modal-title', 'New Control Panel Instance');
        setInputValue('server-control-panels-api-url', '');
        setInputValue('server-control-panels-port', '2087');
        setInputValue('server-control-panels-username', '');
        setInputValue('server-control-panels-api-token', '');
        setInputValue('server-control-panels-api-key', '');
        setInputValue('server-control-panels-password', '');
        setInputValue('server-control-panels-notes', '');
        setSelectValue('server-control-panels-status', 'Active');
        setSelectValue('server-control-panels-server-id', selectedServerFromQuery ? String(selectedServerFromQuery) : '');
        setSelectValue('server-control-panels-type-id', '');
        const httpsInput = document.getElementById('server-control-panels-use-https');
        if (httpsInput) {
            httpsInput.checked = true;
        }
        const serverInput = document.getElementById('server-control-panels-server-id');
        const typeInput = document.getElementById('server-control-panels-type-id');
        if (serverInput) {
            serverInput.disabled = false;
        }
        if (typeInput) {
            typeInput.disabled = false;
        }
        await loadIpOptions(getSelectNumberValue('server-control-panels-server-id'));
        showModal('server-control-panels-edit-modal');
    }
    async function openEdit(id) {
        const panel = allPanels.find((item) => item.id === id);
        if (!panel) {
            return;
        }
        editingId = id;
        setText('server-control-panels-modal-title', 'Edit Control Panel Instance');
        setSelectValue('server-control-panels-server-id', String(panel.serverId));
        setSelectValue('server-control-panels-type-id', String(panel.controlPanelTypeId));
        setInputValue('server-control-panels-api-url', panel.apiUrl || '');
        setInputValue('server-control-panels-port', String(panel.port || 2087));
        setInputValue('server-control-panels-username', panel.username || '');
        setInputValue('server-control-panels-api-token', '');
        setInputValue('server-control-panels-api-key', '');
        setInputValue('server-control-panels-password', '');
        setInputValue('server-control-panels-notes', '');
        setSelectValue('server-control-panels-status', panel.status || 'Active');
        const httpsInput = document.getElementById('server-control-panels-use-https');
        if (httpsInput) {
            httpsInput.checked = panel.useHttps !== false;
        }
        const serverInput = document.getElementById('server-control-panels-server-id');
        const typeInput = document.getElementById('server-control-panels-type-id');
        if (serverInput) {
            serverInput.disabled = true;
        }
        if (typeInput) {
            typeInput.disabled = true;
        }
        await loadIpOptions(panel.serverId);
        setSelectValue('server-control-panels-ip-id', panel.ipAddressId != null ? String(panel.ipAddressId) : '');
        showModal('server-control-panels-edit-modal');
    }
    async function save() {
        const serverId = getSelectNumberValue('server-control-panels-server-id');
        const controlPanelTypeId = getSelectNumberValue('server-control-panels-type-id');
        const apiUrl = getInputValue('server-control-panels-api-url');
        if (!editingId && (!serverId || !controlPanelTypeId)) {
            showError('Server and panel type are required.');
            return;
        }
        if (!apiUrl) {
            showError('API URL is required.');
            return;
        }
        const commonPayload = {
            apiUrl,
            port: getNumberValue('server-control-panels-port') || 2087,
            useHttps: isChecked('server-control-panels-use-https'),
            apiToken: getInputValue('server-control-panels-api-token') || null,
            apiKey: getInputValue('server-control-panels-api-key') || null,
            username: getInputValue('server-control-panels-username') || null,
            password: getInputValue('server-control-panels-password') || null,
            additionalSettings: null,
            status: getInputValue('server-control-panels-status') || 'Active',
            ipAddressId: getNullableNumberValue('server-control-panels-ip-id'),
            notes: getInputValue('server-control-panels-notes') || null,
        };
        const response = editingId
            ? await apiRequest(`${getApiBaseUrl()}/ServerControlPanels/${editingId}`, {
                method: 'PUT',
                body: JSON.stringify(commonPayload),
            })
            : await apiRequest(`${getApiBaseUrl()}/ServerControlPanels`, {
                method: 'POST',
                body: JSON.stringify({
                    serverId,
                    controlPanelTypeId,
                    ...commonPayload,
                }),
            });
        if (!response.success) {
            showError(response.message || 'Save failed.');
            return;
        }
        hideModal('server-control-panels-edit-modal');
        showSuccess(editingId ? 'Control panel instance updated.' : 'Control panel instance created.');
        await loadPanels();
    }
    function openDelete(id) {
        const panel = allPanels.find((item) => item.id === id);
        if (!panel) {
            return;
        }
        pendingDeleteId = id;
        setText('server-control-panels-delete-name', `${getServerName(panel.serverId)} · ${getPanelTypeName(panel.controlPanelTypeId)}`);
        showModal('server-control-panels-delete-modal');
    }
    async function doDelete() {
        if (!pendingDeleteId) {
            return;
        }
        const response = await apiRequest(`${getApiBaseUrl()}/ServerControlPanels/${pendingDeleteId}`, { method: 'DELETE' });
        hideModal('server-control-panels-delete-modal');
        if (!response.success) {
            showError(response.message || 'Delete failed.');
            return;
        }
        pendingDeleteId = null;
        showSuccess('Control panel instance deleted.');
        await loadPanels();
    }
    async function testConnection(id) {
        const response = await apiRequest(`${getApiBaseUrl()}/ServerControlPanels/${id}/test-connection`, { method: 'POST' });
        if (!response.success) {
            showError(response.message || 'Connection test failed.');
            return;
        }
        showSuccess('Connection test executed.');
        await loadPanels();
    }
    function bindActions() {
        document.getElementById('server-control-panels-create')?.addEventListener('click', () => { void openCreate(); });
        document.getElementById('server-control-panels-save')?.addEventListener('click', () => { void save(); });
        document.getElementById('server-control-panels-confirm-delete')?.addEventListener('click', () => { void doDelete(); });
        document.getElementById('server-control-panels-filter-server')?.addEventListener('change', applyFilters);
        document.getElementById('server-control-panels-filter-type')?.addEventListener('change', applyFilters);
        document.getElementById('server-control-panels-filter-health')?.addEventListener('change', applyFilters);
        document.getElementById('server-control-panels-server-id')?.addEventListener('change', () => {
            void loadIpOptions(getSelectNumberValue('server-control-panels-server-id'));
        });
        const body = document.getElementById('server-control-panels-table-body');
        body?.addEventListener('click', (event) => {
            const target = event.target;
            const button = target.closest('button[data-action]');
            if (!button) {
                return;
            }
            const id = Number(button.dataset.id ?? '0');
            if (!id) {
                return;
            }
            if (button.dataset.action === 'edit') {
                void openEdit(id);
                return;
            }
            if (button.dataset.action === 'delete') {
                openDelete(id);
                return;
            }
            if (button.dataset.action === 'test') {
                void testConnection(id);
            }
        });
    }
    function setInitialServerFilterFromQuery() {
        const params = new URLSearchParams(window.location.search);
        const raw = Number(params.get('server-id') ?? '0');
        if (!Number.isFinite(raw) || raw <= 0) {
            return;
        }
        selectedServerFromQuery = raw;
        const filter = document.getElementById('server-control-panels-filter-server');
        if (filter) {
            filter.value = String(raw);
        }
    }
    function showSuccess(message) {
        const alert = document.getElementById('server-control-panels-alert-success');
        if (!alert) {
            return;
        }
        alert.textContent = message;
        alert.classList.remove('d-none');
        document.getElementById('server-control-panels-alert-error')?.classList.add('d-none');
    }
    function showError(message) {
        const alert = document.getElementById('server-control-panels-alert-error');
        if (!alert) {
            return;
        }
        alert.textContent = message;
        alert.classList.remove('d-none');
        document.getElementById('server-control-panels-alert-success')?.classList.add('d-none');
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
    function setText(id, value) {
        const element = document.getElementById(id);
        if (element) {
            element.textContent = value;
        }
    }
    function setInputValue(id, value) {
        const input = document.getElementById(id);
        if (input) {
            input.value = value;
        }
    }
    function setSelectValue(id, value) {
        const input = document.getElementById(id);
        if (input) {
            input.value = value;
        }
    }
    function getInputValue(id) {
        const input = document.getElementById(id);
        return (input?.value ?? '').trim();
    }
    function getSelectNumberValue(id) {
        const input = document.getElementById(id);
        const parsed = Number(input?.value ?? '0');
        return Number.isFinite(parsed) ? parsed : 0;
    }
    function getNumberValue(id) {
        const parsed = Number(getInputValue(id));
        return Number.isFinite(parsed) ? parsed : 0;
    }
    function getNullableNumberValue(id) {
        const value = getInputValue(id);
        if (!value) {
            return null;
        }
        const parsed = Number(value);
        return Number.isFinite(parsed) ? parsed : null;
    }
    function isChecked(id) {
        const input = document.getElementById(id);
        return !!input?.checked;
    }
    function esc(text) {
        const map = { '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#039;' };
        return (text || '').toString().replace(/[&<>"']/g, (char) => map[char]);
    }
    async function initializePage() {
        const page = document.getElementById('server-control-panels-page');
        if (!page || page.dataset.initialized === 'true') {
            return;
        }
        page.dataset.initialized = 'true';
        bindActions();
        await loadLookups();
        setInitialServerFilterFromQuery();
        await loadPanels();
    }
    function setupPageObserver() {
        void initializePage();
        if (document.body) {
            const observer = new MutationObserver(() => {
                const page = document.getElementById('server-control-panels-page');
                if (page && page.dataset.initialized !== 'true') {
                    void initializePage();
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
//# sourceMappingURL=server-control-panels.js.map
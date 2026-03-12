// @ts-nocheck
(function() {
interface Server {
    id: number;
    name: string;
    location?: string | null;
    serverTypeId: number;
    serverTypeName?: string;
    operatingSystemId: number;
    operatingSystemName?: string;
    hostProviderId?: number | null;
    hostProviderName?: string | null;
    status: boolean | null;
    cpuCores?: number | null;
    ramMB?: number | null;
    diskSpaceGB?: number | null;
    notes?: string | null;
}

interface LookupItem {
    id: number;
    name?: string;
    displayName?: string;
}

interface ServerControlPanel {
    id: number;
    serverId: number;
    controlPanelTypeId: number;
    apiUrl: string;
    port: number;
    useHttps: boolean;
    username?: string | null;
    status: string;
    ipAddressId?: number | null;
    ipAddressValue?: string | null;
    isConnectionHealthy?: boolean | null;
    lastConnectionTest?: string | null;
}

interface ServerIpAddress {
    id: number;
    serverId: number;
    ipAddress: string;
    isPrimary: boolean;
}

interface ApiResponse<T> {
    success: boolean;
    data?: T;
    message?: string;
}

function getApiBaseUrl(): string {
    return (window as any).AppSettings?.apiBaseUrl ?? '';
}

let allServers: Server[] = [];
let serverTypes: LookupItem[] = [];
let operatingSystems: LookupItem[] = [];
let hostProviders: LookupItem[] = [];
let controlPanelTypes: LookupItem[] = [];
let editingId: number | null = null;
let pendingDeleteId: number | null = null;

let serverPanels: ServerControlPanel[] = [];
let serverIps: ServerIpAddress[] = [];
let editingPanelId: number | null = null;
let pendingPanelDeleteId: number | null = null;

function getAuthToken(): string | null {
    const auth = (window as any).Auth;
    if (auth?.getToken) {
        return auth.getToken();
    }

    return sessionStorage.getItem('rp_authToken');
}

async function apiRequest<T>(endpoint: string, options: RequestInit = {}): Promise<ApiResponse<T>> {
    try {
        const headers: Record<string, string> = {
            'Content-Type': 'application/json',
            ...(options.headers as Record<string, string>),
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
    } catch (error) {
        console.error('Servers request failed', error);
        return {
            success: false,
            message: 'Network error. Please try again.',
        };
    }
}

function extractArray(data: any): any[] {
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

async function loadLookupData(): Promise<void> {
    const [serverTypesRes, osRes, providersRes, panelTypesRes] = await Promise.all([
        apiRequest<LookupItem[]>(`${getApiBaseUrl()}/ServerTypes`, { method: 'GET' }),
        apiRequest<LookupItem[]>(`${getApiBaseUrl()}/OperatingSystems`, { method: 'GET' }),
        apiRequest<LookupItem[]>(`${getApiBaseUrl()}/HostProviders`, { method: 'GET' }),
        apiRequest<LookupItem[]>(`${getApiBaseUrl()}/ControlPanelTypes/active`, { method: 'GET' }),
    ]);

    serverTypes = normalizeLookupArray(serverTypesRes.data);
    operatingSystems = normalizeLookupArray(osRes.data);
    hostProviders = normalizeLookupArray(providersRes.data);
    controlPanelTypes = normalizeLookupArray(panelTypesRes.data);

    populateDropdowns();
    populatePanelTypeDropdown();
}

function normalizeLookupArray(data: any): LookupItem[] {
    const rawItems = extractArray(data);
    return rawItems.map((item: any) => ({
        id: item.id ?? item.Id ?? 0,
        name: item.name ?? item.Name ?? '',
        displayName: item.displayName ?? item.DisplayName ?? item.name ?? item.Name ?? '',
    }));
}

function populateDropdowns(): void {
    const serverTypeSelect = document.getElementById('servers-server-type-id') as HTMLSelectElement | null;
    if (serverTypeSelect) {
        const selected = serverTypeSelect.value;
        serverTypeSelect.innerHTML = '<option value="">Select Server Type...</option>' +
            serverTypes.map((t) => `<option value="${t.id}">${esc(t.displayName || t.name || '')}</option>`).join('');
        if (selected) {
            serverTypeSelect.value = selected;
        }
    }

    const osSelect = document.getElementById('servers-operating-system-id') as HTMLSelectElement | null;
    if (osSelect) {
        const selected = osSelect.value;
        osSelect.innerHTML = '<option value="">Select OS...</option>' +
            operatingSystems.map((os) => `<option value="${os.id}">${esc(os.displayName || os.name || '')}</option>`).join('');
        if (selected) {
            osSelect.value = selected;
        }
    }

    const providerSelect = document.getElementById('servers-host-provider-id') as HTMLSelectElement | null;
    if (providerSelect) {
        const selected = providerSelect.value;
        providerSelect.innerHTML = '<option value="">Select Provider...</option>' +
            hostProviders.map((p) => `<option value="${p.id}">${esc(p.displayName || p.name || '')}</option>`).join('');
        if (selected) {
            providerSelect.value = selected;
        }
    }
}

function populatePanelTypeDropdown(): void {
    const select = document.getElementById('servers-panel-control-panel-type-id') as HTMLSelectElement | null;
    if (!select) {
        return;
    }

    const selected = select.value;
    select.innerHTML = '<option value="">Select Panel Type...</option>' +
        controlPanelTypes.map((t) => `<option value="${t.id}">${esc(t.displayName || t.name || '')}</option>`).join('');

    if (selected) {
        select.value = selected;
    }
}

async function loadServers(): Promise<void> {
    const tableBody = document.getElementById('servers-table-body');
    if (!tableBody) {
        return;
    }

    tableBody.innerHTML = '<tr><td colspan="8" class="text-center"><div class="spinner-border text-primary"></div></td></tr>';

    const response = await apiRequest<Server[]>(`${getApiBaseUrl()}/Servers`, { method: 'GET' });
    if (!response.success) {
        showError(response.message || 'Failed to load servers');
        tableBody.innerHTML = '<tr><td colspan="8" class="text-center text-danger">Failed to load data</td></tr>';
        return;
    }

    const rawItems = extractArray(response.data);
    allServers = rawItems.map((item: any) => {
        const rawStatus = item.status ?? item.Status;
        let statusValue: boolean | null = null;
        if (typeof rawStatus === 'boolean') {
            statusValue = rawStatus;
        } else if (typeof rawStatus === 'string') {
            statusValue = rawStatus.toLowerCase() === 'active' ? true : rawStatus.toLowerCase() === 'inactive' ? false : null;
        }

        return {
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
    });

    renderTable();
}

function renderTable(): void {
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

function getStatusBadgeColor(status: boolean | null): string {
    if (status === true) {
        return 'success';
    }
    if (status === false) {
        return 'secondary';
    }
    return 'warning';
}

function getStatusText(status: boolean | null): string {
    if (status === true) {
        return 'Active';
    }
    if (status === false) {
        return 'Inactive';
    }
    return 'Unknown';
}

function openCreate(): void {
    editingId = null;

    const modalTitle = document.getElementById('servers-modal-title');
    if (modalTitle) {
        modalTitle.textContent = 'New Server';
    }

    const form = document.getElementById('servers-form') as HTMLFormElement | null;
    form?.reset();

    const statusCheckbox = document.getElementById('servers-status') as HTMLInputElement | null;
    if (statusCheckbox) {
        statusCheckbox.checked = true;
    }

    resetServerPanelSectionForNewServer();
    populateDropdowns();
    showModal('servers-edit-modal');
}

async function openEdit(id: number): Promise<void> {
    const server = allServers.find((item) => item.id === id);
    if (!server) {
        return;
    }

    editingId = id;

    const modalTitle = document.getElementById('servers-modal-title');
    if (modalTitle) {
        modalTitle.textContent = 'Edit Server';
    }

    const nameInput = document.getElementById('servers-name') as HTMLInputElement | null;
    const locationInput = document.getElementById('servers-location') as HTMLInputElement | null;
    const serverTypeInput = document.getElementById('servers-server-type-id') as HTMLSelectElement | null;
    const osInput = document.getElementById('servers-operating-system-id') as HTMLSelectElement | null;
    const providerInput = document.getElementById('servers-host-provider-id') as HTMLSelectElement | null;
    const statusInput = document.getElementById('servers-status') as HTMLInputElement | null;
    const cpuCoresInput = document.getElementById('servers-cpu-cores') as HTMLInputElement | null;
    const ramMBInput = document.getElementById('servers-ram-mb') as HTMLInputElement | null;
    const diskSpaceGBInput = document.getElementById('servers-disk-space-gb') as HTMLInputElement | null;
    const notesInput = document.getElementById('servers-notes') as HTMLTextAreaElement | null;

    if (nameInput) {
        nameInput.value = server.name || '';
    }
    if (locationInput) {
        locationInput.value = server.location || '';
    }

    populateDropdowns();

    if (serverTypeInput) {
        serverTypeInput.value = server.serverTypeId ? String(server.serverTypeId) : '';
    }
    if (osInput) {
        osInput.value = server.operatingSystemId ? String(server.operatingSystemId) : '';
    }
    if (providerInput) {
        providerInput.value = server.hostProviderId ? String(server.hostProviderId) : '';
    }
    if (statusInput) {
        statusInput.checked = server.status !== false;
    }

    if (cpuCoresInput) {
        cpuCoresInput.value = server.cpuCores != null ? String(server.cpuCores) : '';
    }
    if (ramMBInput) {
        ramMBInput.value = server.ramMB != null ? String(server.ramMB) : '';
    }
    if (diskSpaceGBInput) {
        diskSpaceGBInput.value = server.diskSpaceGB != null ? String(server.diskSpaceGB) : '';
    }
    if (notesInput) {
        notesInput.value = server.notes || '';
    }

    await loadServerPanelSection(id);
    showModal('servers-edit-modal');
}

async function saveServer(): Promise<void> {
    const nameInput = document.getElementById('servers-name') as HTMLInputElement | null;
    const locationInput = document.getElementById('servers-location') as HTMLInputElement | null;
    const serverTypeInput = document.getElementById('servers-server-type-id') as HTMLSelectElement | null;
    const osInput = document.getElementById('servers-operating-system-id') as HTMLSelectElement | null;
    const providerInput = document.getElementById('servers-host-provider-id') as HTMLSelectElement | null;
    const statusInput = document.getElementById('servers-status') as HTMLInputElement | null;
    const cpuCoresInput = document.getElementById('servers-cpu-cores') as HTMLInputElement | null;
    const ramMBInput = document.getElementById('servers-ram-mb') as HTMLInputElement | null;
    const diskSpaceGBInput = document.getElementById('servers-disk-space-gb') as HTMLInputElement | null;
    const notesInput = document.getElementById('servers-notes') as HTMLTextAreaElement | null;

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
        await loadServers();
    } else {
        showError(response.message || 'Save failed');
    }
}

function openDelete(id: number, name: string): void {
    pendingDeleteId = id;

    const deleteName = document.getElementById('servers-delete-name');
    if (deleteName) {
        deleteName.textContent = name;
    }

    showModal('servers-delete-modal');
}

async function doDelete(): Promise<void> {
    if (!pendingDeleteId) {
        return;
    }

    const response = await apiRequest(`${getApiBaseUrl()}/Servers/${pendingDeleteId}`, { method: 'DELETE' });
    hideModal('servers-delete-modal');

    if (response.success) {
        showSuccess('Server deleted successfully');
        await loadServers();
    } else {
        showError(response.message || 'Delete failed');
    }

    pendingDeleteId = null;
}

function resetServerPanelSectionForNewServer(): void {
    const hint = document.getElementById('servers-panels-new-server-hint');
    const wrapper = document.getElementById('servers-panels-list-wrapper');
    const addButton = document.getElementById('servers-panels-add') as HTMLButtonElement | null;

    hint?.classList.remove('d-none');
    wrapper?.classList.add('d-none');

    if (addButton) {
        addButton.disabled = true;
    }

    serverPanels = [];
    serverIps = [];
    renderServerPanelList();
}

async function loadServerPanelSection(serverId: number): Promise<void> {
    const hint = document.getElementById('servers-panels-new-server-hint');
    const wrapper = document.getElementById('servers-panels-list-wrapper');
    const addButton = document.getElementById('servers-panels-add') as HTMLButtonElement | null;

    hint?.classList.add('d-none');
    wrapper?.classList.remove('d-none');

    if (addButton) {
        addButton.disabled = false;
    }

    await Promise.all([
        loadServerPanels(serverId),
        loadServerIps(serverId),
    ]);
}

async function loadServerPanels(serverId: number): Promise<void> {
    const response = await apiRequest<ServerControlPanel[]>(`${getApiBaseUrl()}/ServerControlPanels/server/${serverId}`, { method: 'GET' });
    if (!response.success) {
        serverPanels = [];
        renderServerPanelList();
        return;
    }

    serverPanels = extractArray(response.data).map((item: any) => ({
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

    renderServerPanelList();
}

async function loadServerIps(serverId: number): Promise<void> {
    const response = await apiRequest<ServerIpAddress[]>(`${getApiBaseUrl()}/ServerIpAddresses/server/${serverId}`, { method: 'GET' });
    if (!response.success) {
        serverIps = [];
        populatePanelIpDropdown();
        return;
    }

    serverIps = extractArray(response.data).map((item: any) => ({
        id: item.id ?? item.Id ?? 0,
        serverId: item.serverId ?? item.ServerId ?? 0,
        ipAddress: item.ipAddress ?? item.IpAddress ?? '',
        isPrimary: item.isPrimary ?? item.IsPrimary ?? false,
    }));

    populatePanelIpDropdown();
}

function renderServerPanelList(): void {
    const list = document.getElementById('servers-panels-list');
    if (!list) {
        return;
    }

    if (!serverPanels.length) {
        list.innerHTML = '<li class="list-group-item text-muted">No control panel instances configured.</li>';
        return;
    }

    list.innerHTML = serverPanels.map((panel) => {
        const typeName = getControlPanelTypeName(panel.controlPanelTypeId);
        const health = panel.isConnectionHealthy == null
            ? '<span class="badge bg-secondary">Unknown</span>'
            : panel.isConnectionHealthy
                ? '<span class="badge bg-success">Healthy</span>'
                : '<span class="badge bg-danger">Error</span>';

        return `
            <li class="list-group-item">
                <div class="d-flex justify-content-between align-items-start gap-2">
                    <div>
                        <div class="fw-semibold">${esc(typeName)} <span class="text-muted">${esc(panel.apiUrl || '-')}</span></div>
                        <div class="small text-muted">Port ${panel.port || '-'} · ${panel.useHttps ? 'HTTPS' : 'HTTP'}${panel.ipAddressValue ? ` · IP ${esc(panel.ipAddressValue)}` : ''} · Status ${esc(panel.status || '-')}</div>
                    </div>
                    <div class="d-flex gap-1">
                        ${health}
                        <button class="btn btn-sm btn-outline-secondary" type="button" data-action="panel-test" data-id="${panel.id}" title="Test"><i class="bi bi-plug"></i></button>
                        <button class="btn btn-sm btn-outline-primary" type="button" data-action="panel-edit" data-id="${panel.id}" title="Edit"><i class="bi bi-pencil"></i></button>
                        <button class="btn btn-sm btn-outline-danger" type="button" data-action="panel-delete" data-id="${panel.id}" title="Delete"><i class="bi bi-trash"></i></button>
                    </div>
                </div>
            </li>`;
    }).join('');
}

function getControlPanelTypeName(id: number): string {
    const match = controlPanelTypes.find((item) => item.id === id);
    return match?.displayName || match?.name || `Type #${id}`;
}

function populatePanelIpDropdown(): void {
    const select = document.getElementById('servers-panel-ip-address-id') as HTMLSelectElement | null;
    if (!select) {
        return;
    }

    const selected = select.value;
    select.innerHTML = '<option value="">No IP binding</option>' +
        serverIps.map((ip) => `<option value="${ip.id}">${esc(ip.ipAddress)}${ip.isPrimary ? ' (Primary)' : ''}</option>`).join('');

    if (selected) {
        select.value = selected;
    }
}

function openCreateServerPanel(): void {
    if (!editingId) {
        showError('Save the server before adding panel instances.');
        return;
    }

    editingPanelId = null;

    setText('servers-panel-modal-title', 'Add Control Panel Instance');
    setInputValue('servers-panel-api-url', '');
    setInputValue('servers-panel-port', '2087');
    setInputValue('servers-panel-username', '');
    setInputValue('servers-panel-api-token', '');
    setInputValue('servers-panel-api-key', '');
    setInputValue('servers-panel-password', '');
    setInputValue('servers-panel-notes', '');
    setSelectValue('servers-panel-status', 'Active');
    setSelectValue('servers-panel-control-panel-type-id', '');
    setSelectValue('servers-panel-ip-address-id', '');

    const typeSelect = document.getElementById('servers-panel-control-panel-type-id') as HTMLSelectElement | null;
    if (typeSelect) {
        typeSelect.disabled = false;
    }

    const httpsInput = document.getElementById('servers-panel-use-https') as HTMLInputElement | null;
    if (httpsInput) {
        httpsInput.checked = true;
    }

    showModal('servers-panel-edit-modal');
}

function openEditServerPanel(id: number): void {
    const panel = serverPanels.find((item) => item.id === id);
    if (!panel) {
        return;
    }

    editingPanelId = id;
    setText('servers-panel-modal-title', 'Edit Control Panel Instance');

    setInputValue('servers-panel-api-url', panel.apiUrl || '');
    setInputValue('servers-panel-port', String(panel.port || 2087));
    setInputValue('servers-panel-username', panel.username || '');
    setInputValue('servers-panel-api-token', '');
    setInputValue('servers-panel-api-key', '');
    setInputValue('servers-panel-password', '');
    setInputValue('servers-panel-notes', '');
    setSelectValue('servers-panel-status', panel.status || 'Active');
    setSelectValue('servers-panel-control-panel-type-id', String(panel.controlPanelTypeId));
    setSelectValue('servers-panel-ip-address-id', panel.ipAddressId != null ? String(panel.ipAddressId) : '');

    const httpsInput = document.getElementById('servers-panel-use-https') as HTMLInputElement | null;
    if (httpsInput) {
        httpsInput.checked = panel.useHttps !== false;
    }

    const typeSelect = document.getElementById('servers-panel-control-panel-type-id') as HTMLSelectElement | null;
    if (typeSelect) {
        typeSelect.disabled = true;
    }

    showModal('servers-panel-edit-modal');
}

async function saveServerPanel(): Promise<void> {
    if (!editingId) {
        return;
    }

    const controlPanelTypeId = getSelectNumberValue('servers-panel-control-panel-type-id');
    const apiUrl = getInputValue('servers-panel-api-url');

    if (!editingPanelId && !controlPanelTypeId) {
        showError('Panel type is required.');
        return;
    }

    if (!apiUrl) {
        showError('API URL is required.');
        return;
    }

    const commonPayload = {
        apiUrl,
        port: getNumberValue('servers-panel-port') || 2087,
        useHttps: isChecked('servers-panel-use-https'),
        apiToken: getInputValue('servers-panel-api-token') || null,
        apiKey: getInputValue('servers-panel-api-key') || null,
        username: getInputValue('servers-panel-username') || null,
        password: getInputValue('servers-panel-password') || null,
        additionalSettings: null,
        status: getInputValue('servers-panel-status') || 'Active',
        ipAddressId: getNullableNumberValue('servers-panel-ip-address-id'),
        notes: getInputValue('servers-panel-notes') || null,
    };

    const response = editingPanelId
        ? await apiRequest(`${getApiBaseUrl()}/ServerControlPanels/${editingPanelId}`, {
            method: 'PUT',
            body: JSON.stringify(commonPayload),
        })
        : await apiRequest(`${getApiBaseUrl()}/ServerControlPanels`, {
            method: 'POST',
            body: JSON.stringify({
                serverId: editingId,
                controlPanelTypeId,
                ...commonPayload,
            }),
        });

    if (!response.success) {
        showError(response.message || 'Failed to save control panel instance.');
        return;
    }

    hideModal('servers-panel-edit-modal');
    showSuccess(editingPanelId ? 'Control panel instance updated.' : 'Control panel instance created.');

    await loadServerPanelSection(editingId);
}

async function testServerPanel(id: number): Promise<void> {
    const response = await apiRequest(`${getApiBaseUrl()}/ServerControlPanels/${id}/test-connection`, { method: 'POST' });
    if (!response.success) {
        showError(response.message || 'Connection test failed.');
        return;
    }

    showSuccess('Connection test executed.');
    if (editingId) {
        await loadServerPanelSection(editingId);
    }
}

async function deleteServerPanel(): Promise<void> {
    if (!pendingPanelDeleteId) {
        return;
    }

    const response = await apiRequest(`${getApiBaseUrl()}/ServerControlPanels/${pendingPanelDeleteId}`, { method: 'DELETE' });
    if (!response.success) {
        showError(response.message || 'Failed to delete control panel instance.');
        return;
    }

    pendingPanelDeleteId = null;
    showSuccess('Control panel instance deleted.');

    if (editingId) {
        await loadServerPanelSection(editingId);
    }
}

function openManageAllPanelsPage(): void {
    const target = editingId
        ? `/infrastructure/server-control-panels?server-id=${encodeURIComponent(String(editingId))}`
        : '/infrastructure/server-control-panels';

    window.location.href = target;
}

function esc(text: string): string {
    const map: Record<string, string> = { '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#039;' };
    return (text || '').toString().replace(/[&<>"']/g, (char) => map[char]);
}

function showSuccess(message: string): void {
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

function showError(message: string): void {
    const alert = document.getElementById('servers-alert-error');
    if (!alert) {
        return;
    }

    alert.textContent = message;
    alert.classList.remove('d-none');

    const successAlert = document.getElementById('servers-alert-success');
    successAlert?.classList.add('d-none');
}

function showModal(id: string): void {
    const element = document.getElementById(id);
    if (!element || !(window as any).bootstrap) {
        return;
    }

    const modal = (window as any).bootstrap.Modal.getOrCreateInstance(element);
    modal.show();
}

function hideModal(id: string): void {
    const element = document.getElementById(id);
    if (!element || !(window as any).bootstrap) {
        return;
    }

    const modal = (window as any).bootstrap.Modal.getInstance(element);
    modal?.hide();
}

function setText(id: string, value: string): void {
    const element = document.getElementById(id);
    if (element) {
        element.textContent = value;
    }
}

function setInputValue(id: string, value: string): void {
    const input = document.getElementById(id) as HTMLInputElement | HTMLTextAreaElement | null;
    if (input) {
        input.value = value;
    }
}

function setSelectValue(id: string, value: string): void {
    const input = document.getElementById(id) as HTMLSelectElement | null;
    if (input) {
        input.value = value;
    }
}

function getInputValue(id: string): string {
    const input = document.getElementById(id) as HTMLInputElement | HTMLTextAreaElement | null;
    return (input?.value ?? '').trim();
}

function getNumberValue(id: string): number {
    const parsed = Number(getInputValue(id));
    return Number.isFinite(parsed) ? parsed : 0;
}

function getNullableNumberValue(id: string): number | null {
    const value = getInputValue(id);
    if (!value) {
        return null;
    }

    const parsed = Number(value);
    return Number.isFinite(parsed) ? parsed : null;
}

function getSelectNumberValue(id: string): number {
    const input = document.getElementById(id) as HTMLSelectElement | null;
    const parsed = Number(input?.value ?? '0');
    return Number.isFinite(parsed) ? parsed : 0;
}

function isChecked(id: string): boolean {
    const input = document.getElementById(id) as HTMLInputElement | null;
    return !!input?.checked;
}

function bindTableActions(): void {
    const tableBody = document.getElementById('servers-table-body');
    if (!tableBody) {
        return;
    }

    tableBody.addEventListener('click', (event) => {
        const target = event.target as HTMLElement;
        const button = target.closest('button[data-action]') as HTMLButtonElement | null;
        if (!button) {
            return;
        }

        const id = Number(button.dataset.id);
        if (!id) {
            return;
        }

        if (button.dataset.action === 'edit') {
            void openEdit(id);
            return;
        }

        if (button.dataset.action === 'delete') {
            openDelete(id, button.dataset.name ?? '');
        }
    });
}

function bindServerPanelActions(): void {
    document.getElementById('servers-panels-add')?.addEventListener('click', openCreateServerPanel);
    document.getElementById('servers-panels-manage-all')?.addEventListener('click', openManageAllPanelsPage);
    document.getElementById('servers-panel-save')?.addEventListener('click', saveServerPanel);

    const list = document.getElementById('servers-panels-list');
    list?.addEventListener('click', (event) => {
        const target = event.target as HTMLElement;
        const button = target.closest('button[data-action]') as HTMLButtonElement | null;
        if (!button) {
            return;
        }

        const id = Number(button.dataset.id ?? '0');
        if (!id) {
            return;
        }

        if (button.dataset.action === 'panel-edit') {
            openEditServerPanel(id);
            return;
        }

        if (button.dataset.action === 'panel-test') {
            void testServerPanel(id);
            return;
        }

        if (button.dataset.action === 'panel-delete') {
            pendingPanelDeleteId = id;
            void deleteServerPanel();
        }
    });
}

function initializeServersPage(): void {
    const page = document.getElementById('servers-page');
    if (!page || page.dataset.initialized === 'true') {
        return;
    }

    page.dataset.initialized = 'true';

    document.getElementById('servers-create')?.addEventListener('click', openCreate);
    document.getElementById('servers-save')?.addEventListener('click', saveServer);
    document.getElementById('servers-confirm-delete')?.addEventListener('click', doDelete);

    bindTableActions();
    bindServerPanelActions();

    void loadLookupData();
    void loadServers();
}

function setupPageObserver(): void {
    initializeServersPage();

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

if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', setupPageObserver);
} else {
    setupPageObserver();
}
})();

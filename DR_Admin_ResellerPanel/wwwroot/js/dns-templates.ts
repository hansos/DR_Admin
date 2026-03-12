// @ts-nocheck
(function() {
interface DnsZonePackage {
    id: number;
    name: string;
    description?: string | null;
    isActive: boolean;
    isDefault: boolean;
    sortOrder: number;
    records?: DnsZonePackageRecord[];
    controlPanels?: DnsZonePackageControlPanelSummary[];
    servers?: DnsZonePackageServerSummary[];
}

interface DnsZonePackageRecord {
    id: number;
    dnsZonePackageId: number;
    dnsRecordTypeId: number;
    name: string;
    value: string;
    valueSourceType: string;
    valueSourceReference?: string | null;
    ttl: number;
    priority?: number | null;
    weight?: number | null;
    port?: number | null;
    notes?: string | null;
}

interface DnsZonePackageControlPanelSummary {
    controlPanelId: number;
    apiUrl: string;
    serverName: string;
    controlPanelTypeName: string;
}

interface DnsZonePackageServerSummary {
    serverId: number;
    serverName: string;
    status?: boolean | null;
}

interface DnsRecordType {
    id: number;
    type: string;
    hasPriority: boolean;
    hasWeight: boolean;
    hasPort: boolean;
    isEditableByUser?: boolean;
    defaultTTL?: number;
    isActive?: boolean;
}

interface ServerOption {
    id: number;
    name: string;
}

interface ControlPanelOption {
    id: number;
    apiUrl: string;
    serverName: string;
    controlPanelTypeName: string;
}

interface ApiResponse<T> {
    success: boolean;
    data?: T;
    message?: string;
}

let templates: DnsZonePackage[] = [];
let selectedTemplateId: number | null = null;
let selectedTemplate: DnsZonePackage | null = null;
let templateRecords: DnsZonePackageRecord[] = [];
let assignedServers: DnsZonePackageServerSummary[] = [];
let assignedPanels: DnsZonePackageControlPanelSummary[] = [];
let serverOptions: ServerOption[] = [];
let controlPanelOptions: ControlPanelOption[] = [];
let recordTypes: DnsRecordType[] = [];

let editingTemplateId: number | null = null;
let pendingTemplateDeleteId: number | null = null;
let editingRecordId: number | null = null;
let pendingRecordDeleteId: number | null = null;

function getApiBaseUrl(): string {
    return (window as any).AppSettings?.apiBaseUrl ?? '';
}

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
            success: (data as any)?.success !== false,
            data: (data as any)?.data ?? data,
            message: (data as any)?.message,
        };
    } catch (error) {
        console.error('DNS templates request failed', error);
        return {
            success: false,
            message: 'Network error. Please try again.',
        };
    }
}

function extractArray(raw: unknown): any[] {
    if (Array.isArray(raw)) {
        return raw;
    }

    if (raw && typeof raw === 'object') {
        const obj = raw as Record<string, unknown>;
        if (Array.isArray(obj.data)) {
            return obj.data;
        }
        if (Array.isArray(obj.Data)) {
            return obj.Data;
        }
        if (obj.data && typeof obj.data === 'object') {
            const nested = obj.data as Record<string, unknown>;
            if (Array.isArray(nested.items)) {
                return nested.items;
            }
            if (Array.isArray(nested.Items)) {
                return nested.Items;
            }
        }
    }

    return [];
}

function initializePage(): void {
    const page = document.getElementById('dns-templates-page') as HTMLElement | null;
    if (!page || page.dataset.initialized === 'true') {
        return;
    }

    page.dataset.initialized = 'true';

    document.getElementById('dns-templates-create')?.addEventListener('click', openCreateTemplate);
    document.getElementById('dns-templates-save')?.addEventListener('click', saveTemplate);
    document.getElementById('dns-templates-confirm-delete')?.addEventListener('click', deleteTemplate);

    document.getElementById('dns-templates-record-create')?.addEventListener('click', openCreateRecord);
    document.getElementById('dns-templates-record-save')?.addEventListener('click', saveRecord);
    document.getElementById('dns-templates-record-confirm-delete')?.addEventListener('click', deleteRecord);
    document.getElementById('dns-templates-record-type')?.addEventListener('change', updateRecordFieldVisibility);
    document.getElementById('dns-templates-record-value-source')?.addEventListener('change', onRecordValueSourceChanged);

    document.getElementById('dns-templates-server-assign')?.addEventListener('click', assignServer);
    document.getElementById('dns-templates-panel-assign')?.addEventListener('click', assignControlPanel);

    bindTableActions();

    void Promise.all([
        loadRecordTypes(),
        loadServerOptions(),
        loadControlPanelOptions(),
    ]).then(loadTemplates);
}

function bindTableActions(): void {
    const templateBody = document.getElementById('dns-templates-table-body');
    templateBody?.addEventListener('click', (event) => {
        const target = event.target as HTMLElement;
        const button = target.closest('button[data-action]') as HTMLButtonElement | null;
        if (!button) {
            return;
        }

        const id = Number(button.dataset.id ?? '0');
        if (!Number.isFinite(id) || id <= 0) {
            return;
        }

        const action = button.dataset.action;
        if (action === 'select') {
            void selectTemplate(id);
            return;
        }

        if (action === 'edit') {
            openEditTemplate(id);
            return;
        }

        if (action === 'delete') {
            openDeleteTemplate(id);
        }
    });

    const recordBody = document.getElementById('dns-templates-records-table-body');
    recordBody?.addEventListener('click', (event) => {
        const target = event.target as HTMLElement;
        const button = target.closest('button[data-action]') as HTMLButtonElement | null;
        if (!button) {
            return;
        }

        const id = Number(button.dataset.id ?? '0');
        if (!Number.isFinite(id) || id <= 0) {
            return;
        }

        if (button.dataset.action === 'edit-record') {
            openEditRecord(id);
            return;
        }

        if (button.dataset.action === 'delete-record') {
            openDeleteRecord(id);
        }
    });

    const serverList = document.getElementById('dns-templates-server-list');
    serverList?.addEventListener('click', (event) => {
        const target = event.target as HTMLElement;
        const button = target.closest('button[data-action="remove-server"]') as HTMLButtonElement | null;
        if (!button) {
            return;
        }

        const id = Number(button.dataset.id ?? '0');
        if (!Number.isFinite(id) || id <= 0) {
            return;
        }

        void removeServer(id);
    });

    const panelList = document.getElementById('dns-templates-panel-list');
    panelList?.addEventListener('click', (event) => {
        const target = event.target as HTMLElement;
        const button = target.closest('button[data-action="remove-panel"]') as HTMLButtonElement | null;
        if (!button) {
            return;
        }

        const id = Number(button.dataset.id ?? '0');
        if (!Number.isFinite(id) || id <= 0) {
            return;
        }

        void removeControlPanel(id);
    });
}

async function loadTemplates(): Promise<void> {
    const response = await apiRequest<DnsZonePackage[]>(`${getApiBaseUrl()}/DnsZonePackages`, { method: 'GET' });
    if (!response.success) {
        showError(response.message || 'Failed to load DNS templates.');
        return;
    }

    const items = extractArray(response.data).map(normalizeTemplate);
    templates = items.sort((a: DnsZonePackage, b: DnsZonePackage) => a.sortOrder - b.sortOrder || a.name.localeCompare(b.name));

    renderTemplates();

    if (selectedTemplateId && templates.some((template) => template.id === selectedTemplateId)) {
        await selectTemplate(selectedTemplateId);
    } else {
        clearSelectionUi();
    }
}

async function selectTemplate(templateId: number): Promise<void> {
    selectedTemplateId = templateId;

    const response = await apiRequest<DnsZonePackage>(`${getApiBaseUrl()}/DnsZonePackages/${templateId}/assignments`, { method: 'GET' });
    if (!response.success || !response.data) {
        showError(response.message || 'Failed to load template details.');
        return;
    }

    selectedTemplate = normalizeTemplate(response.data);
    templateRecords = extractArray((response.data as any).records ?? (response.data as any).Records).map(normalizeRecord);
    assignedServers = extractArray((response.data as any).servers ?? (response.data as any).Servers).map(normalizeServerSummary);
    assignedPanels = extractArray((response.data as any).controlPanels ?? (response.data as any).ControlPanels).map(normalizePanelSummary);

    renderTemplates();
    renderSelectedTemplateInfo();
    renderRecords();
    renderAssignments();
    updateSelectionDependentControls();
    refreshARecordNameSuggestions();
}

function normalizeTemplate(raw: any): DnsZonePackage {
    return {
        id: raw.id ?? raw.Id ?? 0,
        name: raw.name ?? raw.Name ?? '',
        description: raw.description ?? raw.Description ?? null,
        isActive: raw.isActive ?? raw.IsActive ?? false,
        isDefault: raw.isDefault ?? raw.IsDefault ?? false,
        sortOrder: raw.sortOrder ?? raw.SortOrder ?? 0,
        records: extractArray(raw.records ?? raw.Records).map(normalizeRecord),
        controlPanels: extractArray(raw.controlPanels ?? raw.ControlPanels).map(normalizePanelSummary),
        servers: extractArray(raw.servers ?? raw.Servers).map(normalizeServerSummary),
    };
}

function normalizeRecord(raw: any): DnsZonePackageRecord {
    return {
        id: raw.id ?? raw.Id ?? 0,
        dnsZonePackageId: raw.dnsZonePackageId ?? raw.DnsZonePackageId ?? 0,
        dnsRecordTypeId: raw.dnsRecordTypeId ?? raw.DnsRecordTypeId ?? 0,
        name: raw.name ?? raw.Name ?? '',
        value: raw.value ?? raw.Value ?? '',
        valueSourceType: raw.valueSourceType ?? raw.ValueSourceType ?? 'Manual',
        valueSourceReference: raw.valueSourceReference ?? raw.ValueSourceReference ?? null,
        ttl: raw.ttl ?? raw.TTL ?? 3600,
        priority: raw.priority ?? raw.Priority ?? null,
        weight: raw.weight ?? raw.Weight ?? null,
        port: raw.port ?? raw.Port ?? null,
        notes: raw.notes ?? raw.Notes ?? null,
    };
}

function normalizeServerSummary(raw: any): DnsZonePackageServerSummary {
    return {
        serverId: raw.serverId ?? raw.ServerId ?? 0,
        serverName: raw.serverName ?? raw.ServerName ?? '',
        status: raw.status ?? raw.Status ?? null,
    };
}

function normalizePanelSummary(raw: any): DnsZonePackageControlPanelSummary {
    return {
        controlPanelId: raw.controlPanelId ?? raw.ControlPanelId ?? 0,
        apiUrl: raw.apiUrl ?? raw.ApiUrl ?? '',
        serverName: raw.serverName ?? raw.ServerName ?? '',
        controlPanelTypeName: raw.controlPanelTypeName ?? raw.ControlPanelTypeName ?? '',
    };
}

async function loadRecordTypes(): Promise<void> {
    const response = await apiRequest<DnsRecordType[]>(`${getApiBaseUrl()}/DnsRecordTypes`, { method: 'GET' });
    if (!response.success) {
        return;
    }

    recordTypes = extractArray(response.data).map((item: any) => ({
        id: item.id ?? item.Id ?? 0,
        type: item.type ?? item.Type ?? '',
        hasPriority: item.hasPriority ?? item.HasPriority ?? false,
        hasWeight: item.hasWeight ?? item.HasWeight ?? false,
        hasPort: item.hasPort ?? item.HasPort ?? false,
        isEditableByUser: item.isEditableByUser ?? item.IsEditableByUser ?? true,
        defaultTTL: item.defaultTTL ?? item.DefaultTTL ?? 3600,
        isActive: item.isActive ?? item.IsActive ?? true,
    })).filter((item: DnsRecordType) => item.id > 0 && item.type);

    const select = document.getElementById('dns-templates-record-type') as HTMLSelectElement | null;
    if (select) {
        select.innerHTML = recordTypes
            .filter((type) => type.isActive !== false && type.isEditableByUser !== false)
            .sort((a, b) => a.type.localeCompare(b.type))
            .map((type) => `<option value="${type.id}">${esc(type.type)}</option>`)
            .join('');
    }
}

async function loadServerOptions(): Promise<void> {
    const response = await apiRequest<ServerOption[]>(`${getApiBaseUrl()}/Servers`, { method: 'GET' });
    if (!response.success) {
        return;
    }

    serverOptions = extractArray(response.data)
        .map((item: any) => ({ id: Number(item.id ?? item.Id ?? 0), name: String(item.name ?? item.Name ?? '') }))
        .filter((item: ServerOption) => Number.isFinite(item.id) && item.id > 0)
        .sort((a: ServerOption, b: ServerOption) => a.name.localeCompare(b.name));

    renderServerSelectOptions();
}

async function loadControlPanelOptions(): Promise<void> {
    const response = await apiRequest<ControlPanelOption[]>(`${getApiBaseUrl()}/ServerControlPanels`, { method: 'GET' });
    if (!response.success) {
        return;
    }

    controlPanelOptions = extractArray(response.data)
        .map((item: any) => ({
            id: Number(item.id ?? item.Id ?? 0),
            apiUrl: String(item.apiUrl ?? item.ApiUrl ?? ''),
            serverName: String(item.serverName ?? item.ServerName ?? ''),
            controlPanelTypeName: String(item.controlPanelTypeName ?? item.ControlPanelTypeName ?? ''),
        }))
        .filter((item: ControlPanelOption) => Number.isFinite(item.id) && item.id > 0)
        .sort((a: ControlPanelOption, b: ControlPanelOption) => a.id - b.id);

    renderPanelSelectOptions();
}

function renderTemplates(): void {
    const body = document.getElementById('dns-templates-table-body');
    if (!body) {
        return;
    }

    setText('dns-templates-count', `${templates.length} template${templates.length === 1 ? '' : 's'}`);

    if (!templates.length) {
        body.innerHTML = '<tr><td colspan="4" class="text-center text-muted">No templates found.</td></tr>';
        return;
    }

    body.innerHTML = templates.map((template) => {
        const activeBadge = `<span class="badge bg-${template.isActive ? 'success' : 'secondary'}">${template.isActive ? 'Active' : 'Inactive'}</span>`;
        const defaultBadge = template.isDefault
            ? '<span class="badge bg-primary">Default</span>'
            : '<span class="badge bg-light text-dark border">No</span>';
        const selectedClass = selectedTemplateId === template.id ? 'table-primary' : '';

        return `
        <tr class="${selectedClass}">
            <td><button class="btn btn-link btn-sm p-0 text-start" type="button" data-action="select" data-id="${template.id}">${esc(template.name)}</button></td>
            <td>${activeBadge}</td>
            <td>${defaultBadge}</td>
            <td class="text-end">
                <div class="btn-group btn-group-sm">
                    <button class="btn btn-outline-primary" type="button" data-action="edit" data-id="${template.id}" title="Edit"><i class="bi bi-pencil"></i></button>
                    <button class="btn btn-outline-danger" type="button" data-action="delete" data-id="${template.id}" title="Delete"><i class="bi bi-trash"></i></button>
                </div>
            </td>
        </tr>`;
    }).join('');
}

function renderSelectedTemplateInfo(): void {
    const noSelection = document.getElementById('dns-templates-no-selection');
    const wrapper = document.getElementById('dns-templates-selected-wrapper');

    if (!selectedTemplate) {
        noSelection?.classList.remove('d-none');
        wrapper?.classList.add('d-none');
        setText('dns-templates-selected-name', 'None selected');
        return;
    }

    noSelection?.classList.add('d-none');
    wrapper?.classList.remove('d-none');

    setText('dns-templates-selected-name', selectedTemplate.name || 'Unnamed');
    setText('dns-templates-selected-description', selectedTemplate.description || '-');
    setText('dns-templates-selected-sort', String(selectedTemplate.sortOrder ?? 0));
    setText('dns-templates-selected-active', selectedTemplate.isActive ? 'Yes' : 'No');
}

function renderRecords(): void {
    const body = document.getElementById('dns-templates-records-table-body');
    if (!body) {
        return;
    }

    if (!selectedTemplateId) {
        body.innerHTML = '<tr><td colspan="5" class="text-center text-muted">Select a template to view records.</td></tr>';
        return;
    }

    if (!templateRecords.length) {
        body.innerHTML = '<tr><td colspan="5" class="text-center text-muted">No records in this template.</td></tr>';
        return;
    }

    body.innerHTML = templateRecords.map((record) => `
        <tr>
            <td><code>${esc(getRecordTypeLabel(record.dnsRecordTypeId))}</code></td>
            <td>${esc(record.name || '@')}</td>
            <td class="text-break"><code class="small">${esc(record.value || '-')}</code>${record.valueSourceType && record.valueSourceType !== 'Manual' ? ` <span class="badge bg-info text-dark">${esc(record.valueSourceType)}</span>` : ''}</td>
            <td>${record.ttl ?? '-'}</td>
            <td class="text-end">
                <div class="btn-group btn-group-sm">
                    <button class="btn btn-outline-primary" type="button" data-action="edit-record" data-id="${record.id}"><i class="bi bi-pencil"></i></button>
                    <button class="btn btn-outline-danger" type="button" data-action="delete-record" data-id="${record.id}"><i class="bi bi-trash"></i></button>
                </div>
            </td>
        </tr>`).join('');
}

function renderAssignments(): void {
    const serverList = document.getElementById('dns-templates-server-list');
    const panelList = document.getElementById('dns-templates-panel-list');

    setText('dns-templates-server-count', String(assignedServers.length));
    setText('dns-templates-panel-count', String(assignedPanels.length));

    if (serverList) {
        if (!assignedServers.length) {
            serverList.innerHTML = '<li class="list-group-item text-muted">No servers assigned.</li>';
        } else {
            serverList.innerHTML = assignedServers.map((server) => `
                <li class="list-group-item d-flex justify-content-between align-items-center">
                    <span>${esc(server.serverName || `Server #${server.serverId}`)}</span>
                    <button class="btn btn-sm btn-outline-danger" type="button" data-action="remove-server" data-id="${server.serverId}"><i class="bi bi-x-lg"></i></button>
                </li>`).join('');
        }
    }

    if (panelList) {
        if (!assignedPanels.length) {
            panelList.innerHTML = '<li class="list-group-item text-muted">No control panels assigned.</li>';
        } else {
            panelList.innerHTML = assignedPanels.map((panel) => {
                const label = panel.serverName
                    ? `${panel.serverName} · ${panel.controlPanelTypeName || panel.apiUrl || `#${panel.controlPanelId}`}`
                    : panel.controlPanelTypeName || panel.apiUrl || `Panel #${panel.controlPanelId}`;
                return `
                <li class="list-group-item d-flex justify-content-between align-items-center">
                    <span>${esc(label)}</span>
                    <button class="btn btn-sm btn-outline-danger" type="button" data-action="remove-panel" data-id="${panel.controlPanelId}"><i class="bi bi-x-lg"></i></button>
                </li>`;
            }).join('');
        }
    }

    renderServerSelectOptions();
    renderPanelSelectOptions();
}

function renderServerSelectOptions(): void {
    const select = document.getElementById('dns-templates-server-select') as HTMLSelectElement | null;
    if (!select) {
        return;
    }

    const assignedIds = new Set(assignedServers.map((item) => item.serverId));
    const available = serverOptions.filter((item) => !assignedIds.has(item.id));

    select.innerHTML = '<option value="">Select server</option>' + available
        .map((item) => `<option value="${item.id}">${esc(item.name || `Server #${item.id}`)}</option>`)
        .join('');
}

function renderPanelSelectOptions(): void {
    const select = document.getElementById('dns-templates-panel-select') as HTMLSelectElement | null;
    if (!select) {
        return;
    }

    const assignedIds = new Set(assignedPanels.map((item) => item.controlPanelId));
    const available = controlPanelOptions.filter((item) => !assignedIds.has(item.id));

    select.innerHTML = '<option value="">Select control panel</option>' + available
        .map((item) => {
            const label = item.serverName
                ? `${item.serverName} · ${item.controlPanelTypeName || item.apiUrl || `#${item.id}`}`
                : item.controlPanelTypeName || item.apiUrl || `Panel #${item.id}`;
            return `<option value="${item.id}">${esc(label)}</option>`;
        })
        .join('');
}

function updateSelectionDependentControls(): void {
    const hasSelection = !!selectedTemplateId;

    const recordCreate = document.getElementById('dns-templates-record-create') as HTMLButtonElement | null;
    const serverSelect = document.getElementById('dns-templates-server-select') as HTMLSelectElement | null;
    const panelSelect = document.getElementById('dns-templates-panel-select') as HTMLSelectElement | null;
    const serverAssign = document.getElementById('dns-templates-server-assign') as HTMLButtonElement | null;
    const panelAssign = document.getElementById('dns-templates-panel-assign') as HTMLButtonElement | null;

    if (recordCreate) {
        recordCreate.disabled = !hasSelection;
    }

    if (serverSelect) {
        serverSelect.disabled = !hasSelection;
    }

    if (panelSelect) {
        panelSelect.disabled = !hasSelection;
    }

    if (serverAssign) {
        serverAssign.disabled = !hasSelection;
    }

    if (panelAssign) {
        panelAssign.disabled = !hasSelection;
    }
}

function openCreateTemplate(): void {
    editingTemplateId = null;
    setText('dns-templates-edit-title', 'New Template');

    setInputValue('dns-templates-name', '');
    setInputValue('dns-templates-description', '');
    setInputValue('dns-templates-sort-order', '0');

    const isActive = document.getElementById('dns-templates-is-active') as HTMLInputElement | null;
    const isDefault = document.getElementById('dns-templates-is-default') as HTMLInputElement | null;
    if (isActive) {
        isActive.checked = true;
    }
    if (isDefault) {
        isDefault.checked = false;
    }

    showModal('dns-templates-edit-modal');
}

function openEditTemplate(id: number): void {
    const template = templates.find((item) => item.id === id);
    if (!template) {
        return;
    }

    editingTemplateId = id;
    setText('dns-templates-edit-title', 'Edit Template');

    setInputValue('dns-templates-name', template.name);
    setInputValue('dns-templates-description', template.description || '');
    setInputValue('dns-templates-sort-order', String(template.sortOrder ?? 0));

    const isActive = document.getElementById('dns-templates-is-active') as HTMLInputElement | null;
    const isDefault = document.getElementById('dns-templates-is-default') as HTMLInputElement | null;
    if (isActive) {
        isActive.checked = template.isActive;
    }
    if (isDefault) {
        isDefault.checked = template.isDefault;
    }

    showModal('dns-templates-edit-modal');
}

async function saveTemplate(): Promise<void> {
    const name = getInputValue('dns-templates-name');
    if (!name) {
        showError('Template name is required.');
        return;
    }

    const payload = {
        name,
        description: getInputValue('dns-templates-description') || null,
        sortOrder: getNumberValue('dns-templates-sort-order'),
        isActive: isChecked('dns-templates-is-active'),
        isDefault: isChecked('dns-templates-is-default'),
    };

    const response = editingTemplateId
        ? await apiRequest(`${getApiBaseUrl()}/DnsZonePackages/${editingTemplateId}`, { method: 'PUT', body: JSON.stringify(payload) })
        : await apiRequest(`${getApiBaseUrl()}/DnsZonePackages`, { method: 'POST', body: JSON.stringify(payload) });

    if (!response.success) {
        showError(response.message || 'Failed to save template.');
        return;
    }

    hideModal('dns-templates-edit-modal');
    showSuccess(editingTemplateId ? 'Template updated.' : 'Template created.');

    await loadTemplates();
}

function openDeleteTemplate(id: number): void {
    const template = templates.find((item) => item.id === id);
    if (!template) {
        return;
    }

    pendingTemplateDeleteId = id;
    setText('dns-templates-delete-name', template.name || `#${id}`);
    showModal('dns-templates-delete-modal');
}

async function deleteTemplate(): Promise<void> {
    if (!pendingTemplateDeleteId) {
        return;
    }

    const response = await apiRequest(`${getApiBaseUrl()}/DnsZonePackages/${pendingTemplateDeleteId}`, { method: 'DELETE' });
    hideModal('dns-templates-delete-modal');

    if (!response.success) {
        showError(response.message || 'Failed to delete template.');
        return;
    }

    if (selectedTemplateId === pendingTemplateDeleteId) {
        selectedTemplateId = null;
    }

    pendingTemplateDeleteId = null;
    showSuccess('Template deleted.');
    await loadTemplates();
}

function openCreateRecord(): void {
    if (!selectedTemplateId) {
        showError('Select a template first.');
        return;
    }

    editingRecordId = null;
    setText('dns-templates-record-edit-title', 'New Template Record');

    const defaultTypeId = recordTypes[0]?.id?.toString() ?? '';
    const defaultTtl = recordTypes[0]?.defaultTTL?.toString() ?? '3600';

    setSelectValue('dns-templates-record-type', defaultTypeId);
    setInputValue('dns-templates-record-name', '');
    setInputValue('dns-templates-record-value', '');
    setInputValue('dns-templates-record-ttl', defaultTtl);
    setInputValue('dns-templates-record-priority', '');
    setInputValue('dns-templates-record-weight', '');
    setInputValue('dns-templates-record-port', '');
    setInputValue('dns-templates-record-notes', '');
    setSelectValue('dns-templates-record-value-source', 'Manual');

    refreshARecordNameSuggestions();
    updateRecordFieldVisibility();
    showModal('dns-templates-record-edit-modal');
}

function openEditRecord(id: number): void {
    const record = templateRecords.find((item) => item.id === id);
    if (!record) {
        return;
    }

    const type = recordTypes.find((item) => item.id === record.dnsRecordTypeId);
    if (type && type.isEditableByUser === false) {
        showError('This DNS record type is not editable.');
        return;
    }

    editingRecordId = id;
    setText('dns-templates-record-edit-title', 'Edit Template Record');

    setSelectValue('dns-templates-record-type', String(record.dnsRecordTypeId));
    setInputValue('dns-templates-record-name', record.name || '');
    setInputValue('dns-templates-record-value', record.value || '');
    setInputValue('dns-templates-record-ttl', String(record.ttl ?? 3600));
    setInputValue('dns-templates-record-priority', record.priority != null ? String(record.priority) : '');
    setInputValue('dns-templates-record-weight', record.weight != null ? String(record.weight) : '');
    setInputValue('dns-templates-record-port', record.port != null ? String(record.port) : '');
    setInputValue('dns-templates-record-notes', record.notes || '');
    setSelectValue('dns-templates-record-value-source', record.valueSourceType || 'Manual');

    refreshARecordNameSuggestions();
    updateRecordFieldVisibility();
    showModal('dns-templates-record-edit-modal');
}

async function saveRecord(): Promise<void> {
    if (!selectedTemplateId) {
        return;
    }

    const dnsRecordTypeId = getSelectNumberValue('dns-templates-record-type');
    const name = getInputValue('dns-templates-record-name');
    const value = getInputValue('dns-templates-record-value');
    const valueSourceType = getSelectValue('dns-templates-record-value-source') || 'Manual';

    if (!dnsRecordTypeId || !name) {
        showError('Record type and name are required.');
        return;
    }

    const selectedType = recordTypes.find((item) => item.id === dnsRecordTypeId);
    const isAType = (selectedType?.type || '').trim().toUpperCase() === 'A';

    if (valueSourceType === 'Manual' && !value) {
        showError('Value is required for manual value source.');
        return;
    }

    if (isAType && valueSourceType === 'Manual' && !isValidIpAddress(value)) {
        showError('For A records, Value must be a valid IPv4 or IPv6 address.');
        return;
    }

    const payload = {
        dnsZonePackageId: selectedTemplateId,
        dnsRecordTypeId,
        name,
        value,
        valueSourceType,
        valueSourceReference: null,
        ttl: getNumberValue('dns-templates-record-ttl') || 3600,
        priority: getNullableNumberValue('dns-templates-record-priority'),
        weight: getNullableNumberValue('dns-templates-record-weight'),
        port: getNullableNumberValue('dns-templates-record-port'),
        notes: getInputValue('dns-templates-record-notes') || null,
    };

    const response = editingRecordId
        ? await apiRequest(`${getApiBaseUrl()}/DnsZonePackageRecords/${editingRecordId}`, { method: 'PUT', body: JSON.stringify(payload) })
        : await apiRequest(`${getApiBaseUrl()}/DnsZonePackageRecords`, { method: 'POST', body: JSON.stringify(payload) });

    if (!response.success) {
        showError(response.message || 'Failed to save template record.');
        return;
    }

    hideModal('dns-templates-record-edit-modal');
    showSuccess(editingRecordId ? 'Template record updated.' : 'Template record created.');

    if (selectedTemplateId) {
        await selectTemplate(selectedTemplateId);
    }
}

function openDeleteRecord(id: number): void {
    const record = templateRecords.find((item) => item.id === id);
    if (!record) {
        return;
    }

    pendingRecordDeleteId = id;
    setText('dns-templates-record-delete-name', `${getRecordTypeLabel(record.dnsRecordTypeId)} ${record.name}`);
    showModal('dns-templates-record-delete-modal');
}

async function deleteRecord(): Promise<void> {
    if (!pendingRecordDeleteId) {
        return;
    }

    const response = await apiRequest(`${getApiBaseUrl()}/DnsZonePackageRecords/${pendingRecordDeleteId}`, { method: 'DELETE' });
    hideModal('dns-templates-record-delete-modal');

    if (!response.success) {
        showError(response.message || 'Failed to delete template record.');
        return;
    }

    pendingRecordDeleteId = null;
    showSuccess('Template record deleted.');

    if (selectedTemplateId) {
        await selectTemplate(selectedTemplateId);
    }
}

async function assignServer(): Promise<void> {
    if (!selectedTemplateId) {
        return;
    }

    const serverId = getSelectNumberValue('dns-templates-server-select');
    if (!serverId) {
        showError('Select a server to assign.');
        return;
    }

    const response = await apiRequest(`${getApiBaseUrl()}/DnsZonePackages/${selectedTemplateId}/servers/${serverId}`, { method: 'POST' });
    if (!response.success) {
        showError(response.message || 'Failed to assign server.');
        return;
    }

    showSuccess('Server assigned.');
    await selectTemplate(selectedTemplateId);
}

async function removeServer(serverId: number): Promise<void> {
    if (!selectedTemplateId) {
        return;
    }

    const response = await apiRequest(`${getApiBaseUrl()}/DnsZonePackages/${selectedTemplateId}/servers/${serverId}`, { method: 'DELETE' });
    if (!response.success) {
        showError(response.message || 'Failed to remove server assignment.');
        return;
    }

    showSuccess('Server assignment removed.');
    await selectTemplate(selectedTemplateId);
}

async function assignControlPanel(): Promise<void> {
    if (!selectedTemplateId) {
        return;
    }

    const controlPanelId = getSelectNumberValue('dns-templates-panel-select');
    if (!controlPanelId) {
        showError('Select a control panel to assign.');
        return;
    }

    const response = await apiRequest(`${getApiBaseUrl()}/DnsZonePackages/${selectedTemplateId}/control-panels/${controlPanelId}`, { method: 'POST' });
    if (!response.success) {
        showError(response.message || 'Failed to assign control panel.');
        return;
    }

    showSuccess('Control panel assigned.');
    await selectTemplate(selectedTemplateId);
}

async function removeControlPanel(controlPanelId: number): Promise<void> {
    if (!selectedTemplateId) {
        return;
    }

    const response = await apiRequest(`${getApiBaseUrl()}/DnsZonePackages/${selectedTemplateId}/control-panels/${controlPanelId}`, { method: 'DELETE' });
    if (!response.success) {
        showError(response.message || 'Failed to remove control panel assignment.');
        return;
    }

    showSuccess('Control panel assignment removed.');
    await selectTemplate(selectedTemplateId);
}

function updateRecordFieldVisibility(): void {
    const typeId = getSelectNumberValue('dns-templates-record-type');
    const type = recordTypes.find((item) => item.id === typeId);
    const typeName = (type?.type || '').trim().toUpperCase();

    const showPriority = (type?.hasPriority ?? false) || typeName === 'MX' || typeName === 'SRV';
    const showWeight = (type?.hasWeight ?? false) || typeName === 'SRV';
    const showPort = (type?.hasPort ?? false) || typeName === 'SRV';

    toggleField('dns-templates-field-priority', showPriority);
    toggleField('dns-templates-field-weight', showWeight);
    toggleField('dns-templates-field-port', showPort);

    configureRecordValueSourceOptions(type);
    configureRecordValueInput(type);
}

function configureRecordValueSourceOptions(type: DnsRecordType | undefined): void {
    const wrapper = document.getElementById('dns-templates-record-value-source-wrapper');
    const select = document.getElementById('dns-templates-record-value-source') as HTMLSelectElement | null;
    const help = document.getElementById('dns-templates-record-value-source-help');
    if (!wrapper || !select) {
        return;
    }

    const current = select.value || 'Manual';
    const typeName = (type?.type || '').trim().toUpperCase();

    if (typeName === 'A') {
        wrapper.classList.remove('d-none');
        select.innerHTML =
            '<option value="Manual">Manual</option>' +
            '<option value="ServerIp">Use server IP</option>' +
            '<option value="PanelIp">Use panel IP</option>';
        select.value = ['Manual', 'ServerIp', 'PanelIp'].includes(current) ? current : 'Manual';
        if (help) {
            help.textContent = 'Choose if A record value should be entered manually or resolved from assigned server/panel.';
        }
        onRecordValueSourceChanged();
        return;
    }

    if (typeName === 'CNAME' || typeName === 'MX') {
        wrapper.classList.remove('d-none');
        select.innerHTML =
            '<option value="Manual">Manual</option>' +
            '<option value="ServerHost">Use server host</option>' +
            '<option value="PanelHost">Use panel host</option>';
        select.value = ['Manual', 'ServerHost', 'PanelHost'].includes(current) ? current : 'Manual';
        if (help) {
            help.textContent = typeName === 'MX'
                ? 'Choose if MX value should be entered manually or resolved from assigned server/panel host.'
                : 'Choose if CNAME value should be entered manually or resolved from assigned server/panel host.';
        }
        onRecordValueSourceChanged();
        return;
    }

    wrapper.classList.add('d-none');
    select.innerHTML = '<option value="Manual">Manual</option>';
    select.value = 'Manual';
    onRecordValueSourceChanged();
}

function onRecordValueSourceChanged(): void {
    const input = document.getElementById('dns-templates-record-value') as HTMLInputElement | null;
    const valueSource = getSelectValue('dns-templates-record-value-source') || 'Manual';
    if (!input) {
        return;
    }

    const isManual = valueSource === 'Manual';
    input.readOnly = !isManual;
    input.classList.toggle('bg-light', !isManual);
}

function configureRecordValueInput(type: DnsRecordType | undefined): void {
    const input = document.getElementById('dns-templates-record-value') as HTMLInputElement | null;
    const help = document.getElementById('dns-templates-record-value-help');
    if (!input) {
        return;
    }

    const isAType = (type?.type || '').trim().toUpperCase() === 'A';
    const upperType = (type?.type || '').trim().toUpperCase();
    const isCnameType = upperType === 'CNAME';
    const isMxType = upperType === 'MX';
    if (isAType) {
        input.setAttribute('placeholder', 'e.g. 203.0.113.10');
        input.removeAttribute('list');
        if (help) {
            help.textContent = 'For A records, enter a valid IPv4 or IPv6 address.';
        }
        return;
    }

    if (isCnameType) {
        input.setAttribute('placeholder', 'e.g. target.example.com');
        input.setAttribute('list', 'dns-templates-a-record-name-list');
        if (help) {
            help.textContent = 'For CNAME, use host name, select A-record name suggestion, or dynamic source.';
        }
        return;
    }

    if (isMxType) {
        input.setAttribute('placeholder', 'e.g. mail.example.com');
        input.setAttribute('list', 'dns-templates-a-record-name-list');
        if (help) {
            help.textContent = 'For MX, use mail host, select A-record name suggestion, or dynamic source.';
        }
        return;
    }

    input.setAttribute('placeholder', 'Select A-record name or type manually');
    input.setAttribute('list', 'dns-templates-a-record-name-list');
    if (help) {
        help.textContent = 'For non-A records, select an A-record name from suggestions or type manually.';
    }
}

function getSelectValue(id: string): string {
    const input = document.getElementById(id) as HTMLSelectElement | null;
    return (input?.value ?? '').trim();
}

function refreshARecordNameSuggestions(): void {
    const list = document.getElementById('dns-templates-a-record-name-list');
    if (!list) {
        return;
    }

    const aType = recordTypes.find((item) => (item.type || '').trim().toUpperCase() === 'A');
    const aTypeId = aType?.id ?? 0;

    const names = new Set<string>();
    templateRecords
        .filter((record) => record.dnsRecordTypeId === aTypeId)
        .forEach((record) => {
            const name = (record.name || '').trim();
            if (name) {
                names.add(name);
            }
        });

    list.innerHTML = Array.from(names)
        .sort((a, b) => a.localeCompare(b))
        .map((name) => `<option value="${esc(name)}"></option>`)
        .join('');
}

function isValidIpAddress(value: string): boolean {
    const ipv4 = /^(25[0-5]|2[0-4]\d|1?\d?\d)(\.(25[0-5]|2[0-4]\d|1?\d?\d)){3}$/;
    const ipv6 = /^([\da-fA-F]{1,4}:){1,7}[\da-fA-F]{1,4}$|^::1$|^::$/;
    return ipv4.test(value) || ipv6.test(value);
}

function toggleField(id: string, visible: boolean): void {
    const element = document.getElementById(id);
    if (!element) {
        return;
    }

    element.classList.toggle('d-none', !visible);
}

function getRecordTypeLabel(id: number): string {
    const match = recordTypes.find((item) => item.id === id);
    return match?.type || `Type #${id}`;
}

function clearSelectionUi(): void {
    selectedTemplate = null;
    selectedTemplateId = null;
    templateRecords = [];
    assignedServers = [];
    assignedPanels = [];

    renderTemplates();
    renderSelectedTemplateInfo();
    renderRecords();
    renderAssignments();
    updateSelectionDependentControls();
}

function showSuccess(message: string): void {
    const alert = document.getElementById('dns-templates-alert-success');
    if (!alert) {
        return;
    }

    alert.textContent = message;
    alert.classList.remove('d-none');
    document.getElementById('dns-templates-alert-error')?.classList.add('d-none');
}

function showError(message: string): void {
    const alert = document.getElementById('dns-templates-alert-error');
    if (!alert) {
        return;
    }

    alert.textContent = message;
    alert.classList.remove('d-none');
    document.getElementById('dns-templates-alert-success')?.classList.add('d-none');
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
    const raw = getInputValue(id);
    const parsed = Number(raw);
    return Number.isFinite(parsed) ? parsed : 0;
}

function getNullableNumberValue(id: string): number | null {
    const raw = getInputValue(id);
    if (!raw) {
        return null;
    }

    const parsed = Number(raw);
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

function esc(text: string): string {
    const map: Record<string, string> = { '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#039;' };
    return (text || '').toString().replace(/[&<>"']/g, (char) => map[char]);
}

function showModal(id: string): void {
    const element = document.getElementById(id);
    if (!element || !(window as any).bootstrap) {
        return;
    }

    const modal = new (window as any).bootstrap.Modal(element);
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

function setupPageObserver(): void {
    initializePage();

    if (document.body) {
        const observer = new MutationObserver(() => {
            const page = document.getElementById('dns-templates-page') as HTMLElement | null;
            if (page && page.dataset.initialized !== 'true') {
                initializePage();
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

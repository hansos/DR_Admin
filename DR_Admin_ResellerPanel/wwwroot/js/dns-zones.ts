// @ts-nocheck
(function() {
interface Domain {
    id: number;
    name: string;
}

interface DnsRecord {
    id: number;
    domainId: number;
    dnsRecordTypeId?: number;
    type: string;
    name: string;
    value: string;
    ttl: number;
    priority?: number | null;
    weight?: number | null;
    port?: number | null;
    isEditableByUser?: boolean;
    hasPriority?: boolean;
    hasWeight?: boolean;
    hasPort?: boolean;
}

interface DnsRecordType {
    id: number;
    type: string;
    description?: string;
    hasPriority: boolean;
    hasWeight: boolean;
    hasPort: boolean;
    isEditableByUser: boolean;
    isActive?: boolean;
    defaultTTL?: number;
}

interface ApiResponse<T> {
    success: boolean;
    data?: T;
    message?: string;
}

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
        console.error('DNS zones request failed', error);
        return {
            success: false,
            message: 'Network error. Please try again.',
        };
    }
}

let selectedDomainId: number | null = null;
let selectedDomainName: string | null = null;
let records: DnsRecord[] = [];
let recordTypes: DnsRecordType[] = [];
let pendingRecords: DnsRecord[] = [];
let editingRecordId: number | null = null;
let pendingDeleteId: number | null = null;

function initializePage(): void {
    const page = document.getElementById('dns-zones-page') as HTMLElement | null;
    if (!page || (page as any).dataset.initialized === 'true') {
        return;
    }

    (page as any).dataset.initialized = 'true';

    document.getElementById('dns-zones-select')?.addEventListener('click', openSelectDomainModal);
    document.getElementById('dns-zones-domain-load')?.addEventListener('click', loadDomainFromModal);
    document.getElementById('dns-zones-add')?.addEventListener('click', openCreate);
    document.getElementById('dns-zones-save')?.addEventListener('click', saveRecord);
    document.getElementById('dns-zones-confirm-delete')?.addEventListener('click', deleteRecord);
    document.getElementById('dns-zones-record-type')?.addEventListener('change', updateFieldVisibility);
    document.getElementById('dns-zones-sync')?.addEventListener('click', openSyncModal);
    document.getElementById('dns-zones-sync-confirm')?.addEventListener('click', performSync);

    const input = document.getElementById('dns-zones-domain-name') as HTMLInputElement | null;
    input?.addEventListener('keydown', (event) => {
        if (event.key === 'Enter') {
            event.preventDefault();
            loadDomainFromModal();
        }
    });

    bindTableActions();

    loadDnsRecordTypes();

    const params = new URLSearchParams(window.location.search);
    const domainIdParam = params.get('domain-id');
    const parsed = Number(domainIdParam);
    if (domainIdParam && Number.isFinite(parsed) && parsed > 0) {
        loadDomainById(parsed);
    } else {
        showNoSelection();
        openSelectDomainModal();
    }
}

async function loadDnsRecordTypes(): Promise<void> {
    const response = await apiRequest<DnsRecordType[]>(`${getApiBaseUrl()}/DnsRecordTypes`, { method: 'GET' });
    if (!response.success) {
        showError(response.message || 'Failed to load DNS record types.');
        return;
    }

    const raw = response.data as any;
    const items = Array.isArray(raw)
        ? raw
        : Array.isArray(raw?.data)
            ? raw.data
            : Array.isArray(raw?.Data)
                ? raw.Data
                : [];

    recordTypes = items.map((item: any) => ({
        id: item.id ?? item.Id ?? 0,
        type: item.type ?? item.Type ?? '',
        description: item.description ?? item.Description ?? '',
        hasPriority: item.hasPriority ?? item.HasPriority ?? false,
        hasWeight: item.hasWeight ?? item.HasWeight ?? false,
        hasPort: item.hasPort ?? item.HasPort ?? false,
        isEditableByUser: item.isEditableByUser ?? item.IsEditableByUser ?? true,
        isActive: item.isActive ?? item.IsActive ?? true,
        defaultTTL: item.defaultTTL ?? item.DefaultTTL ?? undefined,
    }));

    renderRecordTypeOptions();
}

function renderRecordTypeOptions(): void {
    const select = document.getElementById('dns-zones-record-type') as HTMLSelectElement | null;
    if (!select) {
        return;
    }

    const activeTypes = recordTypes.filter((type) => type.type && type.isActive !== false);
    activeTypes.sort((a, b) => a.type.localeCompare(b.type));

    select.innerHTML = activeTypes
        .map((type) => `<option value="${type.id}">${esc(type.type)}</option>`)
        .join('');
}

function bindTableActions(): void {
    const tableBody = document.getElementById('dns-zones-table-body');
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
            openEdit(id);
            return;
        }

        if (button.dataset.action === 'delete') {
            openDelete(id);
        }
    });
}

async function loadDomainById(domainId: number): Promise<void> {
    clearAlerts();
    setLoading(true);

    const response = await apiRequest<Domain>(`${getApiBaseUrl()}/RegisteredDomains/${domainId}`, { method: 'GET' });
    if (!response.success || !response.data) {
        setLoading(false);
        showError(response.message || 'Failed to load domain.');
        showNoSelection();
        openSelectDomainModal();
        return;
    }

    const domain = normalizeDomain(response.data);
    setSelectedDomain(domain);
    await loadRecords();
}

async function loadDomainFromModal(): Promise<void> {
    const input = document.getElementById('dns-zones-domain-name') as HTMLInputElement | null;
    const name = (input?.value ?? '').trim();

    if (!name) {
        showError('Enter a domain name to load.');
        return;
    }

    clearAlerts();
    setLoading(true);

    const response = await apiRequest<Domain>(`${getApiBaseUrl()}/RegisteredDomains/name/${encodeURIComponent(name)}`, { method: 'GET' });
    if (!response.success || !response.data) {
        setLoading(false);
        showError(response.message || 'Domain not found.');
        return;
    }

    const domain = normalizeDomain(response.data);
    setSelectedDomain(domain);
    hideModal('dns-zones-select-modal');
    await loadRecords();
}

function setSelectedDomain(domain: Domain): void {
    selectedDomainId = domain.id;
    selectedDomainName = domain.name || `Domain #${domain.id}`;

    setText('dns-zones-selected-domain', selectedDomainName);
    setText('dns-zones-selected-id', String(domain.id));
    setSelectButtonLabel(selectedDomainName);
    updateSyncButtonState();
    updateAddButtonState();
}

function normalizeDomain(raw: any): Domain {
    return {
        id: raw.id ?? raw.Id ?? 0,
        name: raw.name ?? raw.Name ?? raw.domainName ?? '',
    };
}

async function loadRecords(): Promise<void> {
    if (!selectedDomainId) {
        showNoSelection();
        return;
    }

    setLoading(true);

    const [activeResponse, deletedResponse] = await Promise.all([
        apiRequest<DnsRecord[]>(`${getApiBaseUrl()}/DnsRecords/domain/${selectedDomainId}`, { method: 'GET' }),
        apiRequest<DnsRecord[]>(`${getApiBaseUrl()}/DnsRecords/domain/${selectedDomainId}/deleted`, { method: 'GET' }),
    ]);

    if (!activeResponse.success) {
        setLoading(false);
        showError(activeResponse.message || 'Failed to load DNS records.');
        return;
    }

    const activeRaw = activeResponse.data as any;
    const activeRecords = Array.isArray(activeRaw)
        ? activeRaw
        : Array.isArray(activeRaw?.data)
            ? activeRaw.data
            : Array.isArray(activeRaw?.Data)
                ? activeRaw.Data
                : [];

    const deletedRaw = deletedResponse.success ? (deletedResponse.data as any) : [];
    const deletedRecords = Array.isArray(deletedRaw)
        ? deletedRaw
        : Array.isArray(deletedRaw?.data)
            ? deletedRaw.data
            : Array.isArray(deletedRaw?.Data)
                ? deletedRaw.Data
                : [];

    records = [...activeRecords, ...deletedRecords];

    renderRecords();
    setLoading(false);
    updatePendingSyncBadge();
    updateSyncButtonState();
    updateAddButtonState();
}

function renderRecords(): void {
    const tableBody = document.getElementById('dns-zones-table-body');
    if (!tableBody) {
        return;
    }

    setText('dns-zones-record-count', `${records.length} record${records.length === 1 ? '' : 's'}`);

    if (!records.length) {
        tableBody.innerHTML = '';
        showEmpty();
        return;
    }

    tableBody.innerHTML = records.map((record) => {
        const isDeleted = record.isDeleted === true;
        const editable = record.isEditableByUser !== false && !isDeleted;
        const rowClass = isDeleted ? 'table-danger' : editable ? '' : 'table-warning';
        const lockBadge = editable ? '' : ' <span class="badge bg-secondary" title="Not editable"><i class="bi bi-lock"></i></span>';
        const pendingBadge = isDeleted ? ' <span class="badge bg-danger" title="Pending sync"><i class="bi bi-clock"></i> Pending</span>' : '';
        return `
        <tr class="${rowClass}">
            <td>${esc(record.type || '-')}${lockBadge}${pendingBadge}</td>
            <td>${esc(record.name || '-')}</td>
            <td>${esc(record.value || '-')}</td>
            <td>${record.ttl ?? '-'}</td>
            <td>${record.priority ?? '-'}</td>
            <td>${record.weight ?? '-'}</td>
            <td>${record.port ?? '-'}</td>
            <td class="text-end">
                <div class="btn-group btn-group-sm">
                    <button class="btn btn-outline-primary" type="button" data-action="edit" data-id="${record.id}" title="Edit" ${editable ? '' : 'disabled'}>
                        <i class="bi bi-pencil"></i>
                    </button>
                    <button class="btn btn-outline-danger" type="button" data-action="delete" data-id="${record.id}" title="Delete" ${editable ? '' : 'disabled'}>
                        <i class="bi bi-trash"></i>
                    </button>
                </div>
            </td>
        </tr>`;
    }).join('');

    showTable();
}

function openCreate(): void {
    if (!selectedDomainId) {
        showError('Select a domain before adding DNS records.');
        return;
    }

    editingRecordId = null;

    const defaultTypeId = recordTypes[0]?.id?.toString() ?? '';
    const defaultTtl = recordTypes[0]?.defaultTTL?.toString() ?? '3600';

    setSelectValue('dns-zones-record-type', defaultTypeId);
    setInputValue('dns-zones-record-name', '');
    setInputValue('dns-zones-record-value', '');
    setInputValue('dns-zones-record-ttl', defaultTtl);
    setInputValue('dns-zones-record-priority', '');
    setInputValue('dns-zones-record-weight', '');
    setInputValue('dns-zones-record-port', '');

    updateFieldVisibility();
    showModal('dns-zones-edit-modal');
}

function openEdit(id: number): void {
    const record = records.find((item) => item.id === id);
    if (!record) {
        return;
    }

    if (record.isEditableByUser === false) {
        showError('This DNS record type is not editable.');
        return;
    }

    editingRecordId = id;

    setSelectValue('dns-zones-record-type', record.dnsRecordTypeId?.toString() ?? getRecordTypeIdByName(record.type)?.toString() ?? '');
    setInputValue('dns-zones-record-name', record.name);
    setInputValue('dns-zones-record-value', record.value);
    setInputValue('dns-zones-record-ttl', record.ttl?.toString() ?? '');
    setInputValue('dns-zones-record-priority', record.priority?.toString() ?? '');
    setInputValue('dns-zones-record-weight', record.weight?.toString() ?? '');
    setInputValue('dns-zones-record-port', record.port?.toString() ?? '');

    updateFieldVisibility();

    showModal('dns-zones-edit-modal');
}

async function saveRecord(): Promise<void> {
    if (!selectedDomainId) {
        return;
    }

    const dnsRecordTypeId = getSelectNumberValue('dns-zones-record-type');
    if (!dnsRecordTypeId) {
        showError('Select a DNS record type.');
        return;
    }

    const payload = {
        domainId: selectedDomainId,
        dnsRecordTypeId,
        name: getInputValue('dns-zones-record-name'),
        value: getInputValue('dns-zones-record-value'),
        ttl: getNumberValue('dns-zones-record-ttl'),
        priority: getNullableNumberValue('dns-zones-record-priority'),
        weight: getNullableNumberValue('dns-zones-record-weight'),
        port: getNullableNumberValue('dns-zones-record-port'),
    };

    const response = editingRecordId
        ? await apiRequest(`${getApiBaseUrl()}/DnsRecords/${editingRecordId}`, {
            method: 'PUT',
            body: JSON.stringify(payload),
        })
        : await apiRequest(`${getApiBaseUrl()}/DnsRecords`, {
            method: 'POST',
            body: JSON.stringify(payload),
        });

    if (!response.success) {
        showError(response.message || 'Failed to save DNS record.');
        return;
    }

    hideModal('dns-zones-edit-modal');
    showSuccess(editingRecordId ? 'DNS record updated successfully.' : 'DNS record created successfully.');
    await loadRecords();
}

function openDelete(id: number): void {
    const record = records.find((item) => item.id === id);
    if (!record) {
        return;
    }

    if (record.isEditableByUser === false) {
        showError('This DNS record type cannot be deleted.');
        return;
    }

    pendingDeleteId = id;
    const label = `${record.type} ${record.name || '@'}`;
    setText('dns-zones-delete-name', label);

    showModal('dns-zones-delete-modal');
}

async function deleteRecord(): Promise<void> {
    if (!pendingDeleteId) {
        return;
    }

    const response = await apiRequest(`${getApiBaseUrl()}/DnsRecords/${pendingDeleteId}`, { method: 'DELETE' });
    hideModal('dns-zones-delete-modal');

    if (!response.success) {
        showError(response.message || 'Failed to delete DNS record.');
        return;
    }

    showSuccess('DNS record deleted successfully.');
    pendingDeleteId = null;
    await loadRecords();
}

function setLoading(isLoading: boolean): void {
    const loading = document.getElementById('dns-zones-loading');
    const tableWrapper = document.getElementById('dns-zones-table-wrapper');
    const empty = document.getElementById('dns-zones-empty');
    const noSelection = document.getElementById('dns-zones-no-selection');

    if (loading) {
        loading.classList.toggle('d-none', !isLoading);
    }

    if (isLoading) {
        tableWrapper?.classList.add('d-none');
        empty?.classList.add('d-none');
        noSelection?.classList.add('d-none');
    }
}

function showNoSelection(): void {
    setLoading(false);
    document.getElementById('dns-zones-no-selection')?.classList.remove('d-none');
    setText('dns-zones-selected-domain', 'No domain selected');
    setText('dns-zones-selected-id', '-');
    setSelectButtonLabel(null);
    setText('dns-zones-record-count', '0 records');
    updateSyncButtonState();
    updateAddButtonState();
}

function updateSyncButtonState(): void {
    const button = document.getElementById('dns-zones-sync') as HTMLButtonElement | null;
    if (button) {
        button.disabled = !selectedDomainId;
    }
}

function updateAddButtonState(): void {
    const button = document.getElementById('dns-zones-add') as HTMLButtonElement | null;
    if (button) {
        button.disabled = !selectedDomainId;
    }
}

function updatePendingSyncBadge(): void {
    const badge = document.getElementById('dns-zones-pending-count');
    if (!badge) {
        return;
    }

    const count = records.filter((record) => record.isPendingSync).length;
    badge.textContent = String(count);
    badge.classList.toggle('d-none', count === 0);
}

async function openSyncModal(): Promise<void> {
    if (!selectedDomainId) {
        showError('Select a domain before syncing.');
        return;
    }

    const loading = document.getElementById('dns-zones-sync-loading');
    const empty = document.getElementById('dns-zones-sync-empty');
    const content = document.getElementById('dns-zones-sync-content');
    const progress = document.getElementById('dns-zones-sync-progress');
    const summary = document.getElementById('dns-zones-sync-summary');
    const confirmBtn = document.getElementById('dns-zones-sync-confirm') as HTMLButtonElement | null;
    const cancelBtn = document.getElementById('dns-zones-sync-cancel') as HTMLButtonElement | null;

    loading?.classList.remove('d-none');
    empty?.classList.add('d-none');
    content?.classList.add('d-none');
    progress?.classList.add('d-none');
    summary?.classList.add('d-none');
    if (confirmBtn) {
        confirmBtn.disabled = true;
        confirmBtn.innerHTML = '<i class="bi bi-arrow-repeat"></i> Sync All to DNS Server';
    }
    if (cancelBtn) {
        cancelBtn.disabled = false;
        cancelBtn.textContent = 'Cancel';
    }

    const bar = document.getElementById('dns-zones-sync-progress-bar') as HTMLElement | null;
    if (bar) {
        bar.style.width = '0%';
        bar.className = 'progress-bar progress-bar-striped progress-bar-animated bg-warning';
    }

    showModal('dns-zones-sync-modal');

    const response = await apiRequest<DnsRecord[]>(`${getApiBaseUrl()}/DnsRecords/domain/${selectedDomainId}/pending-sync`, { method: 'GET' });
    if (!response.success) {
        pendingRecords = [];
        showError(response.message || 'Failed to load pending records.');
    } else {
        const raw = response.data as any;
        pendingRecords = Array.isArray(raw)
            ? raw
            : Array.isArray(raw?.data)
                ? raw.data
                : Array.isArray(raw?.Data)
                    ? raw.Data
                    : [];
    }

    loading?.classList.add('d-none');

    if (!pendingRecords.length) {
        empty?.classList.remove('d-none');
        return;
    }

    setText('dns-zones-sync-count', String(pendingRecords.length));
    renderSyncTable();
    content?.classList.remove('d-none');
    if (confirmBtn) {
        confirmBtn.disabled = false;
    }
}

function renderSyncTable(): void {
    const body = document.getElementById('dns-zones-sync-table');
    if (!body) {
        return;
    }

    body.innerHTML = pendingRecords.map((record) => {
        return `
        <tr id="dns-zones-sync-row-${record.id}">
            <td>${esc(record.type || '-')}</td>
            <td><code>${esc(getFullDnsName(record.name, selectedDomainName ?? ''))}</code></td>
            <td class="text-break"><code class="small">${esc(record.value || '-')}</code></td>
            <td>${record.ttl ?? '-'}</td>
            <td><span class="badge bg-warning text-dark"><i class="bi bi-clock"></i> Pending</span></td>
        </tr>`;
    }).join('');
}

async function performSync(): Promise<void> {
    const confirmBtn = document.getElementById('dns-zones-sync-confirm') as HTMLButtonElement | null;
    const cancelBtn = document.getElementById('dns-zones-sync-cancel') as HTMLButtonElement | null;
    const progress = document.getElementById('dns-zones-sync-progress');
    const bar = document.getElementById('dns-zones-sync-progress-bar') as HTMLElement | null;
    const progressText = document.getElementById('dns-zones-sync-progress-text');
    const summary = document.getElementById('dns-zones-sync-summary');

    if (confirmBtn) {
        confirmBtn.disabled = true;
        confirmBtn.innerHTML = '<span class="spinner-border spinner-border-sm"></span> Syncing...';
    }
    if (cancelBtn) {
        cancelBtn.disabled = true;
    }
    progress?.classList.remove('d-none');
    summary?.classList.add('d-none');

    let succeeded = 0;
    let failed = 0;
    const total = pendingRecords.length;

    for (let i = 0; i < total; i++) {
        const record = pendingRecords[i];
        const pct = Math.round(((i + 1) / total) * 100);
        if (progressText) {
            progressText.textContent = `${i + 1} / ${total}`;
        }
        if (bar) {
            bar.style.width = `${pct}%`;
            bar.setAttribute('aria-valuenow', String(pct));
        }

        const row = document.getElementById(`dns-zones-sync-row-${record.id}`);
        try {
            const result = await apiRequest(`${getApiBaseUrl()}/DnsRecords/${record.id}/push`, { method: 'POST' });
            if (result.success) {
                succeeded++;
                if (row && row.cells[4]) {
                    row.cells[4].innerHTML = '<span class="badge bg-success"><i class="bi bi-check2"></i> Synced</span>';
                }
            } else {
                failed++;
                const errMsg = (result.data as any)?.message ?? result.message ?? 'Failed';
                if (row && row.cells[4]) {
                    row.cells[4].innerHTML = `<span class="badge bg-danger" title="${esc(errMsg)}"><i class="bi bi-x"></i> Failed</span>`;
                }
            }
        } catch (error) {
            failed++;
            if (row && row.cells[4]) {
                row.cells[4].innerHTML = '<span class="badge bg-danger"><i class="bi bi-x"></i> Error</span>';
            }
        }
    }

    if (bar) {
        bar.classList.remove('progress-bar-animated');
        bar.classList.toggle('bg-success', failed === 0);
    }

    if (summary) {
        if (failed === 0) {
            summary.className = 'mt-3 alert alert-success';
            summary.innerHTML = `<i class="bi bi-check-circle-fill"></i> All <strong>${succeeded}</strong> record(s) were successfully marked as synced.`;
        } else {
            summary.className = 'mt-3 alert alert-warning';
            summary.innerHTML = `<i class="bi bi-exclamation-triangle-fill"></i> <strong>${succeeded}</strong> record(s) synced, <strong>${failed}</strong> failed.`;
        }
        summary.classList.remove('d-none');
    }

    if (confirmBtn) {
        confirmBtn.innerHTML = '<i class="bi bi-check2-circle"></i> Done';
    }
    if (cancelBtn) {
        cancelBtn.disabled = false;
        cancelBtn.textContent = 'Close';
    }

    await loadRecords();
}

function getFullDnsName(name: string, domainName: string): string {
    if (!name || name === '@') {
        return domainName || '@';
    }

    if (!domainName) {
        return name;
    }

    if (name.endsWith(`.${domainName}`) || name === domainName) {
        return name;
    }

    return `${name}.${domainName}`;
}

function showEmpty(): void {
    document.getElementById('dns-zones-empty')?.classList.remove('d-none');
    document.getElementById('dns-zones-table-wrapper')?.classList.add('d-none');
}

function showTable(): void {
    document.getElementById('dns-zones-table-wrapper')?.classList.remove('d-none');
    document.getElementById('dns-zones-empty')?.classList.add('d-none');
    document.getElementById('dns-zones-no-selection')?.classList.add('d-none');
}

function openSelectDomainModal(): void {
    const input = document.getElementById('dns-zones-domain-name') as HTMLInputElement | null;
    if (input) {
        input.value = selectedDomainName ?? '';
    }

    showModal('dns-zones-select-modal');
    input?.focus();
}

function setSelectButtonLabel(domainName: string | null): void {
    const button = document.getElementById('dns-zones-select') as HTMLButtonElement | null;
    if (!button) {
        return;
    }

    button.innerHTML = domainName
        ? `<i class="bi bi-search"></i> ${esc(domainName)}`
        : '<i class="bi bi-search"></i> Select domain';
}

function setText(id: string, value: string): void {
    const el = document.getElementById(id);
    if (el) {
        el.textContent = value;
    }
}

function setInputValue(id: string, value: string): void {
    const el = document.getElementById(id) as HTMLInputElement | null;
    if (el) {
        el.value = value ?? '';
    }
}

function setSelectValue(id: string, value: string): void {
    const el = document.getElementById(id) as HTMLSelectElement | null;
    if (el) {
        el.value = value ?? '';
    }
}

function getInputValue(id: string): string {
    const el = document.getElementById(id) as HTMLInputElement | null;
    return (el?.value ?? '').trim();
}

function getSelectNumberValue(id: string): number {
    const el = document.getElementById(id) as HTMLSelectElement | null;
    const raw = (el?.value ?? '').trim();
    const parsed = Number(raw);
    return Number.isFinite(parsed) ? parsed : 0;
}

function getNumberValue(id: string): number {
    const value = getInputValue(id);
    const parsed = Number(value);
    return Number.isFinite(parsed) ? parsed : 0;
}

function updateFieldVisibility(): void {
    const dnsRecordTypeId = getSelectNumberValue('dns-zones-record-type');
    const type = recordTypes.find((item) => item.id === dnsRecordTypeId);

    toggleField('dns-zones-field-priority', type?.hasPriority ?? false);
    toggleField('dns-zones-field-weight', type?.hasWeight ?? false);
    toggleField('dns-zones-field-port', type?.hasPort ?? false);
}

function toggleField(id: string, visible: boolean): void {
    const field = document.getElementById(id);
    if (!field) {
        return;
    }

    field.classList.toggle('d-none', !visible);
}

function getRecordTypeIdByName(typeName: string): number | null {
    if (!typeName) {
        return null;
    }

    const match = recordTypes.find((item) => item.type.toLowerCase() === typeName.toLowerCase());
    return match?.id ?? null;
}

function getNullableNumberValue(id: string): number | null {
    const value = getInputValue(id);
    if (!value) {
        return null;
    }

    const parsed = Number(value);
    return Number.isFinite(parsed) ? parsed : null;
}

function showSuccess(message: string): void {
    const alert = document.getElementById('dns-zones-alert-success');
    if (!alert) {
        return;
    }

    alert.textContent = message;
    alert.classList.remove('d-none');
    document.getElementById('dns-zones-alert-error')?.classList.add('d-none');
}

function showError(message: string): void {
    const alert = document.getElementById('dns-zones-alert-error');
    if (!alert) {
        return;
    }

    alert.textContent = message;
    alert.classList.remove('d-none');
    document.getElementById('dns-zones-alert-success')?.classList.add('d-none');
}

function clearAlerts(): void {
    document.getElementById('dns-zones-alert-success')?.classList.add('d-none');
    document.getElementById('dns-zones-alert-error')?.classList.add('d-none');
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
            const page = document.getElementById('dns-zones-page') as HTMLElement | null;
            if (page && (page as any).dataset.initialized !== 'true') {
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

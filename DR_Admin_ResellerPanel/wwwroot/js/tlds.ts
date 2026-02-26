(function () {
interface Tld {
    id: number;
    extension: string;
    description: string;
    isActive: boolean;
    defaultRegistrationYears: number | null;
    maxRegistrationYears: number | null;
    requiresPrivacy: boolean;
    notes: string | null;
}

interface Registrar {
    id: number;
    name: string;
    code: string;
    isActive: boolean;
}

interface RegistrarTld {
    id: number;
    registrarId: number;
    tldId: number;
    tldExtension: string;
    isActive: boolean;
}

interface ApiResponse<T> {
    success: boolean;
    data?: T;
    message?: string;
}

interface BootstrapModal {
    show: () => void;
    hide: () => void;
}

interface BootstrapModalStatic {
    new (element: Element): BootstrapModal;
    getInstance: (element: Element) => BootstrapModal | null;
}

interface BootstrapGlobal {
    Modal: BootstrapModalStatic;
}

interface AppWindow extends Window {
    AppSettings?: { apiBaseUrl?: string };
    Auth?: { getToken?: () => string | null };
    bootstrap?: BootstrapGlobal;
}

interface CreateTldPayload {
    extension: string;
    description: string;
    isSecondLevel: boolean;
    isActive: boolean;
    defaultRegistrationYears: number | null;
    maxRegistrationYears: number | null;
    requiresPrivacy: boolean;
    notes: string | null;
}

interface UpdateTldPayload {
    extension: string;
    description: string;
    isActive: boolean;
    defaultRegistrationYears: number | null;
    maxRegistrationYears: number | null;
    requiresPrivacy: boolean;
    notes: string | null;
}

const w = window as AppWindow;

let allTlds: Tld[] = [];
let activeRegistrars: Registrar[] = [];
let editingId: number | null = null;
let pendingDeleteId: number | null = null;
let selectedRegistrarFilterId: number | null = null;
let currentPage = 1;
let pageSize = 25;
let totalCount = 0;
let totalPages = 1;

function getApiBaseUrl(): string {
    const baseUrl = w.AppSettings?.apiBaseUrl;
    if (!baseUrl) {
        return window.location.protocol === 'https:'
            ? 'https://localhost:7201/api/v1'
            : 'http://localhost:5133/api/v1';
    }

    return baseUrl;
}

function getAuthToken(): string | null {
    const token = w.Auth?.getToken?.();
    if (token) {
        return token;
    }

    return sessionStorage.getItem('rp_authToken');
}

function isRecord(value: unknown): value is Record<string, unknown> {
    return typeof value === 'object' && value !== null;
}

function getString(value: unknown): string {
    return typeof value === 'string' ? value : '';
}

function getBoolean(value: unknown, fallback: boolean = false): boolean {
    return typeof value === 'boolean' ? value : fallback;
}

function getNumber(value: unknown): number | null {
    return typeof value === 'number' && Number.isFinite(value) ? value : null;
}

function getListFromUnknown(raw: unknown): unknown[] {
    if (Array.isArray(raw)) {
        return raw;
    }

    if (!isRecord(raw)) {
        return [];
    }

    const candidates: unknown[] = [raw.Data, raw.data, raw.items, raw.Items];
    for (const candidate of candidates) {
        if (Array.isArray(candidate)) {
            return candidate;
        }
    }

    return [];
}

async function apiRequest<T>(endpoint: string, options: RequestInit = {}): Promise<ApiResponse<T>> {
    try {
        const headers: Record<string, string> = {
            'Content-Type': 'application/json',
            ...(options.headers as Record<string, string> | undefined),
        };

        const authToken = getAuthToken();
        if (authToken) {
            headers.Authorization = `Bearer ${authToken}`;
        }

        const response = await fetch(endpoint, {
            ...options,
            headers,
            credentials: 'include',
        });

        const contentType = response.headers.get('content-type') ?? '';
        const hasJson = contentType.includes('application/json');
        const data: unknown = hasJson ? await response.json() : null;

        if (!response.ok) {
            let message = `Request failed with status ${response.status}`;
            if (isRecord(data)) {
                const candidate = data.message ?? data.title;
                if (typeof candidate === 'string' && candidate.trim()) {
                    message = candidate;
                }
            }

            return {
                success: false,
                message,
            };
        }

        if (isRecord(data) && data.success === false) {
            return {
                success: false,
                message: getString(data.message) || 'Request failed',
            };
        }

        if (isRecord(data) && 'data' in data) {
            return {
                success: true,
                data: data.data as T,
                message: getString(data.message),
            };
        }

        return {
            success: true,
            data: data as T,
        };
    } catch (error) {
        console.error('TLD request failed', error);
        return {
            success: false,
            message: 'Network error. Please try again.',
        };
    }
}

function normalizeTld(item: unknown): Tld {
    if (!isRecord(item)) {
        return {
            id: 0,
            extension: '',
            description: '',
            isActive: true,
            defaultRegistrationYears: null,
            maxRegistrationYears: null,
            requiresPrivacy: false,
            notes: null,
        };
    }

    return {
        id: getNumber(item.id) ?? getNumber(item.Id) ?? 0,
        extension: getString(item.extension ?? item.Extension).replace(/^\./, ''),
        description: getString(item.description ?? item.Description),
        isActive: getBoolean(item.isActive ?? item.IsActive, true),
        defaultRegistrationYears: getNumber(item.defaultRegistrationYears ?? item.DefaultRegistrationYears),
        maxRegistrationYears: getNumber(item.maxRegistrationYears ?? item.MaxRegistrationYears),
        requiresPrivacy: getBoolean(item.requiresPrivacy ?? item.RequiresPrivacy),
        notes: getString(item.notes ?? item.Notes) || null,
    };
}

function normalizeRegistrar(item: unknown): Registrar {
    if (!isRecord(item)) {
        return {
            id: 0,
            name: '',
            code: '',
            isActive: false,
        };
    }

    return {
        id: getNumber(item.id) ?? getNumber(item.Id) ?? 0,
        name: getString(item.name ?? item.Name),
        code: getString(item.code ?? item.Code),
        isActive: getBoolean(item.isActive ?? item.IsActive),
    };
}

function normalizeRegistrarTld(item: unknown): RegistrarTld {
    if (!isRecord(item)) {
        return {
            id: 0,
            registrarId: 0,
            tldId: 0,
            tldExtension: '',
            isActive: false,
        };
    }

    return {
        id: getNumber(item.id) ?? getNumber(item.Id) ?? 0,
        registrarId: getNumber(item.registrarId ?? item.RegistrarId) ?? 0,
        tldId: getNumber(item.tldId ?? item.TldId) ?? 0,
        tldExtension: getString(item.tldExtension ?? item.TldExtension),
        isActive: getBoolean(item.isActive ?? item.IsActive),
    };
}

function extractPagedTldData(raw: unknown): { items: Tld[]; page: number; size: number; count: number; pages: number } {
    const items = getListFromUnknown(raw)
        .map(normalizeTld)
        .filter((item) => item.id > 0 && item.isActive);

    if (!isRecord(raw)) {
        const count = items.length;
        return {
            items,
            page: currentPage,
            size: pageSize,
            count,
            pages: Math.max(1, Math.ceil(count / pageSize)),
        };
    }

    const page = getNumber(raw.currentPage ?? raw.CurrentPage) ?? currentPage;
    const size = getNumber(raw.pageSize ?? raw.PageSize) ?? pageSize;
    const count = getNumber(raw.totalCount ?? raw.TotalCount) ?? items.length;
    const pages = getNumber(raw.totalPages ?? raw.TotalPages) ?? Math.max(1, Math.ceil((count || 1) / (size || 1)));

    return {
        items,
        page,
        size,
        count,
        pages,
    };
}

function extractPagedRegistrarTldData(raw: unknown): { items: RegistrarTld[]; page: number; size: number; count: number; pages: number } {
    const items = getListFromUnknown(raw)
        .map(normalizeRegistrarTld)
        .filter((item) => item.id > 0 && item.isActive);

    if (!isRecord(raw)) {
        const count = items.length;
        return {
            items,
            page: currentPage,
            size: pageSize,
            count,
            pages: Math.max(1, Math.ceil(count / pageSize)),
        };
    }

    const page = getNumber(raw.currentPage ?? raw.CurrentPage) ?? currentPage;
    const size = getNumber(raw.pageSize ?? raw.PageSize) ?? pageSize;
    const count = getNumber(raw.totalCount ?? raw.TotalCount) ?? items.length;
    const pages = getNumber(raw.totalPages ?? raw.TotalPages) ?? Math.max(1, Math.ceil((count || 1) / (size || 1)));

    return {
        items,
        page,
        size,
        count,
        pages,
    };
}

function esc(text: string): string {
    const map: Record<string, string> = {
        '&': '&amp;',
        '<': '&lt;',
        '>': '&gt;',
        '"': '&quot;',
        "'": '&#039;',
    };

    return (text || '').replace(/[&<>"']/g, (char) => map[char]);
}

function showSuccess(message: string): void {
    const alert = document.getElementById('tlds-alert-success');
    if (!alert) {
        return;
    }

    alert.textContent = message;
    alert.classList.remove('d-none');

    const errorAlert = document.getElementById('tlds-alert-error');
    errorAlert?.classList.add('d-none');

    setTimeout(() => alert.classList.add('d-none'), 5000);
}

function showError(message: string): void {
    const alert = document.getElementById('tlds-alert-error');
    if (!alert) {
        return;
    }

    alert.textContent = message;
    alert.classList.remove('d-none');

    const successAlert = document.getElementById('tlds-alert-success');
    successAlert?.classList.add('d-none');
}

function showModal(id: string): void {
    const element = document.getElementById(id);
    const bootstrapApi = w.bootstrap;
    if (!element || !bootstrapApi) {
        return;
    }

    const modal = new bootstrapApi.Modal(element);
    modal.show();
}

function hideModal(id: string): void {
    const element = document.getElementById(id);
    const bootstrapApi = w.bootstrap;
    if (!element || !bootstrapApi) {
        return;
    }

    const modal = bootstrapApi.Modal.getInstance(element);
    modal?.hide();
}

function readPageSize(): void {
    const input = document.getElementById('tlds-page-size') as HTMLSelectElement | null;
    const raw = input?.value ?? '';
    const parsed = Number(raw);

    if (Number.isFinite(parsed) && parsed > 0) {
        pageSize = parsed;
    }
}

function getRegistrarFilterId(): number | null {
    const input = document.getElementById('tlds-filter-registrar') as HTMLSelectElement | null;
    const raw = input?.value ?? '';
    if (!raw) {
        return null;
    }

    const parsed = Number(raw);
    return Number.isFinite(parsed) && parsed > 0 ? parsed : null;
}

function getSelectedRegistrarIdsFromModal(): number[] {
    const select = document.getElementById('tlds-registrars') as HTMLSelectElement | null;
    if (!select) {
        return [];
    }

    const values = Array.from(select.selectedOptions)
        .map((opt) => Number(opt.value))
        .filter((value) => Number.isFinite(value) && value > 0);

    return Array.from(new Set(values));
}

function renderRegistrarFilterOptions(): void {
    const select = document.getElementById('tlds-filter-registrar') as HTMLSelectElement | null;
    if (!select) {
        return;
    }

    const selectedValue = selectedRegistrarFilterId !== null ? String(selectedRegistrarFilterId) : '';
    const options = ['<option value="">All registrars</option>'];

    for (const registrar of activeRegistrars) {
        const display = registrar.code ? `${registrar.name} (${registrar.code})` : registrar.name;
        options.push(`<option value="${registrar.id}">${esc(display)}</option>`);
    }

    select.innerHTML = options.join('');
    select.value = selectedValue;
}

function renderRegistrarMultiSelect(selectedRegistrarIds: number[]): void {
    const select = document.getElementById('tlds-registrars') as HTMLSelectElement | null;
    if (!select) {
        return;
    }

    const selectedSet = new Set(selectedRegistrarIds);
    const options = activeRegistrars.map((registrar) => {
        const selected = selectedSet.has(registrar.id) ? ' selected' : '';
        const display = registrar.code ? `${registrar.name} (${registrar.code})` : registrar.name;
        return `<option value="${registrar.id}"${selected}>${esc(display)}</option>`;
    });

    select.innerHTML = options.join('');
}

function buildTldsUrl(): string {
    const params = new URLSearchParams();
    params.set('pageNumber', String(currentPage));
    params.set('pageSize', String(pageSize));

    if (selectedRegistrarFilterId !== null) {
        params.set('isActive', 'true');
        return `${getApiBaseUrl()}/RegistrarTlds/registrar/${selectedRegistrarFilterId}?${params.toString()}`;
    }

    return `${getApiBaseUrl()}/Tlds/active?${params.toString()}`;
}

function renderTable(): void {
    const tableBody = document.getElementById('tlds-table-body');
    if (!tableBody) {
        return;
    }

    if (!allTlds.length) {
        tableBody.innerHTML = '<tr><td colspan="7" class="text-center text-muted">No active TLDs found.</td></tr>';
        return;
    }

    tableBody.innerHTML = allTlds.map((item) => {
        const extension = item.extension.startsWith('.') ? item.extension : `.${item.extension}`;
        return `
        <tr>
            <td>${item.id}</td>
            <td><code>${esc(extension)}</code></td>
            <td>${esc(item.description || '-')}</td>
            <td>${item.defaultRegistrationYears ?? '-'}</td>
            <td>${item.maxRegistrationYears ?? '-'}</td>
            <td><span class="badge bg-${item.requiresPrivacy ? 'warning text-dark' : 'secondary'}">${item.requiresPrivacy ? 'Required' : 'Optional'}</span></td>
            <td>
                <div class="btn-group btn-group-sm">
                    <button class="btn btn-outline-primary" type="button" data-action="edit" data-id="${item.id}" title="Edit"><i class="bi bi-pencil"></i></button>
                    <button class="btn btn-outline-danger" type="button" data-action="delete" data-id="${item.id}" data-name="${esc(extension)}" title="Delete"><i class="bi bi-trash"></i></button>
                </div>
            </td>
        </tr>`;
    }).join('');
}

function renderPaginationInfo(): void {
    const info = document.getElementById('tlds-pagination-info');
    if (!info) {
        return;
    }

    if (!totalCount) {
        info.textContent = 'Showing 0 of 0';
        return;
    }

    const start = (currentPage - 1) * pageSize + 1;
    const end = Math.min(currentPage * pageSize, totalCount);
    info.textContent = `Showing ${start}-${end} of ${totalCount}`;
}

function renderPagingControls(): void {
    const list = document.getElementById('tlds-paging-controls-list');
    if (!list) {
        return;
    }

    if (!totalCount || totalPages <= 1) {
        list.innerHTML = '';
        return;
    }

    const makeItem = (label: string, page: number, disabled: boolean, active = false): string => {
        const cls = `page-item${disabled ? ' disabled' : ''}${active ? ' active' : ''}`;
        const ariaCurrent = active ? ' aria-current="page"' : '';
        const ariaDisabled = disabled ? ' aria-disabled="true" tabindex="-1"' : '';
        const dataPage = disabled ? '' : ` data-page="${page}"`;
        return `<li class="${cls}"><a class="page-link" href="#"${dataPage}${ariaCurrent}${ariaDisabled}>${label}</a></li>`;
    };

    const makeEllipsis = (): string => '<li class="page-item disabled"><span class="page-link">…</span></li>';
    let html = '';

    html += makeItem('Previous', currentPage - 1, currentPage <= 1);

    const pages = new Set<number>();
    pages.add(1);
    if (totalPages >= 2) {
        pages.add(2);
        pages.add(totalPages - 1);
    }
    pages.add(totalPages);

    for (let p = currentPage - 1; p <= currentPage + 1; p++) {
        if (p >= 1 && p <= totalPages) {
            pages.add(p);
        }
    }

    const sorted = Array.from(pages).sort((a, b) => a - b);
    let last = 0;

    for (const p of sorted) {
        if (last && p - last > 1) {
            html += makeEllipsis();
        }

        html += makeItem(String(p), p, false, p === currentPage);
        last = p;
    }

    html += makeItem('Next', currentPage + 1, currentPage >= totalPages);
    list.innerHTML = html;
}

async function loadActiveRegistrars(): Promise<void> {
    const response = await apiRequest<unknown>(`${getApiBaseUrl()}/Registrars/active`, { method: 'GET' });
    if (!response.success) {
        showError(response.message || 'Failed to load registrars');
        activeRegistrars = [];
        renderRegistrarFilterOptions();
        renderRegistrarMultiSelect([]);
        return;
    }

    activeRegistrars = getListFromUnknown(response.data)
        .map(normalizeRegistrar)
        .filter((item) => item.id > 0 && item.isActive)
        .sort((a, b) => a.name.localeCompare(b.name));

    renderRegistrarFilterOptions();
    renderRegistrarMultiSelect([]);
}

async function loadTlds(): Promise<void> {
    const tableBody = document.getElementById('tlds-table-body');
    if (!tableBody) {
        return;
    }

    tableBody.innerHTML = '<tr><td colspan="7" class="text-center"><div class="spinner-border text-primary"></div></td></tr>';

    readPageSize();
    const response = await apiRequest<unknown>(buildTldsUrl(), { method: 'GET' });

    if (!response.success) {
        showError(response.message || 'Failed to load active TLDs');
        tableBody.innerHTML = '<tr><td colspan="7" class="text-center text-danger">Failed to load data</td></tr>';
        return;
    }

    if (selectedRegistrarFilterId !== null) {
        const parsed = extractPagedRegistrarTldData(response.data);
        allTlds = parsed.items.map((item) => ({
            id: item.tldId,
            extension: item.tldExtension.replace(/^\./, ''),
            description: '',
            isActive: item.isActive,
            defaultRegistrationYears: null,
            maxRegistrationYears: null,
            requiresPrivacy: false,
            notes: null,
        }));

        currentPage = parsed.page;
        pageSize = parsed.size;
        totalCount = parsed.count;
        totalPages = parsed.pages;
    } else {
        const parsed = extractPagedTldData(response.data);
        allTlds = parsed.items;
        currentPage = parsed.page;
        pageSize = parsed.size;
        totalCount = parsed.count;
        totalPages = parsed.pages;
    }

    renderTable();
    renderPaginationInfo();
    renderPagingControls();
}

async function getAssignedRegistrarIds(tldId: number): Promise<number[]> {
    const response = await apiRequest<unknown>(`${getApiBaseUrl()}/RegistrarTlds/tld/${tldId}`, { method: 'GET' });
    if (!response.success) {
        return [];
    }

    const ids = getListFromUnknown(response.data)
        .map(normalizeRegistrarTld)
        .map((item) => item.registrarId)
        .filter((id) => id > 0);

    return Array.from(new Set(ids));
}

async function assignRegistrarToTld(registrarId: number, tldId: number): Promise<void> {
    const response = await apiRequest<unknown>(`${getApiBaseUrl()}/Registrars/${registrarId}/tld/${tldId}`, {
        method: 'POST',
    });

    if (!response.success) {
        const message = (response.message || '').toLowerCase();
        if (message.includes('already')) {
            return;
        }

        throw new Error(response.message || `Failed to assign registrar ${registrarId}`);
    }
}

async function syncSelectedRegistrars(tldId: number, selectedRegistrarIds: number[]): Promise<void> {
    if (selectedRegistrarIds.length === 0) {
        return;
    }

    const existingIds = await getAssignedRegistrarIds(tldId);
    const existingSet = new Set(existingIds);

    for (const registrarId of selectedRegistrarIds) {
        if (!existingSet.has(registrarId)) {
            await assignRegistrarToTld(registrarId, tldId);
        }
    }
}

function openCreate(): void {
    editingId = null;

    const modalTitle = document.getElementById('tlds-modal-title');
    if (modalTitle) {
        modalTitle.textContent = 'Add TLD';
    }

    const form = document.getElementById('tlds-form') as HTMLFormElement | null;
    form?.reset();

    const isActive = document.getElementById('tlds-is-active') as HTMLInputElement | null;
    if (isActive) {
        isActive.checked = true;
    }

    const secondLevelContainer = document.getElementById('tlds-is-second-level-container');
    secondLevelContainer?.classList.remove('d-none');

    renderRegistrarMultiSelect([]);
    showModal('tlds-edit-modal');
}

async function openEdit(id: number): Promise<void> {
    const currentItem = allTlds.find((item) => item.id === id);
    if (!currentItem) {
        return;
    }

    let tld = currentItem;
    const tldResponse = await apiRequest<unknown>(`${getApiBaseUrl()}/Tlds/${id}`, { method: 'GET' });
    if (tldResponse.success && tldResponse.data) {
        tld = normalizeTld(tldResponse.data);
    }

    editingId = id;

    const modalTitle = document.getElementById('tlds-modal-title');
    if (modalTitle) {
        modalTitle.textContent = 'Edit TLD';
    }

    const extension = document.getElementById('tlds-extension') as HTMLInputElement | null;
    const description = document.getElementById('tlds-description') as HTMLInputElement | null;
    const defaultYears = document.getElementById('tlds-default-years') as HTMLInputElement | null;
    const maxYears = document.getElementById('tlds-max-years') as HTMLInputElement | null;
    const requiresPrivacy = document.getElementById('tlds-requires-privacy') as HTMLInputElement | null;
    const isActive = document.getElementById('tlds-is-active') as HTMLInputElement | null;
    const notes = document.getElementById('tlds-notes') as HTMLTextAreaElement | null;
    const isSecondLevel = document.getElementById('tlds-is-second-level') as HTMLInputElement | null;

    if (extension) {
        extension.value = tld.extension.replace(/^\./, '');
    }
    if (description) {
        description.value = tld.description || '';
    }
    if (defaultYears) {
        defaultYears.value = tld.defaultRegistrationYears !== null ? String(tld.defaultRegistrationYears) : '';
    }
    if (maxYears) {
        maxYears.value = tld.maxRegistrationYears !== null ? String(tld.maxRegistrationYears) : '';
    }
    if (requiresPrivacy) {
        requiresPrivacy.checked = tld.requiresPrivacy;
    }
    if (isActive) {
        isActive.checked = tld.isActive;
    }
    if (notes) {
        notes.value = tld.notes || '';
    }
    if (isSecondLevel) {
        isSecondLevel.checked = false;
    }

    const secondLevelContainer = document.getElementById('tlds-is-second-level-container');
    secondLevelContainer?.classList.add('d-none');

    const selectedRegistrarIds = await getAssignedRegistrarIds(id);
    renderRegistrarMultiSelect(selectedRegistrarIds);

    showModal('tlds-edit-modal');
}

function getOptionalNumber(id: string): number | null {
    const input = document.getElementById(id) as HTMLInputElement | null;
    const raw = input?.value.trim() ?? '';
    if (!raw) {
        return null;
    }

    const parsed = Number(raw);
    return Number.isFinite(parsed) ? parsed : null;
}

async function saveTld(): Promise<void> {
    const extensionInput = document.getElementById('tlds-extension') as HTMLInputElement | null;
    const descriptionInput = document.getElementById('tlds-description') as HTMLInputElement | null;
    const secondLevelInput = document.getElementById('tlds-is-second-level') as HTMLInputElement | null;
    const activeInput = document.getElementById('tlds-is-active') as HTMLInputElement | null;
    const requiresPrivacyInput = document.getElementById('tlds-requires-privacy') as HTMLInputElement | null;
    const notesInput = document.getElementById('tlds-notes') as HTMLTextAreaElement | null;

    const extension = (extensionInput?.value ?? '').trim().replace(/^\./, '').toLowerCase();
    if (!extension) {
        showError('Extension is required');
        return;
    }

    const defaultYears = getOptionalNumber('tlds-default-years');
    const maxYears = getOptionalNumber('tlds-max-years');

    if (defaultYears !== null && defaultYears < 1) {
        showError('Default years must be at least 1');
        return;
    }

    if (maxYears !== null && maxYears < 1) {
        showError('Max years must be at least 1');
        return;
    }

    if (defaultYears !== null && maxYears !== null && defaultYears > maxYears) {
        showError('Default years cannot be greater than max years');
        return;
    }

    const payloadBase = {
        extension,
        description: (descriptionInput?.value ?? '').trim(),
        isActive: activeInput?.checked ?? true,
        defaultRegistrationYears: defaultYears,
        maxRegistrationYears: maxYears,
        requiresPrivacy: requiresPrivacyInput?.checked ?? false,
        notes: (notesInput?.value ?? '').trim() || null,
    };

    const response = editingId !== null
        ? await apiRequest<unknown>(`${getApiBaseUrl()}/Tlds/${editingId}`, {
            method: 'PUT',
            body: JSON.stringify(payloadBase as UpdateTldPayload),
        })
        : await apiRequest<unknown>(`${getApiBaseUrl()}/Tlds`, {
            method: 'POST',
            body: JSON.stringify({
                ...payloadBase,
                isSecondLevel: secondLevelInput?.checked ?? false,
            } as CreateTldPayload),
        });

    if (!response.success) {
        showError(response.message || 'Save failed');
        return;
    }

    const savedTld = normalizeTld(response.data);
    const effectiveTldId = editingId ?? (savedTld.id > 0 ? savedTld.id : null);
    if (effectiveTldId === null) {
        showError('TLD saved, but could not determine the TLD ID for registrar assignment.');
        return;
    }

    const selectedRegistrarIds = getSelectedRegistrarIdsFromModal();
    try {
        await syncSelectedRegistrars(effectiveTldId, selectedRegistrarIds);
    } catch (error) {
        const message = error instanceof Error ? error.message : 'Failed to assign selected registrars';
        showError(message);
        return;
    }

    hideModal('tlds-edit-modal');
    showSuccess(editingId !== null ? 'TLD updated successfully' : 'TLD created successfully');
    await loadTlds();
}

function openDelete(id: number, displayName: string): void {
    pendingDeleteId = id;

    const deleteName = document.getElementById('tlds-delete-name');
    if (deleteName) {
        deleteName.textContent = displayName;
    }

    showModal('tlds-delete-modal');
}

async function doDelete(): Promise<void> {
    if (pendingDeleteId === null) {
        return;
    }

    const response = await apiRequest<void>(`${getApiBaseUrl()}/Tlds/${pendingDeleteId}`, { method: 'DELETE' });
    hideModal('tlds-delete-modal');

    if (!response.success) {
        showError(response.message || 'Delete failed');
        pendingDeleteId = null;
        return;
    }

    showSuccess('TLD soft-deleted successfully');

    if (allTlds.length === 1 && currentPage > 1) {
        currentPage -= 1;
    }

    pendingDeleteId = null;
    await loadTlds();
}

function changePage(page: number): void {
    if (page < 1 || page > totalPages) {
        return;
    }

    currentPage = page;
    void loadTlds();
}

function bindPagingControlsActions(): void {
    const container = document.getElementById('tlds-paging-controls');
    if (!container) {
        return;
    }

    container.addEventListener('click', (event) => {
        const target = event.target as HTMLElement;
        const link = target.closest('a[data-page]') as HTMLAnchorElement | null;
        if (!link) {
            return;
        }

        event.preventDefault();

        const rawPage = link.dataset.page ?? '';
        const page = Number(rawPage);
        if (Number.isFinite(page)) {
            changePage(page);
        }
    });
}

function bindTableActions(): void {
    const tableBody = document.getElementById('tlds-table-body');
    if (!tableBody) {
        return;
    }

    tableBody.addEventListener('click', (event) => {
        const target = event.target as HTMLElement;
        const button = target.closest('button[data-action]') as HTMLButtonElement | null;
        if (!button) {
            return;
        }

        const id = Number(button.dataset.id ?? '0');
        if (!Number.isFinite(id) || id <= 0) {
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

function initializeTldsPage(): void {
    const page = document.getElementById('tlds-page');
    if (!page || page.dataset.initialized === 'true') {
        return;
    }

    page.dataset.initialized = 'true';

    document.getElementById('tlds-create')?.addEventListener('click', openCreate);
    document.getElementById('tlds-save')?.addEventListener('click', () => { void saveTld(); });
    document.getElementById('tlds-confirm-delete')?.addEventListener('click', () => { void doDelete(); });
    document.getElementById('tlds-refresh')?.addEventListener('click', () => { void loadTlds(); });
    document.getElementById('tlds-filter-registrar')?.addEventListener('change', () => {
        selectedRegistrarFilterId = getRegistrarFilterId();
        currentPage = 1;
        void loadTlds();
    });
    document.getElementById('tlds-page-size')?.addEventListener('change', () => {
        currentPage = 1;
        void loadTlds();
    });

    bindPagingControlsActions();
    bindTableActions();

    void (async () => {
        await loadActiveRegistrars();
        selectedRegistrarFilterId = getRegistrarFilterId();
        await loadTlds();
    })();
}

function setupPageObserver(): void {
    initializeTldsPage();

    if (document.body) {
        const observer = new MutationObserver(() => {
            const page = document.getElementById('tlds-page');
            if (page && page.dataset.initialized !== 'true') {
                initializeTldsPage();
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

(function () {
interface Tld {
    id: number;
    extension: string;
    description: string;
    isActive: boolean;
    registrationPrice: number | null;
    renewalPrice: number | null;
    transferPrice: number | null;
    priceCurrency: string | null;
    defaultRegistrationYears: number | null;
    maxRegistrationYears: number | null;
    requiresPrivacy: boolean;
    notes: string | null;
}

interface TldSalesPricing {
    id: number;
    tldId: number;
    effectiveFrom: string;
    effectiveTo: string | null;
    registrationPrice: number;
    renewalPrice: number;
    transferPrice: number;
    privacyPrice: number | null;
    firstYearRegistrationPrice: number | null;
    currency: string;
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

interface RegistrarCostPriceRow {
    registrarName: string;
    registrationCost: number | null;
    renewalCost: number | null;
    transferCost: number | null;
    currency: string;
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
let modalCurrentSalesPricing: TldSalesPricing | null = null;
let createPreviewRegistrarCostRows: RegistrarCostPriceRow[] = [];

function getApiBaseUrl(): string {
    const baseUrl = w.AppSettings?.apiBaseUrl;
    if (!baseUrl) {
        return window.location.protocol === 'https:'
            ? 'https://localhost:7201/api/v1'
            : 'http://localhost:5133/api/v1';
    }
    return baseUrl;
}

async function previewRegistrarPricesForCreate(extension: string): Promise<void> {
    const normalized = extension.trim().replace(/^\./, '').toLowerCase();
    if (!normalized) {
        createPreviewRegistrarCostRows = [];
        renderRegistrarCostRows([]);
        return;
    }

    setRegistrarPricesLoading(true);
    try {
        const response = await apiRequest<unknown>(`${getApiBaseUrl()}/RegistrarTldCostPricing/preview/extension/${encodeURIComponent(normalized)}`, {
            method: 'GET',
        });

        if (!response.success) {
            createPreviewRegistrarCostRows = [];
            renderRegistrarCostRows([]);
            return;
        }

        createPreviewRegistrarCostRows = getListFromUnknown(response.data)
            .filter(isRecord)
            .map((item) => ({
                registrarName: getString(item.registrarName ?? item.RegistrarName),
                registrationCost: getDecimal(item.registrationCost ?? item.RegistrationCost),
                renewalCost: getDecimal(item.renewalCost ?? item.RenewalCost),
                transferCost: getDecimal(item.transferCost ?? item.TransferCost),
                currency: getString(item.currency ?? item.Currency) || '-',
            }))
            .filter((row) => row.registrarName.trim().length > 0)
            .sort((a, b) => a.registrarName.localeCompare(b.registrarName));

        renderRegistrarCostRows(createPreviewRegistrarCostRows);
    }
    finally {
        setRegistrarPricesLoading(false);
    }
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

function getDecimal(value: unknown): number | null {
    if (typeof value === 'number' && Number.isFinite(value)) {
        return value;
    }

    if (typeof value === 'string') {
        const parsed = Number(value);
        if (Number.isFinite(parsed)) {
            return parsed;
        }
    }

    return null;
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

function normalizeSalesPricing(item: unknown): TldSalesPricing {
    if (!isRecord(item)) {
        return {
            id: 0,
            tldId: 0,
            effectiveFrom: '',
            effectiveTo: null,
            registrationPrice: 0,
            renewalPrice: 0,
            transferPrice: 0,
            privacyPrice: null,
            firstYearRegistrationPrice: null,
            currency: 'USD',
        };
    }
    return {
        id: getNumber(item.id) ?? getNumber(item.Id) ?? 0,
        tldId: getNumber(item.tldId ?? item.TldId) ?? 0,
        effectiveFrom: getString(item.effectiveFrom ?? item.EffectiveFrom),
        effectiveTo: getString(item.effectiveTo ?? item.EffectiveTo) || null,
        registrationPrice: getDecimal(item.registrationPrice ?? item.RegistrationPrice) ?? 0,
        renewalPrice: getDecimal(item.renewalPrice ?? item.RenewalPrice) ?? 0,
        transferPrice: getDecimal(item.transferPrice ?? item.TransferPrice) ?? 0,
        privacyPrice: getDecimal(item.privacyPrice ?? item.PrivacyPrice),
        firstYearRegistrationPrice: getDecimal(item.firstYearRegistrationPrice ?? item.FirstYearRegistrationPrice),
        currency: getString(item.currency ?? item.Currency) || 'USD',
    };
}

function pickBestPricingFromHistory(items: TldSalesPricing[]): TldSalesPricing | null {
    if (!items.length) {
        return null;
    }

    const now = Date.now();
    const activeNow = items
        .filter((item) => {
            const from = Date.parse(item.effectiveFrom);
            if (!Number.isFinite(from) || from > now) {
                return false;
            }

            if (!item.effectiveTo) {
                return true;
            }

            const to = Date.parse(item.effectiveTo);
            return !Number.isFinite(to) || to > now;
        })
        .sort((a, b) => Date.parse(b.effectiveFrom) - Date.parse(a.effectiveFrom));

    if (activeNow.length > 0) {
        return activeNow[0];
    }

    return items
        .slice()
        .sort((a, b) => Date.parse(b.effectiveFrom) - Date.parse(a.effectiveFrom))[0] ?? null;
}

function toDateTimeLocalValue(iso: string | null): string {
    if (!iso) {
        return '';
    }

    const parsed = new Date(iso);
    if (Number.isNaN(parsed.getTime())) {
        return '';
    }

    const offsetMs = parsed.getTimezoneOffset() * 60000;
    const local = new Date(parsed.getTime() - offsetMs);
    return local.toISOString().slice(0, 16);
}

function readDateTimeLocalAsIso(id: string): string | null {
    const input = document.getElementById(id) as HTMLInputElement | null;
    const raw = input?.value.trim() ?? '';
    if (!raw) {
        return null;
    }

    const parsed = new Date(raw);
    return Number.isNaN(parsed.getTime()) ? null : parsed.toISOString();
}

function formatPrice(value: number | null): string {
    if (value === null || !Number.isFinite(value)) {
        return '-';
    }

    return value.toFixed(2);
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
            registrationPrice: null,
            renewalPrice: null,
            transferPrice: null,
            priceCurrency: null,
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
        registrationPrice: null,
        renewalPrice: null,
        transferPrice: null,
        priceCurrency: null,
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
        tableBody.innerHTML = '<tr><td colspan="11" class="text-center text-muted">No active TLDs found.</td></tr>';
        return;
    }

    tableBody.innerHTML = allTlds.map((item) => {
        const extension = item.extension.startsWith('.') ? item.extension : `.${item.extension}`;
        return `
        <tr>
            <td>${item.id}</td>
            <td><code>${esc(extension)}</code></td>
            <td>${esc(item.description || '-')}</td>
            <td>${formatPrice(item.registrationPrice)}</td>
            <td>${formatPrice(item.renewalPrice)}</td>
            <td>${formatPrice(item.transferPrice)}</td>
            <td>${esc(item.priceCurrency || '-')}</td>
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

    tableBody.innerHTML = '<tr><td colspan="11" class="text-center"><div class="spinner-border text-primary"></div></td></tr>';

    readPageSize();
    const response = await apiRequest<unknown>(buildTldsUrl(), { method: 'GET' });

    if (!response.success) {
        showError(response.message || 'Failed to load active TLDs');
        tableBody.innerHTML = '<tr><td colspan="11" class="text-center text-danger">Failed to load data</td></tr>';
        return;
    }

    if (selectedRegistrarFilterId !== null) {
        const parsed = extractPagedRegistrarTldData(response.data);
        allTlds = parsed.items.map((item) => ({
            id: item.tldId,
            extension: item.tldExtension.replace(/^\./, ''),
            description: '',
            isActive: item.isActive,
            registrationPrice: null,
            renewalPrice: null,
            transferPrice: null,
            priceCurrency: null,
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

    void loadSalesPricingForCurrentRows();
}

async function loadSalesPricingForCurrentRows(): Promise<void> {
    const pricingByTldId = new Map<number, TldSalesPricing>();

    await Promise.all(allTlds.map(async (tld) => {
        const response = await apiRequest<unknown>(`${getApiBaseUrl()}/tld-pricing/sales/tld/${tld.id}/current`, { method: 'GET' });
        if (!response.success || !response.data) {
            return;
        }

        const pricing = normalizeSalesPricing(response.data);
        if (pricing.tldId > 0) {
            pricingByTldId.set(pricing.tldId, pricing);
        }
    }));

    for (const tld of allTlds) {
        const pricing = pricingByTldId.get(tld.id);
        if (!pricing) {
            tld.registrationPrice = null;
            tld.renewalPrice = null;
            tld.transferPrice = null;
            tld.priceCurrency = null;
            continue;
        }

        tld.registrationPrice = pricing.registrationPrice;
        tld.renewalPrice = pricing.renewalPrice;
        tld.transferPrice = pricing.transferPrice;
        tld.priceCurrency = pricing.currency;
    }

    renderTable();
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

async function getAssignedRegistrarTlds(tldId: number): Promise<RegistrarTld[]> {
    const response = await apiRequest<unknown>(`${getApiBaseUrl()}/RegistrarTlds/tld/${tldId}`, { method: 'GET' });
    if (!response.success) {
        return [];
    }

    return getListFromUnknown(response.data)
        .map(normalizeRegistrarTld)
        .filter((item) => item.id > 0 && item.isActive);
}

function setSalesPricingInputs(pricing: TldSalesPricing | null): void {
    const registrationInput = document.getElementById('tlds-registration-price') as HTMLInputElement | null;
    const renewalInput = document.getElementById('tlds-renewal-price') as HTMLInputElement | null;
    const transferInput = document.getElementById('tlds-transfer-price') as HTMLInputElement | null;
    const privacyInput = document.getElementById('tlds-privacy-price') as HTMLInputElement | null;
    const firstYearInput = document.getElementById('tlds-first-year-price') as HTMLInputElement | null;
    const currencyInput = document.getElementById('tlds-price-currency') as HTMLInputElement | null;
    const effectiveFromInput = document.getElementById('tlds-price-effective-from') as HTMLInputElement | null;
    const effectiveToInput = document.getElementById('tlds-price-effective-to') as HTMLInputElement | null;

    if (registrationInput) {
        registrationInput.value = pricing ? String(pricing.registrationPrice) : '';
    }

    if (renewalInput) {
        renewalInput.value = pricing ? String(pricing.renewalPrice) : '';
    }

    if (transferInput) {
        transferInput.value = pricing ? String(pricing.transferPrice) : '';
    }

    if (privacyInput) {
        privacyInput.value = pricing?.privacyPrice !== null && pricing?.privacyPrice !== undefined
            ? String(pricing.privacyPrice)
            : '';
    }

    if (firstYearInput) {
        firstYearInput.value = pricing?.firstYearRegistrationPrice !== null && pricing?.firstYearRegistrationPrice !== undefined
            ? String(pricing.firstYearRegistrationPrice)
            : '';
    }

    if (currencyInput) {
        currencyInput.value = pricing?.currency || 'USD';
    }

    if (effectiveFromInput) {
        effectiveFromInput.value = toDateTimeLocalValue(pricing?.effectiveFrom ?? null);
    }

    if (effectiveToInput) {
        effectiveToInput.value = toDateTimeLocalValue(pricing?.effectiveTo ?? null);
    }
}

function setRegistrarPricesLoading(isLoading: boolean): void {
    const loading = document.getElementById('tlds-registrar-prices-loading');
    loading?.classList.toggle('d-none', !isLoading);
}

function renderRegistrarCostRows(rows: RegistrarCostPriceRow[]): void {
    const empty = document.getElementById('tlds-registrar-prices-empty');
    const tableWrap = document.getElementById('tlds-registrar-prices-table-wrap');
    const body = document.getElementById('tlds-registrar-prices-body');

    if (!empty || !tableWrap || !body) {
        return;
    }

    if (!rows.length) {
        empty.classList.remove('d-none');
        tableWrap.classList.add('d-none');
        body.innerHTML = '';
        return;
    }

    empty.classList.add('d-none');
    tableWrap.classList.remove('d-none');

    body.innerHTML = rows.map((row) => `
        <tr>
            <td>${esc(row.registrarName)}</td>
            <td>${formatPrice(row.registrationCost)}</td>
            <td>${formatPrice(row.renewalCost)}</td>
            <td>${formatPrice(row.transferCost)}</td>
            <td>${esc(row.currency)}</td>
        </tr>
    `).join('');
}

async function loadRegistrarPricesForModal(tldId: number | null): Promise<void> {
    if (tldId === null) {
        renderRegistrarCostRows([]);
        return;
    }

    setRegistrarPricesLoading(true);

    try {
        const response = await apiRequest<unknown>(`${getApiBaseUrl()}/RegistrarTldCostPricing/tld/${tldId}/current/ensure`, {
            method: 'POST',
        });

        if (!response.success) {
            renderRegistrarCostRows([]);
            return;
        }

        const rows: RegistrarCostPriceRow[] = getListFromUnknown(response.data)
            .filter(isRecord)
            .map((item) => ({
                registrarName: getString(item.registrarName ?? item.RegistrarName),
                registrationCost: getDecimal(item.registrationCost ?? item.RegistrationCost),
                renewalCost: getDecimal(item.renewalCost ?? item.RenewalCost),
                transferCost: getDecimal(item.transferCost ?? item.TransferCost),
                currency: getString(item.currency ?? item.Currency) || '-',
            }))
            .filter((row) => row.registrarName.trim().length > 0)
            .sort((a, b) => a.registrarName.localeCompare(b.registrarName));

        renderRegistrarCostRows(rows);
    }
    finally {
        setRegistrarPricesLoading(false);
    }
}

async function loadSalesPricingForModal(tldId: number | null): Promise<void> {
    modalCurrentSalesPricing = null;
    setSalesPricingInputs(null);

    if (tldId === null) {
        return;
    }

    const response = await apiRequest<unknown>(`${getApiBaseUrl()}/tld-pricing/sales/tld/${tldId}?includeArchived=true`, {
        method: 'GET',
    });

    if (!response.success) {
        return;
    }

    const history = getListFromUnknown(response.data)
        .map(normalizeSalesPricing)
        .filter((item) => item.tldId === tldId)
        .filter((item) => item.effectiveFrom.length > 0);

    const selected = pickBestPricingFromHistory(history);
    if (!selected) {
        return;
    }

    modalCurrentSalesPricing = selected;
    setSalesPricingInputs(selected);
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
    modalCurrentSalesPricing = null;
    createPreviewRegistrarCostRows = [];

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
    setSalesPricingInputs(null);
    renderRegistrarCostRows([]);
    showModal('tlds-edit-modal');
}

async function openEdit(id: number): Promise<void> {
    const currentItem = allTlds.find((item) => item.id === id);
    if (!currentItem) {
        return;
    }

    let tld = currentItem;

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
    renderRegistrarMultiSelect([]);
    setSalesPricingInputs(null);
    renderRegistrarCostRows([]);
    showModal('tlds-edit-modal');

    void loadRegistrarPricesForModal(id);
    void loadSalesPricingForModal(id);

    const tldResponse = await apiRequest<unknown>(`${getApiBaseUrl()}/Tlds/${id}`, { method: 'GET' });
    if (tldResponse.success && tldResponse.data) {
        tld = normalizeTld(tldResponse.data);

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
    }

    const selectedRegistrarIds = await getAssignedRegistrarIds(id);
    renderRegistrarMultiSelect(selectedRegistrarIds);
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

async function upsertSalesPricingForTld(tldId: number): Promise<void> {
    const registrationPrice = getOptionalNumber('tlds-registration-price');
    const renewalPrice = getOptionalNumber('tlds-renewal-price');
    const transferPrice = getOptionalNumber('tlds-transfer-price');
    const privacyPrice = getOptionalNumber('tlds-privacy-price');
    const firstYearRegistrationPrice = getOptionalNumber('tlds-first-year-price');
    const currencyInput = document.getElementById('tlds-price-currency') as HTMLInputElement | null;
    const currency = (currencyInput?.value ?? '').trim().toUpperCase() || 'USD';
    const effectiveFromIso = readDateTimeLocalAsIso('tlds-price-effective-from');
    const effectiveToIso = readDateTimeLocalAsIso('tlds-price-effective-to');

    const hasAnyPriceInput =
        registrationPrice !== null ||
        renewalPrice !== null ||
        transferPrice !== null ||
        privacyPrice !== null ||
        firstYearRegistrationPrice !== null;

    if (!hasAnyPriceInput) {
        return;
    }

    if (registrationPrice === null || renewalPrice === null || transferPrice === null) {
        throw new Error('Registration, renewal and transfer prices are required when setting sales pricing.');
    }

    if (registrationPrice < 0 || renewalPrice < 0 || transferPrice < 0 ||
        (privacyPrice !== null && privacyPrice < 0) ||
        (firstYearRegistrationPrice !== null && firstYearRegistrationPrice < 0)) {
        throw new Error('Prices cannot be negative.');
    }

    const currentEffectiveFromIso = modalCurrentSalesPricing?.effectiveFrom ?? null;
    const currentEffectiveToIso = modalCurrentSalesPricing?.effectiveTo ?? null;

    const isSameAsCurrent =
        modalCurrentSalesPricing !== null &&
        currentEffectiveFromIso === effectiveFromIso &&
        currentEffectiveToIso === effectiveToIso &&
        modalCurrentSalesPricing.registrationPrice === registrationPrice &&
        modalCurrentSalesPricing.renewalPrice === renewalPrice &&
        modalCurrentSalesPricing.transferPrice === transferPrice &&
        modalCurrentSalesPricing.privacyPrice === privacyPrice &&
        modalCurrentSalesPricing.firstYearRegistrationPrice === firstYearRegistrationPrice &&
        modalCurrentSalesPricing.currency.toUpperCase() === currency;

    if (isSameAsCurrent) {
        return;
    }

    const minEffectiveFromDate = new Date(Date.now() + 60 * 1000);
    const effectiveFromDate = effectiveFromIso
        ? new Date(Math.max(new Date(effectiveFromIso).getTime(), minEffectiveFromDate.getTime()))
        : minEffectiveFromDate;
    const effectiveToDate = effectiveToIso ? new Date(effectiveToIso) : null;

    if (effectiveToDate !== null && effectiveToDate <= effectiveFromDate) {
        throw new Error('Effective to must be later than effective from.');
    }

    const payload = {
        tldId,
        effectiveFrom: effectiveFromDate.toISOString(),
        effectiveTo: effectiveToDate ? effectiveToDate.toISOString() : null,
        registrationPrice,
        renewalPrice,
        transferPrice,
        privacyPrice,
        firstYearRegistrationPrice,
        currency,
        isPromotional: false,
        promotionName: null,
        isActive: true,
        notes: 'Updated from reseller panel /tld/list',
    };

    const response = await apiRequest<unknown>(`${getApiBaseUrl()}/tld-pricing/sales`, {
        method: 'POST',
        body: JSON.stringify(payload),
    });

    if (!response.success || !response.data) {
        throw new Error(response.message || 'Failed to save TLD sales pricing.');
    }

    const pricing = normalizeSalesPricing(response.data);
    modalCurrentSalesPricing = pricing;

    const target = allTlds.find((item) => item.id === tldId);
    if (target) {
        target.registrationPrice = pricing.registrationPrice;
        target.renewalPrice = pricing.renewalPrice;
        target.transferPrice = pricing.transferPrice;
        target.priceCurrency = pricing.currency;
    }
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

        if (editingId === null && createPreviewRegistrarCostRows.length > 0) {
            const ensureResponse = await apiRequest<unknown>(`${getApiBaseUrl()}/RegistrarTldCostPricing/tld/${effectiveTldId}/current/ensure`, {
                method: 'POST',
            });

            if (!ensureResponse.success) {
                throw new Error(ensureResponse.message || 'Failed to store registrar cost prices for the new TLD.');
            }
        }

        await upsertSalesPricingForTld(effectiveTldId);
    } catch (error) {
        const message = error instanceof Error ? error.message : 'Failed to save TLD pricing data';
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
    document.getElementById('tlds-extension')?.addEventListener('blur', () => {
        if (editingId !== null) {
            return;
        }

        const extensionInput = document.getElementById('tlds-extension') as HTMLInputElement | null;
        const extension = extensionInput?.value ?? '';
        void previewRegistrarPricesForCreate(extension);
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

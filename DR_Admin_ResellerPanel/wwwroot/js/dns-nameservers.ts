// @ts-nocheck
(function() {
interface Domain {
    id: number;
    name: string;
}

interface NameServer {
    id: number;
    domainId: number;
    hostname: string;
    ipAddress?: string | null;
    isPrimary: boolean;
    sortOrder: number;
}

interface ApiResponse<T> {
    success: boolean;
    data?: T;
    message?: string;
}

type NameServerMode = 'registrar' | 'self';
type SortColumn = 'domain' | 'hostname' | 'ipAddress' | 'mode' | 'isPrimary' | 'sortOrder';

let selectedDomainId: number | null = null;
let selectedDomainName: string | null = null;
let showAllDomains = false;
let allNameServers: NameServer[] = [];
let filteredNameServers: NameServer[] = [];
let editingId: number | null = null;
let pendingDeleteId: number | null = null;
const domainLookup = new Map<number, string>();
let sortColumn: SortColumn = 'domain';
let sortDirection: 'asc' | 'desc' = 'asc';

let filterDomain = '';
let filterHostname = '';
let filterIp = '';
let filterMode: 'all' | NameServerMode = 'all';
let filterPrimary: 'all' | 'yes' | 'no' = 'all';
let filterSort = '';

let currentPage = 1;
let pageSize = 25;
let totalCount = 0;
let totalPages = 1;

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
        console.error('Nameservers request failed', error);
        return {
            success: false,
            message: 'Network error. Please try again.',
        };
    }
}

function initializePage(): void {
    const page = document.getElementById('dns-nameservers-page') as HTMLElement | null;
    if (!page || page.dataset.initialized === 'true') {
        return;
    }

    page.dataset.initialized = 'true';

    document.getElementById('dns-nameservers-add')?.addEventListener('click', openCreate);
    document.getElementById('dns-nameservers-save')?.addEventListener('click', saveNameServer);
    document.getElementById('dns-nameservers-confirm-delete')?.addEventListener('click', deleteNameServer);
    document.getElementById('dns-nameservers-mode-filter')?.addEventListener('change', applyModeFilter);
    document.getElementById('dns-nameservers-filter-domain')?.addEventListener('input', applyModeFilter);
    document.getElementById('dns-nameservers-filter-hostname')?.addEventListener('input', applyModeFilter);
    document.getElementById('dns-nameservers-filter-ip')?.addEventListener('input', applyModeFilter);
    document.getElementById('dns-nameservers-filter-primary')?.addEventListener('change', applyModeFilter);
    document.getElementById('dns-nameservers-filter-sort')?.addEventListener('input', applyModeFilter);
    document.getElementById('dns-nameservers-page-size')?.addEventListener('change', () => {
        loadPageSizeFromUi();
        currentPage = 1;
        renderNameServers();
    });

    bindTableActions();
    void ensureDomainLookupLoaded();

    void showAllDomainNameServers();
}

function bindTableActions(): void {
    const tableBody = document.getElementById('dns-nameservers-table-body');
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
            openEdit(id);
            return;
        }

        if (button.dataset.action === 'delete') {
            openDelete(id);
        }
    });

    const tableHead = tableBody.closest('table')?.querySelector('thead');
    tableHead?.addEventListener('click', (event) => {
        const target = event.target as HTMLElement;
        const button = target.closest('button[data-sort]') as HTMLButtonElement | null;
        if (!button) {
            return;
        }

        const column = (button.dataset.sort ?? '') as SortColumn;
        if (!column) {
            return;
        }

        if (sortColumn === column) {
            sortDirection = sortDirection === 'asc' ? 'desc' : 'asc';
        } else {
            sortColumn = column;
            sortDirection = 'asc';
        }

        currentPage = 1;
        renderNameServers();
    });
}

function normalizeDomain(raw: any): Domain {
    return {
        id: raw.id ?? raw.Id ?? 0,
        name: raw.name ?? raw.Name ?? raw.domainName ?? '',
    };
}

function setSelectedDomain(domain: Domain): void {
    showAllDomains = false;
    selectedDomainId = domain.id;
    selectedDomainName = domain.name || `Domain #${domain.id}`;

    setText('dns-nameservers-selected-domain', selectedDomainName);
    setText('dns-nameservers-selected-id', String(domain.id));

    const addButton = document.getElementById('dns-nameservers-add') as HTMLButtonElement | null;
    if (addButton) {
        addButton.disabled = false;
    }

}

async function loadNameServers(): Promise<void> {
    if (showAllDomains) {
        await loadAllNameServers();
        return;
    }

    if (!selectedDomainId) {
        showNoSelection();
        return;
    }

    setLoading(true);
    const response = await apiRequest<NameServer[]>(`${getApiBaseUrl()}/NameServers/domain/${selectedDomainId}`, { method: 'GET' });
    if (!response.success) {
        setLoading(false);
        showError(response.message || 'Failed to load nameservers.');
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

    allNameServers = items.map(normalizeNameServer);
    applyModeFilter();
    setLoading(false);
}

async function loadAllNameServers(): Promise<void> {
    setLoading(true);
    await ensureDomainLookupLoaded();

    const response = await apiRequest<any>(`${getApiBaseUrl()}/NameServers?pageNumber=1&pageSize=2000`, { method: 'GET' });
    if (!response.success) {
        setLoading(false);
        showError(response.message || 'Failed to load nameservers.');
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

    allNameServers = items.map(normalizeNameServer);
    applyModeFilter();
    setLoading(false);
}

async function showAllDomainNameServers(): Promise<void> {
    showAllDomains = true;
    selectedDomainId = null;
    selectedDomainName = null;

    setText('dns-nameservers-selected-domain', 'All domains');
    setText('dns-nameservers-selected-id', '-');

    const addButton = document.getElementById('dns-nameservers-add') as HTMLButtonElement | null;
    if (addButton) {
        addButton.disabled = true;
    }

    await loadNameServers();
}

async function ensureDomainLookupLoaded(): Promise<void> {
    if (domainLookup.size > 0) {
        return;
    }

    const response = await apiRequest<any>(`${getApiBaseUrl()}/RegisteredDomains?pageNumber=1&pageSize=2000`, { method: 'GET' });
    if (!response.success) {
        return;
    }

    const raw = response.data as any;
    const items = Array.isArray(raw)
        ? raw
        : Array.isArray(raw?.data)
            ? raw.data
            : Array.isArray(raw?.Data)
                ? raw.Data
                : Array.isArray(raw?.items)
                    ? raw.items
                    : Array.isArray(raw?.Items)
                        ? raw.Items
                        : Array.isArray(raw?.data?.items)
                            ? raw.data.items
                            : Array.isArray(raw?.data?.Items)
                                ? raw.data.Items
                                : [];

    items.forEach((item: any) => {
        const id = Number(item.id ?? item.Id ?? 0);
        if (!Number.isFinite(id) || id <= 0) {
            return;
        }

        const name = String(item.name ?? item.Name ?? item.domainName ?? item.DomainName ?? `Domain #${id}`);
        domainLookup.set(id, name);
    });
}

function normalizeNameServer(item: any): NameServer {
    return {
        id: item.id ?? item.Id ?? 0,
        domainId: item.domainId ?? item.DomainId ?? 0,
        hostname: item.hostname ?? item.Hostname ?? '',
        ipAddress: item.ipAddress ?? item.IpAddress ?? null,
        isPrimary: item.isPrimary ?? item.IsPrimary ?? false,
        sortOrder: item.sortOrder ?? item.SortOrder ?? 0,
    };
}

function detectMode(ns: NameServer): NameServerMode {
    const hostname = (ns.hostname || '').toLowerCase();
    const hasIp = !!(ns.ipAddress || '').trim();

    if (hasIp) {
        return 'self';
    }

    const registrarHints = ['awsdns', 'route53', 'registrar-servers', 'cloudflare', 'namecheap', 'godaddy', 'opensrs', 'centralnic', 'dnsimple', 'regtons'];
    if (registrarHints.some((hint) => hostname.includes(hint))) {
        return 'registrar';
    }

    if (selectedDomainName && hostname.endsWith(selectedDomainName.toLowerCase())) {
        return 'self';
    }

    return 'registrar';
}

function applyModeFilter(): void {
    loadFiltersFromUi();
    currentPage = 1;

    filteredNameServers = allNameServers.filter((ns) => {
        const domainName = (domainLookup.get(ns.domainId) || (selectedDomainName ?? `Domain #${ns.domainId}`)).toLowerCase();
        if (filterDomain && !domainName.includes(filterDomain)) {
            return false;
        }

        if (filterHostname && !(ns.hostname || '').toLowerCase().includes(filterHostname)) {
            return false;
        }

        if (filterIp && !(ns.ipAddress || '').toLowerCase().includes(filterIp)) {
            return false;
        }

        const mode = detectMode(ns);
        if (filterMode === 'registrar') {
            if (mode !== 'registrar') {
                return false;
            }
        }
        if (filterMode === 'self') {
            if (mode !== 'self') {
                return false;
            }
        }

        if (filterPrimary === 'yes' && !ns.isPrimary) {
            return false;
        }

        if (filterPrimary === 'no' && ns.isPrimary) {
            return false;
        }

        if (filterSort && !String(ns.sortOrder).toLowerCase().includes(filterSort)) {
            return false;
        }

        return true;
    });

    renderNameServers();
}

function loadPageSizeFromUi(): void {
    const select = document.getElementById('dns-nameservers-page-size') as HTMLSelectElement | null;
    const parsed = Number(select?.value ?? '25');
    if (Number.isFinite(parsed) && parsed > 0) {
        pageSize = parsed;
    }
}

function loadFiltersFromUi(): void {
    filterDomain = ((document.getElementById('dns-nameservers-filter-domain') as HTMLInputElement | null)?.value ?? '').trim().toLowerCase();
    filterHostname = ((document.getElementById('dns-nameservers-filter-hostname') as HTMLInputElement | null)?.value ?? '').trim().toLowerCase();
    filterIp = ((document.getElementById('dns-nameservers-filter-ip') as HTMLInputElement | null)?.value ?? '').trim().toLowerCase();
    filterMode = (((document.getElementById('dns-nameservers-mode-filter') as HTMLSelectElement | null)?.value ?? 'all').trim().toLowerCase() as any) || 'all';
    filterPrimary = (((document.getElementById('dns-nameservers-filter-primary') as HTMLSelectElement | null)?.value ?? 'all').trim().toLowerCase() as any) || 'all';
    filterSort = ((document.getElementById('dns-nameservers-filter-sort') as HTMLInputElement | null)?.value ?? '').trim().toLowerCase();
}

function renderNameServers(): void {
    const tableBody = document.getElementById('dns-nameservers-table-body');
    if (!tableBody) {
        return;
    }

    renderSortableHeaders();

    setText('dns-nameservers-count', `${filteredNameServers.length} nameserver${filteredNameServers.length === 1 ? '' : 's'}`);

    if (!filteredNameServers.length) {
        showTable();
        hideEmpty();
        tableBody.innerHTML = '<tr><td colspan="7" class="text-center text-muted">No nameservers match the current filters.</td></tr>';
        totalCount = 0;
        totalPages = 1;
        currentPage = 1;
        renderPagination();
        return;
    }

    hideEmpty();
    showTable();

    const sorted = [...filteredNameServers].sort(compareNameServers);
    totalCount = sorted.length;
    totalPages = Math.max(1, Math.ceil(totalCount / pageSize));
    if (currentPage > totalPages) {
        currentPage = totalPages;
    }

    const start = (currentPage - 1) * pageSize;
    const paged = sorted.slice(start, start + pageSize);

    tableBody.innerHTML = paged.map((ns) => {
        const mode = detectMode(ns);
        const modeBadge = mode === 'self'
            ? '<span class="badge bg-info text-dark">Self managed</span>'
            : '<span class="badge bg-warning text-dark">Registrar managed</span>';

        return `
            <tr>
                <td>${esc(domainLookup.get(ns.domainId) || (selectedDomainName ?? `Domain #${ns.domainId}`))}</td>
                <td><code>${esc(ns.hostname || '-')}</code></td>
                <td>${esc(ns.ipAddress || '-')}</td>
                <td>${modeBadge}</td>
                <td>${ns.isPrimary ? '<span class="badge bg-success">Yes</span>' : '<span class="badge bg-secondary">No</span>'}</td>
                <td>${ns.sortOrder}</td>
                <td class="text-end">
                    <div class="btn-group btn-group-sm">
                        <button class="btn btn-outline-primary" type="button" data-action="edit" data-id="${ns.id}" title="Edit"><i class="bi bi-pencil"></i></button>
                        <button class="btn btn-outline-danger" type="button" data-action="delete" data-id="${ns.id}" title="Delete"><i class="bi bi-trash"></i></button>
                    </div>
                </td>
            </tr>
        `;
    }).join('');

    renderPagination();
}

function renderPagination(): void {
    const info = document.getElementById('dns-nameservers-pagination-info');
    const list = document.getElementById('dns-nameservers-pagination-list');
    if (!info || !list) {
        return;
    }

    if (!totalCount) {
        info.textContent = 'Showing 0 of 0';
        list.innerHTML = '';
        return;
    }

    const start = (currentPage - 1) * pageSize + 1;
    const end = Math.min(currentPage * pageSize, totalCount);
    info.textContent = `Showing ${start}-${end} of ${totalCount}`;

    if (totalPages <= 1) {
        list.innerHTML = '';
        return;
    }

    const makeItem = (label: string, page: number, disabled: boolean, active = false): string => {
        const cls = `page-item${disabled ? ' disabled' : ''}${active ? ' active' : ''}`;
        const dataPage = disabled ? '' : ` data-page="${page}"`;
        return `<li class="${cls}"><a class="page-link" href="#"${dataPage}>${label}</a></li>`;
    };

    let html = '';
    html += makeItem('Previous', currentPage - 1, currentPage <= 1);
    for (let page = 1; page <= totalPages; page += 1) {
        html += makeItem(String(page), page, false, page === currentPage);
    }
    html += makeItem('Next', currentPage + 1, currentPage >= totalPages);
    list.innerHTML = html;

    list.querySelectorAll('a[data-page]').forEach((anchor) => {
        anchor.addEventListener('click', (event) => {
            event.preventDefault();
            const target = event.currentTarget as HTMLAnchorElement;
            const nextPage = Number(target.dataset.page ?? '0');
            if (!Number.isFinite(nextPage) || nextPage < 1 || nextPage > totalPages) {
                return;
            }

            currentPage = nextPage;
            renderNameServers();
        });
    });
}

function compareNameServers(a: NameServer, b: NameServer): number {
    const domainA = (domainLookup.get(a.domainId) || `Domain #${a.domainId}`).toLowerCase();
    const domainB = (domainLookup.get(b.domainId) || `Domain #${b.domainId}`).toLowerCase();
    const modeA = detectMode(a);
    const modeB = detectMode(b);

    let result = 0;
    switch (sortColumn) {
        case 'domain':
            result = domainA.localeCompare(domainB);
            break;
        case 'hostname':
            result = (a.hostname || '').toLowerCase().localeCompare((b.hostname || '').toLowerCase());
            break;
        case 'ipAddress':
            result = (a.ipAddress || '').toLowerCase().localeCompare((b.ipAddress || '').toLowerCase());
            break;
        case 'mode':
            result = modeA.localeCompare(modeB);
            break;
        case 'isPrimary':
            result = Number(a.isPrimary) - Number(b.isPrimary);
            break;
        case 'sortOrder':
            result = a.sortOrder - b.sortOrder;
            break;
    }

    if (result === 0) {
        result = (a.hostname || '').toLowerCase().localeCompare((b.hostname || '').toLowerCase());
    }

    return sortDirection === 'asc' ? result : -result;
}

function renderSortableHeaders(): void {
    const tableHeadRow = document.querySelector('#dns-nameservers-table-wrapper thead tr');
    if (!tableHeadRow) {
        return;
    }

    const headers: Array<{ key: SortColumn; label: string }> = [
        { key: 'domain', label: 'Domain' },
        { key: 'hostname', label: 'Hostname' },
        { key: 'ipAddress', label: 'IP Address' },
        { key: 'mode', label: 'Mode' },
        { key: 'isPrimary', label: 'Primary' },
        { key: 'sortOrder', label: 'Sort' },
    ];

    const makeIndicator = (key: SortColumn): string => {
        if (sortColumn !== key) {
            return '<i class="bi bi-arrow-down-up ms-1 text-muted"></i>';
        }
        return sortDirection === 'asc'
            ? '<i class="bi bi-sort-down ms-1"></i>'
            : '<i class="bi bi-sort-up ms-1"></i>';
    };

    tableHeadRow.innerHTML = `
        ${headers.map((h) => `<th><button type="button" class="btn btn-link btn-sm text-decoration-none p-0" data-sort="${h.key}">${h.label}${makeIndicator(h.key)}</button></th>`).join('')}
        <th class="text-end">Actions</th>
    `;
}

function openCreate(): void {
    if (!selectedDomainId) {
        showError('Select a domain first.');
        return;
    }

    editingId = null;
    const form = document.getElementById('dns-nameservers-form') as HTMLFormElement | null;
    form?.reset();

    setText('dns-nameservers-edit-title', 'Add Nameserver');
    setCheckboxValue('dns-nameservers-primary', false);
    setInputValue('dns-nameservers-sort-order', String(allNameServers.length));

    showModal('dns-nameservers-edit-modal');
}

function openEdit(id: number): void {
    const ns = allNameServers.find((item) => item.id === id);
    if (!ns) {
        return;
    }

    editingId = id;
    setText('dns-nameservers-edit-title', 'Edit Nameserver');

    setInputValue('dns-nameservers-hostname', ns.hostname);
    setInputValue('dns-nameservers-ip', ns.ipAddress || '');
    setInputValue('dns-nameservers-sort-order', String(ns.sortOrder));
    setCheckboxValue('dns-nameservers-primary', !!ns.isPrimary);

    showModal('dns-nameservers-edit-modal');
}

async function saveNameServer(): Promise<void> {
    if (!selectedDomainId) {
        showError('Select a domain first.');
        return;
    }

    const hostname = getInputValue('dns-nameservers-hostname');
    if (!hostname) {
        showError('Hostname is required.');
        return;
    }

    const payload = {
        domainId: selectedDomainId,
        hostname,
        ipAddress: getInputValue('dns-nameservers-ip') || null,
        isPrimary: getCheckboxValue('dns-nameservers-primary'),
        sortOrder: getNumberValue('dns-nameservers-sort-order'),
    };

    const response = editingId
        ? await apiRequest(`${getApiBaseUrl()}/NameServers/${editingId}`, { method: 'PUT', body: JSON.stringify(payload) })
        : await apiRequest(`${getApiBaseUrl()}/NameServers`, { method: 'POST', body: JSON.stringify(payload) });

    if (!response.success) {
        showError(response.message || 'Failed to save nameserver.');
        return;
    }

    hideModal('dns-nameservers-edit-modal');
    showSuccess(editingId ? 'Nameserver updated.' : 'Nameserver created.');
    await loadNameServers();
}

function openDelete(id: number): void {
    const ns = allNameServers.find((item) => item.id === id);
    if (!ns) {
        return;
    }

    pendingDeleteId = id;
    setText('dns-nameservers-delete-name', ns.hostname || `#${id}`);
    showModal('dns-nameservers-delete-modal');
}

async function deleteNameServer(): Promise<void> {
    if (!pendingDeleteId) {
        return;
    }

    const response = await apiRequest(`${getApiBaseUrl()}/NameServers/${pendingDeleteId}`, { method: 'DELETE' });
    hideModal('dns-nameservers-delete-modal');

    if (!response.success) {
        showError(response.message || 'Delete failed.');
        return;
    }

    showSuccess('Nameserver deleted.');
    pendingDeleteId = null;
    await loadNameServers();
}

function showNoSelection(): void {
    document.getElementById('dns-nameservers-empty-selection')?.classList.remove('d-none');
    document.getElementById('dns-nameservers-loading')?.classList.add('d-none');
    document.getElementById('dns-nameservers-table-wrapper')?.classList.add('d-none');
    document.getElementById('dns-nameservers-empty')?.classList.add('d-none');
}

function showTable(): void {
    document.getElementById('dns-nameservers-empty-selection')?.classList.add('d-none');
    document.getElementById('dns-nameservers-loading')?.classList.add('d-none');
    document.getElementById('dns-nameservers-table-wrapper')?.classList.remove('d-none');
}

function showEmpty(): void {
    document.getElementById('dns-nameservers-empty-selection')?.classList.add('d-none');
    document.getElementById('dns-nameservers-table-wrapper')?.classList.add('d-none');
    document.getElementById('dns-nameservers-empty')?.classList.remove('d-none');
}

function hideEmpty(): void {
    document.getElementById('dns-nameservers-empty')?.classList.add('d-none');
}

function setLoading(isLoading: boolean): void {
    if (isLoading) {
        document.getElementById('dns-nameservers-empty-selection')?.classList.add('d-none');
        document.getElementById('dns-nameservers-table-wrapper')?.classList.add('d-none');
        document.getElementById('dns-nameservers-empty')?.classList.add('d-none');
        document.getElementById('dns-nameservers-loading')?.classList.remove('d-none');
    } else {
        document.getElementById('dns-nameservers-loading')?.classList.add('d-none');
    }
}

function setInputValue(id: string, value: string): void {
    const element = document.getElementById(id) as HTMLInputElement | null;
    if (element) {
        element.value = value;
    }
}

function setCheckboxValue(id: string, value: boolean): void {
    const element = document.getElementById(id) as HTMLInputElement | null;
    if (element) {
        element.checked = value;
    }
}

function getInputValue(id: string): string {
    const element = document.getElementById(id) as HTMLInputElement | null;
    return (element?.value ?? '').trim();
}

function getNumberValue(id: string): number {
    const raw = getInputValue(id);
    const parsed = Number(raw);
    return Number.isFinite(parsed) ? parsed : 0;
}

function getCheckboxValue(id: string): boolean {
    const element = document.getElementById(id) as HTMLInputElement | null;
    return !!element?.checked;
}

function setText(id: string, value: string): void {
    const element = document.getElementById(id);
    if (element) {
        element.textContent = value;
    }
}

function showSuccess(message: string): void {
    const element = document.getElementById('dns-nameservers-alert-success');
    if (!element) {
        return;
    }

    element.textContent = message;
    element.classList.remove('d-none');
    document.getElementById('dns-nameservers-alert-error')?.classList.add('d-none');
}

function showError(message: string): void {
    const element = document.getElementById('dns-nameservers-alert-error');
    if (!element) {
        return;
    }

    element.textContent = message;
    element.classList.remove('d-none');
    document.getElementById('dns-nameservers-alert-success')?.classList.add('d-none');
}

function clearAlerts(): void {
    document.getElementById('dns-nameservers-alert-success')?.classList.add('d-none');
    document.getElementById('dns-nameservers-alert-error')?.classList.add('d-none');
}

function showModal(id: string): void {
    const modalElement = document.getElementById(id);
    if (!modalElement || !(window as any).bootstrap) {
        return;
    }

    const modal = new (window as any).bootstrap.Modal(modalElement);
    modal.show();
}

function hideModal(id: string): void {
    const modalElement = document.getElementById(id);
    if (!modalElement || !(window as any).bootstrap) {
        return;
    }

    const modal = (window as any).bootstrap.Modal.getInstance(modalElement);
    modal?.hide();
}

function esc(value: string): string {
    const map: Record<string, string> = { '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#039;' };
    return (value || '').replace(/[&<>"']/g, (char) => map[char] ?? char);
}

function setupPageObserver(): void {
    initializePage();

    if (document.body) {
        const observer = new MutationObserver(() => {
            const page = document.getElementById('dns-nameservers-page') as HTMLElement | null;
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

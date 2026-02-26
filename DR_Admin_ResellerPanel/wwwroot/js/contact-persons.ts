// @ts-nocheck
(function() {
interface ContactPerson {
    id: number;
    firstName: string;
    lastName: string;
    email: string;
    phone: string;
    position?: string | null;
    department?: string | null;
    isPrimary: boolean;
    isActive: boolean;
    notes?: string | null;
    customerId?: number | null;
    createdAt?: string | null;
    updatedAt?: string | null;
    isDefaultOwner: boolean;
    isDefaultBilling: boolean;
    isDefaultTech: boolean;
    isDefaultAdministrator: boolean;
    isDomainGlobal: boolean;
}

interface CustomerOption {
    id: number;
    name: string;
}

interface PagedResult<T> {
    data?: T[];
    Data?: T[];
    currentPage?: number;
    CurrentPage?: number;
    pageSize?: number;
    PageSize?: number;
    totalCount?: number;
    TotalCount?: number;
    totalPages?: number;
    TotalPages?: number;
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
        console.error('Contact persons request failed', error);
        return {
            success: false,
            message: 'Network error. Please try again.',
        };
    }
}

function extractItems(raw: any): { items: any[]; meta: any } {
    if (Array.isArray(raw)) {
        return { items: raw, meta: null };
    }

    const candidates: any[] = [raw, raw?.data, raw?.Data, raw?.data?.data, raw?.data?.Data];

    const items =
        (Array.isArray(raw?.Data) && raw.Data) ||
        (Array.isArray(raw?.data) && raw.data) ||
        (Array.isArray(raw?.data?.Data) && raw.data.Data) ||
        (Array.isArray(raw?.data?.data) && raw.data.data) ||
        (Array.isArray(raw?.Data?.Data) && raw.Data.Data) ||
        [];

    const meta = candidates.find((c) => c && typeof c === 'object' && (
        c.totalCount !== undefined || c.TotalCount !== undefined ||
        c.totalPages !== undefined || c.TotalPages !== undefined ||
        c.currentPage !== undefined || c.CurrentPage !== undefined ||
        c.pageSize !== undefined || c.PageSize !== undefined
    )) ?? null;

    return { items, meta };
}

let allContactPersons: ContactPerson[] = [];
let customerOptions: CustomerOption[] = [];
let customerLookup = new Map<number, string>();

let editingId: number | null = null;
let pendingDeleteId: number | null = null;

let currentPage = 1;
let pageSize = 25;
let totalCount = 0;
let totalPages = 1;
let filterCustomerId: number | null = null;
let filterSearch = '';

function isAdminUser(): boolean {
    const raw = sessionStorage.getItem('rp_roles');
    if (!raw) {
        return false;
    }

    try {
        const roles = JSON.parse(raw);
        return Array.isArray(roles) && roles.some((role) => String(role).toLowerCase() === 'admin');
    } catch {
        return false;
    }
}

function loadPageSizeFromUi(): void {
    const el = document.getElementById('contact-persons-page-size') as HTMLSelectElement | null;
    const parsed = Number((el?.value ?? '').trim());
    if (Number.isFinite(parsed) && parsed > 0) {
        pageSize = parsed;
    }
}

function buildPagedUrl(): string {
    const params = new URLSearchParams();
    params.set('pageNumber', String(currentPage));
    params.set('pageSize', String(pageSize));
    if (filterCustomerId !== null) {
        params.set('customerId', String(filterCustomerId));
    }
    if (filterSearch) {
        params.set('search', filterSearch);
    }
    return `${getApiBaseUrl()}/ContactPersons?${params.toString()}`;
}

function normalizeContactPerson(item: any): ContactPerson {
    return {
        id: item.id ?? item.Id ?? 0,
        firstName: item.firstName ?? item.FirstName ?? '',
        lastName: item.lastName ?? item.LastName ?? '',
        email: item.email ?? item.Email ?? '',
        phone: item.phone ?? item.Phone ?? '',
        position: item.position ?? item.Position ?? null,
        department: item.department ?? item.Department ?? null,
        isPrimary: item.isPrimary ?? item.IsPrimary ?? false,
        isActive: item.isActive ?? item.IsActive ?? false,
        notes: item.notes ?? item.Notes ?? null,
        customerId: item.customerId ?? item.CustomerId ?? null,
        createdAt: item.createdAt ?? item.CreatedAt ?? null,
        updatedAt: item.updatedAt ?? item.UpdatedAt ?? null,
        isDefaultOwner: item.isDefaultOwner ?? item.IsDefaultOwner ?? false,
        isDefaultBilling: item.isDefaultBilling ?? item.IsDefaultBilling ?? false,
        isDefaultTech: item.isDefaultTech ?? item.IsDefaultTech ?? false,
        isDefaultAdministrator: item.isDefaultAdministrator ?? item.IsDefaultAdministrator ?? false,
        isDomainGlobal: item.isDomainGlobal ?? item.IsDomainGlobal ?? false,
    };
}

function normalizeCustomerOption(item: any): CustomerOption {
    const id = item.id ?? item.Id ?? 0;
    const name = item.customerName ?? item.CustomerName ?? item.name ?? item.Name ?? `Customer ${id}`;
    return { id, name };
}

async function loadCustomersForSelect(): Promise<void> {
    const select = document.getElementById('contact-persons-customer') as HTMLSelectElement | null;
    const filterSelect = document.getElementById('contact-persons-filter-customer') as HTMLSelectElement | null;
    if (!select || !filterSelect) {
        return;
    }

    select.innerHTML = '<option value="">Unassigned</option>';
    filterSelect.innerHTML = '<option value="">All customers</option>';

    const params = new URLSearchParams();
    params.set('pageNumber', '1');
    params.set('pageSize', '1000');

    const response = await apiRequest<PagedResult<CustomerOption> | CustomerOption[]>(`${getApiBaseUrl()}/Customers?${params.toString()}`, { method: 'GET' });
    if (!response.success) {
        return;
    }

    const raw = response.data as any;
    const extracted = extractItems(raw);
    customerOptions = extracted.items.map(normalizeCustomerOption);
    customerOptions.sort((a, b) => a.name.localeCompare(b.name));

    customerLookup = new Map(customerOptions.map((option) => [option.id, option.name]));

    const optionsHtml = customerOptions.map((option) => (
        `<option value="${option.id}">${esc(option.name)}</option>`
    )).join('');

    select.insertAdjacentHTML('beforeend', optionsHtml);
    filterSelect.insertAdjacentHTML('beforeend', optionsHtml);
}

function loadFiltersFromUi(): void {
    const customerFilterEl = document.getElementById('contact-persons-filter-customer') as HTMLSelectElement | null;
    const customerRaw = (customerFilterEl?.value ?? '').trim();
    if (!customerRaw) {
        filterCustomerId = null;
    } else {
        const parsedCustomerId = Number(customerRaw);
        filterCustomerId = Number.isFinite(parsedCustomerId) ? parsedCustomerId : null;
    }

    const searchFilterEl = document.getElementById('contact-persons-filter-search') as HTMLInputElement | null;
    filterSearch = (searchFilterEl?.value ?? '').trim();
}

function applyFilters(): void {
    loadFiltersFromUi();
    currentPage = 1;
    loadContactPersons();
}

function resetFilters(): void {
    const customerFilterEl = document.getElementById('contact-persons-filter-customer') as HTMLSelectElement | null;
    if (customerFilterEl) {
        customerFilterEl.value = '';
    }

    const searchFilterEl = document.getElementById('contact-persons-filter-search') as HTMLInputElement | null;
    if (searchFilterEl) {
        searchFilterEl.value = '';
    }

    applyFilters();
}

async function loadContactPersons(): Promise<void> {
    const tableBody = document.getElementById('contact-persons-table-body');
    if (!tableBody) {
        return;
    }

    tableBody.innerHTML = '<tr><td colspan="10" class="text-center"><div class="spinner-border text-primary"></div></td></tr>';

    loadPageSizeFromUi();
    const response = await apiRequest<PagedResult<ContactPerson> | ContactPerson[]>(buildPagedUrl(), { method: 'GET' });
    if (!response.success) {
        showError(response.message || 'Failed to load contact persons');
        tableBody.innerHTML = '<tr><td colspan="10" class="text-center text-danger">Failed to load data</td></tr>';
        return;
    }

    const raw = response.data as any;
    const extracted = extractItems(raw);
    const meta = extracted.meta ?? raw;

    allContactPersons = extracted.items.map(normalizeContactPerson);

    pageSize = meta?.pageSize ?? meta?.PageSize ?? raw?.pageSize ?? raw?.PageSize ?? pageSize;
    totalCount = meta?.totalCount ?? meta?.TotalCount ?? raw?.totalCount ?? raw?.TotalCount ?? allContactPersons.length;
    totalPages = meta?.totalPages ?? meta?.TotalPages ?? raw?.totalPages ?? raw?.TotalPages ?? Math.max(1, Math.ceil(totalCount / pageSize));
    currentPage = meta?.currentPage ?? meta?.CurrentPage ?? raw?.currentPage ?? raw?.CurrentPage ?? currentPage;

    renderTable();
    renderPagination();
}

function renderTable(): void {
    const tableBody = document.getElementById('contact-persons-table-body');
    if (!tableBody) {
        return;
    }

    if (!allContactPersons.length) {
        tableBody.innerHTML = '<tr><td colspan="10" class="text-center text-muted">No contact persons found.</td></tr>';
        return;
    }

    const adminUser = isAdminUser();

    tableBody.innerHTML = allContactPersons.map((contact) => {
        const fullName = `${contact.firstName} ${contact.lastName}`.trim();
        const customerName = contact.customerId ? customerLookup.get(contact.customerId) || `Customer ${contact.customerId}` : 'Unassigned';
        const primaryBadge = contact.isPrimary
            ? '<span class="badge bg-info">Yes</span>'
            : '<span class="badge bg-secondary">No</span>';
        const activeBadge = contact.isActive
            ? '<span class="badge bg-success">Yes</span>'
            : '<span class="badge bg-secondary">No</span>';
        const defaults = buildDefaultsBadges(contact);

        return `
        <tr>
            <td>${contact.id}</td>
            <td>${esc(customerName)}</td>
            <td>${esc(fullName || '-')}</td>
            <td><a href="mailto:${esc(contact.email)}">${esc(contact.email)}</a></td>
            <td>${esc(contact.phone || '-')}</td>
            <td>${esc(contact.position || '-')}</td>
            <td>${primaryBadge}</td>
            <td>${activeBadge}</td>
            <td>${defaults}</td>
            <td class="text-end">
                <div class="btn-group btn-group-sm">
                    ${adminUser ? `<button class="btn ${contact.isDomainGlobal ? 'btn-success' : 'btn-outline-warning'}" type="button" data-action="toggle-domain-global" data-id="${contact.id}" data-value="${contact.isDomainGlobal ? 'false' : 'true'}" title="${contact.isDomainGlobal ? 'Unset domain global' : 'Set domain global'}"><i class="bi ${contact.isDomainGlobal ? 'bi-globe2' : 'bi-globe'}"></i></button>` : ''}
                    <button class="btn btn-outline-primary" type="button" data-action="edit" data-id="${contact.id}" title="Edit"><i class="bi bi-pencil"></i></button>
                    <button class="btn btn-outline-danger" type="button" data-action="delete" data-id="${contact.id}" data-name="${esc(fullName)}" title="Delete"><i class="bi bi-trash"></i></button>
                </div>
            </td>
        </tr>`;
    }).join('');
}

async function patchIsDomainGlobal(id: number, isDomainGlobal: boolean): Promise<void> {
    const response = await apiRequest(`${getApiBaseUrl()}/ContactPersons/${id}/domain-global`, {
        method: 'PATCH',
        body: JSON.stringify({ isDomainGlobal }),
    });

    if (response.success) {
        showSuccess(`Domain global ${isDomainGlobal ? 'enabled' : 'disabled'} for contact person #${id}`);
        loadContactPersons();
        return;
    }

    showError(response.message || 'Failed to update domain global setting');
}

function buildDefaultsBadges(contact: ContactPerson): string {
    const badges: string[] = [];

    if (contact.isDefaultOwner) {
        badges.push('<span class="badge bg-primary me-1">Owner</span>');
    }
    if (contact.isDefaultBilling) {
        badges.push('<span class="badge bg-warning text-dark me-1">Billing</span>');
    }
    if (contact.isDefaultTech) {
        badges.push('<span class="badge bg-info text-dark me-1">Tech</span>');
    }
    if (contact.isDefaultAdministrator) {
        badges.push('<span class="badge bg-secondary me-1">Admin</span>');
    }

    return badges.length ? badges.join('') : '<span class="text-muted">—</span>';
}

function renderPagingControls(): void {
    const list = document.getElementById('pagingControlsList');
    if (!list) {
        return;
    }

    if (!totalCount || totalPages <= 1) {
        list.innerHTML = '';
        return;
    }

    const makeItem = (label: string, page: number, disabled: boolean, active: boolean = false) => {
        const cls = `page-item${disabled ? ' disabled' : ''}${active ? ' active' : ''}`;
        const ariaCurrent = active ? ' aria-current="page"' : '';
        const ariaDisabled = disabled ? ' aria-disabled="true" tabindex="-1"' : '';
        const dataPage = disabled ? '' : ` data-page="${page}"`;
        return `<li class="${cls}"><a class="page-link" href="#"${dataPage}${ariaCurrent}${ariaDisabled}>${label}</a></li>`;
    };

    const makeEllipsis = () => '<li class="page-item disabled"><span class="page-link">…</span></li>';

    const prevDisabled = currentPage <= 1;
    const nextDisabled = currentPage >= totalPages;

    let html = '';
    html += makeItem('Previous', currentPage - 1, prevDisabled);

    const pages = new Set<number>();
    pages.add(1);
    if (totalPages >= 2) pages.add(2);
    pages.add(totalPages);
    if (totalPages >= 2) pages.add(totalPages - 1);

    const windowSize = 1;
    for (let p = currentPage - windowSize; p <= currentPage + windowSize; p++) {
        if (p >= 1 && p <= totalPages) {
            pages.add(p);
        }
    }

    const sorted = Array.from(pages)
        .filter((p) => p >= 1 && p <= totalPages)
        .sort((a, b) => a - b);

    let last = 0;
    for (const p of sorted) {
        if (last && p - last > 1) {
            html += makeEllipsis();
        }
        html += makeItem(String(p), p, false, p === currentPage);
        last = p;
    }

    html += makeItem('Next', currentPage + 1, nextDisabled);
    list.innerHTML = html;
}

function renderPagination(): void {
    const info = document.getElementById('contact-persons-pagination-info');
    if (!info) {
        return;
    }

    if (!totalCount) {
        info.textContent = 'Showing 0 of 0';
        renderPagingControls();
        return;
    }

    const start = (currentPage - 1) * pageSize + 1;
    const end = Math.min(currentPage * pageSize, totalCount);
    info.textContent = `Showing ${start}-${end} of ${totalCount}`;

    renderPagingControls();
}

function changePage(page: number): void {
    if (page < 1 || page > totalPages) {
        return;
    }

    currentPage = page;
    loadContactPersons();
}

function bindPagingControlsActions(): void {
    const container = document.getElementById('pagingControls');
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
        const page = Number(link.dataset.page);
        if (!Number.isFinite(page)) {
            return;
        }

        changePage(page);
    });
}

function openCreate(): void {
    editingId = null;

    const title = document.getElementById('contact-persons-modal-title');
    if (title) {
        title.textContent = 'New Contact Person';
    }

    const info = document.getElementById('contact-persons-edit-info');
    if (info) {
        info.classList.add('d-none');
        info.textContent = '';
    }

    const form = document.getElementById('contact-persons-form') as HTMLFormElement | null;
    form?.reset();

    setCheckboxValue('contact-persons-is-active', true);
    setCheckboxValue('contact-persons-is-primary', false);
    setCheckboxValue('contact-persons-default-owner', false);
    setCheckboxValue('contact-persons-default-billing', false);
    setCheckboxValue('contact-persons-default-tech', false);
    setCheckboxValue('contact-persons-default-admin', false);

    showModal('contact-persons-edit-modal');
}

function openEdit(id: number): void {
    const contact = allContactPersons.find((c) => c.id === id);
    if (!contact) {
        return;
    }

    editingId = id;

    const title = document.getElementById('contact-persons-modal-title');
    if (title) {
        title.textContent = 'Edit Contact Person';
    }

    const info = document.getElementById('contact-persons-edit-info');
    if (info) {
        const customerName = contact.customerId ? customerLookup.get(contact.customerId) || `Customer ${contact.customerId}` : 'Unassigned';
        info.textContent = `ID: ${contact.id} | Customer: ${customerName}`;
        info.classList.remove('d-none');
    }

    setInputValue('contact-persons-first-name', contact.firstName);
    setInputValue('contact-persons-last-name', contact.lastName);
    setInputValue('contact-persons-email', contact.email);
    setInputValue('contact-persons-phone', contact.phone);
    setInputValue('contact-persons-position', contact.position || '');
    setInputValue('contact-persons-department', contact.department || '');
    setTextAreaValue('contact-persons-notes', contact.notes || '');
    setSelectValue('contact-persons-customer', contact.customerId ? String(contact.customerId) : '');

    setCheckboxValue('contact-persons-is-primary', !!contact.isPrimary);
    setCheckboxValue('contact-persons-is-active', !!contact.isActive);
    setCheckboxValue('contact-persons-default-owner', !!contact.isDefaultOwner);
    setCheckboxValue('contact-persons-default-billing', !!contact.isDefaultBilling);
    setCheckboxValue('contact-persons-default-tech', !!contact.isDefaultTech);
    setCheckboxValue('contact-persons-default-admin', !!contact.isDefaultAdministrator);

    showModal('contact-persons-edit-modal');
}

function setInputValue(id: string, value: string): void {
    const el = document.getElementById(id) as HTMLInputElement | null;
    if (el) {
        el.value = value ?? '';
    }
}

function setTextAreaValue(id: string, value: string): void {
    const el = document.getElementById(id) as HTMLTextAreaElement | null;
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

function setCheckboxValue(id: string, value: boolean): void {
    const el = document.getElementById(id) as HTMLInputElement | null;
    if (el) {
        el.checked = value;
    }
}

function getInputValue(id: string): string {
    const el = document.getElementById(id) as HTMLInputElement | null;
    return (el?.value ?? '').trim();
}

function getTextAreaValue(id: string): string {
    const el = document.getElementById(id) as HTMLTextAreaElement | null;
    return (el?.value ?? '').trim();
}

function getSelectValue(id: string): string {
    const el = document.getElementById(id) as HTMLSelectElement | null;
    return (el?.value ?? '').trim();
}

function getCheckboxValue(id: string): boolean {
    const el = document.getElementById(id) as HTMLInputElement | null;
    return !!el?.checked;
}

function getCustomerIdValue(): number | null {
    const raw = getSelectValue('contact-persons-customer');
    if (!raw) {
        return null;
    }

    const parsed = Number(raw);
    return Number.isFinite(parsed) ? parsed : null;
}

async function saveContactPerson(): Promise<void> {
    const firstName = getInputValue('contact-persons-first-name');
    const lastName = getInputValue('contact-persons-last-name');
    const email = getInputValue('contact-persons-email');
    const phone = getInputValue('contact-persons-phone');

    if (!firstName || !lastName || !email || !phone) {
        showError('First name, last name, email and phone are required');
        return;
    }

    const payload = {
        firstName,
        lastName,
        email,
        phone,
        position: getInputValue('contact-persons-position') || null,
        department: getInputValue('contact-persons-department') || null,
        isPrimary: getCheckboxValue('contact-persons-is-primary'),
        isActive: getCheckboxValue('contact-persons-is-active'),
        notes: getTextAreaValue('contact-persons-notes') || null,
        customerId: getCustomerIdValue(),
        isDefaultOwner: getCheckboxValue('contact-persons-default-owner'),
        isDefaultBilling: getCheckboxValue('contact-persons-default-billing'),
        isDefaultTech: getCheckboxValue('contact-persons-default-tech'),
        isDefaultAdministrator: getCheckboxValue('contact-persons-default-admin'),
    };

    const response = editingId
        ? await apiRequest(`${getApiBaseUrl()}/ContactPersons/${editingId}`, { method: 'PUT', body: JSON.stringify(payload) })
        : await apiRequest(`${getApiBaseUrl()}/ContactPersons`, { method: 'POST', body: JSON.stringify(payload) });

    if (response.success) {
        hideModal('contact-persons-edit-modal');
        showSuccess(editingId ? 'Contact person updated successfully' : 'Contact person created successfully');
        loadContactPersons();
    } else {
        showError(response.message || 'Save failed');
    }
}

function openDelete(id: number, name: string): void {
    pendingDeleteId = id;

    const deleteName = document.getElementById('contact-persons-delete-name');
    if (deleteName) {
        deleteName.textContent = name;
    }

    showModal('contact-persons-delete-modal');
}

async function doDelete(): Promise<void> {
    if (!pendingDeleteId) {
        return;
    }

    const response = await apiRequest(`${getApiBaseUrl()}/ContactPersons/${pendingDeleteId}`, { method: 'DELETE' });
    hideModal('contact-persons-delete-modal');

    if (response.success) {
        showSuccess('Contact person deleted successfully');
        if (currentPage > 1 && allContactPersons.length === 1) {
            currentPage = currentPage - 1;
        }
        loadContactPersons();
    } else {
        showError(response.message || 'Delete failed');
    }

    pendingDeleteId = null;
}

function esc(text: string): string {
    const map: Record<string, string> = { '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#039;' };
    return (text || '').toString().replace(/[&<>"']/g, (char) => map[char]);
}

function showSuccess(message: string): void {
    const alert = document.getElementById('contact-persons-alert-success');
    if (!alert) {
        return;
    }

    alert.textContent = message;
    alert.classList.remove('d-none');

    const errorAlert = document.getElementById('contact-persons-alert-error');
    errorAlert?.classList.add('d-none');

    setTimeout(() => alert.classList.add('d-none'), 5000);
}

function showError(message: string): void {
    const alert = document.getElementById('contact-persons-alert-error');
    if (!alert) {
        return;
    }

    alert.textContent = message;
    alert.classList.remove('d-none');

    const successAlert = document.getElementById('contact-persons-alert-success');
    successAlert?.classList.add('d-none');
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

function bindTableActions(): void {
    const tableBody = document.getElementById('contact-persons-table-body');
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
            openDelete(id, button.dataset.name ?? '');
            return;
        }

        if (button.dataset.action === 'toggle-domain-global') {
            const value = (button.dataset.value ?? '').toLowerCase() === 'true';
            void patchIsDomainGlobal(id, value);
        }
    });
}

function initializeContactPersonsPage(): void {
    const page = document.getElementById('contact-persons-page') as HTMLElement | null;
    if (!page || (page as any).dataset.initialized === 'true') {
        return;
    }

    (page as any).dataset.initialized = 'true';

    document.getElementById('contact-persons-create')?.addEventListener('click', openCreate);
    document.getElementById('contact-persons-save')?.addEventListener('click', saveContactPerson);
    document.getElementById('contact-persons-confirm-delete')?.addEventListener('click', doDelete);

    bindTableActions();
    bindPagingControlsActions();

    document.getElementById('contact-persons-page-size')?.addEventListener('change', () => {
        currentPage = 1;
        loadContactPersons();
    });

    document.getElementById('contact-persons-filter-customer')?.addEventListener('change', applyFilters);
    document.getElementById('contact-persons-filter-reset')?.addEventListener('click', resetFilters);
    document.getElementById('contact-persons-filter-search')?.addEventListener('keydown', (event) => {
        const keyboardEvent = event as KeyboardEvent;
        if (keyboardEvent.key === 'Enter') {
            event.preventDefault();
            applyFilters();
        }
    });
    document.getElementById('contact-persons-filter-search')?.addEventListener('blur', applyFilters);

    loadCustomersForSelect().then(loadContactPersons);
}

function setupPageObserver(): void {
    initializeContactPersonsPage();

    if (document.body) {
        const observer = new MutationObserver(() => {
            const page = document.getElementById('contact-persons-page') as HTMLElement | null;
            if (page && (page as any).dataset.initialized !== 'true') {
                initializeContactPersonsPage();
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

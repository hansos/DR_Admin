// @ts-nocheck
(function() {
interface Registrar {
    id: number;
    name: string;
    code: string;
    isActive: boolean;
    contactEmail?: string | null;
    contactPhone?: string | null;
    website?: string | null;
    notes?: string | null;
    isDefault: boolean;
    createdAt?: string | null;
    updatedAt?: string | null;
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
        console.error('Registrars request failed', error);
        return {
            success: false,
            message: 'Network error. Please try again.',
        };
    }
}

let allRegistrars: Registrar[] = [];
let editingId: number | null = null;
let pendingDeleteId: number | null = null;

let currentPage = 1;
let pageSize = 25;
let totalCount = 0;
let totalPages = 1;

function loadPageSizeFromUi(): void {
    const el = document.getElementById('registrars-page-size') as HTMLSelectElement | null;
    const parsed = Number((el?.value ?? '').trim());
    if (Number.isFinite(parsed) && parsed > 0) {
        pageSize = parsed;
    }
}

function normalizeRegistrar(item: any): Registrar {
    return {
        id: item.id ?? item.Id ?? 0,
        name: item.name ?? item.Name ?? '',
        code: item.code ?? item.Code ?? '',
        isActive: item.isActive ?? item.IsActive ?? false,
        contactEmail: item.contactEmail ?? item.ContactEmail ?? null,
        contactPhone: item.contactPhone ?? item.ContactPhone ?? null,
        website: item.website ?? item.Website ?? null,
        notes: item.notes ?? item.Notes ?? null,
        isDefault: item.isDefault ?? item.IsDefault ?? false,
        createdAt: item.createdAt ?? item.CreatedAt ?? null,
        updatedAt: item.updatedAt ?? item.UpdatedAt ?? null,
    };
}

async function loadRegistrars(): Promise<void> {
    const tableBody = document.getElementById('registrars-table-body');
    if (!tableBody) {
        return;
    }

    tableBody.innerHTML = '<tr><td colspan="8" class="text-center"><div class="spinner-border text-primary"></div></td></tr>';

    const response = await apiRequest<Registrar[]>(`${getApiBaseUrl()}/Registrars`, { method: 'GET' });
    if (!response.success) {
        showError(response.message || 'Failed to load registrars');
        tableBody.innerHTML = '<tr><td colspan="8" class="text-center text-danger">Failed to load data</td></tr>';
        return;
    }

    const rawItems = Array.isArray(response.data)
        ? response.data
        : Array.isArray((response.data as any)?.data)
            ? (response.data as any).data
            : [];

    allRegistrars = rawItems.map(normalizeRegistrar);
    totalCount = allRegistrars.length;

    loadPageSizeFromUi();
    totalPages = Math.max(1, Math.ceil(totalCount / pageSize));
    currentPage = Math.min(currentPage, totalPages);

    renderTable();
    renderPagination();
}

function renderTable(): void {
    const tableBody = document.getElementById('registrars-table-body');
    if (!tableBody) {
        return;
    }

    if (!allRegistrars.length) {
        tableBody.innerHTML = '<tr><td colspan="8" class="text-center text-muted">No registrars found.</td></tr>';
        return;
    }

    const startIndex = (currentPage - 1) * pageSize;
    const pageItems = allRegistrars.slice(startIndex, startIndex + pageSize);

    tableBody.innerHTML = pageItems.map((registrar) => {
        const contactInfo = registrar.contactEmail || registrar.contactPhone
            ? `${registrar.contactEmail ? esc(registrar.contactEmail) : ''}${registrar.contactEmail && registrar.contactPhone ? '<br />' : ''}${registrar.contactPhone ? esc(registrar.contactPhone) : ''}`
            : '<span class="text-muted">-</span>';
        const website = registrar.website ? `<a href="${esc(registrar.website)}" target="_blank" rel="noopener">${esc(registrar.website)}</a>` : '<span class="text-muted">-</span>';
        const activeBadge = registrar.isActive
            ? '<span class="badge bg-success">Yes</span>'
            : '<span class="badge bg-secondary">No</span>';
        const defaultBadge = registrar.isDefault
            ? '<span class="badge bg-primary">Default</span>'
            : '<span class="badge bg-secondary">No</span>';

        return `
        <tr>
            <td>${registrar.id}</td>
            <td>${esc(registrar.name)}</td>
            <td><code>${esc(registrar.code)}</code></td>
            <td>${contactInfo}</td>
            <td>${website}</td>
            <td>${activeBadge}</td>
            <td>${defaultBadge}</td>
            <td class="text-end">
                <div class="btn-group btn-group-sm">
                    <button class="btn btn-outline-primary" type="button" data-action="edit" data-id="${registrar.id}" title="Edit"><i class="bi bi-pencil"></i></button>
                    <button class="btn btn-outline-danger" type="button" data-action="delete" data-id="${registrar.id}" data-name="${esc(registrar.name)}" title="Delete"><i class="bi bi-trash"></i></button>
                </div>
            </td>
        </tr>`;
    }).join('');
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
    const info = document.getElementById('registrars-pagination-info');
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
    renderTable();
    renderPagination();
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

    const title = document.getElementById('registrars-modal-title');
    if (title) {
        title.textContent = 'New Registrar';
    }

    const form = document.getElementById('registrars-form') as HTMLFormElement | null;
    form?.reset();

    setCheckboxValue('registrars-is-active', true);
    setCheckboxValue('registrars-is-default', false);

    showModal('registrars-edit-modal');
}

function openEdit(id: number): void {
    const registrar = allRegistrars.find((r) => r.id === id);
    if (!registrar) {
        return;
    }

    editingId = id;

    const title = document.getElementById('registrars-modal-title');
    if (title) {
        title.textContent = 'Edit Registrar';
    }

    setInputValue('registrars-name', registrar.name);
    setInputValue('registrars-code', registrar.code);
    setInputValue('registrars-contact-email', registrar.contactEmail || '');
    setInputValue('registrars-contact-phone', registrar.contactPhone || '');
    setInputValue('registrars-website', registrar.website || '');
    setTextAreaValue('registrars-notes', registrar.notes || '');

    setCheckboxValue('registrars-is-active', !!registrar.isActive);
    setCheckboxValue('registrars-is-default', !!registrar.isDefault);

    showModal('registrars-edit-modal');
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

function getCheckboxValue(id: string): boolean {
    const el = document.getElementById(id) as HTMLInputElement | null;
    return !!el?.checked;
}

async function saveRegistrar(): Promise<void> {
    const name = getInputValue('registrars-name');
    const code = getInputValue('registrars-code');

    if (!name || !code) {
        showError('Name and Code are required');
        return;
    }

    const payload = {
        name,
        code,
        contactEmail: getInputValue('registrars-contact-email') || null,
        contactPhone: getInputValue('registrars-contact-phone') || null,
        website: getInputValue('registrars-website') || null,
        notes: getTextAreaValue('registrars-notes') || null,
        isActive: getCheckboxValue('registrars-is-active'),
        isDefault: getCheckboxValue('registrars-is-default'),
    };

    const response = editingId
        ? await apiRequest(`${getApiBaseUrl()}/Registrars/${editingId}`, { method: 'PUT', body: JSON.stringify(payload) })
        : await apiRequest(`${getApiBaseUrl()}/Registrars`, { method: 'POST', body: JSON.stringify(payload) });

    if (response.success) {
        hideModal('registrars-edit-modal');
        showSuccess(editingId ? 'Registrar updated successfully' : 'Registrar created successfully');
        loadRegistrars();
    } else {
        showError(response.message || 'Save failed');
    }
}

function openDelete(id: number, name: string): void {
    pendingDeleteId = id;

    const deleteName = document.getElementById('registrars-delete-name');
    if (deleteName) {
        deleteName.textContent = name;
    }

    showModal('registrars-delete-modal');
}

async function doDelete(): Promise<void> {
    if (!pendingDeleteId) {
        return;
    }

    const response = await apiRequest(`${getApiBaseUrl()}/Registrars/${pendingDeleteId}`, { method: 'DELETE' });
    hideModal('registrars-delete-modal');

    if (response.success) {
        showSuccess('Registrar deleted successfully');
        if (currentPage > 1 && (allRegistrars.length - 1) <= (currentPage - 1) * pageSize) {
            currentPage = currentPage - 1;
        }
        loadRegistrars();
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
    const alert = document.getElementById('registrars-alert-success');
    if (!alert) {
        return;
    }

    alert.textContent = message;
    alert.classList.remove('d-none');

    const errorAlert = document.getElementById('registrars-alert-error');
    errorAlert?.classList.add('d-none');

    setTimeout(() => alert.classList.add('d-none'), 5000);
}

function showError(message: string): void {
    const alert = document.getElementById('registrars-alert-error');
    if (!alert) {
        return;
    }

    alert.textContent = message;
    alert.classList.remove('d-none');

    const successAlert = document.getElementById('registrars-alert-success');
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
    const tableBody = document.getElementById('registrars-table-body');
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
        }
    });
}

function initializeRegistrarsPage(): void {
    const page = document.getElementById('registrars-page') as HTMLElement | null;
    if (!page || (page as any).dataset.initialized === 'true') {
        return;
    }

    (page as any).dataset.initialized = 'true';

    document.getElementById('registrars-create')?.addEventListener('click', openCreate);
    document.getElementById('registrars-save')?.addEventListener('click', saveRegistrar);
    document.getElementById('registrars-confirm-delete')?.addEventListener('click', doDelete);

    bindTableActions();
    bindPagingControlsActions();

    document.getElementById('registrars-page-size')?.addEventListener('change', () => {
        currentPage = 1;
        renderTable();
        renderPagination();
    });

    loadRegistrars();
}

function setupPageObserver(): void {
    initializeRegistrarsPage();

    if (document.body) {
        const observer = new MutationObserver(() => {
            const page = document.getElementById('registrars-page') as HTMLElement | null;
            if (page && (page as any).dataset.initialized !== 'true') {
                initializeRegistrarsPage();
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

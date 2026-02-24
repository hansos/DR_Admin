// @ts-nocheck
(function() {
interface Customer {
    id: number;
    referenceNumber: number;
    formattedReferenceNumber?: string | null;
    customerNumber?: number | null;
    formattedCustomerNumber?: string | null;
    name: string;
    email: string;
    phone: string;
    customerName?: string | null;
    taxId?: string | null;
    vatNumber?: string | null;
    isCompany: boolean;
    isActive: boolean;
    status: string;
    balance?: number;
    creditLimit: number;
    notes?: string | null;
    billingEmail?: string | null;
    preferredPaymentMethod?: string | null;
    preferredCurrency: string;
    allowCurrencyOverride: boolean;
    createdAt?: string;
    updatedAt?: string;
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
        console.error('Customers request failed', error);
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

let allCustomers: Customer[] = [];
let editingId: number | null = null;
let pendingDeleteId: number | null = null;

let currentPage = 1;
let pageSize = 25;
let totalCount = 0;
let totalPages = 1;

function loadPageSizeFromUi(): void {
    const el = document.getElementById('customers-page-size') as HTMLSelectElement | null;
    const parsed = Number((el?.value ?? '').trim());
    if (Number.isFinite(parsed) && parsed > 0) {
        pageSize = parsed;
    }
}

function buildPagedUrl(): string {
    const params = new URLSearchParams();
    params.set('pageNumber', String(currentPage));
    params.set('pageSize', String(pageSize));
    return `${getApiBaseUrl()}/Customers?${params.toString()}`;
}

function normalizeItem(item: any): Customer {
    return {
        id: item.id ?? item.Id ?? 0,
        referenceNumber: item.referenceNumber ?? item.ReferenceNumber ?? 0,
        formattedReferenceNumber: item.formattedReferenceNumber ?? item.FormattedReferenceNumber ?? null,
        customerNumber: item.customerNumber ?? item.CustomerNumber ?? null,
        formattedCustomerNumber: item.formattedCustomerNumber ?? item.FormattedCustomerNumber ?? null,
        name: item.name ?? item.Name ?? '',
        email: item.email ?? item.Email ?? '',
        phone: item.phone ?? item.Phone ?? '',
        customerName: item.customerName ?? item.CustomerName ?? null,
        taxId: item.taxId ?? item.TaxId ?? null,
        vatNumber: item.vatNumber ?? item.VatNumber ?? null,
        isCompany: item.isCompany ?? item.IsCompany ?? false,
        isActive: item.isActive ?? item.IsActive ?? false,
        status: item.status ?? item.Status ?? 'Active',
        balance: item.balance ?? item.Balance ?? 0,
        creditLimit: item.creditLimit ?? item.CreditLimit ?? 0,
        notes: item.notes ?? item.Notes ?? null,
        billingEmail: item.billingEmail ?? item.BillingEmail ?? null,
        preferredPaymentMethod: item.preferredPaymentMethod ?? item.PreferredPaymentMethod ?? null,
        preferredCurrency: item.preferredCurrency ?? item.PreferredCurrency ?? 'EUR',
        allowCurrencyOverride: item.allowCurrencyOverride ?? item.AllowCurrencyOverride ?? true,
        createdAt: item.createdAt ?? item.CreatedAt ?? null,
        updatedAt: item.updatedAt ?? item.UpdatedAt ?? null,
    };
}

async function loadCustomers(): Promise<void> {
    const tableBody = document.getElementById('customers-table-body');
    if (!tableBody) {
        return;
    }

    tableBody.innerHTML = '<tr><td colspan="9" class="text-center"><div class="spinner-border text-primary"></div></td></tr>';

    loadPageSizeFromUi();
    const response = await apiRequest<PagedResult<Customer> | Customer[]>(buildPagedUrl(), { method: 'GET' });
    if (!response.success) {
        showError(response.message || 'Failed to load customers');
        tableBody.innerHTML = '<tr><td colspan="9" class="text-center text-danger">Failed to load data</td></tr>';
        return;
    }

    const raw = response.data as any;
    const extracted = extractItems(raw);
    const meta = extracted.meta ?? raw;

    allCustomers = extracted.items.map(normalizeItem);

    pageSize = meta?.pageSize ?? meta?.PageSize ?? raw?.pageSize ?? raw?.PageSize ?? pageSize;
    totalCount = meta?.totalCount ?? meta?.TotalCount ?? raw?.totalCount ?? raw?.TotalCount ?? allCustomers.length;
    totalPages = meta?.totalPages ?? meta?.TotalPages ?? raw?.totalPages ?? raw?.TotalPages ?? Math.max(1, Math.ceil(totalCount / pageSize));
    currentPage = meta?.currentPage ?? meta?.CurrentPage ?? raw?.currentPage ?? raw?.CurrentPage ?? currentPage;

    renderTable();
    renderPagination();
}

function renderTable(): void {
    const tableBody = document.getElementById('customers-table-body');
    if (!tableBody) {
        return;
    }

    if (!allCustomers.length) {
        tableBody.innerHTML = '<tr><td colspan="9" class="text-center text-muted">No customers found.</td></tr>';
        return;
    }

    tableBody.innerHTML = allCustomers.map((customer) => {
        const reference = customer.formattedReferenceNumber || (customer.referenceNumber ? String(customer.referenceNumber) : '-');
        const customerNo = customer.formattedCustomerNumber || (customer.customerNumber ? String(customer.customerNumber) : '-');
        const status = customer.status || '-';
        const activeBadge = customer.isActive
            ? '<span class="badge bg-success">Yes</span>'
            : '<span class="badge bg-secondary">No</span>';

        return `
        <tr>
            <td>${customer.id}</td>
            <td>${esc(reference)}</td>
            <td>${esc(customerNo)}</td>
            <td>${esc(customer.name)}</td>
            <td><a href="mailto:${esc(customer.email)}">${esc(customer.email)}</a></td>
            <td>${esc(customer.phone || '-')}</td>
            <td>${esc(status)}</td>
            <td>${activeBadge}</td>
            <td class="text-end">
                <div class="btn-group btn-group-sm">
                    <button class="btn btn-outline-primary" type="button" data-action="edit" data-id="${customer.id}" title="Edit"><i class="bi bi-pencil"></i></button>
                    <button class="btn btn-outline-danger" type="button" data-action="delete" data-id="${customer.id}" data-name="${esc(customer.name)}" title="Delete"><i class="bi bi-trash"></i></button>
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
    const info = document.getElementById('customers-pagination-info');
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
    loadCustomers();
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

    const title = document.getElementById('customers-modal-title');
    if (title) {
        title.textContent = 'New Customer';
    }

    const info = document.getElementById('customers-edit-info');
    if (info) {
        info.classList.add('d-none');
        info.textContent = '';
    }

    const form = document.getElementById('customers-form') as HTMLFormElement | null;
    form?.reset();

    const isActive = document.getElementById('customers-is-active') as HTMLInputElement | null;
    if (isActive) {
        isActive.checked = true;
    }

    const allowCurrencyOverride = document.getElementById('customers-allow-currency-override') as HTMLInputElement | null;
    if (allowCurrencyOverride) {
        allowCurrencyOverride.checked = true;
    }

    showModal('customers-edit-modal');
}

function openEdit(id: number): void {
    const customer = allCustomers.find((c) => c.id === id);
    if (!customer) {
        return;
    }

    editingId = id;

    const title = document.getElementById('customers-modal-title');
    if (title) {
        title.textContent = 'Edit Customer';
    }

    const info = document.getElementById('customers-edit-info');
    if (info) {
        const reference = customer.formattedReferenceNumber || (customer.referenceNumber ? String(customer.referenceNumber) : '-');
        const customerNo = customer.formattedCustomerNumber || (customer.customerNumber ? String(customer.customerNumber) : '-');
        info.textContent = `ID: ${customer.id} | Reference: ${reference} | Customer: ${customerNo}`;
        info.classList.remove('d-none');
    }

    setInputValue('customers-name', customer.name);
    setInputValue('customers-customer-name', customer.customerName || '');
    setInputValue('customers-email', customer.email);
    setInputValue('customers-billing-email', customer.billingEmail || '');
    setInputValue('customers-phone', customer.phone);
    setInputValue('customers-tax-id', customer.taxId || '');
    setInputValue('customers-vat-number', customer.vatNumber || '');
    setInputValue('customers-status', customer.status || 'Active');
    setInputValue('customers-preferred-currency', customer.preferredCurrency || 'EUR');
    setInputValue('customers-credit-limit', String(customer.creditLimit ?? 0));
    setInputValue('customers-preferred-payment-method', customer.preferredPaymentMethod || '');
    setTextAreaValue('customers-notes', customer.notes || '');

    setCheckboxValue('customers-is-company', !!customer.isCompany);
    setCheckboxValue('customers-is-active', !!customer.isActive);
    setCheckboxValue('customers-allow-currency-override', customer.allowCurrencyOverride !== false);

    showModal('customers-edit-modal');
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

function getNumberValue(id: string): number {
    const raw = getInputValue(id);
    if (!raw) {
        return 0;
    }

    const parsed = Number(raw);
    return Number.isFinite(parsed) ? parsed : 0;
}

function getCheckboxValue(id: string): boolean {
    const el = document.getElementById(id) as HTMLInputElement | null;
    return !!el?.checked;
}

async function saveCustomer(): Promise<void> {
    const name = getInputValue('customers-name');
    const email = getInputValue('customers-email');
    const phone = getInputValue('customers-phone');

    if (!name || !email || !phone) {
        showError('Name, Email and Phone are required');
        return;
    }

    const payload = {
        name,
        customerName: getInputValue('customers-customer-name') || null,
        email,
        billingEmail: getInputValue('customers-billing-email') || null,
        phone,
        taxId: getInputValue('customers-tax-id') || null,
        vatNumber: getInputValue('customers-vat-number') || null,
        status: getInputValue('customers-status') || 'Active',
        preferredCurrency: getInputValue('customers-preferred-currency') || 'EUR',
        creditLimit: getNumberValue('customers-credit-limit'),
        preferredPaymentMethod: getInputValue('customers-preferred-payment-method') || null,
        notes: getTextAreaValue('customers-notes') || null,
        isCompany: getCheckboxValue('customers-is-company'),
        isActive: getCheckboxValue('customers-is-active'),
        allowCurrencyOverride: getCheckboxValue('customers-allow-currency-override'),
    };

    const response = editingId
        ? await apiRequest(`${getApiBaseUrl()}/Customers/${editingId}`, { method: 'PUT', body: JSON.stringify(payload) })
        : await apiRequest(`${getApiBaseUrl()}/Customers`, { method: 'POST', body: JSON.stringify(payload) });

    if (response.success) {
        hideModal('customers-edit-modal');
        showSuccess(editingId ? 'Customer updated successfully' : 'Customer created successfully');
        loadCustomers();
    } else {
        showError(response.message || 'Save failed');
    }
}

function openDelete(id: number, name: string): void {
    pendingDeleteId = id;

    const deleteName = document.getElementById('customers-delete-name');
    if (deleteName) {
        deleteName.textContent = name;
    }

    showModal('customers-delete-modal');
}

async function doDelete(): Promise<void> {
    if (!pendingDeleteId) {
        return;
    }

    const response = await apiRequest(`${getApiBaseUrl()}/Customers/${pendingDeleteId}`, { method: 'DELETE' });
    hideModal('customers-delete-modal');

    if (response.success) {
        showSuccess('Customer deleted successfully');
        if (currentPage > 1 && allCustomers.length === 1) {
            currentPage = currentPage - 1;
        }
        loadCustomers();
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
    const alert = document.getElementById('customers-alert-success');
    if (!alert) {
        return;
    }

    alert.textContent = message;
    alert.classList.remove('d-none');

    const errorAlert = document.getElementById('customers-alert-error');
    errorAlert?.classList.add('d-none');

    setTimeout(() => alert.classList.add('d-none'), 5000);
}

function showError(message: string): void {
    const alert = document.getElementById('customers-alert-error');
    if (!alert) {
        return;
    }

    alert.textContent = message;
    alert.classList.remove('d-none');

    const successAlert = document.getElementById('customers-alert-success');
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
    const tableBody = document.getElementById('customers-table-body');
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

function initializeCustomersPage(): void {
    const page = document.getElementById('customers-page') as HTMLElement | null;
    if (!page || (page as any).dataset.initialized === 'true') {
        return;
    }

    (page as any).dataset.initialized = 'true';

    document.getElementById('customers-create')?.addEventListener('click', openCreate);
    document.getElementById('customers-save')?.addEventListener('click', saveCustomer);
    document.getElementById('customers-confirm-delete')?.addEventListener('click', doDelete);

    bindTableActions();
    bindPagingControlsActions();

    document.getElementById('customers-page-size')?.addEventListener('change', () => {
        currentPage = 1;
        loadCustomers();
    });

    loadCustomers();
}

function setupPageObserver(): void {
    initializeCustomersPage();

    if (document.body) {
        const observer = new MutationObserver(() => {
            const page = document.getElementById('customers-page') as HTMLElement | null;
            if (page && (page as any).dataset.initialized !== 'true') {
                initializeCustomersPage();
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

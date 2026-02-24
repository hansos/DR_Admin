// @ts-nocheck
(function() {
interface Role {
    id: number;
    name: string;
    description: string;
    code: string;
    createdAt?: string;
    updatedAt?: string;
}

interface ApiResponse<T> {
    success: boolean;
    data?: T;
    message?: string;
}

function getApiBaseUrl(): string {
    const baseUrl = (window as any).AppSettings?.apiBaseUrl;
    if (!baseUrl) {
        const fallback = window.location.protocol === 'https:'
            ? 'https://localhost:7201/api/v1'
            : 'http://localhost:5133/api/v1';
        return fallback;
    }
    return baseUrl;
}

let allRoles: Role[] = [];
let editingId: number | null = null;
let pendingDeleteId: number | null = null;

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
            data: ((data as any)?.data ?? data) as T,
            message: (data as any)?.message,
        };
    } catch (error) {
        console.error('Roles request failed', error);
        return {
            success: false,
            message: 'Network error. Please try again.',
        };
    }
}

async function loadRoles(): Promise<void> {
    const tableBody = document.getElementById('roles-table-body');
    if (!tableBody) {
        return;
    }

    tableBody.innerHTML = '<tr><td colspan="6" class="text-center"><div class="spinner-border text-primary"></div></td></tr>';

    const response = await apiRequest<Role[]>(`${getApiBaseUrl()}/Roles`, { method: 'GET' });
    if (!response.success) {
        showError(response.message || 'Failed to load roles');
        tableBody.innerHTML = '<tr><td colspan="6" class="text-center text-danger">Failed to load data</td></tr>';
        return;
    }

    const rawItems = Array.isArray(response.data)
        ? response.data
        : Array.isArray((response.data as any)?.data)
            ? (response.data as any).data
            : [];

    allRoles = rawItems.map((item: any) => ({
        id: item.id ?? item.Id ?? 0,
        name: item.name ?? item.Name ?? '',
        description: item.description ?? item.Description ?? '',
        code: item.code ?? item.Code ?? '',
        createdAt: item.createdAt ?? item.CreatedAt ?? null,
        updatedAt: item.updatedAt ?? item.UpdatedAt ?? null,
    }));

    renderTable();
}

function renderTable(): void {
    const tableBody = document.getElementById('roles-table-body');
    if (!tableBody) {
        return;
    }

    if (!allRoles.length) {
        tableBody.innerHTML = '<tr><td colspan="6" class="text-center text-muted">No roles found. Click "New Role" to add one.</td></tr>';
        return;
    }

    tableBody.innerHTML = allRoles.map((role) => {
        const created = role.createdAt ? formatDate(role.createdAt) : '-';

        return `
        <tr>
            <td>${role.id}</td>
            <td><code>${esc(role.code)}</code></td>
            <td>${esc(role.name)}</td>
            <td>${esc(role.description || '-')}</td>
            <td>${created}</td>
            <td>
                <div class="btn-group btn-group-sm">
                    <button class="btn btn-outline-primary" type="button" data-action="edit" data-id="${role.id}" title="Edit"><i class="bi bi-pencil"></i></button>
                    <button class="btn btn-outline-danger" type="button" data-action="delete" data-id="${role.id}" data-name="${esc(role.name)}" title="Delete"><i class="bi bi-trash"></i></button>
                </div>
            </td>
        </tr>`;
    }).join('');
}

function openCreate(): void {
    editingId = null;

    const modalTitle = document.getElementById('roles-modal-title');
    if (modalTitle) {
        modalTitle.textContent = 'New Role';
    }

    const form = document.getElementById('roles-form') as HTMLFormElement | null;
    form?.reset();

    showModal('roles-edit-modal');
}

function openEdit(id: number): void {
    const role = allRoles.find((item) => item.id === id);
    if (!role) {
        return;
    }

    editingId = id;

    const modalTitle = document.getElementById('roles-modal-title');
    if (modalTitle) {
        modalTitle.textContent = 'Edit Role';
    }

    const codeInput = document.getElementById('roles-code') as HTMLInputElement | null;
    const nameInput = document.getElementById('roles-name') as HTMLInputElement | null;
    const descriptionInput = document.getElementById('roles-description') as HTMLTextAreaElement | null;

    if (codeInput) {
        codeInput.value = role.code;
    }
    if (nameInput) {
        nameInput.value = role.name;
    }
    if (descriptionInput) {
        descriptionInput.value = role.description || '';
    }

    showModal('roles-edit-modal');
}

async function saveRole(): Promise<void> {
    const codeInput = document.getElementById('roles-code') as HTMLInputElement | null;
    const nameInput = document.getElementById('roles-name') as HTMLInputElement | null;
    const descriptionInput = document.getElementById('roles-description') as HTMLTextAreaElement | null;

    const code = codeInput?.value.trim() ?? '';
    const name = nameInput?.value.trim() ?? '';

    if (!code || !name) {
        showError('Code and Name are required');
        return;
    }

    const payload = {
        code,
        name,
        description: descriptionInput?.value.trim() ?? '',
    };

    const response = editingId
        ? await apiRequest(`${getApiBaseUrl()}/Roles/${editingId}`, { method: 'PUT', body: JSON.stringify(payload) })
        : await apiRequest(`${getApiBaseUrl()}/Roles`, { method: 'POST', body: JSON.stringify(payload) });

    if (response.success) {
        hideModal('roles-edit-modal');
        showSuccess(editingId ? 'Role updated successfully' : 'Role created successfully');
        loadRoles();
    } else {
        showError(response.message || 'Save failed');
    }
}

function openDelete(id: number, name: string): void {
    pendingDeleteId = id;

    const deleteName = document.getElementById('roles-delete-name');
    if (deleteName) {
        deleteName.textContent = name;
    }

    showModal('roles-delete-modal');
}

async function doDelete(): Promise<void> {
    if (!pendingDeleteId) {
        return;
    }

    const response = await apiRequest(`${getApiBaseUrl()}/Roles/${pendingDeleteId}`, { method: 'DELETE' });
    hideModal('roles-delete-modal');

    if (response.success) {
        showSuccess('Role deleted successfully');
        loadRoles();
    } else {
        showError(response.message || 'Delete failed');
    }

    pendingDeleteId = null;
}

function esc(text: string): string {
    const map: Record<string, string> = { '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#039;' };
    return (text || '').toString().replace(/[&<>"']/g, (char) => map[char]);
}

function formatDate(value?: string | null): string {
    if (!value) {
        return '';
    }

    try {
        const date = new Date(value);
        if (isNaN(date.getTime())) {
            return value;
        }

        return date.toLocaleString();
    } catch {
        return value;
    }
}

function showSuccess(message: string): void {
    const alert = document.getElementById('roles-alert-success');
    if (!alert) {
        return;
    }

    alert.textContent = message;
    alert.classList.remove('d-none');

    const errorAlert = document.getElementById('roles-alert-error');
    errorAlert?.classList.add('d-none');

    setTimeout(() => alert.classList.add('d-none'), 5000);
}

function showError(message: string): void {
    const alert = document.getElementById('roles-alert-error');
    if (!alert) {
        return;
    }

    alert.textContent = message;
    alert.classList.remove('d-none');

    const successAlert = document.getElementById('roles-alert-success');
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
    const tableBody = document.getElementById('roles-table-body');
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

function initializeRolesPage(): void {
    const page = document.getElementById('roles-page');
    if (!page || page.getAttribute('data-initialized') === 'true') {
        return;
    }

    page.setAttribute('data-initialized', 'true');

    document.getElementById('roles-create')?.addEventListener('click', openCreate);
    document.getElementById('roles-save')?.addEventListener('click', () => { saveRole(); });
    document.getElementById('roles-confirm-delete')?.addEventListener('click', () => { doDelete(); });

    bindTableActions();
    loadRoles();
}

function setupPageObserver(): void {
    initializeRolesPage();

    if (document.body) {
        const observer = new MutationObserver(() => {
            const page = document.getElementById('roles-page');
            if (page && page.getAttribute('data-initialized') !== 'true') {
                initializeRolesPage();
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

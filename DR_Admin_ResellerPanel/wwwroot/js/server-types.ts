// @ts-nocheck
(function() {
interface ServerType {
    id: number;
    name: string;
    displayName: string;
    description?: string | null;
    isActive: boolean;
}

interface ApiResponse<T> {
    success: boolean;
    data?: T;
    message?: string;
}

function getApiBaseUrl(): string {
    return (window as any).AppSettings?.apiBaseUrl ?? '';
}

let allServerTypes: ServerType[] = [];
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
            success: data?.success !== false,
            data: data?.data ?? data,
            message: data?.message,
        };
    } catch (error) {
        console.error('Server types request failed', error);
        return {
            success: false,
            message: 'Network error. Please try again.',
        };
    }
}

async function loadServerTypes(): Promise<void> {
    const tableBody = document.getElementById('server-types-table-body');
    if (!tableBody) {
        return;
    }

    tableBody.innerHTML = '<tr><td colspan="6" class="text-center"><div class="spinner-border text-primary"></div></td></tr>';

    const response = await apiRequest<ServerType[]>(`${getApiBaseUrl()}/ServerTypes`, { method: 'GET' });
    if (!response.success) {
        showError(response.message || 'Failed to load server types');
        tableBody.innerHTML = '<tr><td colspan="6" class="text-center text-danger">Failed to load data</td></tr>';
        return;
    }

    const rawItems = Array.isArray(response.data)
        ? response.data
        : Array.isArray((response.data as any)?.data)
            ? (response.data as any).data
            : [];

    allServerTypes = rawItems.map((item: any) => ({
        id: item.id ?? item.Id ?? 0,
        name: item.name ?? item.Name ?? '',
        displayName: item.displayName ?? item.DisplayName ?? '',
        description: item.description ?? item.Description ?? null,
        isActive: item.isActive ?? item.IsActive ?? false,
    }));

    renderTable();
}

function renderTable(): void {
    const tableBody = document.getElementById('server-types-table-body');
    if (!tableBody) {
        return;
    }

    if (!allServerTypes.length) {
        tableBody.innerHTML = '<tr><td colspan="6" class="text-center text-muted">No server types found. Click "New Server Type" to add one.</td></tr>';
        return;
    }

    tableBody.innerHTML = allServerTypes.map((type) => `
        <tr>
            <td>${type.id}</td>
            <td><code>${esc(type.name)}</code></td>
            <td>${esc(type.displayName)}</td>
            <td>${esc(type.description || '-')}</td>
            <td><span class="badge bg-${type.isActive ? 'success' : 'secondary'}">${type.isActive ? 'Active' : 'Inactive'}</span></td>
            <td>
                <div class="btn-group btn-group-sm">
                    <button class="btn btn-outline-primary" type="button" data-action="edit" data-id="${type.id}" title="Edit"><i class="bi bi-pencil"></i></button>
                    <button class="btn btn-outline-danger" type="button" data-action="delete" data-id="${type.id}" data-name="${esc(type.displayName)}" title="Delete"><i class="bi bi-trash"></i></button>
                </div>
            </td>
        </tr>
    `).join('');
}

function openCreate(): void {
    editingId = null;

    const modalTitle = document.getElementById('server-types-modal-title');
    if (modalTitle) {
        modalTitle.textContent = 'New Server Type';
    }

    const form = document.getElementById('server-types-form') as HTMLFormElement | null;
    form?.reset();

    const isActiveInput = document.getElementById('server-types-is-active') as HTMLInputElement | null;
    if (isActiveInput) {
        isActiveInput.checked = true;
    }

    showModal('server-types-edit-modal');
}

function openEdit(id: number): void {
    const type = allServerTypes.find((item) => item.id === id);
    if (!type) {
        return;
    }

    editingId = id;

    const modalTitle = document.getElementById('server-types-modal-title');
    if (modalTitle) {
        modalTitle.textContent = 'Edit Server Type';
    }

    const nameInput = document.getElementById('server-types-name') as HTMLInputElement | null;
    const displayNameInput = document.getElementById('server-types-display-name') as HTMLInputElement | null;
    const descriptionInput = document.getElementById('server-types-description') as HTMLTextAreaElement | null;
    const isActiveInput = document.getElementById('server-types-is-active') as HTMLInputElement | null;

    if (nameInput) {
        nameInput.value = type.name;
    }
    if (displayNameInput) {
        displayNameInput.value = type.displayName;
    }
    if (descriptionInput) {
        descriptionInput.value = type.description || '';
    }
    if (isActiveInput) {
        isActiveInput.checked = type.isActive;
    }

    showModal('server-types-edit-modal');
}

async function saveServerType(): Promise<void> {
    const nameInput = document.getElementById('server-types-name') as HTMLInputElement | null;
    const displayNameInput = document.getElementById('server-types-display-name') as HTMLInputElement | null;
    const descriptionInput = document.getElementById('server-types-description') as HTMLTextAreaElement | null;
    const isActiveInput = document.getElementById('server-types-is-active') as HTMLInputElement | null;

    const name = nameInput?.value.trim() ?? '';
    const displayName = displayNameInput?.value.trim() ?? '';

    if (!name || !displayName) {
        showError('Internal Name and Display Name are required');
        return;
    }

    const payload = {
        name,
        displayName,
        description: descriptionInput?.value.trim() || null,
        isActive: isActiveInput?.checked ?? false,
    };

    const response = editingId
        ? await apiRequest(`${getApiBaseUrl()}/ServerTypes/${editingId}`, { method: 'PUT', body: JSON.stringify(payload) })
        : await apiRequest(`${getApiBaseUrl()}/ServerTypes`, { method: 'POST', body: JSON.stringify(payload) });

    if (response.success) {
        hideModal('server-types-edit-modal');
        showSuccess(editingId ? 'Server type updated successfully' : 'Server type created successfully');
        loadServerTypes();
    } else {
        showError(response.message || 'Save failed');
    }
}

function openDelete(id: number, name: string): void {
    pendingDeleteId = id;

    const deleteName = document.getElementById('server-types-delete-name');
    if (deleteName) {
        deleteName.textContent = name;
    }

    showModal('server-types-delete-modal');
}

async function doDelete(): Promise<void> {
    if (!pendingDeleteId) {
        return;
    }

    const response = await apiRequest(`${getApiBaseUrl()}/ServerTypes/${pendingDeleteId}`, { method: 'DELETE' });
    hideModal('server-types-delete-modal');

    if (response.success) {
        showSuccess('Server type deleted successfully');
        loadServerTypes();
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
    const alert = document.getElementById('server-types-alert-success');
    if (!alert) {
        return;
    }

    alert.textContent = message;
    alert.classList.remove('d-none');

    const errorAlert = document.getElementById('server-types-alert-error');
    errorAlert?.classList.add('d-none');

    setTimeout(() => alert.classList.add('d-none'), 5000);
}

function showError(message: string): void {
    const alert = document.getElementById('server-types-alert-error');
    if (!alert) {
        return;
    }

    alert.textContent = message;
    alert.classList.remove('d-none');

    const successAlert = document.getElementById('server-types-alert-success');
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
    const tableBody = document.getElementById('server-types-table-body');
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

function initializeServerTypesPage(): void {
    const page = document.getElementById('server-types-page');
    if (!page || page.dataset.initialized === 'true') {
        return;
    }

    page.dataset.initialized = 'true';

    document.getElementById('server-types-create')?.addEventListener('click', openCreate);
    document.getElementById('server-types-save')?.addEventListener('click', saveServerType);
    document.getElementById('server-types-confirm-delete')?.addEventListener('click', doDelete);

    bindTableActions();
    loadServerTypes();
}

function setupPageObserver(): void {
    // Try immediate initialization
    initializeServerTypesPage();

    // Set up MutationObserver for Blazor navigation
    if (document.body) {
        const observer = new MutationObserver(() => {
            const page = document.getElementById('server-types-page');
            if (page && page.dataset.initialized !== 'true') {
                initializeServerTypesPage();
            }
        });
        observer.observe(document.body, { childList: true, subtree: true });
    }
}

// Initialize when DOM is ready
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', setupPageObserver);
} else {
    setupPageObserver();
}
})();

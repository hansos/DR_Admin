// @ts-nocheck
(function() {
interface OperatingSystem {
    id: number;
    name: string;
    displayName: string;
    version?: string | null;
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

let allOperatingSystems: OperatingSystem[] = [];
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
        console.error('Operating systems request failed', error);
        return {
            success: false,
            message: 'Network error. Please try again.',
        };
    }
}

async function loadOperatingSystems(): Promise<void> {
    const tableBody = document.getElementById('operating-systems-table-body');
    if (!tableBody) {
        return;
    }

    tableBody.innerHTML = '<tr><td colspan="7" class="text-center"><div class="spinner-border text-primary"></div></td></tr>';

    const response = await apiRequest<OperatingSystem[]>(`${getApiBaseUrl()}/OperatingSystems`, { method: 'GET' });
    if (!response.success) {
        showError(response.message || 'Failed to load operating systems');
        tableBody.innerHTML = '<tr><td colspan="7" class="text-center text-danger">Failed to load data</td></tr>';
        return;
    }

    const rawItems = Array.isArray(response.data)
        ? response.data
        : Array.isArray((response.data as any)?.data)
            ? (response.data as any).data
            : [];

    allOperatingSystems = rawItems.map((item: any) => ({
        id: item.id ?? item.Id ?? 0,
        name: item.name ?? item.Name ?? '',
        displayName: item.displayName ?? item.DisplayName ?? '',
        version: item.version ?? item.Version ?? null,
        description: item.description ?? item.Description ?? null,
        isActive: item.isActive ?? item.IsActive ?? false,
    }));

    renderTable();
}

function renderTable(): void {
    const tableBody = document.getElementById('operating-systems-table-body');
    if (!tableBody) {
        return;
    }

    if (!allOperatingSystems.length) {
        tableBody.innerHTML = '<tr><td colspan="7" class="text-center text-muted">No operating systems found. Click "New Operating System" to add one.</td></tr>';
        return;
    }

    tableBody.innerHTML = allOperatingSystems.map((os) => `
        <tr>
            <td>${os.id}</td>
            <td><code>${esc(os.name)}</code></td>
            <td>${esc(os.displayName)}</td>
            <td>${esc(os.version || '-')}</td>
            <td>${esc(os.description || '-')}</td>
            <td><span class="badge bg-${os.isActive ? 'success' : 'secondary'}">${os.isActive ? 'Active' : 'Inactive'}</span></td>
            <td>
                <div class="btn-group btn-group-sm">
                    <button class="btn btn-outline-primary" type="button" data-action="edit" data-id="${os.id}" title="Edit"><i class="bi bi-pencil"></i></button>
                    <button class="btn btn-outline-danger" type="button" data-action="delete" data-id="${os.id}" data-name="${esc(os.displayName)}" title="Delete"><i class="bi bi-trash"></i></button>
                </div>
            </td>
        </tr>
    `).join('');
}

function openCreate(): void {
    editingId = null;

    const modalTitle = document.getElementById('operating-systems-modal-title');
    if (modalTitle) {
        modalTitle.textContent = 'New Operating System';
    }

    const form = document.getElementById('operating-systems-form') as HTMLFormElement | null;
    form?.reset();

    const isActiveInput = document.getElementById('operating-systems-is-active') as HTMLInputElement | null;
    if (isActiveInput) {
        isActiveInput.checked = true;
    }

    showModal('operating-systems-edit-modal');
}

function openEdit(id: number): void {
    const os = allOperatingSystems.find((item) => item.id === id);
    if (!os) {
        return;
    }

    editingId = id;

    const modalTitle = document.getElementById('operating-systems-modal-title');
    if (modalTitle) {
        modalTitle.textContent = 'Edit Operating System';
    }

    const nameInput = document.getElementById('operating-systems-name') as HTMLInputElement | null;
    const displayNameInput = document.getElementById('operating-systems-display-name') as HTMLInputElement | null;
    const versionInput = document.getElementById('operating-systems-version') as HTMLInputElement | null;
    const descriptionInput = document.getElementById('operating-systems-description') as HTMLTextAreaElement | null;
    const isActiveInput = document.getElementById('operating-systems-is-active') as HTMLInputElement | null;

    if (nameInput) {
        nameInput.value = os.name;
    }
    if (displayNameInput) {
        displayNameInput.value = os.displayName;
    }
    if (versionInput) {
        versionInput.value = os.version || '';
    }
    if (descriptionInput) {
        descriptionInput.value = os.description || '';
    }
    if (isActiveInput) {
        isActiveInput.checked = os.isActive;
    }

    showModal('operating-systems-edit-modal');
}

async function saveOperatingSystem(): Promise<void> {
    const nameInput = document.getElementById('operating-systems-name') as HTMLInputElement | null;
    const displayNameInput = document.getElementById('operating-systems-display-name') as HTMLInputElement | null;
    const versionInput = document.getElementById('operating-systems-version') as HTMLInputElement | null;
    const descriptionInput = document.getElementById('operating-systems-description') as HTMLTextAreaElement | null;
    const isActiveInput = document.getElementById('operating-systems-is-active') as HTMLInputElement | null;

    const name = nameInput?.value.trim() ?? '';
    const displayName = displayNameInput?.value.trim() ?? '';

    if (!name || !displayName) {
        showError('Internal Name and Display Name are required');
        return;
    }

    const payload = {
        name,
        displayName,
        version: versionInput?.value.trim() || null,
        description: descriptionInput?.value.trim() || null,
        isActive: isActiveInput?.checked ?? false,
    };

    const response = editingId
        ? await apiRequest(`${getApiBaseUrl()}/OperatingSystems/${editingId}`, { method: 'PUT', body: JSON.stringify(payload) })
        : await apiRequest(`${getApiBaseUrl()}/OperatingSystems`, { method: 'POST', body: JSON.stringify(payload) });

    if (response.success) {
        hideModal('operating-systems-edit-modal');
        showSuccess(editingId ? 'Operating system updated successfully' : 'Operating system created successfully');
        loadOperatingSystems();
    } else {
        showError(response.message || 'Save failed');
    }
}

function openDelete(id: number, name: string): void {
    pendingDeleteId = id;

    const deleteName = document.getElementById('operating-systems-delete-name');
    if (deleteName) {
        deleteName.textContent = name;
    }

    showModal('operating-systems-delete-modal');
}

async function doDelete(): Promise<void> {
    if (!pendingDeleteId) {
        return;
    }

    const response = await apiRequest(`${getApiBaseUrl()}/OperatingSystems/${pendingDeleteId}`, { method: 'DELETE' });
    hideModal('operating-systems-delete-modal');

    if (response.success) {
        showSuccess('Operating system deleted successfully');
        loadOperatingSystems();
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
    const alert = document.getElementById('operating-systems-alert-success');
    if (!alert) {
        return;
    }

    alert.textContent = message;
    alert.classList.remove('d-none');

    const errorAlert = document.getElementById('operating-systems-alert-error');
    errorAlert?.classList.add('d-none');

    setTimeout(() => alert.classList.add('d-none'), 5000);
}

function showError(message: string): void {
    const alert = document.getElementById('operating-systems-alert-error');
    if (!alert) {
        return;
    }

    alert.textContent = message;
    alert.classList.remove('d-none');

    const successAlert = document.getElementById('operating-systems-alert-success');
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
    const tableBody = document.getElementById('operating-systems-table-body');
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

function initializeOperatingSystemsPage(): void {
    const page = document.getElementById('operating-systems-page');
    if (!page || page.dataset.initialized === 'true') {
        return;
    }

    page.dataset.initialized = 'true';

    document.getElementById('operating-systems-create')?.addEventListener('click', openCreate);
    document.getElementById('operating-systems-save')?.addEventListener('click', saveOperatingSystem);
    document.getElementById('operating-systems-confirm-delete')?.addEventListener('click', doDelete);

    bindTableActions();
    loadOperatingSystems();
}

function setupPageObserver(): void {
    // Try immediate initialization
    initializeOperatingSystemsPage();

    // Set up MutationObserver for Blazor navigation
    if (document.body) {
        const observer = new MutationObserver(() => {
            const page = document.getElementById('operating-systems-page');
            if (page && page.dataset.initialized !== 'true') {
                initializeOperatingSystemsPage();
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

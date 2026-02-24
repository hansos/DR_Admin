// @ts-nocheck
(function() {
interface ServerIpAddress {
    id: number;
    serverId: number;
    serverName?: string | null;
    ipAddress: string;
    ipVersion: string;
    isPrimary: boolean;
    status: string;
    assignedTo?: string | null;
    notes?: string | null;
}

interface Server {
    id: number;
    name: string;
}

interface ApiResponse<T> {
    success: boolean;
    data?: T;
    message?: string;
}

function getApiBaseUrl(): string {
    return (window as any).AppSettings?.apiBaseUrl ?? '';
}

let allServerIpAddresses: ServerIpAddress[] = [];
let servers: Server[] = [];
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
        console.error('Server IP addresses request failed', error);
        return {
            success: false,
            message: 'Network error. Please try again.',
        };
    }
}

async function loadServers(): Promise<void> {
    const response = await apiRequest<Server[]>(`${getApiBaseUrl()}/Servers`, { method: 'GET' });

    const rawItems = Array.isArray(response.data)
        ? response.data
        : Array.isArray((response.data as any)?.data)
            ? (response.data as any).data
            : [];

    servers = rawItems.map((item: any) => ({
        id: item.id ?? item.Id ?? 0,
        name: item.name ?? item.Name ?? '',
    }));

    populateServerDropdown();
}

function populateServerDropdown(): void {
    const serverSelect = document.getElementById('server-ip-addresses-server-id') as HTMLSelectElement | null;
    if (serverSelect) {
        const selected = serverSelect.value;
        serverSelect.innerHTML = '<option value="">Select Server...</option>' + 
            servers.map(s => `<option value="${s.id}">${esc(s.name)}</option>`).join('');
        if (selected) {
            serverSelect.value = selected;
        }
    }
}

async function loadServerIpAddresses(): Promise<void> {
    const tableBody = document.getElementById('server-ip-addresses-table-body');
    if (!tableBody) {
        return;
    }

    tableBody.innerHTML = '<tr><td colspan="8" class="text-center"><div class="spinner-border text-primary"></div></td></tr>';

    const response = await apiRequest<ServerIpAddress[]>(`${getApiBaseUrl()}/ServerIpAddresses`, { method: 'GET' });
    if (!response.success) {
        showError(response.message || 'Failed to load IP addresses');
        tableBody.innerHTML = '<tr><td colspan="8" class="text-center text-danger">Failed to load data</td></tr>';
        return;
    }

    const rawItems = Array.isArray(response.data)
        ? response.data
        : Array.isArray((response.data as any)?.data)
            ? (response.data as any).data
            : [];

    allServerIpAddresses = rawItems.map((item: any) => ({
        id: item.id ?? item.Id ?? 0,
        serverId: item.serverId ?? item.ServerId ?? 0,
        serverName: item.serverName ?? item.ServerName ?? null,
        ipAddress: item.ipAddress ?? item.IpAddress ?? '',
        ipVersion: item.ipVersion ?? item.IpVersion ?? 'IPv4',
        isPrimary: item.isPrimary ?? item.IsPrimary ?? false,
        status: item.status ?? item.Status ?? 'Active',
        assignedTo: item.assignedTo ?? item.AssignedTo ?? null,
        notes: item.notes ?? item.Notes ?? null,
    }));

    renderTable();
}

function renderTable(): void {
    const tableBody = document.getElementById('server-ip-addresses-table-body');
    if (!tableBody) {
        return;
    }

    if (!allServerIpAddresses.length) {
        tableBody.innerHTML = '<tr><td colspan="8" class="text-center text-muted">No IP addresses found. Click "New IP Address" to add one.</td></tr>';
        return;
    }

    tableBody.innerHTML = allServerIpAddresses.map((ip) => `
        <tr>
            <td>${ip.id}</td>
            <td><code>${esc(ip.ipAddress)}</code></td>
            <td><span class="badge bg-${ip.ipVersion === 'IPv6' ? 'info' : 'secondary'}">${esc(ip.ipVersion)}</span></td>
            <td>${esc(ip.serverName || '-')}</td>
            <td>${ip.isPrimary ? '<span class="badge bg-primary">Primary</span>' : '-'}</td>
            <td><span class="badge bg-${getStatusBadgeColor(ip.status)}">${esc(ip.status)}</span></td>
            <td>${esc(ip.assignedTo || '-')}</td>
            <td>
                <div class="btn-group btn-group-sm">
                    <button class="btn btn-outline-primary" type="button" data-action="edit" data-id="${ip.id}" title="Edit"><i class="bi bi-pencil"></i></button>
                    <button class="btn btn-outline-danger" type="button" data-action="delete" data-id="${ip.id}" data-name="${esc(ip.ipAddress)}" title="Delete"><i class="bi bi-trash"></i></button>
                </div>
            </td>
        </tr>
    `).join('');
}

function getStatusBadgeColor(status: string): string {
    switch (status?.toLowerCase()) {
        case 'active': return 'success';
        case 'reserved': return 'warning';
        case 'blocked': return 'danger';
        default: return 'secondary';
    }
}

function openCreate(): void {
    editingId = null;

    const modalTitle = document.getElementById('server-ip-addresses-modal-title');
    if (modalTitle) {
        modalTitle.textContent = 'New IP Address';
    }

    const form = document.getElementById('server-ip-addresses-form') as HTMLFormElement | null;
    form?.reset();

    const ipVersionSelect = document.getElementById('server-ip-addresses-ip-version') as HTMLSelectElement | null;
    if (ipVersionSelect) {
        ipVersionSelect.value = 'IPv4';
    }

    const statusSelect = document.getElementById('server-ip-addresses-status') as HTMLSelectElement | null;
    if (statusSelect) {
        statusSelect.value = 'Active';
    }

    const isPrimaryInput = document.getElementById('server-ip-addresses-is-primary') as HTMLInputElement | null;
    if (isPrimaryInput) {
        isPrimaryInput.checked = false;
    }

    populateServerDropdown();
    showModal('server-ip-addresses-edit-modal');
}

function openEdit(id: number): void {
    const ip = allServerIpAddresses.find((item) => item.id === id);
    if (!ip) {
        return;
    }

    editingId = id;

    const modalTitle = document.getElementById('server-ip-addresses-modal-title');
    if (modalTitle) {
        modalTitle.textContent = 'Edit IP Address';
    }

    const ipAddressInput = document.getElementById('server-ip-addresses-ip-address') as HTMLInputElement | null;
    const ipVersionInput = document.getElementById('server-ip-addresses-ip-version') as HTMLSelectElement | null;
    const serverIdInput = document.getElementById('server-ip-addresses-server-id') as HTMLSelectElement | null;
    const statusInput = document.getElementById('server-ip-addresses-status') as HTMLSelectElement | null;
    const isPrimaryInput = document.getElementById('server-ip-addresses-is-primary') as HTMLInputElement | null;
    const assignedToInput = document.getElementById('server-ip-addresses-assigned-to') as HTMLInputElement | null;
    const notesInput = document.getElementById('server-ip-addresses-notes') as HTMLTextAreaElement | null;

    if (ipAddressInput) {
        ipAddressInput.value = ip.ipAddress;
    }
    if (ipVersionInput) {
        ipVersionInput.value = ip.ipVersion;
    }

    populateServerDropdown();

    if (serverIdInput) {
        serverIdInput.value = String(ip.serverId);
    }
    if (statusInput) {
        statusInput.value = ip.status;
    }
    if (isPrimaryInput) {
        isPrimaryInput.checked = ip.isPrimary;
    }
    if (assignedToInput) {
        assignedToInput.value = ip.assignedTo || '';
    }
    if (notesInput) {
        notesInput.value = ip.notes || '';
    }

    showModal('server-ip-addresses-edit-modal');
}

async function saveServerIpAddress(): Promise<void> {
    const ipAddressInput = document.getElementById('server-ip-addresses-ip-address') as HTMLInputElement | null;
    const ipVersionInput = document.getElementById('server-ip-addresses-ip-version') as HTMLSelectElement | null;
    const serverIdInput = document.getElementById('server-ip-addresses-server-id') as HTMLSelectElement | null;
    const statusInput = document.getElementById('server-ip-addresses-status') as HTMLSelectElement | null;
    const isPrimaryInput = document.getElementById('server-ip-addresses-is-primary') as HTMLInputElement | null;
    const assignedToInput = document.getElementById('server-ip-addresses-assigned-to') as HTMLInputElement | null;
    const notesInput = document.getElementById('server-ip-addresses-notes') as HTMLTextAreaElement | null;

    const ipAddress = ipAddressInput?.value.trim() ?? '';
    const ipVersion = ipVersionInput?.value?.trim() ?? '';
    const serverId = serverIdInput?.value ? Number(serverIdInput.value) : null;
    const status = statusInput?.value?.trim() ?? 'Active';

    if (!ipAddress || !ipVersion || !serverId || !status) {
        showError('IP Address, IP Version, Server, and Status are required');
        return;
    }

    const payload = {
        serverId,
        ipAddress,
        ipVersion,
        status,
        isPrimary: isPrimaryInput?.checked ?? false,
        assignedTo: assignedToInput?.value.trim() || null,
        notes: notesInput?.value.trim() || null,
    };

    const response = editingId
        ? await apiRequest(`${getApiBaseUrl()}/ServerIpAddresses/${editingId}`, { method: 'PUT', body: JSON.stringify(payload) })
        : await apiRequest(`${getApiBaseUrl()}/ServerIpAddresses`, { method: 'POST', body: JSON.stringify(payload) });

    if (response.success) {
        hideModal('server-ip-addresses-edit-modal');
        showSuccess(editingId ? 'IP address updated successfully' : 'IP address created successfully');
        loadServerIpAddresses();
    } else {
        showError(response.message || 'Save failed');
    }
}

function openDelete(id: number, name: string): void {
    pendingDeleteId = id;

    const deleteName = document.getElementById('server-ip-addresses-delete-name');
    if (deleteName) {
        deleteName.textContent = name;
    }

    showModal('server-ip-addresses-delete-modal');
}

async function doDelete(): Promise<void> {
    if (!pendingDeleteId) {
        return;
    }

    const response = await apiRequest(`${getApiBaseUrl()}/ServerIpAddresses/${pendingDeleteId}`, { method: 'DELETE' });
    hideModal('server-ip-addresses-delete-modal');

    if (response.success) {
        showSuccess('IP address deleted successfully');
        loadServerIpAddresses();
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
    const alert = document.getElementById('server-ip-addresses-alert-success');
    if (!alert) {
        return;
    }

    alert.textContent = message;
    alert.classList.remove('d-none');

    const errorAlert = document.getElementById('server-ip-addresses-alert-error');
    errorAlert?.classList.add('d-none');

    setTimeout(() => alert.classList.add('d-none'), 5000);
}

function showError(message: string): void {
    const alert = document.getElementById('server-ip-addresses-alert-error');
    if (!alert) {
        return;
    }

    alert.textContent = message;
    alert.classList.remove('d-none');

    const successAlert = document.getElementById('server-ip-addresses-alert-success');
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
    const tableBody = document.getElementById('server-ip-addresses-table-body');
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

function initializeServerIpAddressesPage(): void {
    const page = document.getElementById('server-ip-addresses-page');
    if (!page || page.dataset.initialized === 'true') {
        return;
    }

    page.dataset.initialized = 'true';

    document.getElementById('server-ip-addresses-create')?.addEventListener('click', openCreate);
    document.getElementById('server-ip-addresses-save')?.addEventListener('click', saveServerIpAddress);
    document.getElementById('server-ip-addresses-confirm-delete')?.addEventListener('click', doDelete);

    bindTableActions();
    loadServers();
    loadServerIpAddresses();
}

function setupPageObserver(): void {
    // Try immediate initialization
    initializeServerIpAddressesPage();

    // Set up MutationObserver for Blazor navigation
    if (document.body) {
        const observer = new MutationObserver(() => {
            const page = document.getElementById('server-ip-addresses-page');
            if (page && page.dataset.initialized !== 'true') {
                initializeServerIpAddressesPage();
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

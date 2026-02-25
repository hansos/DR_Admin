"use strict";
// @ts-nocheck
(function () {
    function getApiBaseUrl() {
        return window.AppSettings?.apiBaseUrl ?? '';
    }
    let allHostProviders = [];
    let editingId = null;
    let pendingDeleteId = null;
    function getAuthToken() {
        const auth = window.Auth;
        if (auth?.getToken) {
            return auth.getToken();
        }
        return sessionStorage.getItem('rp_authToken');
    }
    async function apiRequest(endpoint, options = {}) {
        try {
            const headers = {
                'Content-Type': 'application/json',
                ...options.headers,
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
        }
        catch (error) {
            console.error('Host providers request failed', error);
            return {
                success: false,
                message: 'Network error. Please try again.',
            };
        }
    }
    async function loadHostProviders() {
        const tableBody = document.getElementById('host-providers-table-body');
        if (!tableBody) {
            return;
        }
        tableBody.innerHTML = '<tr><td colspan="6" class="text-center"><div class="spinner-border text-primary"></div></td></tr>';
        const response = await apiRequest(`${getApiBaseUrl()}/HostProviders`, { method: 'GET' });
        if (!response.success) {
            showError(response.message || 'Failed to load hosting providers');
            tableBody.innerHTML = '<tr><td colspan="6" class="text-center text-danger">Failed to load data</td></tr>';
            return;
        }
        const rawItems = Array.isArray(response.data)
            ? response.data
            : Array.isArray(response.data?.data)
                ? response.data.data
                : [];
        allHostProviders = rawItems.map((item) => ({
            id: item.id ?? item.Id ?? 0,
            name: item.name ?? item.Name ?? '',
            displayName: item.displayName ?? item.DisplayName ?? '',
            description: item.description ?? item.Description ?? null,
            isActive: item.isActive ?? item.IsActive ?? false,
        }));
        renderTable();
    }
    function renderTable() {
        const tableBody = document.getElementById('host-providers-table-body');
        if (!tableBody) {
            return;
        }
        if (!allHostProviders.length) {
            tableBody.innerHTML = '<tr><td colspan="6" class="text-center text-muted">No hosting providers found. Click "New Hosting Provider" to add one.</td></tr>';
            return;
        }
        tableBody.innerHTML = allHostProviders.map((provider) => `
        <tr>
            <td>${provider.id}</td>
            <td><code>${esc(provider.name)}</code></td>
            <td>${esc(provider.displayName)}</td>
            <td>${esc(provider.description || '-')}</td>
            <td><span class="badge bg-${provider.isActive ? 'success' : 'secondary'}">${provider.isActive ? 'Active' : 'Inactive'}</span></td>
            <td>
                <div class="btn-group btn-group-sm">
                    <button class="btn btn-outline-primary" type="button" data-action="edit" data-id="${provider.id}" title="Edit"><i class="bi bi-pencil"></i></button>
                    <button class="btn btn-outline-danger" type="button" data-action="delete" data-id="${provider.id}" data-name="${esc(provider.displayName)}" title="Delete"><i class="bi bi-trash"></i></button>
                </div>
            </td>
        </tr>
    `).join('');
    }
    function openCreate() {
        editingId = null;
        const modalTitle = document.getElementById('host-providers-modal-title');
        if (modalTitle) {
            modalTitle.textContent = 'New Hosting Provider';
        }
        const form = document.getElementById('host-providers-form');
        form?.reset();
        const isActiveInput = document.getElementById('host-providers-is-active');
        if (isActiveInput) {
            isActiveInput.checked = true;
        }
        showModal('host-providers-edit-modal');
    }
    function openEdit(id) {
        const provider = allHostProviders.find((item) => item.id === id);
        if (!provider) {
            return;
        }
        editingId = id;
        const modalTitle = document.getElementById('host-providers-modal-title');
        if (modalTitle) {
            modalTitle.textContent = 'Edit Hosting Provider';
        }
        const nameInput = document.getElementById('host-providers-name');
        const displayNameInput = document.getElementById('host-providers-display-name');
        const descriptionInput = document.getElementById('host-providers-description');
        const isActiveInput = document.getElementById('host-providers-is-active');
        if (nameInput) {
            nameInput.value = provider.name;
        }
        if (displayNameInput) {
            displayNameInput.value = provider.displayName;
        }
        if (descriptionInput) {
            descriptionInput.value = provider.description || '';
        }
        if (isActiveInput) {
            isActiveInput.checked = provider.isActive;
        }
        showModal('host-providers-edit-modal');
    }
    async function saveHostProvider() {
        const nameInput = document.getElementById('host-providers-name');
        const displayNameInput = document.getElementById('host-providers-display-name');
        const descriptionInput = document.getElementById('host-providers-description');
        const isActiveInput = document.getElementById('host-providers-is-active');
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
            ? await apiRequest(`${getApiBaseUrl()}/HostProviders/${editingId}`, { method: 'PUT', body: JSON.stringify(payload) })
            : await apiRequest(`${getApiBaseUrl()}/HostProviders`, { method: 'POST', body: JSON.stringify(payload) });
        if (response.success) {
            hideModal('host-providers-edit-modal');
            showSuccess(editingId ? 'Hosting provider updated successfully' : 'Hosting provider created successfully');
            loadHostProviders();
        }
        else {
            showError(response.message || 'Save failed');
        }
    }
    function openDelete(id, name) {
        pendingDeleteId = id;
        const deleteName = document.getElementById('host-providers-delete-name');
        if (deleteName) {
            deleteName.textContent = name;
        }
        showModal('host-providers-delete-modal');
    }
    async function doDelete() {
        if (!pendingDeleteId) {
            return;
        }
        const response = await apiRequest(`${getApiBaseUrl()}/HostProviders/${pendingDeleteId}`, { method: 'DELETE' });
        hideModal('host-providers-delete-modal');
        if (response.success) {
            showSuccess('Hosting provider deleted successfully');
            loadHostProviders();
        }
        else {
            showError(response.message || 'Delete failed');
        }
        pendingDeleteId = null;
    }
    function esc(text) {
        const map = { '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#039;' };
        return (text || '').toString().replace(/[&<>"']/g, (char) => map[char]);
    }
    function showSuccess(message) {
        const alert = document.getElementById('host-providers-alert-success');
        if (!alert) {
            return;
        }
        alert.textContent = message;
        alert.classList.remove('d-none');
        const errorAlert = document.getElementById('host-providers-alert-error');
        errorAlert?.classList.add('d-none');
        setTimeout(() => alert.classList.add('d-none'), 5000);
    }
    function showError(message) {
        const alert = document.getElementById('host-providers-alert-error');
        if (!alert) {
            return;
        }
        alert.textContent = message;
        alert.classList.remove('d-none');
        const successAlert = document.getElementById('host-providers-alert-success');
        successAlert?.classList.add('d-none');
    }
    function showModal(id) {
        const element = document.getElementById(id);
        if (!element || !window.bootstrap) {
            return;
        }
        const modal = new window.bootstrap.Modal(element);
        modal.show();
    }
    function hideModal(id) {
        const element = document.getElementById(id);
        if (!element || !window.bootstrap) {
            return;
        }
        const modal = window.bootstrap.Modal.getInstance(element);
        modal?.hide();
    }
    function bindTableActions() {
        const tableBody = document.getElementById('host-providers-table-body');
        if (!tableBody) {
            return;
        }
        tableBody.addEventListener('click', (event) => {
            const target = event.target;
            const button = target.closest('button[data-action]');
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
    function initializeHostProvidersPage() {
        const page = document.getElementById('host-providers-page');
        if (!page || page.dataset.initialized === 'true') {
            return;
        }
        page.dataset.initialized = 'true';
        document.getElementById('host-providers-create')?.addEventListener('click', openCreate);
        document.getElementById('host-providers-save')?.addEventListener('click', saveHostProvider);
        document.getElementById('host-providers-confirm-delete')?.addEventListener('click', doDelete);
        bindTableActions();
        loadHostProviders();
    }
    function setupPageObserver() {
        // Try immediate initialization
        initializeHostProvidersPage();
        // Set up MutationObserver for Blazor navigation
        if (document.body) {
            const observer = new MutationObserver(() => {
                const page = document.getElementById('host-providers-page');
                if (page && page.dataset.initialized !== 'true') {
                    initializeHostProvidersPage();
                }
            });
            observer.observe(document.body, { childList: true, subtree: true });
        }
    }
    // Initialize when DOM is ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', setupPageObserver);
    }
    else {
        setupPageObserver();
    }
})();
//# sourceMappingURL=host-providers.js.map
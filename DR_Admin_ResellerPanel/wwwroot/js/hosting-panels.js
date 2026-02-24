// @ts-nocheck
(function () {
    function getApiBaseUrl() {
        return window.AppSettings?.apiBaseUrl ?? '';
    }
    let allHostingPanels = [];
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
            console.error('Hosting panels request failed', error);
            return {
                success: false,
                message: 'Network error. Please try again.',
            };
        }
    }
    async function loadHostingPanels() {
        const tableBody = document.getElementById('hosting-panels-table-body');
        if (!tableBody) {
            return;
        }
        tableBody.innerHTML = '<tr><td colspan="6" class="text-center"><div class="spinner-border text-primary"></div></td></tr>';
        const response = await apiRequest(`${getApiBaseUrl()}/HostingPanels`, { method: 'GET' });
        if (!response.success) {
            showError(response.message || 'Failed to load hosting panels');
            tableBody.innerHTML = '<tr><td colspan="6" class="text-center text-danger">Failed to load data</td></tr>';
            return;
        }
        const rawItems = Array.isArray(response.data)
            ? response.data
            : Array.isArray(response.data?.data)
                ? response.data.data
                : [];
        allHostingPanels = rawItems.map((item) => ({
            id: item.id ?? item.Id ?? 0,
            name: item.name ?? item.Name ?? '',
            displayName: item.displayName ?? item.DisplayName ?? '',
            description: item.description ?? item.Description ?? null,
            isActive: item.isActive ?? item.IsActive ?? false,
        }));
        renderTable();
    }
    function renderTable() {
        const tableBody = document.getElementById('hosting-panels-table-body');
        if (!tableBody) {
            return;
        }
        if (!allHostingPanels.length) {
            tableBody.innerHTML = '<tr><td colspan="6" class="text-center text-muted">No hosting panels found. Click "New Hosting Panel" to add one.</td></tr>';
            return;
        }
        tableBody.innerHTML = allHostingPanels.map((panel) => `
        <tr>
            <td>${panel.id}</td>
            <td><code>${esc(panel.name)}</code></td>
            <td>${esc(panel.displayName)}</td>
            <td>${esc(panel.description || '-')}</td>
            <td><span class="badge bg-${panel.isActive ? 'success' : 'secondary'}">${panel.isActive ? 'Active' : 'Inactive'}</span></td>
            <td>
                <div class="btn-group btn-group-sm">
                    <button class="btn btn-outline-primary" type="button" data-action="edit" data-id="${panel.id}" title="Edit"><i class="bi bi-pencil"></i></button>
                    <button class="btn btn-outline-danger" type="button" data-action="delete" data-id="${panel.id}" data-name="${esc(panel.displayName)}" title="Delete"><i class="bi bi-trash"></i></button>
                </div>
            </td>
        </tr>
    `).join('');
    }
    function openCreate() {
        editingId = null;
        const modalTitle = document.getElementById('hosting-panels-modal-title');
        if (modalTitle) {
            modalTitle.textContent = 'New Hosting Panel';
        }
        const form = document.getElementById('hosting-panels-form');
        form?.reset();
        const isActiveInput = document.getElementById('hosting-panels-is-active');
        if (isActiveInput) {
            isActiveInput.checked = true;
        }
        showModal('hosting-panels-edit-modal');
    }
    function openEdit(id) {
        const panel = allHostingPanels.find((item) => item.id === id);
        if (!panel) {
            return;
        }
        editingId = id;
        const modalTitle = document.getElementById('hosting-panels-modal-title');
        if (modalTitle) {
            modalTitle.textContent = 'Edit Hosting Panel';
        }
        const nameInput = document.getElementById('hosting-panels-name');
        const displayNameInput = document.getElementById('hosting-panels-display-name');
        const descriptionInput = document.getElementById('hosting-panels-description');
        const isActiveInput = document.getElementById('hosting-panels-is-active');
        if (nameInput) {
            nameInput.value = panel.name;
        }
        if (displayNameInput) {
            displayNameInput.value = panel.displayName;
        }
        if (descriptionInput) {
            descriptionInput.value = panel.description || '';
        }
        if (isActiveInput) {
            isActiveInput.checked = panel.isActive;
        }
        showModal('hosting-panels-edit-modal');
    }
    async function saveHostingPanel() {
        const nameInput = document.getElementById('hosting-panels-name');
        const displayNameInput = document.getElementById('hosting-panels-display-name');
        const descriptionInput = document.getElementById('hosting-panels-description');
        const isActiveInput = document.getElementById('hosting-panels-is-active');
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
            ? await apiRequest(`${getApiBaseUrl()}/HostingPanels/${editingId}`, { method: 'PUT', body: JSON.stringify(payload) })
            : await apiRequest(`${getApiBaseUrl()}/HostingPanels`, { method: 'POST', body: JSON.stringify(payload) });
        if (response.success) {
            hideModal('hosting-panels-edit-modal');
            showSuccess(editingId ? 'Hosting panel updated successfully' : 'Hosting panel created successfully');
            loadHostingPanels();
        }
        else {
            showError(response.message || 'Save failed');
        }
    }
    function openDelete(id, name) {
        pendingDeleteId = id;
        const deleteName = document.getElementById('hosting-panels-delete-name');
        if (deleteName) {
            deleteName.textContent = name;
        }
        showModal('hosting-panels-delete-modal');
    }
    async function doDelete() {
        if (!pendingDeleteId) {
            return;
        }
        const response = await apiRequest(`${getApiBaseUrl()}/HostingPanels/${pendingDeleteId}`, { method: 'DELETE' });
        hideModal('hosting-panels-delete-modal');
        if (response.success) {
            showSuccess('Hosting panel deleted successfully');
            loadHostingPanels();
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
        const alert = document.getElementById('hosting-panels-alert-success');
        if (!alert) {
            return;
        }
        alert.textContent = message;
        alert.classList.remove('d-none');
        const errorAlert = document.getElementById('hosting-panels-alert-error');
        errorAlert?.classList.add('d-none');
        setTimeout(() => alert.classList.add('d-none'), 5000);
    }
    function showError(message) {
        const alert = document.getElementById('hosting-panels-alert-error');
        if (!alert) {
            return;
        }
        alert.textContent = message;
        alert.classList.remove('d-none');
        const successAlert = document.getElementById('hosting-panels-alert-success');
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
        const tableBody = document.getElementById('hosting-panels-table-body');
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
    function initializeHostingPanelsPage() {
        const page = document.getElementById('hosting-panels-page');
        if (!page || page.dataset.initialized === 'true') {
            return;
        }
        page.dataset.initialized = 'true';
        document.getElementById('hosting-panels-create')?.addEventListener('click', openCreate);
        document.getElementById('hosting-panels-save')?.addEventListener('click', saveHostingPanel);
        document.getElementById('hosting-panels-confirm-delete')?.addEventListener('click', doDelete);
        bindTableActions();
        loadHostingPanels();
    }
    function setupPageObserver() {
        // Try immediate initialization
        initializeHostingPanelsPage();
        // Set up MutationObserver for Blazor navigation
        if (document.body) {
            const observer = new MutationObserver(() => {
                const page = document.getElementById('hosting-panels-page');
                if (page && page.dataset.initialized !== 'true') {
                    initializeHostingPanelsPage();
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

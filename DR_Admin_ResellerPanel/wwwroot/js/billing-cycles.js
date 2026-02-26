"use strict";
(() => {
    let allBillingCycles = [];
    let editingId = null;
    let pendingDeleteId = null;
    const getApiBaseUrl = () => {
        const settings = window.AppSettings;
        return settings?.apiBaseUrl ?? '';
    };
    const getAuthToken = () => {
        const auth = window.Auth;
        if (auth?.getToken) {
            return auth.getToken();
        }
        return sessionStorage.getItem('rp_authToken');
    };
    const getBootstrap = () => {
        const maybeBootstrap = window.bootstrap;
        return maybeBootstrap ?? null;
    };
    const esc = (text) => {
        const map = { '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#039;' };
        return (text || '').replace(/[&<>"']/g, (char) => map[char] ?? char);
    };
    const apiRequest = async (endpoint, options = {}) => {
        try {
            const headers = {
                'Content-Type': 'application/json',
                ...options.headers,
            };
            const authToken = getAuthToken();
            if (authToken) {
                headers.Authorization = `Bearer ${authToken}`;
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
                    message: (data && (data.message ?? data.title)) ||
                        `Request failed with status ${response.status}`,
                };
            }
            const parsed = data;
            return {
                success: parsed?.success !== false,
                data: parsed?.data ?? data,
                message: parsed?.message,
            };
        }
        catch (error) {
            console.error('Billing cycles request failed', error);
            return {
                success: false,
                message: 'Network error. Please try again.',
            };
        }
    };
    const showSuccess = (message) => {
        const alert = document.getElementById('billing-cycles-alert-success');
        if (!alert) {
            return;
        }
        alert.textContent = message;
        alert.classList.remove('d-none');
        document.getElementById('billing-cycles-alert-error')?.classList.add('d-none');
        setTimeout(() => alert.classList.add('d-none'), 5000);
    };
    const showError = (message) => {
        const alert = document.getElementById('billing-cycles-alert-error');
        if (!alert) {
            return;
        }
        alert.textContent = message;
        alert.classList.remove('d-none');
        document.getElementById('billing-cycles-alert-success')?.classList.add('d-none');
    };
    const showModal = (id) => {
        const element = document.getElementById(id);
        const bootstrap = getBootstrap();
        if (!element || !bootstrap) {
            return;
        }
        const modal = new bootstrap.Modal(element);
        modal.show();
    };
    const hideModal = (id) => {
        const element = document.getElementById(id);
        const bootstrap = getBootstrap();
        if (!element || !bootstrap) {
            return;
        }
        const modal = bootstrap.Modal.getInstance(element);
        modal?.hide();
    };
    const normalizeBillingCycle = (item) => {
        const typed = (item ?? {});
        return {
            id: Number(typed.id ?? typed.Id ?? 0),
            code: String(typed.code ?? typed.Code ?? ''),
            name: String(typed.name ?? typed.Name ?? ''),
            durationInDays: Number(typed.durationInDays ?? typed.DurationInDays ?? 0),
            description: String(typed.description ?? typed.Description ?? ''),
            sortOrder: Number(typed.sortOrder ?? typed.SortOrder ?? 0),
        };
    };
    const renderTable = () => {
        const tableBody = document.getElementById('billing-cycles-table-body');
        if (!tableBody) {
            return;
        }
        if (!allBillingCycles.length) {
            tableBody.innerHTML = '<tr><td colspan="7" class="text-center text-muted">No billing cycles found.</td></tr>';
            return;
        }
        const sorted = [...allBillingCycles].sort((a, b) => a.sortOrder - b.sortOrder || a.durationInDays - b.durationInDays || a.name.localeCompare(b.name));
        tableBody.innerHTML = sorted.map((cycle) => `
            <tr>
                <td>${cycle.id}</td>
                <td><code>${esc(cycle.code)}</code></td>
                <td>${esc(cycle.name)}</td>
                <td>${esc(String(cycle.durationInDays))}</td>
                <td>${esc(String(cycle.sortOrder))}</td>
                <td>${esc(cycle.description || '-')}</td>
                <td class="text-end">
                    <div class="btn-group btn-group-sm">
                        <button class="btn btn-outline-primary" type="button" data-action="edit" data-id="${cycle.id}"><i class="bi bi-pencil"></i></button>
                        <button class="btn btn-outline-danger" type="button" data-action="delete" data-id="${cycle.id}" data-name="${esc(cycle.name)}"><i class="bi bi-trash"></i></button>
                    </div>
                </td>
            </tr>
        `).join('');
    };
    const loadBillingCycles = async () => {
        const tableBody = document.getElementById('billing-cycles-table-body');
        if (tableBody) {
            tableBody.innerHTML = '<tr><td colspan="7" class="text-center"><div class="spinner-border text-primary"></div></td></tr>';
        }
        const response = await apiRequest(`${getApiBaseUrl()}/BillingCycles`, { method: 'GET' });
        if (!response.success) {
            showError(response.message || 'Failed to load billing cycles.');
            if (tableBody) {
                tableBody.innerHTML = '<tr><td colspan="7" class="text-center text-danger">Failed to load data</td></tr>';
            }
            return;
        }
        const raw = response.data;
        const list = Array.isArray(raw)
            ? raw
            : Array.isArray(raw?.data)
                ? (raw.data ?? [])
                : [];
        allBillingCycles = list.map((item) => normalizeBillingCycle(item));
        renderTable();
    };
    const getInputValue = (id) => {
        const input = document.getElementById(id);
        return input?.value.trim() ?? '';
    };
    const setInputValue = (id, value) => {
        const input = document.getElementById(id);
        if (input) {
            input.value = value;
        }
    };
    const openCreate = () => {
        editingId = null;
        const title = document.getElementById('billing-cycles-modal-title');
        if (title) {
            title.textContent = 'New Billing Cycle';
        }
        document.getElementById('billing-cycles-form')?.reset();
        setInputValue('billing-cycles-sort-order', String(Math.max(0, allBillingCycles.length * 10)));
        showModal('billing-cycles-edit-modal');
    };
    const openEdit = (id) => {
        const item = allBillingCycles.find((x) => x.id === id);
        if (!item) {
            return;
        }
        editingId = id;
        const title = document.getElementById('billing-cycles-modal-title');
        if (title) {
            title.textContent = 'Edit Billing Cycle';
        }
        setInputValue('billing-cycles-code', item.code);
        setInputValue('billing-cycles-name', item.name);
        setInputValue('billing-cycles-duration', String(item.durationInDays));
        setInputValue('billing-cycles-sort-order', String(item.sortOrder));
        setInputValue('billing-cycles-description', item.description || '');
        showModal('billing-cycles-edit-modal');
    };
    const saveBillingCycle = async () => {
        const code = getInputValue('billing-cycles-code');
        const name = getInputValue('billing-cycles-name');
        const durationInDays = Number(getInputValue('billing-cycles-duration'));
        const sortOrder = Number(getInputValue('billing-cycles-sort-order'));
        if (!code || !name) {
            showError('Code and Name are required.');
            return;
        }
        if (!Number.isFinite(durationInDays) || durationInDays <= 0) {
            showError('Duration in days must be greater than 0.');
            return;
        }
        if (!Number.isFinite(sortOrder) || sortOrder < 0) {
            showError('Sort order must be 0 or greater.');
            return;
        }
        const payload = {
            code,
            name,
            durationInDays,
            description: getInputValue('billing-cycles-description'),
            sortOrder,
        };
        const response = editingId
            ? await apiRequest(`${getApiBaseUrl()}/BillingCycles/${editingId}`, { method: 'PUT', body: JSON.stringify(payload) })
            : await apiRequest(`${getApiBaseUrl()}/BillingCycles`, { method: 'POST', body: JSON.stringify(payload) });
        if (!response.success) {
            showError(response.message || 'Failed to save billing cycle.');
            return;
        }
        hideModal('billing-cycles-edit-modal');
        showSuccess(editingId ? 'Billing cycle updated successfully.' : 'Billing cycle created successfully.');
        await loadBillingCycles();
    };
    const openDelete = (id, name) => {
        pendingDeleteId = id;
        const deleteName = document.getElementById('billing-cycles-delete-name');
        if (deleteName) {
            deleteName.textContent = name;
        }
        showModal('billing-cycles-delete-modal');
    };
    const doDelete = async () => {
        if (!pendingDeleteId) {
            return;
        }
        const response = await apiRequest(`${getApiBaseUrl()}/BillingCycles/${pendingDeleteId}`, { method: 'DELETE' });
        hideModal('billing-cycles-delete-modal');
        if (!response.success) {
            showError(response.message || 'Failed to delete billing cycle.');
            pendingDeleteId = null;
            return;
        }
        showSuccess('Billing cycle deleted successfully.');
        pendingDeleteId = null;
        await loadBillingCycles();
    };
    const bindEvents = () => {
        document.getElementById('billing-cycles-create')?.addEventListener('click', openCreate);
        document.getElementById('billing-cycles-save')?.addEventListener('click', () => { void saveBillingCycle(); });
        document.getElementById('billing-cycles-confirm-delete')?.addEventListener('click', () => { void doDelete(); });
        document.getElementById('billing-cycles-table-body')?.addEventListener('click', (event) => {
            const target = event.target;
            const button = target.closest('button[data-action]');
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
                openDelete(id, button.dataset.name ?? '');
            }
        });
    };
    const initializePage = async () => {
        const page = document.getElementById('billing-cycles-page');
        if (!page || page.dataset.initialized === 'true') {
            return;
        }
        page.dataset.initialized = 'true';
        bindEvents();
        await loadBillingCycles();
    };
    const setupObserver = () => {
        void initializePage();
        if (document.body) {
            const observer = new MutationObserver(() => {
                const page = document.getElementById('billing-cycles-page');
                if (page && page.dataset.initialized !== 'true') {
                    void initializePage();
                }
            });
            observer.observe(document.body, { childList: true, subtree: true });
        }
    };
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', setupObserver);
    }
    else {
        setupObserver();
    }
})();
//# sourceMappingURL=billing-cycles.js.map
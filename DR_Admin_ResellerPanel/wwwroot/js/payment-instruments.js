"use strict";
(() => {
    let allInstruments = [];
    let allGatewayOptions = [];
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
            console.error('Payment instruments request failed', error);
            return {
                success: false,
                message: 'Network error. Please try again.',
            };
        }
    };
    const showSuccess = (message) => {
        const alert = document.getElementById('payment-instruments-alert-success');
        if (!alert) {
            return;
        }
        alert.textContent = message;
        alert.classList.remove('d-none');
        document.getElementById('payment-instruments-alert-error')?.classList.add('d-none');
        setTimeout(() => alert.classList.add('d-none'), 5000);
    };
    const showError = (message) => {
        const alert = document.getElementById('payment-instruments-alert-error');
        if (!alert) {
            return;
        }
        alert.textContent = message;
        alert.classList.remove('d-none');
        document.getElementById('payment-instruments-alert-success')?.classList.add('d-none');
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
    const normalizeItem = (item) => {
        const typed = (item ?? {});
        return {
            id: Number(typed.id ?? typed.Id ?? 0),
            code: String(typed.code ?? typed.Code ?? ''),
            name: String(typed.name ?? typed.Name ?? ''),
            description: String(typed.description ?? typed.Description ?? ''),
            isActive: Boolean(typed.isActive ?? typed.IsActive ?? false),
            displayOrder: Number(typed.displayOrder ?? typed.DisplayOrder ?? 0),
            defaultGatewayId: Number(typed.defaultGatewayId ?? typed.DefaultGatewayId ?? 0) || null,
        };
    };
    const normalizeGateway = (item) => {
        const typed = (item ?? {});
        return {
            id: Number(typed.id ?? typed.Id ?? 0),
            name: String(typed.name ?? typed.Name ?? ''),
            paymentInstrument: String(typed.paymentInstrument ?? typed.PaymentInstrument ?? ''),
            isActive: Boolean(typed.isActive ?? typed.IsActive ?? false),
        };
    };
    const renderTable = () => {
        const tableBody = document.getElementById('payment-instruments-table-body');
        if (!tableBody) {
            return;
        }
        if (!allInstruments.length) {
            tableBody.innerHTML = '<tr><td colspan="8" class="text-center text-muted">No payment instruments found.</td></tr>';
            return;
        }
        const sorted = [...allInstruments].sort((a, b) => a.displayOrder - b.displayOrder || a.name.localeCompare(b.name));
        tableBody.innerHTML = sorted.map((item) => `
            <tr>
                <td>${item.id}</td>
                <td><code>${esc(item.code)}</code></td>
                <td>${esc(item.name)}</td>
                <td>${item.isActive ? '<span class="badge bg-success">Yes</span>' : '<span class="badge bg-secondary">No</span>'}</td>
                <td>${esc(String(item.displayOrder))}</td>
                <td>${esc(resolveDefaultGatewayName(item))}</td>
                <td>${esc(item.description || '-')}</td>
                <td class="text-end">
                    <div class="btn-group btn-group-sm">
                        <button class="btn btn-outline-primary" type="button" data-action="edit" data-id="${item.id}"><i class="bi bi-pencil"></i></button>
                        <button class="btn btn-outline-danger" type="button" data-action="delete" data-id="${item.id}" data-name="${esc(item.name)}"><i class="bi bi-trash"></i></button>
                    </div>
                </td>
            </tr>
        `).join('');
    };
    const loadItems = async () => {
        const tableBody = document.getElementById('payment-instruments-table-body');
        if (tableBody) {
            tableBody.innerHTML = '<tr><td colspan="8" class="text-center"><div class="spinner-border text-primary"></div></td></tr>';
        }
        const response = await apiRequest(`${getApiBaseUrl()}/PaymentInstruments`, { method: 'GET' });
        if (!response.success) {
            showError(response.message || 'Failed to load payment instruments.');
            if (tableBody) {
                tableBody.innerHTML = '<tr><td colspan="8" class="text-center text-danger">Failed to load data</td></tr>';
            }
            return;
        }
        const raw = response.data;
        const list = Array.isArray(raw)
            ? raw
            : Array.isArray(raw?.data)
                ? (raw.data ?? [])
                : [];
        allInstruments = list.map((item) => normalizeItem(item));
        renderTable();
    };
    const loadGatewayOptions = async () => {
        const response = await apiRequest(`${getApiBaseUrl()}/PaymentGateways/active`, { method: 'GET' });
        if (!response.success) {
            allGatewayOptions = [];
            renderGatewayOptions();
            return;
        }
        const raw = response.data;
        const list = Array.isArray(raw)
            ? raw
            : Array.isArray(raw?.data)
                ? (raw.data ?? [])
                : [];
        allGatewayOptions = list
            .map((item) => normalizeGateway(item))
            .filter((g) => g.id > 0 && g.isActive);
        renderGatewayOptions();
    };
    const renderGatewayOptions = () => {
        const select = document.getElementById('payment-instruments-default-gateway-id');
        if (!select) {
            return;
        }
        select.innerHTML = '<option value="">No default gateway</option>' + allGatewayOptions
            .sort((a, b) => a.name.localeCompare(b.name))
            .map((gateway) => `<option value="${gateway.id}">${esc(gateway.name)} (${esc(gateway.paymentInstrument || '-')})</option>`)
            .join('');
    };
    const resolveDefaultGatewayName = (instrument) => {
        if (!instrument.defaultGatewayId) {
            return '-';
        }
        const gateway = allGatewayOptions.find((x) => x.id === instrument.defaultGatewayId);
        return gateway?.name ?? `Gateway #${instrument.defaultGatewayId}`;
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
        const title = document.getElementById('payment-instruments-modal-title');
        if (title) {
            title.textContent = 'New Payment Instrument';
        }
        document.getElementById('payment-instruments-form')?.reset();
        setInputValue('payment-instruments-sort-order', String(Math.max(0, allInstruments.length * 10)));
        const activeInput = document.getElementById('payment-instruments-is-active');
        if (activeInput) {
            activeInput.checked = true;
        }
        setInputValue('payment-instruments-default-gateway-id', '');
        showModal('payment-instruments-edit-modal');
    };
    const openEdit = (id) => {
        const item = allInstruments.find((x) => x.id === id);
        if (!item) {
            return;
        }
        editingId = id;
        const title = document.getElementById('payment-instruments-modal-title');
        if (title) {
            title.textContent = 'Edit Payment Instrument';
        }
        setInputValue('payment-instruments-code', item.code);
        setInputValue('payment-instruments-name', item.name);
        setInputValue('payment-instruments-sort-order', String(item.displayOrder));
        setInputValue('payment-instruments-description', item.description || '');
        setInputValue('payment-instruments-default-gateway-id', item.defaultGatewayId ? String(item.defaultGatewayId) : '');
        const activeInput = document.getElementById('payment-instruments-is-active');
        if (activeInput) {
            activeInput.checked = item.isActive;
        }
        showModal('payment-instruments-edit-modal');
    };
    const saveItem = async () => {
        const code = getInputValue('payment-instruments-code');
        const name = getInputValue('payment-instruments-name');
        const displayOrder = Number(getInputValue('payment-instruments-sort-order'));
        const isActive = document.getElementById('payment-instruments-is-active')?.checked ?? false;
        const defaultGatewayValue = getInputValue('payment-instruments-default-gateway-id');
        const defaultGatewayId = defaultGatewayValue ? Number(defaultGatewayValue) : null;
        if (!code || !name) {
            showError('Code and Name are required.');
            return;
        }
        if (!Number.isFinite(displayOrder) || displayOrder < 0) {
            showError('Sort order must be 0 or greater.');
            return;
        }
        const payload = {
            code,
            name,
            description: getInputValue('payment-instruments-description'),
            isActive,
            displayOrder,
            defaultGatewayId,
        };
        const response = editingId
            ? await apiRequest(`${getApiBaseUrl()}/PaymentInstruments/${editingId}`, { method: 'PUT', body: JSON.stringify(payload) })
            : await apiRequest(`${getApiBaseUrl()}/PaymentInstruments`, { method: 'POST', body: JSON.stringify(payload) });
        if (!response.success) {
            showError(response.message || 'Failed to save payment instrument.');
            return;
        }
        hideModal('payment-instruments-edit-modal');
        showSuccess(editingId ? 'Payment instrument updated successfully.' : 'Payment instrument created successfully.');
        await loadItems();
    };
    const openDelete = (id, name) => {
        pendingDeleteId = id;
        const deleteName = document.getElementById('payment-instruments-delete-name');
        if (deleteName) {
            deleteName.textContent = name;
        }
        showModal('payment-instruments-delete-modal');
    };
    const doDelete = async () => {
        if (!pendingDeleteId) {
            return;
        }
        const response = await apiRequest(`${getApiBaseUrl()}/PaymentInstruments/${pendingDeleteId}`, { method: 'DELETE' });
        hideModal('payment-instruments-delete-modal');
        if (!response.success) {
            showError(response.message || 'Failed to delete payment instrument.');
            pendingDeleteId = null;
            return;
        }
        showSuccess('Payment instrument deleted successfully.');
        pendingDeleteId = null;
        await loadItems();
    };
    const bindEvents = () => {
        document.getElementById('payment-instruments-create')?.addEventListener('click', openCreate);
        document.getElementById('payment-instruments-save')?.addEventListener('click', () => { void saveItem(); });
        document.getElementById('payment-instruments-confirm-delete')?.addEventListener('click', () => { void doDelete(); });
        document.getElementById('payment-instruments-table-body')?.addEventListener('click', (event) => {
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
        const page = document.getElementById('payment-instruments-page');
        if (!page || page.dataset.initialized === 'true') {
            return;
        }
        page.dataset.initialized = 'true';
        bindEvents();
        await loadGatewayOptions();
        await loadItems();
    };
    const setupObserver = () => {
        void initializePage();
        if (document.body) {
            const observer = new MutationObserver(() => {
                const page = document.getElementById('payment-instruments-page');
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
//# sourceMappingURL=payment-instruments.js.map
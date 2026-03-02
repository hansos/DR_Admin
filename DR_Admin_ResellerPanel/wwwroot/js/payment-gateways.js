"use strict";
(() => {
    let allGateways = [];
    let allInstruments = [];
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
            console.error('Payment gateways request failed', error);
            return {
                success: false,
                message: 'Network error. Please try again.',
            };
        }
    };
    const showSuccess = (message) => {
        const alert = document.getElementById('payment-gateways-alert-success');
        if (!alert) {
            return;
        }
        alert.textContent = message;
        alert.classList.remove('d-none');
        document.getElementById('payment-gateways-alert-error')?.classList.add('d-none');
        setTimeout(() => alert.classList.add('d-none'), 5000);
    };
    const showError = (message) => {
        const alert = document.getElementById('payment-gateways-alert-error');
        if (!alert) {
            return;
        }
        alert.textContent = message;
        alert.classList.remove('d-none');
        document.getElementById('payment-gateways-alert-success')?.classList.add('d-none');
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
    const normalizeGateway = (item) => {
        const typed = (item ?? {});
        return {
            id: Number(typed.id ?? typed.Id ?? 0),
            name: String(typed.name ?? typed.Name ?? ''),
            providerCode: String(typed.providerCode ?? typed.ProviderCode ?? ''),
            paymentInstrument: String(typed.paymentInstrument ?? typed.PaymentInstrument ?? ''),
            paymentInstrumentId: Number(typed.paymentInstrumentId ?? typed.PaymentInstrumentId ?? 0) || null,
            isActive: Boolean(typed.isActive ?? typed.IsActive ?? false),
            isDefault: Boolean(typed.isDefault ?? typed.IsDefault ?? false),
            apiKey: String(typed.apiKey ?? typed.ApiKey ?? ''),
            useSandbox: Boolean(typed.useSandbox ?? typed.UseSandbox ?? false),
            webhookUrl: String(typed.webhookUrl ?? typed.WebhookUrl ?? ''),
            displayOrder: Number(typed.displayOrder ?? typed.DisplayOrder ?? 0),
            description: String(typed.description ?? typed.Description ?? ''),
            logoUrl: String(typed.logoUrl ?? typed.LogoUrl ?? ''),
            supportedCurrencies: String(typed.supportedCurrencies ?? typed.SupportedCurrencies ?? ''),
            feePercentage: Number(typed.feePercentage ?? typed.FeePercentage ?? 0),
            fixedFee: Number(typed.fixedFee ?? typed.FixedFee ?? 0),
            notes: String(typed.notes ?? typed.Notes ?? ''),
        };
    };
    const normalizeInstrument = (item) => {
        const typed = (item ?? {});
        return {
            id: Number(typed.id ?? typed.Id ?? 0),
            code: String(typed.code ?? typed.Code ?? ''),
            name: String(typed.name ?? typed.Name ?? ''),
            isActive: Boolean(typed.isActive ?? typed.IsActive ?? false),
            displayOrder: Number(typed.displayOrder ?? typed.DisplayOrder ?? 0),
        };
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
    const renderInstrumentOptions = () => {
        const select = document.getElementById('payment-gateways-instrument');
        if (!select) {
            return;
        }
        const options = allInstruments
            .filter((instrument) => instrument.isActive)
            .sort((a, b) => (a.displayOrder - b.displayOrder) || a.name.localeCompare(b.name));
        if (!options.length) {
            select.innerHTML = '<option value="">No active instruments</option>';
            return;
        }
        select.innerHTML = options
            .map((instrument) => `<option value="${instrument.id}">${esc(instrument.name)} (${esc(instrument.code)})</option>`)
            .join('');
    };
    const resolveInstrumentName = (gateway) => {
        if (gateway.paymentInstrumentId) {
            const instrument = allInstruments.find((item) => item.id === gateway.paymentInstrumentId);
            if (instrument) {
                return instrument.name;
            }
        }
        return gateway.paymentInstrument || '-';
    };
    const renderTable = () => {
        const tableBody = document.getElementById('payment-gateways-table-body');
        if (!tableBody) {
            return;
        }
        if (!allGateways.length) {
            tableBody.innerHTML = '<tr><td colspan="7" class="text-center text-muted">No payment gateways found.</td></tr>';
            return;
        }
        const sorted = [...allGateways].sort((a, b) => a.displayOrder - b.displayOrder || a.name.localeCompare(b.name));
        tableBody.innerHTML = sorted.map((gateway) => {
            const status = [
                gateway.isActive ? '<span class="badge bg-success me-1">Active</span>' : '<span class="badge bg-secondary me-1">Inactive</span>',
                gateway.isDefault ? '<span class="badge bg-primary">Default</span>' : ''
            ].join('');
            return `
                <tr>
                    <td>${gateway.id}</td>
                    <td>${esc(gateway.name)}</td>
                    <td><code>${esc(gateway.providerCode)}</code></td>
                    <td>${esc(resolveInstrumentName(gateway))}</td>
                    <td>${status}</td>
                    <td>${esc(String(gateway.displayOrder))}</td>
                    <td class="text-end">
                        <div class="btn-group btn-group-sm">
                            <button class="btn btn-outline-primary" type="button" data-action="edit" data-id="${gateway.id}"><i class="bi bi-pencil"></i></button>
                            <button class="btn btn-outline-danger" type="button" data-action="delete" data-id="${gateway.id}" data-name="${esc(gateway.name)}"><i class="bi bi-trash"></i></button>
                        </div>
                    </td>
                </tr>
            `;
        }).join('');
    };
    const loadInstruments = async () => {
        const response = await apiRequest(`${getApiBaseUrl()}/PaymentInstruments/active`, { method: 'GET' });
        if (!response.success) {
            allInstruments = [];
            renderInstrumentOptions();
            return;
        }
        const raw = response.data;
        const list = Array.isArray(raw)
            ? raw
            : Array.isArray(raw?.data)
                ? (raw.data ?? [])
                : [];
        allInstruments = list.map((item) => normalizeInstrument(item));
        renderInstrumentOptions();
    };
    const loadGateways = async () => {
        const tableBody = document.getElementById('payment-gateways-table-body');
        if (tableBody) {
            tableBody.innerHTML = '<tr><td colspan="7" class="text-center"><div class="spinner-border text-primary"></div></td></tr>';
        }
        const response = await apiRequest(`${getApiBaseUrl()}/PaymentGateways`, { method: 'GET' });
        if (!response.success) {
            showError(response.message || 'Failed to load payment gateways.');
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
        allGateways = list.map((item) => normalizeGateway(item));
        renderTable();
    };
    const openCreate = () => {
        editingId = null;
        const title = document.getElementById('payment-gateways-modal-title');
        if (title) {
            title.textContent = 'New Payment Gateway';
        }
        document.getElementById('payment-gateways-form')?.reset();
        setInputValue('payment-gateways-sort-order', String(Math.max(0, allGateways.length * 10)));
        const activeInput = document.getElementById('payment-gateways-is-active');
        if (activeInput) {
            activeInput.checked = true;
        }
        renderInstrumentOptions();
        showModal('payment-gateways-edit-modal');
    };
    const openEdit = (id) => {
        const gateway = allGateways.find((item) => item.id === id);
        if (!gateway) {
            return;
        }
        editingId = id;
        const title = document.getElementById('payment-gateways-modal-title');
        if (title) {
            title.textContent = 'Edit Payment Gateway';
        }
        setInputValue('payment-gateways-name', gateway.name);
        setInputValue('payment-gateways-provider-code', gateway.providerCode);
        if (gateway.paymentInstrumentId) {
            setInputValue('payment-gateways-instrument', String(gateway.paymentInstrumentId));
        }
        else {
            const fallbackInstrument = allInstruments.find((item) => item.code.toLowerCase() === gateway.paymentInstrument.toLowerCase());
            setInputValue('payment-gateways-instrument', fallbackInstrument ? String(fallbackInstrument.id) : '');
        }
        setInputValue('payment-gateways-sort-order', String(gateway.displayOrder));
        setInputValue('payment-gateways-webhook-url', gateway.webhookUrl);
        setInputValue('payment-gateways-description', gateway.description);
        setInputValue('payment-gateways-api-key', '');
        setInputValue('payment-gateways-api-secret', '');
        const activeInput = document.getElementById('payment-gateways-is-active');
        if (activeInput) {
            activeInput.checked = gateway.isActive;
        }
        const defaultInput = document.getElementById('payment-gateways-is-default');
        if (defaultInput) {
            defaultInput.checked = gateway.isDefault;
        }
        showModal('payment-gateways-edit-modal');
    };
    const saveGateway = async () => {
        const name = getInputValue('payment-gateways-name');
        const providerCode = getInputValue('payment-gateways-provider-code');
        const instrumentIdValue = getInputValue('payment-gateways-instrument');
        const instrumentId = Number(instrumentIdValue);
        const displayOrder = Number(getInputValue('payment-gateways-sort-order'));
        const webhookUrl = getInputValue('payment-gateways-webhook-url');
        const description = getInputValue('payment-gateways-description');
        const apiKey = getInputValue('payment-gateways-api-key');
        const apiSecret = getInputValue('payment-gateways-api-secret');
        const isActive = document.getElementById('payment-gateways-is-active')?.checked ?? false;
        const isDefault = document.getElementById('payment-gateways-is-default')?.checked ?? false;
        if (!name || !providerCode) {
            showError('Name and provider code are required.');
            return;
        }
        if (!Number.isFinite(instrumentId) || instrumentId <= 0) {
            showError('Payment instrument is required.');
            return;
        }
        if (!Number.isFinite(displayOrder) || displayOrder < 0) {
            showError('Sort order must be 0 or greater.');
            return;
        }
        const selectedInstrument = allInstruments.find((item) => item.id === instrumentId);
        if (!selectedInstrument) {
            showError('Selected payment instrument is invalid.');
            return;
        }
        const payload = {
            name,
            providerCode,
            paymentInstrumentId: instrumentId,
            paymentInstrument: selectedInstrument.code,
            isActive,
            isDefault,
            useSandbox: true,
            webhookUrl,
            displayOrder,
            description,
            logoUrl: '',
            supportedCurrencies: 'USD,EUR,GBP',
            feePercentage: 0,
            fixedFee: 0,
            notes: '',
            configurationJson: '',
        };
        if (editingId === null) {
            payload.apiKey = apiKey || 'placeholder-key';
            payload.apiSecret = apiSecret || 'placeholder-secret';
            payload.webhookSecret = '';
        }
        else {
            payload.apiKey = apiKey || null;
            payload.apiSecret = apiSecret || null;
            payload.webhookSecret = null;
        }
        const response = editingId === null
            ? await apiRequest(`${getApiBaseUrl()}/PaymentGateways`, { method: 'POST', body: JSON.stringify(payload) })
            : await apiRequest(`${getApiBaseUrl()}/PaymentGateways/${editingId}`, { method: 'PUT', body: JSON.stringify(payload) });
        if (!response.success) {
            showError(response.message || 'Failed to save payment gateway.');
            return;
        }
        hideModal('payment-gateways-edit-modal');
        showSuccess(editingId === null ? 'Payment gateway created successfully.' : 'Payment gateway updated successfully.');
        await loadGateways();
    };
    const openDelete = (id, name) => {
        pendingDeleteId = id;
        const element = document.getElementById('payment-gateways-delete-name');
        if (element) {
            element.textContent = name;
        }
        showModal('payment-gateways-delete-modal');
    };
    const doDelete = async () => {
        if (!pendingDeleteId) {
            return;
        }
        const response = await apiRequest(`${getApiBaseUrl()}/PaymentGateways/${pendingDeleteId}`, { method: 'DELETE' });
        hideModal('payment-gateways-delete-modal');
        if (!response.success) {
            showError(response.message || 'Failed to delete payment gateway.');
            pendingDeleteId = null;
            return;
        }
        showSuccess('Payment gateway deleted successfully.');
        pendingDeleteId = null;
        await loadGateways();
    };
    const bindEvents = () => {
        document.getElementById('payment-gateways-create')?.addEventListener('click', openCreate);
        document.getElementById('payment-gateways-save')?.addEventListener('click', () => { void saveGateway(); });
        document.getElementById('payment-gateways-confirm-delete')?.addEventListener('click', () => { void doDelete(); });
        document.getElementById('payment-gateways-table-body')?.addEventListener('click', (event) => {
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
        const page = document.getElementById('payment-gateways-page');
        if (!page || page.dataset.initialized === 'true') {
            return;
        }
        page.dataset.initialized = 'true';
        bindEvents();
        await loadInstruments();
        await loadGateways();
    };
    const setupObserver = () => {
        void initializePage();
        if (document.body) {
            const observer = new MutationObserver(() => {
                const page = document.getElementById('payment-gateways-page');
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
//# sourceMappingURL=payment-gateways.js.map
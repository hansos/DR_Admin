"use strict";
(() => {
    const pageId = 'tax-jurisdictions-page';
    let allJurisdictions = [];
    let editingId = null;
    let pendingDeleteId = null;
    function getApiBaseUrl() {
        const settings = window.AppSettings;
        return settings?.apiBaseUrl ?? '';
    }
    function getAuthToken() {
        const auth = window.Auth;
        if (auth?.getToken) {
            return auth.getToken();
        }
        return sessionStorage.getItem('rp_authToken');
    }
    function getBootstrap() {
        const maybeBootstrap = window.bootstrap;
        return maybeBootstrap ?? null;
    }
    async function apiRequest(endpoint, options = {}) {
        try {
            const headers = {
                'Content-Type': 'application/json',
                ...options.headers,
            };
            const token = getAuthToken();
            if (token) {
                headers.Authorization = `Bearer ${token}`;
            }
            const response = await fetch(endpoint, {
                ...options,
                headers,
                credentials: 'include',
            });
            const contentType = response.headers.get('content-type') ?? '';
            const hasJson = contentType.includes('application/json');
            const payload = hasJson ? await response.json() : null;
            if (!response.ok) {
                const body = (payload ?? {});
                return {
                    success: false,
                    message: body.message ?? body.title ?? `Request failed with status ${response.status}`,
                };
            }
            const wrapped = (payload ?? {});
            return {
                success: wrapped.success !== false,
                data: wrapped.data ?? payload,
                message: wrapped.message,
            };
        }
        catch {
            return {
                success: false,
                message: 'Network error. Please try again.',
            };
        }
    }
    function esc(value) {
        const map = { '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#039;' };
        return (value || '').replace(/[&<>"']/g, (c) => map[c] ?? c);
    }
    function extractItems(payload) {
        if (Array.isArray(payload)) {
            return payload;
        }
        const obj = (payload ?? {});
        if (Array.isArray(obj.data)) {
            return obj.data;
        }
        if (Array.isArray(obj.Data)) {
            return obj.Data;
        }
        return [];
    }
    function showSuccess(message) {
        const success = document.getElementById('tax-jurisdictions-alert-success');
        const error = document.getElementById('tax-jurisdictions-alert-error');
        if (success) {
            success.textContent = message;
            success.classList.remove('d-none');
        }
        error?.classList.add('d-none');
    }
    function showError(message) {
        const success = document.getElementById('tax-jurisdictions-alert-success');
        const error = document.getElementById('tax-jurisdictions-alert-error');
        if (error) {
            error.textContent = message;
            error.classList.remove('d-none');
        }
        success?.classList.add('d-none');
    }
    function normalizeItem(item) {
        return {
            id: Number(item.id ?? item.Id ?? 0),
            code: String(item.code ?? item.Code ?? ''),
            name: String(item.name ?? item.Name ?? ''),
            countryCode: String(item.countryCode ?? item.CountryCode ?? ''),
            stateCode: String(item.stateCode ?? item.StateCode ?? ''),
            taxAuthority: String(item.taxAuthority ?? item.TaxAuthority ?? ''),
            taxCurrencyCode: String(item.taxCurrencyCode ?? item.TaxCurrencyCode ?? ''),
            isActive: Boolean(item.isActive ?? item.IsActive ?? false),
            notes: String(item.notes ?? item.Notes ?? ''),
        };
    }
    function renderRows() {
        const tbody = document.getElementById('tax-jurisdictions-table-body');
        if (!tbody) {
            return;
        }
        if (allJurisdictions.length === 0) {
            tbody.innerHTML = '<tr><td colspan="7" class="text-center text-muted">No records found.</td></tr>';
            return;
        }
        tbody.innerHTML = allJurisdictions.map((item) => {
            const state = item.stateCode || '-';
            return `<tr>
                <td>${item.id}</td>
                <td>${esc(item.code)}</td>
                <td>${esc(item.name)}</td>
                <td>${esc(item.countryCode)}</td>
                <td>${esc(state)}</td>
                <td>${item.isActive ? 'Active' : 'Inactive'}</td>
                <td class="text-end">
                    <div class="btn-group btn-group-sm">
                        <button class="btn btn-outline-primary" type="button" data-action="edit" data-id="${item.id}"><i class="bi bi-pencil"></i></button>
                        <button class="btn btn-outline-danger" type="button" data-action="delete" data-id="${item.id}" data-name="${esc(item.name)}"><i class="bi bi-trash"></i></button>
                    </div>
                </td>
            </tr>`;
        }).join('');
    }
    function setInputValue(id, value) {
        const el = document.getElementById(id);
        if (el) {
            el.value = value;
        }
    }
    function getInputValue(id) {
        const el = document.getElementById(id);
        return el?.value.trim() ?? '';
    }
    function getCheckboxValue(id) {
        const el = document.getElementById(id);
        return Boolean(el?.checked);
    }
    function showModal(id) {
        const element = document.getElementById(id);
        const bootstrap = getBootstrap();
        if (!element || !bootstrap) {
            return;
        }
        new bootstrap.Modal(element).show();
    }
    function hideModal(id) {
        const element = document.getElementById(id);
        const bootstrap = getBootstrap();
        if (!element || !bootstrap) {
            return;
        }
        bootstrap.Modal.getInstance(element)?.hide();
    }
    function openCreate() {
        editingId = null;
        const title = document.getElementById('tax-jurisdictions-modal-title');
        if (title) {
            title.textContent = 'New Tax Jurisdiction';
        }
        document.getElementById('tax-jurisdictions-form')?.reset();
        setInputValue('tax-jurisdictions-currency', 'EUR');
        showModal('tax-jurisdictions-edit-modal');
    }
    function openEdit(id) {
        const item = allJurisdictions.find((x) => x.id === id);
        if (!item) {
            return;
        }
        editingId = id;
        const title = document.getElementById('tax-jurisdictions-modal-title');
        if (title) {
            title.textContent = 'Edit Tax Jurisdiction';
        }
        setInputValue('tax-jurisdictions-code', item.code);
        setInputValue('tax-jurisdictions-name', item.name);
        setInputValue('tax-jurisdictions-country', item.countryCode);
        setInputValue('tax-jurisdictions-state', item.stateCode);
        setInputValue('tax-jurisdictions-authority', item.taxAuthority);
        setInputValue('tax-jurisdictions-currency', item.taxCurrencyCode);
        setInputValue('tax-jurisdictions-notes', item.notes);
        const isActive = document.getElementById('tax-jurisdictions-is-active');
        if (isActive) {
            isActive.checked = item.isActive;
        }
        showModal('tax-jurisdictions-edit-modal');
    }
    async function saveItem() {
        const code = getInputValue('tax-jurisdictions-code').toUpperCase();
        const name = getInputValue('tax-jurisdictions-name');
        const countryCode = getInputValue('tax-jurisdictions-country').toUpperCase();
        const taxAuthority = getInputValue('tax-jurisdictions-authority');
        const taxCurrencyCode = getInputValue('tax-jurisdictions-currency').toUpperCase();
        if (!code || !name || !countryCode || !taxAuthority || !taxCurrencyCode) {
            showError('Code, Name, Country, Tax authority and Tax currency are required.');
            return;
        }
        const payload = {
            code,
            name,
            countryCode,
            stateCode: getInputValue('tax-jurisdictions-state') || null,
            taxAuthority,
            taxCurrencyCode,
            isActive: getCheckboxValue('tax-jurisdictions-is-active'),
            notes: getInputValue('tax-jurisdictions-notes'),
        };
        const response = editingId
            ? await apiRequest(`${getApiBaseUrl()}/TaxJurisdictions/${editingId}`, { method: 'PUT', body: JSON.stringify(payload) })
            : await apiRequest(`${getApiBaseUrl()}/TaxJurisdictions`, { method: 'POST', body: JSON.stringify(payload) });
        if (!response.success) {
            showError(response.message ?? 'Failed to save tax jurisdiction.');
            return;
        }
        hideModal('tax-jurisdictions-edit-modal');
        showSuccess(editingId ? 'Tax jurisdiction updated.' : 'Tax jurisdiction created.');
        await loadData(false);
    }
    function openDelete(id, name) {
        pendingDeleteId = id;
        const label = document.getElementById('tax-jurisdictions-delete-name');
        if (label) {
            label.textContent = name;
        }
        showModal('tax-jurisdictions-delete-modal');
    }
    async function doDelete() {
        if (!pendingDeleteId) {
            return;
        }
        const response = await apiRequest(`${getApiBaseUrl()}/TaxJurisdictions/${pendingDeleteId}`, { method: 'DELETE' });
        hideModal('tax-jurisdictions-delete-modal');
        if (!response.success) {
            showError(response.message ?? 'Failed to delete tax jurisdiction.');
            pendingDeleteId = null;
            return;
        }
        pendingDeleteId = null;
        showSuccess('Tax jurisdiction deleted.');
        await loadData(false);
    }
    async function loadData(showFeedback) {
        const tbody = document.getElementById('tax-jurisdictions-table-body');
        if (tbody) {
            tbody.innerHTML = '<tr><td colspan="7" class="text-center"><div class="spinner-border text-primary"></div></td></tr>';
        }
        const response = await apiRequest(`${getApiBaseUrl()}/TaxJurisdictions`, { method: 'GET' });
        if (!response.success) {
            showError(response.message ?? 'Failed to load tax jurisdictions.');
            return;
        }
        allJurisdictions = extractItems(response.data).map((item) => normalizeItem(item));
        renderRows();
        if (showFeedback) {
            showSuccess(`Loaded at ${new Date().toLocaleString()}`);
        }
    }
    function initializePage() {
        const page = document.getElementById(pageId);
        if (!page || page.dataset.initialized === 'true') {
            return;
        }
        page.dataset.initialized = 'true';
        document.getElementById('tax-jurisdictions-create')?.addEventListener('click', openCreate);
        document.getElementById('tax-jurisdictions-save')?.addEventListener('click', () => {
            void saveItem();
        });
        document.getElementById('tax-jurisdictions-confirm-delete')?.addEventListener('click', () => {
            void doDelete();
        });
        document.getElementById('tax-jurisdictions-table-body')?.addEventListener('click', (event) => {
            const target = event.target;
            const actionElement = target.closest('[data-action]');
            if (!actionElement) {
                return;
            }
            const action = actionElement.dataset.action;
            const id = Number(actionElement.dataset.id ?? '0');
            if (!Number.isFinite(id) || id <= 0) {
                return;
            }
            if (action === 'edit') {
                openEdit(id);
                return;
            }
            if (action === 'delete') {
                openDelete(id, actionElement.dataset.name ?? `#${id}`);
            }
        });
        document.getElementById('tax-jurisdictions-refresh')?.addEventListener('click', async () => {
            await loadData(true);
        });
        void loadData(false);
    }
    function setup() {
        initializePage();
        if (!document.body) {
            return;
        }
        const observer = new MutationObserver(() => initializePage());
        observer.observe(document.body, { childList: true, subtree: true });
    }
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', setup);
    }
    else {
        setup();
    }
})();
//# sourceMappingURL=tax-jurisdictions.js.map
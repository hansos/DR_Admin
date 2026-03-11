"use strict";
(() => {
    const pageId = 'tax-registrations-page';
    let allRegistrations = [];
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
    function esc(value) {
        const map = { '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#039;' };
        return (value || '').replace(/[&<>"']/g, (c) => map[c] ?? c);
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
            const response = await fetch(endpoint, { ...options, headers, credentials: 'include' });
            const contentType = response.headers.get('content-type') ?? '';
            const payload = contentType.includes('application/json') ? await response.json() : null;
            if (!response.ok) {
                const body = (payload ?? {});
                return { success: false, message: body.message ?? body.title ?? `Request failed with status ${response.status}` };
            }
            const wrapped = (payload ?? {});
            return { success: wrapped.success !== false, data: wrapped.data ?? payload, message: wrapped.message };
        }
        catch {
            return { success: false, message: 'Network error. Please try again.' };
        }
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
        const success = document.getElementById('tax-registrations-alert-success');
        const error = document.getElementById('tax-registrations-alert-error');
        if (success) {
            success.textContent = message;
            success.classList.remove('d-none');
        }
        error?.classList.add('d-none');
    }
    function showError(message) {
        const success = document.getElementById('tax-registrations-alert-success');
        const error = document.getElementById('tax-registrations-alert-error');
        if (error) {
            error.textContent = message;
            error.classList.remove('d-none');
        }
        success?.classList.add('d-none');
    }
    function normalizeRegistration(item) {
        return {
            id: Number(item.id ?? item.Id ?? 0),
            taxJurisdictionId: Number(item.taxJurisdictionId ?? item.TaxJurisdictionId ?? 0),
            legalEntityName: String(item.legalEntityName ?? item.LegalEntityName ?? ''),
            registrationNumber: String(item.registrationNumber ?? item.RegistrationNumber ?? ''),
            effectiveFrom: String(item.effectiveFrom ?? item.EffectiveFrom ?? ''),
            effectiveUntil: String(item.effectiveUntil ?? item.EffectiveUntil ?? ''),
            isActive: Boolean(item.isActive ?? item.IsActive ?? false),
            notes: String(item.notes ?? item.Notes ?? ''),
        };
    }
    function normalizeJurisdiction(item) {
        return {
            id: Number(item.id ?? item.Id ?? 0),
            code: String(item.code ?? item.Code ?? ''),
            name: String(item.name ?? item.Name ?? ''),
        };
    }
    function formatDate(value) {
        const text = String(value ?? '');
        if (!text) {
            return '-';
        }
        const parsed = new Date(text);
        return Number.isNaN(parsed.getTime()) ? text : parsed.toLocaleDateString();
    }
    function renderRows() {
        const tbody = document.getElementById('tax-registrations-table-body');
        if (!tbody) {
            return;
        }
        if (allRegistrations.length === 0) {
            tbody.innerHTML = '<tr><td colspan="7" class="text-center text-muted">No records found.</td></tr>';
            return;
        }
        const jurisdictionMap = new Map(allJurisdictions.map((x) => [x.id, `${x.code} - ${x.name}`]));
        tbody.innerHTML = allRegistrations.map((item) => {
            const jurisdictionLabel = jurisdictionMap.get(item.taxJurisdictionId) ?? String(item.taxJurisdictionId);
            return `<tr>
                <td>${item.id}</td>
                <td>${esc(jurisdictionLabel)}</td>
                <td>${esc(item.registrationNumber)}</td>
                <td>${formatDate(item.effectiveFrom)}</td>
                <td>${formatDate(item.effectiveUntil)}</td>
                <td>${item.isActive ? 'Active' : 'Inactive'}</td>
                <td class="text-end">
                    <div class="btn-group btn-group-sm">
                        <button class="btn btn-outline-primary" type="button" data-action="edit" data-id="${item.id}"><i class="bi bi-pencil"></i></button>
                        <button class="btn btn-outline-danger" type="button" data-action="delete" data-id="${item.id}" data-name="${esc(item.registrationNumber)}"><i class="bi bi-trash"></i></button>
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
    function toInputDateTime(value) {
        if (!value) {
            return '';
        }
        const date = new Date(value);
        if (Number.isNaN(date.getTime())) {
            return '';
        }
        const offset = date.getTimezoneOffset();
        const local = new Date(date.getTime() - (offset * 60000));
        return local.toISOString().slice(0, 16);
    }
    function fromInputDateTime(value) {
        if (!value) {
            return null;
        }
        const date = new Date(value);
        return Number.isNaN(date.getTime()) ? null : date.toISOString();
    }
    function renderJurisdictionOptions() {
        const select = document.getElementById('tax-registrations-jurisdiction');
        if (!select) {
            return;
        }
        select.innerHTML = allJurisdictions.map((item) => `<option value="${item.id}">${esc(item.code)} - ${esc(item.name)}</option>`).join('');
    }
    function openCreate() {
        editingId = null;
        const title = document.getElementById('tax-registrations-modal-title');
        if (title) {
            title.textContent = 'New Tax Registration';
        }
        document.getElementById('tax-registrations-form')?.reset();
        if (allJurisdictions.length > 0) {
            setInputValue('tax-registrations-jurisdiction', String(allJurisdictions[0].id));
        }
        setInputValue('tax-registrations-from', toInputDateTime(new Date().toISOString()));
        showModal('tax-registrations-edit-modal');
    }
    function openEdit(id) {
        const item = allRegistrations.find((x) => x.id === id);
        if (!item) {
            return;
        }
        editingId = id;
        const title = document.getElementById('tax-registrations-modal-title');
        if (title) {
            title.textContent = 'Edit Tax Registration';
        }
        setInputValue('tax-registrations-jurisdiction', String(item.taxJurisdictionId));
        setInputValue('tax-registrations-entity', item.legalEntityName);
        setInputValue('tax-registrations-number', item.registrationNumber);
        setInputValue('tax-registrations-from', toInputDateTime(item.effectiveFrom));
        setInputValue('tax-registrations-until', toInputDateTime(item.effectiveUntil));
        setInputValue('tax-registrations-notes', item.notes);
        const active = document.getElementById('tax-registrations-is-active');
        if (active) {
            active.checked = item.isActive;
        }
        showModal('tax-registrations-edit-modal');
    }
    async function saveItem() {
        const taxJurisdictionId = Number(getInputValue('tax-registrations-jurisdiction'));
        const legalEntityName = getInputValue('tax-registrations-entity');
        const registrationNumber = getInputValue('tax-registrations-number');
        const effectiveFrom = fromInputDateTime(getInputValue('tax-registrations-from'));
        if (!Number.isFinite(taxJurisdictionId) || taxJurisdictionId <= 0 || !legalEntityName || !registrationNumber || !effectiveFrom) {
            showError('Jurisdiction, Legal entity, Registration number and Effective from are required.');
            return;
        }
        const payload = {
            taxJurisdictionId,
            legalEntityName,
            registrationNumber,
            effectiveFrom,
            effectiveUntil: fromInputDateTime(getInputValue('tax-registrations-until')),
            isActive: getCheckboxValue('tax-registrations-is-active'),
            notes: getInputValue('tax-registrations-notes'),
        };
        const response = editingId
            ? await apiRequest(`${getApiBaseUrl()}/TaxRegistrations/${editingId}`, { method: 'PUT', body: JSON.stringify(payload) })
            : await apiRequest(`${getApiBaseUrl()}/TaxRegistrations`, { method: 'POST', body: JSON.stringify(payload) });
        if (!response.success) {
            showError(response.message ?? 'Failed to save tax registration.');
            return;
        }
        hideModal('tax-registrations-edit-modal');
        showSuccess(editingId ? 'Tax registration updated.' : 'Tax registration created.');
        await loadData(false);
    }
    function openDelete(id, name) {
        pendingDeleteId = id;
        const label = document.getElementById('tax-registrations-delete-name');
        if (label) {
            label.textContent = name;
        }
        showModal('tax-registrations-delete-modal');
    }
    async function doDelete() {
        if (!pendingDeleteId) {
            return;
        }
        const response = await apiRequest(`${getApiBaseUrl()}/TaxRegistrations/${pendingDeleteId}`, { method: 'DELETE' });
        hideModal('tax-registrations-delete-modal');
        if (!response.success) {
            showError(response.message ?? 'Failed to delete tax registration.');
            pendingDeleteId = null;
            return;
        }
        pendingDeleteId = null;
        showSuccess('Tax registration deleted.');
        await loadData(false);
    }
    async function loadData(showFeedback) {
        const tbody = document.getElementById('tax-registrations-table-body');
        if (tbody) {
            tbody.innerHTML = '<tr><td colspan="7" class="text-center"><div class="spinner-border text-primary"></div></td></tr>';
        }
        const [registrationsResponse, jurisdictionsResponse] = await Promise.all([
            apiRequest(`${getApiBaseUrl()}/TaxRegistrations`, { method: 'GET' }),
            apiRequest(`${getApiBaseUrl()}/TaxJurisdictions`, { method: 'GET' }),
        ]);
        if (!jurisdictionsResponse.success) {
            showError(jurisdictionsResponse.message ?? 'Failed to load tax jurisdictions.');
            return;
        }
        allJurisdictions = extractItems(jurisdictionsResponse.data).map((item) => normalizeJurisdiction(item));
        renderJurisdictionOptions();
        if (!registrationsResponse.success) {
            showError(registrationsResponse.message ?? 'Failed to load tax registrations.');
            return;
        }
        allRegistrations = extractItems(registrationsResponse.data).map((item) => normalizeRegistration(item));
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
        document.getElementById('tax-registrations-create')?.addEventListener('click', openCreate);
        document.getElementById('tax-registrations-save')?.addEventListener('click', () => {
            void saveItem();
        });
        document.getElementById('tax-registrations-confirm-delete')?.addEventListener('click', () => {
            void doDelete();
        });
        document.getElementById('tax-registrations-table-body')?.addEventListener('click', (event) => {
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
        document.getElementById('tax-registrations-refresh')?.addEventListener('click', async () => {
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
//# sourceMappingURL=tax-registrations.js.map
"use strict";
(() => {
    const pageId = 'tax-categories-page';
    let allCategories = [];
    let allCountries = [];
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
        const success = document.getElementById('tax-categories-alert-success');
        const error = document.getElementById('tax-categories-alert-error');
        if (success) {
            success.textContent = message;
            success.classList.remove('d-none');
        }
        error?.classList.add('d-none');
    }
    function showError(message) {
        const success = document.getElementById('tax-categories-alert-success');
        const error = document.getElementById('tax-categories-alert-error');
        if (error) {
            error.textContent = message;
            error.classList.remove('d-none');
        }
        success?.classList.add('d-none');
    }
    function normalizeItem(item) {
        return {
            id: Number(item.id ?? item.Id ?? 0),
            countryCode: String(item.countryCode ?? item.CountryCode ?? ''),
            stateCode: String(item.stateCode ?? item.StateCode ?? ''),
            code: String(item.code ?? item.Code ?? ''),
            name: String(item.name ?? item.Name ?? ''),
            description: String(item.description ?? item.Description ?? ''),
            isActive: Boolean(item.isActive ?? item.IsActive ?? false),
        };
    }
    function normalizeCountry(item) {
        return {
            code: String(item.code ?? item.Code ?? '').toUpperCase(),
            englishName: String(item.englishName ?? item.EnglishName ?? ''),
            isActive: Boolean(item.isActive ?? item.IsActive ?? false),
        };
    }
    function renderCountryOptions(selectedCountryCode) {
        const countrySelect = document.getElementById('tax-categories-country');
        if (!countrySelect) {
            return;
        }
        const selected = (selectedCountryCode ?? '').trim().toUpperCase();
        const options = ['<option value="">Select country</option>'];
        allCountries
            .filter((country) => country.isActive)
            .sort((a, b) => a.englishName.localeCompare(b.englishName))
            .forEach((country) => {
            const isSelected = selected && country.code === selected ? ' selected' : '';
            options.push(`<option value="${esc(country.code)}"${isSelected}>${esc(country.code)} - ${esc(country.englishName)}</option>`);
        });
        countrySelect.innerHTML = options.join('');
    }
    function renderRows() {
        const tbody = document.getElementById('tax-categories-table-body');
        if (!tbody) {
            return;
        }
        if (allCategories.length === 0) {
            tbody.innerHTML = '<tr><td colspan="7" class="text-center text-muted">No records found.</td></tr>';
            return;
        }
        tbody.innerHTML = allCategories.map((item) => {
            const stateCode = item.stateCode || '-';
            return `<tr>
                <td>${item.id}</td>
                <td>${esc(item.countryCode)}</td>
                <td>${esc(stateCode)}</td>
                <td>${esc(item.code)}</td>
                <td>${esc(item.name)}</td>
                <td>${item.isActive ? 'Active' : 'Inactive'}</td>
                <td class="text-end">
                    <div class="btn-group btn-group-sm">
                        <button class="btn btn-outline-primary" type="button" data-action="edit" data-id="${item.id}"><i class="bi bi-pencil"></i></button>
                        <button class="btn btn-outline-danger" type="button" data-action="delete" data-id="${item.id}" data-name="${esc(item.code)}"><i class="bi bi-trash"></i></button>
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
        const title = document.getElementById('tax-categories-modal-title');
        if (title) {
            title.textContent = 'New Tax Category';
        }
        document.getElementById('tax-categories-form')?.reset();
        renderCountryOptions();
        showModal('tax-categories-edit-modal');
    }
    function openEdit(id) {
        const item = allCategories.find((x) => x.id === id);
        if (!item) {
            return;
        }
        editingId = id;
        const title = document.getElementById('tax-categories-modal-title');
        if (title) {
            title.textContent = 'Edit Tax Category';
        }
        renderCountryOptions(item.countryCode);
        setInputValue('tax-categories-state', item.stateCode);
        setInputValue('tax-categories-code', item.code);
        setInputValue('tax-categories-name', item.name);
        setInputValue('tax-categories-description', item.description);
        const active = document.getElementById('tax-categories-is-active');
        if (active) {
            active.checked = item.isActive;
        }
        showModal('tax-categories-edit-modal');
    }
    async function saveItem() {
        const countryCode = getInputValue('tax-categories-country').toUpperCase();
        const code = getInputValue('tax-categories-code').toUpperCase();
        const name = getInputValue('tax-categories-name');
        if (!countryCode || !code || !name) {
            showError('Country, Code and Name are required.');
            return;
        }
        const payload = {
            countryCode,
            stateCode: getInputValue('tax-categories-state') || null,
            code,
            name,
            description: getInputValue('tax-categories-description'),
            isActive: getCheckboxValue('tax-categories-is-active'),
        };
        const response = editingId
            ? await apiRequest(`${getApiBaseUrl()}/TaxCategories/${editingId}`, { method: 'PUT', body: JSON.stringify(payload) })
            : await apiRequest(`${getApiBaseUrl()}/TaxCategories`, { method: 'POST', body: JSON.stringify(payload) });
        if (!response.success) {
            showError(response.message ?? 'Failed to save tax category.');
            return;
        }
        hideModal('tax-categories-edit-modal');
        showSuccess(editingId ? 'Tax category updated.' : 'Tax category created.');
        await loadData(false);
    }
    function openDelete(id, name) {
        pendingDeleteId = id;
        const label = document.getElementById('tax-categories-delete-name');
        if (label) {
            label.textContent = name;
        }
        showModal('tax-categories-delete-modal');
    }
    async function doDelete() {
        if (!pendingDeleteId) {
            return;
        }
        const response = await apiRequest(`${getApiBaseUrl()}/TaxCategories/${pendingDeleteId}`, { method: 'DELETE' });
        hideModal('tax-categories-delete-modal');
        if (!response.success) {
            showError(response.message ?? 'Failed to delete tax category.');
            pendingDeleteId = null;
            return;
        }
        pendingDeleteId = null;
        showSuccess('Tax category deleted.');
        await loadData(false);
    }
    async function loadData(showFeedback) {
        const tbody = document.getElementById('tax-categories-table-body');
        if (tbody) {
            tbody.innerHTML = '<tr><td colspan="7" class="text-center"><div class="spinner-border text-primary"></div></td></tr>';
        }
        const response = await apiRequest(`${getApiBaseUrl()}/TaxCategories`, { method: 'GET' });
        const countriesResponse = await apiRequest(`${getApiBaseUrl()}/Countries`, { method: 'GET' });
        if (!countriesResponse.success) {
            showError(countriesResponse.message ?? 'Failed to load countries.');
            return;
        }
        allCountries = extractItems(countriesResponse.data).map((item) => normalizeCountry(item));
        renderCountryOptions();
        if (!response.success) {
            showError(response.message ?? 'Failed to load tax categories.');
            return;
        }
        allCategories = extractItems(response.data).map((item) => normalizeItem(item));
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
        document.getElementById('tax-categories-create')?.addEventListener('click', openCreate);
        document.getElementById('tax-categories-save')?.addEventListener('click', () => {
            void saveItem();
        });
        document.getElementById('tax-categories-confirm-delete')?.addEventListener('click', () => {
            void doDelete();
        });
        document.getElementById('tax-categories-table-body')?.addEventListener('click', (event) => {
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
        document.getElementById('tax-categories-refresh')?.addEventListener('click', async () => {
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
//# sourceMappingURL=tax-categories.js.map
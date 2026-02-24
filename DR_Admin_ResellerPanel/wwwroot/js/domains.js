"use strict";
// @ts-nocheck
(function () {
    function getApiBaseUrl() {
        var _a, _b;
        return (_b = (_a = window.AppSettings) === null || _a === void 0 ? void 0 : _a.apiBaseUrl) !== null && _b !== void 0 ? _b : '';
    }
    function getAuthToken() {
        const auth = window.Auth;
        if (auth === null || auth === void 0 ? void 0 : auth.getToken) {
            return auth.getToken();
        }
        return sessionStorage.getItem('rp_authToken');
    }
    async function apiRequest(endpoint, options = {}) {
        var _a, _b, _c;
        try {
            const headers = Object.assign({ 'Content-Type': 'application/json' }, options.headers);
            const authToken = getAuthToken();
            if (authToken) {
                headers['Authorization'] = `Bearer ${authToken}`;
            }
            const response = await fetch(endpoint, Object.assign(Object.assign({}, options), { headers, credentials: 'include' }));
            const contentType = (_a = response.headers.get('content-type')) !== null && _a !== void 0 ? _a : '';
            const hasJson = contentType.includes('application/json');
            const data = hasJson ? await response.json() : null;
            if (!response.ok) {
                return {
                    success: false,
                    message: (data && ((_b = data.message) !== null && _b !== void 0 ? _b : data.title)) || `Request failed with status ${response.status}`,
                };
            }
            return {
                success: (data === null || data === void 0 ? void 0 : data.success) !== false,
                data: (_c = data === null || data === void 0 ? void 0 : data.data) !== null && _c !== void 0 ? _c : data,
                message: data === null || data === void 0 ? void 0 : data.message,
            };
        }
        catch (error) {
            console.error('Domains request failed', error);
            return {
                success: false,
                message: 'Network error. Please try again.',
            };
        }
    }
    function extractItems(raw) {
        var _a, _b, _c, _d, _e, _f;
        if (Array.isArray(raw)) {
            return { items: raw, meta: null };
        }
        const candidates = [raw, raw === null || raw === void 0 ? void 0 : raw.data, raw === null || raw === void 0 ? void 0 : raw.Data, (_a = raw === null || raw === void 0 ? void 0 : raw.data) === null || _a === void 0 ? void 0 : _a.data, (_b = raw === null || raw === void 0 ? void 0 : raw.data) === null || _b === void 0 ? void 0 : _b.Data];
        const items = (Array.isArray(raw === null || raw === void 0 ? void 0 : raw.Data) && raw.Data) ||
            (Array.isArray(raw === null || raw === void 0 ? void 0 : raw.data) && raw.data) ||
            (Array.isArray((_c = raw === null || raw === void 0 ? void 0 : raw.data) === null || _c === void 0 ? void 0 : _c.Data) && raw.data.Data) ||
            (Array.isArray((_d = raw === null || raw === void 0 ? void 0 : raw.data) === null || _d === void 0 ? void 0 : _d.data) && raw.data.data) ||
            (Array.isArray((_e = raw === null || raw === void 0 ? void 0 : raw.Data) === null || _e === void 0 ? void 0 : _e.Data) && raw.Data.Data) ||
            [];
        const meta = (_f = candidates.find((c) => c && typeof c === 'object' && (c.totalCount !== undefined || c.TotalCount !== undefined ||
            c.totalPages !== undefined || c.TotalPages !== undefined ||
            c.currentPage !== undefined || c.CurrentPage !== undefined ||
            c.pageSize !== undefined || c.PageSize !== undefined))) !== null && _f !== void 0 ? _f : null;
        return { items, meta };
    }
    let allDomains = [];
    let editingId = null;
    let pendingDeleteId = null;
    let registrarOptions = [];
    let currentPage = 1;
    let pageSize = 25;
    let totalCount = 0;
    let totalPages = 1;
    function loadPageSizeFromUi() {
        var _a;
        const el = document.getElementById('domains-page-size');
        const parsed = Number(((_a = el === null || el === void 0 ? void 0 : el.value) !== null && _a !== void 0 ? _a : '').trim());
        if (Number.isFinite(parsed) && parsed > 0) {
            pageSize = parsed;
        }
    }
    function buildPagedUrl() {
        const params = new URLSearchParams();
        params.set('pageNumber', String(currentPage));
        params.set('pageSize', String(pageSize));
        return `${getApiBaseUrl()}/RegisteredDomains?${params.toString()}`;
    }
    function normalizeItem(item) {
        var _a, _b, _c, _d, _e, _f, _g, _h, _j, _k, _l, _m, _o, _p, _q, _r, _s, _t, _u, _v, _w, _x, _y, _z;
        return {
            id: (_b = (_a = item.id) !== null && _a !== void 0 ? _a : item.Id) !== null && _b !== void 0 ? _b : 0,
            customerId: (_d = (_c = item.customerId) !== null && _c !== void 0 ? _c : item.CustomerId) !== null && _d !== void 0 ? _d : 0,
            serviceId: (_f = (_e = item.serviceId) !== null && _e !== void 0 ? _e : item.ServiceId) !== null && _f !== void 0 ? _f : 0,
            name: (_h = (_g = item.name) !== null && _g !== void 0 ? _g : item.Name) !== null && _h !== void 0 ? _h : '',
            providerId: (_k = (_j = item.providerId) !== null && _j !== void 0 ? _j : item.ProviderId) !== null && _k !== void 0 ? _k : 0,
            status: (_m = (_l = item.status) !== null && _l !== void 0 ? _l : item.Status) !== null && _m !== void 0 ? _m : '',
            registrationDate: (_p = (_o = item.registrationDate) !== null && _o !== void 0 ? _o : item.RegistrationDate) !== null && _p !== void 0 ? _p : '',
            expirationDate: (_r = (_q = item.expirationDate) !== null && _q !== void 0 ? _q : item.ExpirationDate) !== null && _r !== void 0 ? _r : '',
            createdAt: (_t = (_s = item.createdAt) !== null && _s !== void 0 ? _s : item.CreatedAt) !== null && _t !== void 0 ? _t : null,
            updatedAt: (_v = (_u = item.updatedAt) !== null && _u !== void 0 ? _u : item.UpdatedAt) !== null && _v !== void 0 ? _v : null,
            customer: (_x = (_w = item.customer) !== null && _w !== void 0 ? _w : item.Customer) !== null && _x !== void 0 ? _x : null,
            registrar: (_z = (_y = item.registrar) !== null && _y !== void 0 ? _y : item.Registrar) !== null && _z !== void 0 ? _z : null,
        };
    }
    function normalizeRegistrarOption(item) {
        var _a, _b, _c, _d, _e, _f;
        return {
            id: (_b = (_a = item.id) !== null && _a !== void 0 ? _a : item.Id) !== null && _b !== void 0 ? _b : 0,
            name: (_d = (_c = item.name) !== null && _c !== void 0 ? _c : item.Name) !== null && _d !== void 0 ? _d : '',
            code: (_f = (_e = item.code) !== null && _e !== void 0 ? _e : item.Code) !== null && _f !== void 0 ? _f : null,
        };
    }
    async function loadDomains() {
        var _a, _b, _c, _d, _e, _f, _g, _h, _j, _k, _l, _m, _o, _p, _q, _r, _s;
        const tableBody = document.getElementById('domains-table-body');
        if (!tableBody) {
            return;
        }
        tableBody.innerHTML = '<tr><td colspan="8" class="text-center"><div class="spinner-border text-primary"></div></td></tr>';
        loadPageSizeFromUi();
        const response = await apiRequest(buildPagedUrl(), { method: 'GET' });
        if (!response.success) {
            showError(response.message || 'Failed to load domains');
            tableBody.innerHTML = '<tr><td colspan="8" class="text-center text-danger">Failed to load data</td></tr>';
            return;
        }
        const raw = response.data;
        const extracted = extractItems(raw);
        const meta = (_a = extracted.meta) !== null && _a !== void 0 ? _a : raw;
        allDomains = extracted.items.map(normalizeItem);
        pageSize = (_e = (_d = (_c = (_b = meta === null || meta === void 0 ? void 0 : meta.pageSize) !== null && _b !== void 0 ? _b : meta === null || meta === void 0 ? void 0 : meta.PageSize) !== null && _c !== void 0 ? _c : raw === null || raw === void 0 ? void 0 : raw.pageSize) !== null && _d !== void 0 ? _d : raw === null || raw === void 0 ? void 0 : raw.PageSize) !== null && _e !== void 0 ? _e : pageSize;
        totalCount = (_j = (_h = (_g = (_f = meta === null || meta === void 0 ? void 0 : meta.totalCount) !== null && _f !== void 0 ? _f : meta === null || meta === void 0 ? void 0 : meta.TotalCount) !== null && _g !== void 0 ? _g : raw === null || raw === void 0 ? void 0 : raw.totalCount) !== null && _h !== void 0 ? _h : raw === null || raw === void 0 ? void 0 : raw.TotalCount) !== null && _j !== void 0 ? _j : allDomains.length;
        totalPages = (_o = (_m = (_l = (_k = meta === null || meta === void 0 ? void 0 : meta.totalPages) !== null && _k !== void 0 ? _k : meta === null || meta === void 0 ? void 0 : meta.TotalPages) !== null && _l !== void 0 ? _l : raw === null || raw === void 0 ? void 0 : raw.totalPages) !== null && _m !== void 0 ? _m : raw === null || raw === void 0 ? void 0 : raw.TotalPages) !== null && _o !== void 0 ? _o : Math.max(1, Math.ceil(totalCount / pageSize));
        currentPage = (_s = (_r = (_q = (_p = meta === null || meta === void 0 ? void 0 : meta.currentPage) !== null && _p !== void 0 ? _p : meta === null || meta === void 0 ? void 0 : meta.CurrentPage) !== null && _q !== void 0 ? _q : raw === null || raw === void 0 ? void 0 : raw.currentPage) !== null && _r !== void 0 ? _r : raw === null || raw === void 0 ? void 0 : raw.CurrentPage) !== null && _s !== void 0 ? _s : currentPage;
        renderTable();
        renderPagination();
    }
    async function loadRegistrarsForImport() {
        const select = document.getElementById('domains-import-registrar');
        if (!select) {
            return;
        }
        select.innerHTML = '<option value="">Loading...</option>';
        const response = await apiRequest(`${getApiBaseUrl()}/Registrars`, { method: 'GET' });
        if (!response.success) {
            select.innerHTML = '<option value="">Select registrar</option>';
            showError(response.message || 'Failed to load registrars');
            return;
        }
        const rawItems = Array.isArray(response.data)
            ? response.data
            : Array.isArray((_a = response.data) === null || _a === void 0 ? void 0 : _a.data)
                ? response.data.data
                : [];
        registrarOptions = rawItems.map(normalizeRegistrarOption);
        registrarOptions.sort((a, b) => a.name.localeCompare(b.name));
        const options = registrarOptions.map((registrar) => {
            const label = registrar.code ? `${registrar.name} (${registrar.code})` : registrar.name;
            return `<option value="${registrar.id}">${esc(label)}</option>`;
        }).join('');
        select.innerHTML = `<option value="">Select registrar</option>${options}`;
        var _a;
    }
    function getCustomerName(domain) {
        var _a, _b;
        const cust = domain.customer;
        return (_b = (_a = cust === null || cust === void 0 ? void 0 : cust.name) !== null && _a !== void 0 ? _a : cust === null || cust === void 0 ? void 0 : cust.Name) !== null && _b !== void 0 ? _b : '';
    }
    function getRegistrarName(domain) {
        var _a, _b;
        const reg = domain.registrar;
        return (_b = (_a = reg === null || reg === void 0 ? void 0 : reg.name) !== null && _a !== void 0 ? _a : reg === null || reg === void 0 ? void 0 : reg.Name) !== null && _b !== void 0 ? _b : '';
    }
    function renderTable() {
        const tableBody = document.getElementById('domains-table-body');
        if (!tableBody) {
            return;
        }
        if (!allDomains.length) {
            tableBody.innerHTML = '<tr><td colspan="8" class="text-center text-muted">No domains found.</td></tr>';
            return;
        }
        tableBody.innerHTML = allDomains.map((domain) => {
            const customerName = getCustomerName(domain);
            const registrarName = getRegistrarName(domain);
            const customerDisplay = customerName
                ? `${esc(customerName)} <span class="text-muted">(#${domain.customerId})</span>`
                : `<span class="text-muted">#${domain.customerId}</span>`;
            const registrarDisplay = registrarName
                ? `${esc(registrarName)} <span class="text-muted">(#${domain.providerId})</span>`
                : `<span class="text-muted">#${domain.providerId}</span>`;
            const registered = domain.registrationDate ? formatDate(domain.registrationDate) : '-';
            const expires = domain.expirationDate ? formatDate(domain.expirationDate) : '-';
            return `
        <tr>
            <td>${domain.id}</td>
            <td><code>${esc(domain.name)}</code></td>
            <td>${customerDisplay}</td>
            <td>${registrarDisplay}</td>
            <td>${esc(domain.status || '-')}</td>
            <td>${esc(registered)}</td>
            <td>${esc(expires)}</td>
            <td class="text-end">
                <div class="btn-group btn-group-sm">
                    <button class="btn btn-outline-primary" type="button" data-action="edit" data-id="${domain.id}" title="Edit"><i class="bi bi-pencil"></i></button>
                    <button class="btn btn-outline-danger" type="button" data-action="delete" data-id="${domain.id}" data-name="${esc(domain.name)}" title="Delete"><i class="bi bi-trash"></i></button>
                </div>
            </td>
        </tr>`;
        }).join('');
    }
    function renderPagingControls() {
        const list = document.getElementById('pagingControlsList');
        if (!list) {
            return;
        }
        if (!totalCount || totalPages <= 1) {
            list.innerHTML = '';
            return;
        }
        const makeItem = (label, page, disabled, active = false) => {
            const cls = `page-item${disabled ? ' disabled' : ''}${active ? ' active' : ''}`;
            const ariaCurrent = active ? ' aria-current="page"' : '';
            const ariaDisabled = disabled ? ' aria-disabled="true" tabindex="-1"' : '';
            const dataPage = disabled ? '' : ` data-page="${page}"`;
            return `<li class="${cls}"><a class="page-link" href="#"${dataPage}${ariaCurrent}${ariaDisabled}>${label}</a></li>`;
        };
        const makeEllipsis = () => '<li class="page-item disabled"><span class="page-link">…</span></li>';
        const prevDisabled = currentPage <= 1;
        const nextDisabled = currentPage >= totalPages;
        let html = '';
        html += makeItem('Previous', currentPage - 1, prevDisabled);
        const pages = new Set();
        pages.add(1);
        if (totalPages >= 2)
            pages.add(2);
        pages.add(totalPages);
        if (totalPages >= 2)
            pages.add(totalPages - 1);
        const windowSize = 1;
        for (let p = currentPage - windowSize; p <= currentPage + windowSize; p++) {
            if (p >= 1 && p <= totalPages) {
                pages.add(p);
            }
        }
        const sorted = Array.from(pages)
            .filter((p) => p >= 1 && p <= totalPages)
            .sort((a, b) => a - b);
        let last = 0;
        for (const p of sorted) {
            if (last && p - last > 1) {
                html += makeEllipsis();
            }
            html += makeItem(String(p), p, false, p === currentPage);
            last = p;
        }
        html += makeItem('Next', currentPage + 1, nextDisabled);
        list.innerHTML = html;
    }
    function renderPagination() {
        const info = document.getElementById('domains-pagination-info');
        if (!info) {
            return;
        }
        if (!totalCount) {
            info.textContent = 'Showing 0 of 0';
            renderPagingControls();
            return;
        }
        const start = (currentPage - 1) * pageSize + 1;
        const end = Math.min(currentPage * pageSize, totalCount);
        info.textContent = `Showing ${start}-${end} of ${totalCount}`;
        renderPagingControls();
    }
    function changePage(page) {
        if (page < 1 || page > totalPages) {
            return;
        }
        currentPage = page;
        loadDomains();
    }
    function bindPagingControlsActions() {
        const container = document.getElementById('pagingControls');
        if (!container) {
            return;
        }
        container.addEventListener('click', (event) => {
            const target = event.target;
            const link = target.closest('a[data-page]');
            if (!link) {
                return;
            }
            event.preventDefault();
            const page = Number(link.dataset.page);
            if (!Number.isFinite(page)) {
                return;
            }
            changePage(page);
        });
    }
    function openCreate() {
        editingId = null;
        const title = document.getElementById('domains-modal-title');
        if (title) {
            title.textContent = 'New Domain';
        }
        const info = document.getElementById('domains-edit-info');
        if (info) {
            info.classList.add('d-none');
            info.textContent = '';
        }
        const form = document.getElementById('domains-form');
        form === null || form === void 0 ? void 0 : form.reset();
        setInputValue('domains-status', 'Active');
        setDateTimeLocalValue('domains-registration-date', new Date());
        showModal('domains-edit-modal');
    }
    function openImport() {
        const select = document.getElementById('domains-import-registrar');
        if (select) {
            select.value = '';
        }
        loadRegistrarsForImport();
        showModal('domains-import-modal');
    }
    function openEdit(id) {
        var _a, _b, _c;
        const domain = allDomains.find((d) => d.id === id);
        if (!domain) {
            return;
        }
        editingId = id;
        const title = document.getElementById('domains-modal-title');
        if (title) {
            title.textContent = 'Edit Domain';
        }
        const info = document.getElementById('domains-edit-info');
        if (info) {
            info.textContent = `ID: ${domain.id}`;
            info.classList.remove('d-none');
        }
        setInputValue('domains-name', domain.name);
        setInputValue('domains-status', domain.status || '');
        setInputValue('domains-provider-id', String((_a = domain.providerId) !== null && _a !== void 0 ? _a : 0));
        setInputValue('domains-customer-id', String((_b = domain.customerId) !== null && _b !== void 0 ? _b : 0));
        setInputValue('domains-service-id', String((_c = domain.serviceId) !== null && _c !== void 0 ? _c : 0));
        setDateTimeLocalValue('domains-registration-date', domain.registrationDate ? new Date(domain.registrationDate) : null);
        setDateTimeLocalValue('domains-expiration-date', domain.expirationDate ? new Date(domain.expirationDate) : null);
        showModal('domains-edit-modal');
    }
    function setInputValue(id, value) {
        const el = document.getElementById(id);
        if (el) {
            el.value = value !== null && value !== void 0 ? value : '';
        }
    }
    function getInputValue(id) {
        var _a;
        const el = document.getElementById(id);
        return ((_a = el === null || el === void 0 ? void 0 : el.value) !== null && _a !== void 0 ? _a : '').trim();
    }
    function getSelectValue(id) {
        var _a;
        const el = document.getElementById(id);
        return ((_a = el === null || el === void 0 ? void 0 : el.value) !== null && _a !== void 0 ? _a : '').trim();
    }
    function getNumberValue(id) {
        const raw = getInputValue(id);
        const parsed = Number(raw);
        return Number.isFinite(parsed) ? parsed : 0;
    }
    function setImportBusy(isBusy) {
        const progress = document.getElementById('domains-import-progress');
        const select = document.getElementById('domains-import-registrar');
        const confirm = document.getElementById('domains-import-confirm');
        if (progress) {
            progress.classList.toggle('d-none', !isBusy);
        }
        if (select) {
            select.disabled = isBusy;
        }
        if (confirm) {
            confirm.disabled = isBusy;
        }
    }
    function setImportSummary(message) {
        const summary = document.getElementById('domains-import-summary');
        if (!summary) {
            return;
        }
        if (!message) {
            summary.textContent = '';
            summary.classList.add('d-none');
            return;
        }
        summary.textContent = message;
        summary.classList.remove('d-none');
    }
    function setDateTimeLocalValue(id, date) {
        const el = document.getElementById(id);
        if (!el) {
            return;
        }
        if (!date || Number.isNaN(date.getTime())) {
            el.value = '';
            return;
        }
        const pad = (n) => String(n).padStart(2, '0');
        const value = `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())}T${pad(date.getHours())}:${pad(date.getMinutes())}`;
        el.value = value;
    }
    function getDateTimeIsoValue(id) {
        const raw = getInputValue(id);
        if (!raw) {
            return '';
        }
        const date = new Date(raw);
        if (Number.isNaN(date.getTime())) {
            return '';
        }
        return date.toISOString();
    }
    async function saveDomain() {
        const name = getInputValue('domains-name');
        const customerId = getNumberValue('domains-customer-id');
        const serviceId = getNumberValue('domains-service-id');
        const providerId = getNumberValue('domains-provider-id');
        const registrationDate = getDateTimeIsoValue('domains-registration-date');
        const expirationDate = getDateTimeIsoValue('domains-expiration-date');
        if (!name || !customerId || !serviceId || !providerId || !registrationDate || !expirationDate) {
            showError('Domain Name, Customer ID, Service ID, Registrar ID, Registration Date and Expiration Date are required');
            return;
        }
        const payload = {
            name,
            customerId,
            serviceId,
            providerId,
            status: getInputValue('domains-status') || 'Active',
            registrationDate,
            expirationDate,
        };
        const response = editingId
            ? await apiRequest(`${getApiBaseUrl()}/RegisteredDomains/${editingId}`, { method: 'PUT', body: JSON.stringify(payload) })
            : await apiRequest(`${getApiBaseUrl()}/RegisteredDomains`, { method: 'POST', body: JSON.stringify(payload) });
        if (response.success) {
            hideModal('domains-edit-modal');
            showSuccess(editingId ? 'Domain updated successfully' : 'Domain created successfully');
            loadDomains();
        }
        else {
            showError(response.message || 'Save failed');
        }
    }
    async function importDomains() {
        const registrarValue = getSelectValue('domains-import-registrar');
        const registrarId = Number(registrarValue);
        if (!registrarValue || !Number.isFinite(registrarId) || registrarId <= 0) {
            showError('Select a registrar to import domains');
            return;
        }
        setImportSummary('');
        setImportBusy(true);
        let totalExisting = null;
        const existingResponse = await apiRequest(`${getApiBaseUrl()}/Registrars/${registrarId}/domains`, { method: 'GET' });
        if (existingResponse.success) {
            const existingData = existingResponse.data;
            totalExisting = (existingData === null || existingData === void 0 ? void 0 : existingData.totalCount) ?? (existingData === null || existingData === void 0 ? void 0 : existingData.TotalCount) ?? ((existingData === null || existingData === void 0 ? void 0 : existingData.domains) ? existingData.domains.length : null) ?? ((existingData === null || existingData === void 0 ? void 0 : existingData.Domains) ? existingData.Domains.length : null) ?? null;
        }
        const response = await apiRequest(`${getApiBaseUrl()}/Registrars/${registrarId}/domains/download`, { method: 'POST' });
        setImportBusy(false);
        if (response.success) {
            const resultData = response.data;
            const importedCount = (resultData === null || resultData === void 0 ? void 0 : resultData.count) ?? (resultData === null || resultData === void 0 ? void 0 : resultData.Count) ?? ((resultData === null || resultData === void 0 ? void 0 : resultData.data) ? resultData.data.count : null) ?? ((resultData === null || resultData === void 0 ? void 0 : resultData.data) ? resultData.data.Count : null) ?? null;
            const totalLabel = totalExisting ?? importedCount ?? 0;
            const importedLabel = importedCount ?? 0;
            const summaryMessage = `Imported ${importedLabel} of ${totalLabel} domains.`;
            setImportSummary(summaryMessage);
            showSuccess(summaryMessage);
            loadDomains();
        }
        else {
            showError(response.message || 'Import failed');
        }
    }
    function openDelete(id, name) {
        pendingDeleteId = id;
        const deleteName = document.getElementById('domains-delete-name');
        if (deleteName) {
            deleteName.textContent = name;
        }
        showModal('domains-delete-modal');
    }
    async function doDelete() {
        if (!pendingDeleteId) {
            return;
        }
        const response = await apiRequest(`${getApiBaseUrl()}/RegisteredDomains/${pendingDeleteId}`, { method: 'DELETE' });
        hideModal('domains-delete-modal');
        if (response.success) {
            showSuccess('Domain deleted successfully');
            if (currentPage > 1 && allDomains.length === 1) {
                currentPage = currentPage - 1;
            }
            loadDomains();
        }
        else {
            showError(response.message || 'Delete failed');
        }
        pendingDeleteId = null;
    }
    function bindTableActions() {
        const tableBody = document.getElementById('domains-table-body');
        if (!tableBody) {
            return;
        }
        tableBody.addEventListener('click', (event) => {
            var _a;
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
                openDelete(id, (_a = button.dataset.name) !== null && _a !== void 0 ? _a : '');
            }
        });
    }
    function esc(text) {
        const map = { '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#039;' };
        return (text || '').toString().replace(/[&<>"']/g, (char) => map[char]);
    }
    function formatDate(value) {
        const date = new Date(value);
        if (Number.isNaN(date.getTime())) {
            return value;
        }
        return date.toLocaleString();
    }
    function showSuccess(message) {
        const alert = document.getElementById('domains-alert-success');
        if (!alert) {
            return;
        }
        alert.textContent = message;
        alert.classList.remove('d-none');
        const errorAlert = document.getElementById('domains-alert-error');
        errorAlert === null || errorAlert === void 0 ? void 0 : errorAlert.classList.add('d-none');
        setTimeout(() => alert.classList.add('d-none'), 5000);
    }
    function showError(message) {
        const alert = document.getElementById('domains-alert-error');
        if (!alert) {
            return;
        }
        alert.textContent = message;
        alert.classList.remove('d-none');
        const successAlert = document.getElementById('domains-alert-success');
        successAlert === null || successAlert === void 0 ? void 0 : successAlert.classList.add('d-none');
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
        modal === null || modal === void 0 ? void 0 : modal.hide();
    }
    function initializeDomainsPage() {
        var _a, _b, _c, _d, _e, _f;
        const page = document.getElementById('domains-page');
        if (!page || page.dataset.initialized === 'true') {
            return;
        }
        page.dataset.initialized = 'true';
        (_a = document.getElementById('domains-create')) === null || _a === void 0 ? void 0 : _a.addEventListener('click', openCreate);
        (_b = document.getElementById('domains-save')) === null || _b === void 0 ? void 0 : _b.addEventListener('click', saveDomain);
        (_c = document.getElementById('domains-import')) === null || _c === void 0 ? void 0 : _c.addEventListener('click', openImport);
        (_d = document.getElementById('domains-import-confirm')) === null || _d === void 0 ? void 0 : _d.addEventListener('click', importDomains);
        (_e = document.getElementById('domains-confirm-delete')) === null || _e === void 0 ? void 0 : _e.addEventListener('click', doDelete);
        bindTableActions();
        bindPagingControlsActions();
        (_f = document.getElementById('domains-page-size')) === null || _f === void 0 ? void 0 : _f.addEventListener('change', () => {
            currentPage = 1;
            loadDomains();
        });
        loadDomains();
    }
    function setupPageObserver() {
        initializeDomainsPage();
        if (document.body) {
            const observer = new MutationObserver(() => {
                const page = document.getElementById('domains-page');
                if (page && page.dataset.initialized !== 'true') {
                    initializeDomainsPage();
                }
            });
            observer.observe(document.body, { childList: true, subtree: true });
        }
    }
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', setupPageObserver);
    }
    else {
        setupPageObserver();
    }
})();
//# sourceMappingURL=domains.js.map
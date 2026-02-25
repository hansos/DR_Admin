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
            console.error('Registrars request failed', error);
            return {
                success: false,
                message: 'Network error. Please try again.',
            };
        }
    }
    let allRegistrars = [];
    let editingId = null;
    let pendingDeleteId = null;
    let currentPage = 1;
    let pageSize = 25;
    let totalCount = 0;
    let totalPages = 1;
    function loadPageSizeFromUi() {
        var _a;
        const el = document.getElementById('registrars-page-size');
        const parsed = Number(((_a = el === null || el === void 0 ? void 0 : el.value) !== null && _a !== void 0 ? _a : '').trim());
        if (Number.isFinite(parsed) && parsed > 0) {
            pageSize = parsed;
        }
    }
    function normalizeRegistrar(item) {
        var _a, _b, _c, _d, _e, _f, _g, _h, _j, _k, _l, _m, _o, _p, _q, _r, _s, _t, _u, _v, _w, _x;
        return {
            id: (_b = (_a = item.id) !== null && _a !== void 0 ? _a : item.Id) !== null && _b !== void 0 ? _b : 0,
            name: (_d = (_c = item.name) !== null && _c !== void 0 ? _c : item.Name) !== null && _d !== void 0 ? _d : '',
            code: (_f = (_e = item.code) !== null && _e !== void 0 ? _e : item.Code) !== null && _f !== void 0 ? _f : '',
            isActive: (_h = (_g = item.isActive) !== null && _g !== void 0 ? _g : item.IsActive) !== null && _h !== void 0 ? _h : false,
            contactEmail: (_k = (_j = item.contactEmail) !== null && _j !== void 0 ? _j : item.ContactEmail) !== null && _k !== void 0 ? _k : null,
            contactPhone: (_m = (_l = item.contactPhone) !== null && _l !== void 0 ? _l : item.ContactPhone) !== null && _m !== void 0 ? _m : null,
            website: (_p = (_o = item.website) !== null && _o !== void 0 ? _o : item.Website) !== null && _p !== void 0 ? _p : null,
            notes: (_r = (_q = item.notes) !== null && _q !== void 0 ? _q : item.Notes) !== null && _r !== void 0 ? _r : null,
            isDefault: (_t = (_s = item.isDefault) !== null && _s !== void 0 ? _s : item.IsDefault) !== null && _t !== void 0 ? _t : false,
            createdAt: (_v = (_u = item.createdAt) !== null && _u !== void 0 ? _u : item.CreatedAt) !== null && _v !== void 0 ? _v : null,
            updatedAt: (_x = (_w = item.updatedAt) !== null && _w !== void 0 ? _w : item.UpdatedAt) !== null && _x !== void 0 ? _x : null,
        };
    }
    async function loadRegistrars() {
        var _a;
        const tableBody = document.getElementById('registrars-table-body');
        if (!tableBody) {
            return;
        }
        tableBody.innerHTML = '<tr><td colspan="8" class="text-center"><div class="spinner-border text-primary"></div></td></tr>';
        const response = await apiRequest(`${getApiBaseUrl()}/Registrars`, { method: 'GET' });
        if (!response.success) {
            showError(response.message || 'Failed to load registrars');
            tableBody.innerHTML = '<tr><td colspan="8" class="text-center text-danger">Failed to load data</td></tr>';
            return;
        }
        const rawItems = Array.isArray(response.data)
            ? response.data
            : Array.isArray((_a = response.data) === null || _a === void 0 ? void 0 : _a.data)
                ? response.data.data
                : [];
        allRegistrars = rawItems.map(normalizeRegistrar);
        totalCount = allRegistrars.length;
        loadPageSizeFromUi();
        totalPages = Math.max(1, Math.ceil(totalCount / pageSize));
        currentPage = Math.min(currentPage, totalPages);
        renderTable();
        renderPagination();
    }
    function renderTable() {
        const tableBody = document.getElementById('registrars-table-body');
        if (!tableBody) {
            return;
        }
        if (!allRegistrars.length) {
            tableBody.innerHTML = '<tr><td colspan="8" class="text-center text-muted">No registrars found.</td></tr>';
            return;
        }
        const startIndex = (currentPage - 1) * pageSize;
        const pageItems = allRegistrars.slice(startIndex, startIndex + pageSize);
        tableBody.innerHTML = pageItems.map((registrar) => {
            const contactInfo = registrar.contactEmail || registrar.contactPhone
                ? `${registrar.contactEmail ? esc(registrar.contactEmail) : ''}${registrar.contactEmail && registrar.contactPhone ? '<br />' : ''}${registrar.contactPhone ? esc(registrar.contactPhone) : ''}`
                : '<span class="text-muted">-</span>';
            const website = registrar.website ? `<a href="${esc(registrar.website)}" target="_blank" rel="noopener">${esc(registrar.website)}</a>` : '<span class="text-muted">-</span>';
            const activeBadge = registrar.isActive
                ? '<span class="badge bg-success">Yes</span>'
                : '<span class="badge bg-secondary">No</span>';
            const defaultBadge = registrar.isDefault
                ? '<span class="badge bg-primary">Default</span>'
                : '<span class="badge bg-secondary">No</span>';
            return `
        <tr>
            <td>${registrar.id}</td>
            <td>${esc(registrar.name)}</td>
            <td><code>${esc(registrar.code)}</code></td>
            <td>${contactInfo}</td>
            <td>${website}</td>
            <td>${activeBadge}</td>
            <td>${defaultBadge}</td>
            <td class="text-end">
                <div class="btn-group btn-group-sm">
                    <button class="btn btn-outline-primary" type="button" data-action="edit" data-id="${registrar.id}" title="Edit"><i class="bi bi-pencil"></i></button>
                    <button class="btn btn-outline-danger" type="button" data-action="delete" data-id="${registrar.id}" data-name="${esc(registrar.name)}" title="Delete"><i class="bi bi-trash"></i></button>
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
        const info = document.getElementById('registrars-pagination-info');
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
        renderTable();
        renderPagination();
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
        const title = document.getElementById('registrars-modal-title');
        if (title) {
            title.textContent = 'New Registrar';
        }
        const form = document.getElementById('registrars-form');
        form === null || form === void 0 ? void 0 : form.reset();
        setCheckboxValue('registrars-is-active', true);
        setCheckboxValue('registrars-is-default', false);
        showModal('registrars-edit-modal');
    }
    function openEdit(id) {
        const registrar = allRegistrars.find((r) => r.id === id);
        if (!registrar) {
            return;
        }
        editingId = id;
        const title = document.getElementById('registrars-modal-title');
        if (title) {
            title.textContent = 'Edit Registrar';
        }
        setInputValue('registrars-name', registrar.name);
        setInputValue('registrars-code', registrar.code);
        setInputValue('registrars-contact-email', registrar.contactEmail || '');
        setInputValue('registrars-contact-phone', registrar.contactPhone || '');
        setInputValue('registrars-website', registrar.website || '');
        setTextAreaValue('registrars-notes', registrar.notes || '');
        setCheckboxValue('registrars-is-active', !!registrar.isActive);
        setCheckboxValue('registrars-is-default', !!registrar.isDefault);
        showModal('registrars-edit-modal');
    }
    function setInputValue(id, value) {
        const el = document.getElementById(id);
        if (el) {
            el.value = value !== null && value !== void 0 ? value : '';
        }
    }
    function setTextAreaValue(id, value) {
        const el = document.getElementById(id);
        if (el) {
            el.value = value !== null && value !== void 0 ? value : '';
        }
    }
    function setCheckboxValue(id, value) {
        const el = document.getElementById(id);
        if (el) {
            el.checked = value;
        }
    }
    function getInputValue(id) {
        var _a;
        const el = document.getElementById(id);
        return ((_a = el === null || el === void 0 ? void 0 : el.value) !== null && _a !== void 0 ? _a : '').trim();
    }
    function getTextAreaValue(id) {
        var _a;
        const el = document.getElementById(id);
        return ((_a = el === null || el === void 0 ? void 0 : el.value) !== null && _a !== void 0 ? _a : '').trim();
    }
    function getCheckboxValue(id) {
        const el = document.getElementById(id);
        return !!(el === null || el === void 0 ? void 0 : el.checked);
    }
    async function saveRegistrar() {
        const name = getInputValue('registrars-name');
        const code = getInputValue('registrars-code');
        if (!name || !code) {
            showError('Name and Code are required');
            return;
        }
        const payload = {
            name,
            code,
            contactEmail: getInputValue('registrars-contact-email') || null,
            contactPhone: getInputValue('registrars-contact-phone') || null,
            website: getInputValue('registrars-website') || null,
            notes: getTextAreaValue('registrars-notes') || null,
            isActive: getCheckboxValue('registrars-is-active'),
            isDefault: getCheckboxValue('registrars-is-default'),
        };
        const response = editingId
            ? await apiRequest(`${getApiBaseUrl()}/Registrars/${editingId}`, { method: 'PUT', body: JSON.stringify(payload) })
            : await apiRequest(`${getApiBaseUrl()}/Registrars`, { method: 'POST', body: JSON.stringify(payload) });
        if (response.success) {
            hideModal('registrars-edit-modal');
            showSuccess(editingId ? 'Registrar updated successfully' : 'Registrar created successfully');
            loadRegistrars();
        }
        else {
            showError(response.message || 'Save failed');
        }
    }
    function openDelete(id, name) {
        pendingDeleteId = id;
        const deleteName = document.getElementById('registrars-delete-name');
        if (deleteName) {
            deleteName.textContent = name;
        }
        showModal('registrars-delete-modal');
    }
    async function doDelete() {
        if (!pendingDeleteId) {
            return;
        }
        const response = await apiRequest(`${getApiBaseUrl()}/Registrars/${pendingDeleteId}`, { method: 'DELETE' });
        hideModal('registrars-delete-modal');
        if (response.success) {
            showSuccess('Registrar deleted successfully');
            if (currentPage > 1 && (allRegistrars.length - 1) <= (currentPage - 1) * pageSize) {
                currentPage = currentPage - 1;
            }
            loadRegistrars();
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
        const alert = document.getElementById('registrars-alert-success');
        if (!alert) {
            return;
        }
        alert.textContent = message;
        alert.classList.remove('d-none');
        const errorAlert = document.getElementById('registrars-alert-error');
        errorAlert === null || errorAlert === void 0 ? void 0 : errorAlert.classList.add('d-none');
        setTimeout(() => alert.classList.add('d-none'), 5000);
    }
    function showError(message) {
        const alert = document.getElementById('registrars-alert-error');
        if (!alert) {
            return;
        }
        alert.textContent = message;
        alert.classList.remove('d-none');
        const successAlert = document.getElementById('registrars-alert-success');
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
    function bindTableActions() {
        const tableBody = document.getElementById('registrars-table-body');
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
    function initializeRegistrarsPage() {
        var _a, _b, _c, _d;
        const page = document.getElementById('registrars-page');
        if (!page || page.dataset.initialized === 'true') {
            return;
        }
        page.dataset.initialized = 'true';
        (_a = document.getElementById('registrars-create')) === null || _a === void 0 ? void 0 : _a.addEventListener('click', openCreate);
        (_b = document.getElementById('registrars-save')) === null || _b === void 0 ? void 0 : _b.addEventListener('click', saveRegistrar);
        (_c = document.getElementById('registrars-confirm-delete')) === null || _c === void 0 ? void 0 : _c.addEventListener('click', doDelete);
        bindTableActions();
        bindPagingControlsActions();
        (_d = document.getElementById('registrars-page-size')) === null || _d === void 0 ? void 0 : _d.addEventListener('change', () => {
            currentPage = 1;
            renderTable();
            renderPagination();
        });
        loadRegistrars();
    }
    function setupPageObserver() {
        initializeRegistrarsPage();
        if (document.body) {
            const observer = new MutationObserver(() => {
                const page = document.getElementById('registrars-page');
                if (page && page.dataset.initialized !== 'true') {
                    initializeRegistrarsPage();
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
//# sourceMappingURL=registry-apis.js.map
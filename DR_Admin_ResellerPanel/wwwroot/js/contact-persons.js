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
            console.error('Contact persons request failed', error);
            return {
                success: false,
                message: 'Network error. Please try again.',
            };
        }
    }
    function extractItems(raw) {
        if (Array.isArray(raw)) {
            return { items: raw, meta: null };
        }
        const candidates = [raw, raw === null || raw === void 0 ? void 0 : raw.data, raw === null || raw === void 0 ? void 0 : raw.Data, raw === null || raw === void 0 ? void 0 : raw.data.data, raw === null || raw === void 0 ? void 0 : raw.data.Data];
        const items =
            (Array.isArray(raw === null || raw === void 0 ? void 0 : raw.Data) && raw.Data) ||
                (Array.isArray(raw === null || raw === void 0 ? void 0 : raw.data) && raw.data) ||
                (Array.isArray(raw === null || raw === void 0 ? void 0 : raw.data.Data) && raw.data.Data) ||
                (Array.isArray(raw === null || raw === void 0 ? void 0 : raw.data.data) && raw.data.data) ||
                (Array.isArray(raw === null || raw === void 0 ? void 0 : raw.Data.Data) && raw.Data.Data) ||
                [];
        const meta = candidates.find((c) => c && typeof c === 'object' && (
            c.totalCount !== undefined || c.TotalCount !== undefined ||
                c.totalPages !== undefined || c.TotalPages !== undefined ||
                c.currentPage !== undefined || c.CurrentPage !== undefined ||
                c.pageSize !== undefined || c.PageSize !== undefined)) || null;
        return { items, meta };
    }
    let allContactPersons = [];
    let customerOptions = [];
    let customerLookup = new Map();
    let editingId = null;
    let pendingDeleteId = null;
    let currentPage = 1;
    let pageSize = 25;
    let totalCount = 0;
    let totalPages = 1;
    function loadPageSizeFromUi() {
        const el = document.getElementById('contact-persons-page-size');
        const parsed = Number(((el === null || el === void 0 ? void 0 : el.value) ?? '').trim());
        if (Number.isFinite(parsed) && parsed > 0) {
            pageSize = parsed;
        }
    }
    function buildPagedUrl() {
        const params = new URLSearchParams();
        params.set('pageNumber', String(currentPage));
        params.set('pageSize', String(pageSize));
        return `${getApiBaseUrl()}/ContactPersons?${params.toString()}`;
    }
    function normalizeContactPerson(item) {
        var _a, _b, _c, _d, _e, _f, _g, _h, _j, _k, _l, _m, _o, _p, _q, _r, _s, _t, _u, _v;
        return {
            id: (_b = (_a = item.id) !== null && _a !== void 0 ? _a : item.Id) !== null && _b !== void 0 ? _b : 0,
            firstName: (_d = (_c = item.firstName) !== null && _c !== void 0 ? _c : item.FirstName) !== null && _d !== void 0 ? _d : '',
            lastName: (_f = (_e = item.lastName) !== null && _e !== void 0 ? _e : item.LastName) !== null && _f !== void 0 ? _f : '',
            email: (_h = (_g = item.email) !== null && _g !== void 0 ? _g : item.Email) !== null && _h !== void 0 ? _h : '',
            phone: (_k = (_j = item.phone) !== null && _j !== void 0 ? _j : item.Phone) !== null && _k !== void 0 ? _k : '',
            position: (_m = (_l = item.position) !== null && _l !== void 0 ? _l : item.Position) !== null && _m !== void 0 ? _m : null,
            department: (_p = (_o = item.department) !== null && _o !== void 0 ? _o : item.Department) !== null && _p !== void 0 ? _p : null,
            isPrimary: (_r = (_q = item.isPrimary) !== null && _q !== void 0 ? _q : item.IsPrimary) !== null && _r !== void 0 ? _r : false,
            isActive: (_t = (_s = item.isActive) !== null && _s !== void 0 ? _s : item.IsActive) !== null && _t !== void 0 ? _t : false,
            notes: (_v = (_u = item.notes) !== null && _u !== void 0 ? _u : item.Notes) !== null && _v !== void 0 ? _v : null,
            customerId: item.customerId ?? item.CustomerId ?? null,
            createdAt: item.createdAt ?? item.CreatedAt ?? null,
            updatedAt: item.updatedAt ?? item.UpdatedAt ?? null,
            isDefaultOwner: item.isDefaultOwner ?? item.IsDefaultOwner ?? false,
            isDefaultBilling: item.isDefaultBilling ?? item.IsDefaultBilling ?? false,
            isDefaultTech: item.isDefaultTech ?? item.IsDefaultTech ?? false,
            isDefaultAdministrator: item.isDefaultAdministrator ?? item.IsDefaultAdministrator ?? false,
        };
    }
    function normalizeCustomerOption(item) {
        var _a, _b, _c, _d, _e, _f;
        const id = (_b = (_a = item.id) !== null && _a !== void 0 ? _a : item.Id) !== null && _b !== void 0 ? _b : 0;
        const name = (_f = (_e = (_d = (_c = item.customerName) !== null && _c !== void 0 ? _c : item.CustomerName) !== null && _d !== void 0 ? _d : item.name) !== null && _e !== void 0 ? _e : item.Name) !== null && _f !== void 0 ? _f : `Customer ${id}`;
        return { id, name };
    }
    async function loadCustomersForSelect() {
        const select = document.getElementById('contact-persons-customer');
        if (!select) {
            return;
        }
        select.innerHTML = '<option value="">Unassigned</option>';
        const params = new URLSearchParams();
        params.set('pageNumber', '1');
        params.set('pageSize', '1000');
        const response = await apiRequest(`${getApiBaseUrl()}/Customers?${params.toString()}`, { method: 'GET' });
        if (!response.success) {
            return;
        }
        const raw = response.data;
        const extracted = extractItems(raw);
        customerOptions = extracted.items.map(normalizeCustomerOption);
        customerOptions.sort((a, b) => a.name.localeCompare(b.name));
        customerLookup = new Map(customerOptions.map((option) => [option.id, option.name]));
        select.insertAdjacentHTML('beforeend', customerOptions.map((option) => (`<option value="${option.id}">${esc(option.name)}</option>`)).join(''));
    }
    async function loadContactPersons() {
        const tableBody = document.getElementById('contact-persons-table-body');
        if (!tableBody) {
            return;
        }
        tableBody.innerHTML = '<tr><td colspan="10" class="text-center"><div class="spinner-border text-primary"></div></td></tr>';
        loadPageSizeFromUi();
        const response = await apiRequest(buildPagedUrl(), { method: 'GET' });
        if (!response.success) {
            showError(response.message || 'Failed to load contact persons');
            tableBody.innerHTML = '<tr><td colspan="10" class="text-center text-danger">Failed to load data</td></tr>';
            return;
        }
        const raw = response.data;
        const extracted = extractItems(raw);
        const meta = extracted.meta ?? raw;
        allContactPersons = extracted.items.map(normalizeContactPerson);
        pageSize = (meta === null || meta === void 0 ? void 0 : meta.pageSize) ?? (meta === null || meta === void 0 ? void 0 : meta.PageSize) ?? (raw === null || raw === void 0 ? void 0 : raw.pageSize) ?? (raw === null || raw === void 0 ? void 0 : raw.PageSize) ?? pageSize;
        totalCount = (meta === null || meta === void 0 ? void 0 : meta.totalCount) ?? (meta === null || meta === void 0 ? void 0 : meta.TotalCount) ?? (raw === null || raw === void 0 ? void 0 : raw.totalCount) ?? (raw === null || raw === void 0 ? void 0 : raw.TotalCount) ?? allContactPersons.length;
        totalPages = (meta === null || meta === void 0 ? void 0 : meta.totalPages) ?? (meta === null || meta === void 0 ? void 0 : meta.TotalPages) ?? (raw === null || raw === void 0 ? void 0 : raw.totalPages) ?? (raw === null || raw === void 0 ? void 0 : raw.TotalPages) ?? Math.max(1, Math.ceil(totalCount / pageSize));
        currentPage = (meta === null || meta === void 0 ? void 0 : meta.currentPage) ?? (meta === null || meta === void 0 ? void 0 : meta.CurrentPage) ?? (raw === null || raw === void 0 ? void 0 : raw.currentPage) ?? (raw === null || raw === void 0 ? void 0 : raw.CurrentPage) ?? currentPage;
        renderTable();
        renderPagination();
    }
    function renderTable() {
        const tableBody = document.getElementById('contact-persons-table-body');
        if (!tableBody) {
            return;
        }
        if (!allContactPersons.length) {
            tableBody.innerHTML = '<tr><td colspan="10" class="text-center text-muted">No contact persons found.</td></tr>';
            return;
        }
        tableBody.innerHTML = allContactPersons.map((contact) => {
            const fullName = `${contact.firstName} ${contact.lastName}`.trim();
            const customerName = contact.customerId ? customerLookup.get(contact.customerId) || `Customer ${contact.customerId}` : 'Unassigned';
            const primaryBadge = contact.isPrimary
                ? '<span class="badge bg-info">Yes</span>'
                : '<span class="badge bg-secondary">No</span>';
            const activeBadge = contact.isActive
                ? '<span class="badge bg-success">Yes</span>'
                : '<span class="badge bg-secondary">No</span>';
            const defaults = buildDefaultsBadges(contact);
            return `
        <tr>
            <td>${contact.id}</td>
            <td>${esc(customerName)}</td>
            <td>${esc(fullName || '-')}</td>
            <td><a href="mailto:${esc(contact.email)}">${esc(contact.email)}</a></td>
            <td>${esc(contact.phone || '-')}</td>
            <td>${esc(contact.position || '-')}</td>
            <td>${primaryBadge}</td>
            <td>${activeBadge}</td>
            <td>${defaults}</td>
            <td class="text-end">
                <div class="btn-group btn-group-sm">
                    <button class="btn btn-outline-primary" type="button" data-action="edit" data-id="${contact.id}" title="Edit"><i class="bi bi-pencil"></i></button>
                    <button class="btn btn-outline-danger" type="button" data-action="delete" data-id="${contact.id}" data-name="${esc(fullName)}" title="Delete"><i class="bi bi-trash"></i></button>
                </div>
            </td>
        </tr>`;
        }).join('');
    }
    function buildDefaultsBadges(contact) {
        const badges = [];
        if (contact.isDefaultOwner) {
            badges.push('<span class="badge bg-primary me-1">Owner</span>');
        }
        if (contact.isDefaultBilling) {
            badges.push('<span class="badge bg-warning text-dark me-1">Billing</span>');
        }
        if (contact.isDefaultTech) {
            badges.push('<span class="badge bg-info text-dark me-1">Tech</span>');
        }
        if (contact.isDefaultAdministrator) {
            badges.push('<span class="badge bg-secondary me-1">Admin</span>');
        }
        return badges.length ? badges.join('') : '<span class="text-muted">—</span>';
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
        const info = document.getElementById('contact-persons-pagination-info');
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
        loadContactPersons();
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
        const title = document.getElementById('contact-persons-modal-title');
        if (title) {
            title.textContent = 'New Contact Person';
        }
        const info = document.getElementById('contact-persons-edit-info');
        if (info) {
            info.classList.add('d-none');
            info.textContent = '';
        }
        const form = document.getElementById('contact-persons-form');
        form === null || form === void 0 ? void 0 : form.reset();
        setCheckboxValue('contact-persons-is-active', true);
        setCheckboxValue('contact-persons-is-primary', false);
        setCheckboxValue('contact-persons-default-owner', false);
        setCheckboxValue('contact-persons-default-billing', false);
        setCheckboxValue('contact-persons-default-tech', false);
        setCheckboxValue('contact-persons-default-admin', false);
        showModal('contact-persons-edit-modal');
    }
    function openEdit(id) {
        const contact = allContactPersons.find((c) => c.id === id);
        if (!contact) {
            return;
        }
        editingId = id;
        const title = document.getElementById('contact-persons-modal-title');
        if (title) {
            title.textContent = 'Edit Contact Person';
        }
        const info = document.getElementById('contact-persons-edit-info');
        if (info) {
            const customerName = contact.customerId ? customerLookup.get(contact.customerId) || `Customer ${contact.customerId}` : 'Unassigned';
            info.textContent = `ID: ${contact.id} | Customer: ${customerName}`;
            info.classList.remove('d-none');
        }
        setInputValue('contact-persons-first-name', contact.firstName);
        setInputValue('contact-persons-last-name', contact.lastName);
        setInputValue('contact-persons-email', contact.email);
        setInputValue('contact-persons-phone', contact.phone);
        setInputValue('contact-persons-position', contact.position || '');
        setInputValue('contact-persons-department', contact.department || '');
        setTextAreaValue('contact-persons-notes', contact.notes || '');
        setSelectValue('contact-persons-customer', contact.customerId ? String(contact.customerId) : '');
        setCheckboxValue('contact-persons-is-primary', !!contact.isPrimary);
        setCheckboxValue('contact-persons-is-active', !!contact.isActive);
        setCheckboxValue('contact-persons-default-owner', !!contact.isDefaultOwner);
        setCheckboxValue('contact-persons-default-billing', !!contact.isDefaultBilling);
        setCheckboxValue('contact-persons-default-tech', !!contact.isDefaultTech);
        setCheckboxValue('contact-persons-default-admin', !!contact.isDefaultAdministrator);
        showModal('contact-persons-edit-modal');
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
    function setSelectValue(id, value) {
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
    function getSelectValue(id) {
        var _a;
        const el = document.getElementById(id);
        return ((_a = el === null || el === void 0 ? void 0 : el.value) !== null && _a !== void 0 ? _a : '').trim();
    }
    function getCheckboxValue(id) {
        const el = document.getElementById(id);
        return !!(el === null || el === void 0 ? void 0 : el.checked);
    }
    function getCustomerIdValue() {
        const raw = getSelectValue('contact-persons-customer');
        if (!raw) {
            return null;
        }
        const parsed = Number(raw);
        return Number.isFinite(parsed) ? parsed : null;
    }
    async function saveContactPerson() {
        const firstName = getInputValue('contact-persons-first-name');
        const lastName = getInputValue('contact-persons-last-name');
        const email = getInputValue('contact-persons-email');
        const phone = getInputValue('contact-persons-phone');
        if (!firstName || !lastName || !email || !phone) {
            showError('First name, last name, email and phone are required');
            return;
        }
        const payload = {
            firstName,
            lastName,
            email,
            phone,
            position: getInputValue('contact-persons-position') || null,
            department: getInputValue('contact-persons-department') || null,
            isPrimary: getCheckboxValue('contact-persons-is-primary'),
            isActive: getCheckboxValue('contact-persons-is-active'),
            notes: getTextAreaValue('contact-persons-notes') || null,
            customerId: getCustomerIdValue(),
            isDefaultOwner: getCheckboxValue('contact-persons-default-owner'),
            isDefaultBilling: getCheckboxValue('contact-persons-default-billing'),
            isDefaultTech: getCheckboxValue('contact-persons-default-tech'),
            isDefaultAdministrator: getCheckboxValue('contact-persons-default-admin'),
        };
        const response = editingId
            ? await apiRequest(`${getApiBaseUrl()}/ContactPersons/${editingId}`, { method: 'PUT', body: JSON.stringify(payload) })
            : await apiRequest(`${getApiBaseUrl()}/ContactPersons`, { method: 'POST', body: JSON.stringify(payload) });
        if (response.success) {
            hideModal('contact-persons-edit-modal');
            showSuccess(editingId ? 'Contact person updated successfully' : 'Contact person created successfully');
            loadContactPersons();
        }
        else {
            showError(response.message || 'Save failed');
        }
    }
    function openDelete(id, name) {
        pendingDeleteId = id;
        const deleteName = document.getElementById('contact-persons-delete-name');
        if (deleteName) {
            deleteName.textContent = name;
        }
        showModal('contact-persons-delete-modal');
    }
    async function doDelete() {
        if (!pendingDeleteId) {
            return;
        }
        const response = await apiRequest(`${getApiBaseUrl()}/ContactPersons/${pendingDeleteId}`, { method: 'DELETE' });
        hideModal('contact-persons-delete-modal');
        if (response.success) {
            showSuccess('Contact person deleted successfully');
            if (currentPage > 1 && allContactPersons.length === 1) {
                currentPage = currentPage - 1;
            }
            loadContactPersons();
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
        const alert = document.getElementById('contact-persons-alert-success');
        if (!alert) {
            return;
        }
        alert.textContent = message;
        alert.classList.remove('d-none');
        const errorAlert = document.getElementById('contact-persons-alert-error');
        errorAlert === null || errorAlert === void 0 ? void 0 : errorAlert.classList.add('d-none');
        setTimeout(() => alert.classList.add('d-none'), 5000);
    }
    function showError(message) {
        const alert = document.getElementById('contact-persons-alert-error');
        if (!alert) {
            return;
        }
        alert.textContent = message;
        alert.classList.remove('d-none');
        const successAlert = document.getElementById('contact-persons-alert-success');
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
        const tableBody = document.getElementById('contact-persons-table-body');
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
    function initializeContactPersonsPage() {
        const page = document.getElementById('contact-persons-page');
        if (!page || page.dataset.initialized === 'true') {
            return;
        }
        page.dataset.initialized = 'true';
        document.getElementById('contact-persons-create')?.addEventListener('click', openCreate);
        document.getElementById('contact-persons-save')?.addEventListener('click', saveContactPerson);
        document.getElementById('contact-persons-confirm-delete')?.addEventListener('click', doDelete);
        bindTableActions();
        bindPagingControlsActions();
        document.getElementById('contact-persons-page-size')?.addEventListener('change', () => {
            currentPage = 1;
            loadContactPersons();
        });
        loadCustomersForSelect().then(loadContactPersons);
    }
    function setupPageObserver() {
        initializeContactPersonsPage();
        if (document.body) {
            const observer = new MutationObserver(() => {
                const page = document.getElementById('contact-persons-page');
                if (page && page.dataset.initialized !== 'true') {
                    initializeContactPersonsPage();
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

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
            console.error('Customers request failed', error);
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
    let allCustomers = [];
    let editingId = null;
    let pendingDeleteId = null;
    let currentPage = 1;
    let pageSize = 25;
    let totalCount = 0;
    let totalPages = 1;
    function loadPageSizeFromUi() {
        var _a;
        const el = document.getElementById('customers-page-size');
        const parsed = Number(((_a = el === null || el === void 0 ? void 0 : el.value) !== null && _a !== void 0 ? _a : '').trim());
        if (Number.isFinite(parsed) && parsed > 0) {
            pageSize = parsed;
        }
    }
    function buildPagedUrl() {
        const params = new URLSearchParams();
        params.set('pageNumber', String(currentPage));
        params.set('pageSize', String(pageSize));
        return `${getApiBaseUrl()}/Customers?${params.toString()}`;
    }
    function normalizeItem(item) {
        var _a, _b, _c, _d, _e, _f, _g, _h, _j, _k, _l, _m, _o, _p, _q, _r, _s, _t, _u, _v, _w, _x, _y, _z, _0, _1, _2, _3, _4, _5, _6, _7, _8, _9, _10, _11, _12, _13, _14, _15, _16, _17, _18, _19, _20, _21;
        return {
            id: (_b = (_a = item.id) !== null && _a !== void 0 ? _a : item.Id) !== null && _b !== void 0 ? _b : 0,
            referenceNumber: (_d = (_c = item.referenceNumber) !== null && _c !== void 0 ? _c : item.ReferenceNumber) !== null && _d !== void 0 ? _d : 0,
            formattedReferenceNumber: (_f = (_e = item.formattedReferenceNumber) !== null && _e !== void 0 ? _e : item.FormattedReferenceNumber) !== null && _f !== void 0 ? _f : null,
            customerNumber: (_h = (_g = item.customerNumber) !== null && _g !== void 0 ? _g : item.CustomerNumber) !== null && _h !== void 0 ? _h : null,
            formattedCustomerNumber: (_k = (_j = item.formattedCustomerNumber) !== null && _j !== void 0 ? _j : item.FormattedCustomerNumber) !== null && _k !== void 0 ? _k : null,
            name: (_m = (_l = item.name) !== null && _l !== void 0 ? _l : item.Name) !== null && _m !== void 0 ? _m : '',
            email: (_p = (_o = item.email) !== null && _o !== void 0 ? _o : item.Email) !== null && _p !== void 0 ? _p : '',
            phone: (_r = (_q = item.phone) !== null && _q !== void 0 ? _q : item.Phone) !== null && _r !== void 0 ? _r : '',
            customerName: (_t = (_s = item.customerName) !== null && _s !== void 0 ? _s : item.CustomerName) !== null && _t !== void 0 ? _t : null,
            taxId: (_v = (_u = item.taxId) !== null && _u !== void 0 ? _u : item.TaxId) !== null && _v !== void 0 ? _v : null,
            vatNumber: (_x = (_w = item.vatNumber) !== null && _w !== void 0 ? _w : item.VatNumber) !== null && _x !== void 0 ? _x : null,
            isCompany: (_z = (_y = item.isCompany) !== null && _y !== void 0 ? _y : item.IsCompany) !== null && _z !== void 0 ? _z : false,
            isActive: (_1 = (_0 = item.isActive) !== null && _0 !== void 0 ? _0 : item.IsActive) !== null && _1 !== void 0 ? _1 : false,
            status: (_3 = (_2 = item.status) !== null && _2 !== void 0 ? _2 : item.Status) !== null && _3 !== void 0 ? _3 : 'Active',
            balance: (_5 = (_4 = item.balance) !== null && _4 !== void 0 ? _4 : item.Balance) !== null && _5 !== void 0 ? _5 : 0,
            creditLimit: (_7 = (_6 = item.creditLimit) !== null && _6 !== void 0 ? _6 : item.CreditLimit) !== null && _7 !== void 0 ? _7 : 0,
            notes: (_9 = (_8 = item.notes) !== null && _8 !== void 0 ? _8 : item.Notes) !== null && _9 !== void 0 ? _9 : null,
            billingEmail: (_11 = (_10 = item.billingEmail) !== null && _10 !== void 0 ? _10 : item.BillingEmail) !== null && _11 !== void 0 ? _11 : null,
            preferredPaymentMethod: (_13 = (_12 = item.preferredPaymentMethod) !== null && _12 !== void 0 ? _12 : item.PreferredPaymentMethod) !== null && _13 !== void 0 ? _13 : null,
            preferredCurrency: (_15 = (_14 = item.preferredCurrency) !== null && _14 !== void 0 ? _14 : item.PreferredCurrency) !== null && _15 !== void 0 ? _15 : 'EUR',
            allowCurrencyOverride: (_17 = (_16 = item.allowCurrencyOverride) !== null && _16 !== void 0 ? _16 : item.AllowCurrencyOverride) !== null && _17 !== void 0 ? _17 : true,
            createdAt: (_19 = (_18 = item.createdAt) !== null && _18 !== void 0 ? _18 : item.CreatedAt) !== null && _19 !== void 0 ? _19 : null,
            updatedAt: (_21 = (_20 = item.updatedAt) !== null && _20 !== void 0 ? _20 : item.UpdatedAt) !== null && _21 !== void 0 ? _21 : null,
        };
    }
    async function loadCustomers() {
        var _a, _b, _c, _d, _e, _f, _g, _h, _j, _k, _l, _m, _o, _p, _q, _r, _s;
        const tableBody = document.getElementById('customers-table-body');
        if (!tableBody) {
            return;
        }
        tableBody.innerHTML = '<tr><td colspan="9" class="text-center"><div class="spinner-border text-primary"></div></td></tr>';
        loadPageSizeFromUi();
        const response = await apiRequest(buildPagedUrl(), { method: 'GET' });
        if (!response.success) {
            showError(response.message || 'Failed to load customers');
            tableBody.innerHTML = '<tr><td colspan="9" class="text-center text-danger">Failed to load data</td></tr>';
            return;
        }
        const raw = response.data;
        const extracted = extractItems(raw);
        const meta = (_a = extracted.meta) !== null && _a !== void 0 ? _a : raw;
        allCustomers = extracted.items.map(normalizeItem);
        pageSize = (_e = (_d = (_c = (_b = meta === null || meta === void 0 ? void 0 : meta.pageSize) !== null && _b !== void 0 ? _b : meta === null || meta === void 0 ? void 0 : meta.PageSize) !== null && _c !== void 0 ? _c : raw === null || raw === void 0 ? void 0 : raw.pageSize) !== null && _d !== void 0 ? _d : raw === null || raw === void 0 ? void 0 : raw.PageSize) !== null && _e !== void 0 ? _e : pageSize;
        totalCount = (_j = (_h = (_g = (_f = meta === null || meta === void 0 ? void 0 : meta.totalCount) !== null && _f !== void 0 ? _f : meta === null || meta === void 0 ? void 0 : meta.TotalCount) !== null && _g !== void 0 ? _g : raw === null || raw === void 0 ? void 0 : raw.totalCount) !== null && _h !== void 0 ? _h : raw === null || raw === void 0 ? void 0 : raw.TotalCount) !== null && _j !== void 0 ? _j : allCustomers.length;
        totalPages = (_o = (_m = (_l = (_k = meta === null || meta === void 0 ? void 0 : meta.totalPages) !== null && _k !== void 0 ? _k : meta === null || meta === void 0 ? void 0 : meta.TotalPages) !== null && _l !== void 0 ? _l : raw === null || raw === void 0 ? void 0 : raw.totalPages) !== null && _m !== void 0 ? _m : raw === null || raw === void 0 ? void 0 : raw.TotalPages) !== null && _o !== void 0 ? _o : Math.max(1, Math.ceil(totalCount / pageSize));
        currentPage = (_s = (_r = (_q = (_p = meta === null || meta === void 0 ? void 0 : meta.currentPage) !== null && _p !== void 0 ? _p : meta === null || meta === void 0 ? void 0 : meta.CurrentPage) !== null && _q !== void 0 ? _q : raw === null || raw === void 0 ? void 0 : raw.currentPage) !== null && _r !== void 0 ? _r : raw === null || raw === void 0 ? void 0 : raw.CurrentPage) !== null && _s !== void 0 ? _s : currentPage;
        renderTable();
        renderPagination();
    }
    function renderTable() {
        const tableBody = document.getElementById('customers-table-body');
        if (!tableBody) {
            return;
        }
        if (!allCustomers.length) {
            tableBody.innerHTML = '<tr><td colspan="9" class="text-center text-muted">No customers found.</td></tr>';
            return;
        }
        tableBody.innerHTML = allCustomers.map((customer) => {
            const reference = customer.formattedReferenceNumber || (customer.referenceNumber ? String(customer.referenceNumber) : '-');
            const customerNo = customer.formattedCustomerNumber || (customer.customerNumber ? String(customer.customerNumber) : '-');
            const status = customer.status || '-';
            const activeBadge = customer.isActive
                ? '<span class="badge bg-success">Yes</span>'
                : '<span class="badge bg-secondary">No</span>';
            return `
        <tr>
            <td>${customer.id}</td>
            <td>${esc(reference)}</td>
            <td>${esc(customerNo)}</td>
            <td>${esc(customer.name)}</td>
            <td><a href="mailto:${esc(customer.email)}">${esc(customer.email)}</a></td>
            <td>${esc(customer.phone || '-')}</td>
            <td>${esc(status)}</td>
            <td>${activeBadge}</td>
            <td class="text-end">
                <div class="btn-group btn-group-sm">
                    <button class="btn btn-outline-primary" type="button" data-action="edit" data-id="${customer.id}" title="Edit"><i class="bi bi-pencil"></i></button>
                    <button class="btn btn-outline-danger" type="button" data-action="delete" data-id="${customer.id}" data-name="${esc(customer.name)}" title="Delete"><i class="bi bi-trash"></i></button>
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
        const info = document.getElementById('customers-pagination-info');
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
        loadCustomers();
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
        const title = document.getElementById('customers-modal-title');
        if (title) {
            title.textContent = 'New Customer';
        }
        const info = document.getElementById('customers-edit-info');
        if (info) {
            info.classList.add('d-none');
            info.textContent = '';
        }
        const form = document.getElementById('customers-form');
        form === null || form === void 0 ? void 0 : form.reset();
        const isActive = document.getElementById('customers-is-active');
        if (isActive) {
            isActive.checked = true;
        }
        const allowCurrencyOverride = document.getElementById('customers-allow-currency-override');
        if (allowCurrencyOverride) {
            allowCurrencyOverride.checked = true;
        }
        showModal('customers-edit-modal');
    }
    function openEdit(id) {
        var _a;
        const customer = allCustomers.find((c) => c.id === id);
        if (!customer) {
            return;
        }
        editingId = id;
        const title = document.getElementById('customers-modal-title');
        if (title) {
            title.textContent = 'Edit Customer';
        }
        const info = document.getElementById('customers-edit-info');
        if (info) {
            const reference = customer.formattedReferenceNumber || (customer.referenceNumber ? String(customer.referenceNumber) : '-');
            const customerNo = customer.formattedCustomerNumber || (customer.customerNumber ? String(customer.customerNumber) : '-');
            info.textContent = `ID: ${customer.id} | Reference: ${reference} | Customer: ${customerNo}`;
            info.classList.remove('d-none');
        }
        setInputValue('customers-name', customer.name);
        setInputValue('customers-customer-name', customer.customerName || '');
        setInputValue('customers-email', customer.email);
        setInputValue('customers-billing-email', customer.billingEmail || '');
        setInputValue('customers-phone', customer.phone);
        setInputValue('customers-tax-id', customer.taxId || '');
        setInputValue('customers-vat-number', customer.vatNumber || '');
        setInputValue('customers-status', customer.status || 'Active');
        setInputValue('customers-preferred-currency', customer.preferredCurrency || 'EUR');
        setInputValue('customers-credit-limit', String((_a = customer.creditLimit) !== null && _a !== void 0 ? _a : 0));
        setInputValue('customers-preferred-payment-method', customer.preferredPaymentMethod || '');
        setTextAreaValue('customers-notes', customer.notes || '');
        setCheckboxValue('customers-is-company', !!customer.isCompany);
        setCheckboxValue('customers-is-active', !!customer.isActive);
        setCheckboxValue('customers-allow-currency-override', customer.allowCurrencyOverride !== false);
        showModal('customers-edit-modal');
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
    function getNumberValue(id) {
        const raw = getInputValue(id);
        if (!raw) {
            return 0;
        }
        const parsed = Number(raw);
        return Number.isFinite(parsed) ? parsed : 0;
    }
    function getCheckboxValue(id) {
        const el = document.getElementById(id);
        return !!(el === null || el === void 0 ? void 0 : el.checked);
    }
    async function saveCustomer() {
        const name = getInputValue('customers-name');
        const email = getInputValue('customers-email');
        const phone = getInputValue('customers-phone');
        if (!name || !email || !phone) {
            showError('Name, Email and Phone are required');
            return;
        }
        const payload = {
            name,
            customerName: getInputValue('customers-customer-name') || null,
            email,
            billingEmail: getInputValue('customers-billing-email') || null,
            phone,
            taxId: getInputValue('customers-tax-id') || null,
            vatNumber: getInputValue('customers-vat-number') || null,
            status: getInputValue('customers-status') || 'Active',
            preferredCurrency: getInputValue('customers-preferred-currency') || 'EUR',
            creditLimit: getNumberValue('customers-credit-limit'),
            preferredPaymentMethod: getInputValue('customers-preferred-payment-method') || null,
            notes: getTextAreaValue('customers-notes') || null,
            isCompany: getCheckboxValue('customers-is-company'),
            isActive: getCheckboxValue('customers-is-active'),
            allowCurrencyOverride: getCheckboxValue('customers-allow-currency-override'),
        };
        const response = editingId
            ? await apiRequest(`${getApiBaseUrl()}/Customers/${editingId}`, { method: 'PUT', body: JSON.stringify(payload) })
            : await apiRequest(`${getApiBaseUrl()}/Customers`, { method: 'POST', body: JSON.stringify(payload) });
        if (response.success) {
            hideModal('customers-edit-modal');
            showSuccess(editingId ? 'Customer updated successfully' : 'Customer created successfully');
            loadCustomers();
        }
        else {
            showError(response.message || 'Save failed');
        }
    }
    function openDelete(id, name) {
        pendingDeleteId = id;
        const deleteName = document.getElementById('customers-delete-name');
        if (deleteName) {
            deleteName.textContent = name;
        }
        showModal('customers-delete-modal');
    }
    async function doDelete() {
        if (!pendingDeleteId) {
            return;
        }
        const response = await apiRequest(`${getApiBaseUrl()}/Customers/${pendingDeleteId}`, { method: 'DELETE' });
        hideModal('customers-delete-modal');
        if (response.success) {
            showSuccess('Customer deleted successfully');
            if (currentPage > 1 && allCustomers.length === 1) {
                currentPage = currentPage - 1;
            }
            loadCustomers();
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
        const alert = document.getElementById('customers-alert-success');
        if (!alert) {
            return;
        }
        alert.textContent = message;
        alert.classList.remove('d-none');
        const errorAlert = document.getElementById('customers-alert-error');
        errorAlert === null || errorAlert === void 0 ? void 0 : errorAlert.classList.add('d-none');
        setTimeout(() => alert.classList.add('d-none'), 5000);
    }
    function showError(message) {
        const alert = document.getElementById('customers-alert-error');
        if (!alert) {
            return;
        }
        alert.textContent = message;
        alert.classList.remove('d-none');
        const successAlert = document.getElementById('customers-alert-success');
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
        const tableBody = document.getElementById('customers-table-body');
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
    function initializeCustomersPage() {
        var _a, _b, _c, _d;
        const page = document.getElementById('customers-page');
        if (!page || page.dataset.initialized === 'true') {
            return;
        }
        page.dataset.initialized = 'true';
        (_a = document.getElementById('customers-create')) === null || _a === void 0 ? void 0 : _a.addEventListener('click', openCreate);
        (_b = document.getElementById('customers-save')) === null || _b === void 0 ? void 0 : _b.addEventListener('click', saveCustomer);
        (_c = document.getElementById('customers-confirm-delete')) === null || _c === void 0 ? void 0 : _c.addEventListener('click', doDelete);
        bindTableActions();
        bindPagingControlsActions();
        (_d = document.getElementById('customers-page-size')) === null || _d === void 0 ? void 0 : _d.addEventListener('change', () => {
            currentPage = 1;
            loadCustomers();
        });
        loadCustomers();
    }
    function setupPageObserver() {
        initializeCustomersPage();
        if (document.body) {
            const observer = new MutationObserver(() => {
                const page = document.getElementById('customers-page');
                if (page && page.dataset.initialized !== 'true') {
                    initializeCustomersPage();
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
//# sourceMappingURL=customers.js.map
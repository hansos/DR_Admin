"use strict";
(() => {
    const couponTypeLabels = {
        0: 'Percentage',
        1: 'Fixed amount',
    };
    const appliesToLabels = {
        0: 'Setup fee',
        1: 'Recurring',
        2: 'Both',
        3: 'Total',
    };
    const recurrenceTypeLabels = {
        0: 'None',
        1: 'One time',
        2: 'Recurring years',
    };
    let coupons = [];
    let editingCouponId = null;
    let pendingDeleteCouponId = null;
    let usageRows = [];
    let usageCurrentPage = 1;
    let usagePageSize = 25;
    let usageTotalCount = 0;
    let usageTotalPages = 1;
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
    const apiRequest = async (endpoint, options = {}) => {
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
            const body = hasJson ? await response.json() : null;
            if (!response.ok) {
                const errorBody = (body ?? {});
                return {
                    success: false,
                    message: errorBody.message ?? errorBody.title ?? `Request failed with status ${response.status}`,
                };
            }
            const envelope = (body ?? {});
            return {
                success: envelope.success !== false,
                data: envelope.data ?? body,
                message: envelope.message,
            };
        }
        catch (error) {
            console.error('Coupons request failed', error);
            return {
                success: false,
                message: 'Network error. Please try again.',
            };
        }
    };
    const parseNumber = (value) => {
        if (typeof value === 'number' && Number.isFinite(value)) {
            return value;
        }
        if (typeof value === 'string' && value.trim() !== '') {
            const parsed = Number(value);
            if (Number.isFinite(parsed)) {
                return parsed;
            }
        }
        return 0;
    };
    const parseNullableNumber = (value) => {
        if (value === null || value === undefined || value === '') {
            return null;
        }
        const parsed = parseNumber(value);
        return Number.isFinite(parsed) ? parsed : null;
    };
    const parseString = (value) => typeof value === 'string' ? value : '';
    const parseBoolean = (value) => {
        if (typeof value === 'boolean') {
            return value;
        }
        if (typeof value === 'string') {
            return value.toLowerCase() === 'true';
        }
        return false;
    };
    const parseNumberList = (value) => {
        if (!Array.isArray(value)) {
            return null;
        }
        const parsed = value
            .map((item) => parseNumber(item))
            .filter((item) => Number.isFinite(item) && item > 0);
        return parsed.length ? parsed : null;
    };
    const esc = (text) => {
        const map = { '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#039;' };
        return (text || '').replace(/[&<>"']/g, (char) => map[char] ?? char);
    };
    const formatDate = (value) => {
        if (!value) {
            return '-';
        }
        const date = new Date(value);
        if (Number.isNaN(date.getTime())) {
            return value;
        }
        return date.toLocaleString();
    };
    const formatDateInput = (value) => {
        if (!value) {
            return '';
        }
        const date = new Date(value);
        if (Number.isNaN(date.getTime())) {
            return '';
        }
        const pad = (v) => String(v).padStart(2, '0');
        const year = date.getFullYear();
        const month = pad(date.getMonth() + 1);
        const day = pad(date.getDate());
        const hours = pad(date.getHours());
        const minutes = pad(date.getMinutes());
        return `${year}-${month}-${day}T${hours}:${minutes}`;
    };
    const formatMoney = (amount) => {
        if (!Number.isFinite(amount)) {
            return '-';
        }
        try {
            return new Intl.NumberFormat(undefined, {
                style: 'currency',
                currency: 'EUR',
                minimumFractionDigits: 2,
                maximumFractionDigits: 2,
            }).format(amount);
        }
        catch {
            return amount.toFixed(2);
        }
    };
    const showError = (message) => {
        const alert = document.getElementById('coupons-alert-error');
        if (!alert) {
            return;
        }
        alert.textContent = message;
        alert.classList.remove('d-none');
        document.getElementById('coupons-alert-success')?.classList.add('d-none');
    };
    const showSuccess = (message) => {
        const alert = document.getElementById('coupons-alert-success');
        if (!alert) {
            return;
        }
        alert.textContent = message;
        alert.classList.remove('d-none');
        document.getElementById('coupons-alert-error')?.classList.add('d-none');
        window.setTimeout(() => alert.classList.add('d-none'), 5000);
    };
    const hideModal = (id) => {
        const element = document.getElementById(id);
        const win = window;
        if (!element || !win.bootstrap) {
            return;
        }
        win.bootstrap.Modal.getInstance(element)?.hide();
    };
    const showModal = (id) => {
        const element = document.getElementById(id);
        const win = window;
        if (!element || !win.bootstrap) {
            return;
        }
        new win.bootstrap.Modal(element).show();
    };
    const normalizeCoupon = (item) => {
        const row = (item ?? {});
        return {
            id: parseNumber(row.id ?? row.Id),
            code: parseString(row.code ?? row.Code),
            name: parseString(row.name ?? row.Name),
            description: parseString(row.description ?? row.Description),
            type: parseNumber(row.type ?? row.Type),
            value: parseNumber(row.value ?? row.Value),
            appliesTo: parseNumber(row.appliesTo ?? row.AppliesTo),
            recurrenceType: parseNumber(row.recurrenceType ?? row.RecurrenceType),
            recurringYears: parseNullableNumber(row.recurringYears ?? row.RecurringYears),
            minimumAmount: parseNullableNumber(row.minimumAmount ?? row.MinimumAmount),
            maximumDiscount: parseNullableNumber(row.maximumDiscount ?? row.MaximumDiscount),
            validFrom: parseString(row.validFrom ?? row.ValidFrom),
            validUntil: parseString(row.validUntil ?? row.ValidUntil),
            maxUsages: parseNullableNumber(row.maxUsages ?? row.MaxUsages),
            maxUsagesPerCustomer: parseNullableNumber(row.maxUsagesPerCustomer ?? row.MaxUsagesPerCustomer),
            isActive: parseBoolean(row.isActive ?? row.IsActive),
            usageCount: parseNumber(row.usageCount ?? row.UsageCount),
            allowedServiceTypeIds: parseNumberList(row.allowedServiceTypeIds ?? row.AllowedServiceTypeIds),
            internalNotes: parseString(row.internalNotes ?? row.InternalNotes),
        };
    };
    const normalizeUsage = (item) => {
        const row = (item ?? {});
        return {
            id: parseNumber(row.id ?? row.Id),
            couponId: parseNumber(row.couponId ?? row.CouponId),
            couponCode: parseString(row.couponCode ?? row.CouponCode),
            couponName: parseString(row.couponName ?? row.CouponName),
            customerId: parseNumber(row.customerId ?? row.CustomerId),
            customerName: parseString(row.customerName ?? row.CustomerName),
            quoteId: parseNullableNumber(row.quoteId ?? row.QuoteId),
            orderId: parseNullableNumber(row.orderId ?? row.OrderId),
            discountAmount: parseNumber(row.discountAmount ?? row.DiscountAmount),
            usedAt: parseString(row.usedAt ?? row.UsedAt),
        };
    };
    const extractArray = (payload) => {
        if (Array.isArray(payload)) {
            return payload;
        }
        const obj = payload;
        if (Array.isArray(obj?.data)) {
            return obj.data;
        }
        if (Array.isArray(obj?.Data)) {
            return obj.Data;
        }
        const nestedData = obj?.data;
        if (Array.isArray(nestedData?.data)) {
            return nestedData.data;
        }
        if (Array.isArray(nestedData?.Data)) {
            return nestedData.Data;
        }
        return [];
    };
    const renderCouponsTable = () => {
        const tableBody = document.getElementById('coupons-table-body');
        if (!tableBody) {
            return;
        }
        if (!coupons.length) {
            tableBody.innerHTML = '<tr><td colspan="9" class="text-center text-muted">No coupons found.</td></tr>';
            return;
        }
        tableBody.innerHTML = coupons.map((coupon) => {
            const typeLabel = couponTypeLabels[coupon.type] ?? String(coupon.type);
            const recurrenceLabel = recurrenceTypeLabels[coupon.recurrenceType] ?? String(coupon.recurrenceType);
            const appliesToLabel = appliesToLabels[coupon.appliesTo] ?? String(coupon.appliesTo);
            const valueDisplay = coupon.type === 0 ? `${coupon.value}%` : formatMoney(coupon.value);
            return `
            <tr>
                <td>${coupon.id}</td>
                <td><code>${esc(coupon.code)}</code></td>
                <td>${esc(coupon.name)}</td>
                <td>${esc(typeLabel)}<div class="small text-muted">${esc(appliesToLabel)}</div></td>
                <td>${esc(valueDisplay)}</td>
                <td>${esc(formatDate(coupon.validFrom))}<br /><span class="text-muted small">to ${esc(formatDate(coupon.validUntil))}</span></td>
                <td>${coupon.usageCount}${coupon.maxUsages ? ` / ${coupon.maxUsages}` : ''}<div class="small text-muted">${esc(recurrenceLabel)}</div></td>
                <td><span class="badge bg-${coupon.isActive ? 'success' : 'secondary'}">${coupon.isActive ? 'Active' : 'Inactive'}</span></td>
                <td>
                    <div class="btn-group btn-group-sm">
                        <button class="btn btn-outline-primary" type="button" data-action="edit" data-id="${coupon.id}"><i class="bi bi-pencil"></i></button>
                        <button class="btn btn-outline-danger" type="button" data-action="delete" data-id="${coupon.id}" data-name="${esc(coupon.code)}"><i class="bi bi-trash"></i></button>
                    </div>
                </td>
            </tr>`;
        }).join('');
    };
    const updateUsageCouponFilterOptions = () => {
        const select = document.getElementById('coupons-usages-filter-coupon');
        if (!select) {
            return;
        }
        const currentValue = select.value;
        const options = coupons
            .slice()
            .sort((a, b) => a.code.localeCompare(b.code))
            .map((coupon) => `<option value="${coupon.id}">${esc(coupon.code)} - ${esc(coupon.name)}</option>`)
            .join('');
        select.innerHTML = `<option value="">Any coupon</option>${options}`;
        select.value = coupons.some((c) => String(c.id) === currentValue) ? currentValue : '';
    };
    const loadCoupons = async () => {
        const tableBody = document.getElementById('coupons-table-body');
        if (tableBody) {
            tableBody.innerHTML = '<tr><td colspan="9" class="text-center"><div class="spinner-border text-primary"></div></td></tr>';
        }
        const response = await apiRequest(`${getApiBaseUrl()}/Coupons`, { method: 'GET' });
        if (!response.success) {
            showError(response.message ?? 'Failed to load coupons.');
            if (tableBody) {
                tableBody.innerHTML = '<tr><td colspan="9" class="text-center text-danger">Failed to load coupons</td></tr>';
            }
            return;
        }
        coupons = extractArray(response.data).map(normalizeCoupon);
        renderCouponsTable();
        updateUsageCouponFilterOptions();
    };
    const parseAllowedServiceTypeIds = (raw) => {
        const trimmed = raw.trim();
        if (!trimmed) {
            return null;
        }
        const parts = trimmed.split(',').map((part) => part.trim()).filter((part) => part.length > 0);
        if (!parts.length) {
            return null;
        }
        const parsed = [];
        for (const part of parts) {
            const value = Number(part);
            if (!Number.isInteger(value) || value <= 0) {
                throw new Error('Allowed service type IDs must be a comma-separated list of positive integers.');
            }
            parsed.push(value);
        }
        return Array.from(new Set(parsed));
    };
    const getInputValue = (id) => {
        const element = document.getElementById(id);
        return (element?.value ?? '').trim();
    };
    const getNullableNumberFromInput = (id) => {
        const raw = getInputValue(id);
        if (!raw) {
            return null;
        }
        const parsed = Number(raw);
        return Number.isFinite(parsed) ? parsed : null;
    };
    const getRequiredNumber = (id, fieldName) => {
        const value = getNullableNumberFromInput(id);
        if (value === null) {
            throw new Error(`${fieldName} is required.`);
        }
        return value;
    };
    const toUtcIsoFromDateInput = (value) => {
        const date = new Date(value);
        if (Number.isNaN(date.getTime())) {
            throw new Error('Invalid date value.');
        }
        return date.toISOString();
    };
    const readCouponFormPayload = () => {
        const code = getInputValue('coupons-code');
        const name = getInputValue('coupons-name');
        const validFromRaw = getInputValue('coupons-valid-from');
        const validUntilRaw = getInputValue('coupons-valid-until');
        if (!code) {
            throw new Error('Code is required.');
        }
        if (!name) {
            throw new Error('Name is required.');
        }
        if (!validFromRaw || !validUntilRaw) {
            throw new Error('Valid from and valid until are required.');
        }
        const recurrenceType = getRequiredNumber('coupons-recurrence-type', 'Recurrence type');
        const recurringYears = getNullableNumberFromInput('coupons-recurring-years');
        if (recurrenceType === 2 && (recurringYears === null || recurringYears <= 0)) {
            throw new Error('Recurring years is required when recurrence type is recurring years.');
        }
        const payload = {
            code,
            name,
            description: getInputValue('coupons-description'),
            type: getRequiredNumber('coupons-type', 'Type'),
            value: getRequiredNumber('coupons-value', 'Value'),
            appliesTo: getRequiredNumber('coupons-applies-to', 'Applies To'),
            recurrenceType,
            recurringYears: recurrenceType === 2 ? recurringYears : null,
            minimumAmount: getNullableNumberFromInput('coupons-minimum-amount'),
            maximumDiscount: getNullableNumberFromInput('coupons-maximum-discount'),
            validFrom: toUtcIsoFromDateInput(validFromRaw),
            validUntil: toUtcIsoFromDateInput(validUntilRaw),
            maxUsages: getNullableNumberFromInput('coupons-max-usages'),
            maxUsagesPerCustomer: getNullableNumberFromInput('coupons-max-usages-per-customer'),
            isActive: document.getElementById('coupons-is-active')?.checked ?? false,
            allowedServiceTypeIds: parseAllowedServiceTypeIds(getInputValue('coupons-allowed-service-type-ids')),
            internalNotes: getInputValue('coupons-internal-notes'),
        };
        if (new Date(payload.validUntil) < new Date(payload.validFrom)) {
            throw new Error('Valid until must be after valid from.');
        }
        return payload;
    };
    const updateRecurringYearsAvailability = () => {
        const recurrenceType = Number(getInputValue('coupons-recurrence-type') || '0');
        const recurringYears = document.getElementById('coupons-recurring-years');
        if (!recurringYears) {
            return;
        }
        const enabled = recurrenceType === 2;
        recurringYears.disabled = !enabled;
        if (!enabled) {
            recurringYears.value = '';
        }
    };
    const openCreate = () => {
        editingCouponId = null;
        const title = document.getElementById('coupons-modal-title');
        if (title) {
            title.textContent = 'New Coupon';
        }
        const form = document.getElementById('coupons-form');
        form?.reset();
        const validFrom = document.getElementById('coupons-valid-from');
        const validUntil = document.getElementById('coupons-valid-until');
        const now = new Date();
        const plus30 = new Date(now.getTime() + 30 * 24 * 60 * 60 * 1000);
        if (validFrom) {
            validFrom.value = formatDateInput(now.toISOString());
        }
        if (validUntil) {
            validUntil.value = formatDateInput(plus30.toISOString());
        }
        const isActive = document.getElementById('coupons-is-active');
        if (isActive) {
            isActive.checked = true;
        }
        updateRecurringYearsAvailability();
        showModal('coupons-edit-modal');
    };
    const openEdit = (couponId) => {
        const coupon = coupons.find((item) => item.id === couponId);
        if (!coupon) {
            return;
        }
        editingCouponId = couponId;
        const title = document.getElementById('coupons-modal-title');
        if (title) {
            title.textContent = `Edit Coupon: ${coupon.code}`;
        }
        document.getElementById('coupons-code').value = coupon.code;
        document.getElementById('coupons-name').value = coupon.name;
        document.getElementById('coupons-description').value = coupon.description || '';
        document.getElementById('coupons-type').value = String(coupon.type);
        document.getElementById('coupons-value').value = String(coupon.value);
        document.getElementById('coupons-applies-to').value = String(coupon.appliesTo);
        document.getElementById('coupons-recurrence-type').value = String(coupon.recurrenceType);
        document.getElementById('coupons-recurring-years').value = coupon.recurringYears !== null ? String(coupon.recurringYears) : '';
        document.getElementById('coupons-minimum-amount').value = coupon.minimumAmount !== null ? String(coupon.minimumAmount) : '';
        document.getElementById('coupons-maximum-discount').value = coupon.maximumDiscount !== null ? String(coupon.maximumDiscount) : '';
        document.getElementById('coupons-valid-from').value = formatDateInput(coupon.validFrom);
        document.getElementById('coupons-valid-until').value = formatDateInput(coupon.validUntil);
        document.getElementById('coupons-max-usages').value = coupon.maxUsages !== null ? String(coupon.maxUsages) : '';
        document.getElementById('coupons-max-usages-per-customer').value = coupon.maxUsagesPerCustomer !== null ? String(coupon.maxUsagesPerCustomer) : '';
        document.getElementById('coupons-is-active').checked = coupon.isActive;
        document.getElementById('coupons-allowed-service-type-ids').value = coupon.allowedServiceTypeIds?.join(',') ?? '';
        document.getElementById('coupons-internal-notes').value = coupon.internalNotes || '';
        updateRecurringYearsAvailability();
        showModal('coupons-edit-modal');
    };
    const saveCoupon = async () => {
        try {
            const payload = readCouponFormPayload();
            const endpoint = editingCouponId
                ? `${getApiBaseUrl()}/Coupons/${editingCouponId}`
                : `${getApiBaseUrl()}/Coupons`;
            const method = editingCouponId ? 'PUT' : 'POST';
            const response = await apiRequest(endpoint, {
                method,
                body: JSON.stringify(payload),
            });
            if (!response.success) {
                showError(response.message ?? 'Failed to save coupon.');
                return;
            }
            hideModal('coupons-edit-modal');
            showSuccess(editingCouponId ? 'Coupon updated successfully.' : 'Coupon created successfully.');
            await loadCoupons();
            await loadUsages();
        }
        catch (error) {
            showError(error instanceof Error ? error.message : 'Failed to save coupon.');
        }
    };
    const openDelete = (couponId, couponCode) => {
        pendingDeleteCouponId = couponId;
        const target = document.getElementById('coupons-delete-name');
        if (target) {
            target.textContent = couponCode;
        }
        showModal('coupons-delete-modal');
    };
    const deleteCoupon = async () => {
        if (!pendingDeleteCouponId) {
            return;
        }
        const response = await apiRequest(`${getApiBaseUrl()}/Coupons/${pendingDeleteCouponId}`, {
            method: 'DELETE',
        });
        hideModal('coupons-delete-modal');
        if (!response.success) {
            showError(response.message ?? 'Failed to delete coupon.');
            return;
        }
        showSuccess('Coupon deleted successfully.');
        pendingDeleteCouponId = null;
        await loadCoupons();
        await loadUsages();
    };
    const buildUsageUrl = () => {
        const couponId = getInputValue('coupons-usages-filter-coupon');
        const customerId = getInputValue('coupons-usages-filter-customerid');
        const params = new URLSearchParams();
        params.set('pageNumber', String(usageCurrentPage));
        params.set('pageSize', String(usagePageSize));
        if (couponId) {
            params.set('couponId', couponId);
        }
        if (customerId) {
            params.set('customerId', customerId);
        }
        return `${getApiBaseUrl()}/Coupons/usages?${params.toString()}`;
    };
    const extractPagedUsages = (payload) => {
        const raw = (payload ?? {});
        const rows = Array.isArray(raw.Data)
            ? raw.Data
            : Array.isArray(raw.data)
                ? raw.data
                : [];
        const page = parseNumber(raw.CurrentPage ?? raw.currentPage) || usageCurrentPage;
        const pageSize = parseNumber(raw.PageSize ?? raw.pageSize) || usagePageSize;
        const totalCount = parseNumber(raw.TotalCount ?? raw.totalCount);
        const totalPages = parseNumber(raw.TotalPages ?? raw.totalPages) || Math.max(1, Math.ceil((totalCount || 1) / Math.max(pageSize, 1)));
        return { rows, page, pageSize, totalCount, totalPages };
    };
    const renderUsageTable = () => {
        const tableBody = document.getElementById('coupons-usages-table-body');
        if (!tableBody) {
            return;
        }
        if (!usageRows.length) {
            tableBody.innerHTML = '<tr><td colspan="7" class="text-center text-muted">No coupon usage entries found.</td></tr>';
            return;
        }
        tableBody.innerHTML = usageRows.map((usage) => `
            <tr>
                <td>${usage.id}</td>
                <td>${esc(formatDate(usage.usedAt))}</td>
                <td><code>${esc(usage.couponCode || '-')}</code><div class="small text-muted">${esc(usage.couponName || '')}</div></td>
                <td>#${usage.customerId}<div class="small text-muted">${esc(usage.customerName || '')}</div></td>
                <td>${usage.quoteId ?? '-'}</td>
                <td>${usage.orderId ?? '-'}</td>
                <td>${esc(formatMoney(usage.discountAmount))}</td>
            </tr>
        `).join('');
    };
    const renderUsagePagingControls = () => {
        const list = document.getElementById('coupons-usages-paging-controls-list');
        if (!list) {
            return;
        }
        if (!usageTotalCount || usageTotalPages <= 1) {
            list.innerHTML = '';
            return;
        }
        const makeItem = (label, page, disabled, active = false) => {
            const cls = `page-item${disabled ? ' disabled' : ''}${active ? ' active' : ''}`;
            const dataPage = disabled ? '' : ` data-page="${page}"`;
            const ariaCurrent = active ? ' aria-current="page"' : '';
            return `<li class="${cls}"><a class="page-link" href="#"${dataPage}${ariaCurrent}>${label}</a></li>`;
        };
        let html = '';
        html += makeItem('Previous', usageCurrentPage - 1, usageCurrentPage <= 1);
        const pages = new Set([1, usageTotalPages, usageCurrentPage - 1, usageCurrentPage, usageCurrentPage + 1]);
        const sortedPages = Array.from(pages).filter((p) => p >= 1 && p <= usageTotalPages).sort((a, b) => a - b);
        let previousPage = 0;
        for (const page of sortedPages) {
            if (previousPage && page - previousPage > 1) {
                html += '<li class="page-item disabled"><span class="page-link">…</span></li>';
            }
            html += makeItem(String(page), page, false, page === usageCurrentPage);
            previousPage = page;
        }
        html += makeItem('Next', usageCurrentPage + 1, usageCurrentPage >= usageTotalPages);
        list.innerHTML = html;
    };
    const renderUsagePagination = () => {
        const info = document.getElementById('coupons-usages-pagination-info');
        if (!info) {
            return;
        }
        if (!usageTotalCount) {
            info.textContent = 'Showing 0 of 0';
            renderUsagePagingControls();
            return;
        }
        const start = (usageCurrentPage - 1) * usagePageSize + 1;
        const end = Math.min(usageCurrentPage * usagePageSize, usageTotalCount);
        info.textContent = `Showing ${start}-${end} of ${usageTotalCount}`;
        renderUsagePagingControls();
    };
    const loadUsages = async () => {
        const tableBody = document.getElementById('coupons-usages-table-body');
        if (tableBody) {
            tableBody.innerHTML = '<tr><td colspan="7" class="text-center"><div class="spinner-border text-primary"></div></td></tr>';
        }
        const response = await apiRequest(buildUsageUrl(), { method: 'GET' });
        if (!response.success) {
            showError(response.message ?? 'Failed to load coupon usage history.');
            if (tableBody) {
                tableBody.innerHTML = '<tr><td colspan="7" class="text-center text-danger">Failed to load usage history</td></tr>';
            }
            return;
        }
        const paged = extractPagedUsages(response.data);
        usageRows = paged.rows.map(normalizeUsage);
        usageCurrentPage = paged.page;
        usagePageSize = paged.pageSize;
        usageTotalCount = paged.totalCount;
        usageTotalPages = paged.totalPages;
        const pageSizeSelect = document.getElementById('coupons-usages-page-size');
        if (pageSizeSelect) {
            pageSizeSelect.value = String(usagePageSize);
        }
        renderUsageTable();
        renderUsagePagination();
    };
    const applyUsageFilters = () => {
        usageCurrentPage = 1;
        void loadUsages();
    };
    const resetUsageFilters = () => {
        const coupon = document.getElementById('coupons-usages-filter-coupon');
        const customer = document.getElementById('coupons-usages-filter-customerid');
        if (coupon) {
            coupon.value = '';
        }
        if (customer) {
            customer.value = '';
        }
        usageCurrentPage = 1;
        void loadUsages();
    };
    const bindEvents = () => {
        document.getElementById('coupons-create')?.addEventListener('click', openCreate);
        document.getElementById('coupons-save')?.addEventListener('click', () => { void saveCoupon(); });
        document.getElementById('coupons-confirm-delete')?.addEventListener('click', () => { void deleteCoupon(); });
        document.getElementById('coupons-recurrence-type')?.addEventListener('change', updateRecurringYearsAvailability);
        document.getElementById('coupons-usages-apply')?.addEventListener('click', applyUsageFilters);
        document.getElementById('coupons-usages-reset')?.addEventListener('click', resetUsageFilters);
        document.getElementById('coupons-usages-page-size')?.addEventListener('change', () => {
            const value = Number(getInputValue('coupons-usages-page-size'));
            if (Number.isFinite(value) && value > 0) {
                usagePageSize = value;
            }
            usageCurrentPage = 1;
            void loadUsages();
        });
        document.getElementById('coupons-table-body')?.addEventListener('click', (event) => {
            const target = event.target;
            const button = target.closest('button[data-action]');
            if (!button) {
                return;
            }
            const couponId = Number(button.dataset.id);
            if (!Number.isFinite(couponId) || couponId <= 0) {
                return;
            }
            const action = button.dataset.action;
            if (action === 'edit') {
                openEdit(couponId);
                return;
            }
            if (action === 'delete') {
                openDelete(couponId, button.dataset.name ?? `#${couponId}`);
            }
        });
        document.getElementById('coupons-usages-paging-controls')?.addEventListener('click', (event) => {
            const target = event.target;
            const link = target.closest('a[data-page]');
            if (!link) {
                return;
            }
            event.preventDefault();
            const page = Number(link.dataset.page);
            if (!Number.isFinite(page) || page < 1 || page > usageTotalPages) {
                return;
            }
            usageCurrentPage = page;
            void loadUsages();
        });
    };
    const initializeCouponsPage = () => {
        const page = document.getElementById('coupons-page');
        if (!page || page.dataset.initialized === 'true') {
            return;
        }
        page.dataset.initialized = 'true';
        bindEvents();
        void loadCoupons().then(loadUsages);
    };
    const setupPageObserver = () => {
        initializeCouponsPage();
        if (!document.body) {
            return;
        }
        const observer = new MutationObserver(() => {
            const page = document.getElementById('coupons-page');
            if (page && page.dataset.initialized !== 'true') {
                initializeCouponsPage();
            }
        });
        observer.observe(document.body, { childList: true, subtree: true });
    };
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', setupPageObserver);
    }
    else {
        setupPageObserver();
    }
})();
//# sourceMappingURL=coupons.js.map
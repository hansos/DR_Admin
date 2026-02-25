"use strict";
// @ts-nocheck
(function () {
    function getApiBaseUrl() {
        return window.AppSettings?.apiBaseUrl ?? '';
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
    function getAuthToken() {
        const auth = window.Auth;
        if (auth?.getToken) {
            return auth.getToken();
        }
        return sessionStorage.getItem('rp_authToken');
    }
    async function apiRequest(endpoint, options = {}) {
        try {
            const headers = {
                'Content-Type': 'application/json',
                ...options.headers,
            };
            const authToken = getAuthToken();
            if (authToken) {
                headers['Authorization'] = `Bearer ${authToken}`;
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
                    message: (data && (data.message ?? data.title)) || `Request failed with status ${response.status}`,
                };
            }
            return {
                success: data?.success !== false,
                data: data?.data ?? data,
                message: data?.message,
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
        if (Array.isArray(raw)) {
            return { items: raw, meta: null };
        }
        const candidates = [raw, raw?.data, raw?.Data, raw?.data?.data, raw?.data?.Data];
        const items = (Array.isArray(raw?.Data) && raw.Data) ||
            (Array.isArray(raw?.data) && raw.data) ||
            (Array.isArray(raw?.data?.Data) && raw.data.Data) ||
            (Array.isArray(raw?.data?.data) && raw.data.data) ||
            (Array.isArray(raw?.Data?.Data) && raw.Data.Data) ||
            [];
        const meta = candidates.find((c) => c && typeof c === 'object' && (c.totalCount !== undefined || c.TotalCount !== undefined ||
            c.totalPages !== undefined || c.TotalPages !== undefined ||
            c.currentPage !== undefined || c.CurrentPage !== undefined ||
            c.pageSize !== undefined || c.PageSize !== undefined)) ?? null;
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
        const el = document.getElementById('domains-page-size');
        const parsed = Number((el?.value ?? '').trim());
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
        return {
            id: item.id ?? item.Id ?? 0,
            customerId: item.customerId ?? item.CustomerId ?? 0,
            serviceId: item.serviceId ?? item.ServiceId ?? 0,
            name: item.name ?? item.Name ?? '',
            providerId: item.providerId ?? item.ProviderId ?? 0,
            status: item.status ?? item.Status ?? '',
            registrationDate: item.registrationDate ?? item.RegistrationDate ?? '',
            expirationDate: item.expirationDate ?? item.ExpirationDate ?? '',
            createdAt: item.createdAt ?? item.CreatedAt ?? null,
            updatedAt: item.updatedAt ?? item.UpdatedAt ?? null,
            customer: item.customer ?? item.Customer ?? null,
            registrar: item.registrar ?? item.Registrar ?? null,
        };
    }
    function normalizeRegistrarOption(item) {
        return {
            id: item.id ?? item.Id ?? 0,
            name: item.name ?? item.Name ?? '',
            code: item.code ?? item.Code ?? null,
        };
    }
    async function loadDomains() {
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
        const meta = extracted.meta ?? raw;
        allDomains = extracted.items.map(normalizeItem);
        pageSize = meta?.pageSize ?? meta?.PageSize ?? raw?.pageSize ?? raw?.PageSize ?? pageSize;
        totalCount = meta?.totalCount ?? meta?.TotalCount ?? raw?.totalCount ?? raw?.TotalCount ?? allDomains.length;
        totalPages = meta?.totalPages ?? meta?.TotalPages ?? raw?.totalPages ?? raw?.TotalPages ?? Math.max(1, Math.ceil(totalCount / pageSize));
        currentPage = meta?.currentPage ?? meta?.CurrentPage ?? raw?.currentPage ?? raw?.CurrentPage ?? currentPage;
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
            : Array.isArray(response.data?.data)
                ? response.data.data
                : [];
        registrarOptions = rawItems.map(normalizeRegistrarOption);
        registrarOptions.sort((a, b) => a.name.localeCompare(b.name));
        const options = registrarOptions.map((registrar) => {
            const label = registrar.code ? `${registrar.name} (${registrar.code})` : registrar.name;
            return `<option value="${registrar.id}">${esc(label)}</option>`;
        }).join('');
        select.innerHTML = `<option value="">Select registrar</option>${options}`;
    }
    function openImport() {
        const select = document.getElementById('domains-import-registrar');
        if (select) {
            select.value = '';
        }
        setImportSummary('');
        setImportBusy(false);
        loadRegistrarsForImport();
        showModal('domains-import-modal');
    }
    function getSelectValue(id) {
        const el = document.getElementById(id);
        return (el?.value ?? '').trim();
    }
    function getCustomerName(domain) {
        const cust = domain.customer;
        return cust?.name ?? cust?.Name ?? '';
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
            totalExisting = existingData?.totalCount ?? existingData?.TotalCount ?? existingData?.domains?.length ?? existingData?.Domains?.length ?? null;
        }
        const response = await apiRequest(`${getApiBaseUrl()}/Registrars/${registrarId}/domains/download`, { method: 'POST' });
        setImportBusy(false);
        if (response.success) {
            const resultData = response.data;
            const importedCount = resultData?.count ?? resultData?.Count ?? resultData?.data?.count ?? resultData?.data?.Count ?? null;
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
    function getRegistrarName(domain) {
        const reg = domain.registrar;
        return reg?.name ?? reg?.Name ?? '';
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
                    <button class="btn btn-outline-secondary" type="button" data-action="details" data-id="${domain.id}" title="Details"><i class="bi bi-box-arrow-up-right"></i></button>
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
        form?.reset();
        setInputValue('domains-status', 'Active');
        setDateTimeLocalValue('domains-registration-date', new Date());
        showModal('domains-edit-modal');
    }
    function openEdit(id) {
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
        setInputValue('domains-provider-id', String(domain.providerId ?? 0));
        setInputValue('domains-customer-id', String(domain.customerId ?? 0));
        setInputValue('domains-service-id', String(domain.serviceId ?? 0));
        setDateTimeLocalValue('domains-registration-date', domain.registrationDate ? new Date(domain.registrationDate) : null);
        setDateTimeLocalValue('domains-expiration-date', domain.expirationDate ? new Date(domain.expirationDate) : null);
        showModal('domains-edit-modal');
    }
    function setInputValue(id, value) {
        const el = document.getElementById(id);
        if (el) {
            el.value = value ?? '';
        }
    }
    function getInputValue(id) {
        const el = document.getElementById(id);
        return (el?.value ?? '').trim();
    }
    function getNumberValue(id) {
        const raw = getInputValue(id);
        const parsed = Number(raw);
        return Number.isFinite(parsed) ? parsed : 0;
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
            if (button.dataset.action === 'details') {
                openDetails(id);
                return;
            }
            if (button.dataset.action === 'delete') {
                openDelete(id, button.dataset.name ?? '');
            }
        });
    }
    function openDetails(id) {
        window.location.href = `/domains/details?id=${encodeURIComponent(String(id))}`;
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
        errorAlert?.classList.add('d-none');
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
        successAlert?.classList.add('d-none');
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
        modal?.hide();
    }
    function initializeDomainsPage() {
        const page = document.getElementById('domains-page');
        if (!page || page.dataset.initialized === 'true') {
            return;
        }
        page.dataset.initialized = 'true';
        document.getElementById('domains-create')?.addEventListener('click', openCreate);
        document.getElementById('domains-save')?.addEventListener('click', saveDomain);
        document.getElementById('domains-import')?.addEventListener('click', openImport);
        document.getElementById('domains-import-confirm')?.addEventListener('click', importDomains);
        document.getElementById('domains-confirm-delete')?.addEventListener('click', doDelete);
        bindTableActions();
        bindPagingControlsActions();
        document.getElementById('domains-page-size')?.addEventListener('change', () => {
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
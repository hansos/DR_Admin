"use strict";
// @ts-nocheck
(function () {
    function getApiBaseUrl() {
        return window.AppSettings?.apiBaseUrl ?? '';
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
        const el = document.getElementById('registrars-page-size');
        const parsed = Number((el?.value ?? '').trim());
        if (Number.isFinite(parsed) && parsed > 0) {
            pageSize = parsed;
        }
    }
    function normalizeRegistrar(item) {
        return {
            id: item.id ?? item.Id ?? 0,
            name: item.name ?? item.Name ?? '',
            code: item.code ?? item.Code ?? '',
            isActive: item.isActive ?? item.IsActive ?? false,
            contactEmail: item.contactEmail ?? item.ContactEmail ?? null,
            contactPhone: item.contactPhone ?? item.ContactPhone ?? null,
            website: item.website ?? item.Website ?? null,
            notes: item.notes ?? item.Notes ?? null,
            isDefault: item.isDefault ?? item.IsDefault ?? false,
            createdAt: item.createdAt ?? item.CreatedAt ?? null,
            updatedAt: item.updatedAt ?? item.UpdatedAt ?? null,
        };
    }
    async function loadRegistrars() {
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
            : Array.isArray(response.data?.data)
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
        form?.reset();
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
            el.value = value ?? '';
        }
    }
    function setTextAreaValue(id, value) {
        const el = document.getElementById(id);
        if (el) {
            el.value = value ?? '';
        }
    }
    function setCheckboxValue(id, value) {
        const el = document.getElementById(id);
        if (el) {
            el.checked = value;
        }
    }
    function getInputValue(id) {
        const el = document.getElementById(id);
        return (el?.value ?? '').trim();
    }
    function getTextAreaValue(id) {
        const el = document.getElementById(id);
        return (el?.value ?? '').trim();
    }
    function getCheckboxValue(id) {
        const el = document.getElementById(id);
        return !!el?.checked;
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
        errorAlert?.classList.add('d-none');
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
    function bindTableActions() {
        const tableBody = document.getElementById('registrars-table-body');
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
    function initializeRegistrarsPage() {
        const page = document.getElementById('registrars-page');
        if (!page || page.dataset.initialized === 'true') {
            return;
        }
        page.dataset.initialized = 'true';
        document.getElementById('registrars-create')?.addEventListener('click', openCreate);
        document.getElementById('registrars-save')?.addEventListener('click', saveRegistrar);
        document.getElementById('registrars-confirm-delete')?.addEventListener('click', doDelete);
        bindTableActions();
        bindPagingControlsActions();
        document.getElementById('registrars-page-size')?.addEventListener('change', () => {
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
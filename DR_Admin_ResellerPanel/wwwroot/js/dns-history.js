"use strict";
(() => {
    let allItems = [];
    let filteredItems = [];
    let currentPage = 1;
    let pageSize = 25;
    let totalCount = 0;
    let totalPages = 1;
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
    const esc = (text) => {
        const map = { '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#039;' };
        return (text || '').replace(/[&<>"']/g, (char) => map[char] ?? char);
    };
    const parseString = (value) => typeof value === 'string' ? value : '';
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
    const parseDateTimeLocalAsIso = (id) => {
        const input = document.getElementById(id);
        const raw = (input?.value ?? '').trim();
        if (!raw) {
            return null;
        }
        const parsed = new Date(raw);
        return Number.isNaN(parsed.getTime()) ? null : parsed.toISOString();
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
            console.error('DNS history request failed', error);
            return {
                success: false,
                message: 'Network error. Please try again.',
            };
        }
    };
    const extractList = (payload) => {
        if (Array.isArray(payload)) {
            return payload;
        }
        const objectPayload = payload;
        if (Array.isArray(objectPayload?.items)) {
            return objectPayload.items;
        }
        if (Array.isArray(objectPayload?.Items)) {
            return objectPayload.Items;
        }
        if (Array.isArray(objectPayload?.data)) {
            return objectPayload.data;
        }
        if (Array.isArray(objectPayload?.Data)) {
            return objectPayload.Data;
        }
        return [];
    };
    const normalizeItem = (item) => {
        const row = (item ?? {});
        return {
            id: parseNumber(row.id ?? row.Id),
            registeredDomainId: parseNumber(row.registeredDomainId ?? row.RegisteredDomainId),
            domainName: parseString(row.domainName ?? row.DomainName),
            actionType: (row.actionType ?? row.ActionType ?? ''),
            action: parseString(row.action ?? row.Action),
            details: parseString(row.details ?? row.Details) || null,
            occurredAt: parseString(row.occurredAt ?? row.OccurredAt),
            sourceEntityType: parseString(row.sourceEntityType ?? row.SourceEntityType) || null,
            sourceEntityId: parseNumber(row.sourceEntityId ?? row.SourceEntityId) || null,
            performedByUserId: parseNumber(row.performedByUserId ?? row.PerformedByUserId) || null,
        };
    };
    const showError = (message) => {
        const alert = document.getElementById('dns-history-alert-error');
        if (!alert) {
            return;
        }
        alert.textContent = message;
        alert.classList.remove('d-none');
        document.getElementById('dns-history-alert-success')?.classList.add('d-none');
    };
    const hideError = () => {
        document.getElementById('dns-history-alert-error')?.classList.add('d-none');
    };
    const loadPageSizeFromUi = () => {
        const select = document.getElementById('dns-history-page-size');
        const parsed = Number(select?.value ?? '25');
        if (Number.isFinite(parsed) && parsed > 0) {
            pageSize = parsed;
        }
    };
    const getPagedItems = () => {
        totalCount = filteredItems.length;
        totalPages = Math.max(1, Math.ceil(totalCount / pageSize));
        if (currentPage > totalPages) {
            currentPage = totalPages;
        }
        const start = (currentPage - 1) * pageSize;
        return filteredItems.slice(start, start + pageSize);
    };
    const renderTable = () => {
        const tableBody = document.getElementById('dns-history-table-body');
        if (!tableBody) {
            return;
        }
        const paged = getPagedItems();
        if (!paged.length) {
            tableBody.innerHTML = '<tr><td colspan="6" class="text-center text-muted">No DNS changes found.</td></tr>';
            return;
        }
        tableBody.innerHTML = paged.map((item) => {
            const source = item.sourceEntityType
                ? `${item.sourceEntityType}${item.sourceEntityId ? ` #${item.sourceEntityId}` : ''}`
                : '-';
            return `
                <tr>
                    <td>${esc(formatDate(item.occurredAt))}</td>
                    <td><a href="/domains/details?id=${encodeURIComponent(String(item.registeredDomainId))}"><code>${esc(item.domainName || `#${item.registeredDomainId}`)}</code></a></td>
                    <td>${esc(item.action || 'DNS change')}</td>
                    <td>${esc(item.details || '-')}</td>
                    <td>${esc(source)}</td>
                    <td>${item.performedByUserId ? `#${item.performedByUserId}` : '-'}</td>
                </tr>
            `;
        }).join('');
    };
    const renderPagination = () => {
        const info = document.getElementById('dns-history-pagination-info');
        const list = document.getElementById('dns-history-paging-controls-list');
        if (!info || !list) {
            return;
        }
        if (!totalCount) {
            info.textContent = 'Showing 0 of 0';
            list.innerHTML = '';
            return;
        }
        const start = (currentPage - 1) * pageSize + 1;
        const end = Math.min(currentPage * pageSize, totalCount);
        info.textContent = `Showing ${start}-${end} of ${totalCount}`;
        if (totalPages <= 1) {
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
        const pages = new Set();
        pages.add(1);
        if (totalPages >= 2) {
            pages.add(2);
            pages.add(totalPages - 1);
        }
        pages.add(totalPages);
        for (let page = currentPage - 1; page <= currentPage + 1; page += 1) {
            if (page >= 1 && page <= totalPages) {
                pages.add(page);
            }
        }
        const sortedPages = Array.from(pages)
            .filter((page) => page >= 1 && page <= totalPages)
            .sort((a, b) => a - b);
        let html = '';
        html += makeItem('Previous', currentPage - 1, currentPage <= 1);
        let lastPage = 0;
        for (const page of sortedPages) {
            if (lastPage > 0 && page - lastPage > 1) {
                html += '<li class="page-item disabled"><span class="page-link">…</span></li>';
            }
            html += makeItem(String(page), page, false, page === currentPage);
            lastPage = page;
        }
        html += makeItem('Next', currentPage + 1, currentPage >= totalPages);
        list.innerHTML = html;
    };
    const updateView = () => {
        loadPageSizeFromUi();
        renderTable();
        renderPagination();
    };
    const applyFilters = () => {
        const domainName = document.getElementById('dns-history-filter-domain-name')?.value.trim() ?? '';
        const occurredFrom = parseDateTimeLocalAsIso('dns-history-filter-occurred-from');
        const occurredTo = parseDateTimeLocalAsIso('dns-history-filter-occurred-to');
        void loadDnsHistory(domainName, occurredFrom, occurredTo);
    };
    const resetFilters = () => {
        const domainNameInput = document.getElementById('dns-history-filter-domain-name');
        const occurredFromInput = document.getElementById('dns-history-filter-occurred-from');
        const occurredToInput = document.getElementById('dns-history-filter-occurred-to');
        if (domainNameInput) {
            domainNameInput.value = '';
        }
        if (occurredFromInput) {
            occurredFromInput.value = '';
        }
        if (occurredToInput) {
            occurredToInput.value = '';
        }
        void loadDnsHistory(null, null, null);
    };
    const loadDnsHistory = async (domainName, occurredFrom, occurredTo) => {
        const tableBody = document.getElementById('dns-history-table-body');
        if (tableBody) {
            tableBody.innerHTML = '<tr><td colspan="6" class="text-center"><div class="spinner-border text-primary"></div></td></tr>';
        }
        hideError();
        const query = new URLSearchParams();
        if (domainName && domainName.trim()) {
            query.set('domainName', domainName.trim());
        }
        if (occurredFrom) {
            query.set('occurredFrom', occurredFrom);
        }
        if (occurredTo) {
            query.set('occurredTo', occurredTo);
        }
        const suffix = query.toString() ? `?${query.toString()}` : '';
        const response = await apiRequest(`${getApiBaseUrl()}/RegisteredDomainHistories/dns-changes${suffix}`, { method: 'GET' });
        if (!response.success) {
            showError(response.message || 'Failed to load DNS history.');
            if (tableBody) {
                tableBody.innerHTML = '<tr><td colspan="6" class="text-center text-danger">Failed to load data</td></tr>';
            }
            return;
        }
        allItems = extractList(response.data)
            .map((item) => normalizeItem(item))
            .filter((item) => item.id > 0);
        filteredItems = [...allItems];
        currentPage = 1;
        updateView();
    };
    const bindEvents = () => {
        document.getElementById('dns-history-apply')?.addEventListener('click', applyFilters);
        document.getElementById('dns-history-reset')?.addEventListener('click', resetFilters);
        document.getElementById('dns-history-page-size')?.addEventListener('change', () => {
            currentPage = 1;
            updateView();
        });
        document.getElementById('dns-history-paging-controls')?.addEventListener('click', (event) => {
            const target = event.target;
            const link = target.closest('a[data-page]');
            if (!link) {
                return;
            }
            event.preventDefault();
            const page = Number(link.dataset.page ?? '0');
            if (!Number.isFinite(page) || page < 1 || page > totalPages) {
                return;
            }
            currentPage = page;
            updateView();
        });
    };
    const initializePage = () => {
        const page = document.getElementById('dns-history-page');
        if (!page || page.dataset.initialized === 'true') {
            return;
        }
        page.dataset.initialized = 'true';
        bindEvents();
        void loadDnsHistory(null, null, null);
    };
    const setupObserver = () => {
        initializePage();
        if (document.body) {
            const observer = new MutationObserver(() => {
                const page = document.getElementById('dns-history-page');
                if (page && page.dataset.initialized !== 'true') {
                    initializePage();
                }
            });
            observer.observe(document.body, { childList: true, subtree: true });
        }
    };
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', setupObserver);
    }
    else {
        setupObserver();
    }
})();
//# sourceMappingURL=dns-history.js.map
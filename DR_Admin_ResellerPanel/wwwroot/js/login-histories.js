"use strict";
// @ts-nocheck
(function () {
    function getApiBaseUrl() {
        const baseUrl = window.AppSettings?.apiBaseUrl;
        if (!baseUrl) {
            const fallback = window.location.protocol === 'https:'
                ? 'https://localhost:7201/api/v1'
                : 'http://localhost:5133/api/v1';
            return fallback;
        }
        return baseUrl;
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
                data: data,
                message: data?.message,
            };
        }
        catch (error) {
            console.error('Login histories request failed', error);
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
        // Common envelopes:
        // 1) PagedResult: { Data: [...], TotalCount, CurrentPage, TotalPages, PageSize }
        // 2) { data: { Data: [...], TotalCount, ... } }
        // 3) { data: [...], totalCount, currentPage, ... }
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
    let allLoginHistories = [];
    let currentPage = 1;
    let pageSize = 25;
    let totalCount = 0;
    let totalPages = 1;
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
        // Ensure Page 1 and Page 2 always show (when they exist), plus a small window around current.
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
    function getNumberValue(id) {
        const el = document.getElementById(id);
        if (!el) {
            return null;
        }
        const raw = (el.value ?? '').trim();
        if (!raw) {
            return null;
        }
        const value = Number(raw);
        return Number.isFinite(value) ? value : null;
    }
    function getSelectValue(id) {
        const el = document.getElementById(id);
        return (el?.value ?? '').trim();
    }
    function getDateValue(id) {
        const el = document.getElementById(id);
        const raw = (el?.value ?? '').trim();
        if (!raw) {
            return null;
        }
        const date = new Date(raw);
        if (Number.isNaN(date.getTime())) {
            return null;
        }
        return date.toISOString();
    }
    function loadPageSizeFromUi() {
        const value = getSelectValue('login-histories-page-size');
        const parsed = Number(value);
        if (Number.isFinite(parsed) && parsed > 0) {
            pageSize = parsed;
        }
    }
    function buildPagedUrl() {
        const userId = getNumberValue('login-histories-filter-userid');
        const success = getSelectValue('login-histories-filter-success');
        const from = getDateValue('login-histories-filter-from');
        const to = getDateValue('login-histories-filter-to');
        const params = new URLSearchParams();
        params.set('pageNumber', String(currentPage));
        params.set('pageSize', String(pageSize));
        if (userId !== null) {
            params.set('userId', String(userId));
        }
        if (success === 'true' || success === 'false') {
            params.set('isSuccessful', success);
        }
        if (from) {
            params.set('from', from);
        }
        if (to) {
            params.set('to', to);
        }
        return `${getApiBaseUrl()}/LoginHistories?${params.toString()}`;
    }
    function normalizeItem(item) {
        return {
            id: item.id ?? item.Id ?? 0,
            userId: item.userId ?? item.UserId ?? null,
            username: item.username ?? item.Username ?? null,
            identifier: item.identifier ?? item.Identifier ?? '',
            isSuccessful: item.isSuccessful ?? item.IsSuccessful ?? false,
            attemptedAt: item.attemptedAt ?? item.AttemptedAt ?? '',
            ipAddress: item.ipAddress ?? item.IPAddress ?? '',
            userAgent: item.userAgent ?? item.UserAgent ?? '',
            failureReason: item.failureReason ?? item.FailureReason ?? null,
        };
    }
    async function loadLoginHistories() {
        const tableBody = document.getElementById('login-histories-table-body');
        if (!tableBody) {
            return;
        }
        tableBody.innerHTML = '<tr><td colspan="7" class="text-center"><div class="spinner-border text-primary"></div></td></tr>';
        loadPageSizeFromUi();
        const response = await apiRequest(buildPagedUrl(), { method: 'GET' });
        if (!response.success) {
            showError(response.message || 'Failed to load login history');
            tableBody.innerHTML = '<tr><td colspan="7" class="text-center text-danger">Failed to load data</td></tr>';
            return;
        }
        const raw = response.data;
        const extracted = extractItems(raw);
        const meta = extracted.meta ?? raw;
        allLoginHistories = extracted.items.map(normalizeItem);
        pageSize = meta?.pageSize ?? meta?.PageSize ?? raw?.pageSize ?? raw?.PageSize ?? pageSize;
        totalCount = meta?.totalCount ?? meta?.TotalCount ?? raw?.totalCount ?? raw?.TotalCount ?? allLoginHistories.length;
        totalPages = meta?.totalPages ?? meta?.TotalPages ?? raw?.totalPages ?? raw?.TotalPages ?? Math.max(1, Math.ceil(totalCount / pageSize));
        currentPage = meta?.currentPage ?? meta?.CurrentPage ?? raw?.currentPage ?? raw?.CurrentPage ?? currentPage;
        renderTable();
        renderPagination();
    }
    function renderTable() {
        const tableBody = document.getElementById('login-histories-table-body');
        if (!tableBody) {
            return;
        }
        if (!allLoginHistories.length) {
            tableBody.innerHTML = '<tr><td colspan="7" class="text-center text-muted">No login history entries found.</td></tr>';
            return;
        }
        tableBody.innerHTML = allLoginHistories.map((entry) => {
            const userDisplay = entry.username
                ? `${esc(entry.username)} <span class="text-muted">(#${entry.userId ?? ''})</span>`
                : entry.userId
                    ? `#${entry.userId}`
                    : '<span class="text-muted">(unknown)</span>';
            const attempted = entry.attemptedAt ? formatDate(entry.attemptedAt) : '-';
            const resultBadge = entry.isSuccessful
                ? '<span class="badge bg-success">Success</span>'
                : '<span class="badge bg-danger">Failed</span>';
            return `
        <tr>
            <td>${entry.id}</td>
            <td>${userDisplay}</td>
            <td><code>${esc(entry.identifier)}</code></td>
            <td>${resultBadge}</td>
            <td>${attempted}</td>
            <td>${esc(entry.ipAddress || '-')}</td>
            <td>${esc(entry.failureReason || '-')}</td>
        </tr>`;
        }).join('');
    }
    function renderPagination() {
        const info = document.getElementById('login-histories-pagination-info');
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
        loadLoginHistories();
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
    function applyFilters() {
        currentPage = 1;
        loadLoginHistories();
    }
    function resetFilters() {
        const userId = document.getElementById('login-histories-filter-userid');
        const success = document.getElementById('login-histories-filter-success');
        const from = document.getElementById('login-histories-filter-from');
        const to = document.getElementById('login-histories-filter-to');
        if (userId)
            userId.value = '';
        if (success)
            success.value = '';
        if (from)
            from.value = '';
        if (to)
            to.value = '';
        currentPage = 1;
        loadLoginHistories();
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
        const alert = document.getElementById('login-histories-alert-success');
        if (!alert) {
            return;
        }
        alert.textContent = message;
        alert.classList.remove('d-none');
        const errorAlert = document.getElementById('login-histories-alert-error');
        errorAlert?.classList.add('d-none');
        setTimeout(() => alert.classList.add('d-none'), 5000);
    }
    function showError(message) {
        const alert = document.getElementById('login-histories-alert-error');
        if (!alert) {
            return;
        }
        alert.textContent = message;
        alert.classList.remove('d-none');
        const successAlert = document.getElementById('login-histories-alert-success');
        successAlert?.classList.add('d-none');
    }
    function initializeLoginHistoriesPage() {
        const page = document.getElementById('login-histories-page');
        if (!page || page.dataset.initialized === 'true') {
            return;
        }
        page.dataset.initialized = 'true';
        document.getElementById('login-histories-apply')?.addEventListener('click', applyFilters);
        document.getElementById('login-histories-reset')?.addEventListener('click', resetFilters);
        bindPagingControlsActions();
        document.getElementById('login-histories-page-size')?.addEventListener('change', () => {
            currentPage = 1;
            loadLoginHistories();
        });
        loadLoginHistories();
    }
    function setupPageObserver() {
        initializeLoginHistoriesPage();
        if (document.body) {
            const observer = new MutationObserver(() => {
                const page = document.getElementById('login-histories-page');
                if (page && page.dataset.initialized !== 'true') {
                    initializeLoginHistoriesPage();
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
//# sourceMappingURL=login-histories.js.map
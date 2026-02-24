"use strict";
// @ts-nocheck
(function () {
    function getApiBaseUrl() {
        var _a;
        const baseUrl = (_a = window.AppSettings) === null || _a === void 0 ? void 0 : _a.apiBaseUrl;
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
        if (auth === null || auth === void 0 ? void 0 : auth.getToken) {
            return auth.getToken();
        }
        return sessionStorage.getItem('rp_authToken');
    }
    async function apiRequest(endpoint, options = {}) {
        var _a, _b;
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
                data: data,
                message: data === null || data === void 0 ? void 0 : data.message,
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
        var _a, _b, _c, _d, _e, _f;
        if (Array.isArray(raw)) {
            return { items: raw, meta: null };
        }
        // Common envelopes:
        // 1) PagedResult: { Data: [...], TotalCount, CurrentPage, TotalPages, PageSize }
        // 2) { data: { Data: [...], TotalCount, ... } }
        // 3) { data: [...], totalCount, currentPage, ... }
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
        var _a;
        const el = document.getElementById(id);
        if (!el) {
            return null;
        }
        const raw = ((_a = el.value) !== null && _a !== void 0 ? _a : '').trim();
        if (!raw) {
            return null;
        }
        const value = Number(raw);
        return Number.isFinite(value) ? value : null;
    }
    function getSelectValue(id) {
        var _a;
        const el = document.getElementById(id);
        return ((_a = el === null || el === void 0 ? void 0 : el.value) !== null && _a !== void 0 ? _a : '').trim();
    }
    function getDateValue(id) {
        var _a;
        const el = document.getElementById(id);
        const raw = ((_a = el === null || el === void 0 ? void 0 : el.value) !== null && _a !== void 0 ? _a : '').trim();
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
        var _a, _b, _c, _d, _e, _f, _g, _h, _j, _k, _l, _m, _o, _p, _q, _r, _s, _t;
        return {
            id: (_b = (_a = item.id) !== null && _a !== void 0 ? _a : item.Id) !== null && _b !== void 0 ? _b : 0,
            userId: (_d = (_c = item.userId) !== null && _c !== void 0 ? _c : item.UserId) !== null && _d !== void 0 ? _d : null,
            username: (_f = (_e = item.username) !== null && _e !== void 0 ? _e : item.Username) !== null && _f !== void 0 ? _f : null,
            identifier: (_h = (_g = item.identifier) !== null && _g !== void 0 ? _g : item.Identifier) !== null && _h !== void 0 ? _h : '',
            isSuccessful: (_k = (_j = item.isSuccessful) !== null && _j !== void 0 ? _j : item.IsSuccessful) !== null && _k !== void 0 ? _k : false,
            attemptedAt: (_m = (_l = item.attemptedAt) !== null && _l !== void 0 ? _l : item.AttemptedAt) !== null && _m !== void 0 ? _m : '',
            ipAddress: (_p = (_o = item.ipAddress) !== null && _o !== void 0 ? _o : item.IPAddress) !== null && _p !== void 0 ? _p : '',
            userAgent: (_r = (_q = item.userAgent) !== null && _q !== void 0 ? _q : item.UserAgent) !== null && _r !== void 0 ? _r : '',
            failureReason: (_t = (_s = item.failureReason) !== null && _s !== void 0 ? _s : item.FailureReason) !== null && _t !== void 0 ? _t : null,
        };
    }
    async function loadLoginHistories() {
        var _a, _b, _c, _d, _e, _f, _g, _h, _j, _k, _l, _m, _o, _p, _q, _r, _s;
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
        const meta = (_a = extracted.meta) !== null && _a !== void 0 ? _a : raw;
        allLoginHistories = extracted.items.map(normalizeItem);
        pageSize = (_e = (_d = (_c = (_b = meta === null || meta === void 0 ? void 0 : meta.pageSize) !== null && _b !== void 0 ? _b : meta === null || meta === void 0 ? void 0 : meta.PageSize) !== null && _c !== void 0 ? _c : raw === null || raw === void 0 ? void 0 : raw.pageSize) !== null && _d !== void 0 ? _d : raw === null || raw === void 0 ? void 0 : raw.PageSize) !== null && _e !== void 0 ? _e : pageSize;
        totalCount = (_j = (_h = (_g = (_f = meta === null || meta === void 0 ? void 0 : meta.totalCount) !== null && _f !== void 0 ? _f : meta === null || meta === void 0 ? void 0 : meta.TotalCount) !== null && _g !== void 0 ? _g : raw === null || raw === void 0 ? void 0 : raw.totalCount) !== null && _h !== void 0 ? _h : raw === null || raw === void 0 ? void 0 : raw.TotalCount) !== null && _j !== void 0 ? _j : allLoginHistories.length;
        totalPages = (_o = (_m = (_l = (_k = meta === null || meta === void 0 ? void 0 : meta.totalPages) !== null && _k !== void 0 ? _k : meta === null || meta === void 0 ? void 0 : meta.TotalPages) !== null && _l !== void 0 ? _l : raw === null || raw === void 0 ? void 0 : raw.totalPages) !== null && _m !== void 0 ? _m : raw === null || raw === void 0 ? void 0 : raw.TotalPages) !== null && _o !== void 0 ? _o : Math.max(1, Math.ceil(totalCount / pageSize));
        currentPage = (_s = (_r = (_q = (_p = meta === null || meta === void 0 ? void 0 : meta.currentPage) !== null && _p !== void 0 ? _p : meta === null || meta === void 0 ? void 0 : meta.CurrentPage) !== null && _q !== void 0 ? _q : raw === null || raw === void 0 ? void 0 : raw.currentPage) !== null && _r !== void 0 ? _r : raw === null || raw === void 0 ? void 0 : raw.CurrentPage) !== null && _s !== void 0 ? _s : currentPage;
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
            var _a;
            const userDisplay = entry.username
                ? `${esc(entry.username)} <span class="text-muted">(#${(_a = entry.userId) !== null && _a !== void 0 ? _a : ''})</span>`
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
        errorAlert === null || errorAlert === void 0 ? void 0 : errorAlert.classList.add('d-none');
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
        successAlert === null || successAlert === void 0 ? void 0 : successAlert.classList.add('d-none');
    }
    function initializeLoginHistoriesPage() {
        var _a, _b, _c;
        const page = document.getElementById('login-histories-page');
        if (!page || page.dataset.initialized === 'true') {
            return;
        }
        page.dataset.initialized = 'true';
        (_a = document.getElementById('login-histories-apply')) === null || _a === void 0 ? void 0 : _a.addEventListener('click', applyFilters);
        (_b = document.getElementById('login-histories-reset')) === null || _b === void 0 ? void 0 : _b.addEventListener('click', resetFilters);
        bindPagingControlsActions();
        (_c = document.getElementById('login-histories-page-size')) === null || _c === void 0 ? void 0 : _c.addEventListener('change', () => {
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
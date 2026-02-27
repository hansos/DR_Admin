"use strict";
(function () {
    const w = window;
    let allTlds = [];
    let activeRegistrars = [];
    let editingId = null;
    let pendingDeleteId = null;
    let selectedRegistrarFilterId = null;
    let currentPage = 1;
    let pageSize = 25;
    let totalCount = 0;
    let totalPages = 1;
    let modalCurrentSalesPricing = null;
    let createPreviewRegistrarCostRows = [];
    function getApiBaseUrl() {
        const baseUrl = w.AppSettings?.apiBaseUrl;
        if (!baseUrl) {
            return window.location.protocol === 'https:'
                ? 'https://localhost:7201/api/v1'
                : 'http://localhost:5133/api/v1';
        }
        return baseUrl;
    }
    async function previewRegistrarPricesForCreate(extension) {
        const normalized = extension.trim().replace(/^\./, '').toLowerCase();
        if (!normalized) {
            createPreviewRegistrarCostRows = [];
            renderRegistrarCostRows([]);
            return;
        }
        setRegistrarPricesLoading(true);
        try {
            const response = await apiRequest(`${getApiBaseUrl()}/RegistrarTldCostPricing/preview/extension/${encodeURIComponent(normalized)}`, {
                method: 'GET',
            });
            if (!response.success) {
                createPreviewRegistrarCostRows = [];
                renderRegistrarCostRows([]);
                return;
            }
            createPreviewRegistrarCostRows = getListFromUnknown(response.data)
                .filter(isRecord)
                .map((item) => ({
                registrarName: getString(item.registrarName ?? item.RegistrarName),
                registrationCost: getDecimal(item.registrationCost ?? item.RegistrationCost),
                renewalCost: getDecimal(item.renewalCost ?? item.RenewalCost),
                transferCost: getDecimal(item.transferCost ?? item.TransferCost),
                currency: getString(item.currency ?? item.Currency) || '-',
            }))
                .filter((row) => row.registrarName.trim().length > 0)
                .sort((a, b) => a.registrarName.localeCompare(b.registrarName));
            renderRegistrarCostRows(createPreviewRegistrarCostRows);
        }
        finally {
            setRegistrarPricesLoading(false);
        }
    }
    function getAuthToken() {
        const token = w.Auth?.getToken?.();
        if (token) {
            return token;
        }
        return sessionStorage.getItem('rp_authToken');
    }
    function isRecord(value) {
        return typeof value === 'object' && value !== null;
    }
    function getString(value) {
        return typeof value === 'string' ? value : '';
    }
    function getBoolean(value, fallback = false) {
        return typeof value === 'boolean' ? value : fallback;
    }
    function getNumber(value) {
        return typeof value === 'number' && Number.isFinite(value) ? value : null;
    }
    function getDecimal(value) {
        if (typeof value === 'number' && Number.isFinite(value)) {
            return value;
        }
        if (typeof value === 'string') {
            const parsed = Number(value);
            if (Number.isFinite(parsed)) {
                return parsed;
            }
        }
        return null;
    }
    function getListFromUnknown(raw) {
        if (Array.isArray(raw)) {
            return raw;
        }
        if (!isRecord(raw)) {
            return [];
        }
        const candidates = [raw.Data, raw.data, raw.items, raw.Items];
        for (const candidate of candidates) {
            if (Array.isArray(candidate)) {
                return candidate;
            }
        }
        return [];
    }
    function normalizeSalesPricing(item) {
        if (!isRecord(item)) {
            return {
                id: 0,
                tldId: 0,
                effectiveFrom: '',
                effectiveTo: null,
                registrationPrice: 0,
                renewalPrice: 0,
                transferPrice: 0,
                privacyPrice: null,
                firstYearRegistrationPrice: null,
                currency: 'USD',
            };
        }
        return {
            id: getNumber(item.id) ?? getNumber(item.Id) ?? 0,
            tldId: getNumber(item.tldId ?? item.TldId) ?? 0,
            effectiveFrom: getString(item.effectiveFrom ?? item.EffectiveFrom),
            effectiveTo: getString(item.effectiveTo ?? item.EffectiveTo) || null,
            registrationPrice: getDecimal(item.registrationPrice ?? item.RegistrationPrice) ?? 0,
            renewalPrice: getDecimal(item.renewalPrice ?? item.RenewalPrice) ?? 0,
            transferPrice: getDecimal(item.transferPrice ?? item.TransferPrice) ?? 0,
            privacyPrice: getDecimal(item.privacyPrice ?? item.PrivacyPrice),
            firstYearRegistrationPrice: getDecimal(item.firstYearRegistrationPrice ?? item.FirstYearRegistrationPrice),
            currency: getString(item.currency ?? item.Currency) || 'USD',
        };
    }
    function pickBestPricingFromHistory(items) {
        if (!items.length) {
            return null;
        }
        const now = Date.now();
        const activeNow = items
            .filter((item) => {
            const from = Date.parse(item.effectiveFrom);
            if (!Number.isFinite(from) || from > now) {
                return false;
            }
            if (!item.effectiveTo) {
                return true;
            }
            const to = Date.parse(item.effectiveTo);
            return !Number.isFinite(to) || to > now;
        })
            .sort((a, b) => Date.parse(b.effectiveFrom) - Date.parse(a.effectiveFrom));
        if (activeNow.length > 0) {
            return activeNow[0];
        }
        return items
            .slice()
            .sort((a, b) => Date.parse(b.effectiveFrom) - Date.parse(a.effectiveFrom))[0] ?? null;
    }
    function toDateTimeLocalValue(iso) {
        if (!iso) {
            return '';
        }
        const parsed = new Date(iso);
        if (Number.isNaN(parsed.getTime())) {
            return '';
        }
        const offsetMs = parsed.getTimezoneOffset() * 60000;
        const local = new Date(parsed.getTime() - offsetMs);
        return local.toISOString().slice(0, 16);
    }
    function readDateTimeLocalAsIso(id) {
        const input = document.getElementById(id);
        const raw = input?.value.trim() ?? '';
        if (!raw) {
            return null;
        }
        const parsed = new Date(raw);
        return Number.isNaN(parsed.getTime()) ? null : parsed.toISOString();
    }
    function formatPrice(value) {
        if (value === null || !Number.isFinite(value)) {
            return '-';
        }
        return value.toFixed(2);
    }
    async function apiRequest(endpoint, options = {}) {
        try {
            const headers = {
                'Content-Type': 'application/json',
                ...options.headers,
            };
            const authToken = getAuthToken();
            if (authToken) {
                headers.Authorization = `Bearer ${authToken}`;
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
                let message = `Request failed with status ${response.status}`;
                if (isRecord(data)) {
                    const candidate = data.message ?? data.title;
                    if (typeof candidate === 'string' && candidate.trim()) {
                        message = candidate;
                    }
                }
                return {
                    success: false,
                    message,
                };
            }
            if (isRecord(data) && data.success === false) {
                return {
                    success: false,
                    message: getString(data.message) || 'Request failed',
                };
            }
            if (isRecord(data) && 'data' in data) {
                return {
                    success: true,
                    data: data.data,
                    message: getString(data.message),
                };
            }
            return {
                success: true,
                data: data,
            };
        }
        catch (error) {
            console.error('TLD request failed', error);
            return {
                success: false,
                message: 'Network error. Please try again.',
            };
        }
    }
    function normalizeTld(item) {
        if (!isRecord(item)) {
            return {
                id: 0,
                extension: '',
                description: '',
                isActive: true,
                registrationPrice: null,
                renewalPrice: null,
                transferPrice: null,
                priceCurrency: null,
                defaultRegistrationYears: null,
                maxRegistrationYears: null,
                requiresPrivacy: false,
                notes: null,
            };
        }
        return {
            id: getNumber(item.id) ?? getNumber(item.Id) ?? 0,
            extension: getString(item.extension ?? item.Extension).replace(/^\./, ''),
            description: getString(item.description ?? item.Description),
            isActive: getBoolean(item.isActive ?? item.IsActive, true),
            registrationPrice: null,
            renewalPrice: null,
            transferPrice: null,
            priceCurrency: null,
            defaultRegistrationYears: getNumber(item.defaultRegistrationYears ?? item.DefaultRegistrationYears),
            maxRegistrationYears: getNumber(item.maxRegistrationYears ?? item.MaxRegistrationYears),
            requiresPrivacy: getBoolean(item.requiresPrivacy ?? item.RequiresPrivacy),
            notes: getString(item.notes ?? item.Notes) || null,
        };
    }
    function normalizeRegistrar(item) {
        if (!isRecord(item)) {
            return {
                id: 0,
                name: '',
                code: '',
                isActive: false,
            };
        }
        return {
            id: getNumber(item.id) ?? getNumber(item.Id) ?? 0,
            name: getString(item.name ?? item.Name),
            code: getString(item.code ?? item.Code),
            isActive: getBoolean(item.isActive ?? item.IsActive),
        };
    }
    function normalizeRegistrarTld(item) {
        if (!isRecord(item)) {
            return {
                id: 0,
                registrarId: 0,
                tldId: 0,
                tldExtension: '',
                isActive: false,
            };
        }
        return {
            id: getNumber(item.id) ?? getNumber(item.Id) ?? 0,
            registrarId: getNumber(item.registrarId ?? item.RegistrarId) ?? 0,
            tldId: getNumber(item.tldId ?? item.TldId) ?? 0,
            tldExtension: getString(item.tldExtension ?? item.TldExtension),
            isActive: getBoolean(item.isActive ?? item.IsActive),
        };
    }
    function extractPagedTldData(raw) {
        const items = getListFromUnknown(raw)
            .map(normalizeTld)
            .filter((item) => item.id > 0 && item.isActive);
        if (!isRecord(raw)) {
            const count = items.length;
            return {
                items,
                page: currentPage,
                size: pageSize,
                count,
                pages: Math.max(1, Math.ceil(count / pageSize)),
            };
        }
        const page = getNumber(raw.currentPage ?? raw.CurrentPage) ?? currentPage;
        const size = getNumber(raw.pageSize ?? raw.PageSize) ?? pageSize;
        const count = getNumber(raw.totalCount ?? raw.TotalCount) ?? items.length;
        const pages = getNumber(raw.totalPages ?? raw.TotalPages) ?? Math.max(1, Math.ceil((count || 1) / (size || 1)));
        return {
            items,
            page,
            size,
            count,
            pages,
        };
    }
    function extractPagedRegistrarTldData(raw) {
        const items = getListFromUnknown(raw)
            .map(normalizeRegistrarTld)
            .filter((item) => item.id > 0 && item.isActive);
        if (!isRecord(raw)) {
            const count = items.length;
            return {
                items,
                page: currentPage,
                size: pageSize,
                count,
                pages: Math.max(1, Math.ceil(count / pageSize)),
            };
        }
        const page = getNumber(raw.currentPage ?? raw.CurrentPage) ?? currentPage;
        const size = getNumber(raw.pageSize ?? raw.PageSize) ?? pageSize;
        const count = getNumber(raw.totalCount ?? raw.TotalCount) ?? items.length;
        const pages = getNumber(raw.totalPages ?? raw.TotalPages) ?? Math.max(1, Math.ceil((count || 1) / (size || 1)));
        return {
            items,
            page,
            size,
            count,
            pages,
        };
    }
    function esc(text) {
        const map = {
            '&': '&amp;',
            '<': '&lt;',
            '>': '&gt;',
            '"': '&quot;',
            "'": '&#039;',
        };
        return (text || '').replace(/[&<>"']/g, (char) => map[char]);
    }
    function showSuccess(message) {
        const alert = document.getElementById('tlds-alert-success');
        if (!alert) {
            return;
        }
        alert.textContent = message;
        alert.classList.remove('d-none');
        const errorAlert = document.getElementById('tlds-alert-error');
        errorAlert?.classList.add('d-none');
        setTimeout(() => alert.classList.add('d-none'), 5000);
    }
    function showError(message) {
        const alert = document.getElementById('tlds-alert-error');
        if (!alert) {
            return;
        }
        alert.textContent = message;
        alert.classList.remove('d-none');
        const successAlert = document.getElementById('tlds-alert-success');
        successAlert?.classList.add('d-none');
    }
    function showModal(id) {
        const element = document.getElementById(id);
        const bootstrapApi = w.bootstrap;
        if (!element || !bootstrapApi) {
            return;
        }
        const modal = new bootstrapApi.Modal(element);
        modal.show();
    }
    function hideModal(id) {
        const element = document.getElementById(id);
        const bootstrapApi = w.bootstrap;
        if (!element || !bootstrapApi) {
            return;
        }
        const modal = bootstrapApi.Modal.getInstance(element);
        modal?.hide();
    }
    function readPageSize() {
        const input = document.getElementById('tlds-page-size');
        const raw = input?.value ?? '';
        const parsed = Number(raw);
        if (Number.isFinite(parsed) && parsed > 0) {
            pageSize = parsed;
        }
    }
    function getRegistrarFilterId() {
        const input = document.getElementById('tlds-filter-registrar');
        const raw = input?.value ?? '';
        if (!raw) {
            return null;
        }
        const parsed = Number(raw);
        return Number.isFinite(parsed) && parsed > 0 ? parsed : null;
    }
    function getSelectedRegistrarIdsFromModal() {
        const select = document.getElementById('tlds-registrars');
        if (!select) {
            return [];
        }
        const values = Array.from(select.selectedOptions)
            .map((opt) => Number(opt.value))
            .filter((value) => Number.isFinite(value) && value > 0);
        return Array.from(new Set(values));
    }
    function renderRegistrarFilterOptions() {
        const select = document.getElementById('tlds-filter-registrar');
        if (!select) {
            return;
        }
        const selectedValue = selectedRegistrarFilterId !== null ? String(selectedRegistrarFilterId) : '';
        const options = ['<option value="">All registrars</option>'];
        for (const registrar of activeRegistrars) {
            const display = registrar.code ? `${registrar.name} (${registrar.code})` : registrar.name;
            options.push(`<option value="${registrar.id}">${esc(display)}</option>`);
        }
        select.innerHTML = options.join('');
        select.value = selectedValue;
    }
    function renderRegistrarMultiSelect(selectedRegistrarIds) {
        const select = document.getElementById('tlds-registrars');
        if (!select) {
            return;
        }
        const selectedSet = new Set(selectedRegistrarIds);
        const options = activeRegistrars.map((registrar) => {
            const selected = selectedSet.has(registrar.id) ? ' selected' : '';
            const display = registrar.code ? `${registrar.name} (${registrar.code})` : registrar.name;
            return `<option value="${registrar.id}"${selected}>${esc(display)}</option>`;
        });
        select.innerHTML = options.join('');
    }
    function buildTldsUrl() {
        const params = new URLSearchParams();
        params.set('pageNumber', String(currentPage));
        params.set('pageSize', String(pageSize));
        if (selectedRegistrarFilterId !== null) {
            params.set('isActive', 'true');
            return `${getApiBaseUrl()}/RegistrarTlds/registrar/${selectedRegistrarFilterId}?${params.toString()}`;
        }
        return `${getApiBaseUrl()}/Tlds/active?${params.toString()}`;
    }
    function renderTable() {
        const tableBody = document.getElementById('tlds-table-body');
        if (!tableBody) {
            return;
        }
        if (!allTlds.length) {
            tableBody.innerHTML = '<tr><td colspan="11" class="text-center text-muted">No active TLDs found.</td></tr>';
            return;
        }
        tableBody.innerHTML = allTlds.map((item) => {
            const extension = item.extension.startsWith('.') ? item.extension : `.${item.extension}`;
            return `
        <tr>
            <td>${item.id}</td>
            <td><code>${esc(extension)}</code></td>
            <td>${esc(item.description || '-')}</td>
            <td>${formatPrice(item.registrationPrice)}</td>
            <td>${formatPrice(item.renewalPrice)}</td>
            <td>${formatPrice(item.transferPrice)}</td>
            <td>${esc(item.priceCurrency || '-')}</td>
            <td>${item.defaultRegistrationYears ?? '-'}</td>
            <td>${item.maxRegistrationYears ?? '-'}</td>
            <td><span class="badge bg-${item.requiresPrivacy ? 'warning text-dark' : 'secondary'}">${item.requiresPrivacy ? 'Required' : 'Optional'}</span></td>
            <td>
                <div class="btn-group btn-group-sm">
                    <button class="btn btn-outline-primary" type="button" data-action="edit" data-id="${item.id}" title="Edit"><i class="bi bi-pencil"></i></button>
                    <button class="btn btn-outline-danger" type="button" data-action="delete" data-id="${item.id}" data-name="${esc(extension)}" title="Delete"><i class="bi bi-trash"></i></button>
                </div>
            </td>
        </tr>`;
        }).join('');
    }
    function renderPaginationInfo() {
        const info = document.getElementById('tlds-pagination-info');
        if (!info) {
            return;
        }
        if (!totalCount) {
            info.textContent = 'Showing 0 of 0';
            return;
        }
        const start = (currentPage - 1) * pageSize + 1;
        const end = Math.min(currentPage * pageSize, totalCount);
        info.textContent = `Showing ${start}-${end} of ${totalCount}`;
    }
    function renderPagingControls() {
        const list = document.getElementById('tlds-paging-controls-list');
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
        let html = '';
        html += makeItem('Previous', currentPage - 1, currentPage <= 1);
        const pages = new Set();
        pages.add(1);
        if (totalPages >= 2) {
            pages.add(2);
            pages.add(totalPages - 1);
        }
        pages.add(totalPages);
        for (let p = currentPage - 1; p <= currentPage + 1; p++) {
            if (p >= 1 && p <= totalPages) {
                pages.add(p);
            }
        }
        const sorted = Array.from(pages).sort((a, b) => a - b);
        let last = 0;
        for (const p of sorted) {
            if (last && p - last > 1) {
                html += makeEllipsis();
            }
            html += makeItem(String(p), p, false, p === currentPage);
            last = p;
        }
        html += makeItem('Next', currentPage + 1, currentPage >= totalPages);
        list.innerHTML = html;
    }
    async function loadActiveRegistrars() {
        const response = await apiRequest(`${getApiBaseUrl()}/Registrars/active`, { method: 'GET' });
        if (!response.success) {
            showError(response.message || 'Failed to load registrars');
            activeRegistrars = [];
            renderRegistrarFilterOptions();
            renderRegistrarMultiSelect([]);
            return;
        }
        activeRegistrars = getListFromUnknown(response.data)
            .map(normalizeRegistrar)
            .filter((item) => item.id > 0 && item.isActive)
            .sort((a, b) => a.name.localeCompare(b.name));
        renderRegistrarFilterOptions();
        renderRegistrarMultiSelect([]);
    }
    async function loadTlds() {
        const tableBody = document.getElementById('tlds-table-body');
        if (!tableBody) {
            return;
        }
        tableBody.innerHTML = '<tr><td colspan="11" class="text-center"><div class="spinner-border text-primary"></div></td></tr>';
        readPageSize();
        const response = await apiRequest(buildTldsUrl(), { method: 'GET' });
        if (!response.success) {
            showError(response.message || 'Failed to load active TLDs');
            tableBody.innerHTML = '<tr><td colspan="11" class="text-center text-danger">Failed to load data</td></tr>';
            return;
        }
        if (selectedRegistrarFilterId !== null) {
            const parsed = extractPagedRegistrarTldData(response.data);
            allTlds = parsed.items.map((item) => ({
                id: item.tldId,
                extension: item.tldExtension.replace(/^\./, ''),
                description: '',
                isActive: item.isActive,
                registrationPrice: null,
                renewalPrice: null,
                transferPrice: null,
                priceCurrency: null,
                defaultRegistrationYears: null,
                maxRegistrationYears: null,
                requiresPrivacy: false,
                notes: null,
            }));
            currentPage = parsed.page;
            pageSize = parsed.size;
            totalCount = parsed.count;
            totalPages = parsed.pages;
        }
        else {
            const parsed = extractPagedTldData(response.data);
            allTlds = parsed.items;
            currentPage = parsed.page;
            pageSize = parsed.size;
            totalCount = parsed.count;
            totalPages = parsed.pages;
        }
        renderTable();
        renderPaginationInfo();
        renderPagingControls();
        void loadSalesPricingForCurrentRows();
    }
    async function loadSalesPricingForCurrentRows() {
        const pricingByTldId = new Map();
        await Promise.all(allTlds.map(async (tld) => {
            const response = await apiRequest(`${getApiBaseUrl()}/tld-pricing/sales/tld/${tld.id}/current`, { method: 'GET' });
            if (!response.success || !response.data) {
                return;
            }
            const pricing = normalizeSalesPricing(response.data);
            if (pricing.tldId > 0) {
                pricingByTldId.set(pricing.tldId, pricing);
            }
        }));
        for (const tld of allTlds) {
            const pricing = pricingByTldId.get(tld.id);
            if (!pricing) {
                tld.registrationPrice = null;
                tld.renewalPrice = null;
                tld.transferPrice = null;
                tld.priceCurrency = null;
                continue;
            }
            tld.registrationPrice = pricing.registrationPrice;
            tld.renewalPrice = pricing.renewalPrice;
            tld.transferPrice = pricing.transferPrice;
            tld.priceCurrency = pricing.currency;
        }
        renderTable();
    }
    async function getAssignedRegistrarIds(tldId) {
        const response = await apiRequest(`${getApiBaseUrl()}/RegistrarTlds/tld/${tldId}`, { method: 'GET' });
        if (!response.success) {
            return [];
        }
        const ids = getListFromUnknown(response.data)
            .map(normalizeRegistrarTld)
            .map((item) => item.registrarId)
            .filter((id) => id > 0);
        return Array.from(new Set(ids));
    }
    async function getAssignedRegistrarTlds(tldId) {
        const response = await apiRequest(`${getApiBaseUrl()}/RegistrarTlds/tld/${tldId}`, { method: 'GET' });
        if (!response.success) {
            return [];
        }
        return getListFromUnknown(response.data)
            .map(normalizeRegistrarTld)
            .filter((item) => item.id > 0 && item.isActive);
    }
    function setSalesPricingInputs(pricing) {
        const registrationInput = document.getElementById('tlds-registration-price');
        const renewalInput = document.getElementById('tlds-renewal-price');
        const transferInput = document.getElementById('tlds-transfer-price');
        const privacyInput = document.getElementById('tlds-privacy-price');
        const firstYearInput = document.getElementById('tlds-first-year-price');
        const currencyInput = document.getElementById('tlds-price-currency');
        const effectiveFromInput = document.getElementById('tlds-price-effective-from');
        const effectiveToInput = document.getElementById('tlds-price-effective-to');
        if (registrationInput) {
            registrationInput.value = pricing ? String(pricing.registrationPrice) : '';
        }
        if (renewalInput) {
            renewalInput.value = pricing ? String(pricing.renewalPrice) : '';
        }
        if (transferInput) {
            transferInput.value = pricing ? String(pricing.transferPrice) : '';
        }
        if (privacyInput) {
            privacyInput.value = pricing?.privacyPrice !== null && pricing?.privacyPrice !== undefined
                ? String(pricing.privacyPrice)
                : '';
        }
        if (firstYearInput) {
            firstYearInput.value = pricing?.firstYearRegistrationPrice !== null && pricing?.firstYearRegistrationPrice !== undefined
                ? String(pricing.firstYearRegistrationPrice)
                : '';
        }
        if (currencyInput) {
            currencyInput.value = pricing?.currency || 'USD';
        }
        if (effectiveFromInput) {
            effectiveFromInput.value = toDateTimeLocalValue(pricing?.effectiveFrom ?? null);
        }
        if (effectiveToInput) {
            effectiveToInput.value = toDateTimeLocalValue(pricing?.effectiveTo ?? null);
        }
    }
    function setRegistrarPricesLoading(isLoading) {
        const loading = document.getElementById('tlds-registrar-prices-loading');
        loading?.classList.toggle('d-none', !isLoading);
    }
    function renderRegistrarCostRows(rows) {
        const empty = document.getElementById('tlds-registrar-prices-empty');
        const tableWrap = document.getElementById('tlds-registrar-prices-table-wrap');
        const body = document.getElementById('tlds-registrar-prices-body');
        if (!empty || !tableWrap || !body) {
            return;
        }
        if (!rows.length) {
            empty.classList.remove('d-none');
            tableWrap.classList.add('d-none');
            body.innerHTML = '';
            return;
        }
        empty.classList.add('d-none');
        tableWrap.classList.remove('d-none');
        body.innerHTML = rows.map((row) => `
        <tr>
            <td>${esc(row.registrarName)}</td>
            <td>${formatPrice(row.registrationCost)}</td>
            <td>${formatPrice(row.renewalCost)}</td>
            <td>${formatPrice(row.transferCost)}</td>
            <td>${esc(row.currency)}</td>
        </tr>
    `).join('');
    }
    async function loadRegistrarPricesForModal(tldId) {
        if (tldId === null) {
            renderRegistrarCostRows([]);
            return;
        }
        setRegistrarPricesLoading(true);
        try {
            const response = await apiRequest(`${getApiBaseUrl()}/RegistrarTldCostPricing/tld/${tldId}/current/ensure`, {
                method: 'POST',
            });
            if (!response.success) {
                renderRegistrarCostRows([]);
                return;
            }
            const rows = getListFromUnknown(response.data)
                .filter(isRecord)
                .map((item) => ({
                registrarName: getString(item.registrarName ?? item.RegistrarName),
                registrationCost: getDecimal(item.registrationCost ?? item.RegistrationCost),
                renewalCost: getDecimal(item.renewalCost ?? item.RenewalCost),
                transferCost: getDecimal(item.transferCost ?? item.TransferCost),
                currency: getString(item.currency ?? item.Currency) || '-',
            }))
                .filter((row) => row.registrarName.trim().length > 0)
                .sort((a, b) => a.registrarName.localeCompare(b.registrarName));
            renderRegistrarCostRows(rows);
        }
        finally {
            setRegistrarPricesLoading(false);
        }
    }
    async function loadSalesPricingForModal(tldId) {
        modalCurrentSalesPricing = null;
        setSalesPricingInputs(null);
        if (tldId === null) {
            return;
        }
        const response = await apiRequest(`${getApiBaseUrl()}/tld-pricing/sales/tld/${tldId}?includeArchived=true`, {
            method: 'GET',
        });
        if (!response.success) {
            return;
        }
        const history = getListFromUnknown(response.data)
            .map(normalizeSalesPricing)
            .filter((item) => item.tldId === tldId)
            .filter((item) => item.effectiveFrom.length > 0);
        const selected = pickBestPricingFromHistory(history);
        if (!selected) {
            return;
        }
        modalCurrentSalesPricing = selected;
        setSalesPricingInputs(selected);
    }
    async function assignRegistrarToTld(registrarId, tldId) {
        const response = await apiRequest(`${getApiBaseUrl()}/Registrars/${registrarId}/tld/${tldId}`, {
            method: 'POST',
        });
        if (!response.success) {
            const message = (response.message || '').toLowerCase();
            if (message.includes('already')) {
                return;
            }
            throw new Error(response.message || `Failed to assign registrar ${registrarId}`);
        }
    }
    async function syncSelectedRegistrars(tldId, selectedRegistrarIds) {
        if (selectedRegistrarIds.length === 0) {
            return;
        }
        const existingIds = await getAssignedRegistrarIds(tldId);
        const existingSet = new Set(existingIds);
        for (const registrarId of selectedRegistrarIds) {
            if (!existingSet.has(registrarId)) {
                await assignRegistrarToTld(registrarId, tldId);
            }
        }
    }
    function openCreate() {
        editingId = null;
        modalCurrentSalesPricing = null;
        createPreviewRegistrarCostRows = [];
        const modalTitle = document.getElementById('tlds-modal-title');
        if (modalTitle) {
            modalTitle.textContent = 'Add TLD';
        }
        const form = document.getElementById('tlds-form');
        form?.reset();
        const isActive = document.getElementById('tlds-is-active');
        if (isActive) {
            isActive.checked = true;
        }
        const secondLevelContainer = document.getElementById('tlds-is-second-level-container');
        secondLevelContainer?.classList.remove('d-none');
        renderRegistrarMultiSelect([]);
        setSalesPricingInputs(null);
        renderRegistrarCostRows([]);
        showModal('tlds-edit-modal');
    }
    async function openEdit(id) {
        const currentItem = allTlds.find((item) => item.id === id);
        if (!currentItem) {
            return;
        }
        let tld = currentItem;
        editingId = id;
        const modalTitle = document.getElementById('tlds-modal-title');
        if (modalTitle) {
            modalTitle.textContent = 'Edit TLD';
        }
        const extension = document.getElementById('tlds-extension');
        const description = document.getElementById('tlds-description');
        const defaultYears = document.getElementById('tlds-default-years');
        const maxYears = document.getElementById('tlds-max-years');
        const requiresPrivacy = document.getElementById('tlds-requires-privacy');
        const isActive = document.getElementById('tlds-is-active');
        const notes = document.getElementById('tlds-notes');
        const isSecondLevel = document.getElementById('tlds-is-second-level');
        if (extension) {
            extension.value = tld.extension.replace(/^\./, '');
        }
        if (description) {
            description.value = tld.description || '';
        }
        if (defaultYears) {
            defaultYears.value = tld.defaultRegistrationYears !== null ? String(tld.defaultRegistrationYears) : '';
        }
        if (maxYears) {
            maxYears.value = tld.maxRegistrationYears !== null ? String(tld.maxRegistrationYears) : '';
        }
        if (requiresPrivacy) {
            requiresPrivacy.checked = tld.requiresPrivacy;
        }
        if (isActive) {
            isActive.checked = tld.isActive;
        }
        if (notes) {
            notes.value = tld.notes || '';
        }
        if (isSecondLevel) {
            isSecondLevel.checked = false;
        }
        const secondLevelContainer = document.getElementById('tlds-is-second-level-container');
        secondLevelContainer?.classList.add('d-none');
        renderRegistrarMultiSelect([]);
        setSalesPricingInputs(null);
        renderRegistrarCostRows([]);
        showModal('tlds-edit-modal');
        void loadRegistrarPricesForModal(id);
        void loadSalesPricingForModal(id);
        const tldResponse = await apiRequest(`${getApiBaseUrl()}/Tlds/${id}`, { method: 'GET' });
        if (tldResponse.success && tldResponse.data) {
            tld = normalizeTld(tldResponse.data);
            if (description) {
                description.value = tld.description || '';
            }
            if (defaultYears) {
                defaultYears.value = tld.defaultRegistrationYears !== null ? String(tld.defaultRegistrationYears) : '';
            }
            if (maxYears) {
                maxYears.value = tld.maxRegistrationYears !== null ? String(tld.maxRegistrationYears) : '';
            }
            if (requiresPrivacy) {
                requiresPrivacy.checked = tld.requiresPrivacy;
            }
            if (isActive) {
                isActive.checked = tld.isActive;
            }
            if (notes) {
                notes.value = tld.notes || '';
            }
        }
        const selectedRegistrarIds = await getAssignedRegistrarIds(id);
        renderRegistrarMultiSelect(selectedRegistrarIds);
    }
    function getOptionalNumber(id) {
        const input = document.getElementById(id);
        const raw = input?.value.trim() ?? '';
        if (!raw) {
            return null;
        }
        const parsed = Number(raw);
        return Number.isFinite(parsed) ? parsed : null;
    }
    async function upsertSalesPricingForTld(tldId) {
        const registrationPrice = getOptionalNumber('tlds-registration-price');
        const renewalPrice = getOptionalNumber('tlds-renewal-price');
        const transferPrice = getOptionalNumber('tlds-transfer-price');
        const privacyPrice = getOptionalNumber('tlds-privacy-price');
        const firstYearRegistrationPrice = getOptionalNumber('tlds-first-year-price');
        const currencyInput = document.getElementById('tlds-price-currency');
        const currency = (currencyInput?.value ?? '').trim().toUpperCase() || 'USD';
        const effectiveFromIso = readDateTimeLocalAsIso('tlds-price-effective-from');
        const effectiveToIso = readDateTimeLocalAsIso('tlds-price-effective-to');
        const hasAnyPriceInput = registrationPrice !== null ||
            renewalPrice !== null ||
            transferPrice !== null ||
            privacyPrice !== null ||
            firstYearRegistrationPrice !== null;
        if (!hasAnyPriceInput) {
            return;
        }
        if (registrationPrice === null || renewalPrice === null || transferPrice === null) {
            throw new Error('Registration, renewal and transfer prices are required when setting sales pricing.');
        }
        if (registrationPrice < 0 || renewalPrice < 0 || transferPrice < 0 ||
            (privacyPrice !== null && privacyPrice < 0) ||
            (firstYearRegistrationPrice !== null && firstYearRegistrationPrice < 0)) {
            throw new Error('Prices cannot be negative.');
        }
        const currentEffectiveFromIso = modalCurrentSalesPricing?.effectiveFrom ?? null;
        const currentEffectiveToIso = modalCurrentSalesPricing?.effectiveTo ?? null;
        const isSameAsCurrent = modalCurrentSalesPricing !== null &&
            currentEffectiveFromIso === effectiveFromIso &&
            currentEffectiveToIso === effectiveToIso &&
            modalCurrentSalesPricing.registrationPrice === registrationPrice &&
            modalCurrentSalesPricing.renewalPrice === renewalPrice &&
            modalCurrentSalesPricing.transferPrice === transferPrice &&
            modalCurrentSalesPricing.privacyPrice === privacyPrice &&
            modalCurrentSalesPricing.firstYearRegistrationPrice === firstYearRegistrationPrice &&
            modalCurrentSalesPricing.currency.toUpperCase() === currency;
        if (isSameAsCurrent) {
            return;
        }
        const minEffectiveFromDate = new Date(Date.now() + 60 * 1000);
        const effectiveFromDate = effectiveFromIso
            ? new Date(Math.max(new Date(effectiveFromIso).getTime(), minEffectiveFromDate.getTime()))
            : minEffectiveFromDate;
        const effectiveToDate = effectiveToIso ? new Date(effectiveToIso) : null;
        if (effectiveToDate !== null && effectiveToDate <= effectiveFromDate) {
            throw new Error('Effective to must be later than effective from.');
        }
        const payload = {
            tldId,
            effectiveFrom: effectiveFromDate.toISOString(),
            effectiveTo: effectiveToDate ? effectiveToDate.toISOString() : null,
            registrationPrice,
            renewalPrice,
            transferPrice,
            privacyPrice,
            firstYearRegistrationPrice,
            currency,
            isPromotional: false,
            promotionName: null,
            isActive: true,
            notes: 'Updated from reseller panel /tld/list',
        };
        const response = await apiRequest(`${getApiBaseUrl()}/tld-pricing/sales`, {
            method: 'POST',
            body: JSON.stringify(payload),
        });
        if (!response.success || !response.data) {
            throw new Error(response.message || 'Failed to save TLD sales pricing.');
        }
        const pricing = normalizeSalesPricing(response.data);
        modalCurrentSalesPricing = pricing;
        const target = allTlds.find((item) => item.id === tldId);
        if (target) {
            target.registrationPrice = pricing.registrationPrice;
            target.renewalPrice = pricing.renewalPrice;
            target.transferPrice = pricing.transferPrice;
            target.priceCurrency = pricing.currency;
        }
    }
    async function saveTld() {
        const extensionInput = document.getElementById('tlds-extension');
        const descriptionInput = document.getElementById('tlds-description');
        const secondLevelInput = document.getElementById('tlds-is-second-level');
        const activeInput = document.getElementById('tlds-is-active');
        const requiresPrivacyInput = document.getElementById('tlds-requires-privacy');
        const notesInput = document.getElementById('tlds-notes');
        const extension = (extensionInput?.value ?? '').trim().replace(/^\./, '').toLowerCase();
        if (!extension) {
            showError('Extension is required');
            return;
        }
        const defaultYears = getOptionalNumber('tlds-default-years');
        const maxYears = getOptionalNumber('tlds-max-years');
        if (defaultYears !== null && defaultYears < 1) {
            showError('Default years must be at least 1');
            return;
        }
        if (maxYears !== null && maxYears < 1) {
            showError('Max years must be at least 1');
            return;
        }
        if (defaultYears !== null && maxYears !== null && defaultYears > maxYears) {
            showError('Default years cannot be greater than max years');
            return;
        }
        const payloadBase = {
            extension,
            description: (descriptionInput?.value ?? '').trim(),
            isActive: activeInput?.checked ?? true,
            defaultRegistrationYears: defaultYears,
            maxRegistrationYears: maxYears,
            requiresPrivacy: requiresPrivacyInput?.checked ?? false,
            notes: (notesInput?.value ?? '').trim() || null,
        };
        const response = editingId !== null
            ? await apiRequest(`${getApiBaseUrl()}/Tlds/${editingId}`, {
                method: 'PUT',
                body: JSON.stringify(payloadBase),
            })
            : await apiRequest(`${getApiBaseUrl()}/Tlds`, {
                method: 'POST',
                body: JSON.stringify({
                    ...payloadBase,
                    isSecondLevel: secondLevelInput?.checked ?? false,
                }),
            });
        if (!response.success) {
            showError(response.message || 'Save failed');
            return;
        }
        const savedTld = normalizeTld(response.data);
        const effectiveTldId = editingId ?? (savedTld.id > 0 ? savedTld.id : null);
        if (effectiveTldId === null) {
            showError('TLD saved, but could not determine the TLD ID for registrar assignment.');
            return;
        }
        const selectedRegistrarIds = getSelectedRegistrarIdsFromModal();
        try {
            await syncSelectedRegistrars(effectiveTldId, selectedRegistrarIds);
            if (editingId === null && createPreviewRegistrarCostRows.length > 0) {
                const ensureResponse = await apiRequest(`${getApiBaseUrl()}/RegistrarTldCostPricing/tld/${effectiveTldId}/current/ensure`, {
                    method: 'POST',
                });
                if (!ensureResponse.success) {
                    throw new Error(ensureResponse.message || 'Failed to store registrar cost prices for the new TLD.');
                }
            }
            await upsertSalesPricingForTld(effectiveTldId);
        }
        catch (error) {
            const message = error instanceof Error ? error.message : 'Failed to save TLD pricing data';
            showError(message);
            return;
        }
        hideModal('tlds-edit-modal');
        showSuccess(editingId !== null ? 'TLD updated successfully' : 'TLD created successfully');
        await loadTlds();
    }
    function openDelete(id, displayName) {
        pendingDeleteId = id;
        const deleteName = document.getElementById('tlds-delete-name');
        if (deleteName) {
            deleteName.textContent = displayName;
        }
        showModal('tlds-delete-modal');
    }
    async function doDelete() {
        if (pendingDeleteId === null) {
            return;
        }
        const response = await apiRequest(`${getApiBaseUrl()}/Tlds/${pendingDeleteId}`, { method: 'DELETE' });
        hideModal('tlds-delete-modal');
        if (!response.success) {
            showError(response.message || 'Delete failed');
            pendingDeleteId = null;
            return;
        }
        showSuccess('TLD soft-deleted successfully');
        if (allTlds.length === 1 && currentPage > 1) {
            currentPage -= 1;
        }
        pendingDeleteId = null;
        await loadTlds();
    }
    function changePage(page) {
        if (page < 1 || page > totalPages) {
            return;
        }
        currentPage = page;
        void loadTlds();
    }
    function bindPagingControlsActions() {
        const container = document.getElementById('tlds-paging-controls');
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
            const rawPage = link.dataset.page ?? '';
            const page = Number(rawPage);
            if (Number.isFinite(page)) {
                changePage(page);
            }
        });
    }
    function bindTableActions() {
        const tableBody = document.getElementById('tlds-table-body');
        if (!tableBody) {
            return;
        }
        tableBody.addEventListener('click', (event) => {
            const target = event.target;
            const button = target.closest('button[data-action]');
            if (!button) {
                return;
            }
            const id = Number(button.dataset.id ?? '0');
            if (!Number.isFinite(id) || id <= 0) {
                return;
            }
            if (button.dataset.action === 'edit') {
                void openEdit(id);
                return;
            }
            if (button.dataset.action === 'delete') {
                openDelete(id, button.dataset.name ?? '');
            }
        });
    }
    function initializeTldsPage() {
        const page = document.getElementById('tlds-page');
        if (!page || page.dataset.initialized === 'true') {
            return;
        }
        page.dataset.initialized = 'true';
        document.getElementById('tlds-create')?.addEventListener('click', openCreate);
        document.getElementById('tlds-save')?.addEventListener('click', () => { void saveTld(); });
        document.getElementById('tlds-confirm-delete')?.addEventListener('click', () => { void doDelete(); });
        document.getElementById('tlds-refresh')?.addEventListener('click', () => { void loadTlds(); });
        document.getElementById('tlds-filter-registrar')?.addEventListener('change', () => {
            selectedRegistrarFilterId = getRegistrarFilterId();
            currentPage = 1;
            void loadTlds();
        });
        document.getElementById('tlds-page-size')?.addEventListener('change', () => {
            currentPage = 1;
            void loadTlds();
        });
        document.getElementById('tlds-extension')?.addEventListener('blur', () => {
            if (editingId !== null) {
                return;
            }
            const extensionInput = document.getElementById('tlds-extension');
            const extension = extensionInput?.value ?? '';
            void previewRegistrarPricesForCreate(extension);
        });
        bindPagingControlsActions();
        bindTableActions();
        void (async () => {
            await loadActiveRegistrars();
            selectedRegistrarFilterId = getRegistrarFilterId();
            await loadTlds();
        })();
    }
    function setupPageObserver() {
        initializeTldsPage();
        if (document.body) {
            const observer = new MutationObserver(() => {
                const page = document.getElementById('tlds-page');
                if (page && page.dataset.initialized !== 'true') {
                    initializeTldsPage();
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
//# sourceMappingURL=tlds.js.map
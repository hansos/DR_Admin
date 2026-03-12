"use strict";
(() => {
    const sourceLabels = {
        0: 'Manual',
        1: 'ECB',
        2: 'OpenExchangeRates',
        3: 'Fixer',
        4: 'CurrencyLayer',
        5: 'XE',
        6: 'ExchangeRateHost',
        7: 'Frankfurter',
        8: 'OANDA',
        99: 'Other',
    };
    let allCurrencies = [];
    let allRates = [];
    let editingRateId = null;
    let pendingDeleteRateId = null;
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
    const getBootstrap = () => {
        const maybeBootstrap = window.bootstrap;
        return maybeBootstrap ?? null;
    };
    const esc = (text) => {
        const map = { '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#039;' };
        return (text || '').replace(/[&<>"']/g, (char) => map[char] ?? char);
    };
    const apiRequest = async (endpoint, options = {}) => {
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
                return {
                    success: false,
                    message: (data && (data.message ?? data.title)) ||
                        `Request failed with status ${response.status}`,
                };
            }
            const parsed = data;
            return {
                success: parsed?.success !== false,
                data: parsed?.data ?? data,
                message: parsed?.message,
            };
        }
        catch (error) {
            console.error('Currencies request failed', error);
            return {
                success: false,
                message: 'Network error. Please try again.',
            };
        }
    };
    const showSuccess = (message) => {
        const alert = document.getElementById('currency-management-alert-success');
        if (!alert) {
            return;
        }
        alert.textContent = message;
        alert.classList.remove('d-none');
        document.getElementById('currency-management-alert-error')?.classList.add('d-none');
        setTimeout(() => alert.classList.add('d-none'), 5000);
    };
    const showError = (message) => {
        const alert = document.getElementById('currency-management-alert-error');
        if (!alert) {
            return;
        }
        alert.textContent = message;
        alert.classList.remove('d-none');
        document.getElementById('currency-management-alert-success')?.classList.add('d-none');
    };
    const showModal = (id) => {
        const element = document.getElementById(id);
        const bootstrap = getBootstrap();
        if (!element || !bootstrap) {
            return;
        }
        const modal = new bootstrap.Modal(element);
        modal.show();
    };
    const hideModal = (id) => {
        const element = document.getElementById(id);
        const bootstrap = getBootstrap();
        if (!element || !bootstrap) {
            return;
        }
        const modal = bootstrap.Modal.getInstance(element);
        modal?.hide();
    };
    const normalizeCurrency = (item) => {
        const typed = (item ?? {});
        return {
            id: Number(typed.id ?? typed.Id ?? 0),
            code: String(typed.code ?? typed.Code ?? ''),
            name: String(typed.name ?? typed.Name ?? ''),
            symbol: String(typed.symbol ?? typed.Symbol ?? ''),
            isActive: Boolean(typed.isActive ?? typed.IsActive ?? false),
            isDefault: Boolean(typed.isDefault ?? typed.IsDefault ?? false),
            sortOrder: Number(typed.sortOrder ?? typed.SortOrder ?? 0),
        };
    };
    const parseSourceValue = (value) => {
        if (typeof value === 'number' && Number.isFinite(value)) {
            return value;
        }
        if (typeof value === 'string') {
            const numeric = Number(value);
            if (Number.isFinite(numeric)) {
                return numeric;
            }
            const entry = Object.entries(sourceLabels).find(([, label]) => label.toLowerCase() === value.toLowerCase());
            if (entry) {
                return Number(entry[0]);
            }
        }
        return 0;
    };
    const normalizeRate = (item) => {
        const typed = (item ?? {});
        const source = parseSourceValue(typed.source ?? typed.Source);
        return {
            id: Number(typed.id ?? typed.Id ?? 0),
            baseCurrency: String(typed.baseCurrency ?? typed.BaseCurrency ?? ''),
            targetCurrency: String(typed.targetCurrency ?? typed.TargetCurrency ?? ''),
            rate: Number(typed.rate ?? typed.Rate ?? 0),
            effectiveRate: Number(typed.effectiveRate ?? typed.EffectiveRate ?? 0),
            effectiveDate: String(typed.effectiveDate ?? typed.EffectiveDate ?? ''),
            expiryDate: typed.expiryDate == null && typed.ExpiryDate == null
                ? null
                : String(typed.expiryDate ?? typed.ExpiryDate ?? ''),
            source,
            sourceLabel: sourceLabels[source] ?? 'Manual',
            isActive: Boolean(typed.isActive ?? typed.IsActive ?? false),
            markup: Number(typed.markup ?? typed.Markup ?? 0),
            notes: String(typed.notes ?? typed.Notes ?? ''),
        };
    };
    const formatDateTime = (value) => {
        if (!value) {
            return '-';
        }
        const date = new Date(value);
        if (Number.isNaN(date.getTime())) {
            return '-';
        }
        return date.toLocaleString();
    };
    const toDateTimeLocalInput = (value) => {
        if (!value) {
            return '';
        }
        const date = new Date(value);
        if (Number.isNaN(date.getTime())) {
            return '';
        }
        const year = date.getFullYear();
        const month = String(date.getMonth() + 1).padStart(2, '0');
        const day = String(date.getDate()).padStart(2, '0');
        const hour = String(date.getHours()).padStart(2, '0');
        const minute = String(date.getMinutes()).padStart(2, '0');
        return `${year}-${month}-${day}T${hour}:${minute}`;
    };
    const fromDateTimeLocalInput = (value) => {
        if (!value) {
            return null;
        }
        const date = new Date(value);
        if (Number.isNaN(date.getTime())) {
            return null;
        }
        return date.toISOString();
    };
    const setInputValue = (id, value) => {
        const input = document.getElementById(id);
        if (input) {
            input.value = value;
        }
    };
    const getInputValue = (id) => {
        const input = document.getElementById(id);
        return input?.value.trim() ?? '';
    };
    const renderCurrencyTable = () => {
        const tableBody = document.getElementById('currencies-table-body');
        if (!tableBody) {
            return;
        }
        if (!allCurrencies.length) {
            tableBody.innerHTML = '<tr><td colspan="4" class="text-center text-muted">No currencies found.</td></tr>';
            return;
        }
        const sorted = [...allCurrencies].sort((a, b) => a.sortOrder - b.sortOrder || a.code.localeCompare(b.code));
        tableBody.innerHTML = sorted.map((currency) => `
            <tr>
                <td><code>${esc(currency.code)}</code></td>
                <td>${esc(currency.name)}</td>
                <td>${currency.isDefault ? '<span class="badge bg-primary">Yes</span>' : '-'}</td>
                <td>${currency.isActive ? '<span class="badge bg-success">Yes</span>' : '<span class="badge bg-secondary">No</span>'}</td>
            </tr>
        `).join('');
    };
    const renderCurrencyOptions = () => {
        const baseSelect = document.getElementById('currency-rates-base');
        const targetSelect = document.getElementById('currency-rates-target');
        if (!baseSelect || !targetSelect) {
            return;
        }
        const options = allCurrencies
            .filter((x) => x.isActive)
            .sort((a, b) => a.code.localeCompare(b.code))
            .map((x) => `<option value="${esc(x.code)}">${esc(x.code)} - ${esc(x.name)}</option>`)
            .join('');
        baseSelect.innerHTML = options;
        targetSelect.innerHTML = options;
        const defaultCurrency = allCurrencies.find((x) => x.isDefault)?.code ?? allCurrencies[0]?.code ?? '';
        setInputValue('currency-rates-base', defaultCurrency);
        const secondaryCurrency = allCurrencies.find((x) => x.code !== defaultCurrency)?.code ?? defaultCurrency;
        setInputValue('currency-rates-target', secondaryCurrency);
    };
    const renderRatesTable = () => {
        const tableBody = document.getElementById('currency-rates-table-body');
        if (!tableBody) {
            return;
        }
        if (!allRates.length) {
            tableBody.innerHTML = '<tr><td colspan="10" class="text-center text-muted">No exchange rates found.</td></tr>';
            return;
        }
        const sorted = [...allRates].sort((a, b) => {
            if (a.baseCurrency !== b.baseCurrency) {
                return a.baseCurrency.localeCompare(b.baseCurrency);
            }
            if (a.targetCurrency !== b.targetCurrency) {
                return a.targetCurrency.localeCompare(b.targetCurrency);
            }
            return new Date(b.effectiveDate).getTime() - new Date(a.effectiveDate).getTime();
        });
        tableBody.innerHTML = sorted.map((rate) => `
            <tr>
                <td>${rate.id}</td>
                <td><code>${esc(rate.baseCurrency)}</code> → <code>${esc(rate.targetCurrency)}</code></td>
                <td>${esc(rate.rate.toFixed(6))}</td>
                <td>${esc(rate.effectiveRate.toFixed(6))}</td>
                <td>${esc(rate.markup.toFixed(4))}</td>
                <td>${esc(formatDateTime(rate.effectiveDate))}</td>
                <td>${esc(formatDateTime(rate.expiryDate))}</td>
                <td>${esc(rate.sourceLabel)}</td>
                <td>${rate.isActive ? '<span class="badge bg-success">Yes</span>' : '<span class="badge bg-secondary">No</span>'}</td>
                <td class="text-end">
                    <div class="btn-group btn-group-sm">
                        <button class="btn btn-outline-primary" type="button" data-action="edit" data-id="${rate.id}"><i class="bi bi-pencil"></i></button>
                        <button class="btn btn-outline-danger" type="button" data-action="delete" data-id="${rate.id}" data-name="${esc(`${rate.baseCurrency}/${rate.targetCurrency}`)}"><i class="bi bi-trash"></i></button>
                    </div>
                </td>
            </tr>
        `).join('');
    };
    const loadCurrencies = async () => {
        const response = await apiRequest(`${getApiBaseUrl()}/Currencies`, { method: 'GET' });
        if (!response.success) {
            showError(response.message || 'Failed to load currencies.');
            return;
        }
        const raw = response.data;
        const list = Array.isArray(raw)
            ? raw
            : Array.isArray(raw?.data)
                ? (raw.data ?? [])
                : [];
        allCurrencies = list.map((item) => normalizeCurrency(item));
        renderCurrencyTable();
        renderCurrencyOptions();
    };
    const loadRates = async () => {
        const tableBody = document.getElementById('currency-rates-table-body');
        if (tableBody) {
            tableBody.innerHTML = '<tr><td colspan="10" class="text-center"><div class="spinner-border text-primary"></div></td></tr>';
        }
        const response = await apiRequest(`${getApiBaseUrl()}/Currencies/rates`, { method: 'GET' });
        if (!response.success) {
            showError(response.message || 'Failed to load exchange rates.');
            if (tableBody) {
                tableBody.innerHTML = '<tr><td colspan="10" class="text-center text-danger">Failed to load data</td></tr>';
            }
            return;
        }
        const raw = response.data;
        const list = Array.isArray(raw)
            ? raw
            : Array.isArray(raw?.data)
                ? (raw.data ?? [])
                : [];
        allRates = list.map((item) => normalizeRate(item));
        renderRatesTable();
    };
    const setCurrencyFieldDisabledState = (disabled) => {
        const baseSelect = document.getElementById('currency-rates-base');
        const targetSelect = document.getElementById('currency-rates-target');
        if (baseSelect) {
            baseSelect.disabled = disabled;
        }
        if (targetSelect) {
            targetSelect.disabled = disabled;
        }
    };
    const openCreate = () => {
        editingRateId = null;
        const title = document.getElementById('currency-rates-modal-title');
        if (title) {
            title.textContent = 'New Exchange Rate';
        }
        document.getElementById('currency-rates-form')?.reset();
        setCurrencyFieldDisabledState(false);
        const now = new Date();
        now.setSeconds(0, 0);
        setInputValue('currency-rates-effective-date', toDateTimeLocalInput(now.toISOString()));
        setInputValue('currency-rates-expiry-date', '');
        setInputValue('currency-rates-rate', '1');
        setInputValue('currency-rates-markup', '0');
        setInputValue('currency-rates-source', '0');
        const isActiveInput = document.getElementById('currency-rates-is-active');
        if (isActiveInput) {
            isActiveInput.checked = true;
        }
        renderCurrencyOptions();
        showModal('currency-rates-edit-modal');
    };
    const openEdit = (id) => {
        const item = allRates.find((x) => x.id === id);
        if (!item) {
            return;
        }
        editingRateId = id;
        const title = document.getElementById('currency-rates-modal-title');
        if (title) {
            title.textContent = 'Edit Exchange Rate';
        }
        setInputValue('currency-rates-base', item.baseCurrency);
        setInputValue('currency-rates-target', item.targetCurrency);
        setInputValue('currency-rates-rate', String(item.rate));
        setInputValue('currency-rates-markup', String(item.markup));
        setInputValue('currency-rates-effective-date', toDateTimeLocalInput(item.effectiveDate));
        setInputValue('currency-rates-expiry-date', toDateTimeLocalInput(item.expiryDate));
        setInputValue('currency-rates-source', String(item.source));
        setInputValue('currency-rates-notes', item.notes || '');
        const isActiveInput = document.getElementById('currency-rates-is-active');
        if (isActiveInput) {
            isActiveInput.checked = item.isActive;
        }
        setCurrencyFieldDisabledState(true);
        showModal('currency-rates-edit-modal');
    };
    const saveRate = async () => {
        const baseCurrency = getInputValue('currency-rates-base').toUpperCase();
        const targetCurrency = getInputValue('currency-rates-target').toUpperCase();
        const rate = Number(getInputValue('currency-rates-rate'));
        const markup = Number(getInputValue('currency-rates-markup') || '0');
        const effectiveDate = fromDateTimeLocalInput(getInputValue('currency-rates-effective-date'));
        const expiryDate = fromDateTimeLocalInput(getInputValue('currency-rates-expiry-date'));
        const source = Number(getInputValue('currency-rates-source'));
        const isActive = document.getElementById('currency-rates-is-active')?.checked ?? false;
        const notes = getInputValue('currency-rates-notes');
        if (!baseCurrency || !targetCurrency) {
            showError('Base and target currencies are required.');
            return;
        }
        if (baseCurrency === targetCurrency) {
            showError('Base and target currencies must be different.');
            return;
        }
        if (!Number.isFinite(rate) || rate <= 0) {
            showError('Rate must be greater than 0.');
            return;
        }
        if (!Number.isFinite(markup) || markup < 0) {
            showError('Markup must be 0 or greater.');
            return;
        }
        if (!effectiveDate) {
            showError('Effective date is required.');
            return;
        }
        if (expiryDate && new Date(expiryDate).getTime() <= new Date(effectiveDate).getTime()) {
            showError('Expiry date must be after effective date.');
            return;
        }
        const payload = editingRateId
            ? {
                rate,
                effectiveDate,
                expiryDate,
                source,
                isActive,
                markup,
                notes,
            }
            : {
                baseCurrency,
                targetCurrency,
                rate,
                effectiveDate,
                expiryDate,
                source,
                isActive,
                markup,
                notes,
            };
        const response = editingRateId
            ? await apiRequest(`${getApiBaseUrl()}/Currencies/rates/${editingRateId}`, {
                method: 'PUT',
                body: JSON.stringify(payload),
            })
            : await apiRequest(`${getApiBaseUrl()}/Currencies/rates`, {
                method: 'POST',
                body: JSON.stringify(payload),
            });
        if (!response.success) {
            showError(response.message || 'Failed to save exchange rate.');
            return;
        }
        hideModal('currency-rates-edit-modal');
        showSuccess(editingRateId ? 'Exchange rate updated successfully.' : 'Exchange rate created successfully.');
        await loadRates();
    };
    const openDelete = (id, pairName) => {
        pendingDeleteRateId = id;
        const nameElement = document.getElementById('currency-rates-delete-name');
        if (nameElement) {
            nameElement.textContent = pairName;
        }
        showModal('currency-rates-delete-modal');
    };
    const doDelete = async () => {
        if (!pendingDeleteRateId) {
            return;
        }
        const response = await apiRequest(`${getApiBaseUrl()}/Currencies/rates/${pendingDeleteRateId}`, {
            method: 'DELETE',
        });
        hideModal('currency-rates-delete-modal');
        if (!response.success) {
            showError(response.message || 'Failed to delete exchange rate.');
            pendingDeleteRateId = null;
            return;
        }
        showSuccess('Exchange rate deleted successfully.');
        pendingDeleteRateId = null;
        await loadRates();
    };
    const forceUpdateRates = async () => {
        const button = document.getElementById('currency-rates-force-update');
        const originalHtml = button?.innerHTML ?? '';
        if (button) {
            button.disabled = true;
            button.innerHTML = '<span class="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>Updating...';
        }
        try {
            const response = await apiRequest(`${getApiBaseUrl()}/Currencies/rates/force-update`, {
                method: 'POST',
            });
            if (!response.success) {
                showError(response.message || 'Failed to force update exchange rates.');
                return;
            }
            const changedRates = Number(response.data?.changedRates ?? 0);
            const safeChangedRates = Number.isFinite(changedRates) && changedRates >= 0 ? changedRates : 0;
            showSuccess(`Exchange rates updated. Changed rates: ${safeChangedRates}.`);
            await loadRates();
        }
        finally {
            if (button) {
                button.disabled = false;
                button.innerHTML = originalHtml;
            }
        }
    };
    const bindEvents = () => {
        document.getElementById('currency-rates-force-update')?.addEventListener('click', () => { void forceUpdateRates(); });
        document.getElementById('currency-rates-create')?.addEventListener('click', openCreate);
        document.getElementById('currency-rates-save')?.addEventListener('click', () => { void saveRate(); });
        document.getElementById('currency-rates-confirm-delete')?.addEventListener('click', () => { void doDelete(); });
        document.getElementById('currency-rates-table-body')?.addEventListener('click', (event) => {
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
                openEdit(id);
                return;
            }
            if (button.dataset.action === 'delete') {
                openDelete(id, button.dataset.name ?? '');
            }
        });
    };
    const initializePage = async () => {
        const page = document.getElementById('currency-management-page');
        if (!page || page.dataset.initialized === 'true') {
            return;
        }
        page.dataset.initialized = 'true';
        bindEvents();
        await loadCurrencies();
        await loadRates();
    };
    const setupObserver = () => {
        void initializePage();
        if (document.body) {
            const observer = new MutationObserver(() => {
                const page = document.getElementById('currency-management-page');
                if (page && page.dataset.initialized !== 'true') {
                    void initializePage();
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
//# sourceMappingURL=currencies.js.map
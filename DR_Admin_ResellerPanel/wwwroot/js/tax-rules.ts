(() => {
    interface AppSettings {
        apiBaseUrl?: string;
    }

    interface ApiResponse<T> {
        success: boolean;
        data?: T;
        message?: string;
    }

    interface TaxRule {
        id: number;
        countryCode: string;
        stateCode: string;
        taxCategoryId: number | null;
        taxName: string;
        taxCategory: string;
        taxRate: number;
        isActive: boolean;
        effectiveFrom: string;
        effectiveUntil: string;
        appliesToSetupFees: boolean;
        appliesToRecurring: boolean;
        reverseCharge: boolean;
        taxAuthority: string;
        taxRegistrationNumber: string;
        priority: number;
        internalNotes: string;
    }

    interface TaxCategory {
        id: number;
        countryCode: string;
        stateCode: string;
        code: string;
        name: string;
        isActive: boolean;
    }

    interface CountryOption {
        code: string;
        englishName: string;
        isActive: boolean;
    }

    interface BootstrapModalInstance {
        show(): void;
        hide(): void;
    }

    interface BootstrapModalConstructor {
        new (element: Element): BootstrapModalInstance;
        getInstance(element: Element): BootstrapModalInstance | null;
    }

    interface BootstrapNamespace {
        Modal: BootstrapModalConstructor;
    }

    const pageId = 'tax-rules-page';
    let allRules: TaxRule[] = [];
    let allCategories: TaxCategory[] = [];
    let allCountries: CountryOption[] = [];
    let editingId: number | null = null;
    let pendingDeleteId: number | null = null;

    function getApiBaseUrl(): string {
        const settings = (window as Window & { AppSettings?: AppSettings }).AppSettings;
        return settings?.apiBaseUrl ?? '';
    }

    function getAuthToken(): string | null {
        const auth = (window as Window & { Auth?: { getToken?: () => string | null } }).Auth;
        if (auth?.getToken) {
            return auth.getToken();
        }

        return sessionStorage.getItem('rp_authToken');
    }

    function getBootstrap(): BootstrapNamespace | null {
        const maybeBootstrap = (window as Window & { bootstrap?: BootstrapNamespace }).bootstrap;
        return maybeBootstrap ?? null;
    }

    function esc(value: string): string {
        const map: Record<string, string> = { '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#039;' };
        return (value || '').replace(/[&<>"']/g, (c) => map[c] ?? c);
    }

    async function apiRequest(endpoint: string, options: RequestInit = {}): Promise<ApiResponse<unknown>> {
        try {
            const headers: Record<string, string> = {
                'Content-Type': 'application/json',
                ...(options.headers as Record<string, string> | undefined),
            };
            const token = getAuthToken();
            if (token) {
                headers.Authorization = `Bearer ${token}`;
            }

            const response = await fetch(endpoint, { ...options, headers, credentials: 'include' });
            const contentType = response.headers.get('content-type') ?? '';
            const payload = contentType.includes('application/json') ? await response.json() : null;

            if (!response.ok) {
                const body = (payload ?? {}) as { message?: string; title?: string };
                return { success: false, message: body.message ?? body.title ?? `Request failed with status ${response.status}` };
            }

            const wrapped = (payload ?? {}) as { data?: unknown; success?: boolean; message?: string };
            return { success: wrapped.success !== false, data: wrapped.data ?? payload, message: wrapped.message };
        } catch {
            return { success: false, message: 'Network error. Please try again.' };
        }
    }

    function extractItems(payload: unknown): Record<string, unknown>[] {
        if (Array.isArray(payload)) {
            return payload as Record<string, unknown>[];
        }

        const obj = (payload ?? {}) as { data?: unknown; Data?: unknown };
        if (Array.isArray(obj.data)) {
            return obj.data as Record<string, unknown>[];
        }

        if (Array.isArray(obj.Data)) {
            return obj.Data as Record<string, unknown>[];
        }

        return [];
    }

    function showSuccess(message: string): void {
        const success = document.getElementById('tax-rules-alert-success');
        const error = document.getElementById('tax-rules-alert-error');
        if (success) {
            success.textContent = message;
            success.classList.remove('d-none');
        }
        error?.classList.add('d-none');
    }

    function showError(message: string): void {
        const success = document.getElementById('tax-rules-alert-success');
        const error = document.getElementById('tax-rules-alert-error');
        if (error) {
            error.textContent = message;
            error.classList.remove('d-none');
        }
        success?.classList.add('d-none');
    }

    function normalizeItem(item: Record<string, unknown>): TaxRule {
        const taxCategoryIdRaw = item.taxCategoryId ?? item.TaxCategoryId;
        const taxCategoryIdNumber = Number(taxCategoryIdRaw ?? 0);

        return {
            id: Number(item.id ?? item.Id ?? 0),
            countryCode: String(item.countryCode ?? item.CountryCode ?? ''),
            stateCode: String(item.stateCode ?? item.StateCode ?? ''),
            taxCategoryId: Number.isFinite(taxCategoryIdNumber) && taxCategoryIdNumber > 0 ? taxCategoryIdNumber : null,
            taxName: String(item.taxName ?? item.TaxName ?? ''),
            taxCategory: String(item.taxCategory ?? item.TaxCategory ?? ''),
            taxRate: Number(item.taxRate ?? item.TaxRate ?? 0),
            isActive: Boolean(item.isActive ?? item.IsActive ?? false),
            effectiveFrom: String(item.effectiveFrom ?? item.EffectiveFrom ?? ''),
            effectiveUntil: String(item.effectiveUntil ?? item.EffectiveUntil ?? ''),
            appliesToSetupFees: Boolean(item.appliesToSetupFees ?? item.AppliesToSetupFees ?? false),
            appliesToRecurring: Boolean(item.appliesToRecurring ?? item.AppliesToRecurring ?? false),
            reverseCharge: Boolean(item.reverseCharge ?? item.ReverseCharge ?? false),
            taxAuthority: String(item.taxAuthority ?? item.TaxAuthority ?? ''),
            taxRegistrationNumber: String(item.taxRegistrationNumber ?? item.TaxRegistrationNumber ?? ''),
            priority: Number(item.priority ?? item.Priority ?? 0),
            internalNotes: String(item.internalNotes ?? item.InternalNotes ?? ''),
        };
    }

    function normalizeCountry(item: Record<string, unknown>): CountryOption {
        return {
            code: String(item.code ?? item.Code ?? '').toUpperCase(),
            englishName: String(item.englishName ?? item.EnglishName ?? ''),
            isActive: Boolean(item.isActive ?? item.IsActive ?? false),
        };
    }

    function renderCountryOptions(selectedCountryCode?: string): void {
        const countrySelect = document.getElementById('tax-rules-country') as HTMLSelectElement | null;
        if (!countrySelect) {
            return;
        }

        const selected = (selectedCountryCode ?? '').trim().toUpperCase();
        const options = ['<option value="">Select country</option>'];

        allCountries
            .filter((country) => country.isActive)
            .sort((a, b) => a.englishName.localeCompare(b.englishName))
            .forEach((country) => {
                const isSelected = selected && country.code === selected ? ' selected' : '';
                options.push(`<option value="${esc(country.code)}"${isSelected}>${esc(country.code)} - ${esc(country.englishName)}</option>`);
            });

        countrySelect.innerHTML = options.join('');
    }

    function normalizeCategory(item: Record<string, unknown>): TaxCategory {
        return {
            id: Number(item.id ?? item.Id ?? 0),
            countryCode: String(item.countryCode ?? item.CountryCode ?? ''),
            stateCode: String(item.stateCode ?? item.StateCode ?? ''),
            code: String(item.code ?? item.Code ?? ''),
            name: String(item.name ?? item.Name ?? ''),
            isActive: Boolean(item.isActive ?? item.IsActive ?? false),
        };
    }

    function formatPercent(value: unknown): string {
        const rate = Number(value ?? 0);
        if (!Number.isFinite(rate)) {
            return '-';
        }

        return `${(rate * 100).toFixed(3).replace(/\.?0+$/, '')}%`;
    }

    function formatDateRange(fromValue: unknown, toValue: unknown): string {
        const fromDate = new Date(String(fromValue ?? ''));
        const toDate = new Date(String(toValue ?? ''));

        const fromText = Number.isNaN(fromDate.getTime()) ? '-' : fromDate.toLocaleDateString();
        const toText = Number.isNaN(toDate.getTime()) ? 'Open' : toDate.toLocaleDateString();
        return `${fromText} - ${toText}`;
    }

    function renderRows(): void {
        const tbody = document.getElementById('tax-rules-table-body');
        if (!tbody) {
            return;
        }

        if (allRules.length === 0) {
            tbody.innerHTML = '<tr><td colspan="8" class="text-center text-muted">No records found.</td></tr>';
            return;
        }

        tbody.innerHTML = allRules.map((item) => {
            const stateCode = item.stateCode || '-';
            const dateRange = formatDateRange(item.effectiveFrom, item.effectiveUntil);
            const label = `${item.countryCode}/${item.taxCategory}`;

            return `<tr>
                <td>${item.id}</td>
                <td>${esc(item.countryCode)}</td>
                <td>${esc(stateCode)}</td>
                <td>${esc(item.taxCategory)}</td>
                <td>${formatPercent(item.taxRate)}</td>
                <td>${dateRange}</td>
                <td>${item.priority}</td>
                <td class="text-end">
                    <div class="btn-group btn-group-sm">
                        <button class="btn btn-outline-primary" type="button" data-action="edit" data-id="${item.id}"><i class="bi bi-pencil"></i></button>
                        <button class="btn btn-outline-danger" type="button" data-action="delete" data-id="${item.id}" data-name="${esc(label)}"><i class="bi bi-trash"></i></button>
                    </div>
                </td>
            </tr>`;
        }).join('');
    }

    function setInputValue(id: string, value: string): void {
        const el = document.getElementById(id) as HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement | null;
        if (el) {
            el.value = value;
        }
    }

    function getInputValue(id: string): string {
        const el = document.getElementById(id) as HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement | null;
        return el?.value.trim() ?? '';
    }

    function getCheckboxValue(id: string): boolean {
        const el = document.getElementById(id) as HTMLInputElement | null;
        return Boolean(el?.checked);
    }

    function setCheckboxValue(id: string, checked: boolean): void {
        const el = document.getElementById(id) as HTMLInputElement | null;
        if (el) {
            el.checked = checked;
        }
    }

    function showModal(id: string): void {
        const element = document.getElementById(id);
        const bootstrap = getBootstrap();
        if (!element || !bootstrap) {
            return;
        }

        new bootstrap.Modal(element).show();
    }

    function hideModal(id: string): void {
        const element = document.getElementById(id);
        const bootstrap = getBootstrap();
        if (!element || !bootstrap) {
            return;
        }

        bootstrap.Modal.getInstance(element)?.hide();
    }

    function toInputDateTime(value: string): string {
        if (!value) {
            return '';
        }

        const date = new Date(value);
        if (Number.isNaN(date.getTime())) {
            return '';
        }

        const offset = date.getTimezoneOffset();
        const local = new Date(date.getTime() - (offset * 60000));
        return local.toISOString().slice(0, 16);
    }

    function fromInputDateTime(value: string): string | null {
        if (!value) {
            return null;
        }

        const date = new Date(value);
        return Number.isNaN(date.getTime()) ? null : date.toISOString();
    }

    function getRelevantCategories(countryCodeRaw: string, stateCodeRaw: string): TaxCategory[] {
        const countryCode = countryCodeRaw.trim().toUpperCase();
        const stateCode = stateCodeRaw.trim().toUpperCase();
        if (!countryCode) {
            return [];
        }

        return allCategories
            .filter((item) => item.isActive)
            .filter((item) => item.countryCode.toUpperCase() === countryCode)
            .filter((item) => {
                const itemState = item.stateCode.trim().toUpperCase();
                if (!stateCode) {
                    return itemState.length === 0;
                }

                return itemState.length === 0 || itemState === stateCode;
            })
            .sort((a, b) => a.code.localeCompare(b.code));
    }

    function populateCategoryCombo(countryCode: string, stateCode: string, selectedCategoryId: number | null, selectedCategoryCode: string): void {
        const combo = document.getElementById('tax-rules-category') as HTMLSelectElement | null;
        if (!combo) {
            return;
        }

        const categories = getRelevantCategories(countryCode, stateCode);
        const selectedCode = selectedCategoryCode.trim().toUpperCase();

        const options: string[] = [];
        options.push('<option value="">Select category</option>');
        categories.forEach((item) => {
            const selectedById = selectedCategoryId !== null && item.id === selectedCategoryId;
            const selectedByCode = selectedCategoryId === null && selectedCode && item.code.toUpperCase() === selectedCode;
            const selected = selectedById || selectedByCode ? ' selected' : '';
            options.push(`<option value="${item.id}" data-code="${esc(item.code)}"${selected}>${esc(item.code)} - ${esc(item.name)}</option>`);
        });

        combo.innerHTML = options.join('');

        if (combo.value === '' && selectedCode) {
            const manualOption = document.createElement('option');
            manualOption.value = '';
            manualOption.text = selectedCode;
            manualOption.selected = true;
            combo.add(manualOption);
        }
    }

    function syncCategoryIdInputFromCombo(): void {
        const combo = document.getElementById('tax-rules-category') as HTMLSelectElement | null;
        const categoryIdInput = document.getElementById('tax-rules-category-id') as HTMLInputElement | null;
        if (!combo || !categoryIdInput) {
            return;
        }

        categoryIdInput.value = combo.value;
    }

    function getSelectedCategoryCode(): string {
        const combo = document.getElementById('tax-rules-category') as HTMLSelectElement | null;
        if (!combo || combo.selectedIndex < 0) {
            return '';
        }

        const selectedOption = combo.options[combo.selectedIndex];
        const code = selectedOption.getAttribute('data-code');
        return (code ?? '').trim().toUpperCase();
    }

    function openCreate(): void {
        editingId = null;
        const title = document.getElementById('tax-rules-modal-title');
        if (title) {
            title.textContent = 'New Tax Rule';
        }

        (document.getElementById('tax-rules-form') as HTMLFormElement | null)?.reset();
        renderCountryOptions();
        setInputValue('tax-rules-effective-from', toInputDateTime(new Date().toISOString()));
        setInputValue('tax-rules-priority', '0');
        setCheckboxValue('tax-rules-is-active', true);
        setCheckboxValue('tax-rules-applies-setup', true);
        setCheckboxValue('tax-rules-applies-recurring', true);
        setInputValue('tax-rules-category-id', '');
        populateCategoryCombo('', '', null, '');
        const countryField = document.getElementById('tax-rules-country') as HTMLInputElement | null;
        const stateField = document.getElementById('tax-rules-state') as HTMLInputElement | null;
        if (countryField) {
            countryField.disabled = false;
        }
        if (stateField) {
            stateField.disabled = false;
        }

        showModal('tax-rules-edit-modal');
    }

    function openEdit(id: number): void {
        const item = allRules.find((x) => x.id === id);
        if (!item) {
            return;
        }

        editingId = id;
        const title = document.getElementById('tax-rules-modal-title');
        if (title) {
            title.textContent = 'Edit Tax Rule';
        }

        renderCountryOptions(item.countryCode);
        setInputValue('tax-rules-country', item.countryCode);
        setInputValue('tax-rules-state', item.stateCode);
        setInputValue('tax-rules-category-id', item.taxCategoryId ? String(item.taxCategoryId) : '');
        populateCategoryCombo(item.countryCode, item.stateCode, item.taxCategoryId, item.taxCategory);
        setInputValue('tax-rules-name', item.taxName);
        setInputValue('tax-rules-rate', String(item.taxRate));
        setInputValue('tax-rules-priority', String(item.priority));
        setInputValue('tax-rules-effective-from', toInputDateTime(item.effectiveFrom));
        setInputValue('tax-rules-effective-until', toInputDateTime(item.effectiveUntil));
        setInputValue('tax-rules-authority', item.taxAuthority);
        setInputValue('tax-rules-registration-number', item.taxRegistrationNumber);
        setInputValue('tax-rules-notes', item.internalNotes);
        setCheckboxValue('tax-rules-is-active', item.isActive);
        setCheckboxValue('tax-rules-applies-setup', item.appliesToSetupFees);
        setCheckboxValue('tax-rules-applies-recurring', item.appliesToRecurring);
        setCheckboxValue('tax-rules-reverse-charge', item.reverseCharge);

        const countryField = document.getElementById('tax-rules-country') as HTMLInputElement | null;
        const stateField = document.getElementById('tax-rules-state') as HTMLInputElement | null;
        if (countryField) {
            countryField.disabled = true;
        }
        if (stateField) {
            stateField.disabled = true;
        }

        showModal('tax-rules-edit-modal');
    }

    async function saveItem(): Promise<void> {
        const taxName = getInputValue('tax-rules-name');
        const taxCategory = getSelectedCategoryCode();
        const taxRate = Number(getInputValue('tax-rules-rate'));
        const effectiveFrom = fromInputDateTime(getInputValue('tax-rules-effective-from'));

        if (!taxName || !taxCategory || !Number.isFinite(taxRate) || taxRate < 0 || !effectiveFrom) {
            showError('Tax name, category, tax rate and effective from are required.');
            return;
        }

        const taxCategoryIdRaw = Number(getInputValue('tax-rules-category-id'));
        const payloadBase = {
            taxCategoryId: Number.isFinite(taxCategoryIdRaw) && taxCategoryIdRaw > 0 ? taxCategoryIdRaw : null,
            taxName,
            taxCategory,
            taxRate,
            isActive: getCheckboxValue('tax-rules-is-active'),
            effectiveFrom,
            effectiveUntil: fromInputDateTime(getInputValue('tax-rules-effective-until')),
            appliesToSetupFees: getCheckboxValue('tax-rules-applies-setup'),
            appliesToRecurring: getCheckboxValue('tax-rules-applies-recurring'),
            reverseCharge: getCheckboxValue('tax-rules-reverse-charge'),
            taxAuthority: getInputValue('tax-rules-authority'),
            taxRegistrationNumber: getInputValue('tax-rules-registration-number'),
            priority: Number(getInputValue('tax-rules-priority') || '0'),
            internalNotes: getInputValue('tax-rules-notes'),
        };

        let response: ApiResponse<unknown>;
        if (editingId) {
            response = await apiRequest(`${getApiBaseUrl()}/TaxRules/${editingId}`, { method: 'PUT', body: JSON.stringify(payloadBase) });
        } else {
            const countryCode = getInputValue('tax-rules-country').toUpperCase();
            if (!countryCode) {
                showError('Country is required for new tax rules.');
                return;
            }

            const createPayload = {
                ...payloadBase,
                countryCode,
                stateCode: getInputValue('tax-rules-state') || null,
            };

            response = await apiRequest(`${getApiBaseUrl()}/TaxRules`, { method: 'POST', body: JSON.stringify(createPayload) });
        }

        if (!response.success) {
            showError(response.message ?? 'Failed to save tax rule.');
            return;
        }

        hideModal('tax-rules-edit-modal');
        showSuccess(editingId ? 'Tax rule updated.' : 'Tax rule created.');
        await loadData(false);
    }

    function openDelete(id: number, name: string): void {
        pendingDeleteId = id;
        const label = document.getElementById('tax-rules-delete-name');
        if (label) {
            label.textContent = name;
        }
        showModal('tax-rules-delete-modal');
    }

    async function doDelete(): Promise<void> {
        if (!pendingDeleteId) {
            return;
        }

        const response = await apiRequest(`${getApiBaseUrl()}/TaxRules/${pendingDeleteId}`, { method: 'DELETE' });
        hideModal('tax-rules-delete-modal');

        if (!response.success) {
            showError(response.message ?? 'Failed to delete tax rule.');
            pendingDeleteId = null;
            return;
        }

        pendingDeleteId = null;
        showSuccess('Tax rule deleted.');
        await loadData(false);
    }

    async function loadData(showFeedback: boolean): Promise<void> {
        const tbody = document.getElementById('tax-rules-table-body');
        if (tbody) {
            tbody.innerHTML = '<tr><td colspan="8" class="text-center"><div class="spinner-border text-primary"></div></td></tr>';
        }

        const [rulesResponse, categoriesResponse, countriesResponse] = await Promise.all([
            apiRequest(`${getApiBaseUrl()}/TaxRules`, { method: 'GET' }),
            apiRequest(`${getApiBaseUrl()}/TaxCategories`, { method: 'GET' }),
            apiRequest(`${getApiBaseUrl()}/Countries`, { method: 'GET' }),
        ]);

        if (!countriesResponse.success) {
            showError(countriesResponse.message ?? 'Failed to load countries.');
            return;
        }

        allCountries = extractItems(countriesResponse.data).map((item) => normalizeCountry(item));
        renderCountryOptions(getInputValue('tax-rules-country'));

        if (!categoriesResponse.success) {
            showError(categoriesResponse.message ?? 'Failed to load tax categories.');
            return;
        }

        allCategories = extractItems(categoriesResponse.data).map((item) => normalizeCategory(item));

        if (!rulesResponse.success) {
            showError(rulesResponse.message ?? 'Failed to load tax rules.');
            return;
        }

        allRules = extractItems(rulesResponse.data).map((item) => normalizeItem(item));
        renderRows();
        if (showFeedback) {
            showSuccess(`Loaded at ${new Date().toLocaleString()}`);
        }
    }

    function initializePage(): void {
        const page = document.getElementById(pageId);
        if (!page || page.dataset.initialized === 'true') {
            return;
        }

        page.dataset.initialized = 'true';

        document.getElementById('tax-rules-create')?.addEventListener('click', openCreate);
        document.getElementById('tax-rules-save')?.addEventListener('click', () => {
            void saveItem();
        });
        document.getElementById('tax-rules-confirm-delete')?.addEventListener('click', () => {
            void doDelete();
        });

        document.getElementById('tax-rules-country')?.addEventListener('change', () => {
            populateCategoryCombo(getInputValue('tax-rules-country'), getInputValue('tax-rules-state'), null, '');
            syncCategoryIdInputFromCombo();
        });

        document.getElementById('tax-rules-state')?.addEventListener('input', () => {
            populateCategoryCombo(getInputValue('tax-rules-country'), getInputValue('tax-rules-state'), null, '');
            syncCategoryIdInputFromCombo();
        });

        document.getElementById('tax-rules-category')?.addEventListener('change', () => {
            syncCategoryIdInputFromCombo();
        });

        document.getElementById('tax-rules-table-body')?.addEventListener('click', (event) => {
            const target = event.target as HTMLElement;
            const actionElement = target.closest('[data-action]') as HTMLElement | null;
            if (!actionElement) {
                return;
            }

            const action = actionElement.dataset.action;
            const id = Number(actionElement.dataset.id ?? '0');
            if (!Number.isFinite(id) || id <= 0) {
                return;
            }

            if (action === 'edit') {
                openEdit(id);
                return;
            }

            if (action === 'delete') {
                openDelete(id, actionElement.dataset.name ?? `#${id}`);
            }
        });

        document.getElementById('tax-rules-refresh')?.addEventListener('click', async () => {
            await loadData(true);
        });

        void loadData(false);
    }

    function setup(): void {
        initializePage();

        if (!document.body) {
            return;
        }

        const observer = new MutationObserver(() => initializePage());
        observer.observe(document.body, { childList: true, subtree: true });
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', setup);
    } else {
        setup();
    }
})();

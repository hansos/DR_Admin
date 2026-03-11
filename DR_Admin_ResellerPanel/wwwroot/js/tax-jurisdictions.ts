(() => {
    interface AppSettings {
        apiBaseUrl?: string;
    }

    interface ApiResponse<T> {
        success: boolean;
        data?: T;
        message?: string;
    }

    interface TaxJurisdiction {
        id: number;
        code: string;
        name: string;
        countryCode: string;
        stateCode: string;
        taxAuthority: string;
        taxCurrencyCode: string;
        isActive: boolean;
        notes: string;
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

    const pageId = 'tax-jurisdictions-page';
    let allJurisdictions: TaxJurisdiction[] = [];
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

            const response = await fetch(endpoint, {
                ...options,
                headers,
                credentials: 'include',
            });

            const contentType = response.headers.get('content-type') ?? '';
            const hasJson = contentType.includes('application/json');
            const payload = hasJson ? await response.json() : null;

            if (!response.ok) {
                const body = (payload ?? {}) as { message?: string; title?: string };
                return {
                    success: false,
                    message: body.message ?? body.title ?? `Request failed with status ${response.status}`,
                };
            }

            const wrapped = (payload ?? {}) as { data?: unknown; success?: boolean; message?: string };
            return {
                success: wrapped.success !== false,
                data: wrapped.data ?? payload,
                message: wrapped.message,
            };
        } catch {
            return {
                success: false,
                message: 'Network error. Please try again.',
            };
        }
    }

    function esc(value: string): string {
        const map: Record<string, string> = { '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#039;' };
        return (value || '').replace(/[&<>"']/g, (c) => map[c] ?? c);
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
        const success = document.getElementById('tax-jurisdictions-alert-success');
        const error = document.getElementById('tax-jurisdictions-alert-error');
        if (success) {
            success.textContent = message;
            success.classList.remove('d-none');
        }
        error?.classList.add('d-none');
    }

    function showError(message: string): void {
        const success = document.getElementById('tax-jurisdictions-alert-success');
        const error = document.getElementById('tax-jurisdictions-alert-error');
        if (error) {
            error.textContent = message;
            error.classList.remove('d-none');
        }
        success?.classList.add('d-none');
    }

    function normalizeItem(item: Record<string, unknown>): TaxJurisdiction {
        return {
            id: Number(item.id ?? item.Id ?? 0),
            code: String(item.code ?? item.Code ?? ''),
            name: String(item.name ?? item.Name ?? ''),
            countryCode: String(item.countryCode ?? item.CountryCode ?? ''),
            stateCode: String(item.stateCode ?? item.StateCode ?? ''),
            taxAuthority: String(item.taxAuthority ?? item.TaxAuthority ?? ''),
            taxCurrencyCode: String(item.taxCurrencyCode ?? item.TaxCurrencyCode ?? ''),
            isActive: Boolean(item.isActive ?? item.IsActive ?? false),
            notes: String(item.notes ?? item.Notes ?? ''),
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
        const countrySelect = document.getElementById('tax-jurisdictions-country') as HTMLSelectElement | null;
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

    function renderRows(): void {
        const tbody = document.getElementById('tax-jurisdictions-table-body');
        if (!tbody) {
            return;
        }

        if (allJurisdictions.length === 0) {
            tbody.innerHTML = '<tr><td colspan="7" class="text-center text-muted">No records found.</td></tr>';
            return;
        }

        tbody.innerHTML = allJurisdictions.map((item) => {
            const state = item.stateCode || '-';

            return `<tr>
                <td>${item.id}</td>
                <td>${esc(item.code)}</td>
                <td>${esc(item.name)}</td>
                <td>${esc(item.countryCode)}</td>
                <td>${esc(state)}</td>
                <td>${item.isActive ? 'Active' : 'Inactive'}</td>
                <td class="text-end">
                    <div class="btn-group btn-group-sm">
                        <button class="btn btn-outline-primary" type="button" data-action="edit" data-id="${item.id}"><i class="bi bi-pencil"></i></button>
                        <button class="btn btn-outline-danger" type="button" data-action="delete" data-id="${item.id}" data-name="${esc(item.name)}"><i class="bi bi-trash"></i></button>
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

    function openCreate(): void {
        editingId = null;
        const title = document.getElementById('tax-jurisdictions-modal-title');
        if (title) {
            title.textContent = 'New Tax Jurisdiction';
        }

        (document.getElementById('tax-jurisdictions-form') as HTMLFormElement | null)?.reset();
        renderCountryOptions();
        setInputValue('tax-jurisdictions-currency', 'EUR');
        showModal('tax-jurisdictions-edit-modal');
    }

    function openEdit(id: number): void {
        const item = allJurisdictions.find((x) => x.id === id);
        if (!item) {
            return;
        }

        editingId = id;
        const title = document.getElementById('tax-jurisdictions-modal-title');
        if (title) {
            title.textContent = 'Edit Tax Jurisdiction';
        }

        setInputValue('tax-jurisdictions-code', item.code);
        setInputValue('tax-jurisdictions-name', item.name);
        renderCountryOptions(item.countryCode);
        setInputValue('tax-jurisdictions-state', item.stateCode);
        setInputValue('tax-jurisdictions-authority', item.taxAuthority);
        setInputValue('tax-jurisdictions-currency', item.taxCurrencyCode);
        setInputValue('tax-jurisdictions-notes', item.notes);
        const isActive = document.getElementById('tax-jurisdictions-is-active') as HTMLInputElement | null;
        if (isActive) {
            isActive.checked = item.isActive;
        }

        showModal('tax-jurisdictions-edit-modal');
    }

    async function saveItem(): Promise<void> {
        const code = getInputValue('tax-jurisdictions-code').toUpperCase();
        const name = getInputValue('tax-jurisdictions-name');
        const countryCode = getInputValue('tax-jurisdictions-country').toUpperCase();
        const taxAuthority = getInputValue('tax-jurisdictions-authority');
        const taxCurrencyCode = getInputValue('tax-jurisdictions-currency').toUpperCase();

        if (!code || !name || !countryCode || !taxAuthority || !taxCurrencyCode) {
            showError('Code, Name, Country, Tax authority and Tax currency are required.');
            return;
        }

        const payload = {
            code,
            name,
            countryCode,
            stateCode: getInputValue('tax-jurisdictions-state') || null,
            taxAuthority,
            taxCurrencyCode,
            isActive: getCheckboxValue('tax-jurisdictions-is-active'),
            notes: getInputValue('tax-jurisdictions-notes'),
        };

        const response = editingId
            ? await apiRequest(`${getApiBaseUrl()}/TaxJurisdictions/${editingId}`, { method: 'PUT', body: JSON.stringify(payload) })
            : await apiRequest(`${getApiBaseUrl()}/TaxJurisdictions`, { method: 'POST', body: JSON.stringify(payload) });

        if (!response.success) {
            showError(response.message ?? 'Failed to save tax jurisdiction.');
            return;
        }

        hideModal('tax-jurisdictions-edit-modal');
        showSuccess(editingId ? 'Tax jurisdiction updated.' : 'Tax jurisdiction created.');
        await loadData(false);
    }

    function openDelete(id: number, name: string): void {
        pendingDeleteId = id;
        const label = document.getElementById('tax-jurisdictions-delete-name');
        if (label) {
            label.textContent = name;
        }

        showModal('tax-jurisdictions-delete-modal');
    }

    async function doDelete(): Promise<void> {
        if (!pendingDeleteId) {
            return;
        }

        const response = await apiRequest(`${getApiBaseUrl()}/TaxJurisdictions/${pendingDeleteId}`, { method: 'DELETE' });
        hideModal('tax-jurisdictions-delete-modal');

        if (!response.success) {
            showError(response.message ?? 'Failed to delete tax jurisdiction.');
            pendingDeleteId = null;
            return;
        }

        pendingDeleteId = null;
        showSuccess('Tax jurisdiction deleted.');
        await loadData(false);
    }

    async function loadData(showFeedback: boolean): Promise<void> {
        const tbody = document.getElementById('tax-jurisdictions-table-body');
        if (tbody) {
            tbody.innerHTML = '<tr><td colspan="7" class="text-center"><div class="spinner-border text-primary"></div></td></tr>';
        }

        const response = await apiRequest(`${getApiBaseUrl()}/TaxJurisdictions`, { method: 'GET' });
        const countriesResponse = await apiRequest(`${getApiBaseUrl()}/Countries`, { method: 'GET' });

        if (!countriesResponse.success) {
            showError(countriesResponse.message ?? 'Failed to load countries.');
            return;
        }

        allCountries = extractItems(countriesResponse.data).map((item) => normalizeCountry(item));
        renderCountryOptions();

        if (!response.success) {
            showError(response.message ?? 'Failed to load tax jurisdictions.');
            return;
        }

        allJurisdictions = extractItems(response.data).map((item) => normalizeItem(item));
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

        document.getElementById('tax-jurisdictions-create')?.addEventListener('click', openCreate);
        document.getElementById('tax-jurisdictions-save')?.addEventListener('click', () => {
            void saveItem();
        });
        document.getElementById('tax-jurisdictions-confirm-delete')?.addEventListener('click', () => {
            void doDelete();
        });

        document.getElementById('tax-jurisdictions-table-body')?.addEventListener('click', (event) => {
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

        document.getElementById('tax-jurisdictions-refresh')?.addEventListener('click', async () => {
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

(() => {
    interface AppSettings {
        apiBaseUrl?: string;
    }

    interface ApiResponse<T> {
        success: boolean;
        data?: T;
        message?: string;
    }

    interface TaxCategory {
        id: number;
        countryCode: string;
        stateCode: string;
        code: string;
        name: string;
        description: string;
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

    const pageId = 'tax-categories-page';
    let allCategories: TaxCategory[] = [];
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
        const success = document.getElementById('tax-categories-alert-success');
        const error = document.getElementById('tax-categories-alert-error');
        if (success) {
            success.textContent = message;
            success.classList.remove('d-none');
        }
        error?.classList.add('d-none');
    }

    function showError(message: string): void {
        const success = document.getElementById('tax-categories-alert-success');
        const error = document.getElementById('tax-categories-alert-error');
        if (error) {
            error.textContent = message;
            error.classList.remove('d-none');
        }
        success?.classList.add('d-none');
    }

    function normalizeItem(item: Record<string, unknown>): TaxCategory {
        return {
            id: Number(item.id ?? item.Id ?? 0),
            countryCode: String(item.countryCode ?? item.CountryCode ?? ''),
            stateCode: String(item.stateCode ?? item.StateCode ?? ''),
            code: String(item.code ?? item.Code ?? ''),
            name: String(item.name ?? item.Name ?? ''),
            description: String(item.description ?? item.Description ?? ''),
            isActive: Boolean(item.isActive ?? item.IsActive ?? false),
        };
    }

    function renderRows(): void {
        const tbody = document.getElementById('tax-categories-table-body');
        if (!tbody) {
            return;
        }

        if (allCategories.length === 0) {
            tbody.innerHTML = '<tr><td colspan="7" class="text-center text-muted">No records found.</td></tr>';
            return;
        }

        tbody.innerHTML = allCategories.map((item) => {
            const stateCode = item.stateCode || '-';

            return `<tr>
                <td>${item.id}</td>
                <td>${esc(item.countryCode)}</td>
                <td>${esc(stateCode)}</td>
                <td>${esc(item.code)}</td>
                <td>${esc(item.name)}</td>
                <td>${item.isActive ? 'Active' : 'Inactive'}</td>
                <td class="text-end">
                    <div class="btn-group btn-group-sm">
                        <button class="btn btn-outline-primary" type="button" data-action="edit" data-id="${item.id}"><i class="bi bi-pencil"></i></button>
                        <button class="btn btn-outline-danger" type="button" data-action="delete" data-id="${item.id}" data-name="${esc(item.code)}"><i class="bi bi-trash"></i></button>
                    </div>
                </td>
            </tr>`;
        }).join('');
    }

    function setInputValue(id: string, value: string): void {
        const el = document.getElementById(id) as HTMLInputElement | HTMLTextAreaElement | null;
        if (el) {
            el.value = value;
        }
    }

    function getInputValue(id: string): string {
        const el = document.getElementById(id) as HTMLInputElement | HTMLTextAreaElement | null;
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
        const title = document.getElementById('tax-categories-modal-title');
        if (title) {
            title.textContent = 'New Tax Category';
        }

        (document.getElementById('tax-categories-form') as HTMLFormElement | null)?.reset();
        showModal('tax-categories-edit-modal');
    }

    function openEdit(id: number): void {
        const item = allCategories.find((x) => x.id === id);
        if (!item) {
            return;
        }

        editingId = id;
        const title = document.getElementById('tax-categories-modal-title');
        if (title) {
            title.textContent = 'Edit Tax Category';
        }

        setInputValue('tax-categories-country', item.countryCode);
        setInputValue('tax-categories-state', item.stateCode);
        setInputValue('tax-categories-code', item.code);
        setInputValue('tax-categories-name', item.name);
        setInputValue('tax-categories-description', item.description);
        const active = document.getElementById('tax-categories-is-active') as HTMLInputElement | null;
        if (active) {
            active.checked = item.isActive;
        }

        showModal('tax-categories-edit-modal');
    }

    async function saveItem(): Promise<void> {
        const countryCode = getInputValue('tax-categories-country').toUpperCase();
        const code = getInputValue('tax-categories-code').toUpperCase();
        const name = getInputValue('tax-categories-name');

        if (!countryCode || !code || !name) {
            showError('Country, Code and Name are required.');
            return;
        }

        const payload = {
            countryCode,
            stateCode: getInputValue('tax-categories-state') || null,
            code,
            name,
            description: getInputValue('tax-categories-description'),
            isActive: getCheckboxValue('tax-categories-is-active'),
        };

        const response = editingId
            ? await apiRequest(`${getApiBaseUrl()}/TaxCategories/${editingId}`, { method: 'PUT', body: JSON.stringify(payload) })
            : await apiRequest(`${getApiBaseUrl()}/TaxCategories`, { method: 'POST', body: JSON.stringify(payload) });

        if (!response.success) {
            showError(response.message ?? 'Failed to save tax category.');
            return;
        }

        hideModal('tax-categories-edit-modal');
        showSuccess(editingId ? 'Tax category updated.' : 'Tax category created.');
        await loadData(false);
    }

    function openDelete(id: number, name: string): void {
        pendingDeleteId = id;
        const label = document.getElementById('tax-categories-delete-name');
        if (label) {
            label.textContent = name;
        }
        showModal('tax-categories-delete-modal');
    }

    async function doDelete(): Promise<void> {
        if (!pendingDeleteId) {
            return;
        }

        const response = await apiRequest(`${getApiBaseUrl()}/TaxCategories/${pendingDeleteId}`, { method: 'DELETE' });
        hideModal('tax-categories-delete-modal');

        if (!response.success) {
            showError(response.message ?? 'Failed to delete tax category.');
            pendingDeleteId = null;
            return;
        }

        pendingDeleteId = null;
        showSuccess('Tax category deleted.');
        await loadData(false);
    }

    async function loadData(showFeedback: boolean): Promise<void> {
        const tbody = document.getElementById('tax-categories-table-body');
        if (tbody) {
            tbody.innerHTML = '<tr><td colspan="7" class="text-center"><div class="spinner-border text-primary"></div></td></tr>';
        }

        const response = await apiRequest(`${getApiBaseUrl()}/TaxCategories`, { method: 'GET' });
        if (!response.success) {
            showError(response.message ?? 'Failed to load tax categories.');
            return;
        }

        allCategories = extractItems(response.data).map((item) => normalizeItem(item));
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

        document.getElementById('tax-categories-create')?.addEventListener('click', openCreate);
        document.getElementById('tax-categories-save')?.addEventListener('click', () => {
            void saveItem();
        });
        document.getElementById('tax-categories-confirm-delete')?.addEventListener('click', () => {
            void doDelete();
        });

        document.getElementById('tax-categories-table-body')?.addEventListener('click', (event) => {
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

        document.getElementById('tax-categories-refresh')?.addEventListener('click', async () => {
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

(() => {
    interface AppSettings {
        apiBaseUrl?: string;
    }

    interface ApiResponse<T> {
        success: boolean;
        data?: T;
        message?: string;
    }

    interface BillingCycle {
        id: number;
        code: string;
        name: string;
        durationInDays: number;
        description: string;
        sortOrder: number;
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

    let allBillingCycles: BillingCycle[] = [];
    let editingId: number | null = null;
    let pendingDeleteId: number | null = null;

    const getApiBaseUrl = (): string => {
        const settings = (window as Window & { AppSettings?: AppSettings }).AppSettings;
        return settings?.apiBaseUrl ?? '';
    };

    const getAuthToken = (): string | null => {
        const auth = (window as Window & { Auth?: { getToken?: () => string | null } }).Auth;
        if (auth?.getToken) {
            return auth.getToken();
        }

        return sessionStorage.getItem('rp_authToken');
    };

    const getBootstrap = (): BootstrapNamespace | null => {
        const maybeBootstrap = (window as Window & { bootstrap?: BootstrapNamespace }).bootstrap;
        return maybeBootstrap ?? null;
    };

    const esc = (text: string): string => {
        const map: Record<string, string> = { '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#039;' };
        return (text || '').replace(/[&<>"']/g, (char) => map[char] ?? char);
    };

    const apiRequest = async <T,>(endpoint: string, options: RequestInit = {}): Promise<ApiResponse<T>> => {
        try {
            const headers: Record<string, string> = {
                'Content-Type': 'application/json',
                ...(options.headers as Record<string, string> | undefined),
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
                    message: (data && ((data as { message?: string }).message ?? (data as { title?: string }).title)) ||
                        `Request failed with status ${response.status}`,
                };
            }

            const parsed = data as { success?: boolean; data?: T; message?: string } | null;
            return {
                success: parsed?.success !== false,
                data: parsed?.data ?? (data as T),
                message: parsed?.message,
            };
        } catch (error) {
            console.error('Billing cycles request failed', error);
            return {
                success: false,
                message: 'Network error. Please try again.',
            };
        }
    };

    const showSuccess = (message: string): void => {
        const alert = document.getElementById('billing-cycles-alert-success');
        if (!alert) {
            return;
        }

        alert.textContent = message;
        alert.classList.remove('d-none');
        document.getElementById('billing-cycles-alert-error')?.classList.add('d-none');

        setTimeout(() => alert.classList.add('d-none'), 5000);
    };

    const showError = (message: string): void => {
        const alert = document.getElementById('billing-cycles-alert-error');
        if (!alert) {
            return;
        }

        alert.textContent = message;
        alert.classList.remove('d-none');
        document.getElementById('billing-cycles-alert-success')?.classList.add('d-none');
    };

    const showModal = (id: string): void => {
        const element = document.getElementById(id);
        const bootstrap = getBootstrap();
        if (!element || !bootstrap) {
            return;
        }

        const modal = new bootstrap.Modal(element);
        modal.show();
    };

    const hideModal = (id: string): void => {
        const element = document.getElementById(id);
        const bootstrap = getBootstrap();
        if (!element || !bootstrap) {
            return;
        }

        const modal = bootstrap.Modal.getInstance(element);
        modal?.hide();
    };

    const normalizeBillingCycle = (item: unknown): BillingCycle => {
        const typed = (item ?? {}) as Record<string, unknown>;

        return {
            id: Number(typed.id ?? typed.Id ?? 0),
            code: String(typed.code ?? typed.Code ?? ''),
            name: String(typed.name ?? typed.Name ?? ''),
            durationInDays: Number(typed.durationInDays ?? typed.DurationInDays ?? 0),
            description: String(typed.description ?? typed.Description ?? ''),
            sortOrder: Number(typed.sortOrder ?? typed.SortOrder ?? 0),
        };
    };

    const renderTable = (): void => {
        const tableBody = document.getElementById('billing-cycles-table-body');
        if (!tableBody) {
            return;
        }

        if (!allBillingCycles.length) {
            tableBody.innerHTML = '<tr><td colspan="7" class="text-center text-muted">No billing cycles found.</td></tr>';
            return;
        }

        const sorted = [...allBillingCycles].sort((a, b) => a.sortOrder - b.sortOrder || a.durationInDays - b.durationInDays || a.name.localeCompare(b.name));

        tableBody.innerHTML = sorted.map((cycle) => `
            <tr>
                <td>${cycle.id}</td>
                <td><code>${esc(cycle.code)}</code></td>
                <td>${esc(cycle.name)}</td>
                <td>${esc(String(cycle.durationInDays))}</td>
                <td>${esc(String(cycle.sortOrder))}</td>
                <td>${esc(cycle.description || '-')}</td>
                <td class="text-end">
                    <div class="btn-group btn-group-sm">
                        <button class="btn btn-outline-primary" type="button" data-action="edit" data-id="${cycle.id}"><i class="bi bi-pencil"></i></button>
                        <button class="btn btn-outline-danger" type="button" data-action="delete" data-id="${cycle.id}" data-name="${esc(cycle.name)}"><i class="bi bi-trash"></i></button>
                    </div>
                </td>
            </tr>
        `).join('');
    };

    const loadBillingCycles = async (): Promise<void> => {
        const tableBody = document.getElementById('billing-cycles-table-body');
        if (tableBody) {
            tableBody.innerHTML = '<tr><td colspan="7" class="text-center"><div class="spinner-border text-primary"></div></td></tr>';
        }

        const response = await apiRequest<unknown>(`${getApiBaseUrl()}/BillingCycles`, { method: 'GET' });
        if (!response.success) {
            showError(response.message || 'Failed to load billing cycles.');
            if (tableBody) {
                tableBody.innerHTML = '<tr><td colspan="7" class="text-center text-danger">Failed to load data</td></tr>';
            }
            return;
        }

        const raw = response.data;
        const list = Array.isArray(raw)
            ? raw
            : Array.isArray((raw as { data?: unknown[] } | null)?.data)
                ? ((raw as { data?: unknown[] }).data ?? [])
                : [];

        allBillingCycles = list.map((item) => normalizeBillingCycle(item));
        renderTable();
    };

    const getInputValue = (id: string): string => {
        const input = document.getElementById(id) as HTMLInputElement | HTMLTextAreaElement | null;
        return input?.value.trim() ?? '';
    };

    const setInputValue = (id: string, value: string): void => {
        const input = document.getElementById(id) as HTMLInputElement | HTMLTextAreaElement | null;
        if (input) {
            input.value = value;
        }
    };

    const openCreate = (): void => {
        editingId = null;

        const title = document.getElementById('billing-cycles-modal-title');
        if (title) {
            title.textContent = 'New Billing Cycle';
        }

        (document.getElementById('billing-cycles-form') as HTMLFormElement | null)?.reset();
        setInputValue('billing-cycles-sort-order', String(Math.max(0, allBillingCycles.length * 10)));

        showModal('billing-cycles-edit-modal');
    };

    const openEdit = (id: number): void => {
        const item = allBillingCycles.find((x) => x.id === id);
        if (!item) {
            return;
        }

        editingId = id;

        const title = document.getElementById('billing-cycles-modal-title');
        if (title) {
            title.textContent = 'Edit Billing Cycle';
        }

        setInputValue('billing-cycles-code', item.code);
        setInputValue('billing-cycles-name', item.name);
        setInputValue('billing-cycles-duration', String(item.durationInDays));
        setInputValue('billing-cycles-sort-order', String(item.sortOrder));
        setInputValue('billing-cycles-description', item.description || '');

        showModal('billing-cycles-edit-modal');
    };

    const saveBillingCycle = async (): Promise<void> => {
        const code = getInputValue('billing-cycles-code');
        const name = getInputValue('billing-cycles-name');
        const durationInDays = Number(getInputValue('billing-cycles-duration'));
        const sortOrder = Number(getInputValue('billing-cycles-sort-order'));

        if (!code || !name) {
            showError('Code and Name are required.');
            return;
        }

        if (!Number.isFinite(durationInDays) || durationInDays <= 0) {
            showError('Duration in days must be greater than 0.');
            return;
        }

        if (!Number.isFinite(sortOrder) || sortOrder < 0) {
            showError('Sort order must be 0 or greater.');
            return;
        }

        const payload = {
            code,
            name,
            durationInDays,
            description: getInputValue('billing-cycles-description'),
            sortOrder,
        };

        const response = editingId
            ? await apiRequest<unknown>(`${getApiBaseUrl()}/BillingCycles/${editingId}`, { method: 'PUT', body: JSON.stringify(payload) })
            : await apiRequest<unknown>(`${getApiBaseUrl()}/BillingCycles`, { method: 'POST', body: JSON.stringify(payload) });

        if (!response.success) {
            showError(response.message || 'Failed to save billing cycle.');
            return;
        }

        hideModal('billing-cycles-edit-modal');
        showSuccess(editingId ? 'Billing cycle updated successfully.' : 'Billing cycle created successfully.');
        await loadBillingCycles();
    };

    const openDelete = (id: number, name: string): void => {
        pendingDeleteId = id;
        const deleteName = document.getElementById('billing-cycles-delete-name');
        if (deleteName) {
            deleteName.textContent = name;
        }

        showModal('billing-cycles-delete-modal');
    };

    const doDelete = async (): Promise<void> => {
        if (!pendingDeleteId) {
            return;
        }

        const response = await apiRequest<unknown>(`${getApiBaseUrl()}/BillingCycles/${pendingDeleteId}`, { method: 'DELETE' });
        hideModal('billing-cycles-delete-modal');

        if (!response.success) {
            showError(response.message || 'Failed to delete billing cycle.');
            pendingDeleteId = null;
            return;
        }

        showSuccess('Billing cycle deleted successfully.');
        pendingDeleteId = null;
        await loadBillingCycles();
    };

    const bindEvents = (): void => {
        document.getElementById('billing-cycles-create')?.addEventListener('click', openCreate);
        document.getElementById('billing-cycles-save')?.addEventListener('click', () => { void saveBillingCycle(); });
        document.getElementById('billing-cycles-confirm-delete')?.addEventListener('click', () => { void doDelete(); });

        document.getElementById('billing-cycles-table-body')?.addEventListener('click', (event) => {
            const target = event.target as HTMLElement;
            const button = target.closest<HTMLButtonElement>('button[data-action]');
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

    const initializePage = async (): Promise<void> => {
        const page = document.getElementById('billing-cycles-page') as HTMLElement | null;
        if (!page || page.dataset.initialized === 'true') {
            return;
        }

        page.dataset.initialized = 'true';

        bindEvents();
        await loadBillingCycles();
    };

    const setupObserver = (): void => {
        void initializePage();

        if (document.body) {
            const observer = new MutationObserver(() => {
                const page = document.getElementById('billing-cycles-page') as HTMLElement | null;
                if (page && page.dataset.initialized !== 'true') {
                    void initializePage();
                }
            });

            observer.observe(document.body, { childList: true, subtree: true });
        }
    };

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', setupObserver);
    } else {
        setupObserver();
    }
})();

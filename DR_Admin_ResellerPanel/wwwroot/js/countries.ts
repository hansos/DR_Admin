(() => {
    interface AppSettings {
        apiBaseUrl?: string;
    }

    interface ApiResponse<T> {
        success: boolean;
        data?: T;
        message?: string;
    }

    interface Country {
        id: number;
        code: string;
        tld: string;
        iso3: string;
        numeric: number | null;
        englishName: string;
        localName: string;
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

    let allCountries: Country[] = [];
    let filteredCountries: Country[] = [];
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
            console.error('Countries request failed', error);
            return {
                success: false,
                message: 'Network error. Please try again.',
            };
        }
    };

    const showSuccess = (message: string): void => {
        const alert = document.getElementById('countries-alert-success');
        if (!alert) {
            return;
        }

        alert.textContent = message;
        alert.classList.remove('d-none');
        document.getElementById('countries-alert-error')?.classList.add('d-none');

        setTimeout(() => alert.classList.add('d-none'), 5000);
    };

    const showError = (message: string): void => {
        const alert = document.getElementById('countries-alert-error');
        if (!alert) {
            return;
        }

        alert.textContent = message;
        alert.classList.remove('d-none');
        document.getElementById('countries-alert-success')?.classList.add('d-none');
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

    const normalizeCountry = (item: unknown): Country => {
        const typed = (item ?? {}) as Record<string, unknown>;

        return {
            id: Number(typed.id ?? typed.Id ?? 0),
            code: String(typed.code ?? typed.Code ?? ''),
            tld: String(typed.tld ?? typed.Tld ?? ''),
            iso3: String(typed.iso3 ?? typed.Iso3 ?? ''),
            numeric: typed.numeric == null && typed.Numeric == null
                ? null
                : Number(typed.numeric ?? typed.Numeric ?? 0),
            englishName: String(typed.englishName ?? typed.EnglishName ?? ''),
            localName: String(typed.localName ?? typed.LocalName ?? ''),
            isActive: Boolean(typed.isActive ?? typed.IsActive ?? false),
        };
    };

    const getInputValue = (id: string): string => {
        const input = document.getElementById(id) as HTMLInputElement | null;
        return input?.value.trim() ?? '';
    };

    const setInputValue = (id: string, value: string): void => {
        const input = document.getElementById(id) as HTMLInputElement | null;
        if (input) {
            input.value = value;
        }
    };

    const applyFilters = (): void => {
        const search = getInputValue('countries-filter-search').toLowerCase();
        const activeFilter = getInputValue('countries-filter-active');

        filteredCountries = allCountries.filter((country) => {
            if (activeFilter === 'true' && !country.isActive) {
                return false;
            }

            if (activeFilter === 'false' && country.isActive) {
                return false;
            }

            if (!search) {
                return true;
            }

            return [country.code, country.tld, country.iso3, country.englishName, country.localName]
                .join(' ')
                .toLowerCase()
                .includes(search);
        });

        renderTable();
    };

    const renderTable = (): void => {
        const tableBody = document.getElementById('countries-table-body');
        if (!tableBody) {
            return;
        }

        if (!filteredCountries.length) {
            tableBody.innerHTML = '<tr><td colspan="9" class="text-center text-muted">No countries found.</td></tr>';
            return;
        }

        const sorted = [...filteredCountries].sort((a, b) => a.englishName.localeCompare(b.englishName));

        tableBody.innerHTML = sorted.map((country) => `
            <tr>
                <td>${country.id}</td>
                <td><code>${esc(country.code)}</code></td>
                <td><code>.${esc(country.tld)}</code></td>
                <td>${esc(country.iso3 || '-')}</td>
                <td>${country.numeric ?? '-'}</td>
                <td>${esc(country.englishName)}</td>
                <td>${esc(country.localName)}</td>
                <td>${country.isActive ? '<span class="badge bg-success">Yes</span>' : '<span class="badge bg-secondary">No</span>'}</td>
                <td class="text-end">
                    <div class="btn-group btn-group-sm">
                        <button class="btn btn-outline-primary" type="button" data-action="edit" data-id="${country.id}"><i class="bi bi-pencil"></i></button>
                        <button class="btn btn-outline-danger" type="button" data-action="delete" data-id="${country.id}" data-name="${esc(country.englishName)}"><i class="bi bi-trash"></i></button>
                    </div>
                </td>
            </tr>
        `).join('');
    };

    const loadCountries = async (): Promise<void> => {
        const tableBody = document.getElementById('countries-table-body');
        if (tableBody) {
            tableBody.innerHTML = '<tr><td colspan="9" class="text-center"><div class="spinner-border text-primary"></div></td></tr>';
        }

        const response = await apiRequest<unknown>(`${getApiBaseUrl()}/Countries`, { method: 'GET' });
        if (!response.success) {
            showError(response.message || 'Failed to load countries.');
            if (tableBody) {
                tableBody.innerHTML = '<tr><td colspan="9" class="text-center text-danger">Failed to load data</td></tr>';
            }
            return;
        }

        const raw = response.data;
        const list = Array.isArray(raw)
            ? raw
            : Array.isArray((raw as { data?: unknown[] } | null)?.data)
                ? ((raw as { data?: unknown[] }).data ?? [])
                : Array.isArray((raw as { Data?: unknown[] } | null)?.Data)
                    ? ((raw as { Data?: unknown[] }).Data ?? [])
                    : [];

        allCountries = list.map((item) => normalizeCountry(item));
        applyFilters();
    };

    const openCreate = (): void => {
        editingId = null;

        const title = document.getElementById('countries-modal-title');
        if (title) {
            title.textContent = 'New Country';
        }

        (document.getElementById('countries-form') as HTMLFormElement | null)?.reset();
        const active = document.getElementById('countries-is-active') as HTMLInputElement | null;
        if (active) {
            active.checked = true;
        }

        showModal('countries-edit-modal');
    };

    const openEdit = (id: number): void => {
        const country = allCountries.find((x) => x.id === id);
        if (!country) {
            return;
        }

        editingId = id;

        const title = document.getElementById('countries-modal-title');
        if (title) {
            title.textContent = 'Edit Country';
        }

        setInputValue('countries-code', country.code);
        setInputValue('countries-tld', country.tld);
        setInputValue('countries-iso3', country.iso3);
        setInputValue('countries-numeric', country.numeric == null ? '' : String(country.numeric));
        setInputValue('countries-english-name', country.englishName);
        setInputValue('countries-local-name', country.localName);

        const active = document.getElementById('countries-is-active') as HTMLInputElement | null;
        if (active) {
            active.checked = country.isActive;
        }

        showModal('countries-edit-modal');
    };

    const saveCountry = async (): Promise<void> => {
        const code = getInputValue('countries-code').toUpperCase();
        const tld = getInputValue('countries-tld').toLowerCase();
        const iso3Raw = getInputValue('countries-iso3').toUpperCase();
        const numericRaw = getInputValue('countries-numeric');
        const englishName = getInputValue('countries-english-name');
        const localName = getInputValue('countries-local-name');
        const isActive = (document.getElementById('countries-is-active') as HTMLInputElement | null)?.checked ?? false;

        if (!code || code.length !== 2) {
            showError('Code is required and must be exactly 2 letters.');
            return;
        }

        if (!tld) {
            showError('TLD is required.');
            return;
        }

        if (!englishName || !localName) {
            showError('English name and local name are required.');
            return;
        }

        const numeric = numericRaw ? Number(numericRaw) : null;
        if (numericRaw) {
            if (numeric === null || Number.isNaN(numeric) || numeric <= 0) {
                showError('Numeric must be a positive number.');
                return;
            }
        }

        const payload = {
            code,
            tld,
            iso3: iso3Raw || null,
            numeric,
            englishName,
            localName,
            isActive,
        };

        const response = editingId
            ? await apiRequest<unknown>(`${getApiBaseUrl()}/Countries/${editingId}`, { method: 'PUT', body: JSON.stringify(payload) })
            : await apiRequest<unknown>(`${getApiBaseUrl()}/Countries`, { method: 'POST', body: JSON.stringify(payload) });

        if (!response.success) {
            showError(response.message || 'Failed to save country.');
            return;
        }

        hideModal('countries-edit-modal');
        showSuccess(editingId ? 'Country updated successfully.' : 'Country created successfully.');
        await loadCountries();
    };

    const openDelete = (id: number, name: string): void => {
        pendingDeleteId = id;

        const deleteName = document.getElementById('countries-delete-name');
        if (deleteName) {
            deleteName.textContent = name;
        }

        showModal('countries-delete-modal');
    };

    const doDelete = async (): Promise<void> => {
        if (!pendingDeleteId) {
            return;
        }

        const response = await apiRequest<unknown>(`${getApiBaseUrl()}/Countries/${pendingDeleteId}`, { method: 'DELETE' });
        hideModal('countries-delete-modal');

        if (!response.success) {
            showError(response.message || 'Failed to delete country.');
            pendingDeleteId = null;
            return;
        }

        showSuccess('Country deleted successfully.');
        pendingDeleteId = null;
        await loadCountries();
    };

    const resetFilters = (): void => {
        setInputValue('countries-filter-search', '');
        setInputValue('countries-filter-active', '');
        applyFilters();
    };

    const bindEvents = (): void => {
        document.getElementById('countries-create')?.addEventListener('click', openCreate);
        document.getElementById('countries-save')?.addEventListener('click', () => { void saveCountry(); });
        document.getElementById('countries-confirm-delete')?.addEventListener('click', () => { void doDelete(); });
        document.getElementById('countries-filter-reset')?.addEventListener('click', resetFilters);
        document.getElementById('countries-filter-search')?.addEventListener('input', applyFilters);
        document.getElementById('countries-filter-active')?.addEventListener('change', applyFilters);

        document.getElementById('countries-table-body')?.addEventListener('click', (event) => {
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
        const page = document.getElementById('countries-page') as HTMLElement | null;
        if (!page || page.dataset.initialized === 'true') {
            return;
        }

        page.dataset.initialized = 'true';

        bindEvents();
        await loadCountries();
    };

    const setupObserver = (): void => {
        void initializePage();

        if (document.body) {
            const observer = new MutationObserver(() => {
                const page = document.getElementById('countries-page') as HTMLElement | null;
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

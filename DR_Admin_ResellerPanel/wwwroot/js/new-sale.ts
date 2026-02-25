(() => {
    interface AppSettings {
        apiBaseUrl?: string;
    }

    interface ApiResponse<T> {
        success: boolean;
        data?: T;
        message?: string;
    }

    interface RegistrarOption {
        id: string;
        label: string;
        isDefault: boolean;
    }

    interface CustomerOption {
        id: string;
        label: string;
    }

    interface DomainAvailabilityResult {
        isAvailable?: boolean;
        IsAvailable?: boolean;
        isTldSupported?: boolean;
        IsTldSupported?: boolean;
    }

    type CustomerOptionMode = 'existing' | 'new';

    interface NewSaleState {
        domainName?: string;
        selectedRegistrarId?: string;
        selectedRegistrarLabel?: string;
        selectedDomain?: string;
        customerMode?: CustomerOptionMode;
        selectedCustomerId?: string;
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

    const storageKey = 'new-sale-state';

    const getBootstrap = (): BootstrapNamespace | null => {
        const maybeBootstrap = (window as Window & { bootstrap?: BootstrapNamespace }).bootstrap;
        return maybeBootstrap ?? null;
    };

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

    const apiRequest = async <T,>(endpoint: string, options: RequestInit = {}): Promise<ApiResponse<T>> => {
        try {
            const headers: Record<string, string> = {
                'Content-Type': 'application/json',
                ...(options.headers as Record<string, string> | undefined),
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
            const data = hasJson ? (await response.json()) : null;

            if (!response.ok) {
                const message = (data && ((data as { message?: string }).message ?? (data as { title?: string }).title)) ||
                    `Request failed with status ${response.status}`;
                return {
                    success: false,
                    message,
                };
            }

            const parsed = data as { success?: boolean; data?: T; message?: string } | null;
            return {
                success: parsed?.success !== false,
                data: parsed?.data ?? (data as T),
                message: parsed?.message,
            };
        } catch (error) {
            console.error('New sale request failed', error);
            return {
                success: false,
                message: 'Network error. Please try again.',
            };
        }
    };

    const extractItems = (raw: unknown): { items: unknown[]; meta: unknown } => {
        if (Array.isArray(raw)) {
            return { items: raw, meta: null };
        }

        if (!raw || typeof raw !== 'object') {
            return { items: [], meta: null };
        }

        const typed = raw as { data?: unknown; Data?: unknown; totalCount?: number; TotalCount?: number };
        const nested = typed.data as { data?: unknown; Data?: unknown } | undefined;
        const nestedData = nested?.data as { data?: unknown; Data?: unknown } | undefined;
        const nestedDataAlt = nested?.Data as { data?: unknown; Data?: unknown } | undefined;
        const typedData = typed.Data as { Data?: unknown } | undefined;

        const items = (Array.isArray(typed.Data) && typed.Data) ||
            (Array.isArray(typed.data) && typed.data) ||
            (Array.isArray(nested?.Data) && nested?.Data) ||
            (Array.isArray(nested?.data) && nested?.data) ||
            (Array.isArray(typedData?.Data) && typedData?.Data) ||
            (Array.isArray(nestedData?.Data) && nestedData?.Data) ||
            (Array.isArray(nestedDataAlt?.Data) && nestedDataAlt?.Data) ||
            [];

        const metaCandidates = [raw, typed.data, typed.Data, nested, nestedData];
        const meta = metaCandidates.find((candidate) => {
            if (!candidate || typeof candidate !== 'object') {
                return false;
            }

            const typedCandidate = candidate as {
                totalCount?: number;
                TotalCount?: number;
                totalPages?: number;
                TotalPages?: number;
                currentPage?: number;
                CurrentPage?: number;
                pageSize?: number;
                PageSize?: number;
            };

            return typedCandidate.totalCount !== undefined || typedCandidate.TotalCount !== undefined ||
                typedCandidate.totalPages !== undefined || typedCandidate.TotalPages !== undefined ||
                typedCandidate.currentPage !== undefined || typedCandidate.CurrentPage !== undefined ||
                typedCandidate.pageSize !== undefined || typedCandidate.PageSize !== undefined;
        }) ?? null;

        return { items, meta };
    };

    const esc = (text: string): string => {
        const map: Record<string, string> = { '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#039;' };
        return text.replace(/[&<>"']/g, (char) => map[char] ?? char);
    };

    const showSuccess = (message: string): void => {
        const alert = document.getElementById('new-sale-alert-success');
        if (!alert) {
            return;
        }

        alert.textContent = message;
        alert.classList.remove('d-none');

        const errorAlert = document.getElementById('new-sale-alert-error');
        errorAlert?.classList.add('d-none');

        setTimeout(() => alert.classList.add('d-none'), 5000);
    };

    const showError = (message: string): void => {
        const alert = document.getElementById('new-sale-alert-error');
        if (!alert) {
            return;
        }

        alert.textContent = message;
        alert.classList.remove('d-none');

        const successAlert = document.getElementById('new-sale-alert-success');
        successAlert?.classList.add('d-none');
    };

    const loadState = (): NewSaleState | null => {
        const raw = sessionStorage.getItem(storageKey);
        if (!raw) {
            return null;
        }

        try {
            const parsed = JSON.parse(raw) as unknown;
            if (!parsed || typeof parsed !== 'object') {
                return null;
            }

            const typed = parsed as NewSaleState;
            return {
                domainName: typed.domainName,
                selectedRegistrarId: typed.selectedRegistrarId,
                selectedRegistrarLabel: typed.selectedRegistrarLabel,
                selectedDomain: typed.selectedDomain,
                customerMode: typed.customerMode,
                selectedCustomerId: typed.selectedCustomerId,
            };
        } catch {
            return null;
        }
    };

    const saveState = (state: NewSaleState): void => {
        sessionStorage.setItem(storageKey, JSON.stringify(state));
    };

    const getCurrentState = (): NewSaleState => ({
        domainName: (document.getElementById('new-sale-domain-name') as HTMLInputElement | null)?.value?.trim() ?? '',
        selectedRegistrarId: selectedRegistrarId ?? undefined,
        selectedRegistrarLabel,
        selectedDomain: (document.getElementById('new-sale-selected-domain') as HTMLElement | null)?.textContent ?? undefined,
        customerMode: (document.getElementById('new-sale-customer-new') as HTMLInputElement | null)?.checked ? 'new' : 'existing',
        selectedCustomerId: (document.getElementById('new-sale-customer-select') as HTMLSelectElement | null)?.value ?? undefined,
    });

    let registrarOptions: RegistrarOption[] = [];
    let selectedRegistrarId: string | null = null;
    let selectedRegistrarLabel = '';
    let restoredState: NewSaleState | null = null;

    const setRegistrarSelection = (registrarId: string | null, registrarLabel: string): void => {
        selectedRegistrarId = registrarId ? String(registrarId) : null;
        selectedRegistrarLabel = registrarLabel;

        const display = document.getElementById('new-sale-registrar-display');
        if (display) {
            display.textContent = registrarLabel || 'Not selected';
        }

        saveState(getCurrentState());
    };

    const showModal = (id: string): void => {
        const element = document.getElementById(id);
        const bootstrap = getBootstrap();
        if (!element || !bootstrap) {
            return;
        }

        if (element.parentElement !== document.body) {
            document.body.appendChild(element);
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

    const loadRegistrars = async (): Promise<void> => {
        const select = document.getElementById('new-sale-settings-registrar') as HTMLSelectElement | null;
        if (!select) {
            return;
        }

        select.innerHTML = '<option value="">Loading registrars...</option>';
        const response = await apiRequest<unknown>(`${getApiBaseUrl()}/Registrars/active`, { method: 'GET' });

        if (!response.success) {
            select.innerHTML = '<option value="">Select registrar</option>';
            showError(response.message || 'Failed to load registrars');
            setRegistrarSelection(null, 'Not selected');
            return;
        }

        const raw = response.data;
        const registrars = Array.isArray(raw) ? raw : Array.isArray((raw as { data?: unknown }).data) ? (raw as { data?: unknown[] }).data ?? [] : [];

        if (!registrars.length) {
            select.innerHTML = '<option value="">No registrars available</option>';
            setRegistrarSelection(null, 'Not selected');
            return;
        }

        registrarOptions = registrars.map((registrar) => {
            const typed = registrar as {
                id?: number | string;
                Id?: number | string;
                name?: string;
                Name?: string;
                code?: string;
                Code?: string;
                isDefault?: boolean;
                IsDefault?: boolean;
            };

            const id = typed.id ?? typed.Id ?? 0;
            const name = typed.name ?? typed.Name ?? '';
            const code = typed.code ?? typed.Code ?? '';
            const isDefault = typed.isDefault ?? typed.IsDefault ?? false;
            const label = code ? `${name} (${code})` : name;
            return {
                id: String(id),
                label,
                isDefault,
            };
        });

        const options = registrarOptions.map((registrar) => {
            return `<option value="${registrar.id}">${esc(registrar.label)}</option>`;
        }).join('');

        select.innerHTML = `<option value="">Select registrar</option>${options}`;

        const restoredRegistrar = restoredState?.selectedRegistrarId
            ? registrarOptions.find((registrar) => registrar.id === restoredState?.selectedRegistrarId)
            : null;
        const defaultRegistrar = restoredRegistrar ?? registrarOptions.find((registrar) => registrar.isDefault) ?? registrarOptions[0];

        if (defaultRegistrar) {
            select.value = defaultRegistrar.id;
            setRegistrarSelection(defaultRegistrar.id, defaultRegistrar.label);
        }
    };

    const loadCustomers = async (): Promise<void> => {
        const select = document.getElementById('new-sale-customer-select') as HTMLSelectElement | null;
        if (!select) {
            return;
        }

        select.innerHTML = '<option value="">Loading customers...</option>';
        const params = new URLSearchParams({ pageNumber: '1', pageSize: '100' });
        const response = await apiRequest<unknown>(`${getApiBaseUrl()}/Customers?${params.toString()}`, { method: 'GET' });

        if (!response.success) {
            select.innerHTML = '<option value="">Select customer</option>';
            showError(response.message || 'Failed to load customers');
            return;
        }

        const extracted = extractItems(response.data ?? null);
        const customers = extracted.items;

        if (!customers.length) {
            select.innerHTML = '<option value="">No customers available</option>';
            return;
        }

        const options = customers
            .map((customer) => {
                const typed = customer as { id?: number | string; Id?: number | string; name?: string; Name?: string; email?: string; Email?: string };
                const id = typed.id ?? typed.Id ?? 0;
                const name = typed.name ?? typed.Name ?? '';
                const email = typed.email ?? typed.Email ?? '';
                const label = email ? `${name} (${email})` : name;
                return { id: String(id), label } satisfies CustomerOption;
            })
            .sort((a, b) => a.label.localeCompare(b.label))
            .map((customer) => `<option value="${customer.id}">${esc(customer.label)}</option>`)
            .join('');

        select.innerHTML = `<option value="">Select customer</option>${options}`;

        if (restoredState?.selectedCustomerId) {
            select.value = restoredState.selectedCustomerId;
        }
    };

    const toggleCustomerOption = (): void => {
        const existing = document.getElementById('new-sale-existing-customer');
        const newCustomer = document.getElementById('new-sale-new-customer');
        const existingRadio = document.getElementById('new-sale-customer-existing') as HTMLInputElement | null;

        if (!existing || !newCustomer || !existingRadio) {
            return;
        }

        const showExisting = existingRadio.checked;
        existing.classList.toggle('d-none', !showExisting);
        newCustomer.classList.toggle('d-none', showExisting);

        saveState(getCurrentState());
    };

    const showCustomerSelection = (domainName: string): void => {
        const card = document.getElementById('new-sale-customer-card');
        const selectedDomain = document.getElementById('new-sale-selected-domain');
        const existingRadio = document.getElementById('new-sale-customer-existing') as HTMLInputElement | null;

        if (!card || !selectedDomain || !existingRadio) {
            return;
        }

        selectedDomain.textContent = domainName;
        existingRadio.checked = true;
        toggleCustomerOption();
        card.classList.remove('d-none');
        card.scrollIntoView({ behavior: 'smooth', block: 'start' });

        saveState(getCurrentState());
    };

    const renderSearchResult = (html: string): void => {
        const target = document.getElementById('new-sale-search-result');
        if (target) {
            target.innerHTML = html;
        }
    };

    const checkDomainAvailability = async (): Promise<void> => {
        const domainInput = document.getElementById('new-sale-domain-name') as HTMLInputElement | null;

        if (!domainInput) {
            return;
        }

        const domainName = domainInput.value.trim();
        const registrarId = selectedRegistrarId;

        if (!registrarId) {
            renderSearchResult(`
                <div class="alert alert-warning" role="alert">
                    <i class="bi bi-exclamation-triangle"></i> Please select a registrar.
                </div>
            `);
            return;
        }

        if (!domainName) {
            renderSearchResult(`
                <div class="alert alert-warning" role="alert">
                    <i class="bi bi-exclamation-triangle"></i> Please enter a domain name.
                </div>
            `);
            return;
        }

        renderSearchResult(`
            <div class="text-center py-3">
                <div class="spinner-border text-primary" role="status">
                    <span class="visually-hidden">Checking...</span>
                </div>
                <p class="mt-2 mb-0">Checking domain availability...</p>
            </div>
        `);

        const response = await apiRequest<DomainAvailabilityResult>(
            `${getApiBaseUrl()}/Registrars/${registrarId}/isavailable/${encodeURIComponent(domainName)}`,
            { method: 'GET' }
        );

        if (!response.success) {
            renderSearchResult(`
                <div class="alert alert-danger" role="alert">
                    <i class="bi bi-exclamation-triangle"></i> ${esc(response.message || 'Failed to check domain availability.')}
                </div>
            `);
            return;
        }

        const data = response.data ?? {};
        const isTldSupported = (data.isTldSupported ?? data.IsTldSupported) ?? true;
        const isAvailable = (data.isAvailable ?? data.IsAvailable) === true;

        if (!isTldSupported) {
            renderSearchResult(`
                <div class="alert alert-warning" role="alert">
                    <h6 class="alert-heading"><i class="bi bi-exclamation-triangle"></i> TLD not supported</h6>
                    <p class="mb-0">The selected registrar does not support this TLD. Try a different extension or registrar.</p>
                </div>
            `);
            return;
        }

        if (isAvailable) {
            renderSearchResult(`
                <div class="alert alert-success" role="alert">
                    <h6 class="alert-heading"><i class="bi bi-check-circle"></i> Domain available</h6>
                    <p class="mb-2"><strong>${esc(domainName)}</strong> is available for registration.</p>
                    <button type="button" class="btn btn-success" id="new-sale-register-btn" data-domain="${esc(domainName)}">
                        <i class="bi bi-arrow-right-circle"></i> Register domain
                    </button>
                </div>
            `);
            return;
        }

        renderSearchResult(`
            <div class="alert alert-danger" role="alert">
                <h6 class="alert-heading"><i class="bi bi-x-circle"></i> Domain not available</h6>
                <p><strong>${esc(domainName)}</strong> is already registered.</p>
                <p class="mb-3">You can initiate a domain transfer if the current registrant provides the authorization code. Transfers typically complete within 5-7 days after the current registrar approves the request.</p>
                <button type="button" class="btn btn-outline-primary" id="new-sale-transfer-btn" data-domain="${esc(domainName)}">
                    <i class="bi bi-arrow-left-right"></i> Transfer domain
                </button>
            </div>
        `);
    };

    const bindEvents = (): void => {
        const form = document.getElementById('new-sale-search-form') as HTMLFormElement | null;
        const searchResult = document.getElementById('new-sale-search-result');
        const existingRadio = document.getElementById('new-sale-customer-existing') as HTMLInputElement | null;
        const newRadio = document.getElementById('new-sale-customer-new') as HTMLInputElement | null;
        const openSettings = document.getElementById('new-sale-settings-open');
        const settingsSave = document.getElementById('new-sale-settings-save');
        const settingsSelect = document.getElementById('new-sale-settings-registrar') as HTMLSelectElement | null;
        const domainInput = document.getElementById('new-sale-domain-name') as HTMLInputElement | null;
        const customerSelect = document.getElementById('new-sale-customer-select') as HTMLSelectElement | null;
        const createCustomerButton = document.getElementById('new-sale-create-customer') as HTMLButtonElement | null;

        form?.addEventListener('submit', (event) => {
            event.preventDefault();
            checkDomainAvailability();
        });

        domainInput?.addEventListener('input', () => {
            saveState(getCurrentState());
        });

        customerSelect?.addEventListener('change', () => {
            saveState(getCurrentState());
        });

        searchResult?.addEventListener('click', (event) => {
            const target = event.target as HTMLElement;
            const button = target.closest<HTMLButtonElement>('#new-sale-register-btn');
            if (!button) {
                return;
            }

            const domainName = button.dataset.domain || '';
            if (domainName) {
                showCustomerSelection(domainName);
                showSuccess(`Ready to register ${domainName}. Select a customer to continue.`);
            }
        });

        existingRadio?.addEventListener('change', toggleCustomerOption);
        newRadio?.addEventListener('change', toggleCustomerOption);

        openSettings?.addEventListener('click', () => {
            showModal('new-sale-settings-modal');
        });

        settingsSave?.addEventListener('click', () => {
            if (!settingsSelect) {
                return;
            }

            const registrarId = settingsSelect.value;
            if (!registrarId) {
                showError('Select a registrar to continue.');
                return;
            }

            const selected = registrarOptions.find((registrar) => registrar.id === registrarId);
            setRegistrarSelection(registrarId, selected ? selected.label : 'Selected registrar');
            hideModal('new-sale-settings-modal');
        });

        createCustomerButton?.addEventListener('click', () => {
            saveState(getCurrentState());
            document.dispatchEvent(new CustomEvent('customers:open-create'));
        });

        document.addEventListener('customers:saved', () => {
            void loadCustomers();
        });
    };

    const applyRestoredState = (): void => {
        if (!restoredState) {
            return;
        }

        const domainInput = document.getElementById('new-sale-domain-name') as HTMLInputElement | null;
        if (domainInput && restoredState.domainName) {
            domainInput.value = restoredState.domainName;
        }

        const customerMode = restoredState.customerMode ?? 'existing';
        const existingRadio = document.getElementById('new-sale-customer-existing') as HTMLInputElement | null;
        const newRadio = document.getElementById('new-sale-customer-new') as HTMLInputElement | null;
        if (existingRadio && newRadio) {
            existingRadio.checked = customerMode === 'existing';
            newRadio.checked = customerMode === 'new';
            toggleCustomerOption();
        }

        if (restoredState.selectedDomain) {
            showCustomerSelection(restoredState.selectedDomain);
        }

        if (restoredState.selectedRegistrarId && restoredState.selectedRegistrarLabel) {
            setRegistrarSelection(restoredState.selectedRegistrarId, restoredState.selectedRegistrarLabel);
        }
    };

    const initializeNewSalePage = async (): Promise<void> => {
        const page = document.getElementById('dashboard-new-sale-page') as HTMLElement | null;
        if (!page || page.dataset.initialized === 'true') {
            return;
        }

        page.dataset.initialized = 'true';
        restoredState = loadState();
        bindEvents();
        applyRestoredState();
        await loadRegistrars();
        await loadCustomers();
    };

    const setupPageObserver = (): void => {
        void initializeNewSalePage();

        if (document.body) {
            const observer = new MutationObserver(() => {
                const page = document.getElementById('dashboard-new-sale-page') as HTMLElement | null;
                if (page && page.dataset.initialized !== 'true') {
                    void initializeNewSalePage();
                }
            });
            observer.observe(document.body, { childList: true, subtree: true });
        }
    };

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', setupPageObserver);
    } else {
        setupPageObserver();
    }
})();

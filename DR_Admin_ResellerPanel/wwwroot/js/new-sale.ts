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

    interface CustomerSearchResult {
        id: number;
        name: string;
        customerName: string;
        email: string;
        phone: string;
        formattedReferenceNumber: string;
        formattedCustomerNumber: string;
        status: string;
    }

    interface DomainAvailabilityResult {
        isAvailable?: boolean;
        IsAvailable?: boolean;
        isTldSupported?: boolean;
        IsTldSupported?: boolean;
    }

    interface NewSaleState {
        domainName?: string;
        selectedRegistrarId?: string;
        selectedRegistrarLabel?: string;
        selectedDomain?: string;
        customerSearchQuery?: string;
        selectedCustomer?: CustomerSearchResult;
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
                customerSearchQuery: typed.customerSearchQuery,
                selectedCustomer: typed.selectedCustomer,
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
        customerSearchQuery: (document.getElementById('new-sale-customer-search') as HTMLInputElement | null)?.value?.trim() ?? undefined,
        selectedCustomer: selectedCustomer ?? undefined,
    });

    let registrarOptions: RegistrarOption[] = [];
    let selectedRegistrarId: string | null = null;
    let selectedRegistrarLabel = '';
    let selectedCustomer: CustomerSearchResult | null = null;
    let customerSearchTimer: ReturnType<typeof setTimeout> | null = null;
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

    const normalizeCustomer = (raw: Record<string, unknown>): CustomerSearchResult => ({
        id: (raw.id ?? raw.Id ?? 0) as number,
        name: (raw.name ?? raw.Name ?? '') as string,
        customerName: (raw.customerName ?? raw.CustomerName ?? '') as string,
        email: (raw.email ?? raw.Email ?? '') as string,
        phone: (raw.phone ?? raw.Phone ?? '') as string,
        formattedReferenceNumber: (raw.formattedReferenceNumber ?? raw.FormattedReferenceNumber ?? '') as string,
        formattedCustomerNumber: (raw.formattedCustomerNumber ?? raw.FormattedCustomerNumber ?? '') as string,
        status: (raw.status ?? raw.Status ?? '') as string,
    });

    const showCustomerDetail = (customer: CustomerSearchResult): void => {
        selectedCustomer = customer;

        const detail = document.getElementById('new-sale-customer-detail');
        const searchInput = document.getElementById('new-sale-customer-search') as HTMLInputElement | null;
        if (!detail) {
            return;
        }

        const setField = (id: string, value: string): void => {
            const el = document.getElementById(id);
            if (el) {
                el.textContent = value || '-';
            }
        };

        setField('new-sale-customer-detail-name', customer.name);
        setField('new-sale-customer-detail-contact', customer.customerName);
        setField('new-sale-customer-detail-email', customer.email);
        setField('new-sale-customer-detail-phone', customer.phone);
        setField('new-sale-customer-detail-ref', customer.formattedReferenceNumber);
        setField('new-sale-customer-detail-custno', customer.formattedCustomerNumber);
        setField('new-sale-customer-detail-status', customer.status);

        detail.classList.remove('d-none');

        if (searchInput) {
            searchInput.value = customer.name;
        }

        const statusEl = document.getElementById('new-sale-customer-search-status');
        if (statusEl) {
            statusEl.innerHTML = '';
        }

        saveState(getCurrentState());
    };

    const clearCustomerSelection = (): void => {
        selectedCustomer = null;

        const detail = document.getElementById('new-sale-customer-detail');
        detail?.classList.add('d-none');

        const searchInput = document.getElementById('new-sale-customer-search') as HTMLInputElement | null;
        if (searchInput) {
            searchInput.value = '';
            searchInput.focus();
        }

        const statusEl = document.getElementById('new-sale-customer-search-status');
        if (statusEl) {
            statusEl.innerHTML = '';
        }

        saveState(getCurrentState());
    };

    const showCustomerSelectModal = (customers: CustomerSearchResult[]): void => {
        const body = document.getElementById('new-sale-customer-select-body');
        if (!body) {
            return;
        }

        body.innerHTML = customers.map((c) => `
            <tr>
                <td>${esc(c.name)}</td>
                <td>${esc(c.customerName)}</td>
                <td>${esc(c.email)}</td>
                <td>${esc(c.phone)}</td>
                <td>${esc(c.formattedReferenceNumber)}</td>
                <td class="text-end">
                    <button type="button" class="btn btn-sm btn-primary new-sale-pick-customer" data-customer-id="${c.id}">
                        <i class="bi bi-check-lg"></i> Select
                    </button>
                </td>
            </tr>
        `).join('');

        body.querySelectorAll<HTMLButtonElement>('.new-sale-pick-customer').forEach((btn) => {
            btn.addEventListener('click', () => {
                const id = Number(btn.dataset.customerId);
                const match = customers.find((c) => c.id === id);
                if (match) {
                    showCustomerDetail(match);
                    hideModal('new-sale-customer-select-modal');
                }
            });
        });

        showModal('new-sale-customer-select-modal');
    };

    const searchCustomers = async (query: string): Promise<void> => {
        const statusEl = document.getElementById('new-sale-customer-search-status');
        if (!statusEl) {
            return;
        }

        if (query.length < 2) {
            statusEl.innerHTML = '';
            return;
        }

        statusEl.innerHTML = '<div class="text-muted small"><div class="spinner-border spinner-border-sm me-1"></div> Searching...</div>';

        const params = new URLSearchParams({ query });
        const response = await apiRequest<unknown>(`${getApiBaseUrl()}/Customers/search?${params.toString()}`, { method: 'GET' });

        if (!response.success) {
            statusEl.innerHTML = `<div class="text-danger small"><i class="bi bi-exclamation-triangle"></i> ${esc(response.message || 'Search failed')}</div>`;
            return;
        }

        const raw = response.data;
        const items = Array.isArray(raw) ? raw : Array.isArray((raw as { data?: unknown }).data) ? (raw as { data?: unknown[] }).data ?? [] : [];

        if (items.length === 0) {
            statusEl.innerHTML = '<div class="text-warning small"><i class="bi bi-info-circle"></i> No customers found. You can create a new customer.</div>';
            return;
        }

        const customers = items.map((item) => normalizeCustomer(item as Record<string, unknown>));

        if (customers.length === 1) {
            statusEl.innerHTML = '';
            showCustomerDetail(customers[0]);
            return;
        }

        statusEl.innerHTML = `<div class="text-info small"><i class="bi bi-list"></i> ${customers.length} customers found. Select one from the list.</div>`;
        showCustomerSelectModal(customers);
    };

    const showCustomerSelection = (domainName: string): void => {
        const card = document.getElementById('new-sale-customer-card');
        const selectedDomain = document.getElementById('new-sale-selected-domain');

        if (!card || !selectedDomain) {
            return;
        }

        selectedDomain.textContent = domainName;
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
        const openSettings = document.getElementById('new-sale-settings-open');
        const settingsSave = document.getElementById('new-sale-settings-save');
        const settingsSelect = document.getElementById('new-sale-settings-registrar') as HTMLSelectElement | null;
        const domainInput = document.getElementById('new-sale-domain-name') as HTMLInputElement | null;
        const customerSearchInput = document.getElementById('new-sale-customer-search') as HTMLInputElement | null;
        const customerClearBtn = document.getElementById('new-sale-customer-clear') as HTMLButtonElement | null;
        const createCustomerButton = document.getElementById('new-sale-create-customer') as HTMLButtonElement | null;

        form?.addEventListener('submit', (event) => {
            event.preventDefault();
            checkDomainAvailability();
        });

        domainInput?.addEventListener('input', () => {
            saveState(getCurrentState());
        });

        customerSearchInput?.addEventListener('input', () => {
            if (selectedCustomer) {
                selectedCustomer = null;
                const detail = document.getElementById('new-sale-customer-detail');
                detail?.classList.add('d-none');
            }

            if (customerSearchTimer !== null) {
                clearTimeout(customerSearchTimer);
            }

            const query = customerSearchInput.value.trim();
            if (query.length < 2) {
                const statusEl = document.getElementById('new-sale-customer-search-status');
                if (statusEl) {
                    statusEl.innerHTML = '';
                }
                return;
            }

            customerSearchTimer = setTimeout(() => {
                void searchCustomers(query);
            }, 400);
        });

        customerClearBtn?.addEventListener('click', () => {
            clearCustomerSelection();
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
            clearCustomerSelection();
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

        if (restoredState.selectedDomain) {
            showCustomerSelection(restoredState.selectedDomain);
        }

        if (restoredState.selectedCustomer) {
            showCustomerDetail(restoredState.selectedCustomer);
        } else if (restoredState.customerSearchQuery) {
            const searchInput = document.getElementById('new-sale-customer-search') as HTMLInputElement | null;
            if (searchInput) {
                searchInput.value = restoredState.customerSearchQuery;
            }
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

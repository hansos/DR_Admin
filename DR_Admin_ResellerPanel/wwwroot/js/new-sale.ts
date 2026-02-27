(() => {
    interface AppSettings {
        apiBaseUrl?: string;
    }

    interface ApiResponse<T> {
        success: boolean;
        data?: T;
        message?: string;
        status?: number;
    }

    interface RegistrarOption {
        id: string;
        code: string;
        label: string;
        isDefault: boolean;
    }

    interface DomainAvailabilityResult {
        success?: boolean;
        Success?: boolean;
        domainName?: string;
        DomainName?: string;
        isAvailable?: boolean;
        IsAvailable?: boolean;
        isTldSupported?: boolean;
        IsTldSupported?: boolean;
        message?: string;
        Message?: string;
        premiumPrice?: number | null;
        PremiumPrice?: number | null;
    }

    interface RegisteredDomain {
        id?: number;
        Id?: number;
        name?: string;
        Name?: string;
        status?: string;
        Status?: string;
        expirationDate?: string;
        ExpirationDate?: string;
        customer?: {
            name?: string;
            Name?: string;
        };
        Customer?: {
            name?: string;
            Name?: string;
        };
    }

    interface AlternativesResponse {
        suggestions?: string[];
        Suggestions?: string[];
    }

    interface NewSaleState {
        domainName?: string;
        selectedRegistrarId?: string;
        selectedRegistrarCode?: string;
        selectedRegistrarLabel?: string;
        flowType?: string;
        pricing?: {
            registration: number | null;
            currency: string;
        };
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

    let registrarOptions: RegistrarOption[] = [];
    let selectedRegistrarId: string | null = null;
    let selectedRegistrarCode: string | null = null;
    let selectedRegistrarLabel = '';

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
                headers.Authorization = `Bearer ${authToken}`;
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
                    status: response.status,
                };
            }

            const parsed = data as { success?: boolean; data?: T; message?: string } | null;
            return {
                success: parsed?.success !== false,
                data: parsed?.data ?? (data as T),
                message: parsed?.message,
                status: response.status,
            };
        } catch (error) {
            console.error('New sale request failed', error);
            return {
                success: false,
                message: 'Network error. Please try again.',
                status: 0,
            };
        }
    };

    const esc = (text: string): string => {
        const map: Record<string, string> = { '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#039;' };
        return (text || '').replace(/[&<>"']/g, (char) => map[char] ?? char);
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
            return JSON.parse(raw) as NewSaleState;
        } catch {
            return null;
        }
    };

    const saveState = (state: NewSaleState): void => {
        sessionStorage.setItem(storageKey, JSON.stringify(state));
    };

    const canProceedToPage2 = (state: NewSaleState | null): state is NewSaleState => {
        return !!state?.domainName && !!state.flowType;
    };

    const renderNextStepButton = (): void => {
        const container = document.getElementById('new-sale-next-step-container');
        if (!container) {
            return;
        }

        const state = loadState();
        if (canProceedToPage2(state)) {
            container.classList.remove('d-none');
            return;
        }

        container.classList.add('d-none');
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

    const setRegistrarSelection = (registrar: RegistrarOption | null): void => {
        selectedRegistrarId = registrar?.id ?? null;
        selectedRegistrarCode = registrar?.code ?? null;
        selectedRegistrarLabel = registrar?.label ?? '';

        const display = document.getElementById('new-sale-registrar-display');
        if (display) {
            display.textContent = selectedRegistrarLabel || 'Not selected';
        }

        const current = loadState() ?? {};
        saveState({
            ...current,
            selectedRegistrarId: selectedRegistrarId ?? undefined,
            selectedRegistrarCode: selectedRegistrarCode ?? undefined,
            selectedRegistrarLabel: selectedRegistrarLabel || undefined,
        });
    };

    const renderSearchResult = (html: string): void => {
        const target = document.getElementById('new-sale-search-result');
        if (target) {
            target.innerHTML = html;
        }
    };

    const getDomainInputValue = (): string => {
        const input = document.getElementById('new-sale-domain-name') as HTMLInputElement | null;
        return input?.value.trim().toLowerCase() ?? '';
    };

    const saveDraftDomain = (): void => {
        const current = loadState() ?? {};
        const nextDomainName = getDomainInputValue();
        const currentDomainName = current.domainName ?? '';
        const domainChanged = currentDomainName !== nextDomainName;

        saveState({
            ...current,
            domainName: nextDomainName || undefined,
            selectedRegistrarId: selectedRegistrarId ?? undefined,
            selectedRegistrarCode: selectedRegistrarCode ?? undefined,
            selectedRegistrarLabel: selectedRegistrarLabel || undefined,
            flowType: domainChanged ? undefined : current.flowType,
            pricing: domainChanged ? undefined : current.pricing,
        });

        renderNextStepButton();
    };

    const goToPage2 = (flowType: string, domainName: string, premiumPrice: number | null = null): void => {
        saveState({
            domainName,
            selectedRegistrarId: selectedRegistrarId ?? undefined,
            selectedRegistrarCode: selectedRegistrarCode ?? undefined,
            selectedRegistrarLabel: selectedRegistrarLabel || undefined,
            flowType,
            pricing: {
                registration: premiumPrice,
                currency: 'USD',
            },
        });
        renderNextStepButton();
        showSuccess('Domain selected. Continue to the next page when ready.');
    };

    const navigateToPage2 = (): void => {
        const state = loadState();
        if (!canProceedToPage2(state)) {
            showError('Select a domain action before continuing.');
            return;
        }

        window.location.href = '/dashboard/new-sale/customer';
    };

    const parseAvailability = (data: DomainAvailabilityResult | undefined): {
        isAvailable: boolean;
        isTldSupported: boolean;
        message: string;
        premiumPrice: number | null;
    } => {
        return {
            isAvailable: (data?.isAvailable ?? data?.IsAvailable) === true,
            isTldSupported: (data?.isTldSupported ?? data?.IsTldSupported) !== false,
            message: (data?.message ?? data?.Message ?? '').trim(),
            premiumPrice: data?.premiumPrice ?? data?.PremiumPrice ?? null,
        };
    };

    const checkInternalDomain = async (domainName: string, silent: boolean = false): Promise<RegisteredDomain | null> => {
        const response = await apiRequest<RegisteredDomain>(`${getApiBaseUrl()}/RegisteredDomains/name/${encodeURIComponent(domainName)}`, { method: 'GET' });

        if (response.success && response.data) {
            return response.data;
        }

        if (response.status === 404) {
            return null;
        }

        if (!silent && response.status && response.status !== 404) {
            showError(response.message || 'Failed to check internal domain records.');
        }

        return null;
    };

    const getAlternativeDomains = async (domainName: string): Promise<string[]> => {
        const response = await apiRequest<AlternativesResponse>(`${getApiBaseUrl()}/DomainManager/domain/name/${encodeURIComponent(domainName)}/alternatives?count=12`, {
            method: 'GET',
        });

        if (!response.success) {
            return [];
        }

        const data = response.data;
        const suggestions = data?.suggestions ?? data?.Suggestions ?? [];
        return Array.isArray(suggestions) ? suggestions : [];
    };

    const renderOutcome1A = (domainName: string): void => {
        renderSearchResult(`
            <div class="alert alert-warning" role="alert">
                <h6 class="alert-heading mb-2"><i class="bi bi-exclamation-triangle"></i> 1A — TLD not supported</h6>
                <p class="mb-3">The selected registrar does not support <strong>${esc(domainName)}</strong>.</p>
                <button type="button" class="btn btn-outline-secondary" id="new-sale-change-registrar-btn">
                    <i class="bi bi-gear"></i> Change registrar
                </button>
            </div>
        `);
    };

    const renderOutcome1B = (domainName: string, premiumPrice: number | null): void => {
        const premiumHtml = premiumPrice !== null
            ? `<div class="small text-muted mb-2">Premium price: <strong>${premiumPrice}</strong></div>`
            : '';

        renderSearchResult(`
            <div class="alert alert-success" role="alert">
                <h6 class="alert-heading mb-2"><i class="bi bi-check-circle"></i> 1B — Available for registration</h6>
                <p class="mb-2"><strong>${esc(domainName)}</strong> is available.</p>
                ${premiumHtml}
                <button type="button" class="btn btn-success" id="new-sale-register-btn" data-domain="${esc(domainName)}" data-price="${premiumPrice ?? ''}">
                    <i class="bi bi-arrow-right-circle"></i> Register domain
                </button>
            </div>
        `);
    };

    const renderOutcome1C = (domainName: string): void => {
        renderSearchResult(`
            <div class="alert alert-danger" role="alert">
                <h6 class="alert-heading mb-2"><i class="bi bi-arrow-left-right"></i> 1C — Taken domain options</h6>
                <p class="mb-3"><strong>${esc(domainName)}</strong> is already registered.</p>
                <div class="d-flex flex-wrap gap-2">
                    <button type="button" class="btn btn-outline-primary" id="new-sale-transfer-btn" data-domain="${esc(domainName)}">
                        <i class="bi bi-arrow-left-right"></i> Transfer domain
                    </button>
                    <button type="button" class="btn btn-outline-secondary" id="new-sale-suggest-alternatives-btn" data-domain="${esc(domainName)}">
                        <i class="bi bi-lightbulb"></i> Suggest alternatives
                    </button>
                </div>
            </div>
        `);
    };

    const renderOutcome1D = (domainName: string, domain: RegisteredDomain): void => {
        const owner = domain.customer?.name ?? domain.Customer?.Name ?? '-';
        const status = domain.status ?? domain.Status ?? '-';
        const expiration = domain.expirationDate ?? domain.ExpirationDate ?? '';
        const expirationText = expiration ? new Date(expiration).toLocaleDateString() : '-';

        renderSearchResult(`
            <div class="alert alert-info" role="alert">
                <h6 class="alert-heading mb-2"><i class="bi bi-info-circle"></i> 1D — Already in our system</h6>
                <div class="small mb-3">
                    <div><strong>Domain:</strong> ${esc(domainName)}</div>
                    <div><strong>Owner:</strong> ${esc(owner)}</div>
                    <div><strong>Status:</strong> ${esc(status)}</div>
                    <div><strong>Expiry:</strong> ${esc(expirationText)}</div>
                </div>
                <div class="d-flex flex-wrap gap-2">
                    <button type="button" class="btn btn-outline-primary" id="new-sale-renew-btn" data-domain="${esc(domainName)}">
                        <i class="bi bi-arrow-clockwise"></i> Renew
                    </button>
                    <button type="button" class="btn btn-outline-secondary" id="new-sale-add-services-btn" data-domain="${esc(domainName)}">
                        <i class="bi bi-plus-square"></i> Add services
                    </button>
                    <button type="button" class="btn btn-outline-dark" id="new-sale-contact-owner-btn" data-domain="${esc(domainName)}">
                        <i class="bi bi-person-lines-fill"></i> Contact owner
                    </button>
                </div>
            </div>
        `);
    };

    const updateAlternativeStatus = (domainName: string, status: 'pending' | 'available' | 'not-available'): void => {
        const button = document.querySelector<HTMLButtonElement>(`.new-sale-alternative[data-domain="${CSS.escape(domainName)}"]`);
        if (!button) {
            return;
        }

        const statusElement = button.querySelector<HTMLElement>('.new-sale-alternative-status');
        if (!statusElement) {
            return;
        }

        button.classList.remove('list-group-item-warning', 'list-group-item-success', 'list-group-item-danger');

        if (status === 'pending') {
            button.dataset.availability = 'pending';
            button.disabled = true;
            button.classList.add('list-group-item-warning');
            statusElement.innerHTML = '<span class="badge text-bg-warning"><span class="spinner-border spinner-border-sm me-1" role="status" aria-hidden="true"></span>Pending</span>';
            return;
        }

        if (status === 'available') {
            button.dataset.availability = 'available';
            button.disabled = false;
            button.classList.add('list-group-item-success');
            statusElement.innerHTML = '<span class="badge text-bg-success">Available</span>';
            return;
        }

        button.dataset.availability = 'not-available';
        button.disabled = true;
        button.classList.add('list-group-item-danger');
        statusElement.innerHTML = '<span class="badge text-bg-danger">Not available</span>';
    };

    const checkAlternativeDomainAvailability = async (domainName: string): Promise<'available' | 'not-available'> => {
        if (!selectedRegistrarCode) {
            return 'not-available';
        }

        const internalDomain = await checkInternalDomain(domainName, true);
        if (internalDomain) {
            return 'not-available';
        }

        const response = await apiRequest<DomainAvailabilityResult>(
            `${getApiBaseUrl()}/DomainManager/registrar/${encodeURIComponent(selectedRegistrarCode)}/domain/name/${encodeURIComponent(domainName)}/is-available`,
            { method: 'GET' }
        );

        if (!response.success) {
            return 'not-available';
        }

        const parsed = parseAvailability(response.data);
        return parsed.isTldSupported && parsed.isAvailable ? 'available' : 'not-available';
    };

    const runAlternativeDomainChecks = async (domains: string[]): Promise<void> => {
        await Promise.all(domains.map(async (domain) => {
            updateAlternativeStatus(domain, 'pending');
            const status = await checkAlternativeDomainAvailability(domain);
            updateAlternativeStatus(domain, status);
        }));
    };

    const renderOutcome1E = async (domainName: string): Promise<void> => {
        const suggestions = await getAlternativeDomains(domainName);

        if (!suggestions.length) {
            renderSearchResult(`
                <div class="alert alert-warning" role="alert">
                    <h6 class="alert-heading mb-2"><i class="bi bi-lightbulb"></i> 1E — Select alternative domain</h6>
                    <p class="mb-0">No suggestions were found. Try another name or TLD.</p>
                </div>
            `);
            return;
        }

        const list = suggestions.map((item) => `
            <button type="button" class="list-group-item list-group-item-action d-flex justify-content-between align-items-center new-sale-alternative" data-domain="${esc(item)}" data-availability="pending" disabled>
                <span>${esc(item)}</span>
                <span class="new-sale-alternative-status"><span class="badge text-bg-warning"><span class="spinner-border spinner-border-sm me-1" role="status" aria-hidden="true"></span>Pending</span></span>
            </button>
        `).join('');

        renderSearchResult(`
            <div class="alert alert-warning" role="alert">
                <h6 class="alert-heading mb-2"><i class="bi bi-lightbulb"></i> 1E — Select alternative domain</h6>
                <p class="mb-2">The selected domain is not available. Try one of these alternatives:</p>
                <div class="list-group">${list}</div>
            </div>
        `);

        void runAlternativeDomainChecks(suggestions);
    };

    const checkDomainAvailability = async (): Promise<void> => {
        const domainName = getDomainInputValue();

        if (!selectedRegistrarId || !selectedRegistrarCode) {
            showError('Please select a registrar first.');
            return;
        }

        if (!domainName) {
            showError('Please enter a domain name.');
            return;
        }

        saveDraftDomain();

        renderSearchResult(`
            <div class="text-center py-3">
                <div class="spinner-border text-primary" role="status">
                    <span class="visually-hidden">Checking...</span>
                </div>
                <p class="mt-2 mb-0">Checking domain availability...</p>
            </div>
        `);

        const internalDomain = await checkInternalDomain(domainName);
        if (internalDomain) {
            renderOutcome1D(domainName, internalDomain);
            return;
        }

        const response = await apiRequest<DomainAvailabilityResult>(
            `${getApiBaseUrl()}/DomainManager/registrar/${encodeURIComponent(selectedRegistrarCode)}/domain/name/${encodeURIComponent(domainName)}/is-available`,
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

        const parsed = parseAvailability(response.data);

        if (!parsed.isTldSupported) {
            renderOutcome1A(domainName);
            return;
        }

        if (parsed.isAvailable) {
            renderOutcome1B(domainName, parsed.premiumPrice);
            return;
        }

        renderOutcome1C(domainName);
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
            setRegistrarSelection(null);
            return;
        }

        const raw = response.data;
        const registrars = Array.isArray(raw) ? raw : Array.isArray((raw as { data?: unknown[] })?.data) ? ((raw as { data?: unknown[] }).data ?? []) : [];

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

            const id = String(typed.id ?? typed.Id ?? '');
            const name = String(typed.name ?? typed.Name ?? '');
            const code = String(typed.code ?? typed.Code ?? '');
            const isDefault = (typed.isDefault ?? typed.IsDefault ?? false) === true;
            const label = code ? `${name} (${code})` : name;

            return { id, code, label, isDefault };
        }).filter((item) => !!item.id && !!item.code);

        if (!registrarOptions.length) {
            select.innerHTML = '<option value="">No registrars available</option>';
            setRegistrarSelection(null);
            return;
        }

        select.innerHTML = `<option value="">Select registrar</option>${registrarOptions.map((r) => `<option value="${esc(r.id)}">${esc(r.label)}</option>`).join('')}`;

        const restoredState = loadState();
        const restored = restoredState?.selectedRegistrarId
            ? registrarOptions.find((item) => item.id === restoredState.selectedRegistrarId)
            : null;
        const fallback = restored ?? registrarOptions.find((item) => item.isDefault) ?? registrarOptions[0];

        if (fallback) {
            select.value = fallback.id;
            setRegistrarSelection(fallback);
        }
    };

    const bindEvents = (): void => {
        const form = document.getElementById('new-sale-search-form') as HTMLFormElement | null;
        const openSettings = document.getElementById('new-sale-settings-open');
        const settingsSave = document.getElementById('new-sale-settings-save');
        const settingsSelect = document.getElementById('new-sale-settings-registrar') as HTMLSelectElement | null;
        const domainInput = document.getElementById('new-sale-domain-name') as HTMLInputElement | null;
        const searchResult = document.getElementById('new-sale-search-result');
        const nextStepButton = document.getElementById('new-sale-next-step-btn');

        form?.addEventListener('submit', (event) => {
            event.preventDefault();
            void checkDomainAvailability();
        });

        domainInput?.addEventListener('input', () => {
            saveDraftDomain();
        });

        nextStepButton?.addEventListener('click', navigateToPage2);

        openSettings?.addEventListener('click', () => {
            showModal('new-sale-settings-modal');
        });

        settingsSave?.addEventListener('click', () => {
            if (!settingsSelect) {
                return;
            }

            const id = settingsSelect.value;
            const selected = registrarOptions.find((item) => item.id === id) ?? null;

            if (!selected) {
                showError('Select a registrar to continue.');
                return;
            }

            setRegistrarSelection(selected);
            hideModal('new-sale-settings-modal');
            showSuccess(`Registrar changed to ${selected.label}`);
        });

        searchResult?.addEventListener('click', (event) => {
            const target = event.target as HTMLElement;

            const changeRegistrarButton = target.closest<HTMLButtonElement>('#new-sale-change-registrar-btn');
            if (changeRegistrarButton) {
                showModal('new-sale-settings-modal');
                return;
            }

            const alternativeButton = target.closest<HTMLButtonElement>('.new-sale-alternative');
            if (alternativeButton) {
                if (alternativeButton.dataset.availability !== 'available') {
                    return;
                }

                const domain = alternativeButton.dataset.domain ?? '';
                const input = document.getElementById('new-sale-domain-name') as HTMLInputElement | null;
                if (input && domain) {
                    input.value = domain;
                    saveDraftDomain();
                    void checkDomainAvailability();
                }
                return;
            }

            const registerButton = target.closest<HTMLButtonElement>('#new-sale-register-btn');
            if (registerButton) {
                const domain = registerButton.dataset.domain ?? '';
                const rawPrice = registerButton.dataset.price ?? '';
                const parsedPrice = rawPrice ? Number(rawPrice) : null;
                goToPage2('register', domain, Number.isFinite(parsedPrice as number) ? parsedPrice : null);
                navigateToPage2();
                return;
            }

            const transferButton = target.closest<HTMLButtonElement>('#new-sale-transfer-btn');
            if (transferButton) {
                const domain = transferButton.dataset.domain ?? '';
                goToPage2('transfer', domain, null);
                return;
            }

            const suggestAlternativesButton = target.closest<HTMLButtonElement>('#new-sale-suggest-alternatives-btn');
            if (suggestAlternativesButton) {
                const domain = suggestAlternativesButton.dataset.domain ?? '';
                if (domain) {
                    void renderOutcome1E(domain);
                }
                return;
            }

            const renewButton = target.closest<HTMLButtonElement>('#new-sale-renew-btn');
            if (renewButton) {
                const domain = renewButton.dataset.domain ?? '';
                goToPage2('renew', domain, null);
                return;
            }

            const addServicesButton = target.closest<HTMLButtonElement>('#new-sale-add-services-btn');
            if (addServicesButton) {
                const domain = addServicesButton.dataset.domain ?? '';
                goToPage2('add-services', domain, null);
                return;
            }

            const contactOwnerButton = target.closest<HTMLButtonElement>('#new-sale-contact-owner-btn');
            if (contactOwnerButton) {
                const domain = contactOwnerButton.dataset.domain ?? '';
                goToPage2('contact-owner', domain, null);
            }
        });
    };

    const applyRestoredState = (): void => {
        const state = loadState();
        if (!state) {
            renderNextStepButton();
            return;
        }

        const domainInput = document.getElementById('new-sale-domain-name') as HTMLInputElement | null;
        if (domainInput && state.domainName) {
            domainInput.value = state.domainName;
        }

        renderNextStepButton();
    };

    const initializeNewSalePage = async (): Promise<void> => {
        const page = document.getElementById('dashboard-new-sale-page') as HTMLElement | null;
        if (!page || page.dataset.initialized === 'true') {
            return;
        }

        page.dataset.initialized = 'true';

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

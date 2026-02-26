(() => {
    interface AppSettings {
        apiBaseUrl?: string;
    }

    interface ApiResponse<T> {
        success: boolean;
        data?: T;
        message?: string;
    }

    interface CustomerSummary {
        id: number;
        name: string;
        customerName: string;
    }

    interface HostingPackage {
        id: number;
        name: string;
        description: string;
        diskSpaceMB: number;
        bandwidthMB: number;
        emailAccounts: number;
        databases: number;
        domains: number;
        subdomains: number;
        monthlyPrice: number;
        yearlyPrice: number;
        isActive: boolean;
    }

    interface BillingCycle {
        id: number;
        code: string;
        name: string;
        durationInDays: number;
        sortOrder: number;
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
        selectedCustomer?: CustomerSummary;
        hostingPackageId?: number;
        billingCycleId?: number;
        hostingSkipped?: boolean;
    }

    const storageKey = 'new-sale-state';

    let currentState: NewSaleState | null = null;
    let hostingPackages: HostingPackage[] = [];
    let billingCycles: BillingCycle[] = [];
    let selectedHostingPackageId: number | null = null;

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
            console.error('New sale hosting request failed', error);
            return {
                success: false,
                message: 'Network error. Please try again.',
            };
        }
    };

    const esc = (text: string): string => {
        const map: Record<string, string> = { '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#039;' };
        return (text || '').replace(/[&<>"']/g, (char) => map[char] ?? char);
    };

    const showSuccess = (message: string): void => {
        const alert = document.getElementById('new-sale-hosting-alert-success');
        if (!alert) {
            return;
        }

        alert.textContent = message;
        alert.classList.remove('d-none');
        document.getElementById('new-sale-hosting-alert-error')?.classList.add('d-none');

        setTimeout(() => alert.classList.add('d-none'), 4000);
    };

    const showError = (message: string): void => {
        const alert = document.getElementById('new-sale-hosting-alert-error');
        if (!alert) {
            return;
        }

        alert.textContent = message;
        alert.classList.remove('d-none');
        document.getElementById('new-sale-hosting-alert-success')?.classList.add('d-none');
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

    const saveState = (): void => {
        if (!currentState) {
            return;
        }

        const billingCycle = getSelectedBillingCycleId();

        currentState.hostingPackageId = selectedHostingPackageId ?? undefined;
        currentState.billingCycleId = billingCycle ?? undefined;
        currentState.hostingSkipped = selectedHostingPackageId === null;

        sessionStorage.setItem(storageKey, JSON.stringify(currentState));
    };

    const normalizeHostingPackage = (item: unknown): HostingPackage => {
        const typed = (item ?? {}) as Record<string, unknown>;

        return {
            id: Number(typed.id ?? typed.Id ?? 0),
            name: String(typed.name ?? typed.Name ?? ''),
            description: String(typed.description ?? typed.Description ?? ''),
            diskSpaceMB: Number(typed.diskSpaceMB ?? typed.DiskSpaceMB ?? 0),
            bandwidthMB: Number(typed.bandwidthMB ?? typed.BandwidthMB ?? 0),
            emailAccounts: Number(typed.emailAccounts ?? typed.EmailAccounts ?? 0),
            databases: Number(typed.databases ?? typed.Databases ?? 0),
            domains: Number(typed.domains ?? typed.Domains ?? 0),
            subdomains: Number(typed.subdomains ?? typed.Subdomains ?? 0),
            monthlyPrice: Number(typed.monthlyPrice ?? typed.MonthlyPrice ?? 0),
            yearlyPrice: Number(typed.yearlyPrice ?? typed.YearlyPrice ?? 0),
            isActive: (typed.isActive ?? typed.IsActive ?? false) === true,
        };
    };

    const normalizeBillingCycle = (item: unknown): BillingCycle => {
        const typed = (item ?? {}) as Record<string, unknown>;
        return {
            id: Number(typed.id ?? typed.Id ?? 0),
            code: String(typed.code ?? typed.Code ?? ''),
            name: String(typed.name ?? typed.Name ?? ''),
            durationInDays: Number(typed.durationInDays ?? typed.DurationInDays ?? 0),
            sortOrder: Number(typed.sortOrder ?? typed.SortOrder ?? 0),
        };
    };

    const setContextHeader = (): void => {
        if (!currentState) {
            return;
        }

        const domain = document.getElementById('new-sale-hosting-domain');
        const flow = document.getElementById('new-sale-hosting-flow');
        const customer = document.getElementById('new-sale-hosting-customer');

        if (domain) {
            domain.textContent = currentState.domainName || '-';
        }

        if (flow) {
            flow.textContent = currentState.flowType || '-';
        }

        if (customer) {
            customer.textContent = currentState.selectedCustomer?.name || currentState.selectedCustomer?.customerName || '-';
        }
    };

    const getSelectedBillingCycleId = (): number | null => {
        const select = document.getElementById('new-sale-hosting-billing-cycle') as HTMLSelectElement | null;
        const parsed = Number(select?.value ?? '');
        return Number.isFinite(parsed) && parsed > 0 ? parsed : null;
    };

    const getSelectedHostingPackage = (): HostingPackage | null => {
        if (selectedHostingPackageId === null) {
            return null;
        }

        const pkg = hostingPackages.find((item) => item.id === selectedHostingPackageId);
        return pkg ?? null;
    };

    const formatPrice = (value: number): string => {
        return value.toFixed(2);
    };

    const resolvePriceForBillingCycle = (pkg: HostingPackage, cycle: BillingCycle): number => {
        const code = cycle.code.toLowerCase();
        const name = cycle.name.toLowerCase();

        if (code.includes('year') || name.includes('year') || cycle.durationInDays >= 360) {
            return pkg.yearlyPrice;
        }

        return pkg.monthlyPrice;
    };

    const renderPricePreview = (): void => {
        const preview = document.getElementById('new-sale-hosting-price-preview');
        if (!preview) {
            return;
        }

        const selectedPackage = getSelectedHostingPackage();
        const selectedCycleId = getSelectedBillingCycleId();
        const selectedCycle = selectedCycleId ? billingCycles.find((item) => item.id === selectedCycleId) : undefined;

        if (!selectedPackage || !selectedCycle) {
            preview.textContent = '-';
            return;
        }

        const value = resolvePriceForBillingCycle(selectedPackage, selectedCycle);
        preview.textContent = `${formatPrice(value)} / ${selectedCycle.name}`;
    };

    const setNextEnabled = (): void => {
        const nextButton = document.getElementById('new-sale-hosting-next') as HTMLButtonElement | null;
        if (!nextButton) {
            return;
        }

        const hasPackage = selectedHostingPackageId !== null;
        const hasCycle = getSelectedBillingCycleId() !== null;
        nextButton.disabled = !(hasPackage && hasCycle);
    };

    const renderPackages = (): void => {
        const wrapper = document.getElementById('new-sale-hosting-packages');
        const count = document.getElementById('new-sale-hosting-package-count');

        if (count) {
            count.textContent = `${hostingPackages.length} package${hostingPackages.length === 1 ? '' : 's'}`;
        }

        if (!wrapper) {
            return;
        }

        if (!hostingPackages.length) {
            wrapper.innerHTML = '<div class="col-12"><div class="alert alert-warning mb-0">No active hosting packages found.</div></div>';
            return;
        }

        wrapper.innerHTML = hostingPackages.map((pkg) => {
            const isSelected = pkg.id === selectedHostingPackageId;
            const activeClass = isSelected ? 'border-primary shadow-sm' : 'border-light';
            const checked = isSelected ? 'checked' : '';

            return `
                <div class="col-12 col-xl-6">
                    <label class="card h-100 ${activeClass}" for="new-sale-hosting-package-${pkg.id}">
                        <div class="card-body">
                            <div class="d-flex justify-content-between align-items-start gap-2 mb-2">
                                <div>
                                    <h6 class="mb-1">${esc(pkg.name)}</h6>
                                    <div class="text-muted small">${esc(pkg.description || 'No description')}</div>
                                </div>
                                <input class="form-check-input mt-1" type="radio" name="new-sale-hosting-package" id="new-sale-hosting-package-${pkg.id}" value="${pkg.id}" ${checked} />
                            </div>
                            <div class="row g-2 small">
                                <div class="col-6"><span class="text-muted">Disk:</span> ${esc(String(pkg.diskSpaceMB))} MB</div>
                                <div class="col-6"><span class="text-muted">Bandwidth:</span> ${esc(String(pkg.bandwidthMB))} MB</div>
                                <div class="col-6"><span class="text-muted">Email:</span> ${esc(String(pkg.emailAccounts))}</div>
                                <div class="col-6"><span class="text-muted">Databases:</span> ${esc(String(pkg.databases))}</div>
                                <div class="col-6"><span class="text-muted">Domains:</span> ${esc(String(pkg.domains))}</div>
                                <div class="col-6"><span class="text-muted">Subdomains:</span> ${esc(String(pkg.subdomains))}</div>
                            </div>
                            <div class="mt-3 small text-muted">Monthly: ${esc(formatPrice(pkg.monthlyPrice))} · Yearly: ${esc(formatPrice(pkg.yearlyPrice))}</div>
                        </div>
                    </label>
                </div>
            `;
        }).join('');
    };

    const renderBillingCycles = (): void => {
        const select = document.getElementById('new-sale-hosting-billing-cycle') as HTMLSelectElement | null;
        if (!select) {
            return;
        }

        if (!billingCycles.length) {
            select.innerHTML = '<option value="">No billing cycles available</option>';
            return;
        }

        const sorted = [...billingCycles].sort((a, b) => a.sortOrder - b.sortOrder || a.durationInDays - b.durationInDays);
        select.innerHTML = `<option value="">Select billing cycle</option>${sorted.map((cycle) => `<option value="${cycle.id}">${esc(cycle.name)} (${cycle.durationInDays} days)</option>`).join('')}`;

        const savedBillingCycleId = currentState?.billingCycleId;
        if (savedBillingCycleId && sorted.some((item) => item.id === savedBillingCycleId)) {
            select.value = String(savedBillingCycleId);
        }
    };

    const loadHostingPackages = async (): Promise<void> => {
        const status = document.getElementById('new-sale-hosting-packages-status');
        if (status) {
            status.innerHTML = '<span class="spinner-border spinner-border-sm me-1"></span>Loading hosting packages...';
        }

        const response = await apiRequest<unknown>(`${getApiBaseUrl()}/HostingPackages/active`, { method: 'GET' });
        if (!response.success) {
            hostingPackages = [];
            renderPackages();
            showError(response.message || 'Failed to load hosting packages.');
            if (status) {
                status.innerHTML = '<span class="text-danger">Failed to load hosting packages.</span>';
            }
            return;
        }

        const raw = response.data;
        const list = Array.isArray(raw)
            ? raw
            : Array.isArray((raw as { data?: unknown[] } | null)?.data)
                ? ((raw as { data?: unknown[] }).data ?? [])
                : [];

        hostingPackages = list.map((item) => normalizeHostingPackage(item)).filter((item) => item.id > 0 && item.isActive);
        const savedPackageId = currentState?.hostingPackageId;
        if (savedPackageId && hostingPackages.some((item) => item.id === savedPackageId)) {
            selectedHostingPackageId = savedPackageId;
        }

        renderPackages();

        if (status) {
            status.innerHTML = `${hostingPackages.length} active hosting package(s) available.`;
        }
    };

    const loadBillingCycles = async (): Promise<void> => {
        const response = await apiRequest<unknown>(`${getApiBaseUrl()}/BillingCycles`, { method: 'GET' });
        if (!response.success) {
            billingCycles = [];
            renderBillingCycles();
            showError(response.message || 'Failed to load billing cycles.');
            return;
        }

        const raw = response.data;
        const list = Array.isArray(raw)
            ? raw
            : Array.isArray((raw as { data?: unknown[] } | null)?.data)
                ? ((raw as { data?: unknown[] }).data ?? [])
                : [];

        billingCycles = list.map((item) => normalizeBillingCycle(item)).filter((item) => item.id > 0);
        renderBillingCycles();
    };

    const selectPackage = (packageId: number): void => {
        if (!hostingPackages.some((item) => item.id === packageId)) {
            return;
        }

        selectedHostingPackageId = packageId;
        renderPackages();
        renderPricePreview();
        setNextEnabled();
        saveState();
    };

    const proceedNext = (): void => {
        if (selectedHostingPackageId === null) {
            showError('Select a hosting package or click Skip hosting.');
            return;
        }

        if (getSelectedBillingCycleId() === null) {
            showError('Select a billing cycle to continue.');
            return;
        }

        saveState();
        window.location.href = '/dashboard/new-sale/services';
    };

    const skipHosting = (): void => {
        selectedHostingPackageId = null;

        if (currentState) {
            currentState.hostingPackageId = undefined;
            currentState.billingCycleId = undefined;
            currentState.hostingSkipped = true;
            sessionStorage.setItem(storageKey, JSON.stringify(currentState));
        }

        showSuccess('Hosting skipped.');
        window.location.href = '/dashboard/new-sale/services';
    };

    const bindEvents = (): void => {
        document.getElementById('new-sale-hosting-packages')?.addEventListener('change', (event) => {
            const target = event.target as HTMLInputElement;
            if (target.name !== 'new-sale-hosting-package') {
                return;
            }

            const packageId = Number(target.value ?? '0');
            if (Number.isFinite(packageId) && packageId > 0) {
                selectPackage(packageId);
            }
        });

        document.getElementById('new-sale-hosting-billing-cycle')?.addEventListener('change', () => {
            renderPricePreview();
            setNextEnabled();
            saveState();
        });

        document.getElementById('new-sale-hosting-next')?.addEventListener('click', proceedNext);
        document.getElementById('new-sale-hosting-skip')?.addEventListener('click', skipHosting);
    };

    const initializePage = async (): Promise<void> => {
        const page = document.getElementById('dashboard-new-sale-hosting-page') as HTMLElement | null;
        if (!page || page.dataset.initialized === 'true') {
            return;
        }

        page.dataset.initialized = 'true';

        currentState = loadState();
        if (!currentState?.domainName || !currentState?.flowType || !currentState?.selectedCustomer) {
            window.location.href = '/dashboard/new-sale';
            return;
        }

        selectedHostingPackageId = currentState.hostingPackageId ?? null;

        setContextHeader();
        bindEvents();
        await Promise.all([loadHostingPackages(), loadBillingCycles()]);
        renderPricePreview();
        setNextEnabled();
    };

    const setupObserver = (): void => {
        void initializePage();

        if (document.body) {
            const observer = new MutationObserver(() => {
                const page = document.getElementById('dashboard-new-sale-hosting-page') as HTMLElement | null;
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

(() => {
    interface AppSettings {
        apiBaseUrl?: string;
    }

    interface ApiResponse<T> {
        success: boolean;
        data?: T;
        message?: string;
    }

    interface Customer {
        id: number;
        name: string;
        customerName: string;
        email: string;
        phone: string;
        status: string;
        isActive: boolean;
        isCompany: boolean;
        customerNumber: number | null;
        referenceNumber: number | null;
        formattedCustomerNumber: string;
        formattedReferenceNumber: string;
    }

    interface ContactPerson {
        id: number;
        firstName: string;
        lastName: string;
        email: string;
        phone: string;
        isActive: boolean;
        isPrimary: boolean;
        isDefaultOwner: boolean;
        isDefaultAdministrator: boolean;
        isDefaultTech: boolean;
        isDefaultBilling: boolean;
        isDomainGlobal: boolean;
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
        selectedCustomer?: Customer;
        domainContacts?: {
            registrantContactId?: number;
            adminContactId?: number;
            techContactId?: number;
            billingContactId?: number;
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

    let currentState: NewSaleState | null = null;
    let customerResults: Customer[] = [];
    let selectedCustomer: Customer | null = null;
    let customerContacts: ContactPerson[] = [];
    let globalDomainContacts: ContactPerson[] = [];

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
            console.error('New sale customer request failed', error);
            return {
                success: false,
                message: 'Network error. Please try again.',
            };
        }
    };

    const showSuccess = (message: string): void => {
        const alert = document.getElementById('new-sale-customer-alert-success');
        if (!alert) {
            return;
        }

        alert.textContent = message;
        alert.classList.remove('d-none');
        document.getElementById('new-sale-customer-alert-error')?.classList.add('d-none');

        setTimeout(() => alert.classList.add('d-none'), 5000);
    };

    const showError = (message: string): void => {
        const alert = document.getElementById('new-sale-customer-alert-error');
        if (!alert) {
            return;
        }

        alert.textContent = message;
        alert.classList.remove('d-none');
        document.getElementById('new-sale-customer-alert-success')?.classList.add('d-none');
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

        currentState.selectedCustomer = selectedCustomer ?? undefined;
        currentState.domainContacts = {
            registrantContactId: getSelectNumber('new-sale-contact-registrant') ?? undefined,
            adminContactId: getSelectNumber('new-sale-contact-admin') ?? undefined,
            techContactId: getSelectNumber('new-sale-contact-tech') ?? undefined,
            billingContactId: getSelectNumber('new-sale-contact-billing') ?? undefined,
        };

        sessionStorage.setItem(storageKey, JSON.stringify(currentState));
    };

    const normalizeCustomer = (item: unknown): Customer => {
        const typed = (item ?? {}) as Record<string, unknown>;

        const customerNumber = Number(typed.customerNumber ?? typed.CustomerNumber);
        const referenceNumber = Number(typed.referenceNumber ?? typed.ReferenceNumber);

        return {
            id: Number(typed.id ?? typed.Id ?? 0),
            name: String(typed.name ?? typed.Name ?? ''),
            customerName: String(typed.customerName ?? typed.CustomerName ?? ''),
            email: String(typed.email ?? typed.Email ?? ''),
            phone: String(typed.phone ?? typed.Phone ?? ''),
            status: String(typed.status ?? typed.Status ?? ''),
            isActive: (typed.isActive ?? typed.IsActive ?? false) === true,
            isCompany: (typed.isCompany ?? typed.IsCompany ?? false) === true,
            customerNumber: Number.isFinite(customerNumber) ? customerNumber : null,
            referenceNumber: Number.isFinite(referenceNumber) ? referenceNumber : null,
            formattedCustomerNumber: String(typed.formattedCustomerNumber ?? typed.FormattedCustomerNumber ?? ''),
            formattedReferenceNumber: String(typed.formattedReferenceNumber ?? typed.FormattedReferenceNumber ?? ''),
        };
    };

    const normalizeContactPerson = (item: unknown): ContactPerson => {
        const typed = (item ?? {}) as Record<string, unknown>;
        return {
            id: Number(typed.id ?? typed.Id ?? 0),
            firstName: String(typed.firstName ?? typed.FirstName ?? ''),
            lastName: String(typed.lastName ?? typed.LastName ?? ''),
            email: String(typed.email ?? typed.Email ?? ''),
            phone: String(typed.phone ?? typed.Phone ?? ''),
            isActive: (typed.isActive ?? typed.IsActive ?? false) === true,
            isPrimary: (typed.isPrimary ?? typed.IsPrimary ?? false) === true,
            isDefaultOwner: (typed.isDefaultOwner ?? typed.IsDefaultOwner ?? false) === true,
            isDefaultAdministrator: (typed.isDefaultAdministrator ?? typed.IsDefaultAdministrator ?? false) === true,
            isDefaultTech: (typed.isDefaultTech ?? typed.IsDefaultTech ?? false) === true,
            isDefaultBilling: (typed.isDefaultBilling ?? typed.IsDefaultBilling ?? false) === true,
            isDomainGlobal: (typed.isDomainGlobal ?? typed.IsDomainGlobal ?? false) === true,
        };
    };

    const getSelectableContacts = (): ContactPerson[] => {
        const byId = new Map<number, ContactPerson>();

        customerContacts.forEach((contact) => {
            byId.set(contact.id, contact);
        });

        globalDomainContacts
            .filter((contact) => contact.isDomainGlobal)
            .forEach((contact) => {
                if (!byId.has(contact.id)) {
                    byId.set(contact.id, contact);
                }
            });

        return Array.from(byId.values());
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

    const getInput = (id: string): string => {
        const el = document.getElementById(id) as HTMLInputElement | null;
        return (el?.value ?? '').trim();
    };

    const getCheckbox = (id: string): boolean => {
        const el = document.getElementById(id) as HTMLInputElement | null;
        return !!el?.checked;
    };

    const getSelectNumber = (id: string): number | null => {
        const el = document.getElementById(id) as HTMLSelectElement | null;
        const value = Number(el?.value ?? '');
        return Number.isFinite(value) && value > 0 ? value : null;
    };

    const setContextHeader = (): void => {
        if (!currentState) {
            return;
        }

        const domain = document.getElementById('new-sale-customer-domain');
        const flow = document.getElementById('new-sale-customer-flow');
        const registrar = document.getElementById('new-sale-customer-registrar');

        if (domain) {
            domain.textContent = currentState.domainName || '-';
        }
        if (flow) {
            flow.textContent = currentState.flowType || '-';
        }
        if (registrar) {
            registrar.textContent = currentState.selectedRegistrarLabel || '-';
        }
    };

    const renderCustomerResults = (): void => {
        const body = document.getElementById('new-sale-customer-results-body');
        if (!body) {
            return;
        }

        if (!customerResults.length) {
            body.innerHTML = '<tr><td colspan="8" class="text-center text-muted">No matching customers found.</td></tr>';
            return;
        }

        body.innerHTML = customerResults.map((customer) => {
            const customerNo = customer.formattedCustomerNumber || (customer.customerNumber !== null ? String(customer.customerNumber) : '-');
            const referenceNo = customer.formattedReferenceNumber || (customer.referenceNumber !== null ? String(customer.referenceNumber) : '-');
            return `
                <tr>
                    <td>${customer.id}</td>
                    <td>${esc(customer.name)}</td>
                    <td>${esc(customer.email)}</td>
                    <td>${esc(customer.phone)}</td>
                    <td>${esc(customer.status || '-')}</td>
                    <td>${esc(customerNo)}</td>
                    <td>${esc(referenceNo)}</td>
                    <td class="text-end">
                        <button class="btn btn-sm btn-outline-primary" type="button" data-action="select-customer" data-id="${customer.id}">
                            <i class="bi bi-check-lg"></i> Select
                        </button>
                    </td>
                </tr>
            `;
        }).join('');
    };

    const applyAdvancedFilters = (items: Customer[]): Customer[] => {
        const name = getInput('new-sale-customer-name').toLowerCase();
        const email = getInput('new-sale-customer-email').toLowerCase();
        const phone = getInput('new-sale-customer-phone').toLowerCase();
        const status = getInput('new-sale-customer-status').toLowerCase();
        const customerNo = getInput('new-sale-customer-customer-number').toLowerCase();
        const referenceNo = getInput('new-sale-customer-reference-number').toLowerCase();
        const onlyActive = getCheckbox('new-sale-customer-only-active');
        const onlyCompany = getCheckbox('new-sale-customer-only-company');

        return items.filter((customer) => {
            if (name && !customer.name.toLowerCase().includes(name) && !customer.customerName.toLowerCase().includes(name)) {
                return false;
            }
            if (email && !customer.email.toLowerCase().includes(email)) {
                return false;
            }
            if (phone && !customer.phone.toLowerCase().includes(phone)) {
                return false;
            }
            if (status && !customer.status.toLowerCase().includes(status)) {
                return false;
            }

            const customerNoValue = (customer.formattedCustomerNumber || (customer.customerNumber !== null ? String(customer.customerNumber) : '')).toLowerCase();
            if (customerNo && !customerNoValue.includes(customerNo)) {
                return false;
            }

            const referenceValue = (customer.formattedReferenceNumber || (customer.referenceNumber !== null ? String(customer.referenceNumber) : '')).toLowerCase();
            if (referenceNo && !referenceValue.includes(referenceNo)) {
                return false;
            }

            if (onlyActive && !customer.isActive) {
                return false;
            }

            if (onlyCompany && !customer.isCompany) {
                return false;
            }

            return true;
        });
    };

    const runCustomerSearch = async (): Promise<void> => {
        const status = document.getElementById('new-sale-customer-search-status');
        if (status) {
            status.innerHTML = '<div class="text-muted small"><span class="spinner-border spinner-border-sm me-1"></span>Searching...</div>';
        }

        const baseQuery = getInput('new-sale-customer-query');
        const advancedQueryParts = [
            getInput('new-sale-customer-name'),
            getInput('new-sale-customer-email'),
            getInput('new-sale-customer-phone'),
            getInput('new-sale-customer-customer-number'),
            getInput('new-sale-customer-reference-number'),
            getInput('new-sale-customer-status'),
        ].filter((part) => !!part);

        const query = (baseQuery || advancedQueryParts.join(' ')).trim();
        if (!query || query.length < 2) {
            if (status) {
                status.innerHTML = '<div class="text-warning small">Enter at least 2 characters.</div>';
            }
            customerResults = [];
            renderCustomerResults();
            return;
        }

        const response = await apiRequest<unknown>(`${getApiBaseUrl()}/Customers/search?query=${encodeURIComponent(query)}`, { method: 'GET' });
        if (!response.success) {
            if (status) {
                status.innerHTML = `<div class="text-danger small">${esc(response.message || 'Search failed')}</div>`;
            }
            customerResults = [];
            renderCustomerResults();
            return;
        }

        const raw = response.data;
        const list = Array.isArray(raw) ? raw : Array.isArray((raw as { data?: unknown[] } | null)?.data) ? ((raw as { data?: unknown[] }).data ?? []) : [];
        const normalized = list.map((item) => normalizeCustomer(item));
        customerResults = applyAdvancedFilters(normalized);

        if (status) {
            status.innerHTML = `<div class="text-muted small">${customerResults.length} customer(s) found.</div>`;
        }

        renderCustomerResults();
    };

    const renderSelectedCustomer = (): void => {
        const card = document.getElementById('new-sale-customer-selected-card');
        const summary = document.getElementById('new-sale-customer-selected-summary');
        const contactsCard = document.getElementById('new-sale-contact-persons-card');

        if (!card || !summary || !contactsCard) {
            return;
        }

        if (!selectedCustomer) {
            card.classList.add('d-none');
            contactsCard.classList.add('d-none');
            summary.innerHTML = '';
            return;
        }

        const customerNo = selectedCustomer.formattedCustomerNumber || (selectedCustomer.customerNumber !== null ? String(selectedCustomer.customerNumber) : '-');
        const referenceNo = selectedCustomer.formattedReferenceNumber || (selectedCustomer.referenceNumber !== null ? String(selectedCustomer.referenceNumber) : '-');

        summary.innerHTML = `
            <div class="row g-2">
                <div class="col-12 col-md-6"><strong>Name:</strong> ${esc(selectedCustomer.name)}</div>
                <div class="col-12 col-md-6"><strong>Contact name:</strong> ${esc(selectedCustomer.customerName || '-')}</div>
                <div class="col-12 col-md-6"><strong>Email:</strong> ${esc(selectedCustomer.email)}</div>
                <div class="col-12 col-md-6"><strong>Phone:</strong> ${esc(selectedCustomer.phone)}</div>
                <div class="col-12 col-md-6"><strong>Customer #:</strong> ${esc(customerNo)}</div>
                <div class="col-12 col-md-6"><strong>Reference #:</strong> ${esc(referenceNo)}</div>
            </div>
        `;

        card.classList.remove('d-none');
        contactsCard.classList.remove('d-none');
    };

    const defaultsToLabel = (contact: ContactPerson): string => {
        const labels: string[] = [];
        if (contact.isDefaultOwner) labels.push('Owner');
        if (contact.isDefaultAdministrator) labels.push('Admin');
        if (contact.isDefaultTech) labels.push('Tech');
        if (contact.isDefaultBilling) labels.push('Billing');

        return labels.length ? labels.join(', ') : '—';
    };

    const setRoleSelectOptions = (id: string, contacts: ContactPerson[], selectedId: number | undefined, preferenceSelector: (contact: ContactPerson) => boolean): void => {
        const select = document.getElementById(id) as HTMLSelectElement | null;
        if (!select) {
            return;
        }

        const customerContactIds = new Set<number>(customerContacts.map((contact) => contact.id));

        const options = contacts.map((contact) => {
            const fullName = `${contact.firstName} ${contact.lastName}`.trim();
            const isGlobalOnlyContact = contact.isDomainGlobal && !customerContactIds.has(contact.id);
            const label = isGlobalOnlyContact
                ? `${fullName || contact.email} (Global)`
                : (fullName || contact.email);

            return `<option value="${contact.id}">${esc(label)}</option>`;
        }).join('');

        select.innerHTML = `<option value="">Select contact</option>${options}`;

        if (selectedId && contacts.some((c) => c.id === selectedId)) {
            select.value = String(selectedId);
            return;
        }

        const preferred = contacts.find(preferenceSelector) ?? contacts.find((c) => c.isPrimary) ?? contacts[0];
        if (preferred) {
            select.value = String(preferred.id);
        }
    };

    const renderContacts = (): void => {
        const body = document.getElementById('new-sale-contact-persons-body');
        if (!body) {
            return;
        }

        if (!customerContacts.length) {
            body.innerHTML = '<tr><td colspan="5" class="text-center text-muted">No contact persons found for selected customer.</td></tr>';
        } else {
            body.innerHTML = customerContacts.map((contact) => {
                const fullName = `${contact.firstName} ${contact.lastName}`.trim();
                return `
                    <tr>
                        <td>${esc(fullName || '-')}</td>
                        <td>${esc(contact.email)}</td>
                        <td>${esc(contact.phone)}</td>
                        <td>${esc(defaultsToLabel(contact))}</td>
                        <td>${contact.isActive ? '<span class="badge bg-success">Active</span>' : '<span class="badge bg-secondary">Inactive</span>'}</td>
                    </tr>
                `;
            }).join('');
        }

        const selectableContacts = getSelectableContacts();
        const saved = currentState?.domainContacts;
        setRoleSelectOptions('new-sale-contact-registrant', selectableContacts, saved?.registrantContactId, (c) => c.isDefaultOwner);
        setRoleSelectOptions('new-sale-contact-admin', selectableContacts, saved?.adminContactId, (c) => c.isDefaultAdministrator);
        setRoleSelectOptions('new-sale-contact-tech', selectableContacts, saved?.techContactId, (c) => c.isDefaultTech);
        setRoleSelectOptions('new-sale-contact-billing', selectableContacts, saved?.billingContactId, (c) => c.isDefaultBilling);

        saveState();
    };

    const loadGlobalDomainContacts = async (): Promise<void> => {
        const response = await apiRequest<unknown>(`${getApiBaseUrl()}/ContactPersons/domain-global`, { method: 'GET' });
        if (!response.success) {
            globalDomainContacts = [];
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

        globalDomainContacts = list.map((item) => normalizeContactPerson(item));
    };

    const loadContactPersons = async (customerId: number): Promise<void> => {
        const body = document.getElementById('new-sale-contact-persons-body');
        if (body) {
            body.innerHTML = '<tr><td colspan="5" class="text-center"><span class="spinner-border spinner-border-sm"></span> Loading...</td></tr>';
        }

        const response = await apiRequest<unknown>(`${getApiBaseUrl()}/ContactPersons/customer/${customerId}`, { method: 'GET' });
        if (!response.success) {
            customerContacts = [];
            renderContacts();
            showError(response.message || 'Failed to load contact persons');
            return;
        }

        const raw = response.data;
        const list = Array.isArray(raw) ? raw : Array.isArray((raw as { data?: unknown[] } | null)?.data) ? ((raw as { data?: unknown[] }).data ?? []) : [];
        customerContacts = list.map((item) => normalizeContactPerson(item));
        await loadGlobalDomainContacts();
        renderContacts();
    };

    const selectCustomer = async (id: number): Promise<void> => {
        const customer = customerResults.find((item) => item.id === id);
        if (!customer) {
            return;
        }

        selectedCustomer = customer;
        renderSelectedCustomer();
        saveState();

        await loadContactPersons(customer.id);
    };

    const resetSearchFilters = (): void => {
        const ids = [
            'new-sale-customer-query',
            'new-sale-customer-name',
            'new-sale-customer-email',
            'new-sale-customer-phone',
            'new-sale-customer-status',
            'new-sale-customer-customer-number',
            'new-sale-customer-reference-number',
        ];

        ids.forEach((id) => {
            const el = document.getElementById(id) as HTMLInputElement | null;
            if (el) {
                el.value = '';
            }
        });

        const active = document.getElementById('new-sale-customer-only-active') as HTMLInputElement | null;
        const company = document.getElementById('new-sale-customer-only-company') as HTMLInputElement | null;
        if (active) active.checked = false;
        if (company) company.checked = false;

        customerResults = [];
        renderCustomerResults();

        const status = document.getElementById('new-sale-customer-search-status');
        if (status) {
            status.innerHTML = '';
        }
    };

    const openAddContactModal = (): void => {
        if (!selectedCustomer) {
            showError('Select a customer before adding contact persons.');
            return;
        }

        const form = document.getElementById('new-sale-contact-add-form') as HTMLFormElement | null;
        form?.reset();

        const active = document.getElementById('new-sale-contact-active') as HTMLInputElement | null;
        if (active) {
            active.checked = true;
        }

        showModal('new-sale-contact-add-modal');
    };

    const saveContactPerson = async (): Promise<void> => {
        if (!selectedCustomer) {
            showError('Select a customer before adding contact persons.');
            return;
        }

        const firstName = getInput('new-sale-contact-first-name');
        const lastName = getInput('new-sale-contact-last-name');
        const email = getInput('new-sale-contact-email');
        const phone = getInput('new-sale-contact-phone');

        if (!firstName || !lastName || !email || !phone) {
            showError('First name, last name, email and phone are required.');
            return;
        }

        const payload = {
            firstName,
            lastName,
            email,
            phone,
            position: getInput('new-sale-contact-position') || null,
            department: getInput('new-sale-contact-department') || null,
            notes: getInput('new-sale-contact-notes') || null,
            isPrimary: getCheckbox('new-sale-contact-primary'),
            isActive: getCheckbox('new-sale-contact-active'),
            isDefaultOwner: getCheckbox('new-sale-contact-default-owner'),
            isDefaultAdministrator: getCheckbox('new-sale-contact-default-admin'),
            isDefaultTech: getCheckbox('new-sale-contact-default-tech'),
            isDefaultBilling: getCheckbox('new-sale-contact-default-billing'),
            customerId: selectedCustomer.id,
        };

        const response = await apiRequest<unknown>(`${getApiBaseUrl()}/ContactPersons`, {
            method: 'POST',
            body: JSON.stringify(payload),
        });

        if (!response.success) {
            showError(response.message || 'Failed to create contact person.');
            return;
        }

        hideModal('new-sale-contact-add-modal');
        showSuccess('Contact person created successfully.');
        await loadContactPersons(selectedCustomer.id);
    };

    const continueToPage3 = (): void => {
        if (!selectedCustomer) {
            showError('Select a customer to continue.');
            return;
        }

        saveState();
        window.location.href = '/dashboard/new-sale/hosting';
    };

    const bindEvents = (): void => {
        const form = document.getElementById('new-sale-customer-search-form') as HTMLFormElement | null;
        const tableBody = document.getElementById('new-sale-customer-results-body');
        const createButton = document.getElementById('new-sale-customer-create');
        const resetButton = document.getElementById('new-sale-customer-reset');
        const toggleAdvanced = document.getElementById('new-sale-customer-advanced-toggle');
        const nextButton = document.getElementById('new-sale-customer-next');
        const clearSelection = document.getElementById('new-sale-customer-clear-selection');

        form?.addEventListener('submit', (event) => {
            event.preventDefault();
            void runCustomerSearch();
        });

        tableBody?.addEventListener('click', (event) => {
            const target = event.target as HTMLElement;
            const button = target.closest<HTMLButtonElement>('button[data-action="select-customer"]');
            if (!button) {
                return;
            }

            const id = Number(button.dataset.id ?? '0');
            if (Number.isFinite(id) && id > 0) {
                void selectCustomer(id);
            }
        });

        createButton?.addEventListener('click', () => {
            document.dispatchEvent(new CustomEvent('customers:open-create'));
        });

        document.addEventListener('customers:saved', () => {
            showSuccess('Customer saved. Run search to select the new customer.');
            void runCustomerSearch();
        });

        resetButton?.addEventListener('click', resetSearchFilters);

        toggleAdvanced?.addEventListener('click', () => {
            const section = document.getElementById('new-sale-customer-advanced');
            section?.classList.toggle('d-none');
        });

        clearSelection?.addEventListener('click', () => {
            selectedCustomer = null;
            customerContacts = [];
            renderSelectedCustomer();
            renderContacts();
            saveState();
        });

        document.getElementById('new-sale-contact-add')?.addEventListener('click', openAddContactModal);
        document.getElementById('new-sale-contact-add-save')?.addEventListener('click', () => { void saveContactPerson(); });

        ['new-sale-contact-registrant', 'new-sale-contact-admin', 'new-sale-contact-tech', 'new-sale-contact-billing']
            .forEach((id) => {
                document.getElementById(id)?.addEventListener('change', saveState);
            });

        nextButton?.addEventListener('click', continueToPage3);
    };

    const restoreSelectedCustomer = async (): Promise<void> => {
        if (!currentState?.selectedCustomer) {
            renderSelectedCustomer();
            return;
        }

        selectedCustomer = currentState.selectedCustomer;
        renderSelectedCustomer();
        await loadContactPersons(selectedCustomer.id);
    };

    const initializePage = async (): Promise<void> => {
        const page = document.getElementById('dashboard-new-sale-customer-page') as HTMLElement | null;
        if (!page || page.dataset.initialized === 'true') {
            return;
        }

        page.dataset.initialized = 'true';

        currentState = loadState();
        if (!currentState?.domainName || !currentState?.flowType) {
            window.location.href = '/dashboard/new-sale';
            return;
        }

        setContextHeader();
        bindEvents();
        await restoreSelectedCustomer();
    };

    const setupObserver = (): void => {
        void initializePage();

        if (document.body) {
            const observer = new MutationObserver(() => {
                const page = document.getElementById('dashboard-new-sale-customer-page') as HTMLElement | null;
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

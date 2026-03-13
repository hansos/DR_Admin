(() => {
    interface AppSettings { apiBaseUrl?: string; }
    interface ApiResponse<T> { success: boolean; data?: T; message?: string; }

    interface Customer {
        id: number;
        referenceNumber?: number;
        formattedReferenceNumber?: string;
        customerNumber?: number | null;
        formattedCustomerNumber?: string | null;
        name: string;
        customerName?: string | null;
        email: string;
        billingEmail?: string | null;
        phone: string;
        countryCode?: string | null;
        taxId?: string | null;
        vatNumber?: string | null;
        status?: string;
        preferredCurrency?: string;
        preferredPaymentMethod?: string | null;
        creditLimit?: number;
        balance?: number;
        notes?: string | null;
        createdAt?: string;
    }

    interface ContactPerson {
        id: number;
        firstName: string;
        lastName: string;
        email: string;
        phone: string;
        position?: string | null;
        department?: string | null;
        notes?: string | null;
        isPrimary: boolean;
        isActive: boolean;
        isDefaultOwner: boolean;
        isDefaultBilling: boolean;
        isDefaultTech: boolean;
        isDefaultAdministrator: boolean;
        isDomainGlobal: boolean;
    }

    interface CustomerInternalNote {
        id: number;
        customerId: number;
        note: string;
        createdByUserId?: number | null;
        createdAt?: string;
    }

    interface CustomerChange {
        id: number;
        customerId: number;
        changeType: string;
        fieldName?: string | null;
        oldValue?: string | null;
        newValue?: string | null;
        changedByUserId?: number | null;
        changedAt?: string;
    }

    interface RegisteredDomain {
        id: number;
        name: string;
        status?: string;
        expirationDate?: string;
    }

    interface SoldHostingPackage {
        id: number;
        registeredDomainId?: number | null;
        hostingPackageId: number;
        status?: string;
        connectedDomainName?: string | null;
    }

    interface SoldOptionalService {
        id: number;
        registeredDomainId?: number | null;
        serviceName?: string | null;
        status?: string;
        connectedDomainName?: string | null;
    }

    let currentCustomerId = 0;
    let editingContactId: number | null = null;
    let allContacts: ContactPerson[] = [];
    let allInternalNotes: CustomerInternalNote[] = [];
    let allChanges: CustomerChange[] = [];
    let allDomains: RegisteredDomain[] = [];
    let allSoldHostingPackages: SoldHostingPackage[] = [];
    let allSoldOptionalServices: SoldOptionalService[] = [];

    const getApiBaseUrl = (): string => ((window as Window & { AppSettings?: AppSettings }).AppSettings?.apiBaseUrl ?? '');

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

    const formatDate = (value?: string): string => {
        if (!value) return '-';
        const date = new Date(value);
        return Number.isNaN(date.getTime()) ? '-' : date.toLocaleString();
    };

    const formatMoney = (amount?: number, currency = 'EUR'): string => {
        const value = Number.isFinite(amount) ? Number(amount) : 0;
        try {
            return new Intl.NumberFormat(undefined, { style: 'currency', currency }).format(value);
        } catch {
            return `${value.toFixed(2)} ${currency}`;
        }
    };

    const apiRequest = async <T,>(endpoint: string, options: RequestInit = {}): Promise<ApiResponse<T>> => {
        try {
            const headers: Record<string, string> = {
                'Content-Type': 'application/json',
                ...(options.headers as Record<string, string> | undefined),
            };

            const token = getAuthToken();
            if (token) headers.Authorization = `Bearer ${token}`;

            const response = await fetch(endpoint, { ...options, headers, credentials: 'include' });
            const contentType = response.headers.get('content-type') ?? '';
            const hasJson = contentType.includes('application/json');
            const body = hasJson ? await response.json() : null;

            if (!response.ok) {
                const err = body as { message?: string; title?: string } | null;
                return { success: false, message: err?.message ?? err?.title ?? `Request failed with status ${response.status}` };
            }

            const envelope = body as { success?: boolean; data?: T; message?: string } | null;
            return { success: envelope?.success !== false, data: envelope?.data ?? (body as T), message: envelope?.message };
        } catch {
            return { success: false, message: 'Network error. Please try again.' };
        }
    };

    const showSuccess = (message: string): void => {
        const alert = document.getElementById('customer-details-alert-success');
        if (!alert) return;
        alert.textContent = message;
        alert.classList.remove('d-none');
        document.getElementById('customer-details-alert-error')?.classList.add('d-none');
    };

    const showError = (message: string): void => {
        const alert = document.getElementById('customer-details-alert-error');
        if (!alert) return;
        alert.textContent = message;
        alert.classList.remove('d-none');
        document.getElementById('customer-details-alert-success')?.classList.add('d-none');
    };

    const setText = (id: string, value: string): void => {
        const el = document.getElementById(id);
        if (el) el.textContent = value;
    };

    const getInputValue = (id: string): string => {
        const el = document.getElementById(id) as HTMLInputElement | HTMLTextAreaElement | null;
        return (el?.value ?? '').trim();
    };

    const setInputValue = (id: string, value: string): void => {
        const el = document.getElementById(id) as HTMLInputElement | HTMLTextAreaElement | null;
        if (el) el.value = value;
    };

    const getCheckboxValue = (id: string): boolean => {
        const el = document.getElementById(id) as HTMLInputElement | null;
        return !!el?.checked;
    };

    const setCheckboxValue = (id: string, value: boolean): void => {
        const el = document.getElementById(id) as HTMLInputElement | null;
        if (el) el.checked = value;
    };

    const getBootstrap = (): { Modal: { new(el: Element): { show(): void; hide(): void }; getInstance(el: Element): { hide(): void } | null } } | null => {
        return (window as Window & { bootstrap?: any }).bootstrap ?? null;
    };

    const showModal = (id: string): void => {
        const modal = document.getElementById(id);
        const bootstrap = getBootstrap();
        if (!modal || !bootstrap) return;
        new bootstrap.Modal(modal).show();
    };

    const hideModal = (id: string): void => {
        const modal = document.getElementById(id);
        const bootstrap = getBootstrap();
        if (!modal || !bootstrap) return;
        bootstrap.Modal.getInstance(modal)?.hide();
    };

    const normalizeContact = (item: any): ContactPerson => ({
        id: Number(item.id ?? item.Id ?? 0),
        firstName: String(item.firstName ?? item.FirstName ?? ''),
        lastName: String(item.lastName ?? item.LastName ?? ''),
        email: String(item.email ?? item.Email ?? ''),
        phone: String(item.phone ?? item.Phone ?? ''),
        position: item.position ?? item.Position ?? null,
        department: item.department ?? item.Department ?? null,
        notes: item.notes ?? item.Notes ?? null,
        isPrimary: Boolean(item.isPrimary ?? item.IsPrimary ?? false),
        isActive: Boolean(item.isActive ?? item.IsActive ?? false),
        isDefaultOwner: Boolean(item.isDefaultOwner ?? item.IsDefaultOwner ?? false),
        isDefaultBilling: Boolean(item.isDefaultBilling ?? item.IsDefaultBilling ?? false),
        isDefaultTech: Boolean(item.isDefaultTech ?? item.IsDefaultTech ?? false),
        isDefaultAdministrator: Boolean(item.isDefaultAdministrator ?? item.IsDefaultAdministrator ?? false),
        isDomainGlobal: Boolean(item.isDomainGlobal ?? item.IsDomainGlobal ?? false),
    });

    const normalizeInternalNote = (item: any): CustomerInternalNote => ({
        id: Number(item.id ?? item.Id ?? 0),
        customerId: Number(item.customerId ?? item.CustomerId ?? 0),
        note: String(item.note ?? item.Note ?? ''),
        createdByUserId: item.createdByUserId ?? item.CreatedByUserId ?? null,
        createdAt: String(item.createdAt ?? item.CreatedAt ?? ''),
    });

    const normalizeCustomerChange = (item: any): CustomerChange => ({
        id: Number(item.id ?? item.Id ?? 0),
        customerId: Number(item.customerId ?? item.CustomerId ?? 0),
        changeType: String(item.changeType ?? item.ChangeType ?? ''),
        fieldName: String(item.fieldName ?? item.FieldName ?? '') || null,
        oldValue: String(item.oldValue ?? item.OldValue ?? '') || null,
        newValue: String(item.newValue ?? item.NewValue ?? '') || null,
        changedByUserId: item.changedByUserId ?? item.ChangedByUserId ?? null,
        changedAt: String(item.changedAt ?? item.ChangedAt ?? ''),
    });

    const normalizeRegisteredDomain = (item: any): RegisteredDomain => ({
        id: Number(item.id ?? item.Id ?? 0),
        name: String(item.name ?? item.Name ?? ''),
        status: String(item.status ?? item.Status ?? ''),
        expirationDate: String(item.expirationDate ?? item.ExpirationDate ?? ''),
    });

    const normalizeSoldHostingPackage = (item: any): SoldHostingPackage => ({
        id: Number(item.id ?? item.Id ?? 0),
        registeredDomainId: item.registeredDomainId ?? item.RegisteredDomainId ?? null,
        hostingPackageId: Number(item.hostingPackageId ?? item.HostingPackageId ?? 0),
        status: String(item.status ?? item.Status ?? ''),
        connectedDomainName: String(item.connectedDomainName ?? item.ConnectedDomainName ?? '') || null,
    });

    const normalizeSoldOptionalService = (item: any): SoldOptionalService => ({
        id: Number(item.id ?? item.Id ?? 0),
        registeredDomainId: item.registeredDomainId ?? item.RegisteredDomainId ?? null,
        serviceName: String(item.serviceName ?? item.ServiceName ?? '') || null,
        status: String(item.status ?? item.Status ?? ''),
        connectedDomainName: String(item.connectedDomainName ?? item.ConnectedDomainName ?? '') || null,
    });

    const renderContacts = (): void => {
        const body = document.getElementById('customer-details-contacts-body');
        if (!body) return;

        setText('customer-details-contact-count', String(allContacts.length));

        if (!allContacts.length) {
            body.innerHTML = '<tr><td colspan="7" class="text-center text-muted">No contact persons found.</td></tr>';
            return;
        }

        const rows = [...allContacts].sort((a, b) => Number(b.isPrimary) - Number(a.isPrimary) || a.id - b.id);
        body.innerHTML = rows.map((contact) => {
            const fullName = `${contact.firstName} ${contact.lastName}`.trim();
            return `
                <tr>
                    <td>${esc(fullName || '-')}</td>
                    <td><a href="mailto:${esc(contact.email)}">${esc(contact.email)}</a></td>
                    <td>${esc(contact.phone || '-')}</td>
                    <td>${esc(contact.position || '-')}</td>
                    <td>${contact.isPrimary ? '<span class="badge bg-info">Yes</span>' : '<span class="badge bg-secondary">No</span>'}</td>
                    <td>${contact.isActive ? '<span class="badge bg-success">Yes</span>' : '<span class="badge bg-secondary">No</span>'}</td>
                    <td class="text-end">
                        <button class="btn btn-sm btn-outline-primary" type="button" data-action="edit-contact" data-id="${contact.id}"><i class="bi bi-pencil"></i></button>
                    </td>
                </tr>
            `;
        }).join('');
    };

    const openContactCreate = (): void => {
        editingContactId = null;
        setText('customer-details-contact-modal-title', 'Add Contact Person');
        (document.getElementById('customer-details-contact-form') as HTMLFormElement | null)?.reset();
        setCheckboxValue('customer-details-contact-active', true);
        showModal('customer-details-contact-modal');
    };

    const openContactEdit = (id: number): void => {
        const contact = allContacts.find((item) => item.id === id);
        if (!contact) return;

        editingContactId = id;
        setText('customer-details-contact-modal-title', 'Edit Contact Person');

        setInputValue('customer-details-contact-first-name', contact.firstName);
        setInputValue('customer-details-contact-last-name', contact.lastName);
        setInputValue('customer-details-contact-email', contact.email);
        setInputValue('customer-details-contact-phone', contact.phone);
        setInputValue('customer-details-contact-position', contact.position || '');
        setInputValue('customer-details-contact-department', contact.department || '');
        setInputValue('customer-details-contact-notes', contact.notes || '');

        setCheckboxValue('customer-details-contact-primary', contact.isPrimary);
        setCheckboxValue('customer-details-contact-active', contact.isActive);
        setCheckboxValue('customer-details-contact-default-owner', contact.isDefaultOwner);
        setCheckboxValue('customer-details-contact-default-billing', contact.isDefaultBilling);
        setCheckboxValue('customer-details-contact-default-tech', contact.isDefaultTech);
        setCheckboxValue('customer-details-contact-default-admin', contact.isDefaultAdministrator);
        setCheckboxValue('customer-details-contact-domain-global', contact.isDomainGlobal);

        showModal('customer-details-contact-modal');
    };

    const loadContacts = async (): Promise<void> => {
        const response = await apiRequest<any[]>(`${getApiBaseUrl()}/ContactPersons/customer/${currentCustomerId}`, { method: 'GET' });
        if (!response.success) {
            showError(response.message || 'Failed to load contact persons.');
            return;
        }

        allContacts = (Array.isArray(response.data) ? response.data : []).map(normalizeContact);
        renderContacts();
    };

    const renderDomains = (): void => {
        const body = document.getElementById('customer-details-domains-body');
        if (!body) {
            return;
        }

        setText('customer-details-domains-count', String(allDomains.length));

        if (!allDomains.length) {
            body.innerHTML = '<tr><td colspan="6" class="text-center text-muted">No domains found for this customer.</td></tr>';
            return;
        }

        const hostingByDomain = new Map<number, SoldHostingPackage[]>();
        for (const item of allSoldHostingPackages) {
            const domainId = Number(item.registeredDomainId ?? 0);
            if (!domainId) {
                continue;
            }

            const existing = hostingByDomain.get(domainId) ?? [];
            existing.push(item);
            hostingByDomain.set(domainId, existing);
        }

        const optionalByDomain = new Map<number, SoldOptionalService[]>();
        for (const item of allSoldOptionalServices) {
            const domainId = Number(item.registeredDomainId ?? 0);
            if (!domainId) {
                continue;
            }

            const existing = optionalByDomain.get(domainId) ?? [];
            existing.push(item);
            optionalByDomain.set(domainId, existing);
        }

        const renderHosting = (domainId: number): string => {
            const items = hostingByDomain.get(domainId) ?? [];
            if (!items.length) {
                return '-';
            }

            return items.map((item) => esc(`Hosting #${item.hostingPackageId}${item.status ? ` (${item.status})` : ''}`)).join('<br/>');
        };

        const renderOptional = (domainId: number): string => {
            const items = optionalByDomain.get(domainId) ?? [];
            if (!items.length) {
                return '-';
            }

            return items
                .map((item) => esc(`${item.serviceName || `Service #${item.id}`}${item.status ? ` (${item.status})` : ''}`))
                .join('<br/>');
        };

        body.innerHTML = allDomains
            .slice()
            .sort((a, b) => (a.name || '').localeCompare(b.name || ''))
            .map((domain) => `
                <tr>
                    <td><code>${esc(domain.name || '-')}</code></td>
                    <td>${esc(domain.status || '-')}</td>
                    <td>${esc(formatDate(domain.expirationDate))}</td>
                    <td>${renderHosting(domain.id)}</td>
                    <td>${renderOptional(domain.id)}</td>
                    <td class="text-end">
                        <a class="btn btn-sm btn-outline-primary" href="/domains/details?id=${domain.id}" title="Open domain">
                            <i class="bi bi-box-arrow-up-right"></i>
                        </a>
                    </td>
                </tr>
            `)
            .join('');
    };

    const loadDomainsWithServices = async (): Promise<void> => {
        const [domainsResponse, hostingResponse, optionalResponse] = await Promise.all([
            apiRequest<any[]>(`${getApiBaseUrl()}/RegisteredDomains/customer/${currentCustomerId}`, { method: 'GET' }),
            apiRequest<any[]>(`${getApiBaseUrl()}/SoldHostingPackages/customer/${currentCustomerId}`, { method: 'GET' }),
            apiRequest<any[]>(`${getApiBaseUrl()}/SoldOptionalServices/customer/${currentCustomerId}`, { method: 'GET' }),
        ]);

        if (!domainsResponse.success) {
            showError(domainsResponse.message || 'Failed to load customer domains.');
            allDomains = [];
            renderDomains();
            return;
        }

        allDomains = (Array.isArray(domainsResponse.data) ? domainsResponse.data : []).map(normalizeRegisteredDomain);

        allSoldHostingPackages = hostingResponse.success
            ? (Array.isArray(hostingResponse.data) ? hostingResponse.data : []).map(normalizeSoldHostingPackage)
            : [];

        allSoldOptionalServices = optionalResponse.success
            ? (Array.isArray(optionalResponse.data) ? optionalResponse.data : []).map(normalizeSoldOptionalService)
            : [];

        renderDomains();
    };

    const renderInternalNotes = (): void => {
        const body = document.getElementById('customer-details-internal-notes-body');
        if (!body) {
            return;
        }

        setText('customer-details-notes-count', String(allInternalNotes.length));

        if (!allInternalNotes.length) {
            body.innerHTML = '<tr><td colspan="3" class="text-center text-muted">No internal notes found.</td></tr>';
            return;
        }

        body.innerHTML = allInternalNotes.map((note) => `
            <tr>
                <td>${esc(formatDate(note.createdAt))}</td>
                <td>${esc(note.note || '-')}</td>
                <td>${note.createdByUserId ? `#${note.createdByUserId}` : '-'}</td>
            </tr>
        `).join('');
    };

    const renderChanges = (): void => {
        const body = document.getElementById('customer-details-changes-body');
        if (!body) {
            return;
        }

        setText('customer-details-changes-count', String(allChanges.length));

        if (!allChanges.length) {
            body.innerHTML = '<tr><td colspan="6" class="text-center text-muted">No customer changes found.</td></tr>';
            return;
        }

        body.innerHTML = allChanges.map((change) => `
            <tr>
                <td>${esc(formatDate(change.changedAt))}</td>
                <td>${esc(change.changeType || '-')}</td>
                <td>${esc(change.fieldName || '-')}</td>
                <td>${esc(change.oldValue || '-')}</td>
                <td>${esc(change.newValue || '-')}</td>
                <td>${change.changedByUserId ? `#${change.changedByUserId}` : '-'}</td>
            </tr>
        `).join('');
    };

    const loadInternalNotes = async (): Promise<void> => {
        const response = await apiRequest<any[]>(`${getApiBaseUrl()}/Customers/${currentCustomerId}/internal-notes`, { method: 'GET' });
        if (!response.success) {
            showError(response.message || 'Failed to load internal notes.');
            return;
        }

        allInternalNotes = (Array.isArray(response.data) ? response.data : []).map(normalizeInternalNote);
        renderInternalNotes();
    };

    const loadChanges = async (): Promise<void> => {
        const response = await apiRequest<any[]>(`${getApiBaseUrl()}/Customers/${currentCustomerId}/changes`, { method: 'GET' });
        if (!response.success) {
            showError(response.message || 'Failed to load customer changes.');
            return;
        }

        allChanges = (Array.isArray(response.data) ? response.data : []).map(normalizeCustomerChange);
        renderChanges();
    };

    const addInternalNote = async (): Promise<void> => {
        const noteInput = document.getElementById('customer-details-internal-note-input') as HTMLTextAreaElement | null;
        const note = (noteInput?.value ?? '').trim();

        if (!note) {
            showError('Internal note cannot be empty.');
            return;
        }

        const response = await apiRequest(`${getApiBaseUrl()}/Customers/${currentCustomerId}/internal-notes`, {
            method: 'POST',
            body: JSON.stringify({ note }),
        });

        if (!response.success) {
            showError(response.message || 'Failed to add internal note.');
            return;
        }

        if (noteInput) {
            noteInput.value = '';
        }

        showSuccess('Internal note added.');
        await loadInternalNotes();
    };

    const saveContact = async (): Promise<void> => {
        const firstName = getInputValue('customer-details-contact-first-name');
        const lastName = getInputValue('customer-details-contact-last-name');
        const email = getInputValue('customer-details-contact-email');
        const phone = getInputValue('customer-details-contact-phone');

        if (!firstName || !lastName || !email || !phone) {
            showError('First name, last name, email and phone are required.');
            return;
        }

        const payload = {
            firstName,
            lastName,
            email,
            phone,
            position: getInputValue('customer-details-contact-position') || null,
            department: getInputValue('customer-details-contact-department') || null,
            notes: getInputValue('customer-details-contact-notes') || null,
            customerId: currentCustomerId,
            isPrimary: getCheckboxValue('customer-details-contact-primary'),
            isActive: getCheckboxValue('customer-details-contact-active'),
            isDefaultOwner: getCheckboxValue('customer-details-contact-default-owner'),
            isDefaultBilling: getCheckboxValue('customer-details-contact-default-billing'),
            isDefaultTech: getCheckboxValue('customer-details-contact-default-tech'),
            isDefaultAdministrator: getCheckboxValue('customer-details-contact-default-admin'),
            isDomainGlobal: getCheckboxValue('customer-details-contact-domain-global'),
        };

        const response = editingContactId
            ? await apiRequest(`${getApiBaseUrl()}/ContactPersons/${editingContactId}`, { method: 'PUT', body: JSON.stringify(payload) })
            : await apiRequest(`${getApiBaseUrl()}/ContactPersons`, { method: 'POST', body: JSON.stringify(payload) });

        if (!response.success) {
            showError(response.message || 'Failed to save contact person.');
            return;
        }

        hideModal('customer-details-contact-modal');
        showSuccess(editingContactId ? 'Contact person updated.' : 'Contact person created.');
        await loadContacts();
    };

    const renderCustomer = (customer: Customer): void => {
        setText('customer-details-id', String(customer.id));
        setText('customer-details-reference', customer.formattedReferenceNumber || String(customer.referenceNumber ?? '-'));
        setText('customer-details-customer-number', customer.formattedCustomerNumber || String(customer.customerNumber ?? '-'));
        setText('customer-details-name', customer.name || '-');
        setText('customer-details-customer-name', customer.customerName || '-');
        setText('customer-details-email', customer.email || '-');
        setText('customer-details-billing-email', customer.billingEmail || '-');
        setText('customer-details-phone', customer.phone || '-');
        setText('customer-details-country', customer.countryCode || '-');
        setText('customer-details-tax-id', customer.taxId || '-');
        setText('customer-details-vat-number', customer.vatNumber || '-');
        setText('customer-details-currency', customer.preferredCurrency || '-');
        setText('customer-details-payment-method', customer.preferredPaymentMethod || '-');
        setText('customer-details-credit-limit', formatMoney(customer.creditLimit, customer.preferredCurrency || 'EUR'));
        setText('customer-details-balance', formatMoney(customer.balance, customer.preferredCurrency || 'EUR'));
        setText('customer-details-created', formatDate(customer.createdAt));
        setText('customer-details-notes', customer.notes || '-');
        setText('customer-details-status-badge', customer.status || '-');
    };

    const loadCustomer = async (): Promise<void> => {
        const response = await apiRequest<Customer>(`${getApiBaseUrl()}/Customers/${currentCustomerId}`, { method: 'GET' });
        if (!response.success || !response.data) {
            showError(response.message || 'Failed to load customer details.');
            return;
        }

        renderCustomer(response.data);
    };

    const initializePage = async (): Promise<void> => {
        const page = document.getElementById('customer-details-page') as HTMLElement | null;
        if (!page || page.dataset.initialized === 'true') return;
        page.dataset.initialized = 'true';

        const params = new URLSearchParams(window.location.search);
        currentCustomerId = Number(params.get('customerId') ?? '0');
        if (!Number.isFinite(currentCustomerId) || currentCustomerId <= 0) {
            showError('customerId query parameter is required.');
            document.getElementById('customer-details-loading')?.classList.add('d-none');
            return;
        }

        document.getElementById('customer-details-contact-create')?.addEventListener('click', openContactCreate);
        document.getElementById('customer-details-add-note')?.addEventListener('click', () => { void addInternalNote(); });
        document.getElementById('customer-details-customer-edit')?.addEventListener('click', () => {
            document.dispatchEvent(new CustomEvent('customers:open-edit', { detail: { id: currentCustomerId } }));
        });
        document.getElementById('customer-details-contact-save')?.addEventListener('click', () => { void saveContact(); });
        document.addEventListener('customers:saved', () => { void loadCustomer(); });
        document.getElementById('customer-details-contacts-body')?.addEventListener('click', (event) => {
            const target = event.target as HTMLElement;
            const button = target.closest<HTMLButtonElement>('button[data-action="edit-contact"]');
            if (!button) return;
            const id = Number(button.dataset.id ?? '0');
            if (!Number.isFinite(id) || id <= 0) return;
            openContactEdit(id);
        });

        await loadCustomer();
        await loadContacts();
        await loadDomainsWithServices();
        await loadInternalNotes();
        await loadChanges();

        document.getElementById('customer-details-loading')?.classList.add('d-none');
        document.getElementById('customer-details-content')?.classList.remove('d-none');
    };

    const setupObserver = (): void => {
        void initializePage();

        if (document.body) {
            const observer = new MutationObserver(() => {
                const page = document.getElementById('customer-details-page') as HTMLElement | null;
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

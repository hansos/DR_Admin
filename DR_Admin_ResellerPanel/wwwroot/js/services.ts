(() => {
    interface AppSettings {
        apiBaseUrl?: string;
    }

    interface ApiResponse<T> {
        success: boolean;
        data?: T;
        message?: string;
    }

    interface ServiceItem {
        id: number;
        name: string;
        description: string;
        serviceTypeId: number;
        billingCycleId: number | null;
        price: number | null;
    }

    interface ServiceType {
        id: number;
        name: string;
    }

    interface BillingCycle {
        id: number;
        name: string;
        code: string;
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

    let services: ServiceItem[] = [];
    let serviceTypes: ServiceType[] = [];
    let billingCycles: BillingCycle[] = [];
    let editingId: number | null = null;
    let pendingDeleteId: number | null = null;

    const getBootstrap = (): BootstrapNamespace | null => {
        const value = (window as Window & { bootstrap?: BootstrapNamespace }).bootstrap;
        return value ?? null;
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
            console.error('Services request failed', error);
            return {
                success: false,
                message: 'Network error. Please try again.',
            };
        }
    };

    const parseListData = (raw: unknown): unknown[] => {
        if (Array.isArray(raw)) {
            return raw;
        }

        const wrapped = raw as { data?: unknown[]; Data?: unknown[] } | null;
        if (Array.isArray(wrapped?.data)) {
            return wrapped.data;
        }

        if (Array.isArray(wrapped?.Data)) {
            return wrapped.Data;
        }

        return [];
    };

    const normalizeService = (item: unknown): ServiceItem => {
        const typed = (item ?? {}) as Record<string, unknown>;
        const rawPrice = Number(typed.price ?? typed.Price);

        return {
            id: Number(typed.id ?? typed.Id ?? 0),
            name: String(typed.name ?? typed.Name ?? ''),
            description: String(typed.description ?? typed.Description ?? ''),
            serviceTypeId: Number(typed.serviceTypeId ?? typed.ServiceTypeId ?? 0),
            billingCycleId: Number(typed.billingCycleId ?? typed.BillingCycleId ?? 0) || null,
            price: Number.isFinite(rawPrice) ? rawPrice : null,
        };
    };

    const normalizeServiceType = (item: unknown): ServiceType => {
        const typed = (item ?? {}) as Record<string, unknown>;
        return {
            id: Number(typed.id ?? typed.Id ?? 0),
            name: String(typed.name ?? typed.Name ?? ''),
        };
    };

    const normalizeBillingCycle = (item: unknown): BillingCycle => {
        const typed = (item ?? {}) as Record<string, unknown>;
        return {
            id: Number(typed.id ?? typed.Id ?? 0),
            name: String(typed.name ?? typed.Name ?? ''),
            code: String(typed.code ?? typed.Code ?? ''),
        };
    };

    const resolveServiceTypeName = (serviceTypeId: number): string => {
        const value = serviceTypes.find((item) => item.id === serviceTypeId);
        return value?.name || '-';
    };

    const resolveBillingCycleName = (billingCycleId: number | null): string => {
        if (!billingCycleId) {
            return '-';
        }

        const value = billingCycles.find((item) => item.id === billingCycleId);
        if (!value) {
            return `#${billingCycleId}`;
        }

        return value.code ? `${value.name} (${value.code})` : value.name;
    };

    const formatMoney = (value: number | null): string => {
        if (value === null) {
            return 'Price on request';
        }

        return value.toFixed(2);
    };

    const showSuccess = (message: string): void => {
        const alert = document.getElementById('services-alert-success');
        if (!alert) {
            return;
        }

        alert.textContent = message;
        alert.classList.remove('d-none');
        document.getElementById('services-alert-error')?.classList.add('d-none');
        setTimeout(() => alert.classList.add('d-none'), 4000);
    };

    const showError = (message: string): void => {
        const alert = document.getElementById('services-alert-error');
        if (!alert) {
            return;
        }

        alert.textContent = message;
        alert.classList.remove('d-none');
        document.getElementById('services-alert-success')?.classList.add('d-none');
    };

    const showModal = (id: string): void => {
        const bootstrap = getBootstrap();
        const modalElement = document.getElementById(id);
        if (!bootstrap || !modalElement) {
            return;
        }

        const modal = bootstrap.Modal.getInstance(modalElement) ?? new bootstrap.Modal(modalElement);
        modal.show();
    };

    const hideModal = (id: string): void => {
        const bootstrap = getBootstrap();
        const modalElement = document.getElementById(id);
        if (!bootstrap || !modalElement) {
            return;
        }

        const modal = bootstrap.Modal.getInstance(modalElement) ?? new bootstrap.Modal(modalElement);
        modal.hide();
    };

    const renderTable = (): void => {
        const tableBody = document.getElementById('services-table-body');
        if (!tableBody) {
            return;
        }

        if (!services.length) {
            tableBody.innerHTML = '<tr><td colspan="7" class="text-center text-muted">No services found. Click "New Service" to add one.</td></tr>';
            return;
        }

        tableBody.innerHTML = services.map((service) => `
            <tr>
                <td>${service.id}</td>
                <td>${esc(service.name)}</td>
                <td>${esc(resolveServiceTypeName(service.serviceTypeId))}</td>
                <td>${esc(resolveBillingCycleName(service.billingCycleId))}</td>
                <td>${esc(formatMoney(service.price))}</td>
                <td>${esc(service.description || '-')}</td>
                <td>
                    <div class="btn-group btn-group-sm">
                        <button class="btn btn-outline-primary" type="button" data-action="edit" data-id="${service.id}" title="Edit"><i class="bi bi-pencil"></i></button>
                        <button class="btn btn-outline-danger" type="button" data-action="delete" data-id="${service.id}" data-name="${esc(service.name)}" title="Delete"><i class="bi bi-trash"></i></button>
                    </div>
                </td>
            </tr>
        `).join('');
    };

    const renderServiceTypeOptions = (): void => {
        const select = document.getElementById('services-service-type') as HTMLSelectElement | null;
        if (!select) {
            return;
        }

        const options = serviceTypes
            .slice()
            .sort((a, b) => a.name.localeCompare(b.name))
            .map((item) => `<option value="${item.id}">${esc(item.name || `Type #${item.id}`)}</option>`)
            .join('');

        select.innerHTML = options || '<option value="">No service types available</option>';
    };

    const renderBillingCycleOptions = (): void => {
        const select = document.getElementById('services-billing-cycle') as HTMLSelectElement | null;
        if (!select) {
            return;
        }

        const options = billingCycles
            .slice()
            .sort((a, b) => a.name.localeCompare(b.name))
            .map((item) => `<option value="${item.id}">${esc(item.code ? `${item.name} (${item.code})` : item.name)}</option>`)
            .join('');

        select.innerHTML = `<option value="">None</option>${options}`;
    };

    const loadData = async (): Promise<void> => {
        const tableBody = document.getElementById('services-table-body');
        if (tableBody) {
            tableBody.innerHTML = '<tr><td colspan="7" class="text-center"><div class="spinner-border text-primary"></div></td></tr>';
        }

        const [serviceTypeResponse, billingCycleResponse, servicesResponse] = await Promise.all([
            apiRequest<unknown>(`${getApiBaseUrl()}/ServiceTypes`, { method: 'GET' }),
            apiRequest<unknown>(`${getApiBaseUrl()}/BillingCycles`, { method: 'GET' }),
            apiRequest<unknown>(`${getApiBaseUrl()}/Services`, { method: 'GET' }),
        ]);

        if (!serviceTypeResponse.success || !billingCycleResponse.success || !servicesResponse.success) {
            serviceTypes = [];
            billingCycles = [];
            services = [];
            renderTable();
            showError(serviceTypeResponse.message || billingCycleResponse.message || servicesResponse.message || 'Failed to load services data.');
            return;
        }

        serviceTypes = parseListData(serviceTypeResponse.data)
            .map((item) => normalizeServiceType(item))
            .filter((item) => item.id > 0);

        billingCycles = parseListData(billingCycleResponse.data)
            .map((item) => normalizeBillingCycle(item))
            .filter((item) => item.id > 0);

        services = parseListData(servicesResponse.data)
            .map((item) => normalizeService(item))
            .filter((item) => item.id > 0)
            .sort((a, b) => b.id - a.id);

        renderServiceTypeOptions();
        renderBillingCycleOptions();
        renderTable();
    };

    const clearForm = (): void => {
        const form = document.getElementById('services-form') as HTMLFormElement | null;
        form?.reset();

        const serviceTypeSelect = document.getElementById('services-service-type') as HTMLSelectElement | null;
        if (serviceTypeSelect && serviceTypeSelect.options.length > 0) {
            serviceTypeSelect.selectedIndex = 0;
        }

        const billingCycleSelect = document.getElementById('services-billing-cycle') as HTMLSelectElement | null;
        if (billingCycleSelect) {
            billingCycleSelect.value = '';
        }
    };

    const openCreate = (): void => {
        editingId = null;
        const title = document.getElementById('services-modal-title');
        if (title) {
            title.textContent = 'New Service';
        }

        clearForm();
        showModal('services-edit-modal');
    };

    const openEdit = (id: number): void => {
        const service = services.find((item) => item.id === id);
        if (!service) {
            return;
        }

        editingId = id;

        const title = document.getElementById('services-modal-title');
        if (title) {
            title.textContent = 'Edit Service';
        }

        const nameInput = document.getElementById('services-name') as HTMLInputElement | null;
        const descriptionInput = document.getElementById('services-description') as HTMLTextAreaElement | null;
        const serviceTypeSelect = document.getElementById('services-service-type') as HTMLSelectElement | null;
        const billingCycleSelect = document.getElementById('services-billing-cycle') as HTMLSelectElement | null;
        const priceInput = document.getElementById('services-price') as HTMLInputElement | null;

        if (nameInput) {
            nameInput.value = service.name;
        }

        if (descriptionInput) {
            descriptionInput.value = service.description || '';
        }

        if (serviceTypeSelect) {
            serviceTypeSelect.value = String(service.serviceTypeId);
        }

        if (billingCycleSelect) {
            billingCycleSelect.value = service.billingCycleId ? String(service.billingCycleId) : '';
        }

        if (priceInput) {
            priceInput.value = service.price === null ? '' : service.price.toFixed(2);
        }

        showModal('services-edit-modal');
    };

    const parsePrice = (value: string): number | null => {
        const trimmed = value.trim();
        if (!trimmed) {
            return null;
        }

        const parsed = Number(trimmed);
        return Number.isFinite(parsed) && parsed >= 0 ? parsed : null;
    };

    const saveService = async (): Promise<void> => {
        const nameInput = document.getElementById('services-name') as HTMLInputElement | null;
        const descriptionInput = document.getElementById('services-description') as HTMLTextAreaElement | null;
        const serviceTypeSelect = document.getElementById('services-service-type') as HTMLSelectElement | null;
        const billingCycleSelect = document.getElementById('services-billing-cycle') as HTMLSelectElement | null;
        const priceInput = document.getElementById('services-price') as HTMLInputElement | null;

        const name = (nameInput?.value ?? '').trim();
        const serviceTypeId = Number(serviceTypeSelect?.value ?? '0');
        const billingCycleId = Number(billingCycleSelect?.value ?? '0') || null;
        const price = parsePrice(priceInput?.value ?? '');

        if (!name) {
            showError('Name is required.');
            return;
        }

        if (!Number.isFinite(serviceTypeId) || serviceTypeId <= 0) {
            showError('Service type is required.');
            return;
        }

        if ((priceInput?.value ?? '').trim().length > 0 && price === null) {
            showError('Price must be a valid non-negative number.');
            return;
        }

        const payload = {
            name,
            description: (descriptionInput?.value ?? '').trim(),
            serviceTypeId,
            billingCycleId,
            price,
            resellerCompanyId: null,
            salesAgentId: null,
        };

        const response = editingId
            ? await apiRequest(`${getApiBaseUrl()}/Services/${editingId}`, { method: 'PUT', body: JSON.stringify(payload) })
            : await apiRequest(`${getApiBaseUrl()}/Services`, { method: 'POST', body: JSON.stringify(payload) });

        if (!response.success) {
            showError(response.message || 'Failed to save service.');
            return;
        }

        hideModal('services-edit-modal');
        showSuccess(editingId ? 'Service updated successfully.' : 'Service created successfully.');
        await loadData();
    };

    const openDelete = (id: number, name: string): void => {
        pendingDeleteId = id;

        const target = document.getElementById('services-delete-name');
        if (target) {
            target.textContent = name || `#${id}`;
        }

        showModal('services-delete-modal');
    };

    const doDelete = async (): Promise<void> => {
        if (!pendingDeleteId) {
            return;
        }

        const response = await apiRequest(`${getApiBaseUrl()}/Services/${pendingDeleteId}`, { method: 'DELETE' });
        hideModal('services-delete-modal');

        if (!response.success) {
            showError(response.message || 'Failed to delete service.');
            return;
        }

        pendingDeleteId = null;
        showSuccess('Service deleted successfully.');
        await loadData();
    };

    const bindEvents = (): void => {
        document.getElementById('services-create')?.addEventListener('click', openCreate);
        document.getElementById('services-save')?.addEventListener('click', () => {
            void saveService();
        });
        document.getElementById('services-confirm-delete')?.addEventListener('click', () => {
            void doDelete();
        });

        document.getElementById('services-table-body')?.addEventListener('click', (event) => {
            const target = event.target as HTMLElement;
            const button = target.closest('button[data-action]') as HTMLButtonElement | null;
            if (!button) {
                return;
            }

            const id = Number(button.dataset.id ?? '0');
            if (!id) {
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

    const initializePage = (): void => {
        const page = document.getElementById('services-page') as HTMLElement | null;
        if (!page || page.dataset.initialized === 'true') {
            return;
        }

        page.dataset.initialized = 'true';
        bindEvents();
        void loadData();
    };

    const setupObserver = (): void => {
        initializePage();

        if (document.body) {
            const observer = new MutationObserver(() => {
                const page = document.getElementById('services-page') as HTMLElement | null;
                if (page && page.dataset.initialized !== 'true') {
                    initializePage();
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

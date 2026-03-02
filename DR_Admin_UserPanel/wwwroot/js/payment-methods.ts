((): void => {
interface UserAccountDto {
    customer?: {
        id: number;
    } | null;
}

interface ActivePaymentGatewayDto {
    paymentInstrument?: string;
}

interface CustomerPaymentMethodDto {
    id: number;
    paymentGatewayId: number;
    paymentGatewayName: string;
    type: string | number;
    cardBrand: string;
    last4Digits: string;
    expiryMonth?: number | null;
    expiryYear?: number | null;
    isDefault: boolean;
    isActive: boolean;
    isVerified: boolean;
    createdAt: string;
}

interface PaymentInstrumentDto {
    id: number;
    code: string;
    name: string;
    isActive: boolean;
    displayOrder: number;
}

interface ModalInstance {
    show: () => void;
    hide: () => void;
}

interface BootstrapModalStatic {
    new(element: Element): ModalInstance;
    getOrCreateInstance: (element: Element) => ModalInstance;
}

interface PaymentMethodsWindow extends Window {
    UserPanelApi?: {
        request: <T>(path: string, options?: RequestInit, requiresAuth?: boolean) => Promise<{ success: boolean; data?: T; message?: string }>;
    };
    UserPanelAlerts?: {
        showSuccess: (id: string, message: string) => void;
        showError: (id: string, message: string) => void;
        hide: (id: string) => void;
    };
    bootstrap?: {
        Modal: BootstrapModalStatic;
    };
}

let paymentMethodsCustomerId: number | null = null;
let paymentMethodsCache: CustomerPaymentMethodDto[] = [];
let instrumentAlternatives: string[] = [];
const fallbackInstruments: string[] = ['CreditCard', 'BankAccount', 'PayPal', 'Cash', 'Other'];
const enabledGatewayInstruments = new Set<string>();

function initializePaymentMethodsPage(): void {
    const page = document.getElementById('payment-methods-page');
    if (!page || page.dataset.initialized === 'true') {
        return;
    }

    page.dataset.initialized = 'true';

    document.getElementById('payment-methods-open-create')?.addEventListener('click', () => {
        openPaymentMethodModal();
    });

    document.getElementById('payment-methods-create-form')?.addEventListener('submit', (event) => {
        event.preventDefault();
        void savePaymentMethod();
    });

    void initializePaymentMethodData();
}

async function initializePaymentMethodData(): Promise<void> {
    await loadEnabledGatewayInstruments();
    await loadGatewayAlternatives();
    await loadPaymentMethods();
}

async function loadEnabledGatewayInstruments(): Promise<void> {
    enabledGatewayInstruments.clear();

    const typedWindow = window as PaymentMethodsWindow;
    const response = await typedWindow.UserPanelApi?.request<ActivePaymentGatewayDto[]>('/PaymentGateways/active', { method: 'GET' }, true);

    if (!response || !response.success || !response.data) {
        return;
    }

    response.data.forEach((gateway) => {
        const instrument = gateway.paymentInstrument?.trim();
        if (instrument) {
            enabledGatewayInstruments.add(normalizeInstrumentKey(instrument));
        }
    });
}

async function loadPaymentMethods(): Promise<void> {
    const typedWindow = window as PaymentMethodsWindow;
    typedWindow.UserPanelAlerts?.hide('payment-methods-alert-success');
    typedWindow.UserPanelAlerts?.hide('payment-methods-alert-error');

    const customerId = await getPaymentMethodsCustomerId();
    if (!customerId) {
        typedWindow.UserPanelAlerts?.showError('payment-methods-alert-error', 'Could not resolve customer for payment methods.');
        renderPaymentMethods([]);
        return;
    }

    const response = await typedWindow.UserPanelApi?.request<CustomerPaymentMethodDto[]>(`/CustomerPaymentMethods/customer/${customerId}`, { method: 'GET' }, true);

    if (!response || !response.success || !response.data) {
        typedWindow.UserPanelAlerts?.showError('payment-methods-alert-error', response?.message ?? 'Could not load payment methods.');
        renderPaymentMethods([]);
        return;
    }

    paymentMethodsCache = response.data;
    renderPaymentMethods(paymentMethodsCache);
}

async function loadGatewayAlternatives(): Promise<void> {
    const typedWindow = window as PaymentMethodsWindow;
    const response = await typedWindow.UserPanelApi?.request<PaymentInstrumentDto[]>('/PaymentInstruments/active', { method: 'GET' }, true);

    if (!response || !response.success || !response.data || response.data.length === 0) {
        instrumentAlternatives = [...fallbackInstruments];
        renderGatewayAlternatives();
        return;
    }

    const ordered = response.data
        .filter((item) => item.isActive)
        .sort((a, b) => (a.displayOrder - b.displayOrder) || a.name.localeCompare(b.name));

    const uniqueInstruments: string[] = [];
    ordered.forEach((gateway) => {
        const instrument = gateway.code?.trim();
        if (!instrument) {
            return;
        }

        const exists = uniqueInstruments.some((value) => value.toLowerCase() === instrument.toLowerCase());
        if (!exists) {
            uniqueInstruments.push(instrument);
        }
    });

    instrumentAlternatives = uniqueInstruments.length > 0 ? uniqueInstruments : [...fallbackInstruments];

    renderGatewayAlternatives();
}

function renderGatewayAlternatives(): void {
    const instrumentSelect = document.getElementById('payment-methods-instrument') as HTMLSelectElement | null;
    if (!instrumentSelect) {
        return;
    }

    if (instrumentAlternatives.length === 0) {
        instrumentSelect.innerHTML = '<option value="">No alternatives available</option>';
        return;
    }

    instrumentSelect.innerHTML = instrumentAlternatives
        .map((instrument) => `<option value="${escapePaymentMethodsText(instrument)}">${escapePaymentMethodsText(instrument)}</option>`)
        .join('');
}

async function getPaymentMethodsCustomerId(): Promise<number | null> {
    if (paymentMethodsCustomerId && paymentMethodsCustomerId > 0) {
        return paymentMethodsCustomerId;
    }

    const typedWindow = window as PaymentMethodsWindow;
    const response = await typedWindow.UserPanelApi?.request<UserAccountDto>('/MyAccount/me', { method: 'GET' }, true);
    const customerId = response?.data?.customer?.id;

    if (!response || !response.success || !customerId || customerId <= 0) {
        return null;
    }

    paymentMethodsCustomerId = customerId;
    return customerId;
}

function renderPaymentMethods(items: CustomerPaymentMethodDto[]): void {
    const tableBody = document.getElementById('payment-methods-table-body');
    if (!tableBody) {
        return;
    }

    if (items.length === 0) {
        tableBody.innerHTML = '<tr><td colspan="4" class="text-center text-muted">No payment methods found.</td></tr>';
        return;
    }

    tableBody.innerHTML = items
        .sort((a, b) => Date.parse(b.createdAt) - Date.parse(a.createdAt))
        .map((item) => {
            const methodText = resolveInstrumentFromPaymentMethod(item) ?? String(item.type);

            const status = [
                item.isDefault ? '<span class="badge bg-primary me-1">Default</span>' : '',
                item.isActive ? '<span class="badge bg-secondary">Active</span>' : '<span class="badge bg-danger">Inactive</span>'
            ].join(' ');

            return `<tr>
                <td>${escapePaymentMethodsText(methodText)}</td>
                <td>${status}</td>
                <td>${formatPaymentMethodsDate(item.createdAt)}</td>
                <td>
                    <div class="btn-group btn-group-sm" role="group">
                        <button class="btn btn-outline-secondary" type="button" data-action="edit" data-id="${item.id}">Change</button>
                        <button class="btn btn-outline-primary" type="button" data-action="default" data-id="${item.id}">Set default</button>
                        <button class="btn btn-outline-danger" type="button" data-action="delete" data-id="${item.id}">Delete</button>
                    </div>
                </td>
            </tr>`;
        })
        .join('');

    tableBody.querySelectorAll('button[data-action="edit"]').forEach((button) => {
        button.addEventListener('click', () => {
            const id = Number.parseInt((button as HTMLButtonElement).dataset.id ?? '0', 10);
            if (id > 0) {
                const item = paymentMethodsCache.find((method) => method.id === id);
                if (item) {
                    openPaymentMethodModal(item);
                }
            }
        });
    });

    tableBody.querySelectorAll('button[data-action="default"]').forEach((button) => {
        button.addEventListener('click', () => {
            const id = Number.parseInt((button as HTMLButtonElement).dataset.id ?? '0', 10);
            if (id > 0) {
                void setDefaultPaymentMethod(id);
            }
        });
    });

    tableBody.querySelectorAll('button[data-action="delete"]').forEach((button) => {
        button.addEventListener('click', () => {
            const id = Number.parseInt((button as HTMLButtonElement).dataset.id ?? '0', 10);
            if (id > 0) {
                void deletePaymentMethod(id);
            }
        });
    });
}

function openPaymentMethodModal(item?: CustomerPaymentMethodDto): void {
    const modalElement = document.getElementById('payment-methods-modal');
    if (!modalElement) {
        return;
    }

    const modalTitle = document.getElementById('payment-methods-modal-title');
    const submitButton = document.getElementById('payment-methods-create') as HTMLButtonElement | null;
    const editIdInput = document.getElementById('payment-methods-edit-id') as HTMLInputElement | null;
    const isDefault = document.getElementById('payment-methods-default') as HTMLInputElement | null;

    if (editIdInput) {
        editIdInput.value = item ? String(item.id) : '';
    }

    if (modalTitle) {
        modalTitle.textContent = item ? 'Change payment instrument' : 'Add payment instrument';
    }

    if (submitButton) {
        submitButton.textContent = item ? 'Save changes' : 'Add instrument';
    }

    const instrumentSelect = document.getElementById('payment-methods-instrument') as HTMLSelectElement | null;
    if (instrumentSelect) {
        const instrument = resolveInstrumentFromPaymentMethod(item) ?? instrumentAlternatives[0] ?? '';
        instrumentSelect.value = instrument;
    }

    if (isDefault) {
        isDefault.checked = item?.isDefault ?? false;
    }

    getPaymentMethodModalInstance(modalElement)?.show();
}

function getPaymentMethodModalInstance(modalElement: Element): ModalInstance | null {
    const typedWindow = window as PaymentMethodsWindow;
    const modalApi = typedWindow.bootstrap?.Modal;
    if (!modalApi) {
        return null;
    }

    return modalApi.getOrCreateInstance(modalElement);
}

async function savePaymentMethod(): Promise<void> {
    const typedWindow = window as PaymentMethodsWindow;
    typedWindow.UserPanelAlerts?.hide('payment-methods-alert-success');
    typedWindow.UserPanelAlerts?.hide('payment-methods-alert-error');

    const customerId = await getPaymentMethodsCustomerId();
    if (!customerId) {
        typedWindow.UserPanelAlerts?.showError('payment-methods-alert-error', 'Could not resolve customer for payment method creation.');
        return;
    }

    const editId = Number.parseInt(readPaymentMethodsInputValue('payment-methods-edit-id'), 10);
    const instrument = readPaymentMethodsInputValue('payment-methods-instrument');
    const isDefault = (document.getElementById('payment-methods-default') as HTMLInputElement | null)?.checked ?? false;

    if (!instrument) {
        typedWindow.UserPanelAlerts?.showError('payment-methods-alert-error', 'Payment instrument is required.');
        return;
    }

    const normalizedInstrument = normalizeInstrumentKey(instrument);
    if (enabledGatewayInstruments.size > 0 && !enabledGatewayInstruments.has(normalizedInstrument)) {
        typedWindow.UserPanelAlerts?.showError('payment-methods-alert-error', 'Selected payment instrument is not available yet. Please choose another one or contact support.');
        return;
    }

    const type = mapInstrumentToType(instrument);

    const isEdit = !Number.isNaN(editId) && editId > 0;
    const response = await typedWindow.UserPanelApi?.request(
        isEdit ? `/CustomerPaymentMethods/${editId}?customerId=${customerId}` : '/CustomerPaymentMethods',
        {
            method: isEdit ? 'PUT' : 'POST',
            body: JSON.stringify(
                isEdit
                    ? {
                        paymentGatewayId: 0,
                        paymentInstrument: instrument,
                        type,
                        isDefault
                    }
                    : {
                        customerId,
                        paymentGatewayId: 0,
                        paymentInstrument: instrument,
                        type,
                        paymentMethodToken: '',
                        isDefault
                    }
            )
        },
        true
    );

    if (!response || !response.success) {
        typedWindow.UserPanelAlerts?.showError('payment-methods-alert-error', response?.message ?? 'Could not save payment method.');
        return;
    }

    const modalElement = document.getElementById('payment-methods-modal');
    if (modalElement) {
        getPaymentMethodModalInstance(modalElement)?.hide();
    }

    clearPaymentMethodsForm();
    typedWindow.UserPanelAlerts?.showSuccess('payment-methods-alert-success', response.message ?? (isEdit ? 'Payment method updated.' : 'Payment method added.'));
    await loadPaymentMethods();
}

async function setDefaultPaymentMethod(paymentMethodId: number): Promise<void> {
    const typedWindow = window as PaymentMethodsWindow;
    const customerId = await getPaymentMethodsCustomerId();

    if (!customerId) {
        typedWindow.UserPanelAlerts?.showError('payment-methods-alert-error', 'Could not resolve customer for default update.');
        return;
    }

    const response = await typedWindow.UserPanelApi?.request(`/CustomerPaymentMethods/${paymentMethodId}/set-default?customerId=${customerId}`, {
        method: 'POST'
    }, true);

    if (!response || !response.success) {
        typedWindow.UserPanelAlerts?.showError('payment-methods-alert-error', response?.message ?? 'Could not update default payment method.');
        return;
    }

    typedWindow.UserPanelAlerts?.showSuccess('payment-methods-alert-success', response.message ?? 'Default payment method updated.');
    await loadPaymentMethods();
}

async function deletePaymentMethod(paymentMethodId: number): Promise<void> {
    const typedWindow = window as PaymentMethodsWindow;
    const customerId = await getPaymentMethodsCustomerId();

    if (!customerId) {
        typedWindow.UserPanelAlerts?.showError('payment-methods-alert-error', 'Could not resolve customer for deletion.');
        return;
    }

    const response = await typedWindow.UserPanelApi?.request(`/CustomerPaymentMethods/${paymentMethodId}?customerId=${customerId}`, {
        method: 'DELETE'
    }, true);

    if (!response || !response.success) {
        typedWindow.UserPanelAlerts?.showError('payment-methods-alert-error', response?.message ?? 'Could not delete payment method.');
        return;
    }

    typedWindow.UserPanelAlerts?.showSuccess('payment-methods-alert-success', 'Payment method deleted.');
    await loadPaymentMethods();
}

function clearPaymentMethodsForm(): void {
    const editIdInput = document.getElementById('payment-methods-edit-id') as HTMLInputElement | null;
    const isDefault = document.getElementById('payment-methods-default') as HTMLInputElement | null;

    if (editIdInput) {
        editIdInput.value = '';
    }

    const instrumentSelect = document.getElementById('payment-methods-instrument') as HTMLSelectElement | null;
    if (instrumentSelect && instrumentAlternatives.length > 0) {
        instrumentSelect.value = instrumentAlternatives[0];
    }

    if (isDefault) {
        isDefault.checked = false;
    }
}

function readPaymentMethodsInputValue(id: string): string {
    const input = document.getElementById(id) as HTMLInputElement | HTMLSelectElement | null;
    return input?.value.trim() ?? '';
}

function formatPaymentMethodsDate(value: string): string {
    const date = new Date(value);
    if (Number.isNaN(date.getTime())) {
        return '-';
    }

    return `${date.toLocaleDateString()} ${date.toLocaleTimeString()}`;
}

function escapePaymentMethodsText(value: string): string {
    return value
        .replace(/&/g, '&amp;')
        .replace(/</g, '&lt;')
        .replace(/>/g, '&gt;')
        .replace(/"/g, '&quot;')
        .replace(/'/g, '&#039;');
}

function normalizeInstrumentKey(value: string): string {
    return value
        .trim()
        .toLowerCase()
        .replace(/\s+/g, '')
        .replace(/-/g, '')
        .replace(/_/g, '');
}

function mapInstrumentToType(instrument: string): number {
    const normalized = instrument.trim().toLowerCase();
    if (normalized === 'creditcard' || normalized === 'credit card' || normalized === 'card') {
        return 0;
    }

    if (normalized === 'bankaccount' || normalized === 'bank account') {
        return 1;
    }

    if (normalized === 'paypal') {
        return 2;
    }

    if (normalized === 'cash') {
        return 3;
    }

    return 99;
}

function resolveInstrumentFromPaymentMethod(item?: CustomerPaymentMethodDto): string | null {
    if (!item) {
        return null;
    }

    if (item.type === 0 || item.type === '0') {
        return 'CreditCard';
    }

    if (item.type === 1 || item.type === '1') {
        return 'BankAccount';
    }

    if (item.type === 2 || item.type === '2') {
        return 'PayPal';
    }

    if (item.type === 3 || item.type === '3') {
        return 'Cash';
    }

    return 'Other';
}

function setupPaymentMethodsObserver(): void {
    initializePaymentMethodsPage();

    const observer = new MutationObserver(() => {
        initializePaymentMethodsPage();
    });

    observer.observe(document.body, { childList: true, subtree: true });
}

if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', setupPaymentMethodsObserver);
} else {
    setupPaymentMethodsObserver();
}
})();

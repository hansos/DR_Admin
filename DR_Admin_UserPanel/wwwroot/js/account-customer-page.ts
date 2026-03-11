((): void => {
interface CustomerPageModel {
    id: number;
    referenceNumber: number;
    customerNumber?: number | null;
    name: string;
    email: string;
    phone: string;
    address: string;
    customerName?: string | null;
    taxId?: string | null;
    vatNumber?: string | null;
    isCompany: boolean;
    isSelfRegistered: boolean;
    isActive: boolean;
    status: string;
    balance: number;
    creditLimit: number;
    notes?: string | null;
    billingEmail?: string | null;
    preferredPaymentMethod?: string | null;
    preferredCurrency: string;
    allowCurrencyOverride: boolean;
}

interface PatchCustomerInfoRequestDto {
    name: string;
    email: string;
    phone: string;
    address: string;
    customerName?: string | null;
    taxId?: string | null;
    vatNumber?: string | null;
    isCompany: boolean;
    isSelfRegistered: boolean;
    isActive: boolean;
    status: string;
    balance: number;
    creditLimit: number;
    notes?: string | null;
    billingEmail?: string | null;
    preferredPaymentMethod?: string | null;
    preferredCurrency: string;
    allowCurrencyOverride: boolean;
}

interface UserAccountDto {
    customer?: CustomerPageModel | null;
}

interface CustomerPageWindow extends Window {
    UserPanelApi?: {
        request: <T>(path: string, options?: RequestInit, requiresAuth?: boolean) => Promise<{ success: boolean; data?: T; message?: string }>;
    };
    UserPanelAlerts?: {
        showSuccess: (id: string, message: string) => void;
        showError: (id: string, message: string) => void;
        hide: (id: string) => void;
    };
}

function initializeCustomerPage(): void {
    const page = document.getElementById('account-customer-page');
    if (!page || page.dataset.bound === 'true') {
        return;
    }

    page.dataset.bound = 'true';

    const form = document.getElementById('account-customer-form') as HTMLFormElement | null;
    if (!form) {
        return;
    }

    const editButton = document.getElementById('account-customer-edit') as HTMLButtonElement | null;
    editButton?.addEventListener('click', () => {
        setFormReadOnly(false);
    });

    void hydrateForm();

    form.addEventListener('submit', async (event: Event) => {
        event.preventDefault();
        await saveCustomerPage();
    });
}

async function hydrateForm(): Promise<void> {
    const typedWindow = window as CustomerPageWindow;
    typedWindow.UserPanelAlerts?.hide('account-customer-alert-success');
    typedWindow.UserPanelAlerts?.hide('account-customer-alert-error');

    const response = await typedWindow.UserPanelApi?.request<UserAccountDto>('/MyAccount/me', { method: 'GET' }, true);

    if (!response || !response.success) {
        typedWindow.UserPanelAlerts?.showError('account-customer-alert-error', response?.message ?? 'Could not load company record.');
        return;
    }

    const customer = response.data?.customer ?? null;
    const referenceElement = document.getElementById('account-customer-reference');
    if (referenceElement) {
        const referenceNumber = customer?.referenceNumber ? String(customer.referenceNumber) : '-';
        const customerNumber = customer?.customerNumber ? String(customer.customerNumber) : '-';
        referenceElement.textContent = `Reference: ${referenceNumber} · Customer number: ${customerNumber}`;
    }

    setInputValue('account-customer-company', customer?.name ?? '');
    setInputValue('account-customer-name', customer?.customerName ?? '');
    setInputValue('account-customer-contact-email', customer?.email ?? '');
    setInputValue('account-customer-billing-email', customer?.billingEmail ?? '');
    setInputValue('account-customer-phone', customer?.phone ?? '');
    setInputValue('account-customer-tax-id', customer?.taxId ?? '');
    setInputValue('account-customer-vat-number', customer?.vatNumber ?? '');
    setInputValue('account-customer-status', customer?.status ?? 'Active');
    setInputValue('account-customer-address', customer?.address ?? '');
    setInputValue('account-customer-balance', toNumberString(customer?.balance));
    setInputValue('account-customer-credit-limit', toNumberString(customer?.creditLimit));
    setInputValue('account-customer-preferred-currency', customer?.preferredCurrency ?? 'EUR');
    setInputValue('account-customer-preferred-payment-method', customer?.preferredPaymentMethod ?? '');
    setTextAreaValue('account-customer-notes', customer?.notes ?? '');
    setCheckboxValue('account-customer-is-company', customer?.isCompany ?? false);
    setCheckboxValue('account-customer-is-self-registered', customer?.isSelfRegistered ?? false);
    setCheckboxValue('account-customer-is-active', customer?.isActive ?? true);
    setCheckboxValue('account-customer-allow-currency-override', customer?.allowCurrencyOverride ?? true);
    setFormReadOnly(true);
}

async function saveCustomerPage(): Promise<void> {
    const typedWindow = window as CustomerPageWindow;
    typedWindow.UserPanelAlerts?.hide('account-customer-alert-success');
    typedWindow.UserPanelAlerts?.hide('account-customer-alert-error');

    const name = getInputValue('account-customer-company').trim();
    const email = getInputValue('account-customer-contact-email').trim();
    const phone = getInputValue('account-customer-phone').trim();
    const address = getInputValue('account-customer-address').trim();

    if (!name || !email) {
        typedWindow.UserPanelAlerts?.showError('account-customer-alert-error', 'Account name and contact email are required.');
        return;
    }

    const payload: PatchCustomerInfoRequestDto = {
        name,
        email,
        phone,
        address,
        customerName: getInputValue('account-customer-name').trim(),
        taxId: toNullable(getInputValue('account-customer-tax-id')),
        vatNumber: toNullable(getInputValue('account-customer-vat-number')),
        isCompany: getCheckboxValue('account-customer-is-company'),
        isSelfRegistered: getCheckboxValue('account-customer-is-self-registered'),
        isActive: getCheckboxValue('account-customer-is-active'),
        status: getInputValue('account-customer-status').trim() || 'Active',
        balance: getNumberValue('account-customer-balance'),
        creditLimit: getNumberValue('account-customer-credit-limit'),
        notes: toNullable(getTextAreaValue('account-customer-notes')),
        billingEmail: toNullable(getInputValue('account-customer-billing-email')),
        preferredPaymentMethod: toNullable(getInputValue('account-customer-preferred-payment-method')),
        preferredCurrency: getInputValue('account-customer-preferred-currency').trim() || 'EUR',
        allowCurrencyOverride: getCheckboxValue('account-customer-allow-currency-override')
    };

    setSavingState(true);

    const response = await typedWindow.UserPanelApi?.request<CustomerPageModel>('/MyAccount/customer', {
        method: 'PATCH',
        body: JSON.stringify(payload)
    }, true);

    setSavingState(false);

    if (!response || !response.success || !response.data) {
        typedWindow.UserPanelAlerts?.showError('account-customer-alert-error', response?.message ?? 'Could not update company record.');
        return;
    }

    setInputValue('account-customer-company', response.data.name ?? name);
    setInputValue('account-customer-name', response.data.customerName ?? payload.customerName ?? '');
    setInputValue('account-customer-contact-email', response.data.email ?? email);
    setInputValue('account-customer-billing-email', response.data.billingEmail ?? payload.billingEmail ?? '');
    setInputValue('account-customer-phone', response.data.phone ?? phone);
    setInputValue('account-customer-tax-id', response.data.taxId ?? payload.taxId ?? '');
    setInputValue('account-customer-vat-number', response.data.vatNumber ?? payload.vatNumber ?? '');
    setInputValue('account-customer-status', response.data.status ?? payload.status);
    setInputValue('account-customer-address', response.data.address ?? address);
    setInputValue('account-customer-balance', toNumberString(response.data.balance));
    setInputValue('account-customer-credit-limit', toNumberString(response.data.creditLimit));
    setInputValue('account-customer-preferred-currency', response.data.preferredCurrency ?? payload.preferredCurrency);
    setInputValue('account-customer-preferred-payment-method', response.data.preferredPaymentMethod ?? payload.preferredPaymentMethod ?? '');
    setTextAreaValue('account-customer-notes', response.data.notes ?? payload.notes ?? '');
    setCheckboxValue('account-customer-is-company', response.data.isCompany);
    setCheckboxValue('account-customer-is-self-registered', response.data.isSelfRegistered);
    setCheckboxValue('account-customer-is-active', response.data.isActive);
    setCheckboxValue('account-customer-allow-currency-override', response.data.allowCurrencyOverride);
    setFormReadOnly(true);

    typedWindow.UserPanelAlerts?.showSuccess('account-customer-alert-success', 'Company record updated.');
}

function setTextAreaValue(id: string, value: string): void {
    const input = document.getElementById(id) as HTMLTextAreaElement | null;
    if (input) {
        input.value = value;
    }
}

function setCheckboxValue(id: string, value: boolean): void {
    const input = document.getElementById(id) as HTMLInputElement | null;
    if (input) {
        input.checked = value;
    }
}

function getTextAreaValue(id: string): string {
    const input = document.getElementById(id) as HTMLTextAreaElement | null;
    return input?.value ?? '';
}

function getCheckboxValue(id: string): boolean {
    const input = document.getElementById(id) as HTMLInputElement | null;
    return input?.checked ?? false;
}

function getNumberValue(id: string): number {
    const raw = getInputValue(id).trim();
    if (!raw) {
        return 0;
    }

    const parsed = Number(raw);
    return Number.isFinite(parsed) ? parsed : 0;
}

function toNullable(value: string): string | null {
    const trimmed = value.trim();
    return trimmed ? trimmed : null;
}

function toNumberString(value: number | null | undefined): string {
    if (typeof value !== 'number' || Number.isNaN(value)) {
        return '0';
    }

    return String(value);
}

function getInputValue(id: string): string {
    const input = document.getElementById(id) as HTMLInputElement | HTMLSelectElement | null;
    return input?.value ?? '';
}

function setInputValue(id: string, value: string): void {
    const input = document.getElementById(id) as HTMLInputElement | HTMLSelectElement | null;
    if (input) {
        if (input instanceof HTMLSelectElement) {
            ensureSelectOption(input, value);
        }

        input.value = value;
    }
}

function ensureSelectOption(select: HTMLSelectElement, value: string): void {
    if (!value) {
        return;
    }

    const exists = Array.from(select.options).some((option) => option.value === value);
    if (!exists) {
        const option = document.createElement('option');
        option.value = value;
        option.text = value;
        select.add(option);
    }
}

function setFormReadOnly(isReadOnly: boolean): void {
    const form = document.getElementById('account-customer-form');
    if (!form) {
        return;
    }

    const editableFields = form.querySelectorAll('[data-editable="true"]');
    editableFields.forEach((field) => {
        const element = field as HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement;
        element.disabled = isReadOnly;
    });

    const editButton = document.getElementById('account-customer-edit') as HTMLButtonElement | null;
    const saveButton = document.getElementById('account-customer-save') as HTMLButtonElement | null;

    if (editButton) {
        editButton.classList.toggle('d-none', !isReadOnly);
    }

    if (saveButton) {
        saveButton.classList.toggle('d-none', isReadOnly);
    }
}

function setSavingState(isSaving: boolean): void {
    const button = document.getElementById('account-customer-save') as HTMLButtonElement | null;
    if (!button) {
        return;
    }

    button.disabled = isSaving;
    button.textContent = isSaving ? 'Saving...' : 'Save company details';
}

function setupCustomerPageObserver(): void {
    initializeCustomerPage();

    const observer = new MutationObserver(() => {
        initializeCustomerPage();
    });

    observer.observe(document.body, { childList: true, subtree: true });
}

if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', setupCustomerPageObserver);
} else {
    setupCustomerPageObserver();
}
})();

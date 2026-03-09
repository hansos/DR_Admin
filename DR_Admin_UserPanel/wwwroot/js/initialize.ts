interface InitializeBuildModeResponse {
    isDebug?: boolean;
    IsDebug?: boolean;
    mode?: string;
    Mode?: string;
}

interface InitializeCustomerRequestDto {
    username: string;
    email: string;
    password: string;
    confirmPassword: string;
    companyName: string;
    companyPhone: string;
    contactFirstName: string;
    contactLastName: string;
}

interface InitializeCustomerResponseDto {
    success: boolean;
    userId: number;
    username: string;
    email: string;
    companyName: string;
    message?: string;
}

interface InitializeImportResponse {
    success?: boolean;
    Success?: boolean;
    errorMessage?: string;
    ErrorMessage?: string;
}

interface InitializeWindow extends Window {
    UserPanelApi?: {
        request: <T>(path: string, options?: RequestInit, requiresAuth?: boolean) => Promise<{ success: boolean; data?: T; message?: string }> ;
    };
    UserPanelAlerts?: {
        showSuccess: (id: string, message: string) => void;
        showError: (id: string, message: string) => void;
        hide: (id: string) => void;
    };
    Blazor?: {
        addEventListener?: (eventName: string, callback: () => void) => void;
    };
}

function initializeShopInitializePage(): void {
    const page = document.getElementById('shop-initialize-page');
    if (!page || page.dataset.bound === 'true') {
        return;
    }

    page.dataset.bound = 'true';

    const button = document.getElementById('shop-initialize-import-button') as HTMLButtonElement | null;
    button?.addEventListener('click', async () => {
        await importShopDebugSnapshot();
    });

    const userForm = document.getElementById('shop-initialize-user-form') as HTMLFormElement | null;
    userForm?.addEventListener('submit', async (event: Event) => {
        event.preventDefault();
        await createShopUser();
    });

    void setupShopInitializeBuildMode();
}

async function createShopUser(): Promise<void> {
    const typedWindow = window as InitializeWindow;
    typedWindow.UserPanelAlerts?.hide('shop-initialize-alert-success');
    typedWindow.UserPanelAlerts?.hide('shop-initialize-alert-error');

    const request = buildShopInitializeRegisterRequest();
    if (!request) {
        typedWindow.UserPanelAlerts?.showError('shop-initialize-alert-error', 'Please fill in all required user fields.');
        return;
    }

    const button = document.getElementById('shop-initialize-user-button') as HTMLButtonElement | null;
    if (button) {
        button.disabled = true;
        button.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>Creating...';
    }

    const response = await typedWindow.UserPanelApi?.request<InitializeCustomerResponseDto>('/Initialization/initialize-customer', {
        method: 'POST',
        body: JSON.stringify(request)
    }, false);

    if (button) {
        button.disabled = false;
        button.innerHTML = '<i class="bi bi-person-check"></i> Create User';
    }

    if (!response || !response.success) {
        typedWindow.UserPanelAlerts?.showError('shop-initialize-alert-error', response?.message ?? 'User creation failed.');
        return;
    }

    typedWindow.UserPanelAlerts?.showSuccess('shop-initialize-alert-success', response.message ?? 'User created successfully.');
}

function buildShopInitializeRegisterRequest(): InitializeCustomerRequestDto | null {
    const username = readShopInitializeValue('shop-initialize-username');
    const email = readShopInitializeValue('shop-initialize-email');
    const password = readShopInitializeValue('shop-initialize-password');
    const confirmPassword = readShopInitializeValue('shop-initialize-confirm-password');
    const customerName = readShopInitializeValue('shop-initialize-customer-name');
    const contactFirstName = readShopInitializeValue('shop-initialize-contact-first-name');
    const contactLastName = readShopInitializeValue('shop-initialize-contact-last-name');
    const customerPhone = readShopInitializeValue('shop-initialize-customer-phone');

    if (!username || !email || !password || !confirmPassword || !customerName || !contactFirstName || !contactLastName) {
        return null;
    }

    if (password !== confirmPassword) {
        return null;
    }

    return {
        username,
        email,
        password,
        confirmPassword,
        companyName: customerName,
        companyPhone: customerPhone,
        contactFirstName,
        contactLastName
    };
}

function readShopInitializeValue(id: string): string {
    const input = document.getElementById(id) as HTMLInputElement | null;
    return input?.value.trim() ?? '';
}

async function setupShopInitializeBuildMode(): Promise<void> {
    const typedWindow = window as InitializeWindow;
    const info = document.getElementById('shop-initialize-alert-info');
    const importSection = document.getElementById('shop-initialize-import-section');

    typedWindow.UserPanelAlerts?.hide('shop-initialize-alert-success');
    typedWindow.UserPanelAlerts?.hide('shop-initialize-alert-error');

    const response = await typedWindow.UserPanelApi?.request<InitializeBuildModeResponse>('/Initialization/build-mode', {
        method: 'GET'
    }, false);

    if (!response || !response.success || !response.data) {
        if (info) {
            info.textContent = 'Unable to verify API build mode.';
        }

        typedWindow.UserPanelAlerts?.showError('shop-initialize-alert-error', response?.message ?? 'Unable to verify API build mode.');
        return;
    }

    const isDebug = response.data.isDebug ?? response.data.IsDebug ?? false;
    const mode = response.data.mode ?? response.data.Mode ?? (isDebug ? 'Debug' : 'Release');

    if (!isDebug) {
        if (info) {
            info.textContent = `Debug import is disabled. API build mode: ${mode}.`;
        }

        return;
    }

    if (info) {
        info.classList.add('d-none');
    }

    importSection?.classList.remove('d-none');
}

async function importShopDebugSnapshot(): Promise<void> {
    const typedWindow = window as InitializeWindow;
    const fileInput = document.getElementById('shop-initialize-file') as HTMLInputElement | null;
    const button = document.getElementById('shop-initialize-import-button') as HTMLButtonElement | null;
    const fileName = fileInput?.value.trim() ?? '';

    typedWindow.UserPanelAlerts?.hide('shop-initialize-alert-success');
    typedWindow.UserPanelAlerts?.hide('shop-initialize-alert-error');

    if (!fileName) {
        typedWindow.UserPanelAlerts?.showError('shop-initialize-alert-error', 'Snapshot file name is required.');
        return;
    }

    if (button) {
        button.disabled = true;
        button.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>Importing...';
    }

    const response = await typedWindow.UserPanelApi?.request<InitializeImportResponse>('/Initialization/import-customer-snapshot', {
        method: 'POST',
        body: JSON.stringify({ fileName })
    }, false);

    if (button) {
        button.disabled = false;
        button.innerHTML = '<i class="bi bi-upload"></i> Import Snapshot';
    }

    if (!response || !response.success || !response.data) {
        typedWindow.UserPanelAlerts?.showError('shop-initialize-alert-error', response?.message ?? 'Snapshot import failed.');
        return;
    }

    const importSuccess = response.data.success ?? response.data.Success ?? false;
    if (!importSuccess) {
        typedWindow.UserPanelAlerts?.showError('shop-initialize-alert-error', response.data.errorMessage ?? response.data.ErrorMessage ?? 'Snapshot import failed.');
        return;
    }

    typedWindow.UserPanelAlerts?.showSuccess('shop-initialize-alert-success', 'Snapshot imported successfully.');
}

if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initializeShopInitializePage);
} else {
    initializeShopInitializePage();
}

function registerShopInitializeEnhancedLoadListener(): void {
    const typedWindow = window as InitializeWindow;

    if (typedWindow.Blazor?.addEventListener) {
        typedWindow.Blazor.addEventListener('enhancedload', initializeShopInitializePage);
        return;
    }

    window.setTimeout(registerShopInitializeEnhancedLoadListener, 100);
}

registerShopInitializeEnhancedLoadListener();

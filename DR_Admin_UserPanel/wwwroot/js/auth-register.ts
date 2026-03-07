interface RegisterRequestDto {
    username: string;
    email: string;
    password: string;
    confirmPassword: string;
    customerName: string;
    customerEmail: string;
    customerPhone: string;
    customerAddress: string;
    siteCode: string;
    isSelfRegisteredCustomer: boolean;
}

interface RegisterResponseDto {
    userId: number;
    email: string;
    message?: string;
}

interface RegisterWindow extends Window {
    UserPanelSettings?: {
        frontendSiteCode: string;
    };
    UserPanelApi?: {
        request: <T>(path: string, options?: RequestInit, requiresAuth?: boolean) => Promise<{ success: boolean; data?: T; message?: string }>;
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

function initializeRegister(): void {
    const form = document.getElementById('auth-register-form') as HTMLFormElement | null;
    if (!form || form.dataset.bound === 'true') {
        return;
    }

    form.dataset.bound = 'true';

    bindRegisterPasswordToggle('auth-register-password', 'auth-register-toggle-password');
    bindRegisterPasswordToggle('auth-register-confirm-password', 'auth-register-toggle-confirm-password');

    form.addEventListener('submit', async (event: Event) => {
        event.preventDefault();

        const typedWindow = window as RegisterWindow;
        typedWindow.UserPanelAlerts?.hide('auth-register-alert-success');
        typedWindow.UserPanelAlerts?.hide('auth-register-alert-error');

        const request = buildRegisterRequest();
        if (!request) {
            typedWindow.UserPanelAlerts?.showError('auth-register-alert-error', 'Please fill in all required registration fields.');
            return;
        }

        const response = await typedWindow.UserPanelApi?.request<RegisterResponseDto>('/MyAccount/register', {
            method: 'POST',
            body: JSON.stringify(request)
        }, false);

        if (!response) {
            typedWindow.UserPanelAlerts?.showError('auth-register-alert-error', 'Registration request could not be sent.');
            return;
        }

        if (!response.success) {
            typedWindow.UserPanelAlerts?.showError('auth-register-alert-error', response.message ?? 'Registration failed.');
            return;
        }

        typedWindow.UserPanelAlerts?.showSuccess('auth-register-alert-success', response.message ?? 'Account created. Redirecting to login...');
        setTimeout(() => {
            window.location.href = '/account/login';
        }, 1200);
    });
}

function bindRegisterPasswordToggle(inputId: string, toggleId: string): void {
    const input = document.getElementById(inputId) as HTMLInputElement | null;
    const toggle = document.getElementById(toggleId) as HTMLButtonElement | null;
    const icon = toggle?.querySelector('i');
    if (!input || !toggle || !icon || toggle.dataset.bound === 'true') {
        return;
    }

    toggle.dataset.bound = 'true';
    toggle.addEventListener('click', () => {
        const isPassword = input.type === 'password';
        input.type = isPassword ? 'text' : 'password';
        icon.className = isPassword ? 'bi bi-eye-slash' : 'bi bi-eye';
        toggle.setAttribute('aria-label', isPassword ? 'Hide password' : 'Show password');
    });
}

function buildRegisterRequest(): RegisterRequestDto | null {
    const username = readValue('auth-register-username');
    const email = readValue('auth-register-email');
    const password = readValue('auth-register-password');
    const confirmPassword = readValue('auth-register-confirm-password');
    const customerName = readValue('auth-register-customer-name');
    const customerPhone = readValue('auth-register-customer-phone');
    const customerAddress = readValue('auth-register-customer-address');

    if (!username || !email || !password || !confirmPassword || !customerName) {
        return null;
    }

    if (password !== confirmPassword) {
        return null;
    }

    const typedWindow = window as RegisterWindow;
    const siteCode = typedWindow.UserPanelSettings?.frontendSiteCode ?? 'shop';

    return {
        username,
        email,
        password,
        confirmPassword,
        customerName,
        customerEmail: email,
        customerPhone,
        customerAddress,
        siteCode,
        isSelfRegisteredCustomer: true
    };
}

function readValue(id: string): string {
    const input = document.getElementById(id) as HTMLInputElement | null;
    return input?.value.trim() ?? '';
}

if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initializeRegister);
} else {
    initializeRegister();
}

function registerRegisterEnhancedLoadListener(): void {
    const typedWindow = window as RegisterWindow;

    if (typedWindow.Blazor?.addEventListener) {
        typedWindow.Blazor.addEventListener('enhancedload', initializeRegister);
        return;
    }

    window.setTimeout(registerRegisterEnhancedLoadListener, 100);
}

registerRegisterEnhancedLoadListener();

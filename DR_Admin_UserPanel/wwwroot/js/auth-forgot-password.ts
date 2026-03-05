interface ForgotPasswordRequestDto {
    email: string;
    siteCode: string;
}

interface ForgotWindow extends Window {
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

function initializeForgotPassword(): void {
    const form = document.getElementById('auth-forgot-password-form') as HTMLFormElement | null;
    if (!form || form.dataset.bound === 'true') {
        return;
    }

    form.dataset.bound = 'true';

    form.addEventListener('submit', async (event: Event) => {
        event.preventDefault();

        const typedWindow = window as ForgotWindow;
        typedWindow.UserPanelAlerts?.hide('auth-forgot-password-alert-success');
        typedWindow.UserPanelAlerts?.hide('auth-forgot-password-alert-error');

        const email = readForgotValue('auth-forgot-password-email');
        if (!email) {
            typedWindow.UserPanelAlerts?.showError('auth-forgot-password-alert-error', 'Email is required.');
            return;
        }

        const siteCode = typedWindow.UserPanelSettings?.frontendSiteCode ?? 'shop';
        const payload: ForgotPasswordRequestDto = { email, siteCode };

        const response = await typedWindow.UserPanelApi?.request<unknown>('/MyAccount/request-password-reset', {
            method: 'POST',
            body: JSON.stringify(payload)
        }, false);

        if (!response || !response.success) {
            typedWindow.UserPanelAlerts?.showError('auth-forgot-password-alert-error', response?.message ?? 'Could not request reset link.');
            return;
        }

        typedWindow.UserPanelAlerts?.showSuccess('auth-forgot-password-alert-success', response.message ?? 'If the account exists, a reset link has been sent.');
        form.reset();
    });
}

function readForgotValue(id: string): string {
    const input = document.getElementById(id) as HTMLInputElement | null;
    return input?.value.trim() ?? '';
}

if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initializeForgotPassword);
} else {
    initializeForgotPassword();
}

function registerForgotEnhancedLoadListener(): void {
    const typedWindow = window as ForgotWindow;

    if (typedWindow.Blazor?.addEventListener) {
        typedWindow.Blazor.addEventListener('enhancedload', initializeForgotPassword);
        return;
    }

    window.setTimeout(registerForgotEnhancedLoadListener, 100);
}

registerForgotEnhancedLoadListener();

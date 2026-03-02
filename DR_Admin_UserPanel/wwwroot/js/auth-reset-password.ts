interface ResetPasswordRequestDto {
    token: string;
    newPassword: string;
    confirmPassword: string;
}

interface ResetWindow extends Window {
    UserPanelApi?: {
        request: <T>(path: string, options?: RequestInit, requiresAuth?: boolean) => Promise<{ success: boolean; data?: T; message?: string }>;
    };
    UserPanelAlerts?: {
        showSuccess: (id: string, message: string) => void;
        showError: (id: string, message: string) => void;
        hide: (id: string) => void;
    };
}

function initializeResetPassword(): void {
    const form = document.getElementById('auth-reset-password-form') as HTMLFormElement | null;
    if (!form || form.dataset.bound === 'true') {
        return;
    }

    form.dataset.bound = 'true';

    form.addEventListener('submit', async (event: Event) => {
        event.preventDefault();

        const typedWindow = window as ResetWindow;
        typedWindow.UserPanelAlerts?.hide('auth-reset-password-alert-success');
        typedWindow.UserPanelAlerts?.hide('auth-reset-password-alert-error');

        const token = new URLSearchParams(window.location.search).get('token') ?? '';
        const newPassword = readResetValue('auth-reset-password-new');
        const confirmPassword = readResetValue('auth-reset-password-confirm');

        if (!token) {
            typedWindow.UserPanelAlerts?.showError('auth-reset-password-alert-error', 'Missing reset token.');
            return;
        }

        if (!newPassword || !confirmPassword) {
            typedWindow.UserPanelAlerts?.showError('auth-reset-password-alert-error', 'Both password fields are required.');
            return;
        }

        if (newPassword !== confirmPassword) {
            typedWindow.UserPanelAlerts?.showError('auth-reset-password-alert-error', 'Passwords do not match.');
            return;
        }

        const payload: ResetPasswordRequestDto = { token, newPassword, confirmPassword };
        const response = await typedWindow.UserPanelApi?.request<unknown>('/MyAccount/reset-password', {
            method: 'POST',
            body: JSON.stringify(payload)
        }, false);

        if (!response || !response.success) {
            typedWindow.UserPanelAlerts?.showError('auth-reset-password-alert-error', response?.message ?? 'Password reset failed.');
            return;
        }

        typedWindow.UserPanelAlerts?.showSuccess('auth-reset-password-alert-success', response.message ?? 'Password reset successful. Redirecting to login.');
        setTimeout(() => {
            window.location.href = '/account/login';
        }, 1200);
    });
}

function readResetValue(id: string): string {
    const input = document.getElementById(id) as HTMLInputElement | null;
    return input?.value.trim() ?? '';
}

if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initializeResetPassword);
} else {
    initializeResetPassword();
}

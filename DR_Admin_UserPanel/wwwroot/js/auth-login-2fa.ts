interface VerifyMailTwoFactorRequestDto {
    challengeToken: string;
    code: string;
}

interface ResendMailTwoFactorRequestDto {
    challengeToken: string;
}

interface LoginResponseDto {
    userId: number;
    username: string;
    accessToken: string;
    refreshToken: string;
    expiresAt: string;
    roles: string[];
}

interface LoginTwoFactorWindow extends Window {
    UserPanelApi?: {
        request: <T>(path: string, options?: RequestInit, requiresAuth?: boolean) => Promise<{ success: boolean; data?: T; message?: string }>;
    };
    UserPanelAuth?: {
        setSession: (session: {
            userId: number;
            username: string;
            accessToken: string;
            refreshToken: string;
            expiresAt: string;
            roles: string[];
        }) => void;
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

const challengeStorageKey = 'up-2fa-challenge';

function initializeLoginTwoFactor(): void {
    const form = document.getElementById('auth-login-2fa-form') as HTMLFormElement | null;
    if (!form || form.dataset.bound === 'true') {
        return;
    }

    form.dataset.bound = 'true';

    const challengeToken = getChallengeToken();
    if (!challengeToken) {
        const typedWindow = window as LoginTwoFactorWindow;
        typedWindow.UserPanelAlerts?.showError('auth-login-2fa-alert-error', 'Two-factor challenge has expired. Please sign in again.');
        window.setTimeout(() => {
            window.location.href = '/account/login';
        }, 1200);
        return;
    }

    form.addEventListener('submit', async (event: Event) => {
        event.preventDefault();
        await verifyCode(challengeToken);
    });

    document.getElementById('auth-login-2fa-resend')?.addEventListener('click', async () => {
        await resendCode(challengeToken);
    });
}

async function verifyCode(challengeToken: string): Promise<void> {
    const typedWindow = window as LoginTwoFactorWindow;
    typedWindow.UserPanelAlerts?.hide('auth-login-2fa-alert-success');
    typedWindow.UserPanelAlerts?.hide('auth-login-2fa-alert-error');

    const codeInput = document.getElementById('auth-login-2fa-code') as HTMLInputElement | null;
    const code = codeInput?.value.trim() ?? '';

    if (code.length < 6) {
        typedWindow.UserPanelAlerts?.showError('auth-login-2fa-alert-error', 'Enter the 6-digit verification code.');
        return;
    }

    const payload: VerifyMailTwoFactorRequestDto = {
        challengeToken,
        code
    };

    const response = await typedWindow.UserPanelApi?.request<LoginResponseDto>('/Auth/2fa/verify', {
        method: 'POST',
        body: JSON.stringify(payload)
    }, false);

    if (!response || !response.success || !response.data) {
        typedWindow.UserPanelAlerts?.showError('auth-login-2fa-alert-error', response?.message ?? 'Verification failed.');
        return;
    }

    sessionStorage.removeItem(challengeStorageKey);

    typedWindow.UserPanelAuth?.setSession({
        userId: response.data.userId,
        username: response.data.username,
        accessToken: response.data.accessToken,
        refreshToken: response.data.refreshToken,
        expiresAt: response.data.expiresAt,
        roles: response.data.roles ?? []
    });

    typedWindow.UserPanelAlerts?.showSuccess('auth-login-2fa-alert-success', 'Verification successful. Redirecting...');

    const returnUrl = new URLSearchParams(window.location.search).get('returnUrl');
    const target = returnUrl && returnUrl.startsWith('/') ? returnUrl : '/dashboard';
    window.setTimeout(() => {
        window.location.href = target;
    }, 700);
}

async function resendCode(challengeToken: string): Promise<void> {
    const typedWindow = window as LoginTwoFactorWindow;
    typedWindow.UserPanelAlerts?.hide('auth-login-2fa-alert-success');
    typedWindow.UserPanelAlerts?.hide('auth-login-2fa-alert-error');

    const payload: ResendMailTwoFactorRequestDto = { challengeToken };
    const response = await typedWindow.UserPanelApi?.request<unknown>('/Auth/2fa/resend', {
        method: 'POST',
        body: JSON.stringify(payload)
    }, false);

    if (!response || !response.success) {
        typedWindow.UserPanelAlerts?.showError('auth-login-2fa-alert-error', response?.message ?? 'Could not resend verification code.');
        return;
    }

    typedWindow.UserPanelAlerts?.showSuccess('auth-login-2fa-alert-success', response.message ?? 'Verification code sent.');
}

function getChallengeToken(): string {
    const queryToken = new URLSearchParams(window.location.search).get('challengeToken');
    if (queryToken && queryToken.trim()) {
        sessionStorage.setItem(challengeStorageKey, queryToken.trim());
        return queryToken.trim();
    }

    return sessionStorage.getItem(challengeStorageKey) ?? '';
}

if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initializeLoginTwoFactor);
} else {
    initializeLoginTwoFactor();
}

function registerLoginTwoFactorEnhancedLoadListener(): void {
    const typedWindow = window as LoginTwoFactorWindow;

    if (typedWindow.Blazor?.addEventListener) {
        typedWindow.Blazor.addEventListener('enhancedload', initializeLoginTwoFactor);
        return;
    }

    window.setTimeout(registerLoginTwoFactorEnhancedLoadListener, 100);
}

registerLoginTwoFactorEnhancedLoadListener();

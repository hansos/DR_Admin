interface LoginRequestDto {
    username: string;
    password: string;
}

interface LoginResponseDto {
    userId: number;
    username: string;
    accessToken: string;
    refreshToken: string;
    expiresAt: string;
    roles: string[];
    requiresTwoFactor?: boolean;
    twoFactorMethod?: string | null;
    twoFactorChallengeToken?: string | null;
}

interface LoginWindow extends Window {
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
        isLoggedIn: () => boolean;
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

function initializeLogin(): void {
    const form = document.getElementById('auth-login-form') as HTMLFormElement | null;
    if (!form || form.dataset.bound === 'true') {
        return;
    }

    form.dataset.bound = 'true';

    const typedWindow = window as LoginWindow;
    if (typedWindow.UserPanelAuth?.isLoggedIn()) {
        window.location.href = '/dashboard';
        return;
    }

    bindLoginPasswordToggle();

    form.addEventListener('submit', async (event: Event) => {
        event.preventDefault();

        typedWindow.UserPanelAlerts?.hide('auth-login-alert-success');
        typedWindow.UserPanelAlerts?.hide('auth-login-alert-error');

        const username = readLoginValue('auth-login-username');
        const password = readLoginValue('auth-login-password');

        if (!username || !password) {
            typedWindow.UserPanelAlerts?.showError('auth-login-alert-error', 'Username and password are required.');
            return;
        }

        const request: LoginRequestDto = { username, password };
        const response = await typedWindow.UserPanelApi?.request<LoginResponseDto>('/Auth/login', {
            method: 'POST',
            body: JSON.stringify(request)
        }, false);

        if (!response || !response.success || !response.data) {
            typedWindow.UserPanelAlerts?.showError('auth-login-alert-error', response?.message ?? 'Login failed.');
            return;
        }

        if (response.data.requiresTwoFactor && response.data.twoFactorChallengeToken) {
            sessionStorage.setItem('up-2fa-challenge', response.data.twoFactorChallengeToken);
            const method = (response.data.twoFactorMethod ?? 'Email').trim() || 'Email';
            sessionStorage.setItem('up-2fa-method', method);

            const statusMessage = method === 'Authenticator'
                ? 'Verification required in Microsoft Authenticator. Redirecting...'
                : 'Verification code sent to your email. Redirecting...';

            typedWindow.UserPanelAlerts?.showSuccess('auth-login-alert-success', statusMessage);

            const returnUrl = new URLSearchParams(window.location.search).get('returnUrl');
            const methodQuery = `method=${encodeURIComponent(method)}`;
            const target = returnUrl && returnUrl.startsWith('/')
                ? `/account/login-2fa?${methodQuery}&returnUrl=${encodeURIComponent(returnUrl)}`
                : `/account/login-2fa?${methodQuery}`;

            setTimeout(() => {
                window.location.href = target;
            }, 600);

            return;
        }

        typedWindow.UserPanelAuth?.setSession({
            userId: response.data.userId,
            username: response.data.username,
            accessToken: response.data.accessToken,
            refreshToken: response.data.refreshToken,
            expiresAt: response.data.expiresAt,
            roles: response.data.roles ?? []
        });

        typedWindow.UserPanelAlerts?.showSuccess('auth-login-alert-success', 'Login successful. Redirecting...');

        const returnUrl = new URLSearchParams(window.location.search).get('returnUrl');
        const target = returnUrl && returnUrl.startsWith('/') ? returnUrl : '/dashboard';
        setTimeout(() => {
            window.location.href = target;
        }, 700);
    });
}

function bindLoginPasswordToggle(): void {
    const input = document.getElementById('auth-login-password') as HTMLInputElement | null;
    const toggle = document.getElementById('auth-login-toggle-password') as HTMLButtonElement | null;
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

function readLoginValue(id: string): string {
    const input = document.getElementById(id) as HTMLInputElement | null;
    return input?.value.trim() ?? '';
}

if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initializeLogin);
} else {
    initializeLogin();
}

function registerLoginEnhancedLoadListener(): void {
    const typedWindow = window as LoginWindow;

    if (typedWindow.Blazor?.addEventListener) {
        typedWindow.Blazor.addEventListener('enhancedload', initializeLogin);
        return;
    }

    window.setTimeout(registerLoginEnhancedLoadListener, 100);
}

registerLoginEnhancedLoadListener();

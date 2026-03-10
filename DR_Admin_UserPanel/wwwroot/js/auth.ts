(() => {

interface AuthSession {
    userId: number;
    username: string;
    accessToken: string;
    refreshToken: string;
    expiresAt: string;
    roles: string[];
}

interface UserPanelAuthApi {
    isLoggedIn: () => boolean;
    getAccessToken: () => string | null;
    getSession: () => AuthSession | null;
    setSession: (session: AuthSession) => void;
    clearSession: () => void;
    logout: () => void;
    enforceGuard: () => void;
}

interface UserPanelWindowWithAuth extends Window {
    UserPanelSettings?: {
        protectedPathPrefixes: string[];
    };
    UserPanelAuth?: UserPanelAuthApi;
    Blazor?: {
        addEventListener?: (eventName: string, callback: () => void) => void;
    };
}

const authStorageKey = 'up_auth_session';

function parseAuthSession(raw: string | null): AuthSession | null {
    if (!raw) {
        return null;
    }

    try {
        const parsed = JSON.parse(raw) as AuthSession;
        if (!parsed.accessToken || !parsed.expiresAt) {
            return null;
        }

        return parsed;
    } catch {
        return null;
    }
}

function getSession(): AuthSession | null {
    return parseAuthSession(localStorage.getItem(authStorageKey));
}

function isExpired(session: AuthSession): boolean {
    const expiry = Date.parse(session.expiresAt);
    if (Number.isNaN(expiry)) {
        return true;
    }

    return Date.now() >= expiry;
}

function setSession(session: AuthSession): void {
    localStorage.setItem(authStorageKey, JSON.stringify(session));
    syncAuthUi();
}

function clearSession(): void {
    localStorage.removeItem(authStorageKey);
    syncAuthUi();
}

function isLoggedIn(): boolean {
    const session = getSession();
    return !!session && !isExpired(session);
}

function getAccessToken(): string | null {
    const session = getSession();
    if (!session || isExpired(session)) {
        return null;
    }

    return session.accessToken;
}

function isPublicRoute(path: string): boolean {
    const normalized = path.toLowerCase();
    return normalized === '/' ||
        normalized === '/about' ||
        normalized === '/about-public' ||
        normalized === '/shop/domain-search' ||
        normalized === '/account/login' ||
        normalized === '/account/register' ||
        normalized === '/account/forgot-password' ||
        normalized === '/account/reset-password' ||
        normalized === '/not-found';
}

function shouldProtect(path: string): boolean {
    const settingsWindow = window as UserPanelWindowWithAuth;
    const prefixes = settingsWindow.UserPanelSettings?.protectedPathPrefixes ?? [];
    const lowerPath = path.toLowerCase();
    return prefixes.some((prefix: string) => lowerPath.startsWith(prefix));
}

function enforceGuard(): void {
    const path = window.location.pathname;
    const session = getSession();

    if (session && isExpired(session)) {
        clearSession();
    }

    const protectedRoute = shouldProtect(path);
    if (protectedRoute && !isLoggedIn() && !isPublicRoute(path)) {
        const returnUrl = encodeURIComponent(window.location.pathname + window.location.search);
        window.location.href = `/account/login?returnUrl=${returnUrl}`;
    }
}

function routeHomeByAuth(): void {
    return;
}

function logout(): void {
    clearSession();
    window.location.href = '/';
}

function syncAuthUi(): void {
    const loggedIn = isLoggedIn();
    const session = getSession();
    const currentPath = window.location.pathname.toLowerCase();

    const logoutButton = document.getElementById('global-logout') as HTMLButtonElement | null;
    if (logoutButton) {
        logoutButton.style.display = loggedIn ? 'inline-block' : 'none';
    }

    const topAuthInfo = document.getElementById('top-auth-info');
    const topAuthUserName = document.getElementById('top-auth-username');
    if (topAuthInfo && topAuthUserName) {
        if (loggedIn && session?.username) {
            topAuthUserName.textContent = session.username;
            topAuthInfo.classList.remove('d-none');
        } else {
            topAuthUserName.textContent = '';
            topAuthInfo.classList.add('d-none');
        }
    }

    const publicAuthLink = document.getElementById('public-auth-link') as HTMLAnchorElement | null;
    if (publicAuthLink) {
        const hidePublicSignIn = !loggedIn && currentPath === '/account/login';
        publicAuthLink.classList.toggle('d-none', hidePublicSignIn);

        if (loggedIn) {
            publicAuthLink.href = '#';
            publicAuthLink.dataset.mode = 'signout';
            publicAuthLink.innerHTML = '<i class="bi bi-box-arrow-in-right"></i>&nbsp;Sign out';
        } else {
            publicAuthLink.href = '/account/login';
            publicAuthLink.dataset.mode = 'signin';
            publicAuthLink.innerHTML = '<i class="bi bi-box-arrow-in-right"></i>&nbsp;Sign in';
        }
    }
}

function bindLogoutButton(): void {
    const logoutButton = document.getElementById('global-logout') as HTMLButtonElement | null;
    if (!logoutButton || logoutButton.dataset.bound === 'true') {
    } else {
        logoutButton.dataset.bound = 'true';
        logoutButton.addEventListener('click', () => {
            logout();
        });
    }

    const topLogout = document.getElementById('top-auth-logout') as HTMLAnchorElement | null;
    if (!topLogout || topLogout.dataset.bound === 'true') {
        return;
    }

    topLogout.dataset.bound = 'true';
    topLogout.addEventListener('click', (event: Event) => {
        event.preventDefault();
        logout();
    });

    const publicAuthLink = document.getElementById('public-auth-link') as HTMLAnchorElement | null;
    if (!publicAuthLink || publicAuthLink.dataset.bound === 'true') {
        return;
    }

    publicAuthLink.dataset.bound = 'true';
    publicAuthLink.addEventListener('click', (event: Event) => {
        const mode = publicAuthLink.dataset.mode ?? '';
        if (mode !== 'signout') {
            return;
        }

        event.preventDefault();
        logout();
    });
}

function initializeAuth(): void {
    enforceGuard();
    routeHomeByAuth();
    bindLogoutButton();
    syncAuthUi();
}

const authWindow = window as UserPanelWindowWithAuth;
authWindow.UserPanelAuth = {
    isLoggedIn,
    getAccessToken,
    getSession,
    setSession,
    clearSession,
    logout,
    enforceGuard
};

if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initializeAuth);
} else {
    initializeAuth();
}

function registerAuthEnhancedLoadListener(): void {
    if (authWindow.Blazor?.addEventListener) {
        authWindow.Blazor.addEventListener('enhancedload', initializeAuth);
        return;
    }

    window.setTimeout(registerAuthEnhancedLoadListener, 100);
}

registerAuthEnhancedLoadListener();

})();

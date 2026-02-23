/**
 * Authentication State Management for DR Admin Reseller Panel.
 * Stores session data in sessionStorage so state is cleared when the browser tab is closed.
 */

const AUTH_TOKEN_KEY           = 'rp_authToken';
const AUTH_USERNAME_KEY        = 'rp_username';
const AUTH_ROLES_KEY           = 'rp_roles';
const AUTH_REFRESH_TOKEN_KEY   = 'rp_refreshToken';
const AUTH_EXPIRES_KEY         = 'rp_tokenExpiresAt';

function authIsLoggedIn(): boolean {
    return !!sessionStorage.getItem(AUTH_TOKEN_KEY);
}

function authGetUsername(): string {
    return sessionStorage.getItem(AUTH_USERNAME_KEY) ?? '';
}

function authGetRoles(): string[] {
    const raw = sessionStorage.getItem(AUTH_ROLES_KEY);
    if (!raw) return [];
    try { return JSON.parse(raw) as string[]; } catch { return []; }
}

function authGetToken(): string | null {
    return sessionStorage.getItem(AUTH_TOKEN_KEY);
}

function authIsExpired(): boolean {
    const token = authGetToken();
    const expiresRaw = sessionStorage.getItem(AUTH_EXPIRES_KEY);

    if (!token) return true;
    if (!expiresRaw) return true;

    const expiresAt = Date.parse(expiresRaw);
    if (Number.isNaN(expiresAt)) return true;

    return Date.now() >= expiresAt;
}

function authSetData(
    username: string,
    token: string,
    refreshToken: string | null,
    roles: string[],
    expiresAt: string | null
): void {
    sessionStorage.setItem(AUTH_USERNAME_KEY, username);
    sessionStorage.setItem(AUTH_TOKEN_KEY, token);
    sessionStorage.setItem(AUTH_ROLES_KEY, JSON.stringify(roles));
    if (refreshToken) sessionStorage.setItem(AUTH_REFRESH_TOKEN_KEY, refreshToken);
    if (expiresAt)    sessionStorage.setItem(AUTH_EXPIRES_KEY, expiresAt);
}

function authClear(): void {
    sessionStorage.removeItem(AUTH_TOKEN_KEY);
    sessionStorage.removeItem(AUTH_USERNAME_KEY);
    sessionStorage.removeItem(AUTH_ROLES_KEY);
    sessionStorage.removeItem(AUTH_REFRESH_TOKEN_KEY);
    sessionStorage.removeItem(AUTH_EXPIRES_KEY);
}

function authLogout(): void {
    authClear();
    window.location.href = '/login';
}

function authUpdateTopRow(): void {
    const userInfoEl  = document.getElementById('top-row-userinfo');
    const usernameEl  = document.getElementById('top-row-username');
    const loginLinkEl = document.getElementById('top-row-login');
    const logoutBtnEl = document.getElementById('top-row-logout');

    const loggedIn = authIsLoggedIn();
    const username = authGetUsername();

    if (usernameEl)  usernameEl.textContent          = username;
    if (userInfoEl)  userInfoEl.style.display         = loggedIn ? 'inline-flex' : 'none';
    if (loginLinkEl) loginLinkEl.style.display        = loggedIn ? 'none'        : 'inline-flex';
    if (logoutBtnEl) logoutBtnEl.style.display        = loggedIn ? 'inline-flex' : 'none';
}

function authEnforce(): void {
    const path = window.location.pathname.toLowerCase();
    const isLoginRoute = path === '/login' || path.startsWith('/login/');
    const expired = authIsExpired();

    if (expired) {
        authClear();
        if (!isLoginRoute) {
            window.location.href = '/login';
        }
        return;
    }
}

document.addEventListener('DOMContentLoaded', () => {
    authEnforce();
    authUpdateTopRow();
});

// Re-run after Blazor enhanced navigation so the top-row stays in sync
document.addEventListener('enhancedload', () => {
    authEnforce();
    authUpdateTopRow();
});

setInterval(() => { authEnforce(); }, 60000);

// Expose on window so login.ts and inline scripts can call it
(window as any).Auth = {
    isLoggedIn:  authIsLoggedIn,
    getUsername: authGetUsername,
    getRoles:    authGetRoles,
    getToken:    authGetToken,
    isExpired:   authIsExpired,
    setData:     authSetData,
    clear:       authClear,
    logout:      authLogout,
    updateTopRow: authUpdateTopRow,
};

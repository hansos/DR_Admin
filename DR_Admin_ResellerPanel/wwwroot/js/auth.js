"use strict";
/**
 * Authentication State Management for DR Admin Reseller Panel.
 * Stores session data in sessionStorage so state is cleared when the browser tab is closed.
 */
const AUTH_TOKEN_KEY = 'rp_authToken';
const AUTH_USERNAME_KEY = 'rp_username';
const AUTH_ROLES_KEY = 'rp_roles';
const AUTH_REFRESH_TOKEN_KEY = 'rp_refreshToken';
const AUTH_EXPIRES_KEY = 'rp_tokenExpiresAt';
function authIsLoggedIn() {
    return !!sessionStorage.getItem(AUTH_TOKEN_KEY);
}
function authGetUsername() {
    return sessionStorage.getItem(AUTH_USERNAME_KEY) ?? '';
}
function authGetRoles() {
    const raw = sessionStorage.getItem(AUTH_ROLES_KEY);
    if (!raw)
        return [];
    try {
        return JSON.parse(raw);
    }
    catch {
        return [];
    }
}
function authGetToken() {
    return sessionStorage.getItem(AUTH_TOKEN_KEY);
}
function authIsExpired() {
    const token = authGetToken();
    const expiresRaw = sessionStorage.getItem(AUTH_EXPIRES_KEY);
    if (!token)
        return true;
    if (!expiresRaw)
        return true;
    const expiresAt = Date.parse(expiresRaw);
    if (Number.isNaN(expiresAt))
        return true;
    return Date.now() >= expiresAt;
}
function authSetData(username, token, refreshToken, roles, expiresAt) {
    sessionStorage.setItem(AUTH_USERNAME_KEY, username);
    sessionStorage.setItem(AUTH_TOKEN_KEY, token);
    sessionStorage.setItem(AUTH_ROLES_KEY, JSON.stringify(roles));
    if (refreshToken)
        sessionStorage.setItem(AUTH_REFRESH_TOKEN_KEY, refreshToken);
    if (expiresAt)
        sessionStorage.setItem(AUTH_EXPIRES_KEY, expiresAt);
}
function authClear() {
    sessionStorage.removeItem(AUTH_TOKEN_KEY);
    sessionStorage.removeItem(AUTH_USERNAME_KEY);
    sessionStorage.removeItem(AUTH_ROLES_KEY);
    sessionStorage.removeItem(AUTH_REFRESH_TOKEN_KEY);
    sessionStorage.removeItem(AUTH_EXPIRES_KEY);
}
function authLogout() {
    authClear();
    window.location.href = '/login';
}
function authUpdateTopRow() {
    const userInfoEl = document.getElementById('top-row-userinfo');
    const usernameEl = document.getElementById('top-row-username');
    const loginLinkEl = document.getElementById('top-row-login');
    const logoutBtnEl = document.getElementById('top-row-logout');
    const loggedIn = authIsLoggedIn();
    const username = authGetUsername();
    if (usernameEl)
        usernameEl.textContent = username;
    if (userInfoEl)
        userInfoEl.style.display = loggedIn ? 'inline-flex' : 'none';
    if (loginLinkEl)
        loginLinkEl.style.display = loggedIn ? 'none' : 'inline-flex';
    if (logoutBtnEl)
        logoutBtnEl.style.display = loggedIn ? 'inline-flex' : 'none';
}
function authEnforce() {
    const path = window.location.pathname.toLowerCase();
    const isPublicRoute = path === '/login' || path.startsWith('/login/') ||
        path === '/forgot-password' || path.startsWith('/forgot-password/') ||
        path === '/my-account/change-password' || path.startsWith('/my-account/change-password') ||
        path === '/my-account/confirm-email' || path.startsWith('/my-account/confirm-email');
    const expired = authIsExpired();
    if (expired) {
        authClear();
        if (!isPublicRoute) {
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
// Use Blazor's API for enhanced navigation events
function setupBlazorNavigationListener() {
    // Check if Blazor object exists and has addEventListener
    const blazor = window.Blazor;
    if (blazor?.addEventListener) {
        blazor.addEventListener('enhancedload', () => {
            authEnforce();
            authUpdateTopRow();
        });
        console.log('Auth: Blazor enhancedload listener registered');
    }
    else {
        // Blazor not loaded yet, retry after a short delay
        setTimeout(setupBlazorNavigationListener, 100);
    }
}
// Start trying to set up the Blazor listener
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', setupBlazorNavigationListener);
}
else {
    setupBlazorNavigationListener();
}
setInterval(() => { authEnforce(); }, 60000);
// Expose on window so login.ts and inline scripts can call it
window.Auth = {
    isLoggedIn: authIsLoggedIn,
    getUsername: authGetUsername,
    getRoles: authGetRoles,
    getToken: authGetToken,
    isExpired: authIsExpired,
    setData: authSetData,
    clear: authClear,
    logout: authLogout,
    updateTopRow: authUpdateTopRow,
};
//# sourceMappingURL=auth.js.map
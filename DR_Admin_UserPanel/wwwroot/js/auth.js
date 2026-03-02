"use strict";
const authStorageKey = 'up_auth_session';
function parseAuthSession(raw) {
    if (!raw) {
        return null;
    }
    try {
        const parsed = JSON.parse(raw);
        if (!parsed.accessToken || !parsed.expiresAt) {
            return null;
        }
        return parsed;
    }
    catch {
        return null;
    }
}
function getSession() {
    return parseAuthSession(localStorage.getItem(authStorageKey));
}
function isExpired(session) {
    const expiry = Date.parse(session.expiresAt);
    if (Number.isNaN(expiry)) {
        return true;
    }
    return Date.now() >= expiry;
}
function setSession(session) {
    localStorage.setItem(authStorageKey, JSON.stringify(session));
    syncAuthUi();
}
function clearSession() {
    localStorage.removeItem(authStorageKey);
    syncAuthUi();
}
function isLoggedIn() {
    const session = getSession();
    return !!session && !isExpired(session);
}
function getAccessToken() {
    const session = getSession();
    if (!session || isExpired(session)) {
        return null;
    }
    return session.accessToken;
}
function isPublicRoute(path) {
    const normalized = path.toLowerCase();
    return normalized === '/' ||
        normalized === '/account/login' ||
        normalized === '/account/register' ||
        normalized === '/account/forgot-password' ||
        normalized === '/account/reset-password' ||
        normalized === '/not-found';
}
function shouldProtect(path) {
    const settingsWindow = window;
    const prefixes = settingsWindow.UserPanelSettings?.protectedPathPrefixes ?? [];
    const lowerPath = path.toLowerCase();
    return prefixes.some((prefix) => lowerPath.startsWith(prefix));
}
function enforceGuard() {
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
function logout() {
    clearSession();
    window.location.href = '/account/login';
}
function syncAuthUi() {
    const logoutButton = document.getElementById('global-logout');
    if (!logoutButton) {
        return;
    }
    logoutButton.style.display = isLoggedIn() ? 'inline-block' : 'none';
}
function bindLogoutButton() {
    const logoutButton = document.getElementById('global-logout');
    if (!logoutButton || logoutButton.dataset.bound === 'true') {
        return;
    }
    logoutButton.dataset.bound = 'true';
    logoutButton.addEventListener('click', () => {
        logout();
    });
}
function initializeAuth() {
    enforceGuard();
    bindLogoutButton();
    syncAuthUi();
}
const authWindow = window;
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
}
else {
    initializeAuth();
}
if (authWindow.Blazor?.addEventListener) {
    authWindow.Blazor.addEventListener('enhancedload', initializeAuth);
}
//# sourceMappingURL=auth.js.map
"use strict";
(() => {
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
            normalized === '/about' ||
            normalized === '/about-public' ||
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
    function routeHomeByAuth() {
        const path = window.location.pathname.toLowerCase();
        if (path !== '/') {
            return;
        }
        const homePage = document.getElementById('home-page');
        if (!homePage) {
            return;
        }
        if (isLoggedIn()) {
            window.location.href = '/dashboard';
            return;
        }
        window.location.href = '/account/login';
    }
    function logout() {
        clearSession();
        window.location.href = '/account/login';
    }
    function syncAuthUi() {
        const loggedIn = isLoggedIn();
        const session = getSession();
        const logoutButton = document.getElementById('global-logout');
        if (logoutButton) {
            logoutButton.style.display = loggedIn ? 'inline-block' : 'none';
        }
        const topAuthInfo = document.getElementById('top-auth-info');
        const topAuthUserName = document.getElementById('top-auth-username');
        if (topAuthInfo && topAuthUserName) {
            if (loggedIn && session?.username) {
                topAuthUserName.textContent = session.username;
                topAuthInfo.classList.remove('d-none');
            }
            else {
                topAuthUserName.textContent = '';
                topAuthInfo.classList.add('d-none');
            }
        }
    }
    function bindLogoutButton() {
        const logoutButton = document.getElementById('global-logout');
        if (!logoutButton || logoutButton.dataset.bound === 'true') {
        }
        else {
            logoutButton.dataset.bound = 'true';
            logoutButton.addEventListener('click', () => {
                logout();
            });
        }
        const topLogout = document.getElementById('top-auth-logout');
        if (!topLogout || topLogout.dataset.bound === 'true') {
            return;
        }
        topLogout.dataset.bound = 'true';
        topLogout.addEventListener('click', (event) => {
            event.preventDefault();
            logout();
        });
    }
    function initializeAuth() {
        enforceGuard();
        routeHomeByAuth();
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
    function registerAuthEnhancedLoadListener() {
        if (authWindow.Blazor?.addEventListener) {
            authWindow.Blazor.addEventListener('enhancedload', initializeAuth);
            return;
        }
        window.setTimeout(registerAuthEnhancedLoadListener, 100);
    }
    registerAuthEnhancedLoadListener();
})();
//# sourceMappingURL=auth.js.map
"use strict";
function resolveApiBaseUrl() {
    const isHttps = window.location.protocol === 'https:';
    return isHttps ? 'https://localhost:7201/api/v1' : 'http://localhost:5133/api/v1';
}
const settingsWindow = window;
settingsWindow.UserPanelSettings = {
    apiBaseUrl: resolveApiBaseUrl(),
    frontendSiteCode: 'shop',
    protectedPathPrefixes: ['/dashboard', '/shop', '/domains', '/hosting', '/billing', '/contacts', '/profile', '/settings', '/security', '/privacy', '/support', '/activity']
};
//# sourceMappingURL=app-settings.js.map
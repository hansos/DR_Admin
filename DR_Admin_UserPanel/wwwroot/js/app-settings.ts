interface UserPanelSettings {
    apiBaseUrl: string;
    protectedPathPrefixes: string[];
}

interface UserPanelWindowWithSettings extends Window {
    UserPanelSettings?: UserPanelSettings;
}

function resolveApiBaseUrl(): string {
    const isHttps = window.location.protocol === 'https:';
    return isHttps ? 'https://localhost:7201/api/v1' : 'http://localhost:5133/api/v1';
}

const settingsWindow = window as UserPanelWindowWithSettings;
settingsWindow.UserPanelSettings = {
    apiBaseUrl: resolveApiBaseUrl(),
    protectedPathPrefixes: ['/dashboard', '/shop', '/domains', '/hosting', '/billing', '/contacts', '/profile', '/settings', '/security', '/privacy', '/support', '/activity']
};

"use strict";
function getApiBaseUrl() {
    const typedWindow = window;
    return typedWindow.UserPanelSettings?.apiBaseUrl ?? '';
}
async function request(path, options = {}, requiresAuth = true) {
    const typedWindow = window;
    const headers = new Headers(options.headers ?? {});
    if (!headers.has('Content-Type')) {
        headers.set('Content-Type', 'application/json');
    }
    if (requiresAuth) {
        const token = typedWindow.UserPanelAuth?.getAccessToken() ?? null;
        if (token) {
            headers.set('Authorization', `Bearer ${token}`);
        }
    }
    const endpoint = `${getApiBaseUrl()}${path}`;
    try {
        const response = await fetch(endpoint, {
            ...options,
            headers,
            credentials: 'include'
        });
        const contentType = response.headers.get('content-type') ?? '';
        const hasJson = contentType.includes('application/json');
        const payload = hasJson ? await response.json() : null;
        if (response.status === 401) {
            typedWindow.UserPanelAuth?.clearSession();
        }
        if (!response.ok) {
            const message = extractMessage(payload) || `Request failed (${response.status})`;
            return { success: false, message, statusCode: response.status };
        }
        const normalized = extractData(payload);
        return {
            success: true,
            data: normalized,
            message: extractMessage(payload),
            statusCode: response.status
        };
    }
    catch {
        return { success: false, message: 'Network error. Please try again.', statusCode: 0 };
    }
}
function extractMessage(payload) {
    if (!payload || typeof payload !== 'object') {
        return undefined;
    }
    const candidate = payload;
    if (typeof candidate.message === 'string') {
        return candidate.message;
    }
    if (typeof candidate.title === 'string') {
        return candidate.title;
    }
    return undefined;
}
function extractData(payload) {
    if (!payload || typeof payload !== 'object') {
        return payload;
    }
    const maybeWrapped = payload;
    return (maybeWrapped.data ?? payload);
}
const apiWindow = window;
apiWindow.UserPanelApi = {
    request
};
//# sourceMappingURL=api-client.js.map
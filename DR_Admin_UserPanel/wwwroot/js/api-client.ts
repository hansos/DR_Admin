interface ApiResult<T> {
    success: boolean;
    data?: T;
    message?: string;
    statusCode?: number;
}

interface UserPanelWindowApi extends Window {
    UserPanelSettings?: {
        apiBaseUrl: string;
    };
    UserPanelAuth?: {
        getAccessToken: () => string | null;
        clearSession: () => void;
    };
    UserPanelApi?: {
        request: <T>(path: string, options?: RequestInit, requiresAuth?: boolean) => Promise<ApiResult<T>>;
    };
}

function getApiBaseUrl(): string {
    const typedWindow = window as UserPanelWindowApi;
    return typedWindow.UserPanelSettings?.apiBaseUrl ?? '';
}

async function request<T>(path: string, options: RequestInit = {}, requiresAuth: boolean = true): Promise<ApiResult<T>> {
    const typedWindow = window as UserPanelWindowApi;
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
        const payload = hasJson ? await response.json() as unknown : null;

        if (response.status === 401) {
            typedWindow.UserPanelAuth?.clearSession();
        }

        if (!response.ok) {
            const message = extractMessage(payload) || `Request failed (${response.status})`;
            return { success: false, message, statusCode: response.status };
        }

        const normalized = extractData<T>(payload);
        return {
            success: true,
            data: normalized,
            message: extractMessage(payload),
            statusCode: response.status
        };
    } catch {
        return { success: false, message: 'Network error. Please try again.', statusCode: 0 };
    }
}

function extractMessage(payload: unknown): string | undefined {
    if (!payload || typeof payload !== 'object') {
        return undefined;
    }

    const candidate = payload as { message?: unknown; title?: unknown };
    if (typeof candidate.message === 'string') {
        return candidate.message;
    }

    if (typeof candidate.title === 'string') {
        return candidate.title;
    }

    return undefined;
}

function extractData<T>(payload: unknown): T {
    if (!payload || typeof payload !== 'object') {
        return payload as T;
    }

    const maybeWrapped = payload as { data?: unknown };
    return (maybeWrapped.data ?? payload) as T;
}

const apiWindow = window as UserPanelWindowApi;
apiWindow.UserPanelApi = {
    request
};

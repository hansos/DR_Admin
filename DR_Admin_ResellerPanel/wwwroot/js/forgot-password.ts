// @ts-nocheck
(function() {

interface ApiResponse<T> {
    success: boolean;
    data?: T;
    message?: string;
}

function getApiBaseUrl(): string {
    const baseUrl = (window as any).AppSettings?.apiBaseUrl;
    if (!baseUrl) {
        const fallback = window.location.protocol === 'https:'
            ? 'https://localhost:7201/api/v1'
            : 'http://localhost:5133/api/v1';
        return fallback;
    }
    return baseUrl;
}

async function apiRequest<T>(endpoint: string, options: RequestInit = {}): Promise<ApiResponse<T>> {
    try {
        const headers: Record<string, string> = {
            'Content-Type': 'application/json',
            ...(options.headers as Record<string, string>),
        };

        const response = await fetch(endpoint, {
            ...options,
            headers,
            credentials: 'include',
        });

        const contentType = response.headers.get('content-type') ?? '';
        const hasJson = contentType.includes('application/json');
        const data = hasJson ? await response.json() : null;

        if (!response.ok) {
            return {
                success: false,
                message: (data && (data.message ?? data.title)) || `Request failed with status ${response.status}`,
            };
        }

        return {
            success: true,
            data: data?.data ?? data,
            message: data?.message,
        };
    } catch (error) {
        console.error('Forgot password request failed', error);
        return {
            success: false,
            message: 'Network error. Please try again.',
        };
    }
}

function showSuccess(message: string): void {
    const alertEl = document.getElementById('forgot-password-alert-success');
    const errorEl = document.getElementById('forgot-password-alert-error');
    
    if (errorEl) {
        errorEl.classList.add('d-none');
        errorEl.textContent = '';
    }
    
    if (alertEl) {
        alertEl.textContent = message;
        alertEl.classList.remove('d-none');
    }
}

function showError(message: string): void {
    const alertEl = document.getElementById('forgot-password-alert-error');
    const successEl = document.getElementById('forgot-password-alert-success');
    
    if (successEl) {
        successEl.classList.add('d-none');
        successEl.textContent = '';
    }
    
    if (alertEl) {
        alertEl.textContent = message;
        alertEl.classList.remove('d-none');
    }
}

function hideAlerts(): void {
    const successEl = document.getElementById('forgot-password-alert-success');
    const errorEl = document.getElementById('forgot-password-alert-error');
    
    if (successEl) {
        successEl.classList.add('d-none');
        successEl.textContent = '';
    }
    
    if (errorEl) {
        errorEl.classList.add('d-none');
        errorEl.textContent = '';
    }
}

async function requestPasswordReset(email: string): Promise<void> {
    const submitBtn = document.getElementById('forgot-password-submit') as HTMLButtonElement | null;
    
    if (submitBtn) {
        submitBtn.disabled = true;
        submitBtn.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>Sending...';
    }

    const response = await apiRequest<any>(`${getApiBaseUrl()}/MyAccount/request-password-reset`, {
        method: 'POST',
        body: JSON.stringify({ email }),
    });

    if (submitBtn) {
        submitBtn.disabled = false;
        submitBtn.innerHTML = '<i class="bi bi-send"></i> Send Reset Link';
    }

    if (response.success) {
        showSuccess(response.message || 'If the email address exists in our system, a password reset link has been sent.');
        
        const form = document.getElementById('forgot-password-form') as HTMLFormElement | null;
        if (form) {
            form.reset();
        }
    } else {
        showError(response.message || 'Failed to request password reset. Please try again.');
    }
}

function bindEvents(): void {
    const form = document.getElementById('forgot-password-form') as HTMLFormElement | null;
    
    if (!form) {
        return;
    }

    form.addEventListener('submit', async (e: Event) => {
        e.preventDefault();
        hideAlerts();

        const emailInput = document.getElementById('forgot-password-email') as HTMLInputElement | null;
        const email = emailInput?.value.trim() ?? '';

        if (!email) {
            showError('Please enter your email address.');
            return;
        }

        const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        if (!emailPattern.test(email)) {
            showError('Please enter a valid email address.');
            return;
        }

        await requestPasswordReset(email);
    });
}

let initialized = false;

function initializeForgotPassword(): boolean {
    const form = document.getElementById('forgot-password-form');
    if (!form) {
        return false;
    }

    if (initialized) {
        return true;
    }

    initialized = true;
    bindEvents();
    console.log('Forgot password page initialized');
    return true;
}

function tryInitialize(): void {
    if (initializeForgotPassword()) {
        return;
    }

    let attempts = 0;
    const maxAttempts = 50;
    const interval = setInterval(() => {
        attempts++;
        if (initializeForgotPassword() || attempts >= maxAttempts) {
            clearInterval(interval);
        }
    }, 100);
}

if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', tryInitialize);
} else {
    tryInitialize();
}

function setupBlazorNavListener(): void {
    const blazor = (window as any).Blazor;
    if (blazor?.addEventListener) {
        blazor.addEventListener('enhancedload', () => {
            initialized = false;
            tryInitialize();
        });
    } else {
        setTimeout(setupBlazorNavListener, 100);
    }
}
setupBlazorNavListener();

window.addEventListener('load', () => {
    if (!initialized) {
        tryInitialize();
    }
});

})();

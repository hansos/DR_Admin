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

function getAuthToken(): string | null {
    const auth = (window as any).Auth;
    if (auth?.getToken) {
        return auth.getToken();
    }
    return sessionStorage.getItem('rp_authToken');
}

async function apiRequest<T>(endpoint: string, options: RequestInit = {}): Promise<ApiResponse<T>> {
    try {
        const headers: Record<string, string> = {
            'Content-Type': 'application/json',
            ...(options.headers as Record<string, string>),
        };

        const authToken = getAuthToken();
        if (authToken) {
            headers['Authorization'] = `Bearer ${authToken}`;
        }

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
        console.error('Change password request failed', error);
        return {
            success: false,
            message: 'Network error. Please try again.',
        };
    }
}

function showSuccess(message: string): void {
    const alertEl = document.getElementById('change-password-alert-success');
    const errorEl = document.getElementById('change-password-alert-error');

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
    const alertEl = document.getElementById('change-password-alert-error');
    const successEl = document.getElementById('change-password-alert-success');

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
    const successEl = document.getElementById('change-password-alert-success');
    const errorEl = document.getElementById('change-password-alert-error');

    if (successEl) {
        successEl.classList.add('d-none');
        successEl.textContent = '';
    }

    if (errorEl) {
        errorEl.classList.add('d-none');
        errorEl.textContent = '';
    }
}

async function changePassword(currentPassword: string, newPassword: string, confirmPassword: string): Promise<void> {
    const submitBtn = document.getElementById('change-password-submit') as HTMLButtonElement | null;

    if (submitBtn) {
        submitBtn.disabled = true;
        submitBtn.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>Changing...';
    }

    const response = await apiRequest<any>(`${getApiBaseUrl()}/MyAccount/change-password`, {
        method: 'POST',
        body: JSON.stringify({ currentPassword, newPassword, confirmPassword }),
    });

    if (submitBtn) {
        submitBtn.disabled = false;
        submitBtn.innerHTML = '<i class="bi bi-check-lg"></i> Change Password';
    }

    if (response.success) {
        showSuccess(response.message || 'Password changed successfully.');

        const form = document.getElementById('change-password-form') as HTMLFormElement | null;
        if (form) {
            form.reset();
        }
    } else {
        showError(response.message || 'Failed to change password. Please try again.');
    }
}

function bindEvents(): void {
    const form = document.getElementById('change-password-form') as HTMLFormElement | null;

    if (!form) {
        return;
    }

    // Password toggle functionality
    setupPasswordToggle('toggle-change-password-current', 'change-password-current', 'toggle-change-password-current-icon');
    setupPasswordToggle('toggle-change-password-new', 'change-password-new', 'toggle-change-password-new-icon');
    setupPasswordToggle('toggle-change-password-confirm', 'change-password-confirm', 'toggle-change-password-confirm-icon');

    form.addEventListener('submit', async (e: Event) => {
        e.preventDefault();
        hideAlerts();

        const currentInput = document.getElementById('change-password-current') as HTMLInputElement | null;
        const newInput = document.getElementById('change-password-new') as HTMLInputElement | null;
        const confirmInput = document.getElementById('change-password-confirm') as HTMLInputElement | null;

        const currentPassword = currentInput?.value ?? '';
        const newPassword = newInput?.value ?? '';
        const confirmPassword = confirmInput?.value ?? '';

        if (!currentPassword) {
            showError('Please enter your current password.');
            return;
        }

        if (!newPassword) {
            showError('Please enter a new password.');
            return;
        }

        if (newPassword.length < 8) {
            showError('New password must be at least 8 characters long.');
            return;
        }

        if (newPassword !== confirmPassword) {
            showError('New passwords do not match.');
            return;
        }

        await changePassword(currentPassword, newPassword, confirmPassword);
    });
}

function setupPasswordToggle(buttonId: string, inputId: string, iconId: string): void {
    const toggleBtn = document.getElementById(buttonId) as HTMLButtonElement | null;
    const passwordInput = document.getElementById(inputId) as HTMLInputElement | null;
    const toggleIcon = document.getElementById(iconId);

    if (toggleBtn && passwordInput && toggleIcon) {
        toggleBtn.addEventListener('click', () => {
            if (passwordInput.type === 'password') {
                passwordInput.type = 'text';
                toggleIcon.className = 'bi bi-eye-slash';
            } else {
                passwordInput.type = 'password';
                toggleIcon.className = 'bi bi-eye';
            }
        });
    }
}

let initialized = false;

function initializeChangePassword(): boolean {
    const page = document.getElementById('change-password-page');
    if (!page) {
        return false;
    }

    if (initialized) {
        return true;
    }

    initialized = true;
    bindEvents();
    console.log('Change password page initialized');
    return true;
}

function tryInitialize(): void {
    if (initializeChangePassword()) {
        return;
    }

    let attempts = 0;
    const maxAttempts = 50;
    const interval = setInterval(() => {
        attempts++;
        if (initializeChangePassword() || attempts >= maxAttempts) {
            clearInterval(interval);
        }
    }, 100);
}

// Initialize when DOM is ready
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', tryInitialize);
} else {
    tryInitialize();
}

// Listen for Blazor's enhanced navigation
function setupBlazorNavListener(): void {
    const blazor = (window as any).Blazor;
    if (blazor?.addEventListener) {
        blazor.addEventListener('enhancedload', () => {
            initialized = false;
            tryInitialize();
        });
        console.log('ChangePassword: Blazor enhancedload listener registered');
    } else {
        setTimeout(setupBlazorNavListener, 100);
    }
}
setupBlazorNavListener();

// Handle InteractiveServer re-render after initial page load
window.addEventListener('load', () => {
    if (!initialized) {
        tryInitialize();
    }
});

})();

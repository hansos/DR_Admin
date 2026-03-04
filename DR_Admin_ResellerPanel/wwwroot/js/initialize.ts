// @ts-nocheck
(function () {
interface InitializationStatusResponse {
    isInitialized?: boolean;
    IsInitialized?: boolean;
}

interface InitializationResponse {
    success?: boolean;
    message?: string;
    username?: string;
}

function getApiBaseUrl(): string {
    return (window as any).AppSettings?.apiBaseUrl ?? '';
}

async function request<T>(url: string, options: RequestInit = {}): Promise<{ ok: boolean; status: number; data: T | null; message: string }> {
    try {
        const response = await fetch(url, {
            ...options,
            headers: {
                'Content-Type': 'application/json',
                ...(options.headers ?? {}),
            },
            credentials: 'include',
        });

        const contentType = response.headers.get('content-type') ?? '';
        const data = contentType.includes('application/json') ? await response.json() : null;
        const message = (data?.message ?? data?.title ?? '') as string;

        return { ok: response.ok, status: response.status, data, message };
    } catch {
        return { ok: false, status: 0, data: null, message: 'Network error. Please check your connection and try again.' };
    }
}

function showMessage(type: 'success' | 'error', message: string): void {
    const successAlert = document.getElementById('initialization-alert-success');
    const errorAlert = document.getElementById('initialization-alert-error');
    if (!successAlert || !errorAlert) {
        return;
    }

    if (type === 'success') {
        successAlert.textContent = message;
        successAlert.classList.remove('d-none');
        errorAlert.classList.add('d-none');
        return;
    }

    errorAlert.textContent = message;
    errorAlert.classList.remove('d-none');
    successAlert.classList.add('d-none');
}

function validateEmail(email: string): boolean {
    return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email);
}

function validatePasswordComplexity(password: string): boolean {
    if (password.length < 8) {
        return false;
    }

    const hasUpper = /[A-Z]/.test(password);
    const hasLower = /[a-z]/.test(password);
    const hasNumber = /\d/.test(password);
    const hasSpecial = /[^A-Za-z0-9]/.test(password);

    return hasUpper && hasLower && hasNumber && hasSpecial;
}

function setSubmitting(isSubmitting: boolean): void {
    const button = document.getElementById('initialization-submit') as HTMLButtonElement | null;
    if (!button) {
        return;
    }

    button.disabled = isSubmitting;
    button.innerHTML = isSubmitting
        ? '<span class="spinner-border spinner-border-sm me-2"></span>Initializing...'
        : '<i class="bi bi-play-circle"></i> Initialize Database';
}

function showLoginSection(): void {
    document.getElementById('initialization-login-section')?.classList.remove('d-none');
}

function hideForm(): void {
    document.getElementById('initialization-form')?.classList.add('d-none');
    document.getElementById('initialization-alert-info')?.classList.add('d-none');
}

function renderSetupResult(): void {
    const wrapper = document.getElementById('initialization-result');
    const summary = document.getElementById('initialization-done-summary');

    if (!wrapper || !summary) {
        return;
    }

    wrapper.classList.remove('d-none');
    summary.textContent = 'The first administrator account was created. Core code tables were also initialized as part of the same process.';
}

function scheduleLoginRedirect(seconds: number): void {
    showLoginSection();
    setTimeout(() => {
        window.location.href = '/login';
    }, seconds * 1000);
}

async function checkStatusAndPreparePage(): Promise<boolean> {
    const response = await request<InitializationStatusResponse>(`${getApiBaseUrl()}/Initialization/status`, { method: 'GET' });
    if (!response.ok || !response.data) {
        showMessage('error', response.message || 'Unable to verify initialization status.');
        return false;
    }

    const isInitialized = response.data.isInitialized ?? response.data.IsInitialized ?? false;
    if (isInitialized) {
        hideForm();
        showMessage('error', 'Database is already initialized. Redirecting to login...');
        scheduleLoginRedirect(5);
        return false;
    }

    return true;
}

async function submitInitialization(): Promise<void> {
    const usernameInput = document.getElementById('initialization-username') as HTMLInputElement | null;
    const emailInput = document.getElementById('initialization-email') as HTMLInputElement | null;
    const passwordInput = document.getElementById('initialization-password') as HTMLInputElement | null;
    const confirmInput = document.getElementById('initialization-password-confirm') as HTMLInputElement | null;

    const username = usernameInput?.value.trim() ?? '';
    const email = emailInput?.value.trim() ?? '';
    const password = passwordInput?.value ?? '';
    const confirmPassword = confirmInput?.value ?? '';

    if (!username) {
        showMessage('error', 'Username is required.');
        return;
    }

    if (!validateEmail(email)) {
        showMessage('error', 'Please enter a valid email address.');
        return;
    }

    if (password !== confirmPassword) {
        showMessage('error', 'Passwords do not match.');
        return;
    }

    if (!validatePasswordComplexity(password)) {
        showMessage('error', 'Password must be at least 8 characters and include uppercase, lowercase, number, and special character.');
        return;
    }

    setSubmitting(true);

    const response = await request<InitializationResponse>(`${getApiBaseUrl()}/Initialization/initialize`, {
        method: 'POST',
        body: JSON.stringify({ username, password, email }),
    });

    if (!response.ok) {
        const alreadyInitialized = (response.message || '').toLowerCase().includes('already exist');
        showMessage('error', response.message || 'Initialization failed.');

        if (alreadyInitialized) {
            hideForm();
            scheduleLoginRedirect(5);
        }

        setSubmitting(false);
        return;
    }

    hideForm();
    showMessage('success', 'Initialization completed successfully.');
    renderSetupResult();
    document.getElementById('initialization-proceed-info')?.classList.remove('d-none');
    showLoginSection();
    setSubmitting(false);
}

function initializePage(): void {
    const page = document.getElementById('initialization-page');
    if (!page || page.dataset.initialized === 'true') {
        return;
    }

    page.dataset.initialized = 'true';

    checkStatusAndPreparePage().then((canContinue) => {
        if (!canContinue) {
            return;
        }

        const form = document.getElementById('initialization-form') as HTMLFormElement | null;
        form?.addEventListener('submit', async (event) => {
            event.preventDefault();
            await submitInitialization();
        });
    });
}

function setupObserver(): void {
    initializePage();

    if (!document.body) {
        return;
    }

    const observer = new MutationObserver(() => {
        const page = document.getElementById('initialization-page');
        if (page && page.dataset.initialized !== 'true') {
            initializePage();
        }
    });

    observer.observe(document.body, { childList: true, subtree: true });
}

if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', setupObserver);
} else {
    setupObserver();
}
})();

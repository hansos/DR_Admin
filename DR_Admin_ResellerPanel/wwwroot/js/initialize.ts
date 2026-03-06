// @ts-nocheck
(function () {
interface InitializationStatusResponse {
    isInitialized?: boolean;
    IsInitialized?: boolean;
}

interface BuildModeResponse {
    isDebug?: boolean;
    IsDebug?: boolean;
}

interface AdminMyCompanyImportResponse {
    success?: boolean;
    Success?: boolean;
    errorMessage?: string;
    ErrorMessage?: string;
}

function showSeedDataSection(): void {
    document.getElementById('initialization-seed-data-section')?.classList.remove('d-none');
}

function setSeedDataSubmitting(isSubmitting: boolean): void {
    const button = document.getElementById('initialization-seed-data-button') as HTMLButtonElement | null;
    if (!button) {
        return;
    }

    button.disabled = isSubmitting;
    button.innerHTML = isSubmitting
        ? '<span class="spinner-border spinner-border-sm me-2"></span>Seeding...'
        : '<i class="bi bi-database-add"></i> Seed Test Data';
}

interface InitializationResponse {
    success?: boolean;
    message?: string;
    username?: string;
}

interface ResellerRuntimeConfigResponse {
    enableTestDataSeedingOnInitialize?: boolean;
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

function setDebugImportSubmitting(isSubmitting: boolean): void {
    const button = document.getElementById('initialization-debug-import-button') as HTMLButtonElement | null;
    if (!button) {
        return;
    }

    button.disabled = isSubmitting;
    button.innerHTML = isSubmitting
        ? '<span class="spinner-border spinner-border-sm me-2"></span>Importing...'
        : '<i class="bi bi-upload"></i> Import Snapshot';
}

async function setupDebugImportSection(): Promise<void> {
    const wrapper = document.getElementById('initialization-debug-import');
    if (!wrapper) {
        return;
    }

    const response = await request<BuildModeResponse>(`${getApiBaseUrl()}/Initialization/build-mode`, { method: 'GET' });
    const isDebug = response.ok && !!response.data && (response.data.isDebug ?? response.data.IsDebug ?? false);

    if (!isDebug) {
        wrapper.classList.add('d-none');
        return;
    }

    wrapper.classList.remove('d-none');
}

async function importDebugSnapshot(): Promise<void> {
    const fileInput = document.getElementById('initialization-debug-import-file') as HTMLInputElement | null;
    const fileName = fileInput?.value.trim() ?? '';

    if (!fileName) {
        showMessage('error', 'Snapshot file name is required for debug import.');
        return;
    }

    setDebugImportSubmitting(true);

    const response = await request<AdminMyCompanyImportResponse>(`${getApiBaseUrl()}/Initialization/import-admin-mycompany-snapshot`, {
        method: 'POST',
        body: JSON.stringify({ fileName }),
    });

    if (!response.ok || !response.data) {
        showMessage('error', response.message || 'Debug import failed.');
        setDebugImportSubmitting(false);
        return;
    }

    const success = response.data.success ?? response.data.Success ?? false;
    if (!success) {
        showMessage('error', response.data.errorMessage ?? response.data.ErrorMessage ?? 'Debug import failed.');
        setDebugImportSubmitting(false);
        return;
    }

    hideForm();
    showMessage('success', 'Snapshot imported successfully. Database is initialized.');
    renderSetupResult('Admin user and MyCompany were imported from debug snapshot.');
    showSeedDataSection();
    document.getElementById('initialization-proceed-info')?.classList.remove('d-none');
    showLoginSection();
    setDebugImportSubmitting(false);
}

async function getEnableSeedTestDataOnInitialize(): Promise<boolean> {
    const defaultValue = (window as any).AppSettings?.enableTestDataSeedingOnInitialize === true;

    try {
        const response = await fetch('/runtime-config', {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
            },
            credentials: 'include',
        });

        if (!response.ok) {
            return defaultValue;
        }

        const data = (await response.json()) as ResellerRuntimeConfigResponse;
        return data.enableTestDataSeedingOnInitialize === true;
    } catch {
        return defaultValue;
    }
}

function setupPasswordToggle(buttonId: string): void {
    const button = document.getElementById(buttonId) as HTMLButtonElement | null;
    if (!button || button.dataset.bound === 'true') {
        return;
    }

    const targetId = button.dataset.target;
    if (!targetId) {
        return;
    }

    const input = document.getElementById(targetId) as HTMLInputElement | null;
    if (!input) {
        return;
    }

    button.dataset.bound = 'true';
    button.addEventListener('click', () => {
        const shouldShow = input.type === 'password';
        input.type = shouldShow ? 'text' : 'password';
        button.innerHTML = shouldShow ? '<i class="bi bi-eye-slash"></i>' : '<i class="bi bi-eye"></i>';
    });
}

async function setupOptionalSeedDataCheckbox(): Promise<void> {
    const wrapper = document.getElementById('initialization-seed-test-data-wrapper');
    if (!wrapper) {
        return;
    }

    const isEnabled = await getEnableSeedTestDataOnInitialize();
    if (isEnabled) {
        wrapper.classList.remove('d-none');
        return;
    }

    wrapper.classList.add('d-none');
}

function hideForm(): void {
    document.getElementById('initialization-form')?.classList.add('d-none');
    document.getElementById('initialization-alert-info')?.classList.add('d-none');
}

function renderSetupResult(extraMessage?: string): void {
    const wrapper = document.getElementById('initialization-result');
    const summary = document.getElementById('initialization-done-summary');

    if (!wrapper || !summary) {
        return;
    }

    wrapper.classList.remove('d-none');
    const baseMessage = 'The first administrator account was created. Core code tables were also initialized as part of the same process.';
    summary.textContent = extraMessage ? `${baseMessage} ${extraMessage}` : baseMessage;
}

function scheduleLoginRedirect(seconds: number): void {
    showLoginSection();
    setTimeout(() => {
        window.location.href = '/login';
    }, seconds * 1000);
}

async function seedData(): Promise<void> {
    setSeedDataSubmitting(true);

    const response = await request<unknown>(`${getApiBaseUrl()}/Test/seed-data`, {
        method: 'POST',
    });

    if (!response.ok) {
        showMessage('error', response.message || 'Seeding test data failed.');
        setSeedDataSubmitting(false);
        return;
    }

    const message = typeof response.data === 'object' && response.data !== null
        ? ((response.data as any).message as string | undefined)
        : undefined;

    showMessage('success', message || 'Test data seeded successfully.');
    setSeedDataSubmitting(false);
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
    const companyNameInput = document.getElementById('initialization-company-name') as HTMLInputElement | null;
    const companyEmailInput = document.getElementById('initialization-company-email') as HTMLInputElement | null;
    const companyPhoneInput = document.getElementById('initialization-company-phone') as HTMLInputElement | null;
    const passwordInput = document.getElementById('initialization-password') as HTMLInputElement | null;
    const confirmInput = document.getElementById('initialization-password-confirm') as HTMLInputElement | null;

    const username = usernameInput?.value.trim() ?? '';
    const email = emailInput?.value.trim() ?? '';
    const companyName = companyNameInput?.value.trim() ?? '';
    const companyEmail = companyEmailInput?.value.trim() ?? '';
    const companyPhone = companyPhoneInput?.value.trim() ?? '';
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

    if (companyEmail && !validateEmail(companyEmail)) {
        showMessage('error', 'Please enter a valid company email address.');
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
        body: JSON.stringify({
            username,
            password,
            email,
            companyName,
            companyEmail,
            companyPhone,
        }),
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

    const seedTestDataCheckbox = document.getElementById('initialization-seed-test-data') as HTMLInputElement | null;
    const enableSeedTestData = await getEnableSeedTestDataOnInitialize();

    let seedWarningMessage = '';

    if (enableSeedTestData && seedTestDataCheckbox?.checked) {
        const seedResponse = await request<unknown>(`${getApiBaseUrl()}/Test/seed-data`, {
            method: 'POST',
        });

        if (!seedResponse.ok) {
            seedWarningMessage = seedResponse.message || 'Extended test data seeding failed. You can run it later from an authenticated admin session.';
        }
    }

    hideForm();
    showMessage('success', 'Initialization completed successfully.');
    renderSetupResult(seedWarningMessage);
    showSeedDataSection();
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

    setupPasswordToggle('initialization-toggle-password');
    setupPasswordToggle('initialization-toggle-password-confirm');
    setupOptionalSeedDataCheckbox();
    setupDebugImportSection();

    const seedDataButton = document.getElementById('initialization-seed-data-button') as HTMLButtonElement | null;
    seedDataButton?.addEventListener('click', async () => {
        await seedData();
    });

    const debugImportButton = document.getElementById('initialization-debug-import-button') as HTMLButtonElement | null;
    debugImportButton?.addEventListener('click', async () => {
        await importDebugSnapshot();
    });

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

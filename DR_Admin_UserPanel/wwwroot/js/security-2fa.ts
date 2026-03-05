((): void => {
interface AccountDto {
    id: number;
    username: string;
    email: string;
    emailConfirmed?: string | null;
}

interface TwoFactorStatusDto {
    enabled: boolean;
    method?: string | null;
    recoveryCodesRemaining?: number | null;
}

interface SecurityTwoFactorWindow extends Window {
    UserPanelSettings?: {
        frontendSiteCode: string;
    };
    UserPanelApi?: {
        request: <T>(path: string, options?: RequestInit, requiresAuth?: boolean) => Promise<{ success: boolean; data?: T; message?: string }>;
    };
    UserPanelAlerts?: {
        showSuccess: (id: string, message: string) => void;
        showError: (id: string, message: string) => void;
        hide: (id: string) => void;
    };
}

function initializeSecurityTwoFactorPage(): void {
    const page = document.getElementById('security-2fa-page');
    if (!page || page.dataset.initialized === 'true') {
        return;
    }

    page.dataset.initialized = 'true';

    document.getElementById('security-2fa-form')?.addEventListener('submit', (event) => {
        event.preventDefault();
        void saveTwoFactorStatus();
    });

    document.getElementById('security-2fa-send-verification')?.addEventListener('click', () => {
        void requestEmailVerification();
    });

    void loadSecurityTwoFactorPage();
}

async function loadSecurityTwoFactorPage(): Promise<void> {
    const typedWindow = window as SecurityTwoFactorWindow;
    typedWindow.UserPanelAlerts?.hide('security-2fa-alert-success');
    typedWindow.UserPanelAlerts?.hide('security-2fa-alert-error');

    const accountResponse = await typedWindow.UserPanelApi?.request<AccountDto>('/MyAccount/me', { method: 'GET' }, true);
    if (!accountResponse || !accountResponse.success || !accountResponse.data) {
        typedWindow.UserPanelAlerts?.showError('security-2fa-alert-error', accountResponse?.message ?? 'Could not load account security details.');
        return;
    }

    renderAccount(accountResponse.data);

    const statusResponse = await typedWindow.UserPanelApi?.request<TwoFactorStatusDto>('/MyAccount/2fa/status', { method: 'GET' }, true);
    if (!statusResponse || !statusResponse.success || !statusResponse.data) {
        renderTwoFactorStatus(null);
        return;
    }

    renderTwoFactorStatus(statusResponse.data);
}

function renderAccount(account: AccountDto): void {
    const summary = document.getElementById('security-2fa-account-summary');
    if (summary) {
        summary.innerHTML = `
            <div><strong>${escapeSecurityTwoFactorText(account.username)}</strong></div>
            <div>${escapeSecurityTwoFactorText(account.email)}</div>
            <div class="text-muted">User ID: ${account.id}</div>
        `;
    }

    const emailStatus = document.getElementById('security-2fa-email-status');
    if (emailStatus) {
        emailStatus.innerHTML = account.emailConfirmed
            ? '<span class="badge bg-success">Email verified</span>'
            : '<span class="badge bg-warning text-dark">Email not verified</span>';
    }

    const verifyButton = document.getElementById('security-2fa-send-verification');
    if (verifyButton) {
        verifyButton.classList.toggle('d-none', !!account.emailConfirmed);
    }
}

function renderTwoFactorStatus(status: TwoFactorStatusDto | null): void {
    const checkbox = document.getElementById('security-2fa-enabled') as HTMLInputElement | null;
    const statusBox = document.getElementById('security-2fa-status');

    if (!checkbox || !statusBox) {
        return;
    }

    if (!status) {
        checkbox.checked = false;
        statusBox.textContent = '2FA endpoint is not available yet for this environment.';
        return;
    }

    checkbox.checked = status.enabled;
    const recovery = status.recoveryCodesRemaining ?? 0;
    const method = status.method?.trim() ? status.method : 'Email';
    statusBox.textContent = status.enabled
        ? `2FA is enabled (${method}). Recovery codes remaining: ${recovery}.`
        : '2FA is currently disabled.';
}

async function requestEmailVerification(): Promise<void> {
    const typedWindow = window as SecurityTwoFactorWindow;
    typedWindow.UserPanelAlerts?.hide('security-2fa-alert-success');
    typedWindow.UserPanelAlerts?.hide('security-2fa-alert-error');

    const siteCode = typedWindow.UserPanelSettings?.frontendSiteCode ?? 'shop';
    const response = await typedWindow.UserPanelApi?.request('/MyAccount/request-email-confirmation', {
        method: 'POST',
        body: JSON.stringify({ siteCode })
    }, true);

    if (!response || !response.success) {
        typedWindow.UserPanelAlerts?.showError('security-2fa-alert-error', response?.message ?? 'Could not send verification email.');
        return;
    }

    typedWindow.UserPanelAlerts?.showSuccess('security-2fa-alert-success', response.message ?? 'Verification email sent.');
}

async function saveTwoFactorStatus(): Promise<void> {
    const typedWindow = window as SecurityTwoFactorWindow;
    typedWindow.UserPanelAlerts?.hide('security-2fa-alert-success');
    typedWindow.UserPanelAlerts?.hide('security-2fa-alert-error');

    const checkbox = document.getElementById('security-2fa-enabled') as HTMLInputElement | null;
    if (!checkbox) {
        return;
    }

    const response = await typedWindow.UserPanelApi?.request('/MyAccount/2fa', {
        method: 'POST',
        body: JSON.stringify({ enabled: checkbox.checked })
    }, true);

    if (!response || !response.success) {
        typedWindow.UserPanelAlerts?.showError('security-2fa-alert-error', response?.message ?? 'Could not update 2FA setting.');
        return;
    }

    typedWindow.UserPanelAlerts?.showSuccess('security-2fa-alert-success', 'Two-factor setting was updated.');
    await loadSecurityTwoFactorPage();
}

function escapeSecurityTwoFactorText(value: string): string {
    return value
        .replace(/&/g, '&amp;')
        .replace(/</g, '&lt;')
        .replace(/>/g, '&gt;')
        .replace(/"/g, '&quot;')
        .replace(/'/g, '&#039;');
}

function setupSecurityTwoFactorObserver(): void {
    initializeSecurityTwoFactorPage();

    const observer = new MutationObserver(() => {
        initializeSecurityTwoFactorPage();
    });

    observer.observe(document.body, { childList: true, subtree: true });
}

if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', setupSecurityTwoFactorObserver);
} else {
    setupSecurityTwoFactorObserver();
}
})();

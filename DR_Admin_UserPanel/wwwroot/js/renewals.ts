interface RenewalsWindow {
    UserPanelApi?: {
        request: <T>(path: string, options?: RequestInit, requiresAuth?: boolean) => Promise<{ success: boolean; data?: T; message?: string }>;
    };
    UserPanelAlerts?: {
        showError: (id: string, message: string) => void;
    };
}

function initializeRenewalsPage(): void {
    const page = document.getElementById('renewals-page');
    if (!page || page.dataset.bound === 'true') {
        return;
    }

    page.dataset.bound = 'true';
    void loadRenewalsSummary();
}

async function loadRenewalsSummary(): Promise<void> {
    const typedWindow = window as RenewalsWindow;
    const response = await typedWindow.UserPanelApi?.request<unknown[]>('/Subscriptions', { method: 'GET' }, true);

    if (!response || !response.success) {
        typedWindow.UserPanelAlerts?.showError('renewals-alert-error', response?.message ?? 'Could not load renewals.');
    }
}

if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initializeRenewalsPage);
} else {
    initializeRenewalsPage();
}

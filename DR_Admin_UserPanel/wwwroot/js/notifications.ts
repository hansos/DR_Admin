interface NotificationsWindow {
    UserPanelApi?: {
        request: <T>(path: string, options?: RequestInit, requiresAuth?: boolean) => Promise<{ success: boolean; data?: T; message?: string }>;
    };
    UserPanelAlerts?: {
        showError: (id: string, message: string) => void;
    };
}

function initializeNotificationsPage(): void {
    const page = document.getElementById('notifications-page');
    if (!page || page.dataset.bound === 'true') {
        return;
    }

    page.dataset.bound = 'true';
    void loadNotificationsSummary();
}

async function loadNotificationsSummary(): Promise<void> {
    const typedWindow = window as NotificationsWindow;
    const response = await typedWindow.UserPanelApi?.request<unknown[]>('/Orders', { method: 'GET' }, true);

    if (!response || !response.success) {
        typedWindow.UserPanelAlerts?.showError('notifications-alert-error', response?.message ?? 'Could not load notifications.');
    }
}

if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initializeNotificationsPage);
} else {
    initializeNotificationsPage();
}

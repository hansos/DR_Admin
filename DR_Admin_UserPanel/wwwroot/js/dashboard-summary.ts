interface OrderSummaryDto {
    id: number;
    orderNumber: string;
    status: string;
    serviceId: number | null;
    recurringAmount: number;
    nextBillingDate: string;
}

interface DashboardWindow extends Window {
    UserPanelApi?: {
        request: <T>(path: string, options?: RequestInit, requiresAuth?: boolean) => Promise<{ success: boolean; data?: T; message?: string }>;
    };
    UserPanelAlerts?: {
        showError: (id: string, message: string) => void;
    };
}

function initializeDashboard(): void {
    const page = document.getElementById('dashboard-summary-page');
    if (!page || page.dataset.bound === 'true') {
        return;
    }

    page.dataset.bound = 'true';
    void loadDashboard();
}

async function loadDashboard(): Promise<void> {
    const typedWindow = window as DashboardWindow;
    const response = await typedWindow.UserPanelApi?.request<OrderSummaryDto[]>('/Orders', { method: 'GET' }, true);

    if (!response || !response.success || !response.data) {
        typedWindow.UserPanelAlerts?.showError('dashboard-alert-error', response?.message ?? 'Could not load dashboard data.');
        return;
    }

    const orders = response.data;
    const activeDomains = orders.filter((order) => order.serviceId === null).length;
    const activeHosting = orders.filter((order) => order.serviceId !== null).length;

    setText('dashboard-active-domains', activeDomains.toString());
    setText('dashboard-active-hosting', activeHosting.toString());
    setText('dashboard-active-services', orders.length.toString());

    const renewals = document.getElementById('dashboard-renewals');
    if (!renewals) {
        return;
    }

    const items = orders
        .filter((order) => !!order.nextBillingDate)
        .sort((a, b) => Date.parse(a.nextBillingDate) - Date.parse(b.nextBillingDate))
        .slice(0, 5);

    if (items.length === 0) {
        renewals.textContent = 'No upcoming renewals.';
        return;
    }

    renewals.innerHTML = items
        .map((order) => `<div class="d-flex justify-content-between border-bottom py-2"><span>${order.orderNumber}</span><span>${formatDate(order.nextBillingDate)}</span></div>`)
        .join('');
}

function formatDate(value: string): string {
    const date = new Date(value);
    if (Number.isNaN(date.getTime())) {
        return '-';
    }

    return date.toLocaleDateString();
}

function setText(id: string, value: string): void {
    const element = document.getElementById(id);
    if (element) {
        element.textContent = value;
    }
}

if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initializeDashboard);
} else {
    initializeDashboard();
}

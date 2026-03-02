((): void => {
interface UserAccountDto {
    customer?: {
        id: number;
    } | null;
}

interface OrderDto {
    id: number;
    customerId: number;
    orderNumber: string;
    status: string;
    createdAt: string;
    updatedAt: string;
    recurringAmount: number;
    currencyCode: string;
}

interface SubscriptionDto {
    id: number;
    customerId: number;
    status: string;
    nextBillingDate: string;
    lastBillingAttempt?: string | null;
    createdAt: string;
    amount: number;
    currencyCode: string;
}

interface LoginHistoryDto {
    id: number;
    attemptedAt: string;
    isSuccessful: boolean;
    ipAddress: string;
}

interface ActivityItem {
    when: string;
    title: string;
    description: string;
    badge: string;
}

interface ActivityTimelineWindow extends Window {
    UserPanelApi?: {
        request: <T>(path: string, options?: RequestInit, requiresAuth?: boolean) => Promise<{ success: boolean; data?: T; message?: string }>;
    };
    UserPanelAlerts?: {
        showError: (id: string, message: string) => void;
        hide: (id: string) => void;
    };
}

function initializeActivityTimelinePage(): void {
    const page = document.getElementById('activity-timeline-page');
    if (!page || page.dataset.initialized === 'true') {
        return;
    }

    page.dataset.initialized = 'true';

    document.getElementById('activity-timeline-refresh')?.addEventListener('click', () => {
        void loadActivityTimeline();
    });

    void loadActivityTimeline();
}

async function loadActivityTimeline(): Promise<void> {
    const typedWindow = window as ActivityTimelineWindow;
    typedWindow.UserPanelAlerts?.hide('activity-timeline-alert-error');

    const customerId = await getActivityTimelineCustomerId();
    if (!customerId) {
        typedWindow.UserPanelAlerts?.showError('activity-timeline-alert-error', 'Could not resolve customer context.');
        renderActivityTimeline([]);
        return;
    }

    const [ordersResponse, subscriptionsResponse, loginsResponse] = await Promise.all([
        typedWindow.UserPanelApi?.request<OrderDto[]>('/Orders', { method: 'GET' }, true),
        typedWindow.UserPanelApi?.request<SubscriptionDto[]>(`/Subscriptions/customer/${customerId}`, { method: 'GET' }, true),
        typedWindow.UserPanelApi?.request<LoginHistoryDto[]>('/LoginHistories?pageNumber=1&pageSize=20', { method: 'GET' }, true)
    ]);

    const orderItems = (ordersResponse?.success && ordersResponse.data)
        ? ordersResponse.data.filter((x) => x.customerId === customerId).map(mapOrderToActivityItem)
        : [];

    const subscriptionItems = (subscriptionsResponse?.success && subscriptionsResponse.data)
        ? subscriptionsResponse.data.map(mapSubscriptionToActivityItem)
        : [];

    const loginItems = (loginsResponse?.success && loginsResponse.data)
        ? normalizeLoginItems(loginsResponse.data).map(mapLoginToActivityItem)
        : [];

    const timeline = [...orderItems, ...subscriptionItems, ...loginItems]
        .sort((a, b) => Date.parse(b.when) - Date.parse(a.when));

    if (timeline.length === 0) {
        typedWindow.UserPanelAlerts?.showError('activity-timeline-alert-error', 'No activity could be loaded from available endpoints.');
    }

    renderActivityTimeline(timeline);
}

async function getActivityTimelineCustomerId(): Promise<number | null> {
    const typedWindow = window as ActivityTimelineWindow;
    const response = await typedWindow.UserPanelApi?.request<UserAccountDto>('/MyAccount/me', { method: 'GET' }, true);
    const id = response?.data?.customer?.id;

    if (!response || !response.success || !id || id <= 0) {
        return null;
    }

    return id;
}

function normalizeLoginItems(payload: LoginHistoryDto[] | { items?: LoginHistoryDto[]; data?: LoginHistoryDto[] }): LoginHistoryDto[] {
    if (Array.isArray(payload)) {
        return payload;
    }

    if (Array.isArray(payload.items)) {
        return payload.items;
    }

    if (Array.isArray(payload.data)) {
        return payload.data;
    }

    return [];
}

function mapOrderToActivityItem(order: OrderDto): ActivityItem {
    return {
        when: order.updatedAt || order.createdAt,
        title: `Order ${escapeActivityTimelineText(order.orderNumber || `#${order.id}`)}`,
        description: `Status: ${escapeActivityTimelineText(order.status)} · ${order.recurringAmount.toFixed(2)} ${escapeActivityTimelineText(order.currencyCode)}`,
        badge: 'Order'
    };
}

function mapSubscriptionToActivityItem(subscription: SubscriptionDto): ActivityItem {
    return {
        when: subscription.lastBillingAttempt || subscription.nextBillingDate || subscription.createdAt,
        title: `Subscription #${subscription.id}`,
        description: `Status: ${escapeActivityTimelineText(subscription.status)} · Next billing: ${formatActivityTimelineDate(subscription.nextBillingDate)} · ${subscription.amount.toFixed(2)} ${escapeActivityTimelineText(subscription.currencyCode)}`,
        badge: 'Subscription'
    };
}

function mapLoginToActivityItem(login: LoginHistoryDto): ActivityItem {
    return {
        when: login.attemptedAt,
        title: login.isSuccessful ? 'Login succeeded' : 'Login failed',
        description: `IP: ${escapeActivityTimelineText(login.ipAddress)}`,
        badge: 'Security'
    };
}

function renderActivityTimeline(items: ActivityItem[]): void {
    const list = document.getElementById('activity-timeline-list');
    if (!list) {
        return;
    }

    if (items.length === 0) {
        list.innerHTML = '<div class="text-muted">No timeline entries available.</div>';
        return;
    }

    list.innerHTML = items.slice(0, 100).map((item) => `
        <div class="border-bottom py-2">
            <div class="d-flex justify-content-between align-items-start gap-2">
                <div>
                    <div class="fw-semibold">${item.title}</div>
                    <div class="small text-muted">${item.description}</div>
                </div>
                <div class="text-end">
                    <span class="badge bg-secondary">${escapeActivityTimelineText(item.badge)}</span>
                    <div class="small text-muted mt-1">${formatActivityTimelineDate(item.when)}</div>
                </div>
            </div>
        </div>
    `).join('');
}

function formatActivityTimelineDate(value: string): string {
    const date = new Date(value);
    if (Number.isNaN(date.getTime())) {
        return '-';
    }

    return `${date.toLocaleDateString()} ${date.toLocaleTimeString()}`;
}

function escapeActivityTimelineText(value: string): string {
    return value
        .replace(/&/g, '&amp;')
        .replace(/</g, '&lt;')
        .replace(/>/g, '&gt;')
        .replace(/"/g, '&quot;')
        .replace(/'/g, '&#039;');
}

function setupActivityTimelineObserver(): void {
    initializeActivityTimelinePage();

    const observer = new MutationObserver(() => {
        initializeActivityTimelinePage();
    });

    observer.observe(document.body, { childList: true, subtree: true });
}

if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', setupActivityTimelineObserver);
} else {
    setupActivityTimelineObserver();
}
})();

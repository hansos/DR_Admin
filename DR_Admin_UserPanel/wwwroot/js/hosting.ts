((): void => {
interface HostingAccountDto {
    id: number;
    username: string;
    status: string;
    planName?: string | null;
    expirationDate: string;
}

interface UserAccountDto {
    customer?: {
        id: number;
    } | null;
}

interface HostingWindow {
    UserPanelApi?: {
        request: <T>(path: string, options?: RequestInit, requiresAuth?: boolean) => Promise<{ success: boolean; data?: T; message?: string }>;
    };
    UserPanelAlerts?: {
        showError: (id: string, message: string) => void;
    };
}

function initializeHostingPage(): void {
    const page = document.getElementById('hosting-page');
    if (!page || page.dataset.initialized === 'true') {
        return;
    }

    page.dataset.initialized = 'true';
    void loadHostingAccounts();
}

async function loadHostingAccounts(): Promise<void> {
    const typedWindow = window as unknown as HostingWindow;
    const customerId = await resolveHostingCustomerId();
    if (!customerId) {
        typedWindow.UserPanelAlerts?.showError('hosting-alert-error', 'Could not resolve customer account.');
        renderHostingRows([]);
        return;
    }

    const response = await typedWindow.UserPanelApi?.request<HostingAccountDto[]>(`/HostingAccounts/customer/${customerId}`, { method: 'GET' }, true);

    if (!response || !response.success || !response.data) {
        typedWindow.UserPanelAlerts?.showError('hosting-alert-error', response?.message ?? 'Could not load hosting subscriptions.');
        renderHostingRows([]);
        return;
    }

    renderHostingRows(response.data);
}

async function resolveHostingCustomerId(): Promise<number | null> {
    const typedWindow = window as unknown as HostingWindow;
    const response = await typedWindow.UserPanelApi?.request<UserAccountDto>('/MyAccount/me', { method: 'GET' }, true);
    return response?.success ? (response.data?.customer?.id ?? null) : null;
}

function renderHostingRows(items: HostingAccountDto[]): void {
    const tableBody = document.getElementById('hosting-table-body');
    if (!tableBody) {
        return;
    }

    if (items.length === 0) {
        tableBody.innerHTML = '<tr><td colspan="6" class="text-center text-muted">No hosting subscriptions found.</td></tr>';
        return;
    }

    tableBody.innerHTML = items.map((item) => {
        const encodedId = encodeURIComponent(item.id.toString());

        return `<tr>
            <td>${item.id}</td>
            <td>${escapeHostingText(item.username)}</td>
            <td>${escapeHostingText(item.status)}</td>
            <td>${escapeHostingText(item.planName ?? '-')}</td>
            <td>${formatHostingDate(item.expirationDate)}</td>
            <td><a class="btn btn-outline-primary btn-sm" href="/hosting/details?id=${encodedId}">Details</a></td>
        </tr>`;
    }).join('');
}

function formatHostingDate(value: string): string {
    const date = new Date(value);
    if (Number.isNaN(date.getTime())) {
        return '-';
    }

    return date.toLocaleDateString();
}

function escapeHostingText(value: string): string {
    return value
        .replace(/&/g, '&amp;')
        .replace(/</g, '&lt;')
        .replace(/>/g, '&gt;')
        .replace(/"/g, '&quot;')
        .replace(/'/g, '&#039;');
}

function setupHostingObserver(): void {
    initializeHostingPage();

    const observer = new MutationObserver(() => {
        initializeHostingPage();
    });

    observer.observe(document.body, { childList: true, subtree: true });
}

if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', setupHostingObserver);
} else {
    setupHostingObserver();
}
})();

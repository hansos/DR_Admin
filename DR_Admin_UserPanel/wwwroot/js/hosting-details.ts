((): void => {
interface HostingDomainDto {
    id: number;
    domainName: string;
    sslEnabled: boolean;
    sslExpirationDate?: string | null;
}

interface HostingAccountDto {
    id: number;
    status: string;
    username: string;
    planName?: string | null;
    expirationDate: string;
    diskUsageMB?: number | null;
    diskQuotaMB?: number | null;
    bandwidthUsageMB?: number | null;
    bandwidthLimitMB?: number | null;
    domains?: HostingDomainDto[] | null;
}

interface HostingDetailsWindow extends Window {
    UserPanelApi?: {
        request: <T>(path: string, options?: RequestInit, requiresAuth?: boolean) => Promise<{ success: boolean; data?: T; message?: string }>;
    };
    UserPanelAlerts?: {
        showSuccess: (id: string, message: string) => void;
        showError: (id: string, message: string) => void;
    };
}

let currentHosting: HostingAccountDto | null = null;

function initializeHostingDetailsPage(): void {
    const page = document.getElementById('hosting-details-page');
    if (!page || page.dataset.initialized === 'true') {
        return;
    }

    page.dataset.initialized = 'true';

    document.getElementById('hosting-details-suspend')?.addEventListener('click', () => {
        void updateHostingStatus('Suspended');
    });

    document.getElementById('hosting-details-reactivate')?.addEventListener('click', () => {
        void updateHostingStatus('Active');
    });

    document.getElementById('hosting-details-upgrade')?.addEventListener('click', () => {
        void updateHostingPlan('upgrade');
    });

    document.getElementById('hosting-details-downgrade')?.addEventListener('click', () => {
        void updateHostingPlan('downgrade');
    });

    void loadHostingDetails();
}

async function loadHostingDetails(): Promise<void> {
    const typedWindow = window as HostingDetailsWindow;
    const id = getHostingDetailsId();

    if (!id) {
        typedWindow.UserPanelAlerts?.showError('hosting-details-alert-error', 'Missing or invalid hosting id.');
        return;
    }

    const response = await typedWindow.UserPanelApi?.request<HostingAccountDto>(`/HostingAccounts/${id}/details`, { method: 'GET' }, true);

    if (!response || !response.success || !response.data) {
        typedWindow.UserPanelAlerts?.showError('hosting-details-alert-error', response?.message ?? 'Could not load hosting details.');
        return;
    }

    currentHosting = response.data;
    renderHostingSummary(response.data);
    renderHostingResources(response.data);
}

function renderHostingSummary(item: HostingAccountDto): void {
    const summary = document.getElementById('hosting-details-summary');
    if (!summary) {
        return;
    }

    summary.innerHTML = `
        <div><strong>${escapeHostingDetailsText(item.username)}</strong></div>
        <div class="small text-muted">Status: ${escapeHostingDetailsText(item.status)}</div>
        <div class="small text-muted">Plan: ${escapeHostingDetailsText(item.planName ?? '-')} · Expires: ${formatHostingDetailsDate(item.expirationDate)}</div>
    `;
}

function renderHostingResources(item: HostingAccountDto): void {
    const box = document.getElementById('hosting-details-resources');
    if (!box) {
        return;
    }

    const sslLines = (item.domains ?? []).map((domain) => {
        const sslState = domain.sslEnabled ? `Enabled${domain.sslExpirationDate ? ` until ${formatHostingDetailsDate(domain.sslExpirationDate)}` : ''}` : 'Disabled';
        return `<div>${escapeHostingDetailsText(domain.domainName)}: ${escapeHostingDetailsText(sslState)}</div>`;
    });

    box.innerHTML = `
        <div>Disk: ${item.diskUsageMB ?? 0} / ${item.diskQuotaMB ?? 0} MB</div>
        <div>Bandwidth: ${item.bandwidthUsageMB ?? 0} / ${item.bandwidthLimitMB ?? 0} MB</div>
        <div class="mt-2 fw-semibold">SSL status</div>
        <div>${sslLines.length > 0 ? sslLines.join('') : '<span class="text-muted">No hosted domains found.</span>'}</div>
    `;
}

async function updateHostingStatus(status: string): Promise<void> {
    if (!currentHosting) {
        return;
    }

    const typedWindow = window as HostingDetailsWindow;
    const response = await typedWindow.UserPanelApi?.request<HostingAccountDto>(`/HostingAccounts/${currentHosting.id}`, {
        method: 'PUT',
        body: JSON.stringify({ status })
    }, true);

    if (!response || !response.success || !response.data) {
        typedWindow.UserPanelAlerts?.showError('hosting-details-alert-error', response?.message ?? 'Could not update hosting status.');
        return;
    }

    currentHosting = response.data;
    renderHostingSummary(response.data);
    typedWindow.UserPanelAlerts?.showSuccess('hosting-details-alert-success', `Hosting status updated to ${status}.`);
}

async function updateHostingPlan(direction: 'upgrade' | 'downgrade'): Promise<void> {
    if (!currentHosting) {
        return;
    }

    const typedWindow = window as HostingDetailsWindow;
    const currentPlan = currentHosting.planName ?? 'Standard';
    const nextPlan = direction === 'upgrade' ? `${currentPlan} Plus` : currentPlan.replace(' Plus', '');

    const response = await typedWindow.UserPanelApi?.request<HostingAccountDto>(`/HostingAccounts/${currentHosting.id}`, {
        method: 'PUT',
        body: JSON.stringify({ planName: nextPlan })
    }, true);

    if (!response || !response.success || !response.data) {
        typedWindow.UserPanelAlerts?.showError('hosting-details-alert-error', response?.message ?? 'Could not update plan.');
        return;
    }

    currentHosting = response.data;
    renderHostingSummary(response.data);
    typedWindow.UserPanelAlerts?.showSuccess('hosting-details-alert-success', `Hosting plan updated to ${nextPlan}.`);
}

function getHostingDetailsId(): number | null {
    const text = new URLSearchParams(window.location.search).get('id');
    if (!text) {
        return null;
    }

    const id = Number.parseInt(text, 10);
    return Number.isNaN(id) || id <= 0 ? null : id;
}

function formatHostingDetailsDate(value: string): string {
    const date = new Date(value);
    if (Number.isNaN(date.getTime())) {
        return '-';
    }

    return date.toLocaleDateString();
}

function escapeHostingDetailsText(value: string): string {
    return value
        .replace(/&/g, '&amp;')
        .replace(/</g, '&lt;')
        .replace(/>/g, '&gt;')
        .replace(/"/g, '&quot;')
        .replace(/'/g, '&#039;');
}

function setupHostingDetailsObserver(): void {
    initializeHostingDetailsPage();

    const observer = new MutationObserver(() => {
        initializeHostingDetailsPage();
    });

    observer.observe(document.body, { childList: true, subtree: true });
}

if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', setupHostingDetailsObserver);
} else {
    setupHostingDetailsObserver();
}
})();

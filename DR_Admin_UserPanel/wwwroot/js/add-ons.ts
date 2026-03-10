((): void => {
interface SoldOptionalServiceDto {
    id: number;
    serviceId: number;
    serviceName: string;
    connectedDomainName: string;
    status: string;
    billingCycle: string;
    quantity: number;
    totalPrice: number;
    currencyCode: string;
    nextBillingDate: string;
    autoRenew: boolean;
}

interface UserAccountDto {
    customer?: {
        id: number;
    } | null;
}

interface AddOnsWindow {
    UserPanelApi?: {
        request: <T>(path: string, options?: RequestInit, requiresAuth?: boolean) => Promise<{ success: boolean; data?: T; message?: string }>;
    };
    UserPanelAlerts?: {
        showError: (id: string, message: string) => void;
        hide: (id: string) => void;
    };
}

let addOnsPageNumber = 1;
let addOnsPageSize = 25;
let addOnsTotalItems = 0;

function initializeAddOnsPage(): void {
    const page = document.getElementById('services-addons-page');
    if (!page || page.dataset.initialized === 'true') {
        return;
    }

    page.dataset.initialized = 'true';

    document.getElementById('services-addons-apply')?.addEventListener('click', () => {
        addOnsPageNumber = 1;
        void loadAddOns();
    });

    document.getElementById('services-addons-prev')?.addEventListener('click', () => {
        if (addOnsPageNumber > 1) {
            addOnsPageNumber -= 1;
            void loadAddOns();
        }
    });

    document.getElementById('services-addons-next')?.addEventListener('click', () => {
        if (addOnsPageNumber * addOnsPageSize < addOnsTotalItems) {
            addOnsPageNumber += 1;
            void loadAddOns();
        }
    });

    document.getElementById('services-addons-page-size')?.addEventListener('change', () => {
        addOnsPageSize = Number.parseInt(readAddOnsInputValue('services-addons-page-size'), 10) || 25;
        addOnsPageNumber = 1;
        void loadAddOns();
    });

    void loadAddOns();
}

async function loadAddOns(): Promise<void> {
    const typedWindow = window as unknown as AddOnsWindow;
    typedWindow.UserPanelAlerts?.hide('services-addons-alert-error');

    addOnsPageSize = Number.parseInt(readAddOnsInputValue('services-addons-page-size'), 10) || 25;

    const customerId = await resolveAddOnsCustomerId();
    if (!customerId) {
        typedWindow.UserPanelAlerts?.showError('services-addons-alert-error', 'Could not resolve customer account.');
        renderAddOnsRows([]);
        setAddOnsPaginationInfo(0);
        return;
    }

    const response = await typedWindow.UserPanelApi?.request<SoldOptionalServiceDto[]>(`/SoldOptionalServices/customer/${customerId}`, { method: 'GET' }, true);

    if (!response || !response.success || !response.data) {
        typedWindow.UserPanelAlerts?.showError('services-addons-alert-error', response?.message ?? 'Could not load add-ons and services.');
        renderAddOnsRows([]);
        setAddOnsPaginationInfo(0);
        return;
    }

    const filtered = applyAddOnsFilters(response.data);
    addOnsTotalItems = filtered.length;
    const paged = getAddOnsPageSlice(filtered);

    renderAddOnsRows(paged);
    setAddOnsPaginationInfo(paged.length);
}

async function resolveAddOnsCustomerId(): Promise<number | null> {
    const typedWindow = window as unknown as AddOnsWindow;
    const response = await typedWindow.UserPanelApi?.request<UserAccountDto>('/MyAccount/me', { method: 'GET' }, true);
    return response?.success ? (response.data?.customer?.id ?? null) : null;
}

function applyAddOnsFilters(items: SoldOptionalServiceDto[]): SoldOptionalServiceDto[] {
    const status = readAddOnsInputValue('services-addons-filter-status').toLowerCase();
    const cycle = readAddOnsInputValue('services-addons-filter-cycle').toLowerCase();

    return items.filter((item) => {
        const statusMatch = !status || item.status.toLowerCase().includes(status);
        const cycleMatch = !cycle || item.billingCycle.toLowerCase().includes(cycle);
        return statusMatch && cycleMatch;
    });
}

function getAddOnsPageSlice(items: SoldOptionalServiceDto[]): SoldOptionalServiceDto[] {
    const start = (addOnsPageNumber - 1) * addOnsPageSize;
    return items.slice(start, start + addOnsPageSize);
}

function renderAddOnsRows(items: SoldOptionalServiceDto[]): void {
    const tableBody = document.getElementById('services-addons-table-body');
    if (!tableBody) {
        return;
    }

    if (items.length === 0) {
        tableBody.innerHTML = '<tr><td colspan="9" class="text-center text-muted">No add-ons or services found.</td></tr>';
        return;
    }

    tableBody.innerHTML = items.map((item) => {
        return `<tr>
            <td>${item.id}</td>
            <td>${escapeAddOnsText(item.serviceName || `#${item.serviceId}`)}</td>
            <td>${escapeAddOnsText(item.connectedDomainName || '-')}</td>
            <td>${escapeAddOnsText(item.status)}</td>
            <td>${escapeAddOnsText(item.billingCycle)}</td>
            <td>${item.quantity}</td>
            <td>${formatAddOnsMoney(item.totalPrice, item.currencyCode)}</td>
            <td>${formatAddOnsDate(item.nextBillingDate)}</td>
            <td>${item.autoRenew ? 'Yes' : 'No'}</td>
        </tr>`;
    }).join('');
}

function setAddOnsPaginationInfo(count: number): void {
    const info = document.getElementById('services-addons-pagination-info');
    if (!info) {
        return;
    }

    info.textContent = `Page ${addOnsPageNumber} · Showing ${count} of ${addOnsTotalItems} item(s)`;
}

function readAddOnsInputValue(id: string): string {
    const input = document.getElementById(id) as HTMLInputElement | HTMLSelectElement | null;
    return input?.value.trim() ?? '';
}

function formatAddOnsDate(value: string): string {
    const date = new Date(value);
    if (Number.isNaN(date.getTime())) {
        return '-';
    }

    return date.toLocaleDateString();
}

function formatAddOnsMoney(value: number, currencyCode: string): string {
    try {
        return new Intl.NumberFormat(undefined, { style: 'currency', currency: currencyCode }).format(value);
    } catch {
        return `${value.toFixed(2)} ${currencyCode}`;
    }
}

function escapeAddOnsText(value: string): string {
    return value
        .replace(/&/g, '&amp;')
        .replace(/</g, '&lt;')
        .replace(/>/g, '&gt;')
        .replace(/"/g, '&quot;')
        .replace(/'/g, '&#039;');
}

function setupAddOnsObserver(): void {
    initializeAddOnsPage();

    const observer = new MutationObserver(() => {
        initializeAddOnsPage();
    });

    observer.observe(document.body, { childList: true, subtree: true });
}

if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', setupAddOnsObserver);
} else {
    setupAddOnsObserver();
}
})();

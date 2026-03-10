((): void => {
interface OrderLineDto {
    id: number;
    lineNumber: number;
    description: string;
    quantity: number;
    unitPrice: number;
    totalPrice: number;
    isRecurring: boolean;
    notes: string;
}

interface OrderDto {
    id: number;
    orderNumber: string;
    customerId: number;
    orderType: string | number;
    status: string | number;
    recurringAmount: number;
    currencyCode: string;
    createdAt: string;
    nextBillingDate: string;
    orderLines: OrderLineDto[];
}

interface UserAccountDto {
    customer?: { id: number } | null;
}

interface PagedResult<T> {
    items?: T[];
    data?: T[];
}

interface OrdersWindow extends Window {
    UserPanelApi?: {
        request: <T>(path: string, options?: RequestInit, requiresAuth?: boolean) => Promise<{ success: boolean; data?: T; message?: string }>;
    };
    UserPanelAlerts?: {
        showError: (id: string, message: string) => void;
    };
}

let ordersCustomerId: number | null = null;
let ordersPageNumber = 1;
const ordersPageSize = 10;
let ordersFiltered: OrderDto[] = [];

function initializeOrdersPage(): void {
    const page = document.getElementById('orders-page');
    if (!page || page.dataset.initialized === 'true') {
        return;
    }

    page.dataset.initialized = 'true';

    document.getElementById('orders-prev')?.addEventListener('click', () => {
        if (ordersPageNumber > 1) {
            ordersPageNumber -= 1;
            renderOrdersPage();
        }
    });

    document.getElementById('orders-next')?.addEventListener('click', () => {
        if (ordersPageNumber * ordersPageSize < ordersFiltered.length) {
            ordersPageNumber += 1;
            renderOrdersPage();
        }
    });

    document.getElementById('orders-table-body')?.addEventListener('click', (event: Event) => {
        const target = event.target as HTMLElement;
        const button = target.closest('button[data-id]') as HTMLButtonElement | null;
        if (!button) {
            return;
        }

        const id = Number.parseInt(button.dataset.id ?? '', 10);
        if (!Number.isNaN(id) && id > 0) {
            void loadOrderDetails(id);
        }
    });

    void loadOrders();
}

async function loadOrders(): Promise<void> {
    const typedWindow = window as OrdersWindow;
    ordersCustomerId = await resolveOrdersCustomerId();

    if (!ordersCustomerId) {
        typedWindow.UserPanelAlerts?.showError('orders-alert-error', 'Could not resolve customer account.');
        ordersFiltered = [];
        renderOrdersPage();
        return;
    }

    const response = await typedWindow.UserPanelApi?.request<PagedResult<OrderDto> | OrderDto[]>('/Orders', { method: 'GET' }, true);

    if (!response || !response.success || !response.data) {
        typedWindow.UserPanelAlerts?.showError('orders-alert-error', response?.message ?? 'Could not load orders.');
        ordersFiltered = [];
        renderOrdersPage();
        return;
    }

    const items = normalizeOrders(response.data);
    ordersFiltered = items
        .filter((item) => item.customerId === ordersCustomerId)
        .sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime());

    ordersPageNumber = 1;
    renderOrdersPage();
}

function normalizeOrders(payload: PagedResult<OrderDto> | OrderDto[]): OrderDto[] {
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

function renderOrdersPage(): void {
    const start = (ordersPageNumber - 1) * ordersPageSize;
    const pageItems = ordersFiltered.slice(start, start + ordersPageSize);
    renderOrdersRows(pageItems);
    renderOrdersPageInfo(pageItems.length);
}

function renderOrdersRows(items: OrderDto[]): void {
    const tableBody = document.getElementById('orders-table-body');
    if (!tableBody) {
        return;
    }

    if (items.length === 0) {
        tableBody.innerHTML = '<tr><td colspan="7" class="text-center text-muted">No orders found.</td></tr>';
        return;
    }

    tableBody.innerHTML = items.map((item) => `
        <tr>
            <td>${escapeOrdersText(item.orderNumber)}</td>
            <td>${escapeOrdersText(String(item.orderType))}</td>
            <td>${escapeOrdersText(String(item.status))}</td>
            <td>${formatOrdersDate(item.createdAt)}</td>
            <td>${formatOrdersDate(item.nextBillingDate)}</td>
            <td>${formatOrdersMoney(item.recurringAmount)} ${escapeOrdersText(item.currencyCode || 'EUR')}</td>
            <td><button class="btn btn-outline-primary btn-sm" type="button" data-id="${item.id}">View lines</button></td>
        </tr>
    `).join('');
}

function renderOrdersPageInfo(count: number): void {
    const info = document.getElementById('orders-pagination-info');
    if (info) {
        info.textContent = `Page ${ordersPageNumber} · Showing ${count} item(s) · Total ${ordersFiltered.length}`;
    }
}

async function loadOrderDetails(id: number): Promise<void> {
    const typedWindow = window as OrdersWindow;
    const response = await typedWindow.UserPanelApi?.request<OrderDto>(`/Orders/${id}`, { method: 'GET' }, true);

    if (!response || !response.success || !response.data) {
        typedWindow.UserPanelAlerts?.showError('orders-alert-error', response?.message ?? 'Could not load order details.');
        return;
    }

    if (ordersCustomerId && response.data.customerId !== ordersCustomerId) {
        typedWindow.UserPanelAlerts?.showError('orders-alert-error', 'Order does not belong to current customer.');
        return;
    }

    const card = document.getElementById('orders-details-card');
    const body = document.getElementById('orders-details-body');
    if (!card || !body) {
        return;
    }

    const lines = response.data.orderLines ?? [];
    const rows = lines.length === 0
        ? '<tr><td colspan="6" class="text-center text-muted">No order lines found.</td></tr>'
        : lines.map((line) => `
            <tr>
                <td>${line.lineNumber}</td>
                <td>${escapeOrdersText(line.description)}</td>
                <td>${line.quantity}</td>
                <td>${formatOrdersMoney(line.unitPrice)}</td>
                <td>${formatOrdersMoney(line.totalPrice)}</td>
                <td>${line.isRecurring ? 'Yes' : 'No'}</td>
            </tr>
        `).join('');

    card.classList.remove('d-none');
    body.innerHTML = `
        <div class="mb-3">
            <div><strong>${escapeOrdersText(response.data.orderNumber)}</strong></div>
            <div>Status: ${escapeOrdersText(String(response.data.status))}</div>
            <div>Type: ${escapeOrdersText(String(response.data.orderType))}</div>
            <div>Created: ${formatOrdersDate(response.data.createdAt)}</div>
        </div>
        <div class="table-responsive">
            <table class="table table-sm align-middle mb-0">
                <thead>
                    <tr>
                        <th>Line</th>
                        <th>Description</th>
                        <th>Qty</th>
                        <th>Unit</th>
                        <th>Total</th>
                        <th>Recurring</th>
                    </tr>
                </thead>
                <tbody>
                    ${rows}
                </tbody>
            </table>
        </div>
    `;
}

async function resolveOrdersCustomerId(): Promise<number | null> {
    const typedWindow = window as OrdersWindow;
    const response = await typedWindow.UserPanelApi?.request<UserAccountDto>('/MyAccount/me', { method: 'GET' }, true);
    return response?.success ? (response.data?.customer?.id ?? null) : null;
}

function formatOrdersDate(value: string): string {
    const date = new Date(value);
    if (Number.isNaN(date.getTime())) {
        return '-';
    }

    return date.toLocaleDateString();
}

function formatOrdersMoney(value: number): string {
    return Number.isFinite(value) ? value.toFixed(2) : '0.00';
}

function escapeOrdersText(value: string): string {
    return value
        .replace(/&/g, '&amp;')
        .replace(/</g, '&lt;')
        .replace(/>/g, '&gt;')
        .replace(/"/g, '&quot;')
        .replace(/'/g, '&#039;');
}

function setupOrdersObserver(): void {
    initializeOrdersPage();

    const observer = new MutationObserver(() => {
        initializeOrdersPage();
    });

    observer.observe(document.body, { childList: true, subtree: true });
}

if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', setupOrdersObserver);
} else {
    setupOrdersObserver();
}
})();

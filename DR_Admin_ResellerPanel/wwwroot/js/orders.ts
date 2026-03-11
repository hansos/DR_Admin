(() => {
    interface AppSettings {
        apiBaseUrl?: string;
    }

    interface ApiResponse<T> {
        success: boolean;
        data?: T;
        message?: string;
    }

    interface ApiErrorBody {
        message?: string;
        title?: string;
    }

    interface ApiEnvelope<T> {
        success?: boolean;
        data?: T;
        message?: string;
    }

    interface Order {
        id: number;
        orderNumber: string;
        customerId: number;
        orderType: string;
        status: string;
        recurringAmount: number;
        currencyCode: string;
        createdAt: string;
        nextBillingDate: string;
        orderLines: OrderLine[];
    }

    interface OrderLine {
        id: number;
        lineNumber: number;
        description: string;
        quantity: number;
        unitPrice: number;
        totalPrice: number;
        isRecurring: boolean;
        notes: string;
    }

    const orderStatusLabels: Record<number, string> = {
        0: 'Pending',
        1: 'Active',
        2: 'Suspended',
        3: 'Cancelled',
        4: 'Expired',
        5: 'Trial',
    };

    const orderTypeLabels: Record<number, string> = {
        0: 'New',
        1: 'Renewal',
        2: 'Upgrade',
        3: 'Downgrade',
        4: 'Addon',
    };

    let allOrders: Order[] = [];
    let filteredOrders: Order[] = [];

    let currentPage = 1;
    let pageSize = 25;
    let totalCount = 0;
    let totalPages = 1;
    let selectedOrderId: number | null = null;

    const getApiBaseUrl = (): string => {
        const settings = (window as Window & { AppSettings?: AppSettings }).AppSettings;
        return settings?.apiBaseUrl ?? '';
    };

    const getAuthToken = (): string | null => {
        const auth = (window as Window & { Auth?: { getToken?: () => string | null } }).Auth;
        if (auth?.getToken) {
            return auth.getToken();
        }

        return sessionStorage.getItem('rp_authToken');
    };

    const esc = (text: string): string => {
        const map: Record<string, string> = { '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#039;' };
        return (text || '').replace(/[&<>"']/g, (char) => map[char] ?? char);
    };

    const parseString = (value: unknown): string => typeof value === 'string' ? value : '';

    const parseNumber = (value: unknown): number => {
        if (typeof value === 'number' && Number.isFinite(value)) {
            return value;
        }

        if (typeof value === 'string' && value.trim() !== '') {
            const parsed = Number(value);
            if (Number.isFinite(parsed)) {
                return parsed;
            }
        }

        return 0;
    };

    const normalizeEnum = (value: unknown, labels: Record<number, string>): string => {
        if (typeof value === 'string' && value.trim() !== '') {
            return value;
        }

        const numeric = parseNumber(value);
        return labels[numeric] ?? String(numeric);
    };

    const formatDate = (value: string): string => {
        if (!value) {
            return '-';
        }

        const date = new Date(value);
        if (Number.isNaN(date.getTime())) {
            return value;
        }

        return date.toLocaleString();
    };

    const formatMoney = (amount: number, currencyCode: string): string => {
        if (!Number.isFinite(amount)) {
            return '-';
        }

        try {
            return new Intl.NumberFormat(undefined, {
                style: 'currency',
                currency: currencyCode || 'EUR',
                minimumFractionDigits: 2,
                maximumFractionDigits: 2,
            }).format(amount);
        } catch {
            return `${amount.toFixed(2)} ${currencyCode || 'EUR'}`;
        }
    };

    const apiRequest = async <T,>(endpoint: string, options: RequestInit = {}): Promise<ApiResponse<T>> => {
        try {
            const headers: Record<string, string> = {
                'Content-Type': 'application/json',
                ...(options.headers as Record<string, string> | undefined),
            };

            const token = getAuthToken();
            if (token) {
                headers.Authorization = `Bearer ${token}`;
            }

            const response = await fetch(endpoint, {
                ...options,
                headers,
                credentials: 'include',
            });

            const contentType = response.headers.get('content-type') ?? '';
            const hasJson = contentType.includes('application/json');
            const body = hasJson ? (await response.json() as unknown) : null;

            if (!response.ok) {
                const errorBody = (body ?? {}) as ApiErrorBody;
                return {
                    success: false,
                    message: errorBody.message ?? errorBody.title ?? `Request failed with status ${response.status}`,
                };
            }

            const envelope = (body ?? {}) as ApiEnvelope<T>;
            return {
                success: envelope.success !== false,
                data: envelope.data ?? (body as T),
                message: envelope.message,
            };
        } catch (error) {
            console.error('Orders request failed', error);
            return {
                success: false,
                message: 'Network error. Please try again.',
            };
        }
    };

    const extractOrders = (payload: unknown): unknown[] => {
        if (Array.isArray(payload)) {
            return payload;
        }

        const objectPayload = payload as { data?: unknown; Data?: unknown } | null;
        if (Array.isArray(objectPayload?.data)) {
            return objectPayload.data;
        }

        if (Array.isArray(objectPayload?.Data)) {
            return objectPayload.Data;
        }

        const nestedData = objectPayload?.data as { data?: unknown; Data?: unknown } | undefined;
        if (Array.isArray(nestedData?.data)) {
            return nestedData.data;
        }

        if (Array.isArray(nestedData?.Data)) {
            return nestedData.Data;
        }

        return [];
    };

    const normalizeOrder = (item: unknown): Order => {
        const row = (item ?? {}) as Record<string, unknown>;
        const orderLinesRaw = row.orderLines ?? row.OrderLines;
        const orderLines = Array.isArray(orderLinesRaw)
            ? orderLinesRaw.map((line) => normalizeOrderLine(line))
            : [];

        return {
            id: parseNumber(row.id ?? row.Id),
            orderNumber: parseString(row.orderNumber ?? row.OrderNumber),
            customerId: parseNumber(row.customerId ?? row.CustomerId),
            orderType: normalizeEnum(row.orderType ?? row.OrderType, orderTypeLabels),
            status: normalizeEnum(row.status ?? row.Status, orderStatusLabels),
            recurringAmount: parseNumber(row.recurringAmount ?? row.RecurringAmount),
            currencyCode: parseString(row.currencyCode ?? row.CurrencyCode) || 'EUR',
            createdAt: parseString(row.createdAt ?? row.CreatedAt),
            nextBillingDate: parseString(row.nextBillingDate ?? row.NextBillingDate),
            orderLines,
        };
    };

    const normalizeOrderLine = (item: unknown): OrderLine => {
        const row = (item ?? {}) as Record<string, unknown>;
        return {
            id: parseNumber(row.id ?? row.Id),
            lineNumber: parseNumber(row.lineNumber ?? row.LineNumber),
            description: parseString(row.description ?? row.Description),
            quantity: parseNumber(row.quantity ?? row.Quantity),
            unitPrice: parseNumber(row.unitPrice ?? row.UnitPrice),
            totalPrice: parseNumber(row.totalPrice ?? row.TotalPrice),
            isRecurring: Boolean(row.isRecurring ?? row.IsRecurring ?? false),
            notes: parseString(row.notes ?? row.Notes),
        };
    };

    const renderOrderLinesPanel = (order: Order): string => {
        if (!order.orderLines.length) {
            return `
                <div class="small text-muted py-2">No order lines found for this order.</div>
            `;
        }

        const lines = [...order.orderLines].sort((a, b) => a.lineNumber - b.lineNumber || a.id - b.id);
        return `
            <div class="py-2">
                <div class="fw-semibold mb-2">Order lines</div>
                <div class="table-responsive">
                    <table class="table table-sm mb-0">
                        <thead>
                            <tr>
                                <th>Line</th>
                                <th>Description</th>
                                <th>Qty</th>
                                <th>Recurring</th>
                                <th class="text-end">Unit</th>
                                <th class="text-end">Total</th>
                            </tr>
                        </thead>
                        <tbody>
                            ${lines.map((line) => `
                                <tr>
                                    <td>${line.lineNumber || '-'}</td>
                                    <td>${esc(line.description || '-')}</td>
                                    <td>${line.quantity}</td>
                                    <td>${line.isRecurring ? '<span class="badge bg-info text-dark">Yes</span>' : '<span class="badge bg-secondary">No</span>'}</td>
                                    <td class="text-end">${esc(formatMoney(line.unitPrice, order.currencyCode))}</td>
                                    <td class="text-end">${esc(formatMoney(line.totalPrice, order.currencyCode))}</td>
                                </tr>
                            `).join('')}
                        </tbody>
                    </table>
                </div>
            </div>
        `;
    };

    const showError = (message: string): void => {
        const alert = document.getElementById('orders-alert-error');
        if (!alert) {
            return;
        }

        alert.textContent = message;
        alert.classList.remove('d-none');
        document.getElementById('orders-alert-success')?.classList.add('d-none');
    };

    const hideError = (): void => {
        document.getElementById('orders-alert-error')?.classList.add('d-none');
    };

    const loadPageSizeFromUi = (): void => {
        const select = document.getElementById('orders-page-size') as HTMLSelectElement | null;
        const parsed = Number(select?.value ?? '25');
        if (Number.isFinite(parsed) && parsed > 0) {
            pageSize = parsed;
        }
    };

    const getFilterValue = (id: string): string => {
        const element = document.getElementById(id) as HTMLInputElement | HTMLSelectElement | null;
        return (element?.value ?? '').trim();
    };

    const applyFilters = (): void => {
        const status = getFilterValue('orders-filter-status');
        const type = getFilterValue('orders-filter-type');
        const customerIdRaw = getFilterValue('orders-filter-customerid');
        const customerId = customerIdRaw ? Number(customerIdRaw) : null;

        filteredOrders = allOrders.filter((order) => {
            if (status && order.status !== status) {
                return false;
            }

            if (type && order.orderType !== type) {
                return false;
            }

            if (customerId !== null && Number.isFinite(customerId) && order.customerId !== customerId) {
                return false;
            }

            return true;
        });

        currentPage = 1;
        updateView();
    };

    const resetFilters = (): void => {
        const status = document.getElementById('orders-filter-status') as HTMLSelectElement | null;
        const type = document.getElementById('orders-filter-type') as HTMLSelectElement | null;
        const customerId = document.getElementById('orders-filter-customerid') as HTMLInputElement | null;

        if (status) {
            status.value = '';
        }

        if (type) {
            type.value = '';
        }

        if (customerId) {
            customerId.value = '';
        }

        filteredOrders = [...allOrders];
        currentPage = 1;
        updateView();
    };

    const getPagedOrders = (): Order[] => {
        totalCount = filteredOrders.length;
        totalPages = Math.max(1, Math.ceil(totalCount / pageSize));
        if (currentPage > totalPages) {
            currentPage = totalPages;
        }

        const start = (currentPage - 1) * pageSize;
        return filteredOrders.slice(start, start + pageSize);
    };

    const renderTable = (): void => {
        const tableBody = document.getElementById('orders-table-body');
        if (!tableBody) {
            return;
        }

        const paged = getPagedOrders();
        if (!paged.length) {
            tableBody.innerHTML = '<tr><td colspan="8" class="text-center text-muted">No orders found.</td></tr>';
            return;
        }

        tableBody.innerHTML = paged.map((order) => {
            const statusClass = order.status === 'Active'
                ? 'success'
                : order.status === 'Pending' || order.status === 'Trial'
                    ? 'warning text-dark'
                    : order.status === 'Cancelled'
                        ? 'danger'
                        : 'secondary';

            const isSelected = selectedOrderId === order.id;
            const selectedClass = isSelected ? 'orders-row-selected' : '';

            return `
                <tr data-order-id="${order.id}" class="${selectedClass}">
                    <td>${order.id}</td>
                    <td><code>${esc(order.orderNumber || '-')}</code></td>
                    <td>${order.customerId}</td>
                    <td>${esc(order.orderType)}</td>
                    <td><span class="badge bg-${statusClass}">${esc(order.status)}</span></td>
                    <td>${esc(formatMoney(order.recurringAmount, order.currencyCode))}</td>
                    <td>${esc(formatDate(order.createdAt))}</td>
                    <td>${esc(formatDate(order.nextBillingDate))}</td>
                </tr>
                ${isSelected ? `<tr class="orders-row-drilldown"><td colspan="8">${renderOrderLinesPanel(order)}</td></tr>` : ''}
            `;
        }).join('');
    };

    const renderPagination = (): void => {
        const info = document.getElementById('orders-pagination-info');
        const list = document.getElementById('orders-paging-controls-list');

        if (!info || !list) {
            return;
        }

        if (!totalCount) {
            info.textContent = 'Showing 0 of 0';
            list.innerHTML = '';
            return;
        }

        const start = (currentPage - 1) * pageSize + 1;
        const end = Math.min(currentPage * pageSize, totalCount);
        info.textContent = `Showing ${start}-${end} of ${totalCount}`;

        if (totalPages <= 1) {
            list.innerHTML = '';
            return;
        }

        const makeItem = (label: string, page: number, disabled: boolean, active = false): string => {
            const cls = `page-item${disabled ? ' disabled' : ''}${active ? ' active' : ''}`;
            const ariaCurrent = active ? ' aria-current="page"' : '';
            const ariaDisabled = disabled ? ' aria-disabled="true" tabindex="-1"' : '';
            const dataPage = disabled ? '' : ` data-page="${page}"`;
            return `<li class="${cls}"><a class="page-link" href="#"${dataPage}${ariaCurrent}${ariaDisabled}>${label}</a></li>`;
        };

        const pages = new Set<number>();
        pages.add(1);
        if (totalPages >= 2) {
            pages.add(2);
            pages.add(totalPages - 1);
        }
        pages.add(totalPages);

        for (let page = currentPage - 1; page <= currentPage + 1; page += 1) {
            if (page >= 1 && page <= totalPages) {
                pages.add(page);
            }
        }

        const sortedPages = Array.from(pages)
            .filter((page) => page >= 1 && page <= totalPages)
            .sort((a, b) => a - b);

        let html = '';
        html += makeItem('Previous', currentPage - 1, currentPage <= 1);

        let lastPage = 0;
        for (const page of sortedPages) {
            if (lastPage > 0 && page - lastPage > 1) {
                html += '<li class="page-item disabled"><span class="page-link">…</span></li>';
            }

            html += makeItem(String(page), page, false, page === currentPage);
            lastPage = page;
        }

        html += makeItem('Next', currentPage + 1, currentPage >= totalPages);
        list.innerHTML = html;
    };

    const updateView = (): void => {
        loadPageSizeFromUi();
        renderTable();
        renderPagination();
    };

    const loadOrders = async (): Promise<void> => {
        const tableBody = document.getElementById('orders-table-body');
        if (tableBody) {
            tableBody.innerHTML = '<tr><td colspan="8" class="text-center"><div class="spinner-border text-primary"></div></td></tr>';
        }

        hideError();
        const response = await apiRequest<unknown>(`${getApiBaseUrl()}/Orders`, { method: 'GET' });

        if (!response.success) {
            showError(response.message || 'Failed to load orders.');
            if (tableBody) {
                tableBody.innerHTML = '<tr><td colspan="8" class="text-center text-danger">Failed to load data</td></tr>';
            }
            return;
        }

        const list = extractOrders(response.data);
        allOrders = list.map((item) => normalizeOrder(item));
        filteredOrders = [...allOrders];
        if (selectedOrderId !== null && !filteredOrders.some((order) => order.id === selectedOrderId)) {
            selectedOrderId = null;
        }

        updateView();
    };

    const changePage = (page: number): void => {
        if (page < 1 || page > totalPages) {
            return;
        }

        currentPage = page;
        updateView();
    };

    const bindEvents = (): void => {
        document.getElementById('orders-apply')?.addEventListener('click', applyFilters);
        document.getElementById('orders-reset')?.addEventListener('click', resetFilters);

        document.getElementById('orders-page-size')?.addEventListener('change', () => {
            currentPage = 1;
            updateView();
        });

        document.getElementById('orders-paging-controls')?.addEventListener('click', (event) => {
            const target = event.target as HTMLElement;
            const link = target.closest<HTMLAnchorElement>('a[data-page]');
            if (!link) {
                return;
            }

            event.preventDefault();
            const page = Number(link.dataset.page ?? '0');
            if (!Number.isFinite(page)) {
                return;
            }

            changePage(page);
        });

        document.getElementById('orders-table-body')?.addEventListener('click', (event) => {
            const target = event.target as HTMLElement;
            const row = target.closest<HTMLTableRowElement>('tr[data-order-id]');
            if (!row) {
                return;
            }

            const orderId = Number(row.dataset.orderId ?? '0');
            if (!Number.isFinite(orderId) || orderId <= 0) {
                return;
            }

            selectedOrderId = selectedOrderId === orderId ? null : orderId;
            updateView();
        });
    };

    const initializePage = (): void => {
        const page = document.getElementById('orders-page') as HTMLElement | null;
        if (!page || page.dataset.initialized === 'true') {
            return;
        }

        page.dataset.initialized = 'true';
        bindEvents();
        void loadOrders();
    };

    const setupObserver = (): void => {
        initializePage();

        if (document.body) {
            const observer = new MutationObserver(() => {
                const page = document.getElementById('orders-page') as HTMLElement | null;
                if (page && page.dataset.initialized !== 'true') {
                    initializePage();
                }
            });

            observer.observe(document.body, { childList: true, subtree: true });
        }
    };

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', setupObserver);
    } else {
        setupObserver();
    }
})();

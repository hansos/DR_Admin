(() => {
    interface AppSettings {
        apiBaseUrl?: string;
    }

    interface ApiResponse<T> {
        success: boolean;
        data?: T;
        message?: string;
    }

    const pageId = 'invoice-tax-audit-page';

    function getApiBaseUrl(): string {
        const settings = (window as Window & { AppSettings?: AppSettings }).AppSettings;
        return settings?.apiBaseUrl ?? '';
    }

    function getAuthToken(): string | null {
        const auth = (window as Window & { Auth?: { getToken?: () => string | null } }).Auth;
        if (auth?.getToken) {
            return auth.getToken();
        }

        return sessionStorage.getItem('rp_authToken');
    }

    async function apiRequest(endpoint: string): Promise<ApiResponse<unknown>> {
        try {
            const headers: Record<string, string> = { 'Content-Type': 'application/json' };
            const token = getAuthToken();
            if (token) {
                headers.Authorization = `Bearer ${token}`;
            }

            const response = await fetch(endpoint, { method: 'GET', headers, credentials: 'include' });
            const contentType = response.headers.get('content-type') ?? '';
            const payload = contentType.includes('application/json') ? await response.json() : null;

            if (!response.ok) {
                const body = (payload ?? {}) as { message?: string; title?: string };
                return { success: false, message: body.message ?? body.title ?? `Request failed with status ${response.status}` };
            }

            const wrapped = (payload ?? {}) as { data?: unknown; success?: boolean; message?: string };
            return { success: wrapped.success !== false, data: wrapped.data ?? payload, message: wrapped.message };
        } catch {
            return { success: false, message: 'Network error. Please try again.' };
        }
    }

    function extractItems(payload: unknown): Record<string, unknown>[] {
        if (Array.isArray(payload)) {
            return payload as Record<string, unknown>[];
        }

        const obj = (payload ?? {}) as { data?: unknown; Data?: unknown };
        if (Array.isArray(obj.data)) {
            return obj.data as Record<string, unknown>[];
        }

        if (Array.isArray(obj.Data)) {
            return obj.Data as Record<string, unknown>[];
        }

        return [];
    }

    function showSuccess(message: string): void {
        const success = document.getElementById('invoice-tax-audit-alert-success');
        const error = document.getElementById('invoice-tax-audit-alert-error');
        if (success) {
            success.textContent = message;
            success.classList.remove('d-none');
        }
        error?.classList.add('d-none');
    }

    function showError(message: string): void {
        const success = document.getElementById('invoice-tax-audit-alert-success');
        const error = document.getElementById('invoice-tax-audit-alert-error');
        if (error) {
            error.textContent = message;
            error.classList.remove('d-none');
        }
        success?.classList.add('d-none');
    }

    function formatMoney(value: unknown): string {
        const amount = Number(value ?? 0);
        return Number.isFinite(amount) ? amount.toFixed(2) : '-';
    }

    function renderRows(items: Record<string, unknown>[]): void {
        const tbody = document.getElementById('invoice-tax-audit-table-body');
        if (!tbody) {
            return;
        }

        const filtered = items.filter((item) => {
            const snapshotId = Number(item.orderTaxSnapshotId ?? item.OrderTaxSnapshotId ?? 0);
            const orderId = Number(item.orderId ?? item.OrderId ?? 0);
            return snapshotId > 0 || orderId > 0;
        });

        if (filtered.length === 0) {
            tbody.innerHTML = '<tr><td colspan="7" class="text-center text-muted">No snapshot-linked invoices found.</td></tr>';
            return;
        }

        tbody.innerHTML = filtered.map((item) => {
            const id = Number(item.id ?? item.Id ?? 0);
            const orderId = Number(item.orderId ?? item.OrderId ?? 0);
            const snapshotId = Number(item.orderTaxSnapshotId ?? item.OrderTaxSnapshotId ?? 0);
            const subTotal = formatMoney(item.subTotal ?? item.SubTotal);
            const taxAmount = formatMoney(item.taxAmount ?? item.TaxAmount);
            const totalAmount = formatMoney(item.totalAmount ?? item.TotalAmount);
            const taxName = String(item.taxName ?? item.TaxName ?? '-');

            return `<tr>
                <td>${id}</td>
                <td>${orderId > 0 ? orderId : '-'}</td>
                <td>${snapshotId > 0 ? snapshotId : '-'}</td>
                <td>${subTotal}</td>
                <td>${taxAmount}</td>
                <td>${totalAmount}</td>
                <td>${taxName}</td>
            </tr>`;
        }).join('');
    }

    async function loadData(showFeedback: boolean): Promise<void> {
        const response = await apiRequest(`${getApiBaseUrl()}/Invoices`);
        if (!response.success) {
            showError(response.message ?? 'Failed to load invoices for tax audit.');
            return;
        }

        renderRows(extractItems(response.data));
        if (showFeedback) {
            showSuccess(`Loaded at ${new Date().toLocaleString()}`);
        }
    }

    function initializePage(): void {
        const page = document.getElementById(pageId);
        if (!page || page.dataset.initialized === 'true') {
            return;
        }

        page.dataset.initialized = 'true';

        document.getElementById('invoice-tax-audit-refresh')?.addEventListener('click', async () => {
            await loadData(true);
        });

        void loadData(false);
    }

    function setup(): void {
        initializePage();

        if (!document.body) {
            return;
        }

        const observer = new MutationObserver(() => initializePage());
        observer.observe(document.body, { childList: true, subtree: true });
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', setup);
    } else {
        setup();
    }
})();

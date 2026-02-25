// @ts-nocheck
(function() {
interface Domain {
    id: number;
    name: string;
}

interface PagedResult<T> {
    data?: T[];
    Data?: T[];
    currentPage?: number;
    CurrentPage?: number;
    pageSize?: number;
    PageSize?: number;
    totalCount?: number;
    TotalCount?: number;
    totalPages?: number;
    TotalPages?: number;
}

interface ApiResponse<T> {
    success: boolean;
    data?: T;
    message?: string;
}

function getApiBaseUrl(): string {
    return (window as any).AppSettings?.apiBaseUrl ?? '';
}

function getAuthToken(): string | null {
    const auth = (window as any).Auth;
    if (auth?.getToken) {
        return auth.getToken();
    }

    return sessionStorage.getItem('rp_authToken');
}

async function apiRequest<T>(endpoint: string, options: RequestInit = {}): Promise<ApiResponse<T>> {
    try {
        const headers: Record<string, string> = {
            'Content-Type': 'application/json',
            ...(options.headers as Record<string, string>),
        };

        const authToken = getAuthToken();
        if (authToken) {
            headers['Authorization'] = `Bearer ${authToken}`;
        }

        const response = await fetch(endpoint, {
            ...options,
            headers,
            credentials: 'include',
        });

        const contentType = response.headers.get('content-type') ?? '';
        const hasJson = contentType.includes('application/json');
        const data = hasJson ? await response.json() : null;

        if (!response.ok) {
            return {
                success: false,
                message: (data && (data.message ?? data.title)) || `Request failed with status ${response.status}`,
            };
        }

        return {
            success: (data as any)?.success !== false,
            data: (data as any)?.data ?? data,
            message: (data as any)?.message,
        };
    } catch (error) {
        console.error('Dashboard summary request failed', error);
        return {
            success: false,
            message: 'Network error. Please try again.',
        };
    }
}

function initializePage(): void {
    const page = document.getElementById('dashboard-summary-page') as HTMLElement | null;
    if (!page || (page as any).dataset.initialized === 'true') {
        return;
    }

    (page as any).dataset.initialized = 'true';

    loadPendingSummary();
}
async function loadPendingSummary(): Promise<void> {
    clearError();
    setPendingLoading(true);

    try {
        const domains = await loadAllDomains();
        const pendingResults = await Promise.all(domains.map(async (domain) => {
            const response = await apiRequest<any[]>(`${getApiBaseUrl()}/DnsRecords/domain/${domain.id}/pending-sync`, { method: 'GET' });
            if (!response.success) {
                return { domain, count: null, error: response.message };
            }

            const records = Array.isArray(response.data) ? response.data : [];
            return { domain, count: records.length, error: null };
        }));

        const pending = pendingResults
            .filter((item) => item.count !== null && item.count > 0)
            .sort((a, b) => (b.count ?? 0) - (a.count ?? 0));

        renderPendingTable(pending);

        if (!pending.length) {
            setText('dashboard-summary-pending-note', 'No domains have pending DNS records.');
        } else {
            setText('dashboard-summary-pending-note', `${pending.length} domain(s) require registrar sync.`);
        }
    } catch (error: any) {
        showError(error?.message || 'Failed to load pending DNS records.');
        renderPendingTable([]);
        setText('dashboard-summary-pending-note', 'Unable to load pending DNS records.');
    } finally {
        setPendingLoading(false);
    }
}

async function loadAllDomains(): Promise<Domain[]> {
    let allItems: Domain[] = [];
    let pageNumber = 1;
    const pageSize = 200;
    let totalPages = 1;

    while (pageNumber <= totalPages) {
        const params = new URLSearchParams();
        params.set('pageNumber', String(pageNumber));
        params.set('pageSize', String(pageSize));

        const response = await apiRequest<PagedResult<Domain> | Domain[]>(`${getApiBaseUrl()}/RegisteredDomains?${params.toString()}`, { method: 'GET' });
        if (!response.success) {
            throw new Error(response.message || 'Failed to load domains');
        }

        const raw = response.data as any;
        const extracted = extractItems(raw);
        const meta = extracted.meta ?? raw;

        const items = extracted.items.map(normalizeDomain);
        allItems = allItems.concat(items);

        totalPages = meta?.totalPages ?? meta?.TotalPages ?? raw?.totalPages ?? raw?.TotalPages ?? totalPages;
        pageNumber += 1;

        if (!extracted.items.length) {
            break;
        }
    }

    return allItems;
}

function extractItems(raw: any): { items: any[]; meta: any } {
    if (Array.isArray(raw)) {
        return { items: raw, meta: null };
    }

    const candidates: any[] = [raw, raw?.data, raw?.Data, raw?.data?.data, raw?.data?.Data];

    const items =
        (Array.isArray(raw?.Data) && raw.Data) ||
        (Array.isArray(raw?.data) && raw.data) ||
        (Array.isArray(raw?.data?.Data) && raw.data.Data) ||
        (Array.isArray(raw?.data?.data) && raw.data.data) ||
        (Array.isArray(raw?.Data?.Data) && raw.Data.Data) ||
        [];

    const meta = candidates.find((c) => c && typeof c === 'object' && (
        c.totalCount !== undefined || c.TotalCount !== undefined ||
        c.totalPages !== undefined || c.TotalPages !== undefined ||
        c.currentPage !== undefined || c.CurrentPage !== undefined ||
        c.pageSize !== undefined || c.PageSize !== undefined
    ));

    return { items, meta };
}

function normalizeDomain(raw: any): Domain {
    return {
        id: raw.id ?? raw.Id ?? 0,
        name: raw.name ?? raw.Name ?? raw.domainName ?? '',
    };
}

function setPendingLoading(isLoading: boolean): void {
    const loading = document.getElementById('dashboard-summary-pending-loading');
    if (loading) {
        loading.classList.toggle('d-none', !isLoading);
    }
}

function renderPendingTable(rows: { domain: Domain; count: number | null }[]): void {
    const tbody = document.getElementById('dashboard-summary-pending-table');
    if (!tbody) {
        return;
    }

    if (!rows.length) {
        tbody.innerHTML = '<tr><td colspan="2" class="text-center text-muted">No pending DNS records.</td></tr>';
        return;
    }

    tbody.innerHTML = rows.map((row) => `
        <tr>
            <td><code>${esc(row.domain.name || `Domain #${row.domain.id}`)}</code></td>
            <td class="text-end"><span class="fw-semibold">${row.count ?? '-'}</span></td>
        </tr>
    `).join('');
}

function setText(id: string, value: string): void {
    const element = document.getElementById(id);
    if (element) {
        element.textContent = value;
    }
}

function showError(message: string): void {
    const alert = document.getElementById('dashboard-summary-alert-error');
    if (!alert) {
        return;
    }

    alert.textContent = message;
    alert.classList.remove('d-none');
}

function clearError(): void {
    const alert = document.getElementById('dashboard-summary-alert-error');
    if (alert) {
        alert.textContent = '';
        alert.classList.add('d-none');
    }
}

function esc(value: string): string {
    return value.replace(/[&<>"]/g, (match) => {
        switch (match) {
            case '&':
                return '&amp;';
            case '<':
                return '&lt;';
            case '>':
                return '&gt;';
            case '"':
                return '&quot;';
            default:
                return match;
        }
    });
}

function setupPageObserver(): void {
    initializePage();

    if (document.body) {
        const observer = new MutationObserver(() => {
            const page = document.getElementById('dashboard-summary-page');
            if (page && (page as any).dataset.initialized !== 'true') {
                initializePage();
            }
        });
        observer.observe(document.body, { childList: true, subtree: true });
    }
}

if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', setupPageObserver);
} else {
    setupPageObserver();
}

})();

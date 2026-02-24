// @ts-nocheck
(function() {
interface Domain {
    id: number;
    customerId: number;
    serviceId: number;
    name: string;
    providerId: number;
    status: string;
    registrationDate: string;
    expirationDate: string;
    customer?: { id?: number; Id?: number; name?: string; Name?: string } | null;
    registrar?: { id?: number; Id?: number; name?: string; Name?: string } | null;
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
        console.error('Expiring domains request failed', error);
        return {
            success: false,
            message: 'Network error. Please try again.',
        };
    }
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
    )) ?? null;

    return { items, meta };
}

function normalizeItem(item: any): Domain {
    return {
        id: item.id ?? item.Id ?? 0,
        customerId: item.customerId ?? item.CustomerId ?? 0,
        serviceId: item.serviceId ?? item.ServiceId ?? 0,
        name: item.name ?? item.Name ?? '',
        providerId: item.providerId ?? item.ProviderId ?? 0,
        status: item.status ?? item.Status ?? '',
        registrationDate: item.registrationDate ?? item.RegistrationDate ?? '',
        expirationDate: item.expirationDate ?? item.ExpirationDate ?? '',
        customer: item.customer ?? item.Customer ?? null,
        registrar: item.registrar ?? item.Registrar ?? null,
    };
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

        const items = extracted.items.map(normalizeItem);
        allItems = allItems.concat(items);

        totalPages = meta?.totalPages ?? meta?.TotalPages ?? raw?.totalPages ?? raw?.TotalPages ?? totalPages;
        pageNumber += 1;

        if (!extracted.items.length) {
            break;
        }
    }

    return allItems;
}

function getCustomerName(domain: Domain): string {
    const cust = domain.customer as any;
    return cust?.name ?? cust?.Name ?? '';
}

function getRegistrarName(domain: Domain): string {
    const reg = domain.registrar as any;
    return reg?.name ?? reg?.Name ?? '';
}

function renderSection(bodyId: string, domains: Domain[], emptyMessage: string): void {
    const tbody = document.getElementById(bodyId);
    if (!tbody) {
        return;
    }

    if (!domains.length) {
        tbody.innerHTML = `<tr><td colspan="4" class="text-center text-muted">${emptyMessage}</td></tr>`;
        return;
    }

    tbody.innerHTML = domains.map((domain) => {
        const customerName = getCustomerName(domain);
        const registrarName = getRegistrarName(domain);
        const customerDisplay = customerName
            ? `${esc(customerName)} <span class="text-muted">(#${domain.customerId})</span>`
            : `<span class="text-muted">#${domain.customerId}</span>`;
        const registrarDisplay = registrarName
            ? `${esc(registrarName)} <span class="text-muted">(#${domain.providerId})</span>`
            : `<span class="text-muted">#${domain.providerId}</span>`;
        const expires = domain.expirationDate ? formatDate(domain.expirationDate) : '-';

        return `
        <tr>
            <td><code>${esc(domain.name)}</code></td>
            <td>${customerDisplay}</td>
            <td>${registrarDisplay}</td>
            <td>${esc(expires)}</td>
        </tr>`;
    }).join('');
}

function showError(message: string): void {
    const alert = document.getElementById('expiring-domains-alert-error');
    if (!alert) {
        return;
    }

    alert.textContent = message;
    alert.classList.remove('d-none');
}

function esc(text: string): string {
    const map: Record<string, string> = { '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#039;' };
    return (text || '').toString().replace(/[&<>"']/g, (char) => map[char]);
}

function formatDate(value: string): string {
    const date = new Date(value);
    if (Number.isNaN(date.getTime())) {
        return value;
    }

    return date.toLocaleString();
}

async function loadExpiringDomains(): Promise<void> {
    try {
        const allDomains = await loadAllDomains();
        const now = new Date();
        const sevenDays = new Date(now.getTime());
        sevenDays.setDate(sevenDays.getDate() + 7);
        const thirtyDays = new Date(now.getTime());
        thirtyDays.setDate(thirtyDays.getDate() + 30);

        const expired = allDomains.filter((domain) => {
            const exp = new Date(domain.expirationDate);
            return domain.expirationDate && !Number.isNaN(exp.getTime()) && exp < now;
        });

        const next7 = allDomains.filter((domain) => {
            const exp = new Date(domain.expirationDate);
            return domain.expirationDate && !Number.isNaN(exp.getTime()) && exp >= now && exp <= sevenDays;
        });

        const next30 = allDomains.filter((domain) => {
            const exp = new Date(domain.expirationDate);
            return domain.expirationDate && !Number.isNaN(exp.getTime()) && exp > sevenDays && exp <= thirtyDays;
        });

        renderSection('expiring-domains-7-body', next7, 'No domains expiring in the next 7 days.');
        renderSection('expiring-domains-30-body', next30, 'No domains expiring in 7-30 days.');
        renderSection('expiring-domains-expired-body', expired, 'No expired domains.');
    } catch (error: any) {
        console.error('Failed to load expiring domains', error);
        showError(error?.message || 'Failed to load expiring domains');
    }
}

function initializeExpiringDomainsPage(): void {
    const page = document.getElementById('expiring-domains-page');
    if (!page || (page as any).dataset.initialized === 'true') {
        return;
    }

    (page as any).dataset.initialized = 'true';
    loadExpiringDomains();
}

function setupPageObserver(): void {
    initializeExpiringDomainsPage();

    if (document.body) {
        const observer = new MutationObserver(() => {
            const page = document.getElementById('expiring-domains-page');
            if (page && (page as any).dataset.initialized !== 'true') {
                initializeExpiringDomainsPage();
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

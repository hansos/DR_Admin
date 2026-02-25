"use strict";
// @ts-nocheck
(function () {
    function getApiBaseUrl() {
        return window.AppSettings?.apiBaseUrl ?? '';
    }
    function getAuthToken() {
        const auth = window.Auth;
        if (auth?.getToken) {
            return auth.getToken();
        }
        return sessionStorage.getItem('rp_authToken');
    }
    async function apiRequest(endpoint, options = {}) {
        try {
            const headers = {
                'Content-Type': 'application/json',
                ...options.headers,
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
                success: data?.success !== false,
                data: data?.data ?? data,
                message: data?.message,
            };
        }
        catch (error) {
            console.error('Expiring domains request failed', error);
            return {
                success: false,
                message: 'Network error. Please try again.',
            };
        }
    }
    function extractItems(raw) {
        if (Array.isArray(raw)) {
            return { items: raw, meta: null };
        }
        const candidates = [raw, raw?.data, raw?.Data, raw?.data?.data, raw?.data?.Data];
        const items = (Array.isArray(raw?.Data) && raw.Data) ||
            (Array.isArray(raw?.data) && raw.data) ||
            (Array.isArray(raw?.data?.Data) && raw.data.Data) ||
            (Array.isArray(raw?.data?.data) && raw.data.data) ||
            (Array.isArray(raw?.Data?.Data) && raw.Data.Data) ||
            [];
        const meta = candidates.find((c) => c && typeof c === 'object' && (c.totalCount !== undefined || c.TotalCount !== undefined ||
            c.totalPages !== undefined || c.TotalPages !== undefined ||
            c.currentPage !== undefined || c.CurrentPage !== undefined ||
            c.pageSize !== undefined || c.PageSize !== undefined)) ?? null;
        return { items, meta };
    }
    function normalizeItem(item) {
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
    async function loadAllDomains() {
        let allItems = [];
        let pageNumber = 1;
        const pageSize = 200;
        let totalPages = 1;
        while (pageNumber <= totalPages) {
            const params = new URLSearchParams();
            params.set('pageNumber', String(pageNumber));
            params.set('pageSize', String(pageSize));
            const response = await apiRequest(`${getApiBaseUrl()}/RegisteredDomains?${params.toString()}`, { method: 'GET' });
            if (!response.success) {
                throw new Error(response.message || 'Failed to load domains');
            }
            const raw = response.data;
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
    function getCustomerName(domain) {
        const cust = domain.customer;
        return cust?.name ?? cust?.Name ?? '';
    }
    function getRegistrarName(domain) {
        const reg = domain.registrar;
        return reg?.name ?? reg?.Name ?? '';
    }
    function renderSection(bodyId, domains, emptyMessage) {
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
    function showError(message) {
        const alert = document.getElementById('expiring-domains-alert-error');
        if (!alert) {
            return;
        }
        alert.textContent = message;
        alert.classList.remove('d-none');
    }
    function esc(text) {
        const map = { '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#039;' };
        return (text || '').toString().replace(/[&<>"']/g, (char) => map[char]);
    }
    function formatDate(value) {
        const date = new Date(value);
        if (Number.isNaN(date.getTime())) {
            return value;
        }
        return date.toLocaleString();
    }
    async function loadExpiringDomains() {
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
        }
        catch (error) {
            console.error('Failed to load expiring domains', error);
            showError(error?.message || 'Failed to load expiring domains');
        }
    }
    function initializeExpiringDomainsPage() {
        const page = document.getElementById('expiring-domains-page');
        if (!page || page.dataset.initialized === 'true') {
            return;
        }
        page.dataset.initialized = 'true';
        loadExpiringDomains();
    }
    function setupPageObserver() {
        initializeExpiringDomainsPage();
        if (document.body) {
            const observer = new MutationObserver(() => {
                const page = document.getElementById('expiring-domains-page');
                if (page && page.dataset.initialized !== 'true') {
                    initializeExpiringDomainsPage();
                }
            });
            observer.observe(document.body, { childList: true, subtree: true });
        }
    }
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', setupPageObserver);
    }
    else {
        setupPageObserver();
    }
})();
//# sourceMappingURL=expiring-domains.js.map
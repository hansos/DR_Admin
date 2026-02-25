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
    createdAt?: string;
    updatedAt?: string;
    customer?: { id?: number; Id?: number; name?: string; Name?: string } | null;
    registrar?: { id?: number; Id?: number; name?: string; Name?: string; code?: string; Code?: string } | null;
}

interface DomainContact {
    contactType?: string | null;
    firstName?: string | null;
    lastName?: string | null;
    organization?: string | null;
    email?: string | null;
    phone?: string | null;
    fax?: string | null;
    addressLine1?: string | null;
    addressLine2?: string | null;
    city?: string | null;
    stateProvince?: string | null;
    postalCode?: string | null;
    country?: string | null;
}

interface DnsRecord {
    id: number;
    domainId: number;
    type: string;
    name: string;
    value: string;
    ttl: number;
    priority?: number | null;
    weight?: number | null;
    port?: number | null;
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
        console.error('Domain details request failed', error);
        return {
            success: false,
            message: 'Network error. Please try again.',
        };
    }
}

let domainId: number | null = null;
let currentDomain: Domain | null = null;
let currentRegistrarCode: string | null = null;
let currentDomainName: string | null = null;

function initializePage(): void {
    const page = document.getElementById('domain-details-page') as HTMLElement | null;
    if (!page || (page as any).dataset.initialized === 'true') {
        return;
    }

    (page as any).dataset.initialized = 'true';

    const params = new URLSearchParams(window.location.search);
    const idParam = params.get('id');
    const parsed = Number(idParam);

    document.getElementById('domain-dns-sync')?.addEventListener('click', openDnsSyncModal);
    document.getElementById('domain-dns-sync-confirm')?.addEventListener('click', syncDnsRecords);
    document.getElementById('domain-details-select')?.addEventListener('click', openSelectDomainModal);
    document.getElementById('domain-details-manual-load')?.addEventListener('click', loadDomainFromManualInput);

    if (!idParam || !Number.isFinite(parsed) || parsed <= 0) {
        showManualEntry();
        return;
    }

    domainId = parsed;

    loadDomainDetails();
}

function showManualEntry(): void {
    setLoading(false);

    const manualCard = document.getElementById('domain-details-empty');
    manualCard?.classList.remove('d-none');

    document.getElementById('domain-details-content')?.classList.add('d-none');

    setSelectedDomainLabel(null);
    openSelectDomainModal();
}

function openSelectDomainModal(): void {
    const input = document.getElementById('domain-details-manual-name') as HTMLInputElement | null;
    if (input) {
        input.value = currentDomainName ?? '';
    }

    showModal('domain-details-select-modal');

    input?.focus();

    input?.addEventListener('keydown', (event) => {
        if (event.key === 'Enter') {
            event.preventDefault();
            loadDomainFromManualInput();
        }
    });
}

async function loadDomainFromManualInput(): Promise<void> {
    const input = document.getElementById('domain-details-manual-name') as HTMLInputElement | null;
    const name = (input?.value ?? '').trim();

    if (!name) {
        showError('Enter a domain name to load.');
        return;
    }

    clearAlerts();
    setLoading(true);

    const response = await apiRequest<Domain>(`${getApiBaseUrl()}/RegisteredDomains/name/${encodeURIComponent(name)}`, { method: 'GET' });
    if (!response.success || !response.data) {
        setLoading(false);
        showError(response.message || 'Domain not found.');
        return;
    }

    currentDomain = normalizeDomain(response.data);
    domainId = currentDomain.id;
    updateDomainFields(currentDomain);
    setLoading(false);

    document.getElementById('domain-details-empty')?.classList.add('d-none');
    hideModal('domain-details-select-modal');
    await loadDnsRecords();
    await loadDomainContacts();
}

function setLoading(isLoading: boolean): void {
    const loading = document.getElementById('domain-details-loading');
    const content = document.getElementById('domain-details-content');

    if (loading) {
        loading.classList.toggle('d-none', !isLoading);
    }

    if (content) {
        content.classList.toggle('d-none', isLoading);
    }
}

function showSuccess(message: string): void {
    const alert = document.getElementById('domain-details-alert-success');
    if (!alert) {
        return;
    }

    alert.textContent = message;
    alert.classList.remove('d-none');

    const errorAlert = document.getElementById('domain-details-alert-error');
    errorAlert?.classList.add('d-none');
}

function showError(message: string): void {
    const alert = document.getElementById('domain-details-alert-error');
    if (!alert) {
        return;
    }

    alert.textContent = message;
    alert.classList.remove('d-none');

    const successAlert = document.getElementById('domain-details-alert-success');
    successAlert?.classList.add('d-none');
}

function clearAlerts(): void {
    document.getElementById('domain-details-alert-success')?.classList.add('d-none');
    document.getElementById('domain-details-alert-error')?.classList.add('d-none');
}

function normalizeDomain(item: any): Domain {
    return {
        id: item.id ?? item.Id ?? 0,
        customerId: item.customerId ?? item.CustomerId ?? 0,
        serviceId: item.serviceId ?? item.ServiceId ?? 0,
        name: item.name ?? item.Name ?? '',
        providerId: item.providerId ?? item.ProviderId ?? 0,
        status: item.status ?? item.Status ?? '',
        registrationDate: item.registrationDate ?? item.RegistrationDate ?? '',
        expirationDate: item.expirationDate ?? item.ExpirationDate ?? '',
        createdAt: item.createdAt ?? item.CreatedAt ?? null,
        updatedAt: item.updatedAt ?? item.UpdatedAt ?? null,
        customer: item.customer ?? item.Customer ?? null,
        registrar: item.registrar ?? item.Registrar ?? null,
    };
}

async function loadDomainDetails(): Promise<void> {
    if (!domainId) {
        return;
    }

    clearAlerts();
    setLoading(true);

    const response = await apiRequest<Domain>(`${getApiBaseUrl()}/RegisteredDomains/${domainId}`, { method: 'GET' });
    if (!response.success || !response.data) {
        setLoading(false);
        showError(response.message || 'Failed to load domain details.');
        return;
    }

    currentDomain = normalizeDomain(response.data);
    updateDomainFields(currentDomain);
    setLoading(false);

    await loadDnsRecords();
}

function updateDomainFields(domain: Domain): void {
    currentDomainName = domain.name || null;

    const customerName = domain.customer?.name ?? domain.customer?.Name ?? '';
    const registrarName = domain.registrar?.name ?? domain.registrar?.Name ?? '';
    currentRegistrarCode = domain.registrar?.code ?? domain.registrar?.Code ?? null;

    const registrarDisplay = registrarName
        ? `${registrarName}${currentRegistrarCode ? ` (${currentRegistrarCode})` : ''}`
        : currentRegistrarCode
            ? currentRegistrarCode
            : domain.providerId
                ? `#${domain.providerId}`
                : '-';

    const customerDisplay = customerName
        ? `${customerName} (#${domain.customerId})`
        : domain.customerId
            ? `#${domain.customerId}`
            : '-';

    setText('domain-details-name', domain.name || '-');
    setText('domain-details-id', String(domain.id || '-'));
    setText('domain-details-status', domain.status || '-');
    setText('domain-details-registrar', registrarDisplay);
    setText('domain-details-customer', customerDisplay);
    setText('domain-details-service', domain.serviceId ? String(domain.serviceId) : '-');
    setText('domain-details-registered', domain.registrationDate ? formatDate(domain.registrationDate) : '-');
    setText('domain-details-expires', domain.expirationDate ? formatDate(domain.expirationDate) : '-');

    const downloadButton = document.getElementById('domain-dns-sync') as HTMLButtonElement | null;
    if (downloadButton) {
        downloadButton.disabled = !currentRegistrarCode || !currentDomainName;
    }

    setSelectedDomainLabel(currentDomainName);
}

function setSelectedDomainLabel(domainName: string | null): void {
    const selectButton = document.getElementById('domain-details-select') as HTMLButtonElement | null;
    if (!selectButton) {
        return;
    }

    selectButton.innerHTML = domainName
        ? `<i class="bi bi-search"></i> ${esc(domainName)}`
        : '<i class="bi bi-search"></i> Select domain';
}

async function loadDnsRecords(): Promise<void> {
    const loading = document.getElementById('domain-dns-records-loading');
    const content = document.getElementById('domain-dns-records-content');
    const empty = document.getElementById('domain-dns-records-empty');

    if (loading) {
        loading.classList.remove('d-none');
    }
    content?.classList.add('d-none');
    empty?.classList.add('d-none');

    if (!domainId) {
        return;
    }

    const response = await apiRequest<DnsRecord[]>(`${getApiBaseUrl()}/DnsRecords/domain/${domainId}`, { method: 'GET' });
    if (!response.success) {
        loading?.classList.add('d-none');
        empty?.classList.remove('d-none');
        setRecordCount(0);
        return;
    }

    const records = Array.isArray(response.data) ? response.data : [];
    renderDnsRecords(records);
}

function renderDnsRecords(records: DnsRecord[]): void {
    const loading = document.getElementById('domain-dns-records-loading');
    const content = document.getElementById('domain-dns-records-content');
    const empty = document.getElementById('domain-dns-records-empty');
    const tableBody = document.getElementById('domain-dns-records-table-body');

    loading?.classList.add('d-none');

    if (!tableBody) {
        return;
    }

    if (!records.length) {
        tableBody.innerHTML = '';
        empty?.classList.remove('d-none');
        content?.classList.add('d-none');
        setRecordCount(0);
        return;
    }

    tableBody.innerHTML = records.map((record) => {
        return `
        <tr>
            <td>${esc(record.type || '-')}</td>
            <td>${esc(record.name || '-')}</td>
            <td>${esc(record.value || '-')}</td>
            <td>${record.ttl ?? '-'}</td>
            <td>${record.priority ?? '-'}</td>
            <td>${record.weight ?? '-'}</td>
            <td>${record.port ?? '-'}</td>
        </tr>`;
    }).join('');

    setRecordCount(records.length);
    empty?.classList.add('d-none');
    content?.classList.remove('d-none');
}

function setRecordCount(count: number): void {
    const badge = document.getElementById('domain-dns-record-count');
    if (badge) {
        badge.textContent = `${count} record${count === 1 ? '' : 's'}`;
    }
}

function openDnsSyncModal(): void {
    if (!currentRegistrarCode || !currentDomainName) {
        showError('Registrar code or domain name is missing for sync.');
        return;
    }

    setSyncSummary('');
    setSyncBusy(false);

    setText('domain-dns-sync-domain', currentDomainName || '-');
    setText('domain-dns-sync-registrar', currentRegistrarCode || '-');

    showModal('domain-dns-sync-modal');
}

function setSyncBusy(isBusy: boolean): void {
    const progress = document.getElementById('domain-dns-sync-progress');
    const confirm = document.getElementById('domain-dns-sync-confirm') as HTMLButtonElement | null;

    if (progress) {
        progress.classList.toggle('d-none', !isBusy);
    }

    if (confirm) {
        confirm.disabled = isBusy;
    }
}

function setSyncSummary(message: string): void {
    const summary = document.getElementById('domain-dns-sync-summary');
    if (!summary) {
        return;
    }

    if (!message) {
        summary.textContent = '';
        summary.classList.add('d-none');
        return;
    }

    summary.textContent = message;
    summary.classList.remove('d-none');
}

async function syncDnsRecords(): Promise<void> {
    if (!currentRegistrarCode || !currentDomainName) {
        showError('Registrar code or domain name is missing for sync.');
        return;
    }

    setSyncSummary('');
    setSyncBusy(true);

    const endpoint = `${getApiBaseUrl()}/DomainManager/registrar/${encodeURIComponent(currentRegistrarCode)}/domain/name/${encodeURIComponent(currentDomainName)}/dns-records/sync`;
    const response = await apiRequest(endpoint, { method: 'POST' });

    setSyncBusy(false);

    if (!response.success) {
        setSyncSummary(response.message || 'Sync failed.');
        return;
    }

    setSyncSummary(response.message || 'DNS records synced from registrar.');
    showSuccess('DNS records synced successfully.');
    await loadDnsRecords();
}

function setText(id: string, value: string): void {
    const el = document.getElementById(id);
    if (el) {
        el.textContent = value;
    }
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

function showModal(id: string): void {
    const element = document.getElementById(id);
    if (!element || !(window as any).bootstrap) {
        return;
    }

    const modal = new (window as any).bootstrap.Modal(element);
    modal.show();
}

function hideModal(id: string): void {
    const element = document.getElementById(id);
    if (!element || !(window as any).bootstrap) {
        return;
    }

    const modal = (window as any).bootstrap.Modal.getInstance(element);
    modal?.hide();
}

function setupPageObserver(): void {
    initializePage();

    if (document.body) {
        const observer = new MutationObserver(() => {
            const page = document.getElementById('domain-details-page') as HTMLElement | null;
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

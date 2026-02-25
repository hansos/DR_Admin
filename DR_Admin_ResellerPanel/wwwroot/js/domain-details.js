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
            console.error('Domain details request failed', error);
            return {
                success: false,
                message: 'Network error. Please try again.',
            };
        }
    }
    let domainId = null;
    let currentDomain = null;
    let currentRegistrarCode = null;
    let currentDomainName = null;
    function initializePage() {
        const page = document.getElementById('domain-details-page');
        if (!page || page.dataset.initialized === 'true') {
            return;
        }
        page.dataset.initialized = 'true';
        const params = new URLSearchParams(window.location.search);
        const idParam = params.get('id');
        const parsed = Number(idParam);
        document.getElementById('domain-dns-sync')?.addEventListener('click', openDnsSyncModal);
        document.getElementById('domain-dns-sync-confirm')?.addEventListener('click', syncDnsRecords);
        document.getElementById('domain-details-select')?.addEventListener('click', openSelectDomainModal);
        document.getElementById('domain-details-manual-load')?.addEventListener('click', loadDomainFromManualInput);
        document.getElementById('domain-dns-open-zones')?.addEventListener('click', openDnsZones);
        document.getElementById('domain-dns-open-zones')?.addEventListener('click', openDnsZones);
        if (!idParam || !Number.isFinite(parsed) || parsed <= 0) {
            showManualEntry();
            return;
        }
        function openDnsZones() {
            if (!domainId) {
                showError('Select a domain before opening DNS zones.');
                return;
            }
            window.location.href = `/dns/zones?domain-id=${encodeURIComponent(String(domainId))}`;
        }
        function openDnsZones() {
            if (!domainId) {
                showError('Select a domain before opening DNS zones.');
                return;
            }
            window.location.href = `/dns/zones?domain-id=${encodeURIComponent(String(domainId))}`;
        }
        domainId = parsed;
        loadDomainDetails();
    }
    function showManualEntry() {
        setLoading(false);
        const manualCard = document.getElementById('domain-details-empty');
        manualCard?.classList.remove('d-none');
        document.getElementById('domain-details-content')?.classList.add('d-none');
        setSelectedDomainLabel(null);
        openSelectDomainModal();
    }
    function openSelectDomainModal() {
        const input = document.getElementById('domain-details-manual-name');
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
    async function loadDomainFromManualInput() {
        const input = document.getElementById('domain-details-manual-name');
        const name = (input?.value ?? '').trim();
        if (!name) {
            showError('Enter a domain name to load.');
            return;
        }
        clearAlerts();
        setLoading(true);
        const response = await apiRequest(`${getApiBaseUrl()}/RegisteredDomains/name/${encodeURIComponent(name)}`, { method: 'GET' });
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
    function setLoading(isLoading) {
        const loading = document.getElementById('domain-details-loading');
        const content = document.getElementById('domain-details-content');
        if (loading) {
            loading.classList.toggle('d-none', !isLoading);
        }
        if (content) {
            content.classList.toggle('d-none', isLoading);
        }
    }
    function showSuccess(message) {
        const alert = document.getElementById('domain-details-alert-success');
        if (!alert) {
            return;
        }
        alert.textContent = message;
        alert.classList.remove('d-none');
        const errorAlert = document.getElementById('domain-details-alert-error');
        errorAlert?.classList.add('d-none');
    }
    function showError(message) {
        const alert = document.getElementById('domain-details-alert-error');
        if (!alert) {
            return;
        }
        alert.textContent = message;
        alert.classList.remove('d-none');
        const successAlert = document.getElementById('domain-details-alert-success');
        successAlert?.classList.add('d-none');
    }
    function clearAlerts() {
        document.getElementById('domain-details-alert-success')?.classList.add('d-none');
        document.getElementById('domain-details-alert-error')?.classList.add('d-none');
    }
    function normalizeDomain(item) {
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
    async function loadDomainDetails() {
        if (!domainId) {
            return;
        }
        clearAlerts();
        setLoading(true);
        const response = await apiRequest(`${getApiBaseUrl()}/RegisteredDomains/${domainId}`, { method: 'GET' });
        if (!response.success || !response.data) {
            setLoading(false);
            showError(response.message || 'Failed to load domain details.');
            return;
        }
        currentDomain = normalizeDomain(response.data);
        updateDomainFields(currentDomain);
        setLoading(false);
        await loadDnsRecords();
        await loadDomainContacts();
    }
    function updateDomainFields(domain) {
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
        const downloadButton = document.getElementById('domain-dns-sync');
        if (downloadButton) {
            downloadButton.disabled = !currentRegistrarCode || !currentDomainName;
        }
        setSelectedDomainLabel(currentDomainName);
    }
    function setSelectedDomainLabel(domainName) {
        const selectButton = document.getElementById('domain-details-select');
        if (!selectButton) {
            return;
        }
        selectButton.innerHTML = domainName
            ? `<i class="bi bi-search"></i> ${esc(domainName)}`
            : '<i class="bi bi-search"></i> Select domain';
    }
    async function loadDnsRecords() {
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
        const response = await apiRequest(`${getApiBaseUrl()}/DnsRecords/domain/${domainId}`, { method: 'GET' });
        if (!response.success) {
            loading?.classList.add('d-none');
            empty?.classList.remove('d-none');
            setRecordCount(0);
            return;
        }
        const records = Array.isArray(response.data) ? response.data : [];
        renderDnsRecords(records);
    }
    async function loadDomainContacts() {
        const loading = document.getElementById('domain-contacts-loading');
        const content = document.getElementById('domain-contacts-content');
        const empty = document.getElementById('domain-contacts-empty');
        loading?.classList.remove('d-none');
        content?.classList.add('d-none');
        empty?.classList.add('d-none');
        if (!domainId) {
            loading?.classList.add('d-none');
            empty?.classList.remove('d-none');
            return;
        }
        const response = await apiRequest(`${getApiBaseUrl()}/DomainContacts/domain/${domainId}`, { method: 'GET' });
        if (!response.success) {
            loading?.classList.add('d-none');
            empty?.classList.remove('d-none');
            return;
        }
        const raw = response.data;
        const contacts = Array.isArray(raw)
            ? raw
            : Array.isArray(raw?.data)
                ? raw.data
                : Array.isArray(raw?.Data)
                    ? raw.Data
                    : [];
        renderDomainContacts(contacts);
    }
    function renderDomainContacts(contacts) {
        const loading = document.getElementById('domain-contacts-loading');
        const content = document.getElementById('domain-contacts-content');
        const empty = document.getElementById('domain-contacts-empty');
        const accordion = document.getElementById('domain-contacts-accordion');
        loading?.classList.add('d-none');
        if (!accordion) {
            return;
        }
        if (!contacts.length) {
            accordion.innerHTML = '';
            empty?.classList.remove('d-none');
            content?.classList.add('d-none');
            return;
        }
        const contactOrder = { Registrant: 1, Administrative: 2, Technical: 3, Billing: 4 };
        contacts.sort((a, b) => (contactOrder[a.contactType ?? ''] ?? 999) - (contactOrder[b.contactType ?? ''] ?? 999));
        accordion.innerHTML = contacts.map((contact, index) => {
            const contactId = `domain-contact-${index}`;
            const isFirst = index === 0;
            const type = contact.contactType || 'Contact';
            const fullName = `${contact.firstName ?? ''} ${contact.lastName ?? ''}`.trim() || '-';
            const typeInfo = getContactTypeInfo(type);
            return `
        <div class="accordion-item">
            <h2 class="accordion-header" id="heading-${contactId}">
                <button class="accordion-button ${isFirst ? '' : 'collapsed'}" type="button"
                        data-bs-toggle="collapse" data-bs-target="#collapse-${contactId}"
                        aria-expanded="${isFirst}" aria-controls="collapse-${contactId}">
                    <i class="bi ${typeInfo.icon} ${typeInfo.color} me-2"></i>
                    <strong>${esc(type)}</strong> - ${esc(fullName)}
                </button>
            </h2>
            <div id="collapse-${contactId}" class="accordion-collapse collapse ${isFirst ? 'show' : ''}"
                 aria-labelledby="heading-${contactId}" data-bs-parent="#domain-contacts-accordion">
                <div class="accordion-body">
                    <div class="row">
                        <div class="col-md-6">
                            <table class="table table-sm table-borderless">
                                <tr><th width="40%">First Name:</th><td>${esc(contact.firstName ?? '-')}</td></tr>
                                <tr><th>Last Name:</th><td>${esc(contact.lastName ?? '-')}</td></tr>
                                <tr><th>Organization:</th><td>${esc(contact.organization ?? '-')}</td></tr>
                                <tr><th>Email:</th><td>${contact.email ? `<a href="mailto:${esc(contact.email)}">${esc(contact.email)}</a>` : '-'}</td></tr>
                                <tr><th>Phone:</th><td>${contact.phone ? `<a href="tel:${esc(contact.phone)}">${esc(contact.phone)}</a>` : '-'}</td></tr>
                                <tr><th>Fax:</th><td>${esc(contact.fax ?? '-')}</td></tr>
                            </table>
                        </div>
                        <div class="col-md-6">
                            <table class="table table-sm table-borderless">
                                <tr><th width="40%">Address Line 1:</th><td>${esc(contact.addressLine1 ?? '-')}</td></tr>
                                <tr><th>Address Line 2:</th><td>${esc(contact.addressLine2 ?? '-')}</td></tr>
                                <tr><th>City:</th><td>${esc(contact.city ?? '-')}</td></tr>
                                <tr><th>State/Province:</th><td>${esc(contact.stateProvince ?? '-')}</td></tr>
                                <tr><th>Postal Code:</th><td>${esc(contact.postalCode ?? '-')}</td></tr>
                                <tr><th>Country:</th><td>${esc(contact.country ?? '-')}</td></tr>
                            </table>
                        </div>
                    </div>
                </div>
            </div>
        </div>`;
        }).join('');
        empty?.classList.add('d-none');
        content?.classList.remove('d-none');
    }
    function getContactTypeInfo(contactType) {
        const typeMap = {
            Registrant: { icon: 'bi-person-badge', color: 'text-primary' },
            Administrative: { icon: 'bi-person-gear', color: 'text-success' },
            Technical: { icon: 'bi-person-workspace', color: 'text-info' },
            Billing: { icon: 'bi-credit-card', color: 'text-warning' },
        };
        return typeMap[contactType] ?? { icon: 'bi-person', color: 'text-secondary' };
    }
    function renderDnsRecords(records) {
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
    function setRecordCount(count) {
        const badge = document.getElementById('domain-dns-record-count');
        if (badge) {
            badge.textContent = `${count} record${count === 1 ? '' : 's'}`;
        }
    }
    function openDnsSyncModal() {
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
    function setSyncBusy(isBusy) {
        const progress = document.getElementById('domain-dns-sync-progress');
        const confirm = document.getElementById('domain-dns-sync-confirm');
        if (progress) {
            progress.classList.toggle('d-none', !isBusy);
        }
        if (confirm) {
            confirm.disabled = isBusy;
        }
    }
    function setSyncSummary(message) {
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
    async function syncDnsRecords() {
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
    function setText(id, value) {
        const el = document.getElementById(id);
        if (el) {
            el.textContent = value;
        }
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
    function showModal(id) {
        const element = document.getElementById(id);
        if (!element || !window.bootstrap) {
            return;
        }
        const modal = new window.bootstrap.Modal(element);
        modal.show();
    }
    function hideModal(id) {
        const element = document.getElementById(id);
        if (!element || !window.bootstrap) {
            return;
        }
        const modal = window.bootstrap.Modal.getInstance(element);
        modal?.hide();
    }
    function setupPageObserver() {
        initializePage();
        if (document.body) {
            const observer = new MutationObserver(() => {
                const page = document.getElementById('domain-details-page');
                if (page && page.dataset.initialized !== 'true') {
                    initializePage();
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
//# sourceMappingURL=domain-details.js.map
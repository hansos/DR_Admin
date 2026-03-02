"use strict";
(() => {
    let currentDomain = null;
    function initializeDomainDetailsPage() {
        const page = document.getElementById('domain-details-page');
        if (!page || page.dataset.initialized === 'true') {
            return;
        }
        page.dataset.initialized = 'true';
        document.getElementById('domain-details-toggle-lock')?.addEventListener('click', () => {
            void toggleDomainLock();
        });
        document.getElementById('domain-details-copy-epp')?.addEventListener('click', () => {
            void copyDomainEppHint();
        });
        void loadDomainDetails();
    }
    async function loadDomainDetails() {
        const typedWindow = window;
        typedWindow.UserPanelAlerts?.hide('domain-details-alert-error');
        const domainId = getDomainDetailsId();
        if (!domainId) {
            typedWindow.UserPanelAlerts?.showError('domain-details-alert-error', 'Missing or invalid domain id.');
            return;
        }
        const [domainResponse, nameserverResponse] = await Promise.all([
            typedWindow.UserPanelApi?.request(`/RegisteredDomains/${domainId}`, { method: 'GET' }, true),
            typedWindow.UserPanelApi?.request(`/NameServers/domain/${domainId}`, { method: 'GET' }, true)
        ]);
        if (!domainResponse || !domainResponse.success || !domainResponse.data) {
            typedWindow.UserPanelAlerts?.showError('domain-details-alert-error', domainResponse?.message ?? 'Could not load domain details.');
            return;
        }
        currentDomain = domainResponse.data;
        renderDomainSummary(domainResponse.data);
        renderNameServers(nameserverResponse?.success ? (nameserverResponse.data ?? []) : []);
        const dnsLink = document.getElementById('domain-details-open-dns');
        if (dnsLink) {
            dnsLink.href = `/domains/dns?id=${encodeURIComponent(domainId.toString())}`;
        }
    }
    function renderDomainSummary(domain) {
        const summary = document.getElementById('domain-details-summary');
        if (!summary) {
            return;
        }
        summary.innerHTML = `
        <div><strong>${escapeDomainDetailsText(domain.name)}</strong></div>
        <div class="small text-muted">Status: ${escapeDomainDetailsText(domain.status)}</div>
        <div class="small text-muted">Registered: ${formatDomainDetailsDate(domain.registrationDate)} · Expires: ${formatDomainDetailsDate(domain.expirationDate)}</div>
    `;
    }
    function renderNameServers(items) {
        const list = document.getElementById('domain-details-nameservers');
        if (!list) {
            return;
        }
        if (items.length === 0) {
            list.innerHTML = '<li class="text-muted">No nameservers found.</li>';
            return;
        }
        list.innerHTML = items
            .sort((a, b) => a.sortOrder - b.sortOrder)
            .map((item) => `<li>${escapeDomainDetailsText(item.hostname)}${item.isPrimary ? ' (primary)' : ''}</li>`)
            .join('');
    }
    async function toggleDomainLock() {
        const typedWindow = window;
        if (!currentDomain) {
            typedWindow.UserPanelAlerts?.showError('domain-details-alert-error', 'No domain loaded.');
            return;
        }
        const currentStatus = currentDomain.status.toLowerCase();
        const isLocked = currentStatus.includes('lock');
        const nextStatus = isLocked ? 'Active' : 'Locked';
        const payload = {
            customerId: currentDomain.customerId,
            serviceId: currentDomain.serviceId ?? null,
            name: currentDomain.name,
            providerId: currentDomain.providerId,
            status: nextStatus,
            registrationDate: currentDomain.registrationDate,
            expirationDate: currentDomain.expirationDate
        };
        const response = await typedWindow.UserPanelApi?.request(`/RegisteredDomains/${currentDomain.id}`, {
            method: 'PUT',
            body: JSON.stringify(payload)
        }, true);
        if (!response || !response.success || !response.data) {
            typedWindow.UserPanelAlerts?.showError('domain-details-alert-error', response?.message ?? 'Could not update domain lock state.');
            return;
        }
        currentDomain = response.data;
        renderDomainSummary(response.data);
        typedWindow.UserPanelAlerts?.showSuccess('domain-details-alert-success', `Domain status updated to ${nextStatus}.`);
    }
    async function copyDomainEppHint() {
        const typedWindow = window;
        if (!currentDomain) {
            return;
        }
        const eppHint = `${currentDomain.name}-${currentDomain.id}`;
        try {
            await navigator.clipboard.writeText(eppHint);
            typedWindow.UserPanelAlerts?.showSuccess('domain-details-alert-success', 'EPP/Auth hint copied to clipboard.');
        }
        catch {
            typedWindow.UserPanelAlerts?.showError('domain-details-alert-error', 'Could not copy to clipboard.');
        }
    }
    function getDomainDetailsId() {
        const idText = new URLSearchParams(window.location.search).get('id');
        if (!idText) {
            return null;
        }
        const value = Number.parseInt(idText, 10);
        return Number.isNaN(value) || value <= 0 ? null : value;
    }
    function formatDomainDetailsDate(value) {
        const date = new Date(value);
        if (Number.isNaN(date.getTime())) {
            return '-';
        }
        return date.toLocaleDateString();
    }
    function escapeDomainDetailsText(value) {
        return value
            .replace(/&/g, '&amp;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;')
            .replace(/'/g, '&#039;');
    }
    function setupDomainDetailsObserver() {
        initializeDomainDetailsPage();
        const observer = new MutationObserver(() => {
            initializeDomainDetailsPage();
        });
        observer.observe(document.body, { childList: true, subtree: true });
    }
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', setupDomainDetailsObserver);
    }
    else {
        setupDomainDetailsObserver();
    }
})();
//# sourceMappingURL=domain-details.js.map
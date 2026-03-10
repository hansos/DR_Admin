"use strict";
(() => {
    let domainsPageNumber = 1;
    let domainsPageSize = 25;
    let domainsLastCount = 0;
    function initializeDomainsPage() {
        const page = document.getElementById('domains-page');
        if (!page || page.dataset.initialized === 'true') {
            return;
        }
        page.dataset.initialized = 'true';
        document.getElementById('domains-apply')?.addEventListener('click', () => {
            domainsPageNumber = 1;
            void loadDomains();
        });
        document.getElementById('domains-prev')?.addEventListener('click', () => {
            if (domainsPageNumber > 1) {
                domainsPageNumber -= 1;
                void loadDomains();
            }
        });
        document.getElementById('domains-next')?.addEventListener('click', () => {
            if (domainsLastCount >= domainsPageSize) {
                domainsPageNumber += 1;
                void loadDomains();
            }
        });
        document.getElementById('domains-page-size')?.addEventListener('change', () => {
            domainsPageSize = Number.parseInt(readDomainsInputValue('domains-page-size'), 10) || 25;
            domainsPageNumber = 1;
            void loadDomains();
        });
        void loadDomains();
    }
    async function loadDomains() {
        const typedWindow = window;
        typedWindow.UserPanelAlerts?.hide('domains-alert-error');
        domainsPageSize = Number.parseInt(readDomainsInputValue('domains-page-size'), 10) || 25;
        const customerId = await resolveDomainsCustomerId();
        if (!customerId) {
            typedWindow.UserPanelAlerts?.showError('domains-alert-error', 'Could not resolve customer account.');
            renderDomainsRows([]);
            setDomainsPaginationInfo(0);
            return;
        }
        const response = await typedWindow.UserPanelApi?.request(`/RegisteredDomains/customer/${customerId}?pageNumber=${domainsPageNumber}&pageSize=${domainsPageSize}`, { method: 'GET' }, true);
        if (!response || !response.success || !response.data) {
            typedWindow.UserPanelAlerts?.showError('domains-alert-error', response?.message ?? 'Could not load domains.');
            renderDomainsRows([]);
            return;
        }
        const items = normalizeDomains(response.data);
        const filtered = applyDomainsFilters(items);
        domainsLastCount = filtered.length;
        renderDomainsRows(filtered);
        setDomainsPaginationInfo(filtered.length);
    }
    async function resolveDomainsCustomerId() {
        const typedWindow = window;
        const response = await typedWindow.UserPanelApi?.request('/MyAccount/me', { method: 'GET' }, true);
        return response?.success ? (response.data?.customer?.id ?? null) : null;
    }
    function normalizeDomains(payload) {
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
    function applyDomainsFilters(items) {
        const search = readDomainsInputValue('domains-filter-search').toLowerCase();
        const status = readDomainsInputValue('domains-filter-status').toLowerCase();
        return items.filter((item) => {
            const nameMatch = !search || item.name.toLowerCase().includes(search);
            const statusMatch = !status || item.status.toLowerCase().includes(status);
            return nameMatch && statusMatch;
        });
    }
    function renderDomainsRows(items) {
        const tableBody = document.getElementById('domains-table-body');
        if (!tableBody) {
            return;
        }
        if (items.length === 0) {
            tableBody.innerHTML = '<tr><td colspan="5" class="text-center text-muted">No domains found.</td></tr>';
            return;
        }
        tableBody.innerHTML = items.map((item) => {
            const encodedId = encodeURIComponent(item.id.toString());
            return `<tr>
            <td>${item.id}</td>
            <td>${escapeDomainsText(item.name)}</td>
            <td><span class="badge bg-secondary">${escapeDomainsText(item.status)}</span></td>
            <td>${formatDomainsDate(item.expirationDate)}</td>
            <td>
                <div class="btn-group btn-group-sm">
                    <a class="btn btn-outline-primary" href="/domains/details?id=${encodedId}">Details</a>
                    <a class="btn btn-outline-secondary" href="/domains/dns?id=${encodedId}">DNS</a>
                </div>
            </td>
        </tr>`;
        }).join('');
    }
    function setDomainsPaginationInfo(count) {
        const info = document.getElementById('domains-pagination-info');
        if (!info) {
            return;
        }
        info.textContent = `Page ${domainsPageNumber} · Showing ${count} item(s)`;
    }
    function formatDomainsDate(value) {
        const normalizedValue = value?.trim();
        if (!normalizedValue) {
            return '-';
        }
        const date = new Date(normalizedValue);
        if (Number.isNaN(date.getTime())) {
            return '-';
        }
        if (date.getUTCFullYear() <= 1970) {
            return '-';
        }
        return date.toLocaleDateString();
    }
    function readDomainsInputValue(id) {
        const input = document.getElementById(id);
        return input?.value.trim() ?? '';
    }
    function escapeDomainsText(value) {
        return value
            .replace(/&/g, '&amp;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;')
            .replace(/'/g, '&#039;');
    }
    function setupDomainsObserver() {
        initializeDomainsPage();
        const observer = new MutationObserver(() => {
            initializeDomainsPage();
        });
        observer.observe(document.body, { childList: true, subtree: true });
    }
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', setupDomainsObserver);
    }
    else {
        setupDomainsObserver();
    }
})();
//# sourceMappingURL=domains.js.map
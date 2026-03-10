"use strict";
(() => {
    let hostingPageNumber = 1;
    let hostingPageSize = 25;
    let hostingTotalItems = 0;
    function initializeHostingPage() {
        const page = document.getElementById('hosting-page');
        if (!page || page.dataset.initialized === 'true') {
            return;
        }
        page.dataset.initialized = 'true';
        document.getElementById('hosting-apply')?.addEventListener('click', () => {
            hostingPageNumber = 1;
            void loadHostingAccounts();
        });
        document.getElementById('hosting-prev')?.addEventListener('click', () => {
            if (hostingPageNumber > 1) {
                hostingPageNumber -= 1;
                void loadHostingAccounts();
            }
        });
        document.getElementById('hosting-next')?.addEventListener('click', () => {
            if (hostingPageNumber * hostingPageSize < hostingTotalItems) {
                hostingPageNumber += 1;
                void loadHostingAccounts();
            }
        });
        document.getElementById('hosting-page-size')?.addEventListener('change', () => {
            hostingPageSize = Number.parseInt(readHostingInputValue('hosting-page-size'), 10) || 25;
            hostingPageNumber = 1;
            void loadHostingAccounts();
        });
        void loadHostingAccounts();
    }
    async function loadHostingAccounts() {
        const typedWindow = window;
        typedWindow.UserPanelAlerts?.hide('hosting-alert-error');
        hostingPageSize = Number.parseInt(readHostingInputValue('hosting-page-size'), 10) || 25;
        const customerId = await resolveHostingCustomerId();
        if (!customerId) {
            typedWindow.UserPanelAlerts?.showError('hosting-alert-error', 'Could not resolve customer account.');
            renderHostingRows([]);
            setHostingPaginationInfo(0);
            return;
        }
        const response = await typedWindow.UserPanelApi?.request(`/SoldHostingPackages/customer/${customerId}`, { method: 'GET' }, true);
        if (!response || !response.success || !response.data) {
            typedWindow.UserPanelAlerts?.showError('hosting-alert-error', response?.message ?? 'Could not load hosting subscriptions.');
            renderHostingRows([]);
            setHostingPaginationInfo(0);
            return;
        }
        const filtered = applyHostingFilters(response.data);
        hostingTotalItems = filtered.length;
        const paged = getHostingPageSlice(filtered);
        renderHostingRows(paged);
        setHostingPaginationInfo(paged.length);
    }
    async function resolveHostingCustomerId() {
        const typedWindow = window;
        const response = await typedWindow.UserPanelApi?.request('/MyAccount/me', { method: 'GET' }, true);
        return response?.success ? (response.data?.customer?.id ?? null) : null;
    }
    function applyHostingFilters(items) {
        const status = readHostingInputValue('hosting-filter-status').toLowerCase();
        const cycle = readHostingInputValue('hosting-filter-cycle').toLowerCase();
        return items.filter((item) => {
            const statusMatch = !status || item.status.toLowerCase().includes(status);
            const cycleMatch = !cycle || item.billingCycle.toLowerCase().includes(cycle);
            return statusMatch && cycleMatch;
        });
    }
    function getHostingPageSlice(items) {
        const start = (hostingPageNumber - 1) * hostingPageSize;
        return items.slice(start, start + hostingPageSize);
    }
    function renderHostingRows(items) {
        const tableBody = document.getElementById('hosting-table-body');
        if (!tableBody) {
            return;
        }
        if (items.length === 0) {
            tableBody.innerHTML = '<tr><td colspan="8" class="text-center text-muted">No hosting subscriptions found.</td></tr>';
            return;
        }
        tableBody.innerHTML = items.map((item) => {
            return `<tr>
            <td>${item.id}</td>
            <td>${escapeHostingText(item.status)}</td>
            <td>${escapeHostingText(item.billingCycle)}</td>
            <td>${formatHostingMoney(item.recurringPrice, item.currencyCode)}</td>
            <td>${formatHostingDate(item.nextBillingDate)}</td>
            <td>${item.expiresAt ? formatHostingDate(item.expiresAt) : '-'}</td>
            <td>${item.autoRenew ? 'Yes' : 'No'}</td>
            <td>#${item.orderId}</td>
        </tr>`;
        }).join('');
    }
    function setHostingPaginationInfo(count) {
        const info = document.getElementById('hosting-pagination-info');
        if (!info) {
            return;
        }
        info.textContent = `Page ${hostingPageNumber} · Showing ${count} of ${hostingTotalItems} item(s)`;
    }
    function readHostingInputValue(id) {
        const input = document.getElementById(id);
        return input?.value.trim() ?? '';
    }
    function formatHostingDate(value) {
        const date = new Date(value);
        if (Number.isNaN(date.getTime())) {
            return '-';
        }
        return date.toLocaleDateString();
    }
    function formatHostingMoney(value, currencyCode) {
        try {
            return new Intl.NumberFormat(undefined, { style: 'currency', currency: currencyCode }).format(value);
        }
        catch {
            return `${value.toFixed(2)} ${currencyCode}`;
        }
    }
    function escapeHostingText(value) {
        return value
            .replace(/&/g, '&amp;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;')
            .replace(/'/g, '&#039;');
    }
    function setupHostingObserver() {
        initializeHostingPage();
        const observer = new MutationObserver(() => {
            initializeHostingPage();
        });
        observer.observe(document.body, { childList: true, subtree: true });
    }
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', setupHostingObserver);
    }
    else {
        setupHostingObserver();
    }
})();
//# sourceMappingURL=hosting.js.map
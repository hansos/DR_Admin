"use strict";
(() => {
    function initializeHostingPage() {
        const page = document.getElementById('hosting-page');
        if (!page || page.dataset.initialized === 'true') {
            return;
        }
        page.dataset.initialized = 'true';
        void loadHostingAccounts();
    }
    async function loadHostingAccounts() {
        const typedWindow = window;
        const response = await typedWindow.UserPanelApi?.request('/HostingAccounts', { method: 'GET' }, true);
        if (!response || !response.success || !response.data) {
            typedWindow.UserPanelAlerts?.showError('hosting-alert-error', response?.message ?? 'Could not load hosting accounts.');
            renderHostingRows([]);
            return;
        }
        renderHostingRows(response.data);
    }
    function renderHostingRows(items) {
        const tableBody = document.getElementById('hosting-table-body');
        if (!tableBody) {
            return;
        }
        if (items.length === 0) {
            tableBody.innerHTML = '<tr><td colspan="6" class="text-center text-muted">No hosting accounts found.</td></tr>';
            return;
        }
        tableBody.innerHTML = items.map((item) => {
            const encodedId = encodeURIComponent(item.id.toString());
            return `<tr>
            <td>${item.id}</td>
            <td>${escapeHostingText(item.username)}</td>
            <td>${escapeHostingText(item.status)}</td>
            <td>${escapeHostingText(item.planName ?? '-')}</td>
            <td>${formatHostingDate(item.expirationDate)}</td>
            <td><a class="btn btn-outline-primary btn-sm" href="/hosting/details?id=${encodedId}">Details</a></td>
        </tr>`;
        }).join('');
    }
    function formatHostingDate(value) {
        const date = new Date(value);
        if (Number.isNaN(date.getTime())) {
            return '-';
        }
        return date.toLocaleDateString();
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
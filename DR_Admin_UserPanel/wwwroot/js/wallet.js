"use strict";
(() => {
    function initializeWalletPage() {
        const page = document.getElementById('wallet-page');
        if (!page || page.dataset.initialized === 'true') {
            return;
        }
        page.dataset.initialized = 'true';
        void loadWalletData();
    }
    async function loadWalletData() {
        const typedWindow = window;
        typedWindow.UserPanelAlerts?.hide('wallet-alert-error');
        const customerId = await getWalletCustomerId();
        if (!customerId) {
            typedWindow.UserPanelAlerts?.showError('wallet-alert-error', 'Could not resolve customer for wallet.');
            renderWalletBalance(null);
            renderWalletTransactions([]);
            return;
        }
        const [creditResponse, transactionsResponse] = await Promise.all([
            typedWindow.UserPanelApi?.request(`/CustomerCredits/customer/${customerId}`, { method: 'GET' }, true),
            typedWindow.UserPanelApi?.request(`/CustomerCredits/customer/${customerId}/transactions`, { method: 'GET' }, true)
        ]);
        if (!creditResponse || !creditResponse.success || !creditResponse.data) {
            if (creditResponse?.statusCode === 404) {
                renderWalletBalance({
                    customerId,
                    balance: 0,
                    currencyCode: '',
                    updatedAt: ''
                });
            }
            else {
                typedWindow.UserPanelAlerts?.showError('wallet-alert-error', creditResponse?.message ?? 'Could not load wallet balance.');
                renderWalletBalance(null);
            }
        }
        else {
            renderWalletBalance(creditResponse.data);
        }
        if (!transactionsResponse || !transactionsResponse.success || !transactionsResponse.data) {
            renderWalletTransactions([]);
            return;
        }
        renderWalletTransactions(transactionsResponse.data);
    }
    async function getWalletCustomerId() {
        const typedWindow = window;
        const response = await typedWindow.UserPanelApi?.request('/MyAccount/me', { method: 'GET' }, true);
        const customerId = response?.data?.customer?.id;
        if (!response || !response.success || !customerId || customerId <= 0) {
            return null;
        }
        return customerId;
    }
    function renderWalletBalance(data) {
        const box = document.getElementById('wallet-balance-box');
        if (!box) {
            return;
        }
        if (!data) {
            box.innerHTML = '<div class="text-muted">Wallet balance is unavailable.</div>';
            return;
        }
        box.innerHTML = `
        <div class="display-6 mb-1">${data.balance.toFixed(2)} ${escapeWalletText(data.currencyCode)}</div>
        <div class="text-muted small">Last updated: ${formatWalletDate(data.updatedAt)}</div>
    `;
    }
    function renderWalletTransactions(items) {
        const tableBody = document.getElementById('wallet-transactions-table-body');
        if (!tableBody) {
            return;
        }
        if (items.length === 0) {
            tableBody.innerHTML = '<tr><td colspan="5" class="text-center text-muted">No wallet transactions found.</td></tr>';
            return;
        }
        tableBody.innerHTML = items
            .sort((a, b) => Date.parse(b.createdAt) - Date.parse(a.createdAt))
            .map((item) => `<tr>
            <td>${formatWalletDate(item.createdAt)}</td>
            <td>${escapeWalletText(item.type)}</td>
            <td>${item.amount.toFixed(2)}</td>
            <td>${item.balanceAfter.toFixed(2)}</td>
            <td>${escapeWalletText(item.description)}</td>
        </tr>`)
            .join('');
    }
    function formatWalletDate(value) {
        const date = new Date(value);
        if (Number.isNaN(date.getTime())) {
            return '-';
        }
        return `${date.toLocaleDateString()} ${date.toLocaleTimeString()}`;
    }
    function escapeWalletText(value) {
        return value
            .replace(/&/g, '&amp;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;')
            .replace(/'/g, '&#039;');
    }
    function setupWalletObserver() {
        initializeWalletPage();
        const observer = new MutationObserver(() => {
            initializeWalletPage();
        });
        observer.observe(document.body, { childList: true, subtree: true });
    }
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', setupWalletObserver);
    }
    else {
        setupWalletObserver();
    }
})();
//# sourceMappingURL=wallet.js.map
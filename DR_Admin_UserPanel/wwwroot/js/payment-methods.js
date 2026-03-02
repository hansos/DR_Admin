"use strict";
(() => {
    let paymentMethodsCustomerId = null;
    function initializePaymentMethodsPage() {
        const page = document.getElementById('payment-methods-page');
        if (!page || page.dataset.initialized === 'true') {
            return;
        }
        page.dataset.initialized = 'true';
        document.getElementById('payment-methods-create-form')?.addEventListener('submit', (event) => {
            event.preventDefault();
            void createPaymentMethod();
        });
        void loadPaymentMethods();
    }
    async function loadPaymentMethods() {
        const typedWindow = window;
        typedWindow.UserPanelAlerts?.hide('payment-methods-alert-success');
        typedWindow.UserPanelAlerts?.hide('payment-methods-alert-error');
        const customerId = await getPaymentMethodsCustomerId();
        if (!customerId) {
            typedWindow.UserPanelAlerts?.showError('payment-methods-alert-error', 'Could not resolve customer for payment methods.');
            renderPaymentMethods([]);
            return;
        }
        const response = await typedWindow.UserPanelApi?.request(`/CustomerPaymentMethods/customer/${customerId}`, { method: 'GET' }, true);
        if (!response || !response.success || !response.data) {
            typedWindow.UserPanelAlerts?.showError('payment-methods-alert-error', response?.message ?? 'Could not load payment methods.');
            renderPaymentMethods([]);
            return;
        }
        renderPaymentMethods(response.data);
    }
    async function getPaymentMethodsCustomerId() {
        if (paymentMethodsCustomerId && paymentMethodsCustomerId > 0) {
            return paymentMethodsCustomerId;
        }
        const typedWindow = window;
        const response = await typedWindow.UserPanelApi?.request('/MyAccount/me', { method: 'GET' }, true);
        const customerId = response?.data?.customer?.id;
        if (!response || !response.success || !customerId || customerId <= 0) {
            return null;
        }
        paymentMethodsCustomerId = customerId;
        return customerId;
    }
    function renderPaymentMethods(items) {
        const tableBody = document.getElementById('payment-methods-table-body');
        if (!tableBody) {
            return;
        }
        if (items.length === 0) {
            tableBody.innerHTML = '<tr><td colspan="5" class="text-center text-muted">No payment methods found.</td></tr>';
            return;
        }
        tableBody.innerHTML = items
            .sort((a, b) => Date.parse(b.createdAt) - Date.parse(a.createdAt))
            .map((item) => {
            const methodText = [item.cardBrand, item.last4Digits ? `•••• ${item.last4Digits}` : '', item.expiryMonth && item.expiryYear ? `${item.expiryMonth}/${item.expiryYear}` : '']
                .filter((value) => value && value.trim().length > 0)
                .join(' ');
            const status = [
                item.isDefault ? '<span class="badge bg-primary me-1">Default</span>' : '',
                item.isVerified ? '<span class="badge bg-success me-1">Verified</span>' : '<span class="badge bg-warning text-dark me-1">Unverified</span>',
                item.isActive ? '<span class="badge bg-secondary">Active</span>' : '<span class="badge bg-danger">Inactive</span>'
            ].join(' ');
            return `<tr>
                <td>${escapePaymentMethodsText(methodText || String(item.type))}</td>
                <td>${escapePaymentMethodsText(item.paymentGatewayName || `Gateway #${item.paymentGatewayId}`)}</td>
                <td>${status}</td>
                <td>${formatPaymentMethodsDate(item.createdAt)}</td>
                <td>
                    <div class="btn-group btn-group-sm" role="group">
                        <button class="btn btn-outline-primary" type="button" data-action="default" data-id="${item.id}">Set default</button>
                        <button class="btn btn-outline-danger" type="button" data-action="delete" data-id="${item.id}">Delete</button>
                    </div>
                </td>
            </tr>`;
        })
            .join('');
        tableBody.querySelectorAll('button[data-action="default"]').forEach((button) => {
            button.addEventListener('click', () => {
                const id = Number.parseInt(button.dataset.id ?? '0', 10);
                if (id > 0) {
                    void setDefaultPaymentMethod(id);
                }
            });
        });
        tableBody.querySelectorAll('button[data-action="delete"]').forEach((button) => {
            button.addEventListener('click', () => {
                const id = Number.parseInt(button.dataset.id ?? '0', 10);
                if (id > 0) {
                    void deletePaymentMethod(id);
                }
            });
        });
    }
    async function createPaymentMethod() {
        const typedWindow = window;
        typedWindow.UserPanelAlerts?.hide('payment-methods-alert-success');
        typedWindow.UserPanelAlerts?.hide('payment-methods-alert-error');
        const customerId = await getPaymentMethodsCustomerId();
        if (!customerId) {
            typedWindow.UserPanelAlerts?.showError('payment-methods-alert-error', 'Could not resolve customer for payment method creation.');
            return;
        }
        const gatewayId = Number.parseInt(readPaymentMethodsInputValue('payment-methods-gateway-id'), 10);
        const type = Number.parseInt(readPaymentMethodsInputValue('payment-methods-type'), 10);
        const token = readPaymentMethodsInputValue('payment-methods-token');
        const isDefault = document.getElementById('payment-methods-default')?.checked ?? false;
        if (Number.isNaN(gatewayId) || gatewayId <= 0 || Number.isNaN(type) || type <= 0 || !token) {
            typedWindow.UserPanelAlerts?.showError('payment-methods-alert-error', 'Gateway, type, and token are required.');
            return;
        }
        const response = await typedWindow.UserPanelApi?.request('/CustomerPaymentMethods', {
            method: 'POST',
            body: JSON.stringify({
                customerId,
                paymentGatewayId: gatewayId,
                type,
                paymentMethodToken: token,
                isDefault
            })
        }, true);
        if (!response || !response.success) {
            typedWindow.UserPanelAlerts?.showError('payment-methods-alert-error', response?.message ?? 'Could not create payment method.');
            return;
        }
        clearPaymentMethodsForm();
        typedWindow.UserPanelAlerts?.showSuccess('payment-methods-alert-success', response.message ?? 'Payment method added.');
        await loadPaymentMethods();
    }
    async function setDefaultPaymentMethod(paymentMethodId) {
        const typedWindow = window;
        const customerId = await getPaymentMethodsCustomerId();
        if (!customerId) {
            typedWindow.UserPanelAlerts?.showError('payment-methods-alert-error', 'Could not resolve customer for default update.');
            return;
        }
        const response = await typedWindow.UserPanelApi?.request(`/CustomerPaymentMethods/${paymentMethodId}/set-default?customerId=${customerId}`, {
            method: 'POST'
        }, true);
        if (!response || !response.success) {
            typedWindow.UserPanelAlerts?.showError('payment-methods-alert-error', response?.message ?? 'Could not update default payment method.');
            return;
        }
        typedWindow.UserPanelAlerts?.showSuccess('payment-methods-alert-success', response.message ?? 'Default payment method updated.');
        await loadPaymentMethods();
    }
    async function deletePaymentMethod(paymentMethodId) {
        const typedWindow = window;
        const customerId = await getPaymentMethodsCustomerId();
        if (!customerId) {
            typedWindow.UserPanelAlerts?.showError('payment-methods-alert-error', 'Could not resolve customer for deletion.');
            return;
        }
        const response = await typedWindow.UserPanelApi?.request(`/CustomerPaymentMethods/${paymentMethodId}?customerId=${customerId}`, {
            method: 'DELETE'
        }, true);
        if (!response || !response.success) {
            typedWindow.UserPanelAlerts?.showError('payment-methods-alert-error', response?.message ?? 'Could not delete payment method.');
            return;
        }
        typedWindow.UserPanelAlerts?.showSuccess('payment-methods-alert-success', 'Payment method deleted.');
        await loadPaymentMethods();
    }
    function clearPaymentMethodsForm() {
        const tokenInput = document.getElementById('payment-methods-token');
        const isDefault = document.getElementById('payment-methods-default');
        if (tokenInput) {
            tokenInput.value = '';
        }
        if (isDefault) {
            isDefault.checked = false;
        }
    }
    function readPaymentMethodsInputValue(id) {
        const input = document.getElementById(id);
        return input?.value.trim() ?? '';
    }
    function formatPaymentMethodsDate(value) {
        const date = new Date(value);
        if (Number.isNaN(date.getTime())) {
            return '-';
        }
        return `${date.toLocaleDateString()} ${date.toLocaleTimeString()}`;
    }
    function escapePaymentMethodsText(value) {
        return value
            .replace(/&/g, '&amp;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;')
            .replace(/'/g, '&#039;');
    }
    function setupPaymentMethodsObserver() {
        initializePaymentMethodsPage();
        const observer = new MutationObserver(() => {
            initializePaymentMethodsPage();
        });
        observer.observe(document.body, { childList: true, subtree: true });
    }
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', setupPaymentMethodsObserver);
    }
    else {
        setupPaymentMethodsObserver();
    }
})();
//# sourceMappingURL=payment-methods.js.map
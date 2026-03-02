"use strict";
function initializeCheckout() {
    const page = document.getElementById('checkout-page');
    if (!page || page.dataset.bound === 'true') {
        return;
    }
    page.dataset.bound = 'true';
    renderSummary();
    renderPaymentStatusFromQuery();
    const form = document.getElementById('checkout-form');
    form?.addEventListener('submit', async (event) => {
        event.preventDefault();
        await submitCheckout();
    });
}
function renderSummary() {
    const typedWindow = window;
    const state = typedWindow.UserPanelCart?.getState();
    const container = document.getElementById('checkout-summary-lines');
    if (!container || !state) {
        return;
    }
    let oneTimeTotal = 0;
    let recurringTotal = 0;
    const lines = [];
    if (state.domain) {
        oneTimeTotal += state.domain.premiumPrice;
        lines.push(`<div class="d-flex justify-content-between"><span>Domain</span><span>${state.domain.premiumPrice.toFixed(2)}</span></div>`);
    }
    state.hosting.forEach((item) => {
        const amount = item.billingCycle === 'yearly' ? item.yearlyPrice : item.monthlyPrice;
        recurringTotal += amount;
        lines.push(`<div class="d-flex justify-content-between"><span>Hosting #${item.id}</span><span>${amount.toFixed(2)}</span></div>`);
    });
    state.services.forEach((item) => {
        recurringTotal += item.price;
        lines.push(`<div class="d-flex justify-content-between"><span>Service #${item.id}</span><span>${item.price.toFixed(2)}</span></div>`);
    });
    lines.push('<hr/>');
    lines.push(`<div class="d-flex justify-content-between"><span>Discount</span><span>- ${state.discount.toFixed(2)}</span></div>`);
    lines.push(`<div class="d-flex justify-content-between fw-semibold"><span>Total now</span><span>${Math.max(0, oneTimeTotal + recurringTotal - state.discount).toFixed(2)}</span></div>`);
    container.innerHTML = lines.join('');
}
async function submitCheckout() {
    const typedWindow = window;
    typedWindow.UserPanelAlerts?.hide('checkout-alert-success');
    typedWindow.UserPanelAlerts?.hide('checkout-alert-error');
    const customerIdValue = document.getElementById('checkout-customer-id')?.value ?? '';
    const customerId = Number.parseInt(customerIdValue, 10);
    const confirmed = document.getElementById('checkout-confirm')?.checked ?? false;
    if (Number.isNaN(customerId) || customerId <= 0) {
        typedWindow.UserPanelAlerts?.showError('checkout-alert-error', 'Customer ID is required.');
        return;
    }
    if (!confirmed) {
        typedWindow.UserPanelAlerts?.showError('checkout-alert-error', 'You must confirm the order details before placing the order.');
        return;
    }
    const state = typedWindow.UserPanelCart?.getState();
    if (!state) {
        typedWindow.UserPanelAlerts?.showError('checkout-alert-error', 'Cart could not be loaded.');
        return;
    }
    const recurringAmount = state.hosting.reduce((sum, item) => sum + (item.billingCycle === 'yearly' ? item.yearlyPrice : item.monthlyPrice), 0)
        + state.services.reduce((sum, item) => sum + item.price, 0)
        - state.discount;
    const payload = {
        customerId,
        serviceId: state.hosting[0]?.id ?? null,
        quoteId: null,
        orderType: 'New',
        startDate: new Date().toISOString(),
        endDate: new Date(Date.now() + (365 * 24 * 60 * 60 * 1000)).toISOString(),
        nextBillingDate: new Date(Date.now() + (30 * 24 * 60 * 60 * 1000)).toISOString(),
        setupFee: state.domain?.premiumPrice ?? 0,
        recurringAmount: Math.max(0, recurringAmount),
        couponCode: null,
        autoRenew: true
    };
    const response = await typedWindow.UserPanelApi?.request('/Orders', {
        method: 'POST',
        body: JSON.stringify(payload)
    }, true);
    if (!response || !response.success || !response.data) {
        typedWindow.UserPanelAlerts?.showError('checkout-alert-error', response?.message ?? 'Could not place order.');
        return;
    }
    typedWindow.UserPanelAlerts?.showSuccess('checkout-alert-success', `Order created: ${response.data.orderNumber}`);
    const status = document.getElementById('checkout-payment-status');
    if (status) {
        status.textContent = 'Order created. Payment initialization is pending API integration.';
    }
}
function renderPaymentStatusFromQuery() {
    const status = new URLSearchParams(window.location.search).get('paymentStatus');
    const container = document.getElementById('checkout-payment-status');
    if (!container || !status) {
        return;
    }
    const normalized = status.toLowerCase();
    if (normalized === 'success') {
        container.textContent = 'Payment completed successfully.';
    }
    else if (normalized === 'failed') {
        container.textContent = 'Payment failed. Please retry.';
    }
    else {
        container.textContent = 'Payment is pending confirmation.';
    }
}
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initializeCheckout);
}
else {
    initializeCheckout();
}
//# sourceMappingURL=checkout.js.map
"use strict";
const checkoutSuccessOrderMarkerStorageKey = 'up_checkout_last_added_order';
function initializeCheckoutSuccessPage() {
    const page = document.getElementById('checkout-success-page');
    if (!page || page.dataset.initialized === 'true') {
        return;
    }
    page.dataset.initialized = 'true';
    const marker = getCheckoutSuccessStoredOrderMarker();
    const query = new URLSearchParams(window.location.search);
    const status = (query.get('paymentStatus') ?? 'success').trim().toLowerCase();
    renderCheckoutSuccessStatus(status);
    renderCheckoutSuccessOrders(marker, query);
    if (status === 'success') {
        const typedWindow = window;
        typedWindow.UserPanelCart?.clear();
    }
}
function getCheckoutSuccessStoredOrderMarker() {
    try {
        const raw = sessionStorage.getItem(checkoutSuccessOrderMarkerStorageKey);
        if (!raw) {
            return null;
        }
        return JSON.parse(raw);
    }
    catch {
        return null;
    }
}
function renderCheckoutSuccessStatus(status) {
    const badge = document.getElementById('checkout-success-badge');
    const statusText = document.getElementById('checkout-success-payment-status-text');
    const message = document.getElementById('checkout-success-message');
    const subtitle = document.getElementById('checkout-success-subtitle');
    const primaryLink = document.getElementById('checkout-success-primary-link');
    const nextSteps = document.getElementById('checkout-success-next-steps');
    if (!badge || !statusText || !message || !subtitle || !primaryLink || !nextSteps) {
        return;
    }
    if (status === 'failed') {
        badge.className = 'badge bg-danger';
        badge.textContent = 'Failed';
        statusText.textContent = 'Payment failed';
        message.textContent = 'Your payment was not completed. Retry payment from invoices or start checkout again.';
        subtitle.textContent = 'Resolve payment to activate services.';
        primaryLink.href = '/billing/invoices';
        primaryLink.textContent = 'Go to invoices';
        nextSteps.innerHTML = '<li>Open invoices and retry payment for the order.</li><li>Return to checkout if you need to change payment instrument.</li>';
        return;
    }
    if (status === 'pending') {
        badge.className = 'badge bg-warning text-dark';
        badge.textContent = 'Pending';
        statusText.textContent = 'Payment pending confirmation';
        message.textContent = 'Your payment is pending verification. We will activate services when payment is confirmed.';
        subtitle.textContent = 'Track payment state from invoices.';
        primaryLink.href = '/billing/invoices';
        primaryLink.textContent = 'Track in invoices';
        nextSteps.innerHTML = '<li>Check invoices for updated payment status.</li><li>Go to domains to verify activation when status becomes paid.</li>';
        return;
    }
    badge.className = 'badge bg-success';
    badge.textContent = 'Paid';
    statusText.textContent = 'Payment confirmed';
    message.textContent = 'Payment is successful. Domain registration and provisioning are being synchronized.';
    subtitle.textContent = 'Continue to domains or review invoice details.';
    primaryLink.href = '/domains';
    primaryLink.textContent = 'Go to domains';
    nextSteps.innerHTML = '<li>Open domains to verify registration status.</li><li>Open invoices for payment receipt and accounting records.</li>';
}
function renderCheckoutSuccessOrders(marker, query) {
    const orderIdElement = document.getElementById('checkout-success-order-id');
    const orderNumbersElement = document.getElementById('checkout-success-order-numbers');
    if (!orderIdElement || !orderNumbersElement) {
        return;
    }
    const queryOrderId = Number.parseInt(query.get('orderId') ?? '', 10);
    const orderId = !Number.isNaN(queryOrderId) && queryOrderId > 0
        ? queryOrderId
        : (marker?.orderId ?? 0);
    orderIdElement.textContent = orderId > 0 ? orderId.toString() : '-';
    const queryOrderNumbers = (query.get('orderNumbers') ?? '')
        .split(',')
        .map((value) => value.trim())
        .filter((value) => value.length > 0);
    const markerOrderNumbers = (Array.isArray(marker?.orderNumbers) && marker.orderNumbers.length > 0)
        ? marker.orderNumbers
        : (marker?.orderNumber ? [marker.orderNumber] : []);
    const orderNumbers = queryOrderNumbers.length > 0 ? queryOrderNumbers : markerOrderNumbers;
    if (orderNumbers.length === 0) {
        orderNumbersElement.textContent = 'No order data available.';
        return;
    }
    orderNumbersElement.innerHTML = orderNumbers
        .map((value) => `<div>${escapeCheckoutSuccessText(value)}</div>`)
        .join('');
}
function escapeCheckoutSuccessText(value) {
    return value
        .replace(/&/g, '&amp;')
        .replace(/</g, '&lt;')
        .replace(/>/g, '&gt;')
        .replace(/"/g, '&quot;')
        .replace(/'/g, '&#039;');
}
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initializeCheckoutSuccessPage);
}
else {
    initializeCheckoutSuccessPage();
}
//# sourceMappingURL=checkout-success.js.map
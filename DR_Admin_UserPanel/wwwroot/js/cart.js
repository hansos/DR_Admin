"use strict";
function initializeCart() {
    const page = document.getElementById('cart-page');
    if (!page || page.dataset.bound === 'true') {
        return;
    }
    page.dataset.bound = 'true';
    const discountInput = document.getElementById('cart-discount');
    discountInput?.addEventListener('input', () => {
        const state = getCartState();
        const value = Number.parseFloat(discountInput.value);
        state.discount = Number.isNaN(value) ? 0 : Math.max(0, value);
        saveCartState(state);
        renderCart(state);
    });
    void loadOptions();
    const initialState = getCartState();
    if (discountInput) {
        discountInput.value = initialState.discount.toString();
    }
    renderCart(initialState);
    applyFlowFocus();
}
function applyFlowFocus() {
    const focus = new URLSearchParams(window.location.search).get('focus');
    if (focus !== 'services') {
        return;
    }
    const servicesCard = document.getElementById('cart-services-card');
    if (!servicesCard) {
        return;
    }
    servicesCard.scrollIntoView({ behavior: 'smooth', block: 'start' });
    servicesCard.classList.add('border', 'border-primary');
    window.setTimeout(() => {
        servicesCard.classList.remove('border', 'border-primary');
    }, 2400);
    const typedWindow = window;
    typedWindow.UserPanelAlerts?.showSuccess('cart-alert-success', 'Great choice. Add optional services to complete your domain bundle.');
}
async function loadOptions() {
    const typedWindow = window;
    const [hostingResponse, servicesResponse] = await Promise.all([
        typedWindow.UserPanelApi?.request('/HostingPackages/active', { method: 'GET' }, true),
        typedWindow.UserPanelApi?.request('/Services', { method: 'GET' }, true)
    ]);
    if (!hostingResponse?.success) {
        typedWindow.UserPanelAlerts?.showError('cart-alert-error', hostingResponse?.message ?? 'Could not load hosting packages.');
    }
    else {
        renderHostingOptions(hostingResponse.data ?? []);
    }
    if (!servicesResponse?.success) {
        typedWindow.UserPanelAlerts?.showError('cart-alert-error', servicesResponse?.message ?? 'Could not load optional services.');
    }
    else {
        renderServiceOptions(servicesResponse.data ?? []);
    }
}
function renderHostingOptions(items) {
    const hostList = document.getElementById('cart-hosting-list');
    if (!hostList) {
        return;
    }
    hostList.innerHTML = '';
    const state = getCartState();
    items.forEach((item) => {
        const container = document.createElement('div');
        container.className = 'list-group-item d-flex flex-wrap justify-content-between align-items-center gap-2';
        const selected = state.hosting.some((entry) => entry.id === item.id);
        container.innerHTML = `
            <div>
                <div class="fw-semibold">${item.name}</div>
                <div class="small text-muted">Monthly ${item.monthlyPrice.toFixed(2)} / Yearly ${item.yearlyPrice.toFixed(2)}</div>
            </div>
            <div class="d-flex gap-2 align-items-center">
                <select class="form-select form-select-sm" data-role="billing-cycle">
                    <option value="monthly">Monthly</option>
                    <option value="yearly">Yearly</option>
                </select>
                <button class="btn btn-sm ${selected ? 'btn-outline-danger' : 'btn-outline-primary'}" type="button" data-role="toggle">${selected ? 'Remove' : 'Add'}</button>
            </div>
        `;
        const cycleSelect = container.querySelector('[data-role="billing-cycle"]');
        const toggleButton = container.querySelector('[data-role="toggle"]');
        const selectedEntry = state.hosting.find((entry) => entry.id === item.id);
        cycleSelect.value = selectedEntry?.billingCycle ?? 'monthly';
        toggleButton.addEventListener('click', () => {
            const current = getCartState();
            const existingIndex = current.hosting.findIndex((entry) => entry.id === item.id);
            if (existingIndex >= 0) {
                current.hosting.splice(existingIndex, 1);
            }
            else {
                current.hosting.push({
                    id: item.id,
                    name: item.name,
                    monthlyPrice: item.monthlyPrice,
                    yearlyPrice: item.yearlyPrice,
                    billingCycle: cycleSelect.value === 'yearly' ? 'yearly' : 'monthly'
                });
            }
            saveCartState(current);
            renderCart(current);
            renderHostingOptions(items);
        });
        cycleSelect.addEventListener('change', () => {
            const current = getCartState();
            const existing = current.hosting.find((entry) => entry.id === item.id);
            if (!existing) {
                return;
            }
            existing.billingCycle = cycleSelect.value === 'yearly' ? 'yearly' : 'monthly';
            saveCartState(current);
            renderCart(current);
        });
        hostList.appendChild(container);
    });
}
function renderServiceOptions(items) {
    const list = document.getElementById('cart-services-list');
    if (!list) {
        return;
    }
    list.innerHTML = '';
    const state = getCartState();
    items.forEach((item) => {
        const selected = state.services.some((entry) => entry.id === item.id);
        const price = typeof item.price === 'number' ? item.price : 0;
        const row = document.createElement('div');
        row.className = 'list-group-item d-flex justify-content-between align-items-center gap-2';
        row.innerHTML = `
            <div>
                <div class="fw-semibold">${item.name}</div>
                <div class="small text-muted">${price.toFixed(2)}</div>
            </div>
            <button class="btn btn-sm ${selected ? 'btn-outline-danger' : 'btn-outline-primary'}" type="button">${selected ? 'Remove' : 'Add'}</button>
        `;
        const button = row.querySelector('button');
        button.addEventListener('click', () => {
            const current = getCartState();
            const existingIndex = current.services.findIndex((entry) => entry.id === item.id);
            if (existingIndex >= 0) {
                current.services.splice(existingIndex, 1);
            }
            else {
                current.services.push({ id: item.id, name: item.name, price });
            }
            saveCartState(current);
            renderCart(current);
            renderServiceOptions(items);
        });
        list.appendChild(row);
    });
}
function renderCart(state) {
    const domainSummary = document.getElementById('cart-domain-summary');
    if (domainSummary) {
        domainSummary.textContent = state.domain
            ? `${state.domain.domainName} (${state.domain.periodYears} year)${state.domain.includePrivacy ? ' + privacy' : ''}`
            : 'No domain selected.';
    }
    const orderLines = document.getElementById('cart-order-lines');
    const oneTimeEl = document.getElementById('cart-total-one-time');
    const recurringEl = document.getElementById('cart-total-recurring');
    const grandEl = document.getElementById('cart-total-grand');
    let oneTimeTotal = 0;
    let recurringTotal = 0;
    const lines = [];
    if (state.domain) {
        oneTimeTotal += state.domain.premiumPrice;
        lines.push(`<div class="d-flex justify-content-between"><span>Domain: ${state.domain.domainName}</span><span>${state.domain.premiumPrice.toFixed(2)}</span></div>`);
    }
    state.hosting.forEach((hosting) => {
        const amount = hosting.billingCycle === 'yearly' ? hosting.yearlyPrice : hosting.monthlyPrice;
        recurringTotal += amount;
        lines.push(`<div class="d-flex justify-content-between"><span>Hosting: ${hosting.name} (${hosting.billingCycle})</span><span>${amount.toFixed(2)}</span></div>`);
    });
    state.services.forEach((service) => {
        recurringTotal += service.price;
        lines.push(`<div class="d-flex justify-content-between"><span>Service: ${service.name}</span><span>${service.price.toFixed(2)}</span></div>`);
    });
    const grandTotal = Math.max(0, oneTimeTotal + recurringTotal - state.discount);
    if (orderLines) {
        orderLines.innerHTML = lines.length > 0 ? lines.join('') : '<div class="text-muted">No order lines selected yet.</div>';
    }
    if (oneTimeEl) {
        oneTimeEl.textContent = oneTimeTotal.toFixed(2);
    }
    if (recurringEl) {
        recurringEl.textContent = recurringTotal.toFixed(2);
    }
    if (grandEl) {
        grandEl.textContent = grandTotal.toFixed(2);
    }
}
function getCartState() {
    const typedWindow = window;
    return typedWindow.UserPanelCart?.getState() ?? { domain: null, hosting: [], services: [], discount: 0 };
}
function saveCartState(state) {
    const typedWindow = window;
    typedWindow.UserPanelCart?.saveState(state);
}
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initializeCart);
}
else {
    initializeCart();
}
//# sourceMappingURL=cart.js.map
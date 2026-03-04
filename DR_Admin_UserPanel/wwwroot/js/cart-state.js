"use strict";
const cartStorageKey = 'up_cart_state';
function emptyCartState() {
    return {
        domain: null,
        hosting: [],
        services: [],
        discount: 0
    };
}
function getState() {
    const raw = sessionStorage.getItem(cartStorageKey);
    if (!raw) {
        return emptyCartState();
    }
    try {
        const parsed = JSON.parse(raw);
        return {
            domain: parsed.domain ?? null,
            hosting: Array.isArray(parsed.hosting) ? parsed.hosting : [],
            services: Array.isArray(parsed.services) ? parsed.services : [],
            discount: typeof parsed.discount === 'number' ? parsed.discount : 0
        };
    }
    catch {
        return emptyCartState();
    }
}
function saveState(state) {
    sessionStorage.setItem(cartStorageKey, JSON.stringify(state));
    window.dispatchEvent(new Event('up:cart-changed'));
}
function clear() {
    sessionStorage.removeItem(cartStorageKey);
    window.dispatchEvent(new Event('up:cart-changed'));
}
const cartWindow = window;
cartWindow.UserPanelCart = {
    getState,
    saveState,
    clear
};
//# sourceMappingURL=cart-state.js.map
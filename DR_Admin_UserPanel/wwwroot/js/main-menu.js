"use strict";
const mainMenuStorageKey = 'up_main_menu_expanded_section';
function getMenuContainer() {
    return document.getElementById('user-menu');
}
function hasCartOrderLines() {
    try {
        const raw = sessionStorage.getItem('up_cart_state');
        if (!raw) {
            return false;
        }
        const state = JSON.parse(raw);
        const hasDomain = state.domain !== null && state.domain !== undefined;
        const hasHosting = Array.isArray(state.hosting) && state.hosting.length > 0;
        const hasServices = Array.isArray(state.services) && state.services.length > 0;
        return hasDomain || hasHosting || hasServices;
    }
    catch {
        return false;
    }
}
function updateCheckoutMenuAvailability() {
    const checkoutLink = document.querySelector('#user-menu .nav-subitems a.nav-link-item[href="/shop/checkout"]');
    if (!checkoutLink) {
        return;
    }
    const enabled = hasCartOrderLines();
    checkoutLink.classList.toggle('disabled', !enabled);
    checkoutLink.setAttribute('aria-disabled', enabled ? 'false' : 'true');
    if (!enabled) {
        checkoutLink.setAttribute('tabindex', '-1');
        if (window.location.pathname.toLowerCase() === '/shop/checkout') {
            window.location.href = '/shop/domain-search';
        }
        return;
    }
    checkoutLink.removeAttribute('tabindex');
}
function persistExpandedSection(section) {
    try {
        if (section) {
            localStorage.setItem(mainMenuStorageKey, section);
            return;
        }
        localStorage.removeItem(mainMenuStorageKey);
    }
    catch {
    }
}
function expandGroup(group) {
    const menu = getMenuContainer();
    if (!menu) {
        return;
    }
    const groups = menu.querySelectorAll('.nav-group');
    groups.forEach((node) => {
        const element = node;
        if (element === group) {
            element.classList.add('expanded');
            return;
        }
        element.classList.remove('expanded');
    });
    persistExpandedSection(group.dataset.section ?? null);
}
function updateActiveLink() {
    const menu = getMenuContainer();
    if (!menu) {
        return null;
    }
    const currentPath = window.location.pathname.toLowerCase();
    const links = menu.querySelectorAll('.nav-subitems a.nav-link-item');
    let matchedGroup = null;
    links.forEach((linkNode) => {
        const link = linkNode;
        const href = (link.getAttribute('href') ?? '').toLowerCase();
        if (!href || href === '#') {
            link.classList.remove('active');
            return;
        }
        const isMatch = currentPath === href || currentPath.startsWith(`${href}/`);
        if (isMatch) {
            link.classList.add('active');
            if (!matchedGroup) {
                matchedGroup = link.closest('.nav-group');
            }
            return;
        }
        link.classList.remove('active');
    });
    return matchedGroup;
}
function restoreExpandedGroup() {
    const menu = getMenuContainer();
    if (!menu) {
        return false;
    }
    let section = null;
    try {
        section = localStorage.getItem(mainMenuStorageKey);
    }
    catch {
    }
    if (!section) {
        return false;
    }
    const group = menu.querySelector(`.nav-group[data-section="${section}"]`);
    if (!group) {
        return false;
    }
    expandGroup(group);
    return true;
}
function toggleGroup(group) {
    expandGroup(group);
}
function bindGroupToggleEvents() {
    document.addEventListener('click', (event) => {
        const target = event.target;
        const subLink = target.closest('#user-menu .nav-subitems a.nav-link-item');
        if (subLink) {
            if (subLink.classList.contains('disabled')) {
                event.preventDefault();
                return;
            }
            const group = subLink.closest('.nav-group');
            if (group) {
                expandGroup(group);
            }
            const menu = getMenuContainer();
            if (menu) {
                const links = menu.querySelectorAll('.nav-subitems a.nav-link-item');
                links.forEach((linkNode) => {
                    const link = linkNode;
                    if (link === subLink) {
                        link.classList.add('active');
                        return;
                    }
                    link.classList.remove('active');
                });
            }
            return;
        }
        const header = target.closest('#user-menu .nav-group-header');
        if (!header) {
            return;
        }
        event.preventDefault();
        const group = header.closest('.nav-group');
        if (!group) {
            return;
        }
        toggleGroup(group);
    });
}
let initialized = false;
function initializeMainMenu() {
    const menu = getMenuContainer();
    if (!menu) {
        return;
    }
    if (!initialized) {
        bindGroupToggleEvents();
        window.addEventListener('up:cart-changed', updateCheckoutMenuAvailability);
        initialized = true;
    }
    updateCheckoutMenuAvailability();
    const activeGroup = updateActiveLink();
    if (activeGroup) {
        expandGroup(activeGroup);
        return;
    }
    if (restoreExpandedGroup()) {
        return;
    }
    const firstGroup = menu.querySelector('.nav-group');
    if (firstGroup) {
        expandGroup(firstGroup);
    }
}
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initializeMainMenu);
}
else {
    initializeMainMenu();
}
window.addEventListener('popstate', initializeMainMenu);
const menuWindow = window;
function registerEnhancedLoadListener() {
    if (menuWindow.Blazor?.addEventListener) {
        menuWindow.Blazor.addEventListener('enhancedload', initializeMainMenu);
        return;
    }
    window.setTimeout(registerEnhancedLoadListener, 100);
}
registerEnhancedLoadListener();
//# sourceMappingURL=main-menu.js.map
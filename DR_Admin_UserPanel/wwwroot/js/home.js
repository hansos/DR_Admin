"use strict";
function initializeHomePage() {
    const form = document.getElementById('home-domain-search-form');
    if (!form || form.dataset.bound === 'true') {
        return;
    }
    form.dataset.bound = 'true';
    form.addEventListener('submit', (event) => {
        event.preventDefault();
        const input = document.getElementById('home-domain-search-input');
        const domain = input?.value.trim() ?? '';
        if (!domain) {
            input?.focus();
            return;
        }
        window.location.href = `/shop/domain-search?domain=${encodeURIComponent(domain)}`;
    });
}
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initializeHomePage);
}
else {
    initializeHomePage();
}
function registerHomeEnhancedLoadListener() {
    const typedWindow = window;
    if (typedWindow.Blazor?.addEventListener) {
        typedWindow.Blazor.addEventListener('enhancedload', initializeHomePage);
        return;
    }
    window.setTimeout(registerHomeEnhancedLoadListener, 100);
}
registerHomeEnhancedLoadListener();
//# sourceMappingURL=home.js.map
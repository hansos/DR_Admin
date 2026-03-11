"use strict";
(() => {
    const pageId = 'tax-categories-page';
    function showSuccess(message) {
        const success = document.getElementById('tax-categories-alert-success');
        const error = document.getElementById('tax-categories-alert-error');
        if (success) {
            success.textContent = message;
            success.classList.remove('d-none');
        }
        error?.classList.add('d-none');
    }
    function initializePage() {
        const page = document.getElementById(pageId);
        if (!page || page.dataset.initialized === 'true') {
            return;
        }
        page.dataset.initialized = 'true';
        document.getElementById('tax-categories-refresh')?.addEventListener('click', () => {
            showSuccess(`Refreshed at ${new Date().toLocaleString()}`);
        });
    }
    function setup() {
        initializePage();
        if (!document.body) {
            return;
        }
        const observer = new MutationObserver(() => initializePage());
        observer.observe(document.body, { childList: true, subtree: true });
    }
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', setup);
    }
    else {
        setup();
    }
})();
//# sourceMappingURL=tax-categories.js.map
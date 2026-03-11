"use strict";
(() => {
    const pageId = 'tax-evidence-page';
    function showSuccess(message) {
        const success = document.getElementById('tax-evidence-alert-success');
        const error = document.getElementById('tax-evidence-alert-error');
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
        const tbody = document.getElementById('tax-evidence-table-body');
        if (tbody) {
            tbody.innerHTML = '<tr><td colspan="7" class="text-center text-muted">Evidence API endpoint is not exposed yet.</td></tr>';
        }
        document.getElementById('tax-evidence-refresh')?.addEventListener('click', () => {
            showSuccess('Evidence API endpoint is not exposed yet.');
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
//# sourceMappingURL=tax-evidence.js.map
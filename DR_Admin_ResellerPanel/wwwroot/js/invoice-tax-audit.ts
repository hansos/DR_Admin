(() => {
    const pageId = 'invoice-tax-audit-page';

    function showSuccess(message: string): void {
        const success = document.getElementById('invoice-tax-audit-alert-success');
        const error = document.getElementById('invoice-tax-audit-alert-error');
        if (success) {
            success.textContent = message;
            success.classList.remove('d-none');
        }
        error?.classList.add('d-none');
    }

    function initializePage(): void {
        const page = document.getElementById(pageId);
        if (!page || page.dataset.initialized === 'true') {
            return;
        }

        page.dataset.initialized = 'true';

        document.getElementById('invoice-tax-audit-refresh')?.addEventListener('click', () => {
            showSuccess(`Refreshed at ${new Date().toLocaleString()}`);
        });
    }

    function setup(): void {
        initializePage();

        if (!document.body) {
            return;
        }

        const observer = new MutationObserver(() => initializePage());
        observer.observe(document.body, { childList: true, subtree: true });
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', setup);
    } else {
        setup();
    }
})();

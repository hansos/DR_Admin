(() => {
    const pageId = 'tax-finalize-tool-page';

    function showSuccess(message: string): void {
        const success = document.getElementById('tax-finalize-tool-alert-success');
        const error = document.getElementById('tax-finalize-tool-alert-error');
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

        document.getElementById('tax-finalize-tool-run')?.addEventListener('click', () => {
            const output = document.getElementById('tax-finalize-tool-result');
            if (output) {
                output.textContent = JSON.stringify({ message: 'Finalize request runner placeholder', executedAt: new Date().toISOString() }, null, 2);
            }
            showSuccess('Finalize tool executed.');
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

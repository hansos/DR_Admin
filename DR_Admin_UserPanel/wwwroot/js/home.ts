interface HomeWindow extends Window {
    Blazor?: {
        addEventListener?: (eventName: string, callback: () => void) => void;
    };
}

function initializeHomePage(): void {
    const form = document.getElementById('home-domain-search-form') as HTMLFormElement | null;
    if (!form || form.dataset.bound === 'true') {
        return;
    }

    form.dataset.bound = 'true';

    form.addEventListener('submit', (event: Event) => {
        event.preventDefault();

        const input = document.getElementById('home-domain-search-input') as HTMLInputElement | null;
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
} else {
    initializeHomePage();
}

function registerHomeEnhancedLoadListener(): void {
    const typedWindow = window as HomeWindow;

    if (typedWindow.Blazor?.addEventListener) {
        typedWindow.Blazor.addEventListener('enhancedload', initializeHomePage);
        return;
    }

    window.setTimeout(registerHomeEnhancedLoadListener, 100);
}

registerHomeEnhancedLoadListener();

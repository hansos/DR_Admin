interface DomainAvailabilityResult {
    success: boolean;
    domainName: string;
    isAvailable: boolean;
    isTldSupported: boolean;
    message: string;
    premiumPrice?: number;
}

interface DomainSearchWindow extends Window {
    UserPanelApi?: {
        request: <T>(path: string, options?: RequestInit, requiresAuth?: boolean) => Promise<{ success: boolean; data?: T; message?: string }>;
    };
    UserPanelAlerts?: {
        showSuccess: (id: string, message: string) => void;
        showError: (id: string, message: string) => void;
        hide: (id: string) => void;
    };
    UserPanelCart?: {
        getState: () => {
            domain: {
                domainName: string;
                registrarCode: string;
                periodYears: number;
                includePrivacy: boolean;
                premiumPrice: number;
            } | null;
            hosting: unknown[];
            services: unknown[];
            discount: number;
        };
        saveState: (state: {
            domain: {
                domainName: string;
                registrarCode: string;
                periodYears: number;
                includePrivacy: boolean;
                premiumPrice: number;
            } | null;
            hosting: unknown[];
            services: unknown[];
            discount: number;
        }) => void;
    };
}

let latestResult: DomainAvailabilityResult | null = null;

function initializeDomainSearch(): void {
    const form = document.getElementById('domain-search-form') as HTMLFormElement | null;
    if (!form || form.dataset.bound === 'true') {
        return;
    }

    form.dataset.bound = 'true';

    const addButton = document.getElementById('domain-search-add-to-cart') as HTMLButtonElement | null;
    addButton?.addEventListener('click', () => {
        addResultToCart();
    });

    form.addEventListener('submit', async (event: Event) => {
        event.preventDefault();

        const typedWindow = window as DomainSearchWindow;
        typedWindow.UserPanelAlerts?.hide('domain-search-alert-success');
        typedWindow.UserPanelAlerts?.hide('domain-search-alert-error');

        const domainName = getInputValue('domain-search-input');
        const registrarCode = getInputValue('domain-search-registrar');

        if (!domainName || !registrarCode) {
            typedWindow.UserPanelAlerts?.showError('domain-search-alert-error', 'Domain name and registrar code are required.');
            return;
        }

        const encodedDomain = encodeURIComponent(domainName);
        const encodedRegistrar = encodeURIComponent(registrarCode);

        const response = await typedWindow.UserPanelApi?.request<DomainAvailabilityResult>(`/DomainManager/registrar/${encodedRegistrar}/domain/name/${encodedDomain}/is-available`, {
            method: 'GET'
        }, true);

        if (!response || !response.success || !response.data) {
            typedWindow.UserPanelAlerts?.showError('domain-search-alert-error', response?.message ?? 'Could not check domain availability.');
            renderResult(null);
            return;
        }

        latestResult = response.data;
        renderResult(response.data);
    });
}

function renderResult(result: DomainAvailabilityResult | null): void {
    const card = document.getElementById('domain-search-result-card');
    const summary = document.getElementById('domain-search-result-summary');
    const addButton = document.getElementById('domain-search-add-to-cart');

    if (!card || !summary || !addButton) {
        return;
    }

    if (!result) {
        card.classList.add('d-none');
        addButton.classList.add('d-none');
        summary.textContent = '';
        return;
    }

    card.classList.remove('d-none');

    if (result.isAvailable) {
        summary.textContent = `${result.domainName} is available${typeof result.premiumPrice === 'number' ? ` (Premium: ${result.premiumPrice.toFixed(2)})` : ''}.`;
        addButton.classList.remove('d-none');
    } else {
        summary.textContent = result.message || `${result.domainName} is not available.`;
        addButton.classList.add('d-none');
    }
}

function addResultToCart(): void {
    if (!latestResult || !latestResult.isAvailable) {
        return;
    }

    const typedWindow = window as DomainSearchWindow;
    const periodValue = Number.parseInt(getInputValue('domain-search-period'), 10);
    const periodYears = Number.isNaN(periodValue) ? 1 : periodValue;

    const privacyInput = document.getElementById('domain-search-privacy') as HTMLInputElement | null;
    const includePrivacy = !!privacyInput?.checked;

    const state = typedWindow.UserPanelCart?.getState();
    if (!state) {
        return;
    }

    state.domain = {
        domainName: latestResult.domainName,
        registrarCode: getInputValue('domain-search-registrar'),
        periodYears,
        includePrivacy,
        premiumPrice: typeof latestResult.premiumPrice === 'number' ? latestResult.premiumPrice : 0
    };

    typedWindow.UserPanelCart?.saveState(state);
    typedWindow.UserPanelAlerts?.showSuccess('domain-search-alert-success', 'Domain added to cart.');
}

function getInputValue(id: string): string {
    const input = document.getElementById(id) as HTMLInputElement | HTMLSelectElement | null;
    return input?.value.trim() ?? '';
}

if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initializeDomainSearch);
} else {
    initializeDomainSearch();
}

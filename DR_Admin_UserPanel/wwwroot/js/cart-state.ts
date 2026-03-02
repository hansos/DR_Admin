interface CartDomainItem {
    domainName: string;
    registrarCode: string;
    periodYears: number;
    includePrivacy: boolean;
    premiumPrice: number;
}

interface CartHostingItem {
    id: number;
    name: string;
    monthlyPrice: number;
    yearlyPrice: number;
    billingCycle: 'monthly' | 'yearly';
}

interface CartServiceItem {
    id: number;
    name: string;
    price: number;
}

interface CartState {
    domain: CartDomainItem | null;
    hosting: CartHostingItem[];
    services: CartServiceItem[];
    discount: number;
}

interface UserPanelWindowCart extends Window {
    UserPanelCart?: {
        getState: () => CartState;
        saveState: (state: CartState) => void;
        clear: () => void;
    };
}

const cartStorageKey = 'up_cart_state';

function emptyCartState(): CartState {
    return {
        domain: null,
        hosting: [],
        services: [],
        discount: 0
    };
}

function getState(): CartState {
    const raw = sessionStorage.getItem(cartStorageKey);
    if (!raw) {
        return emptyCartState();
    }

    try {
        const parsed = JSON.parse(raw) as CartState;
        return {
            domain: parsed.domain ?? null,
            hosting: Array.isArray(parsed.hosting) ? parsed.hosting : [],
            services: Array.isArray(parsed.services) ? parsed.services : [],
            discount: typeof parsed.discount === 'number' ? parsed.discount : 0
        };
    } catch {
        return emptyCartState();
    }
}

function saveState(state: CartState): void {
    sessionStorage.setItem(cartStorageKey, JSON.stringify(state));
}

function clear(): void {
    sessionStorage.removeItem(cartStorageKey);
}

const cartWindow = window as UserPanelWindowCart;
cartWindow.UserPanelCart = {
    getState,
    saveState,
    clear
};

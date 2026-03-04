(function () {

interface UserPanelWindowWithBlazorEvents extends Window {
    Blazor?: {
        addEventListener?: (eventName: string, callback: () => void) => void;
    };
}

function normalizeText(value: string): string {
    return value.replace(/\s+/g, ' ').trim().toLowerCase();
}

function hasBootstrapIcon(element: HTMLElement): boolean {
    return element.querySelector('i.bi') !== null;
}

function prependBootstrapIcon(element: HTMLElement, icon: string): void {
    if (hasBootstrapIcon(element)) {
        return;
    }

    const iconElement = document.createElement('i');
    iconElement.className = `bi bi-${icon} me-2`;

    if (element.firstChild) {
        element.insertBefore(iconElement, element.firstChild);
        return;
    }

    element.appendChild(iconElement);
}

function applyMenuIcons(): void {
    const menuLinkIcons = new Map<string, string>([
        ['/dashboard', 'speedometer2'],
        ['/activity', 'activity'],
        ['/settings/notifications', 'bell'],
        ['/domains', 'globe2'],
        ['/hosting', 'hdd-rack'],
        ['/contacts', 'people'],
        ['/billing/invoices', 'receipt'],
        ['/billing/renewals', 'calendar-check'],
        ['/billing/payment-methods', 'credit-card-2-front'],
        ['/billing/wallet', 'wallet2'],
        ['/security/2fa', 'shield-lock'],
        ['/security/sessions', 'pc-display-horizontal'],
        ['/privacy', 'file-earmark-lock2'],
        ['/shop/domain-search', 'search'],
        ['/shop/checkout', 'cart-check'],
        ['/support/tickets', 'life-preserver'],
        ['/about', 'info-circle']
    ]);

    menuLinkIcons.forEach((icon, href) => {
        const selector = `#user-menu .nav-subitems a.nav-link-item[href="${href}"]`;
        const link = document.querySelector(selector) as HTMLElement | null;
        if (link) {
            prependBootstrapIcon(link, icon);
        }
    });

    const topMenuLinkIcons = new Map<string, string>([
        ['/account/settings', 'gear'],
        ['/account/change-password', 'key'],
        ['/account/customer-page', 'person-vcard'],
        ['/about-public', 'info-circle']
    ]);

    topMenuLinkIcons.forEach((icon, href) => {
        const links = document.querySelectorAll(`a[href="${href}"]`);
        links.forEach((node) => {
            const link = node as HTMLElement;
            prependBootstrapIcon(link, icon);
        });
    });
}

function applyPageTitleIcons(): void {
    const path = window.location.pathname.toLowerCase();
    const pathIconMap: Array<{ prefix: string; icon: string }> = [
        { prefix: '/account/login', icon: 'box-arrow-in-right' },
        { prefix: '/account/register', icon: 'person-plus' },
        { prefix: '/account/forgot-password', icon: 'question-circle' },
        { prefix: '/account/reset-password', icon: 'arrow-repeat' },
        { prefix: '/account/change-password', icon: 'key' },
        { prefix: '/account/settings', icon: 'gear' },
        { prefix: '/account/customer-page', icon: 'person-vcard' },
        { prefix: '/dashboard', icon: 'speedometer2' },
        { prefix: '/activity', icon: 'activity' },
        { prefix: '/shop/domain-search', icon: 'search' },
        { prefix: '/shop/checkout', icon: 'cart-check' },
        { prefix: '/domains/details', icon: 'info-circle' },
        { prefix: '/domains/dns', icon: 'hdd-network' },
        { prefix: '/domains', icon: 'globe2' },
        { prefix: '/hosting/details', icon: 'server' },
        { prefix: '/hosting', icon: 'hdd-rack' },
        { prefix: '/contacts', icon: 'people' },
        { prefix: '/billing/invoices', icon: 'receipt' },
        { prefix: '/billing/renewals', icon: 'calendar-check' },
        { prefix: '/billing/payment-methods', icon: 'credit-card-2-front' },
        { prefix: '/billing/wallet', icon: 'wallet2' },
        { prefix: '/settings/notifications', icon: 'bell' },
        { prefix: '/security/2fa', icon: 'shield-lock' },
        { prefix: '/security/sessions', icon: 'pc-display-horizontal' },
        { prefix: '/privacy', icon: 'file-earmark-lock2' },
        { prefix: '/support/tickets', icon: 'life-preserver' },
        { prefix: '/about-public', icon: 'info-circle' },
        { prefix: '/about', icon: 'info-circle' },
        { prefix: '/', icon: 'shop' }
    ];

    const pageTitle = document.querySelector('main article .container h1.h4, main article .container h1, .public-content .container h1') as HTMLElement | null;
    if (!pageTitle) {
        return;
    }

    const matched = pathIconMap.find((entry) => path === entry.prefix || path.startsWith(`${entry.prefix}/`));
    if (matched) {
        prependBootstrapIcon(pageTitle, matched.icon);
    }
}

function applyCardHeaderIcons(): void {
    const headerKeywordIcons: Array<{ keyword: string; icon: string }> = [
        { keyword: 'invoice', icon: 'receipt' },
        { keyword: 'payment', icon: 'credit-card-2-front' },
        { keyword: 'renewal', icon: 'calendar-event' },
        { keyword: 'order', icon: 'bag-check' },
        { keyword: 'summary', icon: 'list-check' },
        { keyword: 'customer', icon: 'person-vcard' },
        { keyword: 'wallet', icon: 'wallet2' },
        { keyword: 'security', icon: 'shield-lock' },
        { keyword: 'notification', icon: 'bell' },
        { keyword: 'ticket', icon: 'life-preserver' }
    ];

    const headers = document.querySelectorAll('.card-header');
    headers.forEach((node) => {
        const header = node as HTMLElement;
        if (hasBootstrapIcon(header)) {
            return;
        }

        const text = normalizeText(header.textContent ?? '');
        if (!text) {
            return;
        }

        const matched = headerKeywordIcons.find((entry) => text.includes(entry.keyword));
        if (matched) {
            prependBootstrapIcon(header, matched.icon);
        }
    });
}

function applyActionButtonIcons(): void {
    const buttonIcons = new Map<string, string>([
        ['add payment instrument', 'plus-circle'],
        ['save', 'check-lg'],
        ['place order', 'credit-card'],
        ['continue to payment', 'arrow-right-circle'],
        ['buy domain', 'cart-plus'],
        ['back to order', 'arrow-left-circle'],
        ['continue', 'arrow-right-circle']
    ]);

    const actionButtons = document.querySelectorAll('button.btn, a.btn');
    actionButtons.forEach((node) => {
        const element = node as HTMLElement;
        if (hasBootstrapIcon(element)) {
            return;
        }

        const text = normalizeText(element.textContent ?? '');
        if (!text) {
            return;
        }

        const icon = buttonIcons.get(text);
        if (icon) {
            prependBootstrapIcon(element, icon);
        }
    });
}

function applyGlobalIcons(): void {
    applyMenuIcons();
    applyPageTitleIcons();
    applyCardHeaderIcons();
    applyActionButtonIcons();
}

if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', applyGlobalIcons);
} else {
    applyGlobalIcons();
}

window.addEventListener('popstate', applyGlobalIcons);

const iconWindow = window as UserPanelWindowWithBlazorEvents;
function registerGlobalIconsEnhancedLoadListener(): void {
    if (iconWindow.Blazor?.addEventListener) {
        iconWindow.Blazor.addEventListener('enhancedload', applyGlobalIcons);
        return;
    }

    window.setTimeout(registerGlobalIconsEnhancedLoadListener, 100);
}

registerGlobalIconsEnhancedLoadListener();

})();

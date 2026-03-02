interface UserPanelWindowWithBlazor extends Window {
    Blazor?: {
        addEventListener?: (eventName: string, callback: () => void) => void;
    };
}

const mainMenuStorageKey = 'up_main_menu_expanded_section';

function getMenuContainer(): HTMLElement | null {
    return document.getElementById('user-menu');
}

function persistExpandedSection(section: string | null): void {
    try {
        if (section) {
            localStorage.setItem(mainMenuStorageKey, section);
            return;
        }

        localStorage.removeItem(mainMenuStorageKey);
    } catch {
    }
}

function expandGroup(group: HTMLElement): void {
    const menu = getMenuContainer();
    if (!menu) {
        return;
    }

    const groups = menu.querySelectorAll('.nav-group');
    groups.forEach((node) => {
        const element = node as HTMLElement;
        if (element === group) {
            element.classList.add('expanded');
            return;
        }

        element.classList.remove('expanded');
    });

    persistExpandedSection(group.dataset.section ?? null);
}

function updateActiveLink(): HTMLElement | null {
    const menu = getMenuContainer();
    if (!menu) {
        return null;
    }

    const currentPath = window.location.pathname.toLowerCase();
    const links = menu.querySelectorAll('.nav-subitems a.nav-link-item');
    let matchedGroup: HTMLElement | null = null;

    links.forEach((linkNode) => {
        const link = linkNode as HTMLAnchorElement;
        const href = (link.getAttribute('href') ?? '').toLowerCase();
        if (!href || href === '#') {
            link.classList.remove('active');
            return;
        }

        const isMatch = currentPath === href || currentPath.startsWith(`${href}/`);
        if (isMatch) {
            link.classList.add('active');
            if (!matchedGroup) {
                matchedGroup = link.closest('.nav-group') as HTMLElement | null;
            }
            return;
        }

        link.classList.remove('active');
    });

    return matchedGroup;
}

function restoreExpandedGroup(): boolean {
    const menu = getMenuContainer();
    if (!menu) {
        return false;
    }

    let section: string | null = null;

    try {
        section = localStorage.getItem(mainMenuStorageKey);
    } catch {
    }

    if (!section) {
        return false;
    }

    const group = menu.querySelector(`.nav-group[data-section="${section}"]`) as HTMLElement | null;
    if (!group) {
        return false;
    }

    expandGroup(group);
    return true;
}

function toggleGroup(group: HTMLElement): void {
    expandGroup(group);
}

function bindGroupToggleEvents(): void {
    document.addEventListener('click', (event: MouseEvent) => {
        const target = event.target as HTMLElement;

        const subLink = target.closest('#user-menu .nav-subitems a.nav-link-item') as HTMLAnchorElement | null;
        if (subLink) {
            const group = subLink.closest('.nav-group') as HTMLElement | null;
            if (group) {
                expandGroup(group);
            }

            const menu = getMenuContainer();
            if (menu) {
                const links = menu.querySelectorAll('.nav-subitems a.nav-link-item');
                links.forEach((linkNode) => {
                    const link = linkNode as HTMLAnchorElement;
                    if (link === subLink) {
                        link.classList.add('active');
                        return;
                    }

                    link.classList.remove('active');
                });
            }

            return;
        }

        const header = target.closest('#user-menu .nav-group-header') as HTMLButtonElement | null;
        if (!header) {
            return;
        }

        event.preventDefault();

        const group = header.closest('.nav-group') as HTMLElement | null;
        if (!group) {
            return;
        }

        toggleGroup(group);
    });
}

let initialized = false;

function initializeMainMenu(): void {
    const menu = getMenuContainer();
    if (!menu) {
        return;
    }

    if (!initialized) {
        bindGroupToggleEvents();
        initialized = true;
    }

    const activeGroup = updateActiveLink();
    if (activeGroup) {
        expandGroup(activeGroup);
        return;
    }

    if (restoreExpandedGroup()) {
        return;
    }

    const firstGroup = menu.querySelector('.nav-group') as HTMLElement | null;
    if (firstGroup) {
        expandGroup(firstGroup);
    }
}

if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initializeMainMenu);
} else {
    initializeMainMenu();
}

window.addEventListener('popstate', initializeMainMenu);

const menuWindow = window as UserPanelWindowWithBlazor;
function registerEnhancedLoadListener(): void {
    if (menuWindow.Blazor?.addEventListener) {
        menuWindow.Blazor.addEventListener('enhancedload', initializeMainMenu);
        return;
    }

    window.setTimeout(registerEnhancedLoadListener, 100);
}

registerEnhancedLoadListener();

// @ts-nocheck
(function() {
let expandedSection: string | null = null;
let navMenuInitialized = false;


function toggleSection(section: string): void {
    const navGroups = document.querySelectorAll('#nav-menu .nav-group');
    console.log("Ready to toggle the main menu...");
    navGroups.forEach((group) => {
        const groupSection = (group as HTMLElement).dataset.section;
        console.log('Toggling section:', groupSection);
        if (groupSection === section) {
            if (expandedSection === section) {
                // Collapse current section
                group.classList.remove('expanded');
                expandedSection = null;
            } else {
                // Expand this section
                group.classList.add('expanded');
                expandedSection = section;
            }
        } else {
            // Collapse other sections
            group.classList.remove('expanded');
        }
    });
}

function updateActiveLink(): void {
    const currentPath = window.location.pathname.replace(/^\//, '').toLowerCase();
    const links = document.querySelectorAll('#nav-menu .nav-sublink');
    let foundActive = false;

    links.forEach((link) => {
        const href = (link.getAttribute('href') || '').toLowerCase();
        const isActive = currentPath === href || 
            (href === '' && (currentPath === '' || currentPath === '/'));

        if (isActive && !foundActive) {
            link.classList.add('active');
            foundActive = true;
            // Auto-expand the parent section
            const parentGroup = link.closest('.nav-group') as HTMLElement | null;
            if (parentGroup?.dataset.section && expandedSection !== parentGroup.dataset.section) {
                // Collapse all others first
                document.querySelectorAll('#nav-menu .nav-group').forEach(g => g.classList.remove('expanded'));
                parentGroup.classList.add('expanded');
                expandedSection = parentGroup.dataset.section;
            }
        } else {
            link.classList.remove('active');
        }
    });
}

function closeMenuOnMobile(): void {
    const toggler = document.getElementById('nav-menu-toggler') as HTMLInputElement | null;
    if (toggler) {
        toggler.checked = false;
    }
}

let eventsBound = false;

function bindNavMenuEvents(): void {
    console.log('Trying to bind NavMenuEvents...')

    // Use event delegation on document to survive Blazor DOM replacement
    if (eventsBound) {
        console.log('Events already bound to document');
        return;
    }

    eventsBound = true;

    // Bind click events using document-level delegation
    document.addEventListener('click', (event) => {
        const target = event.target as HTMLElement;

        // Check if click is within nav-menu
        const navMenu = target.closest('#nav-menu');
        if (!navMenu) {
            return;
        }

        const header = target.closest('.nav-group-header') as HTMLElement | null;

        if (header) {
            event.preventDefault();
            event.stopPropagation();
            const group = header.closest('.nav-group') as HTMLElement | null;
            if (group?.dataset.section) {
                console.log('Header clicked, calling toggleSection');
                toggleSection(group.dataset.section);
            }
            return;
        }

        // Handle nav link clicks - close mobile menu
        const link = target.closest('.nav-sublink') as HTMLAnchorElement | null;
        if (link) {
            closeMenuOnMobile();
        }
    });

    console.log('Events bound to document for nav-menu delegation');
}

function initializeNavMenu(): boolean {
    const navMenu = document.getElementById('nav-menu');
    if (!navMenu) {
        return false;
    }

    if (navMenuInitialized) {
        return true;
    }

    navMenuInitialized = true;
    bindNavMenuEvents();
    updateActiveLink();
    console.log('Nav menu initialized');
    return true;
}

function tryInitialize(): void {
    if (initializeNavMenu()) {
        return;
    }

    // Retry with polling if not found
    let attempts = 0;
    const maxAttempts = 50;
    const interval = setInterval(() => {
        attempts++;
        if (initializeNavMenu() || attempts >= maxAttempts) {
            clearInterval(interval);
        }
    }, 100);
}

// Initialize when DOM is ready
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', tryInitialize);
} else {
    tryInitialize();
}

// Also listen for Blazor's page enhancements
window.addEventListener('load', () => {
    if (!navMenuInitialized) {
        tryInitialize();
        console.log('Added EventListener for load')
    }
    else {
        console.log('EventListener for load already added')
    }
});

// Listen for Blazor's enhanced navigation using Blazor API
function setupBlazorNavListener(): void {
    const blazor = (window as any).Blazor;
    if (blazor?.addEventListener) {
        blazor.addEventListener('enhancedload', () => {
            updateActiveLink();
        });
        console.log('NavMenu: Blazor enhancedload listener registered');
    } else {
        setTimeout(setupBlazorNavListener, 100);
    }
}
setupBlazorNavListener();
})();

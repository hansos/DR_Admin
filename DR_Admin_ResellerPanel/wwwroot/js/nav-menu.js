"use strict";
// @ts-nocheck
(function () {
    let expandedSection = null;
    let navMenuInitialized = false;
    function toggleSection(section) {
        const navGroups = document.querySelectorAll('#nav-menu .nav-group');
        console.log("Ready to toggle the main menu...");
        navGroups.forEach((group) => {
            const groupSection = group.dataset.section;
            console.log('Toggling section:', groupSection);
            if (groupSection === section) {
                if (expandedSection === section) {
                    // Collapse current section
                    group.classList.remove('expanded');
                    expandedSection = null;
                }
                else {
                    // Expand this section
                    group.classList.add('expanded');
                    expandedSection = section;
                }
            }
            else {
                // Collapse other sections
                group.classList.remove('expanded');
            }
        });
    }
    function updateActiveLink() {
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
                const parentGroup = link.closest('.nav-group');
                if ((parentGroup === null || parentGroup === void 0 ? void 0 : parentGroup.dataset.section) && expandedSection !== parentGroup.dataset.section) {
                    // Collapse all others first
                    document.querySelectorAll('#nav-menu .nav-group').forEach(g => g.classList.remove('expanded'));
                    parentGroup.classList.add('expanded');
                    expandedSection = parentGroup.dataset.section;
                }
            }
            else {
                link.classList.remove('active');
            }
        });
    }
    function closeMenuOnMobile() {
        const toggler = document.getElementById('nav-menu-toggler');
        if (toggler) {
            toggler.checked = false;
        }
    }
    let eventsBound = false;
    function bindNavMenuEvents() {
        console.log('Trying to bind NavMenuEvents...');
        // Use event delegation on document to survive Blazor DOM replacement
        if (eventsBound) {
            console.log('Events already bound to document');
            return;
        }
        eventsBound = true;
        // Bind click events using document-level delegation
        document.addEventListener('click', (event) => {
            const target = event.target;
            // Check if click is within nav-menu
            const navMenu = target.closest('#nav-menu');
            if (!navMenu) {
                return;
            }
            const header = target.closest('.nav-group-header');
            if (header) {
                event.preventDefault();
                event.stopPropagation();
                const group = header.closest('.nav-group');
                if (group === null || group === void 0 ? void 0 : group.dataset.section) {
                    console.log('Header clicked, calling toggleSection');
                    toggleSection(group.dataset.section);
                }
                return;
            }
            // Handle nav link clicks - close mobile menu
            const link = target.closest('.nav-sublink');
            if (link) {
                closeMenuOnMobile();
            }
        });
        console.log('Events bound to document for nav-menu delegation');
    }
    function initializeNavMenu() {
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
    function tryInitialize() {
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
    }
    else {
        tryInitialize();
    }
    // Also listen for Blazor's page enhancements
    window.addEventListener('load', () => {
        if (!navMenuInitialized) {
            tryInitialize();
            console.log('Added EventListener for load');
        }
        else {
            console.log('EventListener for load already added');
        }
    });
    // Listen for Blazor's enhanced navigation using Blazor API
    function setupBlazorNavListener() {
        const blazor = window.Blazor;
        if (blazor === null || blazor === void 0 ? void 0 : blazor.addEventListener) {
            blazor.addEventListener('enhancedload', () => {
                updateActiveLink();
            });
            console.log('NavMenu: Blazor enhancedload listener registered');
        }
        else {
            setTimeout(setupBlazorNavListener, 100);
        }
    }
    setupBlazorNavListener();
})();
//# sourceMappingURL=nav-menu.js.map
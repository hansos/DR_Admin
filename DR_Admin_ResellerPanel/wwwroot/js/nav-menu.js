"use strict";
// @ts-nocheck
(function () {
    let expandedSection = null;
    let navMenuInitialized = false;
    const expandedSectionStorageKey = 'nav-menu-expanded-section';
    let navMenuObserver = null;
    let restoreRetryTimer = null;
    function toggleSection(section) {
        const navGroups = document.querySelectorAll('#nav-menu .nav-group');
        console.log("Ready to toggle the main menu...");
        navGroups.forEach((group) => {
            const groupSection = group.dataset.section;
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
        persistExpandedSection(expandedSection);
    }
    function observeNavMenu(navMenu) {
        if (navMenuObserver) {
            navMenuObserver.disconnect();
        }
        navMenuObserver = new MutationObserver(() => {
            // Blazor re-rendered the nav-menu, restore state
            restoreExpandedSection();
            updateActiveLink();
        });
        navMenuObserver.observe(navMenu, { childList: true, subtree: true });
    }
    function persistExpandedSection(section) {
        try {
            if (section) {
                localStorage.setItem(expandedSectionStorageKey, section);
            }
            else {
                localStorage.removeItem(expandedSectionStorageKey);
            }
        }
        catch (error) {
            console.warn('NavMenu: Failed to persist expanded section', error);
        }
    }
    function restoreExpandedSection() {
        let storedSection = null;
        try {
            storedSection = localStorage.getItem(expandedSectionStorageKey);
        }
        catch (error) {
            console.warn('NavMenu: Failed to read expanded section from storage', error);
        }
        if (!storedSection) {
            return;
        }
        const targetGroup = document.querySelector(`#nav-menu .nav-group[data-section="${storedSection}"]`);
        if (!targetGroup) {
            // Element not found yet, schedule retry
            scheduleRestoreRetry();
            return;
        }
        // Check if already expanded
        if (targetGroup.classList.contains('expanded')) {
            expandedSection = storedSection;
            return;
        }
        document.querySelectorAll('#nav-menu .nav-group').forEach(g => g.classList.remove('expanded'));
        targetGroup.classList.add('expanded');
        expandedSection = storedSection;
    }
    function scheduleRestoreRetry() {
        if (restoreRetryTimer !== null) {
            return;
        }
        let attempts = 0;
        const maxAttempts = 20;
        restoreRetryTimer = window.setInterval(() => {
            attempts++;
            const storedSection = localStorage.getItem(expandedSectionStorageKey);
            const targetGroup = storedSection
                ? document.querySelector(`#nav-menu .nav-group[data-section="${storedSection}"]`)
                : null;
            if (targetGroup && !targetGroup.classList.contains('expanded')) {
                document.querySelectorAll('#nav-menu .nav-group').forEach(g => g.classList.remove('expanded'));
                targetGroup.classList.add('expanded');
                expandedSection = storedSection;
            }
            if ((targetGroup && targetGroup.classList.contains('expanded')) || attempts >= maxAttempts) {
                if (restoreRetryTimer !== null) {
                    clearInterval(restoreRetryTimer);
                    restoreRetryTimer = null;
                }
            }
        }, 100);
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
                    persistExpandedSection(expandedSection);
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
        observeNavMenu(navMenu);
        restoreExpandedSection();
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
    // Listen for Blazor's enhanced navigation using Blazor API
    function setupBlazorNavListener() {
        const blazor = window.Blazor;
        if (blazor === null || blazor === void 0 ? void 0 : blazor.addEventListener) {
            blazor.addEventListener('enhancedload', () => {
                restoreExpandedSection();
                updateActiveLink();
            });
            console.log('NavMenu: Blazor enhancedload listener registered');
        }
        else {
            setTimeout(setupBlazorNavListener, 100);
        }
    }
    setupBlazorNavListener();
    // Handle InteractiveServer re-render after initial page load
    window.addEventListener('load', () => {
        if (!navMenuInitialized) {
            tryInitialize();
        }
        // Schedule restoration attempts after Blazor's interactive render kicks in
        setTimeout(() => {
            restoreExpandedSection();
            updateActiveLink();
        }, 200);
        setTimeout(() => {
            restoreExpandedSection();
            updateActiveLink();
        }, 500);
    });
})();
//# sourceMappingURL=nav-menu.js.map
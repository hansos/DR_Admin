"use strict";
(() => {
    const storageKey = 'up_account_settings';
    function initializeAccountSettingsPage() {
        const page = document.getElementById('account-settings-page');
        if (!page || page.dataset.bound === 'true') {
            return;
        }
        page.dataset.bound = 'true';
        const form = document.getElementById('account-settings-form');
        if (!form) {
            return;
        }
        hydrateForm();
        form.addEventListener('submit', (event) => {
            event.preventDefault();
            saveSettings();
        });
    }
    function hydrateForm() {
        const sessionRaw = localStorage.getItem('up_auth_session');
        const storedRaw = localStorage.getItem(storageKey);
        let stored = {};
        if (storedRaw) {
            try {
                stored = JSON.parse(storedRaw);
            }
            catch {
                stored = {};
            }
        }
        let sessionUsername = '';
        if (sessionRaw) {
            try {
                const session = JSON.parse(sessionRaw);
                sessionUsername = session.username ?? '';
            }
            catch {
                sessionUsername = '';
            }
        }
        setInputValue('account-settings-first-name', stored.firstName ?? '');
        setInputValue('account-settings-last-name', stored.lastName ?? '');
        setInputValue('account-settings-email', stored.email ?? sessionUsername);
        setInputValue('account-settings-phone', stored.phone ?? '');
    }
    function saveSettings() {
        hideAlert('account-settings-alert-success');
        hideAlert('account-settings-alert-error');
        const firstName = getInputValue('account-settings-first-name');
        const lastName = getInputValue('account-settings-last-name');
        const email = getInputValue('account-settings-email');
        const phone = getInputValue('account-settings-phone');
        if (!firstName || !lastName || !email) {
            showAlert('account-settings-alert-error', 'First name, last name, and email are required.');
            return;
        }
        const payload = {
            firstName,
            lastName,
            email,
            phone
        };
        localStorage.setItem(storageKey, JSON.stringify(payload));
        showAlert('account-settings-alert-success', 'Account settings saved.');
    }
    function getInputValue(id) {
        const input = document.getElementById(id);
        return input?.value.trim() ?? '';
    }
    function setInputValue(id, value) {
        const input = document.getElementById(id);
        if (input) {
            input.value = value;
        }
    }
    function showAlert(id, message) {
        const element = document.getElementById(id);
        if (!element) {
            return;
        }
        element.textContent = message;
        element.classList.remove('d-none');
    }
    function hideAlert(id) {
        const element = document.getElementById(id);
        if (!element) {
            return;
        }
        element.textContent = '';
        element.classList.add('d-none');
    }
    function setupAccountSettingsObserver() {
        initializeAccountSettingsPage();
        const observer = new MutationObserver(() => {
            initializeAccountSettingsPage();
        });
        observer.observe(document.body, { childList: true, subtree: true });
    }
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', setupAccountSettingsObserver);
    }
    else {
        setupAccountSettingsObserver();
    }
})();
//# sourceMappingURL=account-settings.js.map
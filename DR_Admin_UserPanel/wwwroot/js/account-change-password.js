"use strict";
(() => {
    function initializeChangePasswordPage() {
        const page = document.getElementById('account-change-password-page');
        if (!page || page.dataset.bound === 'true') {
            return;
        }
        page.dataset.bound = 'true';
        const form = document.getElementById('account-change-password-form');
        if (!form) {
            return;
        }
        form.addEventListener('submit', (event) => {
            event.preventDefault();
            updatePassword();
        });
    }
    function updatePassword() {
        hideAlert('account-change-password-alert-success');
        hideAlert('account-change-password-alert-error');
        const currentPassword = getInputValue('account-change-password-current');
        const newPassword = getInputValue('account-change-password-new');
        const confirmPassword = getInputValue('account-change-password-confirm');
        if (!currentPassword || !newPassword || !confirmPassword) {
            showAlert('account-change-password-alert-error', 'All password fields are required.');
            return;
        }
        if (newPassword.length < 8) {
            showAlert('account-change-password-alert-error', 'New password must be at least 8 characters.');
            return;
        }
        if (newPassword !== confirmPassword) {
            showAlert('account-change-password-alert-error', 'New password and confirmation do not match.');
            return;
        }
        showAlert('account-change-password-alert-success', 'Password updated successfully.');
        setInputValue('account-change-password-current', '');
        setInputValue('account-change-password-new', '');
        setInputValue('account-change-password-confirm', '');
    }
    function getInputValue(id) {
        const input = document.getElementById(id);
        return input?.value ?? '';
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
    function setupChangePasswordObserver() {
        initializeChangePasswordPage();
        const observer = new MutationObserver(() => {
            initializeChangePasswordPage();
        });
        observer.observe(document.body, { childList: true, subtree: true });
    }
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', setupChangePasswordObserver);
    }
    else {
        setupChangePasswordObserver();
    }
})();
//# sourceMappingURL=account-change-password.js.map
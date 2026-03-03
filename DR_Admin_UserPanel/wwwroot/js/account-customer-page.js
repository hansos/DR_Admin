"use strict";
(() => {
    const storageKey = 'up_customer_page';
    function initializeCustomerPage() {
        const page = document.getElementById('account-customer-page');
        if (!page || page.dataset.bound === 'true') {
            return;
        }
        page.dataset.bound = 'true';
        const form = document.getElementById('account-customer-form');
        if (!form) {
            return;
        }
        hydrateForm();
        form.addEventListener('submit', (event) => {
            event.preventDefault();
            saveCustomerPage();
        });
    }
    function hydrateForm() {
        const raw = localStorage.getItem(storageKey);
        if (!raw) {
            return;
        }
        try {
            const model = JSON.parse(raw);
            setInputValue('account-customer-company', model.company ?? '');
            setInputValue('account-customer-contact-email', model.contactEmail ?? '');
            setInputValue('account-customer-phone', model.phone ?? '');
            setInputValue('account-customer-address', model.address ?? '');
        }
        catch {
            setInputValue('account-customer-company', '');
            setInputValue('account-customer-contact-email', '');
            setInputValue('account-customer-phone', '');
            setInputValue('account-customer-address', '');
        }
    }
    function saveCustomerPage() {
        hideAlert('account-customer-alert-success');
        hideAlert('account-customer-alert-error');
        const company = getInputValue('account-customer-company').trim();
        const contactEmail = getInputValue('account-customer-contact-email').trim();
        const phone = getInputValue('account-customer-phone').trim();
        const address = getInputValue('account-customer-address').trim();
        if (!company || !contactEmail) {
            showAlert('account-customer-alert-error', 'Company name and billing email are required.');
            return;
        }
        const payload = {
            company,
            contactEmail,
            phone,
            address
        };
        localStorage.setItem(storageKey, JSON.stringify(payload));
        showAlert('account-customer-alert-success', 'Customer page updated.');
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
    function setupCustomerPageObserver() {
        initializeCustomerPage();
        const observer = new MutationObserver(() => {
            initializeCustomerPage();
        });
        observer.observe(document.body, { childList: true, subtree: true });
    }
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', setupCustomerPageObserver);
    }
    else {
        setupCustomerPageObserver();
    }
})();
//# sourceMappingURL=account-customer-page.js.map
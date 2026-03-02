"use strict";
(() => {
    let contactPersonsCustomerId = null;
    let contactPersonsItems = [];
    function initializeContactPersonsPage() {
        const page = document.getElementById('contact-persons-page');
        if (!page || page.dataset.initialized === 'true') {
            return;
        }
        page.dataset.initialized = 'true';
        const form = document.getElementById('contact-persons-form');
        form?.addEventListener('submit', async (event) => {
            event.preventDefault();
            await saveContactPerson();
        });
        document.getElementById('contact-persons-reset')?.addEventListener('click', () => {
            resetContactPersonsForm();
        });
        document.getElementById('contact-persons-table-body')?.addEventListener('click', (event) => {
            const target = event.target;
            const button = target.closest('button[data-action]');
            if (!button) {
                return;
            }
            const id = Number.parseInt(button.dataset.id ?? '', 10);
            if (Number.isNaN(id) || id <= 0) {
                return;
            }
            if (button.dataset.action === 'edit') {
                editContactPerson(id);
                return;
            }
            if (button.dataset.action === 'delete') {
                void deleteContactPerson(id);
            }
        });
        void loadContactPersons();
    }
    async function loadContactPersons() {
        const typedWindow = window;
        contactPersonsCustomerId = await resolveContactPersonsCustomerId();
        if (!contactPersonsCustomerId) {
            typedWindow.UserPanelAlerts?.showError('contact-persons-alert-error', 'Could not resolve customer profile.');
            renderContactPersonsRows([]);
            return;
        }
        const response = await typedWindow.UserPanelApi?.request(`/ContactPersons/customer/${contactPersonsCustomerId}`, { method: 'GET' }, true);
        if (!response || !response.success || !response.data) {
            typedWindow.UserPanelAlerts?.showError('contact-persons-alert-error', response?.message ?? 'Could not load contact persons.');
            renderContactPersonsRows([]);
            return;
        }
        contactPersonsItems = response.data;
        renderContactPersonsRows(response.data);
    }
    function renderContactPersonsRows(items) {
        const tableBody = document.getElementById('contact-persons-table-body');
        if (!tableBody) {
            return;
        }
        if (items.length === 0) {
            tableBody.innerHTML = '<tr><td colspan="5" class="text-center text-muted">No contact persons found.</td></tr>';
            return;
        }
        tableBody.innerHTML = items.map((item) => {
            const roles = [];
            if (item.isPrimary) {
                roles.push('Primary');
            }
            if (item.isDefaultBilling) {
                roles.push('Billing');
            }
            if (item.isDefaultTech) {
                roles.push('Technical');
            }
            if (item.isDefaultAdministrator) {
                roles.push('Admin');
            }
            return `<tr>
            <td>${escapeContactPersonsText(`${item.firstName} ${item.lastName}`.trim())}</td>
            <td>${escapeContactPersonsText(item.email)}</td>
            <td>${escapeContactPersonsText(item.phone)}</td>
            <td>${escapeContactPersonsText(roles.join(', ') || '-')}</td>
            <td>
                <div class="btn-group btn-group-sm">
                    <button class="btn btn-outline-primary" type="button" data-action="edit" data-id="${item.id}">Edit</button>
                    <button class="btn btn-outline-danger" type="button" data-action="delete" data-id="${item.id}">Delete</button>
                </div>
            </td>
        </tr>`;
        }).join('');
    }
    function editContactPerson(id) {
        const item = contactPersonsItems.find((entry) => entry.id === id);
        if (!item) {
            return;
        }
        setContactPersonsInputValue('contact-persons-id', item.id.toString());
        setContactPersonsInputValue('contact-persons-first-name', item.firstName);
        setContactPersonsInputValue('contact-persons-last-name', item.lastName);
        setContactPersonsInputValue('contact-persons-email', item.email);
        setContactPersonsInputValue('contact-persons-phone', item.phone);
        const primaryInput = document.getElementById('contact-persons-is-primary');
        if (primaryInput) {
            primaryInput.checked = item.isPrimary;
        }
    }
    async function saveContactPerson() {
        const typedWindow = window;
        const firstName = readContactPersonsInputValue('contact-persons-first-name');
        const lastName = readContactPersonsInputValue('contact-persons-last-name');
        const email = readContactPersonsInputValue('contact-persons-email');
        const phone = readContactPersonsInputValue('contact-persons-phone');
        if (!firstName || !lastName || !email) {
            typedWindow.UserPanelAlerts?.showError('contact-persons-alert-error', 'First name, last name and email are required.');
            return;
        }
        if (!contactPersonsCustomerId) {
            typedWindow.UserPanelAlerts?.showError('contact-persons-alert-error', 'Missing customer context.');
            return;
        }
        const isPrimary = document.getElementById('contact-persons-is-primary')?.checked ?? false;
        const payload = {
            firstName,
            lastName,
            email,
            phone,
            customerId: contactPersonsCustomerId,
            isPrimary,
            isActive: true,
            isDefaultOwner: false,
            isDefaultBilling: false,
            isDefaultTech: false,
            isDefaultAdministrator: false,
            isDomainGlobal: false
        };
        const idText = readContactPersonsInputValue('contact-persons-id');
        const id = Number.parseInt(idText, 10);
        const isEdit = !Number.isNaN(id) && id > 0;
        const response = await typedWindow.UserPanelApi?.request(isEdit ? `/ContactPersons/${id}` : '/ContactPersons', {
            method: isEdit ? 'PUT' : 'POST',
            body: JSON.stringify(payload)
        }, true);
        if (!response || !response.success) {
            typedWindow.UserPanelAlerts?.showError('contact-persons-alert-error', response?.message ?? 'Could not save contact person.');
            return;
        }
        typedWindow.UserPanelAlerts?.showSuccess('contact-persons-alert-success', isEdit ? 'Contact updated.' : 'Contact created.');
        resetContactPersonsForm();
        await loadContactPersons();
    }
    async function deleteContactPerson(id) {
        const typedWindow = window;
        const response = await typedWindow.UserPanelApi?.request(`/ContactPersons/${id}`, { method: 'DELETE' }, true);
        if (!response || !response.success) {
            typedWindow.UserPanelAlerts?.showError('contact-persons-alert-error', response?.message ?? 'Could not delete contact person.');
            return;
        }
        typedWindow.UserPanelAlerts?.showSuccess('contact-persons-alert-success', 'Contact deleted.');
        await loadContactPersons();
    }
    function resetContactPersonsForm() {
        setContactPersonsInputValue('contact-persons-id', '');
        setContactPersonsInputValue('contact-persons-first-name', '');
        setContactPersonsInputValue('contact-persons-last-name', '');
        setContactPersonsInputValue('contact-persons-email', '');
        setContactPersonsInputValue('contact-persons-phone', '');
        const primaryInput = document.getElementById('contact-persons-is-primary');
        if (primaryInput) {
            primaryInput.checked = false;
        }
    }
    async function resolveContactPersonsCustomerId() {
        const typedWindow = window;
        const response = await typedWindow.UserPanelApi?.request('/MyAccount/me', { method: 'GET' }, true);
        return response?.success ? (response.data?.customer?.id ?? null) : null;
    }
    function readContactPersonsInputValue(id) {
        const input = document.getElementById(id);
        return input?.value.trim() ?? '';
    }
    function setContactPersonsInputValue(id, value) {
        const input = document.getElementById(id);
        if (input) {
            input.value = value;
        }
    }
    function escapeContactPersonsText(value) {
        return value
            .replace(/&/g, '&amp;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;')
            .replace(/'/g, '&#039;');
    }
    function setupContactPersonsObserver() {
        initializeContactPersonsPage();
        const observer = new MutationObserver(() => {
            initializeContactPersonsPage();
        });
        observer.observe(document.body, { childList: true, subtree: true });
    }
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', setupContactPersonsObserver);
    }
    else {
        setupContactPersonsObserver();
    }
})();
//# sourceMappingURL=contact-persons.js.map
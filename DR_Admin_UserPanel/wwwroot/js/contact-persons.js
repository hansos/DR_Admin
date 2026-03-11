"use strict";
(() => {
    let contactPersonsCustomerId = null;
    let contactPersonsItems = [];
    let contactPersonsModal = null;
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
        document.getElementById('contact-persons-add')?.addEventListener('click', () => {
            openContactPersonModal();
        });
        document.getElementById('contact-persons-reset')?.addEventListener('click', () => {
            resetContactPersonsForm();
        });
        document.getElementById('contact-persons-list')?.addEventListener('click', (event) => {
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
        initializeContactPersonsModal();
        void loadContactPersons();
    }
    function initializeContactPersonsModal() {
        const typedWindow = window;
        const modalElement = document.getElementById('contact-persons-modal');
        const modalFactory = typedWindow.bootstrap?.Modal;
        if (!modalElement || !modalFactory) {
            contactPersonsModal = null;
            return;
        }
        contactPersonsModal = modalFactory.getOrCreateInstance(modalElement);
    }
    async function loadContactPersons() {
        const typedWindow = window;
        typedWindow.UserPanelAlerts?.hide('contact-persons-alert-success');
        typedWindow.UserPanelAlerts?.hide('contact-persons-alert-error');
        contactPersonsCustomerId = await resolveContactPersonsCustomerId();
        if (!contactPersonsCustomerId) {
            typedWindow.UserPanelAlerts?.showError('contact-persons-alert-error', 'Could not resolve customer profile.');
            renderContactPersonsItems([]);
            return;
        }
        const response = await typedWindow.UserPanelApi?.request(`/ContactPersons/customer/${contactPersonsCustomerId}`, { method: 'GET' }, true);
        if (!response || !response.success || !response.data) {
            typedWindow.UserPanelAlerts?.showError('contact-persons-alert-error', response?.message ?? 'Could not load contact persons.');
            renderContactPersonsItems([]);
            return;
        }
        contactPersonsItems = response.data;
        renderContactPersonsItems(response.data);
    }
    function renderContactPersonsItems(items) {
        const list = document.getElementById('contact-persons-list');
        if (!list) {
            return;
        }
        if (items.length === 0) {
            list.innerHTML = '<li class="list-group-item text-center text-muted">No contact persons found.</li>';
            return;
        }
        list.innerHTML = items.map((item) => {
            const roles = [];
            if (item.isPrimary) {
                roles.push('Primary');
            }
            if (item.isDefaultOwner) {
                roles.push('Owner');
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
            const fullName = `${item.firstName} ${item.lastName}`.trim();
            return `<li class="list-group-item">
            <div class="d-flex flex-column flex-xl-row justify-content-between gap-3">
                <div class="row g-2 flex-grow-1">
                    <div class="col-12 col-md-6 col-xl-3">
                        <div class="small text-muted">Name</div>
                        <div class="fw-semibold">${escapeContactPersonsText(fullName || '-')}</div>
                    </div>
                    <div class="col-12 col-md-6 col-xl-3">
                        <div class="small text-muted">Email</div>
                        <div>${escapeContactPersonsText(item.email || '-')}</div>
                    </div>
                    <div class="col-12 col-md-6 col-xl-2">
                        <div class="small text-muted">Phone</div>
                        <div>${escapeContactPersonsText(item.phone || '-')}</div>
                    </div>
                    <div class="col-12 col-md-6 col-xl-2">
                        <div class="small text-muted">Position</div>
                        <div>${escapeContactPersonsText(item.position ?? '-')}</div>
                    </div>
                    <div class="col-12 col-md-6 col-xl-2">
                        <div class="small text-muted">Department</div>
                        <div>${escapeContactPersonsText(item.department ?? '-')}</div>
                    </div>
                    <div class="col-12 col-md-8">
                        <div class="small text-muted">Roles</div>
                        <div>${escapeContactPersonsText(roles.join(', ') || '-')}</div>
                    </div>
                    <div class="col-12 col-md-4">
                        <div class="small text-muted">State</div>
                        <div>${item.isActive ? 'Active' : 'Inactive'}${item.isDomainGlobal ? ' · Domain global' : ''}</div>
                    </div>
                    <div class="col-12">
                        <div class="small text-muted">Notes</div>
                        <div>${escapeContactPersonsText(item.notes ?? '-')}</div>
                    </div>
                </div>
                <div class="d-flex gap-2 align-items-start">
                    <button class="btn btn-outline-primary btn-sm" type="button" data-action="edit" data-id="${item.id}">Edit</button>
                    <button class="btn btn-outline-danger btn-sm" type="button" data-action="delete" data-id="${item.id}">Delete</button>
                </div>
            </div>
        </li>`;
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
        setContactPersonsInputValue('contact-persons-position', item.position ?? '');
        setContactPersonsInputValue('contact-persons-department', item.department ?? '');
        setContactPersonsTextAreaValue('contact-persons-notes', item.notes ?? '');
        setContactPersonsCheckboxValue('contact-persons-is-primary', item.isPrimary);
        setContactPersonsCheckboxValue('contact-persons-is-active', item.isActive);
        setContactPersonsCheckboxValue('contact-persons-is-default-owner', item.isDefaultOwner);
        setContactPersonsCheckboxValue('contact-persons-is-default-billing', item.isDefaultBilling);
        setContactPersonsCheckboxValue('contact-persons-is-default-tech', item.isDefaultTech);
        setContactPersonsCheckboxValue('contact-persons-is-default-administrator', item.isDefaultAdministrator);
        setContactPersonsCheckboxValue('contact-persons-is-domain-global', item.isDomainGlobal);
        setContactPersonsModalTitle('Edit contact person');
        contactPersonsModal?.show();
    }
    function openContactPersonModal() {
        resetContactPersonsForm();
        setContactPersonsModalTitle('Add contact person');
        contactPersonsModal?.show();
    }
    async function saveContactPerson() {
        const typedWindow = window;
        typedWindow.UserPanelAlerts?.hide('contact-persons-alert-success');
        typedWindow.UserPanelAlerts?.hide('contact-persons-alert-error');
        const form = document.getElementById('contact-persons-form');
        if (form && !form.reportValidity()) {
            return;
        }
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
        const payload = {
            firstName,
            lastName,
            email,
            phone,
            position: toNullable(readContactPersonsInputValue('contact-persons-position')),
            department: toNullable(readContactPersonsInputValue('contact-persons-department')),
            notes: toNullable(readContactPersonsTextAreaValue('contact-persons-notes')),
            customerId: contactPersonsCustomerId,
            isPrimary: getContactPersonsCheckboxValue('contact-persons-is-primary'),
            isActive: getContactPersonsCheckboxValue('contact-persons-is-active'),
            isDefaultOwner: getContactPersonsCheckboxValue('contact-persons-is-default-owner'),
            isDefaultBilling: getContactPersonsCheckboxValue('contact-persons-is-default-billing'),
            isDefaultTech: getContactPersonsCheckboxValue('contact-persons-is-default-tech'),
            isDefaultAdministrator: getContactPersonsCheckboxValue('contact-persons-is-default-administrator'),
            isDomainGlobal: getContactPersonsCheckboxValue('contact-persons-is-domain-global')
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
        contactPersonsModal?.hide();
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
        setContactPersonsInputValue('contact-persons-position', '');
        setContactPersonsInputValue('contact-persons-department', '');
        setContactPersonsTextAreaValue('contact-persons-notes', '');
        setContactPersonsCheckboxValue('contact-persons-is-primary', false);
        setContactPersonsCheckboxValue('contact-persons-is-active', true);
        setContactPersonsCheckboxValue('contact-persons-is-default-owner', false);
        setContactPersonsCheckboxValue('contact-persons-is-default-billing', false);
        setContactPersonsCheckboxValue('contact-persons-is-default-tech', false);
        setContactPersonsCheckboxValue('contact-persons-is-default-administrator', false);
        setContactPersonsCheckboxValue('contact-persons-is-domain-global', false);
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
    function readContactPersonsTextAreaValue(id) {
        const input = document.getElementById(id);
        return input?.value.trim() ?? '';
    }
    function setContactPersonsTextAreaValue(id, value) {
        const input = document.getElementById(id);
        if (input) {
            input.value = value;
        }
    }
    function getContactPersonsCheckboxValue(id) {
        const input = document.getElementById(id);
        return input?.checked ?? false;
    }
    function setContactPersonsCheckboxValue(id, value) {
        const input = document.getElementById(id);
        if (input) {
            input.checked = value;
        }
    }
    function setContactPersonsModalTitle(value) {
        const title = document.getElementById('contact-persons-modal-title');
        if (title) {
            title.textContent = value;
        }
    }
    function toNullable(value) {
        const trimmed = value.trim();
        return trimmed ? trimmed : null;
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
"use strict";
(() => {
    let currentCustomerId = 0;
    let editingContactId = null;
    let allContacts = [];
    const getApiBaseUrl = () => (window.AppSettings?.apiBaseUrl ?? '');
    const getAuthToken = () => {
        const auth = window.Auth;
        if (auth?.getToken) {
            return auth.getToken();
        }
        return sessionStorage.getItem('rp_authToken');
    };
    const esc = (text) => {
        const map = { '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#039;' };
        return (text || '').replace(/[&<>"']/g, (char) => map[char] ?? char);
    };
    const formatDate = (value) => {
        if (!value)
            return '-';
        const date = new Date(value);
        return Number.isNaN(date.getTime()) ? '-' : date.toLocaleString();
    };
    const formatMoney = (amount, currency = 'EUR') => {
        const value = Number.isFinite(amount) ? Number(amount) : 0;
        try {
            return new Intl.NumberFormat(undefined, { style: 'currency', currency }).format(value);
        }
        catch {
            return `${value.toFixed(2)} ${currency}`;
        }
    };
    const apiRequest = async (endpoint, options = {}) => {
        try {
            const headers = {
                'Content-Type': 'application/json',
                ...options.headers,
            };
            const token = getAuthToken();
            if (token)
                headers.Authorization = `Bearer ${token}`;
            const response = await fetch(endpoint, { ...options, headers, credentials: 'include' });
            const contentType = response.headers.get('content-type') ?? '';
            const hasJson = contentType.includes('application/json');
            const body = hasJson ? await response.json() : null;
            if (!response.ok) {
                const err = body;
                return { success: false, message: err?.message ?? err?.title ?? `Request failed with status ${response.status}` };
            }
            const envelope = body;
            return { success: envelope?.success !== false, data: envelope?.data ?? body, message: envelope?.message };
        }
        catch {
            return { success: false, message: 'Network error. Please try again.' };
        }
    };
    const showSuccess = (message) => {
        const alert = document.getElementById('customer-details-alert-success');
        if (!alert)
            return;
        alert.textContent = message;
        alert.classList.remove('d-none');
        document.getElementById('customer-details-alert-error')?.classList.add('d-none');
    };
    const showError = (message) => {
        const alert = document.getElementById('customer-details-alert-error');
        if (!alert)
            return;
        alert.textContent = message;
        alert.classList.remove('d-none');
        document.getElementById('customer-details-alert-success')?.classList.add('d-none');
    };
    const setText = (id, value) => {
        const el = document.getElementById(id);
        if (el)
            el.textContent = value;
    };
    const getInputValue = (id) => {
        const el = document.getElementById(id);
        return (el?.value ?? '').trim();
    };
    const setInputValue = (id, value) => {
        const el = document.getElementById(id);
        if (el)
            el.value = value;
    };
    const getCheckboxValue = (id) => {
        const el = document.getElementById(id);
        return !!el?.checked;
    };
    const setCheckboxValue = (id, value) => {
        const el = document.getElementById(id);
        if (el)
            el.checked = value;
    };
    const getBootstrap = () => {
        return window.bootstrap ?? null;
    };
    const showModal = (id) => {
        const modal = document.getElementById(id);
        const bootstrap = getBootstrap();
        if (!modal || !bootstrap)
            return;
        new bootstrap.Modal(modal).show();
    };
    const hideModal = (id) => {
        const modal = document.getElementById(id);
        const bootstrap = getBootstrap();
        if (!modal || !bootstrap)
            return;
        bootstrap.Modal.getInstance(modal)?.hide();
    };
    const normalizeContact = (item) => ({
        id: Number(item.id ?? item.Id ?? 0),
        firstName: String(item.firstName ?? item.FirstName ?? ''),
        lastName: String(item.lastName ?? item.LastName ?? ''),
        email: String(item.email ?? item.Email ?? ''),
        phone: String(item.phone ?? item.Phone ?? ''),
        position: item.position ?? item.Position ?? null,
        department: item.department ?? item.Department ?? null,
        notes: item.notes ?? item.Notes ?? null,
        isPrimary: Boolean(item.isPrimary ?? item.IsPrimary ?? false),
        isActive: Boolean(item.isActive ?? item.IsActive ?? false),
        isDefaultOwner: Boolean(item.isDefaultOwner ?? item.IsDefaultOwner ?? false),
        isDefaultBilling: Boolean(item.isDefaultBilling ?? item.IsDefaultBilling ?? false),
        isDefaultTech: Boolean(item.isDefaultTech ?? item.IsDefaultTech ?? false),
        isDefaultAdministrator: Boolean(item.isDefaultAdministrator ?? item.IsDefaultAdministrator ?? false),
        isDomainGlobal: Boolean(item.isDomainGlobal ?? item.IsDomainGlobal ?? false),
    });
    const renderContacts = () => {
        const body = document.getElementById('customer-details-contacts-body');
        if (!body)
            return;
        setText('customer-details-contact-count', String(allContacts.length));
        if (!allContacts.length) {
            body.innerHTML = '<tr><td colspan="7" class="text-center text-muted">No contact persons found.</td></tr>';
            return;
        }
        const rows = [...allContacts].sort((a, b) => Number(b.isPrimary) - Number(a.isPrimary) || a.id - b.id);
        body.innerHTML = rows.map((contact) => {
            const fullName = `${contact.firstName} ${contact.lastName}`.trim();
            return `
                <tr>
                    <td>${esc(fullName || '-')}</td>
                    <td><a href="mailto:${esc(contact.email)}">${esc(contact.email)}</a></td>
                    <td>${esc(contact.phone || '-')}</td>
                    <td>${esc(contact.position || '-')}</td>
                    <td>${contact.isPrimary ? '<span class="badge bg-info">Yes</span>' : '<span class="badge bg-secondary">No</span>'}</td>
                    <td>${contact.isActive ? '<span class="badge bg-success">Yes</span>' : '<span class="badge bg-secondary">No</span>'}</td>
                    <td class="text-end">
                        <button class="btn btn-sm btn-outline-primary" type="button" data-action="edit-contact" data-id="${contact.id}"><i class="bi bi-pencil"></i></button>
                    </td>
                </tr>
            `;
        }).join('');
    };
    const openContactCreate = () => {
        editingContactId = null;
        setText('customer-details-contact-modal-title', 'Add Contact Person');
        document.getElementById('customer-details-contact-form')?.reset();
        setCheckboxValue('customer-details-contact-active', true);
        showModal('customer-details-contact-modal');
    };
    const openContactEdit = (id) => {
        const contact = allContacts.find((item) => item.id === id);
        if (!contact)
            return;
        editingContactId = id;
        setText('customer-details-contact-modal-title', 'Edit Contact Person');
        setInputValue('customer-details-contact-first-name', contact.firstName);
        setInputValue('customer-details-contact-last-name', contact.lastName);
        setInputValue('customer-details-contact-email', contact.email);
        setInputValue('customer-details-contact-phone', contact.phone);
        setInputValue('customer-details-contact-position', contact.position || '');
        setInputValue('customer-details-contact-department', contact.department || '');
        setInputValue('customer-details-contact-notes', contact.notes || '');
        setCheckboxValue('customer-details-contact-primary', contact.isPrimary);
        setCheckboxValue('customer-details-contact-active', contact.isActive);
        setCheckboxValue('customer-details-contact-default-owner', contact.isDefaultOwner);
        setCheckboxValue('customer-details-contact-default-billing', contact.isDefaultBilling);
        setCheckboxValue('customer-details-contact-default-tech', contact.isDefaultTech);
        setCheckboxValue('customer-details-contact-default-admin', contact.isDefaultAdministrator);
        setCheckboxValue('customer-details-contact-domain-global', contact.isDomainGlobal);
        showModal('customer-details-contact-modal');
    };
    const loadContacts = async () => {
        const response = await apiRequest(`${getApiBaseUrl()}/ContactPersons/customer/${currentCustomerId}`, { method: 'GET' });
        if (!response.success) {
            showError(response.message || 'Failed to load contact persons.');
            return;
        }
        allContacts = (Array.isArray(response.data) ? response.data : []).map(normalizeContact);
        renderContacts();
    };
    const saveContact = async () => {
        const firstName = getInputValue('customer-details-contact-first-name');
        const lastName = getInputValue('customer-details-contact-last-name');
        const email = getInputValue('customer-details-contact-email');
        const phone = getInputValue('customer-details-contact-phone');
        if (!firstName || !lastName || !email || !phone) {
            showError('First name, last name, email and phone are required.');
            return;
        }
        const payload = {
            firstName,
            lastName,
            email,
            phone,
            position: getInputValue('customer-details-contact-position') || null,
            department: getInputValue('customer-details-contact-department') || null,
            notes: getInputValue('customer-details-contact-notes') || null,
            customerId: currentCustomerId,
            isPrimary: getCheckboxValue('customer-details-contact-primary'),
            isActive: getCheckboxValue('customer-details-contact-active'),
            isDefaultOwner: getCheckboxValue('customer-details-contact-default-owner'),
            isDefaultBilling: getCheckboxValue('customer-details-contact-default-billing'),
            isDefaultTech: getCheckboxValue('customer-details-contact-default-tech'),
            isDefaultAdministrator: getCheckboxValue('customer-details-contact-default-admin'),
            isDomainGlobal: getCheckboxValue('customer-details-contact-domain-global'),
        };
        const response = editingContactId
            ? await apiRequest(`${getApiBaseUrl()}/ContactPersons/${editingContactId}`, { method: 'PUT', body: JSON.stringify(payload) })
            : await apiRequest(`${getApiBaseUrl()}/ContactPersons`, { method: 'POST', body: JSON.stringify(payload) });
        if (!response.success) {
            showError(response.message || 'Failed to save contact person.');
            return;
        }
        hideModal('customer-details-contact-modal');
        showSuccess(editingContactId ? 'Contact person updated.' : 'Contact person created.');
        await loadContacts();
    };
    const renderCustomer = (customer) => {
        setText('customer-details-id', String(customer.id));
        setText('customer-details-reference', customer.formattedReferenceNumber || String(customer.referenceNumber ?? '-'));
        setText('customer-details-customer-number', customer.formattedCustomerNumber || String(customer.customerNumber ?? '-'));
        setText('customer-details-name', customer.name || '-');
        setText('customer-details-customer-name', customer.customerName || '-');
        setText('customer-details-email', customer.email || '-');
        setText('customer-details-billing-email', customer.billingEmail || '-');
        setText('customer-details-phone', customer.phone || '-');
        setText('customer-details-country', customer.countryCode || '-');
        setText('customer-details-tax-id', customer.taxId || '-');
        setText('customer-details-vat-number', customer.vatNumber || '-');
        setText('customer-details-currency', customer.preferredCurrency || '-');
        setText('customer-details-payment-method', customer.preferredPaymentMethod || '-');
        setText('customer-details-credit-limit', formatMoney(customer.creditLimit, customer.preferredCurrency || 'EUR'));
        setText('customer-details-balance', formatMoney(customer.balance, customer.preferredCurrency || 'EUR'));
        setText('customer-details-created', formatDate(customer.createdAt));
        setText('customer-details-notes', customer.notes || '-');
        setText('customer-details-status-badge', customer.status || '-');
    };
    const loadCustomer = async () => {
        const response = await apiRequest(`${getApiBaseUrl()}/Customers/${currentCustomerId}`, { method: 'GET' });
        if (!response.success || !response.data) {
            showError(response.message || 'Failed to load customer details.');
            return;
        }
        renderCustomer(response.data);
    };
    const initializePage = async () => {
        const page = document.getElementById('customer-details-page');
        if (!page || page.dataset.initialized === 'true')
            return;
        page.dataset.initialized = 'true';
        const params = new URLSearchParams(window.location.search);
        currentCustomerId = Number(params.get('customerId') ?? '0');
        if (!Number.isFinite(currentCustomerId) || currentCustomerId <= 0) {
            showError('customerId query parameter is required.');
            document.getElementById('customer-details-loading')?.classList.add('d-none');
            return;
        }
        document.getElementById('customer-details-contact-create')?.addEventListener('click', openContactCreate);
        document.getElementById('customer-details-customer-edit')?.addEventListener('click', () => {
            document.dispatchEvent(new CustomEvent('customers:open-edit', { detail: { id: currentCustomerId } }));
        });
        document.getElementById('customer-details-contact-save')?.addEventListener('click', () => { void saveContact(); });
        document.addEventListener('customers:saved', () => { void loadCustomer(); });
        document.getElementById('customer-details-contacts-body')?.addEventListener('click', (event) => {
            const target = event.target;
            const button = target.closest('button[data-action="edit-contact"]');
            if (!button)
                return;
            const id = Number(button.dataset.id ?? '0');
            if (!Number.isFinite(id) || id <= 0)
                return;
            openContactEdit(id);
        });
        await loadCustomer();
        await loadContacts();
        document.getElementById('customer-details-loading')?.classList.add('d-none');
        document.getElementById('customer-details-content')?.classList.remove('d-none');
    };
    const setupObserver = () => {
        void initializePage();
        if (document.body) {
            const observer = new MutationObserver(() => {
                const page = document.getElementById('customer-details-page');
                if (page && page.dataset.initialized !== 'true') {
                    void initializePage();
                }
            });
            observer.observe(document.body, { childList: true, subtree: true });
        }
    };
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', setupObserver);
    }
    else {
        setupObserver();
    }
})();
//# sourceMappingURL=customer-details.js.map
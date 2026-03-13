"use strict";
(() => {
    let rows = [];
    let selectedRegistrantRow = null;
    let customerOptions = [];
    const getApiBaseUrl = () => {
        const settings = window.AppSettings;
        return settings?.apiBaseUrl ?? '';
    };
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
    const parseString = (value) => (typeof value === 'string' ? value : '').trim();
    const parseNumber = (value) => {
        if (typeof value === 'number' && Number.isFinite(value)) {
            return value;
        }
        if (typeof value === 'string' && value.trim() !== '') {
            const parsed = Number(value);
            if (Number.isFinite(parsed)) {
                return parsed;
            }
        }
        return 0;
    };
    const apiRequest = async (endpoint, options = {}) => {
        try {
            const headers = {
                'Content-Type': 'application/json',
                ...options.headers,
            };
            const token = getAuthToken();
            if (token) {
                headers.Authorization = `Bearer ${token}`;
            }
            const response = await fetch(endpoint, {
                ...options,
                headers,
                credentials: 'include',
            });
            const contentType = response.headers.get('content-type') ?? '';
            const hasJson = contentType.includes('application/json');
            const body = hasJson ? await response.json() : null;
            if (!response.ok) {
                const errorBody = (body ?? {});
                return {
                    success: false,
                    message: errorBody.message ?? errorBody.title ?? `Request failed with status ${response.status}`,
                };
            }
            const envelope = (body ?? {});
            return {
                success: envelope.success !== false,
                data: envelope.data ?? body,
                message: envelope.message,
            };
        }
        catch {
            return {
                success: false,
                message: 'Network error. Please try again.',
            };
        }
    };
    const extractList = (payload) => {
        if (Array.isArray(payload)) {
            return payload;
        }
        const objectPayload = payload;
        if (Array.isArray(objectPayload?.items)) {
            return objectPayload.items;
        }
        if (Array.isArray(objectPayload?.Items)) {
            return objectPayload.Items;
        }
        if (Array.isArray(objectPayload?.data)) {
            return objectPayload.data;
        }
        if (Array.isArray(objectPayload?.Data)) {
            return objectPayload.Data;
        }
        return [];
    };
    const normalizeRegistrar = (item) => {
        const row = (item ?? {});
        return {
            id: parseNumber(row.id ?? row.Id),
            name: parseString(row.name ?? row.Name),
        };
    };
    const normalizeRegistrant = (item) => {
        const row = (item ?? {});
        return {
            organization: parseString(row.organization ?? row.Organization) || null,
            firstName: parseString(row.firstName ?? row.FirstName) || null,
            lastName: parseString(row.lastName ?? row.LastName) || null,
            email: parseString(row.email ?? row.Email) || null,
            countryCode: parseString(row.countryCode ?? row.CountryCode) || null,
            phone: parseString(row.phone ?? row.Phone) || null,
        };
    };
    const normalizeCustomerOption = (item) => {
        const row = (item ?? {});
        return {
            id: parseNumber(row.id ?? row.Id),
            name: parseString(row.name ?? row.Name),
            email: parseString(row.email ?? row.Email),
        };
    };
    const showError = (message) => {
        const alert = document.getElementById('domain-registrants-alert-error');
        if (!alert) {
            return;
        }
        alert.textContent = message;
        alert.classList.remove('d-none');
        document.getElementById('domain-registrants-alert-success')?.classList.add('d-none');
    };
    const hideError = () => {
        document.getElementById('domain-registrants-alert-error')?.classList.add('d-none');
    };
    const setLoading = (isLoading) => {
        document.getElementById('domain-registrants-loading')?.classList.toggle('d-none', !isLoading);
        document.getElementById('domain-registrants-content')?.classList.toggle('d-none', isLoading);
    };
    const showModal = (id) => {
        const element = document.getElementById(id);
        const bootstrap = window.bootstrap;
        if (!element || !bootstrap) {
            return;
        }
        const modal = new bootstrap.Modal(element);
        modal.show();
    };
    const hideModal = (id) => {
        const element = document.getElementById(id);
        const bootstrap = window.bootstrap;
        if (!element || !bootstrap) {
            return;
        }
        const modal = bootstrap.Modal.getInstance(element);
        modal?.hide();
    };
    const loadCustomerOptions = async () => {
        const response = await apiRequest(`${getApiBaseUrl()}/Customers?pageNumber=1&pageSize=200`, { method: 'GET' });
        if (!response.success) {
            customerOptions = [];
            return;
        }
        customerOptions = extractList(response.data)
            .map((item) => normalizeCustomerOption(item))
            .filter((item) => item.id > 0)
            .sort((a, b) => a.name.localeCompare(b.name));
        const select = document.getElementById('domain-registrants-company-select');
        if (!select) {
            return;
        }
        const options = customerOptions
            .map((item) => `<option value="${item.id}">${esc(item.name || item.email || `#${item.id}`)}</option>`)
            .join('');
        select.innerHTML = `<option value="">Select company</option>${options}`;
    };
    const checkEmailExists = async (email) => {
        if (!email) {
            return false;
        }
        const response = await apiRequest(`${getApiBaseUrl()}/Customers/check-email?email=${encodeURIComponent(email)}`, { method: 'GET' });
        if (!response.success || !response.data) {
            return false;
        }
        const payload = response.data;
        return payload.exists === true || payload.Exists === true;
    };
    const resolveEmailExistence = async (inputRows) => {
        const uniqueEmails = Array.from(new Set(inputRows
            .map((item) => item.rawEmail)
            .filter((email) => email.length > 0)));
        const checks = await Promise.all(uniqueEmails.map(async (email) => ({
            email,
            exists: await checkEmailExists(email),
        })));
        const existsMap = new Map();
        for (const check of checks) {
            existsMap.set(check.email.toLowerCase(), check.exists);
        }
        return inputRows.map((item) => ({
            ...item,
            emailExistsInCustomers: item.rawEmail ? (existsMap.get(item.rawEmail.toLowerCase()) ?? false) : false,
        }));
    };
    const createCustomerFromRegistrant = async (row) => {
        if (!row.rawEmail) {
            showError('Registrant email is required to create customer.');
            return;
        }
        const fullName = `${row.firstName} ${row.lastName}`.trim();
        const accountName = row.organization !== '-' ? row.organization : (fullName || row.rawEmail);
        const payload = {
            name: accountName,
            customerName: fullName || null,
            email: row.rawEmail,
            billingEmail: row.rawEmail,
            phone: row.phone !== '-' ? row.phone : 'N/A',
            countryCode: row.countryCode !== '-' ? row.countryCode : null,
            taxId: null,
            vatNumber: null,
            isCompany: row.organization !== '-',
            isSelfRegistered: false,
            isActive: true,
            status: 'Active',
            creditLimit: 0,
            notes: `Created semi-automatically from registrar registrant list (${row.registrarName}).`,
            preferredPaymentMethod: null,
            preferredCurrency: 'EUR',
            allowCurrencyOverride: true,
        };
        const response = await apiRequest(`${getApiBaseUrl()}/Customers`, {
            method: 'POST',
            body: JSON.stringify(payload),
        });
        if (!response.success) {
            showError(response.message || 'Failed to create customer from registrant.');
            return;
        }
        await loadRegistrants();
    };
    const openAddContactModal = async (row) => {
        selectedRegistrantRow = row;
        const selectedEmail = document.getElementById('domain-registrants-selected-email');
        if (selectedEmail) {
            selectedEmail.textContent = row.rawEmail || '-';
        }
        if (customerOptions.length === 0) {
            await loadCustomerOptions();
        }
        showModal('domain-registrants-company-modal');
    };
    const addRegistrantAsContact = async () => {
        if (!selectedRegistrantRow) {
            return;
        }
        const select = document.getElementById('domain-registrants-company-select');
        const selectedCustomerId = parseNumber(select?.value ?? '0');
        if (selectedCustomerId <= 0) {
            showError('Select a company/customer first.');
            return;
        }
        if (!selectedRegistrantRow.rawEmail) {
            showError('Registrant email is required to create contact person.');
            return;
        }
        const payload = {
            firstName: selectedRegistrantRow.firstName !== '-' ? selectedRegistrantRow.firstName : 'Unknown',
            lastName: selectedRegistrantRow.lastName !== '-' ? selectedRegistrantRow.lastName : 'Unknown',
            email: selectedRegistrantRow.rawEmail,
            phone: selectedRegistrantRow.phone !== '-' ? selectedRegistrantRow.phone : 'N/A',
            position: null,
            department: null,
            isPrimary: false,
            isActive: true,
            notes: `Added semi-automatically from registrar registrant list (${selectedRegistrantRow.registrarName}).`,
            customerId: selectedCustomerId,
            isDefaultOwner: false,
            isDefaultBilling: false,
            isDefaultTech: false,
            isDefaultAdministrator: false,
            isDomainGlobal: false,
        };
        const response = await apiRequest(`${getApiBaseUrl()}/ContactPersons`, {
            method: 'POST',
            body: JSON.stringify(payload),
        });
        if (!response.success) {
            showError(response.message || 'Failed to add registrant as contact person.');
            return;
        }
        hideModal('domain-registrants-company-modal');
    };
    const renderRows = () => {
        const body = document.getElementById('domain-registrants-table-body');
        if (!body) {
            return;
        }
        const countBadge = document.getElementById('domain-registrants-count');
        if (countBadge) {
            countBadge.textContent = String(rows.length);
        }
        if (!rows.length) {
            body.innerHTML = '<tr><td colspan="8" class="text-center text-muted">No registrants found.</td></tr>';
            return;
        }
        body.innerHTML = rows.map((row) => `
            <tr>
                <td>${esc(row.organization || '-')}</td>
                <td>${esc(row.firstName || '-')}</td>
                <td>${esc(row.lastName || '-')}</td>
                <td>${esc(row.email || '-')}</td>
                <td>${row.emailExistsInCustomers ? '<span class="badge bg-success">Yes</span>' : '<span class="badge bg-warning text-dark">No</span>'}</td>
                <td>${esc(row.countryCode || '-')}</td>
                <td>${esc(row.registrarName || '-')}</td>
                <td class="text-end">
                    ${row.emailExistsInCustomers ? '-' : `
                        <div class="btn-group btn-group-sm">
                            <button class="btn btn-outline-primary" type="button" data-action="create-customer" data-email="${esc(row.rawEmail)}"><i class="bi bi-person-plus"></i> Add customer</button>
                            <button class="btn btn-outline-secondary" type="button" data-action="add-contact" data-email="${esc(row.rawEmail)}"><i class="bi bi-building-add"></i> Add contact</button>
                        </div>
                    `}
                </td>
            </tr>
        `).join('');
    };
    const loadRegistrants = async () => {
        hideError();
        setLoading(true);
        const registrarResponse = await apiRequest(`${getApiBaseUrl()}/Registrars`, { method: 'GET' });
        if (!registrarResponse.success) {
            setLoading(false);
            showError(registrarResponse.message || 'Failed to load registrars.');
            return;
        }
        const registrars = extractList(registrarResponse.data)
            .map((item) => normalizeRegistrar(item))
            .filter((item) => item.id > 0 && item.name);
        const contactResponses = await Promise.all(registrars.map(async (registrar) => {
            const response = await apiRequest(`${getApiBaseUrl()}/Registrars/${registrar.id}/contacts/registrants`, { method: 'GET' });
            return { registrar, response };
        }));
        const mergedRows = [];
        for (const item of contactResponses) {
            if (!item.response.success) {
                continue;
            }
            const contacts = extractList(item.response.data)
                .map((entry) => normalizeRegistrant(entry));
            for (const contact of contacts) {
                mergedRows.push({
                    organization: contact.organization ?? '-',
                    firstName: contact.firstName ?? '-',
                    lastName: contact.lastName ?? '-',
                    email: contact.email ?? '-',
                    rawEmail: contact.email ?? '',
                    countryCode: contact.countryCode ?? '-',
                    registrarName: item.registrar.name,
                    phone: contact.phone ?? '-',
                    emailExistsInCustomers: false,
                });
            }
        }
        rows = (await resolveEmailExistence(mergedRows)).sort((a, b) => {
            const org = a.organization.localeCompare(b.organization);
            if (org !== 0) {
                return org;
            }
            const last = a.lastName.localeCompare(b.lastName);
            if (last !== 0) {
                return last;
            }
            return a.firstName.localeCompare(b.firstName);
        });
        setLoading(false);
        renderRows();
    };
    const bindEvents = () => {
        document.getElementById('domain-registrants-refresh')?.addEventListener('click', () => {
            void loadRegistrants();
        });
        document.getElementById('domain-registrants-table-body')?.addEventListener('click', (event) => {
            const target = event.target;
            const button = target.closest('button[data-action]');
            if (!button) {
                return;
            }
            const email = parseString(button.dataset.email ?? '');
            const row = rows.find((item) => item.rawEmail.toLowerCase() === email.toLowerCase());
            if (!row) {
                return;
            }
            if (button.dataset.action === 'create-customer') {
                void createCustomerFromRegistrant(row);
                return;
            }
            if (button.dataset.action === 'add-contact') {
                void openAddContactModal(row);
            }
        });
        document.getElementById('domain-registrants-add-contact-confirm')?.addEventListener('click', () => {
            void addRegistrantAsContact();
        });
    };
    const initializePage = () => {
        const page = document.getElementById('domain-registrants-page');
        if (!page || page.dataset.initialized === 'true') {
            return;
        }
        page.dataset.initialized = 'true';
        bindEvents();
        void loadRegistrants();
    };
    const setupObserver = () => {
        initializePage();
        if (document.body) {
            const observer = new MutationObserver(() => {
                const page = document.getElementById('domain-registrants-page');
                if (page && page.dataset.initialized !== 'true') {
                    initializePage();
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
//# sourceMappingURL=domain-registrants.js.map
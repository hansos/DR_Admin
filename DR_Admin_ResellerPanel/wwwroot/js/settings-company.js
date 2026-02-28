"use strict";
(() => {
    function getApiBaseUrl() {
        const appSettings = window.AppSettings;
        return appSettings?.apiBaseUrl ?? '';
    }
    function getAuthToken() {
        const auth = window.Auth;
        if (auth?.getToken) {
            return auth.getToken();
        }
        return sessionStorage.getItem('rp_authToken');
    }
    async function apiRequest(url, options) {
        try {
            const headers = new Headers(options.headers);
            headers.set('Content-Type', 'application/json');
            const token = getAuthToken();
            if (token) {
                headers.set('Authorization', `Bearer ${token}`);
            }
            const response = await fetch(url, {
                ...options,
                headers,
                credentials: 'include',
            });
            const contentType = response.headers.get('content-type') ?? '';
            const isJson = contentType.includes('application/json');
            const body = isJson ? (await response.json()) : null;
            if (!response.ok) {
                return {
                    ok: false,
                    status: response.status,
                    data: null,
                    message: extractErrorMessage(body, response.status),
                };
            }
            return {
                ok: true,
                status: response.status,
                data: body,
                message: '',
            };
        }
        catch {
            return {
                ok: false,
                status: 0,
                data: null,
                message: 'Network error. Please try again.',
            };
        }
    }
    function extractErrorMessage(body, status) {
        if (typeof body === 'string' && body.length > 0) {
            return body;
        }
        if (body && typeof body === 'object') {
            const map = body;
            if (typeof map.message === 'string' && map.message.length > 0) {
                return map.message;
            }
            if (typeof map.title === 'string' && map.title.length > 0) {
                return map.title;
            }
        }
        if (status === 404) {
            return 'Company profile not found yet. Fill in the form and save to create it.';
        }
        return `Request failed with status ${status}`;
    }
    function valueOrNull(value) {
        const trimmed = value.trim();
        return trimmed.length > 0 ? trimmed : null;
    }
    function getInputValue(id) {
        const input = document.getElementById(id);
        return input?.value ?? '';
    }
    function setInputValue(id, value) {
        const input = document.getElementById(id);
        if (input) {
            input.value = value ?? '';
        }
    }
    function showSuccess(message) {
        const success = document.getElementById('settings-company-alert-success');
        const error = document.getElementById('settings-company-alert-error');
        if (success) {
            success.textContent = message;
            success.classList.remove('d-none');
        }
        error?.classList.add('d-none');
        setTimeout(() => {
            success?.classList.add('d-none');
        }, 4000);
    }
    function showError(message) {
        const success = document.getElementById('settings-company-alert-success');
        const error = document.getElementById('settings-company-alert-error');
        if (error) {
            error.textContent = message;
            error.classList.remove('d-none');
        }
        success?.classList.add('d-none');
    }
    function mapCompany(data) {
        if (!data || typeof data !== 'object') {
            return null;
        }
        const item = data;
        return {
            id: toNumber(item.id, item.Id),
            name: toString(item.name, item.Name) ?? '',
            legalName: toString(item.legalName, item.LegalName),
            email: toString(item.email, item.Email),
            phone: toString(item.phone, item.Phone),
            addressLine1: toString(item.addressLine1, item.AddressLine1),
            addressLine2: toString(item.addressLine2, item.AddressLine2),
            postalCode: toString(item.postalCode, item.PostalCode),
            city: toString(item.city, item.City),
            state: toString(item.state, item.State),
            countryCode: toString(item.countryCode, item.CountryCode),
            organizationNumber: toString(item.organizationNumber, item.OrganizationNumber),
            taxId: toString(item.taxId, item.TaxId),
            vatNumber: toString(item.vatNumber, item.VatNumber),
            invoiceEmail: toString(item.invoiceEmail, item.InvoiceEmail),
            website: toString(item.website, item.Website),
            logoUrl: toString(item.logoUrl, item.LogoUrl),
            letterheadFooter: toString(item.letterheadFooter, item.LetterheadFooter),
        };
    }
    function toString(...values) {
        for (const value of values) {
            if (typeof value === 'string') {
                return value;
            }
        }
        return null;
    }
    function toNumber(...values) {
        for (const value of values) {
            if (typeof value === 'number' && Number.isFinite(value)) {
                return value;
            }
            if (typeof value === 'string' && value.trim().length > 0) {
                const parsed = Number(value);
                if (Number.isFinite(parsed)) {
                    return parsed;
                }
            }
        }
        return 0;
    }
    function fillForm(company) {
        setInputValue('settings-company-name', company.name);
        setInputValue('settings-company-legal-name', company.legalName);
        setInputValue('settings-company-email', company.email);
        setInputValue('settings-company-invoice-email', company.invoiceEmail);
        setInputValue('settings-company-phone', company.phone);
        setInputValue('settings-company-address1', company.addressLine1);
        setInputValue('settings-company-address2', company.addressLine2);
        setInputValue('settings-company-postal-code', company.postalCode);
        setInputValue('settings-company-city', company.city);
        setInputValue('settings-company-state', company.state);
        setInputValue('settings-company-country-code', company.countryCode);
        setInputValue('settings-company-org-number', company.organizationNumber);
        setInputValue('settings-company-tax-id', company.taxId);
        setInputValue('settings-company-vat-number', company.vatNumber);
        setInputValue('settings-company-website', company.website);
        setInputValue('settings-company-logo-url', company.logoUrl);
        setInputValue('settings-company-letterhead-footer', company.letterheadFooter);
    }
    function collectPayload() {
        return {
            name: getInputValue('settings-company-name').trim(),
            legalName: valueOrNull(getInputValue('settings-company-legal-name')),
            email: valueOrNull(getInputValue('settings-company-email')),
            invoiceEmail: valueOrNull(getInputValue('settings-company-invoice-email')),
            phone: valueOrNull(getInputValue('settings-company-phone')),
            addressLine1: valueOrNull(getInputValue('settings-company-address1')),
            addressLine2: valueOrNull(getInputValue('settings-company-address2')),
            postalCode: valueOrNull(getInputValue('settings-company-postal-code')),
            city: valueOrNull(getInputValue('settings-company-city')),
            state: valueOrNull(getInputValue('settings-company-state')),
            countryCode: valueOrNull(getInputValue('settings-company-country-code')),
            organizationNumber: valueOrNull(getInputValue('settings-company-org-number')),
            taxId: valueOrNull(getInputValue('settings-company-tax-id')),
            vatNumber: valueOrNull(getInputValue('settings-company-vat-number')),
            website: valueOrNull(getInputValue('settings-company-website')),
            logoUrl: valueOrNull(getInputValue('settings-company-logo-url')),
            letterheadFooter: valueOrNull(getInputValue('settings-company-letterhead-footer')),
        };
    }
    async function loadCompany() {
        const response = await apiRequest(`${getApiBaseUrl()}/MyCompany`, { method: 'GET' });
        if (response.ok && response.data) {
            const company = mapCompany(response.data);
            if (company) {
                fillForm(company);
            }
            return;
        }
        if (response.status !== 404) {
            showError(response.message || 'Failed to load company profile');
        }
    }
    async function saveCompany() {
        const payload = collectPayload();
        if (payload.name.length === 0) {
            showError('Company name is required');
            return;
        }
        const response = await apiRequest(`${getApiBaseUrl()}/MyCompany`, {
            method: 'PUT',
            body: JSON.stringify(payload),
        });
        if (!response.ok) {
            showError(response.message || 'Failed to save company profile');
            return;
        }
        showSuccess('Company profile saved successfully');
    }
    function initializeCompanyPage() {
        const page = document.getElementById('settings-company-page');
        if (!page || page.dataset.initialized === 'true') {
            return;
        }
        page.dataset.initialized = 'true';
        const saveButton = document.getElementById('settings-company-save');
        saveButton?.addEventListener('click', () => {
            void saveCompany();
        });
        void loadCompany();
    }
    function setupPageObserver() {
        initializeCompanyPage();
        if (!document.body) {
            return;
        }
        const observer = new MutationObserver(() => {
            const page = document.getElementById('settings-company-page');
            if (page && page.dataset.initialized !== 'true') {
                initializeCompanyPage();
            }
        });
        observer.observe(document.body, { childList: true, subtree: true });
    }
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', setupPageObserver);
    }
    else {
        setupPageObserver();
    }
})();
//# sourceMappingURL=settings-company.js.map
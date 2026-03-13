"use strict";
(() => {
    let allRules = [];
    let tldOptions = [];
    let editingId = null;
    let pendingDeleteId = null;
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
    const getBootstrap = () => {
        return window.bootstrap ?? null;
    };
    const showModal = (id) => {
        const modal = document.getElementById(id);
        const bootstrap = getBootstrap();
        if (!modal || !bootstrap) {
            return;
        }
        new bootstrap.Modal(modal).show();
    };
    const hideModal = (id) => {
        const modal = document.getElementById(id);
        const bootstrap = getBootstrap();
        if (!modal || !bootstrap) {
            return;
        }
        bootstrap.Modal.getInstance(modal)?.hide();
    };
    const esc = (text) => {
        const map = { '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#039;' };
        return (text || '').replace(/[&<>"']/g, (char) => map[char] ?? char);
    };
    const isRecord = (value) => typeof value === 'object' && value !== null;
    const asString = (value) => typeof value === 'string' ? value : '';
    const asBoolean = (value, fallback = false) => typeof value === 'boolean' ? value : fallback;
    const asNumber = (value) => typeof value === 'number' && Number.isFinite(value) ? value : null;
    const asArray = (value) => {
        if (Array.isArray(value)) {
            return value;
        }
        if (!isRecord(value)) {
            return [];
        }
        const candidates = [value.data, value.Data, value.items, value.Items];
        for (const candidate of candidates) {
            if (Array.isArray(candidate)) {
                return candidate;
            }
        }
        return [];
    };
    const apiRequest = async (endpoint, options = {}) => {
        try {
            const headers = {
                'Content-Type': 'application/json',
                ...options.headers,
            };
            const authToken = getAuthToken();
            if (authToken) {
                headers.Authorization = `Bearer ${authToken}`;
            }
            const response = await fetch(endpoint, {
                ...options,
                headers,
                credentials: 'include',
            });
            const contentType = response.headers.get('content-type') ?? '';
            const hasJson = contentType.includes('application/json');
            const data = hasJson ? await response.json() : null;
            if (!response.ok) {
                let message = `Request failed with status ${response.status}`;
                if (isRecord(data)) {
                    const msg = data.message ?? data.title;
                    if (typeof msg === 'string' && msg.trim()) {
                        message = msg;
                    }
                }
                return { success: false, message };
            }
            if (isRecord(data) && data.success === false) {
                return {
                    success: false,
                    message: asString(data.message) || 'Request failed',
                };
            }
            if (isRecord(data) && 'data' in data) {
                return {
                    success: true,
                    data: data.data,
                    message: asString(data.message),
                };
            }
            return { success: true, data: data };
        }
        catch {
            return {
                success: false,
                message: 'Network error. Please try again.',
            };
        }
    };
    const normalizeTld = (item) => {
        const row = isRecord(item) ? item : {};
        return {
            id: asNumber(row.id ?? row.Id) ?? 0,
            extension: asString(row.extension ?? row.Extension).replace(/^\./, ''),
            isActive: asBoolean(row.isActive ?? row.IsActive),
        };
    };
    const normalizeRule = (item) => {
        const row = isRecord(item) ? item : {};
        return {
            id: asNumber(row.id ?? row.Id) ?? 0,
            tldId: asNumber(row.tldId ?? row.TldId) ?? 0,
            tldExtension: asString(row.tldExtension ?? row.TldExtension),
            requireRegistrantContact: asBoolean(row.requireRegistrantContact ?? row.RequireRegistrantContact),
            requireAdministrativeContact: asBoolean(row.requireAdministrativeContact ?? row.RequireAdministrativeContact),
            requireTechnicalContact: asBoolean(row.requireTechnicalContact ?? row.RequireTechnicalContact),
            requireBillingContact: asBoolean(row.requireBillingContact ?? row.RequireBillingContact),
            requiresAuthCodeForTransfer: asBoolean(row.requiresAuthCodeForTransfer ?? row.RequiresAuthCodeForTransfer),
            transferLockDays: asNumber(row.transferLockDays ?? row.TransferLockDays),
            renewalGracePeriodDays: asNumber(row.renewalGracePeriodDays ?? row.RenewalGracePeriodDays),
            redemptionGracePeriodDays: asNumber(row.redemptionGracePeriodDays ?? row.RedemptionGracePeriodDays),
            notes: asString(row.notes ?? row.Notes) || null,
            isActive: asBoolean(row.isActive ?? row.IsActive, true),
        };
    };
    const setInput = (id, value) => {
        const el = document.getElementById(id);
        if (el) {
            el.value = value;
        }
    };
    const getInput = (id) => {
        const el = document.getElementById(id);
        return (el?.value ?? '').trim();
    };
    const setCheck = (id, value) => {
        const el = document.getElementById(id);
        if (el) {
            el.checked = value;
        }
    };
    const getCheck = (id) => {
        const el = document.getElementById(id);
        return !!el?.checked;
    };
    const showSuccess = (message) => {
        const alert = document.getElementById('tld-registry-rules-alert-success');
        if (!alert) {
            return;
        }
        alert.textContent = message;
        alert.classList.remove('d-none');
        document.getElementById('tld-registry-rules-alert-error')?.classList.add('d-none');
    };
    const showError = (message) => {
        const alert = document.getElementById('tld-registry-rules-alert-error');
        if (!alert) {
            return;
        }
        alert.textContent = message;
        alert.classList.remove('d-none');
        document.getElementById('tld-registry-rules-alert-success')?.classList.add('d-none');
    };
    const renderTable = () => {
        const tableBody = document.getElementById('tld-registry-rules-table-body');
        if (!tableBody) {
            return;
        }
        if (allRules.length === 0) {
            tableBody.innerHTML = '<tr><td colspan="6" class="text-center text-muted">No registry rules found.</td></tr>';
            return;
        }
        tableBody.innerHTML = allRules.map((rule) => {
            const contacts = [];
            if (rule.requireRegistrantContact)
                contacts.push('Registrant');
            if (rule.requireAdministrativeContact)
                contacts.push('Admin');
            if (rule.requireTechnicalContact)
                contacts.push('Tech');
            if (rule.requireBillingContact)
                contacts.push('Billing');
            const grace = `Renewal ${rule.renewalGracePeriodDays ?? '-'}d, Redemption ${rule.redemptionGracePeriodDays ?? '-'}d`;
            const transfer = `${rule.requiresAuthCodeForTransfer ? 'Auth code' : 'No auth code'}, lock ${rule.transferLockDays ?? '-'}d`;
            return `
                <tr>
                    <td><code>.${esc(rule.tldExtension || '')}</code></td>
                    <td>${esc(contacts.length > 0 ? contacts.join(', ') : 'None')}</td>
                    <td>${esc(transfer)}</td>
                    <td>${esc(grace)}</td>
                    <td><span class="badge bg-${rule.isActive ? 'success' : 'secondary'}">${rule.isActive ? 'Active' : 'Inactive'}</span></td>
                    <td class="text-end">
                        <div class="btn-group btn-group-sm">
                            <button class="btn btn-outline-primary" type="button" data-action="edit" data-id="${rule.id}" title="Edit"><i class="bi bi-pencil"></i></button>
                            <button class="btn btn-outline-danger" type="button" data-action="delete" data-id="${rule.id}" data-name=".${esc(rule.tldExtension)}" title="Delete"><i class="bi bi-trash"></i></button>
                        </div>
                    </td>
                </tr>
            `;
        }).join('');
    };
    const loadTldOptions = async () => {
        const response = await apiRequest(`${getApiBaseUrl()}/Tlds/active`, { method: 'GET' });
        if (!response.success) {
            showError(response.message || 'Failed to load TLD list.');
            return;
        }
        tldOptions = asArray(response.data)
            .map((item) => normalizeTld(item))
            .filter((item) => item.id > 0 && item.extension)
            .sort((a, b) => a.extension.localeCompare(b.extension));
        const select = document.getElementById('tld-registry-rules-tld-id');
        if (!select) {
            return;
        }
        select.innerHTML = tldOptions
            .map((item) => `<option value="${item.id}">.${esc(item.extension)}</option>`)
            .join('');
    };
    const loadRules = async () => {
        const tableBody = document.getElementById('tld-registry-rules-table-body');
        if (tableBody) {
            tableBody.innerHTML = '<tr><td colspan="6" class="text-center"><div class="spinner-border text-primary"></div></td></tr>';
        }
        const response = await apiRequest(`${getApiBaseUrl()}/TldRegistryRules`, { method: 'GET' });
        if (!response.success) {
            showError(response.message || 'Failed to load registry rules.');
            allRules = [];
            renderTable();
            return;
        }
        allRules = asArray(response.data)
            .map((item) => normalizeRule(item))
            .sort((a, b) => a.tldExtension.localeCompare(b.tldExtension));
        renderTable();
    };
    const openCreate = () => {
        editingId = null;
        document.getElementById('tld-registry-rules-form')?.reset();
        const title = document.getElementById('tld-registry-rules-modal-title');
        if (title) {
            title.textContent = 'New Registry Rule';
        }
        setInput('tld-registry-rules-is-active', 'true');
        setCheck('tld-registry-rules-requires-auth-code', true);
        setCheck('tld-registry-rules-require-registrant', true);
        showModal('tld-registry-rules-edit-modal');
    };
    const openEdit = (id) => {
        const rule = allRules.find((item) => item.id === id);
        if (!rule) {
            return;
        }
        editingId = id;
        const title = document.getElementById('tld-registry-rules-modal-title');
        if (title) {
            title.textContent = 'Edit Registry Rule';
        }
        setInput('tld-registry-rules-tld-id', String(rule.tldId));
        setInput('tld-registry-rules-transfer-lock-days', rule.transferLockDays?.toString() ?? '');
        setInput('tld-registry-rules-renewal-grace-days', rule.renewalGracePeriodDays?.toString() ?? '');
        setInput('tld-registry-rules-redemption-grace-days', rule.redemptionGracePeriodDays?.toString() ?? '');
        setInput('tld-registry-rules-notes', rule.notes ?? '');
        setInput('tld-registry-rules-is-active', rule.isActive ? 'true' : 'false');
        setCheck('tld-registry-rules-requires-auth-code', rule.requiresAuthCodeForTransfer);
        setCheck('tld-registry-rules-require-registrant', rule.requireRegistrantContact);
        setCheck('tld-registry-rules-require-admin', rule.requireAdministrativeContact);
        setCheck('tld-registry-rules-require-tech', rule.requireTechnicalContact);
        setCheck('tld-registry-rules-require-billing', rule.requireBillingContact);
        showModal('tld-registry-rules-edit-modal');
    };
    const saveRule = async () => {
        const tldId = Number(getInput('tld-registry-rules-tld-id'));
        if (!Number.isFinite(tldId) || tldId <= 0) {
            showError('Select a valid TLD.');
            return;
        }
        const transferLockDaysRaw = getInput('tld-registry-rules-transfer-lock-days');
        const renewalGraceDaysRaw = getInput('tld-registry-rules-renewal-grace-days');
        const redemptionGraceDaysRaw = getInput('tld-registry-rules-redemption-grace-days');
        const payload = {
            tldId,
            requireRegistrantContact: getCheck('tld-registry-rules-require-registrant'),
            requireAdministrativeContact: getCheck('tld-registry-rules-require-admin'),
            requireTechnicalContact: getCheck('tld-registry-rules-require-tech'),
            requireBillingContact: getCheck('tld-registry-rules-require-billing'),
            requiresAuthCodeForTransfer: getCheck('tld-registry-rules-requires-auth-code'),
            transferLockDays: transferLockDaysRaw ? Number(transferLockDaysRaw) : null,
            renewalGracePeriodDays: renewalGraceDaysRaw ? Number(renewalGraceDaysRaw) : null,
            redemptionGracePeriodDays: redemptionGraceDaysRaw ? Number(redemptionGraceDaysRaw) : null,
            notes: getInput('tld-registry-rules-notes') || null,
            isActive: getInput('tld-registry-rules-is-active') === 'true',
        };
        const response = editingId
            ? await apiRequest(`${getApiBaseUrl()}/TldRegistryRules/${editingId}`, { method: 'PUT', body: JSON.stringify(payload) })
            : await apiRequest(`${getApiBaseUrl()}/TldRegistryRules`, { method: 'POST', body: JSON.stringify(payload) });
        if (!response.success) {
            showError(response.message || 'Failed to save registry rule.');
            return;
        }
        hideModal('tld-registry-rules-edit-modal');
        showSuccess(editingId ? 'Registry rule updated successfully.' : 'Registry rule created successfully.');
        await loadRules();
    };
    const openDelete = (id, name) => {
        pendingDeleteId = id;
        const deleteName = document.getElementById('tld-registry-rules-delete-name');
        if (deleteName) {
            deleteName.textContent = name;
        }
        showModal('tld-registry-rules-delete-modal');
    };
    const doDelete = async () => {
        if (!pendingDeleteId) {
            return;
        }
        const response = await apiRequest(`${getApiBaseUrl()}/TldRegistryRules/${pendingDeleteId}`, { method: 'DELETE' });
        hideModal('tld-registry-rules-delete-modal');
        if (!response.success) {
            showError(response.message || 'Failed to delete registry rule.');
            return;
        }
        pendingDeleteId = null;
        showSuccess('Registry rule deleted successfully.');
        await loadRules();
    };
    const bindEvents = () => {
        document.getElementById('tld-registry-rules-create')?.addEventListener('click', openCreate);
        document.getElementById('tld-registry-rules-refresh')?.addEventListener('click', () => {
            void loadRules();
        });
        document.getElementById('tld-registry-rules-save')?.addEventListener('click', () => {
            void saveRule();
        });
        document.getElementById('tld-registry-rules-confirm-delete')?.addEventListener('click', () => {
            void doDelete();
        });
        document.getElementById('tld-registry-rules-table-body')?.addEventListener('click', (event) => {
            const target = event.target;
            const button = target.closest('button[data-action]');
            if (!button) {
                return;
            }
            const id = Number(button.dataset.id ?? '0');
            if (!Number.isFinite(id) || id <= 0) {
                return;
            }
            if (button.dataset.action === 'edit') {
                openEdit(id);
                return;
            }
            if (button.dataset.action === 'delete') {
                openDelete(id, button.dataset.name ?? 'selected rule');
            }
        });
    };
    const initializePage = () => {
        const page = document.getElementById('tld-registry-rules-page');
        if (!page || page.dataset.initialized === 'true') {
            return;
        }
        page.dataset.initialized = 'true';
        bindEvents();
        void loadTldOptions();
        void loadRules();
    };
    const setupObserver = () => {
        initializePage();
        if (document.body) {
            const observer = new MutationObserver(() => {
                const page = document.getElementById('tld-registry-rules-page');
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
//# sourceMappingURL=tld-registry-rules.js.map
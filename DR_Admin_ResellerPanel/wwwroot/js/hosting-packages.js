"use strict";
(() => {
    let allHostingPackages = [];
    let editingId = null;
    let pendingDeleteId = null;
    let isSaving = false;
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
        const maybeBootstrap = window.bootstrap;
        return maybeBootstrap ?? null;
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
                return {
                    success: false,
                    message: (data && (data.message ?? data.title)) ||
                        `Request failed with status ${response.status}`,
                };
            }
            const parsed = data;
            return {
                success: parsed?.success !== false,
                data: parsed?.data ?? data,
                message: parsed?.message,
            };
        }
        catch (error) {
            console.error('Hosting packages request failed', error);
            return {
                success: false,
                message: 'Network error. Please try again.',
            };
        }
    };
    const esc = (text) => {
        const map = { '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#039;' };
        return (text || '').replace(/[&<>"']/g, (char) => map[char] ?? char);
    };
    const showSuccess = (message) => {
        const alert = document.getElementById('hosting-packages-alert-success');
        if (!alert) {
            return;
        }
        alert.textContent = message;
        alert.classList.remove('d-none');
        document.getElementById('hosting-packages-alert-error')?.classList.add('d-none');
        setTimeout(() => alert.classList.add('d-none'), 5000);
    };
    const showError = (message) => {
        const alert = document.getElementById('hosting-packages-alert-error');
        if (!alert) {
            return;
        }
        alert.textContent = message;
        alert.classList.remove('d-none');
        document.getElementById('hosting-packages-alert-success')?.classList.add('d-none');
    };
    const showModal = (id) => {
        const element = document.getElementById(id);
        const bootstrap = getBootstrap();
        if (!element || !bootstrap) {
            return;
        }
        const modal = new bootstrap.Modal(element);
        modal.show();
    };
    const hideModal = (id) => {
        const element = document.getElementById(id);
        const bootstrap = getBootstrap();
        if (!element || !bootstrap) {
            return;
        }
        const modal = bootstrap.Modal.getInstance(element);
        modal?.hide();
    };
    const normalizeHostingPackage = (item) => {
        const typed = (item ?? {});
        return {
            id: Number(typed.id ?? typed.Id ?? 0),
            name: String(typed.name ?? typed.Name ?? ''),
            description: String(typed.description ?? typed.Description ?? '') || null,
            diskSpaceMB: Number(typed.diskSpaceMB ?? typed.DiskSpaceMB ?? 0),
            bandwidthMB: Number(typed.bandwidthMB ?? typed.BandwidthMB ?? 0),
            emailAccounts: Number(typed.emailAccounts ?? typed.EmailAccounts ?? 0),
            databases: Number(typed.databases ?? typed.Databases ?? 0),
            domains: Number(typed.domains ?? typed.Domains ?? 0),
            subdomains: Number(typed.subdomains ?? typed.Subdomains ?? 0),
            ftpAccounts: Number(typed.ftpAccounts ?? typed.FtpAccounts ?? 0),
            sslSupport: (typed.sslSupport ?? typed.SslSupport ?? false) === true,
            backupSupport: (typed.backupSupport ?? typed.BackupSupport ?? false) === true,
            dedicatedIp: (typed.dedicatedIp ?? typed.DedicatedIp ?? false) === true,
            monthlyPrice: Number(typed.monthlyPrice ?? typed.MonthlyPrice ?? 0),
            yearlyPrice: Number(typed.yearlyPrice ?? typed.YearlyPrice ?? 0),
            isActive: (typed.isActive ?? typed.IsActive ?? false) === true,
        };
    };
    const renderTable = () => {
        const tableBody = document.getElementById('hosting-packages-table-body');
        if (!tableBody) {
            return;
        }
        if (!allHostingPackages.length) {
            tableBody.innerHTML = '<tr><td colspan="7" class="text-center text-muted">No hosting plans found.</td></tr>';
            return;
        }
        tableBody.innerHTML = allHostingPackages.map((item) => {
            const resources = `${item.diskSpaceMB} MB disk / ${item.bandwidthMB} MB bandwidth`;
            const features = [
                item.sslSupport ? 'SSL' : null,
                item.backupSupport ? 'Backup' : null,
                item.dedicatedIp ? 'Dedicated IP' : null,
            ].filter((x) => !!x).join(', ') || '—';
            return `
                <tr>
                    <td>${item.id}</td>
                    <td>${esc(item.name)}</td>
                    <td>${esc(resources)}</td>
                    <td>${esc(features)}</td>
                    <td>${esc(item.monthlyPrice.toFixed(2))} / ${esc(item.yearlyPrice.toFixed(2))}</td>
                    <td><span class="badge bg-${item.isActive ? 'success' : 'secondary'}">${item.isActive ? 'Active' : 'Inactive'}</span></td>
                    <td class="text-end">
                        <div class="btn-group btn-group-sm">
                            <button class="btn btn-outline-primary" type="button" data-action="edit" data-id="${item.id}"><i class="bi bi-pencil"></i></button>
                            <button class="btn btn-outline-danger" type="button" data-action="delete" data-id="${item.id}" data-name="${esc(item.name)}"><i class="bi bi-trash"></i></button>
                        </div>
                    </td>
                </tr>
            `;
        }).join('');
    };
    const loadHostingPackages = async () => {
        const tableBody = document.getElementById('hosting-packages-table-body');
        if (tableBody) {
            tableBody.innerHTML = '<tr><td colspan="7" class="text-center"><div class="spinner-border text-primary"></div></td></tr>';
        }
        const response = await apiRequest(`${getApiBaseUrl()}/HostingPackages`, { method: 'GET' });
        if (!response.success) {
            showError(response.message || 'Failed to load hosting plans');
            if (tableBody) {
                tableBody.innerHTML = '<tr><td colspan="7" class="text-center text-danger">Failed to load data</td></tr>';
            }
            return;
        }
        const raw = response.data;
        const list = Array.isArray(raw)
            ? raw
            : Array.isArray(raw?.data)
                ? (raw.data ?? [])
                : [];
        allHostingPackages = list.map((item) => normalizeHostingPackage(item));
        renderTable();
    };
    const setInputValue = (id, value) => {
        const el = document.getElementById(id);
        if (el) {
            el.value = value;
        }
    };
    const setCheckbox = (id, value) => {
        const el = document.getElementById(id);
        if (el) {
            el.checked = value;
        }
    };
    const getInputValue = (id) => {
        const el = document.getElementById(id);
        return (el?.value ?? '').trim();
    };
    const getCheckbox = (id) => {
        const el = document.getElementById(id);
        return !!el?.checked;
    };
    const parseNonNegativeInt = (id) => {
        const value = Number(getInputValue(id));
        if (!Number.isFinite(value) || value < 0) {
            return null;
        }
        return Math.floor(value);
    };
    const parseNonNegativeDecimal = (id) => {
        const value = Number(getInputValue(id));
        if (!Number.isFinite(value) || value < 0) {
            return null;
        }
        return value;
    };
    const openCreate = () => {
        editingId = null;
        const title = document.getElementById('hosting-packages-modal-title');
        if (title) {
            title.textContent = 'New Hosting Plan';
        }
        document.getElementById('hosting-packages-form')?.reset();
        setCheckbox('hosting-packages-is-active', true);
        showModal('hosting-packages-edit-modal');
    };
    const openEdit = (id) => {
        const item = allHostingPackages.find((x) => x.id === id);
        if (!item) {
            return;
        }
        editingId = id;
        const title = document.getElementById('hosting-packages-modal-title');
        if (title) {
            title.textContent = 'Edit Hosting Plan';
        }
        setInputValue('hosting-packages-name', item.name);
        setInputValue('hosting-packages-description', item.description ?? '');
        setInputValue('hosting-packages-disk-space', String(item.diskSpaceMB));
        setInputValue('hosting-packages-bandwidth', String(item.bandwidthMB));
        setInputValue('hosting-packages-email-accounts', String(item.emailAccounts));
        setInputValue('hosting-packages-databases', String(item.databases));
        setInputValue('hosting-packages-domains', String(item.domains));
        setInputValue('hosting-packages-subdomains', String(item.subdomains));
        setInputValue('hosting-packages-ftp-accounts', String(item.ftpAccounts));
        setInputValue('hosting-packages-monthly-price', String(item.monthlyPrice));
        setInputValue('hosting-packages-yearly-price', String(item.yearlyPrice));
        setCheckbox('hosting-packages-ssl-support', item.sslSupport);
        setCheckbox('hosting-packages-backup-support', item.backupSupport);
        setCheckbox('hosting-packages-dedicated-ip', item.dedicatedIp);
        setCheckbox('hosting-packages-is-active', item.isActive);
        showModal('hosting-packages-edit-modal');
    };
    const saveHostingPackage = async () => {
        if (isSaving) {
            return;
        }
        isSaving = true;
        const saveButton = document.getElementById('hosting-packages-save');
        if (saveButton) {
            saveButton.disabled = true;
        }
        try {
            const name = getInputValue('hosting-packages-name');
            if (!name) {
                showError('Name is required');
                return;
            }
            const diskSpaceMB = parseNonNegativeInt('hosting-packages-disk-space');
            const bandwidthMB = parseNonNegativeInt('hosting-packages-bandwidth');
            const emailAccounts = parseNonNegativeInt('hosting-packages-email-accounts');
            const databases = parseNonNegativeInt('hosting-packages-databases');
            const domains = parseNonNegativeInt('hosting-packages-domains');
            const subdomains = parseNonNegativeInt('hosting-packages-subdomains');
            const ftpAccounts = parseNonNegativeInt('hosting-packages-ftp-accounts');
            const monthlyPrice = parseNonNegativeDecimal('hosting-packages-monthly-price');
            const yearlyPrice = parseNonNegativeDecimal('hosting-packages-yearly-price');
            if (diskSpaceMB === null || bandwidthMB === null || emailAccounts === null || databases === null ||
                domains === null || subdomains === null || ftpAccounts === null || monthlyPrice === null || yearlyPrice === null) {
                showError('Numeric values must be 0 or greater');
                return;
            }
            const payload = {
                name,
                description: getInputValue('hosting-packages-description') || null,
                diskSpaceMB,
                bandwidthMB,
                emailAccounts,
                databases,
                domains,
                subdomains,
                ftpAccounts,
                sslSupport: getCheckbox('hosting-packages-ssl-support'),
                backupSupport: getCheckbox('hosting-packages-backup-support'),
                dedicatedIp: getCheckbox('hosting-packages-dedicated-ip'),
                monthlyPrice,
                yearlyPrice,
                isActive: getCheckbox('hosting-packages-is-active'),
            };
            const response = editingId
                ? await apiRequest(`${getApiBaseUrl()}/HostingPackages/${editingId}`, { method: 'PUT', body: JSON.stringify(payload) })
                : await apiRequest(`${getApiBaseUrl()}/HostingPackages`, { method: 'POST', body: JSON.stringify(payload) });
            if (!response.success) {
                showError(response.message || 'Save failed');
                return;
            }
            hideModal('hosting-packages-edit-modal');
            showSuccess(editingId ? 'Hosting plan updated successfully' : 'Hosting plan created successfully');
            await loadHostingPackages();
        }
        finally {
            isSaving = false;
            if (saveButton) {
                saveButton.disabled = false;
            }
        }
    };
    const openDelete = (id, name) => {
        pendingDeleteId = id;
        const label = document.getElementById('hosting-packages-delete-name');
        if (label) {
            label.textContent = name;
        }
        showModal('hosting-packages-delete-modal');
    };
    const confirmDelete = async () => {
        if (!pendingDeleteId) {
            return;
        }
        const response = await apiRequest(`${getApiBaseUrl()}/HostingPackages/${pendingDeleteId}`, { method: 'DELETE' });
        hideModal('hosting-packages-delete-modal');
        if (!response.success) {
            showError(response.message || 'Delete failed');
            return;
        }
        showSuccess('Hosting plan deleted successfully');
        pendingDeleteId = null;
        await loadHostingPackages();
    };
    const bindTableActions = () => {
        const tableBody = document.getElementById('hosting-packages-table-body');
        if (!tableBody) {
            return;
        }
        tableBody.addEventListener('click', (event) => {
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
                openDelete(id, button.dataset.name ?? '');
            }
        });
    };
    const initializePage = () => {
        const page = document.getElementById('hosting-packages-page');
        if (!page || page.dataset.initialized === 'true') {
            return;
        }
        page.dataset.initialized = 'true';
        document.getElementById('hosting-packages-create')?.addEventListener('click', openCreate);
        document.getElementById('hosting-packages-save')?.addEventListener('click', () => { void saveHostingPackage(); });
        document.getElementById('hosting-packages-confirm-delete')?.addEventListener('click', () => { void confirmDelete(); });
        bindTableActions();
        void loadHostingPackages();
    };
    const setupObserver = () => {
        initializePage();
        if (document.body) {
            const observer = new MutationObserver(() => {
                const page = document.getElementById('hosting-packages-page');
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
//# sourceMappingURL=hosting-packages.js.map
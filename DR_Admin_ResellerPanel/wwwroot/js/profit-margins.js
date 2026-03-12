"use strict";
(() => {
    const classLabels = {
        1: 'TLD',
        2: 'Hosting',
        3: 'Additional Service',
        4: 'Domain Service',
        99: 'Other',
    };
    let rows = [];
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
                const parsed = (body ?? {});
                return {
                    success: false,
                    message: parsed.message ?? parsed.title ?? `Request failed with status ${response.status}`,
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
    const showSuccess = (message) => {
        const success = document.getElementById('profit-margins-alert-success');
        const error = document.getElementById('profit-margins-alert-error');
        if (success) {
            success.textContent = message;
            success.classList.remove('d-none');
        }
        error?.classList.add('d-none');
    };
    const showError = (message) => {
        const success = document.getElementById('profit-margins-alert-success');
        const error = document.getElementById('profit-margins-alert-error');
        if (error) {
            error.textContent = message;
            error.classList.remove('d-none');
        }
        success?.classList.add('d-none');
    };
    const clearAlerts = () => {
        document.getElementById('profit-margins-alert-success')?.classList.add('d-none');
        document.getElementById('profit-margins-alert-error')?.classList.add('d-none');
    };
    const parseRow = (item) => {
        const row = (item ?? {});
        return {
            id: Number(row.id ?? row.Id ?? 0),
            productClass: Number(row.productClass ?? row.ProductClass ?? 0),
            profitPercent: Number(row.profitPercent ?? row.ProfitPercent ?? 0),
            isActive: Boolean(row.isActive ?? row.IsActive ?? false),
            notes: String(row.notes ?? row.Notes ?? '') || null,
            updatedAt: String(row.updatedAt ?? row.UpdatedAt ?? ''),
        };
    };
    const setForm = (row) => {
        const id = document.getElementById('profit-margins-current-id');
        const productClass = document.getElementById('profit-margins-product-class');
        const profitPercent = document.getElementById('profit-margins-profit-percent');
        const notes = document.getElementById('profit-margins-notes');
        const isActive = document.getElementById('profit-margins-is-active');
        if (!row) {
            if (id)
                id.value = '';
            if (productClass)
                productClass.value = '1';
            if (profitPercent)
                profitPercent.value = '20';
            if (notes)
                notes.value = '';
            if (isActive)
                isActive.checked = true;
            return;
        }
        if (id)
            id.value = String(row.id);
        if (productClass)
            productClass.value = String(row.productClass);
        if (profitPercent)
            profitPercent.value = row.profitPercent.toFixed(2);
        if (notes)
            notes.value = row.notes ?? '';
        if (isActive)
            isActive.checked = row.isActive;
    };
    const renderTable = () => {
        const body = document.getElementById('profit-margins-table-body');
        if (!body) {
            return;
        }
        if (!rows.length) {
            body.innerHTML = '<tr><td colspan="5" class="text-center text-muted">No profit margin settings found.</td></tr>';
            return;
        }
        body.innerHTML = rows
            .slice()
            .sort((a, b) => a.productClass - b.productClass)
            .map((row) => {
            const updated = row.updatedAt ? new Date(row.updatedAt).toLocaleString() : '-';
            return `
                    <tr>
                        <td>${esc(classLabels[row.productClass] ?? String(row.productClass))}</td>
                        <td>${row.profitPercent.toFixed(2)}%</td>
                        <td>${row.isActive ? '<span class="badge bg-success">Yes</span>' : '<span class="badge bg-secondary">No</span>'}</td>
                        <td>${esc(updated)}</td>
                        <td class="text-end">
                            <div class="btn-group btn-group-sm">
                                <button class="btn btn-outline-primary" type="button" data-action="edit" data-id="${row.id}"><i class="bi bi-pencil"></i></button>
                                <button class="btn btn-outline-danger" type="button" data-action="delete" data-id="${row.id}"><i class="bi bi-trash"></i></button>
                            </div>
                        </td>
                    </tr>
                `;
        })
            .join('');
    };
    const loadRows = async () => {
        const body = document.getElementById('profit-margins-table-body');
        if (body) {
            body.innerHTML = '<tr><td colspan="5" class="text-center"><div class="spinner-border text-primary"></div></td></tr>';
        }
        const response = await apiRequest(`${getApiBaseUrl()}/ProfitMarginSettings`, { method: 'GET' });
        if (!response.success) {
            showError(response.message || 'Failed to load profit margin settings.');
            if (body) {
                body.innerHTML = '<tr><td colspan="5" class="text-center text-danger">Failed to load data</td></tr>';
            }
            return;
        }
        const list = Array.isArray(response.data)
            ? response.data
            : Array.isArray(response.data?.data)
                ? (response.data.data ?? [])
                : [];
        rows = list.map((x) => parseRow(x));
        renderTable();
    };
    const save = async () => {
        clearAlerts();
        const id = Number(document.getElementById('profit-margins-current-id')?.value ?? '0');
        const productClass = Number(document.getElementById('profit-margins-product-class')?.value ?? '0');
        const profitPercent = Number(document.getElementById('profit-margins-profit-percent')?.value ?? '0');
        const notes = document.getElementById('profit-margins-notes')?.value.trim() ?? '';
        const isActive = document.getElementById('profit-margins-is-active')?.checked ?? true;
        if (!Number.isFinite(productClass) || productClass <= 0) {
            showError('Select a valid product class.');
            return;
        }
        if (!Number.isFinite(profitPercent) || profitPercent < 0) {
            showError('Profit % must be 0 or greater.');
            return;
        }
        const existingForClass = rows.find((x) => x.productClass === productClass);
        const updateId = id > 0 ? id : existingForClass?.id ?? 0;
        const updatePayload = {
            profitPercent,
            isActive,
            notes,
        };
        const createPayload = {
            productClass,
            profitPercent,
            isActive,
            notes,
        };
        const response = updateId > 0
            ? await apiRequest(`${getApiBaseUrl()}/ProfitMarginSettings/${updateId}`, {
                method: 'PUT',
                body: JSON.stringify(updatePayload),
            })
            : await apiRequest(`${getApiBaseUrl()}/ProfitMarginSettings`, {
                method: 'POST',
                body: JSON.stringify(createPayload),
            });
        if (!response.success) {
            showError(response.message || 'Failed to save profit margin setting.');
            return;
        }
        showSuccess('Profit margin setting saved.');
        setForm(null);
        await loadRows();
    };
    const remove = async (id) => {
        const response = await apiRequest(`${getApiBaseUrl()}/ProfitMarginSettings/${id}`, {
            method: 'DELETE',
        });
        if (!response.success) {
            showError(response.message || 'Failed to delete profit margin setting.');
            return;
        }
        showSuccess('Profit margin setting deleted.');
        setForm(null);
        await loadRows();
    };
    const bindEvents = () => {
        document.getElementById('profit-margins-save')?.addEventListener('click', () => { void save(); });
        document.getElementById('profit-margins-reset')?.addEventListener('click', () => { setForm(null); });
        document.getElementById('profit-margins-refresh')?.addEventListener('click', () => { void loadRows(); });
        document.getElementById('profit-margins-table-body')?.addEventListener('click', (event) => {
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
                const row = rows.find((x) => x.id === id) ?? null;
                setForm(row);
                return;
            }
            if (button.dataset.action === 'delete') {
                void remove(id);
            }
        });
    };
    const initializePage = () => {
        const page = document.getElementById('profit-margins-page');
        if (!page || page.dataset.initialized === 'true') {
            return;
        }
        page.dataset.initialized = 'true';
        bindEvents();
        setForm(null);
        void loadRows();
    };
    const setupObserver = () => {
        initializePage();
        if (document.body) {
            const observer = new MutationObserver(() => {
                const page = document.getElementById('profit-margins-page');
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
//# sourceMappingURL=profit-margins.js.map
(() => {
    interface AppSettings {
        apiBaseUrl?: string;
    }

    interface ApiResponse<T> {
        success: boolean;
        data?: T;
        message?: string;
    }

    interface ProfitMarginSetting {
        id: number;
        productClass: number;
        profitPercent: number;
        isActive: boolean;
        notes: string | null;
        updatedAt: string;
    }

    const classLabels: Record<number, string> = {
        1: 'TLD',
        2: 'Hosting',
        3: 'Additional Service',
        4: 'Domain Service',
        99: 'Other',
    };

    let rows: ProfitMarginSetting[] = [];

    const getApiBaseUrl = (): string => {
        const settings = (window as Window & { AppSettings?: AppSettings }).AppSettings;
        return settings?.apiBaseUrl ?? '';
    };

    const getAuthToken = (): string | null => {
        const auth = (window as Window & { Auth?: { getToken?: () => string | null } }).Auth;
        if (auth?.getToken) {
            return auth.getToken();
        }

        return sessionStorage.getItem('rp_authToken');
    };

    const esc = (text: string): string => {
        const map: Record<string, string> = { '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#039;' };
        return (text || '').replace(/[&<>"']/g, (char) => map[char] ?? char);
    };

    const apiRequest = async <T,>(endpoint: string, options: RequestInit = {}): Promise<ApiResponse<T>> => {
        try {
            const headers: Record<string, string> = {
                'Content-Type': 'application/json',
                ...(options.headers as Record<string, string> | undefined),
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
            const body = hasJson ? (await response.json() as unknown) : null;

            if (!response.ok) {
                const parsed = (body ?? {}) as { message?: string; title?: string };
                return {
                    success: false,
                    message: parsed.message ?? parsed.title ?? `Request failed with status ${response.status}`,
                };
            }

            const envelope = (body ?? {}) as { success?: boolean; data?: T; message?: string };
            return {
                success: envelope.success !== false,
                data: envelope.data ?? (body as T),
                message: envelope.message,
            };
        } catch {
            return {
                success: false,
                message: 'Network error. Please try again.',
            };
        }
    };

    const showSuccess = (message: string): void => {
        const success = document.getElementById('profit-margins-alert-success');
        const error = document.getElementById('profit-margins-alert-error');
        if (success) {
            success.textContent = message;
            success.classList.remove('d-none');
        }

        error?.classList.add('d-none');
    };

    const showError = (message: string): void => {
        const success = document.getElementById('profit-margins-alert-success');
        const error = document.getElementById('profit-margins-alert-error');
        if (error) {
            error.textContent = message;
            error.classList.remove('d-none');
        }

        success?.classList.add('d-none');
    };

    const clearAlerts = (): void => {
        document.getElementById('profit-margins-alert-success')?.classList.add('d-none');
        document.getElementById('profit-margins-alert-error')?.classList.add('d-none');
    };

    const parseRow = (item: unknown): ProfitMarginSetting => {
        const row = (item ?? {}) as Record<string, unknown>;
        return {
            id: Number(row.id ?? row.Id ?? 0),
            productClass: Number(row.productClass ?? row.ProductClass ?? 0),
            profitPercent: Number(row.profitPercent ?? row.ProfitPercent ?? 0),
            isActive: Boolean(row.isActive ?? row.IsActive ?? false),
            notes: String(row.notes ?? row.Notes ?? '') || null,
            updatedAt: String(row.updatedAt ?? row.UpdatedAt ?? ''),
        };
    };

    const setForm = (row: ProfitMarginSetting | null): void => {
        const id = document.getElementById('profit-margins-current-id') as HTMLInputElement | null;
        const productClass = document.getElementById('profit-margins-product-class') as HTMLSelectElement | null;
        const profitPercent = document.getElementById('profit-margins-profit-percent') as HTMLInputElement | null;
        const notes = document.getElementById('profit-margins-notes') as HTMLInputElement | null;
        const isActive = document.getElementById('profit-margins-is-active') as HTMLInputElement | null;

        if (!row) {
            if (id) id.value = '';
            if (productClass) productClass.value = '1';
            if (profitPercent) profitPercent.value = '20';
            if (notes) notes.value = '';
            if (isActive) isActive.checked = true;
            return;
        }

        if (id) id.value = String(row.id);
        if (productClass) productClass.value = String(row.productClass);
        if (profitPercent) profitPercent.value = row.profitPercent.toFixed(2);
        if (notes) notes.value = row.notes ?? '';
        if (isActive) isActive.checked = row.isActive;
    };

    const renderTable = (): void => {
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

    const loadRows = async (): Promise<void> => {
        const body = document.getElementById('profit-margins-table-body');
        if (body) {
            body.innerHTML = '<tr><td colspan="5" class="text-center"><div class="spinner-border text-primary"></div></td></tr>';
        }

        const response = await apiRequest<unknown>(`${getApiBaseUrl()}/ProfitMarginSettings`, { method: 'GET' });
        if (!response.success) {
            showError(response.message || 'Failed to load profit margin settings.');
            if (body) {
                body.innerHTML = '<tr><td colspan="5" class="text-center text-danger">Failed to load data</td></tr>';
            }
            return;
        }

        const list = Array.isArray(response.data)
            ? response.data
            : Array.isArray((response.data as { data?: unknown[] } | null)?.data)
                ? ((response.data as { data?: unknown[] }).data ?? [])
                : [];

        rows = list.map((x) => parseRow(x));
        renderTable();
    };

    const save = async (): Promise<void> => {
        clearAlerts();

        const id = Number((document.getElementById('profit-margins-current-id') as HTMLInputElement | null)?.value ?? '0');
        const productClass = Number((document.getElementById('profit-margins-product-class') as HTMLSelectElement | null)?.value ?? '0');
        const profitPercent = Number((document.getElementById('profit-margins-profit-percent') as HTMLInputElement | null)?.value ?? '0');
        const notes = (document.getElementById('profit-margins-notes') as HTMLInputElement | null)?.value.trim() ?? '';
        const isActive = (document.getElementById('profit-margins-is-active') as HTMLInputElement | null)?.checked ?? true;

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
            ? await apiRequest<unknown>(`${getApiBaseUrl()}/ProfitMarginSettings/${updateId}`, {
                method: 'PUT',
                body: JSON.stringify(updatePayload),
            })
            : await apiRequest<unknown>(`${getApiBaseUrl()}/ProfitMarginSettings`, {
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

    const remove = async (id: number): Promise<void> => {
        const response = await apiRequest<unknown>(`${getApiBaseUrl()}/ProfitMarginSettings/${id}`, {
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

    const bindEvents = (): void => {
        document.getElementById('profit-margins-save')?.addEventListener('click', () => { void save(); });
        document.getElementById('profit-margins-reset')?.addEventListener('click', () => { setForm(null); });
        document.getElementById('profit-margins-refresh')?.addEventListener('click', () => { void loadRows(); });

        document.getElementById('profit-margins-table-body')?.addEventListener('click', (event) => {
            const target = event.target as HTMLElement;
            const button = target.closest<HTMLButtonElement>('button[data-action]');
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

    const initializePage = (): void => {
        const page = document.getElementById('profit-margins-page') as HTMLElement | null;
        if (!page || page.dataset.initialized === 'true') {
            return;
        }

        page.dataset.initialized = 'true';

        bindEvents();
        setForm(null);
        void loadRows();
    };

    const setupObserver = (): void => {
        initializePage();

        if (document.body) {
            const observer = new MutationObserver(() => {
                const page = document.getElementById('profit-margins-page') as HTMLElement | null;
                if (page && page.dataset.initialized !== 'true') {
                    initializePage();
                }
            });

            observer.observe(document.body, { childList: true, subtree: true });
        }
    };

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', setupObserver);
    } else {
        setupObserver();
    }
})();

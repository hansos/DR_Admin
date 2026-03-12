(() => {
    interface AppSettings {
        apiBaseUrl?: string;
    }

    interface ApiResponse<T> {
        success: boolean;
        data?: T;
        message?: string;
    }

    interface ApiEnvelope<T> {
        success?: boolean;
        data?: T;
        message?: string;
    }

    interface ApiErrorBody {
        message?: string;
        title?: string;
    }

    interface TldItem {
        id: number;
        extension: string;
        isActive: boolean;
    }

    interface TldSalesPricing {
        id: number;
        tldId: number;
        effectiveFrom: string;
        effectiveTo: string | null;
        registrationPrice: number;
        renewalPrice: number;
        transferPrice: number;
        currency: string;
        isActive: boolean;
    }

    interface PricingListRow {
        tldId: number;
        extension: string;
        registrationPrice: number | null;
        renewalPrice: number | null;
        transferPrice: number | null;
        currency: string;
        effectiveFrom: string | null;
        effectiveTo: string | null;
        isActive: boolean;
    }

    interface CalculatePricingResponse {
        tldExtension?: string;
        basePrice: number;
        discountAmount: number;
        finalPrice: number;
        currency: string;
        isPromotionalPricing: boolean;
        promotionName?: string | null;
    }

    interface MarginAnalysisResult {
        registrarName?: string | null;
        cost: number;
        costCurrency: string;
    }

    interface CurrencyExchangeRate {
        rate?: number;
        effectiveRate?: number;
    }

    interface ProfitMarginSettingResponse {
        profitPercent?: number;
        ProfitPercent?: number;
    }

    interface CurrencyItem {
        id: number;
        code: string;
        name: string;
        isActive: boolean;
        isDefault: boolean;
        sortOrder: number;
    }

    interface ResellerItem {
        id: number;
        name: string;
        isActive: boolean;
        isDefault: boolean;
    }

    interface CalculatorTransferState {
        tldId: number;
        operationType: string;
        recommendedPrice: number;
        currency: string;
    }

    let allTlds: TldItem[] = [];
    let allRows: PricingListRow[] = [];
    let filteredRows: PricingListRow[] = [];
    let allCurrencies: CurrencyItem[] = [];
    let allResellers: ResellerItem[] = [];
    let calculatorTransferState: CalculatorTransferState | null = null;

    let currentPage = 1;
    let pageSize = 25;
    let totalCount = 0;
    let totalPages = 1;

    const getApiBaseUrl = (): string => {
        const settings = (window as Window & { AppSettings?: AppSettings }).AppSettings;
        return settings?.apiBaseUrl ?? '';
    };

    const convertAmount = async (amount: number, fromCurrency: string, toCurrency: string): Promise<number | null> => {
        const from = fromCurrency.trim().toUpperCase();
        const to = toCurrency.trim().toUpperCase();

        if (!from || !to) {
            return null;
        }

        if (from === to) {
            return amount;
        }

        const response = await apiRequest<CurrencyExchangeRate>(`${getApiBaseUrl()}/Currencies/rates/exchange?from=${encodeURIComponent(from)}&to=${encodeURIComponent(to)}`, {
            method: 'GET',
        });

        if (response.success && response.data) {
            const rate = parseNumber(response.data.effectiveRate ?? response.data.rate);
            if (Number.isFinite(rate) && rate > 0) {
                return amount * rate;
            }
        }

        // Fallback via EUR as base currency in case direct pair is unavailable.
        if (from !== 'EUR' && to !== 'EUR') {
            const fromToEurResponse = await apiRequest<CurrencyExchangeRate>(`${getApiBaseUrl()}/Currencies/rates/exchange?from=${encodeURIComponent(from)}&to=EUR`, {
                method: 'GET',
            });

            const eurToToResponse = await apiRequest<CurrencyExchangeRate>(`${getApiBaseUrl()}/Currencies/rates/exchange?from=EUR&to=${encodeURIComponent(to)}`, {
                method: 'GET',
            });

            if (fromToEurResponse.success && fromToEurResponse.data && eurToToResponse.success && eurToToResponse.data) {
                const toEurRate = parseNumber(fromToEurResponse.data.effectiveRate ?? fromToEurResponse.data.rate);
                const fromEurRate = parseNumber(eurToToResponse.data.effectiveRate ?? eurToToResponse.data.rate);

                if (Number.isFinite(toEurRate) && toEurRate > 0 && Number.isFinite(fromEurRate) && fromEurRate > 0) {
                    return amount * toEurRate * fromEurRate;
                }
            }
        }

        return null;
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

    const parseString = (value: unknown): string => typeof value === 'string' ? value : '';

    const parseNumber = (value: unknown): number => {
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

    const roundToSingleDecimalStep = (value: number): number => {
        return Math.round((value + Number.EPSILON) * 10) / 10;
    };

    const getOptionalNumber = (id: string): number | null => {
        const input = document.getElementById(id) as HTMLInputElement | null;
        const raw = (input?.value ?? '').trim();
        if (!raw) {
            return null;
        }

        const parsed = Number(raw);
        return Number.isFinite(parsed) ? parsed : null;
    };

    const formatDate = (value: string | null): string => {
        if (!value) {
            return '-';
        }

        const date = new Date(value);
        if (Number.isNaN(date.getTime())) {
            return value;
        }

        return date.toLocaleString();
    };

    const formatDateTimeLocal = (value: string | null): string => {
        if (!value) {
            return '';
        }

        const parsed = new Date(value);
        if (Number.isNaN(parsed.getTime())) {
            return '';
        }

        const offsetMs = parsed.getTimezoneOffset() * 60000;
        const local = new Date(parsed.getTime() - offsetMs);
        return local.toISOString().slice(0, 16);
    };

    const parseDateTimeLocalAsIso = (id: string): string | null => {
        const input = document.getElementById(id) as HTMLInputElement | null;
        const raw = (input?.value ?? '').trim();
        if (!raw) {
            return null;
        }

        const parsed = new Date(raw);
        return Number.isNaN(parsed.getTime()) ? null : parsed.toISOString();
    };

    const formatMoney = (amount: number | null, currency: string): string => {
        if (amount === null || !Number.isFinite(amount)) {
            return '-';
        }

        try {
            return new Intl.NumberFormat(undefined, {
                style: 'currency',
                currency: currency || 'USD',
                minimumFractionDigits: 2,
                maximumFractionDigits: 2,
            }).format(amount);
        } catch {
            return `${amount.toFixed(2)} ${currency || 'USD'}`;
        }
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
                const errorBody = (body ?? {}) as ApiErrorBody;
                return {
                    success: false,
                    message: errorBody.message ?? errorBody.title ?? `Request failed with status ${response.status}`,
                };
            }

            const envelope = (body ?? {}) as ApiEnvelope<T>;
            return {
                success: envelope.success !== false,
                data: envelope.data ?? (body as T),
                message: envelope.message,
            };
        } catch (error) {
            console.error('Pricing request failed', error);
            return {
                success: false,
                message: 'Network error. Please try again.',
            };
        }
    };

    const extractList = (payload: unknown): unknown[] => {
        if (Array.isArray(payload)) {
            return payload;
        }

        const objectPayload = payload as { data?: unknown; Data?: unknown; items?: unknown; Items?: unknown } | null;
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

    const normalizeTld = (item: unknown): TldItem => {
        const row = (item ?? {}) as Record<string, unknown>;
        return {
            id: parseNumber(row.id ?? row.Id),
            extension: parseString(row.extension ?? row.Extension),
            isActive: Boolean(row.isActive ?? row.IsActive ?? false),
        };
    };

    const normalizeCurrency = (item: unknown): CurrencyItem => {
        const row = (item ?? {}) as Record<string, unknown>;
        return {
            id: parseNumber(row.id ?? row.Id),
            code: parseString(row.code ?? row.Code).toUpperCase(),
            name: parseString(row.name ?? row.Name),
            isActive: Boolean(row.isActive ?? row.IsActive ?? false),
            isDefault: Boolean(row.isDefault ?? row.IsDefault ?? false),
            sortOrder: parseNumber(row.sortOrder ?? row.SortOrder),
        };
    };

    const normalizeReseller = (item: unknown): ResellerItem => {
        const row = (item ?? {}) as Record<string, unknown>;
        return {
            id: parseNumber(row.id ?? row.Id),
            name: parseString(row.name ?? row.Name),
            isActive: Boolean(row.isActive ?? row.IsActive ?? false),
            isDefault: Boolean(row.isDefault ?? row.IsDefault ?? false),
        };
    };

    const normalizeSalesPricing = (item: unknown): TldSalesPricing => {
        const row = (item ?? {}) as Record<string, unknown>;
        const effectiveToRaw = parseString(row.effectiveTo ?? row.EffectiveTo);
        return {
            id: parseNumber(row.id ?? row.Id),
            tldId: parseNumber(row.tldId ?? row.TldId),
            effectiveFrom: parseString(row.effectiveFrom ?? row.EffectiveFrom),
            effectiveTo: effectiveToRaw || null,
            registrationPrice: parseNumber(row.registrationPrice ?? row.RegistrationPrice),
            renewalPrice: parseNumber(row.renewalPrice ?? row.RenewalPrice),
            transferPrice: parseNumber(row.transferPrice ?? row.TransferPrice),
            currency: parseString(row.currency ?? row.Currency) || 'USD',
            isActive: Boolean(row.isActive ?? row.IsActive ?? false),
        };
    };

    const pickCurrentPricing = (items: TldSalesPricing[]): TldSalesPricing | null => {
        if (!items.length) {
            return null;
        }

        const now = Date.now();
        const current = items
            .filter((item) => {
                const from = Date.parse(item.effectiveFrom);
                if (!Number.isFinite(from) || from > now) {
                    return false;
                }

                if (!item.effectiveTo) {
                    return true;
                }

                const to = Date.parse(item.effectiveTo);
                return !Number.isFinite(to) || to > now;
            })
            .sort((a, b) => Date.parse(b.effectiveFrom) - Date.parse(a.effectiveFrom));

        if (current.length > 0) {
            return current[0];
        }

        return items.slice().sort((a, b) => Date.parse(b.effectiveFrom) - Date.parse(a.effectiveFrom))[0] ?? null;
    };

    const showSuccess = (message: string): void => {
        const alert = document.getElementById('pricing-alert-success');
        if (!alert) {
            return;
        }

        alert.textContent = message;
        alert.classList.remove('d-none');
        document.getElementById('pricing-alert-error')?.classList.add('d-none');
    };

    const showError = (message: string): void => {
        const alert = document.getElementById('pricing-alert-error');
        if (!alert) {
            return;
        }

        alert.textContent = message;
        alert.classList.remove('d-none');
        document.getElementById('pricing-alert-success')?.classList.add('d-none');
    };

    const hideAlerts = (): void => {
        document.getElementById('pricing-alert-success')?.classList.add('d-none');
        document.getElementById('pricing-alert-error')?.classList.add('d-none');
    };

    const renderTldSelectOptions = (): void => {
        const options = allTlds
            .slice()
            .sort((a, b) => a.extension.localeCompare(b.extension))
            .map((tld) => `<option value="${tld.id}">.${esc(tld.extension)}</option>`)
            .join('');

        const calcSelect = document.getElementById('pricing-calc-tld') as HTMLSelectElement | null;
        const editSelect = document.getElementById('pricing-edit-tld') as HTMLSelectElement | null;

        if (calcSelect) {
            calcSelect.innerHTML = options;
        }

        if (editSelect) {
            editSelect.innerHTML = options;
        }
    };

    const ensureSelectHasValue = (select: HTMLSelectElement, value: string): void => {
        if (!value) {
            select.value = '';
            return;
        }

        const exists = Array.from(select.options).some((option) => option.value === value);
        if (!exists) {
            const option = document.createElement('option');
            option.value = value;
            option.textContent = value;
            select.append(option);
        }

        select.value = value;
    };

    const renderCurrencySelectOptions = (): void => {
        const sorted = [...allCurrencies]
            .filter((currency) => currency.isActive && currency.code)
            .sort((a, b) => a.sortOrder - b.sortOrder || a.code.localeCompare(b.code));

        const calcSelect = document.getElementById('pricing-calc-currency') as HTMLSelectElement | null;
        const editSelect = document.getElementById('pricing-edit-currency') as HTMLSelectElement | null;

        if (calcSelect) {
            const previousValue = calcSelect.value.trim().toUpperCase();
            calcSelect.innerHTML = '<option value="">Use pricing currency</option>' + sorted
                .map((currency) => `<option value="${esc(currency.code)}">${esc(currency.code)} - ${esc(currency.name || currency.code)}</option>`)
                .join('');

            const defaultCurrency = sorted.find((currency) => currency.isDefault)?.code ?? sorted[0]?.code ?? '';
            const preferredCurrency = previousValue || defaultCurrency;
            if (preferredCurrency) {
                ensureSelectHasValue(calcSelect, preferredCurrency);
            }
        }

        if (editSelect) {
            editSelect.innerHTML = sorted
                .map((currency) => `<option value="${esc(currency.code)}">${esc(currency.code)} - ${esc(currency.name || currency.code)}</option>`)
                .join('');

            if (editSelect.options.length === 0) {
                editSelect.innerHTML = '<option value="USD">USD</option>';
            }

            const defaultCurrency = sorted.find((currency) => currency.isDefault)?.code ?? sorted[0]?.code ?? 'USD';
            ensureSelectHasValue(editSelect, defaultCurrency);
        }
    };

    const renderResellerSelectOptions = (): void => {
        const sorted = [...allResellers]
            .filter((reseller) => reseller.isActive && reseller.id > 0)
            .sort((a, b) => a.name.localeCompare(b.name));

        const select = document.getElementById('pricing-calc-reseller-id') as HTMLSelectElement | null;
        if (!select) {
            return;
        }

        select.innerHTML = '<option value="">None</option>' + sorted
            .map((reseller) => `<option value="${reseller.id}">#${reseller.id} - ${esc(reseller.name)}</option>`)
            .join('');

        const defaultReseller = sorted.find((reseller) => reseller.isDefault);
        if (defaultReseller) {
            select.value = String(defaultReseller.id);
        }
    };

    const loadReferenceData = async (): Promise<void> => {
        const [currenciesResponse, resellersResponse] = await Promise.all([
            apiRequest<unknown>(`${getApiBaseUrl()}/Currencies`, { method: 'GET' }),
            apiRequest<unknown>(`${getApiBaseUrl()}/ResellerCompanies/active`, { method: 'GET' }),
        ]);

        allCurrencies = currenciesResponse.success
            ? extractList(currenciesResponse.data).map((item) => normalizeCurrency(item)).filter((item) => item.id > 0 && item.code)
            : [];

        if (allCurrencies.length === 0) {
            allCurrencies = [{ id: 0, code: 'USD', name: 'US Dollar', isActive: true, isDefault: true, sortOrder: 0 }];
        }

        allResellers = resellersResponse.success
            ? extractList(resellersResponse.data).map((item) => normalizeReseller(item)).filter((item) => item.id > 0)
            : [];

        renderCurrencySelectOptions();
        renderResellerSelectOptions();
    };

    const loadDefaultProfitPercentForTld = async (): Promise<void> => {
        const profitInput = document.getElementById('pricing-calc-profit-percent') as HTMLInputElement | null;
        if (!profitInput) {
            return;
        }

        const response = await apiRequest<ProfitMarginSettingResponse>(`${getApiBaseUrl()}/ProfitMarginSettings/by-class/Tld`, { method: 'GET' });
        if (!response.success || !response.data) {
            return;
        }

        const value = parseNumber(response.data.profitPercent ?? response.data.ProfitPercent);
        if (Number.isFinite(value) && value >= 0) {
            profitInput.value = value.toFixed(2);
        }
    };

    const loadCurrentPricingForTld = (tldId: number): PricingListRow | null => {
        return allRows.find((x) => x.tldId === tldId) ?? null;
    };

    const setEditFieldsFromRow = (row: PricingListRow | null): void => {
        const registration = document.getElementById('pricing-edit-registration') as HTMLInputElement | null;
        const renewal = document.getElementById('pricing-edit-renewal') as HTMLInputElement | null;
        const transfer = document.getElementById('pricing-edit-transfer') as HTMLInputElement | null;
        const currency = document.getElementById('pricing-edit-currency') as HTMLSelectElement | null;
        const validFrom = document.getElementById('pricing-edit-valid-from') as HTMLInputElement | null;
        const validTo = document.getElementById('pricing-edit-valid-to') as HTMLInputElement | null;

        if (registration) {
            registration.value = row?.registrationPrice !== null && row?.registrationPrice !== undefined ? String(row.registrationPrice) : '';
        }

        if (renewal) {
            renewal.value = row?.renewalPrice !== null && row?.renewalPrice !== undefined ? String(row.renewalPrice) : '';
        }

        if (transfer) {
            transfer.value = row?.transferPrice !== null && row?.transferPrice !== undefined ? String(row.transferPrice) : '';
        }

        if (currency) {
            const defaultCurrency = allCurrencies.find((item) => item.isDefault)?.code ?? 'USD';
            ensureSelectHasValue(currency, row?.currency || defaultCurrency);
        }

        if (validFrom) {
            validFrom.value = formatDateTimeLocal(row?.effectiveFrom ?? null);
        }

        if (validTo) {
            validTo.value = formatDateTimeLocal(row?.effectiveTo ?? null);
        }
    };

    const loadPageSizeFromUi = (): void => {
        const select = document.getElementById('pricing-page-size') as HTMLSelectElement | null;
        const parsed = Number(select?.value ?? '25');
        if (Number.isFinite(parsed) && parsed > 0) {
            pageSize = parsed;
        }
    };

    const getPagedRows = (): PricingListRow[] => {
        totalCount = filteredRows.length;
        totalPages = Math.max(1, Math.ceil(totalCount / pageSize));
        if (currentPage > totalPages) {
            currentPage = totalPages;
        }

        const start = (currentPage - 1) * pageSize;
        return filteredRows.slice(start, start + pageSize);
    };

    const renderTable = (): void => {
        const body = document.getElementById('pricing-table-body');
        if (!body) {
            return;
        }

        const paged = getPagedRows();
        if (!paged.length) {
            body.innerHTML = '<tr><td colspan="7" class="text-center text-muted">No pricing rows found.</td></tr>';
            return;
        }

        body.innerHTML = paged.map((row) => {
            return `
                <tr>
                    <td><code>.${esc(row.extension)}</code></td>
                    <td>${esc(formatMoney(row.registrationPrice, row.currency))}</td>
                    <td>${esc(formatMoney(row.renewalPrice, row.currency))}</td>
                    <td>${esc(formatMoney(row.transferPrice, row.currency))}</td>
                    <td>${esc(row.currency || '-')}</td>
                    <td>${esc(formatDate(row.effectiveFrom))}</td>
                    <td>${esc(formatDate(row.effectiveTo))}</td>
                </tr>
            `;
        }).join('');
    };

    const renderPagination = (): void => {
        const info = document.getElementById('pricing-pagination-info');
        const list = document.getElementById('pricing-paging-controls-list');

        if (!info || !list) {
            return;
        }

        if (!totalCount) {
            info.textContent = 'Showing 0 of 0';
            list.innerHTML = '';
            return;
        }

        const start = (currentPage - 1) * pageSize + 1;
        const end = Math.min(currentPage * pageSize, totalCount);
        info.textContent = `Showing ${start}-${end} of ${totalCount}`;

        if (totalPages <= 1) {
            list.innerHTML = '';
            return;
        }

        const makeItem = (label: string, page: number, disabled: boolean, active = false): string => {
            const cls = `page-item${disabled ? ' disabled' : ''}${active ? ' active' : ''}`;
            const ariaCurrent = active ? ' aria-current="page"' : '';
            const ariaDisabled = disabled ? ' aria-disabled="true" tabindex="-1"' : '';
            const dataPage = disabled ? '' : ` data-page="${page}"`;
            return `<li class="${cls}"><a class="page-link" href="#"${dataPage}${ariaCurrent}${ariaDisabled}>${label}</a></li>`;
        };

        const pages = new Set<number>();
        pages.add(1);
        if (totalPages >= 2) {
            pages.add(2);
            pages.add(totalPages - 1);
        }
        pages.add(totalPages);

        for (let page = currentPage - 1; page <= currentPage + 1; page += 1) {
            if (page >= 1 && page <= totalPages) {
                pages.add(page);
            }
        }

        const sortedPages = Array.from(pages)
            .filter((page) => page >= 1 && page <= totalPages)
            .sort((a, b) => a - b);

        let html = '';
        html += makeItem('Previous', currentPage - 1, currentPage <= 1);

        let lastPage = 0;
        for (const page of sortedPages) {
            if (lastPage > 0 && page - lastPage > 1) {
                html += '<li class="page-item disabled"><span class="page-link">…</span></li>';
            }

            html += makeItem(String(page), page, false, page === currentPage);
            lastPage = page;
        }

        html += makeItem('Next', currentPage + 1, currentPage >= totalPages);
        list.innerHTML = html;
    };

    const updateView = (): void => {
        loadPageSizeFromUi();
        renderTable();
        renderPagination();
    };

    const applyFilters = (): void => {
        const extensionFilter = (document.getElementById('pricing-filter-extension') as HTMLInputElement | null)?.value.trim().toLowerCase() ?? '';

        filteredRows = allRows.filter((row) => {
            if (extensionFilter && !row.extension.toLowerCase().includes(extensionFilter)) {
                return false;
            }
            return true;
        });

        currentPage = 1;
        updateView();
    };

    const resetFilters = (): void => {
        const extensionFilter = document.getElementById('pricing-filter-extension') as HTMLInputElement | null;
        if (extensionFilter) {
            extensionFilter.value = '';
        }

        filteredRows = [...allRows];
        currentPage = 1;
        updateView();
    };

    const loadPricingRows = async (): Promise<void> => {
        const body = document.getElementById('pricing-table-body');
        if (body) {
            body.innerHTML = '<tr><td colspan="7" class="text-center"><div class="spinner-border text-primary"></div></td></tr>';
        }

        hideAlerts();

        await loadReferenceData();

        const tldsResponse = await apiRequest<unknown>(`${getApiBaseUrl()}/Tlds/active`, { method: 'GET' });
        if (!tldsResponse.success) {
            showError(tldsResponse.message || 'Failed to load active TLDs.');
            if (body) {
                body.innerHTML = '<tr><td colspan="7" class="text-center text-danger">Failed to load data</td></tr>';
            }
            return;
        }

        allTlds = extractList(tldsResponse.data)
            .map((item) => normalizeTld(item))
            .filter((item) => item.id > 0 && item.isActive)
            .sort((a, b) => a.extension.localeCompare(b.extension));

        renderTldSelectOptions();

        const rows = await Promise.all(allTlds.map(async (tld) => {
            const pricingResponse = await apiRequest<unknown>(`${getApiBaseUrl()}/tld-pricing/sales/tld/${tld.id}?includeArchived=true`, { method: 'GET' });
            const history = pricingResponse.success
                ? extractList(pricingResponse.data).map((item) => normalizeSalesPricing(item)).filter((item) => item.tldId === tld.id)
                : [];
            const current = pickCurrentPricing(history);

            return {
                tldId: tld.id,
                extension: tld.extension,
                registrationPrice: current ? current.registrationPrice : null,
                renewalPrice: current ? current.renewalPrice : null,
                transferPrice: current ? current.transferPrice : null,
                currency: current?.currency || 'USD',
                effectiveFrom: current?.effectiveFrom ?? null,
                effectiveTo: current?.effectiveTo ?? null,
                isActive: current?.isActive ?? false,
            } as PricingListRow;
        }));

        allRows = rows;
        filteredRows = [...allRows];
        currentPage = 1;

        const editSelect = document.getElementById('pricing-edit-tld') as HTMLSelectElement | null;
        if (editSelect && editSelect.value) {
            const selectedTldId = Number(editSelect.value);
            if (Number.isFinite(selectedTldId) && selectedTldId > 0) {
                setEditFieldsFromRow(loadCurrentPricingForTld(selectedTldId));
            }
        }

        updateView();
    };

    const calculatePricing = async (): Promise<void> => {
        hideAlerts();

        const transferButton = document.getElementById('pricing-transfer-recommended') as HTMLButtonElement | null;
        if (transferButton) {
            transferButton.disabled = true;
        }
        calculatorTransferState = null;

        const tldId = Number((document.getElementById('pricing-calc-tld') as HTMLSelectElement | null)?.value ?? '0');
        const operationType = (document.getElementById('pricing-calc-operation') as HTMLSelectElement | null)?.value || 'Registration';
        const years = Number((document.getElementById('pricing-calc-years') as HTMLInputElement | null)?.value ?? '1');
        const targetCurrencyRaw = (document.getElementById('pricing-calc-currency') as HTMLSelectElement | null)?.value.trim().toUpperCase() ?? '';
        const resellerCompanyIdRaw = (document.getElementById('pricing-calc-reseller-id') as HTMLSelectElement | null)?.value.trim() ?? '';
        const profitPercent = Number((document.getElementById('pricing-calc-profit-percent') as HTMLInputElement | null)?.value ?? '20');
        const isFirstYear = (document.getElementById('pricing-calc-first-year') as HTMLInputElement | null)?.checked ?? true;

        if (!Number.isFinite(tldId) || tldId <= 0) {
            showError('Select a TLD for calculation.');
            return;
        }

        if (!Number.isFinite(years) || years <= 0) {
            showError('Years must be greater than 0.');
            return;
        }

        if (!Number.isFinite(profitPercent) || profitPercent < 0) {
            showError('Fortjeneste % must be 0 or greater.');
            return;
        }

        const resellerCompanyId = resellerCompanyIdRaw ? Number(resellerCompanyIdRaw) : null;

        const [response, marginResponse] = await Promise.all([
            apiRequest<CalculatePricingResponse>(`${getApiBaseUrl()}/tld-pricing/calculate`, {
                method: 'POST',
                body: JSON.stringify({
                    tldId,
                    resellerCompanyId,
                    operationType,
                    years,
                    isFirstYear,
                    targetCurrency: targetCurrencyRaw || null,
                }),
            }),
            apiRequest<MarginAnalysisResult>(`${getApiBaseUrl()}/tld-pricing/margin/tld/${tldId}?operationType=${encodeURIComponent(operationType)}`, {
                method: 'GET',
            }),
        ]);

        if (!response.success || !response.data) {
            showError(response.message || 'Failed to calculate pricing.');
            return;
        }

        const result = response.data;
        const resultCard = document.getElementById('pricing-calc-result');
        resultCard?.classList.remove('d-none');

        const tldText = result.tldExtension ? `.${result.tldExtension}` : '-';
        const currency = result.currency || 'USD';

        const registrarCost = marginResponse.success && marginResponse.data
            ? marginResponse.data.cost
            : 0;
        const registrarCurrency = marginResponse.success && marginResponse.data
            ? (marginResponse.data.costCurrency || currency)
            : currency;
        const registrarName = marginResponse.success && marginResponse.data
            ? (marginResponse.data.registrarName || '-')
            : '-';

        const costForPeriod = registrarCost * years;
        const recommendedFromCost = costForPeriod * (1 + (profitPercent / 100));
        const convertedRecommended = await convertAmount(recommendedFromCost, registrarCurrency, currency);
        const recommendedInResultCurrency = convertedRecommended ?? (registrarCurrency.toUpperCase() === currency.toUpperCase() ? recommendedFromCost : null);
        const recommendedFinal = recommendedInResultCurrency !== null
            ? Math.max(result.finalPrice, recommendedInResultCurrency)
            : result.finalPrice;

        const selectedTargetCurrency = targetCurrencyRaw || currency;
        const convertedRecommendedToTarget = await convertAmount(recommendedFromCost, registrarCurrency, selectedTargetCurrency);
        const recommendedInTargetCurrency = convertedRecommendedToTarget
            ?? (registrarCurrency.toUpperCase() === selectedTargetCurrency.toUpperCase() ? recommendedFromCost : null);
        const recommendedTargetFinal = recommendedInTargetCurrency !== null
            ? Math.max(result.finalPrice, recommendedInTargetCurrency)
            : null;

        const setText = (id: string, value: string): void => {
            const element = document.getElementById(id);
            if (element) {
                element.textContent = value;
            }
        };

        setText('pricing-calc-result-tld', tldText);
        setText('pricing-calc-result-currency', currency);
        setText('pricing-calc-result-base', formatMoney(result.basePrice, currency));
        setText('pricing-calc-result-discount', formatMoney(result.discountAmount, currency));
        setText('pricing-calc-result-final', formatMoney(result.finalPrice, currency));
        setText('pricing-calc-result-promotion', result.promotionName || (result.isPromotionalPricing ? 'Yes' : '-'));
        setText('pricing-calc-result-cost', formatMoney(costForPeriod, registrarCurrency));
        setText('pricing-calc-result-profit', `${profitPercent.toFixed(2)}%`);
        setText('pricing-calc-result-recommended', recommendedInResultCurrency !== null
            ? formatMoney(recommendedFinal, currency)
            : `${formatMoney(recommendedFromCost, registrarCurrency)} (${registrarCurrency})`);
        setText('pricing-calc-result-recommended-target', recommendedTargetFinal !== null
            ? formatMoney(recommendedTargetFinal, selectedTargetCurrency)
            : '-');
        setText('pricing-calc-result-registrar', registrarName);

        if (recommendedTargetFinal !== null) {
            calculatorTransferState = {
                tldId,
                operationType,
                recommendedPrice: recommendedTargetFinal,
                currency: selectedTargetCurrency,
            };

            if (transferButton) {
                transferButton.disabled = false;
            }
        }
    };

    const transferRecommendedToSalesPricing = (): void => {
        if (!calculatorTransferState) {
            showError('Run calculation before transferring recommended pricing.');
            return;
        }

        const editTldSelect = document.getElementById('pricing-edit-tld') as HTMLSelectElement | null;
        const editCurrencySelect = document.getElementById('pricing-edit-currency') as HTMLSelectElement | null;

        const priceFieldId = calculatorTransferState.operationType === 'Renewal'
            ? 'pricing-edit-renewal'
            : calculatorTransferState.operationType === 'Transfer'
                ? 'pricing-edit-transfer'
                : 'pricing-edit-registration';

        const priceInput = document.getElementById(priceFieldId) as HTMLInputElement | null;
        if (!editTldSelect || !editCurrencySelect || !priceInput) {
            showError('Unable to transfer recommended pricing to edit form.');
            return;
        }

        const editRegistrationInput = document.getElementById('pricing-edit-registration') as HTMLInputElement | null;
        const editRenewalInput = document.getElementById('pricing-edit-renewal') as HTMLInputElement | null;
        const editTransferInput = document.getElementById('pricing-edit-transfer') as HTMLInputElement | null;

        const existingValues: Record<string, string> = {
            'pricing-edit-registration': editRegistrationInput?.value ?? '',
            'pricing-edit-renewal': editRenewalInput?.value ?? '',
            'pricing-edit-transfer': editTransferInput?.value ?? '',
        };

        editTldSelect.value = String(calculatorTransferState.tldId);
        editTldSelect.dispatchEvent(new Event('change'));

        ensureSelectHasValue(editCurrencySelect, calculatorTransferState.currency);

        const rounded = roundToSingleDecimalStep(calculatorTransferState.recommendedPrice);
        priceInput.value = rounded.toFixed(2);

        const nonTargetFieldIds = ['pricing-edit-registration', 'pricing-edit-renewal', 'pricing-edit-transfer']
            .filter((id) => id !== priceFieldId);

        for (const fieldId of nonTargetFieldIds) {
            const field = document.getElementById(fieldId) as HTMLInputElement | null;
            if (field) {
                field.value = existingValues[fieldId] ?? '';
            }
        }

        showSuccess(`Transferred recommended ${calculatorTransferState.operationType.toLowerCase()} pricing to edit form.`);
    };

    const savePricing = async (): Promise<void> => {
        hideAlerts();

        const tldId = Number((document.getElementById('pricing-edit-tld') as HTMLSelectElement | null)?.value ?? '0');
        const registrationPrice = getOptionalNumber('pricing-edit-registration');
        const renewalPrice = getOptionalNumber('pricing-edit-renewal');
        const transferPrice = getOptionalNumber('pricing-edit-transfer');
        const currency = ((document.getElementById('pricing-edit-currency') as HTMLSelectElement | null)?.value ?? '').trim().toUpperCase() || 'USD';
        let effectiveFrom = parseDateTimeLocalAsIso('pricing-edit-valid-from');
        const effectiveTo = parseDateTimeLocalAsIso('pricing-edit-valid-to');

        if (!Number.isFinite(tldId) || tldId <= 0) {
            showError('Select a TLD before saving pricing.');
            return;
        }

        if (registrationPrice === null || renewalPrice === null || transferPrice === null) {
            showError('Registration, renewal and transfer prices are required.');
            return;
        }

        if (registrationPrice < 0 || renewalPrice < 0 || transferPrice < 0) {
            showError('Prices cannot be negative.');
            return;
        }

        if (effectiveTo && effectiveFrom) {
            const fromDate = new Date(effectiveFrom);
            const toDate = new Date(effectiveTo);
            if (!Number.isNaN(fromDate.getTime()) && !Number.isNaN(toDate.getTime()) && toDate <= fromDate) {
                showError('Valid to must be later than valid from.');
                return;
            }
        }

        const minimumEffectiveFrom = new Date(Date.now() + 60 * 1000);
        const currentEffectiveFrom = effectiveFrom ? new Date(effectiveFrom) : null;
        if (!currentEffectiveFrom || Number.isNaN(currentEffectiveFrom.getTime()) || currentEffectiveFrom <= new Date()) {
            effectiveFrom = minimumEffectiveFrom.toISOString();
        }

        const payload = {
            tldId,
            effectiveFrom: effectiveFrom,
            effectiveTo: effectiveTo,
            registrationPrice,
            renewalPrice,
            transferPrice,
            privacyPrice: null,
            firstYearRegistrationPrice: null,
            currency,
            isPromotional: false,
            promotionName: null,
            isActive: true,
            notes: 'Updated from reseller panel /billing/pricing',
        };

        const response = await apiRequest<unknown>(`${getApiBaseUrl()}/tld-pricing/sales`, {
            method: 'POST',
            body: JSON.stringify(payload),
        });

        if (!response.success) {
            showError(response.message || 'Failed to save pricing.');
            return;
        }

        showSuccess('Pricing saved.');
        await loadPricingRows();
    };

    const bindEvents = (): void => {
        document.getElementById('pricing-apply')?.addEventListener('click', applyFilters);
        document.getElementById('pricing-reset')?.addEventListener('click', resetFilters);
        document.getElementById('pricing-refresh')?.addEventListener('click', () => {
            void loadPricingRows();
        });
        document.getElementById('pricing-calc-run')?.addEventListener('click', () => {
            void calculatePricing();
        });
        document.getElementById('pricing-transfer-recommended')?.addEventListener('click', transferRecommendedToSalesPricing);
        document.getElementById('pricing-save')?.addEventListener('click', () => {
            void savePricing();
        });

        document.getElementById('pricing-page-size')?.addEventListener('change', () => {
            currentPage = 1;
            updateView();
        });

        document.getElementById('pricing-paging-controls')?.addEventListener('click', (event) => {
            const target = event.target as HTMLElement;
            const link = target.closest<HTMLAnchorElement>('a[data-page]');
            if (!link) {
                return;
            }

            event.preventDefault();
            const page = Number(link.dataset.page ?? '0');
            if (!Number.isFinite(page) || page < 1 || page > totalPages) {
                return;
            }

            currentPage = page;
            updateView();
        });

        document.getElementById('pricing-edit-tld')?.addEventListener('change', () => {
            const tldId = Number((document.getElementById('pricing-edit-tld') as HTMLSelectElement | null)?.value ?? '0');
            if (!Number.isFinite(tldId) || tldId <= 0) {
                setEditFieldsFromRow(null);
                return;
            }

            setEditFieldsFromRow(loadCurrentPricingForTld(tldId));
        });
    };

    const initializePricingPage = (): void => {
        const page = document.getElementById('pricing-page') as HTMLElement | null;
        if (!page || page.dataset.initialized === 'true') {
            return;
        }

        page.dataset.initialized = 'true';

        const params = new URLSearchParams(window.location.search);
        const scope = (params.get('scope') || '').toLowerCase();
        const badge = document.getElementById('pricing-scope-badge');
        if (badge) {
            badge.textContent = scope === 'tld' ? 'TLD prices only' : 'TLD prices';
        }

        bindEvents();
        void loadPricingRows();
        void loadDefaultProfitPercentForTld();
    };

    const initializeRedirectPage = (): void => {
        const page = document.getElementById('tld-pricing-redirect-page') as HTMLElement | null;
        if (!page || page.dataset.initialized === 'true') {
            return;
        }

        page.dataset.initialized = 'true';
        window.location.replace('/billing/pricing?scope=tld');
    };

    const setupObserver = (): void => {
        initializePricingPage();
        initializeRedirectPage();

        if (document.body) {
            const observer = new MutationObserver(() => {
                initializePricingPage();
                initializeRedirectPage();
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

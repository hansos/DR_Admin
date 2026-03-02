(() => {
    interface AppSettings {
        apiBaseUrl?: string;
    }

    interface CustomerSummary {
        id: number;
        name: string;
        customerName?: string;
        email?: string;
    }

    interface HostingPackage {
        id: number;
        name: string;
        monthlyPrice: number;
        yearlyPrice: number;
    }

    interface BillingCycle {
        id: number;
        code: string;
        name: string;
        durationInDays: number;
    }

    interface ServiceItem {
        id: number;
        name: string;
        description: string;
        price: number | null;
    }

    interface ResellerCompany {
        id: number;
        name: string;
        contactPerson: string;
        email: string;
        phone: string;
        address: string;
        city: string;
        state: string;
        postalCode: string;
        countryCode: string;
        companyRegistrationNumber: string;
        vatNumber: string;
    }

    interface ApiResponse<T> {
        success: boolean;
        data?: T;
        message?: string;
    }

    interface NewSaleState {
        showOngoingCard?: boolean;
        isOfferListContext?: boolean;
        domainName?: string;
        flowType?: string;
        pricing?: {
            registration: number | null;
            currency: string;
        };
        selectedCustomer?: CustomerSummary;
        hostingPackageId?: number;
        billingCycleId?: number;
        hostingSkipped?: boolean;
        otherServices?: {
            selectedServiceIds?: number[];
            customServiceNotes?: string;
            transferAuthCode?: string;
            registrationPeriodYears?: number;
            autoRenew?: boolean;
            privacyProtection?: boolean;
            domainOperationPrice?: number | null;
            currency?: string;
        };
        offer?: {
            quoteId?: number;
            status?: string;
            lastAction?: string;
            lastRevisionNumber?: number;
            validUntil?: string;
            couponCode?: string;
            discountPercent?: number;
            notes?: string;
            lineCount?: number;
            oneTimeSubtotal?: number;
            recurringSubtotal?: number;
            grandTotal?: number;
            sentAt?: string;
            acceptedAt?: string;
        };
    }

    interface LineItem {
        serviceId?: number;
        description: string;
        quantity: number;
        unitPrice: number;
        subtotal: number;
        type: 'One-time' | 'Recurring';
    }

    interface OfferDocumentPayload {
        quoteId?: number;
        seller: SellerInfoPayload | null;
        saleContext: SaleContextPayload;
        offerSettings: OfferSettingsPayload;
        lineItems: OfferLineItemPayload[];
        totals: OfferTotalsPayload;
    }

    interface OfferPersistenceResponse {
        quoteId: number;
        status: string;
        revisionNumber: number;
        actionType: string;
        fileName: string;
        outputPath: string;
        sentAt?: string;
        acceptedAt?: string;
        orderId?: number;
        orderNumber?: string;
        orderStatus?: string;
    }

    interface SellerInfoPayload {
        id: number;
        name: string;
        contactPerson: string;
        email: string;
        phone: string;
        address: string;
        city: string;
        state: string;
        postalCode: string;
        countryCode: string;
        companyRegistrationNumber: string;
        vatNumber: string;
    }

    interface SaleContextPayload {
        domainName: string;
        flowType: string;
        customer: {
            id: number;
            name: string;
            customerName: string;
            email: string;
        };
        currency: string;
    }

    interface OfferSettingsPayload {
        validUntil?: string;
        couponCode?: string;
        discountPercent?: number;
        notes?: string;
        sentAt?: string;
        acceptedAt?: string;
    }

    interface OfferLineItemPayload {
        lineNumber: number;
        serviceId?: number;
        description: string;
        quantity: number;
        unitPrice: number;
        subtotal: number;
        type: string;
    }

    interface OfferTotalsPayload {
        lineCount: number;
        oneTimeSubtotal: number;
        recurringSubtotal: number;
        grandTotal: number;
    }

    interface OfferEditorSnapshotResponse {
        quoteId?: number;
        quoteStatus?: string;
        lastAction?: string;
        lastRevisionNumber?: number;
        offer?: unknown;
    }

    interface BootstrapModalInstance {
        show(): void;
        hide(): void;
    }

    interface BootstrapModalConstructor {
        new (element: Element): BootstrapModalInstance;
        getInstance(element: Element): BootstrapModalInstance | null;
    }

    interface BootstrapNamespace {
        Modal: BootstrapModalConstructor;
    }

    const storageKey = 'new-sale-state';
    const originalOfferSignatureStorageKey = 'new-sale-offer-original-signature';

    let currentState: NewSaleState | null = null;
    let currencyCode = 'USD';
    let hostingPackages = new Map<number, HostingPackage>();
    let billingCycles = new Map<number, BillingCycle>();
    let services = new Map<number, ServiceItem>();
    let currentSellerCompany: ResellerCompany | null = null;
    let restoredLineItems: LineItem[] | null = null;
    let initialOfferChangeSignature: string | null = null;

    const getBootstrap = (): BootstrapNamespace | null => {
        const maybeBootstrap = (window as Window & { bootstrap?: BootstrapNamespace }).bootstrap;
        return maybeBootstrap ?? null;
    };

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

    const apiRequest = async <T,>(endpoint: string, options: RequestInit = {}): Promise<ApiResponse<T>> => {
        try {
            const headers: Record<string, string> = {
                'Content-Type': 'application/json',
                ...(options.headers as Record<string, string> | undefined),
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
                    message: (data && ((data as { message?: string }).message ?? (data as { title?: string }).title)) ||
                        `Request failed with status ${response.status}`,
                };
            }

            const parsed = data as { success?: boolean; data?: T; message?: string } | null;
            return {
                success: parsed?.success !== false,
                data: parsed?.data ?? (data as T),
                message: parsed?.message,
            };
        } catch (error) {
            console.error('New sale offer request failed', error);
            return {
                success: false,
                message: 'Network error. Please try again.',
            };
        }
    };

    const esc = (text: string): string => {
        const map: Record<string, string> = { '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#039;' };
        return (text || '').replace(/[&<>"']/g, (char) => map[char] ?? char);
    };

    const loadState = (): NewSaleState | null => {
        const raw = sessionStorage.getItem(storageKey);
        if (!raw) {
            return null;
        }

        try {
            return JSON.parse(raw) as NewSaleState;
        } catch {
            return null;
        }
    };

    const saveState = (): void => {
        if (!currentState) {
            return;
        }

        const validUntil = (document.getElementById('new-sale-offer-valid-until') as HTMLInputElement | null)?.value ?? '';
        const couponCode = (document.getElementById('new-sale-offer-coupon') as HTMLInputElement | null)?.value.trim() ?? '';
        const notes = (document.getElementById('new-sale-offer-notes') as HTMLTextAreaElement | null)?.value.trim() ?? '';

        const discountRaw = (document.getElementById('new-sale-offer-discount') as HTMLInputElement | null)?.value ?? '';
        const discountParsed = Number(discountRaw);
        const discountPercent = Number.isFinite(discountParsed) && discountParsed >= 0 ? Math.min(100, discountParsed) : 0;

        const totals = calculateTotals(buildLineItems());

        currentState.offer = {
            ...currentState.offer,
            validUntil: validUntil || undefined,
            couponCode: couponCode || undefined,
            discountPercent,
            notes: notes || undefined,
            lineCount: totals.lineCount,
            oneTimeSubtotal: totals.oneTime,
            recurringSubtotal: totals.recurring,
            grandTotal: totals.grand,
        };

        sessionStorage.setItem(storageKey, JSON.stringify(currentState));
    };

    const showSuccess = (message: string): void => {
        const alert = document.getElementById('new-sale-offer-alert-success');
        if (!alert) {
            return;
        }

        alert.textContent = message;
        alert.classList.remove('d-none');
        document.getElementById('new-sale-offer-alert-error')?.classList.add('d-none');

        setTimeout(() => alert.classList.add('d-none'), 4000);
    };

    const showError = (message: string): void => {
        const alert = document.getElementById('new-sale-offer-alert-error');
        if (!alert) {
            return;
        }

        alert.textContent = message;
        alert.classList.remove('d-none');
        document.getElementById('new-sale-offer-alert-success')?.classList.add('d-none');
    };

    const normalizeHostingPackage = (item: unknown): HostingPackage => {
        const typed = (item ?? {}) as Record<string, unknown>;
        return {
            id: Number(typed.id ?? typed.Id ?? 0),
            name: String(typed.name ?? typed.Name ?? ''),
            monthlyPrice: Number(typed.monthlyPrice ?? typed.MonthlyPrice ?? 0),
            yearlyPrice: Number(typed.yearlyPrice ?? typed.YearlyPrice ?? 0),
        };
    };

    const isEditingExistingQuote = (): boolean => {
        const quoteId = Number(currentState?.offer?.quoteId ?? 0);
        return currentState?.isOfferListContext === true && quoteId > 0;
    };

    const createOfferChangeSignature = (
        validUntil: string,
        couponCode: string,
        discountPercent: number,
        notes: string,
        lines: LineItem[],
        totals: { lineCount: number; oneTime: number; recurring: number; grand: number }
    ): string => {
        return JSON.stringify({
            validUntil,
            couponCode,
            discountPercent,
            notes,
            lines,
            totals,
        });
    };

    const buildOfferChangeSignature = (): string => {
        const validUntil = (document.getElementById('new-sale-offer-valid-until') as HTMLInputElement | null)?.value ?? '';
        const couponCode = (document.getElementById('new-sale-offer-coupon') as HTMLInputElement | null)?.value.trim() ?? '';
        const notes = (document.getElementById('new-sale-offer-notes') as HTMLTextAreaElement | null)?.value.trim() ?? '';

        const discountRaw = Number((document.getElementById('new-sale-offer-discount') as HTMLInputElement | null)?.value ?? '0');
        const discountPercent = Number.isFinite(discountRaw) && discountRaw >= 0 ? Math.min(100, discountRaw) : 0;

        const lines = buildLineItems();
        const totals = calculateTotals(lines);

        return createOfferChangeSignature(validUntil, couponCode, discountPercent, notes, lines, totals);
    };

    const updateAcceptActionState = (): void => {
        const acceptButton = document.getElementById('new-sale-offer-accept') as HTMLButtonElement | null;
        const confirmButton = document.getElementById('new-sale-offer-accept-confirm') as HTMLButtonElement | null;
        if (!acceptButton) {
            return;
        }

        if (isEditingExistingQuote()) {
            const hasChanges = initialOfferChangeSignature !== null && buildOfferChangeSignature() !== initialOfferChangeSignature;
            acceptButton.innerHTML = 'Save changes and continue <i class="bi bi-arrow-right"></i>';
            acceptButton.disabled = !hasChanges;

            if (confirmButton) {
                confirmButton.textContent = 'Save changes and continue';
            }

            return;
        }

        acceptButton.innerHTML = 'Accept and continue <i class="bi bi-arrow-right"></i>';
        acceptButton.disabled = false;

        if (confirmButton) {
            confirmButton.textContent = 'Yes, accept and continue';
        }
    };

    const normalizeBillingCycle = (item: unknown): BillingCycle => {
        const typed = (item ?? {}) as Record<string, unknown>;
        return {
            id: Number(typed.id ?? typed.Id ?? 0),
            code: String(typed.code ?? typed.Code ?? ''),
            name: String(typed.name ?? typed.Name ?? ''),
            durationInDays: Number(typed.durationInDays ?? typed.DurationInDays ?? 0),
        };
    };

    const normalizeService = (item: unknown): ServiceItem => {
        const typed = (item ?? {}) as Record<string, unknown>;
        const price = Number(typed.price ?? typed.Price);
        return {
            id: Number(typed.id ?? typed.Id ?? 0),
            name: String(typed.name ?? typed.Name ?? ''),
            description: String(typed.description ?? typed.Description ?? ''),
            price: Number.isFinite(price) ? price : null,
        };
    };

    const normalizeResellerCompany = (item: unknown): ResellerCompany => {
        const typed = (item ?? {}) as Record<string, unknown>;
        return {
            id: Number(typed.id ?? typed.Id ?? 0),
            name: String(typed.name ?? typed.Name ?? ''),
            contactPerson: String(typed.contactPerson ?? typed.ContactPerson ?? ''),
            email: String(typed.email ?? typed.Email ?? ''),
            phone: String(typed.phone ?? typed.Phone ?? ''),
            address: String(typed.address ?? typed.Address ?? ''),
            city: String(typed.city ?? typed.City ?? ''),
            state: String(typed.state ?? typed.State ?? ''),
            postalCode: String(typed.postalCode ?? typed.PostalCode ?? ''),
            countryCode: String(typed.countryCode ?? typed.CountryCode ?? ''),
            companyRegistrationNumber: String(typed.companyRegistrationNumber ?? typed.CompanyRegistrationNumber ?? ''),
            vatNumber: String(typed.vatNumber ?? typed.VatNumber ?? ''),
        };
    };

    const parseList = (raw: unknown): unknown[] => {
        if (Array.isArray(raw)) {
            return raw;
        }

        const wrapped = raw as { data?: unknown[]; Data?: unknown[] } | null;
        if (Array.isArray(wrapped?.data)) {
            return wrapped.data;
        }

        if (Array.isArray(wrapped?.Data)) {
            return wrapped.Data;
        }

        return [];
    };

    const getRequestedQuoteId = (): number | null => {
        const params = new URLSearchParams(window.location.search);
        const raw = params.get('quoteId');
        if (!raw) {
            return null;
        }

        const parsed = Number(raw);
        return Number.isInteger(parsed) && parsed > 0 ? parsed : null;
    };

    const normalizeSnapshotLineItem = (item: unknown): LineItem | null => {
        const typed = (item ?? {}) as Record<string, unknown>;
        const description = String(typed.description ?? typed.Description ?? '').trim();
        if (!description) {
            return null;
        }

        const quantityRaw = Number(typed.quantity ?? typed.Quantity ?? 1);
        const quantity = Number.isFinite(quantityRaw) && quantityRaw > 0 ? quantityRaw : 1;

        const unitPriceRaw = Number(typed.unitPrice ?? typed.UnitPrice ?? 0);
        const unitPrice = Number.isFinite(unitPriceRaw) ? unitPriceRaw : 0;

        const subtotalRaw = Number(typed.subtotal ?? typed.Subtotal ?? unitPrice * quantity);
        const subtotal = Number.isFinite(subtotalRaw) ? subtotalRaw : unitPrice * quantity;

        const typeRaw = String(typed.type ?? typed.Type ?? 'One-time').trim().toLowerCase();
        const type: LineItem['type'] = typeRaw === 'recurring' ? 'Recurring' : 'One-time';

        const serviceIdRaw = Number(typed.serviceId ?? typed.ServiceId ?? 0);
        const serviceId = Number.isInteger(serviceIdRaw) && serviceIdRaw > 0 ? serviceIdRaw : undefined;

        return {
            serviceId,
            description,
            quantity,
            unitPrice,
            subtotal,
            type,
        };
    };

    const loadRequestedQuoteState = async (quoteId: number): Promise<boolean> => {
        const response = await apiRequest<OfferEditorSnapshotResponse>(`${getApiBaseUrl()}/System/offer-editor/${quoteId}`, { method: 'GET' });
        if (!response.success || !response.data) {
            showError(response.message ?? `Could not load quote #${quoteId}.`);
            return false;
        }

        const payload = response.data as OfferEditorSnapshotResponse & Record<string, unknown>;
        const snapshotRaw = (payload.offer ?? payload.Offer ?? null) as Record<string, unknown> | null;
        if (!snapshotRaw) {
            showError(`Quote #${quoteId} has no saved offer snapshot.`);
            return false;
        }

        const saleContext = (snapshotRaw.saleContext ?? snapshotRaw.SaleContext ?? {}) as Record<string, unknown>;
        const customer = (saleContext.customer ?? saleContext.Customer ?? {}) as Record<string, unknown>;
        const offerSettings = (snapshotRaw.offerSettings ?? snapshotRaw.OfferSettings ?? {}) as Record<string, unknown>;
        const totals = (snapshotRaw.totals ?? snapshotRaw.Totals ?? {}) as Record<string, unknown>;

        const validUntilRaw = String(offerSettings.validUntil ?? offerSettings.ValidUntil ?? '').trim();
        const couponCodeRaw = String(offerSettings.couponCode ?? offerSettings.CouponCode ?? '').trim();
        const notesRaw = String(offerSettings.notes ?? offerSettings.Notes ?? '').trim();
        const sentAtRaw = String(offerSettings.sentAt ?? offerSettings.SentAt ?? '').trim();
        const acceptedAtRaw = String(offerSettings.acceptedAt ?? offerSettings.AcceptedAt ?? '').trim();

        restoredLineItems = parseList(snapshotRaw.lineItems ?? snapshotRaw.LineItems)
            .map((item) => normalizeSnapshotLineItem(item))
            .filter((item): item is LineItem => item !== null);

        const snapshotDiscountPercentRaw = Number(offerSettings.discountPercent ?? offerSettings.DiscountPercent ?? 0);
        const snapshotDiscountPercent = Number.isFinite(snapshotDiscountPercentRaw) && snapshotDiscountPercentRaw >= 0
            ? Math.min(100, snapshotDiscountPercentRaw)
            : 0;

        const snapshotTotals = {
            lineCount: Number(totals.lineCount ?? totals.LineCount ?? restoredLineItems.length) || restoredLineItems.length,
            oneTime: Number(totals.oneTimeSubtotal ?? totals.OneTimeSubtotal ?? 0),
            recurring: Number(totals.recurringSubtotal ?? totals.RecurringSubtotal ?? 0),
            grand: Number(totals.grandTotal ?? totals.GrandTotal ?? 0),
        };

        const snapshotSignature = createOfferChangeSignature(
            validUntilRaw,
            couponCodeRaw,
            snapshotDiscountPercent,
            notesRaw,
            restoredLineItems,
            snapshotTotals
        );

        sessionStorage.setItem(originalOfferSignatureStorageKey, snapshotSignature);

        const currency = String(saleContext.currency ?? saleContext.Currency ?? 'USD');

        currentState = {
            showOngoingCard: false,
            isOfferListContext: true,
            domainName: String(saleContext.domainName ?? saleContext.DomainName ?? ''),
            flowType: String(saleContext.flowType ?? saleContext.FlowType ?? ''),
            selectedCustomer: {
                id: Number(customer.id ?? customer.Id ?? 0),
                name: String(customer.name ?? customer.Name ?? ''),
                customerName: String(customer.customerName ?? customer.CustomerName ?? ''),
                email: String(customer.email ?? customer.Email ?? ''),
            },
            pricing: {
                registration: null,
                currency,
            },
            otherServices: {
                currency,
            },
            offer: {
                quoteId: Number(payload.quoteId ?? payload.QuoteId ?? quoteId),
                status: String(payload.quoteStatus ?? payload.QuoteStatus ?? ''),
                lastAction: String(payload.lastAction ?? payload.LastAction ?? ''),
                lastRevisionNumber: Number(payload.lastRevisionNumber ?? payload.LastRevisionNumber ?? 0) || undefined,
                validUntil: validUntilRaw || undefined,
                couponCode: couponCodeRaw || undefined,
                discountPercent: Number(offerSettings.discountPercent ?? offerSettings.DiscountPercent ?? 0) || undefined,
                notes: notesRaw || undefined,
                lineCount: snapshotTotals.lineCount,
                oneTimeSubtotal: snapshotTotals.oneTime,
                recurringSubtotal: snapshotTotals.recurring,
                grandTotal: snapshotTotals.grand,
                sentAt: sentAtRaw || undefined,
                acceptedAt: acceptedAtRaw || undefined,
            },
        };

        sessionStorage.setItem(storageKey, JSON.stringify(currentState));
        return true;
    };

    const renderSellerHeader = (company: ResellerCompany | null): void => {
        const nameElement = document.getElementById('new-sale-offer-seller-name');
        const contactElement = document.getElementById('new-sale-offer-seller-contact');
        const addressElement = document.getElementById('new-sale-offer-seller-address');
        const registrationElement = document.getElementById('new-sale-offer-seller-registration');

        if (!nameElement || !contactElement || !addressElement || !registrationElement) {
            return;
        }

        if (!company) {
            nameElement.textContent = 'Seller';
            contactElement.textContent = '-';
            addressElement.textContent = '-';
            registrationElement.textContent = '-';
            return;
        }

        const locationParts = [company.postalCode, company.city, company.state, company.countryCode]
            .map((value) => value.trim())
            .filter((value) => value.length > 0);
        const addressText = [company.address.trim(), locationParts.join(' ')].filter((value) => value.length > 0).join(', ');

        const contactParts = [
            company.contactPerson.trim(),
            company.email.trim(),
            company.phone.trim(),
        ].filter((value) => value.length > 0);

        const registrationParts = [
            company.companyRegistrationNumber.trim() ? `Org no: ${company.companyRegistrationNumber.trim()}` : '',
            company.vatNumber.trim() ? `VAT: ${company.vatNumber.trim()}` : '',
        ].filter((value) => value.length > 0);

        nameElement.textContent = company.name.trim() || 'Seller';
        contactElement.textContent = contactParts.join(' · ') || '-';
        addressElement.textContent = addressText || '-';
        registrationElement.textContent = registrationParts.join(' · ') || '-';
    };

    const loadSellerCompanyInfo = async (): Promise<void> => {
        const defaultResponse = await apiRequest<unknown>(`${getApiBaseUrl()}/ResellerCompanies/default`, { method: 'GET' });
        if (defaultResponse.success && defaultResponse.data) {
            currentSellerCompany = normalizeResellerCompany(defaultResponse.data);
            renderSellerHeader(currentSellerCompany);
            return;
        }

        const activeResponse = await apiRequest<unknown>(`${getApiBaseUrl()}/ResellerCompanies/active`, { method: 'GET' });
        const activeCompanies = parseList(activeResponse.data)
            .map((item) => normalizeResellerCompany(item))
            .filter((item) => item.id > 0);

        currentSellerCompany = activeCompanies.length > 0 ? activeCompanies[0] : null;
        renderSellerHeader(currentSellerCompany);
    };

    const createOfferDocumentPayload = (): OfferDocumentPayload | null => {
        if (!currentState?.selectedCustomer) {
            return null;
        }

        const lines = buildLineItems();
        const totals = calculateTotals(lines);
        const offer = currentState.offer;

        return {
            quoteId: offer?.quoteId,
            seller: currentSellerCompany
                ? {
                    id: currentSellerCompany.id,
                    name: currentSellerCompany.name,
                    contactPerson: currentSellerCompany.contactPerson,
                    email: currentSellerCompany.email,
                    phone: currentSellerCompany.phone,
                    address: currentSellerCompany.address,
                    city: currentSellerCompany.city,
                    state: currentSellerCompany.state,
                    postalCode: currentSellerCompany.postalCode,
                    countryCode: currentSellerCompany.countryCode,
                    companyRegistrationNumber: currentSellerCompany.companyRegistrationNumber,
                    vatNumber: currentSellerCompany.vatNumber,
                }
                : null,
            saleContext: {
                domainName: currentState.domainName ?? '',
                flowType: currentState.flowType ?? '',
                customer: {
                    id: currentState.selectedCustomer.id,
                    name: currentState.selectedCustomer.name,
                    customerName: currentState.selectedCustomer.customerName ?? '',
                    email: currentState.selectedCustomer.email ?? '',
                },
                currency: currencyCode,
            },
            offerSettings: {
                validUntil: offer?.validUntil,
                couponCode: offer?.couponCode,
                discountPercent: offer?.discountPercent,
                notes: offer?.notes,
                sentAt: offer?.sentAt,
                acceptedAt: offer?.acceptedAt,
            },
            lineItems: lines.map((line, index) => ({
                lineNumber: index + 1,
                serviceId: line.serviceId,
                description: line.description,
                quantity: line.quantity,
                unitPrice: line.unitPrice,
                subtotal: line.subtotal,
                type: line.type,
            })),
            totals: {
                lineCount: totals.lineCount,
                oneTimeSubtotal: totals.oneTime,
                recurringSubtotal: totals.recurring,
                grandTotal: totals.grand,
            },
        };
    };

    const applyPersistenceResponse = (responseData: OfferPersistenceResponse): void => {
        if (!currentState) {
            return;
        }

        currentState.offer = {
            ...currentState.offer,
            quoteId: responseData.quoteId,
            status: responseData.status,
            lastAction: responseData.actionType,
            lastRevisionNumber: responseData.revisionNumber,
            sentAt: responseData.sentAt ?? currentState.offer?.sentAt,
            acceptedAt: responseData.acceptedAt ?? currentState.offer?.acceptedAt,
        };

        sessionStorage.setItem(storageKey, JSON.stringify(currentState));
        renderPersistenceState();
    };

    const generateServerPdfForVerification = async (): Promise<boolean> => {
        const payload = createOfferDocumentPayload();
        if (!payload) {
            return false;
        }

        const response = await apiRequest<OfferPersistenceResponse>(`${getApiBaseUrl()}/System/verify-offer-print`, {
            method: 'POST',
            body: JSON.stringify(payload),
        });

        if (!response.success) {
            showError(response.message ?? 'Could not generate PDF file on server.');
            return false;
        }

        if (response.data) {
            applyPersistenceResponse(response.data);
        }

        showSuccess('PDF generated on server in the configured reports folder.');
        return true;
    };

    const formatMoney = (amount: number): string => `${amount.toFixed(2)} ${currencyCode}`;

    const toTitle = (value: string): string => {
        if (!value) {
            return '-';
        }

        return value
            .split('-')
            .map((part) => part.charAt(0).toUpperCase() + part.slice(1))
            .join(' ');
    };

    const setContextHeader = (): void => {
        if (!currentState) {
            return;
        }

        const domain = document.getElementById('new-sale-offer-domain');
        const flow = document.getElementById('new-sale-offer-flow');
        const customer = document.getElementById('new-sale-offer-customer');
        const currency = document.getElementById('new-sale-offer-currency');

        if (domain) {
            domain.textContent = currentState.domainName || '-';
        }

        if (flow) {
            flow.textContent = toTitle(currentState.flowType ?? '');
        }

        if (customer) {
            customer.textContent = currentState.selectedCustomer?.name || currentState.selectedCustomer?.customerName || '-';
        }

        if (currency) {
            currency.textContent = currencyCode;
        }
    };

    const updatePageTitle = (): void => {
        const title = document.getElementById('new-sale-offer-page-title');
        if (!title) {
            return;
        }

        const quoteId = Number(currentState?.offer?.quoteId ?? 0);
        const status = String(currentState?.offer?.status ?? '').trim();
        const label = quoteId > 0 || status.length > 0 ? 'Edit Quote' : 'New Quote';
        title.innerHTML = `<i class="bi bi-receipt"></i> ${label}`;
    };

    const renderPersistenceState = (): void => {
        const quoteId = document.getElementById('new-sale-offer-quote-id');
        const status = document.getElementById('new-sale-offer-status');
        const lastAction = document.getElementById('new-sale-offer-last-action');
        const lastRevision = document.getElementById('new-sale-offer-last-revision');

        if (!quoteId || !status || !lastAction || !lastRevision) {
            return;
        }

        const offer = currentState?.offer;
        quoteId.textContent = offer?.quoteId ? String(offer.quoteId) : '-';
        status.textContent = offer?.status || 'Draft';
        lastAction.textContent = offer?.lastAction || '-';
        lastRevision.textContent = offer?.lastRevisionNumber ? String(offer.lastRevisionNumber) : '-';
    };

    const resolveHostingRecurringPrice = (pkg: HostingPackage, cycle: BillingCycle | null): number => {
        if (!cycle) {
            return pkg.monthlyPrice;
        }

        const code = cycle.code.toLowerCase();
        const name = cycle.name.toLowerCase();
        if (code.includes('year') || name.includes('year') || cycle.durationInDays >= 360) {
            return pkg.yearlyPrice;
        }

        return pkg.monthlyPrice;
    };

    const buildDomainOperationLine = (): LineItem | null => {
        if (!currentState?.domainName || !currentState.flowType) {
            return null;
        }

        const flowType = currentState.flowType.toLowerCase();
        const details: string[] = [];

        if (flowType === 'register') {
            const years = currentState.otherServices?.registrationPeriodYears ?? 1;
            details.push(`${years} year(s)`);
            details.push(currentState.otherServices?.autoRenew === false ? 'Manual renewal' : 'Auto-renew');
            if (currentState.otherServices?.privacyProtection) {
                details.push('Privacy protection');
            }
        }

        if (flowType === 'transfer' && currentState.otherServices?.transferAuthCode) {
            details.push('Auth code provided');
        }

        const detailText = details.length ? ` (${details.join(', ')})` : '';

        const configuredPrice = currentState.otherServices?.domainOperationPrice;
        const fallbackPrice = currentState.pricing?.registration;
        const rawPrice = configuredPrice ?? fallbackPrice ?? 0;
        const price = Number.isFinite(rawPrice) ? rawPrice : 0;
        const lineType: LineItem['type'] = flowType === 'register' || flowType === 'renew'
            ? 'Recurring'
            : 'One-time';

        return {
            description: `${toTitle(flowType)} ${currentState.domainName}${detailText}`,
            quantity: 1,
            unitPrice: price,
            subtotal: price,
            type: lineType,
        };
    };

    const buildHostingLine = (): LineItem | null => {
        if (!currentState?.hostingPackageId) {
            return null;
        }

        const pkg = hostingPackages.get(currentState.hostingPackageId);
        if (!pkg) {
            return {
                description: `Hosting package #${currentState.hostingPackageId}`,
                quantity: 1,
                unitPrice: 0,
                subtotal: 0,
                type: 'Recurring',
            };
        }

        const cycle = currentState.billingCycleId ? (billingCycles.get(currentState.billingCycleId) ?? null) : null;
        const cycleName = cycle?.name || 'Default billing';
        const price = resolveHostingRecurringPrice(pkg, cycle);

        return {
            description: `${pkg.name} (${cycleName})`,
            quantity: 1,
            unitPrice: price,
            subtotal: price,
            type: 'Recurring',
        };
    };

    const buildServiceLines = (): LineItem[] => {
        const selectedIds = currentState?.otherServices?.selectedServiceIds ?? [];

        return selectedIds
            .map((serviceId) => {
                const service = services.get(serviceId);
                if (!service) {
                    return {
                        description: `Service #${serviceId}`,
                        quantity: 1,
                        unitPrice: 0,
                        subtotal: 0,
                        type: 'Recurring' as const,
                    };
                }

                const price = Number.isFinite(service.price ?? Number.NaN) ? (service.price ?? 0) : 0;
                return {
                    serviceId,
                    description: service.name,
                    quantity: 1,
                    unitPrice: price,
                    subtotal: price,
                    type: 'Recurring' as const,
                };
            });
    };

    const buildLineItems = (): LineItem[] => {
        if (restoredLineItems && restoredLineItems.length > 0) {
            return restoredLineItems;
        }

        const lines: LineItem[] = [];

        const domainLine = buildDomainOperationLine();
        if (domainLine) {
            lines.push(domainLine);
        }

        const hostingLine = buildHostingLine();
        if (hostingLine) {
            lines.push(hostingLine);
        }

        lines.push(...buildServiceLines());

        return lines;
    };

    const calculateTotals = (lines: LineItem[]): { lineCount: number; oneTime: number; recurring: number; grand: number } => {
        const discountRaw = Number((document.getElementById('new-sale-offer-discount') as HTMLInputElement | null)?.value ?? '0');
        const discountPercent = Number.isFinite(discountRaw) && discountRaw > 0 ? Math.min(100, discountRaw) : 0;

        const oneTime = lines
            .filter((line) => line.type === 'One-time')
            .reduce((sum, line) => sum + line.subtotal, 0);

        const recurring = lines
            .filter((line) => line.type === 'Recurring')
            .reduce((sum, line) => sum + line.subtotal, 0);

        const gross = oneTime + recurring;
        const discountAmount = gross * (discountPercent / 100);
        const grand = Math.max(0, gross - discountAmount);

        return {
            lineCount: lines.length,
            oneTime,
            recurring,
            grand,
        };
    };

    const renderLines = (): void => {
        const body = document.getElementById('new-sale-offer-lines-body');
        const lineCount = document.getElementById('new-sale-offer-line-count');

        if (!body || !lineCount) {
            return;
        }

        const lines = buildLineItems();

        if (!lines.length) {
            body.innerHTML = '<tr><td colspan="5" class="text-center text-muted">No quote lines available. Go back and add at least one item.</td></tr>';
            lineCount.textContent = '0 lines';
            renderTotals(lines);
            return;
        }

        body.innerHTML = lines.map((line) => `
            <tr>
                <td>${esc(line.description)}</td>
                <td class="text-center">${line.quantity}</td>
                <td class="text-end">${esc(formatMoney(line.unitPrice))}</td>
                <td class="text-end">${esc(formatMoney(line.subtotal))}</td>
                <td class="text-end">${esc(line.type)}</td>
            </tr>
        `).join('');

        lineCount.textContent = `${lines.length} line${lines.length === 1 ? '' : 's'}`;
        renderTotals(lines);
    };

    const renderTotals = (lines: LineItem[]): void => {
        const totalOneTime = document.getElementById('new-sale-offer-total-onetime');
        const totalRecurring = document.getElementById('new-sale-offer-total-recurring');
        const totalGrand = document.getElementById('new-sale-offer-total-grand');
        if (!totalOneTime || !totalRecurring || !totalGrand) {
            return;
        }

        const totals = calculateTotals(lines);
        totalOneTime.textContent = formatMoney(totals.oneTime);
        totalRecurring.textContent = formatMoney(totals.recurring);
        totalGrand.textContent = formatMoney(totals.grand);

        saveState();
        updateAcceptActionState();
    };

    const restoreOfferSettings = (): void => {
        const offer = currentState?.offer;
        const validUntilInput = document.getElementById('new-sale-offer-valid-until') as HTMLInputElement | null;
        const couponInput = document.getElementById('new-sale-offer-coupon') as HTMLInputElement | null;
        const discountInput = document.getElementById('new-sale-offer-discount') as HTMLInputElement | null;
        const notesInput = document.getElementById('new-sale-offer-notes') as HTMLTextAreaElement | null;

        if (validUntilInput) {
            if (offer?.validUntil) {
                validUntilInput.value = offer.validUntil;
            } else {
                const defaultDate = new Date();
                defaultDate.setDate(defaultDate.getDate() + 14);
                validUntilInput.value = defaultDate.toISOString().slice(0, 10);
            }
        }

        if (couponInput) {
            couponInput.value = offer?.couponCode ?? '';
        }

        if (discountInput) {
            discountInput.value = offer?.discountPercent ? String(offer.discountPercent) : '';
        }

        if (notesInput) {
            notesInput.value = offer?.notes ?? currentState?.otherServices?.customServiceNotes ?? '';
        }
    };

    const loadSupportData = async (): Promise<void> => {
        const [hostingResponse, cyclesResponse, servicesResponse] = await Promise.all([
            apiRequest<unknown>(`${getApiBaseUrl()}/HostingPackages/active`, { method: 'GET' }),
            apiRequest<unknown>(`${getApiBaseUrl()}/BillingCycles`, { method: 'GET' }),
            apiRequest<unknown>(`${getApiBaseUrl()}/Services`, { method: 'GET' }),
        ]);

        hostingPackages = new Map<number, HostingPackage>();
        parseList(hostingResponse.data)
            .map((item) => normalizeHostingPackage(item))
            .filter((item) => item.id > 0)
            .forEach((item) => {
                hostingPackages.set(item.id, item);
            });

        billingCycles = new Map<number, BillingCycle>();
        parseList(cyclesResponse.data)
            .map((item) => normalizeBillingCycle(item))
            .filter((item) => item.id > 0)
            .forEach((item) => {
                billingCycles.set(item.id, item);
            });

        services = new Map<number, ServiceItem>();
        parseList(servicesResponse.data)
            .map((item) => normalizeService(item))
            .filter((item) => item.id > 0)
            .forEach((item) => {
                services.set(item.id, item);
            });
    };

    const sendToCustomer = async (): Promise<void> => {
        if (!currentState) {
            return;
        }

        saveState();

        const payload = createOfferDocumentPayload();
        if (!payload) {
            showError('Offer payload is incomplete.');
            return;
        }

        const response = await apiRequest<OfferPersistenceResponse>(`${getApiBaseUrl()}/System/send-offer`, {
            method: 'POST',
            body: JSON.stringify(payload),
        });

        if (!response.success || !response.data) {
            showError(response.message ?? 'Could not send and persist offer.');
            return;
        }

        applyPersistenceResponse(response.data);
        showSuccess('Offer persisted and marked as sent.');
    };

    const closeAcceptConfirmationModal = (): void => {
        const bootstrap = getBootstrap();
        const modalElement = document.getElementById('new-sale-offer-accept-confirm-modal');
        if (!bootstrap || !modalElement) {
            return;
        }

        const modalInstance = bootstrap.Modal.getInstance(modalElement) ?? new bootstrap.Modal(modalElement);
        modalInstance.hide();
    };

    const showAcceptConfirmationModal = (): void => {
        const bootstrap = getBootstrap();
        const modalElement = document.getElementById('new-sale-offer-accept-confirm-modal');
        if (!bootstrap || !modalElement) {
            void acceptAndContinue();
            return;
        }

        const modalInstance = bootstrap.Modal.getInstance(modalElement) ?? new bootstrap.Modal(modalElement);
        modalInstance.show();
    };

    const acceptAndContinue = async (): Promise<void> => {
        if (!currentState) {
            return;
        }

        const lines = buildLineItems();
        if (!lines.length) {
            showError('No offer lines available. Add products before continuing.');
            return;
        }

        saveState();

        const payload = createOfferDocumentPayload();
        if (!payload) {
            showError('Offer payload is incomplete.');
            return;
        }

        const response = await apiRequest<OfferPersistenceResponse>(`${getApiBaseUrl()}/System/accept-offer`, {
            method: 'POST',
            body: JSON.stringify(payload),
        });

        if (!response.success || !response.data) {
            showError(response.message ?? 'Could not accept and convert offer.');
            return;
        }

        closeAcceptConfirmationModal();
        applyPersistenceResponse(response.data);
        window.location.href = '/dashboard/quote/payment';
    };

    const printOffer = async (): Promise<void> => {
        saveState();
        await generateServerPdfForVerification();
    };

    const bindEvents = (): void => {
        ['new-sale-offer-valid-until', 'new-sale-offer-coupon', 'new-sale-offer-discount', 'new-sale-offer-notes']
            .forEach((id) => {
                const eventName = id === 'new-sale-offer-notes' ? 'input' : 'change';
                document.getElementById(id)?.addEventListener(eventName, () => {
                    renderLines();
                });
            });

        document.getElementById('new-sale-offer-send')?.addEventListener('click', sendToCustomer);
        document.getElementById('new-sale-offer-print')?.addEventListener('click', printOffer);
        document.getElementById('new-sale-offer-accept')?.addEventListener('click', showAcceptConfirmationModal);
        document.getElementById('new-sale-offer-accept-confirm')?.addEventListener('click', () => {
            void acceptAndContinue();
        });

        window.addEventListener('afterprint', () => {
            document.body.classList.remove('print-new-sale-offer');
        });
    };

    const focusValidUntilInput = (): void => {
        const validUntilInput = document.getElementById('new-sale-offer-valid-until') as HTMLInputElement | null;
        if (!validUntilInput) {
            return;
        }

        requestAnimationFrame(() => {
            validUntilInput.focus();
            validUntilInput.select();
        });
    };

    const initializePage = async (): Promise<void> => {
        const page = document.getElementById('dashboard-new-sale-offer-page') as HTMLElement | null;
        if (!page || page.dataset.initialized === 'true') {
            return;
        }

        page.dataset.initialized = 'true';

        currentState = loadState();
        restoredLineItems = null;

        const requestedQuoteId = getRequestedQuoteId();
        if (requestedQuoteId) {
            const loaded = await loadRequestedQuoteState(requestedQuoteId);
            if (!loaded) {
                return;
            }
        }

        if (!currentState?.domainName || !currentState?.flowType || !currentState?.selectedCustomer) {
            window.location.href = '/dashboard/quote';
            return;
        }

        currencyCode = currentState.otherServices?.currency || currentState.pricing?.currency || 'USD';

        updatePageTitle();
        setContextHeader();
        restoreOfferSettings();
        renderPersistenceState();
        bindEvents();
        focusValidUntilInput();

        await Promise.all([
            loadSupportData(),
            loadSellerCompanyInfo(),
        ]);
        renderLines();

        if (isEditingExistingQuote()) {
            initialOfferChangeSignature = sessionStorage.getItem(originalOfferSignatureStorageKey) || buildOfferChangeSignature();
        } else {
            initialOfferChangeSignature = null;
            sessionStorage.removeItem(originalOfferSignatureStorageKey);
        }

        updateAcceptActionState();
    };

    const setupObserver = (): void => {
        void initializePage();

        if (document.body) {
            const observer = new MutationObserver(() => {
                const page = document.getElementById('dashboard-new-sale-offer-page') as HTMLElement | null;
                if (page && page.dataset.initialized !== 'true') {
                    void initializePage();
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

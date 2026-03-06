"use strict";
// @ts-nocheck
(function () {
    let hasOngoingWorkflowWarning = false;
    let pendingAcceptQuoteId = null;
    let pendingAcceptQuoteNumber = '';
    const setupTodoItems = [
        {
            key: 'dns-templates',
            name: 'DNS Templates',
            description: 'Create reusable DNS templates for faster and safer zone setup.',
            fixUrl: '/dns/templates',
            checkType: 'static-not-created',
        },
        {
            key: 'billing-cycles',
            name: 'Billing Cycles',
            description: 'Define billing periods used by subscriptions and services.',
            fixUrl: '/billing/cycles',
            endpoint: '/BillingCycles',
            checkType: 'list',
        },
        {
            key: 'payment-gateways',
            name: 'Payment Gateways',
            description: 'Configure active payment providers used for transactions.',
            fixUrl: '/billing/payment-gateways',
            endpoint: '/PaymentGateways',
            dependsOn: ['payment-instruments'],
            checkType: 'list',
        },
        {
            key: 'payment-instruments',
            name: 'Payment Instruments',
            description: 'Define payment instrument options available to customers.',
            fixUrl: '/billing/payment-instruments',
            endpoint: '/PaymentInstruments',
            checkType: 'list',
        },
        {
            key: 'tlds',
            name: 'TLD List',
            description: 'Register and maintain supported top-level domains.',
            fixUrl: '/tld/list',
            endpoint: '/Tlds',
            dependsOn: ['registry-apis'],
            checkType: 'list',
        },
        {
            key: 'registry-apis',
            name: 'Registry APIs',
            description: 'Add registrar/registry integrations for domain operations.',
            fixUrl: '/integrations/registry-apis',
            endpoint: '/Registrars',
            checkType: 'list',
        },
        {
            key: 'server-types',
            name: 'Server Types',
            description: 'Define server type categories used by infrastructure records.',
            fixUrl: '/infrastructure/server-types',
            endpoint: '/ServerTypes',
            checkType: 'list',
        },
        {
            key: 'operating-systems',
            name: 'Operating Systems',
            description: 'Create operating system options used by servers.',
            fixUrl: '/infrastructure/operating-systems',
            endpoint: '/OperatingSystems',
            checkType: 'list',
        },
        {
            key: 'host-providers',
            name: 'Host Providers',
            description: 'Register host providers used by server and package setup.',
            fixUrl: '/infrastructure/host-providers',
            endpoint: '/HostProviders',
            checkType: 'list',
        },
        {
            key: 'servers',
            name: 'Servers',
            description: 'Add the servers used for hosting and provisioning.',
            fixUrl: '/infrastructure/servers',
            endpoint: '/Servers',
            dependsOn: ['server-types', 'operating-systems', 'host-providers'],
            checkType: 'list',
        },
        {
            key: 'ip-addresses',
            name: 'IP Addresses',
            description: 'Register available IP addresses and assign them to servers.',
            fixUrl: '/infrastructure/ip-addresses',
            endpoint: '/ServerIpAddresses',
            dependsOn: ['servers'],
            checkType: 'list',
        },
        {
            key: 'services',
            name: 'Services',
            description: 'Add additional services to sell to customers.',
            fixUrl: '/infrastructure/services',
            endpoint: '/Services',
            dependsOn: ['billing-cycles'],
            checkType: 'list',
        },
        {
            key: 'hosting-packages',
            name: 'Hosting Packages',
            description: 'Define hosting plans available for sale.',
            fixUrl: '/infrastructure/hosting-packages',
            endpoint: '/HostingPackages',
            dependsOn: ['servers', 'control-panel-types'],
            checkType: 'list',
        },
        {
            key: 'control-panel-types',
            name: 'Control Panel Types',
            description: 'Configure hosting panel types (for example, cPanel, Plesk).',
            fixUrl: '/infrastructure/control-panel-types',
            endpoint: '/ControlPanelTypes',
            checkType: 'list',
        },
        {
            key: 'roles',
            name: 'User Roles',
            description: 'Ensure required role definitions and permissions are present.',
            fixUrl: '/user/roles',
            endpoint: '/Roles',
            checkType: 'list',
        },
        {
            key: 'company-settings',
            name: 'Company Setup',
            description: 'Fill in legal and contact profile used in invoices and communication.',
            fixUrl: '/settings/company',
            endpoint: '/MyCompany',
            checkType: 'company',
        },
    ];
    function getApiBaseUrl() {
        return window.AppSettings?.apiBaseUrl ?? '';
    }
    function getBootstrap() {
        return window.bootstrap ?? null;
    }
    function getAuthToken() {
        const auth = window.Auth;
        if (auth?.getToken) {
            return auth.getToken();
        }
        return sessionStorage.getItem('rp_authToken');
    }
    async function apiRequest(endpoint, options = {}) {
        try {
            const headers = {
                'Content-Type': 'application/json',
                ...options.headers,
            };
            const authToken = getAuthToken();
            if (authToken) {
                headers['Authorization'] = `Bearer ${authToken}`;
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
                    message: (data && (data.message ?? data.title)) || `Request failed with status ${response.status}`,
                };
            }
            return {
                success: data?.success !== false,
                data: data?.data ?? data,
                message: data?.message,
            };
        }
        catch (error) {
            console.error('Dashboard summary request failed', error);
            return {
                success: false,
                message: 'Network error. Please try again.',
            };
        }
    }
    function setSeedDataSubmitting(isSubmitting) {
        const button = document.getElementById('dashboard-summary-seed-data-button');
        if (!button) {
            return;
        }
        button.disabled = isSubmitting;
        button.innerHTML = isSubmitting
            ? '<span class="spinner-border spinner-border-sm me-2"></span>Seeding...'
            : '<i class="bi bi-database-add"></i> Seed Test Data';
    }
    function initializePage() {
        const page = document.getElementById('dashboard-summary-page');
        if (!page || page.dataset.initialized === 'true') {
            return;
        }
        page.dataset.initialized = 'true';
        bindQuoteActions();
        bindSeedDataAction();
        renderOngoingWorkflowPanel();
        loadSystemReadinessTodo();
        loadPendingSummary();
        loadSalesSummary();
    }
    function bindSeedDataAction() {
        const button = document.getElementById('dashboard-summary-seed-data-button');
        if (!button || button.dataset.bound === 'true') {
            return;
        }
        button.dataset.bound = 'true';
        button.addEventListener('click', async () => {
            clearError();
            setSeedDataSubmitting(true);
            const response = await apiRequest(`${getApiBaseUrl()}/Test/seed-data`, { method: 'POST' });
            if (!response.success) {
                showError(response.message || 'Failed to seed test data.');
                setSeedDataSubmitting(false);
                return;
            }
            await loadSystemReadinessTodo();
            setSeedDataSubmitting(false);
        });
    }
    async function loadSystemReadinessTodo() {
        const results = await Promise.all(setupTodoItems.map(async (item) => {
            if (item.checkType === 'static-not-created') {
                return {
                    ...item,
                    done: false,
                    detail: 'Not available yet (feature not created).',
                };
            }
            const endpoint = item.endpoint ?? '';
            const response = await apiRequest(`${getApiBaseUrl()}${endpoint}`, { method: 'GET' });
            if (!response.success) {
                return {
                    ...item,
                    done: false,
                    detail: response.message || 'Could not verify this item.',
                };
            }
            if (item.checkType === 'company') {
                const map = (response.data ?? {});
                const hasCompany = Number(map.id ?? map.Id ?? 0) > 0 || String(map.name ?? map.Name ?? '').trim().length > 0;
                return {
                    ...item,
                    done: hasCompany,
                    detail: hasCompany ? 'Company profile exists.' : 'Company profile is missing.',
                };
            }
            const count = extractItems(response.data).items.length;
            return {
                ...item,
                done: count > 0,
                detail: count > 0 ? `${count} item(s) found.` : 'No entries found yet.',
            };
        }));
        renderSystemReadinessTodo(results);
    }
    function registerGlobalReadinessRefresh() {
        const root = window;
        root.DashboardSummary = root.DashboardSummary ?? {};
        root.DashboardSummary.refreshReadiness = async () => {
            await loadSystemReadinessTodo();
        };
    }
    function renderSystemReadinessTodo(items) {
        const tbody = document.getElementById('dashboard-summary-readiness-body');
        if (!tbody) {
            return;
        }
        const order = new Map();
        setupTodoItems.forEach((item, index) => {
            order.set(item.key, index);
        });
        const sortedItems = [...items].sort((a, b) => {
            if (a.done !== b.done) {
                return Number(a.done) - Number(b.done);
            }
            return (order.get(a.key) ?? 0) - (order.get(b.key) ?? 0);
        });
        const completed = items.filter((item) => item.done).length;
        setText('dashboard-summary-readiness-progress', `${completed} / ${items.length} completed`);
        const doneMap = new Map();
        const nameMap = new Map();
        items.forEach((item) => {
            doneMap.set(item.key, item.done);
            nameMap.set(item.key, item.name);
        });
        tbody.innerHTML = sortedItems.map((item) => {
            const missingDependencies = (item.dependsOn ?? []).filter((dependency) => doneMap.get(dependency) !== true);
            const isBlocked = !item.done && missingDependencies.length > 0;
            const badge = item.done
                ? '<span class="badge bg-success">Done</span>'
                : isBlocked
                    ? '<span class="badge bg-secondary">Waiting</span>'
                    : '<span class="badge bg-warning text-dark">To do</span>';
            const actionText = item.done ? 'Viev data' : 'Fix now';
            const dependencyNames = missingDependencies.map((key) => nameMap.get(key) ?? key);
            const dependencyNote = isBlocked
                ? `<div class="text-muted small mt-1">Waiting for: ${esc(dependencyNames.join(', '))}</div>`
                : '';
            const actionHtml = isBlocked
                ? '<button class="btn btn-sm btn-outline-secondary" type="button" disabled>Fix now</button>'
                : `<a class="btn btn-sm btn-outline-primary" href="${esc(item.fixUrl)}">${actionText}</a>`;
            return `
        <tr>
            <td>${badge}</td>
            <td>
                <div class="fw-semibold">${esc(item.name)}</div>
                <div class="text-muted small">${esc(item.detail)}</div>
                ${dependencyNote}
            </td>
            <td>${esc(item.description)}</td>
            <td class="text-end">
                ${actionHtml}
            </td>
        </tr>
        `;
        }).join('');
    }
    function updateSalesCardLayout(quoteCount, orderCount, invoiceCount) {
        const cards = [
            { id: 'dashboard-summary-quotes-card-wrap', count: quoteCount },
            { id: 'dashboard-summary-orders-card-wrap', count: orderCount },
            { id: 'dashboard-summary-open-invoices-card-wrap', count: invoiceCount },
        ];
        const filled = cards.filter((card) => card.count > 0);
        const empty = cards.filter((card) => card.count <= 0);
        filled.forEach((card, index) => {
            const element = document.getElementById(card.id);
            if (!element) {
                return;
            }
            element.className = 'col-12';
            element.style.order = String(index + 1);
        });
        empty.forEach((card, index) => {
            const element = document.getElementById(card.id);
            if (!element) {
                return;
            }
            element.className = 'col-12 col-xl-4';
            element.style.order = String(filled.length + index + 1);
        });
    }
    function renderOngoingWorkflowPanel() {
        const card = document.getElementById('dashboard-summary-workflow-card');
        const draftLink = document.getElementById('dashboard-summary-workflow-link');
        if (!card) {
            return;
        }
        const state = loadNewSaleState();
        const domainName = state?.domainName?.trim() ?? '';
        const customer = state?.selectedCustomer;
        const showOngoingCard = state?.showOngoingCard === true;
        const hideWorkflowPanel = state?.isOfferListContext === true || !showOngoingCard;
        if (!domainName || hideWorkflowPanel) {
            hasOngoingWorkflowWarning = false;
            card.classList.add('d-none');
            return;
        }
        const customerId = Number(customer?.id ?? 0);
        const customerName = customer?.name?.trim() || customer?.customerName?.trim() || (customerId > 0 ? `#${customerId}` : '-');
        setText('dashboard-summary-workflow-domain', domainName);
        setText('dashboard-summary-workflow-customer', customerName);
        setText('dashboard-summary-workflow-status', state?.offer?.status ?? 'Draft');
        if (draftLink) {
            draftLink.classList.remove('d-none');
        }
        hasOngoingWorkflowWarning = true;
        card.classList.remove('d-none');
    }
    async function loadSalesSummary() {
        const response = await apiRequest(`${getApiBaseUrl()}/System/sales-summary`, { method: 'GET' });
        if (!response.success || !response.data) {
            renderSummaryTable('dashboard-summary-offers-body', [], 'Could not load quotes');
            renderSummaryTable('dashboard-summary-orders-body', [], 'Could not load orders');
            renderSummaryTable('dashboard-summary-open-invoices-body', [], 'Could not load open invoices');
            setText('dashboard-summary-offers-count', '0');
            setText('dashboard-summary-orders-count', '0');
            setText('dashboard-summary-open-invoices-count', '0');
            updateSalesCardLayout(0, 0, 0);
            return;
        }
        const offers = extractItems(response.data?.offers).items
            .map(normalizeQuote)
            .filter((item) => item.status.toLowerCase() !== 'converted')
            .slice(0, 8);
        const orders = extractItems(response.data?.orders).items
            .map(normalizeOrder)
            .slice(0, 8);
        const ordersWithAcceptedDraft = orders;
        const openInvoices = extractItems(response.data?.openInvoices).items
            .map(normalizeInvoice)
            .slice(0, 8);
        setText('dashboard-summary-offers-count', String(offers.length));
        setText('dashboard-summary-orders-count', String(ordersWithAcceptedDraft.length));
        setText('dashboard-summary-open-invoices-count', String(openInvoices.length));
        updateSalesCardLayout(offers.length, ordersWithAcceptedDraft.length, openInvoices.length);
        renderQuoteTable(offers);
        renderSummaryTable('dashboard-summary-orders-body', ordersWithAcceptedDraft.map((item) => ({
            identifier: item.orderNumber || `#${item.id}`,
            status: item.status,
            amount: formatMoney(item.totalAmount, item.currencyCode),
        })), 'No orders found');
        renderSummaryTable('dashboard-summary-open-invoices-body', openInvoices.map((item) => ({
            identifier: item.invoiceNumber || `#${item.id}`,
            status: item.status,
            amount: formatMoney(item.totalAmount, item.currencyCode),
        })), 'No open invoices found');
    }
    async function loadOffersSummary() {
        const response = await apiRequest(`${getApiBaseUrl()}/Quotes`, { method: 'GET' });
        if (!response.success) {
            renderSummaryTable('dashboard-summary-offers-body', [], 'Could not load quotes');
            setText('dashboard-summary-offers-count', '0');
            return;
        }
        const rawItems = extractItems(response.data).items;
        const offers = rawItems
            .map(normalizeQuote)
            .filter((item) => item.status.toLowerCase() !== 'converted')
            .sort((a, b) => b.id - a.id)
            .slice(0, 8);
        setText('dashboard-summary-offers-count', String(offers.length));
        renderSummaryTable('dashboard-summary-offers-body', offers.map((item) => ({
            identifier: item.quoteNumber || `#${item.id}`,
            status: item.status,
            amount: formatMoney(item.totalAmount, item.currencyCode),
        })), 'No quotes found');
    }
    async function loadOrdersSummary() {
        const response = await apiRequest(`${getApiBaseUrl()}/Orders`, { method: 'GET' });
        if (!response.success) {
            renderSummaryTable('dashboard-summary-orders-body', [], 'Could not load orders');
            setText('dashboard-summary-orders-count', '0');
            return;
        }
        const rawItems = extractItems(response.data).items;
        const orders = rawItems
            .map(normalizeOrder)
            .sort((a, b) => b.id - a.id)
            .slice(0, 8);
        setText('dashboard-summary-orders-count', String(orders.length));
        renderSummaryTable('dashboard-summary-orders-body', orders.map((item) => ({
            identifier: item.orderNumber || `#${item.id}`,
            status: item.status,
            amount: formatMoney(item.totalAmount, item.currencyCode),
        })), 'No orders found');
    }
    async function loadOpenInvoicesSummary() {
        const [issuedResponse, overdueResponse, draftResponse] = await Promise.all([
            apiRequest(`${getApiBaseUrl()}/Invoices/status/Issued`, { method: 'GET' }),
            apiRequest(`${getApiBaseUrl()}/Invoices/status/Overdue`, { method: 'GET' }),
            apiRequest(`${getApiBaseUrl()}/Invoices/status/Draft`, { method: 'GET' }),
        ]);
        const responses = [issuedResponse, overdueResponse, draftResponse].filter((res) => res.success);
        const failedAll = responses.length === 0;
        if (failedAll) {
            renderSummaryTable('dashboard-summary-open-invoices-body', [], 'Could not load open invoices');
            setText('dashboard-summary-open-invoices-count', '0');
            return;
        }
        const allRawInvoices = responses.flatMap((res) => extractItems(res.data).items);
        const unique = new Map();
        allRawInvoices.map(normalizeInvoice).forEach((item) => {
            unique.set(item.id, item);
        });
        const openInvoices = Array.from(unique.values())
            .sort((a, b) => b.id - a.id)
            .slice(0, 8);
        setText('dashboard-summary-open-invoices-count', String(openInvoices.length));
        renderSummaryTable('dashboard-summary-open-invoices-body', openInvoices.map((item) => ({
            identifier: item.invoiceNumber || `#${item.id}`,
            status: item.status,
            amount: formatMoney(item.totalAmount, item.currencyCode),
        })), 'No open invoices found');
    }
    function loadNewSaleState() {
        const raw = sessionStorage.getItem('new-sale-state');
        if (!raw) {
            return null;
        }
        try {
            return JSON.parse(raw);
        }
        catch {
            return null;
        }
    }
    function normalizeQuote(raw) {
        return {
            id: Number(raw.id ?? raw.Id ?? 0),
            quoteNumber: String(raw.quoteNumber ?? raw.QuoteNumber ?? ''),
            domainName: String(raw.domainName ?? raw.DomainName ?? ''),
            customerName: String(raw.customerName ?? raw.CustomerName ?? ''),
            createdAt: String(raw.createdAt ?? raw.CreatedAt ?? ''),
            status: resolveQuoteStatus(raw.status ?? raw.Status),
            totalAmount: Number(raw.totalAmount ?? raw.TotalAmount ?? 0),
            currencyCode: String(raw.currencyCode ?? raw.CurrencyCode ?? 'USD'),
        };
    }
    function extractDomainName(value) {
        const text = (value || '').trim();
        if (!text) {
            return '-';
        }
        const match = text.match(/[a-z0-9][a-z0-9\-]*\.[a-z0-9][a-z0-9\-.]*/i);
        return (match ? match[0] : text).toLowerCase();
    }
    function formatDate(value) {
        const parsed = new Date(value);
        if (!value || Number.isNaN(parsed.getTime())) {
            return '-';
        }
        return parsed.toLocaleDateString();
    }
    function renderQuoteTable(rows) {
        const body = document.getElementById('dashboard-summary-offers-body');
        if (!body) {
            return;
        }
        if (!rows.length) {
            body.innerHTML = '<tr><td colspan="6" class="text-center text-muted">No quotes found</td></tr>';
            return;
        }
        body.innerHTML = rows.map((item) => {
            const quoteNumber = item.quoteNumber || `#${item.id}`;
            const domainName = extractDomainName(item.domainName);
            const customerName = item.customerName || '-';
            const dateText = formatDate(item.createdAt);
            return `
        <tr>
            <td><a href="/dashboard/quote/offer?quoteId=${encodeURIComponent(String(item.id))}">${esc(quoteNumber)}</a></td>
            <td>${esc(domainName)}</td>
            <td>${esc(customerName)}</td>
            <td>${esc(dateText)}</td>
            <td>${esc(item.status)}</td>
            <td class="text-end">
                <button class="btn btn-sm btn-primary" type="button" data-action="accept-quote" data-id="${item.id}" data-quote-number="${esc(quoteNumber)}">Accept</button>
            </td>
        </tr>
    `;
        }).join('');
    }
    function openAcceptQuoteModal(quoteId, quoteNumber) {
        pendingAcceptQuoteId = quoteId;
        pendingAcceptQuoteNumber = quoteNumber;
        setText('dashboard-summary-accept-quote-number', quoteNumber || `#${quoteId}`);
        const modalElement = document.getElementById('dashboard-summary-accept-quote-modal');
        const bootstrap = getBootstrap();
        if (!modalElement || !bootstrap?.Modal) {
            return;
        }
        const modal = bootstrap.Modal.getInstance(modalElement) || new bootstrap.Modal(modalElement);
        modal.show();
    }
    function closeAcceptQuoteModal() {
        const modalElement = document.getElementById('dashboard-summary-accept-quote-modal');
        const bootstrap = getBootstrap();
        if (!modalElement || !bootstrap?.Modal) {
            return;
        }
        const modal = bootstrap.Modal.getInstance(modalElement);
        modal?.hide();
    }
    async function acceptQuoteFromDashboard() {
        if (!pendingAcceptQuoteId) {
            return;
        }
        clearError();
        const response = await apiRequest(`${getApiBaseUrl()}/Quotes/${pendingAcceptQuoteId}/convert`, { method: 'POST' });
        if (!response.success) {
            showError(response.message || `Could not accept quote ${pendingAcceptQuoteNumber || `#${pendingAcceptQuoteId}`}.`);
            return;
        }
        closeAcceptQuoteModal();
        pendingAcceptQuoteId = null;
        pendingAcceptQuoteNumber = '';
        await loadSalesSummary();
    }
    function bindQuoteActions() {
        const tableBody = document.getElementById('dashboard-summary-offers-body');
        tableBody?.addEventListener('click', (event) => {
            const target = event.target;
            const button = target.closest('button[data-action="accept-quote"]');
            if (!button) {
                return;
            }
            const quoteId = Number(button.dataset.id ?? 0);
            if (!quoteId) {
                return;
            }
            const quoteNumber = button.dataset.quoteNumber || `#${quoteId}`;
            openAcceptQuoteModal(quoteId, quoteNumber);
        });
        document.getElementById('dashboard-summary-accept-quote-confirm')?.addEventListener('click', () => {
            void acceptQuoteFromDashboard();
        });
    }
    function normalizeOrder(raw) {
        return {
            id: Number(raw.id ?? raw.Id ?? 0),
            orderNumber: String(raw.orderNumber ?? raw.OrderNumber ?? ''),
            status: resolveOrderStatus(raw.status ?? raw.Status),
            totalAmount: Number(raw.totalAmount ?? raw.TotalAmount ?? 0),
            currencyCode: String(raw.currencyCode ?? raw.CurrencyCode ?? 'USD'),
            quoteId: Number(raw.quoteId ?? raw.QuoteId ?? 0) || undefined,
        };
    }
    function normalizeInvoice(raw) {
        return {
            id: Number(raw.id ?? raw.Id ?? 0),
            invoiceNumber: String(raw.invoiceNumber ?? raw.InvoiceNumber ?? ''),
            status: resolveInvoiceStatus(raw.status ?? raw.Status),
            totalAmount: Number(raw.totalAmount ?? raw.TotalAmount ?? 0),
            currencyCode: String(raw.currencyCode ?? raw.CurrencyCode ?? 'USD'),
        };
    }
    function resolveQuoteStatus(status) {
        const value = Number(status);
        switch (value) {
            case 0: return 'Draft';
            case 1: return 'Sent';
            case 2: return 'Accepted';
            case 3: return 'Rejected';
            case 4: return 'Expired';
            case 5: return 'Converted';
            default: return String(status ?? '-');
        }
    }
    function resolveOrderStatus(status) {
        const value = Number(status);
        switch (value) {
            case 0: return 'Pending';
            case 1: return 'Active';
            case 2: return 'Suspended';
            case 3: return 'Cancelled';
            case 4: return 'Expired';
            case 5: return 'Trial';
            default: return String(status ?? '-');
        }
    }
    function resolveInvoiceStatus(status) {
        const value = Number(status);
        switch (value) {
            case 0: return 'Draft';
            case 1: return 'Issued';
            case 2: return 'Paid';
            case 3: return 'Overdue';
            case 4: return 'Cancelled';
            case 5: return 'Credited';
            default: return String(status ?? '-');
        }
    }
    function formatMoney(amount, currency) {
        const normalizedAmount = Number.isFinite(amount) ? amount : 0;
        return `${normalizedAmount.toFixed(2)} ${currency || 'USD'}`;
    }
    function renderSummaryTable(bodyId, rows, emptyMessage) {
        const body = document.getElementById(bodyId);
        if (!body) {
            return;
        }
        if (!rows.length) {
            body.innerHTML = `<tr><td colspan="3" class="text-center text-muted">${esc(emptyMessage)}</td></tr>`;
            return;
        }
        body.innerHTML = rows.map((row) => {
            const identifierHtml = row.linkUrl
                ? `<a href="${esc(row.linkUrl)}"${row.openInNewTab ? ' target="_blank" rel="noopener noreferrer"' : ''}>${esc(row.identifier)}</a>`
                : esc(row.identifier);
            return `
        <tr>
            <td>${identifierHtml}</td>
            <td>${esc(row.status)}</td>
            <td class="text-end">${esc(row.amount)}</td>
        </tr>
    `;
        }).join('');
    }
    async function loadPendingSummary() {
        clearError();
        setAllClearVisible(false);
        setPendingLoading(true);
        try {
            const domains = await loadAllDomains();
            const pendingResults = await Promise.all(domains.map(async (domain) => {
                const response = await apiRequest(`${getApiBaseUrl()}/DnsRecords/domain/${domain.id}/pending-sync`, { method: 'GET' });
                if (!response.success) {
                    return { domain, count: null, error: response.message };
                }
                const records = Array.isArray(response.data) ? response.data : [];
                return { domain, count: records.length, error: null };
            }));
            const pending = pendingResults
                .filter((item) => item.count !== null && item.count > 0)
                .sort((a, b) => (b.count ?? 0) - (a.count ?? 0));
            renderPendingTable(pending);
            setPendingCardVisible(pending.length > 0);
            setAllClearVisible(pending.length === 0 && !hasOngoingWorkflowWarning);
            if (!pending.length) {
                setText('dashboard-summary-pending-note', 'No domains have pending DNS records.');
            }
            else {
                setText('dashboard-summary-pending-note', `${pending.length} domain(s) require registrar sync.`);
            }
        }
        catch (error) {
            setPendingCardVisible(true);
            setAllClearVisible(false);
            showError(error?.message || 'Failed to load pending DNS records.');
            renderPendingTable([]);
            setText('dashboard-summary-pending-note', 'Unable to load pending DNS records.');
        }
        finally {
            setPendingLoading(false);
        }
    }
    async function loadAllDomains() {
        let allItems = [];
        let pageNumber = 1;
        const pageSize = 200;
        let totalPages = 1;
        while (pageNumber <= totalPages) {
            const params = new URLSearchParams();
            params.set('pageNumber', String(pageNumber));
            params.set('pageSize', String(pageSize));
            const response = await apiRequest(`${getApiBaseUrl()}/RegisteredDomains?${params.toString()}`, { method: 'GET' });
            if (!response.success) {
                throw new Error(response.message || 'Failed to load domains');
            }
            const raw = response.data;
            const extracted = extractItems(raw);
            const meta = extracted.meta ?? raw;
            const items = extracted.items.map(normalizeDomain);
            allItems = allItems.concat(items);
            totalPages = meta?.totalPages ?? meta?.TotalPages ?? raw?.totalPages ?? raw?.TotalPages ?? totalPages;
            pageNumber += 1;
            if (!extracted.items.length) {
                break;
            }
        }
        return allItems;
    }
    function extractItems(raw) {
        if (Array.isArray(raw)) {
            return { items: raw, meta: null };
        }
        const candidates = [raw, raw?.data, raw?.Data, raw?.data?.data, raw?.data?.Data];
        const items = (Array.isArray(raw?.Data) && raw.Data) ||
            (Array.isArray(raw?.data) && raw.data) ||
            (Array.isArray(raw?.data?.Data) && raw.data.Data) ||
            (Array.isArray(raw?.data?.data) && raw.data.data) ||
            (Array.isArray(raw?.Data?.Data) && raw.Data.Data) ||
            [];
        const meta = candidates.find((c) => c && typeof c === 'object' && (c.totalCount !== undefined || c.TotalCount !== undefined ||
            c.totalPages !== undefined || c.TotalPages !== undefined ||
            c.currentPage !== undefined || c.CurrentPage !== undefined ||
            c.pageSize !== undefined || c.PageSize !== undefined));
        return { items, meta };
    }
    function normalizeDomain(raw) {
        return {
            id: raw.id ?? raw.Id ?? 0,
            name: raw.name ?? raw.Name ?? raw.domainName ?? '',
        };
    }
    function setPendingLoading(isLoading) {
        const loading = document.getElementById('dashboard-summary-pending-loading');
        if (loading) {
            loading.classList.toggle('d-none', !isLoading);
        }
    }
    function setPendingCardVisible(isVisible) {
        const card = document.getElementById('dashboard-summary-pending-card');
        if (card) {
            card.classList.toggle('d-none', !isVisible);
        }
    }
    function setAllClearVisible(isVisible) {
        const card = document.getElementById('dashboard-summary-all-clear-card');
        const readinessCard = document.getElementById('dashboard-summary-readiness-card');
        const shouldShow = readinessCard ? false : isVisible;
        if (card) {
            card.classList.toggle('d-none', !shouldShow);
        }
    }
    function renderPendingTable(rows) {
        const tbody = document.getElementById('dashboard-summary-pending-table');
        if (!tbody) {
            return;
        }
        if (!rows.length) {
            tbody.innerHTML = '';
            return;
        }
        tbody.innerHTML = rows.map((row) => `
        <tr>
            <td><code>${esc(row.domain.name || `Domain #${row.domain.id}`)}</code></td>
            <td class="text-end"><span class="fw-semibold">${row.count ?? '-'}</span></td>
        </tr>
    `).join('');
    }
    function setText(id, value) {
        const element = document.getElementById(id);
        if (element) {
            element.textContent = value;
        }
    }
    function showError(message) {
        const alert = document.getElementById('dashboard-summary-alert-error');
        if (!alert) {
            return;
        }
        alert.textContent = message;
        alert.classList.remove('d-none');
    }
    function clearError() {
        const alert = document.getElementById('dashboard-summary-alert-error');
        if (alert) {
            alert.textContent = '';
            alert.classList.add('d-none');
        }
    }
    function esc(value) {
        return value.replace(/[&<>"]/g, (match) => {
            switch (match) {
                case '&':
                    return '&amp;';
                case '<':
                    return '&lt;';
                case '>':
                    return '&gt;';
                case '"':
                    return '&quot;';
                default:
                    return match;
            }
        });
    }
    function setupPageObserver() {
        initializePage();
        if (document.body) {
            const observer = new MutationObserver(() => {
                const page = document.getElementById('dashboard-summary-page');
                if (page && page.dataset.initialized !== 'true') {
                    initializePage();
                }
            });
            observer.observe(document.body, { childList: true, subtree: true });
        }
    }
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', setupPageObserver);
    }
    else {
        setupPageObserver();
    }
    registerGlobalReadinessRefresh();
})();
//# sourceMappingURL=dashboard-summary.js.map
(() => {
    interface AppSettings {
        apiBaseUrl?: string;
    }

    interface ApiResponse<T> {
        success: boolean;
        data?: T;
        message?: string;
    }

    interface ApiErrorBody {
        message?: string;
        title?: string;
    }

    interface ApiEnvelope<T> {
        success?: boolean;
        data?: T;
        message?: string;
    }

    interface Domain {
        id: number;
        name: string;
    }

    interface DnsTroubleshootResult {
        key: string;
        name: string;
        severity: string;
        passed: boolean;
        message: string;
        details?: string | null;
        fixUrl?: string | null;
    }

    interface DnsTroubleshootReport {
        domainId: number;
        domainName: string;
        generatedAtUtc: string;
        tests: DnsTroubleshootResult[];
    }

    let selectedDomainId: number | null = null;
    let selectedDomainName: string | null = null;

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
            console.error('DNS troubleshoot request failed', error);
            return {
                success: false,
                message: 'Network error. Please try again.',
            };
        }
    };

    const showError = (message: string): void => {
        const alert = document.getElementById('dns-troubleshoot-alert-error');
        if (!alert) {
            return;
        }

        alert.textContent = message;
        alert.classList.remove('d-none');
        document.getElementById('dns-troubleshoot-alert-success')?.classList.add('d-none');
    };

    const hideError = (): void => {
        document.getElementById('dns-troubleshoot-alert-error')?.classList.add('d-none');
    };

    const setText = (id: string, value: string): void => {
        const element = document.getElementById(id);
        if (element) {
            element.textContent = value;
        }
    };

    const showModal = (id: string): void => {
        const element = document.getElementById(id);
        const bootstrap = (window as Window & { bootstrap?: any }).bootstrap;
        if (!element || !bootstrap) {
            return;
        }

        const modal = new bootstrap.Modal(element);
        modal.show();
    };

    const hideModal = (id: string): void => {
        const element = document.getElementById(id);
        const bootstrap = (window as Window & { bootstrap?: any }).bootstrap;
        if (!element || !bootstrap) {
            return;
        }

        const modal = bootstrap.Modal.getInstance(element);
        modal?.hide();
    };

    const setLoading = (isLoading: boolean): void => {
        document.getElementById('dns-troubleshoot-loading')?.classList.toggle('d-none', !isLoading);
        document.getElementById('dns-troubleshoot-table-wrap')?.classList.toggle('d-none', isLoading);
    };

    const showNoSelection = (): void => {
        document.getElementById('dns-troubleshoot-no-selection')?.classList.remove('d-none');
        document.getElementById('dns-troubleshoot-table-wrap')?.classList.add('d-none');
        document.getElementById('dns-troubleshoot-empty')?.classList.add('d-none');
        setText('dns-troubleshoot-generated-at', '-');
        setText('dns-troubleshoot-passed-count', '0 passed');
        setText('dns-troubleshoot-warning-count', '0 warnings');
        setText('dns-troubleshoot-error-count', '0 errors');
    };

    const showTable = (): void => {
        document.getElementById('dns-troubleshoot-no-selection')?.classList.add('d-none');
        document.getElementById('dns-troubleshoot-table-wrap')?.classList.remove('d-none');
        document.getElementById('dns-troubleshoot-empty')?.classList.add('d-none');
    };

    const updateSelectedDomainDisplay = (): void => {
        setText('dns-troubleshoot-domain-name', selectedDomainName || 'No domain selected');
        setText('dns-troubleshoot-domain-id', selectedDomainId ? String(selectedDomainId) : '-');

        const runButton = document.getElementById('dns-troubleshoot-run') as HTMLButtonElement | null;
        if (runButton) {
            runButton.disabled = !selectedDomainId;
        }
    };

    const renderResults = (report: DnsTroubleshootReport): void => {
        const tableBody = document.getElementById('dns-troubleshoot-table-body');
        if (!tableBody) {
            return;
        }

        const tests = Array.isArray(report.tests) ? report.tests : [];
        if (!tests.length) {
            tableBody.innerHTML = '';
            document.getElementById('dns-troubleshoot-empty')?.classList.remove('d-none');
            document.getElementById('dns-troubleshoot-table-wrap')?.classList.add('d-none');
            return;
        }

        const warningCount = tests.filter((x) => !x.passed && x.severity.toLowerCase() === 'warning').length;
        const errorCount = tests.filter((x) => !x.passed && x.severity.toLowerCase() === 'error').length;
        const passedCount = tests.filter((x) => x.passed).length;

        setText('dns-troubleshoot-passed-count', `${passedCount} passed`);
        setText('dns-troubleshoot-warning-count', `${warningCount} warnings`);
        setText('dns-troubleshoot-error-count', `${errorCount} errors`);
        setText('dns-troubleshoot-generated-at', report.generatedAtUtc ? new Date(report.generatedAtUtc).toLocaleString() : '-');

        tableBody.innerHTML = tests.map((test) => {
            const severity = (test.severity || 'Info').toLowerCase();
            const statusBadge = test.passed
                ? '<span class="badge bg-success">Passed</span>'
                : severity === 'error'
                    ? '<span class="badge bg-danger">Error</span>'
                    : severity === 'warning'
                        ? '<span class="badge bg-warning text-dark">Warning</span>'
                        : '<span class="badge bg-secondary">Info</span>';

            const action = test.fixUrl
                ? `<a class="btn btn-sm btn-outline-primary" href="${esc(test.fixUrl)}">Fix now</a>`
                : '-';

            return `
                <tr>
                    <td>${statusBadge}</td>
                    <td>${esc(test.name || test.key || '-')}</td>
                    <td>${esc(test.message || '-')}</td>
                    <td>${esc(test.details || '-')}</td>
                    <td class="text-end">${action}</td>
                </tr>
            `;
        }).join('');

        showTable();
    };

    const runTests = async (): Promise<void> => {
        if (!selectedDomainId) {
            showError('Select a domain first.');
            return;
        }

        hideError();
        setLoading(true);

        const response = await apiRequest<DnsTroubleshootReport>(`${getApiBaseUrl()}/DnsTroubleshoot/domain/${selectedDomainId}`, { method: 'GET' });
        setLoading(false);

        if (!response.success || !response.data) {
            showError(response.message || 'Failed to run DNS troubleshoot tests.');
            return;
        }

        renderResults(response.data);
    };

    const loadDomainFromInput = async (): Promise<void> => {
        const input = document.getElementById('dns-troubleshoot-domain-input') as HTMLInputElement | null;
        const domainName = (input?.value ?? '').trim();

        if (!domainName) {
            showError('Enter a domain name to load.');
            return;
        }

        hideError();
        const response = await apiRequest<Domain>(`${getApiBaseUrl()}/RegisteredDomains/name/${encodeURIComponent(domainName)}`, { method: 'GET' });
        if (!response.success || !response.data) {
            showError(response.message || 'Domain not found.');
            return;
        }

        const data = response.data as any;
        selectedDomainId = Number(data.id ?? data.Id ?? 0);
        selectedDomainName = String(data.name ?? data.Name ?? domainName);

        updateSelectedDomainDisplay();
        hideModal('dns-troubleshoot-select-modal');
        await runTests();
    };

    const loadDomainById = async (domainId: number): Promise<void> => {
        hideError();
        const response = await apiRequest<Domain>(`${getApiBaseUrl()}/RegisteredDomains/${domainId}`, { method: 'GET' });
        if (!response.success || !response.data) {
            showError(response.message || 'Domain not found.');
            return;
        }

        const data = response.data as any;
        selectedDomainId = Number(data.id ?? data.Id ?? 0);
        selectedDomainName = String(data.name ?? data.Name ?? `#${domainId}`);

        updateSelectedDomainDisplay();
        await runTests();
    };

    const bindEvents = (): void => {
        document.getElementById('dns-troubleshoot-select-domain')?.addEventListener('click', () => {
            showModal('dns-troubleshoot-select-modal');
        });

        document.getElementById('dns-troubleshoot-domain-load')?.addEventListener('click', () => {
            void loadDomainFromInput();
        });

        document.getElementById('dns-troubleshoot-run')?.addEventListener('click', () => {
            void runTests();
        });

        const input = document.getElementById('dns-troubleshoot-domain-input') as HTMLInputElement | null;
        input?.addEventListener('keydown', (event) => {
            if (event.key === 'Enter') {
                event.preventDefault();
                void loadDomainFromInput();
            }
        });
    };

    const initializePage = (): void => {
        const page = document.getElementById('dns-troubleshoot-page') as HTMLElement | null;
        if (!page || page.dataset.initialized === 'true') {
            return;
        }

        page.dataset.initialized = 'true';
        bindEvents();
        updateSelectedDomainDisplay();
        showNoSelection();

        const params = new URLSearchParams(window.location.search);
        const domainIdParam = params.get('domain-id');
        const parsedDomainId = Number(domainIdParam ?? '0');
        if (domainIdParam && Number.isFinite(parsedDomainId) && parsedDomainId > 0) {
            void loadDomainById(parsedDomainId);
        }
    };

    const setupObserver = (): void => {
        initializePage();

        if (document.body) {
            const observer = new MutationObserver(() => {
                const page = document.getElementById('dns-troubleshoot-page') as HTMLElement | null;
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

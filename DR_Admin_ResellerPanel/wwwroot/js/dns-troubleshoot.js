"use strict";
(() => {
    let selectedDomainId = null;
    let selectedDomainName = null;
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
        catch (error) {
            console.error('DNS troubleshoot request failed', error);
            return {
                success: false,
                message: 'Network error. Please try again.',
            };
        }
    };
    const showError = (message) => {
        const alert = document.getElementById('dns-troubleshoot-alert-error');
        if (!alert) {
            return;
        }
        alert.textContent = message;
        alert.classList.remove('d-none');
        document.getElementById('dns-troubleshoot-alert-success')?.classList.add('d-none');
    };
    const hideError = () => {
        document.getElementById('dns-troubleshoot-alert-error')?.classList.add('d-none');
    };
    const setText = (id, value) => {
        const element = document.getElementById(id);
        if (element) {
            element.textContent = value;
        }
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
    const setLoading = (isLoading) => {
        document.getElementById('dns-troubleshoot-loading')?.classList.toggle('d-none', !isLoading);
        document.getElementById('dns-troubleshoot-table-wrap')?.classList.toggle('d-none', isLoading);
    };
    const showNoSelection = () => {
        document.getElementById('dns-troubleshoot-no-selection')?.classList.remove('d-none');
        document.getElementById('dns-troubleshoot-table-wrap')?.classList.add('d-none');
        document.getElementById('dns-troubleshoot-empty')?.classList.add('d-none');
        setText('dns-troubleshoot-generated-at', '-');
        setText('dns-troubleshoot-passed-count', '0 passed');
        setText('dns-troubleshoot-warning-count', '0 warnings');
        setText('dns-troubleshoot-error-count', '0 errors');
    };
    const showTable = () => {
        document.getElementById('dns-troubleshoot-no-selection')?.classList.add('d-none');
        document.getElementById('dns-troubleshoot-table-wrap')?.classList.remove('d-none');
        document.getElementById('dns-troubleshoot-empty')?.classList.add('d-none');
    };
    const updateSelectedDomainDisplay = () => {
        setText('dns-troubleshoot-domain-name', selectedDomainName || 'No domain selected');
        setText('dns-troubleshoot-domain-id', selectedDomainId ? String(selectedDomainId) : '-');
        const runButton = document.getElementById('dns-troubleshoot-run');
        if (runButton) {
            runButton.disabled = !selectedDomainId;
        }
    };
    const renderResults = (report) => {
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
    const runTests = async () => {
        if (!selectedDomainId) {
            showError('Select a domain first.');
            return;
        }
        hideError();
        setLoading(true);
        const response = await apiRequest(`${getApiBaseUrl()}/DnsTroubleshoot/domain/${selectedDomainId}`, { method: 'GET' });
        setLoading(false);
        if (!response.success || !response.data) {
            showError(response.message || 'Failed to run DNS troubleshoot tests.');
            return;
        }
        renderResults(response.data);
    };
    const loadDomainFromInput = async () => {
        const input = document.getElementById('dns-troubleshoot-domain-input');
        const domainName = (input?.value ?? '').trim();
        if (!domainName) {
            showError('Enter a domain name to load.');
            return;
        }
        hideError();
        const response = await apiRequest(`${getApiBaseUrl()}/RegisteredDomains/name/${encodeURIComponent(domainName)}`, { method: 'GET' });
        if (!response.success || !response.data) {
            showError(response.message || 'Domain not found.');
            return;
        }
        const data = response.data;
        selectedDomainId = Number(data.id ?? data.Id ?? 0);
        selectedDomainName = String(data.name ?? data.Name ?? domainName);
        updateSelectedDomainDisplay();
        hideModal('dns-troubleshoot-select-modal');
        await runTests();
    };
    const loadDomainById = async (domainId) => {
        hideError();
        const response = await apiRequest(`${getApiBaseUrl()}/RegisteredDomains/${domainId}`, { method: 'GET' });
        if (!response.success || !response.data) {
            showError(response.message || 'Domain not found.');
            return;
        }
        const data = response.data;
        selectedDomainId = Number(data.id ?? data.Id ?? 0);
        selectedDomainName = String(data.name ?? data.Name ?? `#${domainId}`);
        updateSelectedDomainDisplay();
        await runTests();
    };
    const bindEvents = () => {
        document.getElementById('dns-troubleshoot-select-domain')?.addEventListener('click', () => {
            showModal('dns-troubleshoot-select-modal');
        });
        document.getElementById('dns-troubleshoot-domain-load')?.addEventListener('click', () => {
            void loadDomainFromInput();
        });
        document.getElementById('dns-troubleshoot-run')?.addEventListener('click', () => {
            void runTests();
        });
        const input = document.getElementById('dns-troubleshoot-domain-input');
        input?.addEventListener('keydown', (event) => {
            if (event.key === 'Enter') {
                event.preventDefault();
                void loadDomainFromInput();
            }
        });
    };
    const initializePage = () => {
        const page = document.getElementById('dns-troubleshoot-page');
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
    const setupObserver = () => {
        initializePage();
        if (document.body) {
            const observer = new MutationObserver(() => {
                const page = document.getElementById('dns-troubleshoot-page');
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
//# sourceMappingURL=dns-troubleshoot.js.map
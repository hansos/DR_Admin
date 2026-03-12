"use strict";
// @ts-nocheck
(function () {
    let selectedDomainId = null;
    let selectedDomainName = null;
    let showAllDomains = false;
    let allNameServers = [];
    let filteredNameServers = [];
    let editingId = null;
    let pendingDeleteId = null;
    const domainLookup = new Map();
    let serverOptions = [];
    let sortColumn = 'domain';
    let sortDirection = 'asc';
    let filterHostname = '';
    let filterIp = '';
    let filterMode = 'all';
    let filterPrimary = 'all';
    let filterSort = '';
    let currentPage = 1;
    let pageSize = 25;
    let totalCount = 0;
    let totalPages = 1;
    function getApiBaseUrl() {
        return window.AppSettings?.apiBaseUrl ?? '';
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
            console.error('Nameservers request failed', error);
            return {
                success: false,
                message: 'Network error. Please try again.',
            };
        }
    }
    function initializePage() {
        const page = document.getElementById('dns-nameservers-page');
        if (!page || page.dataset.initialized === 'true') {
            return;
        }
        page.dataset.initialized = 'true';
        document.getElementById('dns-nameservers-add')?.addEventListener('click', openCreate);
        document.getElementById('dns-nameservers-save')?.addEventListener('click', saveNameServer);
        document.getElementById('dns-nameservers-confirm-delete')?.addEventListener('click', deleteNameServer);
        document.getElementById('dns-nameservers-mode-filter')?.addEventListener('change', applyModeFilter);
        document.getElementById('dns-nameservers-filter-hostname')?.addEventListener('input', applyModeFilter);
        document.getElementById('dns-nameservers-filter-ip')?.addEventListener('input', applyModeFilter);
        document.getElementById('dns-nameservers-filter-primary')?.addEventListener('change', applyModeFilter);
        document.getElementById('dns-nameservers-filter-sort')?.addEventListener('input', applyModeFilter);
        document.getElementById('dns-nameservers-server-id')?.addEventListener('change', () => {
            void applyServerDefaultIp();
        });
        document.getElementById('dns-nameservers-page-size')?.addEventListener('change', () => {
            loadPageSizeFromUi();
            currentPage = 1;
            renderNameServers();
        });
        bindTableActions();
        void ensureDomainLookupLoaded();
        void ensureServerOptionsLoaded();
        void showAllDomainNameServers();
    }
    function bindTableActions() {
        const tableBody = document.getElementById('dns-nameservers-table-body');
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
                openDelete(id);
                return;
            }
            if (button.dataset.action === 'domains') {
                openDomains(id);
            }
        });
        const tableHead = tableBody.closest('table')?.querySelector('thead');
        tableHead?.addEventListener('click', (event) => {
            const target = event.target;
            const button = target.closest('button[data-sort]');
            if (!button) {
                return;
            }
            const column = (button.dataset.sort ?? '');
            if (!column) {
                return;
            }
            if (sortColumn === column) {
                sortDirection = sortDirection === 'asc' ? 'desc' : 'asc';
            }
            else {
                sortColumn = column;
                sortDirection = 'asc';
            }
            currentPage = 1;
            renderNameServers();
        });
    }
    function normalizeDomain(raw) {
        return {
            id: raw.id ?? raw.Id ?? 0,
            name: raw.name ?? raw.Name ?? raw.domainName ?? '',
        };
    }
    function setSelectedDomain(domain) {
        showAllDomains = false;
        selectedDomainId = domain.id;
        selectedDomainName = domain.name || `Domain #${domain.id}`;
        setText('dns-nameservers-selected-domain', selectedDomainName);
        setText('dns-nameservers-selected-id', String(domain.id));
        const addButton = document.getElementById('dns-nameservers-add');
        if (addButton) {
            addButton.disabled = false;
        }
    }
    async function loadNameServers() {
        if (showAllDomains) {
            await loadAllNameServers();
            return;
        }
        if (!selectedDomainId) {
            showNoSelection();
            return;
        }
        setLoading(true);
        const response = await apiRequest(`${getApiBaseUrl()}/NameServers/domain/${selectedDomainId}`, { method: 'GET' });
        if (!response.success) {
            setLoading(false);
            showError(response.message || 'Failed to load nameservers.');
            return;
        }
        const raw = response.data;
        const items = Array.isArray(raw)
            ? raw
            : Array.isArray(raw?.data)
                ? raw.data
                : Array.isArray(raw?.Data)
                    ? raw.Data
                    : [];
        allNameServers = items.map(normalizeNameServer);
        applyModeFilter();
        setLoading(false);
    }
    async function loadAllNameServers() {
        setLoading(true);
        await ensureDomainLookupLoaded();
        const response = await apiRequest(`${getApiBaseUrl()}/NameServers?pageNumber=1&pageSize=2000`, { method: 'GET' });
        if (!response.success) {
            setLoading(false);
            showError(response.message || 'Failed to load nameservers.');
            return;
        }
        const raw = response.data;
        const items = Array.isArray(raw)
            ? raw
            : Array.isArray(raw?.data)
                ? raw.data
                : Array.isArray(raw?.Data)
                    ? raw.Data
                    : [];
        allNameServers = items.map(normalizeNameServer);
        applyModeFilter();
        setLoading(false);
    }
    async function showAllDomainNameServers() {
        showAllDomains = true;
        selectedDomainId = null;
        selectedDomainName = null;
        setText('dns-nameservers-selected-domain', 'All domains');
        setText('dns-nameservers-selected-id', '-');
        const addButton = document.getElementById('dns-nameservers-add');
        if (addButton) {
            addButton.disabled = false;
        }
        await loadNameServers();
    }
    async function ensureDomainLookupLoaded() {
        if (domainLookup.size > 0) {
            return;
        }
        const response = await apiRequest(`${getApiBaseUrl()}/RegisteredDomains?pageNumber=1&pageSize=2000`, { method: 'GET' });
        if (!response.success) {
            return;
        }
        const raw = response.data;
        const items = Array.isArray(raw)
            ? raw
            : Array.isArray(raw?.data)
                ? raw.data
                : Array.isArray(raw?.Data)
                    ? raw.Data
                    : Array.isArray(raw?.items)
                        ? raw.items
                        : Array.isArray(raw?.Items)
                            ? raw.Items
                            : Array.isArray(raw?.data?.items)
                                ? raw.data.items
                                : Array.isArray(raw?.data?.Items)
                                    ? raw.data.Items
                                    : [];
        items.forEach((item) => {
            const id = Number(item.id ?? item.Id ?? 0);
            if (!Number.isFinite(id) || id <= 0) {
                return;
            }
            const name = String(item.name ?? item.Name ?? item.domainName ?? item.DomainName ?? `Domain #${id}`);
            domainLookup.set(id, name);
        });
    }
    async function ensureServerOptionsLoaded() {
        if (serverOptions.length > 0) {
            return;
        }
        const response = await apiRequest(`${getApiBaseUrl()}/Servers`, { method: 'GET' });
        if (!response.success) {
            return;
        }
        const raw = response.data;
        const items = Array.isArray(raw)
            ? raw
            : Array.isArray(raw?.data)
                ? raw.data
                : Array.isArray(raw?.Data)
                    ? raw.Data
                    : [];
        serverOptions = items
            .map((item) => ({
            id: Number(item.id ?? item.Id ?? 0),
            name: String(item.name ?? item.Name ?? '').trim(),
        }))
            .filter((item) => Number.isFinite(item.id) && item.id > 0)
            .sort((a, b) => a.name.localeCompare(b.name));
    }
    function normalizeNameServer(item) {
        const domainIdsRaw = item.domainIds ?? item.DomainIds;
        const legacyDomainId = item.domainId ?? item.DomainId;
        const normalizedDomainIds = Array.isArray(domainIdsRaw)
            ? domainIdsRaw.map((id) => Number(id)).filter((id) => Number.isFinite(id) && id > 0)
            : (Number.isFinite(Number(legacyDomainId)) && Number(legacyDomainId) > 0 ? [Number(legacyDomainId)] : []);
        return {
            id: item.id ?? item.Id ?? 0,
            domainIds: normalizedDomainIds,
            serverId: item.serverId ?? item.ServerId ?? null,
            hostname: item.hostname ?? item.Hostname ?? '',
            ipAddress: item.ipAddress ?? item.IpAddress ?? null,
            isPrimary: item.isPrimary ?? item.IsPrimary ?? false,
            sortOrder: item.sortOrder ?? item.SortOrder ?? 0,
        };
    }
    function getPrimaryDomainId(ns) {
        return ns.domainIds.length > 0 ? ns.domainIds[0] : 0;
    }
    function getDomainDisplayNames(ns) {
        if (!ns.domainIds.length) {
            return '-';
        }
        return ns.domainIds
            .map((id) => domainLookup.get(id) || `Domain #${id}`)
            .join(', ');
    }
    function detectMode(ns) {
        const hostname = (ns.hostname || '').toLowerCase();
        const hasIp = !!(ns.ipAddress || '').trim();
        if (hasIp) {
            return 'self';
        }
        const registrarHints = ['awsdns', 'route53', 'registrar-servers', 'cloudflare', 'namecheap', 'godaddy', 'opensrs', 'centralnic', 'dnsimple', 'regtons'];
        if (registrarHints.some((hint) => hostname.includes(hint))) {
            return 'registrar';
        }
        if (selectedDomainName && hostname.endsWith(selectedDomainName.toLowerCase())) {
            return 'self';
        }
        return 'registrar';
    }
    function applyModeFilter() {
        loadFiltersFromUi();
        currentPage = 1;
        filteredNameServers = allNameServers.filter((ns) => {
            if (filterHostname && !(ns.hostname || '').toLowerCase().includes(filterHostname)) {
                return false;
            }
            if (filterIp && !(ns.ipAddress || '').toLowerCase().includes(filterIp)) {
                return false;
            }
            const mode = detectMode(ns);
            if (filterMode === 'registrar') {
                if (mode !== 'registrar') {
                    return false;
                }
            }
            if (filterMode === 'self') {
                if (mode !== 'self') {
                    return false;
                }
            }
            if (filterPrimary === 'yes' && !ns.isPrimary) {
                return false;
            }
            if (filterPrimary === 'no' && ns.isPrimary) {
                return false;
            }
            if (filterSort && !String(ns.sortOrder).toLowerCase().includes(filterSort)) {
                return false;
            }
            return true;
        });
        renderNameServers();
    }
    function loadPageSizeFromUi() {
        const select = document.getElementById('dns-nameservers-page-size');
        const parsed = Number(select?.value ?? '25');
        if (Number.isFinite(parsed) && parsed > 0) {
            pageSize = parsed;
        }
    }
    function loadFiltersFromUi() {
        filterHostname = (document.getElementById('dns-nameservers-filter-hostname')?.value ?? '').trim().toLowerCase();
        filterIp = (document.getElementById('dns-nameservers-filter-ip')?.value ?? '').trim().toLowerCase();
        filterMode = (document.getElementById('dns-nameservers-mode-filter')?.value ?? 'all').trim().toLowerCase() || 'all';
        filterPrimary = (document.getElementById('dns-nameservers-filter-primary')?.value ?? 'all').trim().toLowerCase() || 'all';
        filterSort = (document.getElementById('dns-nameservers-filter-sort')?.value ?? '').trim().toLowerCase();
    }
    function renderNameServers() {
        const tableBody = document.getElementById('dns-nameservers-table-body');
        if (!tableBody) {
            return;
        }
        renderSortableHeaders();
        setText('dns-nameservers-count', `${filteredNameServers.length} nameserver${filteredNameServers.length === 1 ? '' : 's'}`);
        if (!filteredNameServers.length) {
            showTable();
            hideEmpty();
            tableBody.innerHTML = '<tr><td colspan="7" class="text-center text-muted">No nameservers match the current filters.</td></tr>';
            totalCount = 0;
            totalPages = 1;
            currentPage = 1;
            renderPagination();
            return;
        }
        hideEmpty();
        showTable();
        const sorted = [...filteredNameServers].sort(compareNameServers);
        totalCount = sorted.length;
        totalPages = Math.max(1, Math.ceil(totalCount / pageSize));
        if (currentPage > totalPages) {
            currentPage = totalPages;
        }
        const start = (currentPage - 1) * pageSize;
        const paged = sorted.slice(start, start + pageSize);
        tableBody.innerHTML = paged.map((ns) => {
            const mode = detectMode(ns);
            const modeBadge = mode === 'self'
                ? '<span class="badge bg-info text-dark">Self managed</span>'
                : '<span class="badge bg-warning text-dark">Registrar managed</span>';
            const domainCount = ns.domainIds.length;
            const domainsButtonText = domainCount === 1 ? '1 domain' : `${domainCount} domains`;
            return `
            <tr>
                <td>
                    <div class="d-flex align-items-center gap-2">
                        <span class="badge bg-secondary">${domainCount}</span>
                        <button class="btn btn-outline-secondary btn-sm" type="button" data-action="domains" data-id="${ns.id}"><i class="bi bi-list-ul"></i> ${domainsButtonText}</button>
                    </div>
                </td>
                <td><code>${esc(ns.hostname || '-')}</code></td>
                <td>${esc(ns.ipAddress || '-')}</td>
                <td>${modeBadge}</td>
                <td>${ns.isPrimary ? '<span class="badge bg-success">Yes</span>' : '<span class="badge bg-secondary">No</span>'}</td>
                <td>${ns.sortOrder}</td>
                <td class="text-end">
                    <div class="btn-group btn-group-sm">
                        <button class="btn btn-outline-primary" type="button" data-action="edit" data-id="${ns.id}" title="Edit"><i class="bi bi-pencil"></i></button>
                        <button class="btn btn-outline-danger" type="button" data-action="delete" data-id="${ns.id}" title="Delete"><i class="bi bi-trash"></i></button>
                    </div>
                </td>
            </tr>
        `;
        }).join('');
        renderPagination();
    }
    function renderPagination() {
        const info = document.getElementById('dns-nameservers-pagination-info');
        const list = document.getElementById('dns-nameservers-pagination-list');
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
        const makeItem = (label, page, disabled, active = false) => {
            const cls = `page-item${disabled ? ' disabled' : ''}${active ? ' active' : ''}`;
            const dataPage = disabled ? '' : ` data-page="${page}"`;
            return `<li class="${cls}"><a class="page-link" href="#"${dataPage}>${label}</a></li>`;
        };
        let html = '';
        html += makeItem('Previous', currentPage - 1, currentPage <= 1);
        for (let page = 1; page <= totalPages; page += 1) {
            html += makeItem(String(page), page, false, page === currentPage);
        }
        html += makeItem('Next', currentPage + 1, currentPage >= totalPages);
        list.innerHTML = html;
        list.querySelectorAll('a[data-page]').forEach((anchor) => {
            anchor.addEventListener('click', (event) => {
                event.preventDefault();
                const target = event.currentTarget;
                const nextPage = Number(target.dataset.page ?? '0');
                if (!Number.isFinite(nextPage) || nextPage < 1 || nextPage > totalPages) {
                    return;
                }
                currentPage = nextPage;
                renderNameServers();
            });
        });
    }
    function compareNameServers(a, b) {
        const domainA = a.domainIds.length;
        const domainB = b.domainIds.length;
        const modeA = detectMode(a);
        const modeB = detectMode(b);
        let result = 0;
        switch (sortColumn) {
            case 'domain':
                result = domainA - domainB;
                break;
            case 'hostname':
                result = (a.hostname || '').toLowerCase().localeCompare((b.hostname || '').toLowerCase());
                break;
            case 'ipAddress':
                result = (a.ipAddress || '').toLowerCase().localeCompare((b.ipAddress || '').toLowerCase());
                break;
            case 'mode':
                result = modeA.localeCompare(modeB);
                break;
            case 'isPrimary':
                result = Number(a.isPrimary) - Number(b.isPrimary);
                break;
            case 'sortOrder':
                result = a.sortOrder - b.sortOrder;
                break;
        }
        if (result === 0) {
            result = (a.hostname || '').toLowerCase().localeCompare((b.hostname || '').toLowerCase());
        }
        return sortDirection === 'asc' ? result : -result;
    }
    function renderSortableHeaders() {
        const tableHeadRow = document.querySelector('#dns-nameservers-table-wrapper thead tr');
        if (!tableHeadRow) {
            return;
        }
        const headers = [
            { key: 'domain', label: 'Domains' },
            { key: 'hostname', label: 'Hostname' },
            { key: 'ipAddress', label: 'IP Address' },
            { key: 'mode', label: 'Mode' },
            { key: 'isPrimary', label: 'Primary' },
            { key: 'sortOrder', label: 'Sort' },
        ];
        const makeIndicator = (key) => {
            if (sortColumn !== key) {
                return '<i class="bi bi-arrow-down-up ms-1 text-muted"></i>';
            }
            return sortDirection === 'asc'
                ? '<i class="bi bi-sort-down ms-1"></i>'
                : '<i class="bi bi-sort-up ms-1"></i>';
        };
        tableHeadRow.innerHTML = `
        ${headers.map((h) => `<th><button type="button" class="btn btn-link btn-sm text-decoration-none p-0" data-sort="${h.key}">${h.label}${makeIndicator(h.key)}</button></th>`).join('')}
        <th class="text-end">Actions</th>
    `;
    }
    function openDomains(id) {
        const ns = allNameServers.find((item) => item.id === id);
        if (!ns) {
            return;
        }
        const list = document.getElementById('dns-nameservers-domains-list');
        if (!list) {
            return;
        }
        setText('dns-nameservers-domains-hostname', ns.hostname || `Nameserver #${id}`);
        if (!ns.domainIds.length) {
            list.innerHTML = '<li class="list-group-item text-muted">No domains linked.</li>';
        }
        else {
            list.innerHTML = ns.domainIds
                .map((domainId) => `<li class="list-group-item">${esc(domainLookup.get(domainId) || `Domain #${domainId}`)}</li>`)
                .join('');
        }
        showModal('dns-nameservers-domains-modal');
    }
    function openCreate() {
        editingId = null;
        const form = document.getElementById('dns-nameservers-form');
        form?.reset();
        void ensureServerOptionsLoaded().then(() => {
            renderServerSelect(null);
        });
        setText('dns-nameservers-edit-title', 'Add Nameserver');
        setCheckboxValue('dns-nameservers-primary', false);
        setInputValue('dns-nameservers-sort-order', String(allNameServers.length));
        showModal('dns-nameservers-edit-modal');
    }
    function openEdit(id) {
        const ns = allNameServers.find((item) => item.id === id);
        if (!ns) {
            return;
        }
        editingId = id;
        setText('dns-nameservers-edit-title', 'Edit Nameserver');
        void ensureServerOptionsLoaded().then(() => {
            renderServerSelect(ns.serverId ?? null);
        });
        setInputValue('dns-nameservers-hostname', ns.hostname);
        setInputValue('dns-nameservers-ip', ns.ipAddress || '');
        setInputValue('dns-nameservers-sort-order', String(ns.sortOrder));
        setCheckboxValue('dns-nameservers-primary', !!ns.isPrimary);
        showModal('dns-nameservers-edit-modal');
    }
    async function saveNameServer() {
        const hostname = getInputValue('dns-nameservers-hostname');
        if (!hostname) {
            showError('Hostname is required.');
            return;
        }
        const selectedServerId = getSelectedServerId();
        const existingNameServer = editingId ? allNameServers.find((item) => item.id === editingId) : null;
        const domainIds = existingNameServer?.domainIds ?? (selectedDomainId ? [selectedDomainId] : []);
        const payload = {
            domainIds,
            serverId: selectedServerId,
            hostname,
            ipAddress: getInputValue('dns-nameservers-ip') || null,
            isPrimary: getCheckboxValue('dns-nameservers-primary'),
            sortOrder: getNumberValue('dns-nameservers-sort-order'),
        };
        const response = editingId
            ? await apiRequest(`${getApiBaseUrl()}/NameServers/${editingId}`, { method: 'PUT', body: JSON.stringify(payload) })
            : await apiRequest(`${getApiBaseUrl()}/NameServers`, { method: 'POST', body: JSON.stringify(payload) });
        if (!response.success) {
            showError(response.message || 'Failed to save nameserver.');
            return;
        }
        hideModal('dns-nameservers-edit-modal');
        showSuccess(editingId ? 'Nameserver updated.' : 'Nameserver created.');
        await loadNameServers();
    }
    function renderServerSelect(selectedServerId) {
        const select = document.getElementById('dns-nameservers-server-id');
        if (!select) {
            return;
        }
        const options = serverOptions
            .map((item) => `<option value="${item.id}">${esc(item.name || `Server #${item.id}`)}</option>`)
            .join('');
        select.innerHTML = `<option value="">No server</option>${options}`;
        if (selectedServerId && Number.isFinite(selectedServerId) && selectedServerId > 0) {
            select.value = String(selectedServerId);
        }
    }
    function getSelectedServerId() {
        const select = document.getElementById('dns-nameservers-server-id');
        const parsed = Number(select?.value ?? '0');
        return Number.isFinite(parsed) && parsed > 0 ? parsed : null;
    }
    async function applyServerDefaultIp() {
        const serverId = getSelectedServerId();
        if (!serverId) {
            return;
        }
        const response = await apiRequest(`${getApiBaseUrl()}/ServerIpAddresses/server/${serverId}`, { method: 'GET' });
        if (!response.success) {
            return;
        }
        const raw = response.data;
        const items = Array.isArray(raw)
            ? raw
            : Array.isArray(raw?.data)
                ? raw.data
                : Array.isArray(raw?.Data)
                    ? raw.Data
                    : [];
        const preferred = items.find((item) => (item.isPrimary ?? item.IsPrimary) === true) ?? items[0];
        const ip = String(preferred?.ipAddress ?? preferred?.IpAddress ?? '').trim();
        if (ip) {
            setInputValue('dns-nameservers-ip', ip);
        }
    }
    function openDelete(id) {
        const ns = allNameServers.find((item) => item.id === id);
        if (!ns) {
            return;
        }
        pendingDeleteId = id;
        setText('dns-nameservers-delete-name', ns.hostname || `#${id}`);
        showModal('dns-nameservers-delete-modal');
    }
    async function deleteNameServer() {
        if (!pendingDeleteId) {
            return;
        }
        const response = await apiRequest(`${getApiBaseUrl()}/NameServers/${pendingDeleteId}`, { method: 'DELETE' });
        hideModal('dns-nameservers-delete-modal');
        if (!response.success) {
            showError(response.message || 'Delete failed.');
            return;
        }
        showSuccess('Nameserver deleted.');
        pendingDeleteId = null;
        await loadNameServers();
    }
    function showNoSelection() {
        document.getElementById('dns-nameservers-empty-selection')?.classList.remove('d-none');
        document.getElementById('dns-nameservers-loading')?.classList.add('d-none');
        document.getElementById('dns-nameservers-table-wrapper')?.classList.add('d-none');
        document.getElementById('dns-nameservers-empty')?.classList.add('d-none');
    }
    function showTable() {
        document.getElementById('dns-nameservers-empty-selection')?.classList.add('d-none');
        document.getElementById('dns-nameservers-loading')?.classList.add('d-none');
        document.getElementById('dns-nameservers-table-wrapper')?.classList.remove('d-none');
    }
    function showEmpty() {
        document.getElementById('dns-nameservers-empty-selection')?.classList.add('d-none');
        document.getElementById('dns-nameservers-table-wrapper')?.classList.add('d-none');
        document.getElementById('dns-nameservers-empty')?.classList.remove('d-none');
    }
    function hideEmpty() {
        document.getElementById('dns-nameservers-empty')?.classList.add('d-none');
    }
    function setLoading(isLoading) {
        if (isLoading) {
            document.getElementById('dns-nameservers-empty-selection')?.classList.add('d-none');
            document.getElementById('dns-nameservers-table-wrapper')?.classList.add('d-none');
            document.getElementById('dns-nameservers-empty')?.classList.add('d-none');
            document.getElementById('dns-nameservers-loading')?.classList.remove('d-none');
        }
        else {
            document.getElementById('dns-nameservers-loading')?.classList.add('d-none');
        }
    }
    function setInputValue(id, value) {
        const element = document.getElementById(id);
        if (element) {
            element.value = value;
        }
    }
    function setCheckboxValue(id, value) {
        const element = document.getElementById(id);
        if (element) {
            element.checked = value;
        }
    }
    function getInputValue(id) {
        const element = document.getElementById(id);
        return (element?.value ?? '').trim();
    }
    function getNumberValue(id) {
        const raw = getInputValue(id);
        const parsed = Number(raw);
        return Number.isFinite(parsed) ? parsed : 0;
    }
    function getCheckboxValue(id) {
        const element = document.getElementById(id);
        return !!element?.checked;
    }
    function setText(id, value) {
        const element = document.getElementById(id);
        if (element) {
            element.textContent = value;
        }
    }
    function showSuccess(message) {
        const element = document.getElementById('dns-nameservers-alert-success');
        if (!element) {
            return;
        }
        element.textContent = message;
        element.classList.remove('d-none');
        document.getElementById('dns-nameservers-alert-error')?.classList.add('d-none');
    }
    function showError(message) {
        const element = document.getElementById('dns-nameservers-alert-error');
        if (!element) {
            return;
        }
        element.textContent = message;
        element.classList.remove('d-none');
        document.getElementById('dns-nameservers-alert-success')?.classList.add('d-none');
    }
    function clearAlerts() {
        document.getElementById('dns-nameservers-alert-success')?.classList.add('d-none');
        document.getElementById('dns-nameservers-alert-error')?.classList.add('d-none');
    }
    function showModal(id) {
        const modalElement = document.getElementById(id);
        if (!modalElement || !window.bootstrap) {
            return;
        }
        const modal = new window.bootstrap.Modal(modalElement);
        modal.show();
    }
    function hideModal(id) {
        const modalElement = document.getElementById(id);
        if (!modalElement || !window.bootstrap) {
            return;
        }
        const modal = window.bootstrap.Modal.getInstance(modalElement);
        modal?.hide();
    }
    function esc(value) {
        const map = { '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#039;' };
        return (value || '').replace(/[&<>"']/g, (char) => map[char] ?? char);
    }
    function setupPageObserver() {
        initializePage();
        if (document.body) {
            const observer = new MutationObserver(() => {
                const page = document.getElementById('dns-nameservers-page');
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
})();
//# sourceMappingURL=dns-nameservers.js.map
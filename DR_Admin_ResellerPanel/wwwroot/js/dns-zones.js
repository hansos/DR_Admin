"use strict";
// @ts-nocheck
(function () {
    function getApiBaseUrl() {
        var _a, _b;
        return (_b = (_a = window.AppSettings) === null || _a === void 0 ? void 0 : _a.apiBaseUrl) !== null && _b !== void 0 ? _b : '';
    }
    function getAuthToken() {
        const auth = window.Auth;
        if (auth === null || auth === void 0 ? void 0 : auth.getToken) {
            return auth.getToken();
        }
        return sessionStorage.getItem('rp_authToken');
    }
    async function apiRequest(endpoint, options = {}) {
        var _a, _b, _c;
        try {
            const headers = Object.assign({ 'Content-Type': 'application/json' }, options.headers);
            const authToken = getAuthToken();
            if (authToken) {
                headers['Authorization'] = `Bearer ${authToken}`;
            }
            const response = await fetch(endpoint, Object.assign(Object.assign({}, options), { headers, credentials: 'include' }));
            const contentType = (_a = response.headers.get('content-type')) !== null && _a !== void 0 ? _a : '';
            const hasJson = contentType.includes('application/json');
            const data = hasJson ? await response.json() : null;
            if (!response.ok) {
                return {
                    success: false,
                    message: (data && ((_b = data.message) !== null && _b !== void 0 ? _b : data.title)) || `Request failed with status ${response.status}`,
                };
            }
            return {
                success: (data === null || data === void 0 ? void 0 : data.success) !== false,
                data: (_c = data === null || data === void 0 ? void 0 : data.data) !== null && _c !== void 0 ? _c : data,
                message: data === null || data === void 0 ? void 0 : data.message,
            };
        }
        catch (error) {
            console.error('DNS zones request failed', error);
            return {
                success: false,
                message: 'Network error. Please try again.',
            };
        }
    }
    let selectedDomainId = null;
    let selectedDomainName = null;
    let records = [];
    let recordTypes = [];
    let pendingRecords = [];
    let editingRecordId = null;
    let pendingDeleteId = null;
    function initializePage() {
        var _a, _b, _c, _d, _e, _f, _g, _h;
        const page = document.getElementById('dns-zones-page');
        if (!page || page.dataset.initialized === 'true') {
            return;
        }
        page.dataset.initialized = 'true';
        (_a = document.getElementById('dns-zones-select')) === null || _a === void 0 ? void 0 : _a.addEventListener('click', openSelectDomainModal);
        (_b = document.getElementById('dns-zones-domain-load')) === null || _b === void 0 ? void 0 : _b.addEventListener('click', loadDomainFromModal);
        (_c = document.getElementById('dns-zones-add')) === null || _c === void 0 ? void 0 : _c.addEventListener('click', openCreate);
        (_d = document.getElementById('dns-zones-save')) === null || _d === void 0 ? void 0 : _d.addEventListener('click', saveRecord);
        (_e = document.getElementById('dns-zones-confirm-delete')) === null || _e === void 0 ? void 0 : _e.addEventListener('click', deleteRecord);
        (_f = document.getElementById('dns-zones-record-type')) === null || _f === void 0 ? void 0 : _f.addEventListener('change', updateFieldVisibility);
        (_g = document.getElementById('dns-zones-sync')) === null || _g === void 0 ? void 0 : _g.addEventListener('click', openSyncModal);
        (_h = document.getElementById('dns-zones-sync-confirm')) === null || _h === void 0 ? void 0 : _h.addEventListener('click', performSync);
        const input = document.getElementById('dns-zones-domain-name');
        input === null || input === void 0 ? void 0 : input.addEventListener('keydown', (event) => {
            if (event.key === 'Enter') {
                event.preventDefault();
                loadDomainFromModal();
            }
        });
        bindTableActions();
        loadDnsRecordTypes();
        const params = new URLSearchParams(window.location.search);
        const domainIdParam = params.get('domain-id');
        const parsed = Number(domainIdParam);
        if (domainIdParam && Number.isFinite(parsed) && parsed > 0) {
            loadDomainById(parsed);
        }
        else {
            showNoSelection();
            openSelectDomainModal();
        }
    }
    async function loadDnsRecordTypes() {
        const response = await apiRequest(`${getApiBaseUrl()}/DnsRecordTypes`, { method: 'GET' });
        if (!response.success) {
            showError(response.message || 'Failed to load DNS record types.');
            return;
        }
        const raw = response.data;
        const items = Array.isArray(raw)
            ? raw
            : Array.isArray(raw === null || raw === void 0 ? void 0 : raw.data)
                ? raw.data
                : Array.isArray(raw === null || raw === void 0 ? void 0 : raw.Data)
                    ? raw.Data
                    : [];
        recordTypes = items.map((item) => {
            var _a, _b, _c, _d, _e, _f, _g, _h, _j, _k, _l, _m, _o, _p, _q, _r, _s, _t;
            return ({
                id: (_b = (_a = item.id) !== null && _a !== void 0 ? _a : item.Id) !== null && _b !== void 0 ? _b : 0,
                type: (_d = (_c = item.type) !== null && _c !== void 0 ? _c : item.Type) !== null && _d !== void 0 ? _d : '',
                description: (_f = (_e = item.description) !== null && _e !== void 0 ? _e : item.Description) !== null && _f !== void 0 ? _f : '',
                hasPriority: (_h = (_g = item.hasPriority) !== null && _g !== void 0 ? _g : item.HasPriority) !== null && _h !== void 0 ? _h : false,
                hasWeight: (_k = (_j = item.hasWeight) !== null && _j !== void 0 ? _j : item.HasWeight) !== null && _k !== void 0 ? _k : false,
                hasPort: (_m = (_l = item.hasPort) !== null && _l !== void 0 ? _l : item.HasPort) !== null && _m !== void 0 ? _m : false,
                isEditableByUser: (_p = (_o = item.isEditableByUser) !== null && _o !== void 0 ? _o : item.IsEditableByUser) !== null && _p !== void 0 ? _p : true,
                isActive: (_r = (_q = item.isActive) !== null && _q !== void 0 ? _q : item.IsActive) !== null && _r !== void 0 ? _r : true,
                defaultTTL: (_t = (_s = item.defaultTTL) !== null && _s !== void 0 ? _s : item.DefaultTTL) !== null && _t !== void 0 ? _t : undefined,
            });
        });
        renderRecordTypeOptions();
    }
    function renderRecordTypeOptions() {
        const select = document.getElementById('dns-zones-record-type');
        if (!select) {
            return;
        }
        const activeTypes = recordTypes.filter((type) => type.type && type.isActive !== false);
        activeTypes.sort((a, b) => a.type.localeCompare(b.type));
        select.innerHTML = activeTypes
            .map((type) => `<option value="${type.id}">${esc(type.type)}</option>`)
            .join('');
    }
    function bindTableActions() {
        const tableBody = document.getElementById('dns-zones-table-body');
        if (!tableBody) {
            return;
        }
        tableBody.addEventListener('click', (event) => {
            const target = event.target;
            const button = target.closest('button[data-action]');
            if (!button) {
                return;
            }
            const id = Number(button.dataset.id);
            if (!id) {
                return;
            }
            if (button.dataset.action === 'edit') {
                openEdit(id);
                return;
            }
            if (button.dataset.action === 'delete') {
                openDelete(id);
            }
        });
    }
    async function loadDomainById(domainId) {
        clearAlerts();
        setLoading(true);
        const response = await apiRequest(`${getApiBaseUrl()}/RegisteredDomains/${domainId}`, { method: 'GET' });
        if (!response.success || !response.data) {
            setLoading(false);
            showError(response.message || 'Failed to load domain.');
            showNoSelection();
            openSelectDomainModal();
            return;
        }
        const domain = normalizeDomain(response.data);
        setSelectedDomain(domain);
        await loadRecords();
    }
    async function loadDomainFromModal() {
        var _a;
        const input = document.getElementById('dns-zones-domain-name');
        const name = ((_a = input === null || input === void 0 ? void 0 : input.value) !== null && _a !== void 0 ? _a : '').trim();
        if (!name) {
            showError('Enter a domain name to load.');
            return;
        }
        clearAlerts();
        setLoading(true);
        const response = await apiRequest(`${getApiBaseUrl()}/RegisteredDomains/name/${encodeURIComponent(name)}`, { method: 'GET' });
        if (!response.success || !response.data) {
            setLoading(false);
            showError(response.message || 'Domain not found.');
            return;
        }
        const domain = normalizeDomain(response.data);
        setSelectedDomain(domain);
        hideModal('dns-zones-select-modal');
        await loadRecords();
    }
    function setSelectedDomain(domain) {
        selectedDomainId = domain.id;
        selectedDomainName = domain.name || `Domain #${domain.id}`;
        setText('dns-zones-selected-domain', selectedDomainName);
        setText('dns-zones-selected-id', String(domain.id));
        setSelectButtonLabel(selectedDomainName);
        updateSyncButtonState();
        updateAddButtonState();
    }
    function normalizeDomain(raw) {
        var _a, _b, _c, _d, _e;
        return {
            id: (_b = (_a = raw.id) !== null && _a !== void 0 ? _a : raw.Id) !== null && _b !== void 0 ? _b : 0,
            name: (_e = (_d = (_c = raw.name) !== null && _c !== void 0 ? _c : raw.Name) !== null && _d !== void 0 ? _d : raw.domainName) !== null && _e !== void 0 ? _e : '',
        };
    }
    async function loadRecords() {
        if (!selectedDomainId) {
            showNoSelection();
            return;
        }
        setLoading(true);
        const [activeResponse, deletedResponse] = await Promise.all([
            apiRequest(`${getApiBaseUrl()}/DnsRecords/domain/${selectedDomainId}`, { method: 'GET' }),
            apiRequest(`${getApiBaseUrl()}/DnsRecords/domain/${selectedDomainId}/deleted`, { method: 'GET' }),
        ]);
        if (!activeResponse.success) {
            setLoading(false);
            showError(activeResponse.message || 'Failed to load DNS records.');
            return;
        }
        const activeRaw = activeResponse.data;
        const activeRecords = Array.isArray(activeRaw)
            ? activeRaw
            : Array.isArray(activeRaw === null || activeRaw === void 0 ? void 0 : activeRaw.data)
                ? activeRaw.data
                : Array.isArray(activeRaw === null || activeRaw === void 0 ? void 0 : activeRaw.Data)
                    ? activeRaw.Data
                    : [];
        const deletedRaw = deletedResponse.success ? deletedResponse.data : [];
        const deletedRecords = Array.isArray(deletedRaw)
            ? deletedRaw
            : Array.isArray(deletedRaw === null || deletedRaw === void 0 ? void 0 : deletedRaw.data)
                ? deletedRaw.data
                : Array.isArray(deletedRaw === null || deletedRaw === void 0 ? void 0 : deletedRaw.Data)
                    ? deletedRaw.Data
                    : [];
        records = [...activeRecords, ...deletedRecords];
        renderRecords();
        setLoading(false);
        updatePendingSyncBadge();
        updateSyncButtonState();
        updateAddButtonState();
    }
    function renderRecords() {
        const tableBody = document.getElementById('dns-zones-table-body');
        if (!tableBody) {
            return;
        }
        setText('dns-zones-record-count', `${records.length} record${records.length === 1 ? '' : 's'}`);
        if (!records.length) {
            tableBody.innerHTML = '';
            showEmpty();
            return;
        }
        tableBody.innerHTML = records.map((record) => {
            var _a, _b, _c, _d;
            const isDeleted = record.isDeleted === true;
            const editable = record.isEditableByUser !== false && !isDeleted;
            const rowClass = isDeleted ? 'table-danger' : editable ? '' : 'table-warning';
            const lockBadge = editable ? '' : ' <span class="badge bg-secondary" title="Not editable"><i class="bi bi-lock"></i></span>';
            const pendingBadge = isDeleted ? ' <span class="badge bg-danger" title="Pending sync"><i class="bi bi-clock"></i> Pending</span>' : '';
            return `
        <tr class="${rowClass}">
            <td>${esc(record.type || '-')}${lockBadge}${pendingBadge}</td>
            <td>${esc(record.name || '-')}</td>
            <td>${esc(record.value || '-')}</td>
            <td>${(_a = record.ttl) !== null && _a !== void 0 ? _a : '-'}</td>
            <td>${(_b = record.priority) !== null && _b !== void 0 ? _b : '-'}</td>
            <td>${(_c = record.weight) !== null && _c !== void 0 ? _c : '-'}</td>
            <td>${(_d = record.port) !== null && _d !== void 0 ? _d : '-'}</td>
            <td class="text-end">
                <div class="btn-group btn-group-sm">
                    <button class="btn btn-outline-primary" type="button" data-action="edit" data-id="${record.id}" title="Edit" ${editable ? '' : 'disabled'}>
                        <i class="bi bi-pencil"></i>
                    </button>
                    <button class="btn btn-outline-danger" type="button" data-action="delete" data-id="${record.id}" title="Delete" ${editable ? '' : 'disabled'}>
                        <i class="bi bi-trash"></i>
                    </button>
                </div>
            </td>
        </tr>`;
        }).join('');
        showTable();
    }
    function openCreate() {
        var _a, _b, _c, _d, _e, _f;
        if (!selectedDomainId) {
            showError('Select a domain before adding DNS records.');
            return;
        }
        editingRecordId = null;
        const defaultTypeId = (_c = (_b = (_a = recordTypes[0]) === null || _a === void 0 ? void 0 : _a.id) === null || _b === void 0 ? void 0 : _b.toString()) !== null && _c !== void 0 ? _c : '';
        const defaultTtl = (_f = (_e = (_d = recordTypes[0]) === null || _d === void 0 ? void 0 : _d.defaultTTL) === null || _e === void 0 ? void 0 : _e.toString()) !== null && _f !== void 0 ? _f : '3600';
        setSelectValue('dns-zones-record-type', defaultTypeId);
        setInputValue('dns-zones-record-name', '');
        setInputValue('dns-zones-record-value', '');
        setInputValue('dns-zones-record-ttl', defaultTtl);
        setInputValue('dns-zones-record-priority', '');
        setInputValue('dns-zones-record-weight', '');
        setInputValue('dns-zones-record-port', '');
        updateFieldVisibility();
        showModal('dns-zones-edit-modal');
    }
    function openEdit(id) {
        var _a, _b, _c, _d, _e, _f, _g, _h, _j, _k, _l, _m;
        const record = records.find((item) => item.id === id);
        if (!record) {
            return;
        }
        if (record.isEditableByUser === false) {
            showError('This DNS record type is not editable.');
            return;
        }
        editingRecordId = id;
        setSelectValue('dns-zones-record-type', (_d = (_b = (_a = record.dnsRecordTypeId) === null || _a === void 0 ? void 0 : _a.toString()) !== null && _b !== void 0 ? _b : (_c = getRecordTypeIdByName(record.type)) === null || _c === void 0 ? void 0 : _c.toString()) !== null && _d !== void 0 ? _d : '');
        setInputValue('dns-zones-record-name', record.name);
        setInputValue('dns-zones-record-value', record.value);
        setInputValue('dns-zones-record-ttl', (_f = (_e = record.ttl) === null || _e === void 0 ? void 0 : _e.toString()) !== null && _f !== void 0 ? _f : '');
        setInputValue('dns-zones-record-priority', (_h = (_g = record.priority) === null || _g === void 0 ? void 0 : _g.toString()) !== null && _h !== void 0 ? _h : '');
        setInputValue('dns-zones-record-weight', (_k = (_j = record.weight) === null || _j === void 0 ? void 0 : _j.toString()) !== null && _k !== void 0 ? _k : '');
        setInputValue('dns-zones-record-port', (_m = (_l = record.port) === null || _l === void 0 ? void 0 : _l.toString()) !== null && _m !== void 0 ? _m : '');
        updateFieldVisibility();
        showModal('dns-zones-edit-modal');
    }
    async function saveRecord() {
        if (!selectedDomainId) {
            return;
        }
        const dnsRecordTypeId = getSelectNumberValue('dns-zones-record-type');
        if (!dnsRecordTypeId) {
            showError('Select a DNS record type.');
            return;
        }
        const payload = {
            domainId: selectedDomainId,
            dnsRecordTypeId,
            name: getInputValue('dns-zones-record-name'),
            value: getInputValue('dns-zones-record-value'),
            ttl: getNumberValue('dns-zones-record-ttl'),
            priority: getNullableNumberValue('dns-zones-record-priority'),
            weight: getNullableNumberValue('dns-zones-record-weight'),
            port: getNullableNumberValue('dns-zones-record-port'),
        };
        const response = editingRecordId
            ? await apiRequest(`${getApiBaseUrl()}/DnsRecords/${editingRecordId}`, {
                method: 'PUT',
                body: JSON.stringify(payload),
            })
            : await apiRequest(`${getApiBaseUrl()}/DnsRecords`, {
                method: 'POST',
                body: JSON.stringify(payload),
            });
        if (!response.success) {
            showError(response.message || 'Failed to save DNS record.');
            return;
        }
        hideModal('dns-zones-edit-modal');
        showSuccess(editingRecordId ? 'DNS record updated successfully.' : 'DNS record created successfully.');
        await loadRecords();
    }
    function openDelete(id) {
        const record = records.find((item) => item.id === id);
        if (!record) {
            return;
        }
        if (record.isEditableByUser === false) {
            showError('This DNS record type cannot be deleted.');
            return;
        }
        pendingDeleteId = id;
        const label = `${record.type} ${record.name || '@'}`;
        setText('dns-zones-delete-name', label);
        showModal('dns-zones-delete-modal');
    }
    async function deleteRecord() {
        if (!pendingDeleteId) {
            return;
        }
        const response = await apiRequest(`${getApiBaseUrl()}/DnsRecords/${pendingDeleteId}`, { method: 'DELETE' });
        hideModal('dns-zones-delete-modal');
        if (!response.success) {
            showError(response.message || 'Failed to delete DNS record.');
            return;
        }
        showSuccess('DNS record deleted successfully.');
        pendingDeleteId = null;
        await loadRecords();
    }
    function setLoading(isLoading) {
        const loading = document.getElementById('dns-zones-loading');
        const tableWrapper = document.getElementById('dns-zones-table-wrapper');
        const empty = document.getElementById('dns-zones-empty');
        const noSelection = document.getElementById('dns-zones-no-selection');
        if (loading) {
            loading.classList.toggle('d-none', !isLoading);
        }
        if (isLoading) {
            tableWrapper === null || tableWrapper === void 0 ? void 0 : tableWrapper.classList.add('d-none');
            empty === null || empty === void 0 ? void 0 : empty.classList.add('d-none');
            noSelection === null || noSelection === void 0 ? void 0 : noSelection.classList.add('d-none');
        }
    }
    function showNoSelection() {
        var _a;
        setLoading(false);
        (_a = document.getElementById('dns-zones-no-selection')) === null || _a === void 0 ? void 0 : _a.classList.remove('d-none');
        setText('dns-zones-selected-domain', 'No domain selected');
        setText('dns-zones-selected-id', '-');
        setSelectButtonLabel(null);
        setText('dns-zones-record-count', '0 records');
        updateSyncButtonState();
        updateAddButtonState();
    }
    function updateSyncButtonState() {
        const button = document.getElementById('dns-zones-sync');
        if (button) {
            button.disabled = !selectedDomainId;
        }
    }
    function updateAddButtonState() {
        const button = document.getElementById('dns-zones-add');
        if (button) {
            button.disabled = !selectedDomainId;
        }
    }
    function updatePendingSyncBadge() {
        const badge = document.getElementById('dns-zones-pending-count');
        if (!badge) {
            return;
        }
        const count = records.filter((record) => record.isPendingSync).length;
        badge.textContent = String(count);
        badge.classList.toggle('d-none', count === 0);
    }
    async function openSyncModal() {
        if (!selectedDomainId) {
            showError('Select a domain before syncing.');
            return;
        }
        const loading = document.getElementById('dns-zones-sync-loading');
        const empty = document.getElementById('dns-zones-sync-empty');
        const content = document.getElementById('dns-zones-sync-content');
        const progress = document.getElementById('dns-zones-sync-progress');
        const summary = document.getElementById('dns-zones-sync-summary');
        const confirmBtn = document.getElementById('dns-zones-sync-confirm');
        const cancelBtn = document.getElementById('dns-zones-sync-cancel');
        loading === null || loading === void 0 ? void 0 : loading.classList.remove('d-none');
        empty === null || empty === void 0 ? void 0 : empty.classList.add('d-none');
        content === null || content === void 0 ? void 0 : content.classList.add('d-none');
        progress === null || progress === void 0 ? void 0 : progress.classList.add('d-none');
        summary === null || summary === void 0 ? void 0 : summary.classList.add('d-none');
        if (confirmBtn) {
            confirmBtn.disabled = true;
            confirmBtn.innerHTML = '<i class="bi bi-arrow-repeat"></i> Sync All to DNS Server';
        }
        if (cancelBtn) {
            cancelBtn.disabled = false;
            cancelBtn.textContent = 'Cancel';
        }
        const bar = document.getElementById('dns-zones-sync-progress-bar');
        if (bar) {
            bar.style.width = '0%';
            bar.className = 'progress-bar progress-bar-striped progress-bar-animated bg-warning';
        }
        showModal('dns-zones-sync-modal');
        const response = await apiRequest(`${getApiBaseUrl()}/DnsRecords/domain/${selectedDomainId}/pending-sync`, { method: 'GET' });
        if (!response.success) {
            pendingRecords = [];
            showError(response.message || 'Failed to load pending records.');
        }
        else {
            const raw = response.data;
            pendingRecords = Array.isArray(raw)
                ? raw
                : Array.isArray(raw === null || raw === void 0 ? void 0 : raw.data)
                    ? raw.data
                    : Array.isArray(raw === null || raw === void 0 ? void 0 : raw.Data)
                        ? raw.Data
                        : [];
        }
        loading === null || loading === void 0 ? void 0 : loading.classList.add('d-none');
        if (!pendingRecords.length) {
            empty === null || empty === void 0 ? void 0 : empty.classList.remove('d-none');
            return;
        }
        setText('dns-zones-sync-count', String(pendingRecords.length));
        renderSyncTable();
        content === null || content === void 0 ? void 0 : content.classList.remove('d-none');
        if (confirmBtn) {
            confirmBtn.disabled = false;
        }
    }
    function renderSyncTable() {
        const body = document.getElementById('dns-zones-sync-table');
        if (!body) {
            return;
        }
        body.innerHTML = pendingRecords.map((record) => {
            var _a;
            return `
        <tr id="dns-zones-sync-row-${record.id}">
            <td>${esc(record.type || '-')}</td>
            <td><code>${esc(getFullDnsName(record.name, selectedDomainName !== null && selectedDomainName !== void 0 ? selectedDomainName : ''))}</code></td>
            <td class="text-break"><code class="small">${esc(record.value || '-')}</code></td>
            <td>${(_a = record.ttl) !== null && _a !== void 0 ? _a : '-'}</td>
            <td><span class="badge bg-warning text-dark"><i class="bi bi-clock"></i> Pending</span></td>
        </tr>`;
        }).join('');
    }
    async function performSync() {
        var _a, _b, _c;
        const confirmBtn = document.getElementById('dns-zones-sync-confirm');
        const cancelBtn = document.getElementById('dns-zones-sync-cancel');
        const progress = document.getElementById('dns-zones-sync-progress');
        const bar = document.getElementById('dns-zones-sync-progress-bar');
        const progressText = document.getElementById('dns-zones-sync-progress-text');
        const summary = document.getElementById('dns-zones-sync-summary');
        if (confirmBtn) {
            confirmBtn.disabled = true;
            confirmBtn.innerHTML = '<span class="spinner-border spinner-border-sm"></span> Syncing...';
        }
        if (cancelBtn) {
            cancelBtn.disabled = true;
        }
        progress === null || progress === void 0 ? void 0 : progress.classList.remove('d-none');
        summary === null || summary === void 0 ? void 0 : summary.classList.add('d-none');
        let succeeded = 0;
        let failed = 0;
        const total = pendingRecords.length;
        for (let i = 0; i < total; i++) {
            const record = pendingRecords[i];
            const pct = Math.round(((i + 1) / total) * 100);
            if (progressText) {
                progressText.textContent = `${i + 1} / ${total}`;
            }
            if (bar) {
                bar.style.width = `${pct}%`;
                bar.setAttribute('aria-valuenow', String(pct));
            }
            const row = document.getElementById(`dns-zones-sync-row-${record.id}`);
            try {
                const result = await apiRequest(`${getApiBaseUrl()}/DnsRecords/${record.id}/push`, { method: 'POST' });
                if (result.success) {
                    succeeded++;
                    if (row && row.cells[4]) {
                        row.cells[4].innerHTML = '<span class="badge bg-success"><i class="bi bi-check2"></i> Synced</span>';
                    }
                }
                else {
                    failed++;
                    const errMsg = (_c = (_b = (_a = result.data) === null || _a === void 0 ? void 0 : _a.message) !== null && _b !== void 0 ? _b : result.message) !== null && _c !== void 0 ? _c : 'Failed';
                    if (row && row.cells[4]) {
                        row.cells[4].innerHTML = `<span class="badge bg-danger" title="${esc(errMsg)}"><i class="bi bi-x"></i> Failed</span>`;
                    }
                }
            }
            catch (error) {
                failed++;
                if (row && row.cells[4]) {
                    row.cells[4].innerHTML = '<span class="badge bg-danger"><i class="bi bi-x"></i> Error</span>';
                }
            }
        }
        if (bar) {
            bar.classList.remove('progress-bar-animated');
            bar.classList.toggle('bg-success', failed === 0);
        }
        if (summary) {
            if (failed === 0) {
                summary.className = 'mt-3 alert alert-success';
                summary.innerHTML = `<i class="bi bi-check-circle-fill"></i> All <strong>${succeeded}</strong> record(s) were successfully marked as synced.`;
            }
            else {
                summary.className = 'mt-3 alert alert-warning';
                summary.innerHTML = `<i class="bi bi-exclamation-triangle-fill"></i> <strong>${succeeded}</strong> record(s) synced, <strong>${failed}</strong> failed.`;
            }
            summary.classList.remove('d-none');
        }
        if (confirmBtn) {
            confirmBtn.innerHTML = '<i class="bi bi-check2-circle"></i> Done';
        }
        if (cancelBtn) {
            cancelBtn.disabled = false;
            cancelBtn.textContent = 'Close';
        }
        await loadRecords();
    }
    function getFullDnsName(name, domainName) {
        if (!name || name === '@') {
            return domainName || '@';
        }
        if (!domainName) {
            return name;
        }
        if (name.endsWith(`.${domainName}`) || name === domainName) {
            return name;
        }
        return `${name}.${domainName}`;
    }
    function showEmpty() {
        var _a, _b;
        (_a = document.getElementById('dns-zones-empty')) === null || _a === void 0 ? void 0 : _a.classList.remove('d-none');
        (_b = document.getElementById('dns-zones-table-wrapper')) === null || _b === void 0 ? void 0 : _b.classList.add('d-none');
    }
    function showTable() {
        var _a, _b, _c;
        (_a = document.getElementById('dns-zones-table-wrapper')) === null || _a === void 0 ? void 0 : _a.classList.remove('d-none');
        (_b = document.getElementById('dns-zones-empty')) === null || _b === void 0 ? void 0 : _b.classList.add('d-none');
        (_c = document.getElementById('dns-zones-no-selection')) === null || _c === void 0 ? void 0 : _c.classList.add('d-none');
    }
    function openSelectDomainModal() {
        const input = document.getElementById('dns-zones-domain-name');
        if (input) {
            input.value = selectedDomainName !== null && selectedDomainName !== void 0 ? selectedDomainName : '';
        }
        showModal('dns-zones-select-modal');
        input === null || input === void 0 ? void 0 : input.focus();
    }
    function setSelectButtonLabel(domainName) {
        const button = document.getElementById('dns-zones-select');
        if (!button) {
            return;
        }
        button.innerHTML = domainName
            ? `<i class="bi bi-search"></i> ${esc(domainName)}`
            : '<i class="bi bi-search"></i> Select domain';
    }
    function setText(id, value) {
        const el = document.getElementById(id);
        if (el) {
            el.textContent = value;
        }
    }
    function setInputValue(id, value) {
        const el = document.getElementById(id);
        if (el) {
            el.value = value !== null && value !== void 0 ? value : '';
        }
    }
    function setSelectValue(id, value) {
        const el = document.getElementById(id);
        if (el) {
            el.value = value !== null && value !== void 0 ? value : '';
        }
    }
    function getInputValue(id) {
        var _a;
        const el = document.getElementById(id);
        return ((_a = el === null || el === void 0 ? void 0 : el.value) !== null && _a !== void 0 ? _a : '').trim();
    }
    function getSelectNumberValue(id) {
        var _a;
        const el = document.getElementById(id);
        const raw = ((_a = el === null || el === void 0 ? void 0 : el.value) !== null && _a !== void 0 ? _a : '').trim();
        const parsed = Number(raw);
        return Number.isFinite(parsed) ? parsed : 0;
    }
    function getNumberValue(id) {
        const value = getInputValue(id);
        const parsed = Number(value);
        return Number.isFinite(parsed) ? parsed : 0;
    }
    function updateFieldVisibility() {
        var _a, _b, _c;
        const dnsRecordTypeId = getSelectNumberValue('dns-zones-record-type');
        const type = recordTypes.find((item) => item.id === dnsRecordTypeId);
        toggleField('dns-zones-field-priority', (_a = type === null || type === void 0 ? void 0 : type.hasPriority) !== null && _a !== void 0 ? _a : false);
        toggleField('dns-zones-field-weight', (_b = type === null || type === void 0 ? void 0 : type.hasWeight) !== null && _b !== void 0 ? _b : false);
        toggleField('dns-zones-field-port', (_c = type === null || type === void 0 ? void 0 : type.hasPort) !== null && _c !== void 0 ? _c : false);
    }
    function toggleField(id, visible) {
        const field = document.getElementById(id);
        if (!field) {
            return;
        }
        field.classList.toggle('d-none', !visible);
    }
    function getRecordTypeIdByName(typeName) {
        var _a;
        if (!typeName) {
            return null;
        }
        const match = recordTypes.find((item) => item.type.toLowerCase() === typeName.toLowerCase());
        return (_a = match === null || match === void 0 ? void 0 : match.id) !== null && _a !== void 0 ? _a : null;
    }
    function getNullableNumberValue(id) {
        const value = getInputValue(id);
        if (!value) {
            return null;
        }
        const parsed = Number(value);
        return Number.isFinite(parsed) ? parsed : null;
    }
    function showSuccess(message) {
        var _a;
        const alert = document.getElementById('dns-zones-alert-success');
        if (!alert) {
            return;
        }
        alert.textContent = message;
        alert.classList.remove('d-none');
        (_a = document.getElementById('dns-zones-alert-error')) === null || _a === void 0 ? void 0 : _a.classList.add('d-none');
    }
    function showError(message) {
        var _a;
        const alert = document.getElementById('dns-zones-alert-error');
        if (!alert) {
            return;
        }
        alert.textContent = message;
        alert.classList.remove('d-none');
        (_a = document.getElementById('dns-zones-alert-success')) === null || _a === void 0 ? void 0 : _a.classList.add('d-none');
    }
    function clearAlerts() {
        var _a, _b;
        (_a = document.getElementById('dns-zones-alert-success')) === null || _a === void 0 ? void 0 : _a.classList.add('d-none');
        (_b = document.getElementById('dns-zones-alert-error')) === null || _b === void 0 ? void 0 : _b.classList.add('d-none');
    }
    function esc(text) {
        const map = { '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#039;' };
        return (text || '').toString().replace(/[&<>"']/g, (char) => map[char]);
    }
    function showModal(id) {
        const element = document.getElementById(id);
        if (!element || !window.bootstrap) {
            return;
        }
        const modal = new window.bootstrap.Modal(element);
        modal.show();
    }
    function hideModal(id) {
        const element = document.getElementById(id);
        if (!element || !window.bootstrap) {
            return;
        }
        const modal = window.bootstrap.Modal.getInstance(element);
        modal === null || modal === void 0 ? void 0 : modal.hide();
    }
    function setupPageObserver() {
        initializePage();
        if (document.body) {
            const observer = new MutationObserver(() => {
                const page = document.getElementById('dns-zones-page');
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
//# sourceMappingURL=dns-zones.js.map
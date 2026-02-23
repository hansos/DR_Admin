/**
 * Control Panels Management Page
 * Full CRUD with filtering, pagination, and FK dropdowns (server, type, IP address)
 */

let allPanels = [];
let filteredPanels = [];
let servers = [];
let panelTypes = [];
let allIpAddresses = [];
let editingId = null;
let pendingDeleteId = null;
let currentPage = 1;
const pageSize = 15;
let searchTerm = '';
let serverFilter = '';
let typeFilter = '';
let statusFilter = '';

document.addEventListener('DOMContentLoaded', async () => {
    checkAuthState();
    await loadLookupData();
    await loadPanels();
    initFilters();
    document.getElementById('saveBtn').addEventListener('click', savePanel);
    document.getElementById('confirmDelete').addEventListener('click', () => doDelete());
    document.getElementById('serverId').addEventListener('change', onServerChange);
});

async function loadLookupData() {
    const [typesRes, serversRes, ipsRes] = await Promise.all([
        window.ControlPanelTypeAPI.getActive(),
        window.ServerAPI.getAll(),
        window.ServerIpAddressAPI.getAll(),
    ]);

    if (typesRes.success) panelTypes = Array.isArray(typesRes.data) ? typesRes.data : [];
    if (serversRes.success) servers = Array.isArray(serversRes.data) ? serversRes.data : [];
    if (ipsRes.success) allIpAddresses = Array.isArray(ipsRes.data) ? ipsRes.data : [];

    populateModalDropdowns();
    populateFilterDropdowns();
}

function populateModalDropdowns() {
    const serverSelect = document.getElementById('serverId');
    serverSelect.querySelectorAll('option:not([value=""])').forEach(o => o.remove());
    servers.forEach(s => serverSelect.add(new Option(s.name, s.id)));

    const typeSelect = document.getElementById('controlPanelTypeId');
    typeSelect.querySelectorAll('option:not([value=""])').forEach(o => o.remove());
    panelTypes.forEach(t => typeSelect.add(new Option(t.displayName, t.id)));
}

function populateFilterDropdowns() {
    const serverFilterEl = document.getElementById('serverFilter');
    serverFilterEl.querySelectorAll('option:not([value=""])').forEach(o => o.remove());
    servers.forEach(s => serverFilterEl.add(new Option(s.name, s.id)));

    const typeFilterEl = document.getElementById('typeFilter');
    typeFilterEl.querySelectorAll('option:not([value=""])').forEach(o => o.remove());
    panelTypes.forEach(t => typeFilterEl.add(new Option(t.displayName, t.id)));
}

function onServerChange() {
    const serverId = parseInt(document.getElementById('serverId').value);
    const ipSelect = document.getElementById('ipAddressId');
    ipSelect.querySelectorAll('option:not([value=""])').forEach(o => o.remove());

    if (serverId) {
        const serverIps = allIpAddresses.filter(ip => ip.serverId === serverId);
        serverIps.forEach(ip => {
            const label = `${ip.ipAddress} (${ip.ipVersion}${ip.isPrimary ? ' · Primary' : ''})`;
            ipSelect.add(new Option(label, ip.id));
        });
        ipSelect.querySelector('option[value=""]').textContent = serverIps.length ? '-- None --' : '-- No IPs found --';
    } else {
        ipSelect.querySelector('option[value=""]').textContent = '-- None / select server first --';
    }
}

async function loadPanels() {
    const tbody = document.getElementById('tableBody');
    tbody.innerHTML = '<tr><td colspan="11" class="text-center p-4"><div class="spinner-border text-primary"></div></td></tr>';

    const res = await window.ServerControlPanelAPI.getAll();
    if (!res.success) {
        showError(res.message || 'Failed to load control panels');
        tbody.innerHTML = '<tr><td colspan="11" class="text-center text-danger p-4">Failed to load data</td></tr>';
        return;
    }

    allPanels = Array.isArray(res.data) ? res.data : [];
    applyFiltersAndRender();
}

function initFilters() {
    let searchTimeout;
    document.getElementById('searchInput').addEventListener('input', e => {
        clearTimeout(searchTimeout);
        searchTimeout = setTimeout(() => {
            searchTerm = e.target.value.toLowerCase();
            applyFiltersAndRender();
        }, 300);
    });

    document.getElementById('serverFilter').addEventListener('change', e => {
        serverFilter = e.target.value;
        applyFiltersAndRender();
    });

    document.getElementById('typeFilter').addEventListener('change', e => {
        typeFilter = e.target.value;
        applyFiltersAndRender();
    });

    document.getElementById('statusFilter').addEventListener('change', e => {
        statusFilter = e.target.value;
        applyFiltersAndRender();
    });

    document.getElementById('clearFilters').addEventListener('click', () => {
        document.getElementById('searchInput').value = '';
        document.getElementById('serverFilter').value = '';
        document.getElementById('typeFilter').value = '';
        document.getElementById('statusFilter').value = '';
        searchTerm = '';
        serverFilter = '';
        typeFilter = '';
        statusFilter = '';
        applyFiltersAndRender();
    });
}

function applyFiltersAndRender() {
    filteredPanels = allPanels.filter(p => {
        if (searchTerm && !p.apiUrl?.toLowerCase().includes(searchTerm)) return false;
        if (serverFilter && p.serverId !== parseInt(serverFilter)) return false;
        if (typeFilter && p.controlPanelTypeId !== parseInt(typeFilter)) return false;
        if (statusFilter && p.status !== statusFilter) return false;
        return true;
    });

    currentPage = 1;
    document.getElementById('panelCount').textContent = `${filteredPanels.length} panel(s)`;
    renderTable();
    renderPagination();
}

function renderTable() {
    const tbody = document.getElementById('tableBody');
    if (!filteredPanels.length) {
        tbody.innerHTML = '<tr><td colspan="11" class="text-center text-muted p-4">No control panels found.</td></tr>';
        return;
    }

    const start = (currentPage - 1) * pageSize;
    const page = filteredPanels.slice(start, start + pageSize);

    tbody.innerHTML = page.map(p => {
        const serverName = servers.find(s => s.id === p.serverId)?.name || `Server #${p.serverId}`;
        const typeName = panelTypes.find(t => t.id === p.controlPanelTypeId)?.displayName || `Type #${p.controlPanelTypeId}`;
        return `
        <tr>
            <td>${p.id}</td>
            <td><a href="/servers.html" class="text-decoration-none">${esc(serverName)}</a></td>
            <td><span class="badge bg-info text-dark">${esc(typeName)}</span></td>
            <td class="text-truncate" style="max-width:200px" title="${esc(p.apiUrl)}">${esc(p.apiUrl)}</td>
            <td>${p.ipAddressValue ? `<code class="small">${esc(p.ipAddressValue)}</code>` : '<span class="text-muted">-</span>'}</td>
            <td>${p.port}</td>
            <td>${p.useHttps ? '<i class="bi bi-shield-check text-success" title="HTTPS"></i>' : '<i class="bi bi-shield-x text-danger" title="HTTP"></i>'}</td>
            <td>${statusBadge(p.status)}</td>
            <td>${healthBadge(p.isConnectionHealthy)}</td>
            <td class="small text-muted">${p.lastConnectionTest ? formatDateTime(p.lastConnectionTest) : '-'}</td>
            <td>
                <div class="btn-group btn-group-sm">
                    <button class="btn btn-outline-secondary" onclick="testConnection(${p.id})" title="Test Connection"><i class="bi bi-plug"></i></button>
                    <button class="btn btn-outline-primary" onclick="openEdit(${p.id})" title="Edit"><i class="bi bi-pencil"></i></button>
                    <button class="btn btn-outline-danger" onclick="openDelete(${p.id}, '${esc(typeName)} on ${esc(serverName)}')" title="Delete"><i class="bi bi-trash"></i></button>
                </div>
            </td>
        </tr>
    `}).join('');
}

function renderPagination() {
    const total = Math.ceil(filteredPanels.length / pageSize);
    const info = document.getElementById('paginationInfo');
    const pag = document.getElementById('pagination');

    const start = Math.min((currentPage - 1) * pageSize + 1, filteredPanels.length);
    const end = Math.min(currentPage * pageSize, filteredPanels.length);
    info.textContent = filteredPanels.length > 0 ? `Showing ${start}–${end} of ${filteredPanels.length}` : 'No results';

    if (total <= 1) { pag.innerHTML = ''; return; }

    let html = `<li class="page-item ${currentPage === 1 ? 'disabled' : ''}">
        <a class="page-link" href="#" onclick="changePage(${currentPage - 1}); return false;">Previous</a></li>`;
    for (let i = 1; i <= total; i++) {
        if (i === 1 || i === total || (i >= currentPage - 2 && i <= currentPage + 2)) {
            html += `<li class="page-item ${i === currentPage ? 'active' : ''}">
                <a class="page-link" href="#" onclick="changePage(${i}); return false;">${i}</a></li>`;
        } else if (i === currentPage - 3 || i === currentPage + 3) {
            html += '<li class="page-item disabled"><span class="page-link">…</span></li>';
        }
    }
    html += `<li class="page-item ${currentPage === total ? 'disabled' : ''}">
        <a class="page-link" href="#" onclick="changePage(${currentPage + 1}); return false;">Next</a></li>`;
    pag.innerHTML = html;
}

function changePage(page) {
    const total = Math.ceil(filteredPanels.length / pageSize);
    if (page < 1 || page > total) return;
    currentPage = page;
    renderTable();
    renderPagination();
}

function openCreate() {
    editingId = null;
    document.getElementById('modalTitle').textContent = 'New Control Panel';
    document.getElementById('form').reset();
    document.getElementById('status').value = 'Active';
    document.getElementById('useHttps').checked = true;
    document.getElementById('port').value = 2087;
    // Reset IP dropdown
    const ipSelect = document.getElementById('ipAddressId');
    ipSelect.querySelectorAll('option:not([value=""])').forEach(o => o.remove());
    ipSelect.querySelector('option[value=""]').textContent = '-- None / select server first --';
    new bootstrap.Modal(document.getElementById('editModal')).show();
}

function openEdit(id) {
    const p = allPanels.find(x => x.id === id);
    if (!p) return;
    editingId = id;

    document.getElementById('modalTitle').textContent = `Edit Control Panel #${p.id}`;
    document.getElementById('serverId').value = p.serverId || '';
    document.getElementById('controlPanelTypeId').value = p.controlPanelTypeId || '';
    document.getElementById('status').value = p.status;
    document.getElementById('apiUrl').value = p.apiUrl || '';
    document.getElementById('port').value = p.port;
    document.getElementById('useHttps').checked = p.useHttps;
    document.getElementById('username').value = p.username || '';
    document.getElementById('apiToken').value = '';
    document.getElementById('apiKey').value = '';
    document.getElementById('password').value = '';
    document.getElementById('additionalSettings').value = p.additionalSettings || '';
    document.getElementById('notes').value = p.notes || '';

    // Populate and select the IP dropdown for this server
    onServerChange();
    if (p.ipAddressId) {
        document.getElementById('ipAddressId').value = p.ipAddressId;
    }

    new bootstrap.Modal(document.getElementById('editModal')).show();
}

async function savePanel() {
    const serverId = parseInt(document.getElementById('serverId').value);
    const controlPanelTypeId = parseInt(document.getElementById('controlPanelTypeId').value);
    const apiUrl = document.getElementById('apiUrl').value.trim();
    const port = parseInt(document.getElementById('port').value);

    if (!serverId) { showError('Server is required'); return; }
    if (!controlPanelTypeId) { showError('Panel Type is required'); return; }
    if (!apiUrl) { showError('API URL is required'); return; }
    if (!port || port < 1 || port > 65535) { showError('Port must be between 1 and 65535'); return; }

    const ipAddressIdRaw = parseInt(document.getElementById('ipAddressId').value);
    const apiToken = document.getElementById('apiToken').value.trim() || null;
    const apiKey = document.getElementById('apiKey').value.trim() || null;
    const password = document.getElementById('password').value || null;

    const data = {
        serverId,
        controlPanelTypeId,
        ipAddressId: ipAddressIdRaw || null,
        apiUrl,
        port,
        useHttps: document.getElementById('useHttps').checked,
        apiToken,
        apiKey,
        username: document.getElementById('username').value.trim() || null,
        password,
        additionalSettings: document.getElementById('additionalSettings').value.trim() || null,
        status: document.getElementById('status').value,
        notes: document.getElementById('notes').value.trim() || null,
    };

    const res = editingId
        ? await window.ServerControlPanelAPI.update(editingId, data)
        : await window.ServerControlPanelAPI.create(data);

    if (res.success) {
        bootstrap.Modal.getInstance(document.getElementById('editModal')).hide();
        showSuccess(editingId ? 'Control panel updated successfully' : 'Control panel created successfully');
        loadPanels();
    } else {
        showError(res.message || 'Save failed');
    }
}

function openDelete(id, label) {
    pendingDeleteId = id;
    document.getElementById('deleteName').textContent = label;
    new bootstrap.Modal(document.getElementById('deleteModal')).show();
}

async function doDelete() {
    if (!pendingDeleteId) return;
    const res = await window.ServerControlPanelAPI.delete(pendingDeleteId);
    bootstrap.Modal.getInstance(document.getElementById('deleteModal')).hide();
    if (res.success) {
        showSuccess('Control panel deleted successfully');
        loadPanels();
    } else {
        showError(res.message || 'Delete failed');
    }
    pendingDeleteId = null;
}

async function testConnection(id) {
    showSuccess('Testing connection…');
    const res = await window.ServerControlPanelAPI.testConnection(id);
    if (res.success && res.data?.success) {
        showSuccess('Connection test successful');
    } else {
        showError('Connection test failed');
    }
    // Reload to reflect updated health status
    loadPanels();
}

function statusBadge(status) {
    const map = { Active: 'success', Inactive: 'secondary', Error: 'danger' };
    const bg = map[status] || 'secondary';
    return `<span class="badge bg-${bg}">${esc(status)}</span>`;
}

function healthBadge(isHealthy) {
    if (isHealthy === true) return '<span class="badge bg-success"><i class="bi bi-check-circle"></i> Healthy</span>';
    if (isHealthy === false) return '<span class="badge bg-danger"><i class="bi bi-x-circle"></i> Unhealthy</span>';
    return '<span class="text-muted">–</span>';
}

function formatDateTime(dateString) {
    if (!dateString) return '-';
    const d = new Date(dateString);
    return d.toLocaleString('en-US', { month: 'short', day: 'numeric', hour: '2-digit', minute: '2-digit' });
}

function esc(text) {
    const map = { '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#039;' };
    return (text || '').toString().replace(/[&<>"']/g, m => map[m]);
}

function showSuccess(msg) {
    const a = document.getElementById('alertSuccess');
    a.textContent = msg;
    a.classList.remove('d-none');
    document.getElementById('alertError').classList.add('d-none');
    setTimeout(() => a.classList.add('d-none'), 5000);
}

function showError(msg) {
    const a = document.getElementById('alertError');
    a.textContent = msg;
    a.classList.remove('d-none');
    document.getElementById('alertSuccess').classList.add('d-none');
}

window.openCreate = openCreate;
window.openEdit = openEdit;
window.openDelete = openDelete;
window.testConnection = testConnection;
window.changePage = changePage;

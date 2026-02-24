/**
 * Servers Management Page
 * Full CRUD with filtering, pagination, and FK dropdowns
 */

let allServers = [];
let filteredServers = [];
let serverTypes = [];
let hostProviders = [];
let operatingSystems = [];
let editingId = null;
let pendingDeleteId = null;
let currentPage = 1;
const pageSize = 15;
let searchTerm = '';
let statusFilter = '';
let typeFilter = '';

document.addEventListener('DOMContentLoaded', async () => {
    checkAuthState();
    await loadLookupData();
    await loadServers();
    initFilters();
    document.getElementById('saveBtn').addEventListener('click', saveServer);
    document.getElementById('confirmDelete').addEventListener('click', () => doDelete());
});

async function loadLookupData() {
    const [typesRes, providersRes, osRes] = await Promise.all([
        window.ServerTypeAPI.getActive(),
        window.HostProviderAPI.getActive(),
        window.OperatingSystemAPI.getActive(),
    ]);

    if (typesRes.success) serverTypes = Array.isArray(typesRes.data) ? typesRes.data : [];
    if (providersRes.success) hostProviders = Array.isArray(providersRes.data) ? providersRes.data : [];
    if (osRes.success) operatingSystems = Array.isArray(osRes.data) ? osRes.data : [];

    populateDropdowns();
    populateTypeFilter();
}

function populateDropdowns() {
    const typeSelect = document.getElementById('serverTypeId');
    const existingTypeOptions = typeSelect.querySelectorAll('option:not([value=""])');
    existingTypeOptions.forEach(o => o.remove());
    serverTypes.forEach(t => {
        typeSelect.add(new Option(t.displayName, t.id));
    });

    const providerSelect = document.getElementById('hostProviderId');
    const existingProviderOptions = providerSelect.querySelectorAll('option:not([value=""])');
    existingProviderOptions.forEach(o => o.remove());
    hostProviders.forEach(p => {
        providerSelect.add(new Option(p.displayName, p.id));
    });

    const osSelect = document.getElementById('operatingSystemId');
    const existingOsOptions = osSelect.querySelectorAll('option:not([value=""])');
    existingOsOptions.forEach(o => o.remove());
    operatingSystems.forEach(os => {
        osSelect.add(new Option(os.displayName, os.id));
    });
}

function populateTypeFilter() {
    const typeFilter = document.getElementById('typeFilter');
    const existingOptions = typeFilter.querySelectorAll('option:not([value=""])');
    existingOptions.forEach(o => o.remove());
    serverTypes.forEach(t => {
        typeFilter.add(new Option(t.displayName, t.id));
    });
}

async function loadServers() {
    const tbody = document.getElementById('tableBody');
    tbody.innerHTML = '<tr><td colspan="11" class="text-center p-4"><div class="spinner-border text-primary"></div></td></tr>';

    const res = await window.ServerAPI.getAll();
    if (!res.success) {
        showError(res.message || 'Failed to load servers');
        tbody.innerHTML = '<tr><td colspan="11" class="text-center text-danger p-4">Failed to load data</td></tr>';
        return;
    }

    allServers = Array.isArray(res.data) ? res.data : [];
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

    document.getElementById('statusFilter').addEventListener('change', e => {
        statusFilter = e.target.value;
        applyFiltersAndRender();
    });

    document.getElementById('typeFilter').addEventListener('change', e => {
        typeFilter = e.target.value;
        applyFiltersAndRender();
    });

    document.getElementById('clearFilters').addEventListener('click', () => {
        document.getElementById('searchInput').value = '';
        document.getElementById('statusFilter').value = '';
        document.getElementById('typeFilter').value = '';
        searchTerm = '';
        statusFilter = '';
        typeFilter = '';
        applyFiltersAndRender();
    });
}

function applyFiltersAndRender() {
    filteredServers = allServers.filter(s => {
        if (searchTerm && !s.name?.toLowerCase().includes(searchTerm) && !s.location?.toLowerCase().includes(searchTerm)) return false;
        if (statusFilter === 'true' && s.status !== true) return false;
        if (statusFilter === 'false' && s.status !== false) return false;
        if (typeFilter && s.serverTypeId !== parseInt(typeFilter)) return false;
        return true;
    });

    currentPage = 1;
    document.getElementById('serverCount').textContent = `${filteredServers.length} server(s)`;
    renderTable();
    renderPagination();
}

function renderTable() {
    const tbody = document.getElementById('tableBody');
    if (!filteredServers.length) {
        tbody.innerHTML = '<tr><td colspan="11" class="text-center text-muted p-4">No servers found.</td></tr>';
        return;
    }

    const start = (currentPage - 1) * pageSize;
    const page = filteredServers.slice(start, start + pageSize);

    tbody.innerHTML = page.map(s => `
        <tr>
            <td>${s.id}</td>
            <td><strong>${esc(s.name)}</strong></td>
            <td>${esc(s.serverTypeName || '-')}</td>
            <td>${esc(s.hostProviderName || '-')}</td>
            <td>${esc(s.location || '-')}</td>
            <td>${esc(s.operatingSystemName || '-')}</td>
            <td>${statusBadge(s.status)}</td>
            <td>${s.cpuCores != null ? s.cpuCores + ' core' + (s.cpuCores !== 1 ? 's' : '') : '-'}</td>
            <td>${s.ramMB != null ? formatRam(s.ramMB) : '-'}</td>
            <td>${s.diskSpaceGB != null ? s.diskSpaceGB + ' GB' : '-'}</td>
            <td>
                <div class="btn-group btn-group-sm">
                    <button class="btn btn-outline-secondary" onclick="location.href='/server-ip-addresses.html?serverId=${s.id}'" title="IP Addresses"><i class="bi bi-router"></i></button>
                    <button class="btn btn-outline-primary" onclick="openEdit(${s.id})" title="Edit"><i class="bi bi-pencil"></i></button>
                    <button class="btn btn-outline-danger" onclick="openDelete(${s.id}, '${esc(s.name)}')" title="Delete"><i class="bi bi-trash"></i></button>
                </div>
            </td>
        </tr>
    `).join('');
}

function renderPagination() {
    const total = Math.ceil(filteredServers.length / pageSize);
    const info = document.getElementById('paginationInfo');
    const pag = document.getElementById('pagination');

    const start = Math.min((currentPage - 1) * pageSize + 1, filteredServers.length);
    const end = Math.min(currentPage * pageSize, filteredServers.length);
    info.textContent = filteredServers.length > 0 ? `Showing ${start}–${end} of ${filteredServers.length}` : 'No results';

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
    const total = Math.ceil(filteredServers.length / pageSize);
    if (page < 1 || page > total) return;
    currentPage = page;
    renderTable();
    renderPagination();
}

function openCreate() {
    editingId = null;
    document.getElementById('modalTitle').textContent = 'New Server';
    document.getElementById('form').reset();
    document.getElementById('status').checked = true;
    new bootstrap.Modal(document.getElementById('editModal')).show();
}

function openEdit(id) {
    const s = allServers.find(x => x.id === id);
    if (!s) return;
    editingId = id;
    document.getElementById('modalTitle').textContent = `Edit Server – ${s.name}`;
    document.getElementById('name').value = s.name;
    document.getElementById('status').checked = s.status === true;
    document.getElementById('serverTypeId').value = s.serverTypeId || '';
    document.getElementById('hostProviderId').value = s.hostProviderId || '';
    document.getElementById('operatingSystemId').value = s.operatingSystemId || '';
    document.getElementById('location').value = s.location || '';
    document.getElementById('cpuCores').value = s.cpuCores ?? '';
    document.getElementById('ramMB').value = s.ramMB ?? '';
    document.getElementById('diskSpaceGB').value = s.diskSpaceGB ?? '';
    document.getElementById('notes').value = s.notes || '';
    new bootstrap.Modal(document.getElementById('editModal')).show();
}

async function saveServer() {
    const name = document.getElementById('name').value.trim();
    const serverTypeId = parseInt(document.getElementById('serverTypeId').value);
    const operatingSystemId = parseInt(document.getElementById('operatingSystemId').value);
    const status = document.getElementById('status').checked;

    if (!name) { showError('Server name is required'); return; }
    if (!serverTypeId) { showError('Server Type is required'); return; }
    if (!operatingSystemId) { showError('Operating System is required'); return; }

    const hostProviderId = parseInt(document.getElementById('hostProviderId').value) || null;
    const cpuCores = parseInt(document.getElementById('cpuCores').value) || null;
    const ramMB = parseInt(document.getElementById('ramMB').value) || null;
    const diskSpaceGB = parseInt(document.getElementById('diskSpaceGB').value) || null;

    const data = {
        name,
        serverTypeId,
        hostProviderId,
        operatingSystemId,
        location: document.getElementById('location').value.trim() || null,
        status,
        cpuCores,
        ramMB,
        diskSpaceGB,
        notes: document.getElementById('notes').value.trim() || null,
    };

    const res = editingId
        ? await window.ServerAPI.update(editingId, data)
        : await window.ServerAPI.create(data);

    if (res.success) {
        bootstrap.Modal.getInstance(document.getElementById('editModal')).hide();
        showSuccess(editingId ? 'Server updated successfully' : 'Server created successfully');
        loadServers();
    } else {
        showError(res.message || 'Save failed');
    }
}

function openDelete(id, name) {
    pendingDeleteId = id;
    document.getElementById('deleteName').textContent = name;
    new bootstrap.Modal(document.getElementById('deleteModal')).show();
}

async function doDelete() {
    if (!pendingDeleteId) return;
    const res = await window.ServerAPI.delete(pendingDeleteId);
    bootstrap.Modal.getInstance(document.getElementById('deleteModal')).hide();
    if (res.success) {
        showSuccess('Server deleted successfully');
        loadServers();
    } else {
        showError(res.message || 'Delete failed');
    }
    pendingDeleteId = null;
}

function statusBadge(status) {
    let text, bg;
    if (status === true) {
        text = 'Active';
        bg = 'success';
    } else if (status === false) {
        text = 'Inactive';
        bg = 'secondary';
    } else {
        text = 'Unknown';
        bg = 'warning';
    }
    return `<span class="badge bg-${bg}">${text}</span>`;
}

function formatRam(mb) {
    if (mb >= 1024) return (mb / 1024).toFixed(mb % 1024 === 0 ? 0 : 1) + ' GB';
    return mb + ' MB';
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
window.changePage = changePage;

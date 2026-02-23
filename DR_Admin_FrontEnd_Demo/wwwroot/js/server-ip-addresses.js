/**
 * Server IP Addresses Management Page
 * Full CRUD with server filter, IP version / status filters, and pagination
 */

let allIpAddresses = [];
let filteredIpAddresses = [];
let servers = [];
let editingId = null;
let pendingDeleteId = null;
let currentPage = 1;
const pageSize = 20;
let serverFilter = '';
let versionFilter = '';
let statusFilter = '';

document.addEventListener('DOMContentLoaded', async () => {
    checkAuthState();
    await loadServers();
    initFilters();
    document.getElementById('saveBtn').addEventListener('click', saveIpAddress);
    document.getElementById('confirmDelete').addEventListener('click', () => doDelete());

    // Support ?serverId=X in URL to pre-filter by server
    const params = new URLSearchParams(window.location.search);
    const serverIdParam = params.get('serverId');
    if (serverIdParam) {
        document.getElementById('serverFilter').value = serverIdParam;
        serverFilter = serverIdParam;
    }

    await loadIpAddresses();
});

async function loadServers() {
    const res = await window.ServerAPI.getAll();
    servers = res.success && Array.isArray(res.data) ? res.data : [];

    const filterSelect = document.getElementById('serverFilter');
    servers.forEach(s => filterSelect.add(new Option(s.name, s.id)));

    const modalSelect = document.getElementById('serverId');
    servers.forEach(s => modalSelect.add(new Option(s.name, s.id)));
}

async function loadIpAddresses() {
    const tbody = document.getElementById('tableBody');
    tbody.innerHTML = '<tr><td colspan="7" class="text-center p-4"><div class="spinner-border text-primary"></div></td></tr>';

    const res = serverFilter
        ? await window.ServerIpAddressAPI.getByServerId(parseInt(serverFilter))
        : await window.ServerIpAddressAPI.getAll();

    if (!res.success) {
        showError(res.message || 'Failed to load IP addresses');
        tbody.innerHTML = '<tr><td colspan="7" class="text-center text-danger p-4">Failed to load data</td></tr>';
        return;
    }

    allIpAddresses = Array.isArray(res.data) ? res.data : [];
    applyFiltersAndRender();
}

function initFilters() {
    document.getElementById('serverFilter').addEventListener('change', e => {
        serverFilter = e.target.value;
        loadIpAddresses();
    });

    document.getElementById('versionFilter').addEventListener('change', e => {
        versionFilter = e.target.value;
        applyFiltersAndRender();
    });

    document.getElementById('statusFilter').addEventListener('change', e => {
        statusFilter = e.target.value;
        applyFiltersAndRender();
    });

    document.getElementById('clearFilters').addEventListener('click', () => {
        document.getElementById('serverFilter').value = '';
        document.getElementById('versionFilter').value = '';
        document.getElementById('statusFilter').value = '';
        serverFilter = '';
        versionFilter = '';
        statusFilter = '';
        loadIpAddresses();
    });
}

function applyFiltersAndRender() {
    filteredIpAddresses = allIpAddresses.filter(ip => {
        if (versionFilter && ip.ipVersion !== versionFilter) return false;
        if (statusFilter && ip.status !== statusFilter) return false;
        return true;
    });

    currentPage = 1;
    document.getElementById('ipCount').textContent = `${filteredIpAddresses.length} IP address(es)`;
    renderTable();
    renderPagination();
}

function getServerName(serverId) {
    const s = servers.find(x => x.id === serverId);
    return s ? s.name : String(serverId);
}

function renderTable() {
    const tbody = document.getElementById('tableBody');
    if (!filteredIpAddresses.length) {
        tbody.innerHTML = '<tr><td colspan="7" class="text-center text-muted p-4">No IP addresses found. Click "New IP Address" to add one.</td></tr>';
        return;
    }

    const start = (currentPage - 1) * pageSize;
    const page = filteredIpAddresses.slice(start, start + pageSize);

    tbody.innerHTML = page.map(ip => `
        <tr>
            <td>${ip.id}</td>
            <td>
                <code>${esc(ip.ipAddress)}</code>
                ${ip.isPrimary ? '<span class="badge bg-primary ms-1">Primary</span>' : ''}
            </td>
            <td><span class="badge bg-secondary">${esc(ip.ipVersion)}</span></td>
            <td>${esc(getServerName(ip.serverId))}</td>
            <td>${statusBadge(ip.status)}</td>
            <td>${esc(ip.assignedTo || '-')}</td>
            <td>
                <div class="btn-group btn-group-sm">
                    <button class="btn btn-outline-primary" onclick="openEdit(${ip.id})" title="Edit"><i class="bi bi-pencil"></i></button>
                    <button class="btn btn-outline-danger" onclick="openDelete(${ip.id}, '${esc(ip.ipAddress)}')" title="Delete"><i class="bi bi-trash"></i></button>
                </div>
            </td>
        </tr>
    `).join('');
}

function renderPagination() {
    const total = Math.ceil(filteredIpAddresses.length / pageSize);
    const info = document.getElementById('paginationInfo');
    const pag = document.getElementById('pagination');

    const start = Math.min((currentPage - 1) * pageSize + 1, filteredIpAddresses.length);
    const end = Math.min(currentPage * pageSize, filteredIpAddresses.length);
    info.textContent = filteredIpAddresses.length > 0 ? `Showing ${start}–${end} of ${filteredIpAddresses.length}` : 'No results';

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
    const total = Math.ceil(filteredIpAddresses.length / pageSize);
    if (page < 1 || page > total) return;
    currentPage = page;
    renderTable();
    renderPagination();
}

function openCreate() {
    editingId = null;
    document.getElementById('modalTitle').textContent = 'New Server IP Address';
    document.getElementById('form').reset();
    document.getElementById('ipVersion').value = 'IPv4';
    document.getElementById('status').value = 'Active';
    document.getElementById('serverId').disabled = false;
    if (serverFilter) document.getElementById('serverId').value = serverFilter;
    new bootstrap.Modal(document.getElementById('editModal')).show();
}

function openEdit(id) {
    const ip = allIpAddresses.find(x => x.id === id);
    if (!ip) return;
    editingId = id;
    document.getElementById('modalTitle').textContent = `Edit IP – ${ip.ipAddress}`;
    document.getElementById('serverId').value = ip.serverId;
    document.getElementById('serverId').disabled = true;
    document.getElementById('ipAddress').value = ip.ipAddress;
    document.getElementById('ipVersion').value = ip.ipVersion;
    document.getElementById('isPrimary').checked = ip.isPrimary;
    document.getElementById('status').value = ip.status;
    document.getElementById('assignedTo').value = ip.assignedTo || '';
    document.getElementById('notes').value = ip.notes || '';
    new bootstrap.Modal(document.getElementById('editModal')).show();
}

async function saveIpAddress() {
    const ipAddress = document.getElementById('ipAddress').value.trim();
    const ipVersion = document.getElementById('ipVersion').value;
    const status = document.getElementById('status').value;

    if (!ipAddress) { showError('IP address is required'); return; }

    let res;
    if (editingId) {
        res = await window.ServerIpAddressAPI.update(editingId, {
            ipAddress,
            ipVersion,
            isPrimary: document.getElementById('isPrimary').checked,
            status,
            assignedTo: document.getElementById('assignedTo').value.trim() || null,
            notes: document.getElementById('notes').value.trim() || null,
        });
    } else {
        const serverId = parseInt(document.getElementById('serverId').value);
        if (!serverId) { showError('Please select a server'); return; }
        res = await window.ServerIpAddressAPI.create({
            serverId,
            ipAddress,
            ipVersion,
            isPrimary: document.getElementById('isPrimary').checked,
            status,
            assignedTo: document.getElementById('assignedTo').value.trim() || null,
            notes: document.getElementById('notes').value.trim() || null,
        });
    }

    if (res.success) {
        bootstrap.Modal.getInstance(document.getElementById('editModal')).hide();
        showSuccess(editingId ? 'IP address updated successfully' : 'IP address created successfully');
        loadIpAddresses();
    } else {
        showError(res.message || 'Save failed');
    }
}

function openDelete(id, ipAddr) {
    pendingDeleteId = id;
    document.getElementById('deleteName').textContent = ipAddr;
    new bootstrap.Modal(document.getElementById('deleteModal')).show();
}

async function doDelete() {
    if (!pendingDeleteId) return;
    const res = await window.ServerIpAddressAPI.delete(pendingDeleteId);
    bootstrap.Modal.getInstance(document.getElementById('deleteModal')).hide();
    if (res.success) {
        showSuccess('IP address deleted successfully');
        loadIpAddresses();
    } else {
        showError(res.message || 'Delete failed');
    }
    pendingDeleteId = null;
}

function statusBadge(status) {
    const map = { Active: 'success', Reserved: 'warning', Blocked: 'danger' };
    const bg = map[status] || 'secondary';
    return `<span class="badge bg-${bg}">${esc(status)}</span>`;
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

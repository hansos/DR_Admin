// dns-zone-templates.js — DNS Zone Templates Management Page

let allTemplates = [];
let filteredTemplates = [];
let allControlPanels = [];
let allServers = [];
let allDnsRecordTypes = [];
let allDomains = [];

let editingId = null;
let pendingDeleteId = null;
let activePackageId = null;   // used by records & assignment modals
let activePackageData = null; // full package with assignments

let currentPage = 1;
const pageSize = 15;

// ── Bootstrap modal references ────────────────────────────────────────────
let editModalBs, recordsModalBs, assignModalBs, applyModalBs, deleteModalBs;

document.addEventListener('DOMContentLoaded', async () => {
    checkAuthState();

    editModalBs    = new bootstrap.Modal(document.getElementById('editModal'));
    recordsModalBs = new bootstrap.Modal(document.getElementById('recordsModal'));
    assignModalBs  = new bootstrap.Modal(document.getElementById('assignModal'));
    applyModalBs   = new bootstrap.Modal(document.getElementById('applyModal'));
    deleteModalBs  = new bootstrap.Modal(document.getElementById('deleteModal'));

    await Promise.all([loadLookupData(), loadTemplates()]);
    initFilters();
    bindButtons();
});

// ── Data loading ──────────────────────────────────────────────────────────

async function loadLookupData() {
    const [cpRes, srvRes, rtRes, domRes] = await Promise.all([
        window.ServerControlPanelAPI.getAll(),
        window.ServerAPI.getAll(),
        window.DnsRecordTypeAPI.getAll(),
        window.RegisteredDomainAPI.getAll(),
    ]);

    if (cpRes.success)  allControlPanels  = Array.isArray(cpRes.data)  ? cpRes.data  : [];
    if (srvRes.success) allServers         = Array.isArray(srvRes.data) ? srvRes.data : [];
    if (rtRes.success)  allDnsRecordTypes  = Array.isArray(rtRes.data)  ? rtRes.data  : [];
    if (domRes.success) allDomains         = Array.isArray(domRes.data) ? domRes.data : [];

    populateRecordTypeDropdown();
    populateDomainDropdown();
    populateCpDropdown();
    populateSrvDropdown();
}

async function loadTemplates() {
    const tbody = document.getElementById('tableBody');
    tbody.innerHTML = '<tr><td colspan="10" class="text-center p-4"><div class="spinner-border text-primary"></div></td></tr>';

    const res = await window.DnsZonePackageAPI.getWithRecords();
    if (!res.success) {
        showError(res.message || 'Failed to load DNS zone templates');
        tbody.innerHTML = '<tr><td colspan="10" class="text-center text-danger p-4">Failed to load data.</td></tr>';
        return;
    }

    allTemplates = Array.isArray(res.data) ? res.data : [];
    applyFiltersAndRender();
}

// ── Filters ───────────────────────────────────────────────────────────────

function initFilters() {
    let t;
    document.getElementById('searchInput').addEventListener('input', e => {
        clearTimeout(t);
        t = setTimeout(() => applyFiltersAndRender(), 300);
    });
    document.getElementById('statusFilter').addEventListener('change', applyFiltersAndRender);
    document.getElementById('defaultFilter').addEventListener('change', applyFiltersAndRender);
    document.getElementById('clearFilters').addEventListener('click', () => {
        document.getElementById('searchInput').value = '';
        document.getElementById('statusFilter').value = '';
        document.getElementById('defaultFilter').checked = false;
        applyFiltersAndRender();
    });
}

function applyFiltersAndRender() {
    const q      = document.getElementById('searchInput').value.toLowerCase();
    const status = document.getElementById('statusFilter').value;
    const defOnly = document.getElementById('defaultFilter').checked;

    filteredTemplates = allTemplates.filter(t => {
        if (q && !t.name?.toLowerCase().includes(q)) return false;
        if (status === 'active'   && !t.isActive)  return false;
        if (status === 'inactive' &&  t.isActive)  return false;
        if (defOnly && !t.isDefault) return false;
        return true;
    });

    currentPage = 1;
    document.getElementById('templateCount').textContent = `${filteredTemplates.length} template(s)`;
    renderTable();
    renderPagination();
}

// ── Table rendering ───────────────────────────────────────────────────────

function renderTable() {
    const tbody = document.getElementById('tableBody');
    if (!filteredTemplates.length) {
        tbody.innerHTML = '<tr><td colspan="10" class="text-center text-muted p-4">No DNS zone templates found.</td></tr>';
        return;
    }

    const start = (currentPage - 1) * pageSize;
    const page  = filteredTemplates.slice(start, start + pageSize);

    tbody.innerHTML = page.map(t => {
        const recordCount = t.records?.length ?? 0;
        const cpCount     = t.controlPanels?.length ?? 0;
        const srvCount    = t.servers?.length ?? 0;
        const statusBadge = t.isActive
            ? '<span class="badge bg-success">Active</span>'
            : '<span class="badge bg-secondary">Inactive</span>';
        const defaultBadge = t.isDefault
            ? '<span class="badge bg-warning text-dark"><i class="bi bi-star-fill"></i> Default</span>'
            : '<span class="text-muted">—</span>';

        return `<tr>
            <td>${t.id}</td>
            <td class="fw-semibold">${esc(t.name)}</td>
            <td class="text-muted small text-truncate" style="max-width:200px">${esc(t.description || '—')}</td>
            <td class="text-center">
                <button class="btn btn-sm btn-outline-primary" onclick="openRecords(${t.id})" title="Manage records">
                    <i class="bi bi-list-ul"></i> ${recordCount}
                </button>
            </td>
            <td class="text-center">
                <button class="btn btn-sm btn-outline-secondary" onclick="openAssignments(${t.id})" title="Manage control panel assignments">
                    <i class="bi bi-speedometer2"></i> ${cpCount}
                </button>
            </td>
            <td class="text-center">
                <button class="btn btn-sm btn-outline-secondary" onclick="openAssignments(${t.id})" title="Manage server assignments">
                    <i class="bi bi-server"></i> ${srvCount}
                </button>
            </td>
            <td>${t.sortOrder}</td>
            <td>${statusBadge}</td>
            <td>${defaultBadge}</td>
            <td>
                <div class="btn-group btn-group-sm">
                    <button class="btn btn-outline-success" onclick="openApply(${t.id}, '${esc(t.name)}')" title="Apply to domain">
                        <i class="bi bi-arrow-right-circle"></i>
                    </button>
                    <button class="btn btn-outline-primary" onclick="openEdit(${t.id})" title="Edit">
                        <i class="bi bi-pencil"></i>
                    </button>
                    <button class="btn btn-outline-danger" onclick="openDelete(${t.id}, '${esc(t.name)}')" title="Delete">
                        <i class="bi bi-trash"></i>
                    </button>
                </div>
            </td>
        </tr>`;
    }).join('');
}

function renderPagination() {
    const total = Math.ceil(filteredTemplates.length / pageSize);
    const info  = document.getElementById('paginationInfo');
    const pag   = document.getElementById('pagination');

    const start = Math.min((currentPage - 1) * pageSize + 1, filteredTemplates.length);
    const end   = Math.min(currentPage * pageSize, filteredTemplates.length);
    info.textContent = filteredTemplates.length > 0 ? `Showing ${start}–${end} of ${filteredTemplates.length}` : 'No results';

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
    const total = Math.ceil(filteredTemplates.length / pageSize);
    if (page < 1 || page > total) return;
    currentPage = page;
    renderTable();
    renderPagination();
}

// ── Create / Edit Modal ───────────────────────────────────────────────────

function openCreate() {
    editingId = null;
    document.getElementById('editModalTitle').textContent = 'New DNS Zone Template';
    document.getElementById('editForm').reset();
    document.getElementById('fIsActive').checked = true;
    document.getElementById('fIsDefault').checked = false;
    document.getElementById('fSortOrder').value = 0;
    document.getElementById('editModalAlert').classList.add('d-none');
    editModalBs.show();
}

function openEdit(id) {
    const t = allTemplates.find(x => x.id === id);
    if (!t) return;
    editingId = id;
    document.getElementById('editModalTitle').textContent = `Edit Template — ${esc(t.name)}`;
    document.getElementById('fName').value        = t.name || '';
    document.getElementById('fDescription').value = t.description || '';
    document.getElementById('fSortOrder').value   = t.sortOrder ?? 0;
    document.getElementById('fIsActive').checked  = t.isActive;
    document.getElementById('fIsDefault').checked = t.isDefault;
    document.getElementById('editModalAlert').classList.add('d-none');
    editModalBs.show();
}

async function saveTemplate() {
    const name = document.getElementById('fName').value.trim();
    if (!name) {
        showModalError('editModalAlert', 'Name is required.');
        return;
    }

    const data = {
        name,
        description: document.getElementById('fDescription').value.trim() || null,
        sortOrder:   parseInt(document.getElementById('fSortOrder').value) || 0,
        isActive:    document.getElementById('fIsActive').checked,
        isDefault:   document.getElementById('fIsDefault').checked,
    };

    const btn = document.getElementById('saveBtn');
    btn.disabled = true;
    btn.innerHTML = '<span class="spinner-border spinner-border-sm"></span> Saving…';

    try {
        const res = editingId
            ? await window.DnsZonePackageAPI.update(editingId, data)
            : await window.DnsZonePackageAPI.create(data);

        if (res.success) {
            editModalBs.hide();
            showSuccess(editingId ? 'Template updated.' : 'Template created.');
            await loadTemplates();
        } else {
            showModalError('editModalAlert', res.message || 'Save failed.');
        }
    } finally {
        btn.disabled = false;
        btn.innerHTML = '<i class="bi bi-save"></i> Save';
    }
}

// ── Records Modal ─────────────────────────────────────────────────────────

async function openRecords(packageId) {
    activePackageId = packageId;
    const t = allTemplates.find(x => x.id === packageId);
    document.getElementById('recordsModalTitle').textContent = t?.name || `Template #${packageId}`;
    document.getElementById('recordsModalAlert').classList.add('d-none');
    resetAddRecordForm();
    renderRecordsTable(t?.records || []);
    recordsModalBs.show();
}

function populateRecordTypeDropdown() {
    const sel = document.getElementById('rType');
    sel.innerHTML = '<option value="">-- Type --</option>';
    allDnsRecordTypes.forEach(rt => {
        sel.add(new Option(rt.type, rt.id));
    });
}

function resetAddRecordForm() {
    ['rType', 'rName', 'rValue', 'rPriority', 'rWeight', 'rPort', 'rNotes'].forEach(id => {
        const el = document.getElementById(id);
        if (el) el.value = '';
    });
    document.getElementById('rTTL').value = 3600;
}

function renderRecordsTable(records) {
    const tbody = document.getElementById('recordsTableBody');
    if (!records.length) {
        tbody.innerHTML = '<tr><td colspan="9" class="text-center text-muted py-3">No records. Add one above.</td></tr>';
        return;
    }

    const typesMap = Object.fromEntries(allDnsRecordTypes.map(rt => [rt.id, rt.type]));
    tbody.innerHTML = records.map(r => `
        <tr>
            <td><span class="badge bg-primary">${esc(typesMap[r.dnsRecordTypeId] || r.dnsRecordTypeId)}</span></td>
            <td><code class="small">${esc(r.name)}</code></td>
            <td class="text-break" style="max-width:200px"><code class="small">${esc(r.value)}</code></td>
            <td>${r.ttl ?? r.tTL ?? 3600}</td>
            <td>${r.priority ?? '—'}</td>
            <td>${r.weight ?? '—'}</td>
            <td>${r.port ?? '—'}</td>
            <td class="small text-muted">${esc(r.notes || '')}</td>
            <td>
                <button class="btn btn-sm btn-outline-danger" onclick="deleteRecord(${r.id})" title="Remove record">
                    <i class="bi bi-trash"></i>
                </button>
            </td>
        </tr>
    `).join('');
}

async function addRecord() {
    const typeId = parseInt(document.getElementById('rType').value);
    const name   = document.getElementById('rName').value.trim();
    const value  = document.getElementById('rValue').value.trim();
    const ttl    = parseInt(document.getElementById('rTTL').value) || 3600;

    if (!typeId || !name || !value) {
        showModalError('recordsModalAlert', 'Type, Name and Value are required.');
        return;
    }

    const data = {
        dnsZonePackageId: activePackageId,
        dnsRecordTypeId: typeId,
        name, value, ttl,
        priority: parseInt(document.getElementById('rPriority').value) || null,
        weight:   parseInt(document.getElementById('rWeight').value)   || null,
        port:     parseInt(document.getElementById('rPort').value)     || null,
        notes:    document.getElementById('rNotes').value.trim()       || null,
    };

    const btn = document.getElementById('addRecordBtn');
    btn.disabled = true;
    try {
        const res = await window.DnsZonePackageRecordAPI.create(data);
        if (res.success) {
            document.getElementById('recordsModalAlert').classList.add('d-none');
            resetAddRecordForm();
            await reloadTemplatesAndRefreshRecordsModal();
        } else {
            showModalError('recordsModalAlert', res.message || 'Failed to add record.');
        }
    } finally {
        btn.disabled = false;
    }
}

async function deleteRecord(recordId) {
    if (!confirm('Remove this record from the template?')) return;
    const res = await window.DnsZonePackageRecordAPI.delete(recordId);
    if (res.success) {
        await reloadTemplatesAndRefreshRecordsModal();
    } else {
        showModalError('recordsModalAlert', res.message || 'Failed to delete record.');
    }
}

async function reloadTemplatesAndRefreshRecordsModal() {
    await loadTemplates();
    const updated = allTemplates.find(x => x.id === activePackageId);
    renderRecordsTable(updated?.records || []);
}

// ── Assignments Modal ─────────────────────────────────────────────────────

async function openAssignments(packageId) {
    activePackageId = packageId;
    const t = allTemplates.find(x => x.id === packageId);
    document.getElementById('assignModalTitle').textContent = t?.name || `Template #${packageId}`;
    document.getElementById('assignModalAlert').classList.add('d-none');

    await refreshAssignments();
    assignModalBs.show();
}

async function refreshAssignments() {
    const res = await window.DnsZonePackageAPI.getWithAssignments(activePackageId);
    if (!res.success) {
        showModalError('assignModalAlert', 'Failed to load assignments.');
        return;
    }
    activePackageData = res.data;
    renderCpTable(activePackageData.controlPanels || []);
    renderSrvTable(activePackageData.servers || []);

    document.getElementById('cpBadge').textContent  = (activePackageData.controlPanels || []).length;
    document.getElementById('srvBadge').textContent = (activePackageData.servers || []).length;
}

function populateCpDropdown() {
    const sel = document.getElementById('cpSelect');
    sel.querySelectorAll('option:not([value=""])').forEach(o => o.remove());
    allControlPanels.forEach(cp => {
        const srvName = allServers.find(s => s.id === cp.serverId)?.name || `Server #${cp.serverId}`;
        sel.add(new Option(`#${cp.id} — ${cp.apiUrl} (${srvName})`, cp.id));
    });
}

function populateSrvDropdown() {
    const sel = document.getElementById('srvSelect');
    sel.querySelectorAll('option:not([value=""])').forEach(o => o.remove());
    allServers.forEach(s => sel.add(new Option(`#${s.id} — ${s.name} (${s.status})`, s.id)));
}

function renderCpTable(assignments) {
    const tbody = document.getElementById('cpTableBody');
    if (!assignments.length) {
        tbody.innerHTML = '<tr><td colspan="4" class="text-center text-muted py-3">No control panels assigned.</td></tr>';
        return;
    }
    tbody.innerHTML = assignments.map(a => `
        <tr>
            <td><code class="small">${esc(a.apiUrl)}</code></td>
            <td>${esc(a.serverName)}</td>
            <td><span class="badge bg-info text-dark">${esc(a.controlPanelTypeName)}</span></td>
            <td>
                <button class="btn btn-sm btn-outline-danger" onclick="removeCp(${a.controlPanelId})" title="Remove">
                    <i class="bi bi-x-circle"></i>
                </button>
            </td>
        </tr>
    `).join('');
}

function renderSrvTable(assignments) {
    const tbody = document.getElementById('srvTableBody');
    if (!assignments.length) {
        tbody.innerHTML = '<tr><td colspan="3" class="text-center text-muted py-3">No servers assigned.</td></tr>';
        return;
    }
    tbody.innerHTML = assignments.map(a => `
        <tr>
            <td class="fw-semibold">${esc(a.serverName)}</td>
            <td>${statusBadge(a.status)}</td>
            <td>
                <button class="btn btn-sm btn-outline-danger" onclick="removeSrv(${a.serverId})" title="Remove">
                    <i class="bi bi-x-circle"></i>
                </button>
            </td>
        </tr>
    `).join('');
}

async function addCp() {
    const cpId = parseInt(document.getElementById('cpSelect').value);
    if (!cpId) { showModalError('assignModalAlert', 'Please select a control panel.'); return; }
    const res = await window.DnsZonePackageAPI.assignControlPanel(activePackageId, cpId);
    if (res.success) {
        document.getElementById('assignModalAlert').classList.add('d-none');
        await refreshAssignments();
        await loadTemplates();
    } else {
        showModalError('assignModalAlert', res.message || 'Failed to assign control panel.');
    }
}

async function removeCp(cpId) {
    const res = await window.DnsZonePackageAPI.removeControlPanel(activePackageId, cpId);
    if (res.success) {
        await refreshAssignments();
        await loadTemplates();
    } else {
        showModalError('assignModalAlert', res.message || 'Failed to remove control panel.');
    }
}

async function addSrv() {
    const srvId = parseInt(document.getElementById('srvSelect').value);
    if (!srvId) { showModalError('assignModalAlert', 'Please select a server.'); return; }
    const res = await window.DnsZonePackageAPI.assignServer(activePackageId, srvId);
    if (res.success) {
        document.getElementById('assignModalAlert').classList.add('d-none');
        await refreshAssignments();
        await loadTemplates();
    } else {
        showModalError('assignModalAlert', res.message || 'Failed to assign server.');
    }
}

async function removeSrv(srvId) {
    const res = await window.DnsZonePackageAPI.removeServer(activePackageId, srvId);
    if (res.success) {
        await refreshAssignments();
        await loadTemplates();
    } else {
        showModalError('assignModalAlert', res.message || 'Failed to remove server.');
    }
}

// ── Apply to Domain Modal ─────────────────────────────────────────────────

function populateDomainDropdown() {
    const sel = document.getElementById('applyDomainSelect');
    sel.innerHTML = '<option value="">-- Select a domain --</option>';
    allDomains
        .sort((a, b) => (a.name || '').localeCompare(b.name || ''))
        .forEach(d => sel.add(new Option(d.name || `Domain #${d.id}`, d.id)));
}

function openApply(packageId, name) {
    activePackageId = packageId;
    document.getElementById('applyTemplateName').textContent = name;
    document.getElementById('applyDomainSelect').value = '';
    document.getElementById('applyModalAlert').classList.add('d-none');
    applyModalBs.show();
}

async function applyToDomain() {
    const domainId = parseInt(document.getElementById('applyDomainSelect').value);
    if (!domainId) { showModalError('applyModalAlert', 'Please select a domain.'); return; }

    const btn = document.getElementById('confirmApplyBtn');
    btn.disabled = true;
    btn.innerHTML = '<span class="spinner-border spinner-border-sm"></span> Applying…';
    try {
        const res = await window.DnsZonePackageAPI.applyToDomain(activePackageId, domainId);
        if (res.success) {
            applyModalBs.hide();
            const domainName = allDomains.find(d => d.id === domainId)?.name || `Domain #${domainId}`;
            showSuccess(`Template applied to ${domainName} successfully.`);
        } else {
            showModalError('applyModalAlert', res.message || 'Failed to apply template.');
        }
    } finally {
        btn.disabled = false;
        btn.innerHTML = '<i class="bi bi-check-circle"></i> Apply Template';
    }
}

// ── Delete Modal ──────────────────────────────────────────────────────────

function openDelete(id, name) {
    pendingDeleteId = id;
    document.getElementById('deleteName').textContent = name;
    deleteModalBs.show();
}

async function doDelete() {
    if (!pendingDeleteId) return;
    const res = await window.DnsZonePackageAPI.delete(pendingDeleteId);
    deleteModalBs.hide();
    pendingDeleteId = null;
    if (res.success) {
        showSuccess('Template deleted.');
        await loadTemplates();
    } else {
        showError(res.message || 'Delete failed.');
    }
}

// ── Wire up buttons ───────────────────────────────────────────────────────

function bindButtons() {
    document.getElementById('saveBtn').addEventListener('click', saveTemplate);
    document.getElementById('addRecordBtn').addEventListener('click', addRecord);
    document.getElementById('addCpBtn').addEventListener('click', addCp);
    document.getElementById('addSrvBtn').addEventListener('click', addSrv);
    document.getElementById('confirmApplyBtn').addEventListener('click', applyToDomain);
    document.getElementById('confirmDeleteBtn').addEventListener('click', doDelete);
}

// ── Helpers ───────────────────────────────────────────────────────────────

function statusBadge(status) {
    const map = { Active: 'success', Inactive: 'secondary', Maintenance: 'warning' };
    const bg = map[status] || 'secondary';
    const txt = (bg === 'warning') ? ' text-dark' : '';
    return `<span class="badge bg-${bg}${txt}">${esc(status || '—')}</span>`;
}

function esc(text) {
    const map = { '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#039;' };
    return (text || '').toString().replace(/[&<>"']/g, m => map[m]);
}

function showSuccess(msg) {
    const el = document.getElementById('alertSuccess');
    document.getElementById('alertSuccessMsg').textContent = msg;
    el.classList.remove('d-none');
    document.getElementById('alertError').classList.add('d-none');
    setTimeout(() => el.classList.add('d-none'), 6000);
}

function showError(msg) {
    const el = document.getElementById('alertError');
    document.getElementById('alertErrorMsg').textContent = msg;
    el.classList.remove('d-none');
    document.getElementById('alertSuccess').classList.add('d-none');
}

function showModalError(elId, msg) {
    const el = document.getElementById(elId);
    if (el) { el.textContent = msg; el.classList.remove('d-none'); }
}

// ── Global exports (called from inline onclick) ───────────────────────────
window.openCreate       = openCreate;
window.openEdit         = openEdit;
window.openRecords      = openRecords;
window.deleteRecord     = deleteRecord;
window.openAssignments  = openAssignments;
window.removeCp         = removeCp;
window.removeSrv        = removeSrv;
window.openApply        = openApply;
window.openDelete       = openDelete;
window.changePage       = changePage;

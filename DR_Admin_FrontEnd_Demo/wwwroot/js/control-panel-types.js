/**
 * Control Panel Types Management Page
 */

let allControlPanelTypes = [];
let editingId = null;
let pendingDeleteId = null;

document.addEventListener('DOMContentLoaded', () => {
    checkAuthState();
    loadControlPanelTypes();
    document.getElementById('saveBtn').addEventListener('click', saveControlPanelType);
    document.getElementById('confirmDelete').addEventListener('click', () => doDelete());
});

async function loadControlPanelTypes() {
    const tbody = document.getElementById('tableBody');
    tbody.innerHTML = '<tr><td colspan="8" class="text-center p-4"><div class="spinner-border text-primary"></div></td></tr>';

    const res = await window.ControlPanelTypeAPI.getAll();
    if (!res.success) {
        showError(res.message || 'Failed to load control panel types');
        tbody.innerHTML = '<tr><td colspan="8" class="text-center text-danger p-4">Failed to load data</td></tr>';
        return;
    }

    allControlPanelTypes = Array.isArray(res.data) ? res.data : [];
    renderTable();
}

function renderTable() {
    const tbody = document.getElementById('tableBody');
    if (!allControlPanelTypes.length) {
        tbody.innerHTML = '<tr><td colspan="8" class="text-center text-muted p-4">No control panel types found. Click "New Control Panel Type" to add one.</td></tr>';
        return;
    }
    tbody.innerHTML = allControlPanelTypes.map(t => `
        <tr>
            <td>${t.id}</td>
            <td><code>${esc(t.name)}</code></td>
            <td><strong>${esc(t.displayName)}</strong></td>
            <td class="text-muted small">${esc(t.description || '-')}</td>
            <td>${t.version ? `<span class="badge bg-light text-dark border">${esc(t.version)}</span>` : '<span class="text-muted">-</span>'}</td>
            <td>${t.websiteUrl ? `<a href="${esc(t.websiteUrl)}" target="_blank" class="text-truncate d-inline-block" style="max-width:160px" title="${esc(t.websiteUrl)}"><i class="bi bi-box-arrow-up-right"></i> Visit</a>` : '<span class="text-muted">-</span>'}</td>
            <td><span class="badge bg-${t.isActive ? 'success' : 'secondary'}">${t.isActive ? 'Active' : 'Inactive'}</span></td>
            <td>
                <div class="btn-group btn-group-sm">
                    <button class="btn btn-outline-primary" onclick="openEdit(${t.id})" title="Edit"><i class="bi bi-pencil"></i></button>
                    <button class="btn btn-outline-danger" onclick="openDelete(${t.id}, '${esc(t.displayName)}')" title="Delete"><i class="bi bi-trash"></i></button>
                </div>
            </td>
        </tr>
    `).join('');
}

function openCreate() {
    editingId = null;
    document.getElementById('modalTitle').textContent = 'New Control Panel Type';
    document.getElementById('form').reset();
    document.getElementById('isActive').checked = true;
    new bootstrap.Modal(document.getElementById('editModal')).show();
}

function openEdit(id) {
    const t = allControlPanelTypes.find(x => x.id === id);
    if (!t) return;
    editingId = id;
    document.getElementById('modalTitle').textContent = `Edit Control Panel Type – ${t.displayName}`;
    document.getElementById('name').value = t.name;
    document.getElementById('displayName').value = t.displayName;
    document.getElementById('description').value = t.description || '';
    document.getElementById('version').value = t.version || '';
    document.getElementById('websiteUrl').value = t.websiteUrl || '';
    document.getElementById('isActive').checked = t.isActive;
    new bootstrap.Modal(document.getElementById('editModal')).show();
}

async function saveControlPanelType() {
    const name = document.getElementById('name').value.trim();
    const displayName = document.getElementById('displayName').value.trim();

    if (!name || !displayName) {
        showError('Internal Name and Display Name are required');
        return;
    }

    const data = {
        name,
        displayName,
        description: document.getElementById('description').value.trim() || null,
        version: document.getElementById('version').value.trim() || null,
        websiteUrl: document.getElementById('websiteUrl').value.trim() || null,
        isActive: document.getElementById('isActive').checked,
    };

    const res = editingId
        ? await window.ControlPanelTypeAPI.update(editingId, data)
        : await window.ControlPanelTypeAPI.create(data);

    if (res.success) {
        bootstrap.Modal.getInstance(document.getElementById('editModal')).hide();
        showSuccess(editingId ? 'Control panel type updated successfully' : 'Control panel type created successfully');
        loadControlPanelTypes();
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
    const res = await window.ControlPanelTypeAPI.delete(pendingDeleteId);
    bootstrap.Modal.getInstance(document.getElementById('deleteModal')).hide();
    if (res.success) {
        showSuccess('Control panel type deleted successfully');
        loadControlPanelTypes();
    } else {
        showError(res.message || 'Delete failed');
    }
    pendingDeleteId = null;
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

/**
 * Server Types Management Page
 */

let allServerTypes = [];
let editingId = null;

document.addEventListener('DOMContentLoaded', () => {
    checkAuthState();
    loadServerTypes();
    document.getElementById('saveBtn').addEventListener('click', saveServerType);
    document.getElementById('confirmDelete').addEventListener('click', () => doDelete());
});

async function loadServerTypes() {
    const tbody = document.getElementById('tableBody');
    tbody.innerHTML = '<tr><td colspan="6" class="text-center"><div class="spinner-border text-primary"></div></td></tr>';

    const res = await window.ServerTypeAPI.getAll();
    if (!res.success) {
        showError(res.message || 'Failed to load server types');
        tbody.innerHTML = '<tr><td colspan="6" class="text-center text-danger">Failed to load data</td></tr>';
        return;
    }

    allServerTypes = Array.isArray(res.data) ? res.data : [];
    renderTable();
}

function renderTable() {
    const tbody = document.getElementById('tableBody');
    if (!allServerTypes.length) {
        tbody.innerHTML = '<tr><td colspan="6" class="text-center text-muted">No server types found. Click "New Server Type" to add one.</td></tr>';
        return;
    }
    tbody.innerHTML = allServerTypes.map(t => `
        <tr>
            <td>${t.id}</td>
            <td><code>${esc(t.name)}</code></td>
            <td>${esc(t.displayName)}</td>
            <td>${esc(t.description || '-')}</td>
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
    document.getElementById('modalTitle').textContent = 'New Server Type';
    document.getElementById('form').reset();
    document.getElementById('isActive').checked = true;
    new bootstrap.Modal(document.getElementById('editModal')).show();
}

function openEdit(id) {
    const t = allServerTypes.find(x => x.id === id);
    if (!t) return;
    editingId = id;
    document.getElementById('modalTitle').textContent = 'Edit Server Type';
    document.getElementById('name').value = t.name;
    document.getElementById('displayName').value = t.displayName;
    document.getElementById('description').value = t.description || '';
    document.getElementById('isActive').checked = t.isActive;
    new bootstrap.Modal(document.getElementById('editModal')).show();
}

async function saveServerType() {
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
        isActive: document.getElementById('isActive').checked,
    };

    const res = editingId
        ? await window.ServerTypeAPI.update(editingId, data)
        : await window.ServerTypeAPI.create(data);

    if (res.success) {
        bootstrap.Modal.getInstance(document.getElementById('editModal')).hide();
        showSuccess(editingId ? 'Server type updated successfully' : 'Server type created successfully');
        loadServerTypes();
    } else {
        showError(res.message || 'Save failed');
    }
}

let pendingDeleteId = null;

function openDelete(id, name) {
    pendingDeleteId = id;
    document.getElementById('deleteName').textContent = name;
    new bootstrap.Modal(document.getElementById('deleteModal')).show();
}

async function doDelete() {
    if (!pendingDeleteId) return;
    const res = await window.ServerTypeAPI.delete(pendingDeleteId);
    bootstrap.Modal.getInstance(document.getElementById('deleteModal')).hide();
    if (res.success) {
        showSuccess('Server type deleted successfully');
        loadServerTypes();
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

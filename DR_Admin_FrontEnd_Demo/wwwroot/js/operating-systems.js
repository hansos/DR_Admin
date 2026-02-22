/**
 * Operating Systems Management Page
 */

let allOperatingSystems = [];
let editingId = null;
let pendingDeleteId = null;

document.addEventListener('DOMContentLoaded', () => {
    checkAuthState();
    loadOperatingSystems();
    document.getElementById('saveBtn').addEventListener('click', saveOperatingSystem);
    document.getElementById('confirmDelete').addEventListener('click', () => doDelete());
});

async function loadOperatingSystems() {
    const tbody = document.getElementById('tableBody');
    tbody.innerHTML = '<tr><td colspan="7" class="text-center"><div class="spinner-border text-primary"></div></td></tr>';

    const res = await window.OperatingSystemAPI.getAll();
    if (!res.success) {
        showError(res.message || 'Failed to load operating systems');
        tbody.innerHTML = '<tr><td colspan="7" class="text-center text-danger">Failed to load data</td></tr>';
        return;
    }

    allOperatingSystems = Array.isArray(res.data) ? res.data : [];
    renderTable();
}

function renderTable() {
    const tbody = document.getElementById('tableBody');
    if (!allOperatingSystems.length) {
        tbody.innerHTML = '<tr><td colspan="7" class="text-center text-muted">No operating systems found. Click "New Operating System" to add one.</td></tr>';
        return;
    }
    tbody.innerHTML = allOperatingSystems.map(os => `
        <tr>
            <td>${os.id}</td>
            <td><code>${esc(os.name)}</code></td>
            <td>${esc(os.displayName)}</td>
            <td>${esc(os.version || '-')}</td>
            <td>${esc(os.description || '-')}</td>
            <td><span class="badge bg-${os.isActive ? 'success' : 'secondary'}">${os.isActive ? 'Active' : 'Inactive'}</span></td>
            <td>
                <div class="btn-group btn-group-sm">
                    <button class="btn btn-outline-primary" onclick="openEdit(${os.id})" title="Edit"><i class="bi bi-pencil"></i></button>
                    <button class="btn btn-outline-danger" onclick="openDelete(${os.id}, '${esc(os.displayName)}')" title="Delete"><i class="bi bi-trash"></i></button>
                </div>
            </td>
        </tr>
    `).join('');
}

function openCreate() {
    editingId = null;
    document.getElementById('modalTitle').textContent = 'New Operating System';
    document.getElementById('form').reset();
    document.getElementById('isActive').checked = true;
    new bootstrap.Modal(document.getElementById('editModal')).show();
}

function openEdit(id) {
    const os = allOperatingSystems.find(x => x.id === id);
    if (!os) return;
    editingId = id;
    document.getElementById('modalTitle').textContent = 'Edit Operating System';
    document.getElementById('name').value = os.name;
    document.getElementById('displayName').value = os.displayName;
    document.getElementById('version').value = os.version || '';
    document.getElementById('description').value = os.description || '';
    document.getElementById('isActive').checked = os.isActive;
    new bootstrap.Modal(document.getElementById('editModal')).show();
}

async function saveOperatingSystem() {
    const name = document.getElementById('name').value.trim();
    const displayName = document.getElementById('displayName').value.trim();

    if (!name || !displayName) {
        showError('Internal Name and Display Name are required');
        return;
    }

    const data = {
        name,
        displayName,
        version: document.getElementById('version').value.trim() || null,
        description: document.getElementById('description').value.trim() || null,
        isActive: document.getElementById('isActive').checked,
    };

    const res = editingId
        ? await window.OperatingSystemAPI.update(editingId, data)
        : await window.OperatingSystemAPI.create(data);

    if (res.success) {
        bootstrap.Modal.getInstance(document.getElementById('editModal')).hide();
        showSuccess(editingId ? 'Operating system updated successfully' : 'Operating system created successfully');
        loadOperatingSystems();
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
    const res = await window.OperatingSystemAPI.delete(pendingDeleteId);
    bootstrap.Modal.getInstance(document.getElementById('deleteModal')).hide();
    if (res.success) {
        showSuccess('Operating system deleted successfully');
        loadOperatingSystems();
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

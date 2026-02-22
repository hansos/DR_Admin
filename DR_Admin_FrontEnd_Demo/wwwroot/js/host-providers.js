/**
 * Host Providers Management Page
 */

let allHostProviders = [];
let editingId = null;
let pendingDeleteId = null;

document.addEventListener('DOMContentLoaded', () => {
    checkAuthState();
    loadHostProviders();
    document.getElementById('saveBtn').addEventListener('click', saveHostProvider);
    document.getElementById('confirmDelete').addEventListener('click', () => doDelete());
});

async function loadHostProviders() {
    const tbody = document.getElementById('tableBody');
    tbody.innerHTML = '<tr><td colspan="7" class="text-center"><div class="spinner-border text-primary"></div></td></tr>';

    const res = await window.HostProviderAPI.getAll();
    if (!res.success) {
        showError(res.message || 'Failed to load host providers');
        tbody.innerHTML = '<tr><td colspan="7" class="text-center text-danger">Failed to load data</td></tr>';
        return;
    }

    allHostProviders = Array.isArray(res.data) ? res.data : [];
    renderTable();
}

function renderTable() {
    const tbody = document.getElementById('tableBody');
    if (!allHostProviders.length) {
        tbody.innerHTML = '<tr><td colspan="7" class="text-center text-muted">No host providers found. Click "New Host Provider" to add one.</td></tr>';
        return;
    }
    tbody.innerHTML = allHostProviders.map(p => `
        <tr>
            <td>${p.id}</td>
            <td><code>${esc(p.name)}</code></td>
            <td>${esc(p.displayName)}</td>
            <td>${esc(p.description || '-')}</td>
            <td>${p.websiteUrl ? `<a href="${esc(p.websiteUrl)}" target="_blank" rel="noopener">${esc(p.websiteUrl)}</a>` : '-'}</td>
            <td><span class="badge bg-${p.isActive ? 'success' : 'secondary'}">${p.isActive ? 'Active' : 'Inactive'}</span></td>
            <td>
                <div class="btn-group btn-group-sm">
                    <button class="btn btn-outline-primary" onclick="openEdit(${p.id})" title="Edit"><i class="bi bi-pencil"></i></button>
                    <button class="btn btn-outline-danger" onclick="openDelete(${p.id}, '${esc(p.displayName)}')" title="Delete"><i class="bi bi-trash"></i></button>
                </div>
            </td>
        </tr>
    `).join('');
}

function openCreate() {
    editingId = null;
    document.getElementById('modalTitle').textContent = 'New Host Provider';
    document.getElementById('form').reset();
    document.getElementById('isActive').checked = true;
    new bootstrap.Modal(document.getElementById('editModal')).show();
}

function openEdit(id) {
    const p = allHostProviders.find(x => x.id === id);
    if (!p) return;
    editingId = id;
    document.getElementById('modalTitle').textContent = 'Edit Host Provider';
    document.getElementById('name').value = p.name;
    document.getElementById('displayName').value = p.displayName;
    document.getElementById('description').value = p.description || '';
    document.getElementById('websiteUrl').value = p.websiteUrl || '';
    document.getElementById('isActive').checked = p.isActive;
    new bootstrap.Modal(document.getElementById('editModal')).show();
}

async function saveHostProvider() {
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
        websiteUrl: document.getElementById('websiteUrl').value.trim() || null,
        isActive: document.getElementById('isActive').checked,
    };

    const res = editingId
        ? await window.HostProviderAPI.update(editingId, data)
        : await window.HostProviderAPI.create(data);

    if (res.success) {
        bootstrap.Modal.getInstance(document.getElementById('editModal')).hide();
        showSuccess(editingId ? 'Host provider updated successfully' : 'Host provider created successfully');
        loadHostProviders();
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
    const res = await window.HostProviderAPI.delete(pendingDeleteId);
    bootstrap.Modal.getInstance(document.getElementById('deleteModal')).hide();
    if (res.success) {
        showSuccess('Host provider deleted successfully');
        loadHostProviders();
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

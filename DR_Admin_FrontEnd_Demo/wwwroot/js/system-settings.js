/**
 * System Settings - Management Page
 */

let currentPage = 1;
let pageSize = 10;
let sortField = 'key';
let sortOrder = 'asc';
let searchTerm = '';
let allSettings = [];
let filteredSettings = [];
let settingToDelete = null;
let editingSettingId = null;

document.addEventListener('DOMContentLoaded', () => {
    initializeEventListeners();
    loadSettings();
});

function initializeEventListeners() {
    let searchTimeout;
    document.getElementById('searchInput')?.addEventListener('input', (e) => {
        clearTimeout(searchTimeout);
        searchTimeout = setTimeout(() => {
            searchTerm = e.target.value.toLowerCase();
            applyFiltersAndRender();
        }, 300);
    });

    document.getElementById('clearFilters')?.addEventListener('click', () => {
        document.getElementById('searchInput').value = '';
        searchTerm = '';
        applyFiltersAndRender();
    });

    document.querySelectorAll('.sort-link').forEach(link => {
        link.addEventListener('click', (e) => {
            e.preventDefault();
            const field = e.currentTarget.getAttribute('data-sort');
            if (sortField === field) {
                sortOrder = sortOrder === 'asc' ? 'desc' : 'asc';
            } else {
                sortField = field;
                sortOrder = 'asc';
            }
            applyFiltersAndRender();
        });
    });

    document.getElementById('confirmDelete')?.addEventListener('click', async () => {
        if (settingToDelete) await deleteSetting(settingToDelete);
    });

    document.getElementById('saveSettingBtn')?.addEventListener('click', async () => {
        await saveSetting();
    });

    document.getElementById('settingModal')?.addEventListener('hidden.bs.modal', () => {
        resetForm();
    });
}

async function loadSettings() {
    try {
        const tbody = document.getElementById('settingsTableBody');
        tbody.innerHTML = '<tr><td colspan="6" class="text-center"><div class="spinner-border text-primary"></div></td></tr>';

        const response = await window.SystemSettingAPI.getSystemSettings();

        if (response.success) {
            allSettings = Array.isArray(response.data) ? response.data : (response.data.items || []);
            applyFiltersAndRender();
        } else {
            if (response.statusCode === 401) {
                showError('Authentication required. Redirecting to login...');
                setTimeout(() => window.location.href = '/login.html', 2000);
            } else if (response.statusCode === 403) {
                showError('Access denied. You need Admin role to view system settings.');
            } else {
                showError(response.message || 'Failed to load system settings. Check authentication.');
            }
            tbody.innerHTML = '<tr><td colspan="6" class="text-center text-danger">Failed to load settings</td></tr>';
        }
    } catch (error) {
        console.error('Error:', error);
        showError('Error loading system settings');
        const tbody = document.getElementById('settingsTableBody');
        tbody.innerHTML = '<tr><td colspan="6" class="text-center text-danger">Error loading data</td></tr>';
    }
}

function applyFiltersAndRender() {
    filteredSettings = allSettings.filter(setting => {
        if (searchTerm) {
            const match = setting.key?.toLowerCase().includes(searchTerm)
                || setting.value?.toLowerCase().includes(searchTerm)
                || setting.description?.toLowerCase().includes(searchTerm);
            if (!match) return false;
        }
        return true;
    });

    filteredSettings.sort((a, b) => {
        let aVal = a[sortField] ?? '';
        let bVal = b[sortField] ?? '';
        if (typeof aVal === 'string') aVal = aVal.toLowerCase();
        if (typeof bVal === 'string') bVal = bVal.toLowerCase();
        if (aVal < bVal) return sortOrder === 'asc' ? -1 : 1;
        if (aVal > bVal) return sortOrder === 'asc' ? 1 : -1;
        return 0;
    });

    currentPage = 1;
    renderTable();
    renderPagination();
}

function renderTable() {
    const tbody = document.getElementById('settingsTableBody');
    const start = (currentPage - 1) * pageSize;
    const pageSettings = filteredSettings.slice(start, start + pageSize);

    if (!pageSettings.length) {
        tbody.innerHTML = '<tr><td colspan="6" class="text-center text-muted">No settings found</td></tr>';
        return;
    }

    tbody.innerHTML = pageSettings.map(s => `
        <tr>
            <td>${s.id}</td>
            <td><code>${esc(s.key)}</code></td>
            <td>${esc(s.value)}</td>
            <td>${esc(s.description)}</td>
            <td>${formatDate(s.updatedAt)}</td>
            <td>
                <div class="btn-group btn-group-sm">
                    <button class="btn btn-outline-primary" onclick="openEditModal(${s.id})" title="Edit"><i class="bi bi-pencil"></i></button>
                    <button class="btn btn-outline-danger" onclick="confirmDelete(${s.id}, '${esc(s.key)}')" title="Delete"><i class="bi bi-trash"></i></button>
                </div>
            </td>
        </tr>
    `).join('');
}

function renderPagination() {
    const total = Math.ceil(filteredSettings.length / pageSize);
    const info = document.getElementById('paginationInfo');
    const pag = document.getElementById('pagination');

    const start = (currentPage - 1) * pageSize + 1;
    const end = Math.min(currentPage * pageSize, filteredSettings.length);
    info.textContent = filteredSettings.length ? `Showing ${start}-${end} of ${filteredSettings.length}` : 'Showing 0 of 0';

    if (total <= 1) { pag.innerHTML = ''; return; }

    let html = `<li class="page-item ${currentPage === 1 ? 'disabled' : ''}">
        <a class="page-link" href="#" onclick="changePage(${currentPage - 1}); return false;">Previous</a></li>`;

    for (let i = 1; i <= total; i++) {
        if (i === 1 || i === total || (i >= currentPage - 2 && i <= currentPage + 2)) {
            html += `<li class="page-item ${i === currentPage ? 'active' : ''}">
                <a class="page-link" href="#" onclick="changePage(${i}); return false;">${i}</a></li>`;
        } else if (i === currentPage - 3 || i === currentPage + 3) {
            html += '<li class="page-item disabled"><span class="page-link">...</span></li>';
        }
    }

    html += `<li class="page-item ${currentPage === total ? 'disabled' : ''}">
        <a class="page-link" href="#" onclick="changePage(${currentPage + 1}); return false;">Next</a></li>`;

    pag.innerHTML = html;
}

function changePage(page) {
    const total = Math.ceil(filteredSettings.length / pageSize);
    if (page < 1 || page > total) return;
    currentPage = page;
    renderTable();
    renderPagination();
}

function openCreateModal() {
    editingSettingId = null;
    document.getElementById('settingModalLabel').textContent = 'New System Setting';
    document.getElementById('settingKey').disabled = false;
    resetForm();
    new bootstrap.Modal(document.getElementById('settingModal')).show();
}

function openEditModal(id) {
    const setting = allSettings.find(s => s.id === id);
    if (!setting) return;

    editingSettingId = id;
    document.getElementById('settingModalLabel').textContent = 'Edit System Setting';
    document.getElementById('settingKey').value = setting.key;
    document.getElementById('settingKey').disabled = true;
    document.getElementById('settingValue').value = setting.value;
    document.getElementById('settingDescription').value = setting.description;
    new bootstrap.Modal(document.getElementById('settingModal')).show();
}

function resetForm() {
    document.getElementById('settingKey').value = '';
    document.getElementById('settingKey').disabled = false;
    document.getElementById('settingValue').value = '';
    document.getElementById('settingDescription').value = '';
    document.getElementById('modalError').classList.add('d-none');
    editingSettingId = null;
}

async function saveSetting() {
    const key = document.getElementById('settingKey').value.trim();
    const value = document.getElementById('settingValue').value.trim();
    const description = document.getElementById('settingDescription').value.trim();

    if (!key || !value) {
        const err = document.getElementById('modalError');
        err.textContent = 'Key and Value are required.';
        err.classList.remove('d-none');
        return;
    }

    try {
        let response;
        if (editingSettingId) {
            response = await window.SystemSettingAPI.updateSystemSetting(editingSettingId, { value, description });
        } else {
            response = await window.SystemSettingAPI.createSystemSetting({ key, value, description });
        }

        if (response.success) {
            bootstrap.Modal.getInstance(document.getElementById('settingModal')).hide();
            showSuccess(editingSettingId ? 'Setting updated' : 'Setting created');
            loadSettings();
        } else {
            const err = document.getElementById('modalError');
            err.textContent = response.message || 'Failed to save setting';
            err.classList.remove('d-none');
        }
    } catch (error) {
        const err = document.getElementById('modalError');
        err.textContent = 'Error saving setting';
        err.classList.remove('d-none');
    }
}

function confirmDelete(id, key) {
    settingToDelete = id;
    document.getElementById('deleteSettingKey').textContent = key;
    new bootstrap.Modal(document.getElementById('deleteModal')).show();
}

async function deleteSetting(id) {
    try {
        const response = await window.SystemSettingAPI.deleteSystemSetting(id);
        bootstrap.Modal.getInstance(document.getElementById('deleteModal')).hide();

        if (response.success) {
            showSuccess('Setting deleted');
            loadSettings();
        } else {
            showError(response.message || 'Delete failed');
        }
    } catch (error) {
        showError('Error deleting setting');
    }
}

function esc(text) {
    const map = { '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#039;' };
    return (text || '').replace(/[&<>"']/g, m => map[m]);
}

function formatDate(dateString) {
    if (!dateString) return '-';
    return new Date(dateString).toLocaleDateString('en-US', { year: 'numeric', month: 'short', day: 'numeric' });
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

window.changePage = changePage;
window.confirmDelete = confirmDelete;
window.openCreateModal = openCreateModal;
window.openEditModal = openEditModal;

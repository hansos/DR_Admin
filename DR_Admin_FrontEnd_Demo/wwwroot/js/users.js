/**
 * User Management - List Page
 */

let currentPage = 1;
let pageSize = 10;
let sortField = 'id';
let sortOrder = 'asc';
let searchTerm = '';
let statusFilter = '';
let roleFilter = '';
let allUsers = [];
let filteredUsers = [];
let userToDelete = null;

document.addEventListener('DOMContentLoaded', () => {
    initializeEventListeners();
    loadUsers();
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

    document.getElementById('statusFilter')?.addEventListener('change', (e) => {
        statusFilter = e.target.value;
        applyFiltersAndRender();
    });

    document.getElementById('roleFilter')?.addEventListener('change', (e) => {
        roleFilter = e.target.value;
        applyFiltersAndRender();
    });

    document.getElementById('clearFilters')?.addEventListener('click', () => {
        document.getElementById('searchInput').value = '';
        document.getElementById('statusFilter').value = '';
        document.getElementById('roleFilter').value = '';
        searchTerm = '';
        statusFilter = '';
        roleFilter = '';
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
        if (userToDelete) await deleteUser(userToDelete);
    });
}

async function loadUsers() {
    try {
        const tbody = document.getElementById('usersTableBody');
        tbody.innerHTML = '<tr><td colspan="7" class="text-center"><div class="spinner-border text-primary"></div></td></tr>';

        const response = await window.UserAPI.getUsers();

        if (response.success) {
            allUsers = Array.isArray(response.data) ? response.data : (response.data.items || []);
            applyFiltersAndRender();
        } else {
            // Check if it's an authentication issue
            if (response.statusCode === 401) {
                showError('Authentication required. Redirecting to login...');
                setTimeout(() => window.location.href = '/login.html', 2000);
            } else if (response.statusCode === 403) {
                showError('Access denied. You need Admin or Support role to view users.');
            } else {
                showError(response.message || 'Failed to load users. Check authentication.');
            }
            tbody.innerHTML = '<tr><td colspan="7" class="text-center text-danger">Failed to load users</td></tr>';
        }
    } catch (error) {
        console.error('Error:', error);
        showError('Error loading users');
        const tbody = document.getElementById('usersTableBody');
        tbody.innerHTML = '<tr><td colspan="7" class="text-center text-danger">Error loading data</td></tr>';
    }
}

function applyFiltersAndRender() {
    filteredUsers = allUsers.filter(user => {
        if (searchTerm) {
            const match = user.username?.toLowerCase().includes(searchTerm) || user.email?.toLowerCase().includes(searchTerm);
            if (!match) return false;
        }
        if (statusFilter && user.isActive !== (statusFilter === 'active')) return false;
        if (roleFilter && !user.roles?.includes(roleFilter)) return false;
        return true;
    });

    filteredUsers.sort((a, b) => {
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
    const tbody = document.getElementById('usersTableBody');
    const start = (currentPage - 1) * pageSize;
    const pageUsers = filteredUsers.slice(start, start + pageSize);

    if (!pageUsers.length) {
        tbody.innerHTML = '<tr><td colspan="7" class="text-center text-muted">No users found</td></tr>';
        return;
    }

    tbody.innerHTML = pageUsers.map(u => `
        <tr>
            <td>${u.id}</td>
            <td>${esc(u.username)}</td>
            <td>${esc(u.email)}</td>
            <td>${renderRoles(u.roles)}</td>
            <td><span class="badge bg-${u.isActive ? 'success' : 'danger'}">${u.isActive ? 'Active' : 'Inactive'}</span></td>
            <td>${formatDate(u.createdAt)}</td>
            <td>
                <div class="btn-group btn-group-sm">
                    <a href="/user-edit.html?id=${u.id}" class="btn btn-outline-primary" title="Edit"><i class="bi bi-pencil"></i></a>
                    <button class="btn btn-outline-danger" onclick="confirmDelete(${u.id}, '${esc(u.username)}')" title="Delete"><i class="bi bi-trash"></i></button>
                </div>
            </td>
        </tr>
    `).join('');
}

function renderRoles(roles) {
    if (!roles || !roles.length) return '<span class="badge bg-secondary">None</span>';
    return roles.map(r => `<span class="badge bg-info me-1">${esc(r)}</span>`).join('');
}

function renderPagination() {
    const total = Math.ceil(filteredUsers.length / pageSize);
    const info = document.getElementById('paginationInfo');
    const pag = document.getElementById('pagination');

    const start = (currentPage - 1) * pageSize + 1;
    const end = Math.min(currentPage * pageSize, filteredUsers.length);
    info.textContent = `Showing ${start}-${end} of ${filteredUsers.length}`;

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
    const total = Math.ceil(filteredUsers.length / pageSize);
    if (page < 1 || page > total) return;
    currentPage = page;
    renderTable();
    renderPagination();
}

function confirmDelete(id, username) {
    userToDelete = id;
    document.getElementById('deleteUserName').textContent = username;
    new bootstrap.Modal(document.getElementById('deleteModal')).show();
}

async function deleteUser(id) {
    try {
        const response = await window.UserAPI.deleteUser(id);
        bootstrap.Modal.getInstance(document.getElementById('deleteModal')).hide();

        if (response.success) {
            showSuccess('User deleted');
            loadUsers();
        } else {
            showError(response.message || 'Delete failed');
        }
    } catch (error) {
        showError('Error deleting user');
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

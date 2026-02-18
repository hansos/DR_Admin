/**
 * User Edit Page
 */

let userId;
let availableRoles = [];
let userRoles = [];

document.addEventListener('DOMContentLoaded', async () => {
    const params = new URLSearchParams(window.location.search);
    userId = params.get('id');

    if (!userId) {
        showError('No user ID provided');
        return;
    }

    // Load roles first, then load user data to ensure checkboxes exist
    await loadRoles();
    await loadUser();

    const form = document.getElementById('userForm');
    form.addEventListener('submit', async (e) => {
        e.preventDefault();
        await updateUser();
    });
});

async function loadRoles() {
    try {
        const response = await window.RoleAPI.getRoles();
        console.log('Roles API response:', response);

        if (response.success) {
            availableRoles = response.data;
            console.log('Available roles loaded:', availableRoles);
            renderRoles();
        } else {
            console.error('Failed to load roles:', response.message);
            document.getElementById('loadingRoles').innerHTML = 
                '<div class="alert alert-warning"><i class="bi bi-exclamation-triangle"></i> Failed to load available roles</div>';
        }
    } catch (error) {
        console.error('Error loading roles:', error);
        document.getElementById('loadingRoles').innerHTML = 
            '<div class="alert alert-danger"><i class="bi bi-x-circle"></i> Error loading roles</div>';
    }
}

function renderRoles() {
    const container = document.getElementById('availableRoles');

    if (availableRoles.length === 0) {
        container.innerHTML = '<div class="alert alert-warning">No roles available in the system</div>';
        document.getElementById('loadingRoles').classList.add('d-none');
        document.getElementById('rolesContainer').classList.remove('d-none');
        return;
    }

    container.innerHTML = availableRoles.map(role => `
        <div class="form-check">
            <input class="form-check-input" type="checkbox" value="${role.name}" 
                   id="role_${role.id}" data-role-id="${role.id}" data-role-name="${role.name}">
            <label class="form-check-label" for="role_${role.id}">
                <strong>${role.name}</strong>
                ${role.description ? `<br><small class="text-muted">${role.description}</small>` : ''}
            </label>
        </div>
    `).join('');

    document.getElementById('loadingRoles').classList.add('d-none');
    document.getElementById('rolesContainer').classList.remove('d-none');
}

function updateRoleCheckboxes() {
    console.log('Updating role checkboxes. User roles:', userRoles);
    console.log('Available roles:', availableRoles);

    availableRoles.forEach(role => {
        const checkbox = document.getElementById(`role_${role.id}`);
        if (checkbox) {
            const isChecked = userRoles.includes(role.name);
            checkbox.checked = isChecked;
            console.log(`Role ${role.name} (${role.id}): ${isChecked ? 'CHECKED' : 'unchecked'}`);
        } else {
            console.warn(`Checkbox for role ${role.name} (${role.id}) not found`);
        }
    });

    const noRolesMsg = document.getElementById('noRolesMessage');
    if (userRoles.length === 0) {
        noRolesMsg.style.display = 'block';
    } else {
        noRolesMsg.style.display = 'none';
    }
}

async function loadUser() {
    try {
        const response = await window.UserAPI.getUser(userId);

        if (response.success) {
            const user = response.data;
            console.log('Loaded user:', user);

            document.getElementById('userId').value = user.id;
            document.getElementById('username').value = user.username || '';
            document.getElementById('email').value = user.email || '';
            document.getElementById('role').value = user.role || '';
            document.getElementById('isActive').value = user.isActive ? 'true' : 'false';
            document.getElementById('customerId').value = user.customerId || '';

            // Store user roles
            userRoles = user.roles || [];
            console.log('User roles array:', userRoles);

            updateRoleCheckboxes();

            document.getElementById('loadingSpinner').classList.add('d-none');
            document.getElementById('userFormCard').classList.remove('d-none');
        } else {
            showError(response.message || 'Failed to load user');
            document.getElementById('loadingSpinner').classList.add('d-none');
        }
    } catch (error) {
        console.error('Error loading user:', error);
        showError('An error occurred while loading the user');
        document.getElementById('loadingSpinner').classList.add('d-none');
    }
}

async function updateUser() {
    const customerIdValue = document.getElementById('customerId').value.trim();

    // Collect selected roles
    const selectedRoles = [];
    availableRoles.forEach(role => {
        const checkbox = document.getElementById(`role_${role.id}`);
        if (checkbox && checkbox.checked) {
            selectedRoles.push(role.name);
        }
    });

    const userData = {
        username: document.getElementById('username').value.trim(),
        email: document.getElementById('email').value.trim(),
        role: document.getElementById('role').value || null,
        roles: selectedRoles,
        customerId: customerIdValue ? parseInt(customerIdValue) : null,
        isActive: document.getElementById('isActive').value === 'true'
    };

    const form = document.getElementById('userForm');
    const submitBtn = form.querySelector('button[type="submit"]');
    const originalText = submitBtn.innerHTML;
    submitBtn.disabled = true;
    submitBtn.innerHTML = '<span class="spinner-border spinner-border-sm"></span> Updating...';

    try {
        const response = await window.UserAPI.updateUser(userId, userData);

        if (response.success) {
            showSuccess('User updated successfully!');
            setTimeout(() => window.location.href = '/users.html', 1500);
        } else {
            showError(response.message || 'Failed to update user');
            submitBtn.disabled = false;
            submitBtn.innerHTML = originalText;
        }
    } catch (error) {
        console.error('Error updating user:', error);
        showError('An error occurred while updating the user');
        submitBtn.disabled = false;
        submitBtn.innerHTML = originalText;
    }
}

function showSuccess(message) {
    const alert = document.getElementById('alertSuccess');
    alert.textContent = message;
    alert.classList.remove('d-none');
    document.getElementById('alertError').classList.add('d-none');
}

function showError(message) {
    const alert = document.getElementById('alertError');
    alert.textContent = message;
    alert.classList.remove('d-none');
    document.getElementById('alertSuccess').classList.add('d-none');
}

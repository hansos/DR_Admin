/**
 * User Edit Page
 */

let userId;

document.addEventListener('DOMContentLoaded', () => {
    const params = new URLSearchParams(window.location.search);
    userId = params.get('id');

    if (!userId) {
        showError('No user ID provided');
        return;
    }

    loadUser();

    const form = document.getElementById('userForm');
    form.addEventListener('submit', async (e) => {
        e.preventDefault();
        await updateUser();
    });
});

async function loadUser() {
    try {
        const response = await window.UserAPI.getUser(userId);

        if (response.success) {
            const user = response.data;
            
            document.getElementById('userId').value = user.id;
            document.getElementById('username').value = user.username || '';
            document.getElementById('email').value = user.email || '';
            document.getElementById('role').value = user.role || '';
            document.getElementById('isActive').value = user.isActive ? 'true' : 'false';
            document.getElementById('customerId').value = user.customerId || '';

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
    
    const userData = {
        username: document.getElementById('username').value.trim(),
        email: document.getElementById('email').value.trim(),
        role: document.getElementById('role').value || null,
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

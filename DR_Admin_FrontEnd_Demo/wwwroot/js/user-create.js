/**
 * User Create Page
 */

document.addEventListener('DOMContentLoaded', () => {
    const form = document.getElementById('userForm');
    
    form.addEventListener('submit', async (e) => {
        e.preventDefault();
        
        const customerIdValue = document.getElementById('customerId').value.trim();
        
        const userData = {
            username: document.getElementById('username').value.trim(),
            email: document.getElementById('email').value.trim(),
            password: document.getElementById('password').value,
            role: document.getElementById('role').value || null,
            customerId: customerIdValue ? parseInt(customerIdValue) : null,
            isActive: document.getElementById('isActive').value === 'true'
        };

        const submitBtn = form.querySelector('button[type="submit"]');
        const originalText = submitBtn.innerHTML;
        submitBtn.disabled = true;
        submitBtn.innerHTML = '<span class="spinner-border spinner-border-sm"></span> Creating...';

        try {
            const response = await window.UserAPI.createUser(userData);

            if (response.success) {
                showSuccess('User created successfully!');
                setTimeout(() => window.location.href = '/users.html', 1500);
            } else {
                showError(response.message || 'Failed to create user');
                submitBtn.disabled = false;
                submitBtn.innerHTML = originalText;
            }
        } catch (error) {
            console.error('Error creating user:', error);
            showError('An error occurred while creating the user');
            submitBtn.disabled = false;
            submitBtn.innerHTML = originalText;
        }
    });
});

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

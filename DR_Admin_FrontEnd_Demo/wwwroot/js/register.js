"use strict";
document.addEventListener('DOMContentLoaded', () => {
    const registerForm = document.getElementById('registerForm');
    if (registerForm) {
        registerForm.addEventListener('submit', async (e) => {
            e.preventDefault();
            window.hideMessage('registerError');
            window.hideMessage('registerSuccess');
            const nameInput = document.getElementById('name');
            const emailInput = document.getElementById('email');
            const passwordInput = document.getElementById('password');
            const confirmPasswordInput = document.getElementById('confirmPassword');
            const name = nameInput.value.trim();
            const email = emailInput.value.trim();
            const password = passwordInput.value;
            const confirmPassword = confirmPasswordInput.value;
            if (!name || !email || !password || !confirmPassword) {
                window.showMessage('registerError', 'Please fill in all fields', true);
                return;
            }
            if (password.length < 6) {
                window.showMessage('registerError', 'Password must be at least 6 characters', true);
                return;
            }
            if (password !== confirmPassword) {
                window.showMessage('registerError', 'Passwords do not match', true);
                return;
            }
            const submitButton = registerForm.querySelector('button[type="submit"]');
            submitButton.disabled = true;
            submitButton.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>Creating account...';
            try {
                const response = await window.AuthAPI.register(name, email, password, confirmPassword);
                if (response.success) {
                    window.showMessage('registerSuccess', 'Registration successful! Redirecting to login...', false);
                    setTimeout(() => {
                        window.location.href = '/Account/Login';
                    }, 1500);
                }
                else {
                    window.showMessage('registerError', response.message || 'Registration failed', true);
                    submitButton.disabled = false;
                    submitButton.innerHTML = '<i class="bi bi-person-plus"></i> Create Account';
                }
            }
            catch (error) {
                console.error('Registration error:', error);
                window.showMessage('registerError', 'An unexpected error occurred', true);
                submitButton.disabled = false;
                submitButton.innerHTML = '<i class="bi bi-person-plus"></i> Create Account';
            }
        });
    }
});
//# sourceMappingURL=register.js.map
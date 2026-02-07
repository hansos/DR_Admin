"use strict";
document.addEventListener('DOMContentLoaded', () => {
    const loginForm = document.getElementById('loginForm');
    if (loginForm) {
        loginForm.addEventListener('submit', async (e) => {
            e.preventDefault();
            window.hideMessage('loginError');
            window.hideMessage('loginSuccess');
            const emailInput = document.getElementById('email');
            const passwordInput = document.getElementById('password');
            const email = emailInput.value.trim();
            const password = passwordInput.value;
            if (!email || !password) {
                window.showMessage('loginError', 'Please enter both email and password', true);
                return;
            }
            const submitButton = loginForm.querySelector('button[type="submit"]');
            submitButton.disabled = true;
            submitButton.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>Signing in...';
            try {
                const response = await window.AuthAPI.login(email, password);
                if (response.success) {
                    window.showMessage('loginSuccess', 'Login successful! Redirecting...', false);
                    setTimeout(() => {
                        window.location.href = '/';
                    }, 1000);
                }
                else {
                    window.showMessage('loginError', response.message || 'Login failed', true);
                    submitButton.disabled = false;
                    submitButton.innerHTML = '<i class="bi bi-box-arrow-in-right"></i> Sign In';
                }
            }
            catch (error) {
                console.error('Login error:', error);
                window.showMessage('loginError', 'An unexpected error occurred', true);
                submitButton.disabled = false;
                submitButton.innerHTML = '<i class="bi bi-box-arrow-in-right"></i> Sign In';
            }
        });
    }
});
//# sourceMappingURL=login.js.map
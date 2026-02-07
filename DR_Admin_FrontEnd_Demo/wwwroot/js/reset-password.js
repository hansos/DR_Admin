"use strict";
document.addEventListener('DOMContentLoaded', () => {
    const resetPasswordForm = document.getElementById('resetPasswordForm');
    if (resetPasswordForm) {
        resetPasswordForm.addEventListener('submit', async (e) => {
            e.preventDefault();
            window.hideMessage('resetError');
            window.hideMessage('resetSuccess');
            const emailInput = document.getElementById('email');
            const email = emailInput.value.trim();
            if (!email) {
                window.showMessage('resetError', 'Please enter your email address', true);
                return;
            }
            const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            if (!emailRegex.test(email)) {
                window.showMessage('resetError', 'Please enter a valid email address', true);
                return;
            }
            const submitButton = resetPasswordForm.querySelector('button[type="submit"]');
            submitButton.disabled = true;
            submitButton.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>Sending...';
            try {
                const response = await window.AuthAPI.resetPassword(email);
                if (response.success) {
                    window.showMessage('resetSuccess', 'Password reset instructions have been sent to your email.', false);
                    emailInput.value = '';
                    submitButton.disabled = false;
                    submitButton.innerHTML = '<i class="bi bi-envelope"></i> Send Reset Link';
                }
                else {
                    window.showMessage('resetError', response.message || 'Failed to send reset email', true);
                    submitButton.disabled = false;
                    submitButton.innerHTML = '<i class="bi bi-envelope"></i> Send Reset Link';
                }
            }
            catch (error) {
                console.error('Reset password error:', error);
                window.showMessage('resetError', 'An unexpected error occurred', true);
                submitButton.disabled = false;
                submitButton.innerHTML = '<i class="bi bi-envelope"></i> Send Reset Link';
            }
        });
    }
});
//# sourceMappingURL=reset-password.js.map
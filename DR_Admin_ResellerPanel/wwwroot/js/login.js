"use strict";
/**
 * Login page logic for DR Admin Reseller Panel.
 * Calls the Auth API, stores credentials in sessionStorage via Auth, and redirects on success.
 */
document.addEventListener('DOMContentLoaded', () => {
    var _a;
    // Redirect immediately if already authenticated
    if ((_a = window.Auth) === null || _a === void 0 ? void 0 : _a.isLoggedIn()) {
        window.location.href = '/';
        return;
    }
    const loginForm = document.getElementById('loginForm');
    const toggleBtn = document.getElementById('togglePassword');
    const passwordInput = document.getElementById('password');
    const toggleIcon = document.getElementById('togglePasswordIcon');
    // Password show/hide toggle
    if (toggleBtn && passwordInput && toggleIcon) {
        toggleBtn.addEventListener('click', () => {
            if (passwordInput.type === 'password') {
                passwordInput.type = 'text';
                toggleIcon.className = 'bi bi-eye-slash';
            }
            else {
                passwordInput.type = 'password';
                toggleIcon.className = 'bi bi-eye';
            }
        });
    }
    if (!loginForm)
        return;
    loginForm.addEventListener('submit', async (e) => {
        var _a, _b, _c, _d, _e, _f, _g;
        e.preventDefault();
        loginHideMessage('loginError');
        loginHideMessage('loginSuccess');
        const emailInput = document.getElementById('email');
        const pwdInput = document.getElementById('password');
        const username = emailInput.value.trim();
        const password = pwdInput.value;
        if (!username || !password) {
            loginShowMessage('loginError', 'Please enter both username and password.');
            return;
        }
        const submitBtn = loginForm.querySelector('button[type="submit"]');
        submitBtn.disabled = true;
        submitBtn.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>Signing in...';
        try {
            const response = await fetch('https://localhost:7201/api/v1/Auth/login', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ username, password }),
                credentials: 'include',
            });
            const contentType = (_a = response.headers.get('content-type')) !== null && _a !== void 0 ? _a : '';
            const data = contentType.includes('application/json') ? await response.json() : null;
            if (response.ok && (data === null || data === void 0 ? void 0 : data.success) !== false) {
                const payload = (_b = data === null || data === void 0 ? void 0 : data.data) !== null && _b !== void 0 ? _b : data;
                const resolvedUsername = (_c = payload === null || payload === void 0 ? void 0 : payload.username) !== null && _c !== void 0 ? _c : username;
                const token = (_d = payload === null || payload === void 0 ? void 0 : payload.accessToken) !== null && _d !== void 0 ? _d : '';
                const refreshToken = (_e = payload === null || payload === void 0 ? void 0 : payload.refreshToken) !== null && _e !== void 0 ? _e : null;
                const roles = Array.isArray(payload === null || payload === void 0 ? void 0 : payload.roles) ? payload.roles : [];
                const expiresAt = (_f = payload === null || payload === void 0 ? void 0 : payload.expiresAt) !== null && _f !== void 0 ? _f : null;
                (_g = window.Auth) === null || _g === void 0 ? void 0 : _g.setData(resolvedUsername, token, refreshToken, roles, expiresAt);
                loginShowMessage('loginSuccess', 'Login successful! Redirecting...');
                setTimeout(() => { window.location.href = '/'; }, 1000);
            }
            else {
                const msg = (data && data.message) ? data.message : `Login failed (${response.status})`;
                loginShowMessage('loginError', msg);
                submitBtn.disabled = false;
                submitBtn.innerHTML = '<i class="bi bi-box-arrow-in-right"></i> Sign In';
            }
        }
        catch (err) {
            console.error('Login error:', err);
            loginShowMessage('loginError', 'Network error. Please check your connection and try again.');
            submitBtn.disabled = false;
            submitBtn.innerHTML = '<i class="bi bi-box-arrow-in-right"></i> Sign In';
        }
    });
});
function loginShowMessage(id, message) {
    const el = document.getElementById(id);
    if (el) {
        el.textContent = message;
        el.classList.remove('d-none');
    }
}
function loginHideMessage(id) {
    const el = document.getElementById(id);
    if (el) {
        el.classList.add('d-none');
        el.textContent = '';
    }
}
//# sourceMappingURL=login.js.map
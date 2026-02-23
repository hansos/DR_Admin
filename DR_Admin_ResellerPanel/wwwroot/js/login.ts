/**
 * Login page logic for DR Admin Reseller Panel.
 * Calls the Auth API, stores credentials in sessionStorage via Auth, and redirects on success.
 */

document.addEventListener('DOMContentLoaded', () => {

    // Redirect immediately if already authenticated
    if ((window as any).Auth?.isLoggedIn()) {
        window.location.href = '/';
        return;
    }

    const loginForm     = document.getElementById('loginForm')     as HTMLFormElement   | null;
    const toggleBtn     = document.getElementById('togglePassword') as HTMLButtonElement | null;
    const passwordInput = document.getElementById('password')       as HTMLInputElement  | null;
    const toggleIcon    = document.getElementById('togglePasswordIcon');

    // Password show/hide toggle
    if (toggleBtn && passwordInput && toggleIcon) {
        toggleBtn.addEventListener('click', () => {
            if (passwordInput.type === 'password') {
                passwordInput.type = 'text';
                toggleIcon.className = 'bi bi-eye-slash';
            } else {
                passwordInput.type = 'password';
                toggleIcon.className = 'bi bi-eye';
            }
        });
    }

    if (!loginForm) return;

    loginForm.addEventListener('submit', async (e: Event) => {
        e.preventDefault();
        loginHideMessage('loginError');
        loginHideMessage('loginSuccess');

        const emailInput = document.getElementById('email')    as HTMLInputElement;
        const pwdInput   = document.getElementById('password') as HTMLInputElement;

        const username = emailInput.value.trim();
        const password = pwdInput.value;

        if (!username || !password) {
            loginShowMessage('loginError', 'Please enter both username and password.');
            return;
        }

        const submitBtn = loginForm.querySelector('button[type="submit"]') as HTMLButtonElement;
        submitBtn.disabled = true;
        submitBtn.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>Signing in...';

        try {
            const response = await fetch('https://localhost:7201/api/v1/Auth/login', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ username, password }),
                credentials: 'include',
            });

            const contentType = response.headers.get('content-type') ?? '';
            const data = contentType.includes('application/json') ? await response.json() : null;

            if (response.ok && data?.success !== false) {
                const payload                  = data?.data ?? data;
                const resolvedUsername: string = payload?.username   ?? username;
                const token: string            = payload?.accessToken ?? '';
                const refreshToken: string | null = payload?.refreshToken ?? null;
                const roles: string[]          = Array.isArray(payload?.roles) ? payload.roles : [];
                const expiresAt: string | null = payload?.expiresAt ?? null;

                (window as any).Auth?.setData(resolvedUsername, token, refreshToken, roles, expiresAt);

                loginShowMessage('loginSuccess', 'Login successful! Redirecting...');
                setTimeout(() => { window.location.href = '/'; }, 1000);
            } else {
                const msg: string = (data && data.message) ? data.message : `Login failed (${response.status})`;
                loginShowMessage('loginError', msg);
                submitBtn.disabled = false;
                submitBtn.innerHTML = '<i class="bi bi-box-arrow-in-right"></i> Sign In';
            }
        } catch (err) {
            console.error('Login error:', err);
            loginShowMessage('loginError', 'Network error. Please check your connection and try again.');
            submitBtn.disabled = false;
            submitBtn.innerHTML = '<i class="bi bi-box-arrow-in-right"></i> Sign In';
        }
    });
});

function loginShowMessage(id: string, message: string): void {
    const el = document.getElementById(id);
    if (el) {
        el.textContent = message;
        el.classList.remove('d-none');
    }
}

function loginHideMessage(id: string): void {
    const el = document.getElementById(id);
    if (el) {
        el.classList.add('d-none');
        el.textContent = '';
    }
}

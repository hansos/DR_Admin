/**
 * Authentication Status Checker
 * Checks if user is authenticated and shows login/logout buttons
 */

// Check authentication status on page load
document.addEventListener('DOMContentLoaded', () => {
    // This will be called on every page
    checkAuthStatus();
});

async function checkAuthStatus() {
    // Try to make a simple API call to check if authenticated
    // If we get 401, we're not logged in
    
    const navbar = document.querySelector('.navbar-nav.ms-auto');
    if (!navbar) return;

    // Add logout button if it doesn't exist
    const existingLogout = navbar.querySelector('.logout-link');
    if (!existingLogout) {
        const logoutLink = document.createElement('a');
        logoutLink.className = 'nav-link logout-link';
        logoutLink.href = '#';
        logoutLink.innerHTML = '<i class="bi bi-box-arrow-right"></i> Logout';
        logoutLink.onclick = (e) => {
            e.preventDefault();
            logout();
        };
        navbar.appendChild(logoutLink);
    }
}

async function logout() {
    if (confirm('Are you sure you want to logout?')) {
        try {
            // Call logout API to revoke refresh token
            if (window.AuthAPI && window.AuthAPI.logout) {
                await window.AuthAPI.logout();
            }
            // Redirect to login page
            window.location.href = '/login.html';
        } catch (error) {
            console.error('Logout error:', error);
            // Even if API call fails, clear session and redirect
            sessionStorage.clear();
            window.location.href = '/login.html';
        }
    }
}

// Make logout available globally
window.logout = logout;
window.checkAuthStatus = checkAuthStatus;

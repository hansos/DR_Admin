/**
 * Authentication State Management
 * Handles user login state across all pages
 */

// Check if user is logged in (using localStorage for demo)
function checkAuthState() {
    const userEmail = localStorage.getItem('userEmail');
    const accountMenu = document.getElementById('accountMenu');
    
    if (accountMenu) {
        if (userEmail) {
            // User is logged in - show account menu
            accountMenu.innerHTML = `
                <div class="dropdown">
                    <a class="nav-link dropdown-toggle" href="#" role="button" data-bs-toggle="dropdown">
                        <i class="bi bi-person-circle"></i> ${userEmail}
                    </a>
                    <ul class="dropdown-menu dropdown-menu-end">
                        <li><a class="dropdown-item" href="#" onclick="logout(); return false;">
                            <i class="bi bi-box-arrow-right"></i> Logout
                        </a></li>
                    </ul>
                </div>
            `;
        } else {
            // User is not logged in - show login link
            accountMenu.innerHTML = `
                <a class="nav-link" href="/login.html">
                    <i class="bi bi-box-arrow-in-right"></i> Login
                </a>
            `;
        }
    }
}

// Logout function
function logout() {
    localStorage.removeItem('userEmail');
    localStorage.removeItem('authToken');
    window.location.href = '/login.html';
}

// Check if user is logged in for protected pages
function requireAuth() {
    const userEmail = localStorage.getItem('userEmail');
    const currentPage = window.location.pathname;
    
    // List of pages that don't require authentication
    const publicPages = ['/', '/index.html', '/login.html', '/register.html', '/reset-password.html'];
    
    if (!userEmail && !publicPages.includes(currentPage)) {
        // Redirect to login if not authenticated
        window.location.href = '/login.html';
        return false;
    }
    
    return true;
}

// Show user status message on home page
function showUserStatus() {
    const userEmail = localStorage.getItem('userEmail');
    const statusDiv = document.getElementById('userStatusMessage');
    
    if (statusDiv) {
        if (userEmail) {
            statusDiv.innerHTML = `
                <div class="col-12">
                    <div class="alert alert-success" role="alert">
                        <h5 class="alert-heading"><i class="bi bi-check-circle"></i> You are logged in</h5>
                        <p class="mb-0">Logged in as: <strong>${userEmail}</strong></p>
                    </div>
                </div>
            `;
        } else {
            statusDiv.innerHTML = `
                <div class="col-12">
                    <div class="alert alert-info" role="alert">
                        <h5 class="alert-heading"><i class="bi bi-info-circle"></i> Getting Started</h5>
                        <p>Please <a href="/login.html" class="alert-link">login</a> or 
                           <a href="/register.html" class="alert-link">register</a> to access all features.</p>
                    </div>
                </div>
            `;
        }
    }
}

// Initialize auth state when page loads
document.addEventListener('DOMContentLoaded', () => {
    checkAuthState();
    requireAuth();
    showUserStatus();
});

// Export functions to global scope
if (typeof window !== 'undefined') {
    (window as any).logout = logout;
    (window as any).checkAuthState = checkAuthState;
    (window as any).requireAuth = requireAuth;
}

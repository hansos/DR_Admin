/**
 * Authentication Guard
 * Automatically checks for valid access tokens and redirects to login if invalid
 */

class AuthGuard {
    constructor(options = {}) {
        this.checkInterval = options.checkInterval || 60000; // Default: check every 60 seconds
        this.redirectUrl = options.redirectUrl || '/login.html';
        this.excludePages = options.excludePages || ['/login.html'];
        this.intervalId = null;
        this.isChecking = false;
    }

    /**
     * Initialize the auth guard
     */
    init() {
        // Don't run on excluded pages (like login page)
        const currentPath = window.location.pathname;
        if (this.excludePages.some(page => currentPath.endsWith(page))) {
            console.log('AuthGuard: Skipping on excluded page');
            return;
        }

        // Check immediately on load
        this.checkAuthentication();

        // Set up periodic checks
        this.startPeriodicCheck();

        // Listen for storage events (logout from another tab)
        window.addEventListener('storage', (e) => {
            if (e.key === 'authToken' || e.key === 'accessToken') {
                if (!e.newValue) {
                    // Token was removed
                    this.handleUnauthenticated();
                }
            }
        });

        // Check on visibility change (when user returns to tab)
        document.addEventListener('visibilitychange', () => {
            if (!document.hidden) {
                this.checkAuthentication();
            }
        });

        console.log('AuthGuard: Initialized with check interval:', this.checkInterval, 'ms');
    }

    /**
     * Start periodic authentication checks
     */
    startPeriodicCheck() {
        if (this.intervalId) {
            clearInterval(this.intervalId);
        }

        this.intervalId = setInterval(() => {
            this.checkAuthentication();
        }, this.checkInterval);
    }

    /**
     * Stop periodic authentication checks
     */
    stopPeriodicCheck() {
        if (this.intervalId) {
            clearInterval(this.intervalId);
            this.intervalId = null;
        }
    }

    /**
     * Check if user is authenticated
     */
    async checkAuthentication() {
        // Prevent concurrent checks
        if (this.isChecking) {
            return;
        }

        this.isChecking = true;

        try {
            // First check if token exists
            const token = this.getToken();
            if (!token) {
                console.log('AuthGuard: No token found');
                this.handleUnauthenticated();
                return;
            }

            // Check if token is expired (if it's a JWT)
            if (this.isTokenExpired(token)) {
                console.log('AuthGuard: Token expired');
                this.handleUnauthenticated();
                return;
            }

            // Verify token with server
            const isValid = await this.verifyTokenWithServer();
            if (!isValid) {
                console.log('AuthGuard: Token verification failed');
                this.handleUnauthenticated();
                return;
            }

            console.log('AuthGuard: Authentication valid');
        } catch (error) {
            console.error('AuthGuard: Error checking authentication:', error);
            // Don't redirect on network errors - give benefit of doubt
        } finally {
            this.isChecking = false;
        }
    }

    /**
     * Get stored authentication token
     */
    getToken() {
        return localStorage.getItem('authToken') || localStorage.getItem('accessToken');
    }

    /**
     * Check if JWT token is expired
     */
    isTokenExpired(token) {
        try {
            // Parse JWT token (format: header.payload.signature)
            const parts = token.split('.');
            if (parts.length !== 3) {
                // Not a JWT token, can't check expiration
                return false;
            }

            const payload = JSON.parse(atob(parts[1]));
            
            // Check expiration (exp claim is in seconds)
            if (payload.exp) {
                const expirationTime = payload.exp * 1000; // Convert to milliseconds
                const currentTime = Date.now();
                
                if (currentTime >= expirationTime) {
                    return true;
                }
            }

            return false;
        } catch (error) {
            console.error('AuthGuard: Error parsing token:', error);
            // If we can't parse it, assume it's not expired
            return false;
        }
    }

    /**
     * Verify token with the server
     */
    async verifyTokenWithServer() {
        try {
            // Use the authClient if available
            if (typeof authClient !== 'undefined' && authClient.verifyToken) {
                const result = await authClient.verifyToken();
                return result.success && result.authenticated;
            }

            // Fallback to direct API call
            const token = this.getToken();
            const response = await fetch('https://localhost:7201/api/v1/Auth/verify', {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                },
                credentials: 'include'
            });

            return response.ok;
        } catch (error) {
            console.error('AuthGuard: Error verifying token with server:', error);
            // Return true on network errors to avoid logging out due to temporary issues
            return true;
        }
    }

    /**
     * Handle unauthenticated state
     */
    handleUnauthenticated() {
        console.log('AuthGuard: User not authenticated, redirecting to login');
        
        // Clear tokens
        this.clearTokens();
        
        // Stop periodic checks
        this.stopPeriodicCheck();
        
        // Store current page to redirect back after login
        const currentPath = window.location.pathname + window.location.search + window.location.hash;
        if (!this.excludePages.some(page => currentPath.endsWith(page))) {
            sessionStorage.setItem('redirectAfterLogin', currentPath);
        }
        
        // Redirect to login
        window.location.href = this.redirectUrl;
    }

    /**
     * Clear stored tokens
     */
    clearTokens() {
        localStorage.removeItem('authToken');
        localStorage.removeItem('accessToken');
        localStorage.removeItem('refreshToken');
    }

    /**
     * Manually trigger authentication check
     */
    check() {
        return this.checkAuthentication();
    }

    /**
     * Update check interval
     */
    setCheckInterval(intervalMs) {
        this.checkInterval = intervalMs;
        this.startPeriodicCheck();
        console.log('AuthGuard: Check interval updated to', intervalMs, 'ms');
    }
}

// Create and initialize global auth guard instance
const authGuard = new AuthGuard({
    checkInterval: 60000, // Check every 60 seconds
    redirectUrl: '/login.html',
    excludePages: ['/login.html']
});

// Auto-initialize on DOM load
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', () => {
        authGuard.init();
    });
} else {
    // DOM already loaded
    authGuard.init();
}

// Export for use in other modules or make available globally
if (typeof module !== 'undefined' && module.exports) {
    module.exports = authGuard;
}

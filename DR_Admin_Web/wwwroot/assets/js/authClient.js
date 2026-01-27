/**
 * Authentication HTTP Client
 * Handles login requests to the API server
 */

const API_BASE_URL = 'https://localhost:7201';

class AuthClient {
    /**
     * Login user with credentials
     * @param {string} username - User's username
     * @param {string} password - User's password
     * @returns {Promise<object>} Login response data
     */
    async login(username, password) {
        try {
            const response = await fetch(`${API_BASE_URL}/api/v1/Auth/login`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    username: username,
                    password: password
                }),
                credentials: 'include' // Include cookies if needed
            });

            if (!response.ok) {
                const errorData = await response.json().catch(() => ({}));
                throw new Error(errorData.message || `Login failed with status: ${response.status}`);
            }

            const data = await response.json();
            
            // Store tokens if present in response
            if (data.token) {
                localStorage.setItem('authToken', data.token);
            }
            if (data.refreshToken) {
                localStorage.setItem('refreshToken', data.refreshToken);
            }
            if (data.accessToken) {
                localStorage.setItem('accessToken', data.accessToken);
            }
            
            return {
                success: true,
                data: data
            };
        } catch (error) {
            console.error('Login error:', error);
            return {
                success: false,
                error: error.message
            };
        }
    }

    /**
     * Get stored authentication token
     * @returns {string|null} The stored token
     */
    getToken() {
        return localStorage.getItem('authToken') || localStorage.getItem('accessToken');
    }

    /**
     * Get stored refresh token
     * @returns {string|null} The stored refresh token
     */
    getRefreshToken() {
        return localStorage.getItem('refreshToken');
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
     * Logout user
     * @returns {Promise<object>} Logout response
     */
    async logout() {
        try {
            const response = await fetch(`${API_BASE_URL}/api/auth/logout`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                credentials: 'include'
            });

            if (!response.ok) {
                throw new Error(`Logout failed with status: ${response.status}`);
            }

            return {
                success: true
            };
        } catch (error) {
            console.error('Logout error:', error);
            return {
                success: false,
                error: error.message
            };
        }
    }

    /**
     * Verify authentication token
     * @returns {Promise<object>} Verification response
     */
    async verifyToken() {
        try {
            const token = this.getToken();
            
            if (!token) {
                return {
                    success: false,
                    authenticated: false
                };
            }

            const response = await fetch(`${API_BASE_URL}/api/v1/Auth/verify`, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                },
                credentials: 'include'
            });

            if (!response.ok) {
                return {
                    success: false,
                    authenticated: false
                };
            }

            const data = await response.json();
            return {
                success: true,
                authenticated: true,
                data: data
            };
        } catch (error) {
            console.error('Token verification error:', error);
            return {
                success: false,
                authenticated: false,
                error: error.message
            };
        }
    }

    /**
     * Make authenticated API request
     * @param {string} endpoint - API endpoint (e.g., '/api/users')
     * @param {object} options - Fetch options
     * @returns {Promise<object>} API response
     */
    async apiRequest(endpoint, options = {}) {
        try {
            const defaultOptions = {
                headers: {
                    'Content-Type': 'application/json',
                },
                credentials: 'include'
            };

            const mergedOptions = {
                ...defaultOptions,
                ...options,
                headers: {
                    ...defaultOptions.headers,
                    ...options.headers
                }
            };

            const response = await fetch(`${API_BASE_URL}${endpoint}`, mergedOptions);

            if (!response.ok) {
                const errorData = await response.json().catch(() => ({}));
                throw new Error(errorData.message || `Request failed with status: ${response.status}`);
            }

            const data = await response.json();
            return {
                success: true,
                data: data
            };
        } catch (error) {
            console.error('API request error:', error);
            return {
                success: false,
                error: error.message
            };
        }
    }
}

// Create singleton instance
const authClient = new AuthClient();

// Export for use in other modules or make available globally
if (typeof module !== 'undefined' && module.exports) {
    module.exports = authClient;
}

/**
 * API Client Module for DR Admin Demo
 * Calls backend API directly at https://localhost:7201
 */
// API Base URL - calls backend API directly
const BASE_URL = 'https://localhost:7201/api/v1';

/**
 * Get JWT token from session storage
 */
function getAuthToken() {
    return sessionStorage.getItem('authToken');
}

/**
 * Generic API request handler
 */
async function apiRequest(endpoint, options = {}) {
    try {
        // Get JWT token from session storage
        const token = getAuthToken();
        
        // Build headers
        const headers = {
            'Content-Type': 'application/json',
            ...options.headers,
        };
        
        // Add Authorization header if token exists
        if (token) {
            headers['Authorization'] = `Bearer ${token}`;
        }
        
        const response = await fetch(endpoint, {
            headers,
            ...options,
        });
        
        // Handle 401 Unauthorized - redirect to login
        if (response.status === 401) {
            return {
                success: false,
                message: 'Authentication required (401 Unauthorized)',
                statusCode: 401
            };
        }
        
        // Handle 403 Forbidden
        if (response.status === 403) {
            return {
                success: false,
                message: 'Access denied. You do not have permission (403 Forbidden)',
                statusCode: 403
            };
        }
        
        const data = await response.json();
        if (!response.ok) {
            return {
                success: false,
                message: data.message || `Error ${response.status}: ${response.statusText}`,
                statusCode: response.status
            };
        }
        return {
            success: data.success !== false,
            data: data.data || data,
            message: data.message,
        };
    }
    catch (error) {
        console.error('API Request Error:', error);
        return {
            success: false,
            message: 'Network error. Please try again.',
        };
    }
}
/**
 * Authentication API calls
 */
const AuthAPI = {
    async login(email, password) {
        const response = await apiRequest(`${BASE_URL}/auth/login`, {
            method: 'POST',
            body: JSON.stringify({ username: email, password: password }),
        });
        
        // If login successful, store the token in session storage
        if (response.success && response.data && response.data.accessToken) {
            sessionStorage.setItem('authToken', response.data.accessToken);
            sessionStorage.setItem('refreshToken', response.data.refreshToken);
            sessionStorage.setItem('username', response.data.username);
            sessionStorage.setItem('roles', JSON.stringify(response.data.roles));
        }
        
        return response;
    },
    async register(name, email, password, confirmPassword) {
        return apiRequest(`${BASE_URL}/myaccount/register`, {
            method: 'POST',
            body: JSON.stringify({ 
                username: email,
                email: email, 
                password: password, 
                confirmPassword: confirmPassword,
                customerName: name,
                customerEmail: email
            }),
        });
    },
    async resetPassword(email) {
        return apiRequest(`${BASE_URL}/myaccount/reset-password`, {
            method: 'POST',
            body: JSON.stringify({ email }),
        });
    },
    async logout() {
        const refreshToken = sessionStorage.getItem('refreshToken');
        if (refreshToken) {
            await apiRequest(`${BASE_URL}/auth/logout`, {
                method: 'POST',
                body: JSON.stringify({ refreshToken }),
            });
        }
        
        // Clear session storage
        sessionStorage.removeItem('authToken');
        sessionStorage.removeItem('refreshToken');
        sessionStorage.removeItem('username');
        sessionStorage.removeItem('roles');
    }
};
/**
 * Domain API calls
 */
const DomainAPI = {
    async getDomains() {
        return apiRequest(`${BASE_URL}/domains`, {
            method: 'GET',
        });
    },
    async searchDomain(domain) {
        return apiRequest(`${BASE_URL}/domains/search?domain=${encodeURIComponent(domain)}`, {
            method: 'GET',
        });
    },
};
/**
 * Hosting API calls
 */
const HostingAPI = {
    async getHostingPlans() {
        return apiRequest(`${BASE_URL}/hosting/plans`, {
            method: 'GET',
        });
    },
    async getMyHosting() {
        return apiRequest(`${BASE_URL}/hosting/services`, {
            method: 'GET',
        });
    },
};
/**
 * Customer API calls
 */
const CustomerAPI = {
    async getCustomers(pageNumber, pageSize) {
        let url = `${BASE_URL}/customers`;
        if (pageNumber && pageSize) {
            url += `?pageNumber=${pageNumber}&pageSize=${pageSize}`;
        }
        return apiRequest(url, { method: 'GET' });
    },
    async getCustomer(id) {
        return apiRequest(`${BASE_URL}/customers/${id}`, { method: 'GET' });
    },
    async createCustomer(customerData) {
        return apiRequest(`${BASE_URL}/customers`, {
            method: 'POST',
            body: JSON.stringify(customerData),
        });
    },
    async updateCustomer(id, customerData) {
        return apiRequest(`${BASE_URL}/customers/${id}`, {
            method: 'PUT',
            body: JSON.stringify(customerData),
        });
    },
    async deleteCustomer(id) {
        return apiRequest(`${BASE_URL}/customers/${id}`, { method: 'DELETE' });
    },
};

/**
 * User API calls
 */
const UserAPI = {
    async getUsers(pageNumber, pageSize) {
        let url = `${BASE_URL}/users`;
        if (pageNumber && pageSize) {
            url += `?pageNumber=${pageNumber}&pageSize=${pageSize}`;
        }
        return apiRequest(url, { method: 'GET' });
    },
    async getUser(id) {
        return apiRequest(`${BASE_URL}/users/${id}`, { method: 'GET' });
    },
    async createUser(userData) {
        return apiRequest(`${BASE_URL}/users`, {
            method: 'POST',
            body: JSON.stringify(userData),
        });
    },
    async updateUser(id, userData) {
        return apiRequest(`${BASE_URL}/users/${id}`, {
            method: 'PUT',
            body: JSON.stringify(userData),
        });
    },
    async deleteUser(id) {
        return apiRequest(`${BASE_URL}/users/${id}`, { method: 'DELETE' });
    },
};
/**
 * Order API calls
 */
const OrderAPI = {
    async getOrders() {
        return apiRequest(`${BASE_URL}/orders`, {
            method: 'GET',
        });
    },
};
/**
 * Utility functions
 */
function showMessage(elementId, message, isError = false) {
    const element = document.getElementById(elementId);
    if (element) {
        element.textContent = message;
        element.classList.remove('d-none');
        element.classList.toggle('alert-danger', isError);
        element.classList.toggle('alert-success', !isError);
    }
}
function hideMessage(elementId) {
    const element = document.getElementById(elementId);
    if (element) {
        element.classList.add('d-none');
    }
}
// Export for use in other modules
if (typeof window !== 'undefined') {
    window.AuthAPI = AuthAPI;
    window.DomainAPI = DomainAPI;
    window.HostingAPI = HostingAPI;
    window.CustomerAPI = CustomerAPI;
    window.UserAPI = UserAPI;
    window.OrderAPI = OrderAPI;
    window.showMessage = showMessage;
    window.hideMessage = hideMessage;
}

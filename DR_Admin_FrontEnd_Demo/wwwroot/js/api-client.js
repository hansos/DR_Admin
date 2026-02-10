/**
 * API Client Module for DR Admin Demo
 * Updated to connect to real DR_Admin API at https://localhost:7201
 */
// API Base URL - calls DR_Admin API directly
const BASE_URL = 'https://localhost:7201/api/v1';
/**
 * Generic API request handler
 */
async function apiRequest(endpoint, options = {}) {
    try {
        const response = await fetch(endpoint, {
            headers: {
                'Content-Type': 'application/json',
                ...options.headers,
            },
            credentials: 'include', // Changed from 'same-origin' to support CORS
            ...options,
        });
        const data = await response.json();
        if (!response.ok) {
            return {
                success: false,
                message: data.message || 'An error occurred',
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
        return apiRequest(`${BASE_URL}/auth/login`, {
            method: 'POST',
            body: JSON.stringify({ username: email, password }),
        });
    },
    async register(name, email, password, confirmPassword) {
        return apiRequest(`${BASE_URL}/myaccount/register`, {
            method: 'POST',
            body: JSON.stringify({
                username: email,
                email,
                password,
                confirmPassword,
                customerName: name,
                customerEmail: email,
                customerPhone: '',
                customerAddress: ''
            }),
        });
    },
    async resetPassword(email) {
        return apiRequest(`${BASE_URL}/myaccount/request-password-reset`, {
            method: 'POST',
            body: JSON.stringify({ email }),
        });
    },
};
/**
 * Domain API calls
 */
const DomainAPI = {
    async getDomains() {
        return apiRequest(`${BASE_URL}/Domains/list`, {
            method: 'GET',
        });
    },
    async searchDomain(domain) {
        return apiRequest(`${BASE_URL}/Domains/search?domain=${encodeURIComponent(domain)}`, {
            method: 'GET',
        });
    },
};
/**
 * Hosting API calls
 */
const HostingAPI = {
    async getHostingPlans() {
        return apiRequest(`${BASE_URL}/Hosting/plans`, {
            method: 'GET',
        });
    },
    async getMyHosting() {
        return apiRequest(`${BASE_URL}/Hosting/services`, {
            method: 'GET',
        });
    },
};
/**
 * Customer API calls
 */
const CustomerAPI = {
    async getCustomers() {
        return apiRequest(`${BASE_URL}/Customers/list`, {
            method: 'GET',
        });
    },
};
/**
 * Order API calls
 */
const OrderAPI = {
    async getOrders() {
        return apiRequest(`${BASE_URL}/Orders/list`, {
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
    window.OrderAPI = OrderAPI;
    window.showMessage = showMessage;
    window.hideMessage = hideMessage;
}

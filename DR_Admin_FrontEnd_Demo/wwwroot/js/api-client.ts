/**
 * API Client Module for DR Admin Demo
 * Updated to connect to real DR_Admin API at https://localhost:7201
 */

// API Base URL - calls go through our frontend proxy controllers
const BASE_URL = '/api';

/**
 * Generic API request handler
 */
async function apiRequest<T>(
    endpoint: string,
    options: RequestInit = {}
): Promise<{ success: boolean; data?: T; message?: string }> {
    try {
        const response = await fetch(endpoint, {
            headers: {
                'Content-Type': 'application/json',
                ...options.headers,
            },
            credentials: 'same-origin',
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
    } catch (error) {
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
    async login(email: string, password: string) {
        return apiRequest(`${BASE_URL}/Account/login`, {
            method: 'POST',
            body: JSON.stringify({ email, password }),
        });
    },

    async register(name: string, email: string, password: string, confirmPassword: string) {
        return apiRequest(`${BASE_URL}/Account/register`, {
            method: 'POST',
            body: JSON.stringify({ name, email, password, confirmPassword }),
        });
    },

    async resetPassword(email: string) {
        return apiRequest(`${BASE_URL}/Account/reset-password`, {
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

    async searchDomain(domain: string) {
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
function showMessage(elementId: string, message: string, isError: boolean = false) {
    const element = document.getElementById(elementId);
    if (element) {
        element.textContent = message;
        element.classList.remove('d-none');
        element.classList.toggle('alert-danger', isError);
        element.classList.toggle('alert-success', !isError);
    }
}

function hideMessage(elementId: string) {
    const element = document.getElementById(elementId);
    if (element) {
        element.classList.add('d-none');
    }
}

// Export for use in other modules
if (typeof window !== 'undefined') {
    (window as any).AuthAPI = AuthAPI;
    (window as any).DomainAPI = DomainAPI;
    (window as any).HostingAPI = HostingAPI;
    (window as any).CustomerAPI = CustomerAPI;
    (window as any).OrderAPI = OrderAPI;
    (window as any).showMessage = showMessage;
    (window as any).hideMessage = hideMessage;
}

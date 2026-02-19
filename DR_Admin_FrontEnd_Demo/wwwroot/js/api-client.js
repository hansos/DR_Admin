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
        // Get auth token from localStorage
        const authToken = localStorage.getItem('authToken');

        // Build headers
        const headers = {
            'Content-Type': 'application/json',
            ...options.headers,
        };

        // Add auth token if available
        if (authToken) {
            headers['Authorization'] = `Bearer ${authToken}`;
        }

        const response = await fetch(endpoint, {
            headers,
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
        const result = await apiRequest(`${BASE_URL}/auth/login`, {
            method: 'POST',
            body: JSON.stringify({ username: email, password }),
        });

        // Store authentication info in localStorage on successful login
        if (result.success) {
            localStorage.setItem('userEmail', email);

            // Backend returns AccessToken and RefreshToken (not just "token")
            if (result.data) {
                if (result.data.accessToken) {
                    localStorage.setItem('authToken', result.data.accessToken);
                    console.log('Access token stored:', result.data.accessToken.substring(0, 20) + '...');
                }
                if (result.data.refreshToken) {
                    localStorage.setItem('refreshToken', result.data.refreshToken);
                    console.log('Refresh token stored');
                }
                if (result.data.username) {
                    localStorage.setItem('username', result.data.username);
                }
                if (result.data.roles) {
                    localStorage.setItem('userRoles', JSON.stringify(result.data.roles));
                }
                if (result.data.expiresAt) {
                    localStorage.setItem('tokenExpiresAt', result.data.expiresAt);
                }
            }
        }

        return result;
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
        return apiRequest(`${BASE_URL}/Customers`, {
            method: 'GET',
        });
    },
    async getCustomer(id) {
        return apiRequest(`${BASE_URL}/Customers/${id}`, {
            method: 'GET',
        });
    },
    async createCustomer(customerData) {
        return apiRequest(`${BASE_URL}/Customers`, {
            method: 'POST',
            body: JSON.stringify(customerData),
        });
    },
    async updateCustomer(id, customerData) {
        return apiRequest(`${BASE_URL}/Customers/${id}`, {
            method: 'PUT',
            body: JSON.stringify(customerData),
        });
    },
    async deleteCustomer(id) {
        return apiRequest(`${BASE_URL}/Customers/${id}`, {
            method: 'DELETE',
        });
    },
};
/**
 * User API calls
 */
const UserAPI = {
    async getUsers() {
        return apiRequest(`${BASE_URL}/Users`, {
            method: 'GET',
        });
    },
    async getUser(id) {
        return apiRequest(`${BASE_URL}/Users/${id}`, {
            method: 'GET',
        });
    },
    async createUser(userData) {
        return apiRequest(`${BASE_URL}/Users`, {
            method: 'POST',
            body: JSON.stringify(userData),
        });
    },
    async updateUser(id, userData) {
        return apiRequest(`${BASE_URL}/Users/${id}`, {
            method: 'PUT',
            body: JSON.stringify(userData),
        });
    },
    async deleteUser(id) {
        return apiRequest(`${BASE_URL}/Users/${id}`, {
            method: 'DELETE',
        });
    },
};
/**
 * Role API calls
 */
const RoleAPI = {
    async getRoles() {
        return apiRequest(`${BASE_URL}/Roles`, {
            method: 'GET',
        });
    },
    async getRole(id) {
        return apiRequest(`${BASE_URL}/Roles/${id}`, {
            method: 'GET',
        });
    },
};
/**
 * Contact Person API calls
 */
const ContactPersonAPI = {
    async getContactPersonsByCustomer(customerId) {
        return apiRequest(`${BASE_URL}/ContactPersons/customer/${customerId}`, {
            method: 'GET',
        });
    },
    async getContactPerson(id) {
        return apiRequest(`${BASE_URL}/ContactPersons/${id}`, {
            method: 'GET',
        });
    },
    async createContactPerson(contactPersonData) {
        return apiRequest(`${BASE_URL}/ContactPersons`, {
            method: 'POST',
            body: JSON.stringify(contactPersonData),
        });
    },
    async updateContactPerson(id, contactPersonData) {
        return apiRequest(`${BASE_URL}/ContactPersons/${id}`, {
            method: 'PUT',
            body: JSON.stringify(contactPersonData),
        });
    },
    async deleteContactPerson(id) {
        return apiRequest(`${BASE_URL}/ContactPersons/${id}`, {
            method: 'DELETE',
        });
    },
};
/**
 * Order API calls
 */
const OrderAPI = {
    async getOrders() {
        return apiRequest(`${BASE_URL}/Orders`, {
            method: 'GET',
        });
    },
};
/**
 * System Setting API calls
 */
const SystemSettingAPI = {
    async getSystemSettings() {
        return apiRequest(`${BASE_URL}/SystemSettings`, {
            method: 'GET',
        });
    },
    async getSystemSetting(id) {
        return apiRequest(`${BASE_URL}/SystemSettings/${id}`, {
            method: 'GET',
        });
    },
    async createSystemSetting(settingData) {
        return apiRequest(`${BASE_URL}/SystemSettings`, {
            method: 'POST',
            body: JSON.stringify(settingData),
        });
    },
    async updateSystemSetting(id, settingData) {
        return apiRequest(`${BASE_URL}/SystemSettings/${id}`, {
            method: 'PUT',
            body: JSON.stringify(settingData),
        });
    },
    async deleteSystemSetting(id) {
        return apiRequest(`${BASE_URL}/SystemSettings/${id}`, {
            method: 'DELETE',
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
    window.RoleAPI = RoleAPI;
    window.ContactPersonAPI = ContactPersonAPI;
    window.OrderAPI = OrderAPI;
    window.SystemSettingAPI = SystemSettingAPI;
    window.showMessage = showMessage;
    window.hideMessage = hideMessage;
}

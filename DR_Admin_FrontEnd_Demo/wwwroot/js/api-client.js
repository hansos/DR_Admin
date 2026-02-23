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
        const authToken = localStorage.getItem('authToken');
        const headers = {
            'Content-Type': 'application/json',
            ...options.headers,
        };
        if (authToken) {
            headers['Authorization'] = `Bearer ${authToken}`;
        }
        const response = await fetch(endpoint, {
            ...options,
            headers,
            credentials: 'include',
        });
        // Some responses (204, 401 with no body) have no JSON body
        const contentType = response.headers.get('content-type') || '';
        const hasJson = contentType.includes('application/json');
        const data = hasJson ? await response.json() : null;
        if (!response.ok) {
            return {
                success: false,
                message: (data && data.message) || `Request failed with status ${response.status}`,
            };
        }
        return {
            success: data?.success !== false,
            data: data?.data ?? data,
            message: data?.message,
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
        if (result.success) {
            localStorage.setItem('userEmail', email);
            const data = result.data;
            if (data) {
                if (data.accessToken) {
                    localStorage.setItem('authToken', data.accessToken);
                }
                if (data.refreshToken) {
                    localStorage.setItem('refreshToken', data.refreshToken);
                }
                if (data.username) {
                    localStorage.setItem('username', data.username);
                }
                if (data.roles) {
                    localStorage.setItem('userRoles', JSON.stringify(data.roles));
                }
                if (data.expiresAt) {
                    localStorage.setItem('tokenExpiresAt', data.expiresAt);
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
        return apiRequest(`${BASE_URL}/Domains/list`, { method: 'GET' });
    },
    async searchDomain(domain) {
        return apiRequest(`${BASE_URL}/Domains/search?domain=${encodeURIComponent(domain)}`, { method: 'GET' });
    },
};
/**
 * Hosting API calls
 */
const HostingAPI = {
    async getHostingPlans() {
        return apiRequest(`${BASE_URL}/Hosting/plans`, { method: 'GET' });
    },
    async getMyHosting() {
        return apiRequest(`${BASE_URL}/Hosting/services`, { method: 'GET' });
    },
};
/**
 * Customer API calls
 */
const CustomerAPI = {
    async getCustomers() { return apiRequest(`${BASE_URL}/Customers`, { method: 'GET' }); },
    async getCustomer(id) { return apiRequest(`${BASE_URL}/Customers/${id}`, { method: 'GET' }); },
    async createCustomer(data) { return apiRequest(`${BASE_URL}/Customers`, { method: 'POST', body: JSON.stringify(data) }); },
    async updateCustomer(id, data) { return apiRequest(`${BASE_URL}/Customers/${id}`, { method: 'PUT', body: JSON.stringify(data) }); },
    async deleteCustomer(id) { return apiRequest(`${BASE_URL}/Customers/${id}`, { method: 'DELETE' }); },
};
/**
 * User API calls
 */
const UserAPI = {
    async getUsers() { return apiRequest(`${BASE_URL}/Users`, { method: 'GET' }); },
    async getUser(id) { return apiRequest(`${BASE_URL}/Users/${id}`, { method: 'GET' }); },
    async createUser(data) { return apiRequest(`${BASE_URL}/Users`, { method: 'POST', body: JSON.stringify(data) }); },
    async updateUser(id, data) { return apiRequest(`${BASE_URL}/Users/${id}`, { method: 'PUT', body: JSON.stringify(data) }); },
    async deleteUser(id) { return apiRequest(`${BASE_URL}/Users/${id}`, { method: 'DELETE' }); },
};
/**
 * Role API calls
 */
const RoleAPI = {
    async getRoles() { return apiRequest(`${BASE_URL}/Roles`, { method: 'GET' }); },
    async getRole(id) { return apiRequest(`${BASE_URL}/Roles/${id}`, { method: 'GET' }); },
};
/**
 * Contact Person API calls
 */
const ContactPersonAPI = {
    async getContactPersonsByCustomer(customerId) { return apiRequest(`${BASE_URL}/ContactPersons/customer/${customerId}`, { method: 'GET' }); },
    async getContactPerson(id) { return apiRequest(`${BASE_URL}/ContactPersons/${id}`, { method: 'GET' }); },
    async createContactPerson(data) { return apiRequest(`${BASE_URL}/ContactPersons`, { method: 'POST', body: JSON.stringify(data) }); },
    async updateContactPerson(id, data) { return apiRequest(`${BASE_URL}/ContactPersons/${id}`, { method: 'PUT', body: JSON.stringify(data) }); },
    async deleteContactPerson(id) { return apiRequest(`${BASE_URL}/ContactPersons/${id}`, { method: 'DELETE' }); },
};
/**
 * Order API calls
 */
const OrderAPI = {
    async getOrders() { return apiRequest(`${BASE_URL}/Orders`, { method: 'GET' }); },
};
/**
 * System Setting API calls
 */
const SystemSettingAPI = {
    async getSystemSettings() { return apiRequest(`${BASE_URL}/SystemSettings`, { method: 'GET' }); },
    async getSystemSetting(id) { return apiRequest(`${BASE_URL}/SystemSettings/${id}`, { method: 'GET' }); },
    async createSystemSetting(data) { return apiRequest(`${BASE_URL}/SystemSettings`, { method: 'POST', body: JSON.stringify(data) }); },
    async updateSystemSetting(id, data) { return apiRequest(`${BASE_URL}/SystemSettings/${id}`, { method: 'PUT', body: JSON.stringify(data) }); },
    async deleteSystemSetting(id) { return apiRequest(`${BASE_URL}/SystemSettings/${id}`, { method: 'DELETE' }); },
};
/**
 * Server Type API calls
 */
const ServerTypeAPI = {
    async getAll() { return apiRequest(`${BASE_URL}/ServerTypes`, { method: 'GET' }); },
    async getActive() { return apiRequest(`${BASE_URL}/ServerTypes/active`, { method: 'GET' }); },
    async getById(id) { return apiRequest(`${BASE_URL}/ServerTypes/${id}`, { method: 'GET' }); },
    async create(data) { return apiRequest(`${BASE_URL}/ServerTypes`, { method: 'POST', body: JSON.stringify(data) }); },
    async update(id, data) { return apiRequest(`${BASE_URL}/ServerTypes/${id}`, { method: 'PUT', body: JSON.stringify(data) }); },
    async delete(id) { return apiRequest(`${BASE_URL}/ServerTypes/${id}`, { method: 'DELETE' }); },
};
/**
 * Host Provider API calls
 */
const HostProviderAPI = {
    async getAll() { return apiRequest(`${BASE_URL}/HostProviders`, { method: 'GET' }); },
    async getActive() { return apiRequest(`${BASE_URL}/HostProviders/active`, { method: 'GET' }); },
    async getById(id) { return apiRequest(`${BASE_URL}/HostProviders/${id}`, { method: 'GET' }); },
    async create(data) { return apiRequest(`${BASE_URL}/HostProviders`, { method: 'POST', body: JSON.stringify(data) }); },
    async update(id, data) { return apiRequest(`${BASE_URL}/HostProviders/${id}`, { method: 'PUT', body: JSON.stringify(data) }); },
    async delete(id) { return apiRequest(`${BASE_URL}/HostProviders/${id}`, { method: 'DELETE' }); },
};
/**
 * Operating System API calls
 */
const OperatingSystemAPI = {
    async getAll() { return apiRequest(`${BASE_URL}/OperatingSystems`, { method: 'GET' }); },
    async getActive() { return apiRequest(`${BASE_URL}/OperatingSystems/active`, { method: 'GET' }); },
    async getById(id) { return apiRequest(`${BASE_URL}/OperatingSystems/${id}`, { method: 'GET' }); },
    async create(data) { return apiRequest(`${BASE_URL}/OperatingSystems`, { method: 'POST', body: JSON.stringify(data) }); },
    async update(id, data) { return apiRequest(`${BASE_URL}/OperatingSystems/${id}`, { method: 'PUT', body: JSON.stringify(data) }); },
    async delete(id) { return apiRequest(`${BASE_URL}/OperatingSystems/${id}`, { method: 'DELETE' }); },
};
/**
 * Server API calls
 */
const ServerAPI = {
    async getAll() { return apiRequest(`${BASE_URL}/Servers`, { method: 'GET' }); },
    async getById(id) { return apiRequest(`${BASE_URL}/Servers/${id}`, { method: 'GET' }); },
    async create(data) { return apiRequest(`${BASE_URL}/Servers`, { method: 'POST', body: JSON.stringify(data) }); },
    async update(id, data) { return apiRequest(`${BASE_URL}/Servers/${id}`, { method: 'PUT', body: JSON.stringify(data) }); },
    async delete(id) { return apiRequest(`${BASE_URL}/Servers/${id}`, { method: 'DELETE' }); },
};
/**
 * Control Panel Type API calls
 */
const ControlPanelTypeAPI = {
    async getAll() { return apiRequest(`${BASE_URL}/ControlPanelTypes`, { method: 'GET' }); },
    async getActive() { return apiRequest(`${BASE_URL}/ControlPanelTypes/active`, { method: 'GET' }); },
    async getById(id) { return apiRequest(`${BASE_URL}/ControlPanelTypes/${id}`, { method: 'GET' }); },
    async create(data) { return apiRequest(`${BASE_URL}/ControlPanelTypes`, { method: 'POST', body: JSON.stringify(data) }); },
    async update(id, data) { return apiRequest(`${BASE_URL}/ControlPanelTypes/${id}`, { method: 'PUT', body: JSON.stringify(data) }); },
    async delete(id) { return apiRequest(`${BASE_URL}/ControlPanelTypes/${id}`, { method: 'DELETE' }); },
};
/**
 * Server Control Panel API calls
 */
const ServerControlPanelAPI = {
    async getAll() { return apiRequest(`${BASE_URL}/ServerControlPanels`, { method: 'GET' }); },
    async getByServerId(serverId) { return apiRequest(`${BASE_URL}/ServerControlPanels/server/${serverId}`, { method: 'GET' }); },
    async getByIpAddressId(ipAddressId) { return apiRequest(`${BASE_URL}/ServerControlPanels/ipaddress/${ipAddressId}`, { method: 'GET' }); },
    async getById(id) { return apiRequest(`${BASE_URL}/ServerControlPanels/${id}`, { method: 'GET' }); },
    async create(data) { return apiRequest(`${BASE_URL}/ServerControlPanels`, { method: 'POST', body: JSON.stringify(data) }); },
    async update(id, data) { return apiRequest(`${BASE_URL}/ServerControlPanels/${id}`, { method: 'PUT', body: JSON.stringify(data) }); },
    async delete(id) { return apiRequest(`${BASE_URL}/ServerControlPanels/${id}`, { method: 'DELETE' }); },
    async testConnection(id) { return apiRequest(`${BASE_URL}/ServerControlPanels/${id}/test-connection`, { method: 'POST' }); },
};
/**
 * Server IP Address API calls
 */
const ServerIpAddressAPI = {
    async getAll() { return apiRequest(`${BASE_URL}/ServerIpAddresses`, { method: 'GET' }); },
    async getByServerId(serverId) { return apiRequest(`${BASE_URL}/ServerIpAddresses/server/${serverId}`, { method: 'GET' }); },
    async getById(id) { return apiRequest(`${BASE_URL}/ServerIpAddresses/${id}`, { method: 'GET' }); },
    async create(data) { return apiRequest(`${BASE_URL}/ServerIpAddresses`, { method: 'POST', body: JSON.stringify(data) }); },
    async update(id, data) { return apiRequest(`${BASE_URL}/ServerIpAddresses/${id}`, { method: 'PUT', body: JSON.stringify(data) }); },
    async delete(id) { return apiRequest(`${BASE_URL}/ServerIpAddresses/${id}`, { method: 'DELETE' }); },
};
/**
 * DNS Record Type API calls
 */
const DnsRecordTypeAPI = {
    async getAll() { return apiRequest(`${BASE_URL}/DnsRecordTypes`, { method: 'GET' }); },
};
/**
 * DNS Record API calls
 */
const DnsRecordAPI = {
    async getByDomain(domainId) { return apiRequest(`${BASE_URL}/DnsRecords/domain/${domainId}`, { method: 'GET' }); },
    async getDeletedByDomain(domainId) { return apiRequest(`${BASE_URL}/DnsRecords/domain/${domainId}/deleted`, { method: 'GET' }); },
    async getById(id) { return apiRequest(`${BASE_URL}/DnsRecords/${id}`, { method: 'GET' }); },
    async create(data) { return apiRequest(`${BASE_URL}/DnsRecords`, { method: 'POST', body: JSON.stringify(data) }); },
    async update(id, data) { return apiRequest(`${BASE_URL}/DnsRecords/${id}`, { method: 'PUT', body: JSON.stringify(data) }); },
    async softDelete(id) { return apiRequest(`${BASE_URL}/DnsRecords/${id}`, { method: 'DELETE' }); },
    async hardDelete(id) { return apiRequest(`${BASE_URL}/DnsRecords/${id}/hard`, { method: 'DELETE' }); },
    async restore(id) { return apiRequest(`${BASE_URL}/DnsRecords/${id}/restore`, { method: 'POST' }); },
    async getPendingSyncByDomain(domainId) { return apiRequest(`${BASE_URL}/DnsRecords/domain/${domainId}/pending-sync`, { method: 'GET' }); },
    async markSynced(id) { return apiRequest(`${BASE_URL}/DnsRecords/${id}/mark-synced`, { method: 'POST' }); },
    async push(id) { return apiRequest(`${BASE_URL}/DnsRecords/${id}/push`, { method: 'POST' }); },
    async pushPending(domainId) { return apiRequest(`${BASE_URL}/DnsRecords/domain/${domainId}/push-pending`, { method: 'POST' }); },
};
/**
 * Registered Domain API calls
 */
const RegisteredDomainAPI = {
    async getAll() { return apiRequest(`${BASE_URL}/RegisteredDomains`, { method: 'GET' }); },
    async getById(id) { return apiRequest(`${BASE_URL}/RegisteredDomains/${id}`, { method: 'GET' }); },
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
    window.ServerTypeAPI = ServerTypeAPI;
    window.HostProviderAPI = HostProviderAPI;
    window.OperatingSystemAPI = OperatingSystemAPI;
    window.ServerAPI = ServerAPI;
    window.ServerIpAddressAPI = ServerIpAddressAPI;
    window.DnsRecordTypeAPI = DnsRecordTypeAPI;
    window.DnsRecordAPI = DnsRecordAPI;
    window.RegisteredDomainAPI = RegisteredDomainAPI;
    window.ControlPanelTypeAPI = ControlPanelTypeAPI;
    window.ServerControlPanelAPI = ServerControlPanelAPI;
    window.showMessage = showMessage;
    window.hideMessage = hideMessage;
}

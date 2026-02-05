/**
 * Domain Registration HTTP Client
 * Handles domain registration, availability checks, and pricing requests
 */

const API_BASE_URL = 'https://localhost:7201';

class DomainRegistrationClient {
    /**
     * Get authorization headers with bearer token
     * @returns {Object} Headers object
     */
    getAuthHeaders() {
        const token = localStorage.getItem('authToken') || localStorage.getItem('accessToken');
        
        const headers = {
            'Content-Type': 'application/json',
        };
        
        if (token) {
            headers['Authorization'] = `Bearer ${token}`;
        }
        
        return headers;
    }

    /**
     * Register a new domain (customer self-service)
     * @param {Object} data - Domain registration data
     * @param {string} data.domainName - Full domain name (e.g., example.com)
     * @param {number} data.years - Number of years (1-10)
     * @param {boolean} data.autoRenew - Enable auto-renewal
     * @param {boolean} data.privacyProtection - Enable privacy protection
     * @param {string} data.notes - Optional notes
     * @returns {Promise<Object>} Registration response
     */
    async registerDomain(data) {
        try {
            const response = await fetch(`${API_BASE_URL}/api/v1/RegisteredDomains/register`, {
                method: 'POST',
                headers: this.getAuthHeaders(),
                body: JSON.stringify(data),
                credentials: 'include'
            });

            if (!response.ok) {
                const errorData = await response.json().catch(() => ({}));
                throw new Error(errorData.message || errorData.title || `Registration failed with status: ${response.status}`);
            }

            const result = await response.json();
            return result;
        } catch (error) {
            console.error('Domain registration error:', error);
            throw error;
        }
    }

    /**
     * Register a domain for a specific customer (Admin/Sales only)
     * @param {Object} data - Domain registration data
     * @param {number} data.customerId - Customer ID
     * @param {string} data.domainName - Full domain name
     * @param {number} data.registrarId - Registrar ID
     * @param {number} data.years - Number of years (1-10)
     * @param {boolean} data.autoRenew - Enable auto-renewal
     * @param {boolean} data.privacyProtection - Enable privacy protection
     * @param {string} data.notes - Optional notes
     * @returns {Promise<Object>} Registration response
     */
    async registerDomainForCustomer(data) {
        try {
            const response = await fetch(`${API_BASE_URL}/api/v1/RegisteredDomains/register-for-customer`, {
                method: 'POST',
                headers: this.getAuthHeaders(),
                body: JSON.stringify(data),
                credentials: 'include'
            });

            if (!response.ok) {
                const errorData = await response.json().catch(() => ({}));
                throw new Error(errorData.message || errorData.title || `Registration failed with status: ${response.status}`);
            }

            const result = await response.json();
            return result;
        } catch (error) {
            console.error('Domain registration for customer error:', error);
            throw error;
        }
    }

    /**
     * Check if a domain is available for registration
     * @param {string} domainName - Full domain name to check
     * @returns {Promise<Object>} Availability response
     */
    async checkAvailability(domainName) {
        try {
            const response = await fetch(`${API_BASE_URL}/api/v1/RegisteredDomains/check-availability`, {
                method: 'POST',
                headers: this.getAuthHeaders(),
                body: JSON.stringify({ domainName }),
                credentials: 'include'
            });

            if (!response.ok) {
                const errorData = await response.json().catch(() => ({}));
                throw new Error(errorData.message || errorData.title || `Availability check failed with status: ${response.status}`);
            }

            const result = await response.json();
            return result;
        } catch (error) {
            console.error('Domain availability check error:', error);
            throw error;
        }
    }

    /**
     * Get pricing information for a specific TLD
     * @param {string} tld - Top-level domain (e.g., "com", "net")
     * @param {number|null} registrarId - Optional specific registrar ID
     * @returns {Promise<Object>} Pricing information
     */
    async getPricing(tld, registrarId = null) {
        try {
            let url = `${API_BASE_URL}/api/v1/RegisteredDomains/pricing/${tld}`;
            
            if (registrarId) {
                url += `?registrarId=${registrarId}`;
            }

            const response = await fetch(url, {
                method: 'GET',
                headers: this.getAuthHeaders(),
                credentials: 'include'
            });

            if (!response.ok) {
                if (response.status === 404) {
                    return null; // Pricing not found
                }
                const errorData = await response.json().catch(() => ({}));
                throw new Error(errorData.message || errorData.title || `Pricing fetch failed with status: ${response.status}`);
            }

            const result = await response.json();
            return result;
        } catch (error) {
            console.error('Domain pricing fetch error:', error);
            throw error;
        }
    }

    /**
     * Get all available TLDs with pricing
     * @returns {Promise<Array>} List of available TLDs
     */
    async getAvailableTlds() {
        try {
            const response = await fetch(`${API_BASE_URL}/api/v1/RegisteredDomains/available-tlds`, {
                method: 'GET',
                headers: this.getAuthHeaders(),
                credentials: 'include'
            });

            if (!response.ok) {
                const errorData = await response.json().catch(() => ({}));
                throw new Error(errorData.message || errorData.title || `TLDs fetch failed with status: ${response.status}`);
            }

            const result = await response.json();
            return result;
        } catch (error) {
            console.error('Available TLDs fetch error:', error);
            throw error;
        }
    }

    /**
     * Get list of all registered domains (with pagination)
     * @param {number} page - Page number (1-based)
     * @param {number} pageSize - Number of items per page
     * @returns {Promise<Object>} Paginated list of domains
     */
    async getRegisteredDomains(page = 1, pageSize = 20) {
        try {
            const response = await fetch(`${API_BASE_URL}/api/v1/RegisteredDomains?page=${page}&pageSize=${pageSize}`, {
                method: 'GET',
                headers: this.getAuthHeaders(),
                credentials: 'include'
            });

            if (!response.ok) {
                const errorData = await response.json().catch(() => ({}));
                throw new Error(errorData.message || errorData.title || `Fetch failed with status: ${response.status}`);
            }

            const result = await response.json();
            return result;
        } catch (error) {
            console.error('Registered domains fetch error:', error);
            throw error;
        }
    }

    /**
     * Get a specific domain by ID
     * @param {number} id - Domain ID
     * @returns {Promise<Object>} Domain details
     */
    async getDomainById(id) {
        try {
            const response = await fetch(`${API_BASE_URL}/api/v1/RegisteredDomains/${id}`, {
                method: 'GET',
                headers: this.getAuthHeaders(),
                credentials: 'include'
            });

            if (!response.ok) {
                if (response.status === 404) {
                    return null;
                }
                const errorData = await response.json().catch(() => ({}));
                throw new Error(errorData.message || errorData.title || `Fetch failed with status: ${response.status}`);
            }

            const result = await response.json();
            return result;
        } catch (error) {
            console.error('Domain fetch error:', error);
            throw error;
        }
    }

    /**
     * Search domains by name
     * @param {string} searchTerm - Search term
     * @returns {Promise<Array>} Matching domains
     */
    async searchDomains(searchTerm) {
        try {
            const response = await fetch(`${API_BASE_URL}/api/v1/RegisteredDomains/search?term=${encodeURIComponent(searchTerm)}`, {
                method: 'GET',
                headers: this.getAuthHeaders(),
                credentials: 'include'
            });

            if (!response.ok) {
                const errorData = await response.json().catch(() => ({}));
                throw new Error(errorData.message || errorData.title || `Search failed with status: ${response.status}`);
            }

            const result = await response.json();
            return result;
        } catch (error) {
            console.error('Domain search error:', error);
            throw error;
        }
    }
}

// Export for use in other scripts
if (typeof module !== 'undefined' && module.exports) {
    module.exports = DomainRegistrationClient;
}

// domains-all.js - Handle all domains page

let allDomains = [];
let filteredDomains = [];
let currentPage = 1;
const domainsPerPage = 10;

document.addEventListener('DOMContentLoaded', async () => {
    await loadAllDomains();
    await loadRegistrars();
    setupEventListeners();
});

function setupEventListeners() {
    const applyFiltersBtn = document.getElementById('applyFiltersBtn');
    const searchFilter = document.getElementById('searchFilter');

    if (applyFiltersBtn) {
        applyFiltersBtn.addEventListener('click', applyFilters);
    }

    if (searchFilter) {
        searchFilter.addEventListener('keyup', (e) => {
            if (e.key === 'Enter') {
                applyFilters();
            }
        });
    }
}

async function loadRegistrars() {
    try {
        const registrarFilter = document.getElementById('registrarFilter');

        console.log('Fetching registrars from API...');

        // Fetch registrars from API
        const authToken = localStorage.getItem('authToken');
        const headers = {
            'Content-Type': 'application/json'
        };

        if (authToken && !authToken.startsWith('demo-token-')) {
            headers['Authorization'] = `Bearer ${authToken}`;
        }

        const response = await fetch('https://localhost:7201/api/v1/Registrars/active', {
            method: 'GET',
            headers: headers,
            credentials: 'include'
        });

        if (response.ok) {
            const registrars = await response.json();
            console.log('Registrars loaded:', registrars);

            if (Array.isArray(registrars) && registrars.length > 0) {
                registrars.forEach(registrar => {
                    const option = document.createElement('option');
                    option.value = registrar.name;
                    option.textContent = registrar.name;
                    registrarFilter.appendChild(option);
                });
            }
        } else {
            // Fallback to sample data
            console.warn('Failed to load registrars from API, using fallback');
            const sampleRegistrars = ['AWS Route53', 'Cloudflare', 'GoDaddy', 'Namecheap', 'Google Domains'];
            sampleRegistrars.forEach(registrar => {
                const option = document.createElement('option');
                option.value = registrar;
                option.textContent = registrar;
                registrarFilter.appendChild(option);
            });
        }

    } catch (error) {
        console.error('Error loading registrars:', error);
        // Fallback to sample data on error
        const sampleRegistrars = ['AWS Route53', 'Cloudflare', 'GoDaddy', 'Namecheap', 'Google Domains'];
        const registrarFilter = document.getElementById('registrarFilter');
        sampleRegistrars.forEach(registrar => {
            const option = document.createElement('option');
            option.value = registrar;
            option.textContent = registrar;
            registrarFilter.appendChild(option);
        });
    }
}

async function loadAllDomains() {
    const loadingDiv = document.getElementById('loadingDomains');
    const tableDiv = document.getElementById('domainsTable');
    const noDomainsDiv = document.getElementById('noDomainsMessage');

    try {
        // Show loading
        loadingDiv.classList.remove('d-none');
        tableDiv.classList.add('d-none');
        noDomainsDiv.classList.add('d-none');

        console.log('Fetching all domains from API...');

        // Fetch domains from API: GET /api/v1/RegisteredDomains
        const authToken = localStorage.getItem('authToken');
        const headers = {
            'Content-Type': 'application/json'
        };

        if (authToken && !authToken.startsWith('demo-token-')) {
            headers['Authorization'] = `Bearer ${authToken}`;
        }

        const response = await fetch('https://localhost:7201/api/v1/RegisteredDomains', {
            method: 'GET',
            headers: headers,
            credentials: 'include'
        });

        console.log('Domains API Response Status:', response.status);

        if (!response.ok) {
            const errorText = await response.text();
            console.error('Domains API Error Response:', errorText);
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        const result = await response.json();
        console.log('Domains loaded from API:', result);

        // Handle backend response format
        if (result.domains && Array.isArray(result.domains)) {
            allDomains = result.domains;
        } else if (Array.isArray(result)) {
            allDomains = result;
        } else {
            console.warn('Unexpected response format, using fallback');
            allDomains = generateSampleAllDomains();
        }

        filteredDomains = [...allDomains];

        // Hide loading
        loadingDiv.classList.add('d-none');

        if (allDomains.length === 0) {
            noDomainsDiv.classList.remove('d-none');
            noDomainsDiv.innerHTML = '<i class="bi bi-info-circle"></i> No domains found in the database.';
            return;
        }

        updateDomainCount();
        displayDomains();
        tableDiv.classList.remove('d-none');

    } catch (error) {
        console.error('Error loading domains from API:', error);
        console.error('Error details:', { message: error.message, stack: error.stack });

        loadingDiv.classList.add('d-none');

        // Fallback to demo data on error
        console.log('Using demo data as fallback');
        allDomains = generateSampleAllDomains();
        filteredDomains = [...allDomains];

        if (allDomains.length > 0) {
            updateDomainCount();
            displayDomains();
            tableDiv.classList.remove('d-none');

            // Show warning that we're using demo data
            const warningDiv = document.createElement('div');
            warningDiv.className = 'alert alert-warning mt-3';
            warningDiv.innerHTML = '<i class="bi bi-exclamation-triangle"></i> <strong>Demo Mode:</strong> Could not connect to API. Showing sample data.';
            tableDiv.insertBefore(warningDiv, tableDiv.firstChild);
        } else {
            noDomainsDiv.classList.remove('d-none');
            noDomainsDiv.innerHTML = '<i class="bi bi-exclamation-circle"></i> Error loading domains. Please try again.';
        }
    }
}

function generateSampleAllDomains() {
    const domains = [];
    const customers = ['John Doe', 'Jane Smith', 'Acme Corp', 'Tech Solutions', 'Global Enterprises'];
    const statuses = ['Active', 'Active', 'Active', 'Expired', 'Pending', 'Suspended'];
    const registrars = ['AWS Route53', 'Cloudflare', 'GoDaddy', 'Namecheap', 'Google Domains'];
    const tlds = ['.com', '.net', '.org', '.io', '.tech', '.dev', '.app'];

    // Generate 25 sample domains
    for (let i = 1; i <= 25; i++) {
        const registeredDate = new Date();
        registeredDate.setDate(registeredDate.getDate() - Math.floor(Math.random() * 1000));
        
        const expiryDate = new Date(registeredDate);
        expiryDate.setFullYear(expiryDate.getFullYear() + 1);

        domains.push({
            id: i,
            domainName: `example${i}${tlds[i % tlds.length]}`,
            customerId: (i % 5) + 1,
            customerName: customers[i % customers.length],
            status: statuses[i % statuses.length],
            registrar: registrars[i % registrars.length],
            registeredDate: registeredDate.toISOString().split('T')[0],
            expiryDate: expiryDate.toISOString().split('T')[0],
            autoRenew: Math.random() > 0.3
        });
    }

    return domains;
}

function applyFilters() {
    const statusFilter = document.getElementById('statusFilter').value;
    const registrarFilter = document.getElementById('registrarFilter').value;
    const searchFilter = document.getElementById('searchFilter').value.toLowerCase();

    filteredDomains = allDomains.filter(domain => {
        // Get domain name (API returns 'name' not 'domainName')
        const domainName = (domain.name || domain.domainName || '').toLowerCase();

        const matchesStatus = !statusFilter || domain.status === statusFilter;
        const matchesRegistrar = !registrarFilter || domain.registrar === registrarFilter;
        const matchesSearch = !searchFilter || domainName.includes(searchFilter);

        return matchesStatus && matchesRegistrar && matchesSearch;
    });

    currentPage = 1;
    updateDomainCount();
    displayDomains();
}

function displayDomains() {
    const tableBody = document.getElementById('domainsTableBody');
    const noDomainsDiv = document.getElementById('noDomainsMessage');
    const tableDiv = document.getElementById('domainsTable');

    if (filteredDomains.length === 0) {
        tableDiv.classList.add('d-none');
        noDomainsDiv.classList.remove('d-none');
        noDomainsDiv.innerHTML = '<i class="bi bi-info-circle"></i> No domains match the current filters.';
        return;
    }

    tableDiv.classList.remove('d-none');
    noDomainsDiv.classList.add('d-none');

    const startIndex = (currentPage - 1) * domainsPerPage;
    const endIndex = startIndex + domainsPerPage;
    const domainsToDisplay = filteredDomains.slice(startIndex, endIndex);

    tableBody.innerHTML = '';
    domainsToDisplay.forEach(domain => {
        const row = createDomainRow(domain);
        tableBody.appendChild(row);
    });

    renderPagination();
}

function createDomainRow(domain) {
    const row = document.createElement('tr');

    // API returns 'name' not 'domainName'
    const domainName = domain.name || domain.domainName || 'N/A';

    // Format dates - handle multiple property name variations
    const registeredDate = domain.registrationDate || domain.registeredDate || domain.createdAt || 'N/A';
    const expiryDate = domain.expirationDate || domain.expiryDate || domain.expiry || 'N/A';

    // Determine user-friendly status
    const userFriendlyStatus = determineStatus(domain, expiryDate, registeredDate);
    const statusBadge = getStatusBadge(userFriendlyStatus);

    const autoRenewIcon = domain.autoRenew 
        ? '<i class="bi bi-check-circle-fill text-success"></i>' 
        : '<i class="bi bi-x-circle-fill text-secondary"></i>';

    // Extract customer name (from customer object or fallback to ID)
    let customerName = 'N/A';

    // First try to get customer from the customer object (API structure)
    if (domain.customer?.name) {
        customerName = domain.customer.name;
    } else if (domain.customerName) {
        customerName = domain.customerName;
    } else if (domain.customer?.customerName) {
        customerName = domain.customer.customerName;
    } else if (domain.customer) {
        // Try to build name from customer object properties
        if (domain.customer.organization) {
            customerName = domain.customer.organization;
        } else if (domain.customer.firstName || domain.customer.lastName) {
            const firstName = domain.customer.firstName || '';
            const lastName = domain.customer.lastName || '';
            customerName = `${firstName} ${lastName}`.trim() || 'N/A';
        } else if (domain.customer.email) {
            customerName = domain.customer.email;
        }
    } else if (domain.customerId) {
        // Fallback to customer ID if no name available
        customerName = `Customer #${domain.customerId}`;
    } else {
        // Check for contacts array (registrar data structure)
        let registrantContact = null;
        if (domain.contacts && Array.isArray(domain.contacts)) {
            registrantContact = domain.contacts.find(c => c.contactType === 'Registrant');
        }

        if (registrantContact?.organization) {
            customerName = registrantContact.organization;
        } else if (registrantContact?.firstName || registrantContact?.lastName) {
            const firstName = registrantContact.firstName || '';
            const lastName = registrantContact.lastName || '';
            customerName = `${firstName} ${lastName}`.trim() || 'N/A';
        }
    }

    // Get registrar/provider name
    let registrarName = 'N/A';

    // First try to get registrar from the registrar object (API structure)
    if (domain.registrar?.name) {
        registrarName = domain.registrar.name;
    } else if (domain.registrarName) {
        registrarName = domain.registrarName;
    } else if (typeof domain.registrar === 'string') {
        registrarName = domain.registrar;
    } else if (domain.providerId) {
        // Fallback to provider ID if no name available
        registrarName = `Provider #${domain.providerId}`;
    }

    row.innerHTML = `
        <td><strong>${domainName}</strong></td>
        <td>${customerName}</td>
        <td>${statusBadge}</td>
        <td>${registrarName}</td>
        <td>${formatDate(registeredDate)}</td>
        <td>${formatDate(expiryDate)}</td>
        <td class="text-center">${autoRenewIcon}</td>
        <td>
            <div class="btn-group btn-group-sm" role="group">
                <button type="button" class="btn btn-outline-primary btn-view-details" data-domain-id="${domain.id}" title="View Details">
                    <i class="bi bi-eye"></i>
                </button>
                <button type="button" class="btn btn-outline-secondary" title="Manage DNS">
                    <i class="bi bi-gear"></i>
                </button>
                <button type="button" class="btn btn-outline-success" title="Renew">
                    <i class="bi bi-arrow-clockwise"></i>
                </button>
            </div>
        </td>
    `;

    // Add click handler for View Details button
    const viewDetailsBtn = row.querySelector('.btn-view-details');
    if (viewDetailsBtn) {
        viewDetailsBtn.addEventListener('click', () => {
            window.location.href = `/domain-details.html?id=${domain.id}`;
        });
    }

    return row;
}

function determineStatus(domain, expiryDate, registeredDate) {
    // If we have a simple status already, use it
    if (domain.status && !domain.status.includes(',') && !domain.status.includes('Prohibited')) {
        const simpleStatuses = ['Active', 'Expired', 'Suspended', 'Cancelled'];
        if (simpleStatuses.includes(domain.status)) {
            return domain.status;
        }
    }

    // Determine status based on dates
    const now = new Date();

    try {
        // Check expiration date
        if (expiryDate && expiryDate !== 'N/A') {
            const expiry = new Date(expiryDate);

            // Already expired
            if (expiry < now) {
                return 'Expired';
            }

            // Expiring soon (within 30 days)
            const daysUntilExpiry = Math.floor((expiry - now) / (1000 * 60 * 60 * 24));
            if (daysUntilExpiry <= 30) {
                return 'Expiring Soon';
            }
        }

        // Check if newly registered (within 7 days)
        if (registeredDate && registeredDate !== 'N/A') {
            const registered = new Date(registeredDate);
            const daysSinceRegistration = Math.floor((now - registered) / (1000 * 60 * 60 * 24));
            if (daysSinceRegistration <= 7 && daysSinceRegistration >= 0) {
                return 'New';
            }
        }

    } catch (error) {
        console.warn('Error determining status:', error);
    }

    // Default to Active if nothing else matches
    return 'Active';
}

function formatDate(dateString) {
    if (!dateString || dateString === 'N/A') return 'N/A';
    try {
        const date = new Date(dateString);
        return date.toISOString().split('T')[0];
    } catch {
        return dateString;
    }
}

function renderPagination() {
    const pagination = document.getElementById('pagination');
    const totalPages = Math.ceil(filteredDomains.length / domainsPerPage);

    if (totalPages <= 1) {
        pagination.innerHTML = '';
        return;
    }

    pagination.innerHTML = '';

    // Previous button
    const prevLi = document.createElement('li');
    prevLi.className = `page-item ${currentPage === 1 ? 'disabled' : ''}`;
    prevLi.innerHTML = `<a class="page-link" href="#" aria-label="Previous"><span aria-hidden="true">&laquo;</span></a>`;
    prevLi.addEventListener('click', (e) => {
        e.preventDefault();
        if (currentPage > 1) {
            currentPage--;
            displayDomains();
        }
    });
    pagination.appendChild(prevLi);

    // Page numbers
    for (let i = 1; i <= totalPages; i++) {
        const pageLi = document.createElement('li');
        pageLi.className = `page-item ${i === currentPage ? 'active' : ''}`;
        pageLi.innerHTML = `<a class="page-link" href="#">${i}</a>`;
        pageLi.addEventListener('click', (e) => {
            e.preventDefault();
            currentPage = i;
            displayDomains();
        });
        pagination.appendChild(pageLi);
    }

    // Next button
    const nextLi = document.createElement('li');
    nextLi.className = `page-item ${currentPage === totalPages ? 'disabled' : ''}`;
    nextLi.innerHTML = `<a class="page-link" href="#" aria-label="Next"><span aria-hidden="true">&raquo;</span></a>`;
    nextLi.addEventListener('click', (e) => {
        e.preventDefault();
        if (currentPage < totalPages) {
            currentPage++;
            displayDomains();
        }
    });
    pagination.appendChild(nextLi);
}

function updateDomainCount() {
    const domainCount = document.getElementById('domainCount');
    domainCount.textContent = `${filteredDomains.length} domain${filteredDomains.length !== 1 ? 's' : ''}`;
}

function getStatusBadge(status) {
    const badges = {
        'Active': 'badge bg-success',
        'Expired': 'badge bg-danger',
        'Expiring Soon': 'badge bg-warning text-dark',
        'New': 'badge bg-primary',
        'Pending': 'badge bg-warning text-dark',
        'Suspended': 'badge bg-dark',
        'Cancelled': 'badge bg-secondary'
    };

    const badgeClass = badges[status] || 'badge bg-secondary';
    return `<span class="${badgeClass}">${status}</span>`;
}

function showAlert(elementId, message, type) {
    const element = document.getElementById(elementId);
    if (element) {
        element.className = `alert alert-${type}`;
        element.innerHTML = `<i class="bi bi-info-circle"></i> ${message}`;
        element.classList.remove('d-none');
    }
}

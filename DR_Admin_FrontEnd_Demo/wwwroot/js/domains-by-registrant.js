// domains-by-registrant.js - Handle domains by registrar page

document.addEventListener('DOMContentLoaded', async () => {
    await loadRegistrars();
    setupEventListeners();
});

function setupEventListeners() {
    const loadDomainsBtn = document.getElementById('loadDomainsBtn');
    const downloadDomainsBtn = document.getElementById('downloadDomainsBtn');
    const registrarSelect = document.getElementById('registrarSelect');

    if (loadDomainsBtn) {
        loadDomainsBtn.addEventListener('click', async () => {
            const registrarId = registrarSelect.value;
            if (registrarId) {
                await loadDomainsByRegistrar(registrarId);
            } else {
                showAlert('noDomainsMessage', 'Please select a registrar first.', 'warning');
            }
        });
    }

    if (downloadDomainsBtn) {
        downloadDomainsBtn.addEventListener('click', async () => {
            const registrarId = registrarSelect.value;
            if (registrarId) {
                await downloadDomainsFromRegistrar(registrarId);
            } else {
                showDownloadStatus('Please select a registrar first.', 'warning');
            }
        });
    }
}

async function loadRegistrars() {
    const registrarSelect = document.getElementById('registrarSelect');

    try {
        // Show loading state
        registrarSelect.innerHTML = '<option value="">Loading registrars...</option>';
        registrarSelect.disabled = true;

        console.log('Fetching registrars from API...');

        // Fetch registrars from API
        // API endpoint: GET /api/v1/Registrars/active (no auth required)
        const authToken = localStorage.getItem('authToken');
        const headers = {
            'Content-Type': 'application/json'
        };

        // Only add Authorization header if we have a token
        if (authToken && !authToken.startsWith('demo-token-')) {
            headers['Authorization'] = `Bearer ${authToken}`;
        }

        const response = await fetch('https://localhost:7201/api/v1/Registrars/active', {
            method: 'GET',
            headers: headers,
            credentials: 'include'
        });

        console.log('API Response Status:', response.status);

        if (!response.ok) {
            const errorText = await response.text();
            console.error('API Error Response:', errorText);
            throw new Error(`HTTP error! status: ${response.status}, message: ${errorText}`);
        }

        const registrars = await response.json();
        console.log('Registrars loaded from API:', registrars);

        // Populate the dropdown
        registrarSelect.innerHTML = '<option value="">-- Select a Registrar --</option>';

        if (Array.isArray(registrars) && registrars.length > 0) {
            console.log(`Populating dropdown with ${registrars.length} registrars`);
            registrars.forEach(registrar => {
                const option = document.createElement('option');
                option.value = registrar.id;
                option.textContent = `${registrar.name} (${registrar.code})`;
                option.dataset.name = registrar.name;
                registrarSelect.appendChild(option);
            });

            // Add success indicator
            const successMsg = document.createElement('small');
            successMsg.className = 'text-success d-block mt-1';
            successMsg.innerHTML = '<i class="bi bi-check-circle"></i> Loaded from database';
            registrarSelect.parentElement.appendChild(successMsg);
            setTimeout(() => successMsg.remove(), 3000);
        } else {
            console.warn('No registrars returned from API');
            registrarSelect.innerHTML = '<option value="">No registrars available</option>';
        }

        registrarSelect.disabled = false;

    } catch (error) {
        console.error('Error loading registrars from API:', error);
        console.error('Error details:', {
            message: error.message,
            stack: error.stack
        });

        // Show error message
        const errorMsg = document.createElement('small');
        errorMsg.className = 'text-danger d-block mt-1';
        errorMsg.innerHTML = `<i class="bi bi-exclamation-triangle"></i> API Error: ${error.message}. Using demo data.`;
        registrarSelect.parentElement.appendChild(errorMsg);

        // Fallback to demo data if API fails
        console.log('Falling back to demo data for registrars');
        const sampleRegistrars = [
            { id: 1, code: 'aws', name: 'AWS Route53 (DEMO)' },
            { id: 2, code: 'cloudflare', name: 'Cloudflare (DEMO)' },
            { id: 3, code: 'godaddy', name: 'GoDaddy (DEMO)' },
            { id: 4, code: 'namecheap', name: 'Namecheap (DEMO)' },
            { id: 5, code: 'google', name: 'Google Domains (DEMO)' }
        ];

        registrarSelect.innerHTML = '<option value="">-- Select a Registrar (Demo Data) --</option>';
        sampleRegistrars.forEach(registrar => {
            const option = document.createElement('option');
            option.value = registrar.id;
            option.textContent = `${registrar.name} (${registrar.code})`;
            option.dataset.name = registrar.name;
            registrarSelect.appendChild(option);
        });

        registrarSelect.disabled = false;
    }
}

async function loadDomainsByRegistrar(registrarId) {
    const loadingDiv = document.getElementById('loadingDomains');
    const tableDiv = document.getElementById('domainsTable');
    const noDomainsDiv = document.getElementById('noDomainsMessage');
    const tableBody = document.getElementById('domainsTableBody');
    const selectedRegistrarName = document.getElementById('selectedRegistrarName');
    const registrarSelect = document.getElementById('registrarSelect');

    try {
        // Show loading
        loadingDiv.classList.remove('d-none');
        tableDiv.classList.add('d-none');
        noDomainsDiv.classList.add('d-none');

        // Get registrar name
        const selectedOption = registrarSelect.options[registrarSelect.selectedIndex];
        const registrarName = selectedOption.dataset.name || selectedOption.textContent;

        console.log(`Fetching domains for registrar ID: ${registrarId}`);

        // Fetch domains from API
        // API endpoint: GET /api/v1/Registrars/{registrarId}/domains
        const authToken = localStorage.getItem('authToken');
        const headers = {
            'Content-Type': 'application/json'
        };

        // Only add Authorization header if we have a valid token
        if (authToken && !authToken.startsWith('demo-token-')) {
            headers['Authorization'] = `Bearer ${authToken}`;
        }

        const response = await fetch(`https://localhost:7201/api/v1/Registrars/${registrarId}/domains`, {
            method: 'GET',
            headers: headers,
            credentials: 'include'
        });

        console.log('Domains API Response Status:', response.status);

        if (!response.ok) {
            const errorText = await response.text();
            console.error('Domains API Error Response:', errorText);
            throw new Error(`HTTP error! status: ${response.status}, message: ${errorText}`);
        }

        const result = await response.json();
        console.log('Domains loaded from API:', result);

        // Hide loading
        loadingDiv.classList.add('d-none');

        // Handle backend response format: {success: true, domains: [...], totalCount: 6}
        let domainList = [];
        if (result.domains && Array.isArray(result.domains)) {
            domainList = result.domains;
        } else if (Array.isArray(result)) {
            // Fallback: if backend returns array directly
            domainList = result;
        }

        if (domainList.length === 0) {
            noDomainsDiv.classList.remove('d-none');
            noDomainsDiv.innerHTML = `<i class="bi bi-info-circle"></i> No domains found for this registrar in the database.`;
            return;
        }

        // Display registrar name
        selectedRegistrarName.innerHTML = `${registrarName} <small class="text-success">(${domainList.length} domain${domainList.length !== 1 ? 's' : ''} from database)</small>`;

        // Populate table
        tableBody.innerHTML = '';
        domainList.forEach(domain => {
            const row = createDomainRow(domain);
            tableBody.appendChild(row);
        });

        tableDiv.classList.remove('d-none');

    } catch (error) {
        console.error('Error loading domains from API:', error);
        console.error('Error details:', {
            message: error.message,
            stack: error.stack
        });

        loadingDiv.classList.add('d-none');

        // Show demo data on error
        console.log('Falling back to demo data for domains');
        const demoData = generateSampleDomains(registrarId);

        if (demoData.length === 0) {
            noDomainsDiv.classList.remove('d-none');
            noDomainsDiv.innerHTML = `<i class="bi bi-info-circle"></i> No domains found (using demo data). API Error: ${error.message}`;
            return;
        }

        const selectedOption = registrarSelect.options[registrarSelect.selectedIndex];
        const registrarName = selectedOption.dataset.name || selectedOption.textContent;
        selectedRegistrarName.innerHTML = `${registrarName} <small class="text-warning">(Demo Data - API unavailable)</small>`;

        tableBody.innerHTML = '';
        demoData.forEach(domain => {
            const row = createDomainRow(domain);
            tableBody.appendChild(row);
        });

        tableDiv.classList.remove('d-none');
    }
}

function generateSampleDomains(registrarId) {
    // Generate sample domains based on registrar ID
    const domainCount = Math.floor(Math.random() * 5) + 1; // 1-5 domains
    const domains = [];

    const statuses = ['Active', 'Active', 'Active', 'Expired', 'Pending'];
    const customers = ['John Doe', 'Jane Smith', 'Acme Corp', 'Tech Solutions', 'Global Enterprises'];
    const tlds = ['.com', '.net', '.org', '.io', '.tech'];

    for (let i = 0; i < domainCount; i++) {
        const registeredDate = new Date();
        registeredDate.setFullYear(registeredDate.getFullYear() - Math.floor(Math.random() * 3));

        const expiryDate = new Date(registeredDate);
        expiryDate.setFullYear(expiryDate.getFullYear() + 1);

        domains.push({
            id: i + 1,
            domainName: `registrar${registrarId}-domain${i + 1}${tlds[i % tlds.length]}`,
            customerName: customers[i % customers.length],
            status: statuses[i % statuses.length],
            registeredDate: registeredDate.toISOString().split('T')[0],
            expiryDate: expiryDate.toISOString().split('T')[0],
            autoRenew: Math.random() > 0.3
        });
    }

    return domains;
}

function createDomainRow(domain) {
    const row = document.createElement('tr');

    // Format dates if they exist
    // Backend can return: registrationDate, registeredDate, createdAt
    const registeredDate = domain.registrationDate || domain.registeredDate || domain.createdAt || 'N/A';
    // Backend can return: expirationDate, expiryDate, expiry
    const expiryDate = domain.expirationDate || domain.expiryDate || domain.expiry || 'N/A';

    // Determine user-friendly status based on dates and technical status
    const userFriendlyStatus = determineStatus(domain, expiryDate, registeredDate);
    const statusBadge = getStatusBadge(userFriendlyStatus);

    const autoRenewIcon = domain.autoRenew 
        ? '<i class="bi bi-check-circle-fill text-success"></i>' 
        : '<i class="bi bi-x-circle-fill text-secondary"></i>';

    // Show Organization, or First + Last Name if organization is null
    let customerName = 'N/A';

    // Find the Registrant contact from the contacts array
    let registrantContact = null;
    if (domain.contacts && Array.isArray(domain.contacts)) {
        registrantContact = domain.contacts.find(c => c.contactType === 'Registrant');
    }

    // Check for organization first (from registrant contact or direct property)
    if (registrantContact?.organization) {
        customerName = registrantContact.organization;
    } else if (domain.organization || domain.customer?.organization) {
        customerName = domain.organization || domain.customer.organization;
    }
    // If no organization, use first + last name from registrant contact
    else if (registrantContact?.firstName || registrantContact?.lastName) {
        const firstName = registrantContact.firstName || '';
        const lastName = registrantContact.lastName || '';
        customerName = `${firstName} ${lastName}`.trim() || 'N/A';
    }
    // Fallback to domain properties
    else if (domain.firstName || domain.lastName || domain.customer?.firstName || domain.customer?.lastName) {
        const firstName = domain.firstName || domain.customer?.firstName || '';
        const lastName = domain.lastName || domain.customer?.lastName || '';
        customerName = `${firstName} ${lastName}`.trim() || 'N/A';
    }
    // Fallback to customerName or customer.name
    else if (domain.customerName || domain.customer?.name) {
        customerName = domain.customerName || domain.customer.name;
    }

    row.innerHTML = `
        <td><strong>${domain.domainName}</strong></td>
        <td>${customerName}</td>
        <td>${statusBadge}</td>
        <td>${formatDate(registeredDate)}</td>
        <td>${formatDate(expiryDate)}</td>
        <td class="text-center">${autoRenewIcon}</td>
        <td>
            <div class="btn-group btn-group-sm" role="group">
                <button type="button" class="btn btn-outline-primary" title="View Details">
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

async function downloadDomainsFromRegistrar(registrarId) {
    const downloadBtn = document.getElementById('downloadDomainsBtn');
    const registrarSelect = document.getElementById('registrarSelect');

    try {
        // Get registrar name
        const selectedOption = registrarSelect.options[registrarSelect.selectedIndex];
        const registrarName = selectedOption.dataset.name || selectedOption.textContent;

        // Disable button and show loading state
        downloadBtn.disabled = true;
        downloadBtn.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>Syncing domains...';

        console.log(`Downloading domains for registrar ID: ${registrarId}`);

        // Call the download API endpoint
        // API endpoint: POST /api/v1/Registrars/{registrarId}/domains/download
        const authToken = localStorage.getItem('authToken');
        const headers = {
            'Content-Type': 'application/json'
        };

        // Add Authorization header if we have a valid token
        if (authToken && !authToken.startsWith('demo-token-')) {
            headers['Authorization'] = `Bearer ${authToken}`;
        }

        const response = await fetch(`https://localhost:7201/api/v1/Registrars/${registrarId}/domains/download`, {
            method: 'POST',
            headers: headers,
            credentials: 'include'
        });

        console.log('Download API Response Status:', response.status);

        if (!response.ok) {
            const errorText = await response.text();
            console.error('Download API Error Response:', errorText);
            throw new Error(`HTTP error! status: ${response.status}, message: ${errorText}`);
        }

        const result = await response.json();
        console.log('Download result:', result);

        // Show success message
        const message = result.message || `Successfully synced domains from ${registrarName}`;
        const count = result.count || result.totalCount || 0;
        showDownloadStatus(
            `<strong>Success!</strong> ${message}. ${count} domain${count !== 1 ? 's' : ''} synced.`,
            'success'
        );

        // Reload the domains list to show updated data
        setTimeout(async () => {
            await loadDomainsByRegistrar(registrarId);
        }, 1500);

    } catch (error) {
        console.error('Error downloading domains:', error);
        console.error('Error details:', {
            message: error.message,
            stack: error.stack
        });

        showDownloadStatus(
            `<strong>Error!</strong> Failed to sync domains: ${error.message}`,
            'danger'
        );

    } finally {
        // Re-enable button
        downloadBtn.disabled = false;
        downloadBtn.innerHTML = '<i class="bi bi-cloud-download"></i> Sync Domains from Registrar';
    }
}

function showDownloadStatus(message, type) {
    const statusDiv = document.getElementById('downloadStatus');
    if (statusDiv) {
        statusDiv.className = `alert alert-${type}`;
        statusDiv.innerHTML = message;
        statusDiv.classList.remove('d-none');

        // Auto-hide after 10 seconds
        setTimeout(() => {
            statusDiv.classList.add('d-none');
        }, 10000);
    }
}

function getStatusBadge(status) {
    const badges = {
        'Active': 'badge bg-success',
        'Expired': 'badge bg-danger',
        'Expiring Soon': 'badge bg-warning text-dark',
        'New': 'badge bg-primary',
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

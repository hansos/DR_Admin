// domains-by-registrant.js - Handle domains by registrar page

document.addEventListener('DOMContentLoaded', async () => {
    await loadRegistrars();
    setupEventListeners();
});

function setupEventListeners() {
    const loadDomainsBtn = document.getElementById('loadDomainsBtn');
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

    const statusBadge = getStatusBadge(domain.status);
    const autoRenewIcon = domain.autoRenew 
        ? '<i class="bi bi-check-circle-fill text-success"></i>' 
        : '<i class="bi bi-x-circle-fill text-secondary"></i>';

    // Format dates if they exist
    const registeredDate = domain.registeredDate || domain.createdAt || 'N/A';
    const expiryDate = domain.expiryDate || domain.expiry || 'N/A';
    const customerName = domain.customerName || domain.customer?.name || 'N/A';

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

function formatDate(dateString) {
    if (!dateString || dateString === 'N/A') return 'N/A';
    try {
        const date = new Date(dateString);
        return date.toISOString().split('T')[0];
    } catch {
        return dateString;
    }
}

function getStatusBadge(status) {
    const badges = {
        'Active': 'badge bg-success',
        'Expired': 'badge bg-danger',
        'Pending': 'badge bg-warning text-dark',
        'Suspended': 'badge bg-secondary'
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

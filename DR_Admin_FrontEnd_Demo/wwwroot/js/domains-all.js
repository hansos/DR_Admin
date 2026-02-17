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
        
        // For demo purposes, create sample registrars
        // In production, fetch from: GET /api/v1/Registrars
        const sampleRegistrars = [
            'AWS Route53',
            'Cloudflare',
            'GoDaddy',
            'Namecheap',
            'Google Domains'
        ];

        sampleRegistrars.forEach(registrar => {
            const option = document.createElement('option');
            option.value = registrar;
            option.textContent = registrar;
            registrarFilter.appendChild(option);
        });

    } catch (error) {
        console.error('Error loading registrars:', error);
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

        // In production, fetch from: GET /api/v1/RegisteredDomains
        // This endpoint requires Admin.Only policy
        // For demo purposes, generate sample data
        allDomains = generateSampleAllDomains();
        filteredDomains = [...allDomains];

        // Hide loading
        loadingDiv.classList.add('d-none');

        if (allDomains.length === 0) {
            noDomainsDiv.classList.remove('d-none');
            return;
        }

        updateDomainCount();
        displayDomains();
        tableDiv.classList.remove('d-none');

    } catch (error) {
        console.error('Error loading domains:', error);
        loadingDiv.classList.add('d-none');
        showAlert('noDomainsMessage', 'Error loading domains. Please try again.', 'danger');
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
        const matchesStatus = !statusFilter || domain.status === statusFilter;
        const matchesRegistrar = !registrarFilter || domain.registrar === registrarFilter;
        const matchesSearch = !searchFilter || domain.domainName.toLowerCase().includes(searchFilter);

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
    
    const statusBadge = getStatusBadge(domain.status);
    const autoRenewIcon = domain.autoRenew 
        ? '<i class="bi bi-check-circle-fill text-success"></i>' 
        : '<i class="bi bi-x-circle-fill text-secondary"></i>';

    row.innerHTML = `
        <td><strong>${domain.domainName}</strong></td>
        <td>${domain.customerName}</td>
        <td>${statusBadge}</td>
        <td>${domain.registrar}</td>
        <td>${domain.registeredDate}</td>
        <td>${domain.expiryDate}</td>
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

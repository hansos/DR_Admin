// domain-details.js - Handle domain details page

let domainId = null;
let domainData = null;

document.addEventListener('DOMContentLoaded', async () => {
    // Get domain ID from URL parameter
    const urlParams = new URLSearchParams(window.location.search);
    domainId = urlParams.get('id');

    if (!domainId) {
        showError('No domain ID specified in URL');
        return;
    }

    await loadDomainDetails();
});

async function loadDomainDetails() {
    const loadingSection = document.getElementById('loadingSection');
    const errorSection = document.getElementById('errorSection');
    const domainContent = document.getElementById('domainContent');

    try {
        // Show loading
        loadingSection.classList.remove('d-none');
        errorSection.classList.add('d-none');
        domainContent.classList.add('d-none');

        console.log(`Fetching domain details for ID: ${domainId}`);

        // Fetch domain from API: GET /api/v1/RegisteredDomains/{id}
        const authToken = localStorage.getItem('authToken');
        const headers = {
            'Content-Type': 'application/json'
        };

        if (authToken && !authToken.startsWith('demo-token-')) {
            headers['Authorization'] = `Bearer ${authToken}`;
        }

        const response = await fetch(`https://localhost:7201/api/v1/RegisteredDomains/${domainId}`, {
            method: 'GET',
            headers: headers,
            credentials: 'include'
        });

        console.log('Domain API Response Status:', response.status);

        if (!response.ok) {
            if (response.status === 404) {
                throw new Error('Domain not found');
            }
            const errorText = await response.text();
            console.error('Domain API Error Response:', errorText);
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        domainData = await response.json();
        console.log('Domain loaded from API:', domainData);

        // Hide loading
        loadingSection.classList.add('d-none');

        // Display domain details
        displayDomainDetails(domainData);

        // Load additional related data
        await loadNameServers(domainId);
        await loadDnsRecords(domainId);
        await loadDomainContacts(domainId);

        domainContent.classList.remove('d-none');

    } catch (error) {
        console.error('Error loading domain details:', error);
        loadingSection.classList.add('d-none');

        // Show error for real API errors
        showError(error.message || 'Failed to load domain details. Please try again.');
    }
}

function displayDomainDetails(domain) {
    // Get domain name (API returns 'name' not 'domainName')
    const domainName = domain.name || domain.domainName || 'Unknown';

    // Update page title and header
    document.title = `${domainName} - Domain Details - DR Admin Demo`;
    document.getElementById('breadcrumbDomain').textContent = domainName;
    document.getElementById('domainNameTitle').textContent = domainName;

    // Basic Information
    document.getElementById('domainName').textContent = domainName;
    document.getElementById('domainId').textContent = domain.id || '-';

    // Status badge
    const statusElement = document.getElementById('domainStatus');
    const statusBadge = getStatusBadge(domain.status || 'Unknown');
    statusElement.innerHTML = statusBadge;

    // Registrar
    let registrarName = '-';
    if (domain.registrar?.name) {
        registrarName = domain.registrar.name;
    } else if (domain.registrarName) {
        registrarName = domain.registrarName;
    } else if (typeof domain.registrar === 'string') {
        registrarName = domain.registrar;
    } else if (domain.registrarId) {
        registrarName = `Registrar #${domain.registrarId}`;
    }
    document.getElementById('domainRegistrar').textContent = registrarName;

    // Auto Renew
    const autoRenewElement = document.getElementById('domainAutoRenew');
    autoRenewElement.innerHTML = domain.autoRenew 
        ? '<span class="badge bg-success"><i class="bi bi-check-circle"></i> Enabled</span>'
        : '<span class="badge bg-secondary"><i class="bi bi-x-circle"></i> Disabled</span>';

    // Privacy Protection
    const privacyElement = document.getElementById('domainPrivacyProtection');
    privacyElement.innerHTML = domain.privacyProtection 
        ? '<span class="badge bg-success"><i class="bi bi-shield-check"></i> Enabled</span>'
        : '<span class="badge bg-secondary"><i class="bi bi-shield-x"></i> Disabled</span>';

    // Dates
    const registrationDate = domain.registrationDate || domain.registeredDate || domain.createdAt;
    const expirationDate = domain.expirationDate || domain.expiryDate || domain.expiry;

    document.getElementById('domainRegistrationDate').textContent = formatDate(registrationDate);
    document.getElementById('domainExpirationDate').textContent = formatDate(expirationDate);

    // Calculate days until expiry
    if (expirationDate) {
        const daysUntilExpiry = calculateDaysUntilExpiry(expirationDate);
        const expiryElement = document.getElementById('domainDaysUntilExpiry');
        
        if (daysUntilExpiry < 0) {
            expiryElement.innerHTML = `<span class="badge bg-danger">Expired ${Math.abs(daysUntilExpiry)} days ago</span>`;
        } else if (daysUntilExpiry <= 30) {
            expiryElement.innerHTML = `<span class="badge bg-warning text-dark">${daysUntilExpiry} days</span>`;
        } else {
            expiryElement.innerHTML = `<span class="badge bg-success">${daysUntilExpiry} days</span>`;
        }
    } else {
        document.getElementById('domainDaysUntilExpiry').textContent = '-';
    }

    // Prices
    const registrationPrice = domain.registrationPrice 
        ? `$${parseFloat(domain.registrationPrice).toFixed(2)}`
        : '-';
    const renewalPrice = domain.renewalPrice 
        ? `$${parseFloat(domain.renewalPrice).toFixed(2)}`
        : '-';

    document.getElementById('domainRegistrationPrice').textContent = registrationPrice;
    document.getElementById('domainRenewalPrice').textContent = renewalPrice;

    // Notes
    if (domain.notes && domain.notes.trim() !== '') {
        document.getElementById('domainNotes').textContent = domain.notes;
    } else {
        document.getElementById('domainNotes').textContent = 'No notes available';
    }

    // Customer Information
    document.getElementById('customerId').textContent = domain.customerId || '-';

    if (domain.customer) {
        const customer = domain.customer;
        
        // Customer name
        let customerName = '-';
        if (customer.organization) {
            customerName = customer.organization;
        } else if (customer.firstName || customer.lastName) {
            customerName = `${customer.firstName || ''} ${customer.lastName || ''}`.trim();
        } else if (customer.name) {
            customerName = customer.name;
        }
        document.getElementById('customerName').textContent = customerName;

        // Customer email
        document.getElementById('customerEmail').innerHTML = customer.email 
            ? `<a href="mailto:${customer.email}">${customer.email}</a>`
            : '-';

        // Customer phone
        document.getElementById('customerPhone').innerHTML = customer.phone 
            ? `<a href="tel:${customer.phone}">${customer.phone}</a>`
            : '-';

        // Customer organization
        document.getElementById('customerOrganization').textContent = customer.organization || '-';
    } else {
        document.getElementById('customerName').textContent = '-';
        document.getElementById('customerEmail').textContent = '-';
        document.getElementById('customerPhone').textContent = '-';
        document.getElementById('customerOrganization').textContent = '-';
    }

    // Service Information
    document.getElementById('serviceId').textContent = domain.serviceId || '-';

    // Audit Information
    document.getElementById('domainCreatedAt').textContent = formatDateTime(domain.createdAt);
    document.getElementById('domainUpdatedAt').textContent = formatDateTime(domain.updatedAt);
}

async function loadNameServers(domainId) {
    const loadingDiv = document.getElementById('nameServersLoading');
    const contentDiv = document.getElementById('nameServersContent');
    const emptyDiv = document.getElementById('nameServersEmpty');

    try {
        loadingDiv.classList.remove('d-none');
        contentDiv.classList.add('d-none');
        emptyDiv.classList.add('d-none');

        console.log(`Fetching name servers for domain ID: ${domainId}`);

        const authToken = localStorage.getItem('authToken');
        const headers = {
            'Content-Type': 'application/json'
        };

        if (authToken && !authToken.startsWith('demo-token-')) {
            headers['Authorization'] = `Bearer ${authToken}`;
        }

        const response = await fetch(`https://localhost:7201/api/v1/NameServers/domain/${domainId}`, {
            method: 'GET',
            headers: headers,
            credentials: 'include'
        });

        if (response.ok) {
            const nameServers = await response.json();
            console.log('Name servers loaded:', nameServers);

            loadingDiv.classList.add('d-none');

            if (Array.isArray(nameServers) && nameServers.length > 0) {
                displayNameServers(nameServers);
                contentDiv.classList.remove('d-none');
            } else {
                emptyDiv.classList.remove('d-none');
            }
        } else {
            console.warn('Failed to load name servers, hiding section');
            loadingDiv.classList.add('d-none');
            emptyDiv.classList.remove('d-none');
        }

    } catch (error) {
        console.error('Error loading name servers:', error);
        loadingDiv.classList.add('d-none');
        emptyDiv.classList.remove('d-none');
    }
}

function displayNameServers(nameServers) {
    const tableBody = document.getElementById('nameServersTableBody');
    tableBody.innerHTML = '';

    // Sort by sort order
    nameServers.sort((a, b) => (a.sortOrder || 0) - (b.sortOrder || 0));

    nameServers.forEach(ns => {
        const row = document.createElement('tr');
        
        const isPrimaryBadge = ns.isPrimary 
            ? '<span class="badge bg-primary">Primary</span>'
            : '<span class="badge bg-secondary">Secondary</span>';

        row.innerHTML = `
            <td><strong>${ns.hostname || '-'}</strong></td>
            <td>${ns.ipAddress || '-'}</td>
            <td>${isPrimaryBadge}</td>
            <td>${ns.sortOrder || '-'}</td>
        `;

        tableBody.appendChild(row);
    });
}

async function loadDnsRecords(domainId) {
    const loadingDiv = document.getElementById('dnsRecordsLoading');
    const contentDiv = document.getElementById('dnsRecordsContent');
    const emptyDiv = document.getElementById('dnsRecordsEmpty');

    try {
        loadingDiv.classList.remove('d-none');
        contentDiv.classList.add('d-none');
        emptyDiv.classList.add('d-none');

        console.log(`Fetching DNS records for domain ID: ${domainId}`);

        const authToken = localStorage.getItem('authToken');
        const headers = {
            'Content-Type': 'application/json'
        };

        if (authToken && !authToken.startsWith('demo-token-')) {
            headers['Authorization'] = `Bearer ${authToken}`;
        }

        const response = await fetch(`https://localhost:7201/api/v1/DnsRecords/domain/${domainId}`, {
            method: 'GET',
            headers: headers,
            credentials: 'include'
        });

        if (response.ok) {
            const dnsRecords = await response.json();
            console.log('DNS records loaded:', dnsRecords);

            loadingDiv.classList.add('d-none');

            if (Array.isArray(dnsRecords) && dnsRecords.length > 0) {
                displayDnsRecords(dnsRecords);
                contentDiv.classList.remove('d-none');
            } else {
                emptyDiv.classList.remove('d-none');
            }
        } else {
            console.warn('Failed to load DNS records, hiding section');
            loadingDiv.classList.add('d-none');
            emptyDiv.classList.remove('d-none');
        }

    } catch (error) {
        console.error('Error loading DNS records:', error);
        loadingDiv.classList.add('d-none');
        emptyDiv.classList.remove('d-none');
    }
}

function displayDnsRecords(dnsRecords) {
    const tableBody = document.getElementById('dnsRecordsTableBody');
    tableBody.innerHTML = '';

    // Sort by type, then name
    dnsRecords.sort((a, b) => {
        const typeCompare = (a.type || '').localeCompare(b.type || '');
        if (typeCompare !== 0) return typeCompare;
        return (a.name || '').localeCompare(b.name || '');
    });

    dnsRecords.forEach(record => {
        const row = document.createElement('tr');
        
        // Get record type badge
        const typeBadge = getDnsRecordTypeBadge(record.type);

        row.innerHTML = `
            <td>${typeBadge}</td>
            <td><code>${record.name || '@'}</code></td>
            <td><code>${record.value || '-'}</code></td>
            <td>${record.ttl || '-'}</td>
            <td>${record.priority || '-'}</td>
        `;

        tableBody.appendChild(row);
    });
}

function getDnsRecordTypeBadge(type) {
    const badges = {
        'A': '<span class="badge bg-primary">A</span>',
        'AAAA': '<span class="badge bg-primary">AAAA</span>',
        'CNAME': '<span class="badge bg-info">CNAME</span>',
        'MX': '<span class="badge bg-success">MX</span>',
        'TXT': '<span class="badge bg-warning text-dark">TXT</span>',
        'NS': '<span class="badge bg-secondary">NS</span>',
        'SRV': '<span class="badge bg-dark">SRV</span>',
        'CAA': '<span class="badge bg-danger">CAA</span>'
    };

    return badges[type] || `<span class="badge bg-light text-dark">${type}</span>`;
}

async function loadDomainContacts(domainId) {
    const loadingDiv = document.getElementById('contactsLoading');
    const contentDiv = document.getElementById('contactsContent');
    const emptyDiv = document.getElementById('contactsEmpty');

    try {
        loadingDiv.classList.remove('d-none');
        contentDiv.classList.add('d-none');
        emptyDiv.classList.add('d-none');

        console.log(`Fetching domain contacts for domain ID: ${domainId}`);

        const authToken = localStorage.getItem('authToken');
        const headers = {
            'Content-Type': 'application/json'
        };

        if (authToken && !authToken.startsWith('demo-token-')) {
            headers['Authorization'] = `Bearer ${authToken}`;
        }

        const response = await fetch(`https://localhost:7201/api/v1/DomainContacts/domain/${domainId}`, {
            method: 'GET',
            headers: headers,
            credentials: 'include'
        });

        if (response.ok) {
            const contacts = await response.json();
            console.log('Domain contacts loaded:', contacts);

            loadingDiv.classList.add('d-none');

            if (Array.isArray(contacts) && contacts.length > 0) {
                displayDomainContacts(contacts);
                contentDiv.classList.remove('d-none');
            } else {
                emptyDiv.classList.remove('d-none');
            }
        } else {
            console.warn('Failed to load domain contacts, hiding section');
            loadingDiv.classList.add('d-none');
            emptyDiv.classList.remove('d-none');
        }

    } catch (error) {
        console.error('Error loading domain contacts:', error);
        loadingDiv.classList.add('d-none');
        emptyDiv.classList.remove('d-none');
    }
}

function displayDomainContacts(contacts) {
    const accordion = document.getElementById('contactsAccordion');
    accordion.innerHTML = '';

    // Sort contacts by type (Registrant, Administrative, Technical, Billing)
    const contactOrder = { 'Registrant': 1, 'Administrative': 2, 'Technical': 3, 'Billing': 4 };
    contacts.sort((a, b) => {
        const orderA = contactOrder[a.contactType] || 999;
        const orderB = contactOrder[b.contactType] || 999;
        return orderA - orderB;
    });

    contacts.forEach((contact, index) => {
        const contactId = `contact-${index}`;
        const isFirst = index === 0;

        const accordionItem = document.createElement('div');
        accordionItem.className = 'accordion-item';

        // Get contact type icon and color
        const { icon, color } = getContactTypeInfo(contact.contactType);

        // Build full name
        const fullName = `${contact.firstName || ''} ${contact.lastName || ''}`.trim() || '-';

        accordionItem.innerHTML = `
            <h2 class="accordion-header" id="heading-${contactId}">
                <button class="accordion-button ${isFirst ? '' : 'collapsed'}" type="button" 
                        data-bs-toggle="collapse" data-bs-target="#collapse-${contactId}" 
                        aria-expanded="${isFirst}" aria-controls="collapse-${contactId}">
                    <i class="bi ${icon} ${color} me-2"></i>
                    <strong>${contact.contactType}</strong> - ${fullName}
                </button>
            </h2>
            <div id="collapse-${contactId}" class="accordion-collapse collapse ${isFirst ? 'show' : ''}" 
                 aria-labelledby="heading-${contactId}" data-bs-parent="#contactsAccordion">
                <div class="accordion-body">
                    <div class="row">
                        <div class="col-md-6">
                            <table class="table table-sm table-borderless">
                                <tr>
                                    <th width="40%">First Name:</th>
                                    <td>${contact.firstName || '-'}</td>
                                </tr>
                                <tr>
                                    <th>Last Name:</th>
                                    <td>${contact.lastName || '-'}</td>
                                </tr>
                                <tr>
                                    <th>Organization:</th>
                                    <td>${contact.organization || '-'}</td>
                                </tr>
                                <tr>
                                    <th>Email:</th>
                                    <td>${contact.email ? `<a href="mailto:${contact.email}">${contact.email}</a>` : '-'}</td>
                                </tr>
                                <tr>
                                    <th>Phone:</th>
                                    <td>${contact.phone ? `<a href="tel:${contact.phone}">${contact.phone}</a>` : '-'}</td>
                                </tr>
                                <tr>
                                    <th>Fax:</th>
                                    <td>${contact.fax || '-'}</td>
                                </tr>
                            </table>
                        </div>
                        <div class="col-md-6">
                            <table class="table table-sm table-borderless">
                                <tr>
                                    <th width="40%">Address Line 1:</th>
                                    <td>${contact.addressLine1 || '-'}</td>
                                </tr>
                                <tr>
                                    <th>Address Line 2:</th>
                                    <td>${contact.addressLine2 || '-'}</td>
                                </tr>
                                <tr>
                                    <th>City:</th>
                                    <td>${contact.city || '-'}</td>
                                </tr>
                                <tr>
                                    <th>State/Province:</th>
                                    <td>${contact.stateProvince || '-'}</td>
                                </tr>
                                <tr>
                                    <th>Postal Code:</th>
                                    <td>${contact.postalCode || '-'}</td>
                                </tr>
                                <tr>
                                    <th>Country:</th>
                                    <td>${contact.country || '-'}</td>
                                </tr>
                            </table>
                        </div>
                    </div>
                </div>
            </div>
        `;

        accordion.appendChild(accordionItem);
    });
}

function getContactTypeInfo(contactType) {
    const typeMap = {
        'Registrant': { icon: 'bi-person-badge', color: 'text-primary' },
        'Administrative': { icon: 'bi-person-gear', color: 'text-success' },
        'Technical': { icon: 'bi-person-workspace', color: 'text-info' },
        'Billing': { icon: 'bi-credit-card', color: 'text-warning' }
    };

    return typeMap[contactType] || { icon: 'bi-person', color: 'text-secondary' };
}

function getStatusBadge(status) {
    const statusMap = {
        'Active': '<span class="badge bg-success">Active</span>',
        'Expired': '<span class="badge bg-danger">Expired</span>',
        'Expiring Soon': '<span class="badge bg-warning text-dark">Expiring Soon</span>',
        'New': '<span class="badge bg-info">New</span>',
        'Pending': '<span class="badge bg-warning text-dark">Pending</span>',
        'Suspended': '<span class="badge bg-danger">Suspended</span>',
        'Cancelled': '<span class="badge bg-secondary">Cancelled</span>'
    };

    return statusMap[status] || `<span class="badge bg-secondary">${status}</span>`;
}

function formatDate(dateString) {
    if (!dateString || dateString === 'N/A') return '-';
    
    try {
        const date = new Date(dateString);
        if (isNaN(date.getTime())) return '-';
        
        return date.toLocaleDateString('en-US', {
            year: 'numeric',
            month: 'short',
            day: 'numeric'
        });
    } catch (error) {
        return '-';
    }
}

function formatDateTime(dateString) {
    if (!dateString || dateString === 'N/A') return '-';
    
    try {
        const date = new Date(dateString);
        if (isNaN(date.getTime())) return '-';
        
        return date.toLocaleString('en-US', {
            year: 'numeric',
            month: 'short',
            day: 'numeric',
            hour: '2-digit',
            minute: '2-digit'
        });
    } catch (error) {
        return '-';
    }
}

function calculateDaysUntilExpiry(expiryDate) {
    if (!expiryDate) return null;
    
    try {
        const expiry = new Date(expiryDate);
        const now = new Date();
        const diffTime = expiry - now;
        const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
        return diffDays;
    } catch (error) {
        return null;
    }
}

function showError(message) {
    const errorSection = document.getElementById('errorSection');
    const errorMessage = document.getElementById('errorMessage');
    
    errorMessage.textContent = message;
    errorSection.classList.remove('d-none');
}

// domains-register.js - Handle domain registration page

document.addEventListener('DOMContentLoaded', async () => {
    await loadRegistrars();
    await loadCustomers();
    setupEventListeners();
});

function setupEventListeners() {
    const searchForm = document.getElementById('domainSearchForm');
    const registrationForm = document.getElementById('domainRegistrationForm');
    const cancelBtn = document.getElementById('cancelRegistrationBtn');

    if (searchForm) {
        searchForm.addEventListener('submit', async (e) => {
            e.preventDefault();
            await checkDomainAvailability();
        });
    }

    if (registrationForm) {
        registrationForm.addEventListener('submit', async (e) => {
            e.preventDefault();
            await registerDomain();
        });
    }

    if (cancelBtn) {
        cancelBtn.addEventListener('click', () => {
            hideRegistrationForm();
        });
    }
}

async function loadRegistrars() {
    const registrarSelect = document.getElementById('registrarSelect');
    const registrarSearchSelect = document.getElementById('registrarSearchSelect');

    try {
        // Show loading state
        registrarSearchSelect.innerHTML = '<option value="">Loading registrars...</option>';
        registrarSearchSelect.disabled = true;

        console.log('Fetching registrars from API...');

        // Fetch registrars from API
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

        // Populate both dropdowns
        registrarSearchSelect.innerHTML = '<option value="">-- Select Registrar --</option>';

        if (Array.isArray(registrars) && registrars.length > 0) {
            console.log(`Populating dropdowns with ${registrars.length} registrars`);
            registrars.forEach(registrar => {
                // For search select
                const searchOption = document.createElement('option');
                searchOption.value = registrar.id;
                searchOption.textContent = `${registrar.name} (${registrar.code})`;
                registrarSearchSelect.appendChild(searchOption);

                // For registration select
                const option = document.createElement('option');
                option.value = registrar.id;
                option.dataset.code = registrar.code;
                option.textContent = `${registrar.name} (${registrar.code})`;
                registrarSelect.appendChild(option);
            });
        } else {
            console.warn('No registrars returned from API');
            registrarSearchSelect.innerHTML = '<option value="">No registrars available</option>';
        }

        registrarSearchSelect.disabled = false;

    } catch (error) {
        console.error('Error loading registrars:', error);
        registrarSearchSelect.innerHTML = '<option value="">-- Select Registrar --</option>';
        registrarSearchSelect.disabled = false;

        // Show error message to user
        const searchResult = document.getElementById('searchResult');
        if (searchResult) {
            searchResult.innerHTML = `
                <div class="alert alert-danger" role="alert">
                    <i class="bi bi-exclamation-triangle"></i> Error loading registrars: ${error.message}. Please refresh the page.
                </div>
            `;
        }
    }
}

async function loadCustomers() {
    const customerSelect = document.getElementById('customerSelect');

    const authToken = localStorage.getItem('authToken');
    if (!authToken) {
        customerSelect.innerHTML = '<option value="">-- Login to load customers --</option>';
        customerSelect.disabled = true;
        return;
    }

    try {
        customerSelect.innerHTML = '<option value="">Loading customers...</option>';
        customerSelect.disabled = true;

        const response = await window.CustomerAPI.getCustomers();

        customerSelect.innerHTML = '<option value="">-- Select Customer --</option>';

        if (response.success) {
            const customers = Array.isArray(response.data) ? response.data : (response.data.items || []);

            if (customers.length > 0) {
                customers.forEach(customer => {
                    const option = document.createElement('option');
                    option.value = customer.id;
                    option.textContent = `${customer.name} (${customer.email})`;
                    customerSelect.appendChild(option);
                });
            } else {
                customerSelect.innerHTML = '<option value="">No customers available</option>';
            }
        } else {
            console.error('Failed to load customers:', response.message);
            customerSelect.innerHTML = '<option value="">Error loading customers</option>';
        }

        customerSelect.disabled = false;

    } catch (error) {
        console.error('Error loading customers:', error);
        customerSelect.innerHTML = '<option value="">-- Select Customer --</option>';
        customerSelect.disabled = false;
    }
}

async function checkDomainAvailability() {
    const domainName = document.getElementById('domainName').value.trim();
    const searchResult = document.getElementById('searchResult');

    if (!domainName) {
        searchResult.innerHTML = `
            <div class="alert alert-warning" role="alert">
                <i class="bi bi-exclamation-triangle"></i> Please enter a domain name.
            </div>
        `;
        return;
    }

    const registrarSearchSelect = document.getElementById('registrarSearchSelect');
    if (!registrarSearchSelect.value) {
        searchResult.innerHTML = `
            <div class="alert alert-warning" role="alert">
                <i class="bi bi-exclamation-triangle"></i> Please select a registrar first.
            </div>
        `;
        return;
    }

    try {
        // Show loading
        searchResult.innerHTML = `
            <div class="text-center py-3">
                <div class="spinner-border text-primary" role="status">
                    <span class="visually-hidden">Checking...</span>
                </div>
                <p class="mt-2">Checking domain availability...</p>
            </div>
        `;

        const registrarId = registrarSearchSelect.value;

        // Prepare authentication headers
        const authToken = localStorage.getItem('authToken');
        const headers = {
            'Content-Type': 'application/json'
        };

        // Only add Authorization header if we have a token
        if (authToken && !authToken.startsWith('demo-token-')) {
            headers['Authorization'] = `Bearer ${authToken}`;
        }

        const response = await fetch(`https://localhost:7201/api/v1/Registrars/${registrarId}/isavailable/${encodeURIComponent(domainName)}`, {
            method: 'GET',
            headers: headers,
            credentials: 'include'
        });

        if (!response.ok) {
            const errorText = await response.text();
            console.error('API Error Response:', errorText);
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        const data = await response.json();
        console.log('Domain availability check response:', data);

        // Check if TLD is supported first
        if (data.isTldSupported === false) {
            // Extract TLD from domain name
            const parts = domainName.split('.');
            const tld = parts.length > 1 ? parts[parts.length - 1] : 'unknown';

            searchResult.innerHTML = `
                <div class="alert alert-warning" role="alert">
                    <h5 class="alert-heading"><i class="bi bi-exclamation-triangle"></i> TLD Not Supported</h5>
                    <p>The <strong>.${tld}</strong> top-level domain is not supported by the selected registrar.</p>
                    <p class="mb-0">${data.message || 'Please select a different registrar or try a different domain extension.'}</p>
                </div>
            `;
            return;
        }

        // Correctly parse the boolean value
        const isAvailable = data.isAvailable === true;

        if (isAvailable) {
            searchResult.innerHTML = `
                <div class="alert alert-success" role="alert">
                    <h5 class="alert-heading"><i class="bi bi-check-circle"></i> Domain Available!</h5>
                    <p class="mb-0"><strong>${domainName}</strong> is available for registration.</p>
                    <hr>
                    <p class="mb-0">
                        <button type="button" class="btn btn-success" id="proceedToRegisterBtn">
                            <i class="bi bi-arrow-right-circle"></i> Proceed to Register
                        </button>
                    </p>
                </div>
            `;

            // Add event listener to proceed button
            document.getElementById('proceedToRegisterBtn').addEventListener('click', () => {
                showRegistrationForm(domainName);
            });

        } else {
            // Suggest alternatives
            const alternatives = generateAlternatives(domainName);
            searchResult.innerHTML = `
                <div class="alert alert-danger" role="alert">
                    <h5 class="alert-heading"><i class="bi bi-x-circle"></i> Domain Not Available</h5>
                    <p><strong>${domainName}</strong> is already registered.</p>
                    <hr>
                    <p class="mb-2">Try these alternatives:</p>
                    <ul class="mb-0">
                        ${alternatives.map(alt => `<li>${alt}</li>`).join('')}
                    </ul>
                </div>
            `;
        }

    } catch (error) {
        console.error('Error checking domain availability:', error);
        searchResult.innerHTML = `
            <div class="alert alert-danger" role="alert">
                <i class="bi bi-exclamation-triangle"></i> Error checking domain availability. Please try again.
            </div>
        `;
    }
}

function generateAlternatives(domainName) {
    const parts = domainName.split('.');
    const name = parts[0];
    const tld = parts[1] || 'com';

    const alternatives = [];
    const altTlds = ['net', 'org', 'io', 'tech', 'dev'];
    
    // Alternative TLDs
    altTlds.forEach(altTld => {
        if (altTld !== tld) {
            alternatives.push(`${name}.${altTld}`);
        }
    });

    // Alternative names
    alternatives.push(`get${name}.${tld}`);
    alternatives.push(`my${name}.${tld}`);

    return alternatives.slice(0, 5);
}

function showRegistrationForm(domainName) {
    const registrationFormCard = document.getElementById('registrationFormCard');
    const selectedDomain = document.getElementById('selectedDomain');

    selectedDomain.value = domainName;

    // Auto-select the same registrar used for the availability search
    const registrarSearchSelect = document.getElementById('registrarSearchSelect');
    const registrarSelect = document.getElementById('registrarSelect');
    if (registrarSearchSelect.value) {
        registrarSelect.value = registrarSearchSelect.value;
    }

    registrationFormCard.classList.remove('d-none');

    // Scroll to form
    registrationFormCard.scrollIntoView({ behavior: 'smooth', block: 'start' });
}

function hideRegistrationForm() {
    const registrationFormCard = document.getElementById('registrationFormCard');
    const registrationForm = document.getElementById('domainRegistrationForm');
    
    registrationFormCard.classList.add('d-none');
    registrationForm.reset();
}

async function registerDomain() {
    const registrationResult = document.getElementById('registrationResult');

    try {
        // Get form values
        const domainName = document.getElementById('selectedDomain').value;
        const years = parseInt(document.getElementById('registrationYears').value, 10);
        const registrarSelect = document.getElementById('registrarSelect');
        const customerSelect = document.getElementById('customerSelect');
        const autoRenew = document.getElementById('autoRenew').checked;
        const privacyProtection = document.getElementById('privacyProtection').checked;

        // Validate registrar and customer before proceeding
        if (!registrarSelect.value) {
            registrationResult.innerHTML = `
                <div class="alert alert-warning" role="alert">
                    <i class="bi bi-exclamation-triangle"></i> Please select a registrar.
                </div>
            `;
            registrarSelect.focus();
            return;
        }

        if (!customerSelect.value) {
            registrationResult.innerHTML = `
                <div class="alert alert-warning" role="alert">
                    <i class="bi bi-exclamation-triangle"></i> Please select a customer.
                </div>
            `;
            customerSelect.focus();
            return;
        }

        const registrarId = parseInt(registrarSelect.value, 10);
        const registrarCode = registrarSelect.options[registrarSelect.selectedIndex].dataset.code;
        const customerId = parseInt(customerSelect.value, 10);

        // Contact information
        const contactFirstName = document.getElementById('contactFirstName').value.trim();
        const contactLastName = document.getElementById('contactLastName').value.trim();
        const contactEmail = document.getElementById('contactEmail').value.trim();
        const contactPhone = document.getElementById('contactPhone').value.trim();
        const contactOrganization = document.getElementById('contactOrganization').value.trim();
        const contactAddress = document.getElementById('contactAddress').value.trim();
        const contactCity = document.getElementById('contactCity').value.trim();
        const contactState = document.getElementById('contactState').value.trim();
        const contactPostalCode = document.getElementById('contactPostalCode').value.trim();
        const contactCountry = document.getElementById('contactCountry').value.trim();

        // Show loading
        registrationResult.innerHTML = `
            <div class="alert alert-info" role="alert">
                <div class="d-flex align-items-center">
                    <div class="spinner-border spinner-border-sm me-2" role="status">
                        <span class="visually-hidden">Processing...</span>
                    </div>
                    <div>Processing domain registration...</div>
                </div>
            </div>
        `;

        // Prepare authentication headers
        const authToken = localStorage.getItem('authToken');
        const headers = {
            'Content-Type': 'application/json'
        };
        if (authToken && !authToken.startsWith('demo-token-')) {
            headers['Authorization'] = `Bearer ${authToken}`;
        }

        const fetchOptions = { headers, credentials: 'include' };

        // Step 1: Check if domain already exists in the system
        const existingDomainResponse = await fetch(`https://localhost:7201/api/v1/RegisteredDomains/name/${encodeURIComponent(domainName)}`, {
            method: 'GET',
            ...fetchOptions
        });

        if (existingDomainResponse.ok) {
            throw new Error(`Domain "${domainName}" is already registered in the system.`);
        }

        // Step 2: Create a Service record for the domain registration
        // Find the DOMAIN service type
        const serviceTypesResponse = await fetch('https://localhost:7201/api/v1/ServiceTypes', {
            method: 'GET',
            ...fetchOptions
        });

        if (!serviceTypesResponse.ok) {
            const errorText = await serviceTypesResponse.text();
            throw new Error(`Failed to load service types: ${errorText}`);
        }

        const serviceTypes = await serviceTypesResponse.json();
        const domainServiceType = serviceTypes.find(st => st.name === 'DOMAIN' || st.name === 'Domain Registration');

        if (!domainServiceType) {
            throw new Error('Domain service type not found. Please run system initialization first.');
        }

        const createServicePayload = {
            name: domainName,
            description: `Domain registration for ${domainName}`,
            serviceTypeId: domainServiceType.id
        };

        const serviceResponse = await fetch('https://localhost:7201/api/v1/Services', {
            method: 'POST',
            ...fetchOptions,
            body: JSON.stringify(createServicePayload)
        });

        if (!serviceResponse.ok) {
            const errorText = await serviceResponse.text();
            throw new Error(`Failed to create service: ${errorText}`);
        }

        const createdService = await serviceResponse.json();

        // Step 3: Create the RegisteredDomain record
        const now = new Date();
        const expiration = new Date(now);
        expiration.setFullYear(expiration.getFullYear() + years);

        const createDomainPayload = {
            customerId: customerId,
            serviceId: createdService.id,
            name: domainName,
            providerId: registrarId,
            status: 'Pending',
            registrationDate: now.toISOString(),
            expirationDate: expiration.toISOString()
        };

        const domainResponse = await fetch('https://localhost:7201/api/v1/RegisteredDomains', {
            method: 'POST',
            ...fetchOptions,
            body: JSON.stringify(createDomainPayload)
        });

        if (!domainResponse.ok) {
            const errorText = await domainResponse.text();
            throw new Error(`Failed to create domain record: ${errorText}`);
        }

        const createdDomain = await domainResponse.json();
        const registeredDomainId = createdDomain.id;

        // Step 4: Create domain contacts for all roles using the same contact info
        const contactRoles = ['Registrant', 'Administrative', 'Technical', 'Billing'];

        for (const role of contactRoles) {
            const contactPayload = {
                contactType: role,
                firstName: contactFirstName,
                lastName: contactLastName,
                organization: contactOrganization || null,
                email: contactEmail,
                phone: contactPhone,
                address1: contactAddress,
                city: contactCity,
                state: contactState || null,
                postalCode: contactPostalCode,
                countryCode: contactCountry,
                isActive: true,
                domainId: registeredDomainId,
                isPrivacyProtected: privacyProtection
            };

            const contactResponse = await fetch('https://localhost:7201/api/v1/DomainContacts', {
                method: 'POST',
                ...fetchOptions,
                body: JSON.stringify(contactPayload)
            });

            if (!contactResponse.ok) {
                const errorText = await contactResponse.text();
                throw new Error(`Failed to create ${role} contact: ${errorText}`);
            }
        }

        // Step 5: Register the domain with the registrar
        const registerResponse = await fetch(`https://localhost:7201/api/v1/DomainManager/registrar/${encodeURIComponent(registrarCode)}/domain/${registeredDomainId}`, {
            method: 'POST',
            ...fetchOptions
        });

        if (!registerResponse.ok) {
            const errorText = await registerResponse.text();
            throw new Error(`Domain registration failed: ${errorText}`);
        }

        const registerResult = await registerResponse.json();

        if (registerResult.success) {
            registrationResult.innerHTML = `
                <div class="alert alert-success" role="alert">
                    <h5 class="alert-heading"><i class="bi bi-check-circle"></i> Domain Registered Successfully!</h5>
                    <p><strong>${domainName}</strong> has been registered for ${years} year(s).</p>
                    <hr>
                    <p class="mb-0">
                        <strong>Customer:</strong> ${document.getElementById('customerSelect').options[document.getElementById('customerSelect').selectedIndex].text}<br>
                        <strong>Registrar:</strong> ${registrarSelect.options[registrarSelect.selectedIndex].text}<br>
                        <strong>Registration Period:</strong> ${years} year(s)
                    </p>
                    <hr>
                    <p class="mb-0">
                        <a href="/domains-all.html" class="btn btn-primary">
                            <i class="bi bi-list-ul"></i> View All Domains
                        </a>
                        <button type="button" class="btn btn-success" onclick="window.location.reload()">
                            <i class="bi bi-plus-circle"></i> Register Another
                        </button>
                    </p>
                </div>
            `;

            // Hide registration form
            hideRegistrationForm();

            // Scroll to result
            registrationResult.scrollIntoView({ behavior: 'smooth', block: 'start' });
        } else {
            registrationResult.innerHTML = `
                <div class="alert alert-danger" role="alert">
                    <h5 class="alert-heading"><i class="bi bi-x-circle"></i> Registration Failed</h5>
                    <p>There was an error registering <strong>${domainName}</strong>.</p>
                    <p class="mb-0">${registerResult.message || 'Please verify your information and try again, or contact support if the problem persists.'}</p>
                </div>
            `;
        }

    } catch (error) {
        console.error('Error registering domain:', error);
        registrationResult.innerHTML = `
            <div class="alert alert-danger" role="alert">
                <h5 class="alert-heading"><i class="bi bi-exclamation-triangle"></i> Error</h5>
                <p class="mb-0">${error.message || 'An unexpected error occurred. Please try again later.'}</p>
            </div>
        `;
    }
}

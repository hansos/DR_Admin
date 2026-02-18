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
    try {
        const customerSelect = document.getElementById('customerSelect');
        
        // For demo purposes, create sample customers
        // In production, fetch from: GET /api/v1/Customers
        const sampleCustomers = [
            { id: 1, name: 'John Doe', email: 'john@example.com' },
            { id: 2, name: 'Jane Smith', email: 'jane@example.com' },
            { id: 3, name: 'Acme Corporation', email: 'contact@acme.com' },
            { id: 4, name: 'Tech Solutions Inc', email: 'info@techsolutions.com' },
            { id: 5, name: 'Global Enterprises', email: 'admin@global.com' }
        ];

        sampleCustomers.forEach(customer => {
            const option = document.createElement('option');
            option.value = customer.id;
            option.textContent = `${customer.name} (${customer.email})`;
            customerSelect.appendChild(option);
        });

    } catch (error) {
        console.error('Error loading customers:', error);
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
        const years = document.getElementById('registrationYears').value;
        const registrarCode = document.getElementById('registrarSelect').value;
        const customerId = document.getElementById('customerSelect').value;

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

        // In production, call: POST /api/v1/DomainManager/registrar/{registrarCode}/domain/{registeredDomainId}
        // For demo purposes, simulate API call
        await new Promise(resolve => setTimeout(resolve, 2000));

        // Simulate success
        const success = Math.random() > 0.2; // 80% success rate

        if (success) {
            registrationResult.innerHTML = `
                <div class="alert alert-success" role="alert">
                    <h5 class="alert-heading"><i class="bi bi-check-circle"></i> Domain Registered Successfully!</h5>
                    <p><strong>${domainName}</strong> has been registered for ${years} year(s).</p>
                    <hr>
                    <p class="mb-0">
                        <strong>Customer:</strong> ${document.getElementById('customerSelect').options[document.getElementById('customerSelect').selectedIndex].text}<br>
                        <strong>Registrar:</strong> ${document.getElementById('registrarSelect').options[document.getElementById('registrarSelect').selectedIndex].text}<br>
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
                    <p class="mb-0">Please verify your information and try again, or contact support if the problem persists.</p>
                </div>
            `;
        }

    } catch (error) {
        console.error('Error registering domain:', error);
        registrationResult.innerHTML = `
            <div class="alert alert-danger" role="alert">
                <h5 class="alert-heading"><i class="bi bi-exclamation-triangle"></i> Error</h5>
                <p class="mb-0">An unexpected error occurred. Please try again later.</p>
            </div>
        `;
    }
}

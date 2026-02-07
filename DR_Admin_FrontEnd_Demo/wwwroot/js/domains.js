"use strict";
document.addEventListener('DOMContentLoaded', () => {
    loadDomains();
    const searchForm = document.getElementById('domainSearchForm');
    if (searchForm) {
        searchForm.addEventListener('submit', async (e) => {
            e.preventDefault();
            await searchDomain();
        });
    }
});
async function loadDomains() {
    const loadingDiv = document.getElementById('loadingDomains');
    const tableDiv = document.getElementById('domainsTable');
    const noDomainsDiv = document.getElementById('noDomainsMessage');
    const tableBody = document.getElementById('domainsTableBody');
    if (!loadingDiv || !tableDiv || !noDomainsDiv || !tableBody)
        return;
    loadingDiv.classList.remove('d-none');
    tableDiv.classList.add('d-none');
    noDomainsDiv.classList.add('d-none');
    try {
        const response = await window.DomainAPI.getDomains();
        if (response.success && response.data && response.data.length > 0) {
            tableBody.innerHTML = response.data.map((domain) => `
                <tr>
                    <td><strong>${domain.name}</strong></td>
                    <td><span class="badge bg-${domain.status === 'Active' ? 'success' : 'warning'}">${domain.status}</span></td>
                    <td>${domain.expiryDate}</td>
                    <td>
                        <i class="bi bi-${domain.autoRenew ? 'check-circle text-success' : 'x-circle text-danger'}"></i>
                        ${domain.autoRenew ? 'Yes' : 'No'}
                    </td>
                    <td>
                        <button class="btn btn-sm btn-outline-primary" onclick="manageDomain(${domain.id})">
                            <i class="bi bi-gear"></i> Manage
                        </button>
                    </td>
                </tr>
            `).join('');
            loadingDiv.classList.add('d-none');
            tableDiv.classList.remove('d-none');
        }
        else {
            loadingDiv.classList.add('d-none');
            noDomainsDiv.classList.remove('d-none');
        }
    }
    catch (error) {
        console.error('Error loading domains:', error);
        loadingDiv.classList.add('d-none');
        noDomainsDiv.classList.remove('d-none');
    }
}
async function searchDomain() {
    const domainInput = document.getElementById('domainName');
    const resultDiv = document.getElementById('searchResult');
    if (!domainInput || !resultDiv)
        return;
    const domain = domainInput.value.trim();
    if (!domain)
        return;
    resultDiv.innerHTML = '<div class="alert alert-info"><i class="bi bi-hourglass-split"></i> Searching...</div>';
    try {
        const response = await window.DomainAPI.searchDomain(domain);
        if (response.success) {
            const data = response.data;
            if (data.available) {
                resultDiv.innerHTML = `
                    <div class="alert alert-success">
                        <h5><i class="bi bi-check-circle"></i> ${data.domain} is available!</h5>
                        <p class="mb-2">Price: $${data.price}/year</p>
                        <button class="btn btn-success" onclick="registerDomain('${data.domain}')">
                            <i class="bi bi-cart-plus"></i> Register Domain
                        </button>
                    </div>
                `;
            }
            else {
                resultDiv.innerHTML = `
                    <div class="alert alert-warning">
                        <h5><i class="bi bi-x-circle"></i> ${data.domain} is not available</h5>
                        <p class="mb-0">This domain is already registered. Try a different name.</p>
                    </div>
                `;
            }
        }
        else {
            resultDiv.innerHTML = `<div class="alert alert-danger">${response.message || 'Search failed'}</div>`;
        }
    }
    catch (error) {
        console.error('Domain search error:', error);
        resultDiv.innerHTML = '<div class="alert alert-danger">An error occurred during search</div>';
    }
}
function registerDomain(domain) {
    alert(`Domain registration for ${domain} will be implemented here.\n\nThis would typically:\n1. Add domain to cart\n2. Proceed to checkout\n3. Process payment\n4. Submit registration to DR_Admin API`);
}
function manageDomain(domainId) {
    alert(`Domain management page for domain ID ${domainId} will be implemented here.\n\nTypical features:\n- DNS management\n- Contact information\n- Renewal settings\n- Transfer domain`);
}
if (typeof window !== 'undefined') {
    window.loadDomains = loadDomains;
    window.searchDomain = searchDomain;
    window.registerDomain = registerDomain;
    window.manageDomain = manageDomain;
}
//# sourceMappingURL=domains.js.map
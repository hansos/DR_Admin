"use strict";
document.addEventListener('DOMContentLoaded', () => {
    loadHostingPlans();
    loadHostingServices();
});
async function loadHostingPlans() {
    const loadingDiv = document.getElementById('loadingPlans');
    const plansDiv = document.getElementById('hostingPlans');
    if (!loadingDiv || !plansDiv)
        return;
    loadingDiv.classList.remove('d-none');
    plansDiv.classList.add('d-none');
    try {
        const response = await window.HostingAPI.getHostingPlans();
        if (response.success && response.data && response.data.length > 0) {
            plansDiv.innerHTML = response.data.map((plan) => `
                <div class="col-md-4 mb-3">
                    <div class="card h-100 shadow-sm">
                        <div class="card-header bg-primary text-white">
                            <h5 class="mb-0">${plan.name}</h5>
                        </div>
                        <div class="card-body">
                            <h3 class="text-primary">$${plan.price}<small class="text-muted">/mo</small></h3>
                            <ul class="list-unstyled mt-3">
                                <li><i class="bi bi-check-circle text-success"></i> ${plan.diskSpace} Disk Space</li>
                                <li><i class="bi bi-check-circle text-success"></i> ${plan.bandwidth} Bandwidth</li>
                                <li><i class="bi bi-check-circle text-success"></i> Email Accounts</li>
                                <li><i class="bi bi-check-circle text-success"></i> 24/7 Support</li>
                            </ul>
                        </div>
                        <div class="card-footer">
                            <button class="btn btn-primary w-100" onclick="purchaseHosting(${plan.id}, '${plan.name}')">
                                <i class="bi bi-cart-plus"></i> Order Now
                            </button>
                        </div>
                    </div>
                </div>
            `).join('');
            loadingDiv.classList.add('d-none');
            plansDiv.classList.remove('d-none');
        }
    }
    catch (error) {
        console.error('Error loading hosting plans:', error);
        loadingDiv.innerHTML = '<div class="alert alert-danger">Failed to load hosting plans</div>';
    }
}
async function loadHostingServices() {
    const loadingDiv = document.getElementById('loadingServices');
    const tableDiv = document.getElementById('hostingServicesTable');
    const noServicesDiv = document.getElementById('noServicesMessage');
    const tableBody = document.getElementById('hostingServicesTableBody');
    if (!loadingDiv || !tableDiv || !noServicesDiv || !tableBody)
        return;
    loadingDiv.classList.remove('d-none');
    tableDiv.classList.add('d-none');
    noServicesDiv.classList.add('d-none');
    try {
        const response = await window.HostingAPI.getMyHosting();
        if (response.success && response.data && response.data.length > 0) {
            tableBody.innerHTML = response.data.map((service) => `
                <tr>
                    <td><strong>${service.plan}</strong></td>
                    <td>${service.domain}</td>
                    <td><span class="badge bg-${service.status === 'Active' ? 'success' : 'warning'}">${service.status}</span></td>
                    <td>${service.renewalDate}</td>
                    <td>
                        <button class="btn btn-sm btn-outline-primary" onclick="manageHosting(${service.id})">
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
            noServicesDiv.classList.remove('d-none');
        }
    }
    catch (error) {
        console.error('Error loading hosting services:', error);
        loadingDiv.classList.add('d-none');
        noServicesDiv.classList.remove('d-none');
    }
}
function purchaseHosting(_planId, planName) {
    alert(`Hosting purchase for ${planName} will be implemented here.\n\nThis would typically:\n1. Show domain selection\n2. Add to cart\n3. Proceed to checkout\n4. Process payment\n5. Provision hosting via DR_Admin API`);
}
function manageHosting(serviceId) {
    alert(`Hosting management for service ID ${serviceId} will be implemented here.\n\nTypical features:\n- File manager\n- Database management\n- Email accounts\n- Backups\n- Statistics`);
}
if (typeof window !== 'undefined') {
    window.loadHostingPlans = loadHostingPlans;
    window.loadHostingServices = loadHostingServices;
    window.purchaseHosting = purchaseHosting;
    window.manageHosting = manageHosting;
}
//# sourceMappingURL=hosting.js.map
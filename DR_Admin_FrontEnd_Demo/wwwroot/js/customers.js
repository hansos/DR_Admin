"use strict";
document.addEventListener('DOMContentLoaded', () => {
    loadCustomers();
});
async function loadCustomers() {
    const loadingDiv = document.getElementById('loadingCustomers');
    const tableDiv = document.getElementById('customersTable');
    const noCustomersDiv = document.getElementById('noCustomersMessage');
    const tableBody = document.getElementById('customersTableBody');
    if (!loadingDiv || !tableDiv || !noCustomersDiv || !tableBody)
        return;
    loadingDiv.classList.remove('d-none');
    tableDiv.classList.add('d-none');
    noCustomersDiv.classList.add('d-none');
    try {
        const response = await window.CustomerAPI.getCustomers();
        if (response.success && response.data && response.data.length > 0) {
            tableBody.innerHTML = response.data.map((customer) => `
                <tr>
                    <td>${customer.id}</td>
                    <td><strong>${customer.name}</strong></td>
                    <td>${customer.email}</td>
                    <td>${customer.totalOrders}</td>
                    <td><span class="badge bg-${customer.status === 'Active' ? 'success' : 'secondary'}">${customer.status}</span></td>
                    <td>${customer.joinDate}</td>
                    <td>
                        <div class="btn-group btn-group-sm" role="group">
                            <button class="btn btn-outline-primary" onclick="viewCustomer(${customer.id})">
                                <i class="bi bi-eye"></i> View
                            </button>
                            <button class="btn btn-outline-secondary" onclick="editCustomer(${customer.id})">
                                <i class="bi bi-pencil"></i> Edit
                            </button>
                        </div>
                    </td>
                </tr>
            `).join('');
            loadingDiv.classList.add('d-none');
            tableDiv.classList.remove('d-none');
        }
        else {
            loadingDiv.classList.add('d-none');
            noCustomersDiv.classList.remove('d-none');
        }
    }
    catch (error) {
        console.error('Error loading customers:', error);
        loadingDiv.classList.add('d-none');
        noCustomersDiv.classList.remove('d-none');
    }
}
function viewCustomer(customerId) {
    alert(`Customer details page for ID ${customerId} will be implemented here.\n\nTypical information:\n- Contact details\n- Order history\n- Active services\n- Payment history`);
}
function editCustomer(customerId) {
    alert(`Customer edit page for ID ${customerId} will be implemented here.\n\nTypical actions:\n- Update contact info\n- Change status\n- Add notes\n- Manage permissions`);
}
if (typeof window !== 'undefined') {
    window.loadCustomers = loadCustomers;
    window.viewCustomer = viewCustomer;
    window.editCustomer = editCustomer;
}
//# sourceMappingURL=customers.js.map
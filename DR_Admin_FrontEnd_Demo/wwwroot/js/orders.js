"use strict";
document.addEventListener('DOMContentLoaded', () => {
    loadOrders();
});
async function loadOrders() {
    const loadingDiv = document.getElementById('loadingOrders');
    const tableDiv = document.getElementById('ordersTable');
    const noOrdersDiv = document.getElementById('noOrdersMessage');
    const tableBody = document.getElementById('ordersTableBody');
    if (!loadingDiv || !tableDiv || !noOrdersDiv || !tableBody)
        return;
    loadingDiv.classList.remove('d-none');
    tableDiv.classList.add('d-none');
    noOrdersDiv.classList.add('d-none');
    try {
        const response = await window.OrderAPI.getOrders();
        if (response.success && response.data && response.data.length > 0) {
            const statusFilter = document.getElementById('statusFilter')?.value || '';
            let filteredOrders = response.data;
            if (statusFilter) {
                filteredOrders = filteredOrders.filter((order) => order.status === statusFilter);
            }
            if (filteredOrders.length > 0) {
                tableBody.innerHTML = filteredOrders.map((order) => `
                    <tr>
                        <td><strong>#${order.id}</strong></td>
                        <td>${order.date}</td>
                        <td>${order.customer}</td>
                        <td>${order.items}</td>
                        <td><strong>$${order.total}</strong></td>
                        <td>
                            <span class="badge bg-${getStatusBadgeClass(order.status)}">
                                ${order.status}
                            </span>
                        </td>
                        <td>
                            <div class="btn-group btn-group-sm" role="group">
                                <button class="btn btn-outline-primary" onclick="viewOrder(${order.id})">
                                    <i class="bi bi-eye"></i> View
                                </button>
                                <button class="btn btn-outline-secondary" onclick="downloadInvoice(${order.id})">
                                    <i class="bi bi-download"></i> Invoice
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
                noOrdersDiv.classList.remove('d-none');
            }
        }
        else {
            loadingDiv.classList.add('d-none');
            noOrdersDiv.classList.remove('d-none');
        }
    }
    catch (error) {
        console.error('Error loading orders:', error);
        loadingDiv.classList.add('d-none');
        noOrdersDiv.classList.remove('d-none');
    }
}
function getStatusBadgeClass(status) {
    switch (status) {
        case 'Completed':
            return 'success';
        case 'Pending':
            return 'warning';
        case 'Cancelled':
            return 'danger';
        default:
            return 'secondary';
    }
}
function viewOrder(orderId) {
    alert(`Order details page for order #${orderId} will be implemented here.\n\nTypical information:\n- Order items\n- Customer info\n- Payment status\n- Delivery status`);
}
function downloadInvoice(orderId) {
    alert(`Invoice download for order #${orderId} will be implemented here.\n\nThis would typically generate a PDF invoice via the DR_Admin API.`);
}
if (typeof window !== 'undefined') {
    window.loadOrders = loadOrders;
    window.viewOrder = viewOrder;
    window.downloadInvoice = downloadInvoice;
    window.getStatusBadgeClass = getStatusBadgeClass;
}
//# sourceMappingURL=orders.js.map
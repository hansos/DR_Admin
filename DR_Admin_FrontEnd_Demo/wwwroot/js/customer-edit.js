/**
 * Customer Edit Page
 */

let customerId;

document.addEventListener('DOMContentLoaded', () => {
    const params = new URLSearchParams(window.location.search);
    customerId = params.get('id');

    if (!customerId) {
        showError('No customer ID provided');
        return;
    }

    loadCustomer();

    const form = document.getElementById('customerForm');
    form.addEventListener('submit', async (e) => {
        e.preventDefault();
        await updateCustomer();
    });
});

async function loadCustomer() {
    try {
        const response = await window.CustomerAPI.getCustomer(customerId);

        if (response.success) {
            const customer = response.data;
            
            document.getElementById('customerId').value = customer.id;
            document.getElementById('name').value = customer.name || '';
            document.getElementById('email').value = customer.email || '';
            document.getElementById('phone').value = customer.phone || '';
            document.getElementById('isActive').value = customer.isActive ? 'true' : 'false';
            document.getElementById('notes').value = customer.notes || '';

            document.getElementById('loadingSpinner').classList.add('d-none');
            document.getElementById('customerFormCard').classList.remove('d-none');
        } else {
            showError(response.message || 'Failed to load customer');
            document.getElementById('loadingSpinner').classList.add('d-none');
        }
    } catch (error) {
        console.error('Error loading customer:', error);
        showError('An error occurred while loading the customer');
        document.getElementById('loadingSpinner').classList.add('d-none');
    }
}

async function updateCustomer() {
    const customerData = {
        name: document.getElementById('name').value.trim(),
        email: document.getElementById('email').value.trim(),
        phone: document.getElementById('phone').value.trim(),
        isActive: document.getElementById('isActive').value === 'true',
        notes: document.getElementById('notes').value.trim() || null
    };

    const form = document.getElementById('customerForm');
    const submitBtn = form.querySelector('button[type="submit"]');
    const originalText = submitBtn.innerHTML;
    submitBtn.disabled = true;
    submitBtn.innerHTML = '<span class="spinner-border spinner-border-sm"></span> Updating...';

    try {
        const response = await window.CustomerAPI.updateCustomer(customerId, customerData);

        if (response.success) {
            showSuccess('Customer updated successfully!');
            setTimeout(() => window.location.href = '/customers.html', 1500);
        } else {
            showError(response.message || 'Failed to update customer');
            submitBtn.disabled = false;
            submitBtn.innerHTML = originalText;
        }
    } catch (error) {
        console.error('Error updating customer:', error);
        showError('An error occurred while updating the customer');
        submitBtn.disabled = false;
        submitBtn.innerHTML = originalText;
    }
}

function showSuccess(message) {
    const alert = document.getElementById('alertSuccess');
    alert.textContent = message;
    alert.classList.remove('d-none');
    document.getElementById('alertError').classList.add('d-none');
}

function showError(message) {
    const alert = document.getElementById('alertError');
    alert.textContent = message;
    alert.classList.remove('d-none');
    document.getElementById('alertSuccess').classList.add('d-none');
}

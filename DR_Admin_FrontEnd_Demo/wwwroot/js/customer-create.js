/**
 * Customer Create Page
 */

document.addEventListener('DOMContentLoaded', () => {
    const form = document.getElementById('customerForm');
    
    form.addEventListener('submit', async (e) => {
        e.preventDefault();
        
        const customerData = {
            name: document.getElementById('name').value.trim(),
            email: document.getElementById('email').value.trim(),
            phone: document.getElementById('phone').value.trim(),
            isCompany: document.getElementById('isCompany').value === 'true',
            preferredCurrency: document.getElementById('preferredCurrency').value,
            taxId: document.getElementById('taxId').value.trim() || null,
            vatNumber: document.getElementById('vatNumber').value.trim() || null,
            notes: document.getElementById('notes').value.trim() || null
        };

        const submitBtn = form.querySelector('button[type="submit"]');
        const originalText = submitBtn.innerHTML;
        submitBtn.disabled = true;
        submitBtn.innerHTML = '<span class="spinner-border spinner-border-sm"></span> Creating...';

        try {
            const response = await window.CustomerAPI.createCustomer(customerData);

            if (response.success) {
                showSuccess('Customer created successfully!');
                setTimeout(() => window.location.href = '/customers.html', 1500);
            } else {
                showError(response.message || 'Failed to create customer');
                submitBtn.disabled = false;
                submitBtn.innerHTML = originalText;
            }
        } catch (error) {
            console.error('Error creating customer:', error);
            showError('An error occurred while creating the customer');
            submitBtn.disabled = false;
            submitBtn.innerHTML = originalText;
        }
    });
});

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

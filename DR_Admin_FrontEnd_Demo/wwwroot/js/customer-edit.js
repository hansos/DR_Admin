/**
 * Customer Edit Page with Contact Person Management
 */

let customerId;
let editingContactId = null;
let deletingContactId = null;

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

    // Contact Person event listeners
    document.getElementById('addContactPersonBtn')?.addEventListener('click', () => {
        openContactPersonModal();
    });

    document.getElementById('saveContactPersonBtn')?.addEventListener('click', async () => {
        await saveContactPerson();
    });

    document.getElementById('confirmDeleteContactBtn')?.addEventListener('click', async () => {
        await deleteContactPerson();
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
            document.getElementById('contactPersonsCard').classList.remove('d-none');

            // Load contact persons
            await loadContactPersons();
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

// ==================== Contact Person Management ====================

async function loadContactPersons() {
    try {
        document.getElementById('contactPersonsLoading').classList.remove('d-none');
        document.getElementById('contactPersonsContent').classList.add('d-none');

        const response = await window.ContactPersonAPI.getContactPersonsByCustomer(customerId);

        if (response.success) {
            const contactPersons = Array.isArray(response.data) ? response.data : [];
            renderContactPersons(contactPersons);

            document.getElementById('contactPersonsLoading').classList.add('d-none');
            document.getElementById('contactPersonsContent').classList.remove('d-none');
        } else {
            showError('Failed to load contact persons: ' + (response.message || 'Unknown error'));
            document.getElementById('contactPersonsLoading').classList.add('d-none');
        }
    } catch (error) {
        console.error('Error loading contact persons:', error);
        showError('An error occurred while loading contact persons');
        document.getElementById('contactPersonsLoading').classList.add('d-none');
    }
}

function renderContactPersons(contactPersons) {
    const tbody = document.getElementById('contactPersonsTableBody');
    const noContactPersons = document.getElementById('noContactPersons');

    if (contactPersons.length === 0) {
        tbody.innerHTML = '';
        noContactPersons.classList.remove('d-none');
        return;
    }

    noContactPersons.classList.add('d-none');

    tbody.innerHTML = contactPersons.map(contact => `
        <tr>
            <td>
                ${contact.firstName} ${contact.lastName}
                ${contact.isPrimary ? '<span class="badge bg-primary ms-2">Primary</span>' : ''}
            </td>
            <td>${contact.email}</td>
            <td>${contact.phone}</td>
            <td>${contact.position || '-'}</td>
            <td>
                <i class="bi ${contact.isPrimary ? 'bi-star-fill text-warning' : 'bi-star'}"></i>
            </td>
            <td>
                <span class="badge ${contact.isActive ? 'bg-success' : 'bg-secondary'}">
                    ${contact.isActive ? 'Active' : 'Inactive'}
                </span>
            </td>
            <td>
                <button class="btn btn-sm btn-outline-primary" onclick="editContactPerson(${contact.id})">
                    <i class="bi bi-pencil"></i>
                </button>
                <button class="btn btn-sm btn-outline-danger" onclick="confirmDeleteContact(${contact.id})">
                    <i class="bi bi-trash"></i>
                </button>
            </td>
        </tr>
    `).join('');
}

function openContactPersonModal(contactPerson = null) {
    editingContactId = contactPerson ? contactPerson.id : null;
    const modalTitle = document.getElementById('contactPersonModalTitle');
    modalTitle.textContent = contactPerson ? 'Edit Contact Person' : 'Add Contact Person';

    // Reset form
    document.getElementById('contactPersonForm').reset();

    if (contactPerson) {
        document.getElementById('contactPersonId').value = contactPerson.id;
        document.getElementById('firstName').value = contactPerson.firstName;
        document.getElementById('lastName').value = contactPerson.lastName;
        document.getElementById('contactEmail').value = contactPerson.email;
        document.getElementById('contactPhone').value = contactPerson.phone;
        document.getElementById('position').value = contactPerson.position || '';
        document.getElementById('department').value = contactPerson.department || '';
        document.getElementById('isPrimary').checked = contactPerson.isPrimary;
        document.getElementById('isContactActive').checked = contactPerson.isActive;
        document.getElementById('contactNotes').value = contactPerson.notes || '';
        document.getElementById('isDefaultOwner').checked = contactPerson.isDefaultOwner || false;
        document.getElementById('isDefaultBilling').checked = contactPerson.isDefaultBilling || false;
        document.getElementById('isDefaultTech').checked = contactPerson.isDefaultTech || false;
        document.getElementById('isDefaultAdministrator').checked = contactPerson.isDefaultAdministrator || false;
    } else {
        document.getElementById('contactPersonId').value = '';
        document.getElementById('isContactActive').checked = true;
    }

    const modal = new bootstrap.Modal(document.getElementById('contactPersonModal'));
    modal.show();
}

async function editContactPerson(id) {
    try {
        const response = await window.ContactPersonAPI.getContactPerson(id);
        if (response.success) {
            openContactPersonModal(response.data);
        } else {
            showError('Failed to load contact person');
        }
    } catch (error) {
        console.error('Error loading contact person:', error);
        showError('An error occurred while loading the contact person');
    }
}

async function saveContactPerson() {
    const form = document.getElementById('contactPersonForm');
    if (!form.checkValidity()) {
        form.reportValidity();
        return;
    }

    const contactPersonData = {
        firstName: document.getElementById('firstName').value.trim(),
        lastName: document.getElementById('lastName').value.trim(),
        email: document.getElementById('contactEmail').value.trim(),
        phone: document.getElementById('contactPhone').value.trim(),
        position: document.getElementById('position').value.trim() || null,
        department: document.getElementById('department').value.trim() || null,
        isPrimary: document.getElementById('isPrimary').checked,
        isActive: document.getElementById('isContactActive').checked,
        notes: document.getElementById('contactNotes').value.trim() || null,
        customerId: parseInt(customerId),
        isDefaultOwner: document.getElementById('isDefaultOwner').checked,
        isDefaultBilling: document.getElementById('isDefaultBilling').checked,
        isDefaultTech: document.getElementById('isDefaultTech').checked,
        isDefaultAdministrator: document.getElementById('isDefaultAdministrator').checked
    };

    const saveBtn = document.getElementById('saveContactPersonBtn');
    const originalText = saveBtn.innerHTML;
    saveBtn.disabled = true;
    saveBtn.innerHTML = '<span class="spinner-border spinner-border-sm"></span> Saving...';

    try {
        let response;
        if (editingContactId) {
            response = await window.ContactPersonAPI.updateContactPerson(editingContactId, contactPersonData);
        } else {
            response = await window.ContactPersonAPI.createContactPerson(contactPersonData);
        }

        if (response.success) {
            const modal = bootstrap.Modal.getInstance(document.getElementById('contactPersonModal'));
            modal.hide();
            showSuccess(`Contact person ${editingContactId ? 'updated' : 'created'} successfully!`);
            await loadContactPersons();
        } else {
            alert('Failed to save contact person: ' + (response.message || 'Unknown error'));
        }
    } catch (error) {
        console.error('Error saving contact person:', error);
        alert('An error occurred while saving the contact person');
    } finally {
        saveBtn.disabled = false;
        saveBtn.innerHTML = originalText;
    }
}

function confirmDeleteContact(id) {
    deletingContactId = id;
    const modal = new bootstrap.Modal(document.getElementById('deleteContactModal'));
    modal.show();
}

async function deleteContactPerson() {
    if (!deletingContactId) return;

    const deleteBtn = document.getElementById('confirmDeleteContactBtn');
    const originalText = deleteBtn.innerHTML;
    deleteBtn.disabled = true;
    deleteBtn.innerHTML = '<span class="spinner-border spinner-border-sm"></span> Deleting...';

    try {
        const response = await window.ContactPersonAPI.deleteContactPerson(deletingContactId);

        if (response.success) {
            const modal = bootstrap.Modal.getInstance(document.getElementById('deleteContactModal'));
            modal.hide();
            showSuccess('Contact person deleted successfully!');
            await loadContactPersons();
        } else {
            alert('Failed to delete contact person: ' + (response.message || 'Unknown error'));
        }
    } catch (error) {
        console.error('Error deleting contact person:', error);
        alert('An error occurred while deleting the contact person');
    } finally {
        deleteBtn.disabled = false;
        deleteBtn.innerHTML = originalText;
        deletingContactId = null;
    }
}

// Make functions globally accessible for inline onclick handlers
window.editContactPerson = editContactPerson;
window.confirmDeleteContact = confirmDeleteContact;

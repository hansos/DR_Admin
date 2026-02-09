/**
 * Utility Functions for DR Admin Demo
 */

// Format currency
function formatCurrency(amount, currency = 'USD') {
    return new Intl.NumberFormat('en-US', {
        style: 'currency',
        currency: currency
    }).format(amount);
}

// Escape HTML to prevent XSS
function escapeHtml(text) {
    if (!text) return '';
    const map = {
        '&': '&amp;',
        '<': '&lt;',
        '>': '&gt;',
        '"': '&quot;',
        "'": '&#039;'
    };
    return text.toString().replace(/[&<>"']/g, m => map[m]);
}

// Format date
function formatDate(dateString) {
    if (!dateString) return '-';
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', {
        year: 'numeric',
        month: 'short',
        day: 'numeric'
    });
}

// Format datetime
function formatDateTime(dateString) {
    if (!dateString) return '-';
    const date = new Date(dateString);
    return date.toLocaleString('en-US', {
        year: 'numeric',
        month: 'short',
        day: 'numeric',
        hour: '2-digit',
        minute: '2-digit'
    });
}

// Get URL parameter
function getUrlParameter(name) {
    const params = new URLSearchParams(window.location.search);
    return params.get(name);
}

// Show/hide loading spinner
function showLoading(elementId) {
    const el = document.getElementById(elementId);
    if (el) el.classList.remove('d-none');
}

function hideLoading(elementId) {
    const el = document.getElementById(elementId);
    if (el) el.classList.add('d-none');
}

// Export to window
if (typeof window !== 'undefined') {
    window.formatCurrency = formatCurrency;
    window.escapeHtml = escapeHtml;
    window.formatDate = formatDate;
    window.formatDateTime = formatDateTime;
    window.getUrlParameter = getUrlParameter;
    window.showLoading = showLoading;
    window.hideLoading = hideLoading;
}

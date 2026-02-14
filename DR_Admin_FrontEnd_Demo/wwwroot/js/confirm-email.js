/**
 * Email Confirmation Page Script
 * Handles email confirmation using token from URL query parameter
 */

// API Base URL - calls DR_Admin API directly
const BASE_URL = 'https://localhost:7201/api/v1';

/**
 * Show specific state and hide others
 */
function showState(stateName) {
    const states = ['loadingState', 'successState', 'errorState', 'missingTokenState'];
    states.forEach(state => {
        const element = document.getElementById(state);
        if (element) {
            if (state === stateName) {
                element.classList.remove('d-none');
            } else {
                element.classList.add('d-none');
            }
        }
    });
}

/**
 * Get query parameter from URL
 */
function getQueryParameter(name) {
    const urlParams = new URLSearchParams(window.location.search);
    return urlParams.get(name);
}

/**
 * Call the confirm email API
 */
async function confirmEmail(token) {
    try {
        const response = await fetch(`${BASE_URL}/myaccount/confirm-email`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            credentials: 'include',
            body: JSON.stringify({
                confirmationToken: token
            }),
        });

        const data = await response.json();

        if (response.ok) {
            // Success
            showState('successState');
            console.log('Email confirmed successfully');
        } else {
            // API returned an error
            showState('errorState');
            const errorMessageElement = document.getElementById('errorMessage');
            if (errorMessageElement) {
                errorMessageElement.textContent = data.message || 'The confirmation link is invalid or has expired.';
            }
            console.error('Email confirmation failed:', data.message);
        }
    } catch (error) {
        // Network or other error
        showState('errorState');
        const errorMessageElement = document.getElementById('errorMessage');
        if (errorMessageElement) {
            errorMessageElement.textContent = 'Unable to connect to the server. Please check your internet connection and try again.';
        }
        console.error('Error during email confirmation:', error);
    }
}

/**
 * Initialize the page
 */
async function initConfirmEmail() {
    // Get token from URL
    const token = getQueryParameter('token');

    if (!token) {
        // No token provided
        showState('missingTokenState');
        return;
    }

    // Show loading state
    showState('loadingState');

    // Wait a moment for better UX (optional)
    await new Promise(resolve => setTimeout(resolve, 500));

    // Confirm the email
    await confirmEmail(token);
}

// Run when page loads
document.addEventListener('DOMContentLoaded', initConfirmEmail);

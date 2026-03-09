"use strict";
function initializeShopInitializePage() {
    const page = document.getElementById('shop-initialize-page');
    if (!page || page.dataset.bound === 'true') {
        return;
    }
    page.dataset.bound = 'true';
    const button = document.getElementById('shop-initialize-import-button');
    button?.addEventListener('click', async () => {
        await importShopDebugSnapshot();
    });
    const userForm = document.getElementById('shop-initialize-user-form');
    userForm?.addEventListener('submit', async (event) => {
        event.preventDefault();
        await createShopUser();
    });
    void setupShopInitializeBuildMode();
}
async function createShopUser() {
    const typedWindow = window;
    typedWindow.UserPanelAlerts?.hide('shop-initialize-alert-success');
    typedWindow.UserPanelAlerts?.hide('shop-initialize-alert-error');
    const request = buildShopInitializeRegisterRequest();
    if (!request) {
        typedWindow.UserPanelAlerts?.showError('shop-initialize-alert-error', 'Please fill in all required user fields.');
        return;
    }
    const button = document.getElementById('shop-initialize-user-button');
    if (button) {
        button.disabled = true;
        button.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>Creating...';
    }
    const response = await typedWindow.UserPanelApi?.request('/Initialization/initialize-customer', {
        method: 'POST',
        body: JSON.stringify(request)
    }, false);
    if (button) {
        button.disabled = false;
        button.innerHTML = '<i class="bi bi-person-check"></i> Create User';
    }
    if (!response || !response.success) {
        typedWindow.UserPanelAlerts?.showError('shop-initialize-alert-error', response?.message ?? 'User creation failed.');
        return;
    }
    typedWindow.UserPanelAlerts?.showSuccess('shop-initialize-alert-success', response.message ?? 'User created successfully.');
}
function buildShopInitializeRegisterRequest() {
    const username = readShopInitializeValue('shop-initialize-username');
    const email = readShopInitializeValue('shop-initialize-email');
    const password = readShopInitializeValue('shop-initialize-password');
    const confirmPassword = readShopInitializeValue('shop-initialize-confirm-password');
    const customerName = readShopInitializeValue('shop-initialize-customer-name');
    const contactFirstName = readShopInitializeValue('shop-initialize-contact-first-name');
    const contactLastName = readShopInitializeValue('shop-initialize-contact-last-name');
    const customerPhone = readShopInitializeValue('shop-initialize-customer-phone');
    if (!username || !email || !password || !confirmPassword || !customerName || !contactFirstName || !contactLastName) {
        return null;
    }
    if (password !== confirmPassword) {
        return null;
    }
    return {
        username,
        email,
        password,
        confirmPassword,
        companyName: customerName,
        companyPhone: customerPhone,
        contactFirstName,
        contactLastName
    };
}
function readShopInitializeValue(id) {
    const input = document.getElementById(id);
    return input?.value.trim() ?? '';
}
async function setupShopInitializeBuildMode() {
    const typedWindow = window;
    const info = document.getElementById('shop-initialize-alert-info');
    const importSection = document.getElementById('shop-initialize-import-section');
    typedWindow.UserPanelAlerts?.hide('shop-initialize-alert-success');
    typedWindow.UserPanelAlerts?.hide('shop-initialize-alert-error');
    const response = await typedWindow.UserPanelApi?.request('/Initialization/build-mode', {
        method: 'GET'
    }, false);
    if (!response || !response.success || !response.data) {
        if (info) {
            info.textContent = 'Unable to verify API build mode.';
        }
        typedWindow.UserPanelAlerts?.showError('shop-initialize-alert-error', response?.message ?? 'Unable to verify API build mode.');
        return;
    }
    const isDebug = response.data.isDebug ?? response.data.IsDebug ?? false;
    const mode = response.data.mode ?? response.data.Mode ?? (isDebug ? 'Debug' : 'Release');
    if (!isDebug) {
        if (info) {
            info.textContent = `Debug import is disabled. API build mode: ${mode}.`;
        }
        return;
    }
    if (info) {
        info.classList.add('d-none');
    }
    importSection?.classList.remove('d-none');
}
async function importShopDebugSnapshot() {
    const typedWindow = window;
    const fileInput = document.getElementById('shop-initialize-file');
    const button = document.getElementById('shop-initialize-import-button');
    const fileName = fileInput?.value.trim() ?? '';
    typedWindow.UserPanelAlerts?.hide('shop-initialize-alert-success');
    typedWindow.UserPanelAlerts?.hide('shop-initialize-alert-error');
    if (!fileName) {
        typedWindow.UserPanelAlerts?.showError('shop-initialize-alert-error', 'Snapshot file name is required.');
        return;
    }
    if (button) {
        button.disabled = true;
        button.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>Importing...';
    }
    const response = await typedWindow.UserPanelApi?.request('/Initialization/import-customer-snapshot', {
        method: 'POST',
        body: JSON.stringify({ fileName })
    }, false);
    if (button) {
        button.disabled = false;
        button.innerHTML = '<i class="bi bi-upload"></i> Import Snapshot';
    }
    if (!response || !response.success || !response.data) {
        typedWindow.UserPanelAlerts?.showError('shop-initialize-alert-error', response?.message ?? 'Snapshot import failed.');
        return;
    }
    const importSuccess = response.data.success ?? response.data.Success ?? false;
    if (!importSuccess) {
        typedWindow.UserPanelAlerts?.showError('shop-initialize-alert-error', response.data.errorMessage ?? response.data.ErrorMessage ?? 'Snapshot import failed.');
        return;
    }
    typedWindow.UserPanelAlerts?.showSuccess('shop-initialize-alert-success', 'Snapshot imported successfully.');
}
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initializeShopInitializePage);
}
else {
    initializeShopInitializePage();
}
function registerShopInitializeEnhancedLoadListener() {
    const typedWindow = window;
    if (typedWindow.Blazor?.addEventListener) {
        typedWindow.Blazor.addEventListener('enhancedload', initializeShopInitializePage);
        return;
    }
    window.setTimeout(registerShopInitializeEnhancedLoadListener, 100);
}
registerShopInitializeEnhancedLoadListener();
//# sourceMappingURL=initialize.js.map
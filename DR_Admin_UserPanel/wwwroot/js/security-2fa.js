"use strict";
(() => {
    let currentTwoFactorMethod = 'Email';
    function initializeSecurityTwoFactorPage() {
        const page = document.getElementById('security-2fa-page');
        if (!page || page.dataset.initialized === 'true') {
            return;
        }
        page.dataset.initialized = 'true';
        page.addEventListener('submit', (event) => {
            const target = event.target;
            if (target?.id !== 'security-2fa-form') {
                return;
            }
            event.preventDefault();
            void saveTwoFactorStatus();
        });
        page.addEventListener('click', (event) => {
            const target = event.target;
            const button = target?.closest('button');
            if (!button) {
                return;
            }
            if (button.id === 'security-2fa-send-verification') {
                void requestEmailVerification();
                return;
            }
            if (button.id === 'security-2fa-setup-authenticator') {
                void beginAuthenticatorSetup();
                return;
            }
            if (button.id === 'security-2fa-confirm-authenticator') {
                void confirmAuthenticatorSetup();
                return;
            }
            if (button.id === 'security-2fa-disable-existing') {
                void disableTwoFactor();
                return;
            }
            if (button.id === 'security-2fa-enable-existing') {
                void enableTwoFactorUsingExistingSettings();
                return;
            }
            if (button.id === 'security-2fa-delete-existing') {
                void deleteTwoFactor();
            }
        });
        page.addEventListener('change', (event) => {
            const target = event.target;
            if (target?.id === 'security-2fa-enabled') {
                updateVerificationMethodAvailability();
                return;
            }
            if (target?.name === 'security-2fa-method') {
                updateAuthenticatorPanel();
            }
        });
        void loadSecurityTwoFactorPage();
    }
    async function loadSecurityTwoFactorPage() {
        const typedWindow = window;
        typedWindow.UserPanelAlerts?.hide('security-2fa-alert-success');
        typedWindow.UserPanelAlerts?.hide('security-2fa-alert-error');
        const accountResponse = await typedWindow.UserPanelApi?.request('/MyAccount/me', { method: 'GET' }, true);
        if (!accountResponse || !accountResponse.success || !accountResponse.data) {
            typedWindow.UserPanelAlerts?.showError('security-2fa-alert-error', accountResponse?.message ?? 'Could not load account security details.');
            return;
        }
        renderAccount(accountResponse.data);
        const statusResponse = await typedWindow.UserPanelApi?.request('/MyAccount/2fa/status', { method: 'GET' }, true);
        if (!statusResponse || !statusResponse.success || !statusResponse.data) {
            renderTwoFactorStatus(null);
            return;
        }
        renderTwoFactorStatus(statusResponse.data);
    }
    function renderAccount(account) {
        const summary = document.getElementById('security-2fa-account-summary');
        if (summary) {
            summary.innerHTML = `
            <div><strong>${escapeSecurityTwoFactorText(account.username)}</strong></div>
            <div>${escapeSecurityTwoFactorText(account.email)}</div>
            <div class="text-muted">User ID: ${account.id}</div>
        `;
        }
        const emailStatus = document.getElementById('security-2fa-email-status');
        if (emailStatus) {
            emailStatus.innerHTML = account.emailConfirmed
                ? '<span class="badge bg-success">Email verified</span>'
                : '<span class="badge bg-warning text-dark">Email not verified</span>';
        }
        const verifyButton = document.getElementById('security-2fa-send-verification');
        if (verifyButton) {
            verifyButton.classList.toggle('d-none', !!account.emailConfirmed);
        }
    }
    function renderTwoFactorStatus(status) {
        const checkbox = document.getElementById('security-2fa-enabled');
        const statusBox = document.getElementById('security-2fa-status');
        if (!checkbox || !statusBox) {
            return;
        }
        if (!status) {
            setTwoFactorActionsMode('definition');
            checkbox.checked = false;
            currentTwoFactorMethod = 'Email';
            setSelectedMethod('Email');
            updateVerificationMethodAvailability();
            updateAuthenticatorPanel();
            hideAuthenticatorRegistrationUi();
            showNextStepInfo('');
            statusBox.textContent = '2FA endpoint is not available yet for this environment.';
            return;
        }
        const hasReusableSetup = !status.enabled && normalizeTwoFactorMethod(status.method) === 'Authenticator';
        setTwoFactorActionsMode(status.enabled ? 'enabled' : (hasReusableSetup ? 'disabled' : 'definition'));
        checkbox.checked = status.enabled;
        updateVerificationMethodAvailability();
        const recovery = status.recoveryCodesRemaining ?? 0;
        const method = normalizeTwoFactorMethod(status.method);
        currentTwoFactorMethod = method;
        setSelectedMethod(method);
        updateAuthenticatorPanel();
        if (status.enabled && method === 'Authenticator') {
            hideAuthenticatorRegistrationUi();
            showNextStepInfo('Authenticator is active. You can continue using your app at sign-in.');
        }
        else {
            showAuthenticatorRegistrationUi();
            showNextStepInfo('');
        }
        statusBox.textContent = status.enabled
            ? `2FA is enabled (${method}). Recovery codes remaining: ${recovery}.`
            : '2FA is currently disabled.';
    }
    function setTwoFactorActionsMode(mode) {
        const definitionControls = document.getElementById('security-2fa-definition-controls');
        const existingActions = document.getElementById('security-2fa-existing-actions');
        const enableButton = document.getElementById('security-2fa-enable-existing');
        const disableButton = document.getElementById('security-2fa-disable-existing');
        if (definitionControls) {
            definitionControls.classList.toggle('d-none', mode !== 'definition');
        }
        if (existingActions) {
            existingActions.classList.toggle('d-none', mode === 'definition');
        }
        if (enableButton) {
            enableButton.classList.toggle('d-none', mode !== 'disabled');
        }
        if (disableButton) {
            disableButton.classList.toggle('d-none', mode !== 'enabled');
        }
    }
    async function enableTwoFactorUsingExistingSettings() {
        const typedWindow = window;
        typedWindow.UserPanelAlerts?.hide('security-2fa-alert-success');
        typedWindow.UserPanelAlerts?.hide('security-2fa-alert-error');
        const response = await typedWindow.UserPanelApi?.request('/MyAccount/2fa', {
            method: 'POST',
            body: JSON.stringify({ enabled: true, method: currentTwoFactorMethod })
        }, true);
        if (!response || !response.success) {
            typedWindow.UserPanelAlerts?.showError('security-2fa-alert-error', response?.message ?? 'Could not re-enable 2FA with existing settings.');
            return;
        }
        typedWindow.UserPanelAlerts?.showSuccess('security-2fa-alert-success', response.message ?? 'Two-factor authentication was enabled.');
        await loadSecurityTwoFactorPage();
    }
    function updateAuthenticatorPanel() {
        const panel = document.getElementById('security-2fa-authenticator-panel');
        if (!panel) {
            return;
        }
        const isEnabled = document.getElementById('security-2fa-enabled')?.checked ?? false;
        panel.classList.toggle('d-none', !isEnabled || getSelectedMethod() !== 'Authenticator');
        if (!isEnabled || getSelectedMethod() !== 'Authenticator') {
            showNextStepInfo('');
        }
    }
    function updateVerificationMethodAvailability() {
        const isEnabled = document.getElementById('security-2fa-enabled')?.checked ?? false;
        const methodOptions = document.querySelectorAll('input[name="security-2fa-method"]');
        methodOptions.forEach((option) => {
            option.disabled = !isEnabled;
        });
        const setupButton = document.getElementById('security-2fa-setup-authenticator');
        const confirmButton = document.getElementById('security-2fa-confirm-authenticator');
        const codeInput = document.getElementById('security-2fa-authenticator-code');
        if (setupButton) {
            setupButton.disabled = !isEnabled;
        }
        if (confirmButton) {
            confirmButton.disabled = !isEnabled;
        }
        if (codeInput) {
            codeInput.disabled = !isEnabled;
        }
        updateAuthenticatorPanel();
    }
    function setSelectedMethod(method) {
        const normalized = normalizeTwoFactorMethod(method);
        const emailOption = document.getElementById('security-2fa-method-email');
        const authOption = document.getElementById('security-2fa-method-authenticator');
        if (emailOption && authOption) {
            emailOption.checked = normalized === 'Email';
            authOption.checked = normalized === 'Authenticator';
        }
    }
    function getSelectedMethod() {
        const selected = document.querySelector('input[name="security-2fa-method"]:checked');
        return normalizeTwoFactorMethod(selected?.value ?? 'Email');
    }
    function normalizeTwoFactorMethod(value) {
        return value?.trim().toLowerCase() === 'authenticator' ? 'Authenticator' : 'Email';
    }
    async function beginAuthenticatorSetup() {
        const typedWindow = window;
        typedWindow.UserPanelAlerts?.hide('security-2fa-alert-success');
        typedWindow.UserPanelAlerts?.hide('security-2fa-alert-error');
        const response = await typedWindow.UserPanelApi?.request('/MyAccount/2fa/authenticator/setup', {
            method: 'POST'
        }, true);
        if (!response || !response.success || !response.data) {
            typedWindow.UserPanelAlerts?.showError('security-2fa-alert-error', response?.message ?? 'Could not initialize authenticator setup.');
            return;
        }
        const qrWrapper = document.getElementById('security-2fa-qr-wrapper');
        const qrImage = document.getElementById('security-2fa-qr-image');
        const manualKey = document.getElementById('security-2fa-manual-key');
        showAuthenticatorRegistrationUi();
        showNextStepInfo('Step 1: Scan the QR code in Microsoft Authenticator. Step 2: Enter the 6-digit code and press Confirm app.');
        if (qrWrapper && qrImage) {
            qrImage.src = `https://api.qrserver.com/v1/create-qr-code/?size=220x220&data=${encodeURIComponent(response.data.qrCodeUri)}`;
            qrWrapper.classList.remove('d-none');
        }
        if (manualKey) {
            manualKey.innerHTML = `<strong>Manual key:</strong> ${escapeSecurityTwoFactorText(response.data.sharedKey)}`;
        }
        typedWindow.UserPanelAlerts?.showSuccess('security-2fa-alert-success', 'Authenticator setup created. Scan QR code and confirm with a 6-digit code.');
    }
    async function confirmAuthenticatorSetup() {
        const typedWindow = window;
        typedWindow.UserPanelAlerts?.hide('security-2fa-alert-success');
        typedWindow.UserPanelAlerts?.hide('security-2fa-alert-error');
        const codeInput = document.getElementById('security-2fa-authenticator-code');
        const code = (codeInput?.value ?? '').replace(/\D/g, '').trim();
        if (code.length !== 6) {
            typedWindow.UserPanelAlerts?.showError('security-2fa-alert-error', 'Enter a valid 6-digit authenticator code.');
            return;
        }
        if (codeInput) {
            codeInput.value = code;
        }
        const response = await typedWindow.UserPanelApi?.request('/MyAccount/2fa/authenticator/confirm', {
            method: 'POST',
            body: JSON.stringify({ code })
        }, true);
        if (!response || !response.success) {
            typedWindow.UserPanelAlerts?.showError('security-2fa-alert-error', response?.message ?? 'Could not confirm authenticator setup.');
            return;
        }
        const confirmButton = document.getElementById('security-2fa-confirm-authenticator');
        if (confirmButton) {
            confirmButton.disabled = true;
        }
        if (codeInput) {
            codeInput.disabled = true;
        }
        hideAuthenticatorRegistrationUi();
        showNextStepInfo('Authenticator confirmed successfully.');
        typedWindow.UserPanelAlerts?.showSuccess('security-2fa-alert-success', response.message ?? 'Authenticator was configured successfully.');
        await loadSecurityTwoFactorPage();
    }
    async function disableTwoFactor() {
        const typedWindow = window;
        typedWindow.UserPanelAlerts?.hide('security-2fa-alert-success');
        typedWindow.UserPanelAlerts?.hide('security-2fa-alert-error');
        const response = await typedWindow.UserPanelApi?.request('/MyAccount/2fa', {
            method: 'POST',
            body: JSON.stringify({ enabled: false, method: 'Email' })
        }, true);
        if (!response || !response.success) {
            typedWindow.UserPanelAlerts?.showError('security-2fa-alert-error', response?.message ?? 'Could not disable 2FA.');
            return;
        }
        typedWindow.UserPanelAlerts?.showSuccess('security-2fa-alert-success', response.message ?? 'Two-factor authentication was disabled.');
        await loadSecurityTwoFactorPage();
    }
    async function deleteTwoFactor() {
        const typedWindow = window;
        typedWindow.UserPanelAlerts?.hide('security-2fa-alert-success');
        typedWindow.UserPanelAlerts?.hide('security-2fa-alert-error');
        const response = await typedWindow.UserPanelApi?.request('/MyAccount/2fa', {
            method: 'DELETE'
        }, true);
        if (!response || !response.success) {
            typedWindow.UserPanelAlerts?.showError('security-2fa-alert-error', response?.message ?? 'Could not delete 2FA setup.');
            return;
        }
        typedWindow.UserPanelAlerts?.showSuccess('security-2fa-alert-success', response.message ?? 'Two-factor setup was deleted.');
        await loadSecurityTwoFactorPage();
    }
    function showAuthenticatorRegistrationUi() {
        const setupControls = document.getElementById('security-2fa-authenticator-setup-controls');
        if (setupControls) {
            setupControls.classList.remove('d-none');
        }
    }
    function hideAuthenticatorRegistrationUi() {
        const setupControls = document.getElementById('security-2fa-authenticator-setup-controls');
        if (setupControls) {
            setupControls.classList.add('d-none');
        }
        const qrWrapper = document.getElementById('security-2fa-qr-wrapper');
        if (qrWrapper) {
            qrWrapper.classList.add('d-none');
        }
        const manualKey = document.getElementById('security-2fa-manual-key');
        if (manualKey) {
            manualKey.textContent = '';
        }
    }
    function showNextStepInfo(message) {
        const info = document.getElementById('security-2fa-next-step');
        if (!info) {
            return;
        }
        if (!message.trim()) {
            info.textContent = '';
            info.classList.add('d-none');
            return;
        }
        info.textContent = message;
        info.classList.remove('d-none');
    }
    async function requestEmailVerification() {
        const typedWindow = window;
        typedWindow.UserPanelAlerts?.hide('security-2fa-alert-success');
        typedWindow.UserPanelAlerts?.hide('security-2fa-alert-error');
        const siteCode = typedWindow.UserPanelSettings?.frontendSiteCode ?? 'shop';
        const response = await typedWindow.UserPanelApi?.request('/MyAccount/request-email-confirmation', {
            method: 'POST',
            body: JSON.stringify({ siteCode })
        }, true);
        if (!response || !response.success) {
            typedWindow.UserPanelAlerts?.showError('security-2fa-alert-error', response?.message ?? 'Could not send verification email.');
            return;
        }
        typedWindow.UserPanelAlerts?.showSuccess('security-2fa-alert-success', response.message ?? 'Verification email sent.');
    }
    async function saveTwoFactorStatus() {
        const typedWindow = window;
        typedWindow.UserPanelAlerts?.hide('security-2fa-alert-success');
        typedWindow.UserPanelAlerts?.hide('security-2fa-alert-error');
        const checkbox = document.getElementById('security-2fa-enabled');
        if (!checkbox) {
            return;
        }
        const response = await typedWindow.UserPanelApi?.request('/MyAccount/2fa', {
            method: 'POST',
            body: JSON.stringify({ enabled: checkbox.checked, method: getSelectedMethod() })
        }, true);
        if (!response || !response.success) {
            typedWindow.UserPanelAlerts?.showError('security-2fa-alert-error', response?.message ?? 'Could not update 2FA setting.');
            return;
        }
        typedWindow.UserPanelAlerts?.showSuccess('security-2fa-alert-success', 'Two-factor setting was updated.');
        await loadSecurityTwoFactorPage();
    }
    function escapeSecurityTwoFactorText(value) {
        return value
            .replace(/&/g, '&amp;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;')
            .replace(/'/g, '&#039;');
    }
    function setupSecurityTwoFactorObserver() {
        initializeSecurityTwoFactorPage();
        const observer = new MutationObserver(() => {
            initializeSecurityTwoFactorPage();
        });
        observer.observe(document.body, { childList: true, subtree: true });
    }
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', setupSecurityTwoFactorObserver);
    }
    else {
        setupSecurityTwoFactorObserver();
    }
    function registerSecurityTwoFactorEnhancedLoadListener() {
        const typedWindow = window;
        if (typedWindow.Blazor?.addEventListener) {
            typedWindow.Blazor.addEventListener('enhancedload', initializeSecurityTwoFactorPage);
            return;
        }
        window.setTimeout(registerSecurityTwoFactorEnhancedLoadListener, 100);
    }
    registerSecurityTwoFactorEnhancedLoadListener();
})();
//# sourceMappingURL=security-2fa.js.map
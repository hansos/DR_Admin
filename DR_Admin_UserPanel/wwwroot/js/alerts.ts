interface UserPanelWindowAlerts extends Window {
    UserPanelAlerts?: {
        showSuccess: (id: string, message: string) => void;
        showError: (id: string, message: string) => void;
        hide: (id: string) => void;
    };
}

function showAlert(id: string, message: string, variant: 'success' | 'error'): void {
    const element = document.getElementById(id);
    if (!element) {
        return;
    }

    element.textContent = message;
    element.classList.remove('d-none', 'alert-success', 'alert-danger');
    element.classList.add(variant === 'success' ? 'alert-success' : 'alert-danger');
}

function hide(id: string): void {
    const element = document.getElementById(id);
    if (!element) {
        return;
    }

    element.classList.add('d-none');
    element.textContent = '';
}

const alertsWindow = window as UserPanelWindowAlerts;
alertsWindow.UserPanelAlerts = {
    showSuccess: (id: string, message: string) => showAlert(id, message, 'success'),
    showError: (id: string, message: string) => showAlert(id, message, 'error'),
    hide
};

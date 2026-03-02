"use strict";
(() => {
    let dnsDomainId = null;
    let dnsRecordTypes = [];
    function initializeDnsZonesPage() {
        const page = document.getElementById('dns-zones-page');
        if (!page || page.dataset.initialized === 'true') {
            return;
        }
        page.dataset.initialized = 'true';
        dnsDomainId = getDnsDomainId();
        const form = document.getElementById('dns-zones-form');
        form?.addEventListener('submit', async (event) => {
            event.preventDefault();
            await createDnsRecord();
        });
        document.getElementById('dns-zones-type')?.addEventListener('change', () => {
            syncDnsTypeFieldRequirements();
        });
        const table = document.getElementById('dns-zones-table-body');
        table?.addEventListener('click', (event) => {
            const target = event.target;
            const button = target.closest('button[data-id]');
            if (!button) {
                return;
            }
            const id = Number.parseInt(button.dataset.id ?? '', 10);
            if (!Number.isNaN(id) && id > 0) {
                void deleteDnsRecord(id);
            }
        });
        void loadDnsData();
    }
    async function loadDnsData() {
        const typedWindow = window;
        typedWindow.UserPanelAlerts?.hide('dns-zones-alert-error');
        if (!dnsDomainId) {
            typedWindow.UserPanelAlerts?.showError('dns-zones-alert-error', 'Missing or invalid domain id.');
            renderDnsRows([]);
            return;
        }
        const [typesResponse, recordsResponse] = await Promise.all([
            typedWindow.UserPanelApi?.request('/DnsRecordTypes', { method: 'GET' }, true),
            typedWindow.UserPanelApi?.request(`/DnsRecords/domain/${dnsDomainId}`, { method: 'GET' }, true)
        ]);
        dnsRecordTypes = (typesResponse?.success ? (typesResponse.data ?? []) : []).filter((item) => item.isActive);
        renderDnsTypeOptions(dnsRecordTypes);
        syncDnsTypeFieldRequirements();
        if (!recordsResponse?.success) {
            typedWindow.UserPanelAlerts?.showError('dns-zones-alert-error', recordsResponse?.message ?? 'Could not load DNS records.');
            renderDnsRows([]);
            return;
        }
        renderDnsRows(recordsResponse.data ?? []);
    }
    function renderDnsTypeOptions(items) {
        const select = document.getElementById('dns-zones-type');
        if (!select) {
            return;
        }
        if (items.length === 0) {
            select.innerHTML = '<option value="">No types available</option>';
            return;
        }
        select.innerHTML = items.map((item) => `<option value="${item.id}">${escapeDnsText(item.type)}</option>`).join('');
    }
    function renderDnsRows(items) {
        const tableBody = document.getElementById('dns-zones-table-body');
        if (!tableBody) {
            return;
        }
        if (items.length === 0) {
            tableBody.innerHTML = '<tr><td colspan="6" class="text-center text-muted">No DNS records found.</td></tr>';
            return;
        }
        tableBody.innerHTML = items.map((item) => `
        <tr>
            <td>${escapeDnsText(item.type)}</td>
            <td>${escapeDnsText(item.name)}</td>
            <td>${escapeDnsText(item.value)}</td>
            <td>${item.ttl}</td>
            <td>${item.priority ?? '-'}</td>
            <td>
                <button class="btn btn-outline-danger btn-sm" type="button" data-id="${item.id}">Delete</button>
            </td>
        </tr>
    `).join('');
    }
    function syncDnsTypeFieldRequirements() {
        const typeId = Number.parseInt(readDnsInputValue('dns-zones-type'), 10);
        const selectedType = dnsRecordTypes.find((item) => item.id === typeId) ?? null;
        const priorityInput = document.getElementById('dns-zones-priority');
        const weightInput = document.getElementById('dns-zones-weight');
        const portInput = document.getElementById('dns-zones-port');
        const ttlInput = document.getElementById('dns-zones-ttl');
        if (priorityInput) {
            priorityInput.disabled = !selectedType?.hasPriority;
            if (!selectedType?.hasPriority) {
                priorityInput.value = '';
            }
        }
        if (weightInput) {
            weightInput.disabled = !selectedType?.hasWeight;
            if (!selectedType?.hasWeight) {
                weightInput.value = '';
            }
        }
        if (portInput) {
            portInput.disabled = !selectedType?.hasPort;
            if (!selectedType?.hasPort) {
                portInput.value = '';
            }
        }
        if (ttlInput && selectedType?.defaultTTL) {
            ttlInput.value = selectedType.defaultTTL.toString();
        }
    }
    async function createDnsRecord() {
        const typedWindow = window;
        typedWindow.UserPanelAlerts?.hide('dns-zones-alert-success');
        typedWindow.UserPanelAlerts?.hide('dns-zones-alert-error');
        if (!dnsDomainId) {
            typedWindow.UserPanelAlerts?.showError('dns-zones-alert-error', 'Missing domain id.');
            return;
        }
        const dnsRecordTypeId = Number.parseInt(readDnsInputValue('dns-zones-type'), 10);
        const name = readDnsInputValue('dns-zones-name');
        const value = readDnsInputValue('dns-zones-value');
        const ttl = Number.parseInt(readDnsInputValue('dns-zones-ttl'), 10);
        if (Number.isNaN(dnsRecordTypeId) || !name || !value || Number.isNaN(ttl)) {
            typedWindow.UserPanelAlerts?.showError('dns-zones-alert-error', 'Type, name, value and TTL are required.');
            return;
        }
        const payload = {
            domainId: dnsDomainId,
            dnsRecordTypeId,
            name,
            value,
            ttl,
            priority: parseOptionalDnsNumber('dns-zones-priority'),
            weight: parseOptionalDnsNumber('dns-zones-weight'),
            port: parseOptionalDnsNumber('dns-zones-port')
        };
        const response = await typedWindow.UserPanelApi?.request('/DnsRecords', {
            method: 'POST',
            body: JSON.stringify(payload)
        }, true);
        if (!response || !response.success) {
            typedWindow.UserPanelAlerts?.showError('dns-zones-alert-error', response?.message ?? 'Could not create DNS record.');
            return;
        }
        typedWindow.UserPanelAlerts?.showSuccess('dns-zones-alert-success', 'DNS record added.');
        const form = document.getElementById('dns-zones-form');
        form?.reset();
        syncDnsTypeFieldRequirements();
        await loadDnsData();
    }
    async function deleteDnsRecord(id) {
        const typedWindow = window;
        const response = await typedWindow.UserPanelApi?.request(`/DnsRecords/${id}`, {
            method: 'DELETE'
        }, true);
        if (!response || !response.success) {
            typedWindow.UserPanelAlerts?.showError('dns-zones-alert-error', response?.message ?? 'Could not delete DNS record.');
            return;
        }
        typedWindow.UserPanelAlerts?.showSuccess('dns-zones-alert-success', 'DNS record deleted.');
        await loadDnsData();
    }
    function getDnsDomainId() {
        const idText = new URLSearchParams(window.location.search).get('id');
        if (!idText) {
            return null;
        }
        const id = Number.parseInt(idText, 10);
        return Number.isNaN(id) || id <= 0 ? null : id;
    }
    function parseOptionalDnsNumber(id) {
        const value = readDnsInputValue(id);
        if (!value) {
            return null;
        }
        const parsed = Number.parseInt(value, 10);
        return Number.isNaN(parsed) ? null : parsed;
    }
    function readDnsInputValue(id) {
        const input = document.getElementById(id);
        return input?.value.trim() ?? '';
    }
    function escapeDnsText(value) {
        return value
            .replace(/&/g, '&amp;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;')
            .replace(/'/g, '&#039;');
    }
    function setupDnsObserver() {
        initializeDnsZonesPage();
        const observer = new MutationObserver(() => {
            initializeDnsZonesPage();
        });
        observer.observe(document.body, { childList: true, subtree: true });
    }
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', setupDnsObserver);
    }
    else {
        setupDnsObserver();
    }
})();
//# sourceMappingURL=dns-zones.js.map
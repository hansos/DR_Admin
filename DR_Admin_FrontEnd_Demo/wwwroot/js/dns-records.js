// dns-records.js - Domains+DNS Management Page

let selectedDomainId = null;
let selectedDomainName = '';
let dnsRecordTypes = [];
let activeRecords = [];
let deletedRecords = [];
let pendingRecords = [];
let recordToDelete = null;
let recordToHardDelete = null;
let editingRecordId = null;

document.addEventListener('DOMContentLoaded', async () => {
    checkAuthState();
    await Promise.all([loadDomains(), loadDnsRecordTypes()]);
    initEventListeners();
});

async function loadDomains() {
    const select = document.getElementById('domainSelect');
    try {
        const result = await window.RegisteredDomainAPI.getAll();
        if (result.success) {
            const domains = Array.isArray(result.data) ? result.data : [];
            select.innerHTML = '<option value="">-- Select a domain --</option>';
            if (domains.length === 0) {
                select.innerHTML = '<option value="">-- No domains available --</option>';
                return;
            }
            domains
                .sort((a, b) => (a.name || '').localeCompare(b.name || ''))
                .forEach(d => {
                    const opt = document.createElement('option');
                    opt.value = d.id;
                    opt.textContent = d.name || d.domainName || `Domain #${d.id}`;
                    select.appendChild(opt);
                });
        } else {
            select.innerHTML = '<option value="">-- Failed to load domains --</option>';
            console.warn('Failed to load domains:', result.message);
        }
    } catch (err) {
        select.innerHTML = '<option value="">-- Error loading domains --</option>';
        console.error('Error loading domains:', err);
    }
}

async function loadDnsRecordTypes() {
    try {
        const result = await window.DnsRecordTypeAPI.getAll();
        if (result.success) {
            dnsRecordTypes = Array.isArray(result.data) ? result.data : [];
        } else {
            console.warn('Failed to load DNS record types:', result.message);
        }
    } catch (err) {
        console.error('Error loading DNS record types:', err);
    }
}

function initEventListeners() {
    document.getElementById('domainSelect').addEventListener('change', async (e) => {
        selectedDomainId = e.target.value || null;
        selectedDomainName = selectedDomainId ? (e.target.options[e.target.selectedIndex]?.text || '') : '';
        document.getElementById('addRecordBtn').disabled = !selectedDomainId;
        document.getElementById('syncToServerBtn').disabled = !selectedDomainId;
        if (selectedDomainId) {
            await loadRecords();
        } else {
            showNoSelection();
        }
    });

    document.getElementById('showDeleted').addEventListener('change', () => renderTable());
    document.getElementById('addRecordBtn').addEventListener('click', () => openCreateModal());
    document.getElementById('syncToServerBtn').addEventListener('click', () => openSyncModal());
    document.getElementById('confirmSyncBtn').addEventListener('click', () => performSync());
    document.getElementById('saveRecordBtn').addEventListener('click', saveRecord);
    document.getElementById('recordType').addEventListener('change', updateFieldVisibility);

    document.getElementById('confirmSoftDelete').addEventListener('click', async () => {
        if (recordToDelete !== null) await performSoftDelete(recordToDelete);
    });

    document.getElementById('confirmHardDelete').addEventListener('click', async () => {
        if (recordToHardDelete !== null) await performHardDelete(recordToHardDelete);
    });
}

async function loadRecords() {
    setLoadingState(true);
    try {
        const [activeResult, deletedResult] = await Promise.all([
            window.DnsRecordAPI.getByDomain(selectedDomainId),
            window.DnsRecordAPI.getDeletedByDomain(selectedDomainId),
        ]);

        activeRecords = (activeResult.success && Array.isArray(activeResult.data)) ? activeResult.data : [];
        deletedRecords = (deletedResult.success && Array.isArray(deletedResult.data)) ? deletedResult.data : [];

        const select = document.getElementById('domainSelect');
        const domainName = select.options[select.selectedIndex]?.text || '';
        document.getElementById('selectedDomainLabel').textContent = domainName;

        renderTable();
    } catch (err) {
        console.error('Error loading DNS records:', err);
        showError('Failed to load DNS records.');
    } finally {
        setLoadingState(false);
    }
}

function renderTable() {
    const showDeleted = document.getElementById('showDeleted').checked;
    const records = showDeleted ? [...activeRecords, ...deletedRecords] : activeRecords;

    records.sort((a, b) => {
        const tc = (a.type || '').localeCompare(b.type || '');
        return tc !== 0 ? tc : (a.name || '').localeCompare(b.name || '');
    });

    document.getElementById('recordCount').textContent = records.length;
    const tbody = document.getElementById('recordsTableBody');

    if (!records.length) {
        document.getElementById('recordsTableWrapper').classList.add('d-none');
        document.getElementById('noRecordsMsg').classList.remove('d-none');
        return;
    }

    document.getElementById('noRecordsMsg').classList.add('d-none');
    document.getElementById('recordsTableWrapper').classList.remove('d-none');

    tbody.innerHTML = records.map(r => {        const typeBadge = getDnsTypeBadge(r.type);
        const syncBadge = r.isPendingSync
            ? '<span class="badge bg-warning text-dark" title="Pending sync"><i class="bi bi-arrow-repeat"></i> Pending</span>'
            : '<span class="badge bg-success" title="Synced"><i class="bi bi-check2"></i> Synced</span>';
        const deletedBadge = r.isDeleted
            ? ' <span class="badge bg-danger" title="Soft deleted"><i class="bi bi-trash"></i></span>'
            : '';
        const lockBadge = !r.isEditableByUser
            ? ' <span class="badge bg-secondary" title="System-managed"><i class="bi bi-lock"></i></span>'
            : '';

        const extras = [];
        if (r.priority !== null && r.priority !== undefined) extras.push(`P:${r.priority}`);
        if (r.weight !== null && r.weight !== undefined) extras.push(`W:${r.weight}`);
        if (r.port !== null && r.port !== undefined) extras.push(`Port:${r.port}`);

        const rowClass = r.isDeleted ? 'table-secondary' : '';
        return `<tr class="${rowClass}" data-id="${r.id}">
            <td>${typeBadge}</td>
            <td><code>${esc(getFullDnsName(r.name, selectedDomainName))}</code></td>
            <td class="text-break" style="max-width:250px"><code class="small">${esc(r.value || '-')}</code></td>
            <td>${r.ttl ?? '-'}</td>
            <td class="text-nowrap small text-muted">${extras.join(' ') || '-'}</td>
            <td class="text-nowrap">${syncBadge}${deletedBadge}${lockBadge}</td>
            <td class="text-nowrap">${buildActionButtons(r)}</td>
        </tr>`;
    }).join('');

    updatePendingSyncBadge();
}

function buildActionButtons(r) {
    if (r.isDeleted) {
        const fullName = getFullDnsName(r.name, selectedDomainName);
        return `
            <button class="btn btn-sm btn-outline-success" onclick="restoreRecord(${r.id})" title="Restore record">
                <i class="bi bi-arrow-counterclockwise"></i>
            </button>
            <button class="btn btn-sm btn-outline-danger ms-1" onclick="promptHardDelete(${r.id}, '${esc(r.type)} ${esc(fullName)}')" title="Hard delete (permanent)">
                <i class="bi bi-trash-fill"></i>
            </button>`;
    }

    const editBtn = r.isEditableByUser
        ? `<button class="btn btn-sm btn-outline-primary" onclick="openEditModal(${r.id})" title="Edit record"><i class="bi bi-pencil"></i></button>`
        : `<button class="btn btn-sm btn-outline-secondary" disabled title="System-managed, not editable"><i class="bi bi-pencil"></i></button>`;

    const syncBtn = r.isPendingSync
        ? `<button class="btn btn-sm btn-outline-info ms-1" onclick="markSynced(${r.id})" title="Mark as synced"><i class="bi bi-check2-circle"></i></button>`
        : '';

    const fullName = getFullDnsName(r.name, selectedDomainName);
    const deleteBtn = `<button class="btn btn-sm btn-outline-warning ms-1" onclick="promptSoftDelete(${r.id}, '${esc(r.type)} ${esc(fullName)}')" title="Soft delete"><i class="bi bi-trash"></i></button>`;

    return editBtn + syncBtn + deleteBtn;
}

function getDnsTypeBadge(type) {
    const colors = {
        'A': 'primary', 'AAAA': 'primary', 'CNAME': 'info',
        'MX': 'success', 'TXT': 'warning', 'NS': 'secondary',
        'SRV': 'dark', 'CAA': 'danger', 'PTR': 'light'
    };
    const color = colors[type] || 'secondary';
    const textClass = (color === 'warning' || color === 'light') ? ' text-dark' : '';
    return `<span class="badge bg-${color}${textClass}">${esc(type || '?')}</span>`;
}

function openCreateModal() {
    editingRecordId = null;
    document.getElementById('recordModalTitle').textContent = 'Add DNS Record';
    document.getElementById('recordForm').reset();
    document.getElementById('recordId').value = '';
    document.getElementById('recordTTL').value = 3600;
    document.getElementById('modalAlertError').classList.add('d-none');
    populateTypeDropdown(null);
    updateFieldVisibility();
    updateDomainSuffix();
    new bootstrap.Modal(document.getElementById('recordModal')).show();
}

function openEditModal(id) {
    editingRecordId = id;
    const record = [...activeRecords, ...deletedRecords].find(r => r.id === id);
    if (!record) {
        showError('Record not found. Please refresh the page.');
        return;
    }

    document.getElementById('recordModalTitle').textContent = 'Edit DNS Record';
    document.getElementById('recordId').value = record.id;
    document.getElementById('modalAlertError').classList.add('d-none');
    populateTypeDropdown(record.dnsRecordTypeId);
    document.getElementById('recordName').value = record.name || '';
    document.getElementById('recordValue').value = record.value || '';
    document.getElementById('recordTTL').value = record.ttl ?? 3600;
    document.getElementById('recordPriority').value = record.priority ?? '';
    document.getElementById('recordWeight').value = record.weight ?? '';
    document.getElementById('recordPort').value = record.port ?? '';
    updateFieldVisibility();
    updateDomainSuffix();

    new bootstrap.Modal(document.getElementById('recordModal')).show();
}

function populateTypeDropdown(selectedTypeId) {
    const select = document.getElementById('recordType');
    select.innerHTML = '<option value="">-- Select type --</option>';
    dnsRecordTypes.forEach(t => {
        const opt = document.createElement('option');
        opt.value = t.id;
        opt.textContent = t.type + (t.description ? ` \u2014 ${t.description}` : '');
        opt.dataset.hasPriority = t.hasPriority;
        opt.dataset.hasWeight = t.hasWeight;
        opt.dataset.hasPort = t.hasPort;
        opt.dataset.defaultTtl = t.defaultTTL;
        if (t.id === selectedTypeId) opt.selected = true;
        select.appendChild(opt);
    });
}

function updateFieldVisibility() {
    const select = document.getElementById('recordType');
    const opt = select.options[select.selectedIndex];
    const hasPriority = opt?.dataset.hasPriority === 'true';
    const hasWeight = opt?.dataset.hasWeight === 'true';
    const hasPort = opt?.dataset.hasPort === 'true';

    document.getElementById('priorityField').classList.toggle('d-none', !hasPriority);
    document.getElementById('weightField').classList.toggle('d-none', !hasWeight);
    document.getElementById('portField').classList.toggle('d-none', !hasPort);

    if (!editingRecordId && opt?.dataset.defaultTtl) {
        document.getElementById('recordTTL').value = opt.dataset.defaultTtl;
    }
}

async function saveRecord() {
    const typeId = parseInt(document.getElementById('recordType').value);
    const name = document.getElementById('recordName').value.trim();
    const value = document.getElementById('recordValue').value.trim();
    const ttl = parseInt(document.getElementById('recordTTL').value) || 3600;
    const priorityVal = document.getElementById('recordPriority').value;
    const weightVal = document.getElementById('recordWeight').value;
    const portVal = document.getElementById('recordPort').value;

    if (!typeId || !name || !value) {
        document.getElementById('modalAlertError').textContent = 'Please fill in all required fields: Type, Name, and Value.';
        document.getElementById('modalAlertError').classList.remove('d-none');
        return;
    }

    const payload = {
        domainId: parseInt(selectedDomainId),
        dnsRecordTypeId: typeId,
        name,
        value,
        ttl,
        priority: priorityVal !== '' ? parseInt(priorityVal) : null,
        weight: weightVal !== '' ? parseInt(weightVal) : null,
        port: portVal !== '' ? parseInt(portVal) : null,
    };

    const btn = document.getElementById('saveRecordBtn');
    btn.disabled = true;
    btn.innerHTML = '<span class="spinner-border spinner-border-sm"></span> Saving...';

    try {
        let result;
        if (editingRecordId) {
            result = await window.DnsRecordAPI.update(editingRecordId, payload);
        } else {
            result = await window.DnsRecordAPI.create(payload);
        }

        if (result.success) {
            bootstrap.Modal.getInstance(document.getElementById('recordModal')).hide();
            showSuccess(editingRecordId ? 'DNS record updated successfully.' : 'DNS record created successfully.');
            await loadRecords();
        } else {
            document.getElementById('modalAlertError').textContent = result.message || 'Failed to save record.';
            document.getElementById('modalAlertError').classList.remove('d-none');
        }
    } catch (err) {
        document.getElementById('modalAlertError').textContent = 'An error occurred while saving.';
        document.getElementById('modalAlertError').classList.remove('d-none');
        console.error(err);
    } finally {
        btn.disabled = false;
        btn.innerHTML = '<i class="bi bi-save"></i> Save Record';
    }
}

function promptSoftDelete(id, desc) {
    recordToDelete = id;
    document.getElementById('deleteRecordDesc').textContent = desc;
    new bootstrap.Modal(document.getElementById('deleteModal')).show();
}

async function performSoftDelete(id) {
    try {
        const result = await window.DnsRecordAPI.softDelete(id);
        bootstrap.Modal.getInstance(document.getElementById('deleteModal')).hide();
        if (result.success) {
            showSuccess('DNS record marked for deletion (soft-deleted).');
            await loadRecords();
        } else {
            showError(result.message || 'Failed to delete record.');
        }
    } catch (err) {
        showError('Error deleting record.');
        console.error(err);
    } finally {
        recordToDelete = null;
    }
}

function promptHardDelete(id, desc) {
    recordToHardDelete = id;
    document.getElementById('hardDeleteRecordDesc').textContent = desc;
    new bootstrap.Modal(document.getElementById('hardDeleteModal')).show();
}

async function performHardDelete(id) {
    try {
        const result = await window.DnsRecordAPI.hardDelete(id);
        bootstrap.Modal.getInstance(document.getElementById('hardDeleteModal')).hide();
        if (result.success) {
            showSuccess('DNS record permanently deleted.');
            await loadRecords();
        } else {
            showError(result.message || 'Failed to permanently delete record.');
        }
    } catch (err) {
        showError('Error permanently deleting record.');
        console.error(err);
    } finally {
        recordToHardDelete = null;
    }
}

async function restoreRecord(id) {
    try {
        const result = await window.DnsRecordAPI.restore(id);
        if (result.success) {
            showSuccess('DNS record restored successfully.');
            await loadRecords();
        } else {
            showError(result.message || 'Failed to restore record.');
        }
    } catch (err) {
        showError('Error restoring record.');
        console.error(err);
    }
}

async function markSynced(id) {
    try {
        const result = await window.DnsRecordAPI.markSynced(id);
        if (result.success) {
            showSuccess('DNS record marked as synced.');
            await loadRecords();
        } else {
            showError(result.message || 'Failed to mark record as synced.');
        }
    } catch (err) {
        showError('Error marking record as synced.');
        console.error(err);
    }
}

function setLoadingState(show) {
    document.getElementById('loadingRecords').classList.toggle('d-none', !show);
    document.getElementById('noSelectionMsg').classList.add('d-none');
    if (show) {
        document.getElementById('recordsTableWrapper').classList.add('d-none');
        document.getElementById('noRecordsMsg').classList.add('d-none');
    }
}

function showNoSelection() {
    document.getElementById('noSelectionMsg').classList.remove('d-none');
    document.getElementById('loadingRecords').classList.add('d-none');
    document.getElementById('recordsTableWrapper').classList.add('d-none');
    document.getElementById('noRecordsMsg').classList.add('d-none');
    document.getElementById('recordCount').textContent = '0';
    document.getElementById('selectedDomainLabel').textContent = 'No domain selected';
    document.getElementById('syncToServerBtn').disabled = true;
    document.getElementById('pendingSyncCount').classList.add('d-none');
    activeRecords = [];
    deletedRecords = [];
    selectedDomainName = '';
}

function updatePendingSyncBadge() {
    const badge = document.getElementById('pendingSyncCount');
    const count = activeRecords.filter(r => r.isPendingSync).length;
    if (count > 0) {
        badge.textContent = count;
        badge.classList.remove('d-none');
    } else {
        badge.classList.add('d-none');
    }
}

async function openSyncModal() {
    const syncModal = new bootstrap.Modal(document.getElementById('syncModal'));

    document.getElementById('syncModalLoading').classList.remove('d-none');
    document.getElementById('syncModalEmpty').classList.add('d-none');
    document.getElementById('syncModalContent').classList.add('d-none');
    document.getElementById('syncProgressWrapper').classList.add('d-none');
    document.getElementById('syncSummary').classList.add('d-none');
    document.getElementById('confirmSyncBtn').disabled = true;
    document.getElementById('confirmSyncBtn').innerHTML = '<i class="bi bi-arrow-repeat"></i> Sync All to DNS Server';
    document.getElementById('syncCancelBtn').disabled = false;
    document.getElementById('syncCancelBtn').textContent = 'Cancel';

    const bar = document.getElementById('syncProgressBar');
    bar.style.width = '0%';
    bar.className = 'progress-bar progress-bar-striped progress-bar-animated bg-warning';

    syncModal.show();

    try {
        const result = await window.DnsRecordAPI.getPendingSyncByDomain(selectedDomainId);
        pendingRecords = (result.success && Array.isArray(result.data)) ? result.data : [];
    } catch (err) {
        console.error('Failed to load pending-sync records:', err);
        pendingRecords = [];
    }

    document.getElementById('syncModalLoading').classList.add('d-none');

    if (pendingRecords.length === 0) {
        document.getElementById('syncModalEmpty').classList.remove('d-none');
    } else {
        document.getElementById('syncPendingCount').textContent = pendingRecords.length;
        renderSyncTable();
        document.getElementById('syncModalContent').classList.remove('d-none');
        document.getElementById('confirmSyncBtn').disabled = false;
    }
}

function renderSyncTable() {
    const tbody = document.getElementById('syncRecordsTableBody');
    tbody.innerHTML = pendingRecords.map(r => `
        <tr id="sync-row-${r.id}">
            <td>${getDnsTypeBadge(r.type)}</td>
            <td><code>${esc(getFullDnsName(r.name, selectedDomainName))}</code></td>
            <td class="text-break"><code class="small">${esc(r.value || '-')}</code></td>
            <td>${r.ttl ?? '-'}</td>
            <td><span class="badge bg-warning text-dark"><i class="bi bi-clock"></i> Pending</span></td>
        </tr>
    `).join('');
}

async function performSync() {
    const confirmBtn = document.getElementById('confirmSyncBtn');
    const cancelBtn = document.getElementById('syncCancelBtn');
    const progressWrapper = document.getElementById('syncProgressWrapper');
    const progressBar = document.getElementById('syncProgressBar');
    const progressText = document.getElementById('syncProgressText');
    const summary = document.getElementById('syncSummary');

    confirmBtn.disabled = true;
    confirmBtn.innerHTML = '<span class="spinner-border spinner-border-sm"></span> Syncing...';
    cancelBtn.disabled = true;
    progressWrapper.classList.remove('d-none');
    summary.classList.add('d-none');

    let succeeded = 0;
    let failed = 0;
    const total = pendingRecords.length;

    for (let i = 0; i < total; i++) {
        const record = pendingRecords[i];
        const pct = Math.round(((i + 1) / total) * 100);
        progressText.textContent = `${i + 1} / ${total}`;
        progressBar.style.width = `${pct}%`;
        progressBar.setAttribute('aria-valuenow', pct);

        const row = document.getElementById(`sync-row-${record.id}`);
        try {
            const result = await window.DnsRecordAPI.push(record.id);
            if (result.success) {
                succeeded++;
                if (row) row.cells[4].innerHTML = '<span class="badge bg-success"><i class="bi bi-check2"></i> Synced</span>';
            } else {
                failed++;
                const errMsg = (result.data?.message || result.message || 'Failed').substring(0, 80);
                if (row) row.cells[4].innerHTML = `<span class="badge bg-danger" title="${esc(errMsg)}"><i class="bi bi-x"></i> Failed</span>`;
            }
        } catch (err) {
            failed++;
            if (row) row.cells[4].innerHTML = '<span class="badge bg-danger"><i class="bi bi-x"></i> Error</span>';
            console.error(`Error pushing record ${record.id}:`, err);
        }
    }

    progressBar.classList.remove('progress-bar-animated');

    if (failed === 0) {
        progressBar.classList.replace('bg-warning', 'bg-success');
        summary.className = 'mt-3 alert alert-success';
        summary.innerHTML = `<i class="bi bi-check-circle-fill"></i> All <strong>${succeeded}</strong> record(s) were successfully marked as synced.`;
    } else {
        summary.className = 'mt-3 alert alert-warning';
        summary.innerHTML = `<i class="bi bi-exclamation-triangle-fill"></i> <strong>${succeeded}</strong> record(s) synced, <strong>${failed}</strong> failed. Check the table above for details.`;
    }
    summary.classList.remove('d-none');

    confirmBtn.innerHTML = '<i class="bi bi-check2-circle"></i> Done';
    cancelBtn.disabled = false;
    cancelBtn.textContent = 'Close';

    await loadRecords();
}

function getFullDnsName(name, domainName) {
    if (!name || name === '@') return domainName || '@';
    if (!domainName) return name;
    if (name.endsWith('.' + domainName) || name === domainName) return name;
    return `${name}.${domainName}`;
}

function updateDomainSuffix() {
    const suffix = document.getElementById('recordNameSuffix');
    if (suffix) {
        suffix.textContent = selectedDomainName ? `.${selectedDomainName}` : '';
    }
}

function esc(text) {
    const map = { '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#039;' };
    return (text || '').toString().replace(/[&<>"']/g, m => map[m]);
}

function showSuccess(msg) {
    document.getElementById('alertSuccessMsg').textContent = msg;
    document.getElementById('alertSuccess').classList.remove('d-none');
    document.getElementById('alertError').classList.add('d-none');
    setTimeout(() => document.getElementById('alertSuccess').classList.add('d-none'), 6000);
}

function showError(msg) {
    document.getElementById('alertErrorMsg').textContent = msg;
    document.getElementById('alertError').classList.remove('d-none');
    document.getElementById('alertSuccess').classList.add('d-none');
}

window.openEditModal = openEditModal;
window.promptSoftDelete = promptSoftDelete;
window.promptHardDelete = promptHardDelete;
window.restoreRecord = restoreRecord;
window.markSynced = markSynced;
window.openSyncModal = openSyncModal;
window.performSync = performSync;

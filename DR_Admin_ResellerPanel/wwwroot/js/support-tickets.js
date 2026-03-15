"use strict";
(function () {
    const typedWindow = window;
    let items = [];
    let currentPage = 1;
    let pageSize = 25;
    let totalPages = 1;
    let totalCount = 0;
    let selectedTicketId = null;
    let autoRefreshHandle = null;
    let isAutoRefreshing = false;
    function getApiBaseUrl() {
        return typedWindow.AppSettings?.apiBaseUrl ?? '';
    }
    function getAuthToken() {
        if (typedWindow.Auth?.getToken) {
            return typedWindow.Auth.getToken();
        }
        return sessionStorage.getItem('rp_authToken');
    }
    async function apiRequest(endpoint, options = {}) {
        try {
            const headers = new Headers(options.headers ?? {});
            headers.set('Content-Type', 'application/json');
            const authToken = getAuthToken();
            if (authToken) {
                headers.set('Authorization', `Bearer ${authToken}`);
            }
            const response = await fetch(endpoint, {
                ...options,
                headers,
                credentials: 'include'
            });
            const contentType = response.headers.get('content-type') ?? '';
            const hasJson = contentType.includes('application/json');
            const data = hasJson ? await response.json() : null;
            if (!response.ok) {
                const payload = data;
                return {
                    success: false,
                    message: payload?.message ?? payload?.title ?? `Request failed with status ${response.status}`
                };
            }
            return {
                success: true,
                data: data
            };
        }
        catch {
            return {
                success: false,
                message: 'Network error. Please try again.'
            };
        }
    }
    function normalizeTicket(item) {
        const raw = item;
        return {
            id: Number(raw.id ?? raw.Id ?? 0),
            customerId: Number(raw.customerId ?? raw.CustomerId ?? 0),
            customerName: String(raw.customerName ?? raw.CustomerName ?? ''),
            subject: String(raw.subject ?? raw.Subject ?? ''),
            description: String(raw.description ?? raw.Description ?? ''),
            status: String(raw.status ?? raw.Status ?? ''),
            priority: String(raw.priority ?? raw.Priority ?? ''),
            updatedAt: String(raw.updatedAt ?? raw.UpdatedAt ?? ''),
            lastMessageAt: (raw.lastMessageAt ?? raw.LastMessageAt ?? null),
            messages: normalizeMessages(raw.messages ?? raw.Messages)
        };
    }
    function normalizeMessages(value) {
        if (!Array.isArray(value)) {
            return [];
        }
        return value.map((item) => {
            const raw = item;
            return {
                id: Number(raw.id ?? raw.Id ?? 0),
                senderUserId: Number(raw.senderUserId ?? raw.SenderUserId ?? 0),
                senderUsername: String(raw.senderUsername ?? raw.SenderUsername ?? ''),
                senderRole: String(raw.senderRole ?? raw.SenderRole ?? ''),
                message: String(raw.message ?? raw.Message ?? ''),
                createdAt: String(raw.createdAt ?? raw.CreatedAt ?? '')
            };
        });
    }
    function getStatusFilter() {
        const status = document.getElementById('reseller-support-status-filter');
        return status?.value ?? '';
    }
    function buildTicketsUrl() {
        const params = new URLSearchParams();
        params.set('pageNumber', String(currentPage));
        params.set('pageSize', String(pageSize));
        const status = getStatusFilter();
        if (status) {
            params.set('status', status);
        }
        return `${getApiBaseUrl()}/SupportTickets?${params.toString()}`;
    }
    async function loadTickets() {
        const tableBody = document.getElementById('reseller-support-table-body');
        if (!tableBody) {
            return;
        }
        tableBody.innerHTML = '<tr><td colspan="7" class="text-center"><div class="spinner-border text-primary"></div></td></tr>';
        hideAlerts();
        const response = await apiRequest(buildTicketsUrl(), { method: 'GET' });
        if (!response.success || !response.data) {
            showError(response.message ?? 'Failed to load support tickets.');
            tableBody.innerHTML = '<tr><td colspan="7" class="text-center text-danger">Failed to load tickets.</td></tr>';
            return;
        }
        const result = response.data;
        const listCandidate = Array.isArray(result.data)
            ? result.data
            : Array.isArray(result.Data)
                ? result.Data
                : Array.isArray(response.data)
                    ? response.data
                    : [];
        items = listCandidate.map((item) => normalizeTicket(item));
        totalCount = Number(result.totalCount ?? result.TotalCount ?? items.length);
        currentPage = Number(result.currentPage ?? result.CurrentPage ?? currentPage);
        pageSize = Number(result.pageSize ?? result.PageSize ?? pageSize);
        totalPages = Math.max(1, Number(result.totalPages ?? result.TotalPages ?? Math.ceil(totalCount / Math.max(pageSize, 1))));
        renderTable();
        renderPagination();
        if (selectedTicketId !== null) {
            const stillExists = items.some((ticket) => ticket.id === selectedTicketId);
            if (!stillExists) {
                selectedTicketId = null;
                clearDetailPane();
            }
        }
    }
    function renderTable() {
        const tableBody = document.getElementById('reseller-support-table-body');
        if (!tableBody) {
            return;
        }
        if (items.length === 0) {
            tableBody.innerHTML = '<tr><td colspan="7" class="text-center text-muted">No tickets found.</td></tr>';
            return;
        }
        tableBody.innerHTML = items.map((ticket) => {
            const selectedClass = selectedTicketId === ticket.id ? 'table-primary' : '';
            return `<tr class="${selectedClass}">
            <td>${ticket.id}</td>
            <td>${escapeHtml(ticket.customerName)} (#${ticket.customerId})</td>
            <td>${escapeHtml(ticket.subject)}</td>
            <td>${renderStatusBadge(ticket.status)}</td>
            <td>${escapeHtml(ticket.priority)}</td>
            <td>${formatDate(ticket.lastMessageAt ?? ticket.updatedAt)}</td>
            <td><button type="button" class="btn btn-outline-primary btn-sm" data-action="open" data-id="${ticket.id}">Open</button></td>
        </tr>`;
        }).join('');
    }
    function renderPagination() {
        const info = document.getElementById('reseller-support-pagination-info');
        const list = document.getElementById('reseller-support-pagination-list');
        if (!info || !list) {
            return;
        }
        if (totalCount === 0) {
            info.textContent = 'Showing 0 of 0';
            list.innerHTML = '';
            return;
        }
        const start = (currentPage - 1) * pageSize + 1;
        const end = Math.min(totalCount, currentPage * pageSize);
        info.textContent = `Showing ${start}-${end} of ${totalCount}`;
        const previousDisabled = currentPage <= 1;
        const nextDisabled = currentPage >= totalPages;
        list.innerHTML = `
        <li class="page-item ${previousDisabled ? 'disabled' : ''}"><a class="page-link" href="#" data-page="${currentPage - 1}">Previous</a></li>
        <li class="page-item disabled"><span class="page-link">Page ${currentPage} / ${totalPages}</span></li>
        <li class="page-item ${nextDisabled ? 'disabled' : ''}"><a class="page-link" href="#" data-page="${currentPage + 1}">Next</a></li>`;
    }
    function renderStatusBadge(status) {
        const normalized = status.toLowerCase();
        if (normalized === 'closed') {
            return '<span class="badge bg-secondary">Closed</span>';
        }
        if (normalized === 'inprogress') {
            return '<span class="badge bg-primary">In progress</span>';
        }
        if (normalized === 'waitingforcustomer') {
            return '<span class="badge bg-warning text-dark">Waiting for customer</span>';
        }
        return '<span class="badge bg-success">Open</span>';
    }
    async function openTicket(ticketId) {
        hideAlerts();
        const response = await apiRequest(`${getApiBaseUrl()}/SupportTickets/${ticketId}`, { method: 'GET' });
        if (!response.success || !response.data) {
            showError(response.message ?? 'Could not load ticket details.');
            return;
        }
        const ticket = normalizeTicket(response.data);
        selectedTicketId = ticket.id;
        renderTable();
        renderDetails(ticket);
    }
    function renderDetails(ticket) {
        const title = document.getElementById('reseller-support-details-title');
        const thread = document.getElementById('reseller-support-thread');
        const status = document.getElementById('reseller-support-status');
        const reply = document.getElementById('reseller-support-reply');
        const send = document.getElementById('reseller-support-send');
        if (title) {
            title.textContent = `Ticket #${ticket.id}: ${ticket.subject}`;
        }
        if (status) {
            status.disabled = false;
            status.value = ticket.status;
            status.dataset.ticketId = String(ticket.id);
        }
        if (reply) {
            reply.disabled = true;
            reply.value = '';
            reply.placeholder = 'Replies moved to communication system.';
        }
        if (send) {
            send.disabled = true;
        }
        if (!thread) {
            return;
        }
        const description = escapeHtml(ticket.description).replace(/\n/g, '<br />');
        thread.innerHTML = `
        <div class="mb-3">
            <div class="small text-muted"><strong>Description</strong></div>
            <div class="mt-1">${description || '<span class="text-muted">No description</span>'}</div>
        </div>
        <p class="text-muted mb-0">Replies are deprecated for support tickets. Use communication endpoints.</p>`;
    }
    function clearDetailPane() {
        const title = document.getElementById('reseller-support-details-title');
        const thread = document.getElementById('reseller-support-thread');
        const status = document.getElementById('reseller-support-status');
        const reply = document.getElementById('reseller-support-reply');
        const send = document.getElementById('reseller-support-send');
        if (title) {
            title.textContent = 'Ticket details';
        }
        if (thread) {
            thread.innerHTML = '<p class="text-muted mb-0">Select a ticket to show conversation.</p>';
        }
        if (status) {
            status.disabled = true;
            delete status.dataset.ticketId;
        }
        if (reply) {
            reply.value = '';
            reply.disabled = true;
        }
        if (send) {
            send.disabled = true;
        }
    }
    async function updateStatus(newStatus) {
        const ticketId = selectedTicketId;
        if (!ticketId) {
            return;
        }
        const response = await apiRequest(`${getApiBaseUrl()}/SupportTickets/${ticketId}/status`, {
            method: 'PATCH',
            body: JSON.stringify({ status: newStatus })
        });
        if (!response.success) {
            showError(response.message ?? 'Could not update status.');
            return;
        }
        showSuccess('Ticket status updated.');
        await loadTickets();
        await openTicket(ticketId);
    }
    async function sendReply(event) {
        event.preventDefault();
        showError('Ticket replies are deprecated. Use communication endpoints.');
    }
    function onPaginationClick(event) {
        const target = event.target;
        const link = target.closest('a[data-page]');
        if (!link) {
            return;
        }
        event.preventDefault();
        const nextPage = Number(link.dataset.page ?? '0');
        if (!Number.isFinite(nextPage) || nextPage < 1 || nextPage > totalPages || nextPage === currentPage) {
            return;
        }
        currentPage = nextPage;
        void loadTickets();
    }
    function bindEvents() {
        document.getElementById('reseller-support-refresh')?.addEventListener('click', () => {
            void loadTickets();
        });
        document.getElementById('reseller-support-apply')?.addEventListener('click', () => {
            currentPage = 1;
            void loadTickets();
        });
        document.getElementById('reseller-support-page-size')?.addEventListener('change', () => {
            const input = document.getElementById('reseller-support-page-size');
            const parsed = Number(input?.value ?? '25');
            pageSize = Number.isFinite(parsed) && parsed > 0 ? parsed : 25;
            currentPage = 1;
            void loadTickets();
        });
        document.getElementById('reseller-support-table-body')?.addEventListener('click', (event) => {
            const target = event.target;
            const button = target.closest('button[data-action="open"]');
            if (!button) {
                return;
            }
            const ticketId = Number(button.dataset.id ?? '0');
            if (!Number.isFinite(ticketId) || ticketId <= 0) {
                return;
            }
            void openTicket(ticketId);
        });
        document.getElementById('reseller-support-reply-form')?.addEventListener('submit', (event) => {
            void sendReply(event);
        });
        document.getElementById('reseller-support-status')?.addEventListener('change', (event) => {
            const select = event.target;
            void updateStatus(select.value);
        });
        document.getElementById('reseller-support-pagination-list')?.addEventListener('click', onPaginationClick);
    }
    function showSuccess(message) {
        const success = document.getElementById('reseller-support-alert-success');
        const error = document.getElementById('reseller-support-alert-error');
        if (success) {
            success.textContent = message;
            success.classList.remove('d-none');
        }
        error?.classList.add('d-none');
    }
    function showError(message) {
        const success = document.getElementById('reseller-support-alert-success');
        const error = document.getElementById('reseller-support-alert-error');
        if (error) {
            error.textContent = message;
            error.classList.remove('d-none');
        }
        success?.classList.add('d-none');
    }
    function hideAlerts() {
        document.getElementById('reseller-support-alert-success')?.classList.add('d-none');
        document.getElementById('reseller-support-alert-error')?.classList.add('d-none');
    }
    function formatDate(value) {
        const date = new Date(value);
        if (Number.isNaN(date.getTime())) {
            return '-';
        }
        return date.toLocaleString();
    }
    function escapeHtml(value) {
        return value
            .replace(/&/g, '&amp;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;')
            .replace(/'/g, '&#039;');
    }
    function initializePage() {
        const page = document.getElementById('reseller-support-tickets-page');
        if (!page || page.dataset.initialized === 'true') {
            return;
        }
        page.dataset.initialized = 'true';
        bindEvents();
        clearDetailPane();
        void loadTickets();
    }
    function setupObserver() {
        initializePage();
        const observer = new MutationObserver(() => {
            initializePage();
        });
        observer.observe(document.body, { childList: true, subtree: true });
    }
    function startAutoRefresh() {
        if (autoRefreshHandle !== null) {
            return;
        }
        autoRefreshHandle = window.setInterval(() => {
            if (document.hidden || isAutoRefreshing) {
                return;
            }
            const page = document.getElementById('reseller-support-tickets-page');
            if (!page || page.dataset.initialized !== 'true') {
                return;
            }
            isAutoRefreshing = true;
            void loadTickets()
                .then(async () => {
                if (selectedTicketId !== null) {
                    await openTicket(selectedTicketId);
                }
            })
                .finally(() => {
                isAutoRefreshing = false;
            });
        }, 30000);
    }
    document.addEventListener('visibilitychange', () => {
        if (!document.hidden) {
            const page = document.getElementById('reseller-support-tickets-page');
            if (!page || page.dataset.initialized !== 'true') {
                return;
            }
            void loadTickets().then(async () => {
                if (selectedTicketId !== null) {
                    await openTicket(selectedTicketId);
                }
            });
        }
    });
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', () => {
            setupObserver();
            startAutoRefresh();
        });
    }
    else {
        setupObserver();
        startAutoRefresh();
    }
})();
//# sourceMappingURL=support-tickets.js.map
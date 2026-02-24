var __assign = (this && this.__assign) || function () {
    __assign = Object.assign || function(t) {
        for (var s, i = 1, n = arguments.length; i < n; i++) {
            s = arguments[i];
            for (var p in s) if (Object.prototype.hasOwnProperty.call(s, p))
                t[p] = s[p];
        }
        return t;
    };
    return __assign.apply(this, arguments);
};
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
var __generator = (this && this.__generator) || function (thisArg, body) {
    var _ = { label: 0, sent: function() { if (t[0] & 1) throw t[1]; return t[1]; }, trys: [], ops: [] }, f, y, t, g = Object.create((typeof Iterator === "function" ? Iterator : Object).prototype);
    return g.next = verb(0), g["throw"] = verb(1), g["return"] = verb(2), typeof Symbol === "function" && (g[Symbol.iterator] = function() { return this; }), g;
    function verb(n) { return function (v) { return step([n, v]); }; }
    function step(op) {
        if (f) throw new TypeError("Generator is already executing.");
        while (g && (g = 0, op[0] && (_ = 0)), _) try {
            if (f = 1, y && (t = op[0] & 2 ? y["return"] : op[0] ? y["throw"] || ((t = y["return"]) && t.call(y), 0) : y.next) && !(t = t.call(y, op[1])).done) return t;
            if (y = 0, t) op = [op[0] & 2, t.value];
            switch (op[0]) {
                case 0: case 1: t = op; break;
                case 4: _.label++; return { value: op[1], done: false };
                case 5: _.label++; y = op[1]; op = [0]; continue;
                case 7: op = _.ops.pop(); _.trys.pop(); continue;
                default:
                    if (!(t = _.trys, t = t.length > 0 && t[t.length - 1]) && (op[0] === 6 || op[0] === 2)) { _ = 0; continue; }
                    if (op[0] === 3 && (!t || (op[1] > t[0] && op[1] < t[3]))) { _.label = op[1]; break; }
                    if (op[0] === 6 && _.label < t[1]) { _.label = t[1]; t = op; break; }
                    if (t && _.label < t[2]) { _.label = t[2]; _.ops.push(op); break; }
                    if (t[2]) _.ops.pop();
                    _.trys.pop(); continue;
            }
            op = body.call(thisArg, _);
        } catch (e) { op = [6, e]; y = 0; } finally { f = t = 0; }
        if (op[0] & 5) throw op[1]; return { value: op[0] ? op[1] : void 0, done: true };
    }
};
// @ts-nocheck
(function () {
    function getApiBaseUrl() {
        var _a, _b;
        return (_b = (_a = window.AppSettings) === null || _a === void 0 ? void 0 : _a.apiBaseUrl) !== null && _b !== void 0 ? _b : '';
    }
    var allServerIpAddresses = [];
    var servers = [];
    var editingId = null;
    var pendingDeleteId = null;
    function getAuthToken() {
        var auth = window.Auth;
        if (auth === null || auth === void 0 ? void 0 : auth.getToken) {
            return auth.getToken();
        }
        return sessionStorage.getItem('rp_authToken');
    }
    function apiRequest(endpoint_1) {
        return __awaiter(this, arguments, void 0, function (endpoint, options) {
            var headers, authToken, response, contentType, hasJson, data, _a, error_1;
            var _b, _c, _d;
            if (options === void 0) { options = {}; }
            return __generator(this, function (_e) {
                switch (_e.label) {
                    case 0:
                        _e.trys.push([0, 5, , 6]);
                        headers = __assign({ 'Content-Type': 'application/json' }, options.headers);
                        authToken = getAuthToken();
                        if (authToken) {
                            headers['Authorization'] = "Bearer ".concat(authToken);
                        }
                        return [4 /*yield*/, fetch(endpoint, __assign(__assign({}, options), { headers: headers, credentials: 'include' }))];
                    case 1:
                        response = _e.sent();
                        contentType = (_b = response.headers.get('content-type')) !== null && _b !== void 0 ? _b : '';
                        hasJson = contentType.includes('application/json');
                        if (!hasJson) return [3 /*break*/, 3];
                        return [4 /*yield*/, response.json()];
                    case 2:
                        _a = _e.sent();
                        return [3 /*break*/, 4];
                    case 3:
                        _a = null;
                        _e.label = 4;
                    case 4:
                        data = _a;
                        if (!response.ok) {
                            return [2 /*return*/, {
                                    success: false,
                                    message: (data && ((_c = data.message) !== null && _c !== void 0 ? _c : data.title)) || "Request failed with status ".concat(response.status),
                                }];
                        }
                        return [2 /*return*/, {
                                success: (data === null || data === void 0 ? void 0 : data.success) !== false,
                                data: (_d = data === null || data === void 0 ? void 0 : data.data) !== null && _d !== void 0 ? _d : data,
                                message: data === null || data === void 0 ? void 0 : data.message,
                            }];
                    case 5:
                        error_1 = _e.sent();
                        console.error('Server IP addresses request failed', error_1);
                        return [2 /*return*/, {
                                success: false,
                                message: 'Network error. Please try again.',
                            }];
                    case 6: return [2 /*return*/];
                }
            });
        });
    }
    function loadServers() {
        return __awaiter(this, void 0, void 0, function () {
            var response, rawItems;
            var _a;
            return __generator(this, function (_b) {
                switch (_b.label) {
                    case 0: return [4 /*yield*/, apiRequest("".concat(getApiBaseUrl(), "/Servers"), { method: 'GET' })];
                    case 1:
                        response = _b.sent();
                        rawItems = Array.isArray(response.data)
                            ? response.data
                            : Array.isArray((_a = response.data) === null || _a === void 0 ? void 0 : _a.data)
                                ? response.data.data
                                : [];
                        servers = rawItems.map(function (item) {
                            var _a, _b, _c, _d;
                            return ({
                                id: (_b = (_a = item.id) !== null && _a !== void 0 ? _a : item.Id) !== null && _b !== void 0 ? _b : 0,
                                name: (_d = (_c = item.name) !== null && _c !== void 0 ? _c : item.Name) !== null && _d !== void 0 ? _d : '',
                            });
                        });
                        populateServerDropdown();
                        return [2 /*return*/];
                }
            });
        });
    }
    function populateServerDropdown() {
        var serverSelect = document.getElementById('server-ip-addresses-server-id');
        if (serverSelect) {
            var selected = serverSelect.value;
            serverSelect.innerHTML = '<option value="">Select Server...</option>' +
                servers.map(function (s) { return "<option value=\"".concat(s.id, "\">").concat(esc(s.name), "</option>"); }).join('');
            if (selected) {
                serverSelect.value = selected;
            }
        }
    }
    function loadServerIpAddresses() {
        return __awaiter(this, void 0, void 0, function () {
            var tableBody, response, rawItems;
            var _a;
            return __generator(this, function (_b) {
                switch (_b.label) {
                    case 0:
                        tableBody = document.getElementById('server-ip-addresses-table-body');
                        if (!tableBody) {
                            return [2 /*return*/];
                        }
                        tableBody.innerHTML = '<tr><td colspan="8" class="text-center"><div class="spinner-border text-primary"></div></td></tr>';
                        return [4 /*yield*/, apiRequest("".concat(getApiBaseUrl(), "/ServerIpAddresses"), { method: 'GET' })];
                    case 1:
                        response = _b.sent();
                        if (!response.success) {
                            showError(response.message || 'Failed to load IP addresses');
                            tableBody.innerHTML = '<tr><td colspan="8" class="text-center text-danger">Failed to load data</td></tr>';
                            return [2 /*return*/];
                        }
                        rawItems = Array.isArray(response.data)
                            ? response.data
                            : Array.isArray((_a = response.data) === null || _a === void 0 ? void 0 : _a.data)
                                ? response.data.data
                                : [];
                        allServerIpAddresses = rawItems.map(function (item) {
                            var _a, _b, _c, _d, _e, _f, _g, _h, _j, _k, _l, _m, _o, _p, _q, _r, _s, _t;
                            return ({
                                id: (_b = (_a = item.id) !== null && _a !== void 0 ? _a : item.Id) !== null && _b !== void 0 ? _b : 0,
                                serverId: (_d = (_c = item.serverId) !== null && _c !== void 0 ? _c : item.ServerId) !== null && _d !== void 0 ? _d : 0,
                                serverName: (_f = (_e = item.serverName) !== null && _e !== void 0 ? _e : item.ServerName) !== null && _f !== void 0 ? _f : null,
                                ipAddress: (_h = (_g = item.ipAddress) !== null && _g !== void 0 ? _g : item.IpAddress) !== null && _h !== void 0 ? _h : '',
                                ipVersion: (_k = (_j = item.ipVersion) !== null && _j !== void 0 ? _j : item.IpVersion) !== null && _k !== void 0 ? _k : 'IPv4',
                                isPrimary: (_m = (_l = item.isPrimary) !== null && _l !== void 0 ? _l : item.IsPrimary) !== null && _m !== void 0 ? _m : false,
                                status: (_p = (_o = item.status) !== null && _o !== void 0 ? _o : item.Status) !== null && _p !== void 0 ? _p : 'Active',
                                assignedTo: (_r = (_q = item.assignedTo) !== null && _q !== void 0 ? _q : item.AssignedTo) !== null && _r !== void 0 ? _r : null,
                                notes: (_t = (_s = item.notes) !== null && _s !== void 0 ? _s : item.Notes) !== null && _t !== void 0 ? _t : null,
                            });
                        });
                        renderTable();
                        return [2 /*return*/];
                }
            });
        });
    }
    function renderTable() {
        var tableBody = document.getElementById('server-ip-addresses-table-body');
        if (!tableBody) {
            return;
        }
        if (!allServerIpAddresses.length) {
            tableBody.innerHTML = '<tr><td colspan="8" class="text-center text-muted">No IP addresses found. Click "New IP Address" to add one.</td></tr>';
            return;
        }
        tableBody.innerHTML = allServerIpAddresses.map(function (ip) { return "\n        <tr>\n            <td>".concat(ip.id, "</td>\n            <td><code>").concat(esc(ip.ipAddress), "</code></td>\n            <td><span class=\"badge bg-").concat(ip.ipVersion === 'IPv6' ? 'info' : 'secondary', "\">").concat(esc(ip.ipVersion), "</span></td>\n            <td>").concat(esc(ip.serverName || '-'), "</td>\n            <td>").concat(ip.isPrimary ? '<span class="badge bg-primary">Primary</span>' : '-', "</td>\n            <td><span class=\"badge bg-").concat(getStatusBadgeColor(ip.status), "\">").concat(esc(ip.status), "</span></td>\n            <td>").concat(esc(ip.assignedTo || '-'), "</td>\n            <td>\n                <div class=\"btn-group btn-group-sm\">\n                    <button class=\"btn btn-outline-primary\" type=\"button\" data-action=\"edit\" data-id=\"").concat(ip.id, "\" title=\"Edit\"><i class=\"bi bi-pencil\"></i></button>\n                    <button class=\"btn btn-outline-danger\" type=\"button\" data-action=\"delete\" data-id=\"").concat(ip.id, "\" data-name=\"").concat(esc(ip.ipAddress), "\" title=\"Delete\"><i class=\"bi bi-trash\"></i></button>\n                </div>\n            </td>\n        </tr>\n    "); }).join('');
    }
    function getStatusBadgeColor(status) {
        switch (status === null || status === void 0 ? void 0 : status.toLowerCase()) {
            case 'active': return 'success';
            case 'reserved': return 'warning';
            case 'blocked': return 'danger';
            default: return 'secondary';
        }
    }
    function openCreate() {
        editingId = null;
        var modalTitle = document.getElementById('server-ip-addresses-modal-title');
        if (modalTitle) {
            modalTitle.textContent = 'New IP Address';
        }
        var form = document.getElementById('server-ip-addresses-form');
        form === null || form === void 0 ? void 0 : form.reset();
        var ipVersionSelect = document.getElementById('server-ip-addresses-ip-version');
        if (ipVersionSelect) {
            ipVersionSelect.value = 'IPv4';
        }
        var statusSelect = document.getElementById('server-ip-addresses-status');
        if (statusSelect) {
            statusSelect.value = 'Active';
        }
        var isPrimaryInput = document.getElementById('server-ip-addresses-is-primary');
        if (isPrimaryInput) {
            isPrimaryInput.checked = false;
        }
        populateServerDropdown();
        showModal('server-ip-addresses-edit-modal');
    }
    function openEdit(id) {
        var ip = allServerIpAddresses.find(function (item) { return item.id === id; });
        if (!ip) {
            return;
        }
        editingId = id;
        var modalTitle = document.getElementById('server-ip-addresses-modal-title');
        if (modalTitle) {
            modalTitle.textContent = 'Edit IP Address';
        }
        var ipAddressInput = document.getElementById('server-ip-addresses-ip-address');
        var ipVersionInput = document.getElementById('server-ip-addresses-ip-version');
        var serverIdInput = document.getElementById('server-ip-addresses-server-id');
        var statusInput = document.getElementById('server-ip-addresses-status');
        var isPrimaryInput = document.getElementById('server-ip-addresses-is-primary');
        var assignedToInput = document.getElementById('server-ip-addresses-assigned-to');
        var notesInput = document.getElementById('server-ip-addresses-notes');
        if (ipAddressInput) {
            ipAddressInput.value = ip.ipAddress;
        }
        if (ipVersionInput) {
            ipVersionInput.value = ip.ipVersion;
        }
        populateServerDropdown();
        if (serverIdInput) {
            serverIdInput.value = String(ip.serverId);
        }
        if (statusInput) {
            statusInput.value = ip.status;
        }
        if (isPrimaryInput) {
            isPrimaryInput.checked = ip.isPrimary;
        }
        if (assignedToInput) {
            assignedToInput.value = ip.assignedTo || '';
        }
        if (notesInput) {
            notesInput.value = ip.notes || '';
        }
        showModal('server-ip-addresses-edit-modal');
    }
    function saveServerIpAddress() {
        return __awaiter(this, void 0, void 0, function () {
            var ipAddressInput, ipVersionInput, serverIdInput, statusInput, isPrimaryInput, assignedToInput, notesInput, ipAddress, ipVersion, serverId, status, payload, response, _a;
            var _b, _c, _d, _e, _f, _g;
            return __generator(this, function (_h) {
                switch (_h.label) {
                    case 0:
                        ipAddressInput = document.getElementById('server-ip-addresses-ip-address');
                        ipVersionInput = document.getElementById('server-ip-addresses-ip-version');
                        serverIdInput = document.getElementById('server-ip-addresses-server-id');
                        statusInput = document.getElementById('server-ip-addresses-status');
                        isPrimaryInput = document.getElementById('server-ip-addresses-is-primary');
                        assignedToInput = document.getElementById('server-ip-addresses-assigned-to');
                        notesInput = document.getElementById('server-ip-addresses-notes');
                        ipAddress = (_b = ipAddressInput === null || ipAddressInput === void 0 ? void 0 : ipAddressInput.value.trim()) !== null && _b !== void 0 ? _b : '';
                        ipVersion = (_d = (_c = ipVersionInput === null || ipVersionInput === void 0 ? void 0 : ipVersionInput.value) === null || _c === void 0 ? void 0 : _c.trim()) !== null && _d !== void 0 ? _d : '';
                        serverId = (serverIdInput === null || serverIdInput === void 0 ? void 0 : serverIdInput.value) ? Number(serverIdInput.value) : null;
                        status = (_f = (_e = statusInput === null || statusInput === void 0 ? void 0 : statusInput.value) === null || _e === void 0 ? void 0 : _e.trim()) !== null && _f !== void 0 ? _f : 'Active';
                        if (!ipAddress || !ipVersion || !serverId || !status) {
                            showError('IP Address, IP Version, Server, and Status are required');
                            return [2 /*return*/];
                        }
                        payload = {
                            serverId: serverId,
                            ipAddress: ipAddress,
                            ipVersion: ipVersion,
                            status: status,
                            isPrimary: (_g = isPrimaryInput === null || isPrimaryInput === void 0 ? void 0 : isPrimaryInput.checked) !== null && _g !== void 0 ? _g : false,
                            assignedTo: (assignedToInput === null || assignedToInput === void 0 ? void 0 : assignedToInput.value.trim()) || null,
                            notes: (notesInput === null || notesInput === void 0 ? void 0 : notesInput.value.trim()) || null,
                        };
                        if (!editingId) return [3 /*break*/, 2];
                        return [4 /*yield*/, apiRequest("".concat(getApiBaseUrl(), "/ServerIpAddresses/").concat(editingId), { method: 'PUT', body: JSON.stringify(payload) })];
                    case 1:
                        _a = _h.sent();
                        return [3 /*break*/, 4];
                    case 2: return [4 /*yield*/, apiRequest("".concat(getApiBaseUrl(), "/ServerIpAddresses"), { method: 'POST', body: JSON.stringify(payload) })];
                    case 3:
                        _a = _h.sent();
                        _h.label = 4;
                    case 4:
                        response = _a;
                        if (response.success) {
                            hideModal('server-ip-addresses-edit-modal');
                            showSuccess(editingId ? 'IP address updated successfully' : 'IP address created successfully');
                            loadServerIpAddresses();
                        }
                        else {
                            showError(response.message || 'Save failed');
                        }
                        return [2 /*return*/];
                }
            });
        });
    }
    function openDelete(id, name) {
        pendingDeleteId = id;
        var deleteName = document.getElementById('server-ip-addresses-delete-name');
        if (deleteName) {
            deleteName.textContent = name;
        }
        showModal('server-ip-addresses-delete-modal');
    }
    function doDelete() {
        return __awaiter(this, void 0, void 0, function () {
            var response;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        if (!pendingDeleteId) {
                            return [2 /*return*/];
                        }
                        return [4 /*yield*/, apiRequest("".concat(getApiBaseUrl(), "/ServerIpAddresses/").concat(pendingDeleteId), { method: 'DELETE' })];
                    case 1:
                        response = _a.sent();
                        hideModal('server-ip-addresses-delete-modal');
                        if (response.success) {
                            showSuccess('IP address deleted successfully');
                            loadServerIpAddresses();
                        }
                        else {
                            showError(response.message || 'Delete failed');
                        }
                        pendingDeleteId = null;
                        return [2 /*return*/];
                }
            });
        });
    }
    function esc(text) {
        var map = { '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#039;' };
        return (text || '').toString().replace(/[&<>"']/g, function (char) { return map[char]; });
    }
    function showSuccess(message) {
        var alert = document.getElementById('server-ip-addresses-alert-success');
        if (!alert) {
            return;
        }
        alert.textContent = message;
        alert.classList.remove('d-none');
        var errorAlert = document.getElementById('server-ip-addresses-alert-error');
        errorAlert === null || errorAlert === void 0 ? void 0 : errorAlert.classList.add('d-none');
        setTimeout(function () { return alert.classList.add('d-none'); }, 5000);
    }
    function showError(message) {
        var alert = document.getElementById('server-ip-addresses-alert-error');
        if (!alert) {
            return;
        }
        alert.textContent = message;
        alert.classList.remove('d-none');
        var successAlert = document.getElementById('server-ip-addresses-alert-success');
        successAlert === null || successAlert === void 0 ? void 0 : successAlert.classList.add('d-none');
    }
    function showModal(id) {
        var element = document.getElementById(id);
        if (!element || !window.bootstrap) {
            return;
        }
        var modal = new window.bootstrap.Modal(element);
        modal.show();
    }
    function hideModal(id) {
        var element = document.getElementById(id);
        if (!element || !window.bootstrap) {
            return;
        }
        var modal = window.bootstrap.Modal.getInstance(element);
        modal === null || modal === void 0 ? void 0 : modal.hide();
    }
    function bindTableActions() {
        var tableBody = document.getElementById('server-ip-addresses-table-body');
        if (!tableBody) {
            return;
        }
        tableBody.addEventListener('click', function (event) {
            var _a;
            var target = event.target;
            var button = target.closest('button[data-action]');
            if (!button) {
                return;
            }
            var id = Number(button.dataset.id);
            if (!id) {
                return;
            }
            if (button.dataset.action === 'edit') {
                openEdit(id);
                return;
            }
            if (button.dataset.action === 'delete') {
                openDelete(id, (_a = button.dataset.name) !== null && _a !== void 0 ? _a : '');
            }
        });
    }
    function initializeServerIpAddressesPage() {
        var _a, _b, _c;
        var page = document.getElementById('server-ip-addresses-page');
        if (!page || page.dataset.initialized === 'true') {
            return;
        }
        page.dataset.initialized = 'true';
        (_a = document.getElementById('server-ip-addresses-create')) === null || _a === void 0 ? void 0 : _a.addEventListener('click', openCreate);
        (_b = document.getElementById('server-ip-addresses-save')) === null || _b === void 0 ? void 0 : _b.addEventListener('click', saveServerIpAddress);
        (_c = document.getElementById('server-ip-addresses-confirm-delete')) === null || _c === void 0 ? void 0 : _c.addEventListener('click', doDelete);
        bindTableActions();
        loadServers();
        loadServerIpAddresses();
    }
    function setupPageObserver() {
        // Try immediate initialization
        initializeServerIpAddressesPage();
        // Set up MutationObserver for Blazor navigation
        if (document.body) {
            var observer = new MutationObserver(function () {
                var page = document.getElementById('server-ip-addresses-page');
                if (page && page.dataset.initialized !== 'true') {
                    initializeServerIpAddressesPage();
                }
            });
            observer.observe(document.body, { childList: true, subtree: true });
        }
    }
    // Initialize when DOM is ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', setupPageObserver);
    }
    else {
        setupPageObserver();
    }
})();

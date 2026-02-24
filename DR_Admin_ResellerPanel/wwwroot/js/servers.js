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
    var allServers = [];
    var serverTypes = [];
    var operatingSystems = [];
    var hostProviders = [];
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
                        console.error('Servers request failed', error_1);
                        return [2 /*return*/, {
                                success: false,
                                message: 'Network error. Please try again.',
                            }];
                    case 6: return [2 /*return*/];
                }
            });
        });
    }
    function loadLookupData() {
        return __awaiter(this, void 0, void 0, function () {
            var _a, serverTypesRes, osRes, providersRes;
            return __generator(this, function (_b) {
                switch (_b.label) {
                    case 0: return [4 /*yield*/, Promise.all([
                            apiRequest("".concat(getApiBaseUrl(), "/ServerTypes"), { method: 'GET' }),
                            apiRequest("".concat(getApiBaseUrl(), "/OperatingSystems"), { method: 'GET' }),
                            apiRequest("".concat(getApiBaseUrl(), "/HostProviders"), { method: 'GET' }),
                        ])];
                    case 1:
                        _a = _b.sent(), serverTypesRes = _a[0], osRes = _a[1], providersRes = _a[2];
                        serverTypes = extractArray(serverTypesRes.data);
                        operatingSystems = extractArray(osRes.data);
                        hostProviders = extractArray(providersRes.data);
                        populateDropdowns();
                        return [2 /*return*/];
                }
            });
        });
    }
    function extractArray(data) {
        var rawItems = Array.isArray(data)
            ? data
            : Array.isArray(data === null || data === void 0 ? void 0 : data.data)
                ? data.data
                : [];
        return rawItems.map(function (item) {
            var _a, _b, _c, _d, _e, _f, _g, _h;
            return ({
                id: (_b = (_a = item.id) !== null && _a !== void 0 ? _a : item.Id) !== null && _b !== void 0 ? _b : 0,
                name: (_d = (_c = item.name) !== null && _c !== void 0 ? _c : item.Name) !== null && _d !== void 0 ? _d : '',
                displayName: (_h = (_g = (_f = (_e = item.displayName) !== null && _e !== void 0 ? _e : item.DisplayName) !== null && _f !== void 0 ? _f : item.name) !== null && _g !== void 0 ? _g : item.Name) !== null && _h !== void 0 ? _h : '',
            });
        });
    }
    function populateDropdowns() {
        var serverTypeSelect = document.getElementById('servers-server-type-id');
        if (serverTypeSelect) {
            var selected = serverTypeSelect.value;
            serverTypeSelect.innerHTML = '<option value="">Select Server Type...</option>' +
                serverTypes.map(function (t) { return "<option value=\"".concat(t.id, "\">").concat(esc(t.displayName || t.name || ''), "</option>"); }).join('');
            if (selected) {
                serverTypeSelect.value = selected;
            }
        }
        var osSelect = document.getElementById('servers-operating-system-id');
        if (osSelect) {
            var selected = osSelect.value;
            osSelect.innerHTML = '<option value="">Select OS...</option>' +
                operatingSystems.map(function (os) { return "<option value=\"".concat(os.id, "\">").concat(esc(os.displayName || os.name || ''), "</option>"); }).join('');
            if (selected) {
                osSelect.value = selected;
            }
        }
        var providerSelect = document.getElementById('servers-host-provider-id');
        if (providerSelect) {
            var selected = providerSelect.value;
            providerSelect.innerHTML = '<option value="">Select Provider...</option>' +
                hostProviders.map(function (p) { return "<option value=\"".concat(p.id, "\">").concat(esc(p.displayName || p.name || ''), "</option>"); }).join('');
            if (selected) {
                providerSelect.value = selected;
            }
        }
    }
    function loadServers() {
        return __awaiter(this, void 0, void 0, function () {
            var tableBody, response, rawItems;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        tableBody = document.getElementById('servers-table-body');
                        if (!tableBody) {
                            return [2 /*return*/];
                        }
                        tableBody.innerHTML = '<tr><td colspan="8" class="text-center"><div class="spinner-border text-primary"></div></td></tr>';
                        return [4 /*yield*/, apiRequest("".concat(getApiBaseUrl(), "/Servers"), { method: 'GET' })];
                    case 1:
                        response = _a.sent();
                        if (!response.success) {
                            showError(response.message || 'Failed to load servers');
                            tableBody.innerHTML = '<tr><td colspan="8" class="text-center text-danger">Failed to load data</td></tr>';
                            return [2 /*return*/];
                        }
                        // Debug: Log the raw response structure
                        console.log('API Response structure:', response);
                        console.log('Response.data type:', Array.isArray(response.data) ? 'Array' : typeof response.data);
                        console.log('Response.data:', response.data);
                        rawItems = extractArray(response.data);
                        console.log('Extracted array length:', rawItems.length);
                        if (rawItems.length > 0) {
                            console.log('First raw item from API:', rawItems[0]);
                        }
                        allServers = rawItems.map(function (item) {
                            var _a, _b, _c, _d, _e, _f, _g, _h, _j, _k, _l, _m, _o, _p, _q, _r, _s, _t, _u, _v, _w, _x, _y, _z, _0, _1, _2, _3, _4, _5;
                            var server = {
                                id: (_b = (_a = item.id) !== null && _a !== void 0 ? _a : item.Id) !== null && _b !== void 0 ? _b : 0,
                                name: (_d = (_c = item.name) !== null && _c !== void 0 ? _c : item.Name) !== null && _d !== void 0 ? _d : '',
                                location: (_f = (_e = item.location) !== null && _e !== void 0 ? _e : item.Location) !== null && _f !== void 0 ? _f : null,
                                serverTypeId: (_h = (_g = item.serverTypeId) !== null && _g !== void 0 ? _g : item.ServerTypeId) !== null && _h !== void 0 ? _h : 0,
                                serverTypeName: (_k = (_j = item.serverTypeName) !== null && _j !== void 0 ? _j : item.ServerTypeName) !== null && _k !== void 0 ? _k : null,
                                operatingSystemId: (_m = (_l = item.operatingSystemId) !== null && _l !== void 0 ? _l : item.OperatingSystemId) !== null && _m !== void 0 ? _m : 0,
                                operatingSystemName: (_p = (_o = item.operatingSystemName) !== null && _o !== void 0 ? _o : item.OperatingSystemName) !== null && _p !== void 0 ? _p : null,
                                hostProviderId: (_r = (_q = item.hostProviderId) !== null && _q !== void 0 ? _q : item.HostProviderId) !== null && _r !== void 0 ? _r : null,
                                hostProviderName: (_t = (_s = item.hostProviderName) !== null && _s !== void 0 ? _s : item.HostProviderName) !== null && _t !== void 0 ? _t : null,
                                status: (_v = (_u = item.status) !== null && _u !== void 0 ? _u : item.Status) !== null && _v !== void 0 ? _v : true,
                                cpuCores: (_x = (_w = item.cpuCores) !== null && _w !== void 0 ? _w : item.CpuCores) !== null && _x !== void 0 ? _x : null,
                                ramMB: (_0 = (_z = (_y = item.ramMB) !== null && _y !== void 0 ? _y : item.RamMB) !== null && _z !== void 0 ? _z : item.ramMb) !== null && _0 !== void 0 ? _0 : null,
                                diskSpaceGB: (_3 = (_2 = (_1 = item.diskSpaceGB) !== null && _1 !== void 0 ? _1 : item.DiskSpaceGB) !== null && _2 !== void 0 ? _2 : item.diskSpaceGb) !== null && _3 !== void 0 ? _3 : null,
                                notes: (_5 = (_4 = item.notes) !== null && _4 !== void 0 ? _4 : item.Notes) !== null && _5 !== void 0 ? _5 : null,
                            };
                            // Debug log for first item to verify data structure
                            if (rawItems.indexOf(item) === 0) {
                                console.log('First server after mapping:', server);
                            }
                            return server;
                        });
                        renderTable();
                        return [2 /*return*/];
                }
            });
        });
    }
    function renderTable() {
        var tableBody = document.getElementById('servers-table-body');
        if (!tableBody) {
            return;
        }
        if (!allServers.length) {
            tableBody.innerHTML = '<tr><td colspan="8" class="text-center text-muted">No servers found. Click "New Server" to add one.</td></tr>';
            return;
        }
        tableBody.innerHTML = allServers.map(function (server) { return "\n        <tr>\n            <td>".concat(server.id, "</td>\n            <td><strong>").concat(esc(server.name), "</strong></td>\n            <td>").concat(esc(server.location || '-'), "</td>\n            <td>").concat(esc(server.serverTypeName || '-'), "</td>\n            <td>").concat(esc(server.operatingSystemName || '-'), "</td>\n            <td>").concat(esc(server.hostProviderName || '-'), "</td>\n            <td><span class=\"badge bg-").concat(getStatusBadgeColor(server.status), "\">").concat(esc(getStatusText(server.status)), "</span></td>\n            <td>\n                <div class=\"btn-group btn-group-sm\">\n                    <button class=\"btn btn-outline-primary\" type=\"button\" data-action=\"edit\" data-id=\"").concat(server.id, "\" title=\"Edit\"><i class=\"bi bi-pencil\"></i></button>\n                    <button class=\"btn btn-outline-danger\" type=\"button\" data-action=\"delete\" data-id=\"").concat(server.id, "\" data-name=\"").concat(esc(server.name), "\" title=\"Delete\"><i class=\"bi bi-trash\"></i></button>\n                </div>\n            </td>\n        </tr>\n    "); }).join('');
    }
    function getStatusBadgeColor(status) {
        if (status === true)
            return 'success';
        if (status === false)
            return 'secondary';
        return 'warning';
    }
    function getStatusText(status) {
        if (status === true)
            return 'Active';
        if (status === false)
            return 'Inactive';
        return 'Unknown';
    }
    function openCreate() {
        editingId = null;
        var modalTitle = document.getElementById('servers-modal-title');
        if (modalTitle) {
            modalTitle.textContent = 'New Server';
        }
        var form = document.getElementById('servers-form');
        form === null || form === void 0 ? void 0 : form.reset();
        var statusCheckbox = document.getElementById('servers-status');
        if (statusCheckbox) {
            statusCheckbox.checked = true;
        }
        populateDropdowns();
        showModal('servers-edit-modal');
    }
    function openEdit(id) {
        var server = allServers.find(function (item) { return item.id === id; });
        if (!server) {
            console.error('Server not found with ID:', id);
            return;
        }
        console.log('Editing server:', server);
        editingId = id;
        var modalTitle = document.getElementById('servers-modal-title');
        if (modalTitle) {
            modalTitle.textContent = 'Edit Server';
        }
        var nameInput = document.getElementById('servers-name');
        var locationInput = document.getElementById('servers-location');
        var serverTypeInput = document.getElementById('servers-server-type-id');
        var osInput = document.getElementById('servers-operating-system-id');
        var providerInput = document.getElementById('servers-host-provider-id');
        var statusInput = document.getElementById('servers-status');
        var cpuCoresInput = document.getElementById('servers-cpu-cores');
        var ramMBInput = document.getElementById('servers-ram-mb');
        var diskSpaceGBInput = document.getElementById('servers-disk-space-gb');
        var notesInput = document.getElementById('servers-notes');
        // Set basic text fields
        if (nameInput) {
            nameInput.value = server.name || '';
        }
        if (locationInput) {
            locationInput.value = server.location || '';
        }
        // Populate dropdowns before setting values
        populateDropdowns();
        // Set dropdown values - use empty string if null/undefined to reset to default
        if (serverTypeInput) {
            serverTypeInput.value = server.serverTypeId ? String(server.serverTypeId) : '';
            console.log('Set serverTypeId to:', serverTypeInput.value);
        }
        if (osInput) {
            osInput.value = server.operatingSystemId ? String(server.operatingSystemId) : '';
            console.log('Set operatingSystemId to:', osInput.value);
        }
        if (providerInput) {
            providerInput.value = server.hostProviderId ? String(server.hostProviderId) : '';
            console.log('Set hostProviderId to:', providerInput.value);
        }
        if (statusInput) {
            statusInput.checked = server.status === true;
            console.log('Set status to:', statusInput.checked);
        }
        // Set numeric fields - use empty string if null to clear the field
        if (cpuCoresInput) {
            cpuCoresInput.value = server.cpuCores !== null && server.cpuCores !== undefined ? String(server.cpuCores) : '';
            console.log('Set cpuCores to:', cpuCoresInput.value);
        }
        if (ramMBInput) {
            ramMBInput.value = server.ramMB !== null && server.ramMB !== undefined ? String(server.ramMB) : '';
            console.log('Set ramMB to:', ramMBInput.value);
        }
        if (diskSpaceGBInput) {
            diskSpaceGBInput.value = server.diskSpaceGB !== null && server.diskSpaceGB !== undefined ? String(server.diskSpaceGB) : '';
            console.log('Set diskSpaceGB to:', diskSpaceGBInput.value);
        }
        if (notesInput) {
            notesInput.value = server.notes || '';
        }
        showModal('servers-edit-modal');
    }
    function saveServer() {
        return __awaiter(this, void 0, void 0, function () {
            var nameInput, locationInput, serverTypeInput, osInput, providerInput, statusInput, cpuCoresInput, ramMBInput, diskSpaceGBInput, notesInput, name, serverTypeId, operatingSystemId, status, payload, response, _a;
            var _b, _c;
            return __generator(this, function (_d) {
                switch (_d.label) {
                    case 0:
                        nameInput = document.getElementById('servers-name');
                        locationInput = document.getElementById('servers-location');
                        serverTypeInput = document.getElementById('servers-server-type-id');
                        osInput = document.getElementById('servers-operating-system-id');
                        providerInput = document.getElementById('servers-host-provider-id');
                        statusInput = document.getElementById('servers-status');
                        cpuCoresInput = document.getElementById('servers-cpu-cores');
                        ramMBInput = document.getElementById('servers-ram-mb');
                        diskSpaceGBInput = document.getElementById('servers-disk-space-gb');
                        notesInput = document.getElementById('servers-notes');
                        name = (_b = nameInput === null || nameInput === void 0 ? void 0 : nameInput.value.trim()) !== null && _b !== void 0 ? _b : '';
                        serverTypeId = (serverTypeInput === null || serverTypeInput === void 0 ? void 0 : serverTypeInput.value) ? Number(serverTypeInput.value) : null;
                        operatingSystemId = (osInput === null || osInput === void 0 ? void 0 : osInput.value) ? Number(osInput.value) : null;
                        status = (_c = statusInput === null || statusInput === void 0 ? void 0 : statusInput.checked) !== null && _c !== void 0 ? _c : true;
                        if (!name || !serverTypeId || !operatingSystemId) {
                            showError('Server Name, Server Type, and Operating System are required');
                            return [2 /*return*/];
                        }
                        payload = {
                            name: name,
                            serverTypeId: serverTypeId,
                            operatingSystemId: operatingSystemId,
                            status: status,
                            location: (locationInput === null || locationInput === void 0 ? void 0 : locationInput.value.trim()) || null,
                            hostProviderId: (providerInput === null || providerInput === void 0 ? void 0 : providerInput.value) ? Number(providerInput.value) : null,
                            cpuCores: (cpuCoresInput === null || cpuCoresInput === void 0 ? void 0 : cpuCoresInput.value) ? Number(cpuCoresInput.value) : null,
                            ramMB: (ramMBInput === null || ramMBInput === void 0 ? void 0 : ramMBInput.value) ? Number(ramMBInput.value) : null,
                            diskSpaceGB: (diskSpaceGBInput === null || diskSpaceGBInput === void 0 ? void 0 : diskSpaceGBInput.value) ? Number(diskSpaceGBInput.value) : null,
                            notes: (notesInput === null || notesInput === void 0 ? void 0 : notesInput.value.trim()) || null,
                        };
                        if (!editingId) return [3 /*break*/, 2];
                        return [4 /*yield*/, apiRequest("".concat(getApiBaseUrl(), "/Servers/").concat(editingId), { method: 'PUT', body: JSON.stringify(payload) })];
                    case 1:
                        _a = _d.sent();
                        return [3 /*break*/, 4];
                    case 2: return [4 /*yield*/, apiRequest("".concat(getApiBaseUrl(), "/Servers"), { method: 'POST', body: JSON.stringify(payload) })];
                    case 3:
                        _a = _d.sent();
                        _d.label = 4;
                    case 4:
                        response = _a;
                        if (response.success) {
                            hideModal('servers-edit-modal');
                            showSuccess(editingId ? 'Server updated successfully' : 'Server created successfully');
                            loadServers();
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
        var deleteName = document.getElementById('servers-delete-name');
        if (deleteName) {
            deleteName.textContent = name;
        }
        showModal('servers-delete-modal');
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
                        return [4 /*yield*/, apiRequest("".concat(getApiBaseUrl(), "/Servers/").concat(pendingDeleteId), { method: 'DELETE' })];
                    case 1:
                        response = _a.sent();
                        hideModal('servers-delete-modal');
                        if (response.success) {
                            showSuccess('Server deleted successfully');
                            loadServers();
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
        var alert = document.getElementById('servers-alert-success');
        if (!alert) {
            return;
        }
        alert.textContent = message;
        alert.classList.remove('d-none');
        var errorAlert = document.getElementById('servers-alert-error');
        errorAlert === null || errorAlert === void 0 ? void 0 : errorAlert.classList.add('d-none');
        setTimeout(function () { return alert.classList.add('d-none'); }, 5000);
    }
    function showError(message) {
        var alert = document.getElementById('servers-alert-error');
        if (!alert) {
            return;
        }
        alert.textContent = message;
        alert.classList.remove('d-none');
        var successAlert = document.getElementById('servers-alert-success');
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
        var tableBody = document.getElementById('servers-table-body');
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
    function initializeServersPage() {
        var _a, _b, _c;
        var page = document.getElementById('servers-page');
        if (!page || page.dataset.initialized === 'true') {
            return;
        }
        page.dataset.initialized = 'true';
        (_a = document.getElementById('servers-create')) === null || _a === void 0 ? void 0 : _a.addEventListener('click', openCreate);
        (_b = document.getElementById('servers-save')) === null || _b === void 0 ? void 0 : _b.addEventListener('click', saveServer);
        (_c = document.getElementById('servers-confirm-delete')) === null || _c === void 0 ? void 0 : _c.addEventListener('click', doDelete);
        bindTableActions();
        loadLookupData();
        loadServers();
    }
    function setupPageObserver() {
        // Try immediate initialization
        initializeServersPage();
        // Set up MutationObserver for Blazor navigation
        if (document.body) {
            var observer = new MutationObserver(function () {
                var page = document.getElementById('servers-page');
                if (page && page.dataset.initialized !== 'true') {
                    initializeServersPage();
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

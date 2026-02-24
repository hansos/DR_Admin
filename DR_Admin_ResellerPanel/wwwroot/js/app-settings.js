"use strict";
// @ts-nocheck
const apiBaseUrl = window.location.protocol === 'https:'
    ? 'https://localhost:7201/api/v1'
    : 'http://localhost:5133/api/v1';
const appSettings = {
    apiBaseUrl,
};
window.AppSettings = appSettings;
//# sourceMappingURL=app-settings.js.map
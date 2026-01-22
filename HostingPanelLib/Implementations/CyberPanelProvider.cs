using HostingPanelLib.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace HostingPanelLib.Implementations
{
    public class CyberPanelProvider : BaseHostingPanel
    {
        private readonly string _apiKey;
        private readonly string _adminUsername;
        private readonly string _adminPassword;
        private readonly int _port;
        private readonly bool _useHttps;

        public CyberPanelProvider(string apiUrl, string apiKey, string adminUsername, string adminPassword, int port, bool useHttps)
            : base(apiUrl)
        {
            _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
            _adminUsername = adminUsername ?? throw new ArgumentNullException(nameof(adminUsername));
            _adminPassword = adminPassword ?? throw new ArgumentNullException(nameof(adminPassword));
            _port = port;
            _useHttps = useHttps;

            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
        }

        public override async Task<HostingAccountResult> CreateWebHostingAccountAsync(HostingAccountRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Domain))
                {
                    return CreateHostingErrorResult("Domain name is required", "INVALID_DOMAIN");
                }

                if (string.IsNullOrWhiteSpace(request.Username))
                {
                    return CreateHostingErrorResult("Username is required", "INVALID_USERNAME");
                }

                if (string.IsNullOrWhiteSpace(request.Email))
                {
                    return CreateHostingErrorResult("Email is required", "INVALID_EMAIL");
                }

                // CyberPanel createWebsite API endpoint
                var endpoint = "/api/createWebsite";

                // Build request payload according to CyberPanel API specs
                var payload = new
                {
                    adminUser = _adminUsername,
                    adminPass = _adminPassword,
                    domainName = request.Domain,
                    ownerEmail = request.Email,
                    packageName = request.Plan ?? "Default",
                    websiteOwner = request.Username,
                    ownerPassword = request.Password,
                    phpSelection = "PHP 8.1", // Default PHP version
                    ssl = 1, // Enable SSL by default
                    dkimCheck = 1, // Enable DKIM
                    openBasedir = 1 // Enable open_basedir protection
                };

                var response = await _httpClient.PostAsJsonAsync(endpoint, payload);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return CreateHostingErrorResult(
                        $"CyberPanel API request failed: {response.StatusCode} - {responseContent}",
                        response.StatusCode.ToString()
                    );
                }

                // Parse CyberPanel response
                using var jsonDoc = JsonDocument.Parse(responseContent);
                var root = jsonDoc.RootElement;

                // CyberPanel returns status: 1 for success, 0 for failure
                if (root.TryGetProperty("status", out var statusElement))
                {
                    var status = statusElement.GetInt32();
                    
                    if (status == 1)
                    {
                        // Success
                        var message = root.TryGetProperty("message", out var msgElement) 
                            ? msgElement.GetString() 
                            : "Website created successfully";

                        return new HostingAccountResult
                        {
                            Success = true,
                            Message = message ?? "Website created successfully",
                            AccountId = request.Domain,
                            Username = request.Username,
                            Domain = request.Domain,
                            PlanName = request.Plan ?? "Default",
                            CreatedDate = DateTime.UtcNow
                        };
                    }
                    else
                    {
                        // Failure - extract error message
                        var errorMessage = root.TryGetProperty("error_message", out var errMsgElement)
                            ? errMsgElement.GetString()
                            : root.TryGetProperty("message", out var msgElement2)
                                ? msgElement2.GetString()
                                : "Unknown error occurred";

                        return CreateHostingErrorResult(
                            errorMessage ?? "Failed to create website",
                            "CYBERPANEL_ERROR"
                        );
                    }
                }

                return CreateHostingErrorResult("Invalid response format from CyberPanel", "INVALID_RESPONSE");
            }
            catch (HttpRequestException ex)
            {
                return CreateHostingErrorResult(
                    $"Network error while connecting to CyberPanel: {ex.Message}",
                    "NETWORK_ERROR"
                );
            }
            catch (JsonException ex)
            {
                return CreateHostingErrorResult(
                    $"Failed to parse CyberPanel response: {ex.Message}",
                    "JSON_PARSE_ERROR"
                );
            }
            catch (Exception ex)
            {
                return CreateHostingErrorResult(
                    $"Unexpected error: {ex.Message}",
                    "UNEXPECTED_ERROR"
                );
            }
        }

        public override async Task<AccountUpdateResult> UpdateWebHostingAccountAsync(string accountId, HostingAccountRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(accountId))
                {
                    return CreateUpdateErrorResult("Account ID (domain name) is required", "INVALID_ACCOUNT_ID");
                }

                // CyberPanel modifyWebsite API endpoint
                var endpoint = "/api/modifyWebsite";

                // Build request payload - CyberPanel modifyWebsite supports various modifications
                var payload = new
                {
                    adminUser = _adminUsername,
                    adminPass = _adminPassword,
                    domainName = accountId,
                    packageName = request.Plan,
                    adminEmail = request.Email,
                    phpSelection = request.AdditionalSettings?.GetValueOrDefault("phpVersion") ?? "PHP 8.1",
                    // Additional settings can be passed through AdditionalSettings dictionary
                };

                var response = await _httpClient.PostAsJsonAsync(endpoint, payload);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return CreateUpdateErrorResult(
                        $"CyberPanel API request failed: {response.StatusCode} - {responseContent}",
                        response.StatusCode.ToString()
                    );
                }

                // Parse CyberPanel response
                using var jsonDoc = JsonDocument.Parse(responseContent);
                var root = jsonDoc.RootElement;

                if (root.TryGetProperty("status", out var statusElement))
                {
                    var status = statusElement.GetInt32();

                    if (status == 1)
                    {
                        var message = root.TryGetProperty("message", out var msgElement)
                            ? msgElement.GetString()
                            : "Website updated successfully";

                        return new AccountUpdateResult
                        {
                            Success = true,
                            Message = message ?? "Website updated successfully",
                            AccountId = accountId,
                            UpdatedField = "WebsiteConfiguration",
                            UpdatedDate = DateTime.UtcNow
                        };
                    }
                    else
                    {
                        var errorMessage = root.TryGetProperty("error_message", out var errMsgElement)
                            ? errMsgElement.GetString()
                            : root.TryGetProperty("message", out var msgElement2)
                                ? msgElement2.GetString()
                                : "Unknown error occurred";

                        return CreateUpdateErrorResult(
                            errorMessage ?? "Failed to update website",
                            "CYBERPANEL_ERROR"
                        );
                    }
                }

                return CreateUpdateErrorResult("Invalid response format from CyberPanel", "INVALID_RESPONSE");
            }
            catch (HttpRequestException ex)
            {
                return CreateUpdateErrorResult(
                    $"Network error while connecting to CyberPanel: {ex.Message}",
                    "NETWORK_ERROR"
                );
            }
            catch (JsonException ex)
            {
                return CreateUpdateErrorResult(
                    $"Failed to parse CyberPanel response: {ex.Message}",
                    "JSON_PARSE_ERROR"
                );
            }
            catch (Exception ex)
            {
                return CreateUpdateErrorResult(
                    $"Unexpected error: {ex.Message}",
                    "UNEXPECTED_ERROR"
                );
            }
        }

        public override async Task<AccountUpdateResult> SuspendWebHostingAccountAsync(string accountId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(accountId))
                {
                    return CreateUpdateErrorResult("Account ID (domain name) is required", "INVALID_ACCOUNT_ID");
                }

                // CyberPanel suspendWebsite API endpoint
                var endpoint = "/api/suspendWebsite";

                // Build request payload
                var payload = new
                {
                    adminUser = _adminUsername,
                    adminPass = _adminPassword,
                    websiteName = accountId
                };

                var response = await _httpClient.PostAsJsonAsync(endpoint, payload);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return CreateUpdateErrorResult(
                        $"CyberPanel API request failed: {response.StatusCode} - {responseContent}",
                        response.StatusCode.ToString()
                    );
                }

                // Parse CyberPanel response
                using var jsonDoc = JsonDocument.Parse(responseContent);
                var root = jsonDoc.RootElement;

                if (root.TryGetProperty("status", out var statusElement))
                {
                    var status = statusElement.GetInt32();

                    if (status == 1)
                    {
                        var message = root.TryGetProperty("message", out var msgElement)
                            ? msgElement.GetString()
                            : "Website suspended successfully";

                        return new AccountUpdateResult
                        {
                            Success = true,
                            Message = message ?? "Website suspended successfully",
                            AccountId = accountId,
                            UpdatedField = "Status",
                            OldValue = "Active",
                            NewValue = "Suspended",
                            UpdatedDate = DateTime.UtcNow
                        };
                    }
                    else
                    {
                        var errorMessage = root.TryGetProperty("error_message", out var errMsgElement)
                            ? errMsgElement.GetString()
                            : root.TryGetProperty("message", out var msgElement2)
                                ? msgElement2.GetString()
                                : "Unknown error occurred";

                        return CreateUpdateErrorResult(
                            errorMessage ?? "Failed to suspend website",
                            "CYBERPANEL_ERROR"
                        );
                    }
                }

                return CreateUpdateErrorResult("Invalid response format from CyberPanel", "INVALID_RESPONSE");
            }
            catch (HttpRequestException ex)
            {
                return CreateUpdateErrorResult(
                    $"Network error while connecting to CyberPanel: {ex.Message}",
                    "NETWORK_ERROR"
                );
            }
            catch (JsonException ex)
            {
                return CreateUpdateErrorResult(
                    $"Failed to parse CyberPanel response: {ex.Message}",
                    "JSON_PARSE_ERROR"
                );
            }
            catch (Exception ex)
            {
                return CreateUpdateErrorResult(
                    $"Unexpected error: {ex.Message}",
                    "UNEXPECTED_ERROR"
                );
            }
        }

        public override async Task<AccountUpdateResult> UnsuspendWebHostingAccountAsync(string accountId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(accountId))
                {
                    return CreateUpdateErrorResult("Account ID (domain name) is required", "INVALID_ACCOUNT_ID");
                }

                // CyberPanel unsuspendWebsite API endpoint (also called activateWebsite in some versions)
                var endpoint = "/api/unsuspendWebsite";

                // Build request payload
                var payload = new
                {
                    adminUser = _adminUsername,
                    adminPass = _adminPassword,
                    websiteName = accountId
                };

                var response = await _httpClient.PostAsJsonAsync(endpoint, payload);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return CreateUpdateErrorResult(
                        $"CyberPanel API request failed: {response.StatusCode} - {responseContent}",
                        response.StatusCode.ToString()
                    );
                }

                // Parse CyberPanel response
                using var jsonDoc = JsonDocument.Parse(responseContent);
                var root = jsonDoc.RootElement;

                if (root.TryGetProperty("status", out var statusElement))
                {
                    var status = statusElement.GetInt32();

                    if (status == 1)
                    {
                        var message = root.TryGetProperty("message", out var msgElement)
                            ? msgElement.GetString()
                            : "Website unsuspended successfully";

                        return new AccountUpdateResult
                        {
                            Success = true,
                            Message = message ?? "Website unsuspended successfully",
                            AccountId = accountId,
                            UpdatedField = "Status",
                            OldValue = "Suspended",
                            NewValue = "Active",
                            UpdatedDate = DateTime.UtcNow
                        };
                    }
                    else
                    {
                        var errorMessage = root.TryGetProperty("error_message", out var errMsgElement)
                            ? errMsgElement.GetString()
                            : root.TryGetProperty("message", out var msgElement2)
                                ? msgElement2.GetString()
                                : "Unknown error occurred";

                        return CreateUpdateErrorResult(
                            errorMessage ?? "Failed to unsuspend website",
                            "CYBERPANEL_ERROR"
                        );
                    }
                }

                return CreateUpdateErrorResult("Invalid response format from CyberPanel", "INVALID_RESPONSE");
            }
            catch (HttpRequestException ex)
            {
                return CreateUpdateErrorResult(
                    $"Network error while connecting to CyberPanel: {ex.Message}",
                    "NETWORK_ERROR"
                );
            }
            catch (JsonException ex)
            {
                return CreateUpdateErrorResult(
                    $"Failed to parse CyberPanel response: {ex.Message}",
                    "JSON_PARSE_ERROR"
                );
            }
            catch (Exception ex)
            {
                return CreateUpdateErrorResult(
                    $"Unexpected error: {ex.Message}",
                    "UNEXPECTED_ERROR"
                );
            }
        }

        public override async Task<AccountUpdateResult> DeleteWebHostingAccountAsync(string accountId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(accountId))
                {
                    return CreateUpdateErrorResult("Account ID (domain name) is required", "INVALID_ACCOUNT_ID");
                }

                // CyberPanel deleteWebsite API endpoint
                var endpoint = "/api/deleteWebsite";

                // Build request payload
                var payload = new
                {
                    adminUser = _adminUsername,
                    adminPass = _adminPassword,
                    domainName = accountId
                };

                var response = await _httpClient.PostAsJsonAsync(endpoint, payload);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return CreateUpdateErrorResult(
                        $"CyberPanel API request failed: {response.StatusCode} - {responseContent}",
                        response.StatusCode.ToString()
                    );
                }

                // Parse CyberPanel response
                using var jsonDoc = JsonDocument.Parse(responseContent);
                var root = jsonDoc.RootElement;

                if (root.TryGetProperty("status", out var statusElement))
                {
                    var status = statusElement.GetInt32();

                    if (status == 1)
                    {
                        var message = root.TryGetProperty("message", out var msgElement)
                            ? msgElement.GetString()
                            : "Website deleted successfully";

                        return new AccountUpdateResult
                        {
                            Success = true,
                            Message = message ?? "Website deleted successfully",
                            AccountId = accountId,
                            UpdatedField = "Status",
                            OldValue = "Active",
                            NewValue = "Deleted",
                            UpdatedDate = DateTime.UtcNow
                        };
                    }
                    else
                    {
                        var errorMessage = root.TryGetProperty("error_message", out var errMsgElement)
                            ? errMsgElement.GetString()
                            : root.TryGetProperty("message", out var msgElement2)
                                ? msgElement2.GetString()
                                : "Unknown error occurred";

                        return CreateUpdateErrorResult(
                            errorMessage ?? "Failed to delete website",
                            "CYBERPANEL_ERROR"
                        );
                    }
                }

                return CreateUpdateErrorResult("Invalid response format from CyberPanel", "INVALID_RESPONSE");
            }
            catch (HttpRequestException ex)
            {
                return CreateUpdateErrorResult(
                    $"Network error while connecting to CyberPanel: {ex.Message}",
                    "NETWORK_ERROR"
                );
            }
            catch (JsonException ex)
            {
                return CreateUpdateErrorResult(
                    $"Failed to parse CyberPanel response: {ex.Message}",
                    "JSON_PARSE_ERROR"
                );
            }
            catch (Exception ex)
            {
                return CreateUpdateErrorResult(
                    $"Unexpected error: {ex.Message}",
                    "UNEXPECTED_ERROR"
                );
            }
        }

        public override async Task<AccountInfoResult> GetWebHostingAccountInfoAsync(string accountId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(accountId))
                {
                    return CreateInfoErrorResult("Account ID (domain name) is required", "INVALID_ACCOUNT_ID");
                }

                // CyberPanel getWebsiteDetails API endpoint
                var endpoint = "/api/fetchWebsiteDetails";

                // Build request payload
                var payload = new
                {
                    adminUser = _adminUsername,
                    adminPass = _adminPassword,
                    domainName = accountId
                };

                var response = await _httpClient.PostAsJsonAsync(endpoint, payload);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return CreateInfoErrorResult(
                        $"CyberPanel API request failed: {response.StatusCode} - {responseContent}",
                        response.StatusCode.ToString()
                    );
                }

                // Parse CyberPanel response
                using var jsonDoc = JsonDocument.Parse(responseContent);
                var root = jsonDoc.RootElement;

                if (root.TryGetProperty("status", out var statusElement))
                {
                    var status = statusElement.GetInt32();

                    if (status == 1)
                    {
                        // Extract website details from response
                        var data = root.TryGetProperty("data", out var dataElement) ? dataElement : root;

                        return new AccountInfoResult
                        {
                            Success = true,
                            Message = "Website details retrieved successfully",
                            AccountId = accountId,
                            Domain = accountId,
                            Username = data.TryGetProperty("admin", out var adminElement) ? adminElement.GetString() : null,
                            Email = data.TryGetProperty("adminEmail", out var emailElement) ? emailElement.GetString() : null,
                            Plan = data.TryGetProperty("package", out var packageElement) ? packageElement.GetString() : null,
                            Status = data.TryGetProperty("state", out var stateElement) ? stateElement.GetString() : "Unknown",
                            DiskUsageMB = data.TryGetProperty("diskUsed", out var diskUsedElement) ? diskUsedElement.GetInt32() : null,
                            IpAddress = data.TryGetProperty("ipAddress", out var ipElement) ? ipElement.GetString() : null,
                            AdditionalInfo = new Dictionary<string, object>
                            {
                                ["phpVersion"] = data.TryGetProperty("php", out var phpElement) ? phpElement.GetString() ?? "Unknown" : "Unknown",
                                ["ssl"] = data.TryGetProperty("ssl", out var sslElement) ? sslElement.GetInt32() : 0
                            }
                        };
                    }
                    else
                    {
                        var errorMessage = root.TryGetProperty("error_message", out var errMsgElement)
                            ? errMsgElement.GetString()
                            : root.TryGetProperty("message", out var msgElement)
                                ? msgElement.GetString()
                                : "Unknown error occurred";

                        return CreateInfoErrorResult(
                            errorMessage ?? "Failed to get website details",
                            "CYBERPANEL_ERROR"
                        );
                    }
                }

                return CreateInfoErrorResult("Invalid response format from CyberPanel", "INVALID_RESPONSE");
            }
            catch (HttpRequestException ex)
            {
                return CreateInfoErrorResult(
                    $"Network error while connecting to CyberPanel: {ex.Message}",
                    "NETWORK_ERROR"
                );
            }
            catch (JsonException ex)
            {
                return CreateInfoErrorResult(
                    $"Failed to parse CyberPanel response: {ex.Message}",
                    "JSON_PARSE_ERROR"
                );
            }
            catch (Exception ex)
            {
                return CreateInfoErrorResult(
                    $"Unexpected error: {ex.Message}",
                    "UNEXPECTED_ERROR"
                );
            }
        }

        public override async Task<List<AccountInfoResult>> ListWebHostingAccountsAsync()
        {
            try
            {
                // CyberPanel listWebsites API endpoint
                var endpoint = "/api/fetchWebsites";

                // Build request payload
                var payload = new
                {
                    adminUser = _adminUsername,
                    adminPass = _adminPassword
                };

                var response = await _httpClient.PostAsJsonAsync(endpoint, payload);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new List<AccountInfoResult>();
                }

                // Parse CyberPanel response
                using var jsonDoc = JsonDocument.Parse(responseContent);
                var root = jsonDoc.RootElement;

                var results = new List<AccountInfoResult>();

                if (root.TryGetProperty("status", out var statusElement) && statusElement.GetInt32() == 1)
                {
                    // CyberPanel returns array of websites in "data" property
                    if (root.TryGetProperty("data", out var dataElement) && dataElement.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var website in dataElement.EnumerateArray())
                        {
                            var domain = website.TryGetProperty("domain", out var domainElement) ? domainElement.GetString() : null;
                            
                            if (!string.IsNullOrEmpty(domain))
                            {
                                results.Add(new AccountInfoResult
                                {
                                    Success = true,
                                    AccountId = domain,
                                    Domain = domain,
                                    Username = website.TryGetProperty("admin", out var adminElement) ? adminElement.GetString() : null,
                                    Email = website.TryGetProperty("adminEmail", out var emailElement) ? emailElement.GetString() : null,
                                    Plan = website.TryGetProperty("package", out var packageElement) ? packageElement.GetString() : null,
                                    Status = website.TryGetProperty("state", out var stateElement) ? stateElement.GetString() : "Unknown",
                                    IpAddress = website.TryGetProperty("ipAddress", out var ipElement) ? ipElement.GetString() : null
                                });
                            }
                        }
                    }
                }

                return results;
            }
            catch (Exception)
            {
                return new List<AccountInfoResult>();
            }
        }

        public override async Task<MailAccountResult> CreateMailAccountAsync(MailAccountRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.EmailAddress))
                {
                    return CreateMailErrorResult("Email address is required", "INVALID_EMAIL");
                }

                if (string.IsNullOrWhiteSpace(request.Password))
                {
                    return CreateMailErrorResult("Password is required", "INVALID_PASSWORD");
                }

                if (string.IsNullOrWhiteSpace(request.Domain))
                {
                    return CreateMailErrorResult("Domain is required", "INVALID_DOMAIN");
                }

                // CyberPanel createEmail API endpoint
                var endpoint = "/api/createEmail";

                // Build request payload
                var payload = new
                {
                    adminUser = _adminUsername,
                    adminPass = _adminPassword,
                    domainName = request.Domain,
                    userName = request.EmailAddress.Split('@')[0],
                    password = request.Password,
                    quota = request.QuotaMB ?? 500 // Default 500MB quota
                };

                var response = await _httpClient.PostAsJsonAsync(endpoint, payload);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return CreateMailErrorResult(
                        $"CyberPanel API request failed: {response.StatusCode} - {responseContent}",
                        response.StatusCode.ToString()
                    );
                }

                // Parse CyberPanel response
                using var jsonDoc = JsonDocument.Parse(responseContent);
                var root = jsonDoc.RootElement;

                if (root.TryGetProperty("status", out var statusElement))
                {
                    var status = statusElement.GetInt32();

                    if (status == 1)
                    {
                        var message = root.TryGetProperty("message", out var msgElement)
                            ? msgElement.GetString()
                            : "Email account created successfully";

                        return new MailAccountResult
                        {
                            Success = true,
                            Message = message ?? "Email account created successfully",
                            AccountId = request.EmailAddress,
                            EmailAddress = request.EmailAddress,
                            Domain = request.Domain,
                            QuotaMB = request.QuotaMB ?? 500,
                            CreatedDate = DateTime.UtcNow
                        };
                    }
                    else
                    {
                        var errorMessage = root.TryGetProperty("error_message", out var errMsgElement)
                            ? errMsgElement.GetString()
                            : root.TryGetProperty("message", out var msgElement2)
                                ? msgElement2.GetString()
                                : "Unknown error occurred";

                        return CreateMailErrorResult(
                            errorMessage ?? "Failed to create email account",
                            "CYBERPANEL_ERROR"
                        );
                    }
                }

                return CreateMailErrorResult("Invalid response format from CyberPanel", "INVALID_RESPONSE");
            }
            catch (HttpRequestException ex)
            {
                return CreateMailErrorResult(
                    $"Network error while connecting to CyberPanel: {ex.Message}",
                    "NETWORK_ERROR"
                );
            }
            catch (JsonException ex)
            {
                return CreateMailErrorResult(
                    $"Failed to parse CyberPanel response: {ex.Message}",
                    "JSON_PARSE_ERROR"
                );
            }
            catch (Exception ex)
            {
                return CreateMailErrorResult(
                    $"Unexpected error: {ex.Message}",
                    "UNEXPECTED_ERROR"
                );
            }
        }

        public override async Task<AccountUpdateResult> UpdateMailAccountAsync(string accountId, MailAccountRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(accountId))
                {
                    return CreateUpdateErrorResult("Account ID (email address) is required", "INVALID_ACCOUNT_ID");
                }

                // CyberPanel primarily supports quota changes via changeEmailQuota
                var endpoint = "/api/changeEmailQuota";

                var emailParts = accountId.Split('@');
                if (emailParts.Length != 2)
                {
                    return CreateUpdateErrorResult("Invalid email address format", "INVALID_EMAIL_FORMAT");
                }

                // Build request payload
                var payload = new
                {
                    adminUser = _adminUsername,
                    adminPass = _adminPassword,
                    domainName = emailParts[1],
                    emailUsername = emailParts[0],
                    quota = request.QuotaMB ?? 500
                };

                var response = await _httpClient.PostAsJsonAsync(endpoint, payload);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return CreateUpdateErrorResult(
                        $"CyberPanel API request failed: {response.StatusCode} - {responseContent}",
                        response.StatusCode.ToString()
                    );
                }

                // Parse CyberPanel response
                using var jsonDoc = JsonDocument.Parse(responseContent);
                var root = jsonDoc.RootElement;

                if (root.TryGetProperty("status", out var statusElement))
                {
                    var status = statusElement.GetInt32();

                    if (status == 1)
                    {
                        var message = root.TryGetProperty("message", out var msgElement)
                            ? msgElement.GetString()
                            : "Email account updated successfully";

                        return new AccountUpdateResult
                        {
                            Success = true,
                            Message = message ?? "Email account updated successfully",
                            AccountId = accountId,
                            UpdatedField = "Quota",
                            NewValue = request.QuotaMB,
                            UpdatedDate = DateTime.UtcNow
                        };
                    }
                    else
                    {
                        var errorMessage = root.TryGetProperty("error_message", out var errMsgElement)
                            ? errMsgElement.GetString()
                            : root.TryGetProperty("message", out var msgElement2)
                                ? msgElement2.GetString()
                                : "Unknown error occurred";

                        return CreateUpdateErrorResult(
                            errorMessage ?? "Failed to update email account",
                            "CYBERPANEL_ERROR"
                        );
                    }
                }

                return CreateUpdateErrorResult("Invalid response format from CyberPanel", "INVALID_RESPONSE");
            }
            catch (HttpRequestException ex)
            {
                return CreateUpdateErrorResult(
                    $"Network error while connecting to CyberPanel: {ex.Message}",
                    "NETWORK_ERROR"
                );
            }
            catch (JsonException ex)
            {
                return CreateUpdateErrorResult(
                    $"Failed to parse CyberPanel response: {ex.Message}",
                    "JSON_PARSE_ERROR"
                );
            }
            catch (Exception ex)
            {
                return CreateUpdateErrorResult(
                    $"Unexpected error: {ex.Message}",
                    "UNEXPECTED_ERROR"
                );
            }
        }

        public override async Task<AccountUpdateResult> DeleteMailAccountAsync(string accountId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(accountId))
                {
                    return CreateUpdateErrorResult("Account ID (email address) is required", "INVALID_ACCOUNT_ID");
                }

                var emailParts = accountId.Split('@');
                if (emailParts.Length != 2)
                {
                    return CreateUpdateErrorResult("Invalid email address format", "INVALID_EMAIL_FORMAT");
                }

                // CyberPanel deleteEmail API endpoint
                var endpoint = "/api/deleteEmail";

                // Build request payload
                var payload = new
                {
                    adminUser = _adminUsername,
                    adminPass = _adminPassword,
                    domainName = emailParts[1],
                    emailUsername = emailParts[0]
                };

                var response = await _httpClient.PostAsJsonAsync(endpoint, payload);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return CreateUpdateErrorResult(
                        $"CyberPanel API request failed: {response.StatusCode} - {responseContent}",
                        response.StatusCode.ToString()
                    );
                }

                // Parse CyberPanel response
                using var jsonDoc = JsonDocument.Parse(responseContent);
                var root = jsonDoc.RootElement;

                if (root.TryGetProperty("status", out var statusElement))
                {
                    var status = statusElement.GetInt32();

                    if (status == 1)
                    {
                        var message = root.TryGetProperty("message", out var msgElement)
                            ? msgElement.GetString()
                            : "Email account deleted successfully";

                        return new AccountUpdateResult
                        {
                            Success = true,
                            Message = message ?? "Email account deleted successfully",
                            AccountId = accountId,
                            UpdatedField = "Status",
                            OldValue = "Active",
                            NewValue = "Deleted",
                            UpdatedDate = DateTime.UtcNow
                        };
                    }
                    else
                    {
                        var errorMessage = root.TryGetProperty("error_message", out var errMsgElement)
                            ? errMsgElement.GetString()
                            : root.TryGetProperty("message", out var msgElement2)
                                ? msgElement2.GetString()
                                : "Unknown error occurred";

                        return CreateUpdateErrorResult(
                            errorMessage ?? "Failed to delete email account",
                            "CYBERPANEL_ERROR"
                        );
                    }
                }

                return CreateUpdateErrorResult("Invalid response format from CyberPanel", "INVALID_RESPONSE");
            }
            catch (HttpRequestException ex)
            {
                return CreateUpdateErrorResult(
                    $"Network error while connecting to CyberPanel: {ex.Message}",
                    "NETWORK_ERROR"
                );
            }
            catch (JsonException ex)
            {
                return CreateUpdateErrorResult(
                    $"Failed to parse CyberPanel response: {ex.Message}",
                    "JSON_PARSE_ERROR"
                );
            }
            catch (Exception ex)
            {
                return CreateUpdateErrorResult(
                    $"Unexpected error: {ex.Message}",
                    "UNEXPECTED_ERROR"
                );
            }
        }

        public override async Task<AccountInfoResult> GetMailAccountInfoAsync(string accountId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(accountId))
                {
                    return CreateInfoErrorResult("Account ID (email address) is required", "INVALID_ACCOUNT_ID");
                }

                var emailParts = accountId.Split('@');
                if (emailParts.Length != 2)
                {
                    return CreateInfoErrorResult("Invalid email address format", "INVALID_EMAIL_FORMAT");
                }

                // CyberPanel doesn't have a dedicated getEmailDetails endpoint
                // We'll use listEmails and filter for the specific account
                var allAccounts = await ListMailAccountsAsync(emailParts[1]);
                var accountInfo = allAccounts.FirstOrDefault(a => a.Email?.Equals(accountId, StringComparison.OrdinalIgnoreCase) == true);

                if (accountInfo != null && accountInfo.Success)
                {
                    return accountInfo;
                }

                return CreateInfoErrorResult("Email account not found", "NOT_FOUND");
            }
            catch (Exception ex)
            {
                return CreateInfoErrorResult(
                    $"Unexpected error: {ex.Message}",
                    "UNEXPECTED_ERROR"
                );
            }
        }

        public override async Task<List<AccountInfoResult>> ListMailAccountsAsync(string domain)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(domain))
                {
                    return new List<AccountInfoResult>();
                }

                // CyberPanel listEmails API endpoint
                var endpoint = "/api/listEmails";

                // Build request payload
                var payload = new
                {
                    adminUser = _adminUsername,
                    adminPass = _adminPassword,
                    domainName = domain
                };

                var response = await _httpClient.PostAsJsonAsync(endpoint, payload);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new List<AccountInfoResult>();
                }

                // Parse CyberPanel response
                using var jsonDoc = JsonDocument.Parse(responseContent);
                var root = jsonDoc.RootElement;

                var results = new List<AccountInfoResult>();

                if (root.TryGetProperty("status", out var statusElement) && statusElement.GetInt32() == 1)
                {
                    // CyberPanel returns array of emails in "data" property
                    if (root.TryGetProperty("data", out var dataElement) && dataElement.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var email in dataElement.EnumerateArray())
                        {
                            var emailAddress = email.TryGetProperty("email", out var emailElement) ? emailElement.GetString() : null;
                            
                            if (!string.IsNullOrEmpty(emailAddress))
                            {
                                results.Add(new AccountInfoResult
                                {
                                    Success = true,
                                    AccountId = emailAddress,
                                    Email = emailAddress,
                                    Domain = domain,
                                    DiskQuotaMB = email.TryGetProperty("quota", out var quotaElement) ? quotaElement.GetInt32() : null,
                                    DiskUsageMB = email.TryGetProperty("diskUsed", out var usedElement) ? usedElement.GetInt32() : null
                                });
                            }
                        }
                    }
                }

                return results;
            }
            catch (Exception)
            {
                return new List<AccountInfoResult>();
            }
        }

        public override async Task<AccountUpdateResult> ChangeMailPasswordAsync(string accountId, string newPassword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(accountId))
                {
                    return CreateUpdateErrorResult("Account ID (email address) is required", "INVALID_ACCOUNT_ID");
                }

                if (string.IsNullOrWhiteSpace(newPassword))
                {
                    return CreateUpdateErrorResult("New password is required", "INVALID_PASSWORD");
                }

                var emailParts = accountId.Split('@');
                if (emailParts.Length != 2)
                {
                    return CreateUpdateErrorResult("Invalid email address format", "INVALID_EMAIL_FORMAT");
                }

                // CyberPanel changeEmailPassword API endpoint
                var endpoint = "/api/changeEmailPassword";

                // Build request payload
                var payload = new
                {
                    adminUser = _adminUsername,
                    adminPass = _adminPassword,
                    domainName = emailParts[1],
                    emailUsername = emailParts[0],
                    password = newPassword
                };

                var response = await _httpClient.PostAsJsonAsync(endpoint, payload);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return CreateUpdateErrorResult(
                        $"CyberPanel API request failed: {response.StatusCode} - {responseContent}",
                        response.StatusCode.ToString()
                    );
                }

                // Parse CyberPanel response
                using var jsonDoc = JsonDocument.Parse(responseContent);
                var root = jsonDoc.RootElement;

                if (root.TryGetProperty("status", out var statusElement))
                {
                    var status = statusElement.GetInt32();

                    if (status == 1)
                    {
                        var message = root.TryGetProperty("message", out var msgElement)
                            ? msgElement.GetString()
                            : "Email password changed successfully";

                        return new AccountUpdateResult
                        {
                            Success = true,
                            Message = message ?? "Email password changed successfully",
                            AccountId = accountId,
                            UpdatedField = "Password",
                            UpdatedDate = DateTime.UtcNow
                        };
                    }
                    else
                    {
                        var errorMessage = root.TryGetProperty("error_message", out var errMsgElement)
                            ? errMsgElement.GetString()
                            : root.TryGetProperty("message", out var msgElement2)
                                ? msgElement2.GetString()
                                : "Unknown error occurred";

                        return CreateUpdateErrorResult(
                            errorMessage ?? "Failed to change email password",
                            "CYBERPANEL_ERROR"
                        );
                    }
                }

                return CreateUpdateErrorResult("Invalid response format from CyberPanel", "INVALID_RESPONSE");
            }
            catch (HttpRequestException ex)
            {
                return CreateUpdateErrorResult(
                    $"Network error while connecting to CyberPanel: {ex.Message}",
                    "NETWORK_ERROR"
                );
            }
            catch (JsonException ex)
            {
                return CreateUpdateErrorResult(
                    $"Failed to parse CyberPanel response: {ex.Message}",
                    "JSON_PARSE_ERROR"
                );
            }
            catch (Exception ex)
            {
                return CreateUpdateErrorResult(
                    $"Unexpected error: {ex.Message}",
                    "UNEXPECTED_ERROR"
                );
            }
        }

        public override async Task<AccountUpdateResult> SetDiskQuotaAsync(string accountId, int quotaMB)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(accountId))
                {
                    return CreateUpdateErrorResult("Account ID (domain name) is required", "INVALID_ACCOUNT_ID");
                }

                if (quotaMB < 0)
                {
                    return CreateUpdateErrorResult("Quota must be a positive value", "INVALID_QUOTA");
                }

                // CyberPanel modifyWebsite API endpoint for changing disk quota
                var endpoint = "/api/changePackage";

                // Build request payload
                // Note: CyberPanel manages quotas through packages, not individual settings
                // For direct quota modification, we'd need to use modifyWebsite with package changes
                var payload = new
                {
                    adminUser = _adminUsername,
                    adminPass = _adminPassword,
                    websiteName = accountId,
                    diskSpace = quotaMB // Disk space in MB
                };

                var response = await _httpClient.PostAsJsonAsync(endpoint, payload);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return CreateUpdateErrorResult(
                        $"CyberPanel API request failed: {response.StatusCode} - {responseContent}",
                        response.StatusCode.ToString()
                    );
                }

                // Parse CyberPanel response
                using var jsonDoc = JsonDocument.Parse(responseContent);
                var root = jsonDoc.RootElement;

                if (root.TryGetProperty("status", out var statusElement))
                {
                    var status = statusElement.GetInt32();

                    if (status == 1)
                    {
                        var message = root.TryGetProperty("message", out var msgElement)
                            ? msgElement.GetString()
                            : "Disk quota updated successfully";

                        return new AccountUpdateResult
                        {
                            Success = true,
                            Message = message ?? "Disk quota updated successfully",
                            AccountId = accountId,
                            UpdatedField = "DiskQuota",
                            NewValue = quotaMB,
                            UpdatedDate = DateTime.UtcNow
                        };
                    }
                    else
                    {
                        var errorMessage = root.TryGetProperty("error_message", out var errMsgElement)
                            ? errMsgElement.GetString()
                            : root.TryGetProperty("message", out var msgElement2)
                                ? msgElement2.GetString()
                                : "Unknown error occurred";

                        return CreateUpdateErrorResult(
                            errorMessage ?? "Failed to set disk quota",
                            "CYBERPANEL_ERROR"
                        );
                    }
                }

                return CreateUpdateErrorResult("Invalid response format from CyberPanel", "INVALID_RESPONSE");
            }
            catch (HttpRequestException ex)
            {
                return CreateUpdateErrorResult(
                    $"Network error while connecting to CyberPanel: {ex.Message}",
                    "NETWORK_ERROR"
                );
            }
            catch (JsonException ex)
            {
                return CreateUpdateErrorResult(
                    $"Failed to parse CyberPanel response: {ex.Message}",
                    "JSON_PARSE_ERROR"
                );
            }
            catch (Exception ex)
            {
                return CreateUpdateErrorResult(
                    $"Unexpected error: {ex.Message}",
                    "UNEXPECTED_ERROR"
                );
            }
        }

        public override async Task<AccountUpdateResult> SetBandwidthLimitAsync(string accountId, int bandwidthMB)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(accountId))
                {
                    return CreateUpdateErrorResult("Account ID (domain name) is required", "INVALID_ACCOUNT_ID");
                }

                if (bandwidthMB < 0)
                {
                    return CreateUpdateErrorResult("Bandwidth must be a positive value", "INVALID_BANDWIDTH");
                }

                // CyberPanel modifyWebsite API endpoint for changing bandwidth
                var endpoint = "/api/changePackage";

                // Build request payload
                // Note: CyberPanel manages bandwidth through packages
                var payload = new
                {
                    adminUser = _adminUsername,
                    adminPass = _adminPassword,
                    websiteName = accountId,
                    bandwidth = bandwidthMB // Bandwidth in MB
                };

                var response = await _httpClient.PostAsJsonAsync(endpoint, payload);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return CreateUpdateErrorResult(
                        $"CyberPanel API request failed: {response.StatusCode} - {responseContent}",
                        response.StatusCode.ToString()
                    );
                }

                // Parse CyberPanel response
                using var jsonDoc = JsonDocument.Parse(responseContent);
                var root = jsonDoc.RootElement;

                if (root.TryGetProperty("status", out var statusElement))
                {
                    var status = statusElement.GetInt32();

                    if (status == 1)
                    {
                        var message = root.TryGetProperty("message", out var msgElement)
                            ? msgElement.GetString()
                            : "Bandwidth limit updated successfully";

                        return new AccountUpdateResult
                        {
                            Success = true,
                            Message = message ?? "Bandwidth limit updated successfully",
                            AccountId = accountId,
                            UpdatedField = "BandwidthLimit",
                            NewValue = bandwidthMB,
                            UpdatedDate = DateTime.UtcNow
                        };
                    }
                    else
                    {
                        var errorMessage = root.TryGetProperty("error_message", out var errMsgElement)
                            ? errMsgElement.GetString()
                            : root.TryGetProperty("message", out var msgElement2)
                                ? msgElement2.GetString()
                                : "Unknown error occurred";

                        return CreateUpdateErrorResult(
                            errorMessage ?? "Failed to set bandwidth limit",
                            "CYBERPANEL_ERROR"
                        );
                    }
                }

                return CreateUpdateErrorResult("Invalid response format from CyberPanel", "INVALID_RESPONSE");
            }
            catch (HttpRequestException ex)
            {
                return CreateUpdateErrorResult(
                    $"Network error while connecting to CyberPanel: {ex.Message}",
                    "NETWORK_ERROR"
                );
            }
            catch (JsonException ex)
            {
                return CreateUpdateErrorResult(
                    $"Failed to parse CyberPanel response: {ex.Message}",
                    "JSON_PARSE_ERROR"
                );
            }
            catch (Exception ex)
            {
                return CreateUpdateErrorResult(
                    $"Unexpected error: {ex.Message}",
                    "UNEXPECTED_ERROR"
                );
            }
        }
    }
}

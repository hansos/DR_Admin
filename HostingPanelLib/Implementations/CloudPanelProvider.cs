using HostingPanelLib.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace HostingPanelLib.Implementations
{
    public class CloudPanelProvider : BaseHostingPanel
    {
        private readonly string _apiKey;
        private readonly int _port;
        private readonly bool _useHttps;

        public CloudPanelProvider(string apiUrl, string apiKey, int port, bool useHttps)
            : base(apiUrl)
        {
            _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
            _port = port;
            _useHttps = useHttps;

            _httpClient.DefaultRequestHeaders.Add("X-API-Key", _apiKey);
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
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

                if (string.IsNullOrWhiteSpace(request.Password))
                {
                    return CreateHostingErrorResult("Password is required", "INVALID_PASSWORD");
                }

                // CloudPanel API endpoint for site creation
                var endpoint = "/api/v1/sites";

                // Build request payload
                var payload = new
                {
                    domainName = request.Domain,
                    userName = request.Username,
                    userPassword = request.Password,
                    userEmail = request.Email ?? "",
                    phpVersion = request.AdditionalSettings?.GetValueOrDefault("phpVersion") ?? "8.2",
                    vhostTemplate = request.AdditionalSettings?.GetValueOrDefault("vhostTemplate") ?? "Generic",
                    siteUser = request.Username,
                    siteUserPassword = request.Password
                };

                var response = await _httpClient.PostAsJsonAsync(endpoint, payload);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return CreateHostingErrorResult(
                        $"CloudPanel API request failed: {response.StatusCode} - {responseContent}",
                        response.StatusCode.ToString()
                    );
                }

                // Parse CloudPanel response
                using var jsonDoc = JsonDocument.Parse(responseContent);
                var root = jsonDoc.RootElement;

                // CloudPanel typically returns success with site data
                if (root.TryGetProperty("success", out var successElement) && successElement.GetBoolean())
                {
                    var siteId = root.TryGetProperty("data", out var dataElement) && 
                                 dataElement.TryGetProperty("id", out var idElement)
                        ? idElement.GetInt32().ToString()
                        : request.Domain;

                    var message = root.TryGetProperty("message", out var msgElement)
                        ? msgElement.GetString()
                        : "Site created successfully";

                    return new HostingAccountResult
                    {
                        Success = true,
                        Message = message ?? "Site created successfully",
                        AccountId = siteId,
                        Username = request.Username,
                        Domain = request.Domain,
                        PlanName = request.Plan,
                        CreatedDate = DateTime.UtcNow
                    };
                }
                else if (root.TryGetProperty("error", out var errorElement))
                {
                    var errorMessage = errorElement.GetString() ?? "Unknown error occurred";
                    return CreateHostingErrorResult(errorMessage, "CLOUDPANEL_ERROR");
                }
                else if (root.TryGetProperty("message", out var messageElement))
                {
                    var message = messageElement.GetString() ?? "Unknown error occurred";
                    return CreateHostingErrorResult(message, "CLOUDPANEL_ERROR");
                }

                return CreateHostingErrorResult("Invalid response format from CloudPanel", "INVALID_RESPONSE");
            }
            catch (HttpRequestException ex)
            {
                return CreateHostingErrorResult(
                    $"Network error while connecting to CloudPanel: {ex.Message}",
                    "NETWORK_ERROR"
                );
            }
            catch (JsonException ex)
            {
                return CreateHostingErrorResult(
                    $"Failed to parse CloudPanel response: {ex.Message}",
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
                    return CreateUpdateErrorResult("Account ID (site ID) is required", "INVALID_ACCOUNT_ID");
                }

                // CloudPanel API endpoint for site update
                var endpoint = $"/api/v1/sites/{accountId}";

                // Build request payload - only include fields to update
                var payload = new Dictionary<string, object>();

                if (!string.IsNullOrWhiteSpace(request.Domain))
                    payload["domainName"] = request.Domain;
                
                if (request.AdditionalSettings?.ContainsKey("phpVersion") == true)
                    payload["phpVersion"] = request.AdditionalSettings["phpVersion"];

                if (request.AdditionalSettings?.ContainsKey("vhostTemplate") == true)
                    payload["vhostTemplate"] = request.AdditionalSettings["vhostTemplate"];

                var response = await _httpClient.PutAsJsonAsync(endpoint, payload);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return CreateUpdateErrorResult(
                        $"CloudPanel API request failed: {response.StatusCode} - {responseContent}",
                        response.StatusCode.ToString()
                    );
                }

                // Parse CloudPanel response
                using var jsonDoc = JsonDocument.Parse(responseContent);
                var root = jsonDoc.RootElement;

                if (root.TryGetProperty("success", out var successElement) && successElement.GetBoolean())
                {
                    var message = root.TryGetProperty("message", out var msgElement)
                        ? msgElement.GetString()
                        : "Site updated successfully";

                    return new AccountUpdateResult
                    {
                        Success = true,
                        Message = message ?? "Site updated successfully",
                        AccountId = accountId,
                        UpdatedField = "SiteConfiguration",
                        UpdatedDate = DateTime.UtcNow
                    };
                }
                else if (root.TryGetProperty("error", out var errorElement))
                {
                    var errorMessage = errorElement.GetString() ?? "Unknown error occurred";
                    return CreateUpdateErrorResult(errorMessage, "CLOUDPANEL_ERROR");
                }

                return CreateUpdateErrorResult("Invalid response format from CloudPanel", "INVALID_RESPONSE");
            }
            catch (HttpRequestException ex)
            {
                return CreateUpdateErrorResult(
                    $"Network error while connecting to CloudPanel: {ex.Message}",
                    "NETWORK_ERROR"
                );
            }
            catch (JsonException ex)
            {
                return CreateUpdateErrorResult(
                    $"Failed to parse CloudPanel response: {ex.Message}",
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
                    return CreateUpdateErrorResult("Account ID (site ID) is required", "INVALID_ACCOUNT_ID");
                }

                // CloudPanel API endpoint for site suspension
                var endpoint = $"/api/v1/sites/{accountId}/disable";

                var response = await _httpClient.PostAsync(endpoint, null);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return CreateUpdateErrorResult(
                        $"CloudPanel API request failed: {response.StatusCode} - {responseContent}",
                        response.StatusCode.ToString()
                    );
                }

                // Parse CloudPanel response
                using var jsonDoc = JsonDocument.Parse(responseContent);
                var root = jsonDoc.RootElement;

                if (root.TryGetProperty("success", out var successElement) && successElement.GetBoolean())
                {
                    var message = root.TryGetProperty("message", out var msgElement)
                        ? msgElement.GetString()
                        : "Site suspended successfully";

                    return new AccountUpdateResult
                    {
                        Success = true,
                        Message = message ?? "Site suspended successfully",
                        AccountId = accountId,
                        UpdatedField = "Status",
                        OldValue = "Active",
                        NewValue = "Suspended",
                        UpdatedDate = DateTime.UtcNow
                    };
                }
                else if (root.TryGetProperty("error", out var errorElement))
                {
                    var errorMessage = errorElement.GetString() ?? "Unknown error occurred";
                    return CreateUpdateErrorResult(errorMessage, "CLOUDPANEL_ERROR");
                }

                return CreateUpdateErrorResult("Invalid response format from CloudPanel", "INVALID_RESPONSE");
            }
            catch (HttpRequestException ex)
            {
                return CreateUpdateErrorResult(
                    $"Network error while connecting to CloudPanel: {ex.Message}",
                    "NETWORK_ERROR"
                );
            }
            catch (JsonException ex)
            {
                return CreateUpdateErrorResult(
                    $"Failed to parse CloudPanel response: {ex.Message}",
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
                    return CreateUpdateErrorResult("Account ID (site ID) is required", "INVALID_ACCOUNT_ID");
                }

                // CloudPanel API endpoint for site activation
                var endpoint = $"/api/v1/sites/{accountId}/enable";

                var response = await _httpClient.PostAsync(endpoint, null);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return CreateUpdateErrorResult(
                        $"CloudPanel API request failed: {response.StatusCode} - {responseContent}",
                        response.StatusCode.ToString()
                    );
                }

                // Parse CloudPanel response
                using var jsonDoc = JsonDocument.Parse(responseContent);
                var root = jsonDoc.RootElement;

                if (root.TryGetProperty("success", out var successElement) && successElement.GetBoolean())
                {
                    var message = root.TryGetProperty("message", out var msgElement)
                        ? msgElement.GetString()
                        : "Site unsuspended successfully";

                    return new AccountUpdateResult
                    {
                        Success = true,
                        Message = message ?? "Site unsuspended successfully",
                        AccountId = accountId,
                        UpdatedField = "Status",
                        OldValue = "Suspended",
                        NewValue = "Active",
                        UpdatedDate = DateTime.UtcNow
                    };
                }
                else if (root.TryGetProperty("error", out var errorElement))
                {
                    var errorMessage = errorElement.GetString() ?? "Unknown error occurred";
                    return CreateUpdateErrorResult(errorMessage, "CLOUDPANEL_ERROR");
                }

                return CreateUpdateErrorResult("Invalid response format from CloudPanel", "INVALID_RESPONSE");
            }
            catch (HttpRequestException ex)
            {
                return CreateUpdateErrorResult(
                    $"Network error while connecting to CloudPanel: {ex.Message}",
                    "NETWORK_ERROR"
                );
            }
            catch (JsonException ex)
            {
                return CreateUpdateErrorResult(
                    $"Failed to parse CloudPanel response: {ex.Message}",
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
                    return CreateUpdateErrorResult("Account ID (site ID) is required", "INVALID_ACCOUNT_ID");
                }

                // CloudPanel API endpoint for site deletion
                var endpoint = $"/api/v1/sites/{accountId}";

                var response = await _httpClient.DeleteAsync(endpoint);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return CreateUpdateErrorResult(
                        $"CloudPanel API request failed: {response.StatusCode} - {responseContent}",
                        response.StatusCode.ToString()
                    );
                }

                // Parse CloudPanel response
                using var jsonDoc = JsonDocument.Parse(responseContent);
                var root = jsonDoc.RootElement;

                if (root.TryGetProperty("success", out var successElement) && successElement.GetBoolean())
                {
                    var message = root.TryGetProperty("message", out var msgElement)
                        ? msgElement.GetString()
                        : "Site deleted successfully";

                    return new AccountUpdateResult
                    {
                        Success = true,
                        Message = message ?? "Site deleted successfully",
                        AccountId = accountId,
                        UpdatedField = "Status",
                        OldValue = "Active",
                        NewValue = "Deleted",
                        UpdatedDate = DateTime.UtcNow
                    };
                }
                else if (root.TryGetProperty("error", out var errorElement))
                {
                    var errorMessage = errorElement.GetString() ?? "Unknown error occurred";
                    return CreateUpdateErrorResult(errorMessage, "CLOUDPANEL_ERROR");
                }

                return CreateUpdateErrorResult("Invalid response format from CloudPanel", "INVALID_RESPONSE");
            }
            catch (HttpRequestException ex)
            {
                return CreateUpdateErrorResult(
                    $"Network error while connecting to CloudPanel: {ex.Message}",
                    "NETWORK_ERROR"
                );
            }
            catch (JsonException ex)
            {
                return CreateUpdateErrorResult(
                    $"Failed to parse CloudPanel response: {ex.Message}",
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
                    return CreateInfoErrorResult("Account ID (site ID) is required", "INVALID_ACCOUNT_ID");
                }

                // CloudPanel API endpoint for site details
                var endpoint = $"/api/v1/sites/{accountId}";

                var response = await _httpClient.GetAsync(endpoint);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return CreateInfoErrorResult(
                        $"CloudPanel API request failed: {response.StatusCode} - {responseContent}",
                        response.StatusCode.ToString()
                    );
                }

                // Parse CloudPanel response
                using var jsonDoc = JsonDocument.Parse(responseContent);
                var root = jsonDoc.RootElement;

                if (root.TryGetProperty("success", out var successElement) && successElement.GetBoolean() &&
                    root.TryGetProperty("data", out var data))
                {
                    return new AccountInfoResult
                    {
                        Success = true,
                        Message = "Site information retrieved successfully",
                        AccountId = accountId,
                        Domain = data.TryGetProperty("domainName", out var domainElement) ? domainElement.GetString() : null,
                        Username = data.TryGetProperty("userName", out var userElement) ? userElement.GetString() : null,
                        Email = data.TryGetProperty("userEmail", out var emailElement) ? emailElement.GetString() : null,
                        Status = data.TryGetProperty("status", out var statusElement) ? statusElement.GetString() : "Unknown",
                        DiskUsageMB = data.TryGetProperty("diskUsed", out var diskUsedElement) ? (int?)diskUsedElement.GetInt32() : null,
                        DiskQuotaMB = data.TryGetProperty("diskQuota", out var diskQuotaElement) ? (int?)diskQuotaElement.GetInt32() : null,
                        IpAddress = data.TryGetProperty("ipAddress", out var ipElement) ? ipElement.GetString() : null,
                        CreatedDate = data.TryGetProperty("createdAt", out var createdElement) 
                            ? DateTime.Parse(createdElement.GetString() ?? DateTime.UtcNow.ToString())
                            : null,
                        AdditionalInfo = new Dictionary<string, object>
                        {
                            ["phpVersion"] = data.TryGetProperty("phpVersion", out var phpElement) ? phpElement.GetString() ?? "Unknown" : "Unknown",
                            ["vhostTemplate"] = data.TryGetProperty("vhostTemplate", out var templateElement) ? templateElement.GetString() ?? "Generic" : "Generic"
                        }
                    };
                }
                else if (root.TryGetProperty("error", out var errorElement))
                {
                    var errorMessage = errorElement.GetString() ?? "Unknown error occurred";
                    return CreateInfoErrorResult(errorMessage, "CLOUDPANEL_ERROR");
                }

                return CreateInfoErrorResult("Invalid response format from CloudPanel", "INVALID_RESPONSE");
            }
            catch (HttpRequestException ex)
            {
                return CreateInfoErrorResult(
                    $"Network error while connecting to CloudPanel: {ex.Message}",
                    "NETWORK_ERROR"
                );
            }
            catch (JsonException ex)
            {
                return CreateInfoErrorResult(
                    $"Failed to parse CloudPanel response: {ex.Message}",
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
                // CloudPanel API endpoint for listing sites
                var endpoint = "/api/v1/sites";

                var response = await _httpClient.GetAsync(endpoint);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new List<AccountInfoResult>();
                }

                // Parse CloudPanel response
                using var jsonDoc = JsonDocument.Parse(responseContent);
                var root = jsonDoc.RootElement;

                var results = new List<AccountInfoResult>();

                if (root.TryGetProperty("success", out var successElement) && successElement.GetBoolean() &&
                    root.TryGetProperty("data", out var dataArray) && dataArray.ValueKind == JsonValueKind.Array)
                {
                    foreach (var site in dataArray.EnumerateArray())
                    {
                        var siteId = site.TryGetProperty("id", out var idElement) 
                            ? idElement.GetInt32().ToString() 
                            : null;

                        if (!string.IsNullOrEmpty(siteId))
                        {
                            results.Add(new AccountInfoResult
                            {
                                Success = true,
                                AccountId = siteId,
                                Domain = site.TryGetProperty("domainName", out var domainElement) ? domainElement.GetString() : null,
                                Username = site.TryGetProperty("userName", out var userElement) ? userElement.GetString() : null,
                                Email = site.TryGetProperty("userEmail", out var emailElement) ? emailElement.GetString() : null,
                                Status = site.TryGetProperty("status", out var statusElement) ? statusElement.GetString() : "Unknown",
                                DiskUsageMB = site.TryGetProperty("diskUsed", out var diskUsedElement) ? (int?)diskUsedElement.GetInt32() : null,
                                DiskQuotaMB = site.TryGetProperty("diskQuota", out var diskQuotaElement) ? (int?)diskQuotaElement.GetInt32() : null,
                                IpAddress = site.TryGetProperty("ipAddress", out var ipElement) ? ipElement.GetString() : null
                            });
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

                var emailParts = request.EmailAddress.Split('@');
                if (emailParts.Length != 2)
                {
                    return CreateMailErrorResult("Invalid email address format", "INVALID_EMAIL_FORMAT");
                }

                // CloudPanel API endpoint for email account creation
                var endpoint = "/api/v1/email-accounts";

                // Build request payload
                var payload = new
                {
                    emailAddress = request.EmailAddress,
                    password = request.Password,
                    quota = request.QuotaMB ?? 1024, // Default 1GB quota
                    domain = request.Domain
                };

                var response = await _httpClient.PostAsJsonAsync(endpoint, payload);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return CreateMailErrorResult(
                        $"CloudPanel API request failed: {response.StatusCode} - {responseContent}",
                        response.StatusCode.ToString()
                    );
                }

                // Parse CloudPanel response
                using var jsonDoc = JsonDocument.Parse(responseContent);
                var root = jsonDoc.RootElement;

                if (root.TryGetProperty("success", out var successElement) && successElement.GetBoolean())
                {
                    var message = root.TryGetProperty("message", out var msgElement)
                        ? msgElement.GetString()
                        : "Email account created successfully";

                    var accountId = root.TryGetProperty("data", out var dataElement) && 
                                   dataElement.TryGetProperty("id", out var idElement)
                        ? idElement.GetInt32().ToString()
                        : request.EmailAddress;

                    return new MailAccountResult
                    {
                        Success = true,
                        Message = message ?? "Email account created successfully",
                        AccountId = accountId,
                        EmailAddress = request.EmailAddress,
                        Domain = request.Domain,
                        QuotaMB = request.QuotaMB ?? 1024,
                        CreatedDate = DateTime.UtcNow
                    };
                }
                else if (root.TryGetProperty("error", out var errorElement))
                {
                    var errorMessage = errorElement.GetString() ?? "Unknown error occurred";
                    return CreateMailErrorResult(errorMessage, "CLOUDPANEL_ERROR");
                }

                return CreateMailErrorResult("Invalid response format from CloudPanel", "INVALID_RESPONSE");
            }
            catch (HttpRequestException ex)
            {
                return CreateMailErrorResult(
                    $"Network error while connecting to CloudPanel: {ex.Message}",
                    "NETWORK_ERROR"
                );
            }
            catch (JsonException ex)
            {
                return CreateMailErrorResult(
                    $"Failed to parse CloudPanel response: {ex.Message}",
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
                    return CreateUpdateErrorResult("Account ID (email account ID) is required", "INVALID_ACCOUNT_ID");
                }

                // CloudPanel API endpoint for email account update
                var endpoint = $"/api/v1/email-accounts/{accountId}";

                // Build request payload
                var payload = new Dictionary<string, object>();

                if (request.QuotaMB.HasValue)
                    payload["quota"] = request.QuotaMB.Value;

                if (!string.IsNullOrWhiteSpace(request.Password))
                    payload["password"] = request.Password;

                var response = await _httpClient.PutAsJsonAsync(endpoint, payload);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return CreateUpdateErrorResult(
                        $"CloudPanel API request failed: {response.StatusCode} - {responseContent}",
                        response.StatusCode.ToString()
                    );
                }

                // Parse CloudPanel response
                using var jsonDoc = JsonDocument.Parse(responseContent);
                var root = jsonDoc.RootElement;

                if (root.TryGetProperty("success", out var successElement) && successElement.GetBoolean())
                {
                    var message = root.TryGetProperty("message", out var msgElement)
                        ? msgElement.GetString()
                        : "Email account updated successfully";

                    return new AccountUpdateResult
                    {
                        Success = true,
                        Message = message ?? "Email account updated successfully",
                        AccountId = accountId,
                        UpdatedField = "EmailConfiguration",
                        NewValue = request.QuotaMB,
                        UpdatedDate = DateTime.UtcNow
                    };
                }
                else if (root.TryGetProperty("error", out var errorElement))
                {
                    var errorMessage = errorElement.GetString() ?? "Unknown error occurred";
                    return CreateUpdateErrorResult(errorMessage, "CLOUDPANEL_ERROR");
                }

                return CreateUpdateErrorResult("Invalid response format from CloudPanel", "INVALID_RESPONSE");
            }
            catch (HttpRequestException ex)
            {
                return CreateUpdateErrorResult(
                    $"Network error while connecting to CloudPanel: {ex.Message}",
                    "NETWORK_ERROR"
                );
            }
            catch (JsonException ex)
            {
                return CreateUpdateErrorResult(
                    $"Failed to parse CloudPanel response: {ex.Message}",
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
                    return CreateUpdateErrorResult("Account ID (email account ID) is required", "INVALID_ACCOUNT_ID");
                }

                // CloudPanel API endpoint for email account deletion
                var endpoint = $"/api/v1/email-accounts/{accountId}";

                var response = await _httpClient.DeleteAsync(endpoint);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return CreateUpdateErrorResult(
                        $"CloudPanel API request failed: {response.StatusCode} - {responseContent}",
                        response.StatusCode.ToString()
                    );
                }

                // Parse CloudPanel response
                using var jsonDoc = JsonDocument.Parse(responseContent);
                var root = jsonDoc.RootElement;

                if (root.TryGetProperty("success", out var successElement) && successElement.GetBoolean())
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
                else if (root.TryGetProperty("error", out var errorElement))
                {
                    var errorMessage = errorElement.GetString() ?? "Unknown error occurred";
                    return CreateUpdateErrorResult(errorMessage, "CLOUDPANEL_ERROR");
                }

                return CreateUpdateErrorResult("Invalid response format from CloudPanel", "INVALID_RESPONSE");
            }
            catch (HttpRequestException ex)
            {
                return CreateUpdateErrorResult(
                    $"Network error while connecting to CloudPanel: {ex.Message}",
                    "NETWORK_ERROR"
                );
            }
            catch (JsonException ex)
            {
                return CreateUpdateErrorResult(
                    $"Failed to parse CloudPanel response: {ex.Message}",
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
                    return CreateInfoErrorResult("Account ID (email account ID) is required", "INVALID_ACCOUNT_ID");
                }

                // CloudPanel API endpoint for email account details
                var endpoint = $"/api/v1/email-accounts/{accountId}";

                var response = await _httpClient.GetAsync(endpoint);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return CreateInfoErrorResult(
                        $"CloudPanel API request failed: {response.StatusCode} - {responseContent}",
                        response.StatusCode.ToString()
                    );
                }

                // Parse CloudPanel response
                using var jsonDoc = JsonDocument.Parse(responseContent);
                var root = jsonDoc.RootElement;

                if (root.TryGetProperty("success", out var successElement) && successElement.GetBoolean() &&
                    root.TryGetProperty("data", out var data))
                {
                    return new AccountInfoResult
                    {
                        Success = true,
                        Message = "Email account information retrieved successfully",
                        AccountId = accountId,
                        Email = data.TryGetProperty("emailAddress", out var emailElement) ? emailElement.GetString() : null,
                        Domain = data.TryGetProperty("domain", out var domainElement) ? domainElement.GetString() : null,
                        DiskQuotaMB = data.TryGetProperty("quota", out var quotaElement) ? (int?)quotaElement.GetInt32() : null,
                        DiskUsageMB = data.TryGetProperty("diskUsed", out var usedElement) ? (int?)usedElement.GetInt32() : null,
                        CreatedDate = data.TryGetProperty("createdAt", out var createdElement) 
                            ? DateTime.Parse(createdElement.GetString() ?? DateTime.UtcNow.ToString())
                            : null
                    };
                }
                else if (root.TryGetProperty("error", out var errorElement))
                {
                    var errorMessage = errorElement.GetString() ?? "Unknown error occurred";
                    return CreateInfoErrorResult(errorMessage, "CLOUDPANEL_ERROR");
                }

                return CreateInfoErrorResult("Invalid response format from CloudPanel", "INVALID_RESPONSE");
            }
            catch (HttpRequestException ex)
            {
                return CreateInfoErrorResult(
                    $"Network error while connecting to CloudPanel: {ex.Message}",
                    "NETWORK_ERROR"
                );
            }
            catch (JsonException ex)
            {
                return CreateInfoErrorResult(
                    $"Failed to parse CloudPanel response: {ex.Message}",
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

        public override async Task<List<AccountInfoResult>> ListMailAccountsAsync(string domain)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(domain))
                {
                    return new List<AccountInfoResult>();
                }

                // CloudPanel API endpoint for listing email accounts
                var endpoint = $"/api/v1/email-accounts?domain={domain}";

                var response = await _httpClient.GetAsync(endpoint);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new List<AccountInfoResult>();
                }

                // Parse CloudPanel response
                using var jsonDoc = JsonDocument.Parse(responseContent);
                var root = jsonDoc.RootElement;

                var results = new List<AccountInfoResult>();

                if (root.TryGetProperty("success", out var successElement) && successElement.GetBoolean() &&
                    root.TryGetProperty("data", out var dataArray) && dataArray.ValueKind == JsonValueKind.Array)
                {
                    foreach (var emailAccount in dataArray.EnumerateArray())
                    {
                        var accountId = emailAccount.TryGetProperty("id", out var idElement) 
                            ? idElement.GetInt32().ToString() 
                            : null;

                        if (!string.IsNullOrEmpty(accountId))
                        {
                            results.Add(new AccountInfoResult
                            {
                                Success = true,
                                AccountId = accountId,
                                Email = emailAccount.TryGetProperty("emailAddress", out var emailElement) ? emailElement.GetString() : null,
                                Domain = domain,
                                DiskQuotaMB = emailAccount.TryGetProperty("quota", out var quotaElement) ? (int?)quotaElement.GetInt32() : null,
                                DiskUsageMB = emailAccount.TryGetProperty("diskUsed", out var usedElement) ? (int?)usedElement.GetInt32() : null
                            });
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
                    return CreateUpdateErrorResult("Account ID (email account ID) is required", "INVALID_ACCOUNT_ID");
                }

                if (string.IsNullOrWhiteSpace(newPassword))
                {
                    return CreateUpdateErrorResult("New password is required", "INVALID_PASSWORD");
                }

                // CloudPanel API endpoint for changing email password
                var endpoint = $"/api/v1/email-accounts/{accountId}/password";

                // Build request payload
                var payload = new
                {
                    password = newPassword
                };

                var response = await _httpClient.PutAsJsonAsync(endpoint, payload);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return CreateUpdateErrorResult(
                        $"CloudPanel API request failed: {response.StatusCode} - {responseContent}",
                        response.StatusCode.ToString()
                    );
                }

                // Parse CloudPanel response
                using var jsonDoc = JsonDocument.Parse(responseContent);
                var root = jsonDoc.RootElement;

                if (root.TryGetProperty("success", out var successElement) && successElement.GetBoolean())
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
                else if (root.TryGetProperty("error", out var errorElement))
                {
                    var errorMessage = errorElement.GetString() ?? "Unknown error occurred";
                    return CreateUpdateErrorResult(errorMessage, "CLOUDPANEL_ERROR");
                }

                return CreateUpdateErrorResult("Invalid response format from CloudPanel", "INVALID_RESPONSE");
            }
            catch (HttpRequestException ex)
            {
                return CreateUpdateErrorResult(
                    $"Network error while connecting to CloudPanel: {ex.Message}",
                    "NETWORK_ERROR"
                );
            }
            catch (JsonException ex)
            {
                return CreateUpdateErrorResult(
                    $"Failed to parse CloudPanel response: {ex.Message}",
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
                    return CreateUpdateErrorResult("Account ID (site ID) is required", "INVALID_ACCOUNT_ID");
                }

                if (quotaMB < 0)
                {
                    return CreateUpdateErrorResult("Quota must be a positive value", "INVALID_QUOTA");
                }

                // CloudPanel API endpoint for updating site quota
                var endpoint = $"/api/v1/sites/{accountId}/quota";

                // Build request payload
                var payload = new
                {
                    quota = quotaMB
                };

                var response = await _httpClient.PutAsJsonAsync(endpoint, payload);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return CreateUpdateErrorResult(
                        $"CloudPanel API request failed: {response.StatusCode} - {responseContent}",
                        response.StatusCode.ToString()
                    );
                }

                // Parse CloudPanel response
                using var jsonDoc = JsonDocument.Parse(responseContent);
                var root = jsonDoc.RootElement;

                if (root.TryGetProperty("success", out var successElement) && successElement.GetBoolean())
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
                else if (root.TryGetProperty("error", out var errorElement))
                {
                    var errorMessage = errorElement.GetString() ?? "Unknown error occurred";
                    return CreateUpdateErrorResult(errorMessage, "CLOUDPANEL_ERROR");
                }

                return CreateUpdateErrorResult("Invalid response format from CloudPanel", "INVALID_RESPONSE");
            }
            catch (HttpRequestException ex)
            {
                return CreateUpdateErrorResult(
                    $"Network error while connecting to CloudPanel: {ex.Message}",
                    "NETWORK_ERROR"
                );
            }
            catch (JsonException ex)
            {
                return CreateUpdateErrorResult(
                    $"Failed to parse CloudPanel response: {ex.Message}",
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
                    return CreateUpdateErrorResult("Account ID (site ID) is required", "INVALID_ACCOUNT_ID");
                }

                if (bandwidthMB < 0)
                {
                    return CreateUpdateErrorResult("Bandwidth must be a positive value", "INVALID_BANDWIDTH");
                }

                // Note: CloudPanel does not natively support per-site bandwidth limits
                // This would typically need to be implemented via server-level tools like nginx limit_rate
                // or external monitoring tools
                
                // For now, we'll return a not supported response
                // If CloudPanel adds this feature in the future, update this endpoint
                await Task.CompletedTask;
                return CreateUpdateErrorResult(
                    "CloudPanel does not support per-site bandwidth limits through the API. " +
                    "Bandwidth management should be configured at the server level using nginx or similar tools.",
                    "NOT_SUPPORTED"
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

        public override async Task<DatabaseResult> CreateDatabaseAsync(DatabaseRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.DatabaseName))
                {
                    return CreateDatabaseErrorResult("Database name is required", "INVALID_DATABASE_NAME");
                }

                if (string.IsNullOrWhiteSpace(request.Domain))
                {
                    return CreateDatabaseErrorResult("Domain is required", "INVALID_DOMAIN");
                }

                // CloudPanel API endpoint for database creation
                var endpoint = "/api/v1/databases";

                // Build request payload
                var payload = new
                {
                    databaseName = request.DatabaseName,
                    domain = request.Domain,
                    databaseType = request.DatabaseType ?? "mysql",
                    databaseUser = request.Username ?? request.DatabaseName,
                    databasePassword = request.Password ?? Guid.NewGuid().ToString("N").Substring(0, 16)
                };

                var response = await _httpClient.PostAsJsonAsync(endpoint, payload);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return CreateDatabaseErrorResult(
                        $"CloudPanel API request failed: {response.StatusCode} - {responseContent}",
                        response.StatusCode.ToString()
                    );
                }

                // Parse CloudPanel response
                using var jsonDoc = JsonDocument.Parse(responseContent);
                var root = jsonDoc.RootElement;

                if (root.TryGetProperty("success", out var successElement) && successElement.GetBoolean())
                {
                    var message = root.TryGetProperty("message", out var msgElement)
                        ? msgElement.GetString()
                        : "Database created successfully";

                    var databaseId = root.TryGetProperty("data", out var dataElement) && 
                                    dataElement.TryGetProperty("id", out var idElement)
                        ? idElement.GetInt32().ToString()
                        : request.DatabaseName;

                    return new DatabaseResult
                    {
                        Success = true,
                        Message = message ?? "Database created successfully",
                        DatabaseId = databaseId,
                        DatabaseName = request.DatabaseName,
                        DatabaseType = request.DatabaseType ?? "mysql",
                        Server = "localhost",
                        Port = 3306,
                        Username = request.Username ?? request.DatabaseName,
                        CreatedDate = DateTime.UtcNow
                    };
                }
                else if (root.TryGetProperty("error", out var errorElement))
                {
                    var errorMessage = errorElement.GetString() ?? "Unknown error occurred";
                    return CreateDatabaseErrorResult(errorMessage, "CLOUDPANEL_ERROR");
                }

                return CreateDatabaseErrorResult("Invalid response format from CloudPanel", "INVALID_RESPONSE");
            }
            catch (HttpRequestException ex)
            {
                return CreateDatabaseErrorResult(
                    $"Network error while connecting to CloudPanel: {ex.Message}",
                    "NETWORK_ERROR"
                );
            }
            catch (JsonException ex)
            {
                return CreateDatabaseErrorResult(
                    $"Failed to parse CloudPanel response: {ex.Message}",
                    "JSON_PARSE_ERROR"
                );
            }
            catch (Exception ex)
            {
                return CreateDatabaseErrorResult(
                    $"Unexpected error: {ex.Message}",
                    "UNEXPECTED_ERROR"
                );
            }
        }

        public override async Task<AccountUpdateResult> DeleteDatabaseAsync(string databaseId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(databaseId))
                {
                    return CreateUpdateErrorResult("Database ID is required", "INVALID_DATABASE_ID");
                }

                // CloudPanel API endpoint for database deletion
                var endpoint = $"/api/v1/databases/{databaseId}";

                var response = await _httpClient.DeleteAsync(endpoint);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return CreateUpdateErrorResult(
                        $"CloudPanel API request failed: {response.StatusCode} - {responseContent}",
                        response.StatusCode.ToString()
                    );
                }

                // Parse CloudPanel response
                using var jsonDoc = JsonDocument.Parse(responseContent);
                var root = jsonDoc.RootElement;

                if (root.TryGetProperty("success", out var successElement) && successElement.GetBoolean())
                {
                    var message = root.TryGetProperty("message", out var msgElement)
                        ? msgElement.GetString()
                        : "Database deleted successfully";

                    return new AccountUpdateResult
                    {
                        Success = true,
                        Message = message ?? "Database deleted successfully",
                        AccountId = databaseId,
                        UpdatedField = "Status",
                        OldValue = "Active",
                        NewValue = "Deleted",
                        UpdatedDate = DateTime.UtcNow
                    };
                }
                else if (root.TryGetProperty("error", out var errorElement))
                {
                    var errorMessage = errorElement.GetString() ?? "Unknown error occurred";
                    return CreateUpdateErrorResult(errorMessage, "CLOUDPANEL_ERROR");
                }

                return CreateUpdateErrorResult("Invalid response format from CloudPanel", "INVALID_RESPONSE");
            }
            catch (HttpRequestException ex)
            {
                return CreateUpdateErrorResult(
                    $"Network error while connecting to CloudPanel: {ex.Message}",
                    "NETWORK_ERROR"
                );
            }
            catch (JsonException ex)
            {
                return CreateUpdateErrorResult(
                    $"Failed to parse CloudPanel response: {ex.Message}",
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

        public override async Task<AccountInfoResult> GetDatabaseInfoAsync(string databaseId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(databaseId))
                {
                    return CreateInfoErrorResult("Database ID is required", "INVALID_DATABASE_ID");
                }

                // CloudPanel API endpoint for database details
                var endpoint = $"/api/v1/databases/{databaseId}";

                var response = await _httpClient.GetAsync(endpoint);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return CreateInfoErrorResult(
                        $"CloudPanel API request failed: {response.StatusCode} - {responseContent}",
                        response.StatusCode.ToString()
                    );
                }

                // Parse CloudPanel response
                using var jsonDoc = JsonDocument.Parse(responseContent);
                var root = jsonDoc.RootElement;

                if (root.TryGetProperty("success", out var successElement) && successElement.GetBoolean() &&
                    root.TryGetProperty("data", out var data))
                {
                    return new AccountInfoResult
                    {
                        Success = true,
                        Message = "Database information retrieved successfully",
                        AccountId = databaseId,
                        DatabaseName = data.TryGetProperty("databaseName", out var dbNameElement) ? dbNameElement.GetString() : null,
                        DatabaseUser = data.TryGetProperty("databaseUser", out var userElement) ? userElement.GetString() : null,
                        DatabaseType = data.TryGetProperty("databaseType", out var typeElement) ? typeElement.GetString() : "mysql",
                        CreatedDate = data.TryGetProperty("createdAt", out var createdElement) 
                            ? DateTime.Parse(createdElement.GetString() ?? DateTime.UtcNow.ToString())
                            : null
                    };
                }
                else if (root.TryGetProperty("error", out var errorElement))
                {
                    var errorMessage = errorElement.GetString() ?? "Unknown error occurred";
                    return CreateInfoErrorResult(errorMessage, "CLOUDPANEL_ERROR");
                }

                return CreateInfoErrorResult("Invalid response format from CloudPanel", "INVALID_RESPONSE");
            }
            catch (HttpRequestException ex)
            {
                return CreateInfoErrorResult(
                    $"Network error while connecting to CloudPanel: {ex.Message}",
                    "NETWORK_ERROR"
                );
            }
            catch (JsonException ex)
            {
                return CreateInfoErrorResult(
                    $"Failed to parse CloudPanel response: {ex.Message}",
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

        public override async Task<List<AccountInfoResult>> ListDatabasesAsync(string domain)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(domain))
                {
                    return new List<AccountInfoResult>();
                }

                // CloudPanel API endpoint for listing databases
                var endpoint = $"/api/v1/databases?domain={domain}";

                var response = await _httpClient.GetAsync(endpoint);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new List<AccountInfoResult>();
                }

                // Parse CloudPanel response
                using var jsonDoc = JsonDocument.Parse(responseContent);
                var root = jsonDoc.RootElement;

                var results = new List<AccountInfoResult>();

                if (root.TryGetProperty("success", out var successElement) && successElement.GetBoolean() &&
                    root.TryGetProperty("data", out var dataArray) && dataArray.ValueKind == JsonValueKind.Array)
                {
                    foreach (var database in dataArray.EnumerateArray())
                    {
                        var databaseId = database.TryGetProperty("id", out var idElement) 
                            ? idElement.GetInt32().ToString() 
                            : null;

                        if (!string.IsNullOrEmpty(databaseId))
                        {
                            results.Add(new AccountInfoResult
                            {
                                Success = true,
                                AccountId = databaseId,
                                DatabaseName = database.TryGetProperty("databaseName", out var dbNameElement) ? dbNameElement.GetString() : null,
                                DatabaseUser = database.TryGetProperty("databaseUser", out var userElement) ? userElement.GetString() : null,
                                DatabaseType = database.TryGetProperty("databaseType", out var typeElement) ? typeElement.GetString() : "mysql"
                            });
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

        public override async Task<DatabaseResult> CreateDatabaseUserAsync(DatabaseUserRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Username))
                {
                    return CreateDatabaseErrorResult("Username is required", "INVALID_USERNAME");
                }

                if (string.IsNullOrWhiteSpace(request.Password))
                {
                    return CreateDatabaseErrorResult("Password is required", "INVALID_PASSWORD");
                }

                if (string.IsNullOrWhiteSpace(request.DatabaseName))
                {
                    return CreateDatabaseErrorResult("Database name is required", "INVALID_DATABASE_NAME");
                }

                // CloudPanel API endpoint for database user creation
                var endpoint = "/api/v1/database-users";

                // Build request payload
                var payload = new
                {
                    username = request.Username,
                    password = request.Password,
                    databaseName = request.DatabaseName,
                    privileges = request.Privileges ?? new List<string> { "ALL PRIVILEGES" }
                };

                var response = await _httpClient.PostAsJsonAsync(endpoint, payload);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return CreateDatabaseErrorResult(
                        $"CloudPanel API request failed: {response.StatusCode} - {responseContent}",
                        response.StatusCode.ToString()
                    );
                }

                // Parse CloudPanel response
                using var jsonDoc = JsonDocument.Parse(responseContent);
                var root = jsonDoc.RootElement;

                if (root.TryGetProperty("success", out var successElement) && successElement.GetBoolean())
                {
                    var message = root.TryGetProperty("message", out var msgElement)
                        ? msgElement.GetString()
                        : "Database user created successfully";

                    var userId = root.TryGetProperty("data", out var dataElement) && 
                                dataElement.TryGetProperty("id", out var idElement)
                        ? idElement.GetInt32().ToString()
                        : request.Username;

                    return new DatabaseResult
                    {
                        Success = true,
                        Message = message ?? "Database user created successfully",
                        DatabaseId = userId,
                        Username = request.Username,
                        CreatedDate = DateTime.UtcNow
                    };
                }
                else if (root.TryGetProperty("error", out var errorElement))
                {
                    var errorMessage = errorElement.GetString() ?? "Unknown error occurred";
                    return CreateDatabaseErrorResult(errorMessage, "CLOUDPANEL_ERROR");
                }

                return CreateDatabaseErrorResult("Invalid response format from CloudPanel", "INVALID_RESPONSE");
            }
            catch (HttpRequestException ex)
            {
                return CreateDatabaseErrorResult(
                    $"Network error while connecting to CloudPanel: {ex.Message}",
                    "NETWORK_ERROR"
                );
            }
            catch (JsonException ex)
            {
                return CreateDatabaseErrorResult(
                    $"Failed to parse CloudPanel response: {ex.Message}",
                    "JSON_PARSE_ERROR"
                );
            }
            catch (Exception ex)
            {
                return CreateDatabaseErrorResult(
                    $"Unexpected error: {ex.Message}",
                    "UNEXPECTED_ERROR"
                );
            }
        }

        public override async Task<AccountUpdateResult> DeleteDatabaseUserAsync(string userId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    return CreateUpdateErrorResult("User ID is required", "INVALID_USER_ID");
                }

                // CloudPanel API endpoint for database user deletion
                var endpoint = $"/api/v1/database-users/{userId}";

                var response = await _httpClient.DeleteAsync(endpoint);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return CreateUpdateErrorResult(
                        $"CloudPanel API request failed: {response.StatusCode} - {responseContent}",
                        response.StatusCode.ToString()
                    );
                }

                // Parse CloudPanel response
                using var jsonDoc = JsonDocument.Parse(responseContent);
                var root = jsonDoc.RootElement;

                if (root.TryGetProperty("success", out var successElement) && successElement.GetBoolean())
                {
                    var message = root.TryGetProperty("message", out var msgElement)
                        ? msgElement.GetString()
                        : "Database user deleted successfully";

                    return new AccountUpdateResult
                    {
                        Success = true,
                        Message = message ?? "Database user deleted successfully",
                        AccountId = userId,
                        UpdatedField = "Status",
                        OldValue = "Active",
                        NewValue = "Deleted",
                        UpdatedDate = DateTime.UtcNow
                    };
                }
                else if (root.TryGetProperty("error", out var errorElement))
                {
                    var errorMessage = errorElement.GetString() ?? "Unknown error occurred";
                    return CreateUpdateErrorResult(errorMessage, "CLOUDPANEL_ERROR");
                }

                return CreateUpdateErrorResult("Invalid response format from CloudPanel", "INVALID_RESPONSE");
            }
            catch (HttpRequestException ex)
            {
                return CreateUpdateErrorResult(
                    $"Network error while connecting to CloudPanel: {ex.Message}",
                    "NETWORK_ERROR"
                );
            }
            catch (JsonException ex)
            {
                return CreateUpdateErrorResult(
                    $"Failed to parse CloudPanel response: {ex.Message}",
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

        public override async Task<AccountUpdateResult> GrantDatabasePrivilegesAsync(string userId, string databaseId, List<string> privileges)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    return CreateUpdateErrorResult("User ID is required", "INVALID_USER_ID");
                }

                if (string.IsNullOrWhiteSpace(databaseId))
                {
                    return CreateUpdateErrorResult("Database ID is required", "INVALID_DATABASE_ID");
                }

                // CloudPanel API endpoint for granting privileges
                var endpoint = $"/api/v1/database-users/{userId}/privileges";

                // Build request payload
                var payload = new
                {
                    databaseId = databaseId,
                    privileges = privileges ?? new List<string> { "ALL PRIVILEGES" }
                };

                var response = await _httpClient.PutAsJsonAsync(endpoint, payload);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return CreateUpdateErrorResult(
                        $"CloudPanel API request failed: {response.StatusCode} - {responseContent}",
                        response.StatusCode.ToString()
                    );
                }

                // Parse CloudPanel response
                using var jsonDoc = JsonDocument.Parse(responseContent);
                var root = jsonDoc.RootElement;

                if (root.TryGetProperty("success", out var successElement) && successElement.GetBoolean())
                {
                    var message = root.TryGetProperty("message", out var msgElement)
                        ? msgElement.GetString()
                        : "Privileges granted successfully";

                    return new AccountUpdateResult
                    {
                        Success = true,
                        Message = message ?? "Privileges granted successfully",
                        AccountId = userId,
                        UpdatedField = "Privileges",
                        UpdatedDate = DateTime.UtcNow
                    };
                }
                else if (root.TryGetProperty("error", out var errorElement))
                {
                    var errorMessage = errorElement.GetString() ?? "Unknown error occurred";
                    return CreateUpdateErrorResult(errorMessage, "CLOUDPANEL_ERROR");
                }

                return CreateUpdateErrorResult("Invalid response format from CloudPanel", "INVALID_RESPONSE");
            }
            catch (HttpRequestException ex)
            {
                return CreateUpdateErrorResult(
                    $"Network error while connecting to CloudPanel: {ex.Message}",
                    "NETWORK_ERROR"
                );
            }
            catch (JsonException ex)
            {
                return CreateUpdateErrorResult(
                    $"Failed to parse CloudPanel response: {ex.Message}",
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

        public override async Task<AccountUpdateResult> ChangeDatabasePasswordAsync(string userId, string newPassword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    return CreateUpdateErrorResult("User ID is required", "INVALID_USER_ID");
                }

                if (string.IsNullOrWhiteSpace(newPassword))
                {
                    return CreateUpdateErrorResult("New password is required", "INVALID_PASSWORD");
                }

                // CloudPanel API endpoint for changing database user password
                var endpoint = $"/api/v1/database-users/{userId}/password";

                // Build request payload
                var payload = new
                {
                    password = newPassword
                };

                var response = await _httpClient.PutAsJsonAsync(endpoint, payload);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return CreateUpdateErrorResult(
                        $"CloudPanel API request failed: {response.StatusCode} - {responseContent}",
                        response.StatusCode.ToString()
                    );
                }

                // Parse CloudPanel response
                using var jsonDoc = JsonDocument.Parse(responseContent);
                var root = jsonDoc.RootElement;

                if (root.TryGetProperty("success", out var successElement) && successElement.GetBoolean())
                {
                    var message = root.TryGetProperty("message", out var msgElement)
                        ? msgElement.GetString()
                        : "Database password changed successfully";

                    return new AccountUpdateResult
                    {
                        Success = true,
                        Message = message ?? "Database password changed successfully",
                        AccountId = userId,
                        UpdatedField = "Password",
                        UpdatedDate = DateTime.UtcNow
                    };
                }
                else if (root.TryGetProperty("error", out var errorElement))
                {
                    var errorMessage = errorElement.GetString() ?? "Unknown error occurred";
                    return CreateUpdateErrorResult(errorMessage, "CLOUDPANEL_ERROR");
                }

                return CreateUpdateErrorResult("Invalid response format from CloudPanel", "INVALID_RESPONSE");
            }
            catch (HttpRequestException ex)
            {
                return CreateUpdateErrorResult(
                    $"Network error while connecting to CloudPanel: {ex.Message}",
                    "NETWORK_ERROR"
                );
            }
            catch (JsonException ex)
            {
                return CreateUpdateErrorResult(
                    $"Failed to parse CloudPanel response: {ex.Message}",
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

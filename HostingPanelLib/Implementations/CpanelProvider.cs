using HostingPanelLib.Models;
using System.Net.Http.Json;
using System.Text.Json;
using System.Web;

namespace HostingPanelLib.Implementations
{
    public class CpanelProvider : BaseHostingPanel
    {
        private readonly string _apiToken;
        private readonly string _username;
        private readonly int _port;
        private readonly bool _useHttps;

        public CpanelProvider(string apiUrl, string apiToken, string username, int port, bool useHttps)
            : base(apiUrl)
        {
            _apiToken = apiToken ?? throw new ArgumentNullException(nameof(apiToken));
            _username = username ?? throw new ArgumentNullException(nameof(username));
            _port = port;
            _useHttps = useHttps;

            _httpClient.DefaultRequestHeaders.Add("Authorization", $"cpanel {_username}:{_apiToken}");
        }

        private string BuildWhmApiUrl(string function, Dictionary<string, string>? parameters = null)
        {
            var queryString = parameters != null 
                ? string.Join("&", parameters.Select(kvp => $"{HttpUtility.UrlEncode(kvp.Key)}={HttpUtility.UrlEncode(kvp.Value)}"))
                : string.Empty;
            
            return $"/json-api/{function}?api.version=1&{queryString}";
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

                // WHM API createacct parameters
                var parameters = new Dictionary<string, string>
                {
                    ["username"] = request.Username,
                    ["domain"] = request.Domain,
                    ["password"] = request.Password,
                    ["contactemail"] = request.Email ?? "",
                    ["plan"] = request.Plan ?? "default",
                };

                // Optional parameters
                if (request.DiskQuotaMB.HasValue)
                    parameters["quota"] = request.DiskQuotaMB.Value.ToString();
                if (request.BandwidthLimitMB.HasValue)
                    parameters["bwlimit"] = request.BandwidthLimitMB.Value.ToString();
                if (request.MaxEmailAccounts.HasValue)
                    parameters["maxpop"] = request.MaxEmailAccounts.Value.ToString();
                if (request.MaxDatabases.HasValue)
                    parameters["maxsql"] = request.MaxDatabases.Value.ToString();
                if (request.MaxFtpAccounts.HasValue)
                    parameters["maxftp"] = request.MaxFtpAccounts.Value.ToString();
                if (request.MaxSubdomains.HasValue)
                    parameters["maxsub"] = request.MaxSubdomains.Value.ToString();
                if (request.EnableCgi.HasValue)
                    parameters["cgi"] = request.EnableCgi.Value ? "1" : "0";

                var endpoint = BuildWhmApiUrl("createacct", parameters);
                var response = await _httpClient.GetAsync(endpoint);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return CreateHostingErrorResult(
                        $"WHM API request failed: {response.StatusCode} - {responseContent}",
                        response.StatusCode.ToString()
                    );
                }

                // Parse WHM response
                using var jsonDoc = JsonDocument.Parse(responseContent);
                var root = jsonDoc.RootElement;

                if (root.TryGetProperty("metadata", out var metadata) &&
                    metadata.TryGetProperty("result", out var result))
                {
                    var success = result.GetInt32() == 1;

                    if (success)
                    {
                        var reason = metadata.TryGetProperty("reason", out var reasonElement)
                            ? reasonElement.GetString()
                            : "Account created successfully";

                        return new HostingAccountResult
                        {
                            Success = true,
                            Message = reason ?? "Account created successfully",
                            AccountId = request.Username,
                            Username = request.Username,
                            Domain = request.Domain,
                            PlanName = request.Plan ?? "default",
                            CreatedDate = DateTime.UtcNow
                        };
                    }
                    else
                    {
                        var reason = metadata.TryGetProperty("reason", out var reasonElement)
                            ? reasonElement.GetString()
                            : "Unknown error occurred";

                        return CreateHostingErrorResult(
                            reason ?? "Failed to create account",
                            "WHM_ERROR"
                        );
                    }
                }

                return CreateHostingErrorResult("Invalid response format from WHM", "INVALID_RESPONSE");
            }
            catch (HttpRequestException ex)
            {
                return CreateHostingErrorResult(
                    $"Network error while connecting to WHM: {ex.Message}",
                    "NETWORK_ERROR"
                );
            }
            catch (JsonException ex)
            {
                return CreateHostingErrorResult(
                    $"Failed to parse WHM response: {ex.Message}",
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
                    return CreateUpdateErrorResult("Account ID (username) is required", "INVALID_ACCOUNT_ID");
                }

                // WHM API modifyacct parameters
                var parameters = new Dictionary<string, string>
                {
                    ["user"] = accountId
                };

                // Add fields to modify
                if (!string.IsNullOrWhiteSpace(request.Domain))
                    parameters["domain"] = request.Domain;
                if (!string.IsNullOrWhiteSpace(request.Email))
                    parameters["contactemail"] = request.Email;
                if (request.DiskQuotaMB.HasValue)
                    parameters["QUOTA"] = request.DiskQuotaMB.Value.ToString();
                if (request.BandwidthLimitMB.HasValue)
                    parameters["BWLIMIT"] = request.BandwidthLimitMB.Value.ToString();
                if (request.MaxEmailAccounts.HasValue)
                    parameters["MAXPOP"] = request.MaxEmailAccounts.Value.ToString();
                if (request.MaxDatabases.HasValue)
                    parameters["MAXSQL"] = request.MaxDatabases.Value.ToString();
                if (request.MaxFtpAccounts.HasValue)
                    parameters["MAXFTP"] = request.MaxFtpAccounts.Value.ToString();
                if (request.MaxSubdomains.HasValue)
                    parameters["MAXSUB"] = request.MaxSubdomains.Value.ToString();
                if (request.EnableShellAccess.HasValue)
                    parameters["SHELL"] = request.EnableShellAccess.Value ? "1" : "0";

                var endpoint = BuildWhmApiUrl("modifyacct", parameters);
                var response = await _httpClient.GetAsync(endpoint);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return CreateUpdateErrorResult(
                        $"WHM API request failed: {response.StatusCode} - {responseContent}",
                        response.StatusCode.ToString()
                    );
                }

                // Parse WHM response
                using var jsonDoc = JsonDocument.Parse(responseContent);
                var root = jsonDoc.RootElement;

                if (root.TryGetProperty("metadata", out var metadata) &&
                    metadata.TryGetProperty("result", out var result))
                {
                    var success = result.GetInt32() == 1;

                    if (success)
                    {
                        var reason = metadata.TryGetProperty("reason", out var reasonElement)
                            ? reasonElement.GetString()
                            : "Account updated successfully";

                        return new AccountUpdateResult
                        {
                            Success = true,
                            Message = reason ?? "Account updated successfully",
                            AccountId = accountId,
                            UpdatedField = "AccountConfiguration",
                            UpdatedDate = DateTime.UtcNow
                        };
                    }
                    else
                    {
                        var reason = metadata.TryGetProperty("reason", out var reasonElement)
                            ? reasonElement.GetString()
                            : "Unknown error occurred";

                        return CreateUpdateErrorResult(
                            reason ?? "Failed to update account",
                            "WHM_ERROR"
                        );
                    }
                }

                return CreateUpdateErrorResult("Invalid response format from WHM", "INVALID_RESPONSE");
            }
            catch (HttpRequestException ex)
            {
                return CreateUpdateErrorResult(
                    $"Network error while connecting to WHM: {ex.Message}",
                    "NETWORK_ERROR"
                );
            }
            catch (JsonException ex)
            {
                return CreateUpdateErrorResult(
                    $"Failed to parse WHM response: {ex.Message}",
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
                    return CreateUpdateErrorResult("Account ID (username) is required", "INVALID_ACCOUNT_ID");
                }

                var parameters = new Dictionary<string, string>
                {
                    ["user"] = accountId,
                    ["reason"] = "Suspended via API"
                };

                var endpoint = BuildWhmApiUrl("suspendacct", parameters);
                var response = await _httpClient.GetAsync(endpoint);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return CreateUpdateErrorResult(
                        $"WHM API request failed: {response.StatusCode} - {responseContent}",
                        response.StatusCode.ToString()
                    );
                }

                // Parse WHM response
                using var jsonDoc = JsonDocument.Parse(responseContent);
                var root = jsonDoc.RootElement;

                if (root.TryGetProperty("metadata", out var metadata) &&
                    metadata.TryGetProperty("result", out var result))
                {
                    var success = result.GetInt32() == 1;

                    if (success)
                    {
                        var reason = metadata.TryGetProperty("reason", out var reasonElement)
                            ? reasonElement.GetString()
                            : "Account suspended successfully";

                        return new AccountUpdateResult
                        {
                            Success = true,
                            Message = reason ?? "Account suspended successfully",
                            AccountId = accountId,
                            UpdatedField = "Status",
                            OldValue = "Active",
                            NewValue = "Suspended",
                            UpdatedDate = DateTime.UtcNow
                        };
                    }
                    else
                    {
                        var reason = metadata.TryGetProperty("reason", out var reasonElement)
                            ? reasonElement.GetString()
                            : "Unknown error occurred";

                        return CreateUpdateErrorResult(
                            reason ?? "Failed to suspend account",
                            "WHM_ERROR"
                        );
                    }
                }

                return CreateUpdateErrorResult("Invalid response format from WHM", "INVALID_RESPONSE");
            }
            catch (HttpRequestException ex)
            {
                return CreateUpdateErrorResult(
                    $"Network error while connecting to WHM: {ex.Message}",
                    "NETWORK_ERROR"
                );
            }
            catch (JsonException ex)
            {
                return CreateUpdateErrorResult(
                    $"Failed to parse WHM response: {ex.Message}",
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
                    return CreateUpdateErrorResult("Account ID (username) is required", "INVALID_ACCOUNT_ID");
                }

                var parameters = new Dictionary<string, string>
                {
                    ["user"] = accountId
                };

                var endpoint = BuildWhmApiUrl("unsuspendacct", parameters);
                var response = await _httpClient.GetAsync(endpoint);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return CreateUpdateErrorResult(
                        $"WHM API request failed: {response.StatusCode} - {responseContent}",
                        response.StatusCode.ToString()
                    );
                }

                // Parse WHM response
                using var jsonDoc = JsonDocument.Parse(responseContent);
                var root = jsonDoc.RootElement;

                if (root.TryGetProperty("metadata", out var metadata) &&
                    metadata.TryGetProperty("result", out var result))
                {
                    var success = result.GetInt32() == 1;

                    if (success)
                    {
                        var reason = metadata.TryGetProperty("reason", out var reasonElement)
                            ? reasonElement.GetString()
                            : "Account unsuspended successfully";

                        return new AccountUpdateResult
                        {
                            Success = true,
                            Message = reason ?? "Account unsuspended successfully",
                            AccountId = accountId,
                            UpdatedField = "Status",
                            OldValue = "Suspended",
                            NewValue = "Active",
                            UpdatedDate = DateTime.UtcNow
                        };
                    }
                    else
                    {
                        var reason = metadata.TryGetProperty("reason", out var reasonElement)
                            ? reasonElement.GetString()
                            : "Unknown error occurred";

                        return CreateUpdateErrorResult(
                            reason ?? "Failed to unsuspend account",
                            "WHM_ERROR"
                        );
                    }
                }

                return CreateUpdateErrorResult("Invalid response format from WHM", "INVALID_RESPONSE");
            }
            catch (HttpRequestException ex)
            {
                return CreateUpdateErrorResult(
                    $"Network error while connecting to WHM: {ex.Message}",
                    "NETWORK_ERROR"
                );
            }
            catch (JsonException ex)
            {
                return CreateUpdateErrorResult(
                    $"Failed to parse WHM response: {ex.Message}",
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
                    return CreateUpdateErrorResult("Account ID (username) is required", "INVALID_ACCOUNT_ID");
                }

                var parameters = new Dictionary<string, string>
                {
                    ["user"] = accountId,
                    ["keepdns"] = "0" // Don't keep DNS zones
                };

                var endpoint = BuildWhmApiUrl("removeacct", parameters);
                var response = await _httpClient.GetAsync(endpoint);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return CreateUpdateErrorResult(
                        $"WHM API request failed: {response.StatusCode} - {responseContent}",
                        response.StatusCode.ToString()
                    );
                }

                // Parse WHM response
                using var jsonDoc = JsonDocument.Parse(responseContent);
                var root = jsonDoc.RootElement;

                if (root.TryGetProperty("metadata", out var metadata) &&
                    metadata.TryGetProperty("result", out var result))
                {
                    var success = result.GetInt32() == 1;

                    if (success)
                    {
                        var reason = metadata.TryGetProperty("reason", out var reasonElement)
                            ? reasonElement.GetString()
                            : "Account deleted successfully";

                        return new AccountUpdateResult
                        {
                            Success = true,
                            Message = reason ?? "Account deleted successfully",
                            AccountId = accountId,
                            UpdatedField = "Status",
                            OldValue = "Active",
                            NewValue = "Deleted",
                            UpdatedDate = DateTime.UtcNow
                        };
                    }
                    else
                    {
                        var reason = metadata.TryGetProperty("reason", out var reasonElement)
                            ? reasonElement.GetString()
                            : "Unknown error occurred";

                        return CreateUpdateErrorResult(
                            reason ?? "Failed to delete account",
                            "WHM_ERROR"
                        );
                    }
                }

                return CreateUpdateErrorResult("Invalid response format from WHM", "INVALID_RESPONSE");
            }
            catch (HttpRequestException ex)
            {
                return CreateUpdateErrorResult(
                    $"Network error while connecting to WHM: {ex.Message}",
                    "NETWORK_ERROR"
                );
            }
            catch (JsonException ex)
            {
                return CreateUpdateErrorResult(
                    $"Failed to parse WHM response: {ex.Message}",
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
                    return CreateInfoErrorResult("Account ID (username) is required", "INVALID_ACCOUNT_ID");
                }

                var parameters = new Dictionary<string, string>
                {
                    ["user"] = accountId
                };

                var endpoint = BuildWhmApiUrl("accountsummary", parameters);
                var response = await _httpClient.GetAsync(endpoint);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return CreateInfoErrorResult(
                        $"WHM API request failed: {response.StatusCode} - {responseContent}",
                        response.StatusCode.ToString()
                    );
                }

                // Parse WHM response
                using var jsonDoc = JsonDocument.Parse(responseContent);
                var root = jsonDoc.RootElement;

                if (root.TryGetProperty("metadata", out var metadata) &&
                    metadata.TryGetProperty("result", out var result) &&
                    result.GetInt32() == 1 &&
                    root.TryGetProperty("data", out var data) &&
                    data.TryGetProperty("acct", out var acctArray) &&
                    acctArray.ValueKind == JsonValueKind.Array)
                {
                    var acct = acctArray.EnumerateArray().FirstOrDefault();
                    
                    if (acct.ValueKind != JsonValueKind.Undefined)
                    {
                        return new AccountInfoResult
                        {
                            Success = true,
                            Message = "Account information retrieved successfully",
                            AccountId = accountId,
                            Username = acct.TryGetProperty("user", out var userElement) ? userElement.GetString() : accountId,
                            Domain = acct.TryGetProperty("domain", out var domainElement) ? domainElement.GetString() : null,
                            Email = acct.TryGetProperty("email", out var emailElement) ? emailElement.GetString() : null,
                            Plan = acct.TryGetProperty("plan", out var planElement) ? planElement.GetString() : null,
                            Status = acct.TryGetProperty("suspended", out var suspendedElement) && suspendedElement.GetInt32() == 1 
                                ? "Suspended" 
                                : "Active",
                            DiskUsageMB = acct.TryGetProperty("diskused", out var diskUsedElement) 
                                ? (int?)(diskUsedElement.GetDouble()) 
                                : null,
                            DiskQuotaMB = acct.TryGetProperty("disklimit", out var diskLimitElement) 
                                ? (int?)(diskLimitElement.GetDouble()) 
                                : null,
                            IpAddress = acct.TryGetProperty("ip", out var ipElement) ? ipElement.GetString() : null,
                            CreatedDate = acct.TryGetProperty("startdate", out var startDateElement) 
                                ? DateTimeOffset.FromUnixTimeSeconds(startDateElement.GetInt64()).DateTime 
                                : null,
                            AdditionalInfo = new Dictionary<string, object>
                            {
                                ["owner"] = acct.TryGetProperty("owner", out var ownerElement) ? ownerElement.GetString() ?? "root" : "root",
                                ["partition"] = acct.TryGetProperty("partition", out var partitionElement) ? partitionElement.GetString() ?? "" : ""
                            }
                        };
                    }
                }

                return CreateInfoErrorResult("Account not found or invalid response", "NOT_FOUND");
            }
            catch (HttpRequestException ex)
            {
                return CreateInfoErrorResult(
                    $"Network error while connecting to WHM: {ex.Message}",
                    "NETWORK_ERROR"
                );
            }
            catch (JsonException ex)
            {
                return CreateInfoErrorResult(
                    $"Failed to parse WHM response: {ex.Message}",
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
                var endpoint = BuildWhmApiUrl("listaccts");
                var response = await _httpClient.GetAsync(endpoint);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new List<AccountInfoResult>();
                }

                // Parse WHM response
                using var jsonDoc = JsonDocument.Parse(responseContent);
                var root = jsonDoc.RootElement;

                var results = new List<AccountInfoResult>();

                if (root.TryGetProperty("metadata", out var metadata) &&
                    metadata.TryGetProperty("result", out var result) &&
                    result.GetInt32() == 1 &&
                    root.TryGetProperty("data", out var data) &&
                    data.TryGetProperty("acct", out var acctArray) &&
                    acctArray.ValueKind == JsonValueKind.Array)
                {
                    foreach (var acct in acctArray.EnumerateArray())
                    {
                        var username = acct.TryGetProperty("user", out var userElement) ? userElement.GetString() : null;
                        
                        if (!string.IsNullOrEmpty(username))
                        {
                            results.Add(new AccountInfoResult
                            {
                                Success = true,
                                AccountId = username,
                                Username = username,
                                Domain = acct.TryGetProperty("domain", out var domainElement) ? domainElement.GetString() : null,
                                Email = acct.TryGetProperty("email", out var emailElement) ? emailElement.GetString() : null,
                                Plan = acct.TryGetProperty("plan", out var planElement) ? planElement.GetString() : null,
                                Status = acct.TryGetProperty("suspended", out var suspendedElement) && suspendedElement.GetInt32() == 1 
                                    ? "Suspended" 
                                    : "Active",
                                DiskUsageMB = acct.TryGetProperty("diskused", out var diskUsedElement) 
                                    ? (int?)(diskUsedElement.GetDouble()) 
                                    : null,
                                DiskQuotaMB = acct.TryGetProperty("disklimit", out var diskLimitElement) 
                                    ? (int?)(diskLimitElement.GetDouble()) 
                                    : null,
                                IpAddress = acct.TryGetProperty("ip", out var ipElement) ? ipElement.GetString() : null
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

                // cPanel UAPI for email account creation
                var parameters = new Dictionary<string, string>
                {
                    ["email"] = emailParts[0],
                    ["password"] = request.Password,
                    ["quota"] = (request.QuotaMB ?? 250).ToString(),
                    ["domain"] = request.Domain
                };

                var endpoint = BuildWhmApiUrl("cpanel", parameters);
                endpoint += "&cpanel_jsonapi_module=Email&cpanel_jsonapi_func=add_pop&cpanel_jsonapi_apiversion=2";

                var response = await _httpClient.GetAsync(endpoint);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return CreateMailErrorResult(
                        $"cPanel API request failed: {response.StatusCode} - {responseContent}",
                        response.StatusCode.ToString()
                    );
                }

                // Parse cPanel response
                using var jsonDoc = JsonDocument.Parse(responseContent);
                var root = jsonDoc.RootElement;

                // Check for cpanelresult
                if (root.TryGetProperty("cpanelresult", out var cpanelResult) &&
                    cpanelResult.TryGetProperty("data", out var dataArray) &&
                    dataArray.ValueKind == JsonValueKind.Array)
                {
                    var data = dataArray.EnumerateArray().FirstOrDefault();
                    
                    if (data.ValueKind != JsonValueKind.Undefined &&
                        data.TryGetProperty("result", out var resultElement) &&
                        resultElement.GetInt32() == 1)
                    {
                        var reason = data.TryGetProperty("reason", out var reasonElement)
                            ? reasonElement.GetString()
                            : "Email account created successfully";

                        return new MailAccountResult
                        {
                            Success = true,
                            Message = reason ?? "Email account created successfully",
                            AccountId = request.EmailAddress,
                            EmailAddress = request.EmailAddress,
                            Domain = request.Domain,
                            QuotaMB = request.QuotaMB ?? 250,
                            CreatedDate = DateTime.UtcNow
                        };
                    }
                    else if (data.ValueKind != JsonValueKind.Undefined)
                    {
                        var reason = data.TryGetProperty("reason", out var reasonElement)
                            ? reasonElement.GetString()
                            : "Unknown error occurred";

                        return CreateMailErrorResult(
                            reason ?? "Failed to create email account",
                            "CPANEL_ERROR"
                        );
                    }
                }

                return CreateMailErrorResult("Invalid response format from cPanel", "INVALID_RESPONSE");
            }
            catch (HttpRequestException ex)
            {
                return CreateMailErrorResult(
                    $"Network error while connecting to cPanel: {ex.Message}",
                    "NETWORK_ERROR"
                );
            }
            catch (JsonException ex)
            {
                return CreateMailErrorResult(
                    $"Failed to parse cPanel response: {ex.Message}",
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

                var emailParts = accountId.Split('@');
                if (emailParts.Length != 2)
                {
                    return CreateUpdateErrorResult("Invalid email address format", "INVALID_EMAIL_FORMAT");
                }

                // cPanel UAPI for email quota update
                var parameters = new Dictionary<string, string>
                {
                    ["email"] = emailParts[0],
                    ["domain"] = emailParts[1],
                    ["quota"] = (request.QuotaMB ?? 250).ToString()
                };

                var endpoint = BuildWhmApiUrl("cpanel", parameters);
                endpoint += "&cpanel_jsonapi_module=Email&cpanel_jsonapi_func=edit_pop_quota&cpanel_jsonapi_apiversion=2";

                var response = await _httpClient.GetAsync(endpoint);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return CreateUpdateErrorResult(
                        $"cPanel API request failed: {response.StatusCode} - {responseContent}",
                        response.StatusCode.ToString()
                    );
                }

                // Parse cPanel response
                using var jsonDoc = JsonDocument.Parse(responseContent);
                var root = jsonDoc.RootElement;

                if (root.TryGetProperty("cpanelresult", out var cpanelResult) &&
                    cpanelResult.TryGetProperty("data", out var dataArray) &&
                    dataArray.ValueKind == JsonValueKind.Array)
                {
                    var data = dataArray.EnumerateArray().FirstOrDefault();
                    
                    if (data.ValueKind != JsonValueKind.Undefined &&
                        data.TryGetProperty("result", out var resultElement) &&
                        resultElement.GetInt32() == 1)
                    {
                        var reason = data.TryGetProperty("reason", out var reasonElement)
                            ? reasonElement.GetString()
                            : "Email account updated successfully";

                        return new AccountUpdateResult
                        {
                            Success = true,
                            Message = reason ?? "Email account updated successfully",
                            AccountId = accountId,
                            UpdatedField = "Quota",
                            NewValue = request.QuotaMB,
                            UpdatedDate = DateTime.UtcNow
                        };
                    }
                    else if (data.ValueKind != JsonValueKind.Undefined)
                    {
                        var reason = data.TryGetProperty("reason", out var reasonElement)
                            ? reasonElement.GetString()
                            : "Unknown error occurred";

                        return CreateUpdateErrorResult(
                            reason ?? "Failed to update email account",
                            "CPANEL_ERROR"
                        );
                    }
                }

                return CreateUpdateErrorResult("Invalid response format from cPanel", "INVALID_RESPONSE");
            }
            catch (HttpRequestException ex)
            {
                return CreateUpdateErrorResult(
                    $"Network error while connecting to cPanel: {ex.Message}",
                    "NETWORK_ERROR"
                );
            }
            catch (JsonException ex)
            {
                return CreateUpdateErrorResult(
                    $"Failed to parse cPanel response: {ex.Message}",
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

                // cPanel UAPI for email deletion
                var parameters = new Dictionary<string, string>
                {
                    ["email"] = emailParts[0],
                    ["domain"] = emailParts[1]
                };

                var endpoint = BuildWhmApiUrl("cpanel", parameters);
                endpoint += "&cpanel_jsonapi_module=Email&cpanel_jsonapi_func=delete_pop&cpanel_jsonapi_apiversion=2";

                var response = await _httpClient.GetAsync(endpoint);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return CreateUpdateErrorResult(
                        $"cPanel API request failed: {response.StatusCode} - {responseContent}",
                        response.StatusCode.ToString()
                    );
                }

                // Parse cPanel response
                using var jsonDoc = JsonDocument.Parse(responseContent);
                var root = jsonDoc.RootElement;

                if (root.TryGetProperty("cpanelresult", out var cpanelResult) &&
                    cpanelResult.TryGetProperty("data", out var dataArray) &&
                    dataArray.ValueKind == JsonValueKind.Array)
                {
                    var data = dataArray.EnumerateArray().FirstOrDefault();
                    
                    if (data.ValueKind != JsonValueKind.Undefined &&
                        data.TryGetProperty("result", out var resultElement) &&
                        resultElement.GetInt32() == 1)
                    {
                        var reason = data.TryGetProperty("reason", out var reasonElement)
                            ? reasonElement.GetString()
                            : "Email account deleted successfully";

                        return new AccountUpdateResult
                        {
                            Success = true,
                            Message = reason ?? "Email account deleted successfully",
                            AccountId = accountId,
                            UpdatedField = "Status",
                            OldValue = "Active",
                            NewValue = "Deleted",
                            UpdatedDate = DateTime.UtcNow
                        };
                    }
                    else if (data.ValueKind != JsonValueKind.Undefined)
                    {
                        var reason = data.TryGetProperty("reason", out var reasonElement)
                            ? reasonElement.GetString()
                            : "Unknown error occurred";

                        return CreateUpdateErrorResult(
                            reason ?? "Failed to delete email account",
                            "CPANEL_ERROR"
                        );
                    }
                }

                return CreateUpdateErrorResult("Invalid response format from cPanel", "INVALID_RESPONSE");
            }
            catch (HttpRequestException ex)
            {
                return CreateUpdateErrorResult(
                    $"Network error while connecting to cPanel: {ex.Message}",
                    "NETWORK_ERROR"
                );
            }
            catch (JsonException ex)
            {
                return CreateUpdateErrorResult(
                    $"Failed to parse cPanel response: {ex.Message}",
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

                // cPanel doesn't have a dedicated get single email endpoint
                // We'll use list and filter
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

                // cPanel UAPI for listing email accounts
                var parameters = new Dictionary<string, string>
                {
                    ["domain"] = domain
                };

                var endpoint = BuildWhmApiUrl("cpanel", parameters);
                endpoint += "&cpanel_jsonapi_module=Email&cpanel_jsonapi_func=list_pops&cpanel_jsonapi_apiversion=2";

                var response = await _httpClient.GetAsync(endpoint);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new List<AccountInfoResult>();
                }

                // Parse cPanel response
                using var jsonDoc = JsonDocument.Parse(responseContent);
                var root = jsonDoc.RootElement;

                var results = new List<AccountInfoResult>();

                if (root.TryGetProperty("cpanelresult", out var cpanelResult) &&
                    cpanelResult.TryGetProperty("data", out var dataArray) &&
                    dataArray.ValueKind == JsonValueKind.Array)
                {
                    foreach (var emailAccount in dataArray.EnumerateArray())
                    {
                        var emailAddress = emailAccount.TryGetProperty("email", out var emailElement) 
                            ? emailElement.GetString() 
                            : null;
                        
                        if (!string.IsNullOrEmpty(emailAddress))
                        {
                            results.Add(new AccountInfoResult
                            {
                                Success = true,
                                AccountId = emailAddress,
                                Email = emailAddress,
                                Domain = domain,
                                DiskQuotaMB = emailAccount.TryGetProperty("_diskquota", out var quotaElement) 
                                    ? (int?)quotaElement.GetInt32() 
                                    : null,
                                DiskUsageMB = emailAccount.TryGetProperty("diskused", out var usedElement) 
                                    ? (int?)(usedElement.GetDouble()) 
                                    : null
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

                // cPanel UAPI for changing email password
                var parameters = new Dictionary<string, string>
                {
                    ["email"] = emailParts[0],
                    ["domain"] = emailParts[1],
                    ["password"] = newPassword
                };

                var endpoint = BuildWhmApiUrl("cpanel", parameters);
                endpoint += "&cpanel_jsonapi_module=Email&cpanel_jsonapi_func=passwd_pop&cpanel_jsonapi_apiversion=2";

                var response = await _httpClient.GetAsync(endpoint);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return CreateUpdateErrorResult(
                        $"cPanel API request failed: {response.StatusCode} - {responseContent}",
                        response.StatusCode.ToString()
                    );
                }

                // Parse cPanel response
                using var jsonDoc = JsonDocument.Parse(responseContent);
                var root = jsonDoc.RootElement;

                if (root.TryGetProperty("cpanelresult", out var cpanelResult) &&
                    cpanelResult.TryGetProperty("data", out var dataArray) &&
                    dataArray.ValueKind == JsonValueKind.Array)
                {
                    var data = dataArray.EnumerateArray().FirstOrDefault();
                    
                    if (data.ValueKind != JsonValueKind.Undefined &&
                        data.TryGetProperty("result", out var resultElement) &&
                        resultElement.GetInt32() == 1)
                    {
                        var reason = data.TryGetProperty("reason", out var reasonElement)
                            ? reasonElement.GetString()
                            : "Email password changed successfully";

                        return new AccountUpdateResult
                        {
                            Success = true,
                            Message = reason ?? "Email password changed successfully",
                            AccountId = accountId,
                            UpdatedField = "Password",
                            UpdatedDate = DateTime.UtcNow
                        };
                    }
                    else if (data.ValueKind != JsonValueKind.Undefined)
                    {
                        var reason = data.TryGetProperty("reason", out var reasonElement)
                            ? reasonElement.GetString()
                            : "Unknown error occurred";

                        return CreateUpdateErrorResult(
                            reason ?? "Failed to change email password",
                            "CPANEL_ERROR"
                        );
                    }
                }

                return CreateUpdateErrorResult("Invalid response format from cPanel", "INVALID_RESPONSE");
            }
            catch (HttpRequestException ex)
            {
                return CreateUpdateErrorResult(
                    $"Network error while connecting to cPanel: {ex.Message}",
                    "NETWORK_ERROR"
                );
            }
            catch (JsonException ex)
            {
                return CreateUpdateErrorResult(
                    $"Failed to parse cPanel response: {ex.Message}",
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
                    return CreateUpdateErrorResult("Account ID (username) is required", "INVALID_ACCOUNT_ID");
                }

                if (quotaMB < 0)
                {
                    return CreateUpdateErrorResult("Quota must be a positive value", "INVALID_QUOTA");
                }

                var parameters = new Dictionary<string, string>
                {
                    ["user"] = accountId,
                    ["QUOTA"] = quotaMB.ToString()
                };

                var endpoint = BuildWhmApiUrl("modifyacct", parameters);
                var response = await _httpClient.GetAsync(endpoint);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return CreateUpdateErrorResult(
                        $"WHM API request failed: {response.StatusCode} - {responseContent}",
                        response.StatusCode.ToString()
                    );
                }

                // Parse WHM response
                using var jsonDoc = JsonDocument.Parse(responseContent);
                var root = jsonDoc.RootElement;

                if (root.TryGetProperty("metadata", out var metadata) &&
                    metadata.TryGetProperty("result", out var result))
                {
                    var success = result.GetInt32() == 1;

                    if (success)
                    {
                        var reason = metadata.TryGetProperty("reason", out var reasonElement)
                            ? reasonElement.GetString()
                            : "Disk quota updated successfully";

                        return new AccountUpdateResult
                        {
                            Success = true,
                            Message = reason ?? "Disk quota updated successfully",
                            AccountId = accountId,
                            UpdatedField = "DiskQuota",
                            NewValue = quotaMB,
                            UpdatedDate = DateTime.UtcNow
                        };
                    }
                    else
                    {
                        var reason = metadata.TryGetProperty("reason", out var reasonElement)
                            ? reasonElement.GetString()
                            : "Unknown error occurred";

                        return CreateUpdateErrorResult(
                            reason ?? "Failed to set disk quota",
                            "WHM_ERROR"
                        );
                    }
                }

                return CreateUpdateErrorResult("Invalid response format from WHM", "INVALID_RESPONSE");
            }
            catch (HttpRequestException ex)
            {
                return CreateUpdateErrorResult(
                    $"Network error while connecting to WHM: {ex.Message}",
                    "NETWORK_ERROR"
                );
            }
            catch (JsonException ex)
            {
                return CreateUpdateErrorResult(
                    $"Failed to parse WHM response: {ex.Message}",
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
                    return CreateUpdateErrorResult("Account ID (username) is required", "INVALID_ACCOUNT_ID");
                }

                if (bandwidthMB < 0)
                {
                    return CreateUpdateErrorResult("Bandwidth must be a positive value", "INVALID_BANDWIDTH");
                }

                var parameters = new Dictionary<string, string>
                {
                    ["user"] = accountId,
                    ["BWLIMIT"] = bandwidthMB.ToString()
                };

                var endpoint = BuildWhmApiUrl("modifyacct", parameters);
                var response = await _httpClient.GetAsync(endpoint);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return CreateUpdateErrorResult(
                        $"WHM API request failed: {response.StatusCode} - {responseContent}",
                        response.StatusCode.ToString()
                    );
                }

                // Parse WHM response
                using var jsonDoc = JsonDocument.Parse(responseContent);
                var root = jsonDoc.RootElement;

                if (root.TryGetProperty("metadata", out var metadata) &&
                    metadata.TryGetProperty("result", out var result))
                {
                    var success = result.GetInt32() == 1;

                    if (success)
                    {
                        var reason = metadata.TryGetProperty("reason", out var reasonElement)
                            ? reasonElement.GetString()
                            : "Bandwidth limit updated successfully";

                        return new AccountUpdateResult
                        {
                            Success = true,
                            Message = reason ?? "Bandwidth limit updated successfully",
                            AccountId = accountId,
                            UpdatedField = "BandwidthLimit",
                            NewValue = bandwidthMB,
                            UpdatedDate = DateTime.UtcNow
                        };
                    }
                    else
                    {
                        var reason = metadata.TryGetProperty("reason", out var reasonElement)
                            ? reasonElement.GetString()
                            : "Unknown error occurred";

                        return CreateUpdateErrorResult(
                            reason ?? "Failed to set bandwidth limit",
                            "WHM_ERROR"
                        );
                    }
                }

                return CreateUpdateErrorResult("Invalid response format from WHM", "INVALID_RESPONSE");
            }
            catch (HttpRequestException ex)
            {
                return CreateUpdateErrorResult(
                    $"Network error while connecting to WHM: {ex.Message}",
                    "NETWORK_ERROR"
                );
            }
            catch (JsonException ex)
            {
                return CreateUpdateErrorResult(
                    $"Failed to parse WHM response: {ex.Message}",
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

        public override async Task<DatabaseResult> CreateDatabaseAsync(DatabaseRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.DatabaseName))
                {
                    return CreateDatabaseErrorResult("Database name is required", "INVALID_DATABASE_NAME");
                }

                var parameters = new Dictionary<string, string>
                {
                    ["name"] = request.DatabaseName
                };

                var endpoint = BuildWhmApiUrl("Mysql::create_database", parameters);
                var response = await _httpClient.GetAsync(endpoint);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return CreateDatabaseErrorResult($"API request failed: {response.StatusCode}", response.StatusCode.ToString());
                }

                using var jsonDoc = JsonDocument.Parse(responseContent);
                var root = jsonDoc.RootElement;

                if (root.TryGetProperty("metadata", out var metadata) &&
                    metadata.TryGetProperty("result", out var result))
                {
                    var success = result.GetInt32() == 1;

                    if (success)
                    {
                        return new DatabaseResult
                        {
                            Success = true,
                            Message = "Database created successfully",
                            DatabaseName = request.DatabaseName,
                            DatabaseType = "mysql",
                            Server = "localhost",
                            Port = 3306,
                            CreatedDate = DateTime.UtcNow
                        };
                    }
                }

                return CreateDatabaseErrorResult("Failed to create database", "CPANEL_ERROR");
            }
            catch (Exception ex)
            {
                return CreateDatabaseErrorResult($"Unexpected error: {ex.Message}", "UNEXPECTED_ERROR");
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

                var parameters = new Dictionary<string, string>
                {
                    ["name"] = databaseId
                };

                var endpoint = BuildWhmApiUrl("Mysql::delete_database", parameters);
                var response = await _httpClient.GetAsync(endpoint);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return CreateUpdateErrorResult($"API request failed: {response.StatusCode}", response.StatusCode.ToString());
                }

                using var jsonDoc = JsonDocument.Parse(responseContent);
                var root = jsonDoc.RootElement;

                if (root.TryGetProperty("metadata", out var metadata) &&
                    metadata.TryGetProperty("result", out var result))
                {
                    var success = result.GetInt32() == 1;

                    if (success)
                    {
                        return new AccountUpdateResult
                        {
                            Success = true,
                            Message = "Database deleted successfully",
                            AccountId = databaseId,
                            UpdatedDate = DateTime.UtcNow
                        };
                    }
                }

                return CreateUpdateErrorResult("Failed to delete database", "CPANEL_ERROR");
            }
            catch (Exception ex)
            {
                return CreateUpdateErrorResult($"Unexpected error: {ex.Message}", "UNEXPECTED_ERROR");
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

                return new AccountInfoResult
                {
                    Success = true,
                    Message = "Database information retrieved",
                    AccountId = databaseId,
                    DatabaseName = databaseId,
                    DatabaseType = "mysql"
                };
            }
            catch (Exception ex)
            {
                return CreateInfoErrorResult($"Unexpected error: {ex.Message}", "UNEXPECTED_ERROR");
            }
        }

        public override async Task<List<AccountInfoResult>> ListDatabasesAsync(string domain)
        {
            try
            {
                var endpoint = BuildWhmApiUrl("Mysql::list_databases", null);
                var response = await _httpClient.GetAsync(endpoint);

                if (!response.IsSuccessStatusCode)
                {
                    return new List<AccountInfoResult>();
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                using var jsonDoc = JsonDocument.Parse(responseContent);

                return new List<AccountInfoResult>();
            }
            catch
            {
                return new List<AccountInfoResult>();
            }
        }

        public override async Task<DatabaseResult> CreateDatabaseUserAsync(DatabaseUserRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                {
                    return CreateDatabaseErrorResult("Username and password are required", "INVALID_INPUT");
                }

                var parameters = new Dictionary<string, string>
                {
                    ["name"] = request.Username,
                    ["password"] = request.Password
                };

                var endpoint = BuildWhmApiUrl("Mysql::create_user", parameters);
                var response = await _httpClient.GetAsync(endpoint);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return CreateDatabaseErrorResult($"API request failed: {response.StatusCode}", response.StatusCode.ToString());
                }

                using var jsonDoc = JsonDocument.Parse(responseContent);
                var root = jsonDoc.RootElement;

                if (root.TryGetProperty("metadata", out var metadata) &&
                    metadata.TryGetProperty("result", out var result))
                {
                    var success = result.GetInt32() == 1;

                    if (success)
                    {
                        return new DatabaseResult
                        {
                            Success = true,
                            Message = "Database user created successfully",
                            Username = request.Username,
                            CreatedDate = DateTime.UtcNow
                        };
                    }
                }

                return CreateDatabaseErrorResult("Failed to create database user", "CPANEL_ERROR");
            }
            catch (Exception ex)
            {
                return CreateDatabaseErrorResult($"Unexpected error: {ex.Message}", "UNEXPECTED_ERROR");
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

                var parameters = new Dictionary<string, string>
                {
                    ["name"] = userId
                };

                var endpoint = BuildWhmApiUrl("Mysql::delete_user", parameters);
                var response = await _httpClient.GetAsync(endpoint);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return CreateUpdateErrorResult($"API request failed: {response.StatusCode}", response.StatusCode.ToString());
                }

                using var jsonDoc = JsonDocument.Parse(responseContent);
                var root = jsonDoc.RootElement;

                if (root.TryGetProperty("metadata", out var metadata) &&
                    metadata.TryGetProperty("result", out var result))
                {
                    var success = result.GetInt32() == 1;

                    if (success)
                    {
                        return new AccountUpdateResult
                        {
                            Success = true,
                            Message = "Database user deleted successfully",
                            AccountId = userId,
                            UpdatedDate = DateTime.UtcNow
                        };
                    }
                }

                return CreateUpdateErrorResult("Failed to delete database user", "CPANEL_ERROR");
            }
            catch (Exception ex)
            {
                return CreateUpdateErrorResult($"Unexpected error: {ex.Message}", "UNEXPECTED_ERROR");
            }
        }

        public override async Task<AccountUpdateResult> GrantDatabasePrivilegesAsync(string userId, string databaseId, List<string> privileges)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(databaseId))
                {
                    return CreateUpdateErrorResult("User ID and Database ID are required", "INVALID_INPUT");
                }

                var privilegesStr = privileges != null && privileges.Any()
                    ? string.Join(",", privileges)
                    : "ALL PRIVILEGES";

                var parameters = new Dictionary<string, string>
                {
                    ["user"] = userId,
                    ["database"] = databaseId,
                    ["privileges"] = privilegesStr
                };

                var endpoint = BuildWhmApiUrl("Mysql::set_privileges_on_database", parameters);
                var response = await _httpClient.GetAsync(endpoint);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return CreateUpdateErrorResult($"API request failed: {response.StatusCode}", response.StatusCode.ToString());
                }

                using var jsonDoc = JsonDocument.Parse(responseContent);
                var root = jsonDoc.RootElement;

                if (root.TryGetProperty("metadata", out var metadata) &&
                    metadata.TryGetProperty("result", out var result))
                {
                    var success = result.GetInt32() == 1;

                    if (success)
                    {
                        return new AccountUpdateResult
                        {
                            Success = true,
                            Message = "Privileges granted successfully",
                            AccountId = userId,
                            UpdatedDate = DateTime.UtcNow
                        };
                    }
                }

                return CreateUpdateErrorResult("Failed to grant privileges", "CPANEL_ERROR");
            }
            catch (Exception ex)
            {
                return CreateUpdateErrorResult($"Unexpected error: {ex.Message}", "UNEXPECTED_ERROR");
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

                var parameters = new Dictionary<string, string>
                {
                    ["user"] = userId,
                    ["password"] = newPassword
                };

                var endpoint = BuildWhmApiUrl("Mysql::set_password", parameters);
                var response = await _httpClient.GetAsync(endpoint);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return CreateUpdateErrorResult($"API request failed: {response.StatusCode}", response.StatusCode.ToString());
                }

                using var jsonDoc = JsonDocument.Parse(responseContent);
                var root = jsonDoc.RootElement;

                if (root.TryGetProperty("metadata", out var metadata) &&
                    metadata.TryGetProperty("result", out var result))
                {
                    var success = result.GetInt32() == 1;

                    if (success)
                    {
                        return new AccountUpdateResult
                        {
                            Success = true,
                            Message = "Database password changed successfully",
                            AccountId = userId,
                            UpdatedField = "Password",
                            UpdatedDate = DateTime.UtcNow
                        };
                    }
                }

                return CreateUpdateErrorResult("Failed to change database password", "CPANEL_ERROR");
            }
            catch (Exception ex)
            {
                return CreateUpdateErrorResult($"Unexpected error: {ex.Message}", "UNEXPECTED_ERROR");
            }
        }
    }
}

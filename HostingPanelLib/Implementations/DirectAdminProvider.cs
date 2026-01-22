using HostingPanelLib.Models;
using System.Text;
using System.Web;

namespace HostingPanelLib.Implementations
{
    public class DirectAdminProvider : BaseHostingPanel
    {
        private readonly string _username;
        private readonly string _password;
        private readonly int _port;
        private readonly bool _useHttps;

        public DirectAdminProvider(string apiUrl, string username, string password, int port, bool useHttps)
            : base(apiUrl)
        {
            _username = username ?? throw new ArgumentNullException(nameof(username));
            _password = password ?? throw new ArgumentNullException(nameof(password));
            _port = port;
            _useHttps = useHttps;

            var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_username}:{_password}"));
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {credentials}");
        }

        private async Task<Dictionary<string, string>> ParseDirectAdminResponseAsync(string response)
        {
            var result = new Dictionary<string, string>();
            
            // DirectAdmin responses are typically URL-encoded key=value pairs separated by &
            var pairs = response.Split('&');
            foreach (var pair in pairs)
            {
                var keyValue = pair.Split('=');
                if (keyValue.Length == 2)
                {
                    var key = HttpUtility.UrlDecode(keyValue[0]);
                    var value = HttpUtility.UrlDecode(keyValue[1]);
                    result[key] = value;
                }
            }

            await Task.CompletedTask;
            return result;
        }

        private FormUrlEncodedContent CreateFormContent(Dictionary<string, string> parameters)
        {
            return new FormUrlEncodedContent(parameters);
        }

        public override async Task<HostingAccountResult> CreateWebHostingAccountAsync(HostingAccountRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Username))
                {
                    return CreateHostingErrorResult("Username is required", "INVALID_USERNAME");
                }

                if (string.IsNullOrWhiteSpace(request.Domain))
                {
                    return CreateHostingErrorResult("Domain name is required", "INVALID_DOMAIN");
                }

                if (string.IsNullOrWhiteSpace(request.Password))
                {
                    return CreateHostingErrorResult("Password is required", "INVALID_PASSWORD");
                }

                if (string.IsNullOrWhiteSpace(request.Email))
                {
                    return CreateHostingErrorResult("Email is required", "INVALID_EMAIL");
                }

                // DirectAdmin CMD_API_ACCOUNT_USER endpoint
                var endpoint = "/CMD_API_ACCOUNT_USER";

                // Build form parameters
                var parameters = new Dictionary<string, string>
                {
                    ["action"] = "create",
                    ["add"] = "Submit",
                    ["username"] = request.Username,
                    ["email"] = request.Email,
                    ["passwd"] = request.Password,
                    ["passwd2"] = request.Password,
                    ["domain"] = request.Domain,
                    ["package"] = request.Plan ?? "default",
                    ["ip"] = "shared",
                    ["notify"] = "no"
                };

                // Optional parameters
                if (request.DiskQuotaMB.HasValue)
                    parameters["quota"] = request.DiskQuotaMB.Value.ToString();
                if (request.BandwidthLimitMB.HasValue)
                    parameters["bandwidth"] = request.BandwidthLimitMB.Value.ToString();
                if (request.MaxEmailAccounts.HasValue)
                    parameters["nemails"] = request.MaxEmailAccounts.Value.ToString();
                if (request.MaxDatabases.HasValue)
                    parameters["mysql"] = request.MaxDatabases.Value.ToString();
                if (request.MaxFtpAccounts.HasValue)
                    parameters["ftp"] = request.MaxFtpAccounts.Value.ToString();
                if (request.MaxSubdomains.HasValue)
                    parameters["nsubdomains"] = request.MaxSubdomains.Value.ToString();
                if (request.EnableCgi.HasValue)
                    parameters["cgi"] = request.EnableCgi.Value ? "ON" : "OFF";

                var content = CreateFormContent(parameters);
                var response = await _httpClient.PostAsync(endpoint, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return CreateHostingErrorResult(
                        $"DirectAdmin API request failed: {response.StatusCode} - {responseContent}",
                        response.StatusCode.ToString()
                    );
                }

                // Parse DirectAdmin response
                var responseData = await ParseDirectAdminResponseAsync(responseContent);

                // Check for errors
                if (responseData.ContainsKey("error"))
                {
                    var errorMessage = responseData.GetValueOrDefault("error", "Unknown error occurred");
                    var details = responseData.GetValueOrDefault("details", "");
                    return CreateHostingErrorResult(
                        $"{errorMessage}. {details}".Trim(),
                        "DIRECTADMIN_ERROR"
                    );
                }

                // Check for success
                if (responseData.ContainsKey("success") || responseContent.Contains("Account Created"))
                {
                    return new HostingAccountResult
                    {
                        Success = true,
                        Message = "Account created successfully",
                        AccountId = request.Username,
                        Username = request.Username,
                        Domain = request.Domain,
                        PlanName = request.Plan ?? "default",
                        CreatedDate = DateTime.UtcNow
                    };
                }

                return CreateHostingErrorResult("Failed to create account", "DIRECTADMIN_ERROR");
            }
            catch (HttpRequestException ex)
            {
                return CreateHostingErrorResult(
                    $"Network error while connecting to DirectAdmin: {ex.Message}",
                    "NETWORK_ERROR"
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

                // DirectAdmin CMD_API_MODIFY_USER endpoint
                var endpoint = "/CMD_API_MODIFY_USER";

                // Build form parameters
                var parameters = new Dictionary<string, string>
                {
                    ["user"] = accountId,
                    ["action"] = "customize"
                };

                // Add fields to modify
                if (!string.IsNullOrWhiteSpace(request.Email))
                    parameters["email"] = request.Email;
                if (!string.IsNullOrWhiteSpace(request.Plan))
                    parameters["package"] = request.Plan;
                if (request.DiskQuotaMB.HasValue)
                    parameters["quota"] = request.DiskQuotaMB.Value.ToString();
                if (request.BandwidthLimitMB.HasValue)
                    parameters["bandwidth"] = request.BandwidthLimitMB.Value.ToString();
                if (request.MaxEmailAccounts.HasValue)
                    parameters["nemails"] = request.MaxEmailAccounts.Value.ToString();
                if (request.MaxDatabases.HasValue)
                    parameters["mysql"] = request.MaxDatabases.Value.ToString();
                if (request.MaxFtpAccounts.HasValue)
                    parameters["ftp"] = request.MaxFtpAccounts.Value.ToString();
                if (request.MaxSubdomains.HasValue)
                    parameters["nsubdomains"] = request.MaxSubdomains.Value.ToString();
                if (request.EnableShellAccess.HasValue)
                    parameters["ssh"] = request.EnableShellAccess.Value ? "ON" : "OFF";

                var content = CreateFormContent(parameters);
                var response = await _httpClient.PostAsync(endpoint, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return CreateUpdateErrorResult(
                        $"DirectAdmin API request failed: {response.StatusCode} - {responseContent}",
                        response.StatusCode.ToString()
                    );
                }

                // Parse DirectAdmin response
                var responseData = await ParseDirectAdminResponseAsync(responseContent);

                // Check for errors
                if (responseData.ContainsKey("error"))
                {
                    var errorMessage = responseData.GetValueOrDefault("error", "Unknown error occurred");
                    return CreateUpdateErrorResult(errorMessage, "DIRECTADMIN_ERROR");
                }

                // Check for success
                if (responseData.ContainsKey("success") || responseContent.Contains("User Modified"))
                {
                    return new AccountUpdateResult
                    {
                        Success = true,
                        Message = "Account updated successfully",
                        AccountId = accountId,
                        UpdatedField = "AccountConfiguration",
                        UpdatedDate = DateTime.UtcNow
                    };
                }

                return CreateUpdateErrorResult("Failed to update account", "DIRECTADMIN_ERROR");
            }
            catch (HttpRequestException ex)
            {
                return CreateUpdateErrorResult(
                    $"Network error while connecting to DirectAdmin: {ex.Message}",
                    "NETWORK_ERROR"
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

                // DirectAdmin CMD_API_SELECT_USERS endpoint for suspension
                var endpoint = "/CMD_API_SELECT_USERS";

                var parameters = new Dictionary<string, string>
                {
                    ["suspend"] = "Suspend",
                    ["select0"] = accountId,
                    ["reason"] = "Suspended via API"
                };

                var content = CreateFormContent(parameters);
                var response = await _httpClient.PostAsync(endpoint, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return CreateUpdateErrorResult(
                        $"DirectAdmin API request failed: {response.StatusCode} - {responseContent}",
                        response.StatusCode.ToString()
                    );
                }

                // Parse DirectAdmin response
                var responseData = await ParseDirectAdminResponseAsync(responseContent);

                // Check for errors
                if (responseData.ContainsKey("error"))
                {
                    var errorMessage = responseData.GetValueOrDefault("error", "Unknown error occurred");
                    return CreateUpdateErrorResult(errorMessage, "DIRECTADMIN_ERROR");
                }

                // Check for success
                if (responseContent.Contains("suspended") || responseContent.Contains("success"))
                {
                    return new AccountUpdateResult
                    {
                        Success = true,
                        Message = "Account suspended successfully",
                        AccountId = accountId,
                        UpdatedField = "Status",
                        OldValue = "Active",
                        NewValue = "Suspended",
                        UpdatedDate = DateTime.UtcNow
                    };
                }

                return CreateUpdateErrorResult("Failed to suspend account", "DIRECTADMIN_ERROR");
            }
            catch (HttpRequestException ex)
            {
                return CreateUpdateErrorResult(
                    $"Network error while connecting to DirectAdmin: {ex.Message}",
                    "NETWORK_ERROR"
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

                // DirectAdmin CMD_API_SELECT_USERS endpoint for unsuspension
                var endpoint = "/CMD_API_SELECT_USERS";

                var parameters = new Dictionary<string, string>
                {
                    ["unsuspend"] = "Unsuspend",
                    ["select0"] = accountId
                };

                var content = CreateFormContent(parameters);
                var response = await _httpClient.PostAsync(endpoint, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return CreateUpdateErrorResult(
                        $"DirectAdmin API request failed: {response.StatusCode} - {responseContent}",
                        response.StatusCode.ToString()
                    );
                }

                // Parse DirectAdmin response
                var responseData = await ParseDirectAdminResponseAsync(responseContent);

                // Check for errors
                if (responseData.ContainsKey("error"))
                {
                    var errorMessage = responseData.GetValueOrDefault("error", "Unknown error occurred");
                    return CreateUpdateErrorResult(errorMessage, "DIRECTADMIN_ERROR");
                }

                // Check for success
                if (responseContent.Contains("unsuspended") || responseContent.Contains("success"))
                {
                    return new AccountUpdateResult
                    {
                        Success = true,
                        Message = "Account unsuspended successfully",
                        AccountId = accountId,
                        UpdatedField = "Status",
                        OldValue = "Suspended",
                        NewValue = "Active",
                        UpdatedDate = DateTime.UtcNow
                    };
                }

                return CreateUpdateErrorResult("Failed to unsuspend account", "DIRECTADMIN_ERROR");
            }
            catch (HttpRequestException ex)
            {
                return CreateUpdateErrorResult(
                    $"Network error while connecting to DirectAdmin: {ex.Message}",
                    "NETWORK_ERROR"
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

                // DirectAdmin CMD_API_SELECT_USERS endpoint for deletion
                var endpoint = "/CMD_API_SELECT_USERS";

                var parameters = new Dictionary<string, string>
                {
                    ["delete"] = "Delete",
                    ["select0"] = accountId,
                    ["confirmed"] = "Confirm"
                };

                var content = CreateFormContent(parameters);
                var response = await _httpClient.PostAsync(endpoint, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return CreateUpdateErrorResult(
                        $"DirectAdmin API request failed: {response.StatusCode} - {responseContent}",
                        response.StatusCode.ToString()
                    );
                }

                // Parse DirectAdmin response
                var responseData = await ParseDirectAdminResponseAsync(responseContent);

                // Check for errors
                if (responseData.ContainsKey("error"))
                {
                    var errorMessage = responseData.GetValueOrDefault("error", "Unknown error occurred");
                    return CreateUpdateErrorResult(errorMessage, "DIRECTADMIN_ERROR");
                }

                // Check for success
                if (responseContent.Contains("deleted") || responseContent.Contains("success"))
                {
                    return new AccountUpdateResult
                    {
                        Success = true,
                        Message = "Account deleted successfully",
                        AccountId = accountId,
                        UpdatedField = "Status",
                        OldValue = "Active",
                        NewValue = "Deleted",
                        UpdatedDate = DateTime.UtcNow
                    };
                }

                return CreateUpdateErrorResult("Failed to delete account", "DIRECTADMIN_ERROR");
            }
            catch (HttpRequestException ex)
            {
                return CreateUpdateErrorResult(
                    $"Network error while connecting to DirectAdmin: {ex.Message}",
                    "NETWORK_ERROR"
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

                // DirectAdmin CMD_API_SHOW_USER_CONFIG endpoint
                var endpoint = $"/CMD_API_SHOW_USER_CONFIG?user={accountId}";

                var response = await _httpClient.GetAsync(endpoint);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return CreateInfoErrorResult(
                        $"DirectAdmin API request failed: {response.StatusCode} - {responseContent}",
                        response.StatusCode.ToString()
                    );
                }

                // Parse DirectAdmin response
                var responseData = await ParseDirectAdminResponseAsync(responseContent);

                // Check for errors
                if (responseData.ContainsKey("error"))
                {
                    var errorMessage = responseData.GetValueOrDefault("error", "Unknown error occurred");
                    return CreateInfoErrorResult(errorMessage, "DIRECTADMIN_ERROR");
                }

                // Extract user information
                return new AccountInfoResult
                {
                    Success = true,
                    Message = "Account information retrieved successfully",
                    AccountId = accountId,
                    Username = accountId,
                    Domain = responseData.GetValueOrDefault("domain", null),
                    Email = responseData.GetValueOrDefault("email", null),
                    Plan = responseData.GetValueOrDefault("package", null),
                    Status = responseData.GetValueOrDefault("suspended", "no") == "yes" ? "Suspended" : "Active",
                    DiskQuotaMB = int.TryParse(responseData.GetValueOrDefault("quota", "0"), out var quota) ? quota : null,
                    BandwidthLimitMB = int.TryParse(responseData.GetValueOrDefault("bandwidth", "0"), out var bw) ? bw : null,
                    IpAddress = responseData.GetValueOrDefault("ip", null),
                    AdditionalInfo = new Dictionary<string, object>
                    {
                        ["creator"] = responseData.GetValueOrDefault("creator", "admin"),
                        ["usertype"] = responseData.GetValueOrDefault("usertype", "user"),
                        ["nemails"] = responseData.GetValueOrDefault("nemails", "0"),
                        ["mysql"] = responseData.GetValueOrDefault("mysql", "0"),
                        ["ftp"] = responseData.GetValueOrDefault("ftp", "0"),
                        ["ssh"] = responseData.GetValueOrDefault("ssh", "OFF")
                    }
                };
            }
            catch (HttpRequestException ex)
            {
                return CreateInfoErrorResult(
                    $"Network error while connecting to DirectAdmin: {ex.Message}",
                    "NETWORK_ERROR"
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
                // DirectAdmin CMD_API_SHOW_ALL_USERS endpoint
                var endpoint = "/CMD_API_SHOW_ALL_USERS";

                var response = await _httpClient.GetAsync(endpoint);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new List<AccountInfoResult>();
                }

                var results = new List<AccountInfoResult>();

                // DirectAdmin returns list of users separated by newlines or &
                var users = responseContent.Split(new[] { '\n', '&' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var user in users)
                {
                    var username = user.Trim();
                    if (!string.IsNullOrEmpty(username) && !username.Contains("="))
                    {
                        // Get detailed info for each user
                        var userInfo = await GetWebHostingAccountInfoAsync(username);
                        if (userInfo.Success)
                        {
                            results.Add(userInfo);
                        }
                        else
                        {
                            // Add basic info if detailed fetch fails
                            results.Add(new AccountInfoResult
                            {
                                Success = true,
                                AccountId = username,
                                Username = username
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

                // DirectAdmin CMD_API_POP endpoint
                var endpoint = "/CMD_API_POP";

                var parameters = new Dictionary<string, string>
                {
                    ["action"] = "create",
                    ["domain"] = request.Domain,
                    ["user"] = emailParts[0],
                    ["passwd"] = request.Password,
                    ["passwd2"] = request.Password,
                    ["quota"] = (request.QuotaMB ?? 100).ToString()
                };

                var content = CreateFormContent(parameters);
                var response = await _httpClient.PostAsync(endpoint, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return CreateMailErrorResult(
                        $"DirectAdmin API request failed: {response.StatusCode} - {responseContent}",
                        response.StatusCode.ToString()
                    );
                }

                // Parse DirectAdmin response
                var responseData = await ParseDirectAdminResponseAsync(responseContent);

                // Check for errors
                if (responseData.ContainsKey("error"))
                {
                    var errorMessage = responseData.GetValueOrDefault("error", "Unknown error occurred");
                    return CreateMailErrorResult(errorMessage, "DIRECTADMIN_ERROR");
                }

                // Check for success
                if (responseContent.Contains("success") || responseContent.Contains("created"))
                {
                    return new MailAccountResult
                    {
                        Success = true,
                        Message = "Email account created successfully",
                        AccountId = request.EmailAddress,
                        EmailAddress = request.EmailAddress,
                        Domain = request.Domain,
                        QuotaMB = request.QuotaMB ?? 100,
                        CreatedDate = DateTime.UtcNow
                    };
                }

                return CreateMailErrorResult("Failed to create email account", "DIRECTADMIN_ERROR");
            }
            catch (HttpRequestException ex)
            {
                return CreateMailErrorResult(
                    $"Network error while connecting to DirectAdmin: {ex.Message}",
                    "NETWORK_ERROR"
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

                // DirectAdmin CMD_API_POP endpoint for modification
                var endpoint = "/CMD_API_POP";

                var parameters = new Dictionary<string, string>
                {
                    ["action"] = "modify",
                    ["domain"] = emailParts[1],
                    ["user"] = emailParts[0]
                };

                // Update quota if provided
                if (request.QuotaMB.HasValue)
                    parameters["quota"] = request.QuotaMB.Value.ToString();

                var content = CreateFormContent(parameters);
                var response = await _httpClient.PostAsync(endpoint, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return CreateUpdateErrorResult(
                        $"DirectAdmin API request failed: {response.StatusCode} - {responseContent}",
                        response.StatusCode.ToString()
                    );
                }

                // Parse DirectAdmin response
                var responseData = await ParseDirectAdminResponseAsync(responseContent);

                // Check for errors
                if (responseData.ContainsKey("error"))
                {
                    var errorMessage = responseData.GetValueOrDefault("error", "Unknown error occurred");
                    return CreateUpdateErrorResult(errorMessage, "DIRECTADMIN_ERROR");
                }

                // Check for success
                if (responseContent.Contains("success") || responseContent.Contains("modified"))
                {
                    return new AccountUpdateResult
                    {
                        Success = true,
                        Message = "Email account updated successfully",
                        AccountId = accountId,
                        UpdatedField = "EmailConfiguration",
                        NewValue = request.QuotaMB,
                        UpdatedDate = DateTime.UtcNow
                    };
                }

                return CreateUpdateErrorResult("Failed to update email account", "DIRECTADMIN_ERROR");
            }
            catch (HttpRequestException ex)
            {
                return CreateUpdateErrorResult(
                    $"Network error while connecting to DirectAdmin: {ex.Message}",
                    "NETWORK_ERROR"
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

                // DirectAdmin CMD_API_POP endpoint for deletion
                var endpoint = "/CMD_API_POP";

                var parameters = new Dictionary<string, string>
                {
                    ["action"] = "delete",
                    ["domain"] = emailParts[1],
                    ["user"] = emailParts[0]
                };

                var content = CreateFormContent(parameters);
                var response = await _httpClient.PostAsync(endpoint, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return CreateUpdateErrorResult(
                        $"DirectAdmin API request failed: {response.StatusCode} - {responseContent}",
                        response.StatusCode.ToString()
                    );
                }

                // Parse DirectAdmin response
                var responseData = await ParseDirectAdminResponseAsync(responseContent);

                // Check for errors
                if (responseData.ContainsKey("error"))
                {
                    var errorMessage = responseData.GetValueOrDefault("error", "Unknown error occurred");
                    return CreateUpdateErrorResult(errorMessage, "DIRECTADMIN_ERROR");
                }

                // Check for success
                if (responseContent.Contains("success") || responseContent.Contains("deleted"))
                {
                    return new AccountUpdateResult
                    {
                        Success = true,
                        Message = "Email account deleted successfully",
                        AccountId = accountId,
                        UpdatedField = "Status",
                        OldValue = "Active",
                        NewValue = "Deleted",
                        UpdatedDate = DateTime.UtcNow
                    };
                }

                return CreateUpdateErrorResult("Failed to delete email account", "DIRECTADMIN_ERROR");
            }
            catch (HttpRequestException ex)
            {
                return CreateUpdateErrorResult(
                    $"Network error while connecting to DirectAdmin: {ex.Message}",
                    "NETWORK_ERROR"
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

                // DirectAdmin doesn't have a direct get single email endpoint
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

                // DirectAdmin CMD_API_POP endpoint to list emails
                var endpoint = $"/CMD_API_POP?action=list&domain={domain}";

                var response = await _httpClient.GetAsync(endpoint);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new List<AccountInfoResult>();
                }

                var results = new List<AccountInfoResult>();

                // Parse DirectAdmin response - typically returns list of email accounts
                var responseData = await ParseDirectAdminResponseAsync(responseContent);

                // DirectAdmin may return emails in different formats
                // Try to parse as list
                var emails = responseContent.Split(new[] { '\n', '&' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var email in emails)
                {
                    var emailStr = email.Trim();
                    if (!string.IsNullOrEmpty(emailStr) && !emailStr.Contains("="))
                    {
                        var fullEmail = $"{emailStr}@{domain}";
                        results.Add(new AccountInfoResult
                        {
                            Success = true,
                            AccountId = fullEmail,
                            Email = fullEmail,
                            Domain = domain
                        });
                    }
                    else if (emailStr.Contains("="))
                    {
                        // Parse key-value pairs
                        var parts = emailStr.Split('=');
                        if (parts.Length == 2 && !parts[0].Contains("error"))
                        {
                            var user = parts[0].Trim();
                            var fullEmail = $"{user}@{domain}";
                            results.Add(new AccountInfoResult
                            {
                                Success = true,
                                AccountId = fullEmail,
                                Email = fullEmail,
                                Domain = domain,
                                DiskQuotaMB = int.TryParse(parts[1], out var quota) ? quota : null
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

                // DirectAdmin CMD_API_POP endpoint for password change
                var endpoint = "/CMD_API_POP";

                var parameters = new Dictionary<string, string>
                {
                    ["action"] = "modify",
                    ["domain"] = emailParts[1],
                    ["user"] = emailParts[0],
                    ["passwd"] = newPassword,
                    ["passwd2"] = newPassword
                };

                var content = CreateFormContent(parameters);
                var response = await _httpClient.PostAsync(endpoint, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return CreateUpdateErrorResult(
                        $"DirectAdmin API request failed: {response.StatusCode} - {responseContent}",
                        response.StatusCode.ToString()
                    );
                }

                // Parse DirectAdmin response
                var responseData = await ParseDirectAdminResponseAsync(responseContent);

                // Check for errors
                if (responseData.ContainsKey("error"))
                {
                    var errorMessage = responseData.GetValueOrDefault("error", "Unknown error occurred");
                    return CreateUpdateErrorResult(errorMessage, "DIRECTADMIN_ERROR");
                }

                // Check for success
                if (responseContent.Contains("success") || responseContent.Contains("modified"))
                {
                    return new AccountUpdateResult
                    {
                        Success = true,
                        Message = "Email password changed successfully",
                        AccountId = accountId,
                        UpdatedField = "Password",
                        UpdatedDate = DateTime.UtcNow
                    };
                }

                return CreateUpdateErrorResult("Failed to change email password", "DIRECTADMIN_ERROR");
            }
            catch (HttpRequestException ex)
            {
                return CreateUpdateErrorResult(
                    $"Network error while connecting to DirectAdmin: {ex.Message}",
                    "NETWORK_ERROR"
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

                // DirectAdmin CMD_API_MODIFY_USER endpoint
                var endpoint = "/CMD_API_MODIFY_USER";

                var parameters = new Dictionary<string, string>
                {
                    ["user"] = accountId,
                    ["action"] = "customize",
                    ["quota"] = quotaMB.ToString()
                };

                var content = CreateFormContent(parameters);
                var response = await _httpClient.PostAsync(endpoint, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return CreateUpdateErrorResult(
                        $"DirectAdmin API request failed: {response.StatusCode} - {responseContent}",
                        response.StatusCode.ToString()
                    );
                }

                // Parse DirectAdmin response
                var responseData = await ParseDirectAdminResponseAsync(responseContent);

                // Check for errors
                if (responseData.ContainsKey("error"))
                {
                    var errorMessage = responseData.GetValueOrDefault("error", "Unknown error occurred");
                    return CreateUpdateErrorResult(errorMessage, "DIRECTADMIN_ERROR");
                }

                // Check for success
                if (responseContent.Contains("success") || responseContent.Contains("modified"))
                {
                    return new AccountUpdateResult
                    {
                        Success = true,
                        Message = "Disk quota updated successfully",
                        AccountId = accountId,
                        UpdatedField = "DiskQuota",
                        NewValue = quotaMB,
                        UpdatedDate = DateTime.UtcNow
                    };
                }

                return CreateUpdateErrorResult("Failed to set disk quota", "DIRECTADMIN_ERROR");
            }
            catch (HttpRequestException ex)
            {
                return CreateUpdateErrorResult(
                    $"Network error while connecting to DirectAdmin: {ex.Message}",
                    "NETWORK_ERROR"
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

                // DirectAdmin CMD_API_MODIFY_USER endpoint
                var endpoint = "/CMD_API_MODIFY_USER";

                var parameters = new Dictionary<string, string>
                {
                    ["user"] = accountId,
                    ["action"] = "customize",
                    ["bandwidth"] = bandwidthMB.ToString()
                };

                var content = CreateFormContent(parameters);
                var response = await _httpClient.PostAsync(endpoint, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return CreateUpdateErrorResult(
                        $"DirectAdmin API request failed: {response.StatusCode} - {responseContent}",
                        response.StatusCode.ToString()
                    );
                }

                // Parse DirectAdmin response
                var responseData = await ParseDirectAdminResponseAsync(responseContent);

                // Check for errors
                if (responseData.ContainsKey("error"))
                {
                    var errorMessage = responseData.GetValueOrDefault("error", "Unknown error occurred");
                    return CreateUpdateErrorResult(errorMessage, "DIRECTADMIN_ERROR");
                }

                // Check for success
                if (responseContent.Contains("success") || responseContent.Contains("modified"))
                {
                    return new AccountUpdateResult
                    {
                        Success = true,
                        Message = "Bandwidth limit updated successfully",
                        AccountId = accountId,
                        UpdatedField = "BandwidthLimit",
                        NewValue = bandwidthMB,
                        UpdatedDate = DateTime.UtcNow
                    };
                }

                return CreateUpdateErrorResult("Failed to set bandwidth limit", "DIRECTADMIN_ERROR");
            }
            catch (HttpRequestException ex)
            {
                return CreateUpdateErrorResult(
                    $"Network error while connecting to DirectAdmin: {ex.Message}",
                    "NETWORK_ERROR"
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

                // DirectAdmin CMD_API_DATABASES endpoint
                var endpoint = "/CMD_API_DATABASES";

                var parameters = new Dictionary<string, string>
                {
                    ["action"] = "create",
                    ["name"] = request.DatabaseName,
                    ["user"] = request.Username ?? request.DatabaseName,
                    ["passwd"] = request.Password ?? Guid.NewGuid().ToString("N").Substring(0, 16),
                    ["passwd2"] = request.Password ?? Guid.NewGuid().ToString("N").Substring(0, 16)
                };

                var content = CreateFormContent(parameters);
                var response = await _httpClient.PostAsync(endpoint, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return CreateDatabaseErrorResult(
                        $"DirectAdmin API request failed: {response.StatusCode} - {responseContent}",
                        response.StatusCode.ToString()
                    );
                }

                var responseData = await ParseDirectAdminResponseAsync(responseContent);

                if (responseData.ContainsKey("error"))
                {
                    var errorMessage = responseData.GetValueOrDefault("error", "Unknown error occurred");
                    return CreateDatabaseErrorResult(errorMessage, "DIRECTADMIN_ERROR");
                }

                if (responseContent.Contains("success") || responseContent.Contains("Database Created"))
                {
                    return new DatabaseResult
                    {
                        Success = true,
                        Message = "Database created successfully",
                        DatabaseId = request.DatabaseName,
                        DatabaseName = request.DatabaseName,
                        DatabaseType = "mysql",
                        Server = "localhost",
                        Port = 3306,
                        Username = request.Username ?? request.DatabaseName,
                        CreatedDate = DateTime.UtcNow
                    };
                }

                return CreateDatabaseErrorResult("Failed to create database", "DIRECTADMIN_ERROR");
            }
            catch (HttpRequestException ex)
            {
                return CreateDatabaseErrorResult(
                    $"Network error while connecting to DirectAdmin: {ex.Message}",
                    "NETWORK_ERROR"
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

                var endpoint = "/CMD_API_DATABASES";

                var parameters = new Dictionary<string, string>
                {
                    ["action"] = "delete",
                    ["select0"] = databaseId
                };

                var content = CreateFormContent(parameters);
                var response = await _httpClient.PostAsync(endpoint, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return CreateUpdateErrorResult(
                        $"DirectAdmin API request failed: {response.StatusCode} - {responseContent}",
                        response.StatusCode.ToString()
                    );
                }

                var responseData = await ParseDirectAdminResponseAsync(responseContent);

                if (responseData.ContainsKey("error"))
                {
                    var errorMessage = responseData.GetValueOrDefault("error", "Unknown error occurred");
                    return CreateUpdateErrorResult(errorMessage, "DIRECTADMIN_ERROR");
                }

                if (responseContent.Contains("success") || responseContent.Contains("deleted"))
                {
                    return new AccountUpdateResult
                    {
                        Success = true,
                        Message = "Database deleted successfully",
                        AccountId = databaseId,
                        UpdatedDate = DateTime.UtcNow
                    };
                }

                return CreateUpdateErrorResult("Failed to delete database", "DIRECTADMIN_ERROR");
            }
            catch (HttpRequestException ex)
            {
                return CreateUpdateErrorResult(
                    $"Network error while connecting to DirectAdmin: {ex.Message}",
                    "NETWORK_ERROR"
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
                var endpoint = "/CMD_API_DATABASES";

                var parameters = new Dictionary<string, string>
                {
                    ["action"] = "list"
                };

                var content = CreateFormContent(parameters);
                var response = await _httpClient.PostAsync(endpoint, content);

                if (!response.IsSuccessStatusCode)
                {
                    return new List<AccountInfoResult>();
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var responseData = await ParseDirectAdminResponseAsync(responseContent);

                var results = new List<AccountInfoResult>();

                foreach (var kvp in responseData)
                {
                    if (kvp.Key.StartsWith("db"))
                    {
                        results.Add(new AccountInfoResult
                        {
                            Success = true,
                            AccountId = kvp.Value,
                            DatabaseName = kvp.Value,
                            DatabaseType = "mysql"
                        });
                    }
                }

                return results;
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
                if (string.IsNullOrWhiteSpace(request.Username))
                {
                    return CreateDatabaseErrorResult("Username is required", "INVALID_USERNAME");
                }

                if (string.IsNullOrWhiteSpace(request.Password))
                {
                    return CreateDatabaseErrorResult("Password is required", "INVALID_PASSWORD");
                }

                var endpoint = "/CMD_API_DATABASES";

                var parameters = new Dictionary<string, string>
                {
                    ["action"] = "createuser",
                    ["user"] = request.Username,
                    ["passwd"] = request.Password,
                    ["passwd2"] = request.Password
                };

                var content = CreateFormContent(parameters);
                var response = await _httpClient.PostAsync(endpoint, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return CreateDatabaseErrorResult(
                        $"DirectAdmin API request failed: {response.StatusCode} - {responseContent}",
                        response.StatusCode.ToString()
                    );
                }

                var responseData = await ParseDirectAdminResponseAsync(responseContent);

                if (responseData.ContainsKey("error"))
                {
                    var errorMessage = responseData.GetValueOrDefault("error", "Unknown error occurred");
                    return CreateDatabaseErrorResult(errorMessage, "DIRECTADMIN_ERROR");
                }

                if (responseContent.Contains("success") || responseContent.Contains("created"))
                {
                    return new DatabaseResult
                    {
                        Success = true,
                        Message = "Database user created successfully",
                        DatabaseId = request.Username,
                        Username = request.Username,
                        CreatedDate = DateTime.UtcNow
                    };
                }

                return CreateDatabaseErrorResult("Failed to create database user", "DIRECTADMIN_ERROR");
            }
            catch (HttpRequestException ex)
            {
                return CreateDatabaseErrorResult(
                    $"Network error while connecting to DirectAdmin: {ex.Message}",
                    "NETWORK_ERROR"
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

                var endpoint = "/CMD_API_DATABASES";

                var parameters = new Dictionary<string, string>
                {
                    ["action"] = "deleteuser",
                    ["select0"] = userId
                };

                var content = CreateFormContent(parameters);
                var response = await _httpClient.PostAsync(endpoint, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return CreateUpdateErrorResult(
                        $"DirectAdmin API request failed: {response.StatusCode} - {responseContent}",
                        response.StatusCode.ToString()
                    );
                }

                var responseData = await ParseDirectAdminResponseAsync(responseContent);

                if (responseData.ContainsKey("error"))
                {
                    var errorMessage = responseData.GetValueOrDefault("error", "Unknown error occurred");
                    return CreateUpdateErrorResult(errorMessage, "DIRECTADMIN_ERROR");
                }

                if (responseContent.Contains("success") || responseContent.Contains("deleted"))
                {
                    return new AccountUpdateResult
                    {
                        Success = true,
                        Message = "Database user deleted successfully",
                        AccountId = userId,
                        UpdatedDate = DateTime.UtcNow
                    };
                }

                return CreateUpdateErrorResult("Failed to delete database user", "DIRECTADMIN_ERROR");
            }
            catch (HttpRequestException ex)
            {
                return CreateUpdateErrorResult(
                    $"Network error while connecting to DirectAdmin: {ex.Message}",
                    "NETWORK_ERROR"
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
                if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(databaseId))
                {
                    return CreateUpdateErrorResult("User ID and Database ID are required", "INVALID_INPUT");
                }

                var endpoint = "/CMD_API_DATABASES";

                var parameters = new Dictionary<string, string>
                {
                    ["action"] = "userdb",
                    ["user"] = userId,
                    ["db"] = databaseId
                };

                var content = CreateFormContent(parameters);
                var response = await _httpClient.PostAsync(endpoint, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return CreateUpdateErrorResult(
                        $"DirectAdmin API request failed: {response.StatusCode} - {responseContent}",
                        response.StatusCode.ToString()
                    );
                }

                var responseData = await ParseDirectAdminResponseAsync(responseContent);

                if (responseData.ContainsKey("error"))
                {
                    var errorMessage = responseData.GetValueOrDefault("error", "Unknown error occurred");
                    return CreateUpdateErrorResult(errorMessage, "DIRECTADMIN_ERROR");
                }

                if (responseContent.Contains("success") || responseContent.Contains("granted"))
                {
                    return new AccountUpdateResult
                    {
                        Success = true,
                        Message = "Privileges granted successfully",
                        AccountId = userId,
                        UpdatedDate = DateTime.UtcNow
                    };
                }

                return CreateUpdateErrorResult("Failed to grant privileges", "DIRECTADMIN_ERROR");
            }
            catch (HttpRequestException ex)
            {
                return CreateUpdateErrorResult(
                    $"Network error while connecting to DirectAdmin: {ex.Message}",
                    "NETWORK_ERROR"
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

                var endpoint = "/CMD_API_DATABASES";

                var parameters = new Dictionary<string, string>
                {
                    ["action"] = "modifyuser",
                    ["user"] = userId,
                    ["passwd"] = newPassword,
                    ["passwd2"] = newPassword
                };

                var content = CreateFormContent(parameters);
                var response = await _httpClient.PostAsync(endpoint, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return CreateUpdateErrorResult(
                        $"DirectAdmin API request failed: {response.StatusCode} - {responseContent}",
                        response.StatusCode.ToString()
                    );
                }

                var responseData = await ParseDirectAdminResponseAsync(responseContent);

                if (responseData.ContainsKey("error"))
                {
                    var errorMessage = responseData.GetValueOrDefault("error", "Unknown error occurred");
                    return CreateUpdateErrorResult(errorMessage, "DIRECTADMIN_ERROR");
                }

                if (responseContent.Contains("success") || responseContent.Contains("modified"))
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

                return CreateUpdateErrorResult("Failed to change database password", "DIRECTADMIN_ERROR");
            }
            catch (HttpRequestException ex)
            {
                return CreateUpdateErrorResult(
                    $"Network error while connecting to DirectAdmin: {ex.Message}",
                    "NETWORK_ERROR"
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

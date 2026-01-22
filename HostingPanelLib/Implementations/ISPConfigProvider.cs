using HostingPanelLib.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace HostingPanelLib.Implementations
{
    public class ISPConfigProvider : BaseHostingPanel
    {
        private readonly string _username;
        private readonly string _password;
        private readonly int _port;
        private readonly bool _useHttps;
        private readonly string? _remoteApiUrl;
        private string? _sessionId;

        public ISPConfigProvider(string apiUrl, string username, string password, int port, bool useHttps, string? remoteApiUrl)
            : base(apiUrl)
        {
            _username = username ?? throw new ArgumentNullException(nameof(username));
            _password = password ?? throw new ArgumentNullException(nameof(password));
            _port = port;
            _useHttps = useHttps;
            _remoteApiUrl = remoteApiUrl ?? apiUrl;
        }

        private async Task<bool> LoginAsync()
        {
            try
            {
                if (!string.IsNullOrEmpty(_sessionId))
                {
                    return true; // Already logged in
                }

                var endpoint = $"{_remoteApiUrl}/remote/json.php?login";
                
                var loginData = new
                {
                    username = _username,
                    password = _password
                };

                var response = await _httpClient.PostAsJsonAsync(endpoint, loginData);
                var responseContent = await response.Content.ReadAsStringAsync();


                if (!response.IsSuccessStatusCode)
                {
                    return false;
                }

                using var jsonDoc = JsonDocument.Parse(responseContent);
                var root = jsonDoc.RootElement;

                if (root.TryGetProperty("code", out var codeElement) && codeElement.GetString() == "ok" &&
                    root.TryGetProperty("response", out var sessionElement))
                {
                    _sessionId = sessionElement.GetString();
                    return !string.IsNullOrEmpty(_sessionId);
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        private async Task<T?> CallRemoteApiAsync<T>(string method, object? parameters = null)
        {
            if (!await LoginAsync())
            {
                return default;
            }

            var endpoint = $"{_remoteApiUrl}/remote/json.php?{method}";
            
            var requestData = new
            {
                session_id = _sessionId,
                param = parameters ?? new { }
            };

            var response = await _httpClient.PostAsJsonAsync(endpoint, requestData);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return default;
            }

            using var jsonDoc = JsonDocument.Parse(responseContent);
            var root = jsonDoc.RootElement;

            if (root.TryGetProperty("response", out var responseElement))
            {
                return JsonSerializer.Deserialize<T>(responseElement.GetRawText());
            }

            return default;
        }

        public override async Task<HostingAccountResult> CreateWebHostingAccountAsync(HostingAccountRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Domain))
                {
                    return CreateHostingErrorResult("Domain name is required", "INVALID_DOMAIN");
                }

                if (!await LoginAsync())
                {
                    return CreateHostingErrorResult("Failed to authenticate with ISPConfig", "AUTH_FAILED");
                }

                var parameters = new
                {
                    client_id = 1, // Default client
                    domain = request.Domain,
                    type = "vhost",
                    parent_domain_id = 0,
                    vhost_type = "name",
                    hd_quota = request.DiskQuotaMB ?? 1000,
                    traffic_quota = request.BandwidthLimitMB ?? 10000,
                    cgi = request.EnableCgi ?? false ? "y" : "n",
                    ssi = "n",
                    suexec = "y",
                    errordocs = 1,
                    is_subdomainwww = 1,
                    subdomain = "www",
                    php = "fast-cgi",
                    active = "y"
                };

                var result = await CallRemoteApiAsync<Dictionary<string, object>>("sites_web_domain_add", parameters);

                if (result != null && result.ContainsKey("domain_id"))
                {
                    return new HostingAccountResult
                    {
                        Success = true,
                        Message = "Web hosting account created successfully",
                        AccountId = result["domain_id"].ToString(),
                        Domain = request.Domain,
                        CreatedDate = DateTime.UtcNow
                    };
                }

                return CreateHostingErrorResult("Failed to create web hosting account", "ISPCONFIG_ERROR");
            }
            catch (Exception ex)
            {
                return CreateHostingErrorResult($"Unexpected error: {ex.Message}", "UNEXPECTED_ERROR");
            }
        }

        public override async Task<AccountUpdateResult> UpdateWebHostingAccountAsync(string accountId, HostingAccountRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(accountId))
                {
                    return CreateUpdateErrorResult("Account ID (domain ID) is required", "INVALID_ACCOUNT_ID");
                }

                if (!await LoginAsync())
                {
                    return CreateUpdateErrorResult("Failed to authenticate with ISPConfig", "AUTH_FAILED");
                }

                var parameters = new
                {
                    primary_id = int.Parse(accountId),
                    domain = request.Domain,
                    hd_quota = request.DiskQuotaMB,
                    traffic_quota = request.BandwidthLimitMB
                };

                var result = await CallRemoteApiAsync<bool>("sites_web_domain_update", parameters);

                if (result)
                {
                    return new AccountUpdateResult
                    {
                        Success = true,
                        Message = "Web hosting account updated successfully",
                        AccountId = accountId,
                        UpdatedDate = DateTime.UtcNow
                    };
                }

                return CreateUpdateErrorResult("Failed to update web hosting account", "ISPCONFIG_ERROR");
            }
            catch (Exception ex)
            {
                return CreateUpdateErrorResult($"Unexpected error: {ex.Message}", "UNEXPECTED_ERROR");
            }
        }

        public override async Task<AccountUpdateResult> SuspendWebHostingAccountAsync(string accountId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(accountId))
                {
                    return CreateUpdateErrorResult("Account ID (domain ID) is required", "INVALID_ACCOUNT_ID");
                }

                if (!await LoginAsync())
                {
                    return CreateUpdateErrorResult("Failed to authenticate with ISPConfig", "AUTH_FAILED");
                }

                var parameters = new
                {
                    primary_id = int.Parse(accountId),
                    active = "n"
                };

                var result = await CallRemoteApiAsync<bool>("sites_web_domain_update", parameters);

                if (result)
                {
                    return new AccountUpdateResult
                    {
                        Success = true,
                        Message = "Web hosting account suspended successfully",
                        AccountId = accountId,
                        UpdatedField = "Status",
                        OldValue = "Active",
                        NewValue = "Suspended",
                        UpdatedDate = DateTime.UtcNow
                    };
                }

                return CreateUpdateErrorResult("Failed to suspend web hosting account", "ISPCONFIG_ERROR");
            }
            catch (Exception ex)
            {
                return CreateUpdateErrorResult($"Unexpected error: {ex.Message}", "UNEXPECTED_ERROR");
            }
        }

        public override async Task<AccountUpdateResult> UnsuspendWebHostingAccountAsync(string accountId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(accountId))
                {
                    return CreateUpdateErrorResult("Account ID (domain ID) is required", "INVALID_ACCOUNT_ID");
                }

                if (!await LoginAsync())
                {
                    return CreateUpdateErrorResult("Failed to authenticate with ISPConfig", "AUTH_FAILED");
                }

                var parameters = new
                {
                    primary_id = int.Parse(accountId),
                    active = "y"
                };

                var result = await CallRemoteApiAsync<bool>("sites_web_domain_update", parameters);

                if (result)
                {
                    return new AccountUpdateResult
                    {
                        Success = true,
                        Message = "Web hosting account unsuspended successfully",
                        AccountId = accountId,
                        UpdatedField = "Status",
                        OldValue = "Suspended",
                        NewValue = "Active",
                        UpdatedDate = DateTime.UtcNow
                    };
                }

                return CreateUpdateErrorResult("Failed to unsuspend web hosting account", "ISPCONFIG_ERROR");
            }
            catch (Exception ex)
            {
                return CreateUpdateErrorResult($"Unexpected error: {ex.Message}", "UNEXPECTED_ERROR");
            }
        }

        public override async Task<AccountUpdateResult> DeleteWebHostingAccountAsync(string accountId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(accountId))
                {
                    return CreateUpdateErrorResult("Account ID (domain ID) is required", "INVALID_ACCOUNT_ID");
                }

                if (!await LoginAsync())
                {
                    return CreateUpdateErrorResult("Failed to authenticate with ISPConfig", "AUTH_FAILED");
                }

                var parameters = new { primary_id = int.Parse(accountId) };
                var result = await CallRemoteApiAsync<bool>("sites_web_domain_delete", parameters);

                if (result)
                {
                    return new AccountUpdateResult
                    {
                        Success = true,
                        Message = "Web hosting account deleted successfully",
                        AccountId = accountId,
                        UpdatedDate = DateTime.UtcNow
                    };
                }

                return CreateUpdateErrorResult("Failed to delete web hosting account", "ISPCONFIG_ERROR");
            }
            catch (Exception ex)
            {
                return CreateUpdateErrorResult($"Unexpected error: {ex.Message}", "UNEXPECTED_ERROR");
            }
        }

        public override async Task<AccountInfoResult> GetWebHostingAccountInfoAsync(string accountId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(accountId))
                {
                    return CreateInfoErrorResult("Account ID (domain ID) is required", "INVALID_ACCOUNT_ID");
                }

                if (!await LoginAsync())
                {
                    return CreateInfoErrorResult("Failed to authenticate with ISPConfig", "AUTH_FAILED");
                }

                var parameters = new { primary_id = int.Parse(accountId) };
                var result = await CallRemoteApiAsync<Dictionary<string, object>>("sites_web_domain_get", parameters);

                if (result != null)
                {
                    return new AccountInfoResult
                    {
                        Success = true,
                        Message = "Account information retrieved successfully",
                        AccountId = accountId,
                        Domain = result.GetValueOrDefault("domain", null)?.ToString(),
                        Status = result.GetValueOrDefault("active", "n")?.ToString() == "y" ? "Active" : "Suspended",
                        DiskQuotaMB = result.ContainsKey("hd_quota") ? Convert.ToInt32(result["hd_quota"]) : null,
                        BandwidthLimitMB = result.ContainsKey("traffic_quota") ? Convert.ToInt32(result["traffic_quota"]) : null,
                        IpAddress = result.GetValueOrDefault("ip_address", null)?.ToString()
                    };
                }

                return CreateInfoErrorResult("Account not found", "NOT_FOUND");
            }
            catch (Exception ex)
            {
                return CreateInfoErrorResult($"Unexpected error: {ex.Message}", "UNEXPECTED_ERROR");
            }
        }

        public override async Task<List<AccountInfoResult>> ListWebHostingAccountsAsync()
        {
            try
            {
                if (!await LoginAsync())
                {
                    return new List<AccountInfoResult>();
                }

                var result = await CallRemoteApiAsync<List<Dictionary<string, object>>>("client_get_sites_by_user", new { });

                if (result != null)
                {
                    return result.Select(site => new AccountInfoResult
                    {
                        Success = true,
                        AccountId = site.GetValueOrDefault("domain_id", "")?.ToString() ?? "",
                        Domain = site.GetValueOrDefault("domain", null)?.ToString(),
                        Status = site.GetValueOrDefault("active", "n")?.ToString() == "y" ? "Active" : "Suspended"
                    }).ToList();
                }

                return new List<AccountInfoResult>();
            }
            catch
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

                if (!await LoginAsync())
                {
                    return CreateMailErrorResult("Failed to authenticate with ISPConfig", "AUTH_FAILED");
                }

                var emailParts = request.EmailAddress.Split('@');
                if (emailParts.Length != 2)
                {
                    return CreateMailErrorResult("Invalid email address format", "INVALID_EMAIL_FORMAT");
                }

                var parameters = new
                {
                    server_id = 1,
                    email = request.EmailAddress,
                    login = emailParts[0],
                    password = request.Password,
                    name = request.DisplayName ?? emailParts[0],
                    quota = (request.QuotaMB ?? 100) * 1024 * 1024, // Convert MB to bytes
                    cc = "",
                    forward_in_lda = "n",
                    sender_cc = "",
                    homedir = $"/var/vmail/{emailParts[1]}/{emailParts[0]}",
                    autoresponder = request.EnableAutoresponder ?? false ? "y" : "n",
                    autoresponder_text = request.AutoresponderMessage ?? ""
                };

                var result = await CallRemoteApiAsync<Dictionary<string, object>>("mail_user_add", parameters);

                if (result != null && result.ContainsKey("mailuser_id"))
                {
                    return new MailAccountResult
                    {
                        Success = true,
                        Message = "Email account created successfully",
                        AccountId = result["mailuser_id"].ToString(),
                        EmailAddress = request.EmailAddress,
                        Domain = emailParts[1],
                        QuotaMB = request.QuotaMB ?? 100,
                        CreatedDate = DateTime.UtcNow
                    };
                }

                return CreateMailErrorResult("Failed to create email account", "ISPCONFIG_ERROR");
            }
            catch (Exception ex)
            {
                return CreateMailErrorResult($"Unexpected error: {ex.Message}", "UNEXPECTED_ERROR");
            }
        }

        public override async Task<AccountUpdateResult> UpdateMailAccountAsync(string accountId, MailAccountRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(accountId))
                {
                    return CreateUpdateErrorResult("Account ID is required", "INVALID_ACCOUNT_ID");
                }

                if (!await LoginAsync())
                {
                    return CreateUpdateErrorResult("Failed to authenticate with ISPConfig", "AUTH_FAILED");
                }

                var parameters = new
                {
                    primary_id = int.Parse(accountId),
                    quota = request.QuotaMB.HasValue ? request.QuotaMB.Value * 1024 * 1024 : 104857600
                };

                var result = await CallRemoteApiAsync<bool>("mail_user_update", parameters);

                if (result)
                {
                    return new AccountUpdateResult
                    {
                        Success = true,
                        Message = "Email account updated successfully",
                        AccountId = accountId,
                        UpdatedDate = DateTime.UtcNow
                    };
                }

                return CreateUpdateErrorResult("Failed to update email account", "ISPCONFIG_ERROR");
            }
            catch (Exception ex)
            {
                return CreateUpdateErrorResult($"Unexpected error: {ex.Message}", "UNEXPECTED_ERROR");
            }
        }

        public override async Task<AccountUpdateResult> DeleteMailAccountAsync(string accountId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(accountId))
                {
                    return CreateUpdateErrorResult("Account ID is required", "INVALID_ACCOUNT_ID");
                }

                if (!await LoginAsync())
                {
                    return CreateUpdateErrorResult("Failed to authenticate with ISPConfig", "AUTH_FAILED");
                }

                var parameters = new { primary_id = int.Parse(accountId) };
                var result = await CallRemoteApiAsync<bool>("mail_user_delete", parameters);

                if (result)
                {
                    return new AccountUpdateResult
                    {
                        Success = true,
                        Message = "Email account deleted successfully",
                        AccountId = accountId,
                        UpdatedDate = DateTime.UtcNow
                    };
                }

                return CreateUpdateErrorResult("Failed to delete email account", "ISPCONFIG_ERROR");
            }
            catch (Exception ex)
            {
                return CreateUpdateErrorResult($"Unexpected error: {ex.Message}", "UNEXPECTED_ERROR");
            }
        }

        public override async Task<AccountInfoResult> GetMailAccountInfoAsync(string accountId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(accountId))
                {
                    return CreateInfoErrorResult("Account ID is required", "INVALID_ACCOUNT_ID");
                }

                if (!await LoginAsync())
                {
                    return CreateInfoErrorResult("Failed to authenticate with ISPConfig", "AUTH_FAILED");
                }

                var parameters = new { primary_id = int.Parse(accountId) };
                var result = await CallRemoteApiAsync<Dictionary<string, object>>("mail_user_get", parameters);

                if (result != null)
                {
                    var quotaBytes = result.ContainsKey("quota") ? Convert.ToInt64(result["quota"]) : 0;
                    return new AccountInfoResult
                    {
                        Success = true,
                        Message = "Email account retrieved successfully",
                        AccountId = accountId,
                        Email = result.GetValueOrDefault("email", null)?.ToString(),
                        DiskQuotaMB = quotaBytes > 0 ? (int)(quotaBytes / 1024 / 1024) : null
                    };
                }

                return CreateInfoErrorResult("Email account not found", "NOT_FOUND");
            }
            catch (Exception ex)
            {
                return CreateInfoErrorResult($"Unexpected error: {ex.Message}", "UNEXPECTED_ERROR");
            }
        }

        public override async Task<List<AccountInfoResult>> ListMailAccountsAsync(string domain)
        {
            try
            {
                if (!await LoginAsync())
                {
                    return new List<AccountInfoResult>();
                }

                // ISPConfig doesn't have direct domain filter, we get all and filter
                var result = await CallRemoteApiAsync<List<Dictionary<string, object>>>("mail_user_get", new { });

                if (result != null)
                {
                    return result
                        .Where(mail => mail.GetValueOrDefault("email", "")?.ToString()?.EndsWith($"@{domain}") ?? false)
                        .Select(mail =>
                        {
                            var quotaBytes = mail.ContainsKey("quota") ? Convert.ToInt64(mail["quota"]) : 0;
                            return new AccountInfoResult
                            {
                                Success = true,
                                AccountId = mail.GetValueOrDefault("mailuser_id", "")?.ToString() ?? "",
                                Email = mail.GetValueOrDefault("email", null)?.ToString(),
                                Domain = domain,
                                DiskQuotaMB = quotaBytes > 0 ? (int)(quotaBytes / 1024 / 1024) : null
                            };
                        }).ToList();
                }

                return new List<AccountInfoResult>();
            }
            catch
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
                    return CreateUpdateErrorResult("Account ID is required", "INVALID_ACCOUNT_ID");
                }

                if (!await LoginAsync())
                {
                    return CreateUpdateErrorResult("Failed to authenticate with ISPConfig", "AUTH_FAILED");
                }

                var parameters = new
                {
                    primary_id = int.Parse(accountId),
                    password = newPassword
                };

                var result = await CallRemoteApiAsync<bool>("mail_user_update", parameters);

                if (result)
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

                return CreateUpdateErrorResult("Failed to change email password", "ISPCONFIG_ERROR");
            }
            catch (Exception ex)
            {
                return CreateUpdateErrorResult($"Unexpected error: {ex.Message}", "UNEXPECTED_ERROR");
            }
        }

        public override async Task<AccountUpdateResult> SetDiskQuotaAsync(string accountId, int quotaMB)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(accountId))
                {
                    return CreateUpdateErrorResult("Account ID is required", "INVALID_ACCOUNT_ID");
                }

                if (!await LoginAsync())
                {
                    return CreateUpdateErrorResult("Failed to authenticate with ISPConfig", "AUTH_FAILED");
                }

                var parameters = new
                {
                    primary_id = int.Parse(accountId),
                    hd_quota = quotaMB
                };

                var result = await CallRemoteApiAsync<bool>("sites_web_domain_update", parameters);

                if (result)
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

                return CreateUpdateErrorResult("Failed to set disk quota", "ISPCONFIG_ERROR");
            }
            catch (Exception ex)
            {
                return CreateUpdateErrorResult($"Unexpected error: {ex.Message}", "UNEXPECTED_ERROR");
            }
        }

        public override async Task<AccountUpdateResult> SetBandwidthLimitAsync(string accountId, int bandwidthMB)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(accountId))
                {
                    return CreateUpdateErrorResult("Account ID is required", "INVALID_ACCOUNT_ID");
                }

                if (!await LoginAsync())
                {
                    return CreateUpdateErrorResult("Failed to authenticate with ISPConfig", "AUTH_FAILED");
                }

                var parameters = new
                {
                    primary_id = int.Parse(accountId),
                    traffic_quota = bandwidthMB
                };

                var result = await CallRemoteApiAsync<bool>("sites_web_domain_update", parameters);

                if (result)
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

                return CreateUpdateErrorResult("Failed to set bandwidth limit", "ISPCONFIG_ERROR");
            }
            catch (Exception ex)
            {
                return CreateUpdateErrorResult($"Unexpected error: {ex.Message}", "UNEXPECTED_ERROR");
            }
        }
    }
}

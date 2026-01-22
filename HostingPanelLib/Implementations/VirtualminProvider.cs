using HostingPanelLib.Models;
using System.Text;

namespace HostingPanelLib.Implementations
{
    public class VirtualminProvider : BaseHostingPanel
    {
        private readonly string _username;
        private readonly string _password;
        private readonly int _port;
        private readonly bool _useHttps;

        public VirtualminProvider(string apiUrl, string username, string password, int port, bool useHttps)
            : base(apiUrl)
        {
            _username = username ?? throw new ArgumentNullException(nameof(username));
            _password = password ?? throw new ArgumentNullException(nameof(password));
            _port = port;
            _useHttps = useHttps;

            var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_username}:{_password}"));
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {credentials}");
        }

        private async Task<string> ExecuteVirtualminCommandAsync(string program, Dictionary<string, string>? parameters = null)
        {
            var queryString = parameters != null
                ? string.Join("&", parameters.Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"))
                : string.Empty;

            var endpoint = $"/virtual-server/remote.cgi?program={program}&json=1&{queryString}";
            
            var response = await _httpClient.GetAsync(endpoint);
            return await response.Content.ReadAsStringAsync();
        }

        public override async Task<HostingAccountResult> CreateWebHostingAccountAsync(HostingAccountRequest request)
        {
            try
            {
                var parameters = new Dictionary<string, string>
                {
                    ["domain"] = request.Domain ?? "",
                    ["pass"] = request.Password ?? "",
                    ["unix"] = "",
                    ["dir"] = ""
                };

                if (request.DiskQuotaMB.HasValue)
                    parameters["quota"] = (request.DiskQuotaMB.Value * 1024).ToString();
                if (request.BandwidthLimitMB.HasValue)
                    parameters["bw-limit"] = request.BandwidthLimitMB.Value.ToString();

                var response = await ExecuteVirtualminCommandAsync("create-domain", parameters);
                
                if (response.Contains("success") || !response.Contains("error"))
                {
                    return new HostingAccountResult
                    {
                        Success = true,
                        Message = "Domain created successfully",
                        Domain = request.Domain,
                        CreatedDate = DateTime.UtcNow
                    };
                }

                return CreateHostingErrorResult(response, "VIRTUALMIN_ERROR");
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
                var parameters = new Dictionary<string, string> { ["domain"] = accountId };
                if (request.DiskQuotaMB.HasValue)
                    parameters["quota"] = (request.DiskQuotaMB.Value * 1024).ToString();
                if (request.BandwidthLimitMB.HasValue)
                    parameters["bw-limit"] = request.BandwidthLimitMB.Value.ToString();

                var response = await ExecuteVirtualminCommandAsync("modify-domain", parameters);
                
                if (!response.Contains("error"))
                {
                    return new AccountUpdateResult { Success = true, Message = "Domain updated", AccountId = accountId, UpdatedDate = DateTime.UtcNow };
                }

                return CreateUpdateErrorResult(response, "VIRTUALMIN_ERROR");
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
                var parameters = new Dictionary<string, string> { ["domain"] = accountId };
                var response = await ExecuteVirtualminCommandAsync("disable-domain", parameters);
                
                if (!response.Contains("error"))
                {
                    return new AccountUpdateResult { Success = true, Message = "Domain suspended", AccountId = accountId, UpdatedField = "Status", NewValue = "Suspended", UpdatedDate = DateTime.UtcNow };
                }

                return CreateUpdateErrorResult(response, "VIRTUALMIN_ERROR");
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
                var parameters = new Dictionary<string, string> { ["domain"] = accountId };
                var response = await ExecuteVirtualminCommandAsync("enable-domain", parameters);
                
                if (!response.Contains("error"))
                {
                    return new AccountUpdateResult { Success = true, Message = "Domain unsuspended", AccountId = accountId, UpdatedField = "Status", NewValue = "Active", UpdatedDate = DateTime.UtcNow };
                }

                return CreateUpdateErrorResult(response, "VIRTUALMIN_ERROR");
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
                var parameters = new Dictionary<string, string> { ["domain"] = accountId };
                var response = await ExecuteVirtualminCommandAsync("delete-domain", parameters);
                
                if (!response.Contains("error"))
                {
                    return new AccountUpdateResult { Success = true, Message = "Domain deleted", AccountId = accountId, UpdatedDate = DateTime.UtcNow };
                }

                return CreateUpdateErrorResult(response, "VIRTUALMIN_ERROR");
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
                var parameters = new Dictionary<string, string> { ["domain"] = accountId };
                var response = await ExecuteVirtualminCommandAsync("list-domains", parameters);
                
                return new AccountInfoResult { Success = true, AccountId = accountId, Domain = accountId };
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
                await ExecuteVirtualminCommandAsync("list-domains", new Dictionary<string, string> { ["all-domains"] = "" });
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
                var emailParts = request.EmailAddress?.Split('@') ?? Array.Empty<string>();
                var parameters = new Dictionary<string, string>
                {
                    ["domain"] = request.Domain ?? "",
                    ["user"] = emailParts[0],
                    ["pass"] = request.Password ?? "",
                    ["quota"] = ((request.QuotaMB ?? 100) * 1024).ToString()
                };

                var response = await ExecuteVirtualminCommandAsync("create-user", parameters);
                
                if (!response.Contains("error"))
                {
                    return new MailAccountResult { Success = true, Message = "Mail account created", EmailAddress = request.EmailAddress, Domain = request.Domain, CreatedDate = DateTime.UtcNow };
                }

                return CreateMailErrorResult(response, "VIRTUALMIN_ERROR");
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
                var emailParts = accountId.Split('@');
                var parameters = new Dictionary<string, string>
                {
                    ["domain"] = emailParts[1],
                    ["user"] = emailParts[0],
                    ["quota"] = ((request.QuotaMB ?? 100) * 1024).ToString()
                };

                var response = await ExecuteVirtualminCommandAsync("modify-user", parameters);
                
                if (!response.Contains("error"))
                {
                    return new AccountUpdateResult { Success = true, Message = "Mail account updated", AccountId = accountId, UpdatedDate = DateTime.UtcNow };
                }

                return CreateUpdateErrorResult(response, "VIRTUALMIN_ERROR");
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
                var emailParts = accountId.Split('@');
                var parameters = new Dictionary<string, string>
                {
                    ["domain"] = emailParts[1],
                    ["user"] = emailParts[0]
                };

                var response = await ExecuteVirtualminCommandAsync("delete-user", parameters);
                
                if (!response.Contains("error"))
                {
                    return new AccountUpdateResult { Success = true, Message = "Mail account deleted", AccountId = accountId, UpdatedDate = DateTime.UtcNow };
                }

                return CreateUpdateErrorResult(response, "VIRTUALMIN_ERROR");
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
                var emailParts = accountId.Split('@');
                return new AccountInfoResult { Success = true, AccountId = accountId, Email = accountId, Domain = emailParts[1] };
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
                await ExecuteVirtualminCommandAsync("list-users", new Dictionary<string, string> { ["domain"] = domain });
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
                var emailParts = accountId.Split('@');
                var parameters = new Dictionary<string, string>
                {
                    ["domain"] = emailParts[1],
                    ["user"] = emailParts[0],
                    ["pass"] = newPassword
                };

                var response = await ExecuteVirtualminCommandAsync("modify-user", parameters);
                
                if (!response.Contains("error"))
                {
                    return new AccountUpdateResult { Success = true, Message = "Password changed", AccountId = accountId, UpdatedField = "Password", UpdatedDate = DateTime.UtcNow };
                }

                return CreateUpdateErrorResult(response, "VIRTUALMIN_ERROR");
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
                var parameters = new Dictionary<string, string>
                {
                    ["domain"] = accountId,
                    ["quota"] = (quotaMB * 1024).ToString()
                };

                var response = await ExecuteVirtualminCommandAsync("modify-domain", parameters);
                
                if (!response.Contains("error"))
                {
                    return new AccountUpdateResult { Success = true, Message = "Quota updated", AccountId = accountId, UpdatedField = "DiskQuota", NewValue = quotaMB, UpdatedDate = DateTime.UtcNow };
                }

                return CreateUpdateErrorResult(response, "VIRTUALMIN_ERROR");
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
                var parameters = new Dictionary<string, string>
                {
                    ["domain"] = accountId,
                    ["bw-limit"] = bandwidthMB.ToString()
                };

                var response = await ExecuteVirtualminCommandAsync("modify-domain", parameters);
                
                if (!response.Contains("error"))
                {
                    return new AccountUpdateResult { Success = true, Message = "Bandwidth limit updated", AccountId = accountId, UpdatedField = "BandwidthLimit", NewValue = bandwidthMB, UpdatedDate = DateTime.UtcNow };
                }

                return CreateUpdateErrorResult(response, "VIRTUALMIN_ERROR");
            }
            catch (Exception ex)
            {
                return CreateUpdateErrorResult($"Unexpected error: {ex.Message}", "UNEXPECTED_ERROR");
            }
        }
    }
}

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

                if (root.TryGetProperty("code\", out var codeElement) && codeElement.GetString() == \"ok\" &&
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
            // TODO: Implement ISPConfig sites_web_domain_add
            await Task.CompletedTask;
            return CreateHostingErrorResult("Not yet implemented", "NOT_IMPLEMENTED");
        }

        public override async Task<AccountUpdateResult> UpdateWebHostingAccountAsync(string accountId, HostingAccountRequest request)
        {
            // TODO: Implement ISPConfig sites_web_domain_update
            await Task.CompletedTask;
            return CreateUpdateErrorResult("Not yet implemented", "NOT_IMPLEMENTED");
        }

        public override async Task<AccountUpdateResult> SuspendWebHostingAccountAsync(string accountId)
        {
            // TODO: Implement ISPConfig sites_web_domain_update with active=n
            await Task.CompletedTask;
            return CreateUpdateErrorResult("Not yet implemented", "NOT_IMPLEMENTED");
        }

        public override async Task<AccountUpdateResult> UnsuspendWebHostingAccountAsync(string accountId)
        {
            // TODO: Implement ISPConfig sites_web_domain_update with active=y
            await Task.CompletedTask;
            return CreateUpdateErrorResult("Not yet implemented", "NOT_IMPLEMENTED");
        }

        public override async Task<AccountUpdateResult> DeleteWebHostingAccountAsync(string accountId)
        {
            // TODO: Implement ISPConfig sites_web_domain_delete
            await Task.CompletedTask;
            return CreateUpdateErrorResult("Not yet implemented", "NOT_IMPLEMENTED");
        }

        public override async Task<AccountInfoResult> GetWebHostingAccountInfoAsync(string accountId)
        {
            // TODO: Implement ISPConfig sites_web_domain_get
            await Task.CompletedTask;
            return CreateInfoErrorResult("Not yet implemented", "NOT_IMPLEMENTED");
        }

        public override async Task<List<AccountInfoResult>> ListWebHostingAccountsAsync()
        {
            // TODO: Implement ISPConfig client_get_sites_by_user
            await Task.CompletedTask;
            return new List<AccountInfoResult>();
        }

        public override async Task<MailAccountResult> CreateMailAccountAsync(MailAccountRequest request)
        {
            // TODO: Implement ISPConfig mail_user_add
            await Task.CompletedTask;
            return CreateMailErrorResult("Not yet implemented", "NOT_IMPLEMENTED");
        }

        public override async Task<AccountUpdateResult> UpdateMailAccountAsync(string accountId, MailAccountRequest request)
        {
            // TODO: Implement ISPConfig mail_user_update
            await Task.CompletedTask;
            return CreateUpdateErrorResult("Not yet implemented", "NOT_IMPLEMENTED");
        }

        public override async Task<AccountUpdateResult> DeleteMailAccountAsync(string accountId)
        {
            // TODO: Implement ISPConfig mail_user_delete
            await Task.CompletedTask;
            return CreateUpdateErrorResult("Not yet implemented", "NOT_IMPLEMENTED");
        }

        public override async Task<AccountInfoResult> GetMailAccountInfoAsync(string accountId)
        {
            // TODO: Implement ISPConfig mail_user_get
            await Task.CompletedTask;
            return CreateInfoErrorResult("Not yet implemented", "NOT_IMPLEMENTED");
        }

        public override async Task<List<AccountInfoResult>> ListMailAccountsAsync(string domain)
        {
            // TODO: Implement ISPConfig mail_user_get by domain
            await Task.CompletedTask;
            return new List<AccountInfoResult>();
        }

        public override async Task<AccountUpdateResult> ChangeMailPasswordAsync(string accountId, string newPassword)
        {
            // TODO: Implement ISPConfig mail_user_update password
            await Task.CompletedTask;
            return CreateUpdateErrorResult("Not yet implemented", "NOT_IMPLEMENTED");
        }

        public override async Task<AccountUpdateResult> SetDiskQuotaAsync(string accountId, int quotaMB)
        {
            // TODO: Implement ISPConfig sites_web_domain_update quota
            await Task.CompletedTask;
            return CreateUpdateErrorResult("Not yet implemented", "NOT_IMPLEMENTED");
        }

        public override async Task<AccountUpdateResult> SetBandwidthLimitAsync(string accountId, int bandwidthMB)
        {
            // TODO: Implement ISPConfig sites_web_domain_update traffic quota
            await Task.CompletedTask;
            return CreateUpdateErrorResult("Not yet implemented", "NOT_IMPLEMENTED");
        }
    }
}

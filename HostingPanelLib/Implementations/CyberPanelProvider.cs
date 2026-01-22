using HostingPanelLib.Models;

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
            // TODO: Implement CyberPanel createWebsite API
            await Task.CompletedTask;
            return CreateHostingErrorResult("Not yet implemented", "NOT_IMPLEMENTED");
        }

        public override async Task<AccountUpdateResult> UpdateWebHostingAccountAsync(string accountId, HostingAccountRequest request)
        {
            // TODO: Implement CyberPanel modifyWebsite API
            await Task.CompletedTask;
            return CreateUpdateErrorResult("Not yet implemented", "NOT_IMPLEMENTED");
        }

        public override async Task<AccountUpdateResult> SuspendWebHostingAccountAsync(string accountId)
        {
            // TODO: Implement CyberPanel suspendWebsite API
            await Task.CompletedTask;
            return CreateUpdateErrorResult("Not yet implemented", "NOT_IMPLEMENTED");
        }

        public override async Task<AccountUpdateResult> UnsuspendWebHostingAccountAsync(string accountId)
        {
            // TODO: Implement CyberPanel unsuspendWebsite API
            await Task.CompletedTask;
            return CreateUpdateErrorResult("Not yet implemented", "NOT_IMPLEMENTED");
        }

        public override async Task<AccountUpdateResult> DeleteWebHostingAccountAsync(string accountId)
        {
            // TODO: Implement CyberPanel deleteWebsite API
            await Task.CompletedTask;
            return CreateUpdateErrorResult("Not yet implemented", "NOT_IMPLEMENTED");
        }

        public override async Task<AccountInfoResult> GetWebHostingAccountInfoAsync(string accountId)
        {
            // TODO: Implement CyberPanel getWebsiteDetails API
            await Task.CompletedTask;
            return CreateInfoErrorResult("Not yet implemented", "NOT_IMPLEMENTED");
        }

        public override async Task<List<AccountInfoResult>> ListWebHostingAccountsAsync()
        {
            // TODO: Implement CyberPanel listWebsites API
            await Task.CompletedTask;
            return new List<AccountInfoResult>();
        }

        public override async Task<MailAccountResult> CreateMailAccountAsync(MailAccountRequest request)
        {
            // TODO: Implement CyberPanel createEmail API
            await Task.CompletedTask;
            return CreateMailErrorResult("Not yet implemented", "NOT_IMPLEMENTED");
        }

        public override async Task<AccountUpdateResult> UpdateMailAccountAsync(string accountId, MailAccountRequest request)
        {
            // TODO: Implement CyberPanel modifyEmail API
            await Task.CompletedTask;
            return CreateUpdateErrorResult("Not yet implemented", "NOT_IMPLEMENTED");
        }

        public override async Task<AccountUpdateResult> DeleteMailAccountAsync(string accountId)
        {
            // TODO: Implement CyberPanel deleteEmail API
            await Task.CompletedTask;
            return CreateUpdateErrorResult("Not yet implemented", "NOT_IMPLEMENTED");
        }

        public override async Task<AccountInfoResult> GetMailAccountInfoAsync(string accountId)
        {
            // TODO: Implement CyberPanel getEmailDetails API
            await Task.CompletedTask;
            return CreateInfoErrorResult("Not yet implemented", "NOT_IMPLEMENTED");
        }

        public override async Task<List<AccountInfoResult>> ListMailAccountsAsync(string domain)
        {
            // TODO: Implement CyberPanel listEmails API
            await Task.CompletedTask;
            return new List<AccountInfoResult>();
        }

        public override async Task<AccountUpdateResult> ChangeMailPasswordAsync(string accountId, string newPassword)
        {
            // TODO: Implement CyberPanel changeEmailPassword API
            await Task.CompletedTask;
            return CreateUpdateErrorResult("Not yet implemented", "NOT_IMPLEMENTED");
        }

        public override async Task<AccountUpdateResult> SetDiskQuotaAsync(string accountId, int quotaMB)
        {
            // TODO: Implement CyberPanel modifyWebsite disk quota
            await Task.CompletedTask;
            return CreateUpdateErrorResult("Not yet implemented", "NOT_IMPLEMENTED");
        }

        public override async Task<AccountUpdateResult> SetBandwidthLimitAsync(string accountId, int bandwidthMB)
        {
            // TODO: Implement CyberPanel modifyWebsite bandwidth limit
            await Task.CompletedTask;
            return CreateUpdateErrorResult("Not yet implemented", "NOT_IMPLEMENTED");
        }
    }
}

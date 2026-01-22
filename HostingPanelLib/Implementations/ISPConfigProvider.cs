using HostingPanelLib.Models;

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
            _remoteApiUrl = remoteApiUrl;
        }

        private async Task<bool> LoginAsync()
        {
            // TODO: Implement ISPConfig remote API login
            await Task.CompletedTask;
            return false;
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

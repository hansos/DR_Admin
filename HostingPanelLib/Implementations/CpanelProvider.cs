using HostingPanelLib.Models;

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

        public override async Task<HostingAccountResult> CreateWebHostingAccountAsync(HostingAccountRequest request)
        {
            // TODO: Implement cPanel WHM API createacct call
            await Task.CompletedTask;
            return CreateHostingErrorResult("Not yet implemented", "NOT_IMPLEMENTED");
        }

        public override async Task<AccountUpdateResult> UpdateWebHostingAccountAsync(string accountId, HostingAccountRequest request)
        {
            // TODO: Implement cPanel WHM API modifyacct call
            await Task.CompletedTask;
            return CreateUpdateErrorResult("Not yet implemented", "NOT_IMPLEMENTED");
        }

        public override async Task<AccountUpdateResult> SuspendWebHostingAccountAsync(string accountId)
        {
            // TODO: Implement cPanel WHM API suspendacct call
            await Task.CompletedTask;
            return CreateUpdateErrorResult("Not yet implemented", "NOT_IMPLEMENTED");
        }

        public override async Task<AccountUpdateResult> UnsuspendWebHostingAccountAsync(string accountId)
        {
            // TODO: Implement cPanel WHM API unsuspendacct call
            await Task.CompletedTask;
            return CreateUpdateErrorResult("Not yet implemented", "NOT_IMPLEMENTED");
        }

        public override async Task<AccountUpdateResult> DeleteWebHostingAccountAsync(string accountId)
        {
            // TODO: Implement cPanel WHM API removeacct call
            await Task.CompletedTask;
            return CreateUpdateErrorResult("Not yet implemented", "NOT_IMPLEMENTED");
        }

        public override async Task<AccountInfoResult> GetWebHostingAccountInfoAsync(string accountId)
        {
            // TODO: Implement cPanel WHM API accountsummary call
            await Task.CompletedTask;
            return CreateInfoErrorResult("Not yet implemented", "NOT_IMPLEMENTED");
        }

        public override async Task<List<AccountInfoResult>> ListWebHostingAccountsAsync()
        {
            // TODO: Implement cPanel WHM API listaccts call
            await Task.CompletedTask;
            return new List<AccountInfoResult>();
        }

        public override async Task<MailAccountResult> CreateMailAccountAsync(MailAccountRequest request)
        {
            // TODO: Implement cPanel API Email account creation
            await Task.CompletedTask;
            return CreateMailErrorResult("Not yet implemented", "NOT_IMPLEMENTED");
        }

        public override async Task<AccountUpdateResult> UpdateMailAccountAsync(string accountId, MailAccountRequest request)
        {
            // TODO: Implement cPanel API Email account update
            await Task.CompletedTask;
            return CreateUpdateErrorResult("Not yet implemented", "NOT_IMPLEMENTED");
        }

        public override async Task<AccountUpdateResult> DeleteMailAccountAsync(string accountId)
        {
            // TODO: Implement cPanel API Email account deletion
            await Task.CompletedTask;
            return CreateUpdateErrorResult("Not yet implemented", "NOT_IMPLEMENTED");
        }

        public override async Task<AccountInfoResult> GetMailAccountInfoAsync(string accountId)
        {
            // TODO: Implement cPanel API Email account info
            await Task.CompletedTask;
            return CreateInfoErrorResult("Not yet implemented", "NOT_IMPLEMENTED");
        }

        public override async Task<List<AccountInfoResult>> ListMailAccountsAsync(string domain)
        {
            // TODO: Implement cPanel API list email accounts
            await Task.CompletedTask;
            return new List<AccountInfoResult>();
        }

        public override async Task<AccountUpdateResult> ChangeMailPasswordAsync(string accountId, string newPassword)
        {
            // TODO: Implement cPanel API change email password
            await Task.CompletedTask;
            return CreateUpdateErrorResult("Not yet implemented", "NOT_IMPLEMENTED");
        }

        public override async Task<AccountUpdateResult> SetDiskQuotaAsync(string accountId, int quotaMB)
        {
            // TODO: Implement cPanel WHM API modifyacct disk quota
            await Task.CompletedTask;
            return CreateUpdateErrorResult("Not yet implemented", "NOT_IMPLEMENTED");
        }

        public override async Task<AccountUpdateResult> SetBandwidthLimitAsync(string accountId, int bandwidthMB)
        {
            // TODO: Implement cPanel WHM API modifyacct bandwidth limit
            await Task.CompletedTask;
            return CreateUpdateErrorResult("Not yet implemented", "NOT_IMPLEMENTED");
        }
    }
}

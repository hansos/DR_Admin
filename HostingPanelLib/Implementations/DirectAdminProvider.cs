using HostingPanelLib.Models;
using System.Text;

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

        public override async Task<HostingAccountResult> CreateWebHostingAccountAsync(HostingAccountRequest request)
        {
            // TODO: Implement DirectAdmin CMD_API_ACCOUNT_USER
            await Task.CompletedTask;
            return CreateHostingErrorResult("Not yet implemented", "NOT_IMPLEMENTED");
        }

        public override async Task<AccountUpdateResult> UpdateWebHostingAccountAsync(string accountId, HostingAccountRequest request)
        {
            // TODO: Implement DirectAdmin CMD_API_MODIFY_USER
            await Task.CompletedTask;
            return CreateUpdateErrorResult("Not yet implemented", "NOT_IMPLEMENTED");
        }

        public override async Task<AccountUpdateResult> SuspendWebHostingAccountAsync(string accountId)
        {
            // TODO: Implement DirectAdmin CMD_API_SELECT_USERS suspend
            await Task.CompletedTask;
            return CreateUpdateErrorResult("Not yet implemented", "NOT_IMPLEMENTED");
        }

        public override async Task<AccountUpdateResult> UnsuspendWebHostingAccountAsync(string accountId)
        {
            // TODO: Implement DirectAdmin CMD_API_SELECT_USERS unsuspend
            await Task.CompletedTask;
            return CreateUpdateErrorResult("Not yet implemented", "NOT_IMPLEMENTED");
        }

        public override async Task<AccountUpdateResult> DeleteWebHostingAccountAsync(string accountId)
        {
            // TODO: Implement DirectAdmin CMD_API_SELECT_USERS delete
            await Task.CompletedTask;
            return CreateUpdateErrorResult("Not yet implemented", "NOT_IMPLEMENTED");
        }

        public override async Task<AccountInfoResult> GetWebHostingAccountInfoAsync(string accountId)
        {
            // TODO: Implement DirectAdmin CMD_API_SHOW_USER_CONFIG
            await Task.CompletedTask;
            return CreateInfoErrorResult("Not yet implemented", "NOT_IMPLEMENTED");
        }

        public override async Task<List<AccountInfoResult>> ListWebHostingAccountsAsync()
        {
            // TODO: Implement DirectAdmin CMD_API_SHOW_ALL_USERS
            await Task.CompletedTask;
            return new List<AccountInfoResult>();
        }

        public override async Task<MailAccountResult> CreateMailAccountAsync(MailAccountRequest request)
        {
            // TODO: Implement DirectAdmin CMD_API_POP
            await Task.CompletedTask;
            return CreateMailErrorResult("Not yet implemented", "NOT_IMPLEMENTED");
        }

        public override async Task<AccountUpdateResult> UpdateMailAccountAsync(string accountId, MailAccountRequest request)
        {
            // TODO: Implement DirectAdmin CMD_API_POP modify
            await Task.CompletedTask;
            return CreateUpdateErrorResult("Not yet implemented", "NOT_IMPLEMENTED");
        }

        public override async Task<AccountUpdateResult> DeleteMailAccountAsync(string accountId)
        {
            // TODO: Implement DirectAdmin CMD_API_POP delete
            await Task.CompletedTask;
            return CreateUpdateErrorResult("Not yet implemented", "NOT_IMPLEMENTED");
        }

        public override async Task<AccountInfoResult> GetMailAccountInfoAsync(string accountId)
        {
            // TODO: Implement DirectAdmin CMD_API_POP get info
            await Task.CompletedTask;
            return CreateInfoErrorResult("Not yet implemented", "NOT_IMPLEMENTED");
        }

        public override async Task<List<AccountInfoResult>> ListMailAccountsAsync(string domain)
        {
            // TODO: Implement DirectAdmin CMD_API_POP list
            await Task.CompletedTask;
            return new List<AccountInfoResult>();
        }

        public override async Task<AccountUpdateResult> ChangeMailPasswordAsync(string accountId, string newPassword)
        {
            // TODO: Implement DirectAdmin CMD_API_POP change password
            await Task.CompletedTask;
            return CreateUpdateErrorResult("Not yet implemented", "NOT_IMPLEMENTED");
        }

        public override async Task<AccountUpdateResult> SetDiskQuotaAsync(string accountId, int quotaMB)
        {
            // TODO: Implement DirectAdmin CMD_API_MODIFY_USER quota
            await Task.CompletedTask;
            return CreateUpdateErrorResult("Not yet implemented", "NOT_IMPLEMENTED");
        }

        public override async Task<AccountUpdateResult> SetBandwidthLimitAsync(string accountId, int bandwidthMB)
        {
            // TODO: Implement DirectAdmin CMD_API_MODIFY_USER bandwidth
            await Task.CompletedTask;
            return CreateUpdateErrorResult("Not yet implemented", "NOT_IMPLEMENTED");
        }
    }
}

using HostingPanelLib.Interfaces;
using HostingPanelLib.Models;

namespace HostingPanelLib.Implementations
{
    public abstract class BaseHostingPanel : IHostingPanel
    {
        protected readonly string _apiUrl;
        protected readonly HttpClient _httpClient;

        protected BaseHostingPanel(string apiUrl)
        {
            _apiUrl = apiUrl;
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(apiUrl)
            };
        }

        public abstract Task<HostingAccountResult> CreateWebHostingAccountAsync(HostingAccountRequest request);
        public abstract Task<AccountUpdateResult> UpdateWebHostingAccountAsync(string accountId, HostingAccountRequest request);
        public abstract Task<AccountUpdateResult> SuspendWebHostingAccountAsync(string accountId);
        public abstract Task<AccountUpdateResult> UnsuspendWebHostingAccountAsync(string accountId);
        public abstract Task<AccountUpdateResult> DeleteWebHostingAccountAsync(string accountId);
        public abstract Task<AccountInfoResult> GetWebHostingAccountInfoAsync(string accountId);
        public abstract Task<List<AccountInfoResult>> ListWebHostingAccountsAsync();
        public abstract Task<MailAccountResult> CreateMailAccountAsync(MailAccountRequest request);
        public abstract Task<AccountUpdateResult> UpdateMailAccountAsync(string accountId, MailAccountRequest request);
        public abstract Task<AccountUpdateResult> DeleteMailAccountAsync(string accountId);
        public abstract Task<AccountInfoResult> GetMailAccountInfoAsync(string accountId);
        public abstract Task<List<AccountInfoResult>> ListMailAccountsAsync(string domain);
        public abstract Task<AccountUpdateResult> ChangeMailPasswordAsync(string accountId, string newPassword);
        public abstract Task<AccountUpdateResult> SetDiskQuotaAsync(string accountId, int quotaMB);
        public abstract Task<AccountUpdateResult> SetBandwidthLimitAsync(string accountId, int bandwidthMB);
        public abstract Task<DatabaseResult> CreateDatabaseAsync(DatabaseRequest request);
        public abstract Task<AccountUpdateResult> DeleteDatabaseAsync(string databaseId);
        public abstract Task<AccountInfoResult> GetDatabaseInfoAsync(string databaseId);
        public abstract Task<List<AccountInfoResult>> ListDatabasesAsync(string domain);
        public abstract Task<DatabaseResult> CreateDatabaseUserAsync(DatabaseUserRequest request);
        public abstract Task<AccountUpdateResult> DeleteDatabaseUserAsync(string userId);
        public abstract Task<AccountUpdateResult> GrantDatabasePrivilegesAsync(string userId, string databaseId, List<string> privileges);
        public abstract Task<AccountUpdateResult> ChangeDatabasePasswordAsync(string userId, string newPassword);

        protected virtual HostingAccountResult CreateHostingErrorResult(string message, string? errorCode = null)
        {
            return new HostingAccountResult
            {
                Success = false,
                Message = message,
                ErrorCode = errorCode,
                Errors = new List<string> { message }
            };
        }

        protected virtual MailAccountResult CreateMailErrorResult(string message, string? errorCode = null)
        {
            return new MailAccountResult
            {
                Success = false,
                Message = message,
                ErrorCode = errorCode,
                Errors = new List<string> { message }
            };
        }

        protected virtual AccountUpdateResult CreateUpdateErrorResult(string message, string? errorCode = null)
        {
            return new AccountUpdateResult
            {
                Success = false,
                Message = message,
                ErrorCode = errorCode,
                Errors = new List<string> { message }
            };
        }

        protected virtual AccountInfoResult CreateInfoErrorResult(string message, string? errorCode = null)
        {
            return new AccountInfoResult
            {
                Success = false,
                Message = message,
                ErrorCode = errorCode,
                Errors = new List<string> { message }
            };
        }

        protected virtual DatabaseResult CreateDatabaseErrorResult(string message, string? errorCode = null)
        {
            return new DatabaseResult
            {
                Success = false,
                Message = message,
                ErrorCode = errorCode,
                Errors = new List<string> { message }
            };
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _httpClient?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}

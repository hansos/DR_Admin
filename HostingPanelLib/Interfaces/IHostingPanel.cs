using HostingPanelLib.Models;

namespace HostingPanelLib.Interfaces
{
    public interface IHostingPanel
    {
        /// <summary>
        /// Creates a new web hosting account
        /// </summary>
        Task<HostingAccountResult> CreateWebHostingAccountAsync(HostingAccountRequest request);

        /// <summary>
        /// Updates an existing web hosting account
        /// </summary>
        Task<AccountUpdateResult> UpdateWebHostingAccountAsync(string accountId, HostingAccountRequest request);

        /// <summary>
        /// Suspends a web hosting account
        /// </summary>
        Task<AccountUpdateResult> SuspendWebHostingAccountAsync(string accountId);

        /// <summary>
        /// Unsuspends a web hosting account
        /// </summary>
        Task<AccountUpdateResult> UnsuspendWebHostingAccountAsync(string accountId);

        /// <summary>
        /// Deletes a web hosting account
        /// </summary>
        Task<AccountUpdateResult> DeleteWebHostingAccountAsync(string accountId);

        /// <summary>
        /// Gets information about a web hosting account
        /// </summary>
        Task<AccountInfoResult> GetWebHostingAccountInfoAsync(string accountId);

        /// <summary>
        /// Lists all web hosting accounts
        /// </summary>
        Task<List<AccountInfoResult>> ListWebHostingAccountsAsync();

        /// <summary>
        /// Creates a new email account
        /// </summary>
        Task<MailAccountResult> CreateMailAccountAsync(MailAccountRequest request);

        /// <summary>
        /// Updates an existing email account
        /// </summary>
        Task<AccountUpdateResult> UpdateMailAccountAsync(string accountId, MailAccountRequest request);

        /// <summary>
        /// Deletes an email account
        /// </summary>
        Task<AccountUpdateResult> DeleteMailAccountAsync(string accountId);

        /// <summary>
        /// Gets information about an email account
        /// </summary>
        Task<AccountInfoResult> GetMailAccountInfoAsync(string accountId);

        /// <summary>
        /// Lists all email accounts for a domain
        /// </summary>
        Task<List<AccountInfoResult>> ListMailAccountsAsync(string domain);

        /// <summary>
        /// Changes password for an email account
        /// </summary>
        Task<AccountUpdateResult> ChangeMailPasswordAsync(string accountId, string newPassword);

        /// <summary>
        /// Sets disk quota for an account
        /// </summary>
        Task<AccountUpdateResult> SetDiskQuotaAsync(string accountId, int quotaMB);

        /// <summary>
        /// Sets bandwidth limit for an account
        /// </summary>
        Task<AccountUpdateResult> SetBandwidthLimitAsync(string accountId, int bandwidthMB);
    }
}

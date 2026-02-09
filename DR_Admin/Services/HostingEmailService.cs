using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;
using HostingPanelLib.Models;
using HostingPanelLib.Factories;
using HostingPanelLib.Infrastructure.Settings;
using HostingPanelLib.Interfaces;

namespace ISPAdmin.Services;

/// <summary>
/// Service for managing hosting email accounts
/// </summary>
public class HostingEmailService : IHostingEmailService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<HostingEmailService> _logger;
    private static readonly Serilog.ILogger _log = Log.ForContext<HostingEmailService>();

    public HostingEmailService(
        ApplicationDbContext context,
        ILogger<HostingEmailService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<HostingEmailAccountDto?> GetEmailAccountAsync(int id)
    {
        try
        {
            var email = await _context.HostingEmailAccounts
                .Include(e => e.HostingAccount)
                .FirstOrDefaultAsync(e => e.Id == id);

            return email != null ? MapToDto(email) : null;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error getting email account {EmailId}", id);
            throw;
        }
    }

    public async Task<List<HostingEmailAccountDto>> GetEmailAccountsByHostingAccountAsync(int hostingAccountId)
    {
        try
        {
            var emails = await _context.HostingEmailAccounts
                .Include(e => e.HostingAccount)
                .Where(e => e.HostingAccountId == hostingAccountId)
                .OrderBy(e => e.EmailAddress)
                .ToListAsync();

            return emails.Select(MapToDto).ToList();
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error getting email accounts for hosting account {AccountId}", hostingAccountId);
            throw;
        }
    }

    public async Task<List<HostingEmailAccountDto>> GetEmailAccountsByDomainAsync(int hostingAccountId, string domain)
    {
        try
        {
            var emails = await _context.HostingEmailAccounts
                .Include(e => e.HostingAccount)
                .Where(e => e.HostingAccountId == hostingAccountId && e.EmailAddress.EndsWith("@" + domain))
                .OrderBy(e => e.EmailAddress)
                .ToListAsync();

            return emails.Select(MapToDto).ToList();
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error getting email accounts for domain {Domain}", domain);
            throw;
        }
    }

    public async Task<HostingEmailAccountDto> CreateEmailAccountAsync(HostingEmailAccountCreateDto dto, bool syncToServer = false)
    {
        try
        {
            _log.Information("Creating email account {EmailAddress}", dto.EmailAddress);

            var email = new HostingEmailAccount
            {
                HostingAccountId = dto.HostingAccountId,
                EmailAddress = dto.EmailAddress,
                Username = dto.Username,
                PasswordHash = HashPassword(dto.Password),
                QuotaMB = dto.QuotaMB,
                IsForwarderOnly = dto.IsForwarderOnly,
                ForwardTo = dto.ForwardTo,
                AutoResponderEnabled = dto.AutoResponderEnabled,
                AutoResponderMessage = dto.AutoResponderMessage,
                SpamFilterEnabled = dto.SpamFilterEnabled,
                SpamScoreThreshold = dto.SpamScoreThreshold,
                SyncStatus = syncToServer ? "Pending" : "NotSynced"
            };

            _context.HostingEmailAccounts.Add(email);
            await _context.SaveChangesAsync();

            _log.Information("Email account created with ID {EmailId}", email.Id);

            if (syncToServer)
            {
                // Create on server with the original plaintext password
                await SyncEmailAccountToServerAsync(email.Id, dto.Password);
            }

            return MapToDto(email);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error creating email account");
            throw;
        }
    }

    public async Task<HostingEmailAccountDto> UpdateEmailAccountAsync(int id, HostingEmailAccountUpdateDto dto, bool syncToServer = false)
    {
        try
        {
            _log.Information("Updating email account {EmailId}", id);

            var email = await _context.HostingEmailAccounts.FindAsync(id);
            if (email == null)
            {
                throw new InvalidOperationException($"Email account with ID {id} not found");
            }

            if (dto.QuotaMB.HasValue) email.QuotaMB = dto.QuotaMB;
            if (dto.ForwardTo != null) email.ForwardTo = dto.ForwardTo;
            if (dto.AutoResponderEnabled.HasValue) email.AutoResponderEnabled = dto.AutoResponderEnabled.Value;
            if (dto.AutoResponderMessage != null) email.AutoResponderMessage = dto.AutoResponderMessage;
            if (dto.SpamFilterEnabled.HasValue) email.SpamFilterEnabled = dto.SpamFilterEnabled.Value;
            if (dto.SpamScoreThreshold.HasValue) email.SpamScoreThreshold = dto.SpamScoreThreshold;

            if (syncToServer)
            {
                email.SyncStatus = "OutOfSync";
            }

            await _context.SaveChangesAsync();

            _log.Information("Email account {EmailId} updated successfully", id);

            if (syncToServer)
            {
                await SyncEmailAccountToServerAsync(id);
            }

            return MapToDto(email);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error updating email account {EmailId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteEmailAccountAsync(int id, bool deleteFromServer = false)
    {
        try
        {
            _log.Information("Deleting email account {EmailId}", id);

            var email = await _context.HostingEmailAccounts
                .Include(e => e.HostingAccount)
                    .ThenInclude(h => h.ServerControlPanel)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (email == null)
            {
                return false;
            }

            // TODO: Delete from server if requested
            if (deleteFromServer && !string.IsNullOrEmpty(email.ExternalEmailId))
            {
                try
                {
                    if (email.HostingAccount?.ServerControlPanel != null)
                    {
                        var panel = CreateHostingPanel(email.HostingAccount.ServerControlPanel);
                        var deleteResult = await panel.DeleteMailAccountAsync(email.ExternalEmailId);

                        if (deleteResult.Success)
                        {
                            _log.Information("Deleted email {Email} from server", email.EmailAddress);
                        }
                        else
                        {
                            _log.Warning("Failed to delete email {Email} from server: {Message}", 
                                email.EmailAddress, deleteResult.Message);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _log.Error(ex, "Error deleting email {Email} from server", email.EmailAddress);
                }
            }

            _context.HostingEmailAccounts.Remove(email);
            await _context.SaveChangesAsync();

            _log.Information("Email account {EmailId} deleted successfully", id);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error deleting email account {EmailId}", id);
            throw;
        }
    }

    public async Task<bool> ChangeEmailPasswordAsync(int id, string newPassword, bool syncToServer = false)
    {
        try
        {
            var email = await _context.HostingEmailAccounts
                .Include(e => e.HostingAccount)
                    .ThenInclude(h => h.ServerControlPanel!)
                        .ThenInclude(scp => scp.ControlPanelType)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (email == null)
            {
                return false;
            }

            email.PasswordHash = HashPassword(newPassword);
            
            if (syncToServer)
            {
                email.SyncStatus = "OutOfSync";

                // Sync password to server if requested
                if (email.HostingAccount?.ServerControlPanel != null && !string.IsNullOrEmpty(email.ExternalEmailId))
                {
                    try
                    {
                        var panel = CreateHostingPanel(email.HostingAccount.ServerControlPanel);
                        var passwordResult = await panel.ChangeMailPasswordAsync(email.ExternalEmailId, newPassword);

                        if (passwordResult.Success)
                        {
                            email.SyncStatus = "Synced";
                            email.LastSyncedAt = DateTime.UtcNow;
                            _log.Information("Password changed on server for email {Email}", email.EmailAddress);
                        }
                        else
                        {
                            email.SyncStatus = "Error";
                            _log.Warning("Failed to change password on server for email {Email}: {Message}", 
                                email.EmailAddress, passwordResult.Message);
                        }
                    }
                    catch (Exception ex)
                    {
                        _log.Error(ex, "Error changing password on server for email {Email}", email.EmailAddress);
                        email.SyncStatus = "Error";
                    }
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error changing password for email account {EmailId}", id);
            throw;
        }
    }

    public async Task<SyncResultDto> SyncEmailAccountsFromServerAsync(int hostingAccountId)
    {
        var result = new SyncResultDto { Success = false };

        try
        {
            _log.Information("Syncing email accounts from server for hosting account {AccountId}", hostingAccountId);

            var hostingAccount = await _context.HostingAccounts
                .Include(h => h.ServerControlPanel)
                    .ThenInclude(scp => scp!.ControlPanelType)
                .Include(h => h.Domains)
                .FirstOrDefaultAsync(h => h.Id == hostingAccountId);

            if (hostingAccount == null || hostingAccount.ServerControlPanel == null)
            {
                result.Message = "Hosting account or server control panel not found";
                return result;
            }

            if (!hostingAccount.Domains.Any())
            {
                result.Message = "No domains found for this hosting account";
                return result;
            }

            var panel = CreateHostingPanel(hostingAccount.ServerControlPanel);
            int syncedCount = 0;
            var errors = new List<string>();

            // Sync emails for each domain
            foreach (var domain in hostingAccount.Domains)
            {
                try
                {
                    var serverEmails = await panel.ListMailAccountsAsync(domain.DomainName);

                    foreach (var serverEmail in serverEmails)
                    {
                        if (string.IsNullOrEmpty(serverEmail.Email))
                            continue;

                        // Check if email already exists in database
                        var existingEmail = await _context.HostingEmailAccounts
                            .FirstOrDefaultAsync(e => e.EmailAddress == serverEmail.Email && 
                                                    e.HostingAccountId == hostingAccountId);

                        if (existingEmail == null)
                        {
                            // Create new email account
                            var newEmail = new HostingEmailAccount
                            {
                                HostingAccountId = hostingAccountId,
                                EmailAddress = serverEmail.Email!,
                                Username = serverEmail.Email!.Split('@')[0],
                                PasswordHash = string.Empty, // Cannot retrieve password from server
                                QuotaMB = serverEmail.DiskQuotaMB,
                                UsageMB = serverEmail.DiskUsageMB,
                                ExternalEmailId = serverEmail.Email,
                                LastSyncedAt = DateTime.UtcNow,
                                SyncStatus = "Synced"
                            };

                            _context.HostingEmailAccounts.Add(newEmail);
                            syncedCount++;
                        }
                        else
                        {
                            // Update existing email account
                            existingEmail.QuotaMB = serverEmail.DiskQuotaMB;
                            existingEmail.UsageMB = serverEmail.DiskUsageMB;
                            existingEmail.ExternalEmailId = serverEmail.Email;
                            existingEmail.LastSyncedAt = DateTime.UtcNow;
                            existingEmail.SyncStatus = "Synced";
                            syncedCount++;
                        }
                    }
                }
                catch (Exception ex)
                {
                    errors.Add($"Domain {domain.DomainName}: {ex.Message}");
                    _log.Warning(ex, "Error syncing emails for domain {Domain}", domain.DomainName);
                }
            }

            await _context.SaveChangesAsync();

            result.Success = errors.Count == 0 || syncedCount > 0;
            result.RecordsSynced = syncedCount;
            result.Message = errors.Any() 
                ? $"Synced {syncedCount} email accounts with errors: {string.Join("; ", errors)}"
                : $"Successfully synced {syncedCount} email accounts";

            _log.Information("Email sync completed for account {AccountId}. Synced: {Count}", 
                hostingAccountId, syncedCount);

            return result;
        }
        catch (Exception ex)
        {
            result.Message = $"Error syncing email accounts: {ex.Message}";
            _log.Error(ex, "Error syncing email accounts for hosting account {AccountId}", hostingAccountId);
            return result;
        }
    }

    public async Task<SyncResultDto> SyncEmailAccountToServerAsync(int emailAccountId, string? password = null)
    {
        var result = new SyncResultDto { Success = false };

        try
        {
            var email = await _context.HostingEmailAccounts
                .Include(e => e.HostingAccount)
                    .ThenInclude(h => h.ServerControlPanel)
                        .ThenInclude(scp => scp!.ControlPanelType)
                .Include(e => e.HostingAccount)
                    .ThenInclude(h => h.Domains)
                .FirstOrDefaultAsync(e => e.Id == emailAccountId);

            if (email == null)
            {
                result.Message = "Email account not found";
                return result;
            }

            if (email.HostingAccount?.ServerControlPanel == null)
            {
                result.Message = "Server control panel not configured for this account";
                return result;
            }

            // Extract domain from email address
            var emailParts = email.EmailAddress.Split('@');
            if (emailParts.Length != 2)
            {
                result.Message = "Invalid email address format";
                return result;
            }

            var domain = emailParts[1];
            var panel = CreateHostingPanel(email.HostingAccount.ServerControlPanel);

            // Check if email exists on server
            if (string.IsNullOrEmpty(email.ExternalEmailId))
            {
                // Create new email on server
                if (string.IsNullOrEmpty(password))
                {
                    result.Message = "Password is required to create email account on server";
                    result.Success = false;
                    return result;
                }

                var createRequest = new MailAccountRequest
                {
                    EmailAddress = email.EmailAddress,
                    Password = password,
                    Domain = domain,
                    QuotaMB = email.QuotaMB
                };

                var createResult = await panel.CreateMailAccountAsync(createRequest);

                if (!createResult.Success)
                {
                    result.Message = $"Failed to create email on server: {createResult.Message}";
                    email.SyncStatus = "Error";
                    await _context.SaveChangesAsync();
                    return result;
                }

                email.ExternalEmailId = createResult.EmailAddress;
                _log.Information("Created email {Email} on server", email.EmailAddress);
            }
            else
            {
                // Update existing email on server (quota only)
                var updateRequest = new MailAccountRequest
                {
                    QuotaMB = email.QuotaMB
                };

                var updateResult = await panel.UpdateMailAccountAsync(email.ExternalEmailId, updateRequest);

                if (!updateResult.Success)
                {
                    result.Message = $"Failed to update email on server: {updateResult.Message}";
                    email.SyncStatus = "Error";
                    await _context.SaveChangesAsync();
                    return result;
                }

                _log.Information("Updated email {Email} on server", email.EmailAddress);
            }

            email.LastSyncedAt = DateTime.UtcNow;
            email.SyncStatus = "Synced";
            await _context.SaveChangesAsync();

            result.Success = true;
            result.Message = $"Successfully synced email account to server";
            result.RecordsSynced = 1;

            return result;
        }
        catch (Exception ex)
        {
            result.Message = $"Error syncing email account: {ex.Message}";
            _log.Error(ex, "Error syncing email account {EmailId} to server", emailAccountId);
            return result;
        }
    }

    #region Private Helper Methods

    private string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    private IHostingPanel CreateHostingPanel(ServerControlPanel controlPanel)
    {
        var settings = new HostingPanelSettings
        {
            Provider = controlPanel.ControlPanelType.Name.ToLower()
        };

        // Configure provider-specific settings based on control panel type
        switch (controlPanel.ControlPanelType.Name.ToLower())
        {
            case "cpanel":
                settings.Cpanel = new CpanelSettings
                {
                    ApiUrl = controlPanel.ApiUrl,
                    ApiToken = controlPanel.ApiToken ?? string.Empty,
                    Username = controlPanel.Username ?? "root",
                    Port = controlPanel.Port,
                    UseHttps = controlPanel.UseHttps
                };
                break;

            case "plesk":
                settings.Plesk = new PleskSettings
                {
                    ApiUrl = controlPanel.ApiUrl,
                    ApiKey = controlPanel.ApiKey ?? string.Empty,
                    Username = controlPanel.Username ?? string.Empty,
                    Password = controlPanel.PasswordHash ?? string.Empty, // TODO: Decrypt
                    Port = controlPanel.Port,
                    UseHttps = controlPanel.UseHttps
                };
                break;

            default:
                throw new NotSupportedException($"Hosting panel type '{controlPanel.ControlPanelType.Name}' is not supported");
        }

        var factory = new HostingPanelFactory(settings);
        return factory.CreatePanel();
    }

    private HostingEmailAccountDto MapToDto(HostingEmailAccount email)
    {
        return new HostingEmailAccountDto
        {
            Id = email.Id,
            HostingAccountId = email.HostingAccountId,
            EmailAddress = email.EmailAddress,
            Username = email.Username,
            QuotaMB = email.QuotaMB,
            UsageMB = email.UsageMB,
            IsForwarderOnly = email.IsForwarderOnly,
            ForwardTo = email.ForwardTo,
            AutoResponderEnabled = email.AutoResponderEnabled,
            SpamFilterEnabled = email.SpamFilterEnabled,
            ExternalEmailId = email.ExternalEmailId,
            LastSyncedAt = email.LastSyncedAt,
            SyncStatus = email.SyncStatus,
            CreatedAt = email.CreatedAt
        };
    }

    #endregion
}

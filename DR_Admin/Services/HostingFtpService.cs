using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

/// <summary>
/// Service for managing hosting FTP accounts
/// </summary>
public class HostingFtpService : IHostingFtpService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<HostingFtpService> _logger;
    private static readonly Serilog.ILogger _log = Log.ForContext<HostingFtpService>();

    public HostingFtpService(
        ApplicationDbContext context,
        ILogger<HostingFtpService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<HostingFtpAccountDto?> GetFtpAccountAsync(int id)
    {
        try
        {
            var ftp = await _context.HostingFtpAccounts
                .Include(f => f.HostingAccount)
                .FirstOrDefaultAsync(f => f.Id == id);

            return ftp != null ? MapToDto(ftp) : null;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error getting FTP account {FtpId}", id);
            throw;
        }
    }

    public async Task<List<HostingFtpAccountDto>> GetFtpAccountsByHostingAccountAsync(int hostingAccountId)
    {
        try
        {
            var ftpAccounts = await _context.HostingFtpAccounts
                .Include(f => f.HostingAccount)
                .Where(f => f.HostingAccountId == hostingAccountId)
                .OrderBy(f => f.Username)
                .ToListAsync();

            return ftpAccounts.Select(MapToDto).ToList();
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error getting FTP accounts for hosting account {AccountId}", hostingAccountId);
            throw;
        }
    }

    public async Task<HostingFtpAccountDto> CreateFtpAccountAsync(HostingFtpAccountCreateDto dto, bool syncToServer = false)
    {
        try
        {
            _log.Information("Creating FTP account {Username}", dto.Username);

            var ftp = new HostingFtpAccount
            {
                HostingAccountId = dto.HostingAccountId,
                Username = dto.Username,
                PasswordHash = HashPassword(dto.Password),
                HomeDirectory = dto.HomeDirectory,
                QuotaMB = dto.QuotaMB,
                ReadOnly = dto.ReadOnly,
                SftpEnabled = dto.SftpEnabled,
                FtpsEnabled = dto.FtpsEnabled,
                SyncStatus = syncToServer ? "Pending" : "NotSynced"
            };

            _context.HostingFtpAccounts.Add(ftp);
            await _context.SaveChangesAsync();

            _log.Information("FTP account created with ID {FtpId}", ftp.Id);

            if (syncToServer)
            {
                await SyncFtpAccountToServerAsync(ftp.Id);
            }

            return MapToDto(ftp);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error creating FTP account");
            throw;
        }
    }

    public async Task<HostingFtpAccountDto> UpdateFtpAccountAsync(int id, HostingFtpAccountUpdateDto dto, bool syncToServer = false)
    {
        try
        {
            _log.Information("Updating FTP account {FtpId}", id);

            var ftp = await _context.HostingFtpAccounts.FindAsync(id);
            if (ftp == null)
            {
                throw new InvalidOperationException($"FTP account with ID {id} not found");
            }

            if (dto.HomeDirectory != null) ftp.HomeDirectory = dto.HomeDirectory;
            if (dto.QuotaMB.HasValue) ftp.QuotaMB = dto.QuotaMB;
            if (dto.ReadOnly.HasValue) ftp.ReadOnly = dto.ReadOnly.Value;
            if (dto.SftpEnabled.HasValue) ftp.SftpEnabled = dto.SftpEnabled.Value;
            if (dto.FtpsEnabled.HasValue) ftp.FtpsEnabled = dto.FtpsEnabled.Value;

            if (syncToServer)
            {
                ftp.SyncStatus = "OutOfSync";
            }

            await _context.SaveChangesAsync();

            _log.Information("FTP account {FtpId} updated successfully", id);

            if (syncToServer)
            {
                await SyncFtpAccountToServerAsync(id);
            }

            return MapToDto(ftp);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error updating FTP account {FtpId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteFtpAccountAsync(int id, bool deleteFromServer = false)
    {
        try
        {
            _log.Information("Deleting FTP account {FtpId}", id);

            var ftp = await _context.HostingFtpAccounts.FindAsync(id);
            if (ftp == null)
            {
                return false;
            }

            // TODO: Delete from server if requested
            if (deleteFromServer && !string.IsNullOrEmpty(ftp.ExternalFtpId))
            {
                _log.Information("Server deletion not yet implemented for FTP account {FtpId}", id);
            }

            _context.HostingFtpAccounts.Remove(ftp);
            await _context.SaveChangesAsync();

            _log.Information("FTP account {FtpId} deleted successfully", id);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error deleting FTP account {FtpId}", id);
            throw;
        }
    }

    public async Task<bool> ChangeFtpPasswordAsync(int id, string newPassword, bool syncToServer = false)
    {
        try
        {
            var ftp = await _context.HostingFtpAccounts.FindAsync(id);
            if (ftp == null)
            {
                return false;
            }

            ftp.PasswordHash = HashPassword(newPassword);
            
            if (syncToServer)
            {
                ftp.SyncStatus = "OutOfSync";
            }

            await _context.SaveChangesAsync();

            // TODO: Sync password to server if requested

            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error changing password for FTP account {FtpId}", id);
            throw;
        }
    }

    public async Task<SyncResultDto> SyncFtpAccountsFromServerAsync(int hostingAccountId)
    {
        var result = new SyncResultDto { Success = false };

        try
        {
            _log.Information("Syncing FTP accounts from server for hosting account {AccountId}", hostingAccountId);

            var hostingAccount = await _context.HostingAccounts
                .Include(h => h.ServerControlPanel)
                .FirstOrDefaultAsync(h => h.Id == hostingAccountId);

            if (hostingAccount == null || hostingAccount.ServerControlPanel == null)
            {
                result.Message = "Hosting account or server control panel not found";
                return result;
            }

            // TODO: Implement actual sync from server
            // This would require panel API methods for listing FTP accounts

            result.Success = true;
            result.Message = "FTP sync from server not yet fully implemented";
            result.RecordsSynced = 0;

            return result;
        }
        catch (Exception ex)
        {
            result.Message = $"Error syncing FTP accounts: {ex.Message}";
            _log.Error(ex, "Error syncing FTP accounts for hosting account {AccountId}", hostingAccountId);
            return result;
        }
    }

    public async Task<SyncResultDto> SyncFtpAccountToServerAsync(int ftpAccountId)
    {
        var result = new SyncResultDto { Success = false };

        try
        {
            var ftp = await _context.HostingFtpAccounts
                .Include(f => f.HostingAccount)
                    .ThenInclude(h => h.ServerControlPanel)
                .FirstOrDefaultAsync(f => f.Id == ftpAccountId);

            if (ftp == null)
            {
                result.Message = "FTP account not found";
                return result;
            }

            // TODO: Implement actual sync to server

            ftp.LastSyncedAt = DateTime.UtcNow;
            ftp.SyncStatus = "Synced";
            await _context.SaveChangesAsync();

            result.Success = true;
            result.Message = "FTP sync to server not yet fully implemented";
            result.RecordsSynced = 1;

            return result;
        }
        catch (Exception ex)
        {
            result.Message = $"Error syncing FTP account: {ex.Message}";
            _log.Error(ex, "Error syncing FTP account {FtpId} to server", ftpAccountId);
            return result;
        }
    }

    private string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    private HostingFtpAccountDto MapToDto(HostingFtpAccount ftp)
    {
        return new HostingFtpAccountDto
        {
            Id = ftp.Id,
            HostingAccountId = ftp.HostingAccountId,
            Username = ftp.Username,
            HomeDirectory = ftp.HomeDirectory,
            QuotaMB = ftp.QuotaMB,
            ReadOnly = ftp.ReadOnly,
            SftpEnabled = ftp.SftpEnabled,
            FtpsEnabled = ftp.FtpsEnabled,
            ExternalFtpId = ftp.ExternalFtpId,
            LastSyncedAt = ftp.LastSyncedAt,
            SyncStatus = ftp.SyncStatus,
            CreatedAt = ftp.CreatedAt
        };
    }
}

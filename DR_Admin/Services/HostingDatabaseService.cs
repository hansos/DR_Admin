using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Text.Json;
using HostingPanelLib.Models;
using HostingPanelLib.Factories;
using HostingPanelLib.Infrastructure.Settings;
using HostingPanelLib.Interfaces;

namespace ISPAdmin.Services;

/// <summary>
/// Service for managing hosting databases and database users
/// </summary>
public class HostingDatabaseService : IHostingDatabaseService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<HostingDatabaseService> _logger;
    private static readonly Serilog.ILogger _log = Log.ForContext<HostingDatabaseService>();

    public HostingDatabaseService(
        ApplicationDbContext context,
        ILogger<HostingDatabaseService> logger)
    {
        _context = context;
        _logger = logger;
    }

    #region Database Management

    public async Task<HostingDatabaseDto?> GetDatabaseAsync(int id)
    {
        try
        {
            var database = await _context.HostingDatabases
                .Include(d => d.HostingAccount)
                .Include(d => d.DatabaseUsers)
                .FirstOrDefaultAsync(d => d.Id == id);

            return database != null ? MapToDto(database) : null;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error getting database {DatabaseId}", id);
            throw;
        }
    }

    public async Task<List<HostingDatabaseDto>> GetDatabasesByHostingAccountAsync(int hostingAccountId)
    {
        try
        {
            var databases = await _context.HostingDatabases
                .Include(d => d.HostingAccount)
                .Include(d => d.DatabaseUsers)
                .Where(d => d.HostingAccountId == hostingAccountId)
                .OrderBy(d => d.DatabaseName)
                .ToListAsync();

            return databases.Select(MapToDto).ToList();
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error getting databases for hosting account {AccountId}", hostingAccountId);
            throw;
        }
    }

    public async Task<HostingDatabaseDto> CreateDatabaseAsync(HostingDatabaseCreateDto dto, bool syncToServer = false)
    {
        try
        {
            _log.Information("Creating database {DatabaseName}", dto.DatabaseName);

            var database = new HostingDatabase
            {
                HostingAccountId = dto.HostingAccountId,
                DatabaseName = dto.DatabaseName,
                DatabaseType = dto.DatabaseType,
                CharacterSet = dto.CharacterSet,
                Collation = dto.Collation,
                SyncStatus = syncToServer ? "Pending" : "NotSynced"
            };

            _context.HostingDatabases.Add(database);
            await _context.SaveChangesAsync();

            _log.Information("Database created with ID {DatabaseId}", database.Id);

            if (syncToServer)
            {
                await SyncDatabaseToServerAsync(database.Id);
            }

            return MapToDto(database);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error creating database");
            throw;
        }
    }

    public async Task<HostingDatabaseDto> UpdateDatabaseAsync(int id, HostingDatabaseUpdateDto dto)
    {
        try
        {
            _log.Information("Updating database {DatabaseId}", id);

            var database = await _context.HostingDatabases
                .Include(d => d.DatabaseUsers)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (database == null)
            {
                throw new InvalidOperationException($"Database with ID {id} not found");
            }

            if (dto.Notes != null)
            {
                database.Notes = dto.Notes;
            }

            await _context.SaveChangesAsync();

            _log.Information("Database {DatabaseId} updated successfully", id);

            return MapToDto(database);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error updating database {DatabaseId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteDatabaseAsync(int id, bool deleteFromServer = false)
    {
        try
        {
            _log.Information("Deleting database {DatabaseId}", id);

            var database = await _context.HostingDatabases
                .Include(d => d.DatabaseUsers)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (database == null)
            {
                return false;
            }

            // TODO: Delete from server if requested
            if (deleteFromServer && !string.IsNullOrEmpty(database.ExternalDatabaseId))
            {
                _log.Information("Server deletion not yet implemented for database {DatabaseId}", id);
            }

            _context.HostingDatabases.Remove(database);
            await _context.SaveChangesAsync();

            _log.Information("Database {DatabaseId} deleted successfully", id);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error deleting database {DatabaseId}", id);
            throw;
        }
    }

    #endregion

    #region Database User Management

    public async Task<HostingDatabaseUserDto?> GetDatabaseUserAsync(int id)
    {
        try
        {
            var user = await _context.HostingDatabaseUsers
                .Include(u => u.HostingDatabase)
                .FirstOrDefaultAsync(u => u.Id == id);

            return user != null ? MapToDto(user) : null;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error getting database user {UserId}", id);
            throw;
        }
    }

    public async Task<List<HostingDatabaseUserDto>> GetDatabaseUsersByDatabaseAsync(int databaseId)
    {
        try
        {
            var users = await _context.HostingDatabaseUsers
                .Include(u => u.HostingDatabase)
                .Where(u => u.HostingDatabaseId == databaseId)
                .OrderBy(u => u.Username)
                .ToListAsync();

            return users.Select(MapToDto).ToList();
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error getting database users for database {DatabaseId}", databaseId);
            throw;
        }
    }

    public async Task<HostingDatabaseUserDto> CreateDatabaseUserAsync(HostingDatabaseUserCreateDto dto, bool syncToServer = false)
    {
        try
        {
            _log.Information("Creating database user {Username} for database {DatabaseId}", dto.Username, dto.HostingDatabaseId);

            var user = new HostingDatabaseUser
            {
                HostingDatabaseId = dto.HostingDatabaseId,
                Username = dto.Username,
                PasswordHash = HashPassword(dto.Password),
                Privileges = dto.Privileges != null ? JsonSerializer.Serialize(dto.Privileges) : null,
                AllowedHosts = dto.AllowedHosts ?? "localhost",
                SyncStatus = syncToServer ? "Pending" : "NotSynced"
            };

            _context.HostingDatabaseUsers.Add(user);
            await _context.SaveChangesAsync();

            _log.Information("Database user created with ID {UserId}", user.Id);

            if (syncToServer)
            {
                // Create on server with the original plaintext password
                await SyncDatabaseUserToServerAsync(user.Id, dto.Password, dto.Privileges);
            }

            return MapToDto(user);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error creating database user");
            throw;
        }
    }

    public async Task<bool> DeleteDatabaseUserAsync(int id, bool deleteFromServer = false)
    {
        try
        {
            _log.Information("Deleting database user {UserId}", id);

            var user = await _context.HostingDatabaseUsers.FindAsync(id);
            if (user == null)
            {
                return false;
            }

            // TODO: Delete from server if requested

            _context.HostingDatabaseUsers.Remove(user);
            await _context.SaveChangesAsync();

            _log.Information("Database user {UserId} deleted successfully", id);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error deleting database user {UserId}", id);
            throw;
        }
    }

    public async Task<bool> ChangeDatabaseUserPasswordAsync(int id, string newPassword, bool syncToServer = false)
    {
        try
        {
            var user = await _context.HostingDatabaseUsers.FindAsync(id);
            if (user == null)
            {
                return false;
            }

            user.PasswordHash = HashPassword(newPassword);
            
            if (syncToServer)
            {
                user.SyncStatus = "OutOfSync";
            }

            await _context.SaveChangesAsync();

            // TODO: Sync password to server if requested

            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error changing password for database user {UserId}", id);
            throw;
        }
    }

    public async Task<bool> GrantPrivilegesAsync(int userId, int databaseId, List<string> privileges, bool syncToServer = false)
    {
        try
        {
            var user = await _context.HostingDatabaseUsers.FindAsync(userId);
            if (user == null || user.HostingDatabaseId != databaseId)
            {
                return false;
            }

            user.Privileges = JsonSerializer.Serialize(privileges);
            
            if (syncToServer)
            {
                user.SyncStatus = "OutOfSync";
            }

            await _context.SaveChangesAsync();

            // TODO: Sync privileges to server if requested

            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error granting privileges to user {UserId}", userId);
            throw;
        }
    }

    #endregion

    #region Synchronization

    public async Task<SyncResultDto> SyncDatabasesFromServerAsync(int hostingAccountId)
    {
        var result = new SyncResultDto { Success = false };

        try
        {
            _log.Information("Syncing databases from server for hosting account {AccountId}", hostingAccountId);

            var hostingAccount = await _context.HostingAccounts
                .Include(h => h.ServerControlPanel)
                    .ThenInclude(scp => scp!.ControlPanelType)
                .FirstOrDefaultAsync(h => h.Id == hostingAccountId);

            if (hostingAccount == null || hostingAccount.ServerControlPanel == null)
            {
                result.Message = "Hosting account or server control panel not found";
                return result;
            }

            // Note: CPanel's ListDatabasesAsync doesn't provide much detail
            // For now, we'll just mark this as a placeholder
            // In a real implementation, you'd need to call the API and parse results

            result.Success = true;
            result.Message = "Database listing from CPanel API is limited - manual sync recommended";
            result.RecordsSynced = 0;

            return result;
        }
        catch (Exception ex)
        {
            result.Message = $"Error syncing databases: {ex.Message}";
            _log.Error(ex, "Error syncing databases for hosting account {AccountId}", hostingAccountId);
            return result;
        }
    }

    public async Task<SyncResultDto> SyncDatabaseToServerAsync(int databaseId, string? password = null)
    {
        var result = new SyncResultDto { Success = false };

        try
        {
            var database = await _context.HostingDatabases
                .Include(d => d.HostingAccount)
                    .ThenInclude(h => h.ServerControlPanel!)
                        .ThenInclude(scp => scp.ControlPanelType)
                .FirstOrDefaultAsync(d => d.Id == databaseId);

            if (database == null)
            {
                result.Message = "Database not found";
                return result;
            }

            if (database.HostingAccount?.ServerControlPanel == null)
            {
                result.Message = "Server control panel not configured";
                return result;
            }

            var panel = CreateHostingPanel(database.HostingAccount.ServerControlPanel);

            // Check if database exists on server
            if (string.IsNullOrEmpty(database.ExternalDatabaseId))
            {
                // Create new database on server
                var createRequest = new DatabaseRequest
                {
                    DatabaseName = database.DatabaseName
                };

                var createResult = await panel.CreateDatabaseAsync(createRequest);

                if (!createResult.Success)
                {
                    result.Message = $"Failed to create database on server: {createResult.Message}";
                    database.SyncStatus = "Error";
                    await _context.SaveChangesAsync();
                    return result;
                }

                database.ExternalDatabaseId = createResult.DatabaseName;
                _log.Information("Created database {DatabaseName} on server", database.DatabaseName);
            }

            database.LastSyncedAt = DateTime.UtcNow;
            database.SyncStatus = "Synced";
            await _context.SaveChangesAsync();

            result.Success = true;
            result.Message = "Successfully synced database to server";
            result.RecordsSynced = 1;

            return result;
        }
        catch (Exception ex)
        {
            result.Message = $"Error syncing database: {ex.Message}";
            _log.Error(ex, "Error syncing database {DatabaseId} to server", databaseId);
            return result;
        }
    }

    #endregion

    #region Private Helper Methods

    private string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    private async Task<SyncResultDto> SyncDatabaseUserToServerAsync(int userId, string password, List<string>? privileges)
    {
        var result = new SyncResultDto { Success = false };

        try
        {
            var user = await _context.HostingDatabaseUsers
                .Include(u => u.HostingDatabase)
                    .ThenInclude(d => d.HostingAccount)
                        .ThenInclude(h => h.ServerControlPanel!)
                            .ThenInclude(scp => scp.ControlPanelType)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null || user.HostingDatabase?.HostingAccount?.ServerControlPanel == null)
            {
                result.Message = "User or server control panel not found";
                return result;
            }

            var panel = CreateHostingPanel(user.HostingDatabase.HostingAccount.ServerControlPanel);

            // Create database user on server
            var createUserRequest = new DatabaseUserRequest
            {
                Username = user.Username,
                Password = password
            };

            var createUserResult = await panel.CreateDatabaseUserAsync(createUserRequest);

            if (!createUserResult.Success)
            {
                result.Message = $"Failed to create user on server: {createUserResult.Message}";
                user.SyncStatus = "Error";
                await _context.SaveChangesAsync();
                return result;
            }

            user.ExternalUserId = createUserResult.Username;

            // Grant privileges if database is synced
            if (!string.IsNullOrEmpty(user.HostingDatabase.ExternalDatabaseId) && privileges != null && privileges.Any())
            {
                var grantResult = await panel.GrantDatabasePrivilegesAsync(
                    user.Username, 
                    user.HostingDatabase.ExternalDatabaseId, 
                    privileges);

                if (!grantResult.Success)
                {
                    _log.Warning("Failed to grant privileges: {Message}", grantResult.Message);
                }
            }

            user.SyncStatus = "Synced";
            user.LastSyncedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            result.Success = true;
            result.Message = "Database user created on server";
            result.RecordsSynced = 1;

            return result;
        }
        catch (Exception ex)
        {
            result.Message = $"Error syncing user: {ex.Message}";
            _log.Error(ex, "Error syncing database user {UserId} to server", userId);
            return result;
        }
    }

    private IHostingPanel CreateHostingPanel(ServerControlPanel controlPanel)
    {
        var settings = new HostingPanelSettings
        {
            Provider = controlPanel.ControlPanelType.Name.ToLower()
        };

        // Configure provider-specific settings
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
                    Password = controlPanel.PasswordHash ?? string.Empty,
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

    private HostingDatabaseDto MapToDto(HostingDatabase database)
    {
        return new HostingDatabaseDto
        {
            Id = database.Id,
            HostingAccountId = database.HostingAccountId,
            DatabaseName = database.DatabaseName,
            DatabaseType = database.DatabaseType,
            SizeMB = database.SizeMB,
            ServerHost = database.ServerHost,
            ServerPort = database.ServerPort,
            ExternalDatabaseId = database.ExternalDatabaseId,
            LastSyncedAt = database.LastSyncedAt,
            SyncStatus = database.SyncStatus,
            DatabaseUsers = database.DatabaseUsers?.Select(MapToDto).ToList(),
            CreatedAt = database.CreatedAt
        };
    }

    private HostingDatabaseUserDto MapToDto(HostingDatabaseUser user)
    {
        return new HostingDatabaseUserDto
        {
            Id = user.Id,
            HostingDatabaseId = user.HostingDatabaseId,
            Username = user.Username,
            Privileges = user.Privileges,
            AllowedHosts = user.AllowedHosts,
            ExternalUserId = user.ExternalUserId,
            LastSyncedAt = user.LastSyncedAt,
            SyncStatus = user.SyncStatus,
            CreatedAt = user.CreatedAt
        };
    }

    #endregion
}

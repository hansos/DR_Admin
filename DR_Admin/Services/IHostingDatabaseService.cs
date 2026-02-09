using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

/// <summary>
/// Service interface for managing hosting databases and database users
/// </summary>
public interface IHostingDatabaseService
{
    // Database Management
    Task<HostingDatabaseDto?> GetDatabaseAsync(int id);
    Task<List<HostingDatabaseDto>> GetDatabasesByHostingAccountAsync(int hostingAccountId);
    Task<HostingDatabaseDto> CreateDatabaseAsync(HostingDatabaseCreateDto dto, bool syncToServer = false);
    Task<HostingDatabaseDto> UpdateDatabaseAsync(int id, HostingDatabaseUpdateDto dto);
    Task<bool> DeleteDatabaseAsync(int id, bool deleteFromServer = false);
    
    // Database User Management
    Task<HostingDatabaseUserDto?> GetDatabaseUserAsync(int id);
    Task<List<HostingDatabaseUserDto>> GetDatabaseUsersByDatabaseAsync(int databaseId);
    Task<HostingDatabaseUserDto> CreateDatabaseUserAsync(HostingDatabaseUserCreateDto dto, bool syncToServer = false);
    Task<bool> DeleteDatabaseUserAsync(int id, bool deleteFromServer = false);
    Task<bool> ChangeDatabaseUserPasswordAsync(int id, string newPassword, bool syncToServer = false);
    Task<bool> GrantPrivilegesAsync(int userId, int databaseId, List<string> privileges, bool syncToServer = false);
    
    // Synchronization
    Task<SyncResultDto> SyncDatabasesFromServerAsync(int hostingAccountId);
    Task<SyncResultDto> SyncDatabaseToServerAsync(int databaseId, string? password = null);
}

namespace ISPAdmin.Data.Entities;

public class HostingDatabaseUser : EntityBase
{
    public int HostingDatabaseId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? PasswordHash { get; set; } // Encrypted password
    public string? Privileges { get; set; } // JSON array of permissions (SELECT, INSERT, UPDATE, DELETE, etc.)
    public string? AllowedHosts { get; set; } // Comma-separated list of allowed hosts (e.g., "localhost,%.example.com")
    
    // Sync tracking
    public string? ExternalUserId { get; set; }
    public DateTime? LastSyncedAt { get; set; }
    public string? SyncStatus { get; set; } // Synced, OutOfSync, Error, NotSynced
    
    public string? Notes { get; set; }

    public HostingDatabase HostingDatabase { get; set; } = null!;
}

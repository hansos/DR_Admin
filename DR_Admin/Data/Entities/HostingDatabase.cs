namespace ISPAdmin.Data.Entities;

public class HostingDatabase : EntityBase
{
    public int HostingAccountId { get; set; }
    public string DatabaseName { get; set; } = string.Empty;
    public string DatabaseType { get; set; } = "MySQL"; // MySQL, PostgreSQL, MariaDB, etc.
    public int? SizeMB { get; set; }
    public string? ServerHost { get; set; } // Database server hostname
    public int? ServerPort { get; set; }
    public string? CharacterSet { get; set; }
    public string? Collation { get; set; }
    
    // Sync tracking
    public string? ExternalDatabaseId { get; set; }
    public DateTime? LastSyncedAt { get; set; }
    public string? SyncStatus { get; set; } // Synced, OutOfSync, Error, NotSynced
    
    public string? Notes { get; set; }

    public HostingAccount HostingAccount { get; set; } = null!;
    public ICollection<HostingDatabaseUser> DatabaseUsers { get; set; } = new List<HostingDatabaseUser>();
}

namespace ISPAdmin.Data.Entities;

public class HostingFtpAccount : EntityBase
{
    public int HostingAccountId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? PasswordHash { get; set; } // Encrypted password
    public string HomeDirectory { get; set; } = string.Empty;
    public int? QuotaMB { get; set; }
    public bool ReadOnly { get; set; }
    public bool SftpEnabled { get; set; }
    public bool FtpsEnabled { get; set; }
    
    // Sync tracking
    public string? ExternalFtpId { get; set; }
    public DateTime? LastSyncedAt { get; set; }
    public string? SyncStatus { get; set; } // Synced, OutOfSync, Error, NotSynced
    
    public string? Notes { get; set; }

    public HostingAccount HostingAccount { get; set; } = null!;
}

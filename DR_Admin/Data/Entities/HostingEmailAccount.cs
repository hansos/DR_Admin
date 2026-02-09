namespace ISPAdmin.Data.Entities;

public class HostingEmailAccount : EntityBase
{
    public int HostingAccountId { get; set; }
    public string EmailAddress { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string? PasswordHash { get; set; } // Encrypted password
    public int? QuotaMB { get; set; }
    public int? UsageMB { get; set; }
    public bool IsForwarderOnly { get; set; }
    public string? ForwardTo { get; set; } // Comma-separated list of forward addresses
    public bool AutoResponderEnabled { get; set; }
    public string? AutoResponderMessage { get; set; }
    public bool SpamFilterEnabled { get; set; }
    public int? SpamScoreThreshold { get; set; }
    
    // Sync tracking
    public string? ExternalEmailId { get; set; }
    public DateTime? LastSyncedAt { get; set; }
    public string? SyncStatus { get; set; } // Synced, OutOfSync, Error, NotSynced
    
    public string? Notes { get; set; }

    public HostingAccount HostingAccount { get; set; } = null!;
}

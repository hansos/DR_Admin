namespace ISPAdmin.Data.Entities;

public class HostingAccount : EntityBase
{
    public int CustomerId { get; set; }
    public int ServiceId { get; set; }
    public int? ServerId { get; set; }
    public int? ServerControlPanelId { get; set; }
    public string Provider { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime ExpirationDate { get; set; }

    // Sync & Configuration fields
    public string? ExternalAccountId { get; set; }
    public DateTime? LastSyncedAt { get; set; }
    public string? SyncStatus { get; set; } // Synced, OutOfSync, Error, NotSynced
    public string? ConfigurationJson { get; set; } // Provider-specific configuration
    
    // Resource Usage & Limits
    public int? DiskUsageMB { get; set; }
    public int? BandwidthUsageMB { get; set; }
    public int? DiskQuotaMB { get; set; }
    public int? BandwidthLimitMB { get; set; }
    public int? MaxEmailAccounts { get; set; }
    public int? MaxDatabases { get; set; }
    public int? MaxFtpAccounts { get; set; }
    public int? MaxSubdomains { get; set; }
    public string? PlanName { get; set; }

    public Customer Customer { get; set; } = null!;
    public Service Service { get; set; } = null!;
    public Server? Server { get; set; }
    public ServerControlPanel? ServerControlPanel { get; set; }
    
    // Navigation properties for hosting resources
    public ICollection<HostingDomain> Domains { get; set; } = new List<HostingDomain>();
    public ICollection<HostingEmailAccount> EmailAccounts { get; set; } = new List<HostingEmailAccount>();
    public ICollection<HostingDatabase> Databases { get; set; } = new List<HostingDatabase>();
    public ICollection<HostingFtpAccount> FtpAccounts { get; set; } = new List<HostingFtpAccount>();
}

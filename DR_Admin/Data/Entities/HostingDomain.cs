namespace ISPAdmin.Data.Entities;

public class HostingDomain : EntityBase
{
    public int HostingAccountId { get; set; }
    public string DomainName { get; set; } = string.Empty;
    public string DomainType { get; set; } = string.Empty; // Main, Addon, Parked, Subdomain
    public string? DocumentRoot { get; set; }
    public bool SslEnabled { get; set; }
    public DateTime? SslExpirationDate { get; set; }
    public string? SslIssuer { get; set; }
    public bool AutoRenewSsl { get; set; }
    
    // Sync tracking
    public string? ExternalDomainId { get; set; }
    public DateTime? LastSyncedAt { get; set; }
    public string? SyncStatus { get; set; } // Synced, OutOfSync, Error, NotSynced
    
    // Additional configuration
    public bool PhpEnabled { get; set; } = true;
    public string? PhpVersion { get; set; }
    public string? Notes { get; set; }

    public HostingAccount HostingAccount { get; set; } = null!;
}

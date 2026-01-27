namespace ISPAdmin.Data.Entities;

public class HostingPackage : EntityBase
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    /// <summary>
    /// Normalized version of Name for case-insensitive searches
    /// </summary>
    public string NormalizedName { get; set; } = string.Empty;
    
    // Resource Limits
    public int DiskSpaceMB { get; set; }
    public int BandwidthMB { get; set; }
    public int EmailAccounts { get; set; }
    public int Databases { get; set; }
    public int Domains { get; set; }
    public int Subdomains { get; set; }
    public int FtpAccounts { get; set; }
    
    // Features
    public bool SslSupport { get; set; }
    public bool BackupSupport { get; set; }
    public bool DedicatedIp { get; set; }
    
    // Pricing
    public decimal MonthlyPrice { get; set; }
    public decimal YearlyPrice { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public ICollection<Service> Services { get; set; } = new List<Service>();
}

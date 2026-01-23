namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object representing a hosting package
/// </summary>
public class HostingPackageDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the hosting package
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Gets or sets the package name
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the package description
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Gets or sets the disk space in megabytes
    /// </summary>
    public int DiskSpaceMB { get; set; }
    
    /// <summary>
    /// Gets or sets the bandwidth in megabytes
    /// </summary>
    public int BandwidthMB { get; set; }
    
    /// <summary>
    /// Gets or sets the number of email accounts allowed
    /// </summary>
    public int EmailAccounts { get; set; }
    
    /// <summary>
    /// Gets or sets the number of databases allowed
    /// </summary>
    public int Databases { get; set; }
    
    /// <summary>
    /// Gets or sets the number of domains allowed
    /// </summary>
    public int Domains { get; set; }
    
    /// <summary>
    /// Gets or sets the number of subdomains allowed
    /// </summary>
    public int Subdomains { get; set; }
    
    /// <summary>
    /// Gets or sets the number of FTP accounts allowed
    /// </summary>
    public int FtpAccounts { get; set; }
    
    /// <summary>
    /// Gets or sets whether SSL support is included
    /// </summary>
    public bool SslSupport { get; set; }
    
    /// <summary>
    /// Gets or sets whether backup support is included
    /// </summary>
    public bool BackupSupport { get; set; }
    
    /// <summary>
    /// Gets or sets whether a dedicated IP is included
    /// </summary>
    public bool DedicatedIp { get; set; }
    
    /// <summary>
    /// Gets or sets the monthly price
    /// </summary>
    public decimal MonthlyPrice { get; set; }
    
    /// <summary>
    /// Gets or sets the yearly price
    /// </summary>
    public decimal YearlyPrice { get; set; }
    
    /// <summary>
    /// Gets or sets whether this package is active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Gets or sets the date and time when the package was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Gets or sets the date and time when the package was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Data transfer object for creating a new hosting package
/// </summary>
public class CreateHostingPackageDto
{
    /// <summary>
    /// Gets or sets the package name
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the package description
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Gets or sets the disk space in megabytes
    /// </summary>
    public int DiskSpaceMB { get; set; }
    
    /// <summary>
    /// Gets or sets the bandwidth in megabytes
    /// </summary>
    public int BandwidthMB { get; set; }
    
    /// <summary>
    /// Gets or sets the number of email accounts allowed
    /// </summary>
    public int EmailAccounts { get; set; }
    
    /// <summary>
    /// Gets or sets the number of databases allowed
    /// </summary>
    public int Databases { get; set; }
    
    /// <summary>
    /// Gets or sets the number of domains allowed
    /// </summary>
    public int Domains { get; set; }
    
    /// <summary>
    /// Gets or sets the number of subdomains allowed
    /// </summary>
    public int Subdomains { get; set; }
    
    /// <summary>
    /// Gets or sets the number of FTP accounts allowed
    /// </summary>
    public int FtpAccounts { get; set; }
    
    /// <summary>
    /// Gets or sets whether SSL support is included
    /// </summary>
    public bool SslSupport { get; set; }
    
    /// <summary>
    /// Gets or sets whether backup support is included
    /// </summary>
    public bool BackupSupport { get; set; }
    
    /// <summary>
    /// Gets or sets whether a dedicated IP is included
    /// </summary>
    public bool DedicatedIp { get; set; }
    
    /// <summary>
    /// Gets or sets the monthly price
    /// </summary>
    public decimal MonthlyPrice { get; set; }
    
    /// <summary>
    /// Gets or sets the yearly price
    /// </summary>
    public decimal YearlyPrice { get; set; }
    
    /// <summary>
    /// Gets or sets whether this package is active
    /// </summary>
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// Data transfer object for updating an existing hosting package
/// </summary>
public class UpdateHostingPackageDto
{
    /// <summary>
    /// Gets or sets the package name
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the package description
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Gets or sets the disk space in megabytes
    /// </summary>
    public int DiskSpaceMB { get; set; }
    
    /// <summary>
    /// Gets or sets the bandwidth in megabytes
    /// </summary>
    public int BandwidthMB { get; set; }
    
    /// <summary>
    /// Gets or sets the number of email accounts allowed
    /// </summary>
    public int EmailAccounts { get; set; }
    
    /// <summary>
    /// Gets or sets the number of databases allowed
    /// </summary>
    public int Databases { get; set; }
    
    /// <summary>
    /// Gets or sets the number of domains allowed
    /// </summary>
    public int Domains { get; set; }
    
    /// <summary>
    /// Gets or sets the number of subdomains allowed
    /// </summary>
    public int Subdomains { get; set; }
    
    /// <summary>
    /// Gets or sets the number of FTP accounts allowed
    /// </summary>
    public int FtpAccounts { get; set; }
    
    /// <summary>
    /// Gets or sets whether SSL support is included
    /// </summary>
    public bool SslSupport { get; set; }
    
    /// <summary>
    /// Gets or sets whether backup support is included
    /// </summary>
    public bool BackupSupport { get; set; }
    
    /// <summary>
    /// Gets or sets whether a dedicated IP is included
    /// </summary>
    public bool DedicatedIp { get; set; }
    
    /// <summary>
    /// Gets or sets the monthly price
    /// </summary>
    public decimal MonthlyPrice { get; set; }
    
    /// <summary>
    /// Gets or sets the yearly price
    /// </summary>
    public decimal YearlyPrice { get; set; }
    
    /// <summary>
    /// Gets or sets whether this package is active
    /// </summary>
    public bool IsActive { get; set; } = true;
}

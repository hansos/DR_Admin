namespace ISPAdmin.DTOs;

public class HostingAccountDto
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public int ServiceId { get; set; }
    public int? ServerId { get; set; }
    public string? ServerName { get; set; }
    public int? ServerControlPanelId { get; set; }
    public string? ControlPanelType { get; set; }
    public string Provider { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime ExpirationDate { get; set; }
    
    // Sync info
    public string? ExternalAccountId { get; set; }
    public DateTime? LastSyncedAt { get; set; }
    public string? SyncStatus { get; set; }
    
    // Resource info
    public string? PlanName { get; set; }
    public int? DiskUsageMB { get; set; }
    public int? DiskQuotaMB { get; set; }
    public int? BandwidthUsageMB { get; set; }
    public int? BandwidthLimitMB { get; set; }
    public int? MaxEmailAccounts { get; set; }
    public int? MaxDatabases { get; set; }
    public int? MaxFtpAccounts { get; set; }
    public int? MaxSubdomains { get; set; }
    
    // Details (populated when requested)
    public List<HostingDomainDto>? Domains { get; set; }
    public int? EmailAccountCount { get; set; }
    public int? DatabaseCount { get; set; }
    public int? FtpAccountCount { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class HostingAccountCreateDto
{
    public int CustomerId { get; set; }
    public int ServiceId { get; set; }
    public int? ServerId { get; set; }
    public int? ServerControlPanelId { get; set; }
    public string? Provider { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? Status { get; set; }
    public DateTime ExpirationDate { get; set; }
    public string? PlanName { get; set; }
    public int? DiskQuotaMB { get; set; }
    public int? BandwidthLimitMB { get; set; }
    public int? MaxEmailAccounts { get; set; }
    public int? MaxDatabases { get; set; }
    public int? MaxFtpAccounts { get; set; }
    public int? MaxSubdomains { get; set; }
}

public class HostingAccountUpdateDto
{
    public string? Status { get; set; }
    public string? Password { get; set; }
    public string? PlanName { get; set; }
    public int? DiskQuotaMB { get; set; }
    public int? BandwidthLimitMB { get; set; }
    public int? MaxEmailAccounts { get; set; }
    public int? MaxDatabases { get; set; }
    public int? MaxFtpAccounts { get; set; }
    public int? MaxSubdomains { get; set; }
    public DateTime? ExpirationDate { get; set; }
}

public class HostingDomainDto
{
    public int Id { get; set; }
    public int HostingAccountId { get; set; }
    public string DomainName { get; set; } = string.Empty;
    public string DomainType { get; set; } = string.Empty;
    public string? DocumentRoot { get; set; }
    public bool SslEnabled { get; set; }
    public DateTime? SslExpirationDate { get; set; }
    public string? SslIssuer { get; set; }
    public bool PhpEnabled { get; set; }
    public string? PhpVersion { get; set; }
    public string? ExternalDomainId { get; set; }
    public DateTime? LastSyncedAt { get; set; }
    public string? SyncStatus { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class SyncResultDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int RecordsSynced { get; set; }
    public DateTime SyncedAt { get; set; } = DateTime.UtcNow;
}

public class SyncStatusDto
{
    public int HostingAccountId { get; set; }
    public string SyncStatus { get; set; } = string.Empty;
    public DateTime? LastSyncedAt { get; set; }
    public string? ExternalAccountId { get; set; }
}

public class SyncComparisonDto
{
    public int HostingAccountId { get; set; }
    public bool InSync { get; set; }
    public List<string> Differences { get; set; } = new List<string>();
    public DateTime LastChecked { get; set; }
}

public class ResourceUsageDto
{
    public int HostingAccountId { get; set; }
    public int? DiskUsageMB { get; set; }
    public int? DiskQuotaMB { get; set; }
    public int? BandwidthUsageMB { get; set; }
    public int? BandwidthLimitMB { get; set; }
    public int EmailAccountCount { get; set; }
    public int? MaxEmailAccounts { get; set; }
    public int DatabaseCount { get; set; }
    public int? MaxDatabases { get; set; }
    public int FtpAccountCount { get; set; }
    public int? MaxFtpAccounts { get; set; }
    public int DomainCount { get; set; }
    public int? MaxSubdomains { get; set; }
}

// Legacy DTOs - kept for backward compatibility
[Obsolete("Use HostingAccountCreateDto instead")]
public class CreateHostingAccountDto : HostingAccountCreateDto
{
    public string PasswordHash { get; set; } = string.Empty;
}

[Obsolete("Use HostingAccountUpdateDto instead")]
public class UpdateHostingAccountDto : HostingAccountUpdateDto
{
}

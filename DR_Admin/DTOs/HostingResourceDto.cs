namespace ISPAdmin.DTOs;

// ==============================
// Hosting Domain DTOs
// ==============================

public class HostingDomainCreateDto
{
    public int HostingAccountId { get; set; }
    public string DomainName { get; set; } = string.Empty;
    public string DomainType { get; set; } = "Main"; // Main, Addon, Parked, Subdomain
    public string? DocumentRoot { get; set; }
    public bool PhpEnabled { get; set; } = true;
    public string? PhpVersion { get; set; }
    public string? Notes { get; set; }
}

public class HostingDomainUpdateDto
{
    public string? DocumentRoot { get; set; }
    public bool? PhpEnabled { get; set; }
    public string? PhpVersion { get; set; }
    public bool? SslEnabled { get; set; }
    public string? Notes { get; set; }
}

// ==============================
// Hosting Email Account DTOs
// ==============================

public class HostingEmailAccountDto
{
    public int Id { get; set; }
    public int HostingAccountId { get; set; }
    public string EmailAddress { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public int? QuotaMB { get; set; }
    public int? UsageMB { get; set; }
    public bool IsForwarderOnly { get; set; }
    public string? ForwardTo { get; set; }
    public bool AutoResponderEnabled { get; set; }
    public bool SpamFilterEnabled { get; set; }
    public string? ExternalEmailId { get; set; }
    public DateTime? LastSyncedAt { get; set; }
    public string? SyncStatus { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class HostingEmailAccountCreateDto
{
    public int HostingAccountId { get; set; }
    public string EmailAddress { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int? QuotaMB { get; set; }
    public bool IsForwarderOnly { get; set; }
    public string? ForwardTo { get; set; }
    public bool AutoResponderEnabled { get; set; }
    public string? AutoResponderMessage { get; set; }
    public bool SpamFilterEnabled { get; set; }
    public int? SpamScoreThreshold { get; set; }
}

public class HostingEmailAccountUpdateDto
{
    public int? QuotaMB { get; set; }
    public string? ForwardTo { get; set; }
    public bool? AutoResponderEnabled { get; set; }
    public string? AutoResponderMessage { get; set; }
    public bool? SpamFilterEnabled { get; set; }
    public int? SpamScoreThreshold { get; set; }
}

// ==============================
// Hosting Database DTOs
// ==============================

public class HostingDatabaseDto
{
    public int Id { get; set; }
    public int HostingAccountId { get; set; }
    public string DatabaseName { get; set; } = string.Empty;
    public string DatabaseType { get; set; } = string.Empty;
    public int? SizeMB { get; set; }
    public string? ServerHost { get; set; }
    public int? ServerPort { get; set; }
    public string? ExternalDatabaseId { get; set; }
    public DateTime? LastSyncedAt { get; set; }
    public string? SyncStatus { get; set; }
    public List<HostingDatabaseUserDto>? DatabaseUsers { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class HostingDatabaseCreateDto
{
    public int HostingAccountId { get; set; }
    public string DatabaseName { get; set; } = string.Empty;
    public string DatabaseType { get; set; } = "MySQL";
    public string? CharacterSet { get; set; }
    public string? Collation { get; set; }
}

public class HostingDatabaseUpdateDto
{
    public string? Notes { get; set; }
}

// ==============================
// Hosting Database User DTOs
// ==============================

public class HostingDatabaseUserDto
{
    public int Id { get; set; }
    public int HostingDatabaseId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? Privileges { get; set; }
    public string? AllowedHosts { get; set; }
    public string? ExternalUserId { get; set; }
    public DateTime? LastSyncedAt { get; set; }
    public string? SyncStatus { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class HostingDatabaseUserCreateDto
{
    public int HostingDatabaseId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public List<string>? Privileges { get; set; }
    public string? AllowedHosts { get; set; }
}

// ==============================
// Hosting FTP Account DTOs
// ==============================

public class HostingFtpAccountDto
{
    public int Id { get; set; }
    public int HostingAccountId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string HomeDirectory { get; set; } = string.Empty;
    public int? QuotaMB { get; set; }
    public bool ReadOnly { get; set; }
    public bool SftpEnabled { get; set; }
    public bool FtpsEnabled { get; set; }
    public string? ExternalFtpId { get; set; }
    public DateTime? LastSyncedAt { get; set; }
    public string? SyncStatus { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class HostingFtpAccountCreateDto
{
    public int HostingAccountId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string HomeDirectory { get; set; } = string.Empty;
    public int? QuotaMB { get; set; }
    public bool ReadOnly { get; set; }
    public bool SftpEnabled { get; set; } = true;
    public bool FtpsEnabled { get; set; } = true;
}

public class HostingFtpAccountUpdateDto
{
    public string? HomeDirectory { get; set; }
    public int? QuotaMB { get; set; }
    public bool? ReadOnly { get; set; }
    public bool? SftpEnabled { get; set; }
    public bool? FtpsEnabled { get; set; }
}

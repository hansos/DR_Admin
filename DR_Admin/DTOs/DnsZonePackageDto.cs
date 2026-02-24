namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object representing a DNS zone package
/// </summary>
public class DnsZonePackageDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the DNS zone package
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
    /// Gets or sets whether this package is active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Gets or sets whether this is the default package
    /// </summary>
    public bool IsDefault { get; set; } = false;
    
    /// <summary>
    /// Gets or sets the sort order for display purposes
    /// </summary>
    public int SortOrder { get; set; } = 0;
    
    /// <summary>
    /// Gets or sets the reseller company ID (null if sold directly)
    /// </summary>
    public int? ResellerCompanyId { get; set; }
    
    /// <summary>
    /// Gets or sets the sales agent ID (null if self-service)
    /// </summary>
    public int? SalesAgentId { get; set; }
    
    /// <summary>
    /// Gets or sets the date and time when the package was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Gets or sets the date and time when the package was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }
    
    /// <summary>
    /// Gets or sets the DNS zone package records
    /// </summary>
    public ICollection<DnsZonePackageRecordDto> Records { get; set; } = new List<DnsZonePackageRecordDto>();

    /// <summary>
    /// Gets or sets the control panels assigned to this package
    /// </summary>
    public ICollection<DnsZonePackageControlPanelSummaryDto> ControlPanels { get; set; } = new List<DnsZonePackageControlPanelSummaryDto>();

    /// <summary>
    /// Gets or sets the servers assigned to this package
    /// </summary>
    public ICollection<DnsZonePackageServerSummaryDto> Servers { get; set; } = new List<DnsZonePackageServerSummaryDto>();
}

/// <summary>
/// Data transfer object for creating a new DNS zone package
/// </summary>
public class CreateDnsZonePackageDto
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
    /// Gets or sets whether this package is active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Gets or sets whether this is the default package
    /// </summary>
    public bool IsDefault { get; set; } = false;
    
    /// <summary>
    /// Gets or sets the sort order for display purposes
    /// </summary>
    public int SortOrder { get; set; } = 0;
    
    /// <summary>
    /// Gets or sets the reseller company ID (null if sold directly)
    /// </summary>
    public int? ResellerCompanyId { get; set; }
    
    /// <summary>
    /// Gets or sets the sales agent ID (null if self-service)
    /// </summary>
    public int? SalesAgentId { get; set; }
}

/// <summary>
/// Data transfer object for updating an existing DNS zone package
/// </summary>
public class UpdateDnsZonePackageDto
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
    /// Gets or sets whether this package is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets whether this is the default package
    /// </summary>
    public bool IsDefault { get; set; } = false;

    /// <summary>
    /// Gets or sets the sort order for display purposes
    /// </summary>
    public int SortOrder { get; set; } = 0;

    /// <summary>
    /// Gets or sets the reseller company ID (null if sold directly)
    /// </summary>
    public int? ResellerCompanyId { get; set; }

    /// <summary>
    /// Gets or sets the sales agent ID (null if self-service)
    /// </summary>
    public int? SalesAgentId { get; set; }
}

/// <summary>
/// Summary of a control panel assigned to a DNS zone package
/// </summary>
public class DnsZonePackageControlPanelSummaryDto
{
    /// <summary>
    /// Gets or sets the server control panel ID
    /// </summary>
    public int ControlPanelId { get; set; }

    /// <summary>
    /// Gets or sets the API URL of the control panel
    /// </summary>
    public string ApiUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the server this panel belongs to
    /// </summary>
    public string ServerName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the display name of the control panel type
    /// </summary>
    public string ControlPanelTypeName { get; set; } = string.Empty;
}

/// <summary>
/// Summary of a server assigned to a DNS zone package
/// </summary>
public class DnsZonePackageServerSummaryDto
{
    /// <summary>
    /// Gets or sets the server ID
    /// </summary>
    public int ServerId { get; set; }

    /// <summary>
    /// Gets or sets the server name
    /// </summary>
    public string ServerName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the server status
    /// </summary>
    public bool? Status { get; set; }
}

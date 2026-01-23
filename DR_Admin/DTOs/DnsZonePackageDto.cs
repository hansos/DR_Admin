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
}

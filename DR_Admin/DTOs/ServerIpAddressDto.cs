namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object representing a server IP address
/// </summary>
public class ServerIpAddressDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the IP address
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Gets or sets the server ID this IP address belongs to
    /// </summary>
    public int ServerId { get; set; }
    
    /// <summary>
    /// Gets or sets the IP address
    /// </summary>
    public string IpAddress { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the IP version (IPv4 or IPv6)
    /// </summary>
    public string IpVersion { get; set; } = "IPv4";
    
    /// <summary>
    /// Gets or sets whether this is the primary IP address
    /// </summary>
    public bool IsPrimary { get; set; }
    
    /// <summary>
    /// Gets or sets the status of the IP address (Active, Reserved, Blocked)
    /// </summary>
    public string Status { get; set; } = "Active";
    
    /// <summary>
    /// Gets or sets the entity this IP is assigned to (domain or service)
    /// </summary>
    public string? AssignedTo { get; set; }
    
    /// <summary>
    /// Gets or sets additional notes about the IP address
    /// </summary>
    public string? Notes { get; set; }
    
    /// <summary>
    /// Gets or sets the date and time when the IP address was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Gets or sets the date and time when the IP address was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Data transfer object for creating a new server IP address
/// </summary>
public class CreateServerIpAddressDto
{
    /// <summary>
    /// Gets or sets the server ID this IP address belongs to
    /// </summary>
    public int ServerId { get; set; }
    
    /// <summary>
    /// Gets or sets the IP address
    /// </summary>
    public string IpAddress { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the IP version (IPv4 or IPv6)
    /// </summary>
    public string IpVersion { get; set; } = "IPv4";
    
    /// <summary>
    /// Gets or sets whether this is the primary IP address
    /// </summary>
    public bool IsPrimary { get; set; }
    
    /// <summary>
    /// Gets or sets the status of the IP address (Active, Reserved, Blocked)
    /// </summary>
    public string Status { get; set; } = "Active";
    
    /// <summary>
    /// Gets or sets the entity this IP is assigned to (domain or service)
    /// </summary>
    public string? AssignedTo { get; set; }
    
    /// <summary>
    /// Gets or sets additional notes about the IP address
    /// </summary>
    public string? Notes { get; set; }
}

/// <summary>
/// Data transfer object for updating an existing server IP address
/// </summary>
public class UpdateServerIpAddressDto
{
    /// <summary>
    /// Gets or sets the IP address
    /// </summary>
    public string IpAddress { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the IP version (IPv4 or IPv6)
    /// </summary>
    public string IpVersion { get; set; } = "IPv4";
    
    /// <summary>
    /// Gets or sets whether this is the primary IP address
    /// </summary>
    public bool IsPrimary { get; set; }
    
    /// <summary>
    /// Gets or sets the status of the IP address (Active, Reserved, Blocked)
    /// </summary>
    public string Status { get; set; } = "Active";
    
    /// <summary>
    /// Gets or sets the entity this IP is assigned to (domain or service)
    /// </summary>
    public string? AssignedTo { get; set; }
    
    /// <summary>
    /// Gets or sets additional notes about the IP address
    /// </summary>
    public string? Notes { get; set; }
}

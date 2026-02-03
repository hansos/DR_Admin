namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object representing a name server for a domain
/// </summary>
public class NameServerDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the name server
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Gets or sets the domain identifier this name server belongs to
    /// </summary>
    public int DomainId { get; set; }
    
    /// <summary>
    /// Gets or sets the hostname of the name server
    /// </summary>
    public string Hostname { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the IP address of the name server (optional)
    /// </summary>
    public string? IpAddress { get; set; }
    
    /// <summary>
    /// Gets or sets whether this is the primary name server
    /// </summary>
    public bool IsPrimary { get; set; }
    
    /// <summary>
    /// Gets or sets the sort order for displaying name servers
    /// </summary>
    public int SortOrder { get; set; }
    
    /// <summary>
    /// Gets or sets the timestamp when the name server was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Gets or sets the timestamp when the name server was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Data transfer object for creating a new name server
/// </summary>
public class CreateNameServerDto
{
    /// <summary>
    /// Gets or sets the domain identifier this name server belongs to
    /// </summary>
    public int DomainId { get; set; }
    
    /// <summary>
    /// Gets or sets the hostname of the name server
    /// </summary>
    public string Hostname { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the IP address of the name server (optional)
    /// </summary>
    public string? IpAddress { get; set; }
    
    /// <summary>
    /// Gets or sets whether this is the primary name server
    /// </summary>
    public bool IsPrimary { get; set; } = false;
    
    /// <summary>
    /// Gets or sets the sort order for displaying name servers
    /// </summary>
    public int SortOrder { get; set; } = 0;
}

/// <summary>
/// Data transfer object for updating an existing name server
/// </summary>
public class UpdateNameServerDto
{
    /// <summary>
    /// Gets or sets the domain identifier this name server belongs to
    /// </summary>
    public int DomainId { get; set; }
    
    /// <summary>
    /// Gets or sets the hostname of the name server
    /// </summary>
    public string Hostname { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the IP address of the name server (optional)
    /// </summary>
    public string? IpAddress { get; set; }
    
    /// <summary>
    /// Gets or sets whether this is the primary name server
    /// </summary>
    public bool IsPrimary { get; set; }
    
    /// <summary>
    /// Gets or sets the sort order for displaying name servers
    /// </summary>
    public int SortOrder { get; set; }
}

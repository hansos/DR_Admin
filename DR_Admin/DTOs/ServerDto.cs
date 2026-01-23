namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object representing a server
/// </summary>
public class ServerDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the server
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Gets or sets the server name
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the server type (Physical, Cloud, Virtual)
    /// </summary>
    public string ServerType { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the hosting provider (AWS, Azure, DigitalOcean, etc.)
    /// </summary>
    public string? HostProvider { get; set; }
    
    /// <summary>
    /// Gets or sets the server location
    /// </summary>
    public string? Location { get; set; }
    
    /// <summary>
    /// Gets or sets the operating system
    /// </summary>
    public string OperatingSystem { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the server status (Active, Inactive, Maintenance)
    /// </summary>
    public string Status { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the number of CPU cores
    /// </summary>
    public int? CpuCores { get; set; }
    
    /// <summary>
    /// Gets or sets the RAM size in megabytes
    /// </summary>
    public int? RamMB { get; set; }
    
    /// <summary>
    /// Gets or sets the disk space in gigabytes
    /// </summary>
    public int? DiskSpaceGB { get; set; }
    
    /// <summary>
    /// Gets or sets additional notes about the server
    /// </summary>
    public string? Notes { get; set; }
    
    /// <summary>
    /// Gets or sets the date and time when the server was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Gets or sets the date and time when the server was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Data transfer object for creating a new server
/// </summary>
public class CreateServerDto
{
    /// <summary>
    /// Gets or sets the server name
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the server type (Physical, Cloud, Virtual)
    /// </summary>
    public string ServerType { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the hosting provider (AWS, Azure, DigitalOcean, etc.)
    /// </summary>
    public string? HostProvider { get; set; }
    
    /// <summary>
    /// Gets or sets the server location
    /// </summary>
    public string? Location { get; set; }
    
    /// <summary>
    /// Gets or sets the operating system
    /// </summary>
    public string OperatingSystem { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the server status (Active, Inactive, Maintenance)
    /// </summary>
    public string Status { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the number of CPU cores
    /// </summary>
    public int? CpuCores { get; set; }
    
    /// <summary>
    /// Gets or sets the RAM size in megabytes
    /// </summary>
    public int? RamMB { get; set; }
    
    /// <summary>
    /// Gets or sets the disk space in gigabytes
    /// </summary>
    public int? DiskSpaceGB { get; set; }
    
    /// <summary>
    /// Gets or sets additional notes about the server
    /// </summary>
    public string? Notes { get; set; }
}

/// <summary>
/// Data transfer object for updating an existing server
/// </summary>
public class UpdateServerDto
{
    /// <summary>
    /// Gets or sets the server name
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the server type (Physical, Cloud, Virtual)
    /// </summary>
    public string ServerType { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the hosting provider (AWS, Azure, DigitalOcean, etc.)
    /// </summary>
    public string? HostProvider { get; set; }
    
    /// <summary>
    /// Gets or sets the server location
    /// </summary>
    public string? Location { get; set; }
    
    /// <summary>
    /// Gets or sets the operating system
    /// </summary>
    public string OperatingSystem { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the server status (Active, Inactive, Maintenance)
    /// </summary>
    public string Status { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the number of CPU cores
    /// </summary>
    public int? CpuCores { get; set; }
    
    /// <summary>
    /// Gets or sets the RAM size in megabytes
    /// </summary>
    public int? RamMB { get; set; }
    
    /// <summary>
    /// Gets or sets the disk space in gigabytes
    /// </summary>
    public int? DiskSpaceGB { get; set; }
    
    /// <summary>
    /// Gets or sets additional notes about the server
    /// </summary>
    public string? Notes { get; set; }
}

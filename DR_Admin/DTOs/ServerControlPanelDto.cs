namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object representing a server control panel installation
/// </summary>
public class ServerControlPanelDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the server control panel
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Gets or sets the server ID
    /// </summary>
    public int ServerId { get; set; }
    
    /// <summary>
    /// Gets or sets the control panel type ID
    /// </summary>
    public int ControlPanelTypeId { get; set; }
    
    /// <summary>
    /// Gets or sets the API URL
    /// </summary>
    public string ApiUrl { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the port number
    /// </summary>
    public int Port { get; set; } = 2087;
    
    /// <summary>
    /// Gets or sets whether HTTPS is used
    /// </summary>
    public bool UseHttps { get; set; } = true;
    
    /// <summary>
    /// Gets or sets the username for authentication
    /// </summary>
    public string? Username { get; set; }
    
    /// <summary>
    /// Gets or sets the status (Active, Inactive, Error)
    /// </summary>
    public string Status { get; set; } = "Active";
    
    /// <summary>
    /// Gets or sets the date and time of the last connection test
    /// </summary>
    public DateTime? LastConnectionTest { get; set; }
    
    /// <summary>
    /// Gets or sets whether the connection is healthy
    /// </summary>
    public bool? IsConnectionHealthy { get; set; }
    
    /// <summary>
    /// Gets or sets the last error message
    /// </summary>
    public string? LastError { get; set; }
    
    /// <summary>
    /// Gets or sets additional notes
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets the ID of the server IP address this control panel API is bound to
    /// </summary>
    public int? IpAddressId { get; set; }

    /// <summary>
    /// Gets or sets the IP address value this control panel API is bound to (read-only, populated from related entity)
    /// </summary>
    public string? IpAddressValue { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the control panel was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the control panel was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Data transfer object for creating a new server control panel
/// </summary>
public class CreateServerControlPanelDto
{
    /// <summary>
    /// Gets or sets the server ID
    /// </summary>
    public int ServerId { get; set; }
    
    /// <summary>
    /// Gets or sets the control panel type ID
    /// </summary>
    public int ControlPanelTypeId { get; set; }
    
    /// <summary>
    /// Gets or sets the API URL
    /// </summary>
    public string ApiUrl { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the port number
    /// </summary>
    public int Port { get; set; } = 2087;
    
    /// <summary>
    /// Gets or sets whether HTTPS is used
    /// </summary>
    public bool UseHttps { get; set; } = true;
    
    /// <summary>
    /// Gets or sets the API token for authentication
    /// </summary>
    public string? ApiToken { get; set; }
    
    /// <summary>
    /// Gets or sets the API key for authentication
    /// </summary>
    public string? ApiKey { get; set; }
    
    /// <summary>
    /// Gets or sets the username for authentication
    /// </summary>
    public string? Username { get; set; }
    
    /// <summary>
    /// Gets or sets the password for authentication (will be hashed)
    /// </summary>
    public string? Password { get; set; }
    
    /// <summary>
    /// Gets or sets additional settings as JSON
    /// </summary>
    public string? AdditionalSettings { get; set; }
    
    /// <summary>
    /// Gets or sets the status (Active, Inactive, Error)
    /// </summary>
    public string Status { get; set; } = "Active";

    /// <summary>
    /// Gets or sets the ID of the server IP address this control panel API is bound to
    /// </summary>
    public int? IpAddressId { get; set; }

    /// <summary>
    /// Gets or sets additional notes
    /// </summary>
    public string? Notes { get; set; }
}

/// <summary>
/// Data transfer object for updating an existing server control panel
/// </summary>
public class UpdateServerControlPanelDto
{
    /// <summary>
    /// Gets or sets the API URL
    /// </summary>
    public string ApiUrl { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the port number
    /// </summary>
    public int Port { get; set; } = 2087;
    
    /// <summary>
    /// Gets or sets whether HTTPS is used
    /// </summary>
    public bool UseHttps { get; set; } = true;
    
    /// <summary>
    /// Gets or sets the API token for authentication
    /// </summary>
    public string? ApiToken { get; set; }
    
    /// <summary>
    /// Gets or sets the API key for authentication
    /// </summary>
    public string? ApiKey { get; set; }
    
    /// <summary>
    /// Gets or sets the username for authentication
    /// </summary>
    public string? Username { get; set; }
    
    /// <summary>
    /// Gets or sets the password for authentication (will be hashed if provided)
    /// </summary>
    public string? Password { get; set; }
    
    /// <summary>
    /// Gets or sets additional settings as JSON
    /// </summary>
    public string? AdditionalSettings { get; set; }
    
    /// <summary>
    /// Gets or sets the status (Active, Inactive, Error)
    /// </summary>
    public string Status { get; set; } = "Active";

    /// <summary>
    /// Gets or sets the ID of the server IP address this control panel API is bound to
    /// </summary>
    public int? IpAddressId { get; set; }

    /// <summary>
    /// Gets or sets additional notes
    /// </summary>
    public string? Notes { get; set; }
}


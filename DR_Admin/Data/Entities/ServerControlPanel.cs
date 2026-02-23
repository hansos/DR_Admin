namespace ISPAdmin.Data.Entities;

public class ServerControlPanel : EntityBase
{
    public int ServerId { get; set; }
    public int ControlPanelTypeId { get; set; }
    
    // Connection Settings
    public string ApiUrl { get; set; } = string.Empty;
    public int Port { get; set; } = 2087;
    public bool UseHttps { get; set; } = true;
    
    // Authentication
    public string? ApiToken { get; set; }
    public string? ApiKey { get; set; }
    public string? Username { get; set; }
    public string? PasswordHash { get; set; } // Encrypted password
    
    // Additional Settings (stored as JSON for flexibility)
    public string? AdditionalSettings { get; set; }
    
    // Status
    public string Status { get; set; } = "Active"; // Active, Inactive, Error
    public DateTime? LastConnectionTest { get; set; }
    public bool? IsConnectionHealthy { get; set; }
    public string? LastError { get; set; }
    
    public string? Notes { get; set; }

    /// <summary>
    /// Optional FK to the specific IP address this control panel API is bound to.
    /// </summary>
    public int? IpAddressId { get; set; }

    public Server Server { get; set; } = null!;
    public ControlPanelType ControlPanelType { get; set; } = null!;
    public ServerIpAddress? IpAddress { get; set; }
}

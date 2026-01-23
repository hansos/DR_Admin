namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object representing a control panel type
/// </summary>
public class ControlPanelTypeDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the control panel type
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Gets or sets the internal name of the control panel (cpanel, plesk, directadmin, etc.)
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the display name of the control panel
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the description of the control panel
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Gets or sets the version of the control panel
    /// </summary>
    public string? Version { get; set; }
    
    /// <summary>
    /// Gets or sets the website URL of the control panel
    /// </summary>
    public string? WebsiteUrl { get; set; }
    
    /// <summary>
    /// Gets or sets whether this control panel type is active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Gets or sets the date and time when the control panel type was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Gets or sets the date and time when the control panel type was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Data transfer object for creating a new control panel type
/// </summary>
public class CreateControlPanelTypeDto
{
    /// <summary>
    /// Gets or sets the internal name of the control panel (cpanel, plesk, directadmin, etc.)
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the display name of the control panel
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the description of the control panel
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Gets or sets the version of the control panel
    /// </summary>
    public string? Version { get; set; }
    
    /// <summary>
    /// Gets or sets the website URL of the control panel
    /// </summary>
    public string? WebsiteUrl { get; set; }
    
    /// <summary>
    /// Gets or sets whether this control panel type is active
    /// </summary>
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// Data transfer object for updating an existing control panel type
/// </summary>
public class UpdateControlPanelTypeDto
{
    /// <summary>
    /// Gets or sets the internal name of the control panel (cpanel, plesk, directadmin, etc.)
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the display name of the control panel
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the description of the control panel
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Gets or sets the version of the control panel
    /// </summary>
    public string? Version { get; set; }
    
    /// <summary>
    /// Gets or sets the website URL of the control panel
    /// </summary>
    public string? WebsiteUrl { get; set; }
    
    /// <summary>
    /// Gets or sets whether this control panel type is active
    /// </summary>
    public bool IsActive { get; set; } = true;
}

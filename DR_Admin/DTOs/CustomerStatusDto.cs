namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object representing a customer status
/// </summary>
public class CustomerStatusDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the customer status
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Gets or sets the unique code for the status
    /// </summary>
    public string Code { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the display name of the status
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the description of the status
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Gets or sets the color code for UI display (e.g., #FF0000)
    /// </summary>
    public string? Color { get; set; }
    
    /// <summary>
    /// Gets or sets whether this status is active
    /// </summary>
    public bool IsActive { get; set; }
    
    /// <summary>
    /// Gets or sets whether this is the default status for new customers
    /// </summary>
    public bool IsDefault { get; set; }
    
    /// <summary>
    /// Gets or sets the sort order for displaying statuses
    /// </summary>
    public int SortOrder { get; set; }
    
    /// <summary>
    /// Gets or sets the date and time when the status was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Gets or sets the date and time when the status was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Data transfer object for creating a new customer status
/// </summary>
public class CreateCustomerStatusDto
{
    /// <summary>
    /// Unique code for the status
    /// </summary>
    public string Code { get; set; } = string.Empty;
    
    /// <summary>
    /// Display name of the status
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Description of the status
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Color code for UI display (e.g., #FF0000)
    /// </summary>
    public string? Color { get; set; }
    
    /// <summary>
    /// Whether this status is active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Whether this is the default status for new customers
    /// </summary>
    public bool IsDefault { get; set; }
    
    /// <summary>
    /// Sort order for displaying statuses
    /// </summary>
    public int SortOrder { get; set; }
}

/// <summary>
/// Data transfer object for updating an existing customer status
/// </summary>
public class UpdateCustomerStatusDto
{
    /// <summary>
    /// Display name of the status
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Description of the status
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Color code for UI display (e.g., #FF0000)
    /// </summary>
    public string? Color { get; set; }
    
    /// <summary>
    /// Whether this status is active
    /// </summary>
    public bool IsActive { get; set; }
    
    /// <summary>
    /// Whether this is the default status for new customers
    /// </summary>
    public bool IsDefault { get; set; }
    
    /// <summary>
    /// Sort order for displaying statuses
    /// </summary>
    public int SortOrder { get; set; }
}

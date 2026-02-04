namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object representing an address type
/// </summary>
public class AddressTypeDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the address type
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Gets or sets the unique code for the address type
    /// </summary>
    public string Code { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the display name of the address type
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the description of the address type
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Gets or sets whether this address type is active
    /// </summary>
    public bool IsActive { get; set; }
    
    /// <summary>
    /// Gets or sets whether this is the default address type for new addresses
    /// </summary>
    public bool IsDefault { get; set; }
    
    /// <summary>
    /// Gets or sets the sort order for displaying address types
    /// </summary>
    public int SortOrder { get; set; }
    
    /// <summary>
    /// Gets or sets the date and time when the address type was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Gets or sets the date and time when the address type was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Data transfer object for creating a new address type
/// </summary>
public class CreateAddressTypeDto
{
    /// <summary>
    /// Unique code for the address type
    /// </summary>
    public string Code { get; set; } = string.Empty;
    
    /// <summary>
    /// Display name of the address type
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Description of the address type
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Whether this address type is active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Whether this is the default address type for new addresses
    /// </summary>
    public bool IsDefault { get; set; }
    
    /// <summary>
    /// Sort order for displaying address types
    /// </summary>
    public int SortOrder { get; set; }
}

/// <summary>
/// Data transfer object for updating an existing address type
/// </summary>
public class UpdateAddressTypeDto
{
    /// <summary>
    /// Unique code for the address type
    /// </summary>
    public string Code { get; set; } = string.Empty;
    
    /// <summary>
    /// Display name of the address type
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Description of the address type
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Whether this address type is active
    /// </summary>
    public bool IsActive { get; set; }
    
    /// <summary>
    /// Whether this is the default address type for new addresses
    /// </summary>
    public bool IsDefault { get; set; }
    
    /// <summary>
    /// Sort order for displaying address types
    /// </summary>
    public int SortOrder { get; set; }
}

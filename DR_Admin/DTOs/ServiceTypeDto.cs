namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object representing a service type category
/// </summary>
public class ServiceTypeDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the service type
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Gets or sets the service type name
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the service type description
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the date and time when the service type was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Gets or sets the date and time when the service type was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}


/// <summary>
/// Data transfer object for creating a new service type
/// </summary>
public class CreateServiceTypeDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}


/// <summary>
/// Data transfer object for updating an existing service type
/// </summary>
public class UpdateServiceTypeDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

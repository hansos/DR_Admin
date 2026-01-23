namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object representing a service offered to customers
/// </summary>
public class ServiceDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the service
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Gets or sets the service name
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the service description
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the ID of the service type
    /// </summary>
    public int ServiceTypeId { get; set; }
    
    /// <summary>
    /// Gets or sets the ID of the billing cycle
    /// </summary>
    public int BillingCycleId { get; set; }
    
    /// <summary>
    /// Gets or sets the service price
    /// </summary>
    public decimal Price { get; set; }
    
    /// <summary>
    /// Gets or sets the date and time when the service was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Gets or sets the date and time when the service was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}


/// <summary>
/// Data transfer object for creating a new service
/// </summary>
public class CreateServiceDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int ServiceTypeId { get; set; }
    public int BillingCycleId { get; set; }
    public decimal Price { get; set; }
}


/// <summary>
/// Data transfer object for updating an existing service
/// </summary>
public class UpdateServiceDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int ServiceTypeId { get; set; }
    public int BillingCycleId { get; set; }
    public decimal Price { get; set; }
}

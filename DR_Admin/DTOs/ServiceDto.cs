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
    public int? BillingCycleId { get; set; }
    
    /// <summary>
    /// Gets or sets the service price
    /// </summary>
    public decimal? Price { get; set; }
    
    /// <summary>
    /// Gets or sets the reseller company ID (null if sold directly)
    /// </summary>
    public int? ResellerCompanyId { get; set; }
    
    /// <summary>
    /// Gets or sets the sales agent ID (null if self-service)
    /// </summary>
    public int? SalesAgentId { get; set; }
    
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
    public int? BillingCycleId { get; set; }
    
    /// <summary>
    /// Gets or sets the service price
    /// </summary>
    public decimal? Price { get; set; }
    
    /// <summary>
    /// Gets or sets the reseller company ID (null if sold directly)
    /// </summary>
    public int? ResellerCompanyId { get; set; }
    
    /// <summary>
    /// Gets or sets the sales agent ID (null if self-service)
    /// </summary>
    public int? SalesAgentId { get; set; }
}


/// <summary>
/// Data transfer object for updating an existing service
/// </summary>
public class UpdateServiceDto
{
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
    public int? BillingCycleId { get; set; }
    
    /// <summary>
    /// Gets or sets the service price
    /// </summary>
    public decimal? Price { get; set; }
    
    /// <summary>
    /// Gets or sets the reseller company ID (null if sold directly)
    /// </summary>
    public int? ResellerCompanyId { get; set; }
    
    /// <summary>
    /// Gets or sets the sales agent ID (null if self-service)
    /// </summary>
    public int? SalesAgentId { get; set; }
}

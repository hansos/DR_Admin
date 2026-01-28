namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object representing a billing cycle
/// </summary>
public class BillingCycleDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the billing cycle
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Gets or sets the billing cycle code
    /// </summary>
    public string Code { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the billing cycle name (e.g., Monthly, Annually)
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the duration of the billing cycle in days
    /// </summary>
    public int DurationInDays { get; set; }
    
    /// <summary>
    /// Gets or sets the billing cycle description
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the sort order for display
    /// </summary>
    public int SortOrder { get; set; }
    
    /// <summary>
    /// Gets or sets the date and time when the billing cycle was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Gets or sets the date and time when the billing cycle was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}


/// <summary>
/// Data transfer object for creating a new billing cycle
/// </summary>
public class CreateBillingCycleDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int DurationInDays { get; set; }
    public string Description { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}


/// <summary>
/// Data transfer object for updating an existing billing cycle
/// </summary>
public class UpdateBillingCycleDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int DurationInDays { get; set; }
    public string Description { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}

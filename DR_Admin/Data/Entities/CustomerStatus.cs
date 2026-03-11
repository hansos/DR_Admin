namespace ISPAdmin.Data.Entities;

using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
/// Represents a customer status that can be assigned to customers
/// </summary>
public class CustomerStatus : EntityBase
{
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
    /// Gets or sets optional priority value used by seeding and business workflows.
    /// </summary>
    public int? Priority { get; set; }

    /// <summary>
    /// Gets or sets whether this status is a protected system status.
    /// </summary>
    public bool IsSystem { get; set; }

    /// <summary>
    /// Backward-compatible alias for IsSystem.
    /// </summary>
    [Obsolete("Use IsSystem property instead.",true)]
    [NotMapped]
    public bool IsSysten
    {
        get => IsSystem;
        set => IsSystem = value;
    }
    
    /// <summary>
    /// Gets or sets the normalized version of Code for case-insensitive searches
    /// </summary>
    public string NormalizedCode { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the normalized version of Name for case-insensitive searches
    /// </summary>
    public string NormalizedName { get; set; } = string.Empty;
}

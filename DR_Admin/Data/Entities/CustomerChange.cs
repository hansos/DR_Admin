namespace ISPAdmin.Data.Entities;

/// <summary>
/// Represents a tracked customer change event.
/// </summary>
public class CustomerChange : EntityBase
{
    /// <summary>
    /// Gets or sets the customer identifier.
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// Gets or sets the change type (Created, Updated, Deleted).
    /// </summary>
    public string ChangeType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the changed field name.
    /// </summary>
    public string? FieldName { get; set; }

    /// <summary>
    /// Gets or sets the old value.
    /// </summary>
    public string? OldValue { get; set; }

    /// <summary>
    /// Gets or sets the new value.
    /// </summary>
    public string? NewValue { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp of the change.
    /// </summary>
    public DateTime ChangedAt { get; set; }

    /// <summary>
    /// Gets or sets the user identifier that made the change.
    /// </summary>
    public int? ChangedByUserId { get; set; }

    /// <summary>
    /// Gets or sets the related customer.
    /// </summary>
    public Customer Customer { get; set; } = null!;

    /// <summary>
    /// Gets or sets the related user who made the change.
    /// </summary>
    public User? ChangedByUser { get; set; }
}

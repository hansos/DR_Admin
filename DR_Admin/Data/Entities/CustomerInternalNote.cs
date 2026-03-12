namespace ISPAdmin.Data.Entities;

/// <summary>
/// Represents an internal note attached to a customer.
/// </summary>
public class CustomerInternalNote : EntityBase
{
    /// <summary>
    /// Gets or sets the customer identifier.
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// Gets or sets the note text.
    /// </summary>
    public string Note { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user identifier that created the note.
    /// </summary>
    public int? CreatedByUserId { get; set; }

    /// <summary>
    /// Gets or sets the related customer.
    /// </summary>
    public Customer Customer { get; set; } = null!;

    /// <summary>
    /// Gets or sets the related user who created the note.
    /// </summary>
    public User? CreatedByUser { get; set; }
}

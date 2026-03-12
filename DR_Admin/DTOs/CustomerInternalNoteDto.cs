namespace ISPAdmin.DTOs;

/// <summary>
/// Represents an internal customer note.
/// </summary>
public class CustomerInternalNoteDto
{
    /// <summary>
    /// Gets or sets the note identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the customer identifier.
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// Gets or sets the note text.
    /// </summary>
    public string Note { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user identifier who created the note.
    /// </summary>
    public int? CreatedByUserId { get; set; }

    /// <summary>
    /// Gets or sets when the note was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets when the note was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Request for creating an internal customer note.
/// </summary>
public class CreateCustomerInternalNoteDto
{
    /// <summary>
    /// Gets or sets the note text.
    /// </summary>
    public string Note { get; set; } = string.Empty;
}

/// <summary>
/// Represents a tracked customer change.
/// </summary>
public class CustomerChangeDto
{
    /// <summary>
    /// Gets or sets the change identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the customer identifier.
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// Gets or sets the change type.
    /// </summary>
    public string ChangeType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the field name.
    /// </summary>
    public string? FieldName { get; set; }

    /// <summary>
    /// Gets or sets old value.
    /// </summary>
    public string? OldValue { get; set; }

    /// <summary>
    /// Gets or sets new value.
    /// </summary>
    public string? NewValue { get; set; }

    /// <summary>
    /// Gets or sets when the change occurred.
    /// </summary>
    public DateTime ChangedAt { get; set; }

    /// <summary>
    /// Gets or sets the user identifier that made the change.
    /// </summary>
    public int? ChangedByUserId { get; set; }
}

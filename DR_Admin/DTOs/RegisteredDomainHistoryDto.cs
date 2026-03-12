using ISPAdmin.Data.Enums;

namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object representing a registered domain history entry.
/// </summary>
public class RegisteredDomainHistoryDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the history entry.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the related registered domain identifier.
    /// </summary>
    public int RegisteredDomainId { get; set; }

    /// <summary>
    /// Gets or sets the history action type.
    /// </summary>
    public RegisteredDomainHistoryActionType ActionType { get; set; }

    /// <summary>
    /// Gets or sets the short action label.
    /// </summary>
    public string Action { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets optional action details.
    /// </summary>
    public string? Details { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp when the action occurred.
    /// </summary>
    public DateTime OccurredAt { get; set; }

    /// <summary>
    /// Gets or sets the source entity type that triggered the action.
    /// </summary>
    public string? SourceEntityType { get; set; }

    /// <summary>
    /// Gets or sets the source entity identifier that triggered the action.
    /// </summary>
    public int? SourceEntityId { get; set; }

    /// <summary>
    /// Gets or sets the user identifier that performed the action.
    /// </summary>
    public int? PerformedByUserId { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the history entry was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the history entry was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

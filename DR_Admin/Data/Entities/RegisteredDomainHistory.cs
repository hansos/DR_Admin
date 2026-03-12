using ISPAdmin.Data.Enums;

namespace ISPAdmin.Data.Entities;

/// <summary>
/// Represents an immutable history entry for a registered domain action.
/// </summary>
public class RegisteredDomainHistory : EntityBase
{
    /// <summary>
    /// Gets or sets the foreign key to the related registered domain.
    /// </summary>
    public int RegisteredDomainId { get; set; }

    /// <summary>
    /// Gets or sets the action type of the history entry.
    /// </summary>
    public RegisteredDomainHistoryActionType ActionType { get; set; }

    /// <summary>
    /// Gets or sets a short action label.
    /// </summary>
    public string Action { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets optional details for the action.
    /// </summary>
    public string? Details { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp when the action occurred.
    /// </summary>
    public DateTime OccurredAt { get; set; }

    /// <summary>
    /// Gets or sets optional source entity type that triggered this history entry.
    /// </summary>
    public string? SourceEntityType { get; set; }

    /// <summary>
    /// Gets or sets optional source entity identifier that triggered this history entry.
    /// </summary>
    public int? SourceEntityId { get; set; }

    /// <summary>
    /// Gets or sets optional user identifier that performed the action.
    /// </summary>
    public int? PerformedByUserId { get; set; }

    /// <summary>
    /// Gets or sets the related registered domain.
    /// </summary>
    public RegisteredDomain RegisteredDomain { get; set; } = null!;

    /// <summary>
    /// Gets or sets the user who performed the action.
    /// </summary>
    public User? PerformedByUser { get; set; }
}

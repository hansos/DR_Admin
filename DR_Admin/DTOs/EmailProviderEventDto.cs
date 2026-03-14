namespace ISPAdmin.DTOs;

/// <summary>
/// Represents a provider webhook event for email delivery lifecycle updates.
/// </summary>
public class EmailProviderEventDto
{
    /// <summary>
    /// Gets or sets the provider message identifier.
    /// </summary>
    public string ProviderMessageId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the provider event type (for example Delivered, Bounce, Complaint, Opened).
    /// </summary>
    public string EventType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the optional event details payload.
    /// </summary>
    public string? Details { get; set; }

    /// <summary>
    /// Gets or sets the optional provider source identifier.
    /// </summary>
    public string? Source { get; set; }

    /// <summary>
    /// Gets or sets the optional UTC timestamp when the provider reports the event occurred.
    /// </summary>
    public DateTime? OccurredAtUtc { get; set; }
}

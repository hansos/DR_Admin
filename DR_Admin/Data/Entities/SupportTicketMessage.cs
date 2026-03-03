namespace ISPAdmin.Data.Entities;

/// <summary>
/// Represents a single message in a support ticket conversation.
/// </summary>
public class SupportTicketMessage : EntityBase
{
    /// <summary>
    /// Gets or sets the support ticket identifier.
    /// </summary>
    public int SupportTicketId { get; set; }

    /// <summary>
    /// Gets or sets the sender user identifier.
    /// </summary>
    public int SenderUserId { get; set; }

    /// <summary>
    /// Gets or sets the sender role used for rendering conversation sides.
    /// </summary>
    public string SenderRole { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the message body.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the support ticket navigation property.
    /// </summary>
    public SupportTicket? SupportTicket { get; set; }

    /// <summary>
    /// Gets or sets the sender user navigation property.
    /// </summary>
    public User? SenderUser { get; set; }
}

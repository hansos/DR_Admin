namespace ISPAdmin.Data.Entities;

/// <summary>
/// Represents a support ticket created by a customer and handled by support staff.
/// </summary>
public class SupportTicket : EntityBase
{
    /// <summary>
    /// Gets or sets the customer identifier that owns the ticket.
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// Gets or sets the user identifier of the user who created the ticket.
    /// </summary>
    public int CreatedByUserId { get; set; }

    /// <summary>
    /// Gets or sets the support user identifier assigned to the ticket.
    /// </summary>
    public int? AssignedToUserId { get; set; }

    /// <summary>
    /// Gets or sets the ticket subject.
    /// </summary>
    public string Subject { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the ticket status.
    /// </summary>
    public string Status { get; set; } = "Open";

    /// <summary>
    /// Gets or sets the ticket priority.
    /// </summary>
    public string Priority { get; set; } = "Normal";

    /// <summary>
    /// Gets or sets the timestamp of the latest message in the ticket thread.
    /// </summary>
    public DateTime? LastMessageAt { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the ticket was closed.
    /// </summary>
    public DateTime? ClosedAt { get; set; }

    /// <summary>
    /// Gets or sets the customer navigation property.
    /// </summary>
    public Customer? Customer { get; set; }

    /// <summary>
    /// Gets or sets the ticket creator navigation property.
    /// </summary>
    public User? CreatedByUser { get; set; }

    /// <summary>
    /// Gets or sets the assigned support user navigation property.
    /// </summary>
    public User? AssignedToUser { get; set; }

    /// <summary>
    /// Gets or sets the collection of messages in the ticket thread.
    /// </summary>
    public ICollection<SupportTicketMessage> Messages { get; set; } = new List<SupportTicketMessage>();
}

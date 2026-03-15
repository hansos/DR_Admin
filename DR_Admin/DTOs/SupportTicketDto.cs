namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object representing a support ticket.
/// </summary>
public class SupportTicketDto
{
    /// <summary>
    /// Gets or sets the support ticket identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the customer identifier owning the ticket.
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// Gets or sets the customer name.
    /// </summary>
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user identifier of the ticket creator.
    /// </summary>
    public int CreatedByUserId { get; set; }

    /// <summary>
    /// Gets or sets the username of the ticket creator.
    /// </summary>
    public string CreatedByUsername { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the assigned support user identifier.
    /// </summary>
    public int? AssignedToUserId { get; set; }

    /// <summary>
    /// Gets or sets the assigned support department.
    /// </summary>
    public string? AssignedDepartment { get; set; }

    /// <summary>
    /// Gets or sets the username of the assigned support user.
    /// </summary>
    public string? AssignedToUsername { get; set; }

    /// <summary>
    /// Gets or sets the ticket subject.
    /// </summary>
    public string Subject { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the ticket description.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the ticket status.
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the ticket priority.
    /// </summary>
    public string Priority { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the ticket intake source.
    /// </summary>
    public string Source { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the creation timestamp in UTC.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the update timestamp in UTC.
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the last message timestamp in UTC.
    /// </summary>
    public DateTime? LastMessageAt { get; set; }

    /// <summary>
    /// Gets or sets the closed timestamp in UTC.
    /// </summary>
    public DateTime? ClosedAt { get; set; }

    /// <summary>
    /// Gets or sets the ticket conversation messages.
    /// </summary>
    public List<SupportTicketMessageDto> Messages { get; set; } = new();
}

/// <summary>
/// Data transfer object representing a support ticket message.
/// </summary>
public class SupportTicketMessageDto
{
    /// <summary>
    /// Gets or sets the message identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the support ticket identifier.
    /// </summary>
    public int SupportTicketId { get; set; }

    /// <summary>
    /// Gets or sets the sender user identifier.
    /// </summary>
    public int SenderUserId { get; set; }

    /// <summary>
    /// Gets or sets the sender username.
    /// </summary>
    public string SenderUsername { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the sender role.
    /// </summary>
    public string SenderRole { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the message body.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the creation timestamp in UTC.
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Data transfer object used when creating a support ticket.
/// </summary>
public class CreateSupportTicketDto
{
    /// <summary>
    /// Gets or sets the ticket subject.
    /// </summary>
    public string Subject { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the ticket description body.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the ticket priority.
    /// </summary>
    public string Priority { get; set; } = "Normal";

    /// <summary>
    /// Gets or sets the ticket intake source.
    /// </summary>
    public string? Source { get; set; }

    /// <summary>
    /// Gets or sets an optional customer identifier for support-initiated ticket creation.
    /// </summary>
    public int? CustomerId { get; set; }
}

/// <summary>
/// Data transfer object used when posting a ticket message.
/// </summary>
public class CreateSupportTicketMessageDto
{
    /// <summary>
    /// Gets or sets the message body.
    /// </summary>
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// Data transfer object used when updating a support ticket status.
/// </summary>
public class UpdateSupportTicketStatusDto
{
    /// <summary>
    /// Gets or sets the new status value.
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the support department assignment.
    /// </summary>
    public string? AssignedDepartment { get; set; }

    /// <summary>
    /// Gets or sets the support user assignment.
    /// </summary>
    public int? AssignedToUserId { get; set; }
}

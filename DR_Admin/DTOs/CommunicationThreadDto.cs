namespace ISPAdmin.DTOs;

/// <summary>
/// Represents a communication participant in thread responses.
/// </summary>
public class CommunicationParticipantDto
{
    /// <summary>
    /// Gets or sets the participant email address.
    /// </summary>
    public string EmailAddress { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the participant display name.
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// Gets or sets the participant role in the thread.
    /// </summary>
    public string Role { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether this participant is marked as primary.
    /// </summary>
    public bool IsPrimary { get; set; }
}

/// <summary>
/// Represents a communication message in thread responses.
/// </summary>
public class CommunicationMessageDto
{
    /// <summary>
    /// Gets or sets the communication message identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the message direction.
    /// </summary>
    public string Direction { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the provider external message identifier.
    /// </summary>
    public string? ExternalMessageId { get; set; }

    /// <summary>
    /// Gets or sets the from address.
    /// </summary>
    public string FromAddress { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the to addresses.
    /// </summary>
    public string ToAddresses { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the carbon-copy addresses.
    /// </summary>
    public string? CcAddresses { get; set; }

    /// <summary>
    /// Gets or sets the blind-carbon-copy addresses.
    /// </summary>
    public string? BccAddresses { get; set; }

    /// <summary>
    /// Gets or sets the message subject.
    /// </summary>
    public string Subject { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the plain-text body.
    /// </summary>
    public string? BodyText { get; set; }

    /// <summary>
    /// Gets or sets the HTML body.
    /// </summary>
    public string? BodyHtml { get; set; }

    /// <summary>
    /// Gets or sets the provider key.
    /// </summary>
    public string? Provider { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the message is read.
    /// </summary>
    public bool IsRead { get; set; }

    /// <summary>
    /// Gets or sets the received timestamp in UTC.
    /// </summary>
    public DateTime? ReceivedAtUtc { get; set; }

    /// <summary>
    /// Gets or sets the sent timestamp in UTC.
    /// </summary>
    public DateTime? SentAtUtc { get; set; }

    /// <summary>
    /// Gets or sets the read timestamp in UTC.
    /// </summary>
    public DateTime? ReadAtUtc { get; set; }

    /// <summary>
    /// Gets or sets the created timestamp.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the updated timestamp.
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Represents a communication thread summary for list views.
/// </summary>
public class CommunicationThreadDto
{
    /// <summary>
    /// Gets or sets the communication thread identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the thread subject.
    /// </summary>
    public string Subject { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the linked customer identifier.
    /// </summary>
    public int? CustomerId { get; set; }

    /// <summary>
    /// Gets or sets the linked user identifier.
    /// </summary>
    public int? UserId { get; set; }

    /// <summary>
    /// Gets or sets the related entity type.
    /// </summary>
    public string? RelatedEntityType { get; set; }

    /// <summary>
    /// Gets or sets the related entity identifier.
    /// </summary>
    public int? RelatedEntityId { get; set; }

    /// <summary>
    /// Gets or sets the timestamp of the latest message in UTC.
    /// </summary>
    public DateTime? LastMessageAtUtc { get; set; }

    /// <summary>
    /// Gets or sets the thread status.
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the unread message count for the thread.
    /// </summary>
    public int UnreadCount { get; set; }

    /// <summary>
    /// Gets or sets the created timestamp.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the updated timestamp.
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Represents detailed communication thread data including messages and participants.
/// </summary>
public class CommunicationThreadDetailsDto : CommunicationThreadDto
{
    /// <summary>
    /// Gets or sets the thread participants.
    /// </summary>
    public List<CommunicationParticipantDto> Participants { get; set; } = [];

    /// <summary>
    /// Gets or sets the thread messages ordered by sent/received timestamp.
    /// </summary>
    public List<CommunicationMessageDto> Messages { get; set; } = [];
}

/// <summary>
/// Represents the payload used to update a communication thread status.
/// </summary>
public class UpdateCommunicationThreadStatusDto
{
    /// <summary>
    /// Gets or sets the target thread status.
    /// </summary>
    public string Status { get; set; } = string.Empty;
}

/// <summary>
/// Represents the payload used to update a communication message read state.
/// </summary>
public class UpdateCommunicationMessageReadStateDto
{
    /// <summary>
    /// Gets or sets a value indicating whether the message is read.
    /// </summary>
    public bool IsRead { get; set; } = true;
}

/// <summary>
/// Represents the payload used to queue a reply from a communication thread.
/// </summary>
public class CreateCommunicationReplyDto
{
    /// <summary>
    /// Gets or sets the primary recipient address list.
    /// </summary>
    public string To { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the carbon-copy recipient address list.
    /// </summary>
    public string? Cc { get; set; }

    /// <summary>
    /// Gets or sets the blind-carbon-copy recipient address list.
    /// </summary>
    public string? Bcc { get; set; }

    /// <summary>
    /// Gets or sets the optional subject override.
    /// </summary>
    public string? Subject { get; set; }

    /// <summary>
    /// Gets or sets the plain-text body content.
    /// </summary>
    public string? BodyText { get; set; }

    /// <summary>
    /// Gets or sets the HTML body content.
    /// </summary>
    public string? BodyHtml { get; set; }

    /// <summary>
    /// Gets or sets the optional preferred email provider key.
    /// </summary>
    public string? Provider { get; set; }

    /// <summary>
    /// Gets or sets optional attachment paths.
    /// </summary>
    public List<string>? AttachmentPaths { get; set; }
}

namespace ISPAdmin.DTOs;

/// <summary>
/// DTO for queuing an email to be sent
/// </summary>
public class QueueEmailDto
{
    public string To { get; set; } = string.Empty;
    public string? Cc { get; set; }
    public string? Bcc { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string? BodyText { get; set; }
    public string? BodyHtml { get; set; }
    public string? Provider { get; set; }
    public int? CustomerId { get; set; }
    public int? UserId { get; set; }
    public string? RelatedEntityType { get; set; }
    public int? RelatedEntityId { get; set; }
    public List<string>? AttachmentPaths { get; set; }
}

/// <summary>
/// DTO for email queue response
/// </summary>
public class QueueEmailResponseDto
{
    public int Id { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

namespace ISPAdmin.Data.Entities;

public class SentEmail : EntityBase
{
    public DateTime SentDate { get; set; }
    public string From { get; set; } = string.Empty;
    public string To { get; set; } = string.Empty;
    public string? Cc { get; set; }
    public string? Bcc { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string? Body { get; set; }
    public string MessageId { get; set; } = string.Empty;
    public string? Status { get; set; }
    public string? ErrorMessage { get; set; }
    public int? RetryCount { get; set; }
    public int? CustomerId { get; set; }
    public int? UserId { get; set; }
    public string? RelatedEntityType { get; set; }
    public int? RelatedEntityId { get; set; }
    public string? Attachments { get; set; }

    public Customer? Customer { get; set; }
    public User? User { get; set; }
}

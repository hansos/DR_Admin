namespace ISPAdmin.Data.Entities;

public class CommunicationMessage : EntityBase
{
    public int CommunicationThreadId { get; set; }
    public string Direction { get; set; } = CommunicationMessageDirection.Outbound;
    public string? ExternalMessageId { get; set; }
    public string? InternetMessageId { get; set; }
    public string FromAddress { get; set; } = string.Empty;
    public string ToAddresses { get; set; } = string.Empty;
    public string? CcAddresses { get; set; }
    public string? BccAddresses { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string? BodyText { get; set; }
    public string? BodyHtml { get; set; }
    public string? Provider { get; set; }
    public int? SentEmailId { get; set; }
    public bool IsRead { get; set; }
    public DateTime? ReceivedAtUtc { get; set; }
    public DateTime? SentAtUtc { get; set; }
    public DateTime? ReadAtUtc { get; set; }

    public CommunicationThread? CommunicationThread { get; set; }
    public SentEmail? SentEmail { get; set; }
    public ICollection<CommunicationAttachment> Attachments { get; set; } = new List<CommunicationAttachment>();
    public ICollection<CommunicationStatusEvent> StatusEvents { get; set; } = new List<CommunicationStatusEvent>();
}

public static class CommunicationMessageDirection
{
    public const string Inbound = "Inbound";
    public const string Outbound = "Outbound";
}

namespace EmailReceiverLib.Models;

public class InboundEmailMessage
{
    public string ExternalMessageId { get; set; } = string.Empty;
    public string? InternetMessageId { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string FromAddress { get; set; } = string.Empty;
    public string FromName { get; set; } = string.Empty;
    public List<string> ToAddresses { get; set; } = [];
    public string BodyPreview { get; set; } = string.Empty;
    public DateTime? ReceivedAtUtc { get; set; }
    public bool IsRead { get; set; }
    public bool HasAttachments { get; set; }
}

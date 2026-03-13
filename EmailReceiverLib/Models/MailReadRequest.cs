namespace EmailReceiverLib.Models;

public class MailReadRequest
{
    public string? Folder { get; set; }
    public DateTime? ReceivedAfterUtc { get; set; }
    public bool UnreadOnly { get; set; }
    public string? AliasRecipient { get; set; }
    public int MaxItems { get; set; } = 25;
}

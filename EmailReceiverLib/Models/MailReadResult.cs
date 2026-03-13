namespace EmailReceiverLib.Models;

public class MailReadResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? NextCursor { get; set; }
    public List<InboundEmailMessage> Messages { get; set; } = [];
    public List<string> Errors { get; set; } = [];
}

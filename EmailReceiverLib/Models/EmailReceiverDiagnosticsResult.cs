namespace EmailReceiverLib.Models;

public class EmailReceiverDiagnosticsResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string MailboxAddress { get; set; } = string.Empty;
    public string? AliasRecipient { get; set; }
    public string GraphMessagesEndpoint { get; set; } = string.Empty;
    public string TokenTenantId { get; set; } = string.Empty;
    public string TokenClientId { get; set; } = string.Empty;
    public string? TokenAudience { get; set; }
    public DateTime? TokenExpiresAtUtc { get; set; }
    public List<string> TokenRoles { get; set; } = [];
    public List<string> Errors { get; set; } = [];
}

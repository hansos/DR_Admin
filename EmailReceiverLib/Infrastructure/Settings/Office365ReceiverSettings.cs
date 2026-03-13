namespace EmailReceiverLib.Infrastructure.Settings;

public class Office365ReceiverSettings
{
    public string TenantId { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string MailboxAddress { get; set; } = string.Empty;
    public string AliasRecipient { get; set; } = string.Empty;
    public string DefaultFolder { get; set; } = "Inbox";
    public int DefaultMaxItems { get; set; } = 25;
}

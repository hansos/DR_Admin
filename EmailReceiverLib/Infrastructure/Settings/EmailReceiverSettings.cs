namespace EmailReceiverLib.Infrastructure.Settings;

public class EmailReceiverSettings
{
    public string Provider { get; set; } = string.Empty;
    public EmailReceiverPluginSelectionSettings Selection { get; set; } = new();
    public Office365ReceiverSettings? Office365 { get; set; }
}

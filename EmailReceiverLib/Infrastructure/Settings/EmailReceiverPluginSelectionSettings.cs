namespace EmailReceiverLib.Infrastructure.Settings;

public class EmailReceiverPluginSelectionSettings
{
    public string? DefaultPluginKey { get; set; }
    public List<string> EnabledPluginKeys { get; set; } = [];
    public List<string> FallbackPluginKeys { get; set; } = [];
    public List<string> DisabledPluginKeys { get; set; } = [];
}

using EmailReceiverLib.Infrastructure.Settings;
using EmailReceiverLib.Interfaces;
using PluginLib.Plugins;

namespace EmailReceiverLib.Plugins.Email;

public abstract class EmailReceiverPluginBase(
    string key,
    string displayName,
    string version,
    bool isEnabled,
    IReadOnlyCollection<string> capabilities) : IEmailReceiverPlugin
{
    public PluginType Type => PluginType.EmailReceiver;
    public string Key { get; } = key;
    public string DisplayName { get; } = displayName;
    public string Version { get; } = version;
    public IReadOnlyCollection<string> Capabilities { get; } = capabilities;
    public bool IsEnabled { get; } = isEnabled;

    public abstract bool CanCreate(EmailReceiverSettings settings);
    public abstract IEmailReceiver Create(EmailReceiverSettings settings);
}

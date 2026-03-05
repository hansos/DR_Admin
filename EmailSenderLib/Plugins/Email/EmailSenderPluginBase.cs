using EmailSenderLib.Infrastructure.Settings;
using EmailSenderLib.Interfaces;
using PluginLib.Plugins;

namespace EmailSenderLib.Plugins.Email
{
    public abstract class EmailSenderPluginBase(
        string key,
        string displayName,
        string version,
        bool isEnabled,
        IReadOnlyCollection<string> capabilities) : IEmailSenderPlugin
    {
        public PluginType Type => PluginType.Email;
        public string Key { get; } = key;
        public string DisplayName { get; } = displayName;
        public string Version { get; } = version;
        public IReadOnlyCollection<string> Capabilities { get; } = capabilities;
        public bool IsEnabled { get; } = isEnabled;

        public abstract bool CanCreate(EmailSettings settings);
        public abstract IEmailSender Create(EmailSettings settings);
    }
}

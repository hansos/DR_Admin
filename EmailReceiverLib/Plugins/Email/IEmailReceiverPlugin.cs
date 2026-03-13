using EmailReceiverLib.Infrastructure.Settings;
using EmailReceiverLib.Interfaces;
using PluginLib.Plugins;

namespace EmailReceiverLib.Plugins.Email;

public interface IEmailReceiverPlugin : IPlugin
{
    bool CanCreate(EmailReceiverSettings settings);
    IEmailReceiver Create(EmailReceiverSettings settings);
}

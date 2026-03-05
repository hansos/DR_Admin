using EmailSenderLib.Infrastructure.Settings;
using EmailSenderLib.Interfaces;
using PluginLib.Plugins;

namespace EmailSenderLib.Plugins.Email
{
    public interface IEmailSenderPlugin : IPlugin
    {
        bool CanCreate(EmailSettings settings);
        IEmailSender Create(EmailSettings settings);
    }
}

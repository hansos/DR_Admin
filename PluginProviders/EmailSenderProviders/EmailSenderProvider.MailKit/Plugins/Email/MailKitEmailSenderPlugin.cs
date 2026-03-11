using EmailSenderLib.Implementations;
using EmailSenderLib.Infrastructure.Settings;
using EmailSenderLib.Interfaces;
using EmailSenderLib.Plugins.Email;

namespace EmailSenderProviders.MailKit.Plugins.Email
{
    public sealed class MailKitEmailSenderPlugin()
        : EmailSenderPluginBase("mailkit", "MailKit", "1.0.0", true, ["send", "attachments"])
    {
        public override bool CanCreate(EmailSettings settings) => settings.MailKit is not null;

        public override IEmailSender Create(EmailSettings settings)
        {
            if (settings.MailKit is null)
            {
                throw new InvalidOperationException("MailKit settings are not configured");
            }

            return new MailKitEmailSender(
                settings.MailKit.Host,
                settings.MailKit.Port,
                settings.MailKit.Username,
                settings.MailKit.Password,
                settings.MailKit.UseSsl,
                settings.MailKit.FromEmail,
                settings.MailKit.FromName);
        }
    }
}

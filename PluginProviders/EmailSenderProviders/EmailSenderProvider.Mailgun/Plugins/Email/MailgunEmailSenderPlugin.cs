using EmailSenderLib.Implementations;
using EmailSenderLib.Infrastructure.Settings;
using EmailSenderLib.Interfaces;
using EmailSenderLib.Plugins.Email;

namespace EmailSenderProviders.Mailgun.Plugins.Email
{
    public sealed class MailgunEmailSenderPlugin()
        : EmailSenderPluginBase("mailgun", "Mailgun", "1.0.0", true, ["send", "bulk"])
    {
        public override bool CanCreate(EmailSettings settings) => settings.Mailgun is not null;

        public override IEmailSender Create(EmailSettings settings)
        {
            if (settings.Mailgun is null)
            {
                throw new InvalidOperationException("Mailgun settings are not configured");
            }

            return new MailgunEmailSender(
                settings.Mailgun.ApiKey,
                settings.Mailgun.Domain,
                settings.Mailgun.FromEmail,
                settings.Mailgun.FromName,
                settings.Mailgun.Region);
        }
    }
}

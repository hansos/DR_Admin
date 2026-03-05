using EmailSenderLib.Implementations;
using EmailSenderLib.Infrastructure.Settings;
using EmailSenderLib.Interfaces;
using EmailSenderLib.Plugins.Email;

namespace EmailSenderProviders.SendGrid.Plugins.Email
{
    public sealed class SendGridEmailSenderPlugin()
        : EmailSenderPluginBase("sendgrid", "SendGrid", "1.0.0", true, ["send", "bulk"])
    {
        public override bool CanCreate(EmailSettings settings) => settings.SendGrid is not null;

        public override IEmailSender Create(EmailSettings settings)
        {
            if (settings.SendGrid is null)
            {
                throw new InvalidOperationException("SendGrid settings are not configured");
            }

            return new SendGridEmailSender(
                settings.SendGrid.ApiKey,
                settings.SendGrid.FromEmail,
                settings.SendGrid.FromName);
        }
    }
}

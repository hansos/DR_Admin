using EmailSenderLib.Implementations;
using EmailSenderLib.Infrastructure.Settings;
using EmailSenderLib.Interfaces;
using EmailSenderLib.Plugins.Email;

namespace EmailSenderProviders.AmazonSes.Plugins.Email
{
    public sealed class AmazonSesEmailSenderPlugin()
        : EmailSenderPluginBase("amazonses", "Amazon SES", "1.0.0", true, ["send", "bulk"])
    {
        public override bool CanCreate(EmailSettings settings) => settings.AmazonSes is not null;

        public override IEmailSender Create(EmailSettings settings)
        {
            if (settings.AmazonSes is null)
            {
                throw new InvalidOperationException("AmazonSes settings are not configured");
            }

            return new AmazonSesEmailSender(
                settings.AmazonSes.AccessKeyId,
                settings.AmazonSes.SecretAccessKey,
                settings.AmazonSes.Region,
                settings.AmazonSes.FromEmail,
                settings.AmazonSes.FromName);
        }
    }
}

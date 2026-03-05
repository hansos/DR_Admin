using EmailSenderLib.Implementations;
using EmailSenderLib.Infrastructure.Settings;
using EmailSenderLib.Interfaces;
using EmailSenderLib.Plugins.Email;

namespace EmailSenderProviders.Smtp.Plugins.Email
{
    public sealed class SmtpEmailSenderPlugin()
        : EmailSenderPluginBase("smtp", "SMTP", "1.0.0", true, ["send", "attachments"])
    {
        public override bool CanCreate(EmailSettings settings) => settings.Smtp is not null;

        public override IEmailSender Create(EmailSettings settings)
        {
            if (settings.Smtp is null)
            {
                throw new InvalidOperationException("SMTP settings are not configured");
            }

            return new SmtpEmailSender(
                settings.Smtp.Host,
                settings.Smtp.Port,
                settings.Smtp.Username,
                settings.Smtp.Password,
                settings.Smtp.EnableSsl,
                settings.Smtp.FromEmail,
                settings.Smtp.FromName);
        }
    }
}

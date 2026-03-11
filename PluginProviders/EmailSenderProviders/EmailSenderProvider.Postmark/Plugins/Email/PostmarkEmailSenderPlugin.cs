using EmailSenderLib.Implementations;
using EmailSenderLib.Infrastructure.Settings;
using EmailSenderLib.Interfaces;
using EmailSenderLib.Plugins.Email;

namespace EmailSenderProviders.Postmark.Plugins.Email
{
    public sealed class PostmarkEmailSenderPlugin()
        : EmailSenderPluginBase("postmark", "Postmark", "1.0.0", true, ["send"])
    {
        public override bool CanCreate(EmailSettings settings) => settings.Postmark is not null;

        public override IEmailSender Create(EmailSettings settings)
        {
            if (settings.Postmark is null)
            {
                throw new InvalidOperationException("Postmark settings are not configured");
            }

            return new PostmarkEmailSender(
                settings.Postmark.ServerToken,
                settings.Postmark.FromEmail,
                settings.Postmark.FromName);
        }
    }
}

using EmailSenderLib.Implementations;
using EmailSenderLib.Infrastructure.Settings;
using EmailSenderLib.Interfaces;
using EmailSenderLib.Plugins.Email;

namespace EmailSenderProviders.Exchange.Plugins.Email
{
    public sealed class ExchangeEmailSenderPlugin()
        : EmailSenderPluginBase("exchange", "Exchange", "1.0.0", true, ["send"])
    {
        public override bool CanCreate(EmailSettings settings) => settings.Exchange is not null;

        public override IEmailSender Create(EmailSettings settings)
        {
            if (settings.Exchange is null)
            {
                throw new InvalidOperationException("Exchange settings are not configured");
            }

            return new ExchangeEmailSender(
                settings.Exchange.ServerUrl,
                settings.Exchange.Username,
                settings.Exchange.Password,
                settings.Exchange.Domain,
                settings.Exchange.FromEmail,
                settings.Exchange.FromName,
                settings.Exchange.Version);
        }
    }
}

using EmailReceiverLib.Infrastructure.Settings;
using EmailReceiverLib.Interfaces;
using EmailReceiverLib.Plugins.Email;
using EmailReceiverProviders.Office365.Implementations;

namespace EmailReceiverProviders.Office365.Plugins.Email;

public sealed class Office365EmailReceiverPlugin()
    : EmailReceiverPluginBase("office365", "Office365", "1.0.0", true, ["read", "mark-read"])
{
    public override bool CanCreate(EmailReceiverSettings settings)
    {
        var cfg = settings.Office365;
        return cfg is not null
               && !string.IsNullOrWhiteSpace(cfg.TenantId)
               && !string.IsNullOrWhiteSpace(cfg.ClientId)
               && !string.IsNullOrWhiteSpace(cfg.ClientSecret)
               && !string.IsNullOrWhiteSpace(cfg.MailboxAddress);
    }

    public override IEmailReceiver Create(EmailReceiverSettings settings)
    {
        if (settings.Office365 is null)
        {
            throw new InvalidOperationException("Office365 receiver settings are not configured.");
        }

        return new Office365GraphEmailReceiver(settings.Office365);
    }
}

using EmailSenderLib.Implementations;
using EmailSenderLib.Infrastructure.Settings;
using EmailSenderLib.Interfaces;
using EmailSenderLib.Plugins.Email;

namespace EmailSenderProviders.GraphApi.Plugins.Email
{
    public sealed class GraphApiEmailSenderPlugin()
        : EmailSenderPluginBase("graphapi", "Graph API", "1.0.0", true, ["send"])
    {
        public override bool CanCreate(EmailSettings settings) => settings.GraphApi is not null;

        public override IEmailSender Create(EmailSettings settings)
        {
            if (settings.GraphApi is null)
            {
                throw new InvalidOperationException("GraphApi settings are not configured");
            }

            return new GraphApiEmailSender(
                settings.GraphApi.TenantId,
                settings.GraphApi.ClientId,
                settings.GraphApi.ClientSecret,
                settings.GraphApi.FromEmail,
                settings.GraphApi.FromName,
                settings.GraphApi.Scope);
        }
    }
}

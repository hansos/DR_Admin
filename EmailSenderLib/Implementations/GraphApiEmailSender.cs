using EmailSenderLib.Interfaces;

namespace EmailSenderLib.Implementations
{
    public class GraphApiEmailSender : IEmailSender
    {
        private readonly string _tenantId;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _fromEmail;
        private readonly string _fromName;
        private readonly string _scope;

        public GraphApiEmailSender(string tenantId, string clientId, string clientSecret, string fromEmail, string fromName = "", string scope = "https://graph.microsoft.com/.default")
        {
            _tenantId = tenantId;
            _clientId = clientId;
            _clientSecret = clientSecret;
            _fromEmail = fromEmail;
            _fromName = fromName;
            _scope = scope;
        }

        public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = false)
        {
            await SendEmailAsync(to, subject, body, new List<string>(), isHtml);
        }

        public async Task SendEmailAsync(string to, string subject, string body, List<string> attachments, bool isHtml = false)
        {
            // TODO: Implement Microsoft Graph API email sending
            // Requires Microsoft.Graph NuGet package
            throw new NotImplementedException("GraphApi email sending requires Microsoft.Graph package implementation");
        }
    }
}

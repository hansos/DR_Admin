using EmailSenderLib.Interfaces;

namespace EmailSenderLib.Implementations
{
    public class ExchangeEmailSender : IEmailSender
    {
        private readonly string _serverUrl;
        private readonly string _username;
        private readonly string _password;
        private readonly string _domain;
        private readonly string _fromEmail;
        private readonly string _fromName;
        private readonly string _version;

        public ExchangeEmailSender(string serverUrl, string username, string password, string domain, string fromEmail, string fromName = "", string version = "Exchange2013_SP1")
        {
            _serverUrl = serverUrl;
            _username = username;
            _password = password;
            _domain = domain;
            _fromEmail = fromEmail;
            _fromName = fromName;
            _version = version;
        }

        public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = false)
        {
            await SendEmailAsync(to, subject, body, new List<string>(), isHtml);
        }

        public async Task SendEmailAsync(string to, string subject, string body, List<string> attachments, bool isHtml = false)
        {
            // TODO: Implement Exchange Web Services email sending
            // Requires Microsoft.Exchange.WebServices NuGet package
            await Task.CompletedTask;
            throw new NotImplementedException("Exchange email sending requires EWS Managed API implementation");
        }
    }
}

using EmailSenderLib.Interfaces;

namespace EmailSenderLib.Implementations
{
    public class MailgunEmailSender : IEmailSender
    {
        private readonly string _apiKey;
        private readonly string _domain;
        private readonly string _fromEmail;
        private readonly string _fromName;
        private readonly string _region;

        public MailgunEmailSender(string apiKey, string domain, string fromEmail, string fromName = "", string region = "US")
        {
            _apiKey = apiKey;
            _domain = domain;
            _fromEmail = fromEmail;
            _fromName = fromName;
            _region = region;
        }

        public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = false)
        {
            await SendEmailAsync(to, subject, body, new List<string>(), isHtml);
        }

        public async Task SendEmailAsync(string to, string subject, string body, List<string> attachments, bool isHtml = false)
        {
            // TODO: Implement Mailgun email sending via HTTP API
            await Task.CompletedTask;
            throw new NotImplementedException("Mailgun email sending requires HTTP API implementation");
        }
    }
}

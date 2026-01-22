using EmailSenderLib.Interfaces;

namespace EmailSenderLib.Implementations
{
    public class SendGridEmailSender : IEmailSender
    {
        private readonly string _apiKey;
        private readonly string _fromEmail;
        private readonly string _fromName;

        public SendGridEmailSender(string apiKey, string fromEmail, string fromName = "")
        {
            _apiKey = apiKey;
            _fromEmail = fromEmail;
            _fromName = fromName;
        }

        public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = false)
        {
            await SendEmailAsync(to, subject, body, new List<string>(), isHtml);
        }

        public async Task SendEmailAsync(string to, string subject, string body, List<string> attachments, bool isHtml = false)
        {
            // TODO: Implement SendGrid email sending
            // Requires SendGrid NuGet package
            await Task.CompletedTask;
            throw new NotImplementedException("SendGrid email sending requires SendGrid package implementation");
        }
    }
}

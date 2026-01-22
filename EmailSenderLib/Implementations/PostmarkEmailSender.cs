using EmailSenderLib.Interfaces;

namespace EmailSenderLib.Implementations
{
    public class PostmarkEmailSender : IEmailSender
    {
        private readonly string _serverToken;
        private readonly string _fromEmail;
        private readonly string _fromName;

        public PostmarkEmailSender(string serverToken, string fromEmail, string fromName = "")
        {
            _serverToken = serverToken;
            _fromEmail = fromEmail;
            _fromName = fromName;
        }

        public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = false)
        {
            await SendEmailAsync(to, subject, body, new List<string>(), isHtml);
        }

        public async Task SendEmailAsync(string to, string subject, string body, List<string> attachments, bool isHtml = false)
        {
            // TODO: Implement Postmark email sending
            // Requires PostmarkDotNet NuGet package
            await Task.CompletedTask;
            throw new NotImplementedException("Postmark email sending requires PostmarkDotNet package implementation");
        }
    }
}

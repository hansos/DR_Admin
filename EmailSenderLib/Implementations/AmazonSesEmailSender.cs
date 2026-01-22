using EmailSenderLib.Interfaces;

namespace EmailSenderLib.Implementations
{
    public class AmazonSesEmailSender : IEmailSender
    {
        private readonly string _accessKeyId;
        private readonly string _secretAccessKey;
        private readonly string _region;
        private readonly string _fromEmail;
        private readonly string _fromName;

        public AmazonSesEmailSender(string accessKeyId, string secretAccessKey, string region, string fromEmail, string fromName = "")
        {
            _accessKeyId = accessKeyId;
            _secretAccessKey = secretAccessKey;
            _region = region;
            _fromEmail = fromEmail;
            _fromName = fromName;
        }

        public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = false)
        {
            await SendEmailAsync(to, subject, body, new List<string>(), isHtml);
        }

        public async Task SendEmailAsync(string to, string subject, string body, List<string> attachments, bool isHtml = false)
        {
            // TODO: Implement Amazon SES email sending
            // Requires AWSSDK.SimpleEmail NuGet package
            await Task.CompletedTask;
            throw new NotImplementedException("Amazon SES email sending requires AWS SDK implementation");
        }
    }
}

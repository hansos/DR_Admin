using EmailSenderLib.Interfaces;

namespace EmailSenderLib.Implementations
{
    public class MailKitEmailSender : IEmailSender
    {
        private readonly string _host;
        private readonly int _port;
        private readonly string _username;
        private readonly string _password;
        private readonly bool _useSsl;
        private readonly string _fromEmail;
        private readonly string _fromName;

        public MailKitEmailSender(string host, int port, string username, string password, bool useSsl = true, string fromEmail = "", string fromName = "")
        {
            _host = host;
            _port = port;
            _username = username;
            _password = password;
            _useSsl = useSsl;
            _fromEmail = string.IsNullOrEmpty(fromEmail) ? username : fromEmail;
            _fromName = fromName;
        }

        public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = false)
        {
            await SendEmailAsync(to, subject, body, new List<string>(), isHtml);
        }

        public async Task SendEmailAsync(string to, string subject, string body, List<string> attachments, bool isHtml = false)
        {
            await Task.CompletedTask;
            throw new NotImplementedException("MailKit email sending requires MailKit package implementation");
        }
    }
}

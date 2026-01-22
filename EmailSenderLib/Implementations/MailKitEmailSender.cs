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
            // TODO: Implement MailKit email sending
            // Requires MailKit NuGet package
            // Example implementation:
            // using var client = new SmtpClient();
            // await client.ConnectAsync(_host, _port, _useSsl);
            // await client.AuthenticateAsync(_username, _password);
            // var message = new MimeMessage();
            // message.From.Add(new MailboxAddress(_fromName, _fromEmail));
            // message.To.Add(MailboxAddress.Parse(to));
            // message.Subject = subject;
            // var builder = new BodyBuilder { HtmlBody = isHtml ? body : null, TextBody = isHtml ? null : body };
            // foreach (var attachment in attachments ?? [])
            //     if (File.Exists(attachment))
            //         builder.Attachments.Add(attachment);
            // message.Body = builder.ToMessageBody();
            // await client.SendAsync(message);
            // await client.DisconnectAsync(true);
            
            await Task.CompletedTask;
            throw new NotImplementedException("MailKit email sending requires MailKit package implementation");
        }
    }
}

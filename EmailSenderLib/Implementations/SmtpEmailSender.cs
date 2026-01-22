using EmailSenderLib.Interfaces;
using System.Net;
using System.Net.Mail;

namespace EmailSenderLib.Implementations
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly string _host;
        private readonly int _port;
        private readonly string _username;
        private readonly string _password;
        private readonly bool _enableSsl;
        private readonly string _fromEmail;
        private readonly string _fromName;

        public SmtpEmailSender(string host, int port, string username, string password, bool enableSsl, string fromEmail = "", string fromName = "")
        {
            _host = host;
            _port = port;
            _username = username;
            _password = password;
            _enableSsl = enableSsl;
            _fromEmail = string.IsNullOrEmpty(fromEmail) ? username : fromEmail;
            _fromName = fromName;
        }

        public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = false)
        {
            await SendEmailAsync(to, subject, body, new List<string>(), isHtml);
        }

        public async Task SendEmailAsync(string to, string subject, string body, List<string> attachments, bool isHtml = false)
        {
            using var client = new SmtpClient(_host, _port)
            {
                EnableSsl = _enableSsl,
                Credentials = new NetworkCredential(_username, _password)
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_fromEmail, _fromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml
            };

            mailMessage.To.Add(to);

            if (attachments != null)
            {
                foreach (var attachment in attachments)
                {
                    if (File.Exists(attachment))
                    {
                        mailMessage.Attachments.Add(new Attachment(attachment));
                    }
                }
            }

            await client.SendMailAsync(mailMessage);
        }
    }
}

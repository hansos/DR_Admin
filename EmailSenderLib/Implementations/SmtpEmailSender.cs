using EmailSenderLib.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Serilog;

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
        private static readonly ILogger _log = Log.ForContext<SmtpEmailSender>();

        public SmtpEmailSender(string host, int port, string username, string password, bool enableSsl, string fromEmail = "", string fromName = "")
        {
            _host = host;
            _port = port;
            _username = username;
            _password = password;
            _enableSsl = enableSsl;
            _fromEmail = string.IsNullOrEmpty(fromEmail) ? username : fromEmail;
            _fromName = fromName;

            _log.Information("SmtpEmailSender initialized - Host: {Host}, Port: {Port}, SSL: {EnableSsl}, Username: {Username}, FromEmail: {FromEmail}",
                _host, _port, _enableSsl, _username, _fromEmail);
        }

        public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = false)
        {
            await SendEmailAsync(to, subject, body, new List<string>(), isHtml);
        }

        public async Task SendEmailAsync(string to, string subject, string body, List<string> attachments, bool isHtml = false)
        {
            _log.Information("Attempting to send email - To: {To}, Subject: {Subject}, IsHtml: {IsHtml}, Attachments: {AttachmentCount}",
                to, subject, isHtml, attachments?.Count ?? 0);

            _log.Debug("SMTP Configuration - Host: {Host}, Port: {Port}, SSL: {EnableSsl}, Username: {Username}",
                _host, _port, _enableSsl, _username);

            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_fromName, _fromEmail));
                message.To.Add(MailboxAddress.Parse(to));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder();
                if (isHtml)
                {
                    bodyBuilder.HtmlBody = body;
                }
                else
                {
                    bodyBuilder.TextBody = body;
                }

                // Add attachments
                if (attachments != null && attachments.Any())
                {
                    _log.Debug("Processing {Count} attachments", attachments.Count);
                    foreach (var attachment in attachments)
                    {
                        if (File.Exists(attachment))
                        {
                            bodyBuilder.Attachments.Add(attachment);
                            _log.Debug("Attached file: {FileName}", attachment);
                        }
                        else
                        {
                            _log.Warning("Attachment file not found: {FileName}", attachment);
                        }
                    }
                }

                message.Body = bodyBuilder.ToMessageBody();
                _log.Debug("Mail message created - From: {From} ({FromName}), To: {To}", _fromEmail, _fromName, to);

                using var client = new SmtpClient();

                // Determine the secure socket options based on port and SSL setting
                SecureSocketOptions socketOptions;
                if (_port == 465)
                {
                    // Port 465 uses implicit SSL/TLS
                    socketOptions = SecureSocketOptions.SslOnConnect;
                }
                else if (_enableSsl)
                {
                    // Port 587 and others typically use STARTTLS
                    socketOptions = SecureSocketOptions.StartTls;
                }
                else
                {
                    socketOptions = SecureSocketOptions.None;
                }

                _log.Information("Connecting to SMTP server {Host}:{Port} with {SocketOptions}", _host, _port, socketOptions);
                await client.ConnectAsync(_host, _port, socketOptions);

                _log.Debug("Authenticating with SMTP server");
                await client.AuthenticateAsync(_username, _password);

                _log.Information("Sending email via SMTP server {Host}:{Port}", _host, _port);
                await client.SendAsync(message);

                await client.DisconnectAsync(true);
                _log.Information("Email sent successfully to {To}", to);
            }
            catch (AuthenticationException ex)
            {
                _log.Error(ex, "SMTP Authentication Error - Message: {Message}", ex.Message);
                throw;
            }
            catch (MailKit.ServiceNotConnectedException ex)
            {
                _log.Error(ex, "SMTP Connection Error - Not connected to server. Message: {Message}", ex.Message);
                throw;
            }
            catch (MailKit.ServiceNotAuthenticatedException ex)
            {
                _log.Error(ex, "SMTP Authentication Required - Message: {Message}", ex.Message);
                throw;
            }
            catch (InvalidOperationException ex)
            {
                _log.Error(ex, "Invalid Operation - This usually indicates configuration issues. Message: {Message}", ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Unexpected error sending email - Type: {ExceptionType}, Message: {Message}", 
                    ex.GetType().Name, ex.Message);
                throw;
            }
        }
    }
}

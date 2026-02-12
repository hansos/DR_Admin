using EmailSenderLib.Interfaces;
using System.Net;
using System.Net.Mail;
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
                using var client = new SmtpClient(_host, _port)
                {
                    EnableSsl = _enableSsl,
                    Credentials = new NetworkCredential(_username, _password)
                };

                _log.Debug("SMTP client created, preparing mail message");

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_fromEmail, _fromName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = isHtml
                };

                mailMessage.To.Add(to);
                _log.Debug("Mail message created - From: {From} ({FromName}), To: {To}", _fromEmail, _fromName, to);

                if (attachments != null && attachments.Any())
                {
                    _log.Debug("Processing {Count} attachments", attachments.Count);
                    foreach (var attachment in attachments)
                    {
                        if (File.Exists(attachment))
                        {
                            mailMessage.Attachments.Add(new Attachment(attachment));
                            _log.Debug("Attached file: {FileName}", attachment);
                        }
                        else
                        {
                            _log.Warning("Attachment file not found: {FileName}", attachment);
                        }
                    }
                }

                _log.Information("Sending email via SMTP server {Host}:{Port}", _host, _port);
                await client.SendMailAsync(mailMessage);
                _log.Information("Email sent successfully to {To}", to);
            }
            catch (SmtpFailedRecipientsException ex)
            {
                _log.Error(ex, "SMTP Failed Recipients - Failed to send to one or more recipients. InnerExceptions: {InnerExceptionCount}",
                    ex.InnerExceptions?.Length ?? 0);
                foreach (var innerEx in ex.InnerExceptions ?? Array.Empty<SmtpFailedRecipientException>())
                {
                    _log.Error("Failed recipient: {Recipient}, StatusCode: {StatusCode}", 
                        innerEx.FailedRecipient, innerEx.StatusCode);
                }
                throw;
            }
            catch (SmtpException ex)
            {
                _log.Error(ex, "SMTP Error - StatusCode: {StatusCode}, Message: {Message}", 
                    ex.StatusCode, ex.Message);
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

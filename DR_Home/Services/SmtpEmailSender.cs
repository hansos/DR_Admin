using DR_Home.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

namespace DR_Home.Services;

public interface IEmailSender
{
    Task SendAsync(string subject, string body, string replyToEmail);
    Task SendVerificationAsync(string toEmail, string verificationCode);
}

public class SmtpEmailSender : IEmailSender
{
    private readonly EmailSmtpSettings _settings;
    private readonly ILogger<SmtpEmailSender> _logger;

    public SmtpEmailSender(EmailSmtpSettings settings, ILogger<SmtpEmailSender> logger)
    {
        _settings = settings;
        _logger = logger;
    }

    public async Task SendAsync(string subject, string body, string replyToEmail)
    {
        _logger.LogInformation(
            "Sending email via {Host}:{Port} from {From} to {To}",
            _settings.Host, _settings.Port, _settings.FromEmail, _settings.ToEmail);

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_settings.FromName, _settings.FromEmail));
        message.To.Add(MailboxAddress.Parse(_settings.ToEmail));

        if (!string.IsNullOrWhiteSpace(replyToEmail))
            message.ReplyTo.Add(MailboxAddress.Parse(replyToEmail));

        message.Subject = subject;
        message.Body = new TextPart(TextFormat.Plain) { Text = body };

        using var client = new SmtpClient();
        try
        {
            // Connect with STARTTLS on port 587 (required by AWS SES)
            await client.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_settings.Username, _settings.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(quit: true);

            _logger.LogInformation("Email sent successfully to {To}", _settings.ToEmail);
        }
        catch (SmtpCommandException ex)
        {
            _logger.LogError(ex,
                "SMTP command error — StatusCode={StatusCode} ErrorCode={ErrorCode} Message={Message}",
                ex.StatusCode, ex.ErrorCode, ex.Message);
            throw;
        }
        catch (AuthenticationException ex)
        {
            _logger.LogError(ex, "SMTP authentication failed for user {Username}", _settings.Username);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error sending email via {Host}:{Port}", _settings.Host, _settings.Port);
            throw;
        }
    }

    public async Task SendVerificationAsync(string toEmail, string verificationCode)
    {
        _logger.LogInformation("Sending verification email to {To}", toEmail);

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_settings.FromName, _settings.FromEmail));
        message.To.Add(MailboxAddress.Parse(toEmail));
        message.Subject = "DR_Admin — Verify your email address";

        var plainText = new TextPart(TextFormat.Plain)
        {
            Text =
                "DR_Admin — Email Verification\n" +
                "=============================\n\n" +
                "Your verification code is: " + verificationCode + "\n\n" +
                "Enter this code in the contact form to verify your email address.\n" +
                "This code expires in 10 minutes.\n\n" +
                "If you did not request this code, please ignore this email.\n"
        };

        var htmlBody = new TextPart(TextFormat.Html)
        {
            Text =
                "<!DOCTYPE html>" +
                "<html><head><meta charset=\"utf-8\"></head>" +
                "<body style=\"margin:0;padding:0;background:#1a1a2e;font-family:Arial,Helvetica,sans-serif;\">" +
                "<table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" style=\"max-width:600px;margin:0 auto;padding:40px 20px;\">" +
                "<tr><td style=\"text-align:center;padding-bottom:30px;\">" +
                "<h1 style=\"color:#667eea;margin:0;font-size:28px;\">&#128274; DR_Admin</h1>" +
                "<p style=\"color:#adb5bd;margin:5px 0 0;font-size:14px;\">Email Verification</p>" +
                "</td></tr>" +
                "<tr><td style=\"background:#212529;border-radius:12px;padding:40px;text-align:center;\">" +
                "<p style=\"color:#e9ecef;font-size:16px;margin:0 0 25px;\">Your verification code is:</p>" +
                "<div style=\"background:linear-gradient(135deg,#667eea 0%,#764ba2 100%);color:white;font-size:48px;" +
                "font-weight:700;letter-spacing:16px;padding:20px 40px;border-radius:12px;display:inline-block;\">" +
                verificationCode +
                "</div>" +
                "<p style=\"color:#adb5bd;font-size:14px;margin:25px 0 0;\">Enter this code in the contact form to verify your email address." +
                "<br>This code expires in <strong>10 minutes</strong>.</p>" +
                "</td></tr>" +
                "<tr><td style=\"text-align:center;padding-top:25px;\">" +
                "<p style=\"color:#6c757d;font-size:12px;margin:0;\">If you did not request this code, you can safely ignore this email.</p>" +
                "</td></tr>" +
                "</table></body></html>"
        };

        message.Body = new Multipart("alternative") { plainText, htmlBody };

        using var client = new SmtpClient();
        try
        {
            await client.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_settings.Username, _settings.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(quit: true);

            _logger.LogInformation("Verification email sent to {To}", toEmail);
        }
        catch (SmtpCommandException ex)
        {
            _logger.LogError(ex, "SMTP command error sending verification — StatusCode={StatusCode}", ex.StatusCode);
            throw;
        }
        catch (AuthenticationException ex)
        {
            _logger.LogError(ex, "SMTP auth failed sending verification for user {Username}", _settings.Username);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error sending verification email via {Host}:{Port}", _settings.Host, _settings.Port);
            throw;
        }
    }
}

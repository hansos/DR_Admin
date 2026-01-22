using EmailSenderLib.Implementations;
using EmailSenderLib.Infrastructure.Settings;
using EmailSenderLib.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmailSenderLib.Factories
{
    public class EmailSenderFactory
    {
        private readonly EmailSettings _mailSettings;

        public EmailSenderFactory(EmailSettings emailSettings)
        {
            _mailSettings = emailSettings ?? throw new ArgumentNullException(nameof(emailSettings));
        }

        public IEmailSender CreateEmailSender()
        {

            return _mailSettings.Provider.ToLower() switch
            {
                "smtp" => _mailSettings.Smtp is not null 
                    ? new SmtpEmailSender(
                        _mailSettings.Smtp.Host,
                        _mailSettings.Smtp.Port,
                        _mailSettings.Smtp.Username,
                        _mailSettings.Smtp.Password,
                        _mailSettings.Smtp.EnableSsl,
                        _mailSettings.Smtp.FromEmail,
                        _mailSettings.Smtp.FromName
                    )
                    : throw new InvalidOperationException("SMTP settings are not configured"),
                
                "mailkit" => _mailSettings.MailKit is not null
                    ? new MailKitEmailSender(
                        _mailSettings.MailKit.Host,
                        _mailSettings.MailKit.Port,
                        _mailSettings.MailKit.Username,
                        _mailSettings.MailKit.Password,
                        _mailSettings.MailKit.UseSsl,
                        _mailSettings.MailKit.FromEmail,
                        _mailSettings.MailKit.FromName
                    )
                    : throw new InvalidOperationException("MailKit settings are not configured"),
                
                "graphapi" => _mailSettings.GraphApi is not null
                    ? new GraphApiEmailSender(
                        _mailSettings.GraphApi.TenantId,
                        _mailSettings.GraphApi.ClientId,
                        _mailSettings.GraphApi.ClientSecret,
                        _mailSettings.GraphApi.FromEmail,
                        _mailSettings.GraphApi.FromName,
                        _mailSettings.GraphApi.Scope
                    )
                    : throw new InvalidOperationException("GraphApi settings are not configured"),
                
                "exchange" => _mailSettings.Exchange is not null
                    ? new ExchangeEmailSender(
                        _mailSettings.Exchange.ServerUrl,
                        _mailSettings.Exchange.Username,
                        _mailSettings.Exchange.Password,
                        _mailSettings.Exchange.Domain,
                        _mailSettings.Exchange.FromEmail,
                        _mailSettings.Exchange.FromName,
                        _mailSettings.Exchange.Version
                    )
                    : throw new InvalidOperationException("Exchange settings are not configured"),
                
                "sendgrid" => _mailSettings.SendGrid is not null
                    ? new SendGridEmailSender(
                        _mailSettings.SendGrid.ApiKey,
                        _mailSettings.SendGrid.FromEmail,
                        _mailSettings.SendGrid.FromName
                    )
                    : throw new InvalidOperationException("SendGrid settings are not configured"),
                
                "mailgun" => _mailSettings.Mailgun is not null
                    ? new MailgunEmailSender(
                        _mailSettings.Mailgun.ApiKey,
                        _mailSettings.Mailgun.Domain,
                        _mailSettings.Mailgun.FromEmail,
                        _mailSettings.Mailgun.FromName,
                        _mailSettings.Mailgun.Region
                    )
                    : throw new InvalidOperationException("Mailgun settings are not configured"),
                
                "amazonses" => _mailSettings.AmazonSes is not null
                    ? new AmazonSesEmailSender(
                        _mailSettings.AmazonSes.AccessKeyId,
                        _mailSettings.AmazonSes.SecretAccessKey,
                        _mailSettings.AmazonSes.Region,
                        _mailSettings.AmazonSes.FromEmail,
                        _mailSettings.AmazonSes.FromName
                    )
                    : throw new InvalidOperationException("AmazonSes settings are not configured"),
                
                "postmark" => _mailSettings.Postmark is not null
                    ? new PostmarkEmailSender(
                        _mailSettings.Postmark.ServerToken,
                        _mailSettings.Postmark.FromEmail,
                        _mailSettings.Postmark.FromName
                    )
                    : throw new InvalidOperationException("Postmark settings are not configured"),
                
                _ => throw new InvalidOperationException($"Unknown email provider: {_mailSettings.Provider}")
            };
        }
    }
}

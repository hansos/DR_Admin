# EmailSenderLib `IEmailSender` Interface - User Manual

The `IEmailSender` interface provides asynchronous methods for sending emails, supporting both plain text and HTML content as well as attachments. It is designed for integration into applications that need programmatic email delivery.

---

## Table of Contents

1. [Methods and providers](#Methods_and_providers)
2. [Sending Emails](#sending-emails)
    - Basic Email
    - Email with Attachments

---

## Methods and providers
DR_Admin supports the following mail methods and mail sender providers: 

|Provider|Implementation Class|Key Configuration Fields / Requirements|
|---|---|---|
|SMTP|`SmtpEmailSender`|Host, Port, Username, Password, EnableSsl, FromEmail, FromName|
|MailKit|`MailKitEmailSender`|Host, Port, Username, Password, UseSsl, FromEmail, FromName|
|Graph API|`GraphApiEmailSender`|TenantId, ClientId, ClientSecret, FromEmail, FromName, Scope|
|Exchange|`ExchangeEmailSender`|ServerUrl, Username, Password, Domain, FromEmail, FromName, Version|
|SendGrid|`SendGridEmailSender`|ApiKey, FromEmail, FromName|
|Mailgun|`MailgunEmailSender`|ApiKey, Domain, FromEmail, FromName, Region|
|Amazon SES|`AmazonSesEmailSender`|AccessKeyId, SecretAccessKey, Region, FromEmail, FromName|
|Postmark|`PostmarkEmailSender`|ServerToken, FromEmail, FromName|

## Sending Emails

### `SendEmailAsync(string to, string subject, string body, bool isHtml = false)`
Sends a simple email without attachments.

- **Parameters:**
  - `to` (`string`): Recipient email address.
  - `subject` (`string`): Email subject line.
  - `body` (`string`): Email body content.
  - `isHtml` (`bool`, optional): Set to `true` if the body contains HTML content. Default is `false`.
- **Returns:** `Task` — Completes when the email has been sent. Exceptions may be thrown if sending fails.

---

### `SendEmailAsync(string to, string subject, string body, List<string> attachments, bool isHtml = false)`
Sends an email with one or more file attachments.

- **Parameters:**
  - `to` (`string`): Recipient email address.
  - `subject` (`string`): Email subject line.
  - `body` (`string`): Email body content.
  - `attachments` (`List<string>`): List of file paths to attach to the email.
  - `isHtml` (`bool`, optional): Set to `true` if the body contains HTML content. Default is `false`.
- **Returns:** `Task` — Completes when the email has been sent. Exceptions may be thrown if sending fails or if attachments are invalid.

---

## Notes

- Both methods are asynchronous and should be awaited.
- The `to` parameter can typically be a single email address; if multiple recipients are needed, use a comma-separated string or extend the implementation.
- Attachments must be accessible file paths on the local system.
- Exceptions should be handled to capture sending errors, such as invalid addresses, network issues, or authentication failures.


[Return to Index](../Index.md)
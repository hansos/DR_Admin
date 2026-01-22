# EmailSenderLib `IEmailSender` Interface - User Manual

The `IEmailSender` interface provides asynchronous methods for sending emails, supporting both plain text and HTML content as well as attachments. It is designed for integration into applications that need programmatic email delivery.

---

## Table of Contents

1. [Sending Emails](#sending-emails)
    - Basic Email
    - Email with Attachments

---

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

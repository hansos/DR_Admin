namespace MessagingTemplateLib.Models;

/// <summary>
/// Model for MailTwoFactorCode message templates.
/// </summary>
public class MailTwoFactorCodeModel
{
    /// <summary>
    /// Gets or sets the one-time verification code.
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets code expiration time in minutes.
    /// </summary>
    public string ExpirationMinutes { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the recipient email address.
    /// </summary>
    public string Email { get; set; } = string.Empty;
}

namespace MessagingTemplateLib.Models;

/// <summary>
/// Model for PasswordReset message templates
/// </summary>
public class PasswordResetModel
{
    public string ResetUrl { get; set; } = string.Empty;
    public string ExpirationHours { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

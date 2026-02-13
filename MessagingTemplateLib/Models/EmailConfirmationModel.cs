namespace MessagingTemplateLib.Models;

/// <summary>
/// Model for EmailConfirmation message templates
/// </summary>
public class EmailConfirmationModel
{
    public string ConfirmationUrl { get; set; } = string.Empty;
    public string ExpirationDays { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

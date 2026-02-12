namespace EmailSenderLib.Templating.Models;

/// <summary>
/// Model for DomainExpired message templates
/// </summary>
public class DomainExpiredModel
{
    public string DomainName { get; set; } = string.Empty;
    public string ExpiredAt { get; set; } = string.Empty;
    public string AutoRenewEnabled { get; set; } = string.Empty;
    public string RenewUrl { get; set; } = string.Empty;
}

namespace EmailSenderLib.Templating.Models;

/// <summary>
/// Model for DomainRegistered message templates
/// </summary>
public class DomainRegisteredModel
{
    public string DomainName { get; set; } = string.Empty;
    public string RegistrationDate { get; set; } = string.Empty;
    public string ExpirationDate { get; set; } = string.Empty;
    public string AutoRenew { get; set; } = string.Empty;
    public string CustomerPortalUrl { get; set; } = string.Empty;
}

namespace MessagingTemplateLib.Models;

/// <summary>
/// Model for OrderActivated message templates
/// </summary>
public class OrderActivatedModel
{
    public string OrderNumber { get; set; } = string.Empty;
    public string ServiceName { get; set; } = string.Empty;
    public string ActivatedAt { get; set; } = string.Empty;
    public string CustomerPortalUrl { get; set; } = string.Empty;
}

using ISPAdmin.Data.Enums;

namespace ISPAdmin.Domain.Workflows;

/// <summary>
/// Input parameters for domain registration workflow
/// </summary>
public class DomainRegistrationWorkflowInput
{
    public int CustomerId { get; set; }
    public string DomainName { get; set; } = string.Empty;
    public int RegistrarId { get; set; }
    public int Years { get; set; } = 1;
    public bool AutoRenew { get; set; }
    public bool PrivacyProtection { get; set; }
    public int? ServiceId { get; set; }
    public OrderType OrderType { get; set; } = OrderType.New;
    public string? Notes { get; set; }
}

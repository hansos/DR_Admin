namespace ISPAdmin.Data.Entities;

/// <summary>
/// Represents a sold hosting package instance owned by a customer.
/// </summary>
public class SoldHostingPackage : EntityBase
{
    public int CustomerId { get; set; }
    public int HostingPackageId { get; set; }
    public int? RegisteredDomainId { get; set; }
    public int OrderId { get; set; }
    public int? OrderLineId { get; set; }

    public string Status { get; set; } = "PendingProvisioning";
    public string BillingCycle { get; set; } = "monthly";

    public decimal SetupFee { get; set; }
    public decimal RecurringPrice { get; set; }
    public string CurrencyCode { get; set; } = "EUR";

    public DateTime ActivatedAt { get; set; }
    public DateTime NextBillingDate { get; set; }
    public DateTime? ExpiresAt { get; set; }

    public bool AutoRenew { get; set; } = true;

    public string ConfigurationSnapshotJson { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;

    public Customer Customer { get; set; } = null!;
    public HostingPackage HostingPackage { get; set; } = null!;
    public RegisteredDomain? RegisteredDomain { get; set; }
    public Order Order { get; set; } = null!;
    public OrderLine? OrderLine { get; set; }
}

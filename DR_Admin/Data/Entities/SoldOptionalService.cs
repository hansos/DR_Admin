namespace ISPAdmin.Data.Entities;

/// <summary>
/// Represents a sold optional service instance owned by a customer.
/// </summary>
public class SoldOptionalService : EntityBase
{
    public int CustomerId { get; set; }
    public int ServiceId { get; set; }
    public int? RegisteredDomainId { get; set; }
    public int OrderId { get; set; }
    public int? OrderLineId { get; set; }

    public int Quantity { get; set; } = 1;
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }

    public string Status { get; set; } = "Active";
    public string BillingCycle { get; set; } = "monthly";
    public string CurrencyCode { get; set; } = "EUR";

    public DateTime ActivatedAt { get; set; }
    public DateTime NextBillingDate { get; set; }
    public DateTime? ExpiresAt { get; set; }

    public bool AutoRenew { get; set; } = true;

    public string ConfigurationSnapshotJson { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;

    public Customer Customer { get; set; } = null!;
    public Service Service { get; set; } = null!;
    public RegisteredDomain? RegisteredDomain { get; set; }
    public Order Order { get; set; } = null!;
    public OrderLine? OrderLine { get; set; }
}

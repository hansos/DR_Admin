namespace ISPAdmin.DTOs;

public class SoldHostingPackageDto
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public int HostingPackageId { get; set; }
    public int? RegisteredDomainId { get; set; }
    public string ConnectedDomainName { get; set; } = string.Empty;
    public int OrderId { get; set; }
    public int? OrderLineId { get; set; }

    public string Status { get; set; } = string.Empty;
    public string BillingCycle { get; set; } = string.Empty;

    public decimal SetupFee { get; set; }
    public decimal RecurringPrice { get; set; }
    public string CurrencyCode { get; set; } = "EUR";

    public DateTime ActivatedAt { get; set; }
    public DateTime NextBillingDate { get; set; }
    public DateTime? ExpiresAt { get; set; }

    public bool AutoRenew { get; set; }

    public string ConfigurationSnapshotJson { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateSoldHostingPackageDto
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
}

public class UpdateSoldHostingPackageDto
{
    public int? RegisteredDomainId { get; set; }
    public string? Status { get; set; }
    public string? BillingCycle { get; set; }

    public decimal? SetupFee { get; set; }
    public decimal? RecurringPrice { get; set; }
    public string? CurrencyCode { get; set; }

    public DateTime? ActivatedAt { get; set; }
    public DateTime? NextBillingDate { get; set; }
    public DateTime? ExpiresAt { get; set; }

    public bool? AutoRenew { get; set; }

    public string? ConfigurationSnapshotJson { get; set; }
    public string? Notes { get; set; }
}

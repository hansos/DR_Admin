namespace ISPAdmin.DTOs;

public class SoldOptionalServiceDto
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public int ServiceId { get; set; }
    public int OrderId { get; set; }
    public int? OrderLineId { get; set; }

    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }

    public string Status { get; set; } = string.Empty;
    public string BillingCycle { get; set; } = string.Empty;
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

public class CreateSoldOptionalServiceDto
{
    public int CustomerId { get; set; }
    public int ServiceId { get; set; }
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
}

public class UpdateSoldOptionalServiceDto
{
    public int? Quantity { get; set; }
    public decimal? UnitPrice { get; set; }
    public decimal? TotalPrice { get; set; }

    public string? Status { get; set; }
    public string? BillingCycle { get; set; }
    public string? CurrencyCode { get; set; }

    public DateTime? ActivatedAt { get; set; }
    public DateTime? NextBillingDate { get; set; }
    public DateTime? ExpiresAt { get; set; }

    public bool? AutoRenew { get; set; }

    public string? ConfigurationSnapshotJson { get; set; }
    public string? Notes { get; set; }
}

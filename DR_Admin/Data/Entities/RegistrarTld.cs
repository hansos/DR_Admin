namespace ISPAdmin.Data.Entities;

/// <summary>
/// Represents the relationship between a Registrar and a TLD.
/// Pricing information has been moved to RegistrarTldCostPricing (temporal cost pricing) and TldSalesPricing (temporal sales pricing).
/// </summary>
public class RegistrarTld : EntityBase
{
    public int RegistrarId { get; set; }
    public int TldId { get; set; }
    public bool IsActive { get; set; }
    public bool AutoRenew { get; set; }
    public int? MinRegistrationYears { get; set; }
    public int? MaxRegistrationYears { get; set; }
    public string? Notes { get; set; }

    public Registrar Registrar { get; set; } = null!;
    public Tld Tld { get; set; } = null!;
    public ICollection<RegisteredDomain> RegisteredDomains { get; set; } = new List<RegisteredDomain>();
    public ICollection<RegistrarTldCostPricing> CostPricingHistory { get; set; } = new List<RegistrarTldCostPricing>();
}

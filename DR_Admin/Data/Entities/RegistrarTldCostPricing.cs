namespace ISPAdmin.Data.Entities;

/// <summary>
/// Represents temporal pricing for what a registrar charges for a specific TLD
/// Supports historical tracking and future price scheduling
/// </summary>
public class RegistrarTldCostPricing : EntityBase
{
    /// <summary>
    /// Foreign key to the RegistrarTld relationship
    /// </summary>
    public int RegistrarTldId { get; set; }

    /// <summary>
    /// The date and time when this cost pricing becomes effective (UTC)
    /// </summary>
    public DateTime EffectiveFrom { get; set; }

    /// <summary>
    /// The date and time when this cost pricing expires (UTC)
    /// NULL indicates this is the current/future active pricing
    /// </summary>
    public DateTime? EffectiveTo { get; set; }

    /// <summary>
    /// Cost for domain registration (what registrar charges YOU)
    /// </summary>
    public decimal RegistrationCost { get; set; }

    /// <summary>
    /// Cost for domain renewal (what registrar charges YOU)
    /// </summary>
    public decimal RenewalCost { get; set; }

    /// <summary>
    /// Cost for domain transfer (what registrar charges YOU)
    /// </summary>
    public decimal TransferCost { get; set; }

    /// <summary>
    /// Cost for privacy/WHOIS protection (what registrar charges YOU)
    /// </summary>
    public decimal? PrivacyCost { get; set; }

    /// <summary>
    /// Special promotional cost for first-year registration
    /// Used when registrar offers discounted first-year pricing
    /// </summary>
    public decimal? FirstYearRegistrationCost { get; set; }

    /// <summary>
    /// Currency code for these costs (ISO 4217, e.g., "USD", "EUR")
    /// Each registrar may have different currencies
    /// </summary>
    public string Currency { get; set; } = "USD";

    /// <summary>
    /// Indicates if this pricing is still active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Optional notes about this pricing change
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// User or system that created this pricing record
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Navigation property to the RegistrarTld relationship
    /// </summary>
    public RegistrarTld RegistrarTld { get; set; } = null!;
}

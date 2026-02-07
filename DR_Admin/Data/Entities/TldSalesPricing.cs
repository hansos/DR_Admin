namespace ISPAdmin.Data.Entities;

/// <summary>
/// Represents temporal pricing for what YOU charge customers for a specific TLD
/// Linked to TLD (not RegistrarTld) - one price per TLD across all registrars
/// Supports historical tracking and future price scheduling
/// </summary>
public class TldSalesPricing : EntityBase
{
    /// <summary>
    /// Foreign key to the TLD
    /// Linked to TLD (NOT RegistrarTld) - customers see one price per TLD
    /// </summary>
    public int TldId { get; set; }

    /// <summary>
    /// The date and time when this sales pricing becomes effective (UTC)
    /// </summary>
    public DateTime EffectiveFrom { get; set; }

    /// <summary>
    /// The date and time when this sales pricing expires (UTC)
    /// NULL indicates this is the current/future active pricing
    /// </summary>
    public DateTime? EffectiveTo { get; set; }

    /// <summary>
    /// Price for domain registration (what YOU charge customers)
    /// </summary>
    public decimal RegistrationPrice { get; set; }

    /// <summary>
    /// Price for domain renewal (what YOU charge customers)
    /// </summary>
    public decimal RenewalPrice { get; set; }

    /// <summary>
    /// Price for domain transfer (what YOU charge customers)
    /// </summary>
    public decimal TransferPrice { get; set; }

    /// <summary>
    /// Price for privacy/WHOIS protection (what YOU charge customers)
    /// </summary>
    public decimal? PrivacyPrice { get; set; }

    /// <summary>
    /// Special promotional price for first-year registration
    /// Used for promotions (e.g., "First year only $4.99")
    /// </summary>
    public decimal? FirstYearRegistrationPrice { get; set; }

    /// <summary>
    /// Currency code for these prices (ISO 4217, e.g., "USD", "EUR")
    /// </summary>
    public string Currency { get; set; } = "USD";

    /// <summary>
    /// Indicates if this is a promotional pricing
    /// </summary>
    public bool IsPromotional { get; set; } = false;

    /// <summary>
    /// Name of the promotion (e.g., "Black Friday 2025", "New Year Sale")
    /// </summary>
    public string? PromotionName { get; set; }

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
    /// Navigation property to the TLD
    /// </summary>
    public Tld Tld { get; set; } = null!;
}

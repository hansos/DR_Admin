namespace ISPAdmin.Data.Entities;

/// <summary>
/// Represents customer-specific discounts for TLDs linked to ResellerCompany
/// Supports temporal discounts with start/end dates
/// </summary>
public class ResellerTldDiscount : EntityBase
{
    /// <summary>
    /// Foreign key to the ResellerCompany receiving the discount
    /// </summary>
    public int ResellerCompanyId { get; set; }

    /// <summary>
    /// Foreign key to the TLD for which discount applies
    /// </summary>
    public int TldId { get; set; }

    /// <summary>
    /// The date and time when this discount becomes effective (UTC)
    /// </summary>
    public DateTime EffectiveFrom { get; set; }

    /// <summary>
    /// The date and time when this discount expires (UTC)
    /// NULL indicates this discount has no expiry
    /// </summary>
    public DateTime? EffectiveTo { get; set; }

    /// <summary>
    /// Discount percentage (e.g., 10.5 for 10.5% off)
    /// Use either DiscountPercentage OR DiscountAmount, not both
    /// </summary>
    public decimal? DiscountPercentage { get; set; }

    /// <summary>
    /// Fixed discount amount (e.g., 2.00 for $2.00 off)
    /// Use either DiscountPercentage OR DiscountAmount, not both
    /// </summary>
    public decimal? DiscountAmount { get; set; }

    /// <summary>
    /// Currency code for DiscountAmount (ISO 4217, e.g., "USD", "EUR")
    /// Only relevant if DiscountAmount is used
    /// </summary>
    public string? DiscountCurrency { get; set; }

    /// <summary>
    /// Whether this discount applies to domain registration
    /// </summary>
    public bool ApplyToRegistration { get; set; } = true;

    /// <summary>
    /// Whether this discount applies to domain renewal
    /// </summary>
    public bool ApplyToRenewal { get; set; } = true;

    /// <summary>
    /// Whether this discount applies to domain transfer
    /// </summary>
    public bool ApplyToTransfer { get; set; } = false;

    /// <summary>
    /// Indicates if this discount is still active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Optional notes about this discount
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// User or system that created this discount record
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Navigation property to the ResellerCompany
    /// </summary>
    public ResellerCompany ResellerCompany { get; set; } = null!;

    /// <summary>
    /// Navigation property to the TLD
    /// </summary>
    public Tld Tld { get; set; } = null!;
}

namespace ISPAdmin.Data.Entities;

/// <summary>
/// Represents a tax rule for calculating taxes based on customer location
/// </summary>
public class TaxRule : EntityBase
{
    /// <summary>
    /// Country code (ISO 3166-1 alpha-2, e.g., "US", "GB", "DE")
    /// </summary>
    public string CountryCode { get; set; } = string.Empty;

    /// <summary>
    /// State or province code (e.g., "CA" for California, optional)
    /// </summary>
    public string? StateCode { get; set; }

    /// <summary>
    /// Display name for the tax (e.g., "VAT", "GST", "Sales Tax")
    /// </summary>
    public string TaxName { get; set; } = string.Empty;

    /// <summary>
    /// Tax rate as a decimal (e.g., 0.21 for 21%)
    /// </summary>
    public decimal TaxRate { get; set; }

    /// <summary>
    /// Whether this tax rule is currently active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Date when this tax rule becomes effective
    /// </summary>
    public DateTime EffectiveFrom { get; set; }

    /// <summary>
    /// Date when this tax rule expires (null = no expiration)
    /// </summary>
    public DateTime? EffectiveUntil { get; set; }

    /// <summary>
    /// Whether this tax applies to setup fees
    /// </summary>
    public bool AppliesToSetupFees { get; set; } = true;

    /// <summary>
    /// Whether this tax applies to recurring charges
    /// </summary>
    public bool AppliesToRecurring { get; set; } = true;

    /// <summary>
    /// Whether reverse charge applies for B2B transactions (EU VAT)
    /// </summary>
    public bool ReverseCharge { get; set; }

    /// <summary>
    /// Tax jurisdiction or authority
    /// </summary>
    public string TaxAuthority { get; set; } = string.Empty;

    /// <summary>
    /// Tax registration number for this jurisdiction
    /// </summary>
    public string TaxRegistrationNumber { get; set; } = string.Empty;

    /// <summary>
    /// Priority for applying multiple tax rules (higher = higher priority)
    /// </summary>
    public int Priority { get; set; } = 0;

    /// <summary>
    /// Internal notes about this tax rule
    /// </summary>
    public string InternalNotes { get; set; } = string.Empty;

    /// <summary>
    /// Soft delete timestamp
    /// </summary>
    public DateTime? DeletedAt { get; set; }
}

namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object for creating a tax rule
/// </summary>
public class CreateTaxRuleDto
{
    /// <summary>
    /// Country code (ISO 3166-1 alpha-2)
    /// </summary>
    public string CountryCode { get; set; } = string.Empty;

    /// <summary>
    /// State or province code (optional)
    /// </summary>
    public string? StateCode { get; set; }

    /// <summary>
    /// Tax name (e.g., "VAT", "GST")
    /// </summary>
    public string TaxName { get; set; } = string.Empty;

    /// <summary>
    /// Tax rate as decimal (e.g., 0.21 for 21%)
    /// </summary>
    public decimal TaxRate { get; set; }

    /// <summary>
    /// Is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Effective from date
    /// </summary>
    public DateTime EffectiveFrom { get; set; }

    /// <summary>
    /// Effective until date (optional)
    /// </summary>
    public DateTime? EffectiveUntil { get; set; }

    /// <summary>
    /// Applies to setup fees
    /// </summary>
    public bool AppliesToSetupFees { get; set; } = true;

    /// <summary>
    /// Applies to recurring charges
    /// </summary>
    public bool AppliesToRecurring { get; set; } = true;

    /// <summary>
    /// Reverse charge for B2B transactions
    /// </summary>
    public bool ReverseCharge { get; set; }

    /// <summary>
    /// Tax authority
    /// </summary>
    public string TaxAuthority { get; set; } = string.Empty;

    /// <summary>
    /// Tax registration number
    /// </summary>
    public string TaxRegistrationNumber { get; set; } = string.Empty;

    /// <summary>
    /// Priority (higher = higher priority)
    /// </summary>
    public int Priority { get; set; } = 0;

    /// <summary>
    /// Internal notes
    /// </summary>
    public string InternalNotes { get; set; } = string.Empty;
}

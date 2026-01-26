namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object representing a tax rule
/// </summary>
public class TaxRuleDto
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Country code
    /// </summary>
    public string CountryCode { get; set; } = string.Empty;

    /// <summary>
    /// State code
    /// </summary>
    public string? StateCode { get; set; }

    /// <summary>
    /// Tax name
    /// </summary>
    public string TaxName { get; set; } = string.Empty;

    /// <summary>
    /// Tax rate
    /// </summary>
    public decimal TaxRate { get; set; }

    /// <summary>
    /// Is active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Effective from date
    /// </summary>
    public DateTime EffectiveFrom { get; set; }

    /// <summary>
    /// Effective until date
    /// </summary>
    public DateTime? EffectiveUntil { get; set; }

    /// <summary>
    /// Applies to setup fees
    /// </summary>
    public bool AppliesToSetupFees { get; set; }

    /// <summary>
    /// Applies to recurring charges
    /// </summary>
    public bool AppliesToRecurring { get; set; }

    /// <summary>
    /// Reverse charge for B2B
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
    /// Priority
    /// </summary>
    public int Priority { get; set; }

    /// <summary>
    /// Date created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

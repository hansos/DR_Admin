namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object for updating a tax rule
/// </summary>
public class UpdateTaxRuleDto
{
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
    /// Reverse charge
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
    /// Internal notes
    /// </summary>
    public string InternalNotes { get; set; } = string.Empty;
}

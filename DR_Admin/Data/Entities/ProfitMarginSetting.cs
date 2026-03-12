using ISPAdmin.Data.Enums;

namespace ISPAdmin.Data.Entities;

/// <summary>
/// Stores default profit percentage for a specific product class.
/// </summary>
public class ProfitMarginSetting : EntityBase
{
    /// <summary>
    /// Product class this margin applies to.
    /// </summary>
    public ProfitProductClass ProductClass { get; set; }

    /// <summary>
    /// Profit percentage for the product class.
    /// </summary>
    public decimal ProfitPercent { get; set; }

    /// <summary>
    /// Whether this setting is active.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Optional notes.
    /// </summary>
    public string? Notes { get; set; }
}

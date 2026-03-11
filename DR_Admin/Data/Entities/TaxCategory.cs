namespace ISPAdmin.Data.Entities;

/// <summary>
/// Defines a tax category scoped by country and optional state.
/// </summary>
public class TaxCategory : EntityBase
{
    /// <summary>
    /// Tax category code (e.g., STANDARD, REDUCED, EXEMPT).
    /// </summary>
    public string Code { get; set; } = "STANDARD";

    /// <summary>
    /// Display name for the tax category.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// ISO 3166-1 alpha-2 country code.
    /// </summary>
    public string CountryCode { get; set; } = string.Empty;

    /// <summary>
    /// Optional state/province code.
    /// </summary>
    public string? StateCode { get; set; }

    /// <summary>
    /// Optional description.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Indicates whether this category is active.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Tax rules linked to this category.
    /// </summary>
    public ICollection<TaxRule> TaxRules { get; set; } = new List<TaxRule>();
}

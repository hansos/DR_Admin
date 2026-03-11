namespace ISPAdmin.Data.Entities;

/// <summary>
/// Defines a tax jurisdiction used to determine and report VAT/TAX rules.
/// </summary>
public class TaxJurisdiction : EntityBase
{
    /// <summary>
    /// Internal jurisdiction code (e.g., "EU-DE", "US-CA").
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Display name of the jurisdiction.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// ISO 3166-1 alpha-2 country code.
    /// </summary>
    public string CountryCode { get; set; } = string.Empty;

    /// <summary>
    /// Optional state/province code for sub-national tax regions.
    /// </summary>
    public string? StateCode { get; set; }

    /// <summary>
    /// Tax authority name (e.g., "Skatteverket", "California CDTFA").
    /// </summary>
    public string TaxAuthority { get; set; } = string.Empty;

    /// <summary>
    /// Currency code used by the jurisdiction for tax reporting.
    /// </summary>
    public string TaxCurrencyCode { get; set; } = "EUR";

    /// <summary>
    /// Indicates if this jurisdiction is active for tax determination.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Additional internal notes.
    /// </summary>
    public string Notes { get; set; } = string.Empty;

    /// <summary>
    /// Tax rules configured for this jurisdiction.
    /// </summary>
    public ICollection<TaxRule> TaxRules { get; set; } = new List<TaxRule>();

    /// <summary>
    /// Seller tax registrations associated with this jurisdiction.
    /// </summary>
    public ICollection<TaxRegistration> TaxRegistrations { get; set; } = new List<TaxRegistration>();
}

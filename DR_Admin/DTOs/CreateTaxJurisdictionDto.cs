namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object for creating a tax jurisdiction.
/// </summary>
public class CreateTaxJurisdictionDto
{
    /// <summary>
    /// Gets or sets the internal jurisdiction code.
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the display name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the ISO country code.
    /// </summary>
    public string CountryCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the optional state or province code.
    /// </summary>
    public string? StateCode { get; set; }

    /// <summary>
    /// Gets or sets the tax authority name.
    /// </summary>
    public string TaxAuthority { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the tax reporting currency code.
    /// </summary>
    public string TaxCurrencyCode { get; set; } = "EUR";

    /// <summary>
    /// Gets or sets whether the jurisdiction is active.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets internal notes.
    /// </summary>
    public string Notes { get; set; } = string.Empty;
}

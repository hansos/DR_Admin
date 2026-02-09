using ISPAdmin.Data.Enums;

namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object for creating vendor tax profiles
/// </summary>
public class CreateVendorTaxProfileDto
{
    /// <summary>
    /// Gets or sets the vendor ID
    /// </summary>
    public int VendorId { get; set; }

    /// <summary>
    /// Gets or sets the type of vendor
    /// </summary>
    public VendorType VendorType { get; set; }

    /// <summary>
    /// Gets or sets the vendor's tax identification number
    /// </summary>
    public string? TaxIdNumber { get; set; }

    /// <summary>
    /// Gets or sets the ISO 2-letter country code of vendor's tax residence
    /// </summary>
    public string TaxResidenceCountry { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether vendor requires IRS Form 1099
    /// </summary>
    public bool Require1099 { get; set; } = false;

    /// <summary>
    /// Gets or sets whether vendor's W-9 form is on file
    /// </summary>
    public bool W9OnFile { get; set; } = false;

    /// <summary>
    /// Gets or sets the URL or path to W-9 form document
    /// </summary>
    public string? W9FileUrl { get; set; }

    /// <summary>
    /// Gets or sets the withholding tax rate (0-1, e.g., 0.30 for 30%)
    /// </summary>
    public decimal? WithholdingTaxRate { get; set; }

    /// <summary>
    /// Gets or sets whether vendor is exempt from withholding tax due to treaty
    /// </summary>
    public bool TaxTreatyExempt { get; set; } = false;

    /// <summary>
    /// Gets or sets the country code for applicable tax treaty
    /// </summary>
    public string? TaxTreatyCountry { get; set; }

    /// <summary>
    /// Gets or sets additional tax-related notes
    /// </summary>
    public string TaxNotes { get; set; } = string.Empty;
}

using ISPAdmin.Data.Enums;

namespace ISPAdmin.Data.Entities;

/// <summary>
/// Stores vendor tax information for compliance and reporting
/// </summary>
public class VendorTaxProfile : EntityBase
{
    /// <summary>
    /// Generic vendor identifier
    /// </summary>
    public int VendorId { get; set; }

    /// <summary>
    /// Type of vendor
    /// </summary>
    public VendorType VendorType { get; set; }

    /// <summary>
    /// Vendor's tax identification number
    /// </summary>
    public string? TaxIdNumber { get; set; }

    /// <summary>
    /// ISO 2-letter country code of vendor's tax residence
    /// </summary>
    public string TaxResidenceCountry { get; set; } = string.Empty;

    /// <summary>
    /// Indicates if vendor requires IRS Form 1099 (US vendors)
    /// </summary>
    public bool Require1099 { get; set; } = false;

    /// <summary>
    /// Indicates if vendor's W-9 form is on file
    /// </summary>
    public bool W9OnFile { get; set; } = false;

    /// <summary>
    /// URL or path to W-9 form document
    /// </summary>
    public string? W9FileUrl { get; set; }

    /// <summary>
    /// Withholding tax rate (0-1, e.g., 0.30 for 30%)
    /// </summary>
    public decimal? WithholdingTaxRate { get; set; }

    /// <summary>
    /// Indicates if vendor is exempt from withholding tax due to treaty
    /// </summary>
    public bool TaxTreatyExempt { get; set; } = false;

    /// <summary>
    /// Country code for applicable tax treaty
    /// </summary>
    public string? TaxTreatyCountry { get; set; }

    /// <summary>
    /// Additional tax-related notes
    /// </summary>
    public string TaxNotes { get; set; } = string.Empty;
}

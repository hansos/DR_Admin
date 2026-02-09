using ISPAdmin.Data.Enums;

namespace ISPAdmin.Data.Entities;

/// <summary>
/// Stores customer tax identification and validation information
/// </summary>
public class CustomerTaxProfile : EntityBase
{
    /// <summary>
    /// Foreign key to the customer
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// Tax identification number (VAT ID, EIN, etc.)
    /// </summary>
    public string? TaxIdNumber { get; set; }

    /// <summary>
    /// Type of tax identification number
    /// </summary>
    public TaxIdType TaxIdType { get; set; } = TaxIdType.None;

    /// <summary>
    /// Indicates if the tax ID has been validated with tax authority
    /// </summary>
    public bool TaxIdValidated { get; set; } = false;

    /// <summary>
    /// Date when the tax ID was last validated
    /// </summary>
    public DateTime? TaxIdValidationDate { get; set; }

    /// <summary>
    /// Raw response from tax validation service (JSON format)
    /// </summary>
    public string TaxIdValidationResponse { get; set; } = string.Empty;

    /// <summary>
    /// ISO 2-letter country code of tax residence
    /// </summary>
    public string TaxResidenceCountry { get; set; } = string.Empty;

    /// <summary>
    /// Customer type for tax calculation purposes
    /// </summary>
    public CustomerType CustomerType { get; set; } = CustomerType.B2C;

    /// <summary>
    /// Indicates if customer is tax exempt
    /// </summary>
    public bool TaxExempt { get; set; } = false;

    /// <summary>
    /// Reason for tax exemption
    /// </summary>
    public string? TaxExemptionReason { get; set; }

    /// <summary>
    /// URL or path to tax exemption certificate
    /// </summary>
    public string? TaxExemptionCertificateUrl { get; set; }

    /// <summary>
    /// Navigation property to the customer
    /// </summary>
    public Customer Customer { get; set; } = null!;
}

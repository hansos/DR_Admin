using ISPAdmin.Data.Enums;

namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object representing customer tax identification and validation information
/// </summary>
public class CustomerTaxProfileDto
{
    /// <summary>
    /// Gets or sets the unique identifier
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the customer ID
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// Gets or sets the tax identification number (VAT ID, EIN, etc.)
    /// </summary>
    public string? TaxIdNumber { get; set; }

    /// <summary>
    /// Gets or sets the type of tax identification number
    /// </summary>
    public TaxIdType TaxIdType { get; set; }

    /// <summary>
    /// Gets or sets whether the tax ID has been validated
    /// </summary>
    public bool TaxIdValidated { get; set; }

    /// <summary>
    /// Gets or sets the date when the tax ID was last validated
    /// </summary>
    public DateTime? TaxIdValidationDate { get; set; }

    /// <summary>
    /// Gets or sets the ISO 2-letter country code of tax residence
    /// </summary>
    public string TaxResidenceCountry { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the customer type for tax calculation purposes
    /// </summary>
    public CustomerType CustomerType { get; set; }

    /// <summary>
    /// Gets or sets whether the customer is tax exempt
    /// </summary>
    public bool TaxExempt { get; set; }

    /// <summary>
    /// Gets or sets the reason for tax exemption
    /// </summary>
    public string? TaxExemptionReason { get; set; }

    /// <summary>
    /// Gets or sets the URL or path to tax exemption certificate
    /// </summary>
    public string? TaxExemptionCertificateUrl { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the record was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the record was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

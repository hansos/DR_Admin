using ISPAdmin.Data.Enums;

namespace ISPAdmin.Data.Entities;

/// <summary>
/// Stores normalized tax determination evidence used for compliance and audit.
/// </summary>
public class TaxDeterminationEvidence : EntityBase
{
    /// <summary>
    /// Optional customer identifier.
    /// </summary>
    public int? CustomerId { get; set; }

    /// <summary>
    /// Optional order identifier.
    /// </summary>
    public int? OrderId { get; set; }

    /// <summary>
    /// Buyer country code used for tax determination.
    /// </summary>
    public string BuyerCountryCode { get; set; } = string.Empty;

    /// <summary>
    /// Optional buyer state code used for tax determination.
    /// </summary>
    public string? BuyerStateCode { get; set; }

    /// <summary>
    /// Billing country evidence value.
    /// </summary>
    public string BillingCountryCode { get; set; } = string.Empty;

    /// <summary>
    /// IP address used as location evidence.
    /// </summary>
    public string IpAddress { get; set; } = string.Empty;

    /// <summary>
    /// Buyer tax identifier submitted for validation.
    /// </summary>
    public string BuyerTaxId { get; set; } = string.Empty;

    /// <summary>
    /// Indicates whether buyer tax ID was validated.
    /// </summary>
    public bool BuyerTaxIdValidated { get; set; }

    /// <summary>
    /// Validation provider that produced validation output.
    /// </summary>
    public string VatValidationProvider { get; set; } = string.Empty;

    /// <summary>
    /// Validation provider raw response.
    /// </summary>
    public string VatValidationRawResponse { get; set; } = string.Empty;

    /// <summary>
    /// Source of exchange rate used.
    /// </summary>
    public CurrencyRateSource? ExchangeRateSource { get; set; }

    /// <summary>
    /// Timestamp of evidence capture.
    /// </summary>
    public DateTime CapturedAt { get; set; } = DateTime.UtcNow;
}

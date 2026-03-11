using ISPAdmin.Data.Enums;

namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object representing an immutable order tax snapshot.
/// </summary>
public class OrderTaxSnapshotDto
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the order identifier.
    /// </summary>
    public int OrderId { get; set; }

    /// <summary>
    /// Gets or sets the optional tax jurisdiction identifier.
    /// </summary>
    public int? TaxJurisdictionId { get; set; }

    /// <summary>
    /// Gets or sets buyer country code.
    /// </summary>
    public string BuyerCountryCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets optional buyer state code.
    /// </summary>
    public string? BuyerStateCode { get; set; }

    /// <summary>
    /// Gets or sets buyer type.
    /// </summary>
    public CustomerType BuyerType { get; set; }

    /// <summary>
    /// Gets or sets buyer tax identifier.
    /// </summary>
    public string BuyerTaxId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether buyer tax ID was validated.
    /// </summary>
    public bool BuyerTaxIdValidated { get; set; }

    /// <summary>
    /// Gets or sets tax currency code.
    /// </summary>
    public string TaxCurrencyCode { get; set; } = "EUR";

    /// <summary>
    /// Gets or sets display currency code.
    /// </summary>
    public string DisplayCurrencyCode { get; set; } = "EUR";

    /// <summary>
    /// Gets or sets exchange rate.
    /// </summary>
    public decimal? ExchangeRate { get; set; }

    /// <summary>
    /// Gets or sets exchange rate timestamp.
    /// </summary>
    public DateTime? ExchangeRateDate { get; set; }

    /// <summary>
    /// Gets or sets exchange rate source.
    /// </summary>
    public CurrencyRateSource? ExchangeRateSource { get; set; }

    /// <summary>
    /// Gets or sets optional tax determination evidence identifier.
    /// </summary>
    public int? TaxDeterminationEvidenceId { get; set; }

    /// <summary>
    /// Gets or sets net amount.
    /// </summary>
    public decimal NetAmount { get; set; }

    /// <summary>
    /// Gets or sets tax amount.
    /// </summary>
    public decimal TaxAmount { get; set; }

    /// <summary>
    /// Gets or sets gross amount.
    /// </summary>
    public decimal GrossAmount { get; set; }

    /// <summary>
    /// Gets or sets applied tax rate.
    /// </summary>
    public decimal AppliedTaxRate { get; set; }

    /// <summary>
    /// Gets or sets applied tax name.
    /// </summary>
    public string AppliedTaxName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether reverse charge was applied.
    /// </summary>
    public bool ReverseChargeApplied { get; set; }

    /// <summary>
    /// Gets or sets applied rule version.
    /// </summary>
    public string RuleVersion { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets optional idempotency key used for finalize operations.
    /// </summary>
    public string? IdempotencyKey { get; set; }

    /// <summary>
    /// Gets or sets serialized calculation inputs.
    /// </summary>
    public string CalculationInputsJson { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets creation timestamp.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets last update timestamp.
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

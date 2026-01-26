using ISPAdmin.Data.Enums;

namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object for currency exchange rate
/// </summary>
public class CurrencyExchangeRateDto
{
    /// <summary>
    /// Unique identifier for the exchange rate
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The base currency code (ISO 4217, e.g., "EUR")
    /// </summary>
    public string BaseCurrency { get; set; } = "EUR";

    /// <summary>
    /// The target currency code (ISO 4217, e.g., "USD", "GBP")
    /// </summary>
    public string TargetCurrency { get; set; } = string.Empty;

    /// <summary>
    /// The exchange rate from base currency to target currency
    /// </summary>
    public decimal Rate { get; set; }

    /// <summary>
    /// The date and time when this exchange rate becomes effective
    /// </summary>
    public DateTime EffectiveDate { get; set; }

    /// <summary>
    /// The date and time when this exchange rate expires (null if still active)
    /// </summary>
    public DateTime? ExpiryDate { get; set; }

    /// <summary>
    /// The source of this exchange rate
    /// </summary>
    public CurrencyRateSource Source { get; set; }

    /// <summary>
    /// Whether this exchange rate is currently active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Optional markup/margin percentage applied to the base rate
    /// </summary>
    public decimal Markup { get; set; }

    /// <summary>
    /// The final rate including markup
    /// </summary>
    public decimal EffectiveRate { get; set; }

    /// <summary>
    /// Optional notes about this exchange rate
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Date and time when the record was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date and time when the record was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

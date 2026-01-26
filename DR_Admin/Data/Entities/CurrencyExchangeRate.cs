using ISPAdmin.Data.Enums;

namespace ISPAdmin.Data.Entities;

/// <summary>
/// Represents an exchange rate between two currencies at a specific point in time
/// </summary>
public class CurrencyExchangeRate : EntityBase
{
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
    /// Example: If BaseCurrency is EUR and TargetCurrency is USD, a rate of 1.10 means 1 EUR = 1.10 USD
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
    /// The source of this exchange rate (e.g., ECB, Manual, OpenExchangeRates)
    /// </summary>
    public CurrencyRateSource Source { get; set; } = CurrencyRateSource.Manual;

    /// <summary>
    /// Whether this exchange rate is currently active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Optional markup/margin percentage applied to the base rate (for profit margin)
    /// Example: 2.5 means a 2.5% markup is applied
    /// </summary>
    public decimal Markup { get; set; } = 0m;

    /// <summary>
    /// The final rate including markup (Rate * (1 + Markup/100))
    /// </summary>
    public decimal EffectiveRate { get; set; }

    /// <summary>
    /// Optional notes about this exchange rate
    /// </summary>
    public string? Notes { get; set; }
}

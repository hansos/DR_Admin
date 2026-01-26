namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object for currency conversion request
/// </summary>
public class ConvertCurrencyDto
{
    /// <summary>
    /// The amount to convert
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// The source currency code (ISO 4217, e.g., "EUR")
    /// </summary>
    public string FromCurrency { get; set; } = string.Empty;

    /// <summary>
    /// The target currency code (ISO 4217, e.g., "USD")
    /// </summary>
    public string ToCurrency { get; set; } = string.Empty;

    /// <summary>
    /// Optional specific date for the exchange rate (defaults to current date)
    /// </summary>
    public DateTime? RateDate { get; set; }
}

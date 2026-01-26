namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object for currency conversion result
/// </summary>
public class CurrencyConversionResultDto
{
    /// <summary>
    /// The original amount before conversion
    /// </summary>
    public decimal OriginalAmount { get; set; }

    /// <summary>
    /// The source currency code
    /// </summary>
    public string FromCurrency { get; set; } = string.Empty;

    /// <summary>
    /// The target currency code
    /// </summary>
    public string ToCurrency { get; set; } = string.Empty;

    /// <summary>
    /// The exchange rate used for conversion
    /// </summary>
    public decimal ExchangeRate { get; set; }

    /// <summary>
    /// The converted amount in the target currency
    /// </summary>
    public decimal ConvertedAmount { get; set; }

    /// <summary>
    /// The date and time of the exchange rate used
    /// </summary>
    public DateTime RateDate { get; set; }
}

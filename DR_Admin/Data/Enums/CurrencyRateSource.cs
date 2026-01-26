namespace ISPAdmin.Data.Enums;

/// <summary>
/// Represents the source of a currency exchange rate
/// </summary>
public enum CurrencyRateSource
{
    /// <summary>
    /// Exchange rate entered manually by an administrator
    /// </summary>
    Manual = 0,

    /// <summary>
    /// Exchange rate from European Central Bank
    /// </summary>
    ECB = 1,

    /// <summary>
    /// Exchange rate from Open Exchange Rates API
    /// </summary>
    OpenExchangeRates = 2,

    /// <summary>
    /// Exchange rate from Fixer.io API
    /// </summary>
    Fixer = 3,

    /// <summary>
    /// Exchange rate from CurrencyLayer API
    /// </summary>
    CurrencyLayer = 4,

    /// <summary>
    /// Exchange rate from XE.com API
    /// </summary>
    XE = 5,

    /// <summary>
    /// Exchange rate from a custom/other source
    /// </summary>
    Other = 99
}

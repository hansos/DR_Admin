namespace ISPAdmin.Infrastructure.Settings;

/// <summary>
/// Configuration settings for TLD pricing management
/// Controls retention policies, margin alerts, and pricing features
/// </summary>
public class TldPricingSettings
{
    /// <summary>
    /// Configuration section name in appsettings.json
    /// </summary>
    public const string SectionName = "TldPricing";

    /// <summary>
    /// Number of years to retain cost pricing history for compliance (default: 7 years)
    /// </summary>
    public int CostPricingRetentionYears { get; set; } = 7;

    /// <summary>
    /// Number of years to retain sales pricing history (default: 5 years)
    /// </summary>
    public int SalesPricingRetentionYears { get; set; } = 5;

    /// <summary>
    /// Number of years to retain discount history (default: 5 years)
    /// </summary>
    public int DiscountHistoryRetentionYears { get; set; } = 5;

    /// <summary>
    /// Minimum acceptable margin percentage before low margin alerts are triggered (default: 5.0%)
    /// </summary>
    public decimal MinimumMarginPercentage { get; set; } = 5.0m;

    /// <summary>
    /// Whether to send alerts when margins are negative (cost > price)
    /// </summary>
    public bool EnableNegativeMarginAlerts { get; set; } = true;

    /// <summary>
    /// Whether to send alerts when margins are below minimum threshold
    /// </summary>
    public bool EnableLowMarginAlerts { get; set; } = true;

    /// <summary>
    /// Comma-separated email addresses to receive margin alerts
    /// </summary>
    public string MarginAlertEmails { get; set; } = string.Empty;

    /// <summary>
    /// Whether future prices can be edited before their EffectiveFrom date
    /// </summary>
    public bool AllowEditingFuturePrices { get; set; } = true;

    /// <summary>
    /// Maximum number of days in advance that prices can be scheduled (default: 365 days)
    /// </summary>
    public int MaxScheduleDays { get; set; } = 365;

    /// <summary>
    /// Default currency for sales pricing (ISO 4217 code, e.g., "USD", "EUR")
    /// </summary>
    public string DefaultSalesCurrency { get; set; } = "USD";

    /// <summary>
    /// Whether to automatically convert currencies using exchange rates
    /// </summary>
    public bool EnableAutoCurrencyConversion { get; set; } = true;

    /// <summary>
    /// Additional markup percentage to apply on currency conversions (default: 0%)
    /// </summary>
    public decimal CurrencyConversionMarkup { get; set; } = 0.0m;

    /// <summary>
    /// Number of minutes to cache current pricing queries (default: 60 minutes)
    /// </summary>
    public int CurrentPriceCacheMinutes { get; set; } = 60;

    /// <summary>
    /// Whether to allow discount stacking with promotional prices
    /// Business rule: typically set to false (no stacking)
    /// </summary>
    public bool AllowDiscountStacking { get; set; } = false;

    /// <summary>
    /// Whether to automatically archive old pricing data on a schedule
    /// </summary>
    public bool EnableAutoArchiving { get; set; } = true;

    /// <summary>
    /// Hour of day (0-23) when auto-archiving runs if enabled (default: 2 AM)
    /// </summary>
    public int AutoArchivingHour { get; set; } = 2;
}

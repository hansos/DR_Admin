using ISPAdmin.Infrastructure.Settings;
using Microsoft.Extensions.Options;

namespace ISPAdmin.Services.Helpers;

/// <summary>
/// Helper class for validating profit margins and generating alerts
/// </summary>
public class MarginValidator
{
    private readonly TldPricingSettings _settings;

    public MarginValidator(IOptions<TldPricingSettings> settings)
    {
        _settings = settings.Value;
    }

    /// <summary>
    /// Validates margin and returns result with alert flags
    /// </summary>
    /// <param name="cost">Registrar cost</param>
    /// <param name="price">Customer sales price</param>
    /// <param name="costCurrency">Currency of the cost</param>
    /// <param name="priceCurrency">Currency of the price</param>
    /// <returns>Margin validation result</returns>
    public MarginValidationResult ValidateMargin(
        decimal cost,
        decimal price,
        string costCurrency,
        string priceCurrency)
    {
        // Assume currencies are already converted if different
        var marginAmount = price - cost;
        var marginPercentage = cost > 0 ? (marginAmount / cost) * 100 : 0;

        var result = new MarginValidationResult
        {
            Cost = cost,
            Price = price,
            MarginAmount = marginAmount,
            MarginPercentage = marginPercentage,
            CostCurrency = costCurrency,
            PriceCurrency = priceCurrency
        };

        // Check for negative margin
        if (marginAmount < 0)
        {
            result.IsNegativeMargin = true;
            result.ShouldSendAlert = _settings.EnableNegativeMarginAlerts;
            result.AlertMessage = $"CRITICAL: Negative margin detected! Cost ({costCurrency} {cost:F2}) exceeds price ({priceCurrency} {price:F2}). Loss: {Math.Abs(marginAmount):F2}";
        }
        // Check for low margin
        else if (marginPercentage < _settings.MinimumMarginPercentage)
        {
            result.IsLowMargin = true;
            result.ShouldSendAlert = _settings.EnableLowMarginAlerts;
            result.AlertMessage = $"WARNING: Low margin detected! Margin is {marginPercentage:F2}% (below minimum {_settings.MinimumMarginPercentage:F2}%)";
        }
        else
        {
            result.IsHealthyMargin = true;
        }

        return result;
    }

    /// <summary>
    /// Gets the list of email addresses to receive margin alerts
    /// </summary>
    /// <returns>List of email addresses</returns>
    public List<string> GetAlertEmailAddresses()
    {
        if (string.IsNullOrWhiteSpace(_settings.MarginAlertEmails))
            return new List<string>();

        return _settings.MarginAlertEmails
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(email => email.Trim())
            .Where(email => !string.IsNullOrWhiteSpace(email))
            .ToList();
    }
}

/// <summary>
/// Result of margin validation
/// </summary>
public class MarginValidationResult
{
    public decimal Cost { get; set; }
    public decimal Price { get; set; }
    public decimal MarginAmount { get; set; }
    public decimal MarginPercentage { get; set; }
    public string CostCurrency { get; set; } = string.Empty;
    public string PriceCurrency { get; set; } = string.Empty;
    public bool IsNegativeMargin { get; set; }
    public bool IsLowMargin { get; set; }
    public bool IsHealthyMargin { get; set; }
    public bool ShouldSendAlert { get; set; }
    public string? AlertMessage { get; set; }
}

using ISPAdmin.Infrastructure.Settings;
using Microsoft.Extensions.Options;

namespace ISPAdmin.Services.Helpers;

/// <summary>
/// Helper class for managing future pricing schedules
/// Validates editing permissions and schedule dates
/// </summary>
public class FuturePricingManager
{
    private readonly TldPricingSettings _settings;

    public FuturePricingManager(IOptions<TldPricingSettings> settings)
    {
        _settings = settings.Value;
    }

    /// <summary>
    /// Checks if a pricing record can be edited based on its effective date
    /// </summary>
    /// <param name="effectiveFrom">The effective date of the pricing</param>
    /// <returns>True if can be edited, false otherwise</returns>
    public bool CanEdit(DateTime effectiveFrom)
    {
        if (!_settings.AllowEditingFuturePrices)
            return false;

        // Can only edit future prices (not yet effective)
        return effectiveFrom > DateTime.UtcNow;
    }

    /// <summary>
    /// Checks if a pricing record can be deleted based on its effective date
    /// </summary>
    /// <param name="effectiveFrom">The effective date of the pricing</param>
    /// <returns>True if can be deleted, false otherwise</returns>
    public bool CanDelete(DateTime effectiveFrom)
    {
        if (!_settings.AllowEditingFuturePrices)
            return false;

        // Can only delete future prices (not yet effective)
        return effectiveFrom > DateTime.UtcNow;
    }

    /// <summary>
    /// Validates a schedule date for a new or updated pricing
    /// </summary>
    /// <param name="effectiveFrom">The proposed effective date</param>
    /// <returns>Validation result with error message if invalid</returns>
    public (bool IsValid, string? ErrorMessage) ValidateScheduleDate(DateTime effectiveFrom)
    {
        var now = DateTime.UtcNow;

        // Cannot schedule in the past
        if (effectiveFrom < now)
        {
            return (false, "Cannot schedule pricing in the past");
        }

        // Check maximum schedule limit
        var maxDate = now.AddDays(_settings.MaxScheduleDays);
        if (effectiveFrom > maxDate)
        {
            return (false, $"Cannot schedule pricing more than {_settings.MaxScheduleDays} days in advance");
        }

        return (true, null);
    }

    /// <summary>
    /// Determines the state of a pricing record based on its dates
    /// </summary>
    /// <param name="effectiveFrom">The effective start date</param>
    /// <param name="effectiveTo">The effective end date (nullable)</param>
    /// <returns>The pricing state</returns>
    public PricingState GetPricingState(DateTime effectiveFrom, DateTime? effectiveTo)
    {
        var now = DateTime.UtcNow;

        if (effectiveFrom > now)
        {
            return PricingState.Future;
        }

        if (effectiveTo.HasValue && effectiveTo.Value < now)
        {
            return PricingState.Past;
        }

        return PricingState.Current;
    }

    /// <summary>
    /// Gets the maximum allowed schedule date
    /// </summary>
    /// <returns>Maximum schedule date</returns>
    public DateTime GetMaxScheduleDate()
    {
        return DateTime.UtcNow.AddDays(_settings.MaxScheduleDays);
    }
}

/// <summary>
/// Represents the state of a pricing record
/// </summary>
public enum PricingState
{
    /// <summary>
    /// Pricing has not yet become effective
    /// </summary>
    Future,

    /// <summary>
    /// Pricing is currently active
    /// </summary>
    Current,

    /// <summary>
    /// Pricing has expired
    /// </summary>
    Past
}

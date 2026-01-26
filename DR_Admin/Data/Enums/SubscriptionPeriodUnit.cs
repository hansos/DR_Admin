namespace ISPAdmin.Data.Enums;

/// <summary>
/// Represents the unit of time for subscription billing periods
/// </summary>
public enum SubscriptionPeriodUnit
{
    /// <summary>
    /// Billing period measured in days
    /// </summary>
    Days = 0,

    /// <summary>
    /// Billing period measured in months
    /// </summary>
    Months = 1,

    /// <summary>
    /// Billing period measured in years
    /// </summary>
    Years = 2
}

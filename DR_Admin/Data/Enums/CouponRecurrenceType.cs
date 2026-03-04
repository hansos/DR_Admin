namespace ISPAdmin.Data.Enums;

/// <summary>
/// Defines how often a coupon can be applied by the same customer.
/// </summary>
public enum CouponRecurrenceType
{
    /// <summary>
    /// No recurrence restrictions beyond usage limits.
    /// </summary>
    None = 0,

    /// <summary>
    /// Can only be used once by each customer.
    /// </summary>
    OneTime = 1,

    /// <summary>
    /// Can be used once per year for a limited number of years from first usage.
    /// </summary>
    RecurringYears = 2
}

namespace ISPAdmin.Data.Enums;

/// <summary>
/// Represents the status of a subscription
/// </summary>
public enum SubscriptionStatus
{
    /// <summary>
    /// Subscription is active and billing normally
    /// </summary>
    Active = 0,

    /// <summary>
    /// Subscription is currently in trial period
    /// </summary>
    Trialing = 1,

    /// <summary>
    /// Subscription has been paused by customer or admin
    /// </summary>
    Paused = 2,

    /// <summary>
    /// Subscription has been cancelled and will not renew
    /// </summary>
    Cancelled = 3,

    /// <summary>
    /// Payment failed and subscription is past due
    /// </summary>
    PastDue = 4,

    /// <summary>
    /// Subscription has expired (end date reached)
    /// </summary>
    Expired = 5,

    /// <summary>
    /// Subscription is incomplete (awaiting initial payment)
    /// </summary>
    Incomplete = 6
}

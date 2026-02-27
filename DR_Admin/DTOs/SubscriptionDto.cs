using ISPAdmin.Data.Enums;

namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object representing a subscription
/// </summary>
public class SubscriptionDto
{
    /// <summary>
    /// Unique identifier for the subscription
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Customer identifier who owns this subscription
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// Service identifier being subscribed to (null for domain-only subscriptions)
    /// </summary>
    public int? ServiceId { get; set; }

    /// <summary>
    /// Billing cycle identifier
    /// </summary>
    public int BillingCycleId { get; set; }

    /// <summary>
    /// Customer payment method identifier (optional)
    /// </summary>
    public int? CustomerPaymentMethodId { get; set; }

    /// <summary>
    /// Current status of the subscription
    /// </summary>
    public SubscriptionStatus Status { get; set; }

    /// <summary>
    /// Date when the subscription started
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Date when the subscription ends (null for ongoing subscriptions)
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Date when the next billing should occur
    /// </summary>
    public DateTime NextBillingDate { get; set; }

    /// <summary>
    /// Start date of the current billing period
    /// </summary>
    public DateTime CurrentPeriodStart { get; set; }

    /// <summary>
    /// End date of the current billing period
    /// </summary>
    public DateTime CurrentPeriodEnd { get; set; }

    /// <summary>
    /// Recurring amount to charge per billing cycle
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Currency code for the subscription amount
    /// </summary>
    public string CurrencyCode { get; set; } = "EUR";

    /// <summary>
    /// Number of billing period units
    /// </summary>
    public int BillingPeriodCount { get; set; }

    /// <summary>
    /// Unit of the billing period (Days, Months, Years)
    /// </summary>
    public SubscriptionPeriodUnit BillingPeriodUnit { get; set; }

    /// <summary>
    /// Trial end date (null if no trial)
    /// </summary>
    public DateTime? TrialEndDate { get; set; }

    /// <summary>
    /// Whether the subscription is currently in trial period
    /// </summary>
    public bool IsInTrial { get; set; }

    /// <summary>
    /// Number of retry attempts for failed payments
    /// </summary>
    public int RetryCount { get; set; }

    /// <summary>
    /// Maximum number of retry attempts before marking as failed
    /// </summary>
    public int MaxRetryAttempts { get; set; }

    /// <summary>
    /// Date of the last billing attempt
    /// </summary>
    public DateTime? LastBillingAttempt { get; set; }

    /// <summary>
    /// Date of the last successful billing
    /// </summary>
    public DateTime? LastSuccessfulBilling { get; set; }

    /// <summary>
    /// Date when the subscription was cancelled
    /// </summary>
    public DateTime? CancelledAt { get; set; }

    /// <summary>
    /// Reason for cancellation
    /// </summary>
    public string CancellationReason { get; set; } = string.Empty;

    /// <summary>
    /// Date when the subscription was paused
    /// </summary>
    public DateTime? PausedAt { get; set; }

    /// <summary>
    /// Reason for pausing the subscription
    /// </summary>
    public string PauseReason { get; set; } = string.Empty;

    /// <summary>
    /// Additional metadata in JSON format
    /// </summary>
    public string Metadata { get; set; } = string.Empty;

    /// <summary>
    /// Internal notes about the subscription
    /// </summary>
    public string Notes { get; set; } = string.Empty;

    /// <summary>
    /// Quantity of the service
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Whether to send email notifications for billing events
    /// </summary>
    public bool SendEmailNotifications { get; set; }

    /// <summary>
    /// Whether to automatically retry failed payments
    /// </summary>
    public bool AutoRetryFailedPayments { get; set; }

    /// <summary>
    /// Date when the subscription was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date when the subscription was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Data transfer object for creating a new subscription
/// </summary>
public class CreateSubscriptionDto
{
    /// <summary>
    /// Customer identifier who will own this subscription
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// Service identifier to subscribe to (null for domain-only subscriptions)
    /// </summary>
    public int? ServiceId { get; set; }

    /// <summary>
    /// Billing cycle identifier
    /// </summary>
    public int BillingCycleId { get; set; }

    /// <summary>
    /// Customer payment method identifier (optional, can be set later)
    /// </summary>
    public int? CustomerPaymentMethodId { get; set; }

    /// <summary>
    /// Date when the subscription should start (defaults to now)
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// Date when the subscription should end (null for ongoing)
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Recurring amount to charge per billing cycle
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Currency code for the subscription amount
    /// </summary>
    public string CurrencyCode { get; set; } = "EUR";

    /// <summary>
    /// Number of billing period units (default: 1)
    /// </summary>
    public int BillingPeriodCount { get; set; } = 1;

    /// <summary>
    /// Unit of the billing period (default: Months)
    /// </summary>
    public SubscriptionPeriodUnit BillingPeriodUnit { get; set; } = SubscriptionPeriodUnit.Months;

    /// <summary>
    /// Number of trial days (0 for no trial)
    /// </summary>
    public int TrialDays { get; set; }

    /// <summary>
    /// Maximum number of retry attempts for failed payments (default: 3)
    /// </summary>
    public int MaxRetryAttempts { get; set; } = 3;

    /// <summary>
    /// Additional metadata in JSON format
    /// </summary>
    public string Metadata { get; set; } = string.Empty;

    /// <summary>
    /// Internal notes about the subscription
    /// </summary>
    public string Notes { get; set; } = string.Empty;

    /// <summary>
    /// Quantity of the service (default: 1)
    /// </summary>
    public int Quantity { get; set; } = 1;

    /// <summary>
    /// Whether to send email notifications for billing events (default: true)
    /// </summary>
    public bool SendEmailNotifications { get; set; } = true;

    /// <summary>
    /// Whether to automatically retry failed payments (default: true)
    /// </summary>
    public bool AutoRetryFailedPayments { get; set; } = true;
}

/// <summary>
/// Data transfer object for updating an existing subscription
/// </summary>
public class UpdateSubscriptionDto
{
    /// <summary>
    /// Customer payment method identifier
    /// </summary>
    public int? CustomerPaymentMethodId { get; set; }

    /// <summary>
    /// Date when the subscription should end (null for ongoing)
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Recurring amount to charge per billing cycle
    /// </summary>
    public decimal? Amount { get; set; }

    /// <summary>
    /// Currency code for the subscription amount
    /// </summary>
    public string? CurrencyCode { get; set; }

    /// <summary>
    /// Number of billing period units
    /// </summary>
    public int? BillingPeriodCount { get; set; }

    /// <summary>
    /// Unit of the billing period
    /// </summary>
    public SubscriptionPeriodUnit? BillingPeriodUnit { get; set; }

    /// <summary>
    /// Maximum number of retry attempts for failed payments
    /// </summary>
    public int? MaxRetryAttempts { get; set; }

    /// <summary>
    /// Additional metadata in JSON format
    /// </summary>
    public string? Metadata { get; set; }

    /// <summary>
    /// Internal notes about the subscription
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Quantity of the service
    /// </summary>
    public int? Quantity { get; set; }

    /// <summary>
    /// Whether to send email notifications for billing events
    /// </summary>
    public bool? SendEmailNotifications { get; set; }

    /// <summary>
    /// Whether to automatically retry failed payments
    /// </summary>
    public bool? AutoRetryFailedPayments { get; set; }
}

/// <summary>
/// Data transfer object for cancelling a subscription
/// </summary>
public class CancelSubscriptionDto
{
    /// <summary>
    /// Reason for cancelling the subscription
    /// </summary>
    public string CancellationReason { get; set; } = string.Empty;

    /// <summary>
    /// Whether to cancel immediately or at the end of the billing period
    /// </summary>
    public bool CancelImmediately { get; set; }
}

/// <summary>
/// Data transfer object for pausing a subscription
/// </summary>
public class PauseSubscriptionDto
{
    /// <summary>
    /// Reason for pausing the subscription
    /// </summary>
    public string PauseReason { get; set; } = string.Empty;
}

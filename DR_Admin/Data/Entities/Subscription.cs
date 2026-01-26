using ISPAdmin.Data.Enums;

namespace ISPAdmin.Data.Entities;

/// <summary>
/// Represents a recurring billing subscription for a customer's service
/// </summary>
public class Subscription : EntityBase
{
    /// <summary>
    /// Foreign key to the customer who owns this subscription
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// Foreign key to the service being subscribed to
    /// </summary>
    public int ServiceId { get; set; }

    /// <summary>
    /// Foreign key to the billing cycle
    /// </summary>
    public int BillingCycleId { get; set; }

    /// <summary>
    /// Foreign key to the customer's payment method
    /// </summary>
    public int? CustomerPaymentMethodId { get; set; }

    /// <summary>
    /// Current status of the subscription
    /// </summary>
    public SubscriptionStatus Status { get; set; } = SubscriptionStatus.Active;

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
    /// Number of billing period units (e.g., 1 for monthly, 3 for quarterly)
    /// </summary>
    public int BillingPeriodCount { get; set; } = 1;

    /// <summary>
    /// Unit of the billing period (Days, Months, Years)
    /// </summary>
    public SubscriptionPeriodUnit BillingPeriodUnit { get; set; } = SubscriptionPeriodUnit.Months;

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
    public int MaxRetryAttempts { get; set; } = 3;

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
    /// Quantity of the service (for usage-based billing)
    /// </summary>
    public int Quantity { get; set; } = 1;

    /// <summary>
    /// Whether to send email notifications for billing events
    /// </summary>
    public bool SendEmailNotifications { get; set; } = true;

    /// <summary>
    /// Whether to automatically retry failed payments
    /// </summary>
    public bool AutoRetryFailedPayments { get; set; } = true;

    // Navigation Properties

    /// <summary>
    /// Navigation property to the customer
    /// </summary>
    public Customer Customer { get; set; } = null!;

    /// <summary>
    /// Navigation property to the service
    /// </summary>
    public Service Service { get; set; } = null!;

    /// <summary>
    /// Navigation property to the billing cycle
    /// </summary>
    public BillingCycle BillingCycle { get; set; } = null!;

    /// <summary>
    /// Navigation property to the customer payment method
    /// </summary>
    public CustomerPaymentMethod? CustomerPaymentMethod { get; set; }

    /// <summary>
    /// Navigation property to billing history records
    /// </summary>
    public ICollection<SubscriptionBillingHistory> BillingHistories { get; set; } = new List<SubscriptionBillingHistory>();
}

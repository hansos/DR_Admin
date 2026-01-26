using ISPAdmin.Data.Enums;

namespace ISPAdmin.Data.Entities;

/// <summary>
/// Represents a billing history record for a subscription
/// Tracks all billing attempts and their outcomes for audit purposes
/// </summary>
public class SubscriptionBillingHistory : EntityBase
{
    /// <summary>
    /// Foreign key to the subscription
    /// </summary>
    public int SubscriptionId { get; set; }

    /// <summary>
    /// Foreign key to the invoice created for this billing (null if billing failed)
    /// </summary>
    public int? InvoiceId { get; set; }

    /// <summary>
    /// Foreign key to the payment transaction (null if no payment was attempted)
    /// </summary>
    public int? PaymentTransactionId { get; set; }

    /// <summary>
    /// Date and time when the billing was attempted
    /// </summary>
    public DateTime BillingDate { get; set; }

    /// <summary>
    /// Amount that was charged (or attempted to be charged)
    /// </summary>
    public decimal AmountCharged { get; set; }

    /// <summary>
    /// Currency code for the charged amount
    /// </summary>
    public string CurrencyCode { get; set; } = "EUR";

    /// <summary>
    /// Status of this billing attempt
    /// </summary>
    public PaymentTransactionStatus Status { get; set; }

    /// <summary>
    /// Attempt number for this billing cycle (1 for first attempt, 2 for first retry, etc.)
    /// </summary>
    public int AttemptCount { get; set; }

    /// <summary>
    /// Error message if the billing failed
    /// </summary>
    public string ErrorMessage { get; set; } = string.Empty;

    /// <summary>
    /// Billing period start date
    /// </summary>
    public DateTime PeriodStart { get; set; }

    /// <summary>
    /// Billing period end date
    /// </summary>
    public DateTime PeriodEnd { get; set; }

    /// <summary>
    /// Whether this was an automatic billing or manual
    /// </summary>
    public bool IsAutomatic { get; set; } = true;

    /// <summary>
    /// User ID who triggered manual billing (null for automatic)
    /// </summary>
    public int? ProcessedByUserId { get; set; }

    /// <summary>
    /// Additional notes about this billing attempt
    /// </summary>
    public string Notes { get; set; } = string.Empty;

    /// <summary>
    /// Additional metadata in JSON format
    /// </summary>
    public string Metadata { get; set; } = string.Empty;

    // Navigation Properties

    /// <summary>
    /// Navigation property to the subscription
    /// </summary>
    public Subscription Subscription { get; set; } = null!;

    /// <summary>
    /// Navigation property to the invoice (if created)
    /// </summary>
    public Invoice? Invoice { get; set; }

    /// <summary>
    /// Navigation property to the payment transaction (if attempted)
    /// </summary>
    public PaymentTransaction? PaymentTransaction { get; set; }

    /// <summary>
    /// Navigation property to the user who processed manual billing
    /// </summary>
    public User? ProcessedByUser { get; set; }
}

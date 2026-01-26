using ISPAdmin.Data.Enums;

namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object representing a subscription billing history record
/// </summary>
public class SubscriptionBillingHistoryDto
{
    /// <summary>
    /// Unique identifier for the billing history record
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Subscription identifier this billing record belongs to
    /// </summary>
    public int SubscriptionId { get; set; }

    /// <summary>
    /// Invoice identifier created for this billing (null if billing failed)
    /// </summary>
    public int? InvoiceId { get; set; }

    /// <summary>
    /// Payment transaction identifier (null if no payment was attempted)
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
    public bool IsAutomatic { get; set; }

    /// <summary>
    /// User identifier who triggered manual billing (null for automatic)
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

    /// <summary>
    /// Date when this billing history record was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date when this billing history record was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Data transfer object for creating a new subscription billing history record
/// </summary>
public class CreateSubscriptionBillingHistoryDto
{
    /// <summary>
    /// Subscription identifier this billing record belongs to
    /// </summary>
    public int SubscriptionId { get; set; }

    /// <summary>
    /// Invoice identifier created for this billing (optional)
    /// </summary>
    public int? InvoiceId { get; set; }

    /// <summary>
    /// Payment transaction identifier (optional)
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
    /// Attempt number for this billing cycle
    /// </summary>
    public int AttemptCount { get; set; }

    /// <summary>
    /// Error message if the billing failed (optional)
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
    /// User identifier who triggered manual billing (optional)
    /// </summary>
    public int? ProcessedByUserId { get; set; }

    /// <summary>
    /// Additional notes about this billing attempt (optional)
    /// </summary>
    public string Notes { get; set; } = string.Empty;

    /// <summary>
    /// Additional metadata in JSON format (optional)
    /// </summary>
    public string Metadata { get; set; } = string.Empty;
}

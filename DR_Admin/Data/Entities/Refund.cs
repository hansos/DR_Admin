using ISPAdmin.Data.Enums;

namespace ISPAdmin.Data.Entities;

/// <summary>
/// Represents a refund transaction
/// </summary>
public class Refund : EntityBase
{
    /// <summary>
    /// Foreign key to the original payment transaction
    /// </summary>
    public int PaymentTransactionId { get; set; }

    /// <summary>
    /// Foreign key to the invoice
    /// </summary>
    public int InvoiceId { get; set; }

    /// <summary>
    /// Amount being refunded
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Reason for the refund
    /// </summary>
    public string Reason { get; set; } = string.Empty;

    /// <summary>
    /// Current status of the refund
    /// </summary>
    public RefundStatus Status { get; set; } = RefundStatus.Pending;

    /// <summary>
    /// Refund transaction ID from the payment gateway
    /// </summary>
    public string RefundTransactionId { get; set; } = string.Empty;

    /// <summary>
    /// Date when the refund was processed
    /// </summary>
    public DateTime? ProcessedAt { get; set; }

    /// <summary>
    /// Date when the refund failed
    /// </summary>
    public DateTime? FailedAt { get; set; }

    /// <summary>
    /// Reason for failure (if applicable)
    /// </summary>
    public string FailureReason { get; set; } = string.Empty;

    /// <summary>
    /// Raw response from the payment gateway
    /// </summary>
    public string GatewayResponse { get; set; } = string.Empty;

    /// <summary>
    /// ID of the user who initiated the refund
    /// </summary>
    public int? InitiatedByUserId { get; set; }

    /// <summary>
    /// Internal notes about the refund
    /// </summary>
    public string InternalNotes { get; set; } = string.Empty;

    /// <summary>
    /// Soft delete timestamp
    /// </summary>
    public DateTime? DeletedAt { get; set; }

    /// <summary>
    /// Navigation property to the payment transaction
    /// </summary>
    public PaymentTransaction PaymentTransaction { get; set; } = null!;

    /// <summary>
    /// Navigation property to the invoice
    /// </summary>
    public Invoice Invoice { get; set; } = null!;

    /// <summary>
    /// Navigation property to the user who initiated the refund
    /// </summary>
    public User? InitiatedByUser { get; set; }
}

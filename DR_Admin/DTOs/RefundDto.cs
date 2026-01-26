using ISPAdmin.Data.Enums;

namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object representing a refund
/// </summary>
public class RefundDto
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Payment transaction ID
    /// </summary>
    public int PaymentTransactionId { get; set; }

    /// <summary>
    /// Invoice ID
    /// </summary>
    public int InvoiceId { get; set; }

    /// <summary>
    /// Invoice number
    /// </summary>
    public string InvoiceNumber { get; set; } = string.Empty;

    /// <summary>
    /// Refund amount
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Reason for refund
    /// </summary>
    public string Reason { get; set; } = string.Empty;

    /// <summary>
    /// Current status
    /// </summary>
    public RefundStatus Status { get; set; }

    /// <summary>
    /// Refund transaction ID from gateway
    /// </summary>
    public string RefundTransactionId { get; set; } = string.Empty;

    /// <summary>
    /// Date processed
    /// </summary>
    public DateTime? ProcessedAt { get; set; }

    /// <summary>
    /// Date failed
    /// </summary>
    public DateTime? FailedAt { get; set; }

    /// <summary>
    /// Failure reason
    /// </summary>
    public string FailureReason { get; set; } = string.Empty;

    /// <summary>
    /// User who initiated the refund
    /// </summary>
    public int? InitiatedByUserId { get; set; }

    /// <summary>
    /// Date created
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

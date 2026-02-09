using ISPAdmin.Data.Enums;

namespace ISPAdmin.Data.Entities;

/// <summary>
/// Tracks financial losses from refunds where vendor costs cannot be recovered
/// </summary>
public class RefundLossAudit : EntityBase
{
    /// <summary>
    /// Foreign key to the refund transaction
    /// </summary>
    public int RefundId { get; set; }

    /// <summary>
    /// Foreign key to the invoice being refunded
    /// </summary>
    public int InvoiceId { get; set; }

    /// <summary>
    /// Original total amount of the invoice
    /// </summary>
    public decimal OriginalInvoiceAmount { get; set; }

    /// <summary>
    /// Amount being refunded to customer
    /// </summary>
    public decimal RefundedAmount { get; set; }

    /// <summary>
    /// Total vendor costs that cannot be recovered
    /// </summary>
    public decimal VendorCostUnrecoverable { get; set; }

    /// <summary>
    /// Net financial loss (RefundedAmount - RecoverableAmount)
    /// </summary>
    public decimal NetLoss { get; set; }

    /// <summary>
    /// Currency code for all amounts
    /// </summary>
    public string Currency { get; set; } = "EUR";

    /// <summary>
    /// Reason for the refund and resulting loss
    /// </summary>
    public string Reason { get; set; } = string.Empty;

    /// <summary>
    /// Approval status for this loss
    /// </summary>
    public ApprovalStatus ApprovalStatus { get; set; } = ApprovalStatus.PendingApproval;

    /// <summary>
    /// Foreign key to the user who approved/denied the loss
    /// </summary>
    public int? ApprovedByUserId { get; set; }

    /// <summary>
    /// Date and time when the loss was approved/denied
    /// </summary>
    public DateTime? ApprovedAt { get; set; }

    /// <summary>
    /// Reason for denial if approval was denied
    /// </summary>
    public string? DenialReason { get; set; }

    /// <summary>
    /// Internal notes about this refund loss
    /// </summary>
    public string InternalNotes { get; set; } = string.Empty;

    /// <summary>
    /// Navigation property to the refund
    /// </summary>
    public Refund Refund { get; set; } = null!;

    /// <summary>
    /// Navigation property to the invoice
    /// </summary>
    public Invoice Invoice { get; set; } = null!;

    /// <summary>
    /// Navigation property to the approving user
    /// </summary>
    public User? ApprovedByUser { get; set; }
}

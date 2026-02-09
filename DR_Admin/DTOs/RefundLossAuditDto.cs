using ISPAdmin.Data.Enums;

namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object representing financial losses from refunds
/// </summary>
public class RefundLossAuditDto
{
    /// <summary>
    /// Gets or sets the unique identifier
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the refund ID
    /// </summary>
    public int RefundId { get; set; }

    /// <summary>
    /// Gets or sets the invoice ID
    /// </summary>
    public int InvoiceId { get; set; }

    /// <summary>
    /// Gets or sets the original invoice total amount
    /// </summary>
    public decimal OriginalInvoiceAmount { get; set; }

    /// <summary>
    /// Gets or sets the amount being refunded to customer
    /// </summary>
    public decimal RefundedAmount { get; set; }

    /// <summary>
    /// Gets or sets the total vendor costs that cannot be recovered
    /// </summary>
    public decimal VendorCostUnrecoverable { get; set; }

    /// <summary>
    /// Gets or sets the net financial loss
    /// </summary>
    public decimal NetLoss { get; set; }

    /// <summary>
    /// Gets or sets the currency code for all amounts
    /// </summary>
    public string Currency { get; set; } = "EUR";

    /// <summary>
    /// Gets or sets the reason for the refund and resulting loss
    /// </summary>
    public string Reason { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the approval status
    /// </summary>
    public ApprovalStatus ApprovalStatus { get; set; }

    /// <summary>
    /// Gets or sets the user ID who approved/denied the loss
    /// </summary>
    public int? ApprovedByUserId { get; set; }

    /// <summary>
    /// Gets or sets the approval/denial timestamp
    /// </summary>
    public DateTime? ApprovedAt { get; set; }

    /// <summary>
    /// Gets or sets the reason for denial
    /// </summary>
    public string? DenialReason { get; set; }

    /// <summary>
    /// Gets or sets internal notes
    /// </summary>
    public string InternalNotes { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the timestamp when the record was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the record was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

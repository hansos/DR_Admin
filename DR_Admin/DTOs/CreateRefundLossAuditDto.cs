using ISPAdmin.Data.Enums;

namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object for creating refund loss audits
/// </summary>
public class CreateRefundLossAuditDto
{
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
    /// Gets or sets internal notes
    /// </summary>
    public string InternalNotes { get; set; } = string.Empty;
}

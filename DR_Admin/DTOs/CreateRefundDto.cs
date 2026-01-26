namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object for creating a refund
/// </summary>
public class CreateRefundDto
{
    /// <summary>
    /// Payment transaction ID to refund
    /// </summary>
    public int PaymentTransactionId { get; set; }

    /// <summary>
    /// Amount to refund (if null, full amount is refunded)
    /// </summary>
    public decimal? Amount { get; set; }

    /// <summary>
    /// Reason for the refund
    /// </summary>
    public string Reason { get; set; } = string.Empty;

    /// <summary>
    /// Internal notes
    /// </summary>
    public string InternalNotes { get; set; } = string.Empty;
}

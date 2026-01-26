using ISPAdmin.Data.Enums;

namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object representing a credit transaction
/// </summary>
public class CreditTransactionDto
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Customer credit ID
    /// </summary>
    public int CustomerCreditId { get; set; }

    /// <summary>
    /// Transaction type
    /// </summary>
    public CreditTransactionType Type { get; set; }

    /// <summary>
    /// Transaction amount
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Invoice ID (if applicable)
    /// </summary>
    public int? InvoiceId { get; set; }

    /// <summary>
    /// Payment transaction ID (if applicable)
    /// </summary>
    public int? PaymentTransactionId { get; set; }

    /// <summary>
    /// Description
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Balance after this transaction
    /// </summary>
    public decimal BalanceAfter { get; set; }

    /// <summary>
    /// User who created this transaction
    /// </summary>
    public int? CreatedByUserId { get; set; }

    /// <summary>
    /// Date created
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

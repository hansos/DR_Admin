using ISPAdmin.Data.Enums;

namespace ISPAdmin.Data.Entities;

/// <summary>
/// Represents a single credit transaction for a customer
/// </summary>
public class CreditTransaction : EntityBase
{
    /// <summary>
    /// Foreign key to the customer credit account
    /// </summary>
    public int CustomerCreditId { get; set; }

    /// <summary>
    /// Type of credit transaction
    /// </summary>
    public CreditTransactionType Type { get; set; }

    /// <summary>
    /// Amount of the transaction (positive for credit, negative for debit)
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Foreign key to the invoice (if credit was applied to an invoice)
    /// </summary>
    public int? InvoiceId { get; set; }

    /// <summary>
    /// Foreign key to the payment transaction (if credit came from a payment)
    /// </summary>
    public int? PaymentTransactionId { get; set; }

    /// <summary>
    /// Description of the transaction
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Balance after this transaction
    /// </summary>
    public decimal BalanceAfter { get; set; }

    /// <summary>
    /// ID of the user who created this transaction
    /// </summary>
    public int? CreatedByUserId { get; set; }

    /// <summary>
    /// Internal notes
    /// </summary>
    public string InternalNotes { get; set; } = string.Empty;

    /// <summary>
    /// Navigation property to the customer credit account
    /// </summary>
    public CustomerCredit CustomerCredit { get; set; } = null!;

    /// <summary>
    /// Navigation property to the invoice
    /// </summary>
    public Invoice? Invoice { get; set; }

    /// <summary>
    /// Navigation property to the payment transaction
    /// </summary>
    public PaymentTransaction? PaymentTransaction { get; set; }

    /// <summary>
    /// Navigation property to the user who created this transaction
    /// </summary>
    public User? CreatedByUser { get; set; }
}

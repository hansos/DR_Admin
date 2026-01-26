using ISPAdmin.Data.Enums;

namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object for creating a credit transaction
/// </summary>
public class CreateCreditTransactionDto
{
    /// <summary>
    /// Customer ID
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// Transaction type
    /// </summary>
    public CreditTransactionType Type { get; set; }

    /// <summary>
    /// Amount (positive for credit, can be negative for debit)
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Description
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Internal notes
    /// </summary>
    public string InternalNotes { get; set; } = string.Empty;
}

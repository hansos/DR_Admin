using ISPAdmin.Data.Enums;

namespace ISPAdmin.Data.Entities;

/// <summary>
/// Represents a customer's credit balance and transactions
/// </summary>
public class CustomerCredit : EntityBase
{
    /// <summary>
    /// Foreign key to the customer
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// Current credit balance
    /// </summary>
    public decimal Balance { get; set; }

    /// <summary>
    /// Currency code (e.g., "USD", "EUR")
    /// </summary>
    public string CurrencyCode { get; set; } = "EUR";

    /// <summary>
    /// Navigation property to the customer
    /// </summary>
    public Customer Customer { get; set; } = null!;

    /// <summary>
    /// Collection of credit transactions
    /// </summary>
    public ICollection<CreditTransaction> CreditTransactions { get; set; } = new List<CreditTransaction>();
}

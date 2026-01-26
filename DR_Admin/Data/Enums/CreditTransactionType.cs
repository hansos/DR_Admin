namespace ISPAdmin.Data.Enums;

/// <summary>
/// Represents the type of credit transaction
/// </summary>
public enum CreditTransactionType
{
    /// <summary>
    /// Customer deposits credit to their account
    /// </summary>
    Deposit = 0,
    
    /// <summary>
    /// Credit is deducted from the account (e.g., applied to invoice)
    /// </summary>
    Deduction = 1,
    
    /// <summary>
    /// Refund added to credit balance
    /// </summary>
    Refund = 2,
    
    /// <summary>
    /// Manual adjustment by admin
    /// </summary>
    Adjustment = 3,
    
    /// <summary>
    /// Promotional credit added
    /// </summary>
    Promotional = 4
}

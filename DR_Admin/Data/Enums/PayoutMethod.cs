namespace ISPAdmin.Data.Enums;

/// <summary>
/// Represents the method used for vendor payout
/// </summary>
public enum PayoutMethod
{
    /// <summary>
    /// Automated payout via vendor API
    /// </summary>
    API = 0,
    
    /// <summary>
    /// Manual bank transfer
    /// </summary>
    BankTransfer = 1,
    
    /// <summary>
    /// Deduct from prepaid vendor account balance
    /// </summary>
    PrepaidAccount = 2,
    
    /// <summary>
    /// Payment via credit card
    /// </summary>
    CreditCard = 3,
    
    /// <summary>
    /// Other manual payment method
    /// </summary>
    Manual = 4
}

namespace ISPAdmin.Data.Enums;

/// <summary>
/// Represents the type of payment method
/// </summary>
public enum PaymentMethodType
{
    /// <summary>
    /// Credit or debit card payment
    /// </summary>
    CreditCard = 0,
    
    /// <summary>
    /// Bank account payment (ACH, SEPA, etc.)
    /// </summary>
    BankAccount = 1,
    
    /// <summary>
    /// PayPal payment
    /// </summary>
    PayPal = 2,
    
    /// <summary>
    /// Other payment method
    /// </summary>
    Other = 99
}

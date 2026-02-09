namespace ISPAdmin.Data.Enums;

/// <summary>
/// Represents the type of invoice line item
/// </summary>
public enum InvoiceLineType
{
    /// <summary>
    /// Physical or digital product
    /// </summary>
    Product = 0,
    
    /// <summary>
    /// Service (hosting, support, etc.)
    /// </summary>
    Service = 1,
    
    /// <summary>
    /// Payment gateway processing fee
    /// </summary>
    PaymentFee = 2,
    
    /// <summary>
    /// Tax line item
    /// </summary>
    Tax = 3,
    
    /// <summary>
    /// Discount or coupon
    /// </summary>
    Discount = 4,
    
    /// <summary>
    /// Credit applied
    /// </summary>
    Credit = 5
}

namespace ISPAdmin.Data.Enums;

/// <summary>
/// Represents the status of a quote in the sales workflow
/// </summary>
public enum QuoteStatus
{
    /// <summary>
    /// Quote is being prepared and not yet sent to customer
    /// </summary>
    Draft = 0,
    
    /// <summary>
    /// Quote has been sent to the customer
    /// </summary>
    Sent = 1,
    
    /// <summary>
    /// Customer has accepted the quote
    /// </summary>
    Accepted = 2,
    
    /// <summary>
    /// Customer has rejected the quote
    /// </summary>
    Rejected = 3,
    
    /// <summary>
    /// Quote has expired based on valid until date
    /// </summary>
    Expired = 4,
    
    /// <summary>
    /// Quote has been converted to an order
    /// </summary>
    Converted = 5
}

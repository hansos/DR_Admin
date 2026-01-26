namespace ISPAdmin.Data.Enums;

/// <summary>
/// Represents the status of a payment transaction
/// </summary>
public enum PaymentTransactionStatus
{
    /// <summary>
    /// Payment transaction is pending processing
    /// </summary>
    Pending = 0,
    
    /// <summary>
    /// Payment is currently being processed
    /// </summary>
    Processing = 1,
    
    /// <summary>
    /// Payment has been successfully completed
    /// </summary>
    Completed = 2,
    
    /// <summary>
    /// Payment has failed
    /// </summary>
    Failed = 3,
    
    /// <summary>
    /// Payment has been fully refunded
    /// </summary>
    Refunded = 4,
    
    /// <summary>
    /// Payment has been partially refunded
    /// </summary>
    PartiallyRefunded = 5,
    
    /// <summary>
    /// Payment is being disputed by the customer
    /// </summary>
    Disputed = 6,
    
    /// <summary>
    /// Payment has been cancelled before completion
    /// </summary>
    Cancelled = 7
}

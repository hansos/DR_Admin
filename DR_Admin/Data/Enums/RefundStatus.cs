namespace ISPAdmin.Data.Enums;

/// <summary>
/// Represents the status of a refund
/// </summary>
public enum RefundStatus
{
    /// <summary>
    /// Refund request is pending processing
    /// </summary>
    Pending = 0,
    
    /// <summary>
    /// Refund is currently being processed
    /// </summary>
    Processing = 1,
    
    /// <summary>
    /// Refund has been successfully completed
    /// </summary>
    Completed = 2,
    
    /// <summary>
    /// Refund has failed
    /// </summary>
    Failed = 3,
    
    /// <summary>
    /// Refund has been cancelled
    /// </summary>
    Cancelled = 4
}

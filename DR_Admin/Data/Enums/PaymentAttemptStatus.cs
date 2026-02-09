namespace ISPAdmin.Data.Enums;

/// <summary>
/// Represents the status of a payment attempt
/// </summary>
public enum PaymentAttemptStatus
{
    /// <summary>
    /// Payment is pending processing
    /// </summary>
    Pending = 0,
    
    /// <summary>
    /// Payment is currently being processed
    /// </summary>
    Processing = 1,
    
    /// <summary>
    /// Payment requires customer authentication (3D Secure, etc.)
    /// </summary>
    RequiresAuthentication = 2,
    
    /// <summary>
    /// Payment was successful
    /// </summary>
    Succeeded = 3,
    
    /// <summary>
    /// Payment failed
    /// </summary>
    Failed = 4,
    
    /// <summary>
    /// Payment was cancelled
    /// </summary>
    Cancelled = 5
}

namespace ISPAdmin.Data.Enums;

/// <summary>
/// Represents the status of a payment intent in the payment workflow
/// </summary>
public enum PaymentIntentStatus
{
    /// <summary>
    /// Payment intent has been created but not yet authorized
    /// </summary>
    Created = 0,
    
    /// <summary>
    /// Payment has been authorized but not yet captured
    /// </summary>
    Authorized = 1,
    
    /// <summary>
    /// Payment has been captured and funds are being transferred
    /// </summary>
    Captured = 2,
    
    /// <summary>
    /// Payment intent has failed
    /// </summary>
    Failed = 3,
    
    /// <summary>
    /// Payment intent has been cancelled
    /// </summary>
    Cancelled = 4,
    
    /// <summary>
    /// Payment requires additional action (e.g., 3D Secure authentication)
    /// </summary>
    RequiresAction = 5
}

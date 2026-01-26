namespace ISPAdmin.Data.Enums;

/// <summary>
/// Represents the status of an order/subscription
/// </summary>
public enum OrderStatus
{
    /// <summary>
    /// Order is pending activation
    /// </summary>
    Pending = 0,
    
    /// <summary>
    /// Order is active and service is running
    /// </summary>
    Active = 1,
    
    /// <summary>
    /// Order has been suspended due to non-payment or other issues
    /// </summary>
    Suspended = 2,
    
    /// <summary>
    /// Order has been cancelled
    /// </summary>
    Cancelled = 3,
    
    /// <summary>
    /// Order has expired and is no longer active
    /// </summary>
    Expired = 4,
    
    /// <summary>
    /// Order is in trial period
    /// </summary>
    Trial = 5
}

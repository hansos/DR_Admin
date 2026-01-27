namespace ISPAdmin.Data.Enums;

/// <summary>
/// Represents the lifecycle status of a domain
/// </summary>
public enum DomainStatus
{
    /// <summary>
    /// Domain registration is pending (awaiting payment or processing)
    /// </summary>
    PendingRegistration = 0,
    
    /// <summary>
    /// Domain is active and operational
    /// </summary>
    Active = 1,
    
    /// <summary>
    /// Domain is suspended (non-payment, violation, etc.)
    /// </summary>
    Suspended = 2,
    
    /// <summary>
    /// Domain transfer is in progress
    /// </summary>
    PendingTransfer = 3,
    
    /// <summary>
    /// Domain renewal is pending
    /// </summary>
    PendingRenewal = 4,
    
    /// <summary>
    /// Domain has expired
    /// </summary>
    Expired = 5,
    
    /// <summary>
    /// Domain registration/order has been cancelled
    /// </summary>
    Cancelled = 6,
    
    /// <summary>
    /// Domain has been transferred out to another registrar
    /// </summary>
    TransferredOut = 7
}

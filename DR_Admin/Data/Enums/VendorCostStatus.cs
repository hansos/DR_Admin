namespace ISPAdmin.Data.Enums;

/// <summary>
/// Represents the lifecycle status of a vendor cost
/// </summary>
public enum VendorCostStatus
{
    /// <summary>
    /// Cost is projected but not yet committed
    /// </summary>
    Estimated = 0,
    
    /// <summary>
    /// Cost is locked in (e.g., domain reserved, service activated)
    /// </summary>
    Committed = 1,
    
    /// <summary>
    /// Vendor has been paid for this cost
    /// </summary>
    Paid = 2,
    
    /// <summary>
    /// Cost was refunded by vendor
    /// </summary>
    Refunded = 3,
    
    /// <summary>
    /// Unrecoverable due to customer refund
    /// </summary>
    Lost = 4
}

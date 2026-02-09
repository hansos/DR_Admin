namespace ISPAdmin.Data.Enums;

/// <summary>
/// Represents the status of a vendor payout
/// </summary>
public enum VendorPayoutStatus
{
    /// <summary>
    /// Payout is scheduled but not yet processed
    /// </summary>
    Pending = 0,
    
    /// <summary>
    /// Payout is currently being processed
    /// </summary>
    Processing = 1,
    
    /// <summary>
    /// Payout has been successfully completed
    /// </summary>
    Paid = 2,
    
    /// <summary>
    /// Payout failed and needs retry or manual intervention
    /// </summary>
    Failed = 3,
    
    /// <summary>
    /// Payout was cancelled
    /// </summary>
    Cancelled = 4,
    
    /// <summary>
    /// Payout requires manual review before processing
    /// </summary>
    RequiresManualReview = 5
}

namespace ISPAdmin.Data.Enums;

/// <summary>
/// Represents approval status for financial operations
/// </summary>
public enum ApprovalStatus
{
    /// <summary>
    /// Awaiting approval
    /// </summary>
    PendingApproval = 0,
    
    /// <summary>
    /// Approved by authorized user
    /// </summary>
    Approved = 1,
    
    /// <summary>
    /// Denied with reason
    /// </summary>
    Denied = 2
}

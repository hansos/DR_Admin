namespace ISPAdmin.Data.Enums;

/// <summary>
/// Defines refund eligibility for vendor costs
/// </summary>
public enum RefundPolicy
{
    /// <summary>
    /// Cost can be fully refunded at any time
    /// </summary>
    FullyRefundable = 0,
    
    /// <summary>
    /// Cost becomes non-refundable after service activation
    /// </summary>
    NonRefundableAfterActivation = 1,
    
    /// <summary>
    /// Cost is never refundable
    /// </summary>
    NonRefundable = 2,
    
    /// <summary>
    /// Refund amount decreases over time
    /// </summary>
    TimeBasedProrated = 3
}

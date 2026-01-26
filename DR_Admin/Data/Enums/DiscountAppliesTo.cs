namespace ISPAdmin.Data.Enums;

/// <summary>
/// Represents what the discount applies to
/// </summary>
public enum DiscountAppliesTo
{
    /// <summary>
    /// Discount applies to setup fees only
    /// </summary>
    SetupFee = 0,
    
    /// <summary>
    /// Discount applies to recurring charges only
    /// </summary>
    Recurring = 1,
    
    /// <summary>
    /// Discount applies to both setup and recurring charges
    /// </summary>
    Both = 2,
    
    /// <summary>
    /// Discount applies to the total order amount
    /// </summary>
    Total = 3
}

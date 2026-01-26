namespace ISPAdmin.Data.Enums;

/// <summary>
/// Represents the type of discount applied by a coupon
/// </summary>
public enum CouponType
{
    /// <summary>
    /// Percentage-based discount (e.g., 25% off)
    /// </summary>
    Percentage = 0,
    
    /// <summary>
    /// Fixed amount discount (e.g., $50 off)
    /// </summary>
    FixedAmount = 1
}

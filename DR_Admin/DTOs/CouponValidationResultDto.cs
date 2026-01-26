namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object representing coupon validation result
/// </summary>
public class CouponValidationResultDto
{
    /// <summary>
    /// Whether the coupon is valid
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// Validation message
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Calculated discount amount
    /// </summary>
    public decimal DiscountAmount { get; set; }

    /// <summary>
    /// Coupon details if valid
    /// </summary>
    public CouponDto? Coupon { get; set; }
}

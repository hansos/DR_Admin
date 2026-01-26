namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object for validating a coupon
/// </summary>
public class ValidateCouponDto
{
    /// <summary>
    /// Coupon code to validate
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Customer ID
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// Total order amount
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Service type IDs in the order
    /// </summary>
    public List<int> ServiceTypeIds { get; set; } = new();
}

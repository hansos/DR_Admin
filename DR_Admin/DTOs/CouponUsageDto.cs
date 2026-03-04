namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object representing a coupon usage entry
/// </summary>
public class CouponUsageDto
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Coupon identifier
    /// </summary>
    public int CouponId { get; set; }

    /// <summary>
    /// Coupon code
    /// </summary>
    public string CouponCode { get; set; } = string.Empty;

    /// <summary>
    /// Coupon name
    /// </summary>
    public string CouponName { get; set; } = string.Empty;

    /// <summary>
    /// Customer identifier
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// Customer name
    /// </summary>
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// Related quote identifier
    /// </summary>
    public int? QuoteId { get; set; }

    /// <summary>
    /// Related order identifier
    /// </summary>
    public int? OrderId { get; set; }

    /// <summary>
    /// Applied discount amount
    /// </summary>
    public decimal DiscountAmount { get; set; }

    /// <summary>
    /// Date and time when coupon was used
    /// </summary>
    public DateTime UsedAt { get; set; }
}

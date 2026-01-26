using ISPAdmin.Data.Enums;

namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object representing a coupon
/// </summary>
public class CouponDto
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Coupon code
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Display name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Description
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Coupon type
    /// </summary>
    public CouponType Type { get; set; }

    /// <summary>
    /// Discount value
    /// </summary>
    public decimal Value { get; set; }

    /// <summary>
    /// What the discount applies to
    /// </summary>
    public DiscountAppliesTo AppliesTo { get; set; }

    /// <summary>
    /// Minimum amount required
    /// </summary>
    public decimal? MinimumAmount { get; set; }

    /// <summary>
    /// Maximum discount amount
    /// </summary>
    public decimal? MaximumDiscount { get; set; }

    /// <summary>
    /// Valid from date
    /// </summary>
    public DateTime ValidFrom { get; set; }

    /// <summary>
    /// Valid until date
    /// </summary>
    public DateTime ValidUntil { get; set; }

    /// <summary>
    /// Maximum total usages
    /// </summary>
    public int? MaxUsages { get; set; }

    /// <summary>
    /// Maximum usages per customer
    /// </summary>
    public int? MaxUsagesPerCustomer { get; set; }

    /// <summary>
    /// Is currently active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Current usage count
    /// </summary>
    public int UsageCount { get; set; }

    /// <summary>
    /// Date created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

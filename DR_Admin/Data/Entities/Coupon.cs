using ISPAdmin.Data.Enums;

namespace ISPAdmin.Data.Entities;

/// <summary>
/// Represents a discount coupon that can be applied to quotes and orders
/// </summary>
public class Coupon : EntityBase
{
    /// <summary>
    /// Unique coupon code (e.g., "WELCOME50", "SUMMER2024")
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Display name for the coupon
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Normalized version of Name for case-insensitive searches
    /// </summary>
    public string NormalizedName { get; set; } = string.Empty;

    /// <summary>
    /// Description of the coupon
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Type of discount (percentage or fixed amount)
    /// </summary>
    public CouponType Type { get; set; }

    /// <summary>
    /// Discount value (e.g., 50 for 50% or $50)
    /// </summary>
    public decimal Value { get; set; }

    /// <summary>
    /// What the discount applies to
    /// </summary>
    public DiscountAppliesTo AppliesTo { get; set; }

    /// <summary>
    /// Minimum order amount required to use this coupon
    /// </summary>
    public decimal? MinimumAmount { get; set; }

    /// <summary>
    /// Maximum discount amount (for percentage-based coupons)
    /// </summary>
    public decimal? MaximumDiscount { get; set; }

    /// <summary>
    /// Date when the coupon becomes valid
    /// </summary>
    public DateTime ValidFrom { get; set; }

    /// <summary>
    /// Date when the coupon expires
    /// </summary>
    public DateTime ValidUntil { get; set; }

    /// <summary>
    /// Maximum number of times this coupon can be used (null = unlimited)
    /// </summary>
    public int? MaxUsages { get; set; }

    /// <summary>
    /// Maximum number of times this coupon can be used per customer
    /// </summary>
    public int? MaxUsagesPerCustomer { get; set; }

    /// <summary>
    /// Whether this coupon is currently active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Service type IDs this coupon applies to (JSON array, null = all services)
    /// </summary>
    public string? AllowedServiceTypeIdsJson { get; set; }

    /// <summary>
    /// Current usage count
    /// </summary>
    public int UsageCount { get; set; }

    /// <summary>
    /// Internal notes about this coupon
    /// </summary>
    public string InternalNotes { get; set; } = string.Empty;

    /// <summary>
    /// Soft delete timestamp
    /// </summary>
    public DateTime? DeletedAt { get; set; }

    /// <summary>
    /// Collection of coupon usage records
    /// </summary>
    public ICollection<CouponUsage> CouponUsages { get; set; } = new List<CouponUsage>();

    /// <summary>
    /// Collection of quotes using this coupon
    /// </summary>
    public ICollection<Quote> Quotes { get; set; } = new List<Quote>();
}

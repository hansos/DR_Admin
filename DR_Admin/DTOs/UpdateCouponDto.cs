using ISPAdmin.Data.Enums;

namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object for updating a coupon
/// </summary>
public class UpdateCouponDto
{
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
    /// Minimum amount
    /// </summary>
    public decimal? MinimumAmount { get; set; }

    /// <summary>
    /// Maximum discount
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
    /// Maximum usages
    /// </summary>
    public int? MaxUsages { get; set; }

    /// <summary>
    /// Maximum usages per customer
    /// </summary>
    public int? MaxUsagesPerCustomer { get; set; }

    /// <summary>
    /// Is active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Allowed service type IDs
    /// </summary>
    public List<int>? AllowedServiceTypeIds { get; set; }

    /// <summary>
    /// Internal notes
    /// </summary>
    public string InternalNotes { get; set; } = string.Empty;
}

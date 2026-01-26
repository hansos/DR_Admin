namespace ISPAdmin.Data.Entities;

/// <summary>
/// Represents a record of coupon usage by a customer
/// </summary>
public class CouponUsage : EntityBase
{
    /// <summary>
    /// Foreign key to the coupon
    /// </summary>
    public int CouponId { get; set; }

    /// <summary>
    /// Foreign key to the customer
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// Foreign key to the quote (if applied to a quote)
    /// </summary>
    public int? QuoteId { get; set; }

    /// <summary>
    /// Foreign key to the order (if applied to an order)
    /// </summary>
    public int? OrderId { get; set; }

    /// <summary>
    /// Discount amount that was applied
    /// </summary>
    public decimal DiscountAmount { get; set; }

    /// <summary>
    /// Date when the coupon was used
    /// </summary>
    public DateTime UsedAt { get; set; }

    /// <summary>
    /// Navigation property to the coupon
    /// </summary>
    public Coupon Coupon { get; set; } = null!;

    /// <summary>
    /// Navigation property to the customer
    /// </summary>
    public Customer Customer { get; set; } = null!;

    /// <summary>
    /// Navigation property to the quote
    /// </summary>
    public Quote? Quote { get; set; }

    /// <summary>
    /// Navigation property to the order
    /// </summary>
    public Order? Order { get; set; }
}

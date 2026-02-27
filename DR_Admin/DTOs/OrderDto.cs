using ISPAdmin.Data.Enums;

namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object representing a customer order for a service
/// </summary>
public class OrderDto
{
    /// <summary>
    /// Unique identifier for the order
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Order number
    /// </summary>
    public string OrderNumber { get; set; } = string.Empty;

    /// <summary>
    /// Customer ID
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// Service ID (null for domain-only orders)
    /// </summary>
    public int? ServiceId { get; set; }

    /// <summary>
    /// Quote ID (if applicable)
    /// </summary>
    public int? QuoteId { get; set; }

    /// <summary>
    /// Order type
    /// </summary>
    public OrderType OrderType { get; set; }

    /// <summary>
    /// Order status
    /// </summary>
    public OrderStatus Status { get; set; }

    /// <summary>
    /// Service start date
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Service end date
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// Next billing date
    /// </summary>
    public DateTime NextBillingDate { get; set; }

    /// <summary>
    /// Setup fee
    /// </summary>
    public decimal SetupFee { get; set; }

    /// <summary>
    /// Recurring amount
    /// </summary>
    public decimal RecurringAmount { get; set; }

    /// <summary>
    /// Discount amount
    /// </summary>
    public decimal DiscountAmount { get; set; }
    
    /// <summary>
    /// Currency code for this order (ISO 4217)
    /// </summary>
    public string CurrencyCode { get; set; } = "EUR";
    
    /// <summary>
    /// Base currency for accounting purposes
    /// </summary>
    public string BaseCurrencyCode { get; set; } = "EUR";
    
    /// <summary>
    /// Exchange rate applied at order creation
    /// </summary>
    public decimal? ExchangeRate { get; set; }
    
    /// <summary>
    /// Date of the exchange rate used
    /// </summary>
    public DateTime? ExchangeRateDate { get; set; }

    /// <summary>
    /// Trial end date
    /// </summary>
    public DateTime? TrialEndsAt { get; set; }

    /// <summary>
    /// Auto-renew setting
    /// </summary>
    public bool AutoRenew { get; set; }

    /// <summary>
    /// Date created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}


/// <summary>
/// Data transfer object for creating a new order
/// </summary>
public class CreateOrderDto
{
    /// <summary>
    /// Customer ID
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// Service ID (null for domain-only orders)
    /// </summary>
    public int? ServiceId { get; set; }

    /// <summary>
    /// Quote ID (if applicable)
    /// </summary>
    public int? QuoteId { get; set; }

    /// <summary>
    /// Order type
    /// </summary>
    public OrderType OrderType { get; set; } = OrderType.New;

    /// <summary>
    /// Service start date
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Service end date
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// Next billing date
    /// </summary>
    public DateTime NextBillingDate { get; set; }

    /// <summary>
    /// Setup fee (optional, uses service default if not specified)
    /// </summary>
    public decimal? SetupFee { get; set; }

    /// <summary>
    /// Recurring amount (optional, uses service default if not specified)
    /// </summary>
    public decimal? RecurringAmount { get; set; }

    /// <summary>
    /// Coupon code to apply
    /// </summary>
    public string? CouponCode { get; set; }

    /// <summary>
    /// Auto-renew setting
    /// </summary>
    public bool AutoRenew { get; set; } = true;
}


/// <summary>
/// Data transfer object for updating an existing order
/// </summary>
public class UpdateOrderDto
{
    /// <summary>
    /// Service ID (null for domain-only orders)
    /// </summary>
    public int? ServiceId { get; set; }

    /// <summary>
    /// Order status
    /// </summary>
    public OrderStatus Status { get; set; }

    /// <summary>
    /// Service start date
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Service end date
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// Next billing date
    /// </summary>
    public DateTime NextBillingDate { get; set; }

    /// <summary>
    /// Auto-renew setting
    /// </summary>
    public bool AutoRenew { get; set; }

    /// <summary>
    /// Notes
    /// </summary>
    public string Notes { get; set; } = string.Empty;
}

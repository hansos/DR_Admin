using ISPAdmin.Data.Enums;

namespace ISPAdmin.Data.Entities;

/// <summary>
/// Represents a customer's service order/subscription
/// </summary>
public class Order : EntityBase
{
    /// <summary>
    /// Human-readable order number (e.g., "ORD-2024-00001")
    /// </summary>
    public string OrderNumber { get; set; } = string.Empty;

    /// <summary>
    /// Foreign key to the customer
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// Foreign key to the service
    /// </summary>
    public int ServiceId { get; set; }

    /// <summary>
    /// Foreign key to the originating quote (if applicable)
    /// </summary>
    public int? QuoteId { get; set; }

    /// <summary>
    /// Foreign key to the coupon applied to this order
    /// </summary>
    public int? CouponId { get; set; }

    /// <summary>
    /// Type of order
    /// </summary>
    public OrderType OrderType { get; set; } = OrderType.New;

    /// <summary>
    /// Current status of the order
    /// </summary>
    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    /// <summary>
    /// Date when the service starts
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Date when the service ends
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// Next billing date for recurring charges
    /// </summary>
    public DateTime NextBillingDate { get; set; }

    /// <summary>
    /// One-time setup fee charged
    /// </summary>
    public decimal SetupFee { get; set; }

    /// <summary>
    /// Recurring amount charged per billing cycle
    /// </summary>
    public decimal RecurringAmount { get; set; }

    /// <summary>
    /// Discount amount applied to this order
    /// </summary>
    public decimal DiscountAmount { get; set; }
    
    /// <summary>
    /// Currency code for this order (ISO 4217, e.g., "EUR", "USD")
    /// </summary>
    public string CurrencyCode { get; set; } = "EUR";
    
    /// <summary>
    /// Base currency for accounting purposes (e.g., "EUR")
    /// </summary>
    public string BaseCurrencyCode { get; set; } = "EUR";
    
    /// <summary>
    /// Exchange rate applied at order creation (from base to order currency)
    /// Null if base and order currencies are the same
    /// </summary>
    public decimal? ExchangeRate { get; set; }
    
    /// <summary>
    /// Date of the exchange rate used
    /// </summary>
    public DateTime? ExchangeRateDate { get; set; }

    /// <summary>
    /// Trial period end date (if applicable)
    /// </summary>
    public DateTime? TrialEndsAt { get; set; }

    /// <summary>
    /// Date when the service was suspended
    /// </summary>
    public DateTime? SuspendedAt { get; set; }

    /// <summary>
    /// Reason for suspension
    /// </summary>
    public string SuspensionReason { get; set; } = string.Empty;

    /// <summary>
    /// Date when the order was cancelled
    /// </summary>
    public DateTime? CancelledAt { get; set; }

    /// <summary>
    /// Reason for cancellation
    /// </summary>
    public string CancellationReason { get; set; } = string.Empty;

    /// <summary>
    /// Whether the service should auto-renew
    /// </summary>
    public bool AutoRenew { get; set; } = true;

    /// <summary>
    /// Whether renewal reminder has been sent
    /// </summary>
    public bool RenewalReminderSent { get; set; }

    /// <summary>
    /// Additional notes about this order
    /// </summary>
    public string Notes { get; set; } = string.Empty;

    /// <summary>
    /// Internal comments not visible to customer
    /// </summary>
    public string InternalComment { get; set; } = string.Empty;

    /// <summary>
    /// Navigation property to the customer
    /// </summary>
    public Customer Customer { get; set; } = null!;

    /// <summary>
    /// Navigation property to the service
    /// </summary>
    public Service Service { get; set; } = null!;

    /// <summary>
    /// Navigation property to the originating quote
    /// </summary>
    public Quote? Quote { get; set; }

    /// <summary>
    /// Navigation property to the applied coupon
    /// </summary>
    public Coupon? Coupon { get; set; }

    /// <summary>
    /// Collection of invoices for this order
    /// </summary>
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    /// <summary>
    /// Collection of payment intents for this order
    /// </summary>
    public ICollection<PaymentIntent> PaymentIntents { get; set; } = new List<PaymentIntent>();

    /// <summary>
    /// Collection of coupon usages for this order
    /// </summary>
    public ICollection<CouponUsage> CouponUsages { get; set; } = new List<CouponUsage>();
}

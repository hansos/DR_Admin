using ISPAdmin.Data.Enums;

namespace ISPAdmin.Data.Entities;

/// <summary>
/// Represents a sales quote/proposal for services
/// </summary>
public class Quote : EntityBase
{
    /// <summary>
    /// Human-readable quote number (e.g., "Q-2024-00001")
    /// </summary>
    public string QuoteNumber { get; set; } = string.Empty;

    /// <summary>
    /// Foreign key to the customer
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// Current status of the quote
    /// </summary>
    public QuoteStatus Status { get; set; } = QuoteStatus.Draft;

    /// <summary>
    /// Date when the quote expires and is no longer valid
    /// </summary>
    public DateTime ValidUntil { get; set; }

    /// <summary>
    /// Subtotal amount before tax
    /// </summary>
    public decimal SubTotal { get; set; }

    /// <summary>
    /// Total setup fees (one-time charges)
    /// </summary>
    public decimal TotalSetupFee { get; set; }

    /// <summary>
    /// Total recurring charges
    /// </summary>
    public decimal TotalRecurring { get; set; }

    /// <summary>
    /// Total tax amount
    /// </summary>
    public decimal TaxAmount { get; set; }

    /// <summary>
    /// Grand total including tax
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Currency code (e.g., "USD", "EUR")
    /// </summary>
    public string CurrencyCode { get; set; } = "EUR";

    /// <summary>
    /// Tax rate applied to the quote
    /// </summary>
    public decimal TaxRate { get; set; }

    /// <summary>
    /// Name of the tax (e.g., "VAT", "GST")
    /// </summary>
    public string TaxName { get; set; } = "VAT";

    /// <summary>
    /// Billing snapshot of customer name
    /// </summary>
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// Billing snapshot of customer address
    /// </summary>
    public string CustomerAddress { get; set; } = string.Empty;

    /// <summary>
    /// Billing snapshot of customer tax ID
    /// </summary>
    public string CustomerTaxId { get; set; } = string.Empty;

    /// <summary>
    /// Additional notes for the customer
    /// </summary>
    public string Notes { get; set; } = string.Empty;

    /// <summary>
    /// Terms and conditions text
    /// </summary>
    public string TermsAndConditions { get; set; } = string.Empty;

    /// <summary>
    /// Internal comments not visible to customer
    /// </summary>
    public string InternalComment { get; set; } = string.Empty;

    /// <summary>
    /// Date when the quote was sent to the customer
    /// </summary>
    public DateTime? SentAt { get; set; }

    /// <summary>
    /// Date when the customer accepted the quote
    /// </summary>
    public DateTime? AcceptedAt { get; set; }

    /// <summary>
    /// Date when the customer rejected the quote
    /// </summary>
    public DateTime? RejectedAt { get; set; }

    /// <summary>
    /// Reason for rejection (if applicable)
    /// </summary>
    public string RejectionReason { get; set; } = string.Empty;

    /// <summary>
    /// Unique token for customer acceptance via link
    /// </summary>
    public string AcceptanceToken { get; set; } = string.Empty;

    /// <summary>
    /// ID of the user who prepared the quote
    /// </summary>
    public int? PreparedByUserId { get; set; }

    /// <summary>
    /// Coupon code applied to the quote
    /// </summary>
    public int? CouponId { get; set; }

    /// <summary>
    /// Total discount amount applied
    /// </summary>
    public decimal DiscountAmount { get; set; }

    /// <summary>
    /// Soft delete timestamp
    /// </summary>
    public DateTime? DeletedAt { get; set; }

    /// <summary>
    /// Navigation property to the customer
    /// </summary>
    public Customer Customer { get; set; } = null!;

    /// <summary>
    /// Navigation property to the user who prepared the quote
    /// </summary>
    public User? PreparedByUser { get; set; }

    /// <summary>
    /// Navigation property to the applied coupon
    /// </summary>
    public Coupon? Coupon { get; set; }

    /// <summary>
    /// Collection of line items in the quote
    /// </summary>
    public ICollection<QuoteLine> QuoteLines { get; set; } = new List<QuoteLine>();

    /// <summary>
    /// Collection of orders created from this quote
    /// </summary>
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}

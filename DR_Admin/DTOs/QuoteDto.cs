using ISPAdmin.Data.Enums;

namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object representing a quote
/// </summary>
public class QuoteDto
{
    /// <summary>
    /// Unique identifier for the quote
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Human-readable quote number
    /// </summary>
    public string QuoteNumber { get; set; } = string.Empty;

    /// <summary>
    /// Customer ID
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// Customer name
    /// </summary>
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// Current status of the quote
    /// </summary>
    public QuoteStatus Status { get; set; }

    /// <summary>
    /// Date when the quote expires
    /// </summary>
    public DateTime ValidUntil { get; set; }

    /// <summary>
    /// Subtotal amount before tax
    /// </summary>
    public decimal SubTotal { get; set; }

    /// <summary>
    /// Total setup fees
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
    /// Currency code
    /// </summary>
    public string CurrencyCode { get; set; } = "EUR";

    /// <summary>
    /// Tax rate applied
    /// </summary>
    public decimal TaxRate { get; set; }

    /// <summary>
    /// Name of the tax
    /// </summary>
    public string TaxName { get; set; } = "VAT";

    /// <summary>
    /// Customer address snapshot
    /// </summary>
    public string CustomerAddress { get; set; } = string.Empty;

    /// <summary>
    /// Customer tax ID snapshot
    /// </summary>
    public string CustomerTaxId { get; set; } = string.Empty;

    /// <summary>
    /// Additional notes
    /// </summary>
    public string Notes { get; set; } = string.Empty;

    /// <summary>
    /// Terms and conditions
    /// </summary>
    public string TermsAndConditions { get; set; } = string.Empty;

    /// <summary>
    /// Internal comment
    /// </summary>
    public string InternalComment { get; set; } = string.Empty;

    /// <summary>
    /// Date when the quote was sent
    /// </summary>
    public DateTime? SentAt { get; set; }

    /// <summary>
    /// Date when the quote was accepted
    /// </summary>
    public DateTime? AcceptedAt { get; set; }

    /// <summary>
    /// Date when the quote was rejected
    /// </summary>
    public DateTime? RejectedAt { get; set; }

    /// <summary>
    /// Rejection reason
    /// </summary>
    public string RejectionReason { get; set; } = string.Empty;

    /// <summary>
    /// ID of the user who prepared the quote
    /// </summary>
    public int? PreparedByUserId { get; set; }

    /// <summary>
    /// Coupon ID
    /// </summary>
    public int? CouponId { get; set; }

    /// <summary>
    /// Total discount amount
    /// </summary>
    public decimal DiscountAmount { get; set; }

    /// <summary>
    /// Date created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

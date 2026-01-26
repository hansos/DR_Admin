namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object for creating a new quote
/// </summary>
public class CreateQuoteDto
{
    /// <summary>
    /// Customer ID
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// Date when the quote expires
    /// </summary>
    public DateTime ValidUntil { get; set; }

    /// <summary>
    /// Additional notes for the customer
    /// </summary>
    public string Notes { get; set; } = string.Empty;

    /// <summary>
    /// Terms and conditions text
    /// </summary>
    public string TermsAndConditions { get; set; } = string.Empty;

    /// <summary>
    /// Internal comment not visible to customer
    /// </summary>
    public string InternalComment { get; set; } = string.Empty;

    /// <summary>
    /// Coupon code to apply (optional)
    /// </summary>
    public string? CouponCode { get; set; }

    /// <summary>
    /// Collection of quote line items
    /// </summary>
    public List<CreateQuoteLineDto> Lines { get; set; } = new();
}

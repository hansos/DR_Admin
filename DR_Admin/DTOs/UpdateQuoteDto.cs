namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object for updating an existing quote
/// </summary>
public class UpdateQuoteDto
{
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
    /// Collection of quote line items
    /// </summary>
    public List<UpdateQuoteLineDto> Lines { get; set; } = new();
}

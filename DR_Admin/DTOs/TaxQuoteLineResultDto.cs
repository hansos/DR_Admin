namespace ISPAdmin.DTOs;

/// <summary>
/// Represents tax calculation result for a single line.
/// </summary>
public class TaxQuoteLineResultDto
{
    /// <summary>
    /// Gets or sets optional source line identifier.
    /// </summary>
    public int? LineId { get; set; }

    /// <summary>
    /// Gets or sets line description.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets net amount.
    /// </summary>
    public decimal NetAmount { get; set; }

    /// <summary>
    /// Gets or sets applied tax rate.
    /// </summary>
    public decimal TaxRate { get; set; }

    /// <summary>
    /// Gets or sets tax amount.
    /// </summary>
    public decimal TaxAmount { get; set; }

    /// <summary>
    /// Gets or sets gross amount.
    /// </summary>
    public decimal GrossAmount { get; set; }
}

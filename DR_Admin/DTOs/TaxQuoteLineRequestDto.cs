namespace ISPAdmin.DTOs;

/// <summary>
/// Represents an input line for tax quote or finalize calculation.
/// </summary>
public class TaxQuoteLineRequestDto
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
    /// Gets or sets tax category code.
    /// </summary>
    public string TaxCategory { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets net amount for this line.
    /// </summary>
    public decimal NetAmount { get; set; }
}

namespace ISPAdmin.DTOs;

/// <summary>
/// Represents tax calculation quote or finalization output.
/// </summary>
public class TaxQuoteResultDto
{
    /// <summary>
    /// Gets or sets optional persisted snapshot identifier.
    /// </summary>
    public int? SnapshotId { get; set; }

    /// <summary>
    /// Gets or sets optional order identifier.
    /// </summary>
    public int? OrderId { get; set; }

    /// <summary>
    /// Gets or sets optional tax jurisdiction identifier.
    /// </summary>
    public int? TaxJurisdictionId { get; set; }

    /// <summary>
    /// Gets or sets applied tax name.
    /// </summary>
    public string TaxName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets applied tax rate.
    /// </summary>
    public decimal TaxRate { get; set; }

    /// <summary>
    /// Gets or sets whether reverse charge was applied.
    /// </summary>
    public bool ReverseChargeApplied { get; set; }

    /// <summary>
    /// Gets or sets rule version token used for calculation.
    /// </summary>
    public string RuleVersion { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets tax currency code.
    /// </summary>
    public string TaxCurrencyCode { get; set; } = "EUR";

    /// <summary>
    /// Gets or sets display currency code.
    /// </summary>
    public string DisplayCurrencyCode { get; set; } = "EUR";

    /// <summary>
    /// Gets or sets net amount total.
    /// </summary>
    public decimal NetAmount { get; set; }

    /// <summary>
    /// Gets or sets tax amount total.
    /// </summary>
    public decimal TaxAmount { get; set; }

    /// <summary>
    /// Gets or sets gross amount total.
    /// </summary>
    public decimal GrossAmount { get; set; }

    /// <summary>
    /// Gets or sets whether buyer tax identifier was validated.
    /// </summary>
    public bool BuyerTaxIdValidated { get; set; }

    /// <summary>
    /// Gets or sets legal note to display on invoice or checkout.
    /// </summary>
    public string LegalNote { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets calculated line results.
    /// </summary>
    public ICollection<TaxQuoteLineResultDto> Lines { get; set; } = new List<TaxQuoteLineResultDto>();
}

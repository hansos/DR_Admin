using ISPAdmin.Data.Enums;

namespace ISPAdmin.Data.Entities;

/// <summary>
/// Stores immutable tax calculation inputs and outputs for an order at a specific point in time.
/// </summary>
public class OrderTaxSnapshot : EntityBase
{
    /// <summary>
    /// Foreign key to the order.
    /// </summary>
    public int OrderId { get; set; }

    /// <summary>
    /// Optional foreign key to the applied tax jurisdiction.
    /// </summary>
    public int? TaxJurisdictionId { get; set; }

    /// <summary>
    /// Buyer country code used for tax determination.
    /// </summary>
    public string BuyerCountryCode { get; set; } = string.Empty;

    /// <summary>
    /// Optional buyer state/province code used for tax determination.
    /// </summary>
    public string? BuyerStateCode { get; set; }

    /// <summary>
    /// Buyer type used for tax determination.
    /// </summary>
    public CustomerType BuyerType { get; set; } = CustomerType.B2C;

    /// <summary>
    /// Buyer tax identifier used during calculation.
    /// </summary>
    public string BuyerTaxId { get; set; } = string.Empty;

    /// <summary>
    /// Indicates whether the buyer tax ID was validated at snapshot time.
    /// </summary>
    public bool BuyerTaxIdValidated { get; set; }

    /// <summary>
    /// Tax currency used to calculate and report tax.
    /// </summary>
    public string TaxCurrencyCode { get; set; } = "EUR";

    /// <summary>
    /// Customer-facing display currency.
    /// </summary>
    public string DisplayCurrencyCode { get; set; } = "EUR";

    /// <summary>
    /// Exchange rate used to convert tax currency to display currency.
    /// </summary>
    public decimal? ExchangeRate { get; set; }

    /// <summary>
    /// Timestamp of the FX rate used.
    /// </summary>
    public DateTime? ExchangeRateDate { get; set; }

    /// <summary>
    /// Taxable amount before tax.
    /// </summary>
    public decimal NetAmount { get; set; }

    /// <summary>
    /// Calculated tax amount.
    /// </summary>
    public decimal TaxAmount { get; set; }

    /// <summary>
    /// Total amount including tax.
    /// </summary>
    public decimal GrossAmount { get; set; }

    /// <summary>
    /// Applied tax rate at calculation time.
    /// </summary>
    public decimal AppliedTaxRate { get; set; }

    /// <summary>
    /// Applied tax name (e.g., VAT, GST, Sales Tax).
    /// </summary>
    public string AppliedTaxName { get; set; } = "VAT";

    /// <summary>
    /// Indicates whether reverse charge was applied.
    /// </summary>
    public bool ReverseChargeApplied { get; set; }

    /// <summary>
    /// Version token for the rule set used by the calculator.
    /// </summary>
    public string RuleVersion { get; set; } = string.Empty;

    /// <summary>
    /// Serialized tax calculation inputs and evidence used for audit.
    /// </summary>
    public string CalculationInputsJson { get; set; } = string.Empty;

    /// <summary>
    /// Navigation property to the order.
    /// </summary>
    public Order Order { get; set; } = null!;

    /// <summary>
    /// Navigation property to the applied jurisdiction.
    /// </summary>
    public TaxJurisdiction? TaxJurisdiction { get; set; }
}

using ISPAdmin.Data.Enums;

namespace ISPAdmin.DTOs;

/// <summary>
/// Represents tax quote or finalize input payload.
/// </summary>
public class TaxQuoteRequestDto
{
    /// <summary>
    /// Gets or sets optional order identifier.
    /// </summary>
    public int? OrderId { get; set; }

    /// <summary>
    /// Gets or sets optional customer identifier.
    /// </summary>
    public int? CustomerId { get; set; }

    /// <summary>
    /// Gets or sets buyer country code.
    /// </summary>
    public string BuyerCountryCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets optional buyer state code.
    /// </summary>
    public string? BuyerStateCode { get; set; }

    /// <summary>
    /// Gets or sets buyer type.
    /// </summary>
    public CustomerType BuyerType { get; set; } = CustomerType.B2C;

    /// <summary>
    /// Gets or sets buyer tax identifier.
    /// </summary>
    public string BuyerTaxId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether the service should validate buyer tax ID.
    /// </summary>
    public bool ValidateBuyerTaxId { get; set; } = true;

    /// <summary>
    /// Gets or sets transaction date used for effective tax rule lookup.
    /// </summary>
    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets tax currency code.
    /// </summary>
    public string TaxCurrencyCode { get; set; } = "EUR";

    /// <summary>
    /// Gets or sets display currency code.
    /// </summary>
    public string DisplayCurrencyCode { get; set; } = "EUR";

    /// <summary>
    /// Gets or sets optional exchange rate from tax currency to display currency.
    /// </summary>
    public decimal? ExchangeRate { get; set; }

    /// <summary>
    /// Gets or sets optional exchange rate timestamp.
    /// </summary>
    public DateTime? ExchangeRateDate { get; set; }

    /// <summary>
    /// Gets or sets whether trusted server-side exchange rate is required when currencies differ.
    /// </summary>
    public bool RequireTrustedExchangeRate { get; set; } = true;

    /// <summary>
    /// Gets or sets billing country code used as location evidence.
    /// </summary>
    public string BillingCountryCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets source IP used as location evidence.
    /// </summary>
    public string IpAddress { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets optional idempotency key for finalize operation.
    /// </summary>
    public string? IdempotencyKey { get; set; }

    /// <summary>
    /// Gets or sets lines for calculation.
    /// </summary>
    public ICollection<TaxQuoteLineRequestDto> Lines { get; set; } = new List<TaxQuoteLineRequestDto>();
}

namespace ISPAdmin.Data.Entities;

/// <summary>
/// Represents a supported ISO 4217 currency in the system.
/// </summary>
public class Currency : EntityBase
{
    /// <summary>
    /// Gets or sets the ISO 4217 currency code (for example, EUR or USD).
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the display name of the currency.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the symbol used to display the currency.
    /// </summary>
    public string? Symbol { get; set; }

    /// <summary>
    /// Gets or sets whether the currency is active and available for use.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets whether this is the default system currency.
    /// </summary>
    public bool IsDefault { get; set; }

    /// <summary>
    /// Gets or sets whether this currency can be used for customer-facing pricing and transactions.
    /// </summary>
    public bool IsCustomerCurrency { get; set; } = true;

    /// <summary>
    /// Gets or sets whether this currency can be used for provider/vendor-facing pricing and settlements.
    /// </summary>
    public bool IsProviderCurrency { get; set; } = true;

    /// <summary>
    /// Gets or sets the sort order used when listing currencies.
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// Gets or sets the normalized version of <see cref="Code"/>.
    /// </summary>
    public string NormalizedCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the normalized version of <see cref="Name"/>.
    /// </summary>
    public string NormalizedName { get; set; } = string.Empty;
}

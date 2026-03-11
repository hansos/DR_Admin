namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object representing a supported currency.
/// </summary>
public class CurrencyDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the currency.
    /// </summary>
    public int Id { get; set; }

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
    /// Gets or sets whether the currency is active.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Gets or sets whether this is the default currency.
    /// </summary>
    public bool IsDefault { get; set; }

    /// <summary>
    /// Gets or sets whether this currency can be used for customer-facing operations.
    /// </summary>
    public bool IsCustomerCurrency { get; set; }

    /// <summary>
    /// Gets or sets whether this currency can be used for provider-facing operations.
    /// </summary>
    public bool IsProviderCurrency { get; set; }

    /// <summary>
    /// Gets or sets the sort order for displaying the currency.
    /// </summary>
    public int SortOrder { get; set; }
}

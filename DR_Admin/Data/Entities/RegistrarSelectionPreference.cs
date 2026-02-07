namespace ISPAdmin.Data.Entities;

/// <summary>
/// Represents preferences for selecting a registrar when multiple options are available
/// Used to implement business logic: choose cheapest, but prefer bundling
/// </summary>
public class RegistrarSelectionPreference : EntityBase
{
    /// <summary>
    /// Foreign key to the Registrar
    /// </summary>
    public int RegistrarId { get; set; }

    /// <summary>
    /// Priority level for this registrar (lower number = higher priority)
    /// Used when costs are similar
    /// </summary>
    public int Priority { get; set; } = 100;

    /// <summary>
    /// Indicates if this registrar offers hosting services
    /// Used for bundling preference
    /// </summary>
    public bool OffersHosting { get; set; } = false;

    /// <summary>
    /// Indicates if this registrar offers email services
    /// Used for bundling preference
    /// </summary>
    public bool OffersEmail { get; set; } = false;

    /// <summary>
    /// Indicates if this registrar offers SSL certificate services
    /// Used for bundling preference
    /// </summary>
    public bool OffersSsl { get; set; } = false;

    /// <summary>
    /// Maximum cost difference (in USD) where bundling preference applies
    /// Example: If 2.00, will choose bundled registrar if cost is within $2 of cheapest
    /// </summary>
    public decimal? MaxCostDifferenceThreshold { get; set; } = 2.00m;

    /// <summary>
    /// Whether to prefer this registrar when customer has hosting with them
    /// </summary>
    public bool PreferForHostingCustomers { get; set; } = false;

    /// <summary>
    /// Whether to prefer this registrar when customer has email with them
    /// </summary>
    public bool PreferForEmailCustomers { get; set; } = false;

    /// <summary>
    /// Indicates if this preference is still active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Optional notes about this preference configuration
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Navigation property to the Registrar
    /// </summary>
    public Registrar Registrar { get; set; } = null!;
}

namespace ISPAdmin.Data.Enums;

/// <summary>
/// Defines supported product classes for margin configuration.
/// </summary>
public enum ProfitProductClass
{
    /// <summary>
    /// Domain TLD products.
    /// </summary>
    Tld = 1,

    /// <summary>
    /// Hosting server and hosting package products.
    /// </summary>
    Hosting = 2,

    /// <summary>
    /// Additional optional services and add-ons.
    /// </summary>
    AdditionalService = 3,

    /// <summary>
    /// Domain registration extras and related services.
    /// </summary>
    DomainService = 4,

    /// <summary>
    /// Other products not covered by dedicated classes.
    /// </summary>
    Other = 99
}

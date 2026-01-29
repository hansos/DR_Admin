namespace ISPAdmin.Data.Entities;

public class Country : EntityBase
{
    /// <summary>
    /// Iso2
    /// </summary>
    public string Code { get; set; } = string.Empty;


    public string Tld { get; set; } = string.Empty;
    // Iso3 (nullable)
    public string? Iso3 { get; set; }
    // Numeric (nullable)
    public int? Numeric { get; set; }
    public string EnglishName { get; set; } = string.Empty;
    public string LocalName { get; set; } = string.Empty;
    public bool IsActive { get; set; }

    /// <summary>
    /// Normalized version of EnglishName for case-insensitive searches
    /// </summary>
    public string NormalizedEnglishName { get; set; } = string.Empty;

    /// <summary>
    /// Normalized version of LocalName for case-insensitive searches
    /// </summary>
    public string NormalizedLocalName { get; set; } = string.Empty;

    public ICollection<Customer> Customers { get; set; } = new List<Customer>();
    public ICollection<PostalCode> PostalCodes { get; set; } = new List<PostalCode>();
}

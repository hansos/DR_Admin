namespace ISPAdmin.Data.Entities;

public class ResellerCompany : EntityBase
{
    public string Name { get; set; } = string.Empty;
    public string? ContactPerson { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string? CountryCode { get; set; }
    public string? CompanyRegistrationNumber { get; set; }
    public string? TaxId { get; set; }
    public string? VatNumber { get; set; }
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Indicates if this is the default reseller company. Only one reseller can be default at a time.
    /// </summary>
    public bool IsDefault { get; set; } = false;
    
    public string? Notes { get; set; }
    
    /// <summary>
    /// Default currency for this reseller (ISO 4217 code, e.g., "EUR", "USD")
    /// </summary>
    public string DefaultCurrency { get; set; } = "EUR";
    
    /// <summary>
    /// Comma-separated list of supported currencies for this reseller (e.g., "EUR,USD,GBP")
    /// </summary>
    public string? SupportedCurrencies { get; set; }
    
    /// <summary>
    /// Whether to automatically apply currency markup for this reseller
    /// </summary>
    public bool ApplyCurrencyMarkup { get; set; } = false;
    
    /// <summary>
    /// Default currency markup percentage (e.g., 2.5 for 2.5%)
    /// </summary>
    public decimal DefaultCurrencyMarkup { get; set; } = 0m;

    public ICollection<SalesAgent> SalesAgents { get; set; } = new List<SalesAgent>();
    public ICollection<DnsZonePackage> DnsZonePackages { get; set; } = new List<DnsZonePackage>();
    public ICollection<Service> Services { get; set; } = new List<Service>();
}

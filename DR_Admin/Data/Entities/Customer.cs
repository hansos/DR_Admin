namespace ISPAdmin.Data.Entities;

public class Customer : EntityBase
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string? CountryCode { get; set; }
    public string? CompanyName { get; set; }
    public string? TaxId { get; set; }
    public string? VatNumber { get; set; }
    public string? ContactPerson { get; set; }
    public bool IsCompany { get; set; }
    public bool IsActive { get; set; }
    public string Status { get; set; } = "Active";

    /// <summary>
    /// Normalized version of Name for case-insensitive searches
    /// </summary>
    public string NormalizedName { get; set; } = string.Empty;

    /// <summary>
    /// Normalized version of CompanyName for case-insensitive searches
    /// </summary>
    public string? NormalizedCompanyName { get; set; }

    /// <summary>
    /// Normalized version of ContactPerson for case-insensitive searches
    /// </summary>
    public string? NormalizedContactPerson { get; set; }
    public decimal Balance { get; set; }
    public decimal CreditLimit { get; set; }
    public string? Notes { get; set; }
    public string? BillingEmail { get; set; }
    public string? PreferredPaymentMethod { get; set; }
    
    /// <summary>
    /// Customer's preferred currency for invoices and transactions (ISO 4217 code, e.g., "EUR", "USD")
    /// </summary>
    public string PreferredCurrency { get; set; } = "EUR";
    
    /// <summary>
    /// Whether to allow currency override at transaction level
    /// </summary>
    public bool AllowCurrencyOverride { get; set; } = true;

    public Country? Country { get; set; }
    public ICollection<User> Users { get; set; } = new List<User>();
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<Domain> Domains { get; set; } = new List<Domain>();
    public ICollection<HostingAccount> HostingAccounts { get; set; } = new List<HostingAccount>();
}

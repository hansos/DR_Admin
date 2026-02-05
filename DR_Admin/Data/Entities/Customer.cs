namespace ISPAdmin.Data.Entities;

public class Customer : EntityBase
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;

    // Address fields are managed via CustomerAddress records
    public string? CustomerName { get; set; }
    public string? TaxId { get; set; }
    public string? VatNumber { get; set; }
    public bool IsCompany { get; set; }
    public bool IsActive { get; set; }
    public string Status { get; set; } = "Active";
    
    /// <summary>
    /// Gets or sets the customer status ID (foreign key to CustomerStatus table)
    /// </summary>
    public int? CustomerStatusId { get; set; }

    /// <summary>
    /// Normalized version of Name for case-insensitive searches
    /// </summary>
    public string NormalizedName { get; set; } = string.Empty;

    /// <summary>
    /// Normalized version of CustomerName for case-insensitive searches
    /// </summary>
    public string? NormalizedCustomerName { get; set; }
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

    // Country navigation removed; country is represented via CustomerAddress -> PostalCode -> Country
    public CustomerStatus? CustomerStatus { get; set; }
    public ICollection<User> Users { get; set; } = new List<User>();
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<RegisteredDomain> RegisteredDomains { get; set; } = new List<RegisteredDomain>();
    public ICollection<HostingAccount> HostingAccounts { get; set; } = new List<HostingAccount>();
    public ICollection<CustomerAddress> CustomerAddresses { get; set; } = new List<CustomerAddress>();
}

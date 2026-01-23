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
    public decimal Balance { get; set; }
    public decimal CreditLimit { get; set; }
    public string? Notes { get; set; }
    public string? BillingEmail { get; set; }
    public string? PreferredPaymentMethod { get; set; }

    public Country? Country { get; set; }
    public ICollection<User> Users { get; set; } = new List<User>();
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<Domain> Domains { get; set; } = new List<Domain>();
    public ICollection<HostingAccount> HostingAccounts { get; set; } = new List<HostingAccount>();
}

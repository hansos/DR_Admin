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
    public string? Notes { get; set; }

    public ICollection<SalesAgent> SalesAgents { get; set; } = new List<SalesAgent>();
    public ICollection<DnsZonePackage> DnsZonePackages { get; set; } = new List<DnsZonePackage>();
    public ICollection<Service> Services { get; set; } = new List<Service>();
}

namespace ISPAdmin.Data.Entities;

public class Domain : EntityBase
{
    public int CustomerId { get; set; }
    public int ServiceId { get; set; }
    public required string Name { get; set; }
    public int RegistrarId { get; set; }
    public int? RegistrarTldId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime RegistrationDate { get; set; }
    public DateTime ExpirationDate { get; set; }
    public bool AutoRenew { get; set; }
    public bool PrivacyProtection { get; set; }
    public decimal? RegistrationPrice { get; set; }
    public decimal? RenewalPrice { get; set; }
    public string? Notes { get; set; }

    /// <summary>
    /// Normalized version of Name for case-insensitive searches
    /// </summary>
    public string NormalizedName { get; set; } = string.Empty;

    public Customer Customer { get; set; } = null!;
    public Service Service { get; set; } = null!;
    public Registrar Registrar { get; set; } = null!;
    public RegistrarTld? RegistrarTld { get; set; }
    public ICollection<DnsRecord> DnsRecords { get; set; } = new List<DnsRecord>();
    public ICollection<DomainContact> DomainContacts { get; set; } = new List<DomainContact>();
}

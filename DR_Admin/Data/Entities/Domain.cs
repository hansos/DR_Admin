namespace ISPAdmin.Data.Entities;

public class Domain
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string Name { get; set; } = string.Empty;
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

    public Customer Customer { get; set; } = null!;
    public Registrar Registrar { get; set; } = null!;
    public RegistrarTld? RegistrarTld { get; set; }
    public ICollection<DnsRecord> DnsRecords { get; set; } = new List<DnsRecord>();
}

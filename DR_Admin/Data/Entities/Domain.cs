namespace ISPAdmin.Data.Entities;

public class Domain
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int ProviderId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime RegistrationDate { get; set; }
    public DateTime ExpirationDate { get; set; }

    public Customer Customer { get; set; } = null!;
    public DomainProvider Provider { get; set; } = null!;
    public ICollection<DnsRecord> DnsRecords { get; set; } = new List<DnsRecord>();
}

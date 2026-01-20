namespace ISPAdmin.Data.Entities;

public class DnsRecord
{
    public int Id { get; set; }
    public int DomainId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public int TTL { get; set; }

    public Domain Domain { get; set; } = null!;
}

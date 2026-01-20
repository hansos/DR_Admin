namespace ISPAdmin.DTOs;

public class DnsRecordDto
{
    public int Id { get; set; }
    public int DomainId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public int TTL { get; set; }
}

public class CreateDnsRecordDto
{
    public int DomainId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public int TTL { get; set; }
}

public class UpdateDnsRecordDto
{
    public int DomainId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public int TTL { get; set; }
}

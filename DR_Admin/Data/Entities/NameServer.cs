namespace ISPAdmin.Data.Entities;

public class NameServer : EntityBase
{
    public int? ServerId { get; set; }
    public string Hostname { get; set; } = string.Empty;
    public string? IpAddress { get; set; }
    public bool IsPrimary { get; set; } = false;
    public int SortOrder { get; set; } = 0;

    public Server? Server { get; set; }
    public ICollection<NameServerDomain> NameServerDomains { get; set; } = new List<NameServerDomain>();
}

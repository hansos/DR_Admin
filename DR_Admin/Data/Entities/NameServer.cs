namespace ISPAdmin.Data.Entities;

public class NameServer : EntityBase
{
    public int DomainId { get; set; }
    public string Hostname { get; set; } = string.Empty;
    public string? IpAddress { get; set; }
    public bool IsPrimary { get; set; } = false;
    public int SortOrder { get; set; } = 0;

    public Domain Domain { get; set; } = null!;
}

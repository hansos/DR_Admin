namespace ISPAdmin.Data.Entities;

public class NameServerDomain : EntityBase
{
    public int NameServerId { get; set; }
    public int DomainId { get; set; }

    public NameServer NameServer { get; set; } = null!;
    public RegisteredDomain Domain { get; set; } = null!;
}

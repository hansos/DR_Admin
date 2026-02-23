namespace ISPAdmin.Data.Entities;

public class DnsZonePackageServer
{
    public int DnsZonePackageId { get; set; }
    public int ServerId { get; set; }

    public DnsZonePackage DnsZonePackage { get; set; } = null!;
    public Server Server { get; set; } = null!;
}

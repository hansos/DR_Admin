namespace ISPAdmin.Data.Entities;

public class DnsZonePackageControlPanel
{
    public int DnsZonePackageId { get; set; }
    public int ServerControlPanelId { get; set; }

    public DnsZonePackage DnsZonePackage { get; set; } = null!;
    public ServerControlPanel ServerControlPanel { get; set; } = null!;
}

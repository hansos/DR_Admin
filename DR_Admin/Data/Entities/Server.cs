namespace ISPAdmin.Data.Entities;

public class Server : EntityBase
{
    public string Name { get; set; } = string.Empty;
    public int ServerTypeId { get; set; }
    public int? HostProviderId { get; set; }
    public string? Location { get; set; }
    public int OperatingSystemId { get; set; }
    public bool? Status { get; set; } = true;
    public int? CpuCores { get; set; }
    public int? RamMB { get; set; }
    public int? DiskSpaceGB { get; set; }
    public string? Notes { get; set; }

    public ServerType ServerType { get; set; } = null!;
    public HostProvider? HostProvider { get; set; }
    public OperatingSystem OperatingSystem { get; set; } = null!;

    public ICollection<ServerIpAddress> IpAddresses { get; set; } = new List<ServerIpAddress>();
    public ICollection<ServerControlPanel> ControlPanels { get; set; } = new List<ServerControlPanel>();
    public ICollection<HostingAccount> HostingAccounts { get; set; } = new List<HostingAccount>();
    public ICollection<DnsZonePackageServer> DnsZonePackages { get; set; } = new List<DnsZonePackageServer>();
}

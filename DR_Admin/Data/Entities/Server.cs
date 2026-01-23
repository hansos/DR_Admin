namespace ISPAdmin.Data.Entities;

public class Server : EntityBase
{
    public string Name { get; set; } = string.Empty;
    public string ServerType { get; set; } = string.Empty; // Physical, Cloud, Virtual
    public string? HostProvider { get; set; } // AWS, Azure, DigitalOcean, etc.
    public string? Location { get; set; }
    public string OperatingSystem { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty; // Active, Inactive, Maintenance
    public int? CpuCores { get; set; }
    public int? RamMB { get; set; }
    public int? DiskSpaceGB { get; set; }
    public string? Notes { get; set; }

    public ICollection<ServerIpAddress> IpAddresses { get; set; } = new List<ServerIpAddress>();
    public ICollection<ServerControlPanel> ControlPanels { get; set; } = new List<ServerControlPanel>();
    public ICollection<HostingAccount> HostingAccounts { get; set; } = new List<HostingAccount>();
}

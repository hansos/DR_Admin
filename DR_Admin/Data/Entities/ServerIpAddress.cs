namespace ISPAdmin.Data.Entities;

public class ServerIpAddress : EntityBase
{
    public int ServerId { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public string IpVersion { get; set; } = "IPv4"; // IPv4 or IPv6
    public bool IsPrimary { get; set; }
    public string Status { get; set; } = "Active"; // Active, Reserved, Blocked
    public string? AssignedTo { get; set; } // Domain or service using this IP
    public string? Notes { get; set; }

    public Server Server { get; set; } = null!;
    public ICollection<ServerControlPanel> ControlPanels { get; set; } = new List<ServerControlPanel>();
}

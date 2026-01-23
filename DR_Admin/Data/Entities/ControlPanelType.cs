namespace ISPAdmin.Data.Entities;

public class ControlPanelType : EntityBase
{
    public string Name { get; set; } = string.Empty; // cPanel, Plesk, DirectAdmin, ISPConfig, Virtualmin, CyberPanel, CloudPanel
    public string DisplayName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Version { get; set; }
    public string? WebsiteUrl { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<ServerControlPanel> ServerControlPanels { get; set; } = new List<ServerControlPanel>();
}

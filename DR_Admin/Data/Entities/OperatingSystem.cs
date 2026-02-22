namespace ISPAdmin.Data.Entities;

public class OperatingSystem : EntityBase
{
    public string Name { get; set; } = string.Empty; // ubuntu, windows-server-2022
    public string DisplayName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Version { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<Server> Servers { get; set; } = new List<Server>();
}

namespace ISPAdmin.Data.Entities;

public class HostProvider : EntityBase
{
    public string Name { get; set; } = string.Empty; // aws, azure, digitalocean
    public string DisplayName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? WebsiteUrl { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<Server> Servers { get; set; } = new List<Server>();
}

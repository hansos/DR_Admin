namespace ISPAdmin.Data.Entities;

public class ServerType : EntityBase
{
    public string Name { get; set; } = string.Empty; // physical, cloud, virtual
    public string DisplayName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<Server> Servers { get; set; } = new List<Server>();
}

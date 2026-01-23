namespace ISPAdmin.Data.Entities;

public class ServiceType
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ICollection<Service> Services { get; set; } = new List<Service>();
}

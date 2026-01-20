namespace ISPAdmin.Data.Entities;

public class BillingCycle
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int DurationInDays { get; set; }
    public string Description { get; set; } = string.Empty;

    public ICollection<Service> Services { get; set; } = new List<Service>();
}

namespace ISPAdmin.Data.Entities;

public class BillingCycle : EntityBase
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int DurationInDays { get; set; }
    public string Description { get; set; } = string.Empty;
    public int SortOrder { get; set; }

    public ICollection<Service> Services { get; set; } = new List<Service>();
}

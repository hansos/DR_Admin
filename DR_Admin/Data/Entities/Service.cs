namespace ISPAdmin.Data.Entities;

public class Service
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int ServiceTypeId { get; set; }
    public int BillingCycleId { get; set; }
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ServiceType ServiceType { get; set; } = null!;
    public BillingCycle BillingCycle { get; set; } = null!;
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}

namespace ISPAdmin.DTOs;

public class ServiceDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int ServiceTypeId { get; set; }
    public int BillingCycleId { get; set; }
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateServiceDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int ServiceTypeId { get; set; }
    public int BillingCycleId { get; set; }
    public decimal Price { get; set; }
}

public class UpdateServiceDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int ServiceTypeId { get; set; }
    public int BillingCycleId { get; set; }
    public decimal Price { get; set; }
}

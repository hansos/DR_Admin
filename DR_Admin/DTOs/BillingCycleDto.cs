namespace ISPAdmin.DTOs;

public class BillingCycleDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int DurationInDays { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateBillingCycleDto
{
    public string Name { get; set; } = string.Empty;
    public int DurationInDays { get; set; }
    public string Description { get; set; } = string.Empty;
}

public class UpdateBillingCycleDto
{
    public string Name { get; set; } = string.Empty;
    public int DurationInDays { get; set; }
    public string Description { get; set; } = string.Empty;
}

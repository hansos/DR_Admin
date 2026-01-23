namespace ISPAdmin.DTOs;

public class ServiceTypeDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateServiceTypeDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class UpdateServiceTypeDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

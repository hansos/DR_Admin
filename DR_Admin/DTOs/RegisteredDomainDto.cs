namespace ISPAdmin.DTOs;

public class RegisteredDomainDto
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public int ServiceId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int ProviderId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime RegistrationDate { get; set; }
    public DateTime ExpirationDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateRegisteredDomainDto
{
    public int CustomerId { get; set; }
    public int ServiceId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int ProviderId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime RegistrationDate { get; set; }
    public DateTime ExpirationDate { get; set; }
}

public class UpdateRegisteredDomainDto
{
    public int CustomerId { get; set; }
    public int ServiceId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int ProviderId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime RegistrationDate { get; set; }
    public DateTime ExpirationDate { get; set; }
}

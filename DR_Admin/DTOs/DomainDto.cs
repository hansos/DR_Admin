namespace ISPAdmin.DTOs;

public class DomainDto
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int ProviderId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime RegistrationDate { get; set; }
    public DateTime ExpirationDate { get; set; }
}

public class CreateDomainDto
{
    public int CustomerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int ProviderId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime RegistrationDate { get; set; }
    public DateTime ExpirationDate { get; set; }
}

public class UpdateDomainDto
{
    public int CustomerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int ProviderId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime RegistrationDate { get; set; }
    public DateTime ExpirationDate { get; set; }
}

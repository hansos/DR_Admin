namespace ISPAdmin.DTOs;

public class HostingAccountDto
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public int ServiceId { get; set; }
    public string Provider { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? ExpirationDate { get; set; }
}

public class CreateHostingAccountDto
{
    public int CustomerId { get; set; }
    public int ServiceId { get; set; }
    public string Provider { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime? ExpirationDate { get; set; }
}

public class UpdateHostingAccountDto
{
    public int CustomerId { get; set; }
    public int ServiceId { get; set; }
    public string Provider { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime? ExpirationDate { get; set; }
}

namespace ISPAdmin.Data.Entities;

public class HostingAccount : EntityBase
{
    public int CustomerId { get; set; }
    public int ServiceId { get; set; }
    public string Provider { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime ExpirationDate { get; set; }

    public Customer Customer { get; set; } = null!;
    public Service Service { get; set; } = null!;
}

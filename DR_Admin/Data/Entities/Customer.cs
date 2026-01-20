namespace ISPAdmin.Data.Entities;

public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ICollection<User> Users { get; set; } = new List<User>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<Domain> Domains { get; set; } = new List<Domain>();
    public ICollection<HostingAccount> HostingAccounts { get; set; } = new List<HostingAccount>();
}

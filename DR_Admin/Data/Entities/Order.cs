namespace ISPAdmin.Data.Entities;

public class Order
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public int ServiceId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime NextBillingDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Customer Customer { get; set; } = null!;
    public Service Service { get; set; } = null!;
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}

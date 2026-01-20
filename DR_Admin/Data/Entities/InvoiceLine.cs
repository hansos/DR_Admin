namespace ISPAdmin.Data.Entities;

public class InvoiceLine
{
    public int Id { get; set; }
    public int InvoiceId { get; set; }
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public int? ServiceId { get; set; }
    public DateTime CreatedAt { get; set; }

    public Invoice Invoice { get; set; } = null!;
    public Service? Service { get; set; }
}

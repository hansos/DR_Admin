namespace ISPAdmin.DTOs;

public class OrderDto
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public int ServiceId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime NextBillingDate { get; set; }
}

public class CreateOrderDto
{
    public int CustomerId { get; set; }
    public int ServiceId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime NextBillingDate { get; set; }
}

public class UpdateOrderDto
{
    public int CustomerId { get; set; }
    public int ServiceId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime NextBillingDate { get; set; }
}

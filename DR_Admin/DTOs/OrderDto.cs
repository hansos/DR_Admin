namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object representing a customer order for a service
/// </summary>
public class OrderDto
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
}


/// <summary>
/// Data transfer object for creating a new order
/// </summary>
public class CreateOrderDto
{
    public int CustomerId { get; set; }
    public int ServiceId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime NextBillingDate { get; set; }
}


/// <summary>
/// Data transfer object for updating an existing order
/// </summary>
public class UpdateOrderDto
{
    public int CustomerId { get; set; }
    public int ServiceId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime NextBillingDate { get; set; }
}

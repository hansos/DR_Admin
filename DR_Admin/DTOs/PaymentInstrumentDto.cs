namespace ISPAdmin.DTOs;

/// <summary>
/// Payment instrument data transfer object
/// </summary>
public class PaymentInstrumentDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int DisplayOrder { get; set; }
    public int? DefaultGatewayId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// DTO for creating payment instrument
/// </summary>
public class CreatePaymentInstrumentDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public int DisplayOrder { get; set; }
    public int? DefaultGatewayId { get; set; }
}

/// <summary>
/// DTO for updating payment instrument
/// </summary>
public class UpdatePaymentInstrumentDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int DisplayOrder { get; set; }
    public int? DefaultGatewayId { get; set; }
}

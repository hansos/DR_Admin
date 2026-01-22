namespace ISPAdmin.DTOs;

public class RegistrarTldDto
{
    public int Id { get; set; }
    public int RegistrarId { get; set; }
    public string? RegistrarName { get; set; }
    public int TldId { get; set; }
    public string? TldExtension { get; set; }
    public decimal RegistrationCost { get; set; }
    public decimal RegistrationPrice { get; set; }
    public decimal RenewalCost { get; set; }
    public decimal RenewalPrice { get; set; }
    public decimal TransferCost { get; set; }
    public decimal TransferPrice { get; set; }
    public decimal? PrivacyCost { get; set; }
    public decimal? PrivacyPrice { get; set; }
    public string Currency { get; set; } = "USD";
    public bool IsAvailable { get; set; }
    public bool AutoRenew { get; set; }
    public int? MinRegistrationYears { get; set; }
    public int? MaxRegistrationYears { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateRegistrarTldDto
{
    public int RegistrarId { get; set; }
    public int TldId { get; set; }
    public decimal RegistrationCost { get; set; }
    public decimal RegistrationPrice { get; set; }
    public decimal RenewalCost { get; set; }
    public decimal RenewalPrice { get; set; }
    public decimal TransferCost { get; set; }
    public decimal TransferPrice { get; set; }
    public decimal? PrivacyCost { get; set; }
    public decimal? PrivacyPrice { get; set; }
    public string Currency { get; set; } = "USD";
    public bool IsAvailable { get; set; } = true;
    public bool AutoRenew { get; set; } = false;
    public int? MinRegistrationYears { get; set; } = 1;
    public int? MaxRegistrationYears { get; set; } = 10;
    public string? Notes { get; set; }
}

public class UpdateRegistrarTldDto
{
    public int RegistrarId { get; set; }
    public int TldId { get; set; }
    public decimal RegistrationCost { get; set; }
    public decimal RegistrationPrice { get; set; }
    public decimal RenewalCost { get; set; }
    public decimal RenewalPrice { get; set; }
    public decimal TransferCost { get; set; }
    public decimal TransferPrice { get; set; }
    public decimal? PrivacyCost { get; set; }
    public decimal? PrivacyPrice { get; set; }
    public string Currency { get; set; } = "USD";
    public bool IsAvailable { get; set; }
    public bool AutoRenew { get; set; }
    public int? MinRegistrationYears { get; set; }
    public int? MaxRegistrationYears { get; set; }
    public string? Notes { get; set; }
}

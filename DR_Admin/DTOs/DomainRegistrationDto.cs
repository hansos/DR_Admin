using System.ComponentModel.DataAnnotations;

namespace ISPAdmin.DTOs;

/// <summary>
/// DTO for customer self-service domain registration
/// </summary>
public class RegisterDomainDto
{
    [Required(ErrorMessage = "Domain name is required")]
    [StringLength(255, ErrorMessage = "Domain name cannot exceed 255 characters")]
    public string DomainName { get; set; } = string.Empty;
    
    [Range(1, 10, ErrorMessage = "Registration period must be between 1 and 10 years")]
    public int Years { get; set; } = 1;
    
    public bool AutoRenew { get; set; }
    
    public bool PrivacyProtection { get; set; }
    
    [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters")]
    public string? Notes { get; set; }
}

/// <summary>
/// DTO for sales/admin registering domain for a specific customer
/// </summary>
public class RegisterDomainForCustomerDto
{
    [Required(ErrorMessage = "Customer ID is required")]
    public int CustomerId { get; set; }
    
    [Required(ErrorMessage = "Domain name is required")]
    [StringLength(255, ErrorMessage = "Domain name cannot exceed 255 characters")]
    public string DomainName { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Registrar ID is required")]
    public int RegistrarId { get; set; }
    
    [Range(1, 10, ErrorMessage = "Registration period must be between 1 and 10 years")]
    public int Years { get; set; } = 1;
    
    public bool AutoRenew { get; set; }
    
    public bool PrivacyProtection { get; set; }
    
    [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters")]
    public string? Notes { get; set; }
}

/// <summary>
/// Response DTO for domain registration request
/// </summary>
public class DomainRegistrationResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int? OrderId { get; set; }
    public string? OrderNumber { get; set; }
    public int? InvoiceId { get; set; }
    public decimal? TotalAmount { get; set; }
    public string? CorrelationId { get; set; }
    public bool RequiresApproval { get; set; }
    public string? ApprovalStatus { get; set; }
}

/// <summary>
/// DTO for checking domain availability
/// </summary>
public class CheckDomainAvailabilityDto
{
    [Required(ErrorMessage = "Domain name is required")]
    [StringLength(255, ErrorMessage = "Domain name cannot exceed 255 characters")]
    public string DomainName { get; set; } = string.Empty;
}

/// <summary>
/// Response DTO for domain availability check
/// </summary>
public class DomainAvailabilityResponseDto
{
    public string DomainName { get; set; } = string.Empty;
    public bool IsAvailable { get; set; }
    public string Message { get; set; } = string.Empty;
    public decimal? Price { get; set; }
    public string? Currency { get; set; }
    public bool IsPremium { get; set; }
    public List<string> SuggestedAlternatives { get; set; } = new();
}

/// <summary>
/// DTO for getting domain pricing
/// </summary>
public class DomainPricingDto
{
    public string Tld { get; set; } = string.Empty;
    public int RegistrarId { get; set; }
    public string RegistrarName { get; set; } = string.Empty;
    public decimal RegistrationPrice { get; set; }
    public decimal RenewalPrice { get; set; }
    public decimal TransferPrice { get; set; }
    public string Currency { get; set; } = "USD";
    public Dictionary<int, decimal> PriceByYears { get; set; } = new();
}

/// <summary>
/// DTO for listing available TLDs
/// </summary>
public class AvailableTldDto
{
    public int Id { get; set; }
    public string Tld { get; set; } = string.Empty;
    public int RegistrarId { get; set; }
    public string RegistrarName { get; set; } = string.Empty;
    public decimal RegistrationPrice { get; set; }
    public decimal RenewalPrice { get; set; }
    public string Currency { get; set; } = "USD";
    public bool IsActive { get; set; }
}

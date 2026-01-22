namespace ISPAdmin.DTOs;

public class CustomerDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string? CountryCode { get; set; }
    public string? CompanyName { get; set; }
    public string? TaxId { get; set; }
    public string? VatNumber { get; set; }
    public string? ContactPerson { get; set; }
    public bool IsCompany { get; set; }
    public bool IsActive { get; set; }
    public string Status { get; set; } = "Active";
    public decimal Balance { get; set; }
    public decimal CreditLimit { get; set; }
    public string? Notes { get; set; }
    public string? BillingEmail { get; set; }
    public string? PreferredPaymentMethod { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateCustomerDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string? CountryCode { get; set; }
    public string? CompanyName { get; set; }
    public string? TaxId { get; set; }
    public string? VatNumber { get; set; }
    public string? ContactPerson { get; set; }
    public bool IsCompany { get; set; }
    public bool IsActive { get; set; } = true;
    public string Status { get; set; } = "Active";
    public decimal CreditLimit { get; set; }
    public string? Notes { get; set; }
    public string? BillingEmail { get; set; }
    public string? PreferredPaymentMethod { get; set; }
}

public class UpdateCustomerDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string? CountryCode { get; set; }
    public string? CompanyName { get; set; }
    public string? TaxId { get; set; }
    public string? VatNumber { get; set; }
    public string? ContactPerson { get; set; }
    public bool IsCompany { get; set; }
    public bool IsActive { get; set; }
    public string Status { get; set; } = "Active";
    public decimal CreditLimit { get; set; }
    public string? Notes { get; set; }
    public string? BillingEmail { get; set; }
    public string? PreferredPaymentMethod { get; set; }
}

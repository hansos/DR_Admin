namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object representing a customer
/// </summary>
public class CustomerDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the customer
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Gets or sets the customer's name
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the customer's email address
    /// </summary>
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the customer's phone number
    /// </summary>
    public string Phone { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the customer's street address
    /// </summary>
    public string Address { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the customer's city
    /// </summary>
    public string City { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the customer's state/province
    /// </summary>
    public string State { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the customer's postal code
    /// </summary>
    public string PostalCode { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the customer's country code
    /// </summary>
    public string? CountryCode { get; set; }
    
    /// <summary>
    /// Gets or sets the company name (if customer is a company)
    /// </summary>
    public string? CompanyName { get; set; }
    
    /// <summary>
    /// Gets or sets the tax identification number
    /// </summary>
    public string? TaxId { get; set; }
    
    /// <summary>
    /// Gets or sets the VAT (Value Added Tax) number
    /// </summary>
    public string? VatNumber { get; set; }
    
    /// <summary>
    /// Gets or sets the primary contact person's name
    /// </summary>
    public string? ContactPerson { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether the customer is a company
    /// </summary>
    public bool IsCompany { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether the customer account is active
    /// </summary>
    public bool IsActive { get; set; }
    
    /// <summary>
    /// Gets or sets the customer's account status
    /// </summary>
    public string Status { get; set; } = "Active";
    
    /// <summary>
    /// Gets or sets the customer's current account balance
    /// </summary>
    public decimal Balance { get; set; }
    
    /// <summary>
    /// Gets or sets the customer's credit limit
    /// </summary>
    public decimal CreditLimit { get; set; }
    
    /// <summary>
    /// Gets or sets additional notes about the customer
    /// </summary>
    public string? Notes { get; set; }
    
    /// <summary>
    /// Gets or sets the billing email address (if different from main email)
    /// </summary>
    public string? BillingEmail { get; set; }
    
    /// <summary>
    /// Gets or sets the customer's preferred payment method
    /// </summary>
    public string? PreferredPaymentMethod { get; set; }
    
    /// <summary>
    /// Gets or sets the date and time when the customer was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Gets or sets the date and time when the customer was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}


/// <summary>
/// Data transfer object for creating a new customer
/// </summary>
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


/// <summary>
/// Data transfer object for updating an existing customer
/// </summary>
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

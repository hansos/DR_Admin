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
    /// Gets or sets the customer name (company or individual name)
    /// </summary>
    public string? CustomerName { get; set; }
    
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
    /// Gets or sets the customer's preferred currency (ISO 4217 code, e.g., "EUR", "USD")
    /// </summary>
    public string PreferredCurrency { get; set; } = "EUR";
    
    /// <summary>
    /// Gets or sets whether to allow currency override at transaction level
    /// </summary>
    public bool AllowCurrencyOverride { get; set; } = true;
    
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
    /// <summary>
    /// Customer's name
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Customer's email address
    /// </summary>
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// Customer's phone number
    /// </summary>
    public string Phone { get; set; } = string.Empty;
    
    /// <summary>
    /// Customer's street address
    /// </summary>
    public string Address { get; set; } = string.Empty;
    
    /// <summary>
    /// Customer's city
    /// </summary>
    public string City { get; set; } = string.Empty;
    
    /// <summary>
    /// Customer's state/province
    /// </summary>
    public string State { get; set; } = string.Empty;
    
    /// <summary>
    /// Customer's postal code
    /// </summary>
    public string PostalCode { get; set; } = string.Empty;
    
    /// <summary>
    /// Customer's country code
    /// </summary>
    public string? CountryCode { get; set; }
    
    /// <summary>
    /// Customer name (company or individual name)
    /// </summary>
    public string? CustomerName { get; set; }
    
    /// <summary>
    /// Tax identification number
    /// </summary>
    public string? TaxId { get; set; }
    
    /// <summary>
    /// VAT (Value Added Tax) number
    /// </summary>
    public string? VatNumber { get; set; }
    
    /// <summary>
    /// Primary contact person's name
    /// </summary>
    public string? ContactPerson { get; set; }
    
    /// <summary>
    /// Whether the customer is a company
    /// </summary>
    public bool IsCompany { get; set; }
    
    /// <summary>
    /// Whether the customer account is active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Customer's account status
    /// </summary>
    public string Status { get; set; } = "Active";
    
    /// <summary>
    /// Customer's credit limit
    /// </summary>
    public decimal CreditLimit { get; set; }
    
    /// <summary>
    /// Additional notes about the customer
    /// </summary>
    public string? Notes { get; set; }
    
    /// <summary>
    /// Billing email address (if different from main email)
    /// </summary>
    public string? BillingEmail { get; set; }
    
    /// <summary>
    /// Customer's preferred payment method
    /// </summary>
    public string? PreferredPaymentMethod { get; set; }
    
    /// <summary>
    /// Customer's preferred currency (ISO 4217 code, e.g., "EUR", "USD")
    /// </summary>
    public string PreferredCurrency { get; set; } = "EUR";
    
    /// <summary>
    /// Whether to allow currency override at transaction level
    /// </summary>
    public bool AllowCurrencyOverride { get; set; } = true;
}


/// <summary>
/// Data transfer object for updating an existing customer
/// </summary>
public class UpdateCustomerDto
{
    /// <summary>
    /// Customer's name
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Customer's email address
    /// </summary>
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// Customer's phone number
    /// </summary>
    public string Phone { get; set; } = string.Empty;
    
    /// <summary>
    /// Customer's street address
    /// </summary>
    public string Address { get; set; } = string.Empty;
    
    /// <summary>
    /// Customer's city
    /// </summary>
    public string City { get; set; } = string.Empty;
    
    /// <summary>
    /// Customer's state/province
    /// </summary>
    public string State { get; set; } = string.Empty;
    
    /// <summary>
    /// Customer's postal code
    /// </summary>
    public string PostalCode { get; set; } = string.Empty;
    
    /// <summary>
    /// Customer's country code
    /// </summary>
    public string? CountryCode { get; set; }
    
    /// <summary>
    /// Customer name (company or individual name)
    /// </summary>
    public string? CustomerName { get; set; }
    
    /// <summary>
    /// Tax identification number
    /// </summary>
    public string? TaxId { get; set; }
    
    /// <summary>
    /// VAT (Value Added Tax) number
    /// </summary>
    public string? VatNumber { get; set; }
    
    /// <summary>
    /// Primary contact person's name
    /// </summary>
    public string? ContactPerson { get; set; }
    
    /// <summary>
    /// Whether the customer is a company
    /// </summary>
    public bool IsCompany { get; set; }
    
    /// <summary>
    /// Whether the customer account is active
    /// </summary>
    public bool IsActive { get; set; }
    
    /// <summary>
    /// Customer's account status
    /// </summary>
    public string Status { get; set; } = "Active";
    
    /// <summary>
    /// Customer's credit limit
    /// </summary>
    public decimal CreditLimit { get; set; }
    
    /// <summary>
    /// Additional notes about the customer
    /// </summary>
    public string? Notes { get; set; }
    
    /// <summary>
    /// Billing email address (if different from main email)
    /// </summary>
    public string? BillingEmail { get; set; }
    
    /// <summary>
    /// Customer's preferred payment method
    /// </summary>
    public string? PreferredPaymentMethod { get; set; }
    
    /// <summary>
    /// Customer's preferred currency (ISO 4217 code, e.g., "EUR", "USD")
    /// </summary>
    public string PreferredCurrency { get; set; } = "EUR";
    
    /// <summary>
    /// Whether to allow currency override at transaction level
    /// </summary>
    public bool AllowCurrencyOverride { get; set; } = true;
}

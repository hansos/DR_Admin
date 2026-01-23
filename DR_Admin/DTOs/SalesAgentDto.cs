namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object representing a sales agent
/// </summary>
public class SalesAgentDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the sales agent
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Gets or sets the reseller company ID this sales agent belongs to (null if independent)
    /// </summary>
    public int? ResellerCompanyId { get; set; }
    
    /// <summary>
    /// Gets or sets the first name
    /// </summary>
    public string FirstName { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the last name
    /// </summary>
    public string LastName { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the email address
    /// </summary>
    public string? Email { get; set; }
    
    /// <summary>
    /// Gets or sets the phone number
    /// </summary>
    public string? Phone { get; set; }
    
    /// <summary>
    /// Gets or sets the mobile phone number
    /// </summary>
    public string? MobilePhone { get; set; }
    
    /// <summary>
    /// Gets or sets whether the sales agent is active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Gets or sets additional notes about the sales agent
    /// </summary>
    public string? Notes { get; set; }
    
    /// <summary>
    /// Gets or sets the date and time when the sales agent record was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Gets or sets the date and time when the sales agent record was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Data transfer object for creating a new sales agent
/// </summary>
public class CreateSalesAgentDto
{
    /// <summary>
    /// Gets or sets the reseller company ID this sales agent belongs to (null if independent)
    /// </summary>
    public int? ResellerCompanyId { get; set; }
    
    /// <summary>
    /// Gets or sets the first name
    /// </summary>
    public string FirstName { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the last name
    /// </summary>
    public string LastName { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the email address
    /// </summary>
    public string? Email { get; set; }
    
    /// <summary>
    /// Gets or sets the phone number
    /// </summary>
    public string? Phone { get; set; }
    
    /// <summary>
    /// Gets or sets the mobile phone number
    /// </summary>
    public string? MobilePhone { get; set; }
    
    /// <summary>
    /// Gets or sets whether the sales agent is active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Gets or sets additional notes about the sales agent
    /// </summary>
    public string? Notes { get; set; }
}

/// <summary>
/// Data transfer object for updating an existing sales agent
/// </summary>
public class UpdateSalesAgentDto
{
    /// <summary>
    /// Gets or sets the reseller company ID this sales agent belongs to (null if independent)
    /// </summary>
    public int? ResellerCompanyId { get; set; }
    
    /// <summary>
    /// Gets or sets the first name
    /// </summary>
    public string FirstName { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the last name
    /// </summary>
    public string LastName { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the email address
    /// </summary>
    public string? Email { get; set; }
    
    /// <summary>
    /// Gets or sets the phone number
    /// </summary>
    public string? Phone { get; set; }
    
    /// <summary>
    /// Gets or sets the mobile phone number
    /// </summary>
    public string? MobilePhone { get; set; }
    
    /// <summary>
    /// Gets or sets whether the sales agent is active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Gets or sets additional notes about the sales agent
    /// </summary>
    public string? Notes { get; set; }
}

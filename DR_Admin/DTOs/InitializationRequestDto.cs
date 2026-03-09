namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object for system initialization request with first admin user credentials
/// </summary>
public class InitializationRequestDto
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? CompanyName { get; set; }
    public string? CompanyEmail { get; set; }
    public string? CompanyPhone { get; set; }
}

/// <summary>
/// Data transfer object for user panel initialization request.
/// </summary>
public class UserPanelInitializationRequestDto
{
    /// <summary>
    /// Gets or sets the username for the first end-user account.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the login email for the first end-user account.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the password for the first end-user account.
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the password confirmation for validation.
    /// </summary>
    public string ConfirmPassword { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the company name used as customer name.
    /// </summary>
    public string CompanyName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the company phone number.
    /// </summary>
    public string CompanyPhone { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the first name for the primary contact person.
    /// </summary>
    public string ContactFirstName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the last name for the primary contact person.
    /// </summary>
    public string ContactLastName { get; set; } = string.Empty;
}

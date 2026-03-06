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

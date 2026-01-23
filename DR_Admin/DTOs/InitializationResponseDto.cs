namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object for system initialization response
/// </summary>
public class InitializationResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
}

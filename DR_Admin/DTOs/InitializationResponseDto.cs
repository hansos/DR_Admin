namespace ISPAdmin.DTOs;

public class InitializationResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
}

namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object representing an authentication or refresh token
/// </summary>
public class TokenDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string TokenType { get; set; } = string.Empty;
    public DateTime Expiry { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? RevokedAt { get; set; }
}


/// <summary>
/// Data transfer object for creating a new token
/// </summary>
public class CreateTokenDto
{
    public int UserId { get; set; }
    public string TokenType { get; set; } = string.Empty;
    public string TokenValue { get; set; } = string.Empty;
    public DateTime Expiry { get; set; }
}


/// <summary>
/// Data transfer object for updating an existing token (primarily for revocation)
/// </summary>
public class UpdateTokenDto
{
    public DateTime? RevokedAt { get; set; }
}

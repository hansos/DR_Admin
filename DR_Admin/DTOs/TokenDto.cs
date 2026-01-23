namespace ISPAdmin.DTOs;

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

public class CreateTokenDto
{
    public int UserId { get; set; }
    public string TokenType { get; set; } = string.Empty;
    public string TokenValue { get; set; } = string.Empty;
    public DateTime Expiry { get; set; }
}

public class UpdateTokenDto
{
    public DateTime? RevokedAt { get; set; }
}

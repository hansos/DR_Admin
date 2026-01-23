namespace ISPAdmin.DTOs;

public class AuditLogDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string ActionType { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public int EntityId { get; set; }
    public DateTime Timestamp { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string Details { get; set; } = string.Empty;
    public string IPAddress { get; set; } = string.Empty;
}

public class CreateAuditLogDto
{
    public int UserId { get; set; }
    public string ActionType { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public int EntityId { get; set; }
    public string Details { get; set; } = string.Empty;
    public string IPAddress { get; set; } = string.Empty;
}

public class UpdateAuditLogDto
{
    public string Details { get; set; } = string.Empty;
}

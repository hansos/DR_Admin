namespace ISPAdmin.Data.Entities;

public class AuditLog : EntityBase
{
    public int UserId { get; set; }
    public string ActionType { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string Details { get; set; } = string.Empty;
    public string IPAddress { get; set; } = string.Empty;

    public User User { get; set; } = null!;
}

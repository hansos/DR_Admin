namespace ISPAdmin.Data.Entities;

public class CommunicationThread : EntityBase
{
    public string Subject { get; set; } = string.Empty;
    public int? CustomerId { get; set; }
    public int? UserId { get; set; }
    public string? RelatedEntityType { get; set; }
    public int? RelatedEntityId { get; set; }
    public DateTime? LastMessageAtUtc { get; set; }
    public string Status { get; set; } = CommunicationThreadStatus.Open;

    public Customer? Customer { get; set; }
    public User? User { get; set; }
    public ICollection<CommunicationMessage> Messages { get; set; } = new List<CommunicationMessage>();
    public ICollection<CommunicationParticipant> Participants { get; set; } = new List<CommunicationParticipant>();
}

public static class CommunicationThreadStatus
{
    public const string Open = "Open";
    public const string Closed = "Closed";
}

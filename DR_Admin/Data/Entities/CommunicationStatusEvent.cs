namespace ISPAdmin.Data.Entities;

public class CommunicationStatusEvent : EntityBase
{
    public int CommunicationMessageId { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Details { get; set; }
    public string? Source { get; set; }
    public DateTime OccurredAtUtc { get; set; }

    public CommunicationMessage? CommunicationMessage { get; set; }
}

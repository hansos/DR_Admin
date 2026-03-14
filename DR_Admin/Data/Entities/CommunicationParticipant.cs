namespace ISPAdmin.Data.Entities;

public class CommunicationParticipant : EntityBase
{
    public int CommunicationThreadId { get; set; }
    public string EmailAddress { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public string Role { get; set; } = CommunicationParticipantRole.To;
    public bool IsPrimary { get; set; }

    public CommunicationThread? CommunicationThread { get; set; }
}

public static class CommunicationParticipantRole
{
    public const string From = "From";
    public const string To = "To";
    public const string Cc = "Cc";
    public const string Bcc = "Bcc";
}

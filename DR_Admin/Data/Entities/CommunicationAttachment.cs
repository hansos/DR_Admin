namespace ISPAdmin.Data.Entities;

public class CommunicationAttachment : EntityBase
{
    public int CommunicationMessageId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string? ContentType { get; set; }
    public string? StoragePath { get; set; }
    public long? SizeBytes { get; set; }
    public string? InlineContentId { get; set; }

    public CommunicationMessage? CommunicationMessage { get; set; }
}

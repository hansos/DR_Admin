namespace ISPAdmin.Data.Entities;

public class BackupSchedule
{
    public int Id { get; set; }
    public string DatabaseName { get; set; } = string.Empty;
    public string Frequency { get; set; } = string.Empty;
    public DateTime LastBackupDate { get; set; }
    public DateTime NextBackupDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

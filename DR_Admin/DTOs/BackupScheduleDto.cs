namespace ISPAdmin.DTOs;

public class BackupScheduleDto
{
    public int Id { get; set; }
    public string DatabaseName { get; set; } = string.Empty;
    public string Frequency { get; set; } = string.Empty;
    public DateTime? LastBackupDate { get; set; }
    public DateTime? NextBackupDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateBackupScheduleDto
{
    public string DatabaseName { get; set; } = string.Empty;
    public string Frequency { get; set; } = string.Empty;
    public DateTime? NextBackupDate { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class UpdateBackupScheduleDto
{
    public string DatabaseName { get; set; } = string.Empty;
    public string Frequency { get; set; } = string.Empty;
    public DateTime? LastBackupDate { get; set; }
    public DateTime? NextBackupDate { get; set; }
    public string Status { get; set; } = string.Empty;
}

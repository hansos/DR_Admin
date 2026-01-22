namespace ISPAdmin.Data.Entities;

public class Tld
{
    public int Id { get; set; }
    public string Extension { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int? DefaultRegistrationYears { get; set; }
    public int? MaxRegistrationYears { get; set; }
    public bool RequiresPrivacy { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ICollection<RegistrarTld> RegistrarTlds { get; set; } = new List<RegistrarTld>();
}

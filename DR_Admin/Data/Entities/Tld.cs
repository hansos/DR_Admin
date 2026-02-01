namespace ISPAdmin.Data.Entities;

public class Tld : EntityBase
{
    public string Extension { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string RulesUrl { get; set; } = string.Empty;
    public bool IsSecondLevel { get; set; }
    public bool IsActive { get; set; }
    public int? DefaultRegistrationYears { get; set; }
    public int? MaxRegistrationYears { get; set; }
    public bool RequiresPrivacy { get; set; }
    public string? Notes { get; set; }

    public ICollection<RegistrarTld> RegistrarTlds { get; set; } = new List<RegistrarTld>();
}

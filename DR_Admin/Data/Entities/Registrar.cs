namespace ISPAdmin.Data.Entities;

public class Registrar : EntityBase
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }
    public string? Website { get; set; }
    public string? Notes { get; set; }

    public ICollection<RegistrarTld> RegistrarTlds { get; set; } = new List<RegistrarTld>();
    public ICollection<Domain> Domains { get; set; } = new List<Domain>();
}

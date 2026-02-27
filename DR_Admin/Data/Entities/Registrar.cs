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

    /// <summary>
    /// Indicates whether this is the default registrar
    /// </summary>
    public bool IsDefault { get; set; } = false;

    /// <summary>
    /// Normalized version of Name for case-insensitive searches
    /// </summary>
    public string NormalizedName { get; set; } = string.Empty;


    public ICollection<RegistrarTld> RegistrarTlds { get; set; } = new List<RegistrarTld>();
    public ICollection<RegisteredDomain> RegisteredDomains { get; set; } = new List<RegisteredDomain>();
    public ICollection<RegistrarSelectionPreference> SelectionPreferences { get; set; } = new List<RegistrarSelectionPreference>();
    public ICollection<RegistrarTldPriceDownloadSession> PriceDownloadSessions { get; set; } = new List<RegistrarTldPriceDownloadSession>();
}


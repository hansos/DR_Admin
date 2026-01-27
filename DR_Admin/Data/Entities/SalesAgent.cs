namespace ISPAdmin.Data.Entities;

public class SalesAgent : EntityBase
{
    public int? ResellerCompanyId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? MobilePhone { get; set; }
    public bool IsActive { get; set; } = true;
    public string? Notes { get; set; }

    /// <summary>
    /// Normalized version of FirstName for case-insensitive searches
    /// </summary>
    public string NormalizedFirstName { get; set; } = string.Empty;

    /// <summary>
    /// Normalized version of LastName for case-insensitive searches
    /// </summary>
    public string NormalizedLastName { get; set; } = string.Empty;

    public ResellerCompany? ResellerCompany { get; set; }
    public ICollection<DnsZonePackage> DnsZonePackages { get; set; } = new List<DnsZonePackage>();
    public ICollection<Service> Services { get; set; } = new List<Service>();
}

namespace ISPAdmin.Data.Entities;

public class DnsZonePackage : EntityBase
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDefault { get; set; } = false;
    public int SortOrder { get; set; } = 0;
    public int? ResellerCompanyId { get; set; }
    public int? SalesAgentId { get; set; }

    public ResellerCompany? ResellerCompany { get; set; }
    public SalesAgent? SalesAgent { get; set; }
    public ICollection<DnsZonePackageRecord> Records { get; set; } = new List<DnsZonePackageRecord>();
}


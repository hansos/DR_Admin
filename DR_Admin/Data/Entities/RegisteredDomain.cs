using ISPAdmin.Data.Enums;

namespace ISPAdmin.Data.Entities;

public class RegisteredDomain : EntityBase
{
    public int CustomerId { get; set; }
    public int? ServiceId { get; set; }
    public required string Name { get; set; }
    public int RegistrarId { get; set; }
    public int? RegistrarTldId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DomainRegistrationStatus RegistrationStatus { get; set; } = DomainRegistrationStatus.PendingPayment;
    public DateTime? RegistrationDate { get; set; }
    public int RegistrationAttemptCount { get; set; }
    public DateTime? LastRegistrationAttemptUtc { get; set; }
    public DateTime? NextRegistrationAttemptUtc { get; set; }
    public string? RegistrationError { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public bool AutoRenew { get; set; }
    public bool PrivacyProtection { get; set; }
    public decimal? RegistrationPrice { get; set; }
    public decimal? RenewalPrice { get; set; }
    public string? Notes { get; set; }

    /// <summary>
    /// Normalized version of Name for case-insensitive searches
    /// </summary>
    public string NormalizedName { get; set; } = string.Empty;

    public Customer Customer { get; set; } = null!;
    public Service? Service { get; set; }
    public Registrar Registrar { get; set; } = null!;
    public RegistrarTld? RegistrarTld { get; set; }
    public ICollection<DnsRecord> DnsRecords { get; set; } = new List<DnsRecord>();
    public ICollection<DomainContact> DomainContacts { get; set; } = new List<DomainContact>();
    public ICollection<DomainContactAssignment> DomainContactAssignments { get; set; } = new List<DomainContactAssignment>();
    public ICollection<RegisteredDomainHistory> RegisteredDomainHistories { get; set; } = new List<RegisteredDomainHistory>();
    public ICollection<NameServerDomain> NameServerDomains { get; set; } = new List<NameServerDomain>();
    public ICollection<SoldHostingPackage> SoldHostingPackages { get; set; } = new List<SoldHostingPackage>();
    public ICollection<SoldOptionalService> SoldOptionalServices { get; set; } = new List<SoldOptionalService>();
}

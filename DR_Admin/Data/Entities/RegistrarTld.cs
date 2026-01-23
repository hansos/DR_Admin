namespace ISPAdmin.Data.Entities;

public class RegistrarTld : EntityBase
{
    public int RegistrarId { get; set; }
    public int TldId { get; set; }
    public decimal RegistrationCost { get; set; }
    public decimal RegistrationPrice { get; set; }
    public decimal RenewalCost { get; set; }
    public decimal RenewalPrice { get; set; }
    public decimal TransferCost { get; set; }
    public decimal TransferPrice { get; set; }
    public decimal? PrivacyCost { get; set; }
    public decimal? PrivacyPrice { get; set; }
    public string Currency { get; set; } = "USD";
    public bool IsAvailable { get; set; }
    public bool AutoRenew { get; set; }
    public int? MinRegistrationYears { get; set; }
    public int? MaxRegistrationYears { get; set; }
    public string? Notes { get; set; }

    public Registrar Registrar { get; set; } = null!;
    public Tld Tld { get; set; } = null!;
    public ICollection<Domain> Domains { get; set; } = new List<Domain>();
}

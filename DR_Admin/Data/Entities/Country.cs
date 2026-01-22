namespace ISPAdmin.Data.Entities;

public class Country
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Tld { get; set; } = string.Empty;
    public string EnglishName { get; set; } = string.Empty;
    public string LocalName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ICollection<Customer> Customers { get; set; } = new List<Customer>();
    public ICollection<PostalCode> PostalCodes { get; set; } = new List<PostalCode>();
}

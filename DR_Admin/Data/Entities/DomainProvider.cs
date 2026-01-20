namespace ISPAdmin.Data.Entities;

public class DomainProvider
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ApiEndpoint { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string ApiSecret { get; set; } = string.Empty;

    public ICollection<Domain> Domains { get; set; } = new List<Domain>();
}

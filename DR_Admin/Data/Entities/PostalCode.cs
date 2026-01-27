namespace ISPAdmin.Data.Entities;

public class PostalCode : EntityBase
{
    public string Code { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string? State { get; set; }
    public string? Region { get; set; }
    public string? District { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public bool IsActive { get; set; }

    /// <summary>
    /// Normalized version of Code for case-insensitive searches
    /// </summary>
    public string NormalizedCode { get; set; } = string.Empty;

    /// <summary>
    /// Normalized version of CountryCode for case-insensitive searches
    /// </summary>
    public string NormalizedCountryCode { get; set; } = string.Empty;

    /// <summary>
    /// Normalized version of City for case-insensitive searches
    /// </summary>
    public string NormalizedCity { get; set; } = string.Empty;

    /// <summary>
    /// Normalized version of State for case-insensitive searches
    /// </summary>
    public string? NormalizedState { get; set; }

    /// <summary>
    /// Normalized version of Region for case-insensitive searches
    /// </summary>
    public string? NormalizedRegion { get; set; }

    /// <summary>
    /// Normalized version of District for case-insensitive searches
    /// </summary>
    public string? NormalizedDistrict { get; set; }

    public Country Country { get; set; } = null!;
}

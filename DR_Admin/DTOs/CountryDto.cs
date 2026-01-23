namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object representing a country
/// </summary>
public class CountryDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Tld { get; set; } = string.Empty;
    public string EnglishName { get; set; } = string.Empty;
    public string LocalName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}


/// <summary>
/// Data transfer object for creating a new country
/// </summary>
public class CreateCountryDto
{
    public string Code { get; set; } = string.Empty;
    public string Tld { get; set; } = string.Empty;
    public string EnglishName { get; set; } = string.Empty;
    public string LocalName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}


/// <summary>
/// Data transfer object for updating an existing country
/// </summary>
public class UpdateCountryDto
{
    public string Code { get; set; } = string.Empty;
    public string Tld { get; set; } = string.Empty;
    public string EnglishName { get; set; } = string.Empty;
    public string LocalName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

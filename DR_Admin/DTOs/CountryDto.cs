namespace ISPAdmin.DTOs;

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

public class CreateCountryDto
{
    public string Code { get; set; } = string.Empty;
    public string Tld { get; set; } = string.Empty;
    public string EnglishName { get; set; } = string.Empty;
    public string LocalName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}

public class UpdateCountryDto
{
    public string Code { get; set; } = string.Empty;
    public string Tld { get; set; } = string.Empty;
    public string EnglishName { get; set; } = string.Empty;
    public string LocalName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

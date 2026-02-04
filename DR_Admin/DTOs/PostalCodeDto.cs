namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object representing a postal code with geographic information
/// </summary>
public class PostalCodeDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string? State { get; set; }
    public string? Region { get; set; }
    public string? District { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}


/// <summary>
/// Data transfer object for creating a new postal code
/// </summary>
public class CreatePostalCodeDto
{
    public string Code { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string? State { get; set; }
    public string? Region { get; set; }
    public string? District { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public bool IsActive { get; set; } = true;
}


/// <summary>
/// Data transfer object for updating an existing postal code
/// </summary>
public class UpdatePostalCodeDto
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
}

/// <summary>
/// Data transfer object for a postal code import item
/// </summary>
public class ImportPostalCodeItemDto
{
    public string Code { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string? State { get; set; }
    public string? Region { get; set; }
    public string? District { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// Data transfer object for importing postal codes
/// </summary>
public class ImportPostalCodesDto
{
    public string CountryCode { get; set; } = string.Empty;
    public List<ImportPostalCodeItemDto> PostalCodes { get; set; } = new();
}

/// <summary>
/// Data transfer object for uploading postal codes CSV file
/// </summary>
public class UploadPostalCodesCsvDto
{
    public string CountryCode { get; set; } = string.Empty;
    public IFormFile? File { get; set; }
}

/// <summary>
/// Result of postal codes import operation
/// </summary>
public class ImportPostalCodesResultDto
{
    public int TotalProcessed { get; set; }
    public int Created { get; set; }
    public int Updated { get; set; }
    public int Skipped { get; set; }
    public List<string> Errors { get; set; } = new();
}

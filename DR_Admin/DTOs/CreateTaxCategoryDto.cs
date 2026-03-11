namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object for creating a tax category.
/// </summary>
public class CreateTaxCategoryDto
{
    /// <summary>
    /// Gets or sets the category code.
    /// </summary>
    public string Code { get; set; } = "STANDARD";

    /// <summary>
    /// Gets or sets the display name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the country code.
    /// </summary>
    public string CountryCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the optional state code.
    /// </summary>
    public string? StateCode { get; set; }

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether the category is active.
    /// </summary>
    public bool IsActive { get; set; } = true;
}

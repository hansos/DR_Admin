namespace ISPAdmin.DTOs;

/// <summary>
/// Response data transfer object containing generated alternative domain names.
/// </summary>
public class DomainNameAlternativesResponseDto
{
    /// <summary>
    /// Gets or sets the input domain name provided by the caller.
    /// </summary>
    public string InputDomainName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the number of generated suggestions in the response.
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    /// Gets or sets the generated alternative domain names.
    /// </summary>
    public List<string> Suggestions { get; set; } = new();
}

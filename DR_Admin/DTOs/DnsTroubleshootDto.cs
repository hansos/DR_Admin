namespace ISPAdmin.DTOs;

/// <summary>
/// Represents the troubleshooting report for DNS checks on a domain.
/// </summary>
public class DnsTroubleshootReportDto
{
    /// <summary>
    /// Gets or sets the domain identifier.
    /// </summary>
    public int DomainId { get; set; }

    /// <summary>
    /// Gets or sets the domain name.
    /// </summary>
    public string DomainName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the UTC timestamp when this report was generated.
    /// </summary>
    public DateTime GeneratedAtUtc { get; set; }

    /// <summary>
    /// Gets or sets the test results.
    /// </summary>
    public List<DnsTroubleshootTestResultDto> Tests { get; set; } = [];
}

/// <summary>
/// Represents a single DNS troubleshoot test result.
/// </summary>
public class DnsTroubleshootTestResultDto
{
    /// <summary>
    /// Gets or sets the test key.
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the test display name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the severity level (Info, Warning, Error).
    /// </summary>
    public string Severity { get; set; } = "Info";

    /// <summary>
    /// Gets or sets whether the test passed.
    /// </summary>
    public bool Passed { get; set; }

    /// <summary>
    /// Gets or sets the test message.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets optional details for the test result.
    /// </summary>
    public string? Details { get; set; }

    /// <summary>
    /// Gets or sets an optional fix URL.
    /// </summary>
    public string? FixUrl { get; set; }
}

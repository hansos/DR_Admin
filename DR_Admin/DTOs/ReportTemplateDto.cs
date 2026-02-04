using ISPAdmin.Data.Enums;

namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object representing a report template
/// </summary>
public class ReportTemplateDto
{
    /// <summary>
    /// Unique identifier for the report template
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The name of the report template
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Description or notes about the report template's purpose
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// The category/type of the report template
    /// </summary>
    public ReportTemplateType TemplateType { get; set; }

    /// <summary>
    /// The reporting engine this template is designed for
    /// </summary>
    public string ReportEngine { get; set; } = "FastReport";

    /// <summary>
    /// The original filename with extension
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// The size of the file in bytes
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// The MIME type of the file
    /// </summary>
    public string MimeType { get; set; } = string.Empty;

    /// <summary>
    /// Indicates whether this template is currently active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Indicates whether this is the default template for its type
    /// </summary>
    public bool IsDefault { get; set; }

    /// <summary>
    /// JSON string containing data source information and available fields
    /// </summary>
    public string DataSourceInfo { get; set; } = string.Empty;

    /// <summary>
    /// The version of the template
    /// </summary>
    public string Version { get; set; } = "1.0";

    /// <summary>
    /// Tags for categorization and search
    /// </summary>
    public string Tags { get; set; } = string.Empty;

    /// <summary>
    /// The default export format for this report
    /// </summary>
    public string DefaultExportFormat { get; set; } = "PDF";

    /// <summary>
    /// Date and time when the template was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date and time when the template was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

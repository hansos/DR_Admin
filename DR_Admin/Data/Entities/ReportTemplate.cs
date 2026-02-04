using ISPAdmin.Data.Enums;

namespace ISPAdmin.Data.Entities;

/// <summary>
/// Represents a report template for FastReport and other reporting engines
/// </summary>
public class ReportTemplate : EntityBase
{
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
    /// The reporting engine this template is designed for (e.g., "FastReport", "RDLC", etc.)
    /// </summary>
    public string ReportEngine { get; set; } = "FastReport";

    /// <summary>
    /// The actual file content stored as binary data
    /// </summary>
    public byte[] FileContent { get; set; } = Array.Empty<byte>();

    /// <summary>
    /// The original filename with extension
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// The size of the file in bytes
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// The MIME type of the file (e.g., application/octet-stream for .frx files)
    /// </summary>
    public string MimeType { get; set; } = string.Empty;

    /// <summary>
    /// Indicates whether this template is currently active and available for use
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Indicates whether this is the default template for its type
    /// </summary>
    public bool IsDefault { get; set; } = false;

    /// <summary>
    /// JSON string containing data source information and available fields
    /// </summary>
    public string DataSourceInfo { get; set; } = string.Empty;

    /// <summary>
    /// The version of the template (for tracking updates)
    /// </summary>
    public string Version { get; set; } = "1.0";

    /// <summary>
    /// Tags for categorization and search (comma-separated)
    /// </summary>
    public string Tags { get; set; } = string.Empty;

    /// <summary>
    /// The default export format for this report (e.g., PDF, Excel, HTML)
    /// </summary>
    public string DefaultExportFormat { get; set; } = "PDF";

    /// <summary>
    /// Timestamp for soft delete functionality
    /// </summary>
    public DateTime? DeletedAt { get; set; }
}

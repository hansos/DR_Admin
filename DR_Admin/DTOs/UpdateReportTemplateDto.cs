using ISPAdmin.Data.Enums;
using Microsoft.AspNetCore.Http;

namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object for updating an existing report template
/// </summary>
public class UpdateReportTemplateDto
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
    /// The reporting engine this template is designed for
    /// </summary>
    public string ReportEngine { get; set; } = "FastReport";

    /// <summary>
    /// Optional: New file to replace the existing template file
    /// </summary>
    public IFormFile? File { get; set; }

    /// <summary>
    /// Indicates whether this template is currently active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Indicates whether this should be the default template for its type
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
    /// Tags for categorization and search (comma-separated)
    /// </summary>
    public string Tags { get; set; } = string.Empty;

    /// <summary>
    /// The default export format for this report (PDF, Excel, HTML, etc.)
    /// </summary>
    public string DefaultExportFormat { get; set; } = "PDF";
}

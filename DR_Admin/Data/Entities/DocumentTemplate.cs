using ISPAdmin.Data.Enums;

namespace ISPAdmin.Data.Entities;

/// <summary>
/// Represents a document template for generating invoices, orders, emails, and other documents
/// </summary>
public class DocumentTemplate : EntityBase
{
    /// <summary>
    /// The name of the template
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Description or notes about the template's purpose
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// The category/type of the template
    /// </summary>
    public DocumentTemplateType TemplateType { get; set; }

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
    /// The MIME type of the file (e.g., application/pdf, text/html)
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
    /// JSON string containing available placeholder variables (e.g., {{CustomerName}}, {{InvoiceNumber}})
    /// </summary>
    public string PlaceholderVariables { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp for soft delete functionality
    /// </summary>
    public DateTime? DeletedAt { get; set; }
}

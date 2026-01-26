using ISPAdmin.Data.Enums;

namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object representing a document template
/// </summary>
public class DocumentTemplateDto
{
    /// <summary>
    /// Unique identifier for the template
    /// </summary>
    public int Id { get; set; }

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
    /// JSON string containing available placeholder variables
    /// </summary>
    public string PlaceholderVariables { get; set; } = string.Empty;

    /// <summary>
    /// When the template was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// When the template was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Timestamp for soft delete
    /// </summary>
    public DateTime? DeletedAt { get; set; }
}

/// <summary>
/// Data transfer object for creating a new document template
/// </summary>
public class CreateDocumentTemplateDto
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
    /// The uploaded file
    /// </summary>
    public IFormFile? File { get; set; }

    /// <summary>
    /// Indicates whether this template should be active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Indicates whether this should be the default template for its type
    /// </summary>
    public bool IsDefault { get; set; } = false;

    /// <summary>
    /// JSON string containing available placeholder variables
    /// </summary>
    public string PlaceholderVariables { get; set; } = string.Empty;
}

/// <summary>
/// Data transfer object for updating an existing document template
/// </summary>
public class UpdateDocumentTemplateDto
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
    /// Optional file to replace the existing template file
    /// </summary>
    public IFormFile? File { get; set; }

    /// <summary>
    /// Indicates whether this template should be active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Indicates whether this should be the default template for its type
    /// </summary>
    public bool IsDefault { get; set; }

    /// <summary>
    /// JSON string containing available placeholder variables
    /// </summary>
    public string PlaceholderVariables { get; set; } = string.Empty;
}

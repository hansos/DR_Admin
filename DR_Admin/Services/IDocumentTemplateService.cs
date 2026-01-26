using ISPAdmin.Data.Enums;
using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

/// <summary>
/// Service interface for managing document templates
/// </summary>
public interface IDocumentTemplateService
{
    /// <summary>
    /// Retrieves all non-deleted document templates
    /// </summary>
    /// <returns>A collection of document template DTOs</returns>
    Task<IEnumerable<DocumentTemplateDto>> GetAllTemplatesAsync();

    /// <summary>
    /// Retrieves all active document templates
    /// </summary>
    /// <returns>A collection of active document template DTOs</returns>
    Task<IEnumerable<DocumentTemplateDto>> GetActiveTemplatesAsync();

    /// <summary>
    /// Retrieves document templates filtered by type
    /// </summary>
    /// <param name="type">The template type to filter by</param>
    /// <returns>A collection of document template DTOs matching the specified type</returns>
    Task<IEnumerable<DocumentTemplateDto>> GetTemplatesByTypeAsync(DocumentTemplateType type);

    /// <summary>
    /// Retrieves a specific document template by ID
    /// </summary>
    /// <param name="id">The unique identifier of the template</param>
    /// <returns>The document template DTO if found, otherwise null</returns>
    Task<DocumentTemplateDto?> GetTemplateByIdAsync(int id);

    /// <summary>
    /// Retrieves the default template for a specific type
    /// </summary>
    /// <param name="type">The template type</param>
    /// <returns>The default document template DTO for the specified type, or null if none exists</returns>
    Task<DocumentTemplateDto?> GetDefaultTemplateAsync(DocumentTemplateType type);

    /// <summary>
    /// Creates a new document template
    /// </summary>
    /// <param name="createDto">The data transfer object containing template information and file</param>
    /// <returns>The created document template DTO</returns>
    Task<DocumentTemplateDto> CreateTemplateAsync(CreateDocumentTemplateDto createDto);

    /// <summary>
    /// Updates an existing document template
    /// </summary>
    /// <param name="id">The unique identifier of the template to update</param>
    /// <param name="updateDto">The data transfer object containing updated template information</param>
    /// <returns>The updated document template DTO if found, otherwise null</returns>
    Task<DocumentTemplateDto?> UpdateTemplateAsync(int id, UpdateDocumentTemplateDto updateDto);

    /// <summary>
    /// Sets a template as the default for its type
    /// </summary>
    /// <param name="id">The unique identifier of the template to set as default</param>
    /// <returns>True if successful, false if template not found</returns>
    Task<bool> SetDefaultTemplateAsync(int id);

    /// <summary>
    /// Soft deletes a document template
    /// </summary>
    /// <param name="id">The unique identifier of the template to delete</param>
    /// <returns>True if successful, false if template not found</returns>
    Task<bool> SoftDeleteTemplateAsync(int id);

    /// <summary>
    /// Downloads the file content of a specific template
    /// </summary>
    /// <param name="id">The unique identifier of the template</param>
    /// <returns>A tuple containing the file content, filename, and MIME type, or null if not found</returns>
    Task<(byte[] content, string fileName, string mimeType)?> DownloadTemplateAsync(int id);
}

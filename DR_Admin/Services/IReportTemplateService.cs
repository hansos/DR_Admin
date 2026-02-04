using ISPAdmin.Data.Enums;
using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

/// <summary>
/// Service interface for managing report templates
/// </summary>
public interface IReportTemplateService
{
    /// <summary>
    /// Retrieves all non-deleted report templates
    /// </summary>
    /// <returns>A collection of report template DTOs</returns>
    Task<IEnumerable<ReportTemplateDto>> GetAllTemplatesAsync();

    /// <summary>
    /// Retrieves all active report templates
    /// </summary>
    /// <returns>A collection of active report template DTOs</returns>
    Task<IEnumerable<ReportTemplateDto>> GetActiveTemplatesAsync();

    /// <summary>
    /// Retrieves report templates filtered by type
    /// </summary>
    /// <param name="type">The report template type to filter by</param>
    /// <returns>A collection of report template DTOs matching the specified type</returns>
    Task<IEnumerable<ReportTemplateDto>> GetTemplatesByTypeAsync(ReportTemplateType type);

    /// <summary>
    /// Retrieves report templates filtered by reporting engine
    /// </summary>
    /// <param name="engine">The reporting engine name (e.g., "FastReport")</param>
    /// <returns>A collection of report template DTOs matching the specified engine</returns>
    Task<IEnumerable<ReportTemplateDto>> GetTemplatesByEngineAsync(string engine);

    /// <summary>
    /// Retrieves a specific report template by ID
    /// </summary>
    /// <param name="id">The unique identifier of the template</param>
    /// <returns>The report template DTO if found, otherwise null</returns>
    Task<ReportTemplateDto?> GetTemplateByIdAsync(int id);

    /// <summary>
    /// Retrieves the default template for a specific type
    /// </summary>
    /// <param name="type">The report template type</param>
    /// <returns>The default report template DTO for the specified type, or null if none exists</returns>
    Task<ReportTemplateDto?> GetDefaultTemplateAsync(ReportTemplateType type);

    /// <summary>
    /// Creates a new report template
    /// </summary>
    /// <param name="createDto">The data transfer object containing template information and file</param>
    /// <returns>The created report template DTO</returns>
    Task<ReportTemplateDto> CreateTemplateAsync(CreateReportTemplateDto createDto);

    /// <summary>
    /// Updates an existing report template
    /// </summary>
    /// <param name="id">The unique identifier of the template to update</param>
    /// <param name="updateDto">The data transfer object containing updated template information</param>
    /// <returns>The updated report template DTO if found, otherwise null</returns>
    Task<ReportTemplateDto?> UpdateTemplateAsync(int id, UpdateReportTemplateDto updateDto);

    /// <summary>
    /// Sets a template as the default for its type
    /// </summary>
    /// <param name="id">The unique identifier of the template to set as default</param>
    /// <returns>True if successful, false if template not found</returns>
    Task<bool> SetDefaultTemplateAsync(int id);

    /// <summary>
    /// Toggles the active status of a report template
    /// </summary>
    /// <param name="id">The unique identifier of the template</param>
    /// <returns>True if successful, false if template not found</returns>
    Task<bool> ToggleActiveStatusAsync(int id);

    /// <summary>
    /// Soft deletes a report template
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

    /// <summary>
    /// Searches for report templates by name or description
    /// </summary>
    /// <param name="searchTerm">The search term to match against name and description</param>
    /// <returns>A collection of matching report template DTOs</returns>
    Task<IEnumerable<ReportTemplateDto>> SearchTemplatesAsync(string searchTerm);
}

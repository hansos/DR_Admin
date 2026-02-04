using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.Data.Enums;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

/// <summary>
/// Service implementation for managing report templates
/// </summary>
public class ReportTemplateService : IReportTemplateService
{
    private readonly ApplicationDbContext _context;
    private static readonly Serilog.ILogger _log = Log.ForContext<ReportTemplateService>();
    private const long MaxFileSize = 50 * 1024 * 1024; // 50MB
    private static readonly string[] AllowedExtensions = { ".frx", ".rdlc", ".rpt" };

    public ReportTemplateService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves all non-deleted report templates
    /// </summary>
    /// <returns>A collection of report template DTOs</returns>
    public async Task<IEnumerable<ReportTemplateDto>> GetAllTemplatesAsync()
    {
        try
        {
            _log.Information("Fetching all report templates");

            var templates = await _context.ReportTemplates
                .AsNoTracking()
                .Where(t => t.DeletedAt == null)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            var templateDtos = templates.Select(MapToDto);

            _log.Information("Successfully fetched {Count} report templates", templates.Count);
            return templateDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching all report templates");
            throw;
        }
    }

    /// <summary>
    /// Retrieves all active report templates
    /// </summary>
    /// <returns>A collection of active report template DTOs</returns>
    public async Task<IEnumerable<ReportTemplateDto>> GetActiveTemplatesAsync()
    {
        try
        {
            _log.Information("Fetching all active report templates");

            var templates = await _context.ReportTemplates
                .AsNoTracking()
                .Where(t => t.DeletedAt == null && t.IsActive)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            var templateDtos = templates.Select(MapToDto);

            _log.Information("Successfully fetched {Count} active report templates", templates.Count);
            return templateDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching active report templates");
            throw;
        }
    }

    /// <summary>
    /// Retrieves report templates filtered by type
    /// </summary>
    /// <param name="type">The report template type to filter by</param>
    /// <returns>A collection of report template DTOs matching the specified type</returns>
    public async Task<IEnumerable<ReportTemplateDto>> GetTemplatesByTypeAsync(ReportTemplateType type)
    {
        try
        {
            _log.Information("Fetching report templates with type: {TemplateType}", type);

            var templates = await _context.ReportTemplates
                .AsNoTracking()
                .Where(t => t.TemplateType == type && t.DeletedAt == null)
                .OrderByDescending(t => t.IsDefault)
                .ThenByDescending(t => t.CreatedAt)
                .ToListAsync();

            var templateDtos = templates.Select(MapToDto);

            _log.Information("Successfully fetched {Count} report templates with type: {TemplateType}", templates.Count, type);
            return templateDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching report templates with type: {TemplateType}", type);
            throw;
        }
    }

    /// <summary>
    /// Retrieves report templates filtered by reporting engine
    /// </summary>
    /// <param name="engine">The reporting engine name</param>
    /// <returns>A collection of report template DTOs matching the specified engine</returns>
    public async Task<IEnumerable<ReportTemplateDto>> GetTemplatesByEngineAsync(string engine)
    {
        try
        {
            _log.Information("Fetching report templates with engine: {Engine}", engine);

            var templates = await _context.ReportTemplates
                .AsNoTracking()
                .Where(t => t.ReportEngine == engine && t.DeletedAt == null)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            var templateDtos = templates.Select(MapToDto);

            _log.Information("Successfully fetched {Count} report templates with engine: {Engine}", templates.Count, engine);
            return templateDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching report templates with engine: {Engine}", engine);
            throw;
        }
    }

    /// <summary>
    /// Retrieves a specific report template by ID
    /// </summary>
    /// <param name="id">The unique identifier of the template</param>
    /// <returns>The report template DTO if found, otherwise null</returns>
    public async Task<ReportTemplateDto?> GetTemplateByIdAsync(int id)
    {
        try
        {
            _log.Information("Fetching report template with ID: {TemplateId}", id);

            var template = await _context.ReportTemplates
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id && t.DeletedAt == null);

            if (template == null)
            {
                _log.Warning("Report template with ID {TemplateId} not found", id);
                return null;
            }

            _log.Information("Successfully fetched report template with ID: {TemplateId}", id);
            return MapToDto(template);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching report template with ID: {TemplateId}", id);
            throw;
        }
    }

    /// <summary>
    /// Retrieves the default template for a specific type
    /// </summary>
    /// <param name="type">The report template type</param>
    /// <returns>The default report template DTO for the specified type, or null if none exists</returns>
    public async Task<ReportTemplateDto?> GetDefaultTemplateAsync(ReportTemplateType type)
    {
        try
        {
            _log.Information("Fetching default report template for type: {TemplateType}", type);

            var template = await _context.ReportTemplates
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.TemplateType == type && t.IsDefault && t.DeletedAt == null);

            if (template == null)
            {
                _log.Warning("No default report template found for type: {TemplateType}", type);
                return null;
            }

            _log.Information("Successfully fetched default report template for type: {TemplateType}", type);
            return MapToDto(template);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching default report template for type: {TemplateType}", type);
            throw;
        }
    }

    /// <summary>
    /// Creates a new report template
    /// </summary>
    /// <param name="createDto">The data transfer object containing template information and file</param>
    /// <returns>The created report template DTO</returns>
    public async Task<ReportTemplateDto> CreateTemplateAsync(CreateReportTemplateDto createDto)
    {
        try
        {
            _log.Information("Creating new report template: {TemplateName}", createDto.Name);

            // Validate file
            if (createDto.File == null || createDto.File.Length == 0)
            {
                _log.Warning("File is required for creating a report template");
                throw new InvalidOperationException("File is required");
            }

            var fileExtension = Path.GetExtension(createDto.File.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(fileExtension))
            {
                _log.Warning("Invalid file type: {FileExtension}. Allowed types: {AllowedExtensions}", fileExtension, string.Join(", ", AllowedExtensions));
                throw new InvalidOperationException($"Invalid file type. Allowed types: {string.Join(", ", AllowedExtensions)}");
            }

            if (createDto.File.Length > MaxFileSize)
            {
                _log.Warning("File size {FileSize} exceeds maximum allowed size of {MaxFileSize}", createDto.File.Length, MaxFileSize);
                throw new InvalidOperationException($"File size exceeds maximum allowed size of {MaxFileSize / 1024 / 1024}MB");
            }

            // Read file content
            byte[] fileContent;
            using (var memoryStream = new MemoryStream())
            {
                await createDto.File.CopyToAsync(memoryStream);
                fileContent = memoryStream.ToArray();
            }

            // If setting as default, unset any existing default for this type
            if (createDto.IsDefault)
            {
                await UnsetDefaultForTypeAsync(createDto.TemplateType);
            }

            var template = new ReportTemplate
            {
                Name = createDto.Name,
                Description = createDto.Description,
                TemplateType = createDto.TemplateType,
                ReportEngine = createDto.ReportEngine,
                FileContent = fileContent,
                FileName = createDto.File.FileName,
                FileSize = createDto.File.Length,
                MimeType = createDto.File.ContentType,
                IsActive = createDto.IsActive,
                IsDefault = createDto.IsDefault,
                DataSourceInfo = createDto.DataSourceInfo,
                Version = createDto.Version,
                Tags = createDto.Tags,
                DefaultExportFormat = createDto.DefaultExportFormat,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.ReportTemplates.Add(template);
            await _context.SaveChangesAsync();

            _log.Information("Successfully created report template with ID: {TemplateId}", template.Id);
            return MapToDto(template);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while creating report template: {TemplateName}", createDto.Name);
            throw;
        }
    }

    /// <summary>
    /// Updates an existing report template
    /// </summary>
    /// <param name="id">The unique identifier of the template to update</param>
    /// <param name="updateDto">The data transfer object containing updated template information</param>
    /// <returns>The updated report template DTO if found, otherwise null</returns>
    public async Task<ReportTemplateDto?> UpdateTemplateAsync(int id, UpdateReportTemplateDto updateDto)
    {
        try
        {
            _log.Information("Updating report template with ID: {TemplateId}", id);

            var template = await _context.ReportTemplates
                .FirstOrDefaultAsync(t => t.Id == id && t.DeletedAt == null);

            if (template == null)
            {
                _log.Warning("Report template with ID {TemplateId} not found", id);
                return null;
            }

            // If updating file
            if (updateDto.File != null && updateDto.File.Length > 0)
            {
                var fileExtension = Path.GetExtension(updateDto.File.FileName).ToLowerInvariant();
                if (!AllowedExtensions.Contains(fileExtension))
                {
                    _log.Warning("Invalid file type: {FileExtension}", fileExtension);
                    throw new InvalidOperationException($"Invalid file type. Allowed types: {string.Join(", ", AllowedExtensions)}");
                }

                if (updateDto.File.Length > MaxFileSize)
                {
                    _log.Warning("File size {FileSize} exceeds maximum allowed size", updateDto.File.Length);
                    throw new InvalidOperationException($"File size exceeds maximum allowed size of {MaxFileSize / 1024 / 1024}MB");
                }

                using (var memoryStream = new MemoryStream())
                {
                    await updateDto.File.CopyToAsync(memoryStream);
                    template.FileContent = memoryStream.ToArray();
                }

                template.FileName = updateDto.File.FileName;
                template.FileSize = updateDto.File.Length;
                template.MimeType = updateDto.File.ContentType;
            }

            // If setting as default, unset any existing default for this type
            if (updateDto.IsDefault && !template.IsDefault)
            {
                await UnsetDefaultForTypeAsync(updateDto.TemplateType);
            }

            template.Name = updateDto.Name;
            template.Description = updateDto.Description;
            template.TemplateType = updateDto.TemplateType;
            template.ReportEngine = updateDto.ReportEngine;
            template.IsActive = updateDto.IsActive;
            template.IsDefault = updateDto.IsDefault;
            template.DataSourceInfo = updateDto.DataSourceInfo;
            template.Version = updateDto.Version;
            template.Tags = updateDto.Tags;
            template.DefaultExportFormat = updateDto.DefaultExportFormat;
            template.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _log.Information("Successfully updated report template with ID: {TemplateId}", id);
            return MapToDto(template);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while updating report template with ID: {TemplateId}", id);
            throw;
        }
    }

    /// <summary>
    /// Sets a template as the default for its type
    /// </summary>
    /// <param name="id">The unique identifier of the template to set as default</param>
    /// <returns>True if successful, false if template not found</returns>
    public async Task<bool> SetDefaultTemplateAsync(int id)
    {
        try
        {
            _log.Information("Setting report template {TemplateId} as default", id);

            var template = await _context.ReportTemplates
                .FirstOrDefaultAsync(t => t.Id == id && t.DeletedAt == null);

            if (template == null)
            {
                _log.Warning("Report template with ID {TemplateId} not found", id);
                return false;
            }

            // Unset existing default for this type
            await UnsetDefaultForTypeAsync(template.TemplateType);

            template.IsDefault = true;
            template.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _log.Information("Successfully set report template {TemplateId} as default", id);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while setting report template {TemplateId} as default", id);
            throw;
        }
    }

    /// <summary>
    /// Toggles the active status of a report template
    /// </summary>
    /// <param name="id">The unique identifier of the template</param>
    /// <returns>True if successful, false if template not found</returns>
    public async Task<bool> ToggleActiveStatusAsync(int id)
    {
        try
        {
            _log.Information("Toggling active status for report template {TemplateId}", id);

            var template = await _context.ReportTemplates
                .FirstOrDefaultAsync(t => t.Id == id && t.DeletedAt == null);

            if (template == null)
            {
                _log.Warning("Report template with ID {TemplateId} not found", id);
                return false;
            }

            template.IsActive = !template.IsActive;
            template.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _log.Information("Successfully toggled active status for report template {TemplateId} to {IsActive}", id, template.IsActive);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while toggling active status for report template {TemplateId}", id);
            throw;
        }
    }

    /// <summary>
    /// Soft deletes a report template
    /// </summary>
    /// <param name="id">The unique identifier of the template to delete</param>
    /// <returns>True if successful, false if template not found</returns>
    public async Task<bool> SoftDeleteTemplateAsync(int id)
    {
        try
        {
            _log.Information("Soft deleting report template with ID: {TemplateId}", id);

            var template = await _context.ReportTemplates
                .FirstOrDefaultAsync(t => t.Id == id && t.DeletedAt == null);

            if (template == null)
            {
                _log.Warning("Report template with ID {TemplateId} not found", id);
                return false;
            }

            template.DeletedAt = DateTime.UtcNow;
            template.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _log.Information("Successfully soft deleted report template with ID: {TemplateId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while soft deleting report template with ID: {TemplateId}", id);
            throw;
        }
    }

    /// <summary>
    /// Downloads the file content of a specific template
    /// </summary>
    /// <param name="id">The unique identifier of the template</param>
    /// <returns>A tuple containing the file content, filename, and MIME type, or null if not found</returns>
    public async Task<(byte[] content, string fileName, string mimeType)?> DownloadTemplateAsync(int id)
    {
        try
        {
            _log.Information("Downloading report template with ID: {TemplateId}", id);

            var template = await _context.ReportTemplates
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id && t.DeletedAt == null);

            if (template == null)
            {
                _log.Warning("Report template with ID {TemplateId} not found", id);
                return null;
            }

            _log.Information("Successfully retrieved report template file for download: {FileName}", template.FileName);
            return (template.FileContent, template.FileName, template.MimeType);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while downloading report template with ID: {TemplateId}", id);
            throw;
        }
    }

    /// <summary>
    /// Searches for report templates by name or description
    /// </summary>
    /// <param name="searchTerm">The search term to match against name and description</param>
    /// <returns>A collection of matching report template DTOs</returns>
    public async Task<IEnumerable<ReportTemplateDto>> SearchTemplatesAsync(string searchTerm)
    {
        try
        {
            _log.Information("Searching report templates with term: {SearchTerm}", searchTerm);

            var templates = await _context.ReportTemplates
                .AsNoTracking()
                .Where(t => t.DeletedAt == null &&
                           (t.Name.Contains(searchTerm) ||
                            t.Description.Contains(searchTerm) ||
                            t.Tags.Contains(searchTerm)))
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            var templateDtos = templates.Select(MapToDto);

            _log.Information("Successfully found {Count} report templates matching search term", templates.Count);
            return templateDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while searching report templates with term: {SearchTerm}", searchTerm);
            throw;
        }
    }

    /// <summary>
    /// Unsets the default flag for all templates of a specific type
    /// </summary>
    /// <param name="type">The template type</param>
    private async Task UnsetDefaultForTypeAsync(ReportTemplateType type)
    {
        var existingDefaults = await _context.ReportTemplates
            .Where(t => t.TemplateType == type && t.IsDefault && t.DeletedAt == null)
            .ToListAsync();

        foreach (var template in existingDefaults)
        {
            template.IsDefault = false;
            template.UpdatedAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Maps a ReportTemplate entity to a ReportTemplateDto
    /// </summary>
    /// <param name="template">The entity to map</param>
    /// <returns>The mapped DTO</returns>
    private static ReportTemplateDto MapToDto(ReportTemplate template)
    {
        return new ReportTemplateDto
        {
            Id = template.Id,
            Name = template.Name,
            Description = template.Description,
            TemplateType = template.TemplateType,
            ReportEngine = template.ReportEngine,
            FileName = template.FileName,
            FileSize = template.FileSize,
            MimeType = template.MimeType,
            IsActive = template.IsActive,
            IsDefault = template.IsDefault,
            DataSourceInfo = template.DataSourceInfo,
            Version = template.Version,
            Tags = template.Tags,
            DefaultExportFormat = template.DefaultExportFormat,
            CreatedAt = template.CreatedAt,
            UpdatedAt = template.UpdatedAt
        };
    }
}

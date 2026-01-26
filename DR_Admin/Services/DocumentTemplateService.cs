using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.Data.Enums;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

/// <summary>
/// Service implementation for managing document templates
/// </summary>
public class DocumentTemplateService : IDocumentTemplateService
{
    private readonly ApplicationDbContext _context;
    private static readonly Serilog.ILogger _log = Log.ForContext<DocumentTemplateService>();
    private const long MaxFileSize = 10 * 1024 * 1024; // 10MB
    private static readonly string[] AllowedExtensions = { ".docx", ".pdf", ".html", ".txt", ".xlsx", ".htm", ".doc", ".xls" };

    public DocumentTemplateService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves all non-deleted document templates
    /// </summary>
    /// <returns>A collection of document template DTOs</returns>
    public async Task<IEnumerable<DocumentTemplateDto>> GetAllTemplatesAsync()
    {
        try
        {
            _log.Information("Fetching all document templates");

            var templates = await _context.DocumentTemplates
                .AsNoTracking()
                .Where(t => t.DeletedAt == null)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            var templateDtos = templates.Select(MapToDto);

            _log.Information("Successfully fetched {Count} document templates", templates.Count);
            return templateDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching all document templates");
            throw;
        }
    }

    /// <summary>
    /// Retrieves all active document templates
    /// </summary>
    /// <returns>A collection of active document template DTOs</returns>
    public async Task<IEnumerable<DocumentTemplateDto>> GetActiveTemplatesAsync()
    {
        try
        {
            _log.Information("Fetching all active document templates");

            var templates = await _context.DocumentTemplates
                .AsNoTracking()
                .Where(t => t.DeletedAt == null && t.IsActive)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            var templateDtos = templates.Select(MapToDto);

            _log.Information("Successfully fetched {Count} active document templates", templates.Count);
            return templateDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching active document templates");
            throw;
        }
    }

    /// <summary>
    /// Retrieves document templates filtered by type
    /// </summary>
    /// <param name="type">The template type to filter by</param>
    /// <returns>A collection of document template DTOs matching the specified type</returns>
    public async Task<IEnumerable<DocumentTemplateDto>> GetTemplatesByTypeAsync(DocumentTemplateType type)
    {
        try
        {
            _log.Information("Fetching document templates with type: {TemplateType}", type);

            var templates = await _context.DocumentTemplates
                .AsNoTracking()
                .Where(t => t.TemplateType == type && t.DeletedAt == null)
                .OrderByDescending(t => t.IsDefault)
                .ThenByDescending(t => t.CreatedAt)
                .ToListAsync();

            var templateDtos = templates.Select(MapToDto);

            _log.Information("Successfully fetched {Count} document templates with type: {TemplateType}", templates.Count, type);
            return templateDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching document templates with type: {TemplateType}", type);
            throw;
        }
    }

    /// <summary>
    /// Retrieves a specific document template by ID
    /// </summary>
    /// <param name="id">The unique identifier of the template</param>
    /// <returns>The document template DTO if found, otherwise null</returns>
    public async Task<DocumentTemplateDto?> GetTemplateByIdAsync(int id)
    {
        try
        {
            _log.Information("Fetching document template with ID: {TemplateId}", id);

            var template = await _context.DocumentTemplates
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id && t.DeletedAt == null);

            if (template == null)
            {
                _log.Warning("Document template with ID {TemplateId} not found", id);
                return null;
            }

            _log.Information("Successfully fetched document template with ID: {TemplateId}", id);
            return MapToDto(template);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching document template with ID: {TemplateId}", id);
            throw;
        }
    }

    /// <summary>
    /// Retrieves the default template for a specific type
    /// </summary>
    /// <param name="type">The template type</param>
    /// <returns>The default document template DTO for the specified type, or null if none exists</returns>
    public async Task<DocumentTemplateDto?> GetDefaultTemplateAsync(DocumentTemplateType type)
    {
        try
        {
            _log.Information("Fetching default document template for type: {TemplateType}", type);

            var template = await _context.DocumentTemplates
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.TemplateType == type && t.IsDefault && t.DeletedAt == null);

            if (template == null)
            {
                _log.Warning("No default document template found for type: {TemplateType}", type);
                return null;
            }

            _log.Information("Successfully fetched default document template for type: {TemplateType}", type);
            return MapToDto(template);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching default document template for type: {TemplateType}", type);
            throw;
        }
    }

    /// <summary>
    /// Creates a new document template
    /// </summary>
    /// <param name="createDto">The data transfer object containing template information and file</param>
    /// <returns>The created document template DTO</returns>
    public async Task<DocumentTemplateDto> CreateTemplateAsync(CreateDocumentTemplateDto createDto)
    {
        try
        {
            _log.Information("Creating new document template: {TemplateName}", createDto.Name);

            // Validate file
            if (createDto.File == null || createDto.File.Length == 0)
            {
                _log.Warning("File is required for creating a document template");
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

            // If setting as default, unset other defaults for this type
            if (createDto.IsDefault)
            {
                await UnsetDefaultTemplatesForTypeAsync(createDto.TemplateType);
            }

            var template = new DocumentTemplate
            {
                Name = createDto.Name,
                Description = createDto.Description,
                TemplateType = createDto.TemplateType,
                FileContent = fileContent,
                FileName = createDto.File.FileName,
                FileSize = createDto.File.Length,
                MimeType = createDto.File.ContentType,
                IsActive = createDto.IsActive,
                IsDefault = createDto.IsDefault,
                PlaceholderVariables = createDto.PlaceholderVariables
            };

            _context.DocumentTemplates.Add(template);
            await _context.SaveChangesAsync();

            _log.Information("Successfully created document template with ID: {TemplateId}", template.Id);
            return MapToDto(template);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while creating document template: {TemplateName}", createDto.Name);
            throw;
        }
    }

    /// <summary>
    /// Updates an existing document template
    /// </summary>
    /// <param name="id">The unique identifier of the template to update</param>
    /// <param name="updateDto">The data transfer object containing updated template information</param>
    /// <returns>The updated document template DTO if found, otherwise null</returns>
    public async Task<DocumentTemplateDto?> UpdateTemplateAsync(int id, UpdateDocumentTemplateDto updateDto)
    {
        try
        {
            _log.Information("Updating document template with ID: {TemplateId}", id);

            var template = await _context.DocumentTemplates.FindAsync(id);

            if (template == null || template.DeletedAt != null)
            {
                _log.Warning("Document template with ID {TemplateId} not found for update", id);
                return null;
            }

            // If updating file
            if (updateDto.File != null && updateDto.File.Length > 0)
            {
                var fileExtension = Path.GetExtension(updateDto.File.FileName).ToLowerInvariant();
                if (!AllowedExtensions.Contains(fileExtension))
                {
                    _log.Warning("Invalid file type: {FileExtension}. Allowed types: {AllowedExtensions}", fileExtension, string.Join(", ", AllowedExtensions));
                    throw new InvalidOperationException($"Invalid file type. Allowed types: {string.Join(", ", AllowedExtensions)}");
                }

                if (updateDto.File.Length > MaxFileSize)
                {
                    _log.Warning("File size {FileSize} exceeds maximum allowed size of {MaxFileSize}", updateDto.File.Length, MaxFileSize);
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

            // If setting as default, unset other defaults for this type
            if (updateDto.IsDefault && !template.IsDefault)
            {
                await UnsetDefaultTemplatesForTypeAsync(updateDto.TemplateType);
            }

            template.Name = updateDto.Name;
            template.Description = updateDto.Description;
            template.TemplateType = updateDto.TemplateType;
            template.IsActive = updateDto.IsActive;
            template.IsDefault = updateDto.IsDefault;
            template.PlaceholderVariables = updateDto.PlaceholderVariables;

            await _context.SaveChangesAsync();

            _log.Information("Successfully updated document template with ID: {TemplateId}", id);
            return MapToDto(template);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while updating document template with ID: {TemplateId}", id);
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
            _log.Information("Setting document template {TemplateId} as default", id);

            var template = await _context.DocumentTemplates.FindAsync(id);

            if (template == null || template.DeletedAt != null)
            {
                _log.Warning("Document template with ID {TemplateId} not found", id);
                return false;
            }

            // Unset other defaults for this type
            await UnsetDefaultTemplatesForTypeAsync(template.TemplateType);

            template.IsDefault = true;
            await _context.SaveChangesAsync();

            _log.Information("Successfully set document template {TemplateId} as default", id);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while setting document template {TemplateId} as default", id);
            throw;
        }
    }

    /// <summary>
    /// Soft deletes a document template
    /// </summary>
    /// <param name="id">The unique identifier of the template to delete</param>
    /// <returns>True if successful, false if template not found</returns>
    public async Task<bool> SoftDeleteTemplateAsync(int id)
    {
        try
        {
            _log.Information("Soft deleting document template with ID: {TemplateId}", id);

            var template = await _context.DocumentTemplates.FindAsync(id);

            if (template == null || template.DeletedAt != null)
            {
                _log.Warning("Document template with ID {TemplateId} not found for deletion", id);
                return false;
            }

            template.DeletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _log.Information("Successfully soft deleted document template with ID: {TemplateId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while deleting document template with ID: {TemplateId}", id);
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
            _log.Information("Downloading document template with ID: {TemplateId}", id);

            var template = await _context.DocumentTemplates
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id && t.DeletedAt == null);

            if (template == null)
            {
                _log.Warning("Document template with ID {TemplateId} not found for download", id);
                return null;
            }

            _log.Information("Successfully retrieved document template {TemplateId} for download", id);
            return (template.FileContent, template.FileName, template.MimeType);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while downloading document template with ID: {TemplateId}", id);
            throw;
        }
    }

    /// <summary>
    /// Unsets the default flag for all templates of a specific type
    /// </summary>
    /// <param name="type">The template type</param>
    private async Task UnsetDefaultTemplatesForTypeAsync(DocumentTemplateType type)
    {
        var defaultTemplates = await _context.DocumentTemplates
            .Where(t => t.TemplateType == type && t.IsDefault && t.DeletedAt == null)
            .ToListAsync();

        foreach (var template in defaultTemplates)
        {
            template.IsDefault = false;
        }
    }

    /// <summary>
    /// Maps a DocumentTemplate entity to a DocumentTemplateDto
    /// </summary>
    /// <param name="template">The template entity</param>
    /// <returns>The mapped DTO</returns>
    private static DocumentTemplateDto MapToDto(DocumentTemplate template)
    {
        return new DocumentTemplateDto
        {
            Id = template.Id,
            Name = template.Name,
            Description = template.Description,
            TemplateType = template.TemplateType,
            FileName = template.FileName,
            FileSize = template.FileSize,
            MimeType = template.MimeType,
            IsActive = template.IsActive,
            IsDefault = template.IsDefault,
            PlaceholderVariables = template.PlaceholderVariables,
            CreatedAt = template.CreatedAt,
            UpdatedAt = template.UpdatedAt,
            DeletedAt = template.DeletedAt
        };
    }
}

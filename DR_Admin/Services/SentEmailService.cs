using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

/// <summary>
/// Service implementation for managing sent email records
/// </summary>
public class SentEmailService : ISentEmailService
{
    private readonly ApplicationDbContext _context;
    private static readonly Serilog.ILogger _log = Log.ForContext<SentEmailService>();

    public SentEmailService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves all sent email records
    /// </summary>
    /// <returns>Collection of sent email DTOs</returns>
    public async Task<IEnumerable<SentEmailDto>> GetAllSentEmailsAsync()
    {
        try
        {
            _log.Information("Fetching all sent emails");
            
            var emails = await _context.SentEmails
                .AsNoTracking()
                .OrderByDescending(e => e.SentDate)
                .ToListAsync();

            var emailDtos = emails.Select(MapToDto);
            
            _log.Information("Successfully fetched {Count} sent emails", emails.Count);
            return emailDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching all sent emails");
            throw;
        }
    }

    /// <summary>
    /// Retrieves sent emails by customer ID
    /// </summary>
    /// <param name="customerId">The customer ID</param>
    /// <returns>Collection of sent email DTOs for the specified customer</returns>
    public async Task<IEnumerable<SentEmailDto>> GetSentEmailsByCustomerAsync(int customerId)
    {
        try
        {
            _log.Information("Fetching sent emails for customer ID: {CustomerId}", customerId);
            
            var emails = await _context.SentEmails
                .AsNoTracking()
                .Where(e => e.CustomerId == customerId)
                .OrderByDescending(e => e.SentDate)
                .ToListAsync();

            var emailDtos = emails.Select(MapToDto);
            
            _log.Information("Successfully fetched {Count} sent emails for customer ID: {CustomerId}", 
                emails.Count, customerId);
            return emailDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching sent emails for customer ID: {CustomerId}", customerId);
            throw;
        }
    }

    /// <summary>
    /// Retrieves sent emails by user ID
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <returns>Collection of sent email DTOs sent by the specified user</returns>
    public async Task<IEnumerable<SentEmailDto>> GetSentEmailsByUserAsync(int userId)
    {
        try
        {
            _log.Information("Fetching sent emails for user ID: {UserId}", userId);
            
            var emails = await _context.SentEmails
                .AsNoTracking()
                .Where(e => e.UserId == userId)
                .OrderByDescending(e => e.SentDate)
                .ToListAsync();

            var emailDtos = emails.Select(MapToDto);
            
            _log.Information("Successfully fetched {Count} sent emails for user ID: {UserId}", 
                emails.Count, userId);
            return emailDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching sent emails for user ID: {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Retrieves sent emails by related entity
    /// </summary>
    /// <param name="entityType">The entity type (e.g., Invoice, Order)</param>
    /// <param name="entityId">The entity ID</param>
    /// <returns>Collection of sent email DTOs for the specified entity</returns>
    public async Task<IEnumerable<SentEmailDto>> GetSentEmailsByRelatedEntityAsync(string entityType, int entityId)
    {
        try
        {
            _log.Information("Fetching sent emails for entity type: {EntityType}, entity ID: {EntityId}", 
                entityType, entityId);
            
            var emails = await _context.SentEmails
                .AsNoTracking()
                .Where(e => e.RelatedEntityType == entityType && e.RelatedEntityId == entityId)
                .OrderByDescending(e => e.SentDate)
                .ToListAsync();

            var emailDtos = emails.Select(MapToDto);
            
            _log.Information("Successfully fetched {Count} sent emails for entity type: {EntityType}, entity ID: {EntityId}", 
                emails.Count, entityType, entityId);
            return emailDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching sent emails for entity type: {EntityType}, entity ID: {EntityId}", 
                entityType, entityId);
            throw;
        }
    }

    /// <summary>
    /// Retrieves sent emails by date range
    /// </summary>
    /// <param name="startDate">The start date</param>
    /// <param name="endDate">The end date</param>
    /// <returns>Collection of sent email DTOs within the specified date range</returns>
    public async Task<IEnumerable<SentEmailDto>> GetSentEmailsByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        try
        {
            _log.Information("Fetching sent emails between {StartDate} and {EndDate}", startDate, endDate);
            
            var emails = await _context.SentEmails
                .AsNoTracking()
                .Where(e => e.SentDate >= startDate && e.SentDate <= endDate)
                .OrderByDescending(e => e.SentDate)
                .ToListAsync();

            var emailDtos = emails.Select(MapToDto);
            
            _log.Information("Successfully fetched {Count} sent emails between {StartDate} and {EndDate}", 
                emails.Count, startDate, endDate);
            return emailDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching sent emails between {StartDate} and {EndDate}", 
                startDate, endDate);
            throw;
        }
    }

    /// <summary>
    /// Retrieves sent emails by message ID
    /// </summary>
    /// <param name="messageId">The message ID</param>
    /// <returns>The sent email DTO, or null if not found</returns>
    public async Task<SentEmailDto?> GetSentEmailByMessageIdAsync(string messageId)
    {
        try
        {
            _log.Information("Fetching sent email with message ID: {MessageId}", messageId);
            
            var email = await _context.SentEmails
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.MessageId == messageId);

            if (email == null)
            {
                _log.Warning("Sent email with message ID: {MessageId} not found", messageId);
                return null;
            }

            _log.Information("Successfully fetched sent email with message ID: {MessageId}", messageId);
            return MapToDto(email);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching sent email with message ID: {MessageId}", messageId);
            throw;
        }
    }

    /// <summary>
    /// Retrieves a sent email by its ID
    /// </summary>
    /// <param name="id">The ID of the sent email record</param>
    /// <returns>The sent email DTO, or null if not found</returns>
    public async Task<SentEmailDto?> GetSentEmailByIdAsync(int id)
    {
        try
        {
            _log.Information("Fetching sent email with ID: {SentEmailId}", id);
            
            var email = await _context.SentEmails
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id);

            if (email == null)
            {
                _log.Warning("Sent email with ID: {SentEmailId} not found", id);
                return null;
            }

            _log.Information("Successfully fetched sent email with ID: {SentEmailId}", id);
            return MapToDto(email);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching sent email with ID: {SentEmailId}", id);
            throw;
        }
    }

    /// <summary>
    /// Creates a new sent email record
    /// </summary>
    /// <param name="dto">The sent email data to create</param>
    /// <returns>The created sent email DTO</returns>
    public async Task<SentEmailDto> CreateSentEmailAsync(CreateSentEmailDto dto)
    {
        try
        {
            _log.Information("Creating new sent email record for message ID: {MessageId}", dto.MessageId);

            var email = new SentEmail
            {
                SentDate = dto.SentDate,
                From = dto.From,
                To = dto.To,
                Cc = dto.Cc,
                Bcc = dto.Bcc,
                Subject = dto.Subject,
                BodyText = dto.BodyText,
                BodyHtml = dto.BodyHtml,
                MessageId = dto.MessageId,
                Status = dto.Status ?? EmailStatus.Pending,
                ErrorMessage = dto.ErrorMessage,
                RetryCount = dto.RetryCount,
                CustomerId = dto.CustomerId,
                UserId = dto.UserId,
                RelatedEntityType = dto.RelatedEntityType,
                RelatedEntityId = dto.RelatedEntityId,
                Attachments = dto.Attachments
            };

            _context.SentEmails.Add(email);
            await _context.SaveChangesAsync();

            _log.Information("Successfully created sent email record with ID: {SentEmailId}", email.Id);
            return MapToDto(email);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while creating sent email record for message ID: {MessageId}", dto.MessageId);
            throw;
        }
    }

    /// <summary>
    /// Updates an existing sent email record (typically for status updates)
    /// </summary>
    /// <param name="id">The ID of the sent email record to update</param>
    /// <param name="dto">The updated sent email data</param>
    /// <returns>The updated sent email DTO, or null if not found</returns>
    public async Task<SentEmailDto?> UpdateSentEmailAsync(int id, UpdateSentEmailDto dto)
    {
        try
        {
            _log.Information("Updating sent email record with ID: {SentEmailId}", id);
            
            var email = await _context.SentEmails.FindAsync(id);
            
            if (email == null)
            {
                _log.Warning("Sent email with ID: {SentEmailId} not found for update", id);
                return null;
            }

            email.Status = dto.Status;
            email.ErrorMessage = dto.ErrorMessage ?? email.ErrorMessage;
            email.RetryCount = dto.RetryCount;

            await _context.SaveChangesAsync();

            _log.Information("Successfully updated sent email record with ID: {SentEmailId}", id);
            return MapToDto(email);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while updating sent email record with ID: {SentEmailId}", id);
            throw;
        }
    }

    /// <summary>
    /// Deletes a sent email record
    /// </summary>
    /// <param name="id">The ID of the sent email record to delete</param>
    /// <returns>True if deleted successfully, false if not found</returns>
    public async Task<bool> DeleteSentEmailAsync(int id)
    {
        try
        {
            _log.Information("Deleting sent email record with ID: {SentEmailId}", id);
            
            var email = await _context.SentEmails.FindAsync(id);
            
            if (email == null)
            {
                _log.Warning("Sent email with ID: {SentEmailId} not found for deletion", id);
                return false;
            }

            _context.SentEmails.Remove(email);
            await _context.SaveChangesAsync();

            _log.Information("Successfully deleted sent email record with ID: {SentEmailId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while deleting sent email record with ID: {SentEmailId}", id);
            throw;
        }
    }

    private static SentEmailDto MapToDto(SentEmail email)
    {
        return new SentEmailDto
        {
            Id = email.Id,
            SentDate = email.SentDate,
            From = email.From,
            To = email.To,
            Cc = email.Cc,
            Bcc = email.Bcc,
            Subject = email.Subject,
            Body = email.BodyHtml ?? email.BodyText, // For backwards compatibility
            MessageId = email.MessageId,
            Status = email.Status,
            ErrorMessage = email.ErrorMessage,
            RetryCount = email.RetryCount,
            CustomerId = email.CustomerId,
            UserId = email.UserId,
            RelatedEntityType = email.RelatedEntityType,
            RelatedEntityId = email.RelatedEntityId,
            Attachments = email.Attachments,
            CreatedAt = email.CreatedAt,
            UpdatedAt = email.UpdatedAt
        };
    }
}

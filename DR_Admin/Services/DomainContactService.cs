using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

/// <summary>
/// Service implementation for managing domain contact persons
/// </summary>
public class DomainContactService : IDomainContactService
{
    private readonly ApplicationDbContext _context;
    private static readonly Serilog.ILogger _log = Log.ForContext<DomainContactService>();

    public DomainContactService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves all domain contacts in the system
    /// </summary>
    /// <returns>A collection of domain contact DTOs</returns>
    public async Task<IEnumerable<DomainContactDto>> GetAllDomainContactsAsync()
    {
        try
        {
            _log.Information("Fetching all domain contacts");

            var domainContacts = await _context.DomainContacts
                .AsNoTracking()
                .ToListAsync();

            var domainContactDtos = domainContacts.Select(MapToDto);

            _log.Information("Successfully fetched {Count} domain contacts", domainContacts.Count);
            return domainContactDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching all domain contacts");
            throw;
        }
    }

    /// <summary>
    /// Retrieves all domain contacts with pagination
    /// </summary>
    /// <param name="parameters">Pagination parameters</param>
    /// <returns>A paginated result of domain contact DTOs</returns>
    public async Task<PagedResult<DomainContactDto>> GetAllDomainContactsPagedAsync(PaginationParameters parameters)
    {
        try
        {
            _log.Information("Fetching paginated domain contacts - Page: {PageNumber}, PageSize: {PageSize}",
                parameters.PageNumber, parameters.PageSize);

            var totalCount = await _context.DomainContacts
                .AsNoTracking()
                .CountAsync();

            var domainContacts = await _context.DomainContacts
                .AsNoTracking()
                .OrderBy(dc => dc.LastName)
                .ThenBy(dc => dc.FirstName)
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            var domainContactDtos = domainContacts.Select(MapToDto).ToList();

            var result = new PagedResult<DomainContactDto>(
                domainContactDtos,
                totalCount,
                parameters.PageNumber,
                parameters.PageSize);

            _log.Information("Successfully fetched page {PageNumber} of domain contacts - Returned {Count} of {TotalCount} total",
                parameters.PageNumber, domainContactDtos.Count, totalCount);

            return result;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching paginated domain contacts");
            throw;
        }
    }

    /// <summary>
    /// Retrieves all domain contacts for a specific domain
    /// </summary>
    /// <param name="domainId">The domain ID</param>
    /// <returns>A collection of domain contact DTOs for the specified domain</returns>
    public async Task<IEnumerable<DomainContactDto>> GetDomainContactsByDomainIdAsync(int domainId)
    {
        try
        {
            _log.Information("Fetching domain contacts for domain ID: {DomainId}", domainId);

            var domainContacts = await _context.DomainContacts
                .AsNoTracking()
                .Where(dc => dc.DomainId == domainId)
                .ToListAsync();

            var domainContactDtos = domainContacts.Select(MapToDto);

            _log.Information("Successfully fetched {Count} domain contacts for domain ID: {DomainId}", domainContacts.Count, domainId);
            return domainContactDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching domain contacts for domain ID: {DomainId}", domainId);
            throw;
        }
    }

    /// <summary>
    /// Retrieves all domain contacts of a specific type for a domain
    /// </summary>
    /// <param name="domainId">The domain ID</param>
    /// <param name="contactType">The contact type (Registrant, Admin, Technical, Billing)</param>
    /// <returns>A collection of domain contact DTOs matching the criteria</returns>
    public async Task<IEnumerable<DomainContactDto>> GetDomainContactsByTypeAsync(int domainId, string contactType)
    {
        try
        {
            _log.Information("Fetching domain contacts for domain ID: {DomainId} with type: {ContactType}", domainId, contactType);

            var domainContacts = await _context.DomainContacts
                .AsNoTracking()
                .Where(dc => dc.DomainId == domainId && dc.ContactType == contactType)
                .ToListAsync();

            var domainContactDtos = domainContacts.Select(MapToDto);

            _log.Information("Successfully fetched {Count} domain contacts for domain ID: {DomainId} with type: {ContactType}",
                domainContacts.Count, domainId, contactType);
            return domainContactDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching domain contacts for domain ID: {DomainId} with type: {ContactType}", domainId, contactType);
            throw;
        }
    }

    /// <summary>
    /// Retrieves a specific domain contact by their unique identifier
    /// </summary>
    /// <param name="id">The domain contact ID</param>
    /// <returns>The domain contact DTO if found, otherwise null</returns>
    public async Task<DomainContactDto?> GetDomainContactByIdAsync(int id)
    {
        try
        {
            _log.Information("Fetching domain contact with ID: {DomainContactId}", id);

            var domainContact = await _context.DomainContacts
                .AsNoTracking()
                .FirstOrDefaultAsync(dc => dc.Id == id);

            if (domainContact == null)
            {
                _log.Warning("Domain contact with ID {DomainContactId} not found", id);
                return null;
            }

            _log.Information("Successfully fetched domain contact with ID: {DomainContactId}", id);
            return MapToDto(domainContact);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching domain contact with ID: {DomainContactId}", id);
            throw;
        }
    }

    /// <summary>
    /// Creates a new domain contact
    /// </summary>
    /// <param name="createDto">The domain contact creation data</param>
    /// <returns>The newly created domain contact DTO</returns>
    public async Task<DomainContactDto> CreateDomainContactAsync(CreateDomainContactDto createDto)
    {
        try
        {
            _log.Information("Creating new domain contact for domain ID: {DomainId}", createDto.DomainId);

            var domainContact = new DomainContact
            {
                ContactType = createDto.ContactType,
                FirstName = createDto.FirstName,
                LastName = createDto.LastName,
                Organization = createDto.Organization,
                Email = createDto.Email,
                Phone = createDto.Phone,
                Fax = createDto.Fax,
                Address1 = createDto.Address1,
                Address2 = createDto.Address2,
                City = createDto.City,
                State = createDto.State,
                PostalCode = createDto.PostalCode,
                CountryCode = createDto.CountryCode,
                IsActive = createDto.IsActive,
                Notes = createDto.Notes,
                DomainId = createDto.DomainId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.DomainContacts.Add(domainContact);
            await _context.SaveChangesAsync();

            _log.Information("Successfully created domain contact with ID: {DomainContactId}", domainContact.Id);
            return MapToDto(domainContact);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while creating domain contact for domain ID: {DomainId}", createDto.DomainId);
            throw;
        }
    }

    /// <summary>
    /// Updates an existing domain contact
    /// </summary>
    /// <param name="id">The domain contact ID</param>
    /// <param name="updateDto">The domain contact update data</param>
    /// <returns>The updated domain contact DTO if found, otherwise null</returns>
    public async Task<DomainContactDto?> UpdateDomainContactAsync(int id, UpdateDomainContactDto updateDto)
    {
        try
        {
            _log.Information("Updating domain contact with ID: {DomainContactId}", id);

            var domainContact = await _context.DomainContacts.FindAsync(id);

            if (domainContact == null)
            {
                _log.Warning("Domain contact with ID {DomainContactId} not found for update", id);
                return null;
            }

            domainContact.ContactType = updateDto.ContactType;
            domainContact.FirstName = updateDto.FirstName;
            domainContact.LastName = updateDto.LastName;
            domainContact.Organization = updateDto.Organization;
            domainContact.Email = updateDto.Email;
            domainContact.Phone = updateDto.Phone;
            domainContact.Fax = updateDto.Fax;
            domainContact.Address1 = updateDto.Address1;
            domainContact.Address2 = updateDto.Address2;
            domainContact.City = updateDto.City;
            domainContact.State = updateDto.State;
            domainContact.PostalCode = updateDto.PostalCode;
            domainContact.CountryCode = updateDto.CountryCode;
            domainContact.IsActive = updateDto.IsActive;
            domainContact.Notes = updateDto.Notes;
            domainContact.DomainId = updateDto.DomainId;
            domainContact.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _log.Information("Successfully updated domain contact with ID: {DomainContactId}", id);
            return MapToDto(domainContact);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while updating domain contact with ID: {DomainContactId}", id);
            throw;
        }
    }

    /// <summary>
    /// Deletes a domain contact
    /// </summary>
    /// <param name="id">The domain contact ID</param>
    /// <returns>True if the domain contact was deleted, otherwise false</returns>
    public async Task<bool> DeleteDomainContactAsync(int id)
    {
        try
        {
            _log.Information("Deleting domain contact with ID: {DomainContactId}", id);

            var domainContact = await _context.DomainContacts.FindAsync(id);

            if (domainContact == null)
            {
                _log.Warning("Domain contact with ID {DomainContactId} not found for deletion", id);
                return false;
            }

            _context.DomainContacts.Remove(domainContact);
            await _context.SaveChangesAsync();

            _log.Information("Successfully deleted domain contact with ID: {DomainContactId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while deleting domain contact with ID: {DomainContactId}", id);
            throw;
        }
    }

    /// <summary>
    /// Checks if a domain contact exists
    /// </summary>
    /// <param name="id">The domain contact ID</param>
    /// <returns>True if the domain contact exists, otherwise false</returns>
    public async Task<bool> DomainContactExistsAsync(int id)
    {
        try
        {
            _log.Information("Checking if domain contact exists with ID: {DomainContactId}", id);
            return await _context.DomainContacts.AnyAsync(dc => dc.Id == id);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while checking if domain contact exists with ID: {DomainContactId}", id);
            throw;
        }
    }

    /// <summary>
    /// Maps a DomainContact entity to a DomainContactDto
    /// </summary>
    /// <param name="domainContact">The domain contact entity</param>
    /// <returns>The domain contact DTO</returns>
    private static DomainContactDto MapToDto(DomainContact domainContact)
    {
        return new DomainContactDto
        {
            Id = domainContact.Id,
            ContactType = domainContact.ContactType,
            FirstName = domainContact.FirstName,
            LastName = domainContact.LastName,
            Organization = domainContact.Organization,
            Email = domainContact.Email,
            Phone = domainContact.Phone,
            Fax = domainContact.Fax,
            Address1 = domainContact.Address1,
            Address2 = domainContact.Address2,
            City = domainContact.City,
            State = domainContact.State,
            PostalCode = domainContact.PostalCode,
            CountryCode = domainContact.CountryCode,
            IsActive = domainContact.IsActive,
            Notes = domainContact.Notes,
            DomainId = domainContact.DomainId,
            CreatedAt = domainContact.CreatedAt,
            UpdatedAt = domainContact.UpdatedAt
        };
    }
}

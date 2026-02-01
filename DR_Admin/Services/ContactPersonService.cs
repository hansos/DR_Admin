using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

/// <summary>
/// Service implementation for managing contact persons associated with customers
/// </summary>
public class ContactPersonService : IContactPersonService
{
    private readonly ApplicationDbContext _context;
    private static readonly Serilog.ILogger _log = Log.ForContext<ContactPersonService>();

    public ContactPersonService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves all contact persons in the system
    /// </summary>
    /// <returns>A collection of contact person DTOs</returns>
    public async Task<IEnumerable<ContactPersonDto>> GetAllContactPersonsAsync()
    {
        try
        {
            _log.Information("Fetching all contact persons");
            
            var contactPersons = await _context.ContactPersons
                .AsNoTracking()
                .ToListAsync();

            var contactPersonDtos = contactPersons.Select(MapToDto);
            
            _log.Information("Successfully fetched {Count} contact persons", contactPersons.Count);
            return contactPersonDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching all contact persons");
            throw;
        }
    }

    public async Task<PagedResult<ContactPersonDto>> GetAllContactPersonsPagedAsync(PaginationParameters parameters)
    {
        try
        {
            _log.Information("Fetching paginated contact persons - Page: {PageNumber}, PageSize: {PageSize}", 
                parameters.PageNumber, parameters.PageSize);
            
            var totalCount = await _context.ContactPersons
                .AsNoTracking()
                .CountAsync();

            var contactPersons = await _context.ContactPersons
                .AsNoTracking()
                .OrderBy(cp => cp.LastName)
                .ThenBy(cp => cp.FirstName)
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            var contactPersonDtos = contactPersons.Select(MapToDto).ToList();
            
            var result = new PagedResult<ContactPersonDto>(
                contactPersonDtos, 
                totalCount, 
                parameters.PageNumber, 
                parameters.PageSize);

            _log.Information("Successfully fetched page {PageNumber} of contact persons - Returned {Count} of {TotalCount} total", 
                parameters.PageNumber, contactPersonDtos.Count, totalCount);
            
            return result;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching paginated contact persons");
            throw;
        }
    }

    /// <summary>
    /// Retrieves all contact persons for a specific customer
    /// </summary>
    /// <param name="customerId">The customer ID</param>
    /// <returns>A collection of contact person DTOs for the specified customer</returns>
    public async Task<IEnumerable<ContactPersonDto>> GetContactPersonsByCustomerIdAsync(int customerId)
    {
        try
        {
            _log.Information("Fetching contact persons for customer ID: {CustomerId}", customerId);
            
            var contactPersons = await _context.ContactPersons
                .AsNoTracking()
                .Where(cp => cp.CustomerId == customerId)
                .ToListAsync();

            var contactPersonDtos = contactPersons.Select(MapToDto);
            
            _log.Information("Successfully fetched {Count} contact persons for customer ID: {CustomerId}", contactPersons.Count, customerId);
            return contactPersonDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching contact persons for customer ID: {CustomerId}", customerId);
            throw;
        }
    }

    /// <summary>
    /// Retrieves a specific contact person by their unique identifier
    /// </summary>
    /// <param name="id">The contact person ID</param>
    /// <returns>The contact person DTO if found, otherwise null</returns>
    public async Task<ContactPersonDto?> GetContactPersonByIdAsync(int id)
    {
        try
        {
            _log.Information("Fetching contact person with ID: {ContactPersonId}", id);
            
            var contactPerson = await _context.ContactPersons
                .AsNoTracking()
                .FirstOrDefaultAsync(cp => cp.Id == id);

            if (contactPerson == null)
            {
                _log.Warning("Contact person with ID {ContactPersonId} not found", id);
                return null;
            }

            _log.Information("Successfully fetched contact person with ID: {ContactPersonId}", id);
            return MapToDto(contactPerson);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching contact person with ID: {ContactPersonId}", id);
            throw;
        }
    }

    /// <summary>
    /// Creates a new contact person
    /// </summary>
    /// <param name="createDto">The contact person creation data</param>
    /// <returns>The newly created contact person DTO</returns>
    public async Task<ContactPersonDto> CreateContactPersonAsync(CreateContactPersonDto createDto)
    {
        try
        {
            _log.Information("Creating new contact person for customer ID: {CustomerId}", createDto.CustomerId);

            var contactPerson = new ContactPerson
            {
                FirstName = createDto.FirstName,
                LastName = createDto.LastName,
                Email = createDto.Email,
                Phone = createDto.Phone,
                Position = createDto.Position,
                Department = createDto.Department,
                IsPrimary = createDto.IsPrimary,
                IsActive = createDto.IsActive,
                Notes = createDto.Notes,
                CustomerId = createDto.CustomerId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.ContactPersons.Add(contactPerson);
            await _context.SaveChangesAsync();

            _log.Information("Successfully created contact person with ID: {ContactPersonId}", contactPerson.Id);
            return MapToDto(contactPerson);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while creating contact person for customer ID: {CustomerId}", createDto.CustomerId);
            throw;
        }
    }

    /// <summary>
    /// Updates an existing contact person
    /// </summary>
    /// <param name="id">The contact person ID</param>
    /// <param name="updateDto">The contact person update data</param>
    /// <returns>The updated contact person DTO if found, otherwise null</returns>
    public async Task<ContactPersonDto?> UpdateContactPersonAsync(int id, UpdateContactPersonDto updateDto)
    {
        try
        {
            _log.Information("Updating contact person with ID: {ContactPersonId}", id);

            var contactPerson = await _context.ContactPersons.FindAsync(id);

            if (contactPerson == null)
            {
                _log.Warning("Contact person with ID {ContactPersonId} not found for update", id);
                return null;
            }

            contactPerson.FirstName = updateDto.FirstName;
            contactPerson.LastName = updateDto.LastName;
            contactPerson.Email = updateDto.Email;
            contactPerson.Phone = updateDto.Phone;
            contactPerson.Position = updateDto.Position;
            contactPerson.Department = updateDto.Department;
            contactPerson.IsPrimary = updateDto.IsPrimary;
            contactPerson.IsActive = updateDto.IsActive;
            contactPerson.Notes = updateDto.Notes;
            contactPerson.CustomerId = updateDto.CustomerId;
            contactPerson.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _log.Information("Successfully updated contact person with ID: {ContactPersonId}", id);
            return MapToDto(contactPerson);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while updating contact person with ID: {ContactPersonId}", id);
            throw;
        }
    }

    /// <summary>
    /// Deletes a contact person
    /// </summary>
    /// <param name="id">The contact person ID</param>
    /// <returns>True if the contact person was deleted, otherwise false</returns>
    public async Task<bool> DeleteContactPersonAsync(int id)
    {
        try
        {
            _log.Information("Deleting contact person with ID: {ContactPersonId}", id);

            var contactPerson = await _context.ContactPersons.FindAsync(id);

            if (contactPerson == null)
            {
                _log.Warning("Contact person with ID {ContactPersonId} not found for deletion", id);
                return false;
            }

            _context.ContactPersons.Remove(contactPerson);
            await _context.SaveChangesAsync();

            _log.Information("Successfully deleted contact person with ID: {ContactPersonId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while deleting contact person with ID: {ContactPersonId}", id);
            throw;
        }
    }

    /// <summary>
    /// Maps a ContactPerson entity to a ContactPersonDto
    /// </summary>
    /// <param name="contactPerson">The contact person entity</param>
    /// <returns>The mapped contact person DTO</returns>
    private static ContactPersonDto MapToDto(ContactPerson contactPerson)
    {
        return new ContactPersonDto
        {
            Id = contactPerson.Id,
            FirstName = contactPerson.FirstName,
            LastName = contactPerson.LastName,
            Email = contactPerson.Email,
            Phone = contactPerson.Phone,
            Position = contactPerson.Position,
            Department = contactPerson.Department,
            IsPrimary = contactPerson.IsPrimary,
            IsActive = contactPerson.IsActive,
            Notes = contactPerson.Notes,
            CustomerId = contactPerson.CustomerId,
            CreatedAt = contactPerson.CreatedAt,
            UpdatedAt = contactPerson.UpdatedAt
        };
    }
}

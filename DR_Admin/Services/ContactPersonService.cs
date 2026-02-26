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

    /// <summary>
    /// Retrieves all contact persons marked as domain global
    /// </summary>
    /// <returns>A collection of domain global contact person DTOs</returns>
    public async Task<IEnumerable<ContactPersonDto>> GetDomainGlobalContactPersonsAsync()
    {
        try
        {
            _log.Information("Fetching all domain global contact persons");

            var contactPersons = await _context.ContactPersons
                .AsNoTracking()
                .Where(cp => cp.IsDomainGlobal)
                .OrderBy(cp => cp.LastName)
                .ThenBy(cp => cp.FirstName)
                .ToListAsync();

            var contactPersonDtos = contactPersons.Select(MapToDto);

            _log.Information("Successfully fetched {Count} domain global contact persons", contactPersons.Count);
            return contactPersonDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching domain global contact persons");
            throw;
        }
    }

    /// <summary>
    /// Retrieves paginated contact persons with optional filtering by customer and search text.
    /// </summary>
    /// <param name="parameters">Paging parameters.</param>
    /// <param name="customerId">Optional customer ID filter.</param>
    /// <param name="search">Optional free-text search term for name, email, or phone.</param>
    /// <returns>A paged result containing matching contact persons.</returns>
    public async Task<PagedResult<ContactPersonDto>> GetAllContactPersonsPagedAsync(PaginationParameters parameters, int? customerId = null, string? search = null)
    {
        try
        {
            _log.Information("Fetching paginated contact persons - Page: {PageNumber}, PageSize: {PageSize}, CustomerId: {CustomerId}, Search: {Search}", 
                parameters.PageNumber, parameters.PageSize, customerId, search);

            var query = _context.ContactPersons
                .AsNoTracking()
                .AsQueryable();

            if (customerId.HasValue)
            {
                query = query.Where(cp => cp.CustomerId == customerId.Value);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchTerm = search.Trim().ToLower();
                query = query.Where(cp =>
                    cp.FirstName.ToLower().Contains(searchTerm) ||
                    cp.LastName.ToLower().Contains(searchTerm) ||
                    cp.Email.ToLower().Contains(searchTerm) ||
                    cp.Phone.ToLower().Contains(searchTerm));
            }

            var totalCount = await query.CountAsync();

            var contactPersons = await query
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
                IsDefaultOwner = createDto.IsDefaultOwner,
                IsDefaultBilling = createDto.IsDefaultBilling,
                IsDefaultTech = createDto.IsDefaultTech,
                IsDefaultAdministrator = createDto.IsDefaultAdministrator,
                IsDomainGlobal = false,
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
            contactPerson.IsDefaultOwner = updateDto.IsDefaultOwner;
            contactPerson.IsDefaultBilling = updateDto.IsDefaultBilling;
            contactPerson.IsDefaultTech = updateDto.IsDefaultTech;
            contactPerson.IsDefaultAdministrator = updateDto.IsDefaultAdministrator;
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
    /// Updates only the domain global flag for an existing contact person
    /// </summary>
    /// <param name="id">The contact person ID</param>
    /// <param name="isDomainGlobal">The new domain global value</param>
    /// <returns>The updated contact person DTO if found, otherwise null</returns>
    public async Task<ContactPersonDto?> UpdateContactPersonIsDomainGlobalAsync(int id, bool isDomainGlobal)
    {
        try
        {
            _log.Information("Updating IsDomainGlobal for contact person with ID: {ContactPersonId} to {IsDomainGlobal}", id, isDomainGlobal);

            var contactPerson = await _context.ContactPersons.FindAsync(id);

            if (contactPerson == null)
            {
                _log.Warning("Contact person with ID {ContactPersonId} not found for IsDomainGlobal update", id);
                return null;
            }

            contactPerson.IsDomainGlobal = isDomainGlobal;
            contactPerson.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _log.Information("Successfully updated IsDomainGlobal for contact person with ID: {ContactPersonId}", id);
            return MapToDto(contactPerson);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while updating IsDomainGlobal for contact person with ID: {ContactPersonId}", id);
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
    /// Retrieves contact persons for a specific customer categorized by role preference and usage
    /// Three-tiered sorting:
    /// 1. Preferred - marked with IsDefault[Role] flag
    /// 2. Frequently Used - used 3+ times for this role
    /// 3. Available - all other contact persons
    /// </summary>
    /// <param name="customerId">The customer ID</param>
    /// <param name="roleType">The contact role type to filter and sort by</param>
    /// <returns>Categorized list of contact persons sorted by preference</returns>
    public async Task<CategorizedContactPersonListResponse> GetContactPersonsForRoleAsync(int customerId, ContactRoleType roleType)
    {
        try
        {
            _log.Information("Fetching categorized contact persons for customer ID: {CustomerId}, role: {RoleType}", 
                customerId, roleType);

            // Get all active contact persons for the customer
            var contactPersons = await _context.ContactPersons
                .AsNoTracking()
                .Where(cp => cp.CustomerId == customerId && cp.IsActive)
                .ToListAsync();

            // Get IDs of contact persons for this customer
            var contactPersonIds = contactPersons.Select(cp => cp.Id).ToList();

            // Get usage counts for this role
            var usageCounts = await _context.DomainContactAssignments
                .AsNoTracking()
                .Where(a => contactPersonIds.Contains(a.ContactPersonId) && a.RoleType == roleType && a.IsActive)
                .GroupBy(a => a.ContactPersonId)
                .Select(g => new { ContactPersonId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.ContactPersonId, x => x.Count);

            const int frequentUsageThreshold = 3;

            // Categorize and map contact persons
            var categorizedList = contactPersons.Select(cp =>
            {
                var usageCount = usageCounts.GetValueOrDefault(cp.Id, 0);
                var category = DetermineCategory(cp, roleType, usageCount, frequentUsageThreshold);

                return new CategorizedContactPersonDto
                {
                    Id = cp.Id,
                    FirstName = cp.FirstName,
                    LastName = cp.LastName,
                    FullName = $"{cp.FirstName} {cp.LastName}".Trim(),
                    Email = cp.Email,
                    Phone = cp.Phone,
                    Position = cp.Position,
                    Department = cp.Department,
                    IsPrimary = cp.IsPrimary,
                    IsActive = cp.IsActive,
                    Category = category,
                    UsageCount = usageCount,
                    CustomerId = cp.CustomerId
                };
            })
            .OrderBy(cp => cp.Category)
            .ThenByDescending(cp => cp.UsageCount)
            .ThenBy(cp => cp.LastName)
            .ThenBy(cp => cp.FirstName)
            .ToList();

            _log.Information("Successfully fetched {Count} categorized contact persons for customer ID: {CustomerId}, role: {RoleType}. " +
                           "Preferred: {Preferred}, Frequently Used: {FrequentlyUsed}, Available: {Available}",
                categorizedList.Count, customerId, roleType,
                categorizedList.Count(cp => cp.Category == ContactPersonCategory.Preferred),
                categorizedList.Count(cp => cp.Category == ContactPersonCategory.FrequentlyUsed),
                categorizedList.Count(cp => cp.Category == ContactPersonCategory.Available));

            return new CategorizedContactPersonListResponse
            {
                ContactPersons = categorizedList
            };
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching categorized contact persons for customer ID: {CustomerId}, role: {RoleType}", 
                customerId, roleType);
            throw;
        }
    }

    /// <summary>
    /// Determines the category for a contact person based on default flags and usage
    /// </summary>
    private static ContactPersonCategory DetermineCategory(ContactPerson contactPerson, ContactRoleType roleType, 
        int usageCount, int frequentUsageThreshold)
    {
        // Check if marked as default for this role
        var isPreferred = roleType switch
        {
            ContactRoleType.Registrant => contactPerson.IsDefaultOwner,
            ContactRoleType.Administrative => contactPerson.IsDefaultAdministrator,
            ContactRoleType.Technical => contactPerson.IsDefaultTech,
            ContactRoleType.Billing => contactPerson.IsDefaultBilling,
            _ => false
        };

        if (isPreferred)
        {
            return ContactPersonCategory.Preferred;
        }

        if (usageCount >= frequentUsageThreshold)
        {
            return ContactPersonCategory.FrequentlyUsed;
        }

        return ContactPersonCategory.Available;
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
            IsDefaultOwner = contactPerson.IsDefaultOwner,
            IsDefaultBilling = contactPerson.IsDefaultBilling,
            IsDefaultTech = contactPerson.IsDefaultTech,
            IsDefaultAdministrator = contactPerson.IsDefaultAdministrator,
            IsDomainGlobal = contactPerson.IsDomainGlobal,
            CreatedAt = contactPerson.CreatedAt,
            UpdatedAt = contactPerson.UpdatedAt
        };
    }
}

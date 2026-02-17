using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

/// <summary>
/// Service for managing customer operations
/// </summary>
public class CustomerService : ICustomerService
{
    private readonly ApplicationDbContext _context;
    private static readonly Serilog.ILogger _log = Log.ForContext<CustomerService>();

    public CustomerService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves all customers from the database
    /// </summary>
    /// <returns>A collection of all customers</returns>
    public async Task<IEnumerable<CustomerDto>> GetAllCustomersAsync()
    {
        try
        {
            _log.Information("Fetching all customers");
            
            var customers = await _context.Customers
                .AsNoTracking()
                .ToListAsync();

            var customerDtos = customers.Select(MapToDto);
            
            _log.Information("Successfully fetched {Count} customers", customers.Count);
            return customerDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching all customers");
            throw;
        }
    }

    /// <summary>
    /// Retrieves a paginated list of customers
    /// </summary>
    /// <param name="parameters">Pagination parameters including page number and page size</param>
    /// <returns>A paged result containing customers and pagination metadata</returns>
    public async Task<PagedResult<CustomerDto>> GetAllCustomersPagedAsync(PaginationParameters parameters)
    {
        try
        {
            _log.Information("Fetching paginated customers - Page: {PageNumber}, PageSize: {PageSize}", 
                parameters.PageNumber, parameters.PageSize);
            
            var totalCount = await _context.Customers
                .AsNoTracking()
                .CountAsync();

            var customers = await _context.Customers
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            var customerDtos = customers.Select(MapToDto).ToList();
            
            var result = new PagedResult<CustomerDto>(
                customerDtos, 
                totalCount, 
                parameters.PageNumber, 
                parameters.PageSize);

            _log.Information("Successfully fetched page {PageNumber} of customers - Returned {Count} of {TotalCount} total", 
                parameters.PageNumber, customerDtos.Count, totalCount);
            
            return result;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching paginated customers");
            throw;
        }
    }

    /// <summary>
    /// Retrieves a customer by their unique identifier
    /// </summary>
    /// <param name="id">The customer's unique identifier</param>
    /// <returns>The customer if found; otherwise, null</returns>
    public async Task<CustomerDto?> GetCustomerByIdAsync(int id)
    {
        try
        {
            _log.Information("Fetching customer with ID: {CustomerId}", id);
            
            var customer = await _context.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (customer == null)
            {
                _log.Warning("Customer with ID {CustomerId} not found", id);
                return null;
            }

            _log.Information("Successfully fetched customer with ID: {CustomerId}", id);
            return MapToDto(customer);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching customer with ID: {CustomerId}", id);
            throw;
        }
    }

    /// <summary>
    /// Retrieves a customer by their email address
    /// </summary>
    /// <param name="email">The email address to search for</param>
    /// <param name="includeContacts"></param>
    /// <returns>The customer if found; otherwise, null</returns>
    public async Task<CustomerDto?> GetCustomerByEmailAsync(string email, bool? includeContacts = false)
    {
        try
        {
            _log.Information("Fetching customer with email: {Email}", email);
            
            var customer = await _context.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Email == email || c.BillingEmail == email);

            if (customer == null)
            {
                // If includeContacts is set to true, we might want to check if any contact person has the email as well
                if (includeContacts == true)
                {
                    var contact = await _context.ContactPersons
                        .AsNoTracking()
                        .FirstOrDefaultAsync(cp => cp.Email == email);

                    if (contact != null)
                    {
                        customer = await _context.Customers
                            .AsNoTracking()
                            .FirstOrDefaultAsync(c => c.Id == contact.CustomerId);
                    }
                }

                if (customer == null)
                {
                    _log.Warning("Customer with email {Email} not found", email);
                    return null;
                }
            }

            _log.Information("Successfully fetched customer with email: {Email}", email);
            return MapToDto(customer);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while fetching customer with email: {Email}", email);
            throw;
        }
    }

    /// <summary>
    /// Checks if an email address exists in the customer database
    /// </summary>
    /// <param name="email">The email address to check</param>
    /// <returns>True if the email exists; otherwise, false</returns>
    public async Task<bool> CheckEmailExistsAsync(string email)
    {
        try
        {
            _log.Information("Checking if email exists: {Email}", email);
            
            var exists = await _context.Customers
                .AsNoTracking()
                .AnyAsync(c => c.Email == email || c.BillingEmail == email);

            _log.Information("Email {Email} exists: {Exists}", email, exists);
            return exists;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while checking if email exists: {Email}", email);
            throw;
        }
    }

    /// <summary>
    /// Creates a new customer
    /// </summary>
    /// <param name="createDto">The customer data for creation</param>
    /// <returns>The newly created customer</returns>
    public async Task<CustomerDto> CreateCustomerAsync(CreateCustomerDto createDto)
    {
        try
        {
            _log.Information("Creating new customer with email: {Email}", createDto.Email);

            var customer = new Customer
            {
                Name = createDto.Name,
                Email = createDto.Email,
                Phone = createDto.Phone,
                // Address data moved to CustomerAddress - not stored on Customer entity
                CustomerName = createDto.CustomerName,
                TaxId = createDto.TaxId,
                VatNumber = createDto.VatNumber,
                IsCompany = createDto.IsCompany,
                IsActive = createDto.IsActive,
                Status = createDto.Status,
                Balance = 0,
                CreditLimit = createDto.CreditLimit,
                Notes = createDto.Notes,
                BillingEmail = createDto.BillingEmail,
                PreferredPaymentMethod = createDto.PreferredPaymentMethod,
                PreferredCurrency = createDto.PreferredCurrency,
                AllowCurrencyOverride = createDto.AllowCurrencyOverride,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            _log.Information("Successfully created customer with ID: {CustomerId}", customer.Id);
            return MapToDto(customer);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while creating customer with email: {Email}", createDto.Email);
            throw;
        }
    }

    /// <summary>
    /// Updates an existing customer
    /// </summary>
    /// <param name="id">The customer's unique identifier</param>
    /// <param name="updateDto">The updated customer data</param>
    /// <returns>The updated customer if found; otherwise, null</returns>
    public async Task<CustomerDto?> UpdateCustomerAsync(int id, UpdateCustomerDto updateDto)
    {
        try
        {
            _log.Information("Updating customer with ID: {CustomerId}", id);

            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                _log.Warning("Customer with ID {CustomerId} not found for update", id);
                return null;
            }

            customer.Name = updateDto.Name;
            customer.Email = updateDto.Email;
            customer.Phone = updateDto.Phone;
            // Address fields removed: managed via CustomerAddress entities
            customer.CustomerName = updateDto.CustomerName;
            customer.TaxId = updateDto.TaxId;
            customer.VatNumber = updateDto.VatNumber;
            customer.IsCompany = updateDto.IsCompany;
            customer.IsActive = updateDto.IsActive;
            customer.Status = updateDto.Status;
            customer.CreditLimit = updateDto.CreditLimit;
            customer.Notes = updateDto.Notes;
            customer.BillingEmail = updateDto.BillingEmail;
            customer.PreferredPaymentMethod = updateDto.PreferredPaymentMethod;
            customer.PreferredCurrency = updateDto.PreferredCurrency;
            customer.AllowCurrencyOverride = updateDto.AllowCurrencyOverride;
            customer.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _log.Information("Successfully updated customer with ID: {CustomerId}", id);
            return MapToDto(customer);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while updating customer with ID: {CustomerId}", id);
            throw;
        }
    }

    /// <summary>
    /// Deletes a customer from the database
    /// </summary>
    /// <param name="id">The customer's unique identifier</param>
    /// <returns>True if the customer was deleted; otherwise, false</returns>
    public async Task<bool> DeleteCustomerAsync(int id)
    {
        try
        {
            _log.Information("Deleting customer with ID: {CustomerId}", id);

            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                _log.Warning("Customer with ID {CustomerId} not found for deletion", id);
                return false;
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            _log.Information("Successfully deleted customer with ID: {CustomerId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while deleting customer with ID: {CustomerId}", id);
            throw;
        }
    }

    private static CustomerDto MapToDto(Customer customer)
    {
            return new CustomerDto
            {
                Id = customer.Id,
                Name = customer.Name,
                Email = customer.Email,
                Phone = customer.Phone,
                // Address fields moved to CustomerAddress; not present on DTO
                CustomerName = customer.CustomerName,
                TaxId = customer.TaxId,
                VatNumber = customer.VatNumber,
                // ContactPerson removed; use ContactPerson entities instead
                IsCompany = customer.IsCompany,
                IsActive = customer.IsActive,
                Status = customer.Status,
                Balance = customer.Balance,
                CreditLimit = customer.CreditLimit,
                Notes = customer.Notes,
                BillingEmail = customer.BillingEmail,
                PreferredPaymentMethod = customer.PreferredPaymentMethod,
                PreferredCurrency = customer.PreferredCurrency,
                AllowCurrencyOverride = customer.AllowCurrencyOverride,
                CreatedAt = customer.CreatedAt,
                UpdatedAt = customer.UpdatedAt
            };
    }
}

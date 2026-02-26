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

            var customerDtos = await MapToDtosAsync(customers);

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

            var customerDtos = await MapToDtosAsync(customers);
            
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
            return await MapToDtoAsync(customer);
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
            return await MapToDtoAsync(customer);
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

            var nextRefNumber = await GetNextReferenceNumberAsync();

            var customer = new Customer
            {
                ReferenceNumber = nextRefNumber,
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

            _log.Information("Successfully created customer with ID: {CustomerId}, ReferenceNumber: {ReferenceNumber}", customer.Id, customer.ReferenceNumber);
            return await MapToDtoAsync(customer);
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
            return await MapToDtoAsync(customer);
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

    private static CustomerDto MapToDto(Customer customer, string? customerNumberPrefix = null, string? referenceNumberPrefix = null)
    {
            return new CustomerDto
            {
                Id = customer.Id,
                ReferenceNumber = customer.ReferenceNumber,
                FormattedReferenceNumber = FormatReferenceNumber(customer.ReferenceNumber, referenceNumberPrefix),
                CustomerNumber = customer.CustomerNumber,
                FormattedCustomerNumber = FormatCustomerNumber(customer.CustomerNumber, customerNumberPrefix),
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

    /// <summary>
    /// Maps a Customer entity to a CustomerDto, resolving prefixes from the database
    /// </summary>
    private async Task<CustomerDto> MapToDtoAsync(Customer customer)
    {
        var custPrefix = await GetCustomerNumberPrefixAsync();
        var refPrefix = await GetReferenceNumberPrefixAsync();
        return MapToDto(customer, custPrefix, refPrefix);
    }

    /// <summary>
    /// Maps a collection of Customer entities to CustomerDtos, resolving prefixes once
    /// </summary>
    private async Task<List<CustomerDto>> MapToDtosAsync(IEnumerable<Customer> customers)
    {
        var custPrefix = await GetCustomerNumberPrefixAsync();
        var refPrefix = await GetReferenceNumberPrefixAsync();
        return customers.Select(c => MapToDto(c, custPrefix, refPrefix)).ToList();
    }

    /// <summary>
    /// Formats a customer number with the optional prefix (e.g., "CUST-1001")
    /// </summary>
    private static string? FormatCustomerNumber(long? customerNumber, string? prefix)
    {
        if (customerNumber == null)
            return null;

        return string.IsNullOrEmpty(prefix)
            ? customerNumber.Value.ToString()
            : $"{prefix}{customerNumber.Value}";
    }

    /// <summary>
    /// Formats a reference number with the optional prefix (e.g., "REF-1001")
    /// </summary>
    private static string FormatReferenceNumber(long referenceNumber, string? prefix)
    {
        return string.IsNullOrEmpty(prefix)
            ? referenceNumber.ToString()
            : $"{prefix}{referenceNumber}";
    }

    /// <summary>
    /// Gets the next reference number from SystemSettings (key "PNR") and increments it atomically.
    /// If the setting does not exist, it is created with a default starting value of 1.
    /// </summary>
    private async Task<long> GetNextReferenceNumberAsync()
    {
        const string key = "PNR";
        const long defaultStartValue = 1001;

        var setting = await _context.SystemSettings
            .FirstOrDefaultAsync(s => s.Key == key);

        if (setting == null)
        {
            setting = new Data.Entities.SystemSetting
            {
                Key = key,
                Value = (defaultStartValue + 1).ToString(),
                Description = "The next customer reference number (PNR) to assign. Auto-incremented on each new customer creation.",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _context.SystemSettings.Add(setting);
            await _context.SaveChangesAsync();

            _log.Information("Initialized {Key} system setting with starting value {Value}", key, defaultStartValue);
            return defaultStartValue;
        }

        if (!long.TryParse(setting.Value, out var currentValue))
        {
            _log.Warning("Invalid {Key} value '{Value}', resetting to default {Default}", key, setting.Value, defaultStartValue);
            currentValue = defaultStartValue;
        }

        setting.Value = (currentValue + 1).ToString();
        setting.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return currentValue;
    }

    /// <summary>
    /// Gets the optional reference number prefix from SystemSettings (key "RSX")
    /// </summary>
    private async Task<string?> GetReferenceNumberPrefixAsync()
    {
        const string key = "RSX";

        var setting = await _context.SystemSettings
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Key == key);

        return setting?.Value;
    }

    /// <summary>
    /// Gets the next customer number from SystemSettings (key "CNR") and increments it atomically.
    /// If the setting does not exist, it is created with a default starting value of 1.
    /// </summary>
    private async Task<long> GetNextCustomerNumberAsync()
    {
        const string key = "CNR";
        const long defaultStartValue = 1001;

        var setting = await _context.SystemSettings
            .FirstOrDefaultAsync(s => s.Key == key);

        if (setting == null)
        {
            setting = new Data.Entities.SystemSetting
            {
                Key = key,
                Value = (defaultStartValue + 1).ToString(),
                Description = "The next customer number (CNR) to assign. Auto-incremented on each customer's first sale.",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _context.SystemSettings.Add(setting);
            await _context.SaveChangesAsync();

            _log.Information("Initialized {Key} system setting with starting value {Value}", key, defaultStartValue);
            return defaultStartValue;
        }

        if (!long.TryParse(setting.Value, out var currentValue))
        {
            _log.Warning("Invalid {Key} value '{Value}', resetting to default {Default}", key, setting.Value, defaultStartValue);
            currentValue = defaultStartValue;
        }

        setting.Value = (currentValue + 1).ToString();
        setting.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return currentValue;
    }

    /// <summary>
    /// Gets the optional customer number prefix from SystemSettings (key "CSX")
    /// </summary>
    private async Task<string?> GetCustomerNumberPrefixAsync()
    {
        const string key = "CSX";

        var setting = await _context.SystemSettings
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Key == key);

        return setting?.Value;
    }

    /// <inheritdoc />
    public async Task<List<CustomerDto>> SearchCustomersAsync(string query)
    {
        try
        {
            _log.Information("Searching customers with query: {Query}", query);

            var normalizedQuery = query.Trim().ToUpperInvariant();

            // Find customer IDs matched via contact persons
            var contactCustomerIds = await _context.ContactPersons
                .AsNoTracking()
                .Where(cp => cp.CustomerId != null &&
                    (cp.NormalizedFirstName.Contains(normalizedQuery) ||
                     cp.NormalizedLastName.Contains(normalizedQuery) ||
                     cp.Email.Contains(query) ||
                     cp.Phone.Contains(query)))
                .Select(cp => cp.CustomerId!.Value)
                .Distinct()
                .ToListAsync();

            var customers = await _context.Customers
                .AsNoTracking()
                .Where(c =>
                    c.NormalizedName.Contains(normalizedQuery) ||
                    (c.NormalizedCustomerName != null && c.NormalizedCustomerName.Contains(normalizedQuery)) ||
                    c.Email.Contains(query) ||
                    (c.BillingEmail != null && c.BillingEmail.Contains(query)) ||
                    c.Phone.Contains(query) ||
                    c.ReferenceNumber.ToString().Contains(query) ||
                    (c.CustomerNumber != null && c.CustomerNumber.ToString()!.Contains(query)) ||
                    contactCustomerIds.Contains(c.Id))
                .OrderBy(c => c.Name)
                .Take(50)
                .ToListAsync();

            var customerDtos = await MapToDtosAsync(customers);

            _log.Information("Customer search for '{Query}' returned {Count} results", query, customerDtos.Count);
            return customerDtos;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occurred while searching customers with query: {Query}", query);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<long> EnsureCustomerNumberAsync(int customerId)
    {
        var customer = await _context.Customers.FindAsync(customerId);
        if (customer == null)
            throw new InvalidOperationException($"Customer with ID {customerId} not found");

        if (customer.CustomerNumber.HasValue)
        {
            _log.Information("Customer {CustomerId} already has CustomerNumber {CustomerNumber}", customerId, customer.CustomerNumber.Value);
            return customer.CustomerNumber.Value;
        }

        var nextNumber = await GetNextCustomerNumberAsync();
        customer.CustomerNumber = nextNumber;
        customer.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        _log.Information("Assigned CustomerNumber {CustomerNumber} to customer {CustomerId} on first sale", nextNumber, customerId);
        return nextNumber;
    }
}

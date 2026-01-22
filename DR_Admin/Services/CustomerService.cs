using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

public class CustomerService : ICustomerService
{
    private readonly ApplicationDbContext _context;
    private static readonly Serilog.ILogger _log = Log.ForContext<CustomerService>();

    public CustomerService(ApplicationDbContext context)
    {
        _context = context;
    }

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
                Address = createDto.Address,
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
            customer.Address = updateDto.Address;
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
            Address = customer.Address,
            CreatedAt = customer.CreatedAt,
            UpdatedAt = customer.UpdatedAt
        };
    }
}

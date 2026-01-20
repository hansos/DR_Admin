using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.DTOs;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ISPAdmin.Services;

public class CustomerService : ICustomerService
{
    private readonly ApplicationDbContext _context;
    private readonly Serilog.ILogger _logger;

    public CustomerService(ApplicationDbContext context, Serilog.ILogger logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<CustomerDto>> GetAllCustomersAsync()
    {
        try
        {
            _logger.Information("Fetching all customers");
            
            var customers = await _context.Customers
                .AsNoTracking()
                .ToListAsync();

            var customerDtos = customers.Select(MapToDto);
            
            _logger.Information("Successfully fetched {Count} customers", customers.Count);
            return customerDtos;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error occurred while fetching all customers");
            throw;
        }
    }

    public async Task<CustomerDto?> GetCustomerByIdAsync(int id)
    {
        try
        {
            _logger.Information("Fetching customer with ID: {CustomerId}", id);
            
            var customer = await _context.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (customer == null)
            {
                _logger.Warning("Customer with ID {CustomerId} not found", id);
                return null;
            }

            _logger.Information("Successfully fetched customer with ID: {CustomerId}", id);
            return MapToDto(customer);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error occurred while fetching customer with ID: {CustomerId}", id);
            throw;
        }
    }

    public async Task<CustomerDto> CreateCustomerAsync(CreateCustomerDto createDto)
    {
        try
        {
            _logger.Information("Creating new customer with email: {Email}", createDto.Email);

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

            _logger.Information("Successfully created customer with ID: {CustomerId}", customer.Id);
            return MapToDto(customer);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error occurred while creating customer with email: {Email}", createDto.Email);
            throw;
        }
    }

    public async Task<CustomerDto?> UpdateCustomerAsync(int id, UpdateCustomerDto updateDto)
    {
        try
        {
            _logger.Information("Updating customer with ID: {CustomerId}", id);

            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                _logger.Warning("Customer with ID {CustomerId} not found for update", id);
                return null;
            }

            customer.Name = updateDto.Name;
            customer.Email = updateDto.Email;
            customer.Phone = updateDto.Phone;
            customer.Address = updateDto.Address;
            customer.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.Information("Successfully updated customer with ID: {CustomerId}", id);
            return MapToDto(customer);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error occurred while updating customer with ID: {CustomerId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteCustomerAsync(int id)
    {
        try
        {
            _logger.Information("Deleting customer with ID: {CustomerId}", id);

            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                _logger.Warning("Customer with ID {CustomerId} not found for deletion", id);
                return false;
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            _logger.Information("Successfully deleted customer with ID: {CustomerId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error occurred while deleting customer with ID: {CustomerId}", id);
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
